using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class UserControl_DriverSearch : System.Web.UI.UserControl
    {
        public string onDriverSelect = "";
        public string width = "300px";
        public string height = "'auto'";
        public bool loadResources = true;
        public string placeholder = "";
        
        protected string RootUrl;

        public string Input_DriverName;
        public string Input_DriverID;

        protected void Page_Load(object sender, EventArgs e)
        {
            RootUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            Input_DriverName = this.ID + "_" + "_DriverSearch_DriverName_";
            Input_DriverID = this.ID + "_" + "_DriverSearch_DriverID_";
        }

        /// <summary>
        /// Get Form Field _DriverSearch_DriverID_'s value
        /// </summary>
        public int SelectedDriverId
        {
            get {
                int _driverId = 0;
                int.TryParse(Request[Input_DriverID].ToString(), out _driverId);
                return _driverId; 
            }
        }
    }
}