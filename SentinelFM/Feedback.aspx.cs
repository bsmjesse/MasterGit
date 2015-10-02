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
using VLF.MAP;
using VLF.CLS.Interfaces;
using Ext.Net;
using VLF.DAS.Logic;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using VLF.MailLib;
using System.Configuration;

namespace SentinelFM
{
    public partial class Feedback : System.Web.UI.Page
    {
           protected SentinelFMSession sn = null;
        
        public string _xml = "";
        protected clsUtility objUtil;

        protected void Page_Load(object sender, EventArgs e)
        {
             try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"]; 
                    string email = Request.Form["email"];
                    if (!string.IsNullOrEmpty(email))
                    {
                        string lastname = Request.Form["lastname"];
                        string firstname = Request.Form["firstname"];
                        string feedback = Request.Form["comments"];                        
                        string result=string.Empty;
                        string emailTo = "mvaksman@bsmwireless.com;jwitt@bsmwireless.com;tramlogan@bsmwireless.com;sali@bsmwireless.com;ahoxha@bsmwireless.com;mjung@bsmwireless.com;mnancharla@bsmwireless.com;";
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        try
                        {
                            if (!string.IsNullOrEmpty(lastname)
                                && !string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(feedback))
                            {
                                EMailMessage em = new EMailMessage(ConfigurationManager.AppSettings["SMTPServer"]);
                                em.From = email;
                                em.SendMail(emailTo, "Map feedback received from " + firstname + " " + lastname + " Organization: " + sn.User.OrganizationName, feedback);                                
                                //SendEmail(email, "Map feedback received from " + firstname + " " + lastname + " Organization: " + sn.User.OrganizationName, feedback);                                
                            }
                            result = serializer.Serialize(true);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            result = serializer.Serialize("Error in submitting feedback.. Please contact BSM Support...");
                        }                                               
                        Response.Write(result);
                        Response.End();
                    }                   
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }
        }



        public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
        }
        
    }
}