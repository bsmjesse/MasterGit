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
using System.Data.SqlClient;


namespace SentinelFM
{

    public partial class Configuration_WorkingHour_frmWorkingHrsSuppressed : SentinelFMBasePage
    {
        public string selectFleet = "Select a Fleet";
        public string errorLoad = "Failed to load data.";
        public string noData = "There is no data.";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        public string resizeScript = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
                    return;
                }
                if (!IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    FillFleets();
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
            RadScriptManager1.AsyncPostBackTimeout = 300;

            dgSuppressedEmail.RadAjaxManagerControl = RadAjaxManager1;
            resizeScript = SetResizeScript(dgSuppressedEmail.ClientID);
        }

        private void FillFleets()
        {
            DataSet dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));
        }        protected void dgSuppressedEmail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BinddgSuppressedEmai_NewTZ(false);
        }

        // Changes for TimeZone Feature start
        void BinddgSuppressedEmai_NewTZ(Boolean isBound)
        {
            try
            {
                List<WorrkingHoursReport> worrkingHoursReports = new List<WorrkingHoursReport>();
                int fleetID = int.Parse(cboFleet.SelectedValue);
                DateTime dt = System.DateTime.Now.Date;

                if (ddlDateTime.SelectedValue == "1") dt = dt.AddDays(-7);
                if (ddlDateTime.SelectedValue == "2") dt = dt.AddDays(-14);
                if (ddlDateTime.SelectedValue == "3") dt = dt.AddMonths(-1);
                if (ddlDateTime.SelectedValue == "4") dt = dt.AddMonths(-2);
                if (ddlDateTime.SelectedValue == "5") dt = dt.AddMonths(-3);
                if (ddlDateTime.SelectedValue == "6") dt = dt.AddMonths(-6);
                if (ddlDateTime.SelectedValue == "7") dt = dt.AddMonths(-12);
                if (ddlDateTime.SelectedValue == "8") dt = dt.AddYears(-2);
                if (ddlDateTime.SelectedValue == "9") dt = new DateTime(1970, 1, 1);

                string hosConnectionString =
                   ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                DataSet dataSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(hosConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();

                    adapter.SelectCommand = new SqlCommand();
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapter.SelectCommand.CommandText = "sp_vlfAfterHoursAlertGet";
                    adapter.SelectCommand.Connection = connection;

                    SqlParameter sqlPara = new SqlParameter("@OrganizationID", SqlDbType.Int);
                    sqlPara.Value = sn.User.OrganizationId;
                    adapter.SelectCommand.Parameters.Add(sqlPara);

                    sqlPara = new SqlParameter("@FleetID", SqlDbType.Int);
                    sqlPara.Value = fleetID;
                    adapter.SelectCommand.Parameters.Add(sqlPara);

                    sqlPara = new SqlParameter("@OriginDateTime1", SqlDbType.DateTime);
                    sqlPara.Value = dt.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving);
                    adapter.SelectCommand.Parameters.Add(sqlPara);

                    sqlPara = new SqlParameter("@OriginDateTime2", SqlDbType.DateTime);
                    sqlPara.Value = System.DateTime.UtcNow;
                    adapter.SelectCommand.Parameters.Add(sqlPara);

                    adapter.Fill(dataSet);
                    dgSuppressedEmail.DataSource = dataSet;
                    if (isBound) dgSuppressedEmail.DataBind();
                }
            }
            catch (Exception Ex)
            {

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorScript = string.Format("alert('{0}')", "Please select a fleet.");
                RadAjaxManager1.ResponseScripts.Add(errorScript);
            }
        }
        // Changes for TimeZone Feature end

        protected void btmOK_Click(object sender, EventArgs e)
        {
            if (cboFleet.SelectedIndex <= 0)
            {
                string errorScript = string.Format("alert('{0}')", "Please select a fleet.");
                RadAjaxManager1.ResponseScripts.Add(errorScript);

            }

            BinddgSuppressedEmai_NewTZ(true);

        }
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
        }

    }
}