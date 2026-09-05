using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRCGalleryManager.Core.Api.Models;

namespace VRCGalleryManager.Core.Api
{
    public class VRChatApiClient
    {
        public const string DefaultBaseUrl = "https://api.vrchat.cloud/api/1/";
        public const string DefaultUserAgent = "VRCGalleryManager";

        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, string> _cookies = new(StringComparer.OrdinalIgnoreCase);
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public event Action? OnCookiesUpdated;

        public string CookieHeader
        {
            get => string.Join("; ", _cookies.Select(kv => $"{kv.Key}={kv.Value}"));
            set => SetCookieHeader(value);
        }

        public VRChatApiClient(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(DefaultBaseUrl);
            }
        }

        public void SetCookieHeader(string? cookieString)
        {
            _cookies.Clear();
            if (string.IsNullOrWhiteSpace(cookieString))
            {
                OnCookiesUpdated?.Invoke();
                return;
            }

            if (!cookieString.Contains('='))
            {
                cookieString = $"auth={cookieString}";
            }

            var parts = cookieString.Split(';');
            foreach (var part in parts)
            {
                var kv = part.Trim().Split(new[] { '=' }, 2);
                if (kv.Length == 2 && !string.IsNullOrWhiteSpace(kv[0]))
                {
                    _cookies[kv[0].Trim()] = kv[1].Trim();
                }
            }
            OnCookiesUpdated?.Invoke();
        }

        public void ClearCookies()
        {
            _cookies.Clear();
            OnCookiesUpdated?.Invoke();
        }

        public void ClearAuthCookieOnly()
        {
            _cookies.TryRemove("auth", out _);
            OnCookiesUpdated?.Invoke();
        }

        public void UpdateCookie(string name, string value)
        {
            _cookies[name] = value;
            OnCookiesUpdated?.Invoke();
        }

        public bool TryGetCookie(string name, out string? value)
        {
            return _cookies.TryGetValue(name, out value);
        }

