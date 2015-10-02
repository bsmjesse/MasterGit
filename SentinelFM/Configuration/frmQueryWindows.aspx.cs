using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class Configuration_frmQueryWindows : SentinelFMBasePage
    {

        public string LastUpdatedQueryWindowsJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/configuration_querywindows.js")).ToString("yyyyMMddHHmmss");

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}