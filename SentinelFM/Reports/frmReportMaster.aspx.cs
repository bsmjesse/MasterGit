#region Namespace Reference section

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;

using VLF.ERRSecurity;
using VLF.Reports;
using VLF.CLS;
using VLF.PATCH.Logic;

#endregion

namespace SentinelFM
{
    public partial class Reports_frmReportMaster : SentinelFMBasePage
    {
        //
        //protected System.Web.UI.WebControls.CheckBox chkHistIncludeInvalid;
        //protected System.Web.UI.WebControls.DropDownList cboFromHoursSOS;
        //protected System.Web.UI.WebControls.DropDownList cboToHoursSOS;

        public bool ShowOrganizationHierarchy;
        public string OrganizationHierarchyPath = "";
        private string CurrentUICulture = "en-US";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn.Report.ReportActiveTab = 0;

                HttpCookie aCookie = new HttpCookie(sn.User.OrganizationId.ToString() + "SnReportActiveTab");
                aCookie.Value = "0";
                aCookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(aCookie);

                string datetime = "";
           
                try
                {
                    if (Request[this.txtFrom.UniqueID] != null)
                    {
                        //this.txtFrom.Text = Request[this.txtFrom.UniqueID];
                        datetime = Convert.ToDateTime(Request[this.txtFrom.UniqueID].ToString()).ToString(sn.User.DateFormat);
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                    }

                    if (Request[this.txtTo.UniqueID] != null)
                    {
                        //this.txtTo.Text = Request[this.txtTo.UniqueID];
                        datetime = Convert.ToDateTime(Request[this.txtTo.UniqueID].ToString()).ToString(sn.User.DateFormat);
                        this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);                        
                    }
                }
                catch
                {
                    if (!String.IsNullOrEmpty(sn.Report.FromDate))
                    {
                        datetime = Convert.ToDateTime(sn.Report.FromDate).ToString(sn.User.DateFormat);
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                        //this.txtFrom.Text = Convert.ToDateTime(sn.Report.FromDate).ToString("MM/dd/yyyy");
                    }
                    if (!String.IsNullOrEmpty(sn.Report.ToDate))
                    {
                        datetime = Convert.ToDateTime(sn.Report.ToDate).ToString(sn.User.DateFormat);
                        this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                        //this.txtTo.Text = Convert.ToDateTime(sn.Report.ToDate).ToString("MM/dd/yyyy");
                    }
                }

                CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture.ToString();

                txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

                txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;


                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                {
                    //txtFrom.CultureInfo.CultureName = "fr-FR";
                    txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                    txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                    txtFrom.DateInput.DisplayDateFormat = sn.User.DateFormat;
                    txtFrom.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy";

                    //txtTo.CultureInfo.CultureName = "fr-FR";
                    txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                    txtTo.DateInput.DateFormat = sn.User.DateFormat;
                    txtTo.DateInput.DisplayDateFormat = sn.User.DateFormat;
                    txtTo.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy";
                }
                else
                {
                    //txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                    txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                    txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                    txtFrom.DateInput.DisplayDateFormat = sn.User.DateFormat;

                    //txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                    txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                    txtTo.DateInput.DateFormat = sn.User.DateFormat;
                    txtTo.DateInput.DisplayDateFormat = sn.User.DateFormat;
                }

                //Show Busy Message
                cmdShow.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
                cmdPreviewFleetMaintenanceReport.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
                this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
                this.BusyReport.Text = (string)base.GetLocalResourceObject("BusyPreparingMessage");

                #region Organization Hierarchy Initial Section

