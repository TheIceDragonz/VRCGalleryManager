using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;
using VRCGalleryManager.Design;
using VRCGalleryManager.Forms;

namespace VRCGalleryManager.Core
{
    static public class MetaDataImageReader
    {
        public class VrcxData
        {
            public AuthorInfo Author { get; set; }
            public WorldInfo World { get; set; }
            public List<PlayerInfo> Players { get; set; }
        }

        public class AuthorInfo
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }

        public class WorldInfo
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string InstanceId { get; set; }
        }

        public class PlayerInfo
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }

        public static VrcxData? ExtractVrcxData(string filePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(filePath);
                int idx = Array.IndexOf(bytes, (byte)'{');
                if (idx < 0) return null;

                var reader = new Utf8JsonReader(bytes.AsSpan(idx), isFinalBlock: true, state: default);
                using var doc = JsonDocument.ParseValue(ref reader);

                string raw = doc.RootElement.GetRawText();
                return raw.Contains("\"application\":\"VRCX\"")
                    ? JsonConvert.DeserializeObject<VrcxData>(raw)
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public static async void ApiWorldInfo(VrcxData vrcxData, ApiRequest apiRequest, RoundedPictureBox worldImage, Label worldNameLabel)
        {
            try
            {
                var worldApi = await apiRequest.GetWorldInfo(vrcxData.World.Id);
                var finalImageUrl = await HttpImage.GetFinalUrlAsync(worldApi.ThumbnailUrl);
                worldImage.LoadAsync(finalImageUrl);
                worldImage.Cursor = Cursors.Hand;
                worldNameLabel.Text = vrcxData.World.Name;
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP error: {httpEx.Message}");
            }
            catch (Newtonsoft.Json.JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"JSON error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public static (RoundedLabel, bool) UsersInfo(PlayerInfo player)
        {
            bool isfriend = IsFriend(player.Id).Result;
            Color userColor = isfriend ? Color.Orange : Color.White;

            RoundedLabel usersName = new RoundedLabel
            {
                Text = player.DisplayName,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = userColor,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Location = new Point(5, 5),
                BackColor = Color.FromArgb(24, 27, 31),
                BorderSize = 0,
                BorderColor = Color.FromArgb(5, 55, 66),
            };

            if (!string.IsNullOrEmpty(player.Id))
            {
                usersName.Cursor = Cursors.Hand;
                usersName.MouseEnter += (sender, e) => {
                    usersName.BorderSize = 2;
                };
                usersName.MouseLeave += (sender, e) => {
                    usersName.BorderSize = 0;
                };

                usersName.Click += async (s, e) =>
                {
                    Process.Start("explorer.exe", "https://vrchat.com/home/user/" + player.Id);
                };
            }
            else
            {
                usersName.BackColor = Color.FromArgb(16, 18, 20);
                usersName.ForeColor = Color.FromArgb(100, 100, 100);
            }


            return (usersName, isfriend);
        }

        private static Task<bool> IsFriend(string userId)
        {
            if (Settings.Friends.Contains(userId))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