        private void ExtractCookies(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                bool changed = false;
                foreach (var sc in setCookies)
                {
                    var cookiePart = sc.Split(';')[0].Trim();
                    var kv = cookiePart.Split(new[] { '=' }, 2);
                    if (kv.Length == 2 && !string.IsNullOrWhiteSpace(kv[0]))
                    {
                        _cookies[kv[0].Trim()] = kv[1].Trim();
                        changed = true;
                    }
                }
                if (changed)
                {
                    OnCookiesUpdated?.Invoke();
                }
            }
        }

        public async Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request, CancellationToken ct = default)
        {
            if (!request.Headers.Contains("User-Agent"))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", DefaultUserAgent);
            }

            var cookieStr = CookieHeader;
            if (!string.IsNullOrEmpty(cookieStr) && !request.Headers.Contains("Cookie"))
            {
                request.Headers.TryAddWithoutValidation("Cookie", cookieStr);
            }

            var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            ExtractCookies(response);
            return response;
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string rawContent = "";
                try
                {
                    rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch { }

                string message = ExtractErrorMessage(rawContent, $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                throw new VRChatApiException((int)response.StatusCode, message, rawContent);
            }
        }

        public static string ExtractErrorMessage(string? rawContent, string defaultMessage = "API Error")
        {
            if (string.IsNullOrWhiteSpace(rawContent)) return defaultMessage;

            try
            {
                using var doc = JsonDocument.Parse(rawContent);
                if (doc.RootElement.TryGetProperty("error", out var errorEl))
                {
                    if (errorEl.ValueKind == JsonValueKind.Object && errorEl.TryGetProperty("message", out var msgEl))
                    {
                        return msgEl.GetString() ?? defaultMessage;
                    }
                    if (errorEl.ValueKind == JsonValueKind.String)
                    {
                        return errorEl.GetString() ?? defaultMessage;
                    }
                }
                if (doc.RootElement.TryGetProperty("message", out var directMsg))
                {
                    return directMsg.GetString() ?? defaultMessage;
                }
            }
            catch { }

            return defaultMessage;
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken ct = default)
        {
            using var response = await SendRawAsync(request, ct).ConfigureAwait(false);
            await EnsureSuccessAsync(response);

            var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            if (result == null)
            {
                throw new VRChatApiException((int)response.StatusCode, "Failed to deserialize API response.", json);
            }
            return result;
        }

        public async Task SendAsync(HttpRequestMessage request, CancellationToken ct = default)
        {
            using var response = await SendRawAsync(request, ct).ConfigureAwait(false);
            await EnsureSuccessAsync(response);
        }

        #region Authentication

        public async Task<(CurrentUser? user, string rawResponse, int statusCode)> LoginRawAsync(string username, string password, CancellationToken ct = default)
        {
            ClearCookies();

            using var req = new HttpRequestMessage(HttpMethod.Get, "auth/user");
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            using var resp = await SendRawAsync(req, ct).ConfigureAwait(false);
            var raw = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            int code = (int)resp.StatusCode;

            CurrentUser? user = null;
            if (resp.IsSuccessStatusCode)
            {
                try
                {
                    user = JsonSerializer.Deserialize<CurrentUser>(raw, _jsonOptions);
                }
                catch { }
            }

            return (user, raw, code);
        }

        public async Task<bool> VerifyEmail2FAAsync(string code, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, "auth/twofactorauth/emailotp/verify")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { code = code.Trim() }), Encoding.UTF8, "application/json")
            };

            using var resp = await SendRawAsync(req, ct).ConfigureAwait(false);
            var raw = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                throw new VRChatApiException((int)resp.StatusCode, ExtractErrorMessage(raw, "Invalid 2FA email code."), raw);
            }

            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("verified", out var v) && v.GetBoolean())
                {
                    return true;
                }
            }
            catch { }

            return raw.Contains("\"verified\":true");
        }

        public async Task<bool> VerifyTotp2FAAsync(string code, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, "auth/twofactorauth/totp/verify")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { code = code.Trim() }), Encoding.UTF8, "application/json")
            };

            using var resp = await SendRawAsync(req, ct).ConfigureAwait(false);
            var raw = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                // Fallback to otp/verify endpoint if totp/verify returns 404
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    using var reqOtp = new HttpRequestMessage(HttpMethod.Put, "auth/twofactorauth/otp/verify")
                    {
                        Content = new StringContent(JsonSerializer.Serialize(new { code = code.Trim() }), Encoding.UTF8, "application/json")
                    };
                    using var respOtp = await SendRawAsync(reqOtp, ct).ConfigureAwait(false);
                    var rawOtp = await respOtp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    if (!respOtp.IsSuccessStatusCode)
                    {
                        throw new VRChatApiException((int)respOtp.StatusCode, ExtractErrorMessage(rawOtp, "Invalid 2FA code."), rawOtp);
                    }
                    return rawOtp.Contains("\"verified\":true");
                }

                throw new VRChatApiException((int)resp.StatusCode, ExtractErrorMessage(raw, "Invalid 2FA code."), raw);
            }

            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("verified", out var v) && v.GetBoolean())
                {
                    return true;
                }
            }
            catch { }

            return raw.Contains("\"verified\":true");
        }

        public async Task<CurrentUser> GetCurrentUserAsync(CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, "auth/user");
            return await SendAsync<CurrentUser>(req, ct).ConfigureAwait(false);
        }

        public async Task LogoutAsync(CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, "logout");
            try
            {
                await SendAsync(req, ct).ConfigureAwait(false);
            }
            catch { }
            finally
            {
                ClearCookies();
            }
        }

        #endregion

        #region Files & Images

        public async Task<List<VRChatFile>> GetFilesAsync(string tag, int n = 100, CancellationToken ct = default)
        {
            string url = $"files?tag={Uri.EscapeDataString(tag)}&n={n}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            var files = await SendAsync<List<VRChatFile>>(req, ct).ConfigureAwait(false);
            return files ?? new List<VRChatFile>();
        }

        public async Task<VRChatFile> UploadImageAsync(
            Stream stream,
            string fileName,
            string mimeType,
            string tag,
            string? maskTag = null,
            string? animationStyle = null,
            int? frames = null,
            int? framesOverTime = null,
            CancellationToken ct = default)
        {
            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            content.Add(streamContent, "file", fileName);

            content.Add(new StringContent(tag), "tag");

            if (!string.IsNullOrEmpty(maskTag))
            {
                content.Add(new StringContent(maskTag), "maskTag");
            }
            if (!string.IsNullOrEmpty(animationStyle))
            {
                content.Add(new StringContent(animationStyle), "animationStyle");
            }
            if (frames.HasValue && frames.Value > 0)
            {
                content.Add(new StringContent(frames.Value.ToString()), "frames");
            }
            if (framesOverTime.HasValue && framesOverTime.Value > 0)
            {
                content.Add(new StringContent(framesOverTime.Value.ToString()), "framesOverTime");
            }

            using var req = new HttpRequestMessage(HttpMethod.Post, "file/image")
            {
                Content = content
            };

            return await SendAsync<VRChatFile>(req, ct).ConfigureAwait(false);
        }

        public async Task DeleteFileAsync(string fileId, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"file/{Uri.EscapeDataString(fileId)}");
            await SendAsync(req, ct).ConfigureAwait(false);
        }

        #endregion

        #region Prints

        public async Task<List<Print>> GetUserPrintsAsync(string userId, int n = 100, int offset = 0, CancellationToken ct = default)
        {
            string url = $"prints/user/{Uri.EscapeDataString(userId)}?n={n}&offset={offset}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            var prints = await SendAsync<List<Print>>(req, ct).ConfigureAwait(false);
            return prints ?? new List<Print>();
        }

        public async Task<Print> UploadPrintAsync(
            Stream stream,
            string fileName,
            string mimeType,
            DateTime timestamp,
            string? note = null,
            string? worldId = null,
            string? worldName = null,
            CancellationToken ct = default)
        {
            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            content.Add(streamContent, "image", fileName);

            content.Add(new StringContent(timestamp.ToString("o")), "timestamp");

            if (!string.IsNullOrEmpty(note))
            {
                content.Add(new StringContent(note), "note");
            }
            if (!string.IsNullOrEmpty(worldId))
            {
                content.Add(new StringContent(worldId), "worldId");
            }
            if (!string.IsNullOrEmpty(worldName))
            {
                content.Add(new StringContent(worldName), "worldName");
            }

            using var req = new HttpRequestMessage(HttpMethod.Post, "prints")
            {
                Content = content
            };

            return await SendAsync<Print>(req, ct).ConfigureAwait(false);
        }

        public async Task<Print> GetPrintAsync(string printId, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"prints/{Uri.EscapeDataString(printId)}");
            return await SendAsync<Print>(req, ct).ConfigureAwait(false);
        }

        public async Task DeletePrintAsync(string printId, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"prints/{Uri.EscapeDataString(printId)}");
            await SendAsync(req, ct).ConfigureAwait(false);
        }

        #endregion

        #region Inventory

        public async Task<InventoryResponse> GetInventoryAsync(
            string itemType = "sticker",
            string? tags = "Custom Sticker",
            string? flags = "ugc",
            bool archived = false,
            string order = "newest_created",
            int n = 100,
            int offset = 0,
            CancellationToken ct = default)
        {
            var query = new List<string>
            {
                $"n={n}",
                $"offset={offset}",
                $"archived={archived.ToString().ToLower()}",
                $"order={Uri.EscapeDataString(order)}"
            };

            if (!string.IsNullOrEmpty(itemType)) query.Add($"types={Uri.EscapeDataString(itemType)}");
            if (!string.IsNullOrEmpty(tags)) query.Add($"tags={Uri.EscapeDataString(tags)}");
            if (!string.IsNullOrEmpty(flags)) query.Add($"flags={Uri.EscapeDataString(flags)}");

            string url = $"inventory?{string.Join("&", query)}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<InventoryResponse>(req, ct).ConfigureAwait(false);
        }

        public async Task<InventoryItem> GetUserInventoryItemAsync(string userId, string itemId, CancellationToken ct = default)
        {
            string url = $"user/{Uri.EscapeDataString(userId)}/inventory/{Uri.EscapeDataString(itemId)}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<InventoryItem>(req, ct).ConfigureAwait(false);
        }

        public async Task DeleteInventoryItemAsync(string itemId, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"inventory/{Uri.EscapeDataString(itemId)}");
            await SendAsync(req, ct).ConfigureAwait(false);
        }

        #endregion

        #region Users & Worlds

        public async Task<VRChatUser> GetUserAsync(string userId, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"users/{Uri.EscapeDataString(userId)}");
            return await SendAsync<VRChatUser>(req, ct).ConfigureAwait(false);
        }

        public async Task<CurrentUser> UpdateUserAsync(string userId, UpdateUserRequest updateRequest, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
            using var req = new HttpRequestMessage(HttpMethod.Put, $"users/{Uri.EscapeDataString(userId)}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return await SendAsync<CurrentUser>(req, ct).ConfigureAwait(false);
        }

        public async Task<VRChatWorld> GetWorldAsync(string worldId, CancellationToken ct = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"worlds/{Uri.EscapeDataString(worldId)}");
            return await SendAsync<VRChatWorld>(req, ct).ConfigureAwait(false);
        }

        #endregion
    }
}
