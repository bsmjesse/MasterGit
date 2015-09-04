using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Diagnostics; 

namespace SentinelFM
{
    public partial class MapNew_UserControl_AlarmRotatingClient : System.Web.UI.UserControl
    {
        public string strAlarms;
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        public string alarmsScreenHight = "120";
        private Stopwatch Pagewatch = new Stopwatch();
        private Stopwatch watch = new Stopwatch();
        public string headerColor = "#009933";

        protected void Page_Load(object sender, EventArgs e)
        {
            //Clear IIS cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);

            if (sn.User.ViewAlarmScrolling == 0)
                return;

            if (sn.User.ViewMDTMessagesScrolling == 0)
                alarmsScreenHight = "305";
            

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                //((SentinelFMBasePage)this.Page).RedirectToLogin();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScript", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
                return;
            }

            if (sn.User.MenuColor != "")
                headerColor = sn.User.MenuColor;

        }

    }
}