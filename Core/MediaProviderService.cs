using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VRCGalleryManager.Core.Data;

namespace VRCGalleryManager.Core
{
    public class MediaProviderService : IMediaProviderService
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private readonly HttpClientHandler _handler;
        private bool _isWarmedUp = false;
        private readonly SemaphoreSlim _warmupLock = new(1, 1);
        private readonly ConcurrentDictionary<string, (DateTime Expiration, MediaSearchResult Result)> _cache = new(StringComparer.OrdinalIgnoreCase);

        public MediaProviderService()
        {
            _cookieContainer = new CookieContainer();
            try
            {
                _cookieContainer.Add(new Cookie("over18", "1", "/", ".reddit.com"));
            }
            catch { }

            _handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(_handler)
            {
                Timeout = TimeSpan.FromSeconds(25)
            };

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 VRCGalleryManager/1.0");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");

            // Purge old temporary files in the background on startup
            _ = Task.Run(CleanOldTempFiles);
        }

        private static void CleanOldTempFiles()
        {
            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                if (!Directory.Exists(tempDir)) return;
                var cutoff = DateTime.UtcNow.AddHours(-24);
                foreach (var file in Directory.EnumerateFiles(tempDir))
                {
                    try
                    {
                        if (File.GetCreationTimeUtc(file) < cutoff)
                        {
                            File.Delete(file);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void PruneCacheIfFull()
        {
            if (_cache.Count > 250)
            {
                var now = DateTime.UtcNow;
                foreach (var kvp in _cache)
                {
                    if (kvp.Value.Expiration <= now)
                    {
                        _cache.TryRemove(kvp.Key, out _);
                    }
                }
            }
        }

        private async Task EnsureSessionAsync()
        {
            if (_isWarmedUp) return;

            await _warmupLock.WaitAsync();
            try
            {
                if (_isWarmedUp) return;

                try
                {
                    using var warmupReq = new HttpRequestMessage(HttpMethod.Get, "https://old.reddit.com/");
                    using var resp = await _httpClient.SendAsync(warmupReq, HttpCompletionOption.ResponseHeadersRead);
                }
                catch
                {
                    // Non-fatal, continue
                }
                _isWarmedUp = true;
            }
            finally
            {
                _warmupLock.Release();
            }
        }

        private const string CuratedGifSubreddits = "gifs+HighQualityGifs+reactiongifs+wholesomegifs+funnygifs+CatGifs+AnimalGifs+Eyebleach+naturegifs";
        private const string CuratedNsfwGifSubreddits = "gifs+HighQualityGifs+reactiongifs+wholesomegifs+funnygifs+animegifs+nsfw_gifs+NSFW_GIF+boobgifs+assholepixel";
        private const string CuratedStickerSubreddits = "Art+digitalart+AnimeART+wallpaper+wallpapers+memes+dankmemes+aww+Eyebleach+PopArtNouveau+ImaginaryCharacters";
        private const string CuratedNsfwStickerSubreddits = "Art+digitalart+AnimeART+wallpaper+wallpapers+memes+dankmemes+aww+Eyebleach+PopArtNouveau+ImaginaryCharacters+ecchi+hentai";

        public async Task<MediaSearchResult> SearchGifsAsync(
            string query,
            string sort = "relevance",
            string timeRange = "all",
            string? after = null,
            int limit = 40,
            bool includeNsfw = false,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"gif:{query}:{sort}:{timeRange}:{after}:{limit}:{includeNsfw}";
            if (_cache.TryGetValue(cacheKey, out var cached) && DateTime.UtcNow < cached.Expiration)
            {
                return cached.Result;
            }

            await EnsureSessionAsync();

            bool isTrending = string.IsNullOrWhiteSpace(query);
            int currentStage = 0;
            string? subAfter = null;

            if (!string.IsNullOrEmpty(after))
            {
                if (after.StartsWith("s:", StringComparison.Ordinal))
                {
                    var parts = after.Split(':', 3);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int s))
                    {
                        currentStage = s;
                        subAfter = parts.Length == 3 && !string.IsNullOrEmpty(parts[2]) ? parts[2] : null;
                    }
                }
                else
                {
                    subAfter = after;
                }
            }

            const int maxStages = 5;
            var combinedResult = new MediaSearchResult();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Exception? lastException = null;

            while (currentStage < maxStages && combinedResult.Items.Count < limit)
            {
                string path = BuildStagePath(isTrending, query, currentStage, subAfter, limit, includeNsfw);

                try
                {
                    var stageResult = await FetchFromEndpointsAsync(path, includeNsfw, cancellationToken);
                    if (stageResult != null && stageResult.Items.Count > 0)
                    {
                        foreach (var item in stageResult.Items)
                        {
                            if (seenIds.Add(item.Id))
                            {
                                combinedResult.Items.Add(item);
                            }
                        }
                        subAfter = stageResult.After;
                    }
                    else
                    {
                        subAfter = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    subAfter = null;
                }

                if (string.IsNullOrEmpty(subAfter))
                {
                    currentStage++;
                    subAfter = null;
                }

                if (combinedResult.Items.Count >= (limit / 2) || combinedResult.Items.Count >= limit)
                {
                    break;
                }
            }

            if (combinedResult.Items.Count == 0 && lastException != null && currentStage >= maxStages)
            {
                throw lastException;
            }

            if (currentStage < maxStages)
            {
                combinedResult.After = $"s:{currentStage}:{subAfter ?? ""}";
            }
            else
            {
                combinedResult.After = null;
            }

            _cache[cacheKey] = (DateTime.UtcNow.AddMinutes(5), combinedResult);
            PruneCacheIfFull();

            return combinedResult;
        }

        private string BuildStagePath(bool isTrending, string query, int stage, string? subAfter, int limit, bool includeNsfw)
        {
            string afterParam = !string.IsNullOrEmpty(subAfter) ? $"&after={Uri.EscapeDataString(subAfter)}" : "";
            int fetchLimit = Math.Max(limit, 50);
            string nsfwParam = includeNsfw ? "&include_over_18=on" : "";
            string activeSubreddits = includeNsfw ? CuratedNsfwGifSubreddits : CuratedGifSubreddits;

            if (isTrending)
            {
                return stage switch
                {
                    0 => $"/r/{activeSubreddits}/hot.json?limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    1 => $"/r/{activeSubreddits}/top.json?t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    2 => $"/r/{activeSubreddits}/new.json?limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    3 => $"/search.json?q=url:gif&sort=hot&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    _ => $"/search.json?q=url:gif&sort=top&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}"
                };
            }
            else
            {
                string cleanQuery = Uri.EscapeDataString(query.Trim());
                return stage switch
                {
                    0 => $"/search.json?q={cleanQuery}%20url:gif&sort=relevance&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    1 => $"/search.json?q={cleanQuery}%20url:gif&sort=top&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    2 => $"/r/{activeSubreddits}/search.json?q={cleanQuery}&restrict_sr=on&sort=relevance&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    3 => $"/r/{activeSubreddits}/search.json?q={cleanQuery}&restrict_sr=on&sort=top&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    _ => $"/search.json?q={cleanQuery}%20url:gif&sort=new&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}"
                };
            }
        }

        private async Task<MediaSearchResult?> FetchFromEndpointsAsync(string path, bool includeNsfw, CancellationToken cancellationToken)
        {
            string[] domains = new[]
            {
                "https://www.reddit.com",
                "https://new.reddit.com",
                "https://sh.reddit.com"
            };

            foreach (var domain in domains)
            {
                string url = $"{domain}{path}";
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    using var response = await _httpClient.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        using var doc = await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancellationToken);
                        return ParseApiResponse(doc, includeNsfw);
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        continue;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    // Fallback to next domain
                }
            }

            return null;
        }

        private MediaSearchResult ParseApiResponse(JsonDocument doc, bool includeNsfw)
        {
            var result = new MediaSearchResult();

            if (!doc.RootElement.TryGetProperty("data", out var dataElem))
            {
                return result;
            }

            if (dataElem.TryGetProperty("after", out var afterElem) && afterElem.ValueKind == JsonValueKind.String)
            {
                result.After = afterElem.GetString();
            }

            if (dataElem.TryGetProperty("dist", out var distElem) && distElem.ValueKind == JsonValueKind.Number)
            {
                result.Dist = distElem.GetInt32();
            }

            if (dataElem.TryGetProperty("children", out var childrenElem) && childrenElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var child in childrenElem.EnumerateArray())
                {
                    if (!child.TryGetProperty("data", out var postData)) continue;

                    var item = ParseItem(postData, includeNsfw);
                    if (item != null)
                    {
                        result.Items.Add(item);
                    }
                }
            }

            return result;
        }

        private MediaItem? ParseItem(JsonElement postData, bool includeNsfw)
        {
            string id = postData.TryGetProperty("id", out var idElem) ? idElem.GetString() ?? "" : "";
            string title = postData.TryGetProperty("title", out var titleElem) ? titleElem.GetString() ?? "" : "";

            bool isOver18 = false;
            if (postData.TryGetProperty("over_18", out var o18Elem) && o18Elem.ValueKind == JsonValueKind.True)
            {
                isOver18 = true;
            }
            if (postData.TryGetProperty("thumbnail", out var thumbCheck) && thumbCheck.ValueKind == JsonValueKind.String)
            {
                if (string.Equals(thumbCheck.GetString(), "nsfw", StringComparison.OrdinalIgnoreCase))
                {
                    isOver18 = true;
                }
            }

            if (!includeNsfw && isOver18)
            {
                return null;
            }

            string author = postData.TryGetProperty("author", out var authorElem) ? authorElem.GetString() ?? "" : "";
            int ups = postData.TryGetProperty("ups", out var upsElem) ? upsElem.GetInt32() : 0;
            int numComments = postData.TryGetProperty("num_comments", out var commElem) ? commElem.GetInt32() : 0;
            string permalink = postData.TryGetProperty("permalink", out var permElem) ? $"https://reddit.com{permElem.GetString()}" : "";
            double createdUtc = postData.TryGetProperty("created_utc", out var crElem) ? crElem.GetDouble() : 0;
            string postUrl = postData.TryGetProperty("url", out var urlElem) ? urlElem.GetString() ?? "" : "";

            string? gifUrl = null;
            string? mp4Url = null;
            string? staticPreviewUrl = null;
            int width = 0;
            int height = 0;
            string? optimizedThumbnail = null;

            if (postData.TryGetProperty("preview", out var previewElem))
            {
                if (previewElem.TryGetProperty("reddit_video_preview", out var rvpElem) &&
                    rvpElem.TryGetProperty("fallback_url", out var fbElem))
                {
                    string rawFb = WebUtility.HtmlDecode(fbElem.GetString() ?? "");
                    if (!string.IsNullOrEmpty(rawFb))
                    {
                        // Use DASH_360 for preview to massively save bandwidth and GPU load
                        mp4Url = Regex.Replace(rawFb, @"DASH_\d+\.mp4", "DASH_360.mp4", RegexOptions.IgnoreCase);
                    }
                }

                if (previewElem.TryGetProperty("images", out var imagesElem) && 
                    imagesElem.ValueKind == JsonValueKind.Array && 
                    imagesElem.GetArrayLength() > 0)
                {
                    var firstImg = imagesElem[0];

                    if (firstImg.TryGetProperty("source", out var srcElem))
                    {
                        if (srcElem.TryGetProperty("url", out var sUrlElem))
                        {
                            staticPreviewUrl = WebUtility.HtmlDecode(sUrlElem.GetString());
                        }
                        if (srcElem.TryGetProperty("width", out var wElem)) width = wElem.GetInt32();
                        if (srcElem.TryGetProperty("height", out var hElem)) height = hElem.GetInt32();
                    }

                    // Pick optimized ~200-360px thumbnail from preview images resolutions
                    if (firstImg.TryGetProperty("resolutions", out var imgResElem) && imgResElem.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var r in imgResElem.EnumerateArray())
                        {
                            if (r.TryGetProperty("width", out var rw) && rw.GetInt32() >= 200)
                            {
                                if (r.TryGetProperty("url", out var rUrl) && rUrl.ValueKind == JsonValueKind.String)
                                {
                                    optimizedThumbnail = WebUtility.HtmlDecode(rUrl.GetString());
                                    break;
                                }
                            }
                        }
                    }

                    if (firstImg.TryGetProperty("variants", out var variantsElem))
                    {
                        if (variantsElem.TryGetProperty("gif", out var gifVarElem) &&
                            gifVarElem.TryGetProperty("source", out var gifSrcElem) &&
                            gifSrcElem.TryGetProperty("url", out var gifSrcUrlElem))
                        {
                            gifUrl = WebUtility.HtmlDecode(gifSrcUrlElem.GetString());
                            if (gifSrcElem.TryGetProperty("width", out var gw)) width = gw.GetInt32();
                            if (gifSrcElem.TryGetProperty("height", out var gh)) height = gh.GetInt32();
                        }

                        if (variantsElem.TryGetProperty("mp4", out var mp4VarElem))
                        {
                            string? lowResMp4 = null;
                            if (mp4VarElem.TryGetProperty("resolutions", out var mp4ResElem) && mp4ResElem.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var r in mp4ResElem.EnumerateArray())
                                {
                                    if (r.TryGetProperty("width", out var rw) && rw.GetInt32() >= 200)
                                    {
                                        if (r.TryGetProperty("url", out var rUrl) && rUrl.ValueKind == JsonValueKind.String)
                                        {
                                            lowResMp4 = WebUtility.HtmlDecode(rUrl.GetString());
                                            break;
                                        }
                                    }
                                }
                            }

                            string? sourceMp4 = null;
                            if (mp4VarElem.TryGetProperty("source", out var mp4SrcElem) &&
                                mp4SrcElem.TryGetProperty("url", out var mp4SrcUrlElem))
                            {
                                sourceMp4 = WebUtility.HtmlDecode(mp4SrcUrlElem.GetString());
                            }

                            // Use low-res MP4 (~240p-360p) for silky preview, fallback to source if resolutions is empty
                            mp4Url = !string.IsNullOrEmpty(lowResMp4) ? lowResMp4 : sourceMp4;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(postUrl))
            {
                if (postUrl.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                    postUrl.Contains(".gif?", StringComparison.OrdinalIgnoreCase))
                {
                    gifUrl ??= postUrl;
                }
                else if (Regex.IsMatch(postUrl, @"imgur\.com/[^/]+\.gifv?$", RegexOptions.IgnoreCase))
                {
                    gifUrl ??= Regex.Replace(postUrl, @"\.gifv?$", ".gif", RegexOptions.IgnoreCase);
                    if (string.IsNullOrEmpty(mp4Url))
                    {
                        mp4Url = Regex.Replace(postUrl, @"\.gifv?$", ".mp4", RegexOptions.IgnoreCase);
                    }
                }
                else if (postUrl.Contains("giphy.com/media/", StringComparison.OrdinalIgnoreCase) ||
                         postUrl.Contains("media.giphy.com/", StringComparison.OrdinalIgnoreCase))
                {
                    gifUrl ??= postUrl;
                    if (string.IsNullOrEmpty(mp4Url))
                    {
                        // Giphy provides lightweight 200w.mp4 (~80KB) for preview
                        mp4Url = Regex.Replace(postUrl, @"(giphy\.gif|source\.gif).*", "200w.mp4", RegexOptions.IgnoreCase);
                    }
                }
            }

            if (string.IsNullOrEmpty(gifUrl))
            {
                return null;
            }

            string cleanCheck = gifUrl.Split('?')[0].ToLowerInvariant();
            if (cleanCheck.EndsWith(".jpg") || cleanCheck.EndsWith(".jpeg") || cleanCheck.EndsWith(".png") || cleanCheck.EndsWith(".webp"))
            {
                return null;
            }

            if ((width == 0 || height == 0) && postData.TryGetProperty("thumbnail_width", out var tw) && tw.ValueKind == JsonValueKind.Number &&
                postData.TryGetProperty("thumbnail_height", out var th) && th.ValueKind == JsonValueKind.Number)
            {
                width = tw.GetInt32();
                height = th.GetInt32();
            }
            if (width == 0 || height == 0)
            {
                width = 400;
                height = 300;
            }

            string thumbnail = "";
            if (!string.IsNullOrEmpty(optimizedThumbnail))
            {
                thumbnail = optimizedThumbnail;
            }
            else if (postData.TryGetProperty("thumbnail", out var thumbElem) && thumbElem.ValueKind == JsonValueKind.String)
            {
                string thumbStr = thumbElem.GetString() ?? "";
                if (thumbStr.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    thumbnail = thumbStr;
                }
            }
            if (string.IsNullOrEmpty(thumbnail))
            {
                thumbnail = !string.IsNullOrEmpty(staticPreviewUrl) ? staticPreviewUrl : (gifUrl ?? "");
            }

            return new MediaItem
            {
                Id = id,
                Title = title,
                Author = author,
                Ups = ups,
                NumComments = numComments,
                Permalink = permalink,
                ThumbnailUrl = thumbnail,
                MediaUrl = gifUrl,
                Mp4PreviewUrl = mp4Url,
                StaticPreviewUrl = staticPreviewUrl,
                Width = width,
                Height = height,
                CreatedUtc = createdUtc,
                IsOver18 = isOver18
            };
        }

        public async Task<MediaSearchResult> SearchStickersAsync(
            string query,
            string sort = "relevance",
            string timeRange = "all",
            string? after = null,
            int limit = 40,
            bool includeNsfw = false,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"img:{query}:{sort}:{timeRange}:{after}:{limit}:{includeNsfw}";
            if (_cache.TryGetValue(cacheKey, out var cached) && DateTime.UtcNow < cached.Expiration)
            {
                return cached.Result;
            }

            await EnsureSessionAsync();

            bool isTrending = string.IsNullOrWhiteSpace(query);
            int currentStage = 0;
            string? subAfter = null;

            if (!string.IsNullOrEmpty(after))
            {
                if (after.StartsWith("s:", StringComparison.Ordinal))
                {
                    var parts = after.Split(':', 3);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int s))
                    {
                        currentStage = s;
                        subAfter = parts.Length == 3 && !string.IsNullOrEmpty(parts[2]) ? parts[2] : null;
                    }
                }
                else
                {
                    subAfter = after;
                }
            }

            const int maxStages = 5;
            var combinedResult = new MediaSearchResult();
            var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Exception? lastException = null;

            while (currentStage < maxStages && combinedResult.Items.Count < limit)
            {
                string path = BuildStickerStagePath(isTrending, query, currentStage, subAfter, limit, includeNsfw);

                try
                {
                    var stageResult = await FetchStickersFromEndpointsAsync(path, includeNsfw, cancellationToken);
                    if (stageResult != null && stageResult.Items.Count > 0)
                    {
                        foreach (var item in stageResult.Items)
                        {
                            if (seenIds.Add(item.Id))
                            {
                                combinedResult.Items.Add(item);
                            }
                        }
                        subAfter = stageResult.After;
                    }
                    else
                    {
                        subAfter = null;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    subAfter = null;
                }

                if (string.IsNullOrEmpty(subAfter))
                {
                    currentStage++;
                    subAfter = null;
                }

                if (combinedResult.Items.Count >= (limit / 2) || combinedResult.Items.Count >= limit)
                {
                    break;
                }
            }

            if (combinedResult.Items.Count == 0 && lastException != null && currentStage >= maxStages)
            {
                throw lastException;
            }

            if (currentStage < maxStages)
            {
                combinedResult.After = $"s:{currentStage}:{subAfter ?? ""}";
            }
            else
            {
                combinedResult.After = null;
            }

            _cache[cacheKey] = (DateTime.UtcNow.AddMinutes(5), combinedResult);
            PruneCacheIfFull();

            return combinedResult;
        }

        private string BuildStickerStagePath(bool isTrending, string query, int stage, string? subAfter, int limit, bool includeNsfw)
        {
            string afterParam = !string.IsNullOrEmpty(subAfter) ? $"&after={Uri.EscapeDataString(subAfter)}" : "";
            int fetchLimit = Math.Max(limit, 50);
            string nsfwParam = includeNsfw ? "&include_over_18=on" : "";
            string activeSubreddits = includeNsfw ? CuratedNsfwStickerSubreddits : CuratedStickerSubreddits;

            if (isTrending)
            {
                return stage switch
                {
                    0 => $"/r/{activeSubreddits}/hot.json?limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    1 => $"/r/{activeSubreddits}/top.json?t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    2 => $"/r/{activeSubreddits}/top.json?t=month&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    3 => $"/r/{activeSubreddits}/top.json?t=week&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    _ => $"/r/{activeSubreddits}/new.json?limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}"
                };
            }
            else
            {
                string cleanQuery = Uri.EscapeDataString(query.Trim());
                return stage switch
                {
                    0 => $"/search.json?q={cleanQuery}&sort=relevance&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    1 => $"/search.json?q={cleanQuery}&sort=top&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    2 => $"/search.json?q={cleanQuery}&sort=hot&t=all&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    3 => $"/r/{activeSubreddits}/search.json?q={cleanQuery}&restrict_sr=on&sort=relevance&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}",
                    _ => $"/search.json?q={cleanQuery}&sort=new&limit={fetchLimit}{afterParam}&raw_json=1{nsfwParam}"
                };
            }
        }

        private async Task<MediaSearchResult?> FetchStickersFromEndpointsAsync(string path, bool includeNsfw, CancellationToken cancellationToken)
        {
            string[] domains = new[]
            {
                "https://www.reddit.com",
                "https://new.reddit.com",
                "https://sh.reddit.com"
            };

            foreach (var domain in domains)
            {
                string url = $"{domain}{path}";
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    using var response = await _httpClient.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        using var doc = await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancellationToken);
                        return ParseStickerApiResponse(doc, includeNsfw);
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        continue;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    // Fallback to next domain
                }
            }

            return null;
        }

        private MediaSearchResult ParseStickerApiResponse(JsonDocument doc, bool includeNsfw)
        {
            var result = new MediaSearchResult();

            if (!doc.RootElement.TryGetProperty("data", out var dataElem))
            {
                return result;
            }

            if (dataElem.TryGetProperty("after", out var afterElem) && afterElem.ValueKind == JsonValueKind.String)
            {
                result.After = afterElem.GetString();
            }

            if (dataElem.TryGetProperty("dist", out var distElem) && distElem.ValueKind == JsonValueKind.Number)
            {
                result.Dist = distElem.GetInt32();
            }

            if (dataElem.TryGetProperty("children", out var childrenElem) && childrenElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var child in childrenElem.EnumerateArray())
                {
                    if (!child.TryGetProperty("data", out var postData)) continue;

                    var item = ParseStickerItem(postData, includeNsfw);
                    if (item != null)
                    {
                        result.Items.Add(item);
                    }
                }
            }

            return result;
        }

        private MediaItem? ParseStickerItem(JsonElement postData, bool includeNsfw)
        {
            string id = postData.TryGetProperty("id", out var idElem) ? idElem.GetString() ?? "" : "";
            string title = postData.TryGetProperty("title", out var titleElem) ? titleElem.GetString() ?? "" : "";

            bool isOver18 = false;
            if (postData.TryGetProperty("over_18", out var o18Elem) && o18Elem.ValueKind == JsonValueKind.True)
            {
                isOver18 = true;
            }
            if (postData.TryGetProperty("thumbnail", out var thumbCheck) && thumbCheck.ValueKind == JsonValueKind.String)
            {
                if (string.Equals(thumbCheck.GetString(), "nsfw", StringComparison.OrdinalIgnoreCase))
                {
                    isOver18 = true;
                }
            }

            if (!includeNsfw && isOver18)
            {
                return null;
            }

            if (postData.TryGetProperty("is_video", out var isVideoElem) && isVideoElem.ValueKind == JsonValueKind.True)
            {
                return null;
            }

            if (postData.TryGetProperty("post_hint", out var hintElem) && hintElem.ValueKind == JsonValueKind.String)
            {
                string hint = hintElem.GetString() ?? "";
                if (hint.Contains("video", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            string postUrl = postData.TryGetProperty("url_overridden_by_dest", out var destElem) && destElem.ValueKind == JsonValueKind.String
                ? destElem.GetString() ?? ""
                : (postData.TryGetProperty("url", out var urlElem) && urlElem.ValueKind == JsonValueKind.String ? urlElem.GetString() ?? "" : "");

            string cleanPostUrl = postUrl.Split('?')[0].ToLowerInvariant();
            if (cleanPostUrl.EndsWith(".gif") || cleanPostUrl.EndsWith(".gifv") ||
                cleanPostUrl.Contains(".gif?") || cleanPostUrl.Contains("v.redd.it") ||
                cleanPostUrl.Contains("youtube.com") || cleanPostUrl.Contains("youtu.be") ||
                cleanPostUrl.Contains("redgifs.com") || cleanPostUrl.Contains("gfycat.com"))
            {
                return null;
            }

            string? imageUrl = null;
            string? thumbnailUrl = null;
            int width = 0;
            int height = 0;

            if (postData.TryGetProperty("preview", out var previewElem) &&
                previewElem.TryGetProperty("images", out var imagesElem) &&
                imagesElem.ValueKind == JsonValueKind.Array &&
                imagesElem.GetArrayLength() > 0)
            {
                var firstImg = imagesElem[0];

                if (firstImg.TryGetProperty("variants", out var variantsElem) &&
                    variantsElem.TryGetProperty("gif", out _))
                {
                    return null;
                }

                if (firstImg.TryGetProperty("source", out var srcElem))
                {
                    if (srcElem.TryGetProperty("url", out var sUrlElem) && sUrlElem.ValueKind == JsonValueKind.String)
                    {
                        imageUrl = WebUtility.HtmlDecode(sUrlElem.GetString());
                    }
                    if (srcElem.TryGetProperty("width", out var wElem) && wElem.ValueKind == JsonValueKind.Number) width = wElem.GetInt32();
                    if (srcElem.TryGetProperty("height", out var hElem) && hElem.ValueKind == JsonValueKind.Number) height = hElem.GetInt32();
                }

                if (firstImg.TryGetProperty("resolutions", out var resElem) && resElem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var r in resElem.EnumerateArray())
                    {
                        if (r.TryGetProperty("width", out var rw) && rw.GetInt32() >= 300)
                        {
                            if (r.TryGetProperty("url", out var rUrl) && rUrl.ValueKind == JsonValueKind.String)
                            {
                                thumbnailUrl = WebUtility.HtmlDecode(rUrl.GetString());
                                break;
                            }
                        }
                    }
                }
            }

            if (cleanPostUrl.EndsWith(".png") || cleanPostUrl.EndsWith(".jpg") || 
                cleanPostUrl.EndsWith(".jpeg") || cleanPostUrl.EndsWith(".webp") ||
                postUrl.Contains("i.redd.it/", StringComparison.OrdinalIgnoreCase))
            {
                imageUrl = postUrl;
            }

            if (string.IsNullOrEmpty(imageUrl) &&
                postData.TryGetProperty("is_gallery", out var isGalElem) && isGalElem.ValueKind == JsonValueKind.True &&
                postData.TryGetProperty("media_metadata", out var metaElem) && metaElem.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in metaElem.EnumerateObject())
                {
                    var itemMeta = prop.Value;
                    if (itemMeta.TryGetProperty("status", out var stElem) && stElem.GetString() == "valid" &&
                        itemMeta.TryGetProperty("s", out var sObj))
                    {
                        if (sObj.TryGetProperty("u", out var uElem) && uElem.ValueKind == JsonValueKind.String)
                        {
                            imageUrl = WebUtility.HtmlDecode(uElem.GetString());
                        }
                        if (sObj.TryGetProperty("x", out var xElem) && xElem.ValueKind == JsonValueKind.Number) width = xElem.GetInt32();
                        if (sObj.TryGetProperty("y", out var yElem) && yElem.ValueKind == JsonValueKind.Number) height = yElem.GetInt32();

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            string cleanImg = imageUrl.Split('?')[0].ToLowerInvariant();
            if (cleanImg.EndsWith(".gif"))
            {
                return null;
            }

            if (width == 0 || height == 0)
            {
                width = 400;
                height = 400;
            }

            if (string.IsNullOrEmpty(thumbnailUrl))
            {
                thumbnailUrl = imageUrl;
            }

            string author = postData.TryGetProperty("author", out var authorElem) ? authorElem.GetString() ?? "" : "";
            int ups = postData.TryGetProperty("ups", out var upsElem) ? upsElem.GetInt32() : 0;
            int numComments = postData.TryGetProperty("num_comments", out var commElem) ? commElem.GetInt32() : 0;
            string permalink = postData.TryGetProperty("permalink", out var permElem) ? $"https://reddit.com{permElem.GetString()}" : "";
            double createdUtc = postData.TryGetProperty("created_utc", out var crElem) ? crElem.GetDouble() : 0;

            return new MediaItem
            {
                Id = id,
                Title = title,
                Author = author,
                Ups = ups,
                NumComments = numComments,
                Permalink = permalink,
                ThumbnailUrl = thumbnailUrl,
                MediaUrl = imageUrl,
                Mp4PreviewUrl = null,
                StaticPreviewUrl = imageUrl,
                Width = width,
                Height = height,
                CreatedUtc = createdUtc,
                IsOver18 = isOver18
            };
        }

        public async Task<string> DownloadMediaToTempFileAsync(MediaItem item, CancellationToken cancellationToken = default)
        {
            string? downloadUrl = item.MediaUrl ?? item.StaticPreviewUrl;

            if (string.IsNullOrEmpty(downloadUrl))
            {
                throw new InvalidOperationException("No valid image/media URL found for this item.");
            }

            string ext = ".png";
            string cleanUrl = downloadUrl.Split('?')[0].ToLowerInvariant();
            if (cleanUrl.EndsWith(".gif")) ext = ".gif";
            else if (cleanUrl.EndsWith(".jpg") || cleanUrl.EndsWith(".jpeg")) ext = ".jpg";
            else if (cleanUrl.EndsWith(".png")) ext = ".png";
            else if (cleanUrl.EndsWith(".webp")) ext = ".webp";

            using var req = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
            if (downloadUrl.Contains("imgur.com", StringComparison.OrdinalIgnoreCase))
            {
                req.Headers.TryAddWithoutValidation("Referer", "https://imgur.com/");
            }

            using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            resp.EnsureSuccessStatusCode();

            var mediaType = resp.Content.Headers.ContentType?.MediaType?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(mediaType))
            {
                if (mediaType.Contains("gif")) ext = ".gif";
                else if (mediaType.Contains("png")) ext = ".png";
                else if (mediaType.Contains("jpeg") || mediaType.Contains("jpg")) ext = ".jpg";
                else if (mediaType.Contains("webp")) ext = ".webp";
            }

            string tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            string prefix = ext == ".gif" ? "gif" : "media";
            string tempFile = Path.Combine(tempDir, $"{prefix}_{Guid.NewGuid():N}{ext}");

            await using (var contentStream = await resp.Content.ReadAsStreamAsync(cancellationToken))
            await using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await contentStream.CopyToAsync(fileStream, 81920, cancellationToken);
            }

            var fileInfo = new FileInfo(tempFile);
            if (fileInfo.Length == 0)
            {
                try { File.Delete(tempFile); } catch { }
                throw new Exception("Downloaded file was empty.");
            }

            return tempFile;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _handler?.Dispose();
            _warmupLock?.Dispose();
        }
    }
}
