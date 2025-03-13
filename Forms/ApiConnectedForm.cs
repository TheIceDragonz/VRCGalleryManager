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

        public void InitApiRequest(VRCAuth auth)
        {
            apiRequest = new ApiRequest(auth);
        }

    }
}
