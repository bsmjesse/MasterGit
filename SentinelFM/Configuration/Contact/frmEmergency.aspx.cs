using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using VLF.DAS.DB;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace SentinelFM
{
    public partial class Configuration_Contact_frmEmergency :SentinelFMBasePage
    {
        string telephoneTxt = "Telephone";
        string emailTxt = "Email";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        ContactManager contactMsg = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

            try
            {
                if (!IsPostBack)
                {
                    contactMsg = new ContactManager(sConnectionString);
                    BindTelephones();
                    BindEmails();
                    legendPhones.InnerText = telephoneTxt;
                    legendEmails.InnerText = emailTxt;
                    if (Request.QueryString["CP"] != null)
                    {
                        lblDriverName.Text = contactMsg.GetDriverNameByDriverID(int.Parse(Request.QueryString["CP"].ToString()), sn.User.OrganizationId);
                    }
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }

        private void BindEmails()
        {
            if (Request.QueryString["CP"] != null)
            {
                gdEmails.DataSource = contactMsg.GetDriverEmergencyPhoneOrEmailByDriverID(int.Parse(Request.QueryString["CP"].ToString()), false, sn.User.OrganizationId);
                gdEmails.DataBind();
            }
        }

        private void BindTelephones()
        {
            if (Request.QueryString["CP"] != null)
            {
                gdPhones.DataSource = contactMsg.GetDriverEmergencyPhoneOrEmailByDriverID(int.Parse(Request.QueryString["CP"].ToString()), true, sn.User.OrganizationId);
                gdPhones.DataBind();
            }
        }

        protected void gdPhones_ItemDataBound(object sender, GridItemEventArgs e)
        {
            Label lblPriority = (Label)e.Item.FindControl("lblPriority");
            if (lblPriority != null)
            {
                lblPriority.Text = lblPriority.Text + " " + (e.Item.ItemIndex + 1).ToString();
            }

        }
}
}