                ShowOrganizationHierarchy = false;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                }

                if (ShowOrganizationHierarchy)
                {

                    clsUtility objUtil;
                    objUtil = new clsUtility(sn);
                    ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                    string xml = "";
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    {
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                        {

                        }
                    }

                    StringReader strrXML = new StringReader(xml);
                    DataSet dsPref = new DataSet();
                    dsPref.ReadXml(strrXML);

                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {

                        if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                        {
                            string nodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                            OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                        }
                    }

                    ReportBasedOption();
                }
                else
                {
                    this.organizationHierarchy.Visible = false;
                    this.vehicleSelectOption.Visible = false;
                    this.trFleet.Visible = true;
                }

                #endregion

                if (!Page.IsPostBack)
                {
                    if (ShowOrganizationHierarchy)
                        this.optReportBased.SelectedIndex = 0;

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmReportMaster, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    //GuiSecurity(this);
                    GetUserReportsTypes();

                    if (sn.User.UnitOfMes == 1)
                    {
                        this.lblSpeed120.Text = this.lblSpeed120.Text + "120";
                        this.lblSpeed130.Text = this.lblSpeed130.Text + "130";
                        this.lblSpeed140.Text = this.lblSpeed140.Text + "140";
                    }
                    else
                    {
                        this.lblSpeed120.Text = this.lblSpeed120.Text + "75";
                        this.lblSpeed130.Text = this.lblSpeed130.Text + "80";
                        this.lblSpeed140.Text = this.lblSpeed140.Text + "85";
                    }

                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);
                    clsMisc.cboHoursFill(ref cboFromDayH);
                    clsMisc.cboHoursFill(ref cboToDayH);
                    clsMisc.cboHoursFill(ref cboWeekEndFromH);
                    clsMisc.cboHoursFill(ref cboWeekEndToH);

                    //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                    //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;

                    if (sn.Report.FromDate != "")
                    {
                        //this.txtFrom.Text = Convert.ToDateTime(sn.Report.FromDate).ToString("MM/dd/yyyy");
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(Convert.ToDateTime(sn.Report.FromDate).ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
                    }
                    else
                    {
                        //this.txtFrom.Text = DateTime.Now.ToString("MM/dd/yyyy");
                        //this.txtFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(DateTime.Now.ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
                    }


                    if (sn.Report.ToDate != "")
                    {
                        //this.txtTo.Text = Convert.ToDateTime(sn.Report.ToDate).ToString("MM/dd/yyyy");
                        this.txtTo.SelectedDate = ConvertStringToDateTime(Convert.ToDateTime(sn.Report.ToDate).ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
                    }
                    else
                    {
                        //this.txtTo.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                        this.txtTo.SelectedDate = ConvertStringToDateTime(DateTime.Now.AddDays(1).ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
                    }

                    //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy hh:mm");
                    //this.txtTo.Text=DateTime.Now.AddDays(1).ToString("MM/dd/yyyy hh:mm");
                    //this.cboHoursFrom.SelectedIndex = -1;
                    //for (i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                    //{
                    //   if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == 8)
                    //   {
                    //      cboHoursFrom.Items[i].Selected = true;
                    //      break;
                    //   }
                    //}

                    this.cboHoursFrom.SelectedIndex = 0;
                    this.cboHoursTo.SelectedIndex = 0;

                    //this.cboHoursTo.SelectedIndex = -1;
                    //for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                    //{
                    //   if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == Convert.ToInt32(DateTime.Now.AddHours(1).Hour.ToString()))
                    //   {
                    //      this.cboHoursTo.SelectedIndex = i;
                    //      break;
                    //   }
                    //}

                    this.cboFromDayH.SelectedIndex = 8;
                    this.cboToDayH.SelectedIndex = 18;
                    this.cboWeekEndFromH.SelectedIndex = 8;
                    this.cboWeekEndToH.SelectedIndex = 18;

                    #region Fleet list initial section

                    CboFleet_Fill();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

                    if ((sn.Report.FleetId != 0) && (sn.Report.FleetId != -1))
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Report.FleetId.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.Report.FleetId));
                        if (sn.Report.LicensePlate != "")
                            cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.LicensePlate.ToString()));

                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else
                    {
                        this.lblVehicleName.Visible = false;
                        this.cboVehicle.Visible = false;
                    }

                    #endregion

                    //remove report
                    string strViolationSpeedRoad = "18,489,287,628,254,368,664,343,999647,951,999630,480,622,999756";
                    string[] tmp = strViolationSpeedRoad.Split(',');
                    bool removeReport = true;

                    for (int y = 0; y < tmp.Length; y++)
                    {
                        if (sn.User.OrganizationId.ToString() == tmp[y].ToString())
                        {
                            removeReport = false;
                            break;
                        }
                    }

                    if (removeReport)
                    {
                        for (int x = 0; x < sn.Report.UserReportsDataSet.Tables[0].Rows.Count; x++)
                        {
                            if (sn.Report.UserReportsDataSet.Tables[0].Rows[x]["GuiId"].ToString() == "104")
                            {
                                sn.Report.UserReportsDataSet.Tables[0].Rows[x].Delete();
                                this.cboReports.DataSource = sn.Report.UserReportsDataSet;
                                this.cboReports.DataBind();
                                break;
                            }
                        }
                    }

                    //end remove report

                    //cboReports.Items.Add(new ListItem("Geozone Report", "22"));
                    //cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                    //cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
                    //cboReports.Items.Add(new ListItem("Trip Summary Totals Report per Vehicle", "50"));
                    //cboReports.Items.Add(new ListItem("Trip Summary Totals Report per Organization", "51"));
                    //cboReports.Items.Add(new ListItem("HOS Details Report per Driver", "53"));

                    if (sn.User.UserGroupId == 1)
                    {
                        //cboReports.Items.Add(new ListItem("BSM-Vehicle Information Data Dump", "54"));
                        AddReportItem(new ListItem("BSM-Vehicle Information Data Dump", "54"));
                    }

                    //Devin Aecon Start
                    if (sn.User.OrganizationId == 480)
                        AddReportItem(new ListItem("Dispatch Report", "300"));


                    //cboReports.Items.Add(new ListItem("Transportation Mileage Report", "63"));
                    //cboReports.Items.Add(new ListItem("Idling Details Report New", "87"));
                    //cboReports.Items.Add(new ListItem("Idling Summary Report New", "88"));
                    //cboReports.Items.Add(new ListItem("Trip Summary Report New", "89"));

                    if (Request.Cookies[sn.User.OrganizationId.ToString() + "ReportMasterSelectedIndex"] != null && Request.Cookies[sn.User.OrganizationId.ToString() + "ReportMasterSelectedIndex"].Value.Trim() != "")
                    {
                        try
                        {
                            cboReports.SelectedIndex = int.Parse(Request.Cookies[sn.User.OrganizationId.ToString() + "ReportMasterSelectedIndex"].Value);
                        }
                        catch
                        {
                            cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
                        }
                    }
                    else
                    {
                        cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
                    }

                    ReportCriteria();
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
        }

        private void AddReportItem(ListItem NewItem)
        {
            Boolean hasAdd = false;
            for (int index = 0; index < cboReports.Items.Count; index++)
            {
                if (cboReports.Items[index].Text.CompareTo(NewItem.Text) > 0)
                {
                    cboReports.Items.Insert(index, NewItem);
                    hasAdd = true;
                    break;
                }
            }
            if (!hasAdd)
            {
                cboReports.Items.Add(NewItem);
            }
        }

        private void DeleteReportItem(ListItem NewItem)
        {
            Boolean hasDelete = false;
            for (int index = 0; index < cboReports.Items.Count; index++)
            {
                if (cboReports.Items[index].Text.CompareTo(NewItem.Text) > 0)
                {

                    hasDelete = true;
                    break;
                }
            }
            if (hasDelete)
            {
                cboReports.Items.Remove(NewItem);
            }
        }

        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));

                this.cboMaintenanceFleet.DataSource = dsFleets;
                this.cboMaintenanceFleet.DataBind();
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
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                cboVehicle.Items.Clear();

                DataSet dsVehicle = new DataSet();

                string xml = "";

                using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
                {
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                    return;
                }

                dsVehicle.ReadXml(new StringReader(xml));

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));

                if ((sn.Report.VehicleId != 0) && (sn.Report.VehicleId != -1))
                    cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.VehicleId.ToString()));


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
        }

        protected void cmdShow_Click(object sender, System.EventArgs e)
        {

            CreateReportParams("frmReportViewer.aspx");

        }

        private void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.lblMessage.Text = "";
        }

        protected void cboReports_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
				HttpCookie aCookie = new HttpCookie(sn.User.OrganizationId.ToString() + "ReportMasterSelectedIndex");
                aCookie.Value = cboReports.SelectedIndex.ToString();
                aCookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(aCookie);

                ReportCriteria();

                if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "10013" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
                {
                    this.organizationHierarchy.Visible = false;
                    this.vehicleSelectOption.Visible = false;
                    this.trFleet.Visible = true;
                }
                //User Logins Report and Hours of Service  Audit Report
                //if (Convert.ToInt32(cboReports.SelectedItem.Value) == 60 || Convert.ToInt32(cboReports.SelectedItem.Value) == 103)
                if (Convert.ToInt32(cboReports.SelectedItem.Value) == 60)
                {
                    this.cmdSchedule.Enabled = false;
                }
                else
                {
                    this.cmdSchedule.Enabled = true;
                }

                this.cmdViewScheduled.Enabled = true;
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
        }

        /// <summary>
        /// Show/hide controls according to selected report
        /// </summary>
        private void ReportCriteria()
        {
            /*
            this.lblTripSummaryReportDesc.Visible = false;
            this.lblHistoryReportDesc.Visible = false;
            this.lblAlarmReportDesc.Visible = false;
            this.lblStopReportDesc.Visible = false;
            this.lblTripReportDesc.Visible = false;
            this.lblLandmarkActivityReportDesc.Visible = false;
            this.lblMessageReportDescription.Visible = false;
            this.lblOffHoursReportDesc.Visible = false;
            this.lblIdlingDetailsReportDesc.Visible = false;
            this.lblIdlingSummaryReportDesc.Visible = false;
            this.lblFleetViolationDetailsReportDesc.Visible = false;
            this.lblFleetViolationSummaryReportDesc.Visible = false;
            this.lblFleetMaintenanceReportDesc.Visible = false;
            */
            this.tblHistoryOptions.Visible = false;
            this.tblException.Visible = false;
            this.tblException1.Visible = false;
            this.tblOptions1.Visible = false;
            this.tblOptions2.Visible = false;
            this.tblStopReport.Visible = false;
            this.tblOffHours.Visible = false;
            this.tblFleetMaintenance.Visible = false;
            this.tblViolationReport.Visible = false;
            this.chkShowStorePosition.Visible = false;
            this.tblGeneralCriteria.Visible = true;
            this.cboVehicle.Enabled = true;
            FleetVehicleShow(true);
            this.tblPoints.Visible = false;
            this.tblIgnition.Visible = false;
            this.cboViolationSpeed.Visible = false;
            this.tblLandmarkOptions.Visible = false;
            this.tblGeozoneOptions.Visible = false;
            this.tblDriverOptions.Visible = false;
            this.lblReportFormat.Text = base.GetResource("ReportFormatSuggested_Portrait");
            this.txtTo.Enabled = true;
            this.txtFrom.Enabled = true;
            this.cboHoursFrom.Enabled = true;
            this.cboHoursTo.Enabled = true;
            this.chkShowDriver.Visible = false;
            tblRoadSpeed.Visible = false;
            optBaseTable.Visible = true;
            // build descr. name
            this.LabelReportDescription.Text = "";
            //trFleet.Visible = true;
            string resourceDescriptionName = "";
            string filter = String.Format("GuiId = '{0}'", this.cboReports.SelectedValue);
            DataRow[] rowsReport = null;
            if (Util.IsDataSetValid(sn.Report.UserReportsDataSet))
            {
                rowsReport = sn.Report.UserReportsDataSet.Tables[0].Select(filter);
                if (rowsReport != null && rowsReport.Length > 0)
                {
                    resourceDescriptionName = String.Format("Description_{0}", rowsReport[0]["ReportTypesName"]);
                    this.LabelReportDescription.Text = base.GetResource(resourceDescriptionName);
                    if (String.IsNullOrEmpty(this.LabelReportDescription.Text))
                        this.LabelReportDescription.Text = rowsReport[0]["GuiName"].ToString();
                }
            }

            //Devin Aecon Start
            cboFormat.Enabled = true;
            trDispatch.Visible = false;
            //End

            //Mantis# 2553. Time List/Combo is disabled for all Server Reports.
            int val_default;
            int.TryParse(this.cboReports.SelectedValue, out val_default);

            //Enable time selection for SSRS Reports.
            //cboHoursFrom.Enabled = cboHoursTo.Enabled = (val_default > 10000 ? false : true);

            //Devin added on 2014.01.27
            tbAuditReport.Visible = false;

            switch (this.cboReports.SelectedValue)
            {
                case "0": // Trip Details Report
                    this.tblOptions1.Visible = true;
                    this.tblOptions2.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.lblTripReportDesc.Visible = true;
                    this.tblIgnition.Visible = true;
                    break;

                case "1": // Trip Summary Report
                    this.lblTripSummaryReportDesc.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.tblIgnition.Visible = true;
                    break;

                case "2": // Alarms Report
                    this.lblAlarmReportDesc.Visible = true;
                    break;

                case "3": // History Report
                    this.tblHistoryOptions.Visible = true;
                    this.lblHistoryReportDesc.Visible = true;
                    HideHierarchy();
                    this.cboFleet.Enabled = true;
                    break;

                case "4": // Stop Report
                case "10062":
                    this.chkShowStorePosition.Visible = true;
                    this.tblStopReport.Visible = true;
                    this.lblStopReportDesc.Visible = true;
                    tblIgnition.Visible = true;
                    break;

                case "5": // Messages Report
                    this.lblMessageReportDescription.Visible = true;
                    //this.lblReportFormat.Visible = true;
                    break;

                case "6": // Exceptions Report
                    this.tblException.Visible = true;
                    this.tblException1.Visible = true;
                    break;

                case "8": // Off Hours Report
                    this.tblHistoryOptions.Visible = true;
                    this.tblOffHours.Visible = true;
                    this.lblOffHoursReportDesc.Visible = true;
                    break;

                case "9": // Landmark Activity Report
                    this.lblLandmarkActivityReportDesc.Visible = true;
                    break;

                case "10": // Fleet Maintenance Report
                    this.tblGeneralCriteria.Visible = false;
                    this.tblFleetMaintenance.Visible = true;
                    this.lblFleetMaintenanceReportDesc.Visible = true;
                    break;

                case "11": // idling details
                case "12":
                case "13":
                case "14":
                case "15":
                case "16":
                case "18":
                    this.cboVehicle.Enabled = false;
                    this.lblIdlingDetailsReportDesc.Visible = true;
                    break;

                case "17": // violation details 4 fleet
                // case "10013":
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.cboViolationSpeed.Visible = true;
                    break;

                case "19": // idling summary
                case "88":
                case "10017":
                    FleetVehicleShow(false);
                    this.lblIdlingSummaryReportDesc.Visible = true;
                    break;

                case "20": // violation summary 4 fleet
                //case "10014":   // violation summary 4 fleet
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    break;

                case "21": // landmark summary
                case "10067": 
                    this.tblLandmarkOptions.Visible = true;
                    // load landmarks
                    LoadLandmarks();
                    break;

                case "22": // geozone
                case "30":
                    this.tblGeozoneOptions.Visible = true;
                    // load geozone list
                    LoadGeozones();
                    HideHierarchy();
                    this.cboFleet.Enabled = false;
                    break;

                case "23": // landmark details
                case "10066": 
                    this.tblLandmarkOptions.Visible = true;
                    LoadLandmarks();
                    break;

                case "24": // inactivity
                    this.tblIgnition.Visible = true;
                    this.lblReportFormat.Text = base.GetResource("ReportFormatSuggested_Landscape");
                    break;

                case "25": // driver trip details
                    this.tblOptions1.Visible = true;
                    this.tblOptions2.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.lblTripReportDesc.Visible = true;
                    this.tblIgnition.Visible = true;
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(false);
                    break;

                case "26": // driver trip summary
                    this.lblTripSummaryReportDesc.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.tblIgnition.Visible = true;
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(false);
                    break;

                case "27": // driver violation details
                    this.tblViolationReport.Visible = true;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.cboViolationSpeed.Visible = true;
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(false);
                    break;

                case "28": // violation summary 4 fleet
                    this.tblViolationReport.Visible = true;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(false);
                    break;
                case "29":
                    FleetVehicleShow(false);
                    break;

                case "36":
                //case "10023":
                //case "10039":
                //case "10041":
                //case "10078":
                //case "10079":
                    this.cboVehicle.Enabled = false;
                    break;

                case "38":
                case "51":
                case "10011":
                    FleetVehicleShow(false);
                    this.tblIgnition.Visible = true;
                    break;

                case "39":
                case "50":
                case "62":
                case "87":
                //case "10018":
                //case "10002":
                //case "10035":
                    this.cboVehicle.Enabled = false;
                    this.tblIgnition.Visible = true;
                    break;
                case "40":
                case "63":
                case "10020":
                    this.tblIgnition.Visible = true;
                    break;

                case "41":
                case "10073":
                    this.tblLandmarkOptions.Visible = true;
                    LoadLandmarks();
                    break;

                case "53":
                    FleetVehicleShow(false);
                    tblIgnition.Visible = false;
                    this.chkShowStorePosition.Visible = false;
                    this.tblDriverOptions.Visible = true;
                    this.cmdSchedule.Visible = false;
                    this.cmdViewScheduled.Visible = false;
                    LoadDrivers();
                    break;

                case "54":
                case "58":
                    FleetVehicleShow(false);
                    this.txtTo.Enabled = false;
                    this.txtFrom.Enabled = false;
                    this.cboHoursFrom.Enabled = false;
                    this.cboHoursTo.Enabled = false;
                    break;

                case "59":
                case "61":
                case "70":
                    this.cboVehicle.Enabled = false;
                    break;

                case "60":
                    FleetVehicleShow(false);
                    tblIgnition.Visible = false;
                    break;

                case "89":
                    chkShowDriver.Visible = true;
                    break;

                case "97":
                //case "10019":
                    this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = false;
                    lblRoadSpeedDelta.Visible = false;
                    break;

                //Devin chganged for HOS Audit report 2014.01.27 
                case "103":
                    
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(true);
                    tbAuditReport.Visible = true;
                    break;

                case "104": // ROad Speed Violation
                //case "10015":   // ROad Speed Violation
                    this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = true;
                    lblRoadSpeedDelta.Visible = true;
                    break;


                case "105": // Garmin Message
                    HideHierarchy();
                    break;

                //Devin Aecon
                case "300":
                    //chkShowDriver.Visible = true;
                    cboFormat.Enabled = false;
                    trDispatch.Visible = true;
                    radDispatch.SelectedIndex = 0;
                    FleetVehicleShow(false);
                    break;

                case "10002":
                case "10018":
                case "10035":
                    //this.cboVehicle.Enabled = false;
                    this.tblIgnition.Visible = true;
                    break;

                case "10013":   // Idling Detail Report
                    //this.cboVehicle.Enabled = false;
                    this.tblViolationReport.Visible = true;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.cboViolationSpeed.Visible = true;
                    break;

                case "10014":   // violation summary 4 fleet
                    // this.cboVehicle.Enabled = false;
                    this.tblViolationReport.Visible = true;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    break;

                case "10015":   // Road Speed Violation
                    //this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = true;
                    lblRoadSpeedDelta.Visible = true;
                    break;

                case "10019":
                    //this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = false;
                    lblRoadSpeedDelta.Visible = false;
                    break;

                case "10023":
                case "10039":
                case "10041":
                case "10078":
                case "10079":
                case "10040":   //Province/State Mileage Report for Vehilce
                    break;

                case "10024":
                case "10030":
                    this.cboVehicle.Visible = false;
                    FleetVehicleShow(false);
                    this.txtTo.Enabled = false;
                    this.txtFrom.Enabled = false;
                    this.cboHoursFrom.Enabled = false;
                    this.cboHoursTo.Enabled = false;
                    this.vehicleSelectOption.Visible = false;
                    break;

                case "10095":   // AOBR AirMiles Report                   
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();

                    break;
            }

            if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "10013" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
            {
                this.organizationHierarchy.Visible = false;
                this.vehicleSelectOption.Visible = false;
                this.trFleet.Visible = true;
            }
        }

        /// <summary>
        /// Load drivers into ddl
        /// </summary>
        private void LoadDrivers()
        {
            DataSet dsDrivers = new DataSet();
            string xmlResult = "";

            using (ServerDBDriver.DBDriver drv = new global::SentinelFM.ServerDBDriver.DBDriver())
            {
                if (objUtil.ErrCheck(drv.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                    if (objUtil.ErrCheck(drv.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //RedirectToLogin();
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xmlResult))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }

            dsDrivers.ReadXml(new StringReader(xmlResult));

            this.ddlDrivers.Items.Clear();

            if (Util.IsDataSetValid(dsDrivers))
            {
                this.ddlDrivers.DataSource = dsDrivers;
                this.ddlDrivers.DataBind();
                this.ddlDrivers.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlDrivers_Item_0").ToString(), "-1"));
            }
            else
                this.ddlDrivers.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlDrivers_NoAvailable").ToString(), "-100"));

            if ((sn.Report.DriverId != 0) && (sn.Report.DriverId != -1))
                ddlDrivers.SelectedIndex = ddlDrivers.Items.IndexOf(ddlDrivers.Items.FindByValue(sn.Report.DriverId.ToString()));

        }

        /// <summary>
        /// Show or hide fleet / vehicle lables and ddl
        /// </summary>
        /// <param name="showControls"></param>
        private void FleetVehicleShow(bool showControls)
        {
            this.cboFleet.Visible = showControls;
            this.lblFleet.Visible = showControls;
            this.lblVehicleName.Visible = showControls;
            this.cboVehicle.Visible = showControls;

            if (ShowOrganizationHierarchy)
            {
                ReportBasedOption();
                this.vehicleSelectOption.Visible = showControls;
                this.organizationHierarchy.Visible = showControls;
            }
            //string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            //VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            //if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
            //    this.organizationHierarchy.Visible = showControls;
            //else
            //{
            //    this.organizationHierarchy.Visible = false;
            //    this.vehicleSelectOption.Visible = false;
            //}    
        }

        /// <summary>
        /// Load geozones into ddl
        /// </summary>
        private void LoadGeozones()
        {
            DataSet dsGeozones = new DataSet();
            string xmlResult = "";

            using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
            {

                if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                    if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //RedirectToLogin();
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xmlResult))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }

            dsGeozones.ReadXml(new StringReader(xmlResult));

            this.ddlGeozones.Items.Clear();

            if (Util.IsDataSetValid(dsGeozones))
            {
                DataView view = dsGeozones.Tables[0].DefaultView;
                view.Sort = "GeozoneName";
                this.ddlGeozones.DataSource = view;
                this.ddlGeozones.DataBind();
                this.ddlGeozones.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlGeozones_Item_0").ToString(), "-1"));
            }
            else
                this.ddlGeozones.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlGeozones_NoAvailable").ToString(), "-100"));
        }

        /// <summary>
        /// Load landmarks into ddl
        /// </summary>
        private void LoadLandmarks()
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = "";

            //using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
            //{

            //if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
            //    if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //        //RedirectToLogin();
            //        return;
            //    }
            //}

            //if (String.IsNullOrEmpty(xmlResult))
            //{
            //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //return;
            //}

            //dsLandmarks.ReadXml(new StringReader(xmlResult));

            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
            dsLandmarks = org.GetOrganizationLandmark_Public(sn.UserID, sn.User.OrganizationId, false);

            this.ddlLandmarks.Items.Clear();

            if (Util.IsDataSetValid(dsLandmarks))
            {
                this.ddlLandmarks.DataSource = dsLandmarks;
                this.ddlLandmarks.DataBind();
                this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_Item_0").ToString(), "-1"));
            }
            else
                this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_NoAvailable").ToString(), "-100"));


            if ((sn.Report.LandmarkName != "") && (sn.Report.LandmarkName != " -1"))
                ddlLandmarks.SelectedIndex = ddlLandmarks.Items.IndexOf(ddlLandmarks.Items.FindByValue(sn.Report.LandmarkName.ToString()));

        }

        private void chkHistIncludeInvalidGPS_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkHistIncludeInvalidGPS.Checked)
            {
                this.chkHistIncludeCoordinate.Checked = true;
                this.chkHistIncludeCoordinate.Enabled = false;
            }
            else
            {
                this.chkHistIncludeCoordinate.Enabled = true;
            }
        }

        /// <summary>
        /// Get user reports dataset from session, if not valid - use web method
        /// </summary>
        private void GetUserReportsTypes()
        {
            string xml = "";

            //if (Util.IsDataSetValid(sn.Report.UserReportsDataSet))
            //{
            //   this.cboReports.DataSource = sn.Report.UserReportsDataSet;
            //}
            //else
            //{
            DataSet dsReports = new DataSet();

            using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
            {
                if (objUtil.ErrCheck(dbs.GetUserReportsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetUserReportsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
            }

            if (String.IsNullOrEmpty(xml))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }


           ////// if (sn.User.OrganizationId != 1000041)      // 	Milton
  ////         // {

  ////              xml = xml.Replace("17", "10013");       // Fleet Violation Details Report
  ////              xml = xml.Replace("20", "10014");       // Fleet Violation Summary Report
  ////              xml = xml.Replace("104", "10015");      // Speed Violation Details Report by Road Speed
  ////              xml = xml.Replace("97", "10019");       // Speed Violation Summary Report by Road Speed
  ////              xml = xml.Replace("34", "10040");           // Prov/state Mileage Detail for Vehicle
  ////              xml = xml.Replace("36", "10041");           // Prov/state Mileage Summary for Fleet

  ////              if (sn.User.OrganizationId != 952)      // 	G4S
  ////              {
  ////                  xml = xml.Replace("39", "10002");   // Activity Summary Report per Vehicle
  ////                  xml = xml.Replace("38", "10011");   // Activity Summary Report for Organization
  ////                  xml = xml.Replace("88", "10017");   // Idling Summary Report
  ////                  xml = xml.Replace("87", "10018");   // Idling Detail Report
  ////              }
  ////              // Van Houtte Coffee Services Inc
  ////              if (sn.User.OrganizationId == 999695)   // VH
  ////              {
  ////                  xml = xml.Replace("10002", "10035");        // Activity Summary Report per Vehicle
  ////              }

  ////              if (sn.User.OrganizationId == 999630)   // MTS
  ////              {
  ////                  xml = xml.Replace("87", "10018");   // Idling Detail Report
  ////              }

  ////              //Exclude MTS and HQ
  ////              if (sn.User.OrganizationId != 999630 && sn.User.OrganizationId != 999956)
  ////              {
  ////                  xml = xml.Replace("63", "10020");       // IVMR
  ////              }

  ////          //}


            dsReports.ReadXml(new StringReader(xml));
            //dsReports.Tables[0].DefaultView.Sort = "GUIName";   
            this.cboReports.DataSource = dsReports; //dsReports.Tables[0].DefaultView;
            sn.Report.UserReportsDataSet = dsReports;


            //}

            this.cboReports.DataBind();

            //if (sn.User.UserGroupId != 1)
            //{
            //    cboReports.Items.Add(new ListItem("Activity Summary Report for Organization", "38"));
            //    cboReports.Items.Add(new ListItem("Activity Summary Report per Vehicle", "39"));
            //}

        }

        protected void chkWeekend_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cboWeekEndToH.SelectedIndex = 0;
            this.cboWeekEndFromH.SelectedIndex = 0;

            if (chkWeekend.Checked)
            {
                this.cboWeekEndToM.Enabled = false;
                this.cboWeekEndToH.Enabled = false;
                this.cboWeekEndFromM.Enabled = false;
                this.cboWeekEndFromH.Enabled = false;
            }
            else
            {
                this.cboWeekEndToM.Enabled = true;
                this.cboWeekEndToH.Enabled = true;
                this.cboWeekEndFromM.Enabled = true;
                this.cboWeekEndFromH.Enabled = true;
            }
        }

        protected void cmdPreviewFleetMaintenanceReport_Click(object sender, EventArgs e)
        {
            this.BusyReport.Visible = true;
            string xmlParams = "";
            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetMaintenanceFirstParamName, this.cboMaintenanceFleet.SelectedItem.Value.ToString());


            sn.Report.XmlParams = xmlParams;
            sn.Report.FleetId = Convert.ToInt32(this.cboMaintenanceFleet.SelectedItem.Value);
            sn.Report.FleetName = this.cboMaintenanceFleet.SelectedItem.Text;
            sn.Report.ReportFormat = Convert.ToInt32(this.cboFleetReportFormat.SelectedItem.Value.ToString());

            sn.Report.ReportType = cboReports.SelectedItem.Value;
            sn.Report.IsFleet = true;

            Response.Redirect("frmReportViewer.aspx");
        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
            }
        }

        protected void cmdSchedule_Click(object sender, EventArgs e)
        {
            CreateReportParams("../ReportsScheduling/frmReportScheduler.aspx");
        }

        /// <summary>
        /// Create report params and redirect to the next page
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        private void CreateReportParams(string redirectUrl)
        {
            try
            {
                string strFromDate = "", strToDate = "";
                DateTime from, to;
                const string dateFormat = "MM/dd/yyyy HH:mm:ss";

                this.lblMessage.Text = "";

                # region Validation

                if (this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "40" || this.cboReports.SelectedValue == "63" || this.cboReports.SelectedValue == "10020"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectVehicle");
                    return;
                }


                if (this.ddlLandmarks.SelectedIndex == 0 && (this.cboReports.SelectedValue == "21" || this.cboReports.SelectedValue == "23" || this.cboReports.SelectedValue == "41" || this.cboReports.SelectedValue == "10066" || this.cboReports.SelectedValue == "10067"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectLandmark");
                    return;
                }

                if (this.cboFleet.SelectedIndex == 0 && this.cboReports.SelectedValue != "19" && this.cboReports.SelectedValue != "25" && this.cboReports.SelectedValue != "28" && this.cboReports.SelectedValue != "38" && this.cboReports.SelectedValue != "53" && this.cboReports.SelectedValue != "88" && this.cboReports.SelectedValue != "60")
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("valFleetMessage");
                    return;
                }
                // check driver
                if (this.cboReports.SelectedValue == "25" || this.cboReports.SelectedValue == "26" || this.cboReports.SelectedValue == "27" || this.cboReports.SelectedValue == "28" || this.cboReports.SelectedValue == "53")
                {
                    if (this.ddlDrivers.SelectedIndex == 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("valDriver");
                        return;
                    }
                }

                if ((this.ddlGeozones.SelectedIndex == 0) && (this.cboReports.SelectedValue == "22"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("ddlGeozones_Item_0");
                    return;
                }

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                    strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                    strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 0)
                    strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + "12:00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                    strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                    strToDate = txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                    strToDate = txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 0)
                    strToDate = txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + "12:00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                    strToDate = txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

                from = Convert.ToDateTime(strFromDate, ci);
                to = Convert.ToDateTime(strToDate, ci);

                if (from >= to)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = false;
                    this.lblMessage.Text = "";
                }

                if ((this.cboVehicle.SelectedIndex == 0 && this.cboReports.SelectedValue == "3") ||
                    this.cboVehicle.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = false;
                    this.lblMessage.Text = "";
                }

                // Set Report period based on report.
                int reportDaysLimit = 31;

                if ("{10052|10034|10094}".IndexOf(this.cboReports.SelectedValue) > 0)       // year
                    // Multi-Fleet Annual Mileage Summary Report     10052
                    // Quarterly Mileage report (Quarter)            10034
                    // Quarterly Mileage report (Annual)             10094
                    reportDaysLimit = 367;
                else if ("{10002|10011|10020}".IndexOf(this.cboReports.SelectedValue) > 0)  // Quarter
                    // Activity Summary Report for Organization      10011
                    // Activity Summary Report per Vehicle           10002
                    // Individual Vehicle Mileage Report             10020
                    reportDaysLimit = 100;
                else                                                                       // Month
                    // Others
                    reportDaysLimit = 31;

                TimeSpan ts = to - from;
                if (ts.Days > reportDaysLimit)                  //31)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = GetLocalResourceObject("MessageRptDateRange").ToString();
                    return;
                }





                DataSet ds = new DataSet();
                string xmlResult = "";
                using (CrystalRpt.CrystalRpt cr = new CrystalRpt.CrystalRpt())
                {
                    if (objUtil.ErrCheck(cr.OrganizationHistoryDateRangeValidation(sn.UserID, sn.SecId, from.ToString(), to.ToString(), ref xmlResult), false))
                        if (objUtil.ErrCheck(cr.OrganizationHistoryDateRangeValidation(sn.UserID, sn.SecId, from.ToString(), to.ToString(), ref xmlResult), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " OrganizationHistoryDateRangeValidation:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xmlResult))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "OrganizationHistoryDateRangeValidation:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }


                ds.ReadXml(new StringReader(xmlResult));
                if (ds.Tables[0].Rows[0]["InValidCall"].ToString() == "1")
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Please decrease report date/time range! The from date should be greather than:" + System.DateTime.Now.AddDays(-Convert.ToInt16(ds.Tables[0].Rows[0]["MaximumDays"].ToString())).ToShortDateString();
                    return;
                }

                # endregion

                this.BusyReport.Visible = true;

                string xmlParams = "", convFromDate = "", convToDate = "";

                // 'from' and 'to' datetime incl. user pref. timezone and daylight saving
                convFromDate = from.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
                convToDate = to.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
                sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;

                # region Reports
                switch (this.cboReports.SelectedValue)
                {
                    # region Trip Details Report
                    case "0":
                        if (sn.Report.IsFleet)
                        {

                            //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                            StringBuilder sb = new StringBuilder();
                            for (int i = 1; i < cboVehicle.Items.Count; i++)
                            {
                                //sb.AppendLine(cboVehicle.Items[i].Value.Trim());
                                sb.Append(cboVehicle.Items[i].Value.Trim() + ",");
                            }


                            if (sb.ToString().Contains(","))
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, sb.ToString().Substring(0, sb.Length - 1));
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, sb.ToString());

                        }
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripFirstParamName, this.cboVehicle.SelectedValue.Trim());


                        xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);

                        break;
                    # endregion
                    # region Trip Summary Report
                    case "1":
                        if (sn.Report.IsFleet)
                        {
                            //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, this.cboFleet.SelectedItem.Value.ToString());

                            StringBuilder sb = new StringBuilder();
                            for (int i = 1; i < cboVehicle.Items.Count; i++)
                            {
                                //sb.AppendLine(cboVehicle.Items[i].Value);
                                sb.Append(cboVehicle.Items[i].Value.Trim() + ",");
                            }
                            //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString());

                            if (sb.ToString().Contains(","))
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString().Substring(0, sb.Length - 1));
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString());

                        }
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, this.cboVehicle.SelectedValue.Trim());


                        xmlParams += CreateTripSummaryParameteres(convFromDate, convToDate);

                        break;
                    # endregion
                    # region Alarms Report
                    case "2":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetAlarmsFirstParamName, this.cboFleet.SelectedItem.Value.Trim());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpAlarmFirstParamName, this.cboVehicle.SelectedItem.Value.Trim());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpAlarmSecondParamName : ReportTemplate.RpFleetAlarmsSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpAlarmThirdParamName : ReportTemplate.RpFleetAlarmsThirdParamName, convToDate);

                        break;
                    # endregion
                    # region History Report
                    case "3":
                        xmlParams = String.Format("{0};{1};{2};{3};{4}",
                           this.chkHistIncludeCoordinate.Checked.ToString(),
                           this.chkHistIncludeSensors.Checked.ToString(),
                           this.chkHistIncludePositions.Checked.ToString(),
                           this.chkHistIncludeInvalidGPS.Checked.ToString(),
                           this.cboVehicle.SelectedItem.Value.ToString());
                        break;
                    # endregion
                    # region Stop Report
                    case "4":
                        string blnShowsStops = "false";
                        string blnShowsIdles = "false";

                        blnShowsStops = this.optStopFilter.Items[0].Selected.ToString();
                        blnShowsIdles = this.optStopFilter.Items[1].Selected.ToString();
                        if (optStopFilter.Items[2].Selected)
                        {
                            blnShowsStops = "true";
                            blnShowsIdles = "true";
                        }


                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, this.cboStopSequence.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSixthParamName : ReportTemplate.RpFleetStopSixthParamName, blnShowsStops);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSeventhParamName : ReportTemplate.RpFleetStopSeventhParamName, blnShowsIdles);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopEighthParamName : ReportTemplate.RpFleetStopEighthParamName, this.optEndTrip.SelectedItem.Value.ToString());

                        break;
                    # endregion
                    # region Messages Report
                    case "5":
                        sn.Message.BoxId = 0;

                        if (!sn.Report.IsFleet)
                        {
                            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                            DataSet dsVehicle = new DataSet();

                            string xml = "";
                            if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, this.cboVehicle.SelectedItem.Value, ref xml), false))
                                if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, this.cboVehicle.SelectedItem.Value, ref xml), true))
                                {
                                    return;
                                }

                            if (xml != "")
                            {
                                dsVehicle.ReadXml(new StringReader(xml));
                                sn.Message.BoxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
                            }
                            else
                            {
                                return;
                            }

                        }

                        sn.Message.FromDate = convFromDate;
                        sn.Message.ToDate = convToDate;
                        sn.Message.FleetId = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                        sn.Message.FleetName = this.cboFleet.SelectedItem.Text.ToString();
                        sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.ToString();

                        xmlParams = "";
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesFirstParamName, strFromDate);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesSecondParamName, strToDate);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesThirdParamName, sn.Message.FleetId.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesFourthParamName, sn.Message.FleetName.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesFifthParamName, sn.Message.BoxId.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpMessagesSixthParamName, sn.Message.VehicleName.ToString());

                        break;
                    # endregion
                    # region Exception Report
                    case "6":
                        if (sn.Report.IsFleet)
                        {
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFirstParamName, this.cboFleet.SelectedItem.Value.ToString());

                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFourthParamName, chkSOSMode.Checked ? this.cboSOSLimit.SelectedItem.Value.ToString() : "-1");
                            if (this.chkDriverDoorExc.Checked || this.chkPassengerDoorExc.Checked || this.chkRearHopperDoorExc.Checked || this.chkSideHopperDoorExc.Checked
                                || this.chkLocker1.Checked || this.chkLocker2.Checked || this.chkLocker3.Checked || this.chkLocker4.Checked || this.chkLocker5.Checked || this.chkLocker6.Checked
                                || this.chkLocker7.Checked || this.chkLocker8.Checked || this.chkLocker9.Checked)
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFifthParamName, this.cboDoorPeriod.SelectedItem.Value.ToString());
                            else
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFifthParamName, "-1");
                        }
                        else
                        {
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpExceptionFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());

                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFourthParamName, chkSOSMode.Checked ? this.cboSOSLimit.SelectedItem.Value.ToString() : "-1");
                            if (this.chkDriverDoorExc.Checked || this.chkPassengerDoorExc.Checked || this.chkRearHopperDoorExc.Checked || this.chkSideHopperDoorExc.Checked
                             || this.chkLocker1.Checked || this.chkLocker2.Checked || this.chkLocker3.Checked || this.chkLocker4.Checked || this.chkLocker5.Checked || this.chkLocker6.Checked
                             || this.chkLocker7.Checked || this.chkLocker8.Checked || this.chkLocker9.Checked)
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFifthParamName, this.cboDoorPeriod.SelectedItem.Value.ToString());
                            else
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFifthParamName, "-1");

                        }

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSecondParamName : ReportTemplate.RpFleetExceptionSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionThirdParamName : ReportTemplate.RpFleetExceptionThirdParamName, convToDate);

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSixthParamName : ReportTemplate.RpFleetExceptionSixthParamName, chkTAR.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSeventhParamName : ReportTemplate.RpFleetExceptionSeventhParamName, chkImmobilization.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionEightParamName : ReportTemplate.RpFleetExceptionEightParamName, chkDriverDoor.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionNineParamName : ReportTemplate.RpFleetExceptionNineParamName, chkLeash.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTenParamName : ReportTemplate.RpFleetExceptionTenParamName, chkExcBattery.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionElevenParamName : ReportTemplate.RpFleetExceptionElevenParamName, chkExcTamper.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwelveParamName : ReportTemplate.RpFleetExceptionTwelveParamName, chkExcPanic.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionThirteenParamName : ReportTemplate.RpFleetExceptionThirteenParamName, chkExcKeypad.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionFourteenParamName : ReportTemplate.RpFleetExceptionFourteenParamName, chkExcGPS.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionFifteenParamName : ReportTemplate.RpFleetExceptionFifteenParamName, chkExcAVL.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSixteenParamName : ReportTemplate.RpFleetExceptionSixteenParamName, chkExcLeash.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionSeventeenParamName : ReportTemplate.RpFleetExceptionSeventeenParamName, chkDriverDoorExc.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionEighteenParamName : ReportTemplate.RpFleetExceptionEighteenParamName, chkPassengerDoorExc.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionNineteenParamName : ReportTemplate.RpFleetExceptionNineteenParamName, chkSideHopperDoorExc.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyParamName : ReportTemplate.RpFleetExceptionTwentyParamName, chkRearHopperDoorExc.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyFirstParamName : ReportTemplate.RpFleetExceptionTwentyFirstParamName, this.chkIncCurTARMode.Checked.ToString());

                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentySecondParamName  : ReportTemplate.RpFleetExceptionTwentySecondParamName, this.chkLocker1.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyThirdParamName  : ReportTemplate.RpFleetExceptionTwentyThirdParamName, this.chkLocker2.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyFourthParamName : ReportTemplate.RpFleetExceptionTwentyFourthParamName, this.chkLocker3.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyFifthParamName : ReportTemplate.RpFleetExceptionTwentyFifthParamName, this.chkLocker4.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentySixthParamName : ReportTemplate.RpFleetExceptionTwentySixthParamName, this.chkLocker5.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentySeventParamName   : ReportTemplate.RpFleetExceptionTwentySeventParamName , this.chkLocker6.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyEightParamName  : ReportTemplate.RpFleetExceptionTwentyEightParamName , this.chkLocker7.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionTwentyNineParamName  : ReportTemplate.RpFleetExceptionTwentyNineParamName , this.chkLocker8.Checked.ToString());
                        //xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpExceptionThirtyParamName : ReportTemplate.RpFleetExceptionThirtyParamName, this.chkLocker9.Checked.ToString());



                        xmlParams += ReportTemplate.MakePair("Locker1", "False");
                        xmlParams += ReportTemplate.MakePair("Locker2", "False");
                        xmlParams += ReportTemplate.MakePair("Locker3", "False");
                        xmlParams += ReportTemplate.MakePair("Locker4", "False");
                        xmlParams += ReportTemplate.MakePair("Locker5", "False");
                        xmlParams += ReportTemplate.MakePair("Locker6", "False");
                        xmlParams += ReportTemplate.MakePair("Locker7", "False");
                        xmlParams += ReportTemplate.MakePair("Locker8", "False");
                        xmlParams += ReportTemplate.MakePair("Locker9", "False");


                        break;
                    # endregion
                    # region OffHours Report
                    case "8":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetOffHourFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpOffHourFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());

                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourFourthParamName, chkShowStorePosition.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourFifthParamName, this.cboFromDayH.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSixthParamName, this.cboFromDayM.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSeventhParamName, this.cboToDayH.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourEightParamName, this.cboToDayM.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourNineParamName, this.cboWeekEndFromH.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourTenParamName, this.cboWeekEndFromM.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourElevenParamName, this.cboWeekEndToH.SelectedItem.Value.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourTwelveParamName, this.cboWeekEndToM.SelectedItem.Value.ToString());

                        break;
                    # endregion
                    # region Landmark Activity Report
                    case "9":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, cboStopSequence.SelectedItem.Value.ToString());

                        break;
                    # endregion
                    # region Fleet Maintenace Report
                    case "10":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.SelectedItem.Value.ToString());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.Checked.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, cboStopSequence.SelectedItem.Value.ToString());

                        break;
                    # endregion
                    # region Violation Report
                    case "17":
                    case "20":
                        //case "10013":
                        xmlParams = CreateViolationParameters(this.cboReports.SelectedValue);
                        break;
                    # endregion
                    # region Landmark Summary Report
                    case "21":
                        if (sn.Report.IsFleet) // fleet
                            //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                        else
                        {
                            // vehicle
                            //xmlParams = String.Format("{0}={1};{2}={3}",
                            //   ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text,
                            //   ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue);

                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue);
                        }
                        break;
                    # endregion
                    # region Geozone  Reports
                    case "22":
                    case "30":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);

                        if (!sn.Report.IsFleet) // fleet
                        {
                            //xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                        }
                        break;
                    # endregion
                    # region Landmark Details Report
                    case "23":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                        if (!sn.Report.IsFleet) // fleet
                        {
                            //  xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                        }

                        break;
                    # endregion
                    # region Inactivity Report
                    case "24":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        if (!sn.Report.IsFleet) // fleet
                        {
                            //xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                        }
                        break;
                    # endregion
                    # region Driver Trip Details Report
                    case "25":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        //xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);
                        break;
                    # endregion
                    # region Driver Trip Summary Report
                    case "26":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams += CreateTripSummaryParameteres(convFromDate, convToDate);
                        break;
                    # endregion
                    # region Driver Violation Report
                    case "27":
                    case "28":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams += CreateViolationParameters(this.cboReports.SelectedValue);
                        break;
                    # endregion
                    #region State Milage Report
                    case "34":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamIncludeSummary, "0");

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamIncludeSummary, "0");

                        if (!sn.Report.IsFleet) // fleet
                        {
                            // xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                        }
                        break;
                    # endregion
                    #region State Milage Summary Report
                    case "36":
                        // xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamIncludeSummary, "1");
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamIncludeSummary, "1");
                        break;
                    # endregion
                    #region Activity Summary Report for Organization
                    case "38":
                    case "51":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        break;
                    # endregion
                    #region Activity Summary Report per Vehicle
                    case "39":
                    case "50":
                    case "62":
                    case "87":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        break;
                    # endregion
                    #region HOS Details Report
                    case "53":
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        //redirectUrl = "./HOS_Report.aspx";
                        break;
                    # endregion

                    #region New Trips Summary Report per Vehicle
                    case "40":
                    case "63":
                        // xmlParams = String.Format("{0}={1};{2}={3}",
                        //       ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue,
                        //       ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);


                        break;
                    # endregion
                    #region Time At Landmark
                    case "41":


                        if (sn.Report.IsFleet) // fleet
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                        else
                        {

                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue);
                        }
                        break;
                    # endregion

                    #region Idling Summary Report New
                    case "88":
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams += ReportTemplate.MakePair("OrgId", sn.User.OrganizationId.ToString());
                        break;
                    # endregion
                    #region Trip SUmmary New
                    case "89":
                        xmlParams = "3;";
                        xmlParams += chkShowDriver.Checked.ToString() + ";";
                        xmlParams += sn.Report.LicensePlate;
                        break;
                    # endregion
                    #region Road Speed Violation Summary
                    case "97":
                        xmlParams = Convert.ToInt16(this.chkIsPostedOnly.Checked).ToString();
                        break;
                    # endregion

                    #region Road Speed Violation
                    case "104":
                        xmlParams = Convert.ToInt16(this.chkIsPostedOnly.Checked) + ";";
                        xmlParams += this.cboRoadSpeedDelta.SelectedItem.Value.ToString() + ";";

                        break;
                    # endregion

                    //Devin Aecon Start
                    #region Aecon Report
                    case "300":
                        xmlParams = radDispatch.SelectedValue;
                        break;
                    # endregion

                  //Devin Added for audit report 2014.01.27
                  #region HOS Audit Report
                  case "103":
                        /*
                          PreTripInspectionNotDone = 3,
                          DrivingWithMajorDefect = 4,
                          PostTripInspectionNotDone = 5,
                          Work shift violation = 6, 
                          Daily violation = 7,
                          Off Duty violation = 8,
                          Cycle violation = 9,
                         * Driver Without Signed = 10
                         */
                        if (!chkWorkShiftViolation.Checked)
                      {
                          if (xmlParams != "")
                            xmlParams = xmlParams + ",6";
                          else
                            xmlParams = "6";
                      }

                      if (!chkDailyViolation.Checked)
                      {
                          if (xmlParams != "")
                              xmlParams = xmlParams + ",7";
                          else
                              xmlParams = "7";
                      }

                      if (!chkOffDutyViolation.Checked)
                      {
                          if (xmlParams != "")
                              xmlParams = xmlParams + ",8";
                          else
                              xmlParams = "8";
                      }

                      if (!chkCycleViolation.Checked)
                      {
                          if (xmlParams != "")
                              xmlParams = xmlParams + ",9";
                          else
                              xmlParams = "9";
                      }

                      if (!chkPreTripNotDone.Checked)
                      {
                          if (xmlParams != "")
                            xmlParams = xmlParams + ",3";
                          else
                            xmlParams = "3";
                      }
                      if (!chkPostTripNotDone.Checked)
                      {
                          if (xmlParams != "")
                            xmlParams = xmlParams + ",5";
                          else
                            xmlParams = "5";
                      }
                      if (!chkDrivingWithDefect.Checked)
                      {
                          if (xmlParams != "")
                             xmlParams = xmlParams + ",4";
                          else
                              xmlParams = "4";
                      }

                      if (chkDriverWithoutSigned.Checked)
                      {
                          if (xmlParams != "")
                              xmlParams = xmlParams + ",10";
                          else
                              xmlParams = "10";
                      }

                      if (chkLogsNotReceive.Checked)
                      {
                          if (xmlParams != "")
                              xmlParams = xmlParams + ",11";
                          else
                              xmlParams = "11";
                      }


                      xmlParams =  ReportTemplate.MakePair(ReportTemplate.RpDetailedTripEighthParamName, xmlParams);
                      xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                      xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                      break;
                  # endregion

                  #region Server Report Section
                  default:

                        int pid = StringToInt32(this.cboReports.SelectedItem.Value.ToString());
                        if (pid > 10000)
                        {
                            xmlParams = GenerateReportParameters_JSON(pid);
                        }
                        break;
                    #endregion
                }
                #endregion Reports

                sn.Report.GuiId = Convert.ToInt16(this.cboReports.SelectedValue);
                sn.Report.XmlParams = xmlParams;
                //sn.Report.FromDate = strFromDate;
                sn.Report.FromDate = from.ToString();
                //sn.Report.ToDate = strToDate;
                sn.Report.ToDate = to.ToString();
                if (sn.Report.GuiId != 10)
                    sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedValue);
                else
                    sn.Report.ReportFormat = Convert.ToInt32(this.cboFleetReportFormat.SelectedValue);

                sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);
                sn.Report.DriverId = this.ddlDrivers.SelectedValue != "" ? Convert.ToInt32(this.ddlDrivers.SelectedValue) : 0;
                sn.Report.LandmarkName = this.ddlLandmarks.SelectedValue.ToString();
                sn.Report.LicensePlate = this.cboVehicle.SelectedValue;
                sn.Report.FleetName = this.cboFleet.SelectedItem.Text;
                sn.Report.ReportType = cboReports.SelectedValue;
                sn.Report.OrganizationHierarchyNodeCode = Request.Form["OrganizationHierarchyNodeCode"];

                if (ShowOrganizationHierarchy && vehicleSelectOption.Visible && optReportBased.SelectedIndex == 0)
                    sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
                else
                    sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);

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

            Response.Redirect(redirectUrl);
        }

        /// <summary>
        /// Build Violation Parameters
        /// </summary>
        /// <returns></returns>
        private string CreateViolationParameters(string reportGuiId)
        {
            int intCriteria = 0;

            if (this.chkSpeedViolation.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
            if (this.chkHarshAcceleration.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
            if (this.chkHarshBraking.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
            if (this.chkExtremeAcceleration.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
            if (this.chkExtremeBraking.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
            if (this.chkSeatBeltViolation.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;
            if (this.chkReverseSpeed.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_REVERSESPEED;
            if (this.chkReverseDistance.Checked)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_REVERSEDISTANCE;

            StringBuilder Params = new StringBuilder();

            Params.Append(intCriteria);

            // violation summary
            if (reportGuiId == "20" || reportGuiId == "28")
            {
                Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7}",
                   this.txtSpeed120.Text, this.txtSpeed130.Text, this.txtSpeed140.Text,
                   this.txtAccExtreme.Text, this.txtAccHarsh.Text, this.txtBrakingExtreme.Text,
                   this.txtBrakingHarsh.Text, this.txtSeatBelt.Text);
            }
            // violation details
            else
                if (reportGuiId == "17" || reportGuiId == "27" || reportGuiId == "10013")
                {
                    Params.Append("*");
                    string tmpSpeed = "";
                    switch (this.cboViolationSpeed.SelectedValue)
                    {
                        case "1":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "100" : "62");
                            break;
                        case "2":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "105" : "65");
                            break;
                        case "3":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "110" : "68");
                            break;
                        case "4":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "115" : "71");
                            break;
                        case "5":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "120" : "75");
                            break;
                        case "6":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "125" : "77");
                            break;
                        case "7":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "130" : "80");
                            break;
                        case "8":
                            tmpSpeed = (sn.User.UnitOfMes == 1 ? "140" : "85");
                            break;
                    }
                    Params.Append(tmpSpeed);
                }

            return Params.ToString();
        }

        /// <summary>
        /// Trip Summary Parameteres
        /// </summary>
        /// <param name="convFromDate"></param>
        /// <param name="convToDate"></param>
        /// <returns></returns>
        private string CreateTripSummaryParameteres(string convFromDate, string convToDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripSecondParamName : ReportTemplate.RpFleetTripSecondParamName, convFromDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripThirdParamName : ReportTemplate.RpFleetTripThirdParamName, convToDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripFourthParamName : ReportTemplate.RpFleetTripFourthParamName, chkShowStorePosition.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpTripFifthParamName : ReportTemplate.RpFleetTripFifthParamName, this.optEndTrip.SelectedItem.Value.ToString()));
            return sb.ToString();
        }

        /// <summary>
        /// Trip Details Parameteres
        /// </summary>
        /// <param name="xmlParams"></param>
        private string CreateTripDetailsParameteres(string fromDate, string toDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSecondParamName : ReportTemplate.RpFleetDetailedTripSecondParamName, fromDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripThirdParamName : ReportTemplate.RpFleetDetailedTripThirdParamName, toDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripFourthParamName : ReportTemplate.RpFleetDetailedTripFourthParamName, this.chkIncludeStreetAddress.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripFifthParamName : ReportTemplate.RpFleetDetailedTripFifthParamName, this.chkIncludeSensors.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSixthParamName : ReportTemplate.RpFleetDetailedTripSixthParamName, this.chkIncludePosition.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSeventhParamName : ReportTemplate.RpFleetDetailedTripSeventhParamName, this.chkIncludeIdleTime.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripEighthParamName : ReportTemplate.RpFleetDetailedTripEighthParamName, this.chkIncludeSummary.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripNinthParamName : ReportTemplate.RpFleetDetailedTripNinthParamName, this.chkShowStorePosition.Checked.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripTenthParamName : ReportTemplate.RpFleetDetailedTripTenthParamName, this.optEndTrip.SelectedItem.Value.ToString()));
            return sb.ToString();
        }

        protected void cmdViewScheduled_Click(object sender, EventArgs e)
        {
            Response.Redirect("../ReportsScheduling/frmScheduleReportList.aspx");
        }

        protected void optReportBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportBasedOption();
        }

        private void ReportBasedOption()
        {
            if (!this.vehicleSelectOption.Visible) return;

            if (optReportBased.SelectedItem.Value == "0")
            {
                trFleet.Visible = false;
                organizationHierarchy.Visible = true;

                sn.Report.OrganizationHierarchySelected = true;
            }
            else
            {
                trFleet.Visible = true;
                organizationHierarchy.Visible = false;
                sn.Report.OrganizationHierarchySelected = false;
            }
        }

        private void HideHierarchy()
        {
            trFleet.Visible = true;
            organizationHierarchy.Visible = false;
            this.optReportBased.SelectedIndex = 1;
            optBaseTable.Visible = false;
        }

        protected void txtFrom_Load(object sender, EventArgs e)
        {
            txtFrom.DateInput.DateFormat = sn.User.DateFormat;
            txtTo.DateInput.DateFormat = sn.User.DateFormat;
        }

        #region Server Report Section

        /// <summary>
        /// Build parameters string in json format 
        /// </summary>
        /// <returns></returns>
        public string GenerateReportParameters_JSON(int ReportID)      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {
            ReportID = Math.Abs(ReportID);

            StringBuilder sbp = new StringBuilder();

            #region General Parameters Section

            // Report's parameters
            sbp.Append("reportid: " + ReportID.ToString() + ", ");             // this.cboReports.SelectedItem.Value + ", ");
            sbp.Append("reportformat: " + cboFormat.SelectedItem.Text + ", ");              // PDF; EXCEL; WORD;....   .SelectedValue.ToString()
            sbp.Append("reportformatcode: " + cboFormat.SelectedItem.Value + ", ");         // 1;   2;     3;   ....   .SelectedValue.ToString()

            // Credencial Information
            sbp.Append("username: bsmreports" + ", ");
            sbp.Append("password: T0ybhARQ" + ", ");
            sbp.Append("domain: production" + ", ");

            // Application Logon User
            sbp.Append("userid: " + sn.UserID + ", ");

            //Time zone
            sbp.Append("timezone: GMT" + sn.User.TimeZone.ToString() + ", ");

            // User language
            sbp.Append("language: " + sn.SelectedLanguage + ", ");       //  SystemFunctions.GetStandardLanguageCode(HttpContext.Current) + ", ");

            // Organization
            sbp.Append("organization: " + sn.User.OrganizationId + ", ");

            //            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
            //                sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
            //            else if (cboFleet.Visible && cboFleet.Enabled)
            //                sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
            //            else
            //                sbp.Append(string.Empty);

            //            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
            //                sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
            //            else if (cboFleet.Visible && cboFleet.Enabled)
            //                sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);
            //            else
            //                sn.Report.FleetId = 0;

            //            // Vehicle infor
            //            if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
            //            {
            //                sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
            //                sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");
            //            }


            // Hierarchy ~ Fleet
            if (ShowOrganizationHierarchy && organizationHierarchy.Visible && optReportBased.SelectedIndex == 0)
            {
                if (Request.Form["OrganizationHierarchyFleetId"] != "")
                {
                    sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
                    sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
                }

                if (Request.Form["OrganizationHierarchyBoxId"] != "")
                { 
                    sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + ", ");       // Box ID
                    //sbp.Append("vehiclename: " + Request.Form["OrganizationHierarchyVehicleDescription"] + ", ");   //standardReport.cboVehicle_Name
                }
                else
                { // if Mileage Detail Report for Vehicle ()default, switch to for Fleet.
                    if (ReportID == 10040)
                        sbp.Replace("10040", "10039");
                }
            }
            else
            {
                if (cboFleet.Visible && cboFleet.Enabled)
                {
                    sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
                    sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);
                }
                if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
                {
                    // Switch to Mileage detail for Fleet if if for Vehicle.
                    if (this.cboVehicle.SelectedItem.Value == "0")
                    {
                        if (ReportID == 10040)
                            sbp.Replace("10040", "10039");
                    }
                    else
                    {
                        sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
                        //sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
                    }
                }
            }


            // Date range
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

            string sFrom = txtFrom.SelectedDate.Value.ToString("yyyy-MM-dd") + " 12:00:00 AM";
            string sTo = txtFrom.SelectedDate.Value.ToString("yyyy-MM-dd") + " 12:00:00 AM";

            DateTime dtFrom = Convert.ToDateTime(sFrom, ci);
            DateTime dtTo = Convert.ToDateTime(sTo, ci);

            sbp.Append("datefrom: " + dtFrom.ToString("MM/dd/yyyy hh:mm:ss tt") + ", ");
            sbp.Append("dateto: " + dtTo.ToString("MM/dd/yyyy hh:mm:ss tt") + ", ");

            sbp.Append("unitofvolume: " + sn.User.VolumeUnits + ", ");
            sbp.Append("unitofspeed: " + sn.User.UnitOfMes + ", ");

            #endregion

            #region Fleet Violation Reports of Details and Summary

            if (this.tblViolationReport.Visible)
            {
                sbp.Append("violationmask: " + GetViolationMaskString() + ", ");

                // Fleet Violation Detail
                if (this.cboViolationSpeed.Visible && this.cboViolationSpeed.Enabled)
                    sbp.Append("speedlimitation: " + GetViolationSpeed(this.cboViolationSpeed.SelectedItem.Value) + ", ");

                if (this.tblPoints.Visible)
                {
                    // Fleet Violation Summary
                    if (IsValidViolationPoint(this.txtSpeed120.Text))
                        sbp.Append("over120: " + this.txtSpeed120.Text + ", ");
                    else
                        sbp.Append("over120: 10, ");                         //Default

                    if (IsValidViolationPoint(this.txtSpeed130.Text))
                        sbp.Append("over130: " + this.txtSpeed130.Text + ", ");
                    else
                        sbp.Append("over130: 20, ");                         //Default

                    if (IsValidViolationPoint(this.txtSpeed140.Text))
                        sbp.Append("over140: " + this.txtSpeed140.Text + ", ");
                    else
                        sbp.Append("over140: 50, ");                         //Default

                    if (IsValidViolationPoint(this.txtAccHarsh.Text))
                        sbp.Append("accharsh: " + this.txtAccHarsh.Text + ", ");
                    else
                        sbp.Append("accharsh: 10, ");                         //Default

                    if (IsValidViolationPoint(this.txtAccExtreme.Text))
                        sbp.Append("accextreme: " + this.txtAccExtreme.Text + ", ");
                    else
                        sbp.Append("accextreme: 20, ");                         //Default

                    if (IsValidViolationPoint(this.txtBrakingExtreme.Text))
                        sbp.Append("brakiextreme: " + this.txtBrakingExtreme.Text + ", ");
                    else
                        sbp.Append("brakiextreme: 20, ");                         //Default

                    if (IsValidViolationPoint(this.txtBrakingHarsh.Text))
                        sbp.Append("brakharsh: " + this.txtBrakingHarsh.Text + ", ");
                    else
                        sbp.Append("brakharsh: 10, ");                         //Default

                    if (IsValidViolationPoint(this.txtSeatBelt.Text))
                        sbp.Append("seatbelt: " + this.txtSeatBelt.Text + ", ");
                    else
                        sbp.Append("seatbelt: 10, ");                         //Default
                }
            }

            #endregion


            // sensorNum = 3 (default)

            if (this.optEndTrip.Visible && this.optEndTrip.Enabled)
            {
                if (optEndTrip.SelectedIndex >= 0)
                    sbp.Append("sensornumber: " + optEndTrip.SelectedValue + ", ");
                else
                    sbp.Append("sensornumber: 3, ");
            }

            #region Speed Violation Reports
            if (tblRoadSpeed.Visible)
            {

                // all if unchecked.
                if (chkIsPostedOnly.Visible && chkIsPostedOnly.Enabled)
                {
                    if (chkIsPostedOnly.Checked)
                        sbp.Append("postedonly: 1, ");
                    else
                        sbp.Append("postedonly: 2, ");              //  0, ");
                }

                if (cboRoadSpeedDelta.Visible && cboRoadSpeedDelta.Enabled)
                    sbp.Append("roadspeeddelta: " + cboRoadSpeedDelta.SelectedItem.Value + ", ");
            }
            #endregion

            #region Fleet Utilization - not available

            //if (tblIdlingCost.Visible)
            //{
            //    if (txtCost.Visible && txtCost.Enabled)
            //    {
            //        if (osf.isNumeric(txtCost.Text))
            //        {
            //            sbp.Append("costofidling: " + txtCost.Text + ", ");
            //        }
            //        else
            //            sbp.Append("costofidling: 0");
            //    }
            //}

            //if (tblColorFilter.Visible)
            //{
            //    if (txtColorFilter.Visible && txtColorFilter.Enabled)
            //        sbp.Append("colorfilter: " + txtColorFilter.Text + ", ");
            //}

            #endregion

            //Landmark
            if (ddlLandmarks.Visible && ddlLandmarks.Enabled)
            {
                sn.Report.LandmarkName = this.ddlLandmarks.SelectedValue.ToString();
                sbp.Append("landmarkname: " + this.ddlLandmarks.SelectedValue.ToString() + ", ");
            }

            #region Trip Report 

            if (optEndTrip.Visible && optEndTrip.Enabled)
                sbp.Append("triptype: " + optEndTrip.SelectedItem.Value.ToString() + ", ");

            int mask = 0;           // 63. all(max)

            if (chkIncludeStreetAddress.Visible && chkIncludeStreetAddress.Enabled & chkIncludeStreetAddress.Checked)
                mask += 1;          // 1. Address
            if (chkIncludeSensors.Visible && chkIncludeSensors.Enabled && chkIncludeSensors.Checked)
                mask += 2;          // 2. Sensor
            if (chkIncludePosition.Visible && chkIncludePosition.Enabled && chkIncludePosition.Checked)
                mask += 4;          // 4. Position - Box Message In Type = 1 / Scheduled Update
            if (chkIncludeIdleTime.Visible && chkIncludeIdleTime.Enabled && chkIncludeIdleTime.Checked)
                mask += 8;          // 8. Idling
            if(chkShowStorePosition.Visible && chkShowStorePosition.Enabled && chkShowStorePosition.Checked)
                mask +=16;          // 16. Store Position

            sbp.Append("incmask: " + mask.ToString() + ", ");

            #endregion

            if (sbp.Length > 0)
                return "{" + sbp.ToString() + "}";
            else
                return "";
        }

        /// <summary>
        /// for async call parameter
        /// </summary>
        /// <param name="iResult"></param>
        private void wsCallback(IAsyncResult iResult) { }

        /// <summary>
        /// Get violation mask integer number > 0 | 63 for all
        /// </summary>
        /// <returns></returns>
        public int GetViolationMaskNumber()
        {
            int intCriteria = 0;

            // sr.ViolationOverSpeed = this.chkOverSpeed.Checked;
            if (this.chkSpeedViolation.Checked)                                                             // (mbViolationOverSpeed)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
            // sr.ViolationHarshAcceleration = this.chkHarshAcc.Checked;
            if (this.chkHarshAcceleration.Checked)                                                          // (mbViolationHarshAcceleration) 
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
            // sr.ViolationHarshBraking = this.chkHarshBrak.Checked;
            if (this.chkHarshBraking.Checked)                                                               // (mbViolationHarshBraking)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
            //sr.ViolationExtremeAcceleration = this.chkXtremAcc.Checked;
            if (this.chkExtremeAcceleration.Checked)                                                        // (mbViolationXtremeAcceleration)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
            //sr.ViolationExtremeBraking = this.chkXtremBrak.Checked;
            if (this.chkExtremeBraking.Checked)                                                            // (mbViolationXtremeBraking)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
            // sr.ViolationSeatBelt = this.chkSeatBelt.Checked;
            if (this.chkSeatBeltViolation.Checked)                                                        // (mbViolationSeatBelt)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;

            if (intCriteria > 0)
                return intCriteria;
            else
                return 63;
        }

        /// <summary>
        /// Get violation mask string
        /// </summary>
        /// <returns></returns>
        private string GetViolationMaskString()
        {
            return GetViolationMaskNumber().ToString();
        }

        /// <summary>
        /// Metric Unit = Kilo Metre in Factor tables
        /// Server report could not use CreateViolationParameters()
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private string GetViolationSpeed(string value)
        {
            // Mile to KM 1.6093
            string speed = "100";

            switch (value)
            {
                case "8":
                    speed = (sn.User.UnitOfMes == 1) ? "140" : "145"; //85
                    break;
                case "7":
                    speed = "130";              //80
                    break;
                case "6":
                    speed = "125";              //77 
                    break;
                case "5":
                    speed = "120";              //75
                    break;
                case "4":
                    speed = "115";              //71
                    break;
                case "3":
                    speed = "110";              //68
                    break;
                case "2":
                    speed = "105";              //65
                    break;
                case "1":
                default:
                    speed = "100";              //62
                    break;
            }
            return speed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsValidViolationPoint(string value)
        {
            if (!isNumeric(value))
                return false;
            else if (StringToInt(value) > 0)
                return true;
            else
                return false;
        }

        #region Assistance Functions Section

        /// <summary>
        /// Overloading
        /// </summary>
        /// <param name="DateValue"></param>
        /// <param name="Dateformat"></param>
        /// <returns></returns>
        public DateTime ConvertStringToDateTime(string DateValue, string Dateformat)
        {
            return ConvertStringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        /// <summary>
        /// Format Date/Time accouding Current UI Culture.
        /// Support two format: MM/DD/YYYY hh:mm:ss AM|PM (12h, Default) for EN and DD/MM/YYYY HH:MM:SS (24h) for FR. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="cultureinfo"></param>
        /// <returns></returns>
        public DateTime ConvertStringToDateTime(string value, string format, string cultureinfo)
        {

            CultureInfo culture = new CultureInfo(cultureinfo);
            DateTime date = DateTime.Now;
            string err = "";

            try
            {
                if (format.ToLower().IndexOf("hh") >= 0)
                    value = Convert.ToDateTime(value).ToString(format);

                date = DateTime.ParseExact(value, format, null);
                err = "";
            }
            catch (FormatException fx)
            {
                err = fx.Message;
                date = DateTime.Now;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                date = DateTime.Now;
            }
            finally
            {
            }

            return date;
        }

        public bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            double result;
            return Double.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        public bool isNumeric(char val)
        {
            return char.IsNumber(val);
        }

        public bool isNumeric(string val)
        {
            var regex = new Regex(@"^-*[0-9,\.]+$");
            return regex.IsMatch(val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int StringToInt(string value)
        {
            int i = 0;
            if (int.TryParse(value, out i))
                return i;
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Int32 StringToInt32(string value)
        {
            Int32 i = 0;
            if (Int32.TryParse(value, out i))
                return i;
            else
                return 0;
        }

        #endregion

        #endregion
    }
}
