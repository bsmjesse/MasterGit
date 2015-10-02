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

namespace SentinelFM
{
    public partial class Configuration_CustomerReport_CustomerReportEmailBody : SentinelFMBasePage
    {
        public string Error_Load = "Failed to load data.";
        public string errorSave = "Save failed.";
        public string saveSucceed = "Saved Successfully.";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
            }
            if (!IsPostBack)
            {
                CustomReportEmailManager customMg = new CustomReportEmailManager(sConnectionString);
                string emails = string.Empty;
                DataSet ds = customMg.CustomReportEmail_Get(sn.User.OrganizationId, true);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["CustomReportEmailMessage"] != DBNull.Value)
                        emails = ds.Tables[0].Rows[0]["CustomReportEmailMessage"].ToString();
                }
                txtEmails.Content = emails;

            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CustomReportEmailManager CRmgr = new CustomReportEmailManager(sConnectionString);
                CRmgr.CustomReportEmailMessage_Add(txtEmails.Content.Trim(), sn.User.OrganizationId, sn.UserID);
                string errorMsg = string.Format("alert(\"{0}\");", saveSucceed);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }

        }
}
}