using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class Reply : SentinelFMBasePage
    {
        #region Java Script Localization Messages
            public string Warning_MessageSent;
            public string Warning_MessageNotSent;
            public string PageTitle;
            public string Button_Send;
            public string Button_Cancel;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];

            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:"));
                Session.Abandon();
                Response.Write("<SCRIPT Language='javascript'>window.open('../Login.aspx','_top') </SCRIPT>");
                return;
            }

            Warning_MessageSent = GetLocalResourceObject("1_NotificationMessageWasSent").ToString();
            Warning_MessageNotSent = GetLocalResourceObject("1_NotificationMessageWasNotSent").ToString();
            PageTitle = GetLocalResourceObject("1_PageTitle").ToString();
            //Buttons 
            Button_Send = GetLocalResourceObject("1_ButtonSend").ToString();
            Button_Cancel = GetLocalResourceObject("1_ButtonCancel").ToString();
        }
    }
}