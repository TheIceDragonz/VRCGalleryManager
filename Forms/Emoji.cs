using VRCEmojiManager.Core;

namespace VRCEmojiManager.Forms
{
    public partial class Emoji : Form
    {
        private ApiRequest apiRequest;

        private List<string> emojiId = new List<string>();
        private string emojiCount;

        public Emoji(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            emojiPanel.Controls.Clear();
            emojiId.Clear();

            ApiRequest.ApiData emoji = await apiRequest.GetApiData();

            emojiId = emoji.IdEmoji;
            emojiCount = emoji.CountEmoji;

            Cursor = Cursors.WaitCursor;

            foreach (string id in emojiId)
            {
                //* Add Panel
                AddEmojiPanel(id);
            }

            Cursor = Cursors.Default;
        }
    }
}
