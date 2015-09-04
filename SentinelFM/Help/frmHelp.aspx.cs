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

namespace SentinelFM
{

    public partial class Help_frmHelp : SentinelFMBasePage
    {
        public string URL;
        protected SentinelFMSession sn = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || string.IsNullOrEmpty(sn.UserName))
            {
                Response.Write("<SCRIPT Language='javascript'>window.open('../Login.aspx','_top') </SCRIPT>");
            }
        }
    }
}
