using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Globalization;
namespace SentinelFM
{
    public partial class MapNew_UserControl_MessageRotatingClient : System.Web.UI.UserControl
    {
        public string strMessage;
        protected SentinelFMSession sn = null;
        protected System.Web.UI.WebControls.Label lblTotalAlarms;
        protected clsUtility objUtil;
        public string _xml = "";
        public string _checksum = "";
        public string headerColor = "#009933";
        public string MDTMessagesScrollingHight = "120";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn.User.ViewAlarmScrolling == 0)
                MDTMessagesScrollingHight = "275";
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }

            if (sn.User.MenuColor != "")
                headerColor = sn.User.MenuColor;



        }

        public void RedirectToLogin()
        {

            Session.Abandon();
            //((SentinelFMBasePage)this.Page).RedirectToLogin();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScript", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            return;
        }
    }
}