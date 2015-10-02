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
    public partial class frmSessionValidate : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];

                if (Session.Contents == null || sn == null || String.IsNullOrEmpty(sn.UserName))
                {
                    RedirectToLogin();
                    return;
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }



        private void RedirectToLogin()
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "RedirectToLogin - Session timeout. SessionId:" + Session.SessionID.ToString() + ", Form:" + Page.GetType().Name));
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
            return;


        }
    }

}