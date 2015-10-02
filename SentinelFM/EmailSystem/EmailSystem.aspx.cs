using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class EmailSystem : SentinelFMBasePage
    {
        protected SentinelFMSession sn = null;
        protected string ReportServerURL;

        #region Java Script Localization Messages
            public string ErrorMessage;
            public string ErrorMessageToForward;
            public string Warning_DateAndTime;
            public string Warning_DateFromToDate;
            public string Notification_NoData;

        #endregion

        public string ErrorMessageTest;

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    sn = (SentinelFMSession)Session["SentinelFMSession"];  
        //    string userNameScript = "<script language='javascript' type='text/javascript'>var organizationId='" + sn.User.OrganizationId + "'; var userName = '" + sn.UserName + "'; var userID = '" + sn.UserID + "';</script>";
        //    ClientScript.RegisterClientScriptBlock(this.GetType(), "UserInfo", userNameScript);
        //}



        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                

                sn = (SentinelFMSession)Session["SentinelFMSession"];

                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    if (Page.Request.QueryString["sn"] != null)
                    {
                        SentinelFMSessionBase snb = SentinelFMSession.GetSessionBaseByKey((string)Page.Request.QueryString["sn"]);
                        sn = new SentinelFMSession(snb);
                        if (sn == null)
                            return;
                        Session["SentinelFMSession"] = sn;
                    }
                    else if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.Query != null && Page.Request.UrlReferrer.Query.StartsWith("?sn="))
                    {
                        string qs = Page.Request.UrlReferrer.Query.Substring(4);
                        SentinelFMSessionBase snb = SentinelFMSession.GetSessionBaseByKey(qs);
                        sn = new SentinelFMSession(snb);
                        if (sn == null)
                            return;
                        Session["SentinelFMSession"] = sn;
                    }
                    else
                    {
                        RedirectToLogin();
                        return;
                    }

                    sn.User.ExistingPreference(sn);
                    sn.User.GetGuiControlsInfo(sn);
                }

                if (!Page.IsPostBack)
                {
                    if (sn.User.OrganizationName == "" || sn.User.OrganizationName == null)
                    {
                        sn.User.ExistingPreference(sn);
                        sn.User.GetGuiControlsInfo(sn);
                    }
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];
                string userNameScript = "<script language='javascript' type='text/javascript'>var organizationId='" + sn.User.OrganizationId + "'; var userName = '" + sn.UserName + "'; var userID = '" + sn.UserID + "';</script>";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "UserInfo", userNameScript);

                ErrorMessage = GetLocalResourceObject("1_ErrorMessage").ToString();
                ErrorMessageToForward = GetLocalResourceObject("1_ErrorMessageToForward").ToString();
                Warning_DateAndTime = GetLocalResourceObject("1_WarningMessageDateAndTime").ToString();
                Warning_DateFromToDate = GetLocalResourceObject("1_WarningDateFromToDate").ToString();
                Notification_NoData = GetLocalResourceObject("1_NotificationNoData").ToString();    
            }
            catch
            {
                RedirectToLogin();
            }
        }

        private void RedirectToLogin()
        {
            int UserId = 0;
            string frmName = "";

            //Session Check
            try
            {
                SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
                if (snMain != null && snMain.UserID != null)
                    UserId = snMain.UserID;
                else
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
            }

            //Form Name
            try
            {
                frmName = Page.GetType().Name;
            }
            catch { }

            //Get Last Error if exists
            try
            {
                if (Server.GetLastError() != null)
                {
                    Exception ex = Server.GetLastError().GetBaseException();

                    string Excp = "SentinelFM " +
                        "MESSAGE: " + ex.Message +
                        "\nSOURCE: " + ex.Source +
                        "\nFORM: " + frmName +
                        "\nQUERYSTRING: " + Request.QueryString.ToString() +
                        "\nTARGETSITE: " + ex.TargetSite +
                        "\nSTACKTRACE: " + ex.StackTrace;

                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - UserId:" + UserId.ToString() + " , Form:" + frmName + " , Error :" + Excp));
                }
                else
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  UserId:" + UserId.ToString() + " , Form:" + frmName));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - not exception info,  Form:" + frmName));
            }

            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>window.open('../Login2.aspx','_top') </SCRIPT>");
            return;
        }
    }
}
