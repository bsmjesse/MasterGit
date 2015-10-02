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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


namespace SentinelFM
{

    public partial class Configuration_WorkingHour_frmWorkingHrsRpt : SentinelFMBasePage
    {
        public string selectFleet = "Select a Fleet";
        public string errorLoad = "Failed to load data.";
        public string noData = "There is no data.";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
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
        }

        private void FillFleets()
        {
            DataSet dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));
        }

        protected void btmOK_Click(object sender, EventArgs e)
        {
            try
            {
                List<WorrkingHoursReport> worrkingHoursReports = new List<WorrkingHoursReport>();

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

                WorkingHoursManager whMgr = new WorkingHoursManager(sConnectionString);
                DataSet ds = whMgr.FleetWorkingHours_Report(int.Parse(cboFleet.SelectedValue), dt);

                MCCManager mccMgr = new MCCManager(sConnectionString);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WorrkingHoursReport wh = new WorrkingHoursReport();
                    if (dr["Timestamp"] != DBNull.Value)
                    {
                        wh.Timestamp = dr["Timestamp"].ToString();
                    }
                    if (dr["Action"] != DBNull.Value)
                    {
                        wh.Action = dr["Action"].ToString();
                        if (wh.Action.ToLower() == "del") wh.Action = "Delete";
                        if (wh.Action.ToLower() == "upd") wh.Action = "Update";
                        if (wh.Action.ToLower() == "ins") wh.Action = "Insert";
                    }
                    if (dr["TimeZone"] != DBNull.Value)
                    {
                        wh.TimeZone = dr["TimeZone"].ToString();
                    }
                    string timeZoneID = "-1";
                    int dtcDt = 0;
                    if (dr["TimeZoneID"] != DBNull.Value)
                    {
                        timeZoneID = dr["TimeZoneID"].ToString();
                    }
                    if (timeZoneID != "-1")
                    {
                        //ContactManager contactMgr = new ContactManager(sConnectionString);
                        //string tzID = contactMgr.GetTimeZonesByID(timeZoneID.ToString());
                        TimeZoneInfo timeInfo = System.TimeZoneInfo.FindSystemTimeZoneById(timeZoneID);
                        dtcDt = (int)timeInfo.BaseUtcOffset.TotalMinutes;
                    }
                    if (dr["From"] != DBNull.Value && dr["From"].ToString() != "" )
                    {
                        int from = int.Parse(dr["From"].ToString());
                        from = from + dtcDt;
                        string hr = Math.Floor(from * 1.0 / 60).ToString();
                        if (hr.Length == 1) hr = "0" + hr;
                        string minute = (from % 60).ToString();
                        if (minute.Length == 1) minute = "0" + minute;
                        wh.from = hr + ":" + minute;
                    }

                    if (dr["To"] != DBNull.Value && dr["To"].ToString() != "" )
                    {
                        int to = int.Parse(dr["To"].ToString());
                        to = to + dtcDt;

                        string hr = Math.Floor(to * 1.0 / 60).ToString();
                        if (hr.Length == 1) hr = "0" + hr;
                        string minute = (to % 60).ToString();
                        if (minute.Length == 1) minute = "0" + minute;
                        wh.To = hr + ":" + minute;
                    }
                    if (dr["WorkingDays"] != DBNull.Value)
                    {
                        wh.WorkingDays = dr["WorkingDays"].ToString();
                    }
                    if (dr["VehicleName"] != DBNull.Value)
                    {
                        wh.VehicleName = dr["VehicleName"].ToString();
                    }
                    if (dr["UserName"] != DBNull.Value)
                    {
                        wh.UserName = dr["UserName"].ToString();
                    }
                    if (dr["Email"] != DBNull.Value)
                    {
                        wh.Email = dr["Email"].ToString();
                    }

                    worrkingHoursReports.Add(wh);
                }
                if (worrkingHoursReports.Count <= 0)
                {
                    string errorScript = string.Format("alert('{0}')", noData);
                    RadAjaxManager1.ResponseScripts.Add(errorScript);
                    return;
                }

                string repFilePath = Server.MapPath("WorkingHours.rpt");
                ReportDocument repDoc = new ReportDocument();
                repDoc.Load(repFilePath);
                CrystalDecisions.CrystalReports.Engine.TextObject
                                    txt = (CrystalDecisions.CrystalReports.Engine.TextObject)repDoc.ReportDefinition.ReportObjects["FleetName"];
                txt.Text = cboFleet.SelectedItem.Text;
                     
                //repDoc.DataDefinition.FormulaFields[]
                //repDoc.ReportDefinition.ReportObjects.ite.Item["Text12"] = _
         //"mySting info"
                repDoc.SetDataSource(worrkingHoursReports);
                string fileName = string.Format("tmp/wh{0:yyyy}{0:MM}{0:dd}{0:hh}{0:mm}{0:ss}.pdf", DateTime.Now);
                repDoc.ExportToDisk(ExportFormatType.PortableDocFormat, Server.MapPath(fileName));
                iframePdf.Visible = true;
                string url = Request.Url.ToString();
                url = url.Substring(0, Request.Url.ToString().LastIndexOf("/") + 1 ) + fileName  ;
                iframePdf.Attributes.Add("src", url);


                DirectoryInfo direcInfo = new DirectoryInfo(Server.MapPath("tmp/"));
                DateTime now = System.DateTime.Now.AddDays(-2);
                foreach(FileInfo pdfFile in direcInfo.GetFiles("wh*.pdf"))
                {
                    if (pdfFile.CreationTime < now) pdfFile.Delete();
                }
            }
            catch (Exception Ex)
            {
                
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                string errorScript = string.Format("alert('{0}')", errorLoad);
                RadAjaxManager1.ResponseScripts.Add(errorScript);
            }

        }

}
}