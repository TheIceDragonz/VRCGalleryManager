using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCGalleryManager.Core;

namespace VRCGalleryManager.Forms
{
    public partial class ApiConnectedForm : Form
    {
        protected ApiRequest apiRequest;

        public ApiConnectedForm()
        {
            this.Load += ApiConnectedForm_Load;
        }

        private void ApiConnectedForm_Load(object sender, EventArgs e)
        {
            AttachCustomScrollBars(this);
        }

        private void AttachCustomScrollBars(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is ScrollableControl scrollable && scrollable.AutoScroll)
                {
                    VRCGalleryManager.Design.ScrollBarHelper.Attach(scrollable);
                }

                if (control.HasChildren)
                {
                    AttachCustomScrollBars(control);
                }
            }
        }

        public void InitApiRequest(VRCAuth auth)
        {
            apiRequest = new ApiRequest(auth);
        }
    }
}
