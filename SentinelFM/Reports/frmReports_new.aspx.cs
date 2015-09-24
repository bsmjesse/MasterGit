#region Assembly Register Section
// System Assemblies
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Collections.Generic;
// BSM.VLF Assemblies
using VLF.ERRSecurity;
using VLF.Reports;
using VLF.CLS;
// Third Party Assemblies
using Telerik.Web.UI;

#endregion

namespace SentinelFM
{
    public partial class Reports_frmReports_new : SentinelFMBasePage
    {
        public string errvalSelectVehicle = string.Empty;
        public string errvalSelectLandmark = string.Empty;
        public string errvalFleetMessage = string.Empty;
        public string errvalHierarchyMessage = string.Empty;
        public string errDriverMessage = string.Empty;
        public string errvalDriver = string.Empty;
        public string errddlGeozones_Item_0 = string.Empty;
        public string errlblMessage_Text_InvalidDate = string.Empty;
        public string errlblMessage_Text_SelectVehicle = string.Empty;
        public string tblWidth = "30%";
        public bool ShowOrganizationHierarchy;
        public bool OrganizationHierarchySelectVehicle = false;

        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";
        public string PreferOrganizationHierarchyNodeCode = string.Empty;
        public bool IniHierarchyPath = false;

        public bool MutipleUserHierarchyAssignment = false;
        public int VehiclePageSize = 10;

        private string CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower();   //"en-US";
        private string DateFormat = "MM/dd/yyyy";
        private string TimeFormat = "hh:mm:ss tt";

        private bool hiddenHierarchy;
        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        clsStandardReport standardReport = null;
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                CheckTimout();
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                sn.Report.ReportActiveTab = 0;

                ShowOrganizationHierarchy = false;

                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                }




                if (ShowOrganizationHierarchy)
                {
                    MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                    if (ConfigurationManager.AppSettings["VehicleListTreePageSize"] != null)
                        int.TryParse(ConfigurationManager.AppSettings["VehicleListTreePageSize"].ToString(), out VehiclePageSize);

                    clsUtility objUtil;
                    objUtil = new clsUtility(sn);
                    ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                    string defaultnodecode = string.Empty;

                    string xml = "";
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                        {

                        }


                    /*StringReader strrXML = new StringReader(xml);
                    DataSet dsPref = new DataSet();
                    dsPref.ReadXml(strrXML);

                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {

                        if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                        {
                            string nodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                            //Devin added
                            if (!Page.IsPostBack)
                                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                            else
                                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, Request.Form["OrganizationHierarchyNodeCode"].ToString());

                            defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                        }

                    }*/
                    defaultnodecode = sn.User.PreferNodeCodes;


                    defaultnodecode = defaultnodecode ?? string.Empty;
                    if (defaultnodecode == string.Empty)
                    {
                        if (sn.RootOrganizationHierarchyNodeCode == string.Empty)
                        {
                            defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, MutipleUserHierarchyAssignment);
                            sn.RootOrganizationHierarchyNodeCode = defaultnodecode;
                        }
                        else
                            defaultnodecode = sn.RootOrganizationHierarchyNodeCode;
                    }
                    PreferOrganizationHierarchyNodeCode = defaultnodecode;
                    if (!IsPostBack)
                    {
                        DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                        hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                        hidOrganizationHierarchyFleetName.Value = DefaultOrganizationHierarchyFleetName;
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);                       
                        
                    }
                    //OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                    


                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);


                    ReportBasedOption();
                }
                else
                {
                    this.organizationHierarchy.Visible = false;
                    this.vehicleSelectOption.Visible = false;
                    this.trFleet.Visible = true;
                }

                try
                {
                    if (Request[this.txtFrom.UniqueID] != null)
                    {
                        this.txtFrom.SelectedDate = Convert.ToDateTime(Request[this.txtFrom.UniqueID]);
                        this.txtTo.SelectedDate = Convert.ToDateTime(Request[this.txtTo.UniqueID]);
                    }
                }
                catch
                {
                    if (!String.IsNullOrEmpty(sn.Report.FromDate))
                        this.txtFrom.SelectedDate = Convert.ToDateTime(sn.Report.FromDate);
                    if (!String.IsNullOrEmpty(sn.Report.ToDate))
                        this.txtTo.SelectedDate = Convert.ToDateTime(sn.Report.ToDate);
                }

                txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
                txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                {
                    txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                    txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                    //tblWidth = "85%";
                    cboFromDayH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                    cboToDayH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                    cboWeekEndFromH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                    cboWeekEndToH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                    cboHoursFrom.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                    cboHoursTo.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                }
                //if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                //{
                //    txtFrom.CultureInfo.CultureName = "fr-FR";
                //    txtTo.CultureInfo.CultureName = "fr-FR";
                //}
                //else
                //{
                //    txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                //    txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                //}


                //Show Busy Message comment by devin
                //cmdShow.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
                //cmdPreviewFleetMaintenanceReport.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
                //this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
                //this.BusyReport.Text = (string)base.GetLocalResourceObject("BusyPreparingMessage");



                int i = 0;

                if (!Page.IsPostBack)
                {
                    string displayProcessedStandardReports = ConfigurationManager.AppSettings.Get("displayProcessedStandardReports");
                    if (displayProcessedStandardReports == null || displayProcessedStandardReports == string.Empty || displayProcessedStandardReports.ToLower() == "false")
                        ddlReport.Items.FindItemByValue("3").Visible = false;

                    if (ShowOrganizationHierarchy)
                    {
                       optReportBased.Items.FindByValue("0").Selected = true;
                    }
                    else optReportBased.Items.FindByValue("1").Selected = true;
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmReports_new, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
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

                    //clsMisc.cboHoursFill(ref cboHoursFrom);
                    //clsMisc.cboHoursFill(ref cboHoursTo);
                    //clsMisc.cboHoursFill(ref cboFromDayH);
                    //clsMisc.cboHoursFill(ref cboToDayH);
                    //clsMisc.cboHoursFill(ref cboWeekEndFromH);
                    //clsMisc.cboHoursFill(ref cboWeekEndToH);

                    //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                    //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;


                    if (sn.Report.FromDate != "")
                        this.txtFrom.SelectedDate = Convert.ToDateTime(sn.Report.FromDate);
                    else
                    {
                        this.txtFrom.SelectedDate = DateTime.Now;
                        //this.txtFrom.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                    }


                    if (sn.Report.ToDate != "")
                        this.txtTo.SelectedDate = Convert.ToDateTime(sn.Report.ToDate);
                    else
                        this.txtTo.SelectedDate = DateTime.Now.AddDays(1);

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

                    this.cboHoursFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    this.cboHoursTo.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                    //this.cboHoursTo.SelectedIndex = -1;
                    //for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                    //{
                    //   if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == Convert.ToInt32(DateTime.Now.AddHours(1).Hour.ToString()))
                    //   {
                    //      this.cboHoursTo.SelectedIndex = i;
                    //      break;
                    //   }
                    //}

                    this.cboFromDayH.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                    this.cboToDayH.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0); ;
                    this.cboWeekEndFromH.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0); ;
                    this.cboWeekEndToH.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0); ;
                    this.cboFromDayH.TimeView.Interval = new TimeSpan(0, 15, 0);
                    this.cboFromDayH.TimeView.Columns = 8;
                    this.cboToDayH.TimeView.Interval = new TimeSpan(0, 15, 0);
                    this.cboToDayH.TimeView.Columns = 8;

                    this.cboWeekEndFromH.TimeView.Interval = new TimeSpan(0, 15, 0);
                    this.cboWeekEndFromH.TimeView.Columns = 8;
                    this.cboWeekEndToH.TimeView.Interval = new TimeSpan(0, 15, 0);
                    this.cboWeekEndToH.TimeView.Columns = 8;

                    CboFleet_Fill();
                    cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

                    if ((sn.Report.FleetId != 0) && (sn.Report.FleetId != -1))
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(sn.Report.FleetId.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.Report.FleetId));

                        if (sn.Report.LicensePlate != "")
                            cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindItemByValue(sn.Report.LicensePlate.ToString()));

                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else if (sn.User.DefaultFleet != -1)
                    {
                        cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(sn.User.DefaultFleet.ToString()));
                        CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                        this.lblVehicleName.Visible = true;
                        this.cboVehicle.Visible = true;
                    }
                    else
                    {
                        this.lblVehicleName.Visible = false;
                        this.cboVehicle.Visible = false;
                    }

                    getSaturdays();
                                     ////remove report

                    //string strViolationSpeedRoad = "18,489,287,628,254,368,664,343,999647,951,999630,480";
                    //string[] tmp = strViolationSpeedRoad.Split(',');
                    //bool removeReport = true;

                    //for (int y = 0; y < tmp.Length; y++)
                    //{
                    //    if (sn.User.OrganizationId.ToString() == tmp[y].ToString())
                    //    {
                    //        removeReport = false;
                    //        break;
                    //    }
                    //}


                    //if (removeReport)
                    //{
                    //    for (int x = 0; x < sn.Report.UserReportsDataSet.Tables[0].Rows.Count; x++)
                    //    {
                    //        if (sn.Report.UserReportsDataSet.Tables[0].Rows[x]["GuiId"].ToString() == "104")
                    //        {
                    //            sn.Report.UserReportsDataSet.Tables[0].Rows[x].Delete();
                    //            this.cboReports.DataSource = sn.Report.UserReportsDataSet;
                    //            this.cboReports.DataBind();
                    //            break;
                    //        }
                    //    }
                    //}



                    //if (sn.User.OrganizationId == 1)
                    //{
                    //    cboReports.Items.Add(new ListItem("Fuel Transaction report", "29"));
                    //    //cboReports.Items.Add(new ListItem("Geozone Report", "22"));
                    //    //cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                    //    //cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
                    //}

                    //cboReports.Items.Add(new RadComboBoxItem("Trip Summary Totals Report per Vehicle", "50"));
                    //cboReports.Items.Add(new RadComboBoxItem("Trip Summary Totals Report per Organization", "51"));
                    //cboReports.Items.Add(new RadComboBoxItem("HOS Details Report per Driver", "53"));

                    //cboReports.Items.Add(new RadComboBoxItem("HOS Details Report per Driver", "53"));
                    if (sn.User.UserGroupId == 1)
                    {
                        //cboReports.Items.Add(new RadComboBoxItem("BSM-Vehicle Information Data Dump", "54"));
                        AddReportItem(new RadComboBoxItem("BSM-Vehicle Information Data Dump", "54"));
                    }
                    //cboReports.Items.Add(new RadComboBoxItem("Daily Vehicle Activity Report for Fleet", "62"));
                    //cboReports.Items.Add(new RadComboBoxItem("Individual Vehicle Mileage Report", "63"));

                    if (Request.Cookies[sn.User.OrganizationId.ToString() + "ReportsNewSelectedIndex"] != null && Request.Cookies[sn.User.OrganizationId.ToString() + "ReportsNewSelectedIndex"].Value.Trim() != "")
                    {
                        try
                        {
                            cboReports.SelectedIndex = int.Parse(Request.Cookies[sn.User.OrganizationId.ToString() + "ReportsNewSelectedIndex"].Value);
                        }
                        catch {
                            cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindItemByValue(sn.Report.GuiId.ToString()));
                        }
                    }
                    else
                    {
                        cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindItemByValue(sn.Report.GuiId.ToString()));
                    }
                    ReportCriteria();

                    this.frmScheduleReportList.iOrganizationID = sn.User.OrganizationId;
                    this.frmScheduleReportList.iSessionUserID = sn.UserID;
                    this.frmScheduleReportList.ParentUserGroupId = sn.User.ParentUserGroupId;
                    this.frmScheduleReportList.UserGroupId = sn.User.UserGroupId;

                    cmdShow.Attributes.Add("OnClick", "javascript:OpenCreateWindow('" + cvDate.ValidationGroup + "', '" + cmdShowHide.ClientID + "'); return false; ");
                    cmdPreviewFleetMaintenanceReport.Attributes.Add("OnClick", "javascript:OpenCreateWindow(null, '" + cmdPreviewFleetMaintenanceReportHide.ClientID + "'); return false; ");
                    HideAndDispControlsForMyReport();
                }
                else
                {
                    if (lblMessage.ForeColor == Color.Green)
                    {
                        lblMessage.Text = string.Empty;
                        lblMessage.ForeColor = Color.Red;
                    }
                    if (this.cboReports.SelectedValue == "10020")
                        OrganizationHierarchySelectVehicle = true;
                }
                CreateErrorMessageForClientValidate();
                EnableExtendedReports();
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

            //Devin Add
            //if (ShowOrganizationHierarchy)
            //    if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
            //    {
            //        this.organizationHierarchy.Visible = false;
            //        this.vehicleSelectOption.Visible = false;
            //        this.trFleet.Visible = true;
            //    }
        }

        private void AddReportItem(RadComboBoxItem NewItem)
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
        /// <summary>
        /// Check if it is timeout for user report
        /// </summary>
        private void CheckTimout()
        {
            if (Request.QueryString["isMyReport"] != null && Request.QueryString["isMyReport"].ToString() == "1")
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "BacktoCreate", "Sys.Application.add_load(parent.frmMyReports_Login)", true);
            }
        }

        /// <summary>
        /// Hide and Display controls if current screen is my report detail page
        /// </summary>
        private void HideAndDispControlsForMyReport()
        {
            bool isRightUser = false;
            if (Request.QueryString["isMyReport"] != null)
            {
                if (Request.QueryString["isMyReport"].ToString() == "1")
                {
                    VLF3.Domain.InfoStore.UserReport userReport = null;
                    if (Request.QueryString["ID"] != null)
                    {
                        string id = Request.QueryString["ID"].ToString().Trim();

                        //VLF3.Services.InfoStore.UserService uss = VLF3.Services.InfoStore.UserService.GetInstance(sn.UserID);
                        clsAsynGenerateReport uss = new clsAsynGenerateReport();
                        userReport = uss.GetUserReportById(long.Parse(id));

                        //Will be removed by devin begin in the futrue
                        if (userReport != null)
                        {
                            //Temporary comment
                            isRightUser = true;
                            //if (userReport.User != null) 
                            //{
                            //    //for testing by devin
                            //    string myUserID = userReport.User.ToString();
                            //    if (userReport.User.UserId == sn.UserID) isRightUser = true;
                            //}
                        }
                        //end
                    }



                    if (!isRightUser)
                    {
                        Server.Transfer("frmReportError_new.aspx");
                        return;

                    }

                    pnlddlReport.Visible = false;
                    RadTabStrip1.Visible = false;
                    pnlPDF.Visible = false;
                    RadMultiPage1.CssClass = "None";
                    RadMultiPage1.BorderStyle = BorderStyle.None;
                    RadMultiPage1.BorderWidth = 0;
                    cmdShow.Visible = false;
                    cmdShowMyReport.Visible = true;
                    cmdShowMyReportUpdate.Visible = true;

                    cmdPreviewFleetMaintenanceReport.Visible = false;
                    cmdFleetMaintenanceReportMyReport.Visible = true;
                    cmdFleetMaintenanceReportMyReportUpdate.Visible = true;

                    standardReport = new clsStandardReport();
                    if (userReport.CustomProp != null)
                        standardReport.GetCustomProperty(userReport.CustomProp.Trim());

                    DateTime from = DateTime.Now.ToUniversalTime().Date.AddMinutes(userReport.Start);
                    DateTime to = from.AddMinutes(userReport.Period);
                    standardReport.txtFrom = from.Date;
                    standardReport.cboHoursFrom = from;
                    standardReport.txtTo = to.Date;
                    standardReport.cboHoursTo = to;
                    FillControlsByparameters();
                }
            }
        }


        /// <summary>
        /// Enable extended reports according to organization ID
        /// </summary>
        private void EnableExtendedReports()
        {

            if (sn.User.OrganizationId == 1000142 || sn.User.OrganizationId == 123 || sn.SuperOrganizationId == 382 || sn.User.OrganizationId == 327 || sn.User.OrganizationId == 489 || sn.User.OrganizationId == 622 || sn.User.OrganizationId == 570 || sn.User.OrganizationId == 18 || sn.User.OrganizationId == 951 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999620 || sn.User.OrganizationId == 698 || sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 999700 || sn.User.OrganizationId == 999746 || sn.User.OrganizationId == 999994 || sn.User.OrganizationId == 999692 || sn.User.OrganizationId == 1000010 || sn.User.OrganizationId == 655 || sn.User.OrganizationId == 999988 || sn.User.OrganizationId == 1000026 || sn.User.OrganizationId == 1000051 || sn.User.OrganizationId == 1000076 || sn.User.OrganizationId == 1000110 || sn.User.OrganizationId == 655 || sn.User.OrganizationId == 1000097 || sn.User.OrganizationId == 1000120 || sn.User.OrganizationId == 999722 || sn.User.OrganizationId == 999603 || sn.User.OrganizationId == 342 || sn.User.OrganizationId == 664 || sn.User.OrganizationId == 1000176 || sn.User.OrganizationId == 563 || sn.User.OrganizationId == 999693 || sn.User.OrganizationId == 999695  || sn.User.OrganizationId == 1000152 || sn.User.OrganizationId == 1000170)

            {
                ddlReport.Items.FindItemByValue("1").Visible = true;
            }
        }
        /// <summary>
        /// Create error message from resource file
        /// </summary>
        private void CreateErrorMessageForClientValidate()
        {
            errvalSelectVehicle = (string)base.GetLocalResourceObject("valSelectVehicle");
            errvalSelectLandmark = (string)base.GetLocalResourceObject("valSelectLandmark");
            errvalFleetMessage = (string)base.GetLocalResourceObject("valFleetMessage");
            errvalHierarchyMessage = (string)base.GetLocalResourceObject("valHierarchyMessage");
            errvalDriver = (string)base.GetLocalResourceObject("valDriver");
            errDriverMessage = (string)base.GetLocalResourceObject("valDriverMessage");
            errddlGeozones_Item_0 = (string)base.GetLocalResourceObject("ddlGeozones_Item_0");
            errlblMessage_Text_InvalidDate = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
            errlblMessage_Text_SelectVehicle = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
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

                //if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                //    if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //        //RedirectToLogin();
                //        return;
                //    }


                //if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public (sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), false))
                //    if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //        //RedirectToLogin();
                //        return;
                //    }


                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                dsGeozones = org.GetOrganizationGeoZone_Public(sn.UserID,sn.User.OrganizationId, false);
            }

            //if (String.IsNullOrEmpty(xmlResult))
            //{
            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            //    return;
            //}

          //  dsGeozones.ReadXml(new StringReader(xmlResult));

            this.ddlGeozones.Items.Clear();

            if (Util.IsDataSetValid(dsGeozones))
            {
                DataView view = dsGeozones.Tables[0].DefaultView;
                view.Sort = "GeozoneName";
                this.ddlGeozones.DataSource = view;
                this.ddlGeozones.DataBind();
                this.ddlGeozones.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlGeozones_Item_0").ToString(), "-1"));
            }
            else
                this.ddlGeozones.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlGeozones_NoAvailable").ToString(), "-100"));
        }

        protected void cboReports_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                HttpCookie aCookie = new HttpCookie(sn.User.OrganizationId.ToString() + "ReportsNewSelectedIndex");
                aCookie.Value = cboReports.SelectedIndex.ToString();
                aCookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(aCookie);

                ReportCriteria();
                //if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
                //{
                //    this.organizationHierarchy.Visible = false;
                //    this.vehicleSelectOption.Visible = false;
                //    this.trFleet.Visible = true;
                //}
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
        protected void chkWeekend_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cboWeekEndToH.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0); ;
            this.cboWeekEndFromH.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0); ;

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
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
            {
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));

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
            tblViolationReport_ExtendedSummary.Visible=false;
            ViolationReport_ExtendedSummaryPoints.Visible = false; 
            this.chkShowStorePosition.Visible = false;
            this.tblGeneralCriteria.Visible = true;
            this.cboVehicle.Enabled = true;
            FleetVehicleShow(true);
            DateTimeShow(true);
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
            this.chkShowOdometer.Visible = false;
            this.tblRoadSpeed.Visible = false;
            this.trSaturdays.Visible = false;
            this.LabelReportDescription.Text = "";
            this.tblViolationReport_Extended.Visible = false;
            this.chkAllDrivers.Visible = false;
            this.chkShowDriver.Checked = false;
            this.optMaintenanceBased.Items[2].Enabled = false;
            this.trMaintenanceDriver.Visible = false;
            this.optReportBased.Items[2].Enabled = false;
            this.trReportDriver.Visible = false;
            this.tbAuditReport.Visible = false;             //Devin added on 2014.01.27
            this.ddlServiceLandmarks.Visible = false;
            this.lblServiceLandmark.Visible = false;
            this.trMasterDelta.Visible = false;
            this.trObservationTime.Visible = false;
            this.tblReportFieldOption.Visible = false;
            this.tblLandmarkReportGroup.Visible = false;
            this.tblServiceLandmarks.Visible = false;
            this.tblLandmarkCategory.Visible = false;
            this.tblLandmarkListOption.Visible = false;
            this.trReportLayout.Visible = false;
            //this.tblRFViolationWeightPoint.Visible = false;

            this.lblMessage.Text = "";

            string ReportID = this.cboReports.SelectedValue;
            string resourceDescriptionName = "";
            string filter = String.Format("GuiId = '{0}'", this.cboReports.SelectedValue);

            // build descr. name
            sn.Report.ReportType = cboReports.SelectedValue;
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

            hiddenHierarchy = false;
            OrganizationHierarchySelectVehicle = false;

            if (ReportID == "21" && chkShowDriver.Checked)
                ReportID = "131";

            // Enable SSRS Reports w. Time Selection
            // if (Convert.ToInt16(ReportID) >= 10000) { this.cboHoursFrom.Enabled = false;  this.cboHoursTo.Enabled = false; }

            switch (ReportID)
            {
                case "0":       // Trip Details Report
                case "10074":   // Trip Detail Report
                    this.tblOptions1.Visible = true;
                    this.tblOptions2.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.lblTripReportDesc.Visible = true;
                    this.tblIgnition.Visible = true;
                    OrganizationHierarchySelectVehicle = true;
                    break;

                case "1":       // Trip Summary Report (CR)
                    this.lblTripSummaryReportDesc.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.tblIgnition.Visible = true;
                    OrganizationHierarchySelectVehicle = true;
                    break;

                case "2":       // Alarms Report
                case "10063":   // Alarms Report
                    this.lblAlarmReportDesc.Visible = true;
                    break;

                case "3": // History Report
                    this.tblHistoryOptions.Visible = true;
                    this.lblHistoryReportDesc.Visible = true;
                    HideHierarchy();
                    this.cboFleet.Enabled = true;
                    break;

                case "4": // Stop Report
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
                    if (ShowOrganizationHierarchy)
                        maintenanceVehicleSelectOption.Visible = true;
                    break;

                case "11": // idling details
                case "12":
                case "13":
                case "14":
                case "15":
                case "16":
                case "18":
                    this.cboVehicle.Enabled = false;
                    OrganizationHierarchySelectVehicle = false;
                    this.lblIdlingDetailsReportDesc.Visible = true;
                    if (sn.User.OrganizationId ==1000051 || sn.User.OrganizationId ==1000076 || sn.User.OrganizationId ==999722)
                        chkShowDriver.Visible = true;
                    break;

                case "17":      // violation details 4 fleet
                //case "10013":   // Fleet Violation Details Report
                case "10049":   // Multi-Fleet Violation Detail Report - should be replaced 
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.cboViolationSpeed.Visible = true;
		
                    //if (sn.User.OrganizationId == 951) 
                    //   OrganizationHierarchySelectVehicle = true;		
                    //if (sn.User.OrganizationId == 951 || sn.User.OrganizationId ==999994)
                    if (sn.User.OrganizationId ==999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;
                    }
                    break;

                case "19":      // idling summary
                case "88":
                case "10017":   // Idling Summary Report
                    FleetVehicleShow(false);
                    this.lblIdlingSummaryReportDesc.Visible = true;
                    break;

                case "20":      // violation summary 4 fleet
                //case "10014":   // Fleet Violation Summary Report - Stand SSRS Report
                case "10061":   // Fleet Violation Summary Report - Hierarchy Cost Centre.
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;

                    //if (sn.User.OrganizationId == 951)
                    //   OrganizationHierarchySelectVehicle = true;
                    //if (sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999994)
                    if (sn.User.OrganizationId == 999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;
                        this.lblReverseDistance.Visible = true;
                        this.lblReverseSpeed.Visible = true;
                        this.lblHihgRail.Visible = true;
                        this.txtReverseSpeed.Visible = true;
                        this.txtReverseDistance.Visible = true;
                        this.txtHighRail.Visible = true;  
                    }
                    break;

                case "21":      // landmark summary
                case "131":
                case "10067":   // Landmark Summary Report - SSRS Standard
                    this.tblLandmarkOptions.Visible = true;
                    // load landmarks
                    LoadLandmarks();

                    if (sn.User.OrganizationId == 1000051 || sn.User.OrganizationId == 1000076)
                        chkShowDriver.Visible = true;
                    break;

                case "22": // geozone
                case "30":
                    this.tblGeozoneOptions.Visible = true;
                    if (sn.User.OrganizationId == 999700)
                    {
                        this.chkShowOdometer.Visible = true;
                        this.chkShowOdometer.Checked = true;
                    }

                    // load geozone list
                    LoadGeozones();
                    HideHierarchy();
                    //this.cboFleet.Enabled = false;
                    break;

                case "23":      // landmark details
                case "10066":   // Landmark Detail Report - SSRS Standard
                    this.tblLandmarkOptions.Visible = true;
                    LoadLandmarks();
                    if (sn.User.OrganizationId ==1000051 || sn.User.OrganizationId ==1000076)
                        chkShowDriver.Visible = true;
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

                    //if (sn.User.OrganizationId == 951 || sn.User.OrganizationId ==999994)
                    if (sn.User.OrganizationId ==999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;  
                    }
                    break;

                case "28": // violation summary 4 fleet
                    this.tblViolationReport.Visible = true;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(false);

                    //if (sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999994)
                    if (sn.User.OrganizationId == 999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;
                        this.lblReverseDistance.Visible = true;
                        this.lblReverseSpeed.Visible = true;
                        this.lblHihgRail.Visible = true;
                        this.txtReverseSpeed.Visible = true;
                        this.txtReverseDistance.Visible = true;
                        this.txtHighRail.Visible = true;
                    }

                    break;

                case "10043":   // Driver Violation Summary Report - SSRS Standard
                    this.tblViolationReport.Visible = true;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    this.tblDriverOptions.Visible = false;
                    //LoadDrivers();
                    FleetVehicleShow(false);

                    if (sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;
                        this.lblReverseDistance.Visible = true;
                        this.lblReverseSpeed.Visible = true;
                        this.lblHihgRail.Visible = true;
                        this.txtReverseSpeed.Visible = true;
                        this.txtReverseDistance.Visible = true;
                        this.txtHighRail.Visible = true;
                    }
                    break;

                case "29":
                    FleetVehicleShow(false);
                    break;

                case "36":
                //case "10039":
                case "10041":
                case "10055":
                case "10094":                           //Fleet Vehicle Mileage Report
                    this.cboVehicle.Enabled = false;
                    break;

                case "38":
                case "51":
                case "10011":   // Activity Summary Report for Organization
                    FleetVehicleShow(false);
                    this.tblIgnition.Visible = true;
                    break;

                case "39":
                case "50":
                case "62":
                case "87":
                    this.cboVehicle.Enabled = false;
                    this.tblIgnition.Visible = true;
                    break;

                case "40": // Trip Summary Report Extended
                    OrganizationHierarchySelectVehicle = true;
                    break;

                case "63":
                case "10020":
                    this.tblIgnition.Visible = true;
                    OrganizationHierarchySelectVehicle = true;
                    break;

                case "41":
                //case "10073":
                    this.tblLandmarkOptions.Visible = true;
                    LoadLandmarks();
                    break;

                case "53":
                    FleetVehicleShow(false);
                    tblIgnition.Visible = false;
                    this.chkShowStorePosition.Visible = false;
                    this.tblDriverOptions.Visible = true;

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
                case "10078":
                case "10079":
                    this.cboVehicle.Enabled = false;
                    break;

                case "60":
                    FleetVehicleShow(false);
                    tblIgnition.Visible = false;
                    break;

                case "89": // Trip Summary Report (Driver)
                    chkShowDriver.Visible = true;
                    OrganizationHierarchySelectVehicle = true;
                    chkShowDriver.Checked = true;

                    break;

                case "97":
                //case "10019":
                case "10057":
                    this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = false;
                    lblRoadSpeedDelta.Visible = false;
                    break;

                case "103": // Devin chganged for HOS Audit report 2014.01.27 
                    tblDriverOptions.Visible = false;
                    FleetVehicleShow(false);
                    this.tblDriverOptions.Visible = true;
                    LoadDrivers();
                    FleetVehicleShow(true);
                    tbAuditReport.Visible = true;
                    break;

                case "104": // Road Speed Violation
                //case "10015":
                case "10056":
                    this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = true;
                    lblRoadSpeedDelta.Visible = true;
                    break;

                case "105": // Garmin Message
                    //HideHierarchy();
                    break;

               // case "10040": // switched by code
               //     break;

                case "10042":
                    break;

                case "127":
                    this.tblViolationReport_Extended.Visible = true;
                    this.cboVehicle.Enabled = true ;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                   // if (sn.User.OrganizationId == 951) // UP Urgent Fix SpeedListVisible:False (Salman/Amie: #3088)
                   // {
                        cboViolationSpeed_Extended.Visible = false;
                    //}
                    OrganizationHierarchySelectVehicle = true;

                    break;

                case "130":
                    this.tblViolationReport_ExtendedSummary.Visible = true;
                   // this.ViolationReport_ExtendedSummaryPoints.Visible = true;
                    this.cboVehicle.Enabled = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    OrganizationHierarchySelectVehicle = true;
                    break;

                case "10062": //Stop Report DWH
                case "10065": //Stop Report DWH - DRIVER
                    this.chkShowStorePosition.Visible = true;
                    this.tblStopReport.Visible = true;
                    this.lblStopReportDesc.Visible = true;
                    tblIgnition.Visible = false;
                    this.cboVehicle.Enabled = true;
                    this.cboHoursFrom.Enabled = true;
                    this.cboHoursTo.Enabled = true;

                    if (sn.User.OrganizationId ==1000051 || sn.User.OrganizationId ==1000076)
                        chkShowDriver.Visible = true;

                    break;


                case "132":
                    this.tblViolationReport.Visible = true;
                    //this.chkReverseSpeed.Text = "Towing";
                    //this.chkReverseSpeed.Checked = true;
                    this.chkReverseSpeed.Visible = false;
                    this.chkReverseDistance.Text = "Posted Speed";
                    this.chkReverseDistance.Checked = true;
                    //chkReverseSpeed.Visible = true;
                    chkReverseDistance.Visible = true; 
                    this.cboVehicle.Enabled = true;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.chkExtremeAcceleration.Visible = false;
                    chkExtremeAcceleration.Checked = false;  
                    this.chkExtremeBraking.Visible = false;
                    chkExtremeBraking.Checked = false;   
                    break;

                case "133":
                    this.tblViolationReport.Visible = true;
                    chkReverseSpeed.Visible = true;
                    chkReverseDistance.Visible = true; 
                    //this.chkReverseSpeed.Text = "Towing";
                    //this.chkReverseSpeed.Checked = true;
                    this.chkReverseSpeed.Visible = false;
                    this.chkReverseDistance.Text = "Posted Speed";
                    this.chkReverseDistance.Checked = true;
                    this.cboVehicle.Enabled = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    this.tblPoints.Visible = true;
                    
                    //Default score points
                    this.txtReverseSpeed.Text = "1";
                    this.txtSeatBelt.Text = "1";
                    this.txtSpeed120.Text = "1";
                    this.txtSpeed130.Text = "1";
                    this.txtSpeed140.Text = "1";
                    this.txtAccExtreme.Text = "1";
                    this.txtAccHarsh.Text = "1";
                    this.txtBrakingExtreme.Text = "1";
                    this.txtBrakingHarsh.Text = "1";
                    lblHihgRail.Visible = false;  
                    //this.lblHihgRail.Text =  "Towing";
                    //this.txtHighRail.Text = "1";
                    txtHighRail.Visible = false;  

                    this.txtReverseSpeed.Text = "1";
                    this.txtReverseDistance.Text = "1";
                    this.txtOver10.Text = "1"; 
                    this.lblOver10.Visible = true;
                    this.txtOver10.Visible = true;   

                    this.lblSpeed140.Visible = false;
                    this.txtSpeed140.Visible = false;
                    this.lblSpeed120.Text = ">60";
                    this.lblSpeed130.Text = ">70";

                     this.chkExtremeAcceleration.Visible = false;
                    chkExtremeAcceleration.Checked = false;  
                    this.chkExtremeBraking.Visible = false;
                    chkExtremeBraking.Checked = false;
                    txtBrakingExtreme.Visible = false;
                    txtAccExtreme.Visible = false;
                    lblBrakingExtreme.Visible = false;
                    lblAccExtreme.Visible = false;  
                    break;

                case "10001":   // Off Road Miles Report (State-Province) - Temp for testing
                    this.cboVehicle.Enabled = false;
                    this.cboHoursFrom.Enabled = true;
                    this.cboHoursTo.Enabled = true;

                    break;

                case "10002":   // Activity Summary Report per Vehicle - Standard
                case "10048":   // Vehicle Activity Summary Report - Hierarchy

                    OrganizationHierarchySelectVehicle = false;

                    if (sn.User.OrganizationId == 1000051 || sn.User.OrganizationId == 1000076)
                        chkShowDriver.Visible = true;

                    break;

                case "10013":   // Standard Fleet Violation Details Report

                    this.tblViolationReport.Visible = true;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.cboViolationSpeed.Visible = true;

                    if (sn.User.OrganizationId == 999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;
                    }

                    break;

                case "10014":   // Standard Fleet Violation Summary Report

                    this.tblViolationReport.Visible = true;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;

                    if (sn.User.OrganizationId == 999994)
                    {
                        this.chkReverseDistance.Visible = true;
                        this.chkReverseSpeed.Visible = true;
                        this.chkHighRail.Visible = true;
                        this.lblReverseDistance.Visible = true;
                        this.lblReverseSpeed.Visible = true;
                        this.lblHihgRail.Visible = true;
                        this.txtReverseSpeed.Visible = true;
                        this.txtReverseDistance.Visible = true;
                        this.txtHighRail.Visible = true;
                    }

                    break;

                case "10015":   // Speed Violation Details Report by Road Speed - Standard
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = true;
                    lblRoadSpeedDelta.Visible = true;
                    break;

                case "10018":   // Idling Details Report
                case "10051":   // Multi-Fleet Idling Detail Report - should be replaced

                    this.OrganizationHierarchySelectVehicle = false;
                    this.lblIdlingDetailsReportDesc.Visible = true;

                    if (sn.User.OrganizationId == 1000051 || sn.User.OrganizationId == 1000076 || sn.User.OrganizationId == 999722)
                    {
                        chkShowDriver.Visible = true;
                    }
                    
                    break;

                case "10019":   // Standard Speed Violation Summary Report by Road Speed
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = false;
                    lblRoadSpeedDelta.Visible = false;
                    break;

                case "10075":   // Trip Summary Report of Vehicles (RS)
                case "10082":   // Trip Summary Report of Drivers (RS)
                case "10098":   // Trip Summary Report with Vehicle Driver - switch by chkShowDriver
                    this.lblTripSummaryReportDesc.Visible = true;
                    this.chkShowStorePosition.Visible = true;
                    this.chkShowDriver.Visible = true;
                    this.tblIgnition.Visible = false;   //Event supports Trip only, no Tractor and PTO.
                    OrganizationHierarchySelectVehicle = true;
                    break;

                case "10080":   // KPI Weekly Utilization Hours By Fleet
                case "10081":   // KPI Weekly Utilization Hours By Vehicle
                    FleetVehicleShow(false);
                    DateTimeShow(false);
                    this.trSaturdays.Visible = true;
                    break;

                case "10084":   // Driver Utilization Report 
                    this.chkAllDrivers.Visible = true;
                    this.chkAllDrivers.Checked = true;
                    break;

                case "10085": // Vehicle Mileage Report.
                    this.cboVehicle.Enabled = false;
                    break;

                case "10086": // Unknown Driver Utilization Report
                case "10102": // Unknown Driver Utilization w Landmark Report
                    //FleetVehicleShow(false);
                    this.cboVehicle.Enabled = false;
                    break;

                case "10095": // AOBR AirMiles Report    
                    this.tblDriverOptions.Visible = true;               
                    LoadDrivers();                    
                    break;

                //case "10073": // Time at Landmark
                case "10091":   // Time at Mater Landmark - Landmark Events
                    //this.LoadServiceLandmarks(ReportID);
                    this.InitialLandmarkEventReport();
                    break;

                case "10093":   // Monthly Fleet Utilization Report
                     //disable From-Time
                    this.cboHoursFrom.Enabled = false;
                    // disable To-Date and To-Time
                    this.txtTo.Enabled = false;
                    this.cboHoursTo.Enabled = false;
                    // Hide Hierarchy, Fleet and Vehicle
                    FleetVehicleShow(false);
                    break;

                case "10096":   // Time at Mater Landmark
                    this.LoadServiceLandmarks(ReportID);
                    this.trMasterDelta.Visible = true;
                    break;

                case "10097":   // Off Road Mile / Idle / PTO 
                    break;

                case "10099":   // Out Of Home Terminal Report
                    this.trObservationTime.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.cboHoursFrom.Enabled = true;
                    this.cboHoursTo.Enabled = true;
                    break;

                case "10100":   // Asset Engine Hours Detail Report
                    this.cboHoursFrom.Enabled = false;
                    this.cboHoursTo.Enabled = false;
                    break;

                case "10101":   // Equipment Status Report
                    this.cboHoursFrom.Enabled = false;
                    this.cboHoursTo.Enabled = false;
                    break;

                case "10109":   // Tamper Activity Summary Report
                    break;

                case "10110":   // Rag and Fatigue Manage Report
                //    this.tblRFViolationWeightPoint.Visible = true;
                    break;

                case "10111":   // Landmark Auditing Report
                    FleetVehicleShow(false);
                    break;

                case "10112":   // Fleet User Assignment List Report
                case "10113":   // Fleet User Assignment List Report (DrillDown)
                    this.DateTimeLabel.Visible = false;
                    this.DateTimeEntry.Visible = false;
                    this.trReportLayout.Visible = true;
                    this.cboFleet.SelectedIndex = 0;
                    this.cboVehicle.Enabled = false;
                    //FleetVehicleShow(false);

                    break;

                case "10114":   // Reefer Temperature Report
                case "10116":   // Vehicle Door Status Report
                    this.trReportLayout.Visible = true;

                    break;

                default:
                    break;
            }

            if (ShowOrganizationHierarchy) 
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                if (MutipleUserHierarchyAssignment)
                    MutipleUserHierarchyAssignment = IsMultiHierarchyReport(this.cboReports.SelectedValue);
            }

            if (!hiddenHierarchy)
            {
                this.cboFleet.Enabled = true;
            }

            //if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
            //{
            //    this.organizationHierarchy.Visible = false;
            //    this.vehicleSelectOption.Visible = false;
            //    this.trFleet.Visible = true;
            //}

            if (vehicleSelectOption.Visible)
            {
                this.optReportBased.SelectedIndex = 0;
                ReportBasedOption();
            }

            if (maintenanceVehicleSelectOption.Visible)
            {
                this.optMaintenanceBased.SelectedIndex = 0;
                MaintenanceReportBasedOption();
            }

            if (sn.User.ControlEnable(sn, 109))
            {
                if (ReportID == "10075" || ReportID == "10082" || ReportID == "10002" || ReportID == "10048" || ReportID == "10018" || ReportID == "10051")
                {
                    this.vehicleSelectOption.Visible = true;
                    this.optReportBased.Items[2].Enabled = true;

                    if (!ShowOrganizationHierarchy)
                    {
                        this.optReportBased.Items[0].Enabled = false;
                        this.optReportBased.Items[1].Selected = true;
                        this.optReportBased.Items[2].Selected = false;
                    }
                    else
                    {
                        this.optReportBased.Items[0].Enabled = true;
                        this.optReportBased.Items[0].Selected = true;
                    }
                }
                if (ReportID == "0" || ReportID == "10074" || ReportID == "10083")
                {
                    //if (sn.UserID == 13627 || sn.UserID == 13772) 
                    // {
                        this.vehicleSelectOption.Visible = true;
                        this.optReportBased.Items[2].Enabled = true;

                        if (!ShowOrganizationHierarchy)
                        {
                            this.optReportBased.Items[0].Enabled = false;
                            this.optReportBased.Items[1].Selected = true;
                            this.optReportBased.Items[2].Selected = false;
                        }
                        else
                        {
                            this.optReportBased.Items[0].Enabled = true;
                            this.optReportBased.Items[0].Selected = true;
                        }
                    //}
                }
            }

           if (ReportID == "10084")   // Monthly Driver Utilization Report 
           {
               this.vehicleSelectOption.Visible = true;
               this.optReportBased.Items[0].Enabled = ShowOrganizationHierarchy;   // Disable hierarchy - no multi-selection
               this.optReportBased.Items[0].Selected = false;
               this.optReportBased.Items[1].Enabled = true;    // 
               this.optReportBased.Items[1].Selected = false;
               this.optReportBased.Items[2].Enabled = true;
               this.optReportBased.Items[2].Selected = true;

               ReportBasedOption();
           }
        }

        /// <summary>
        /// lsz 2013-03-21 for handling SSRS Reports
        /// </summary>
        /// <returns></returns>
        private bool ReportingServices()
        {
            object asState = null;
            bool bIsValid = false;
            string message = string.Empty;
            string sreport = this.cboReports.SelectedValue;

            if (sn.User.ControlEnable(sn, 109))
            {
                if (StringToInt(sreport) == 0 || StringToInt(sreport) == 10074)
                {
                    if (this.optReportBased.Enabled && this.optReportBased.SelectedIndex == 2)
                        sreport = "10083";
                }
            }

            lblMessage.Visible = false;
            lblMessage.Text = "";                //this.lblMessage.Text = "Rendering report successfully, please review Repository Tab for detail.";

            if (sreport != "10083")
                bIsValid = ReportingServiceValidation();
            else
                bIsValid = true;

            if (bIsValid)
            {
                string sjson = ReportingServiceParameters(StringToInt(sreport));

                if (!string.IsNullOrEmpty(sjson))
                {
                    switch (hidSubmitType.Value)
                    {
                        case "1":
                            #region One Time (Repository) Report
                            {
                                // Asynch Call for enterprise processes
                                //ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();
                                using (ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices())
                                {
                                    IAsyncResult AsyncResult = sr.BeginRenderRepositoryReport(sjson, wsCallback, asState);
                                }

                                #region Testing code
                                // local
                                //Synch call for testing repository report
                                //using (VLF.ASI.Interfaces.ReportingServices rs = new VLF.ASI.Interfaces.ReportingServices())
                                //{
                                //    if (rs.RenderRepositoryReport(sjson))
                                //        message = "";
                                //    else
                                //        rs.ProcessMessage(ref message);
                                //}
                                #endregion
                                #region Crystal Report
                                //clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                                //if (!genReport.CallReportService(sn, this, null, standardReport.keyValue))
                                //{
                                //    lblMessage.Visible = true;
                                //    lblMessage.Text = Resources.Const.Reports_LoadFailed;
                                //    //return;
                                //}
                                //else
                                //{
                                //    RadTabStrip1.FindTabByValue("1").Selected = true;
                                //    RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                                //}
                                #endregion

                                RadTabStrip1.FindTabByValue("1").Selected = true;
                                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                            }
                            #endregion
                            break;

                        case "2":
                            #region Schedule Report
                            {
                                standardReport = new clsStandardReport();
                                GenerateReportParameters();
                                clsXmlUtil xmlDoc = new clsXmlUtil(sn.Report.XmlDOC);
                                xmlDoc.CreateNode("GuiId", sreport);
                                xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat);  // new 2008 - 05 - 05
                                xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                                xmlDoc.CreateNode("FleetId", StringToInt(sn.Report.FleetId.ToString()));
                                xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
                                xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));
                                xmlDoc.CreateNode("XmlParams", sjson);                      //sn.Report.XmlParams);
                                using (ServerReports.Reports reportProxy = new ServerReports.Reports())
                                {
                                    lblMessage.Visible = true;
                                    if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                                        if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                                        {
                                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                                                VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning,
                                                "Schedule report failed. User: " + sn.UserID.ToString() + " Form:frmReportScheduler.aspx"));
                                            ShowMessage(lblMessage, this.GetLocalResourceObject("resScheduleFailed").ToString(), Color.Red);
                                            return false;
                                        }
                                }
                                ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);
                            }
                            #endregion
                            break;

                        case "3":
                            #region My Reports
                            {
                                // Asynch Call for enterprise processes
                                using (ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices())
                                {
                                    IAsyncResult AsyncResult = sr.BeginRenderRepositoryReport(sjson, wsCallback, asState);
                                }
                                #region Testing code
                                // Synch call for testing scheduled report
                                //string Url = "";
                                //if (sr.RenderScheduleReport(sn.UserID, "", "2013/01/27", "2013/01/302", sjson, 1, "en", ref Url))
                                //    message = "";
                                //else
                                //    message = sr.Message().ToString();

                                // Synch call for testing repository report
                                //VLF.ASI.Interfaces.ReportingServices rs = new VLF.ASI.Interfaces.ReportingServices();
                                //if (rs.RenderRepositoryReport(parameters))
                                //    message = "";
                                //else
                                //    message = rs.Message().ToString();
                                #endregion
                                #region Crystal Report
                                //string xmlDOC = sn.Report.XmlDOC;
                                //string reportName = clsAsynGenerateReport.PairFindValue("ReportName", xmlDOC);
                                //string reportDescription = clsAsynGenerateReport.PairFindValue("ReportDescription", xmlDOC);

                                //clsUserReportParams userReportProperty = new clsUserReportParams();
                                //userReportProperty.ReportName = reportName;
                                //userReportProperty.ReportDescription = reportDescription;
                                //userReportProperty.XmlParams = standardReport.CreateCustomProperty();

                                //clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                                //if (!genReport.CallReportService(sn, this, userReportProperty, standardReport.keyValue))
                                //{
                                //    lblMessage.Visible = true;
                                //    lblMessage.Text = Resources.Const.Reports_LoadFailed;
                                //    //return;
                                //}
                                //else
                                //{
                                //    RadTabStrip1.FindTabByValue("1").Selected = true;
                                //    RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                                //}
                                #endregion

                                RadTabStrip1.FindTabByValue("1").Selected = true;
                                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                            }
                            #endregion
                            break;
                    }
                }
                else
                {

                }
            }
            else
            {

            }
            return true;
        }


        /// <summary>
        /// Build parameters string in json format 
        /// </summary>
        /// <returns></returns>
        public string ReportingServiceParameters(int ReportID)      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {
            #region Preprocesses

            StringBuilder sbp = new StringBuilder();
            char delimitor = '\t';
            //string delimitor = ", ";

            ReportID = Math.Abs(ReportID);

            // Report for individual driver over Reports w. Driver Name
            if (this.optReportBased.Enabled && this.optReportBased.SelectedIndex == 2)
            {
                switch (ReportID)
                {
                    case 0:                 // Trip Detail Report (CR)
                    case 10074:             // Trip Detail Report (SR)
                        if (sn.UserID == 13627 || sn.UserID == 13772) ReportID = 10083;
                        break;
                    case 10018:             // Idling Detail Report - Standard
                    case 10051:             // Idling Detail Report - Hierarchy Cost Centre
                        ReportID = 10108;   // Idling Detail Report - Driver
                        break;
                    case 10002:             // Activity Summary Report - Standard
                    case 10048:             // Activity Summary Report - Hierarchy Cost Centre
                        ReportID = 10107;   // Activity Summary Report - Driver
                        break;
                    case 10075:             // Trip Summary Report
                        ReportID = 10082;   // Trip Summary Report of Individual Driver
                        break;
                }
            }
            // Map standard reports to reports with driver. 
            else if (chkShowDriver.Checked)
            {
                switch (ReportID)
                {
                    case 10018:             // Idling Details Report 
                        ReportID = 10044;   // Idling Details Report w. Vehicle Driver
                        break;
                    case 10002:             // Activity Summary Report by Vehicle - Stabdard
                    case 10048:             // Activity Summary Report by Vehicle - Hierarchy
                        ReportID = 10064;   // Activity Summary Report by Vehicle w. Vehicle Driver
                        break;
                    case 10062:             // Stop and Idling Report
                        ReportID = 10065;   // Stop and Idling Report w. Vehicle Driver
                        break;
                    case 10075:             // Trip Summary Report
                        ReportID = 10098;   // Trip Summary Report w. Vehicle Driver
                        break;
                    default:
                        break;
                }
            }

            if (ReportID == 0 || ReportID == 10074)
            {
                if (this.optReportBased.Enabled && this.optReportBased.SelectedIndex == 2)
                {
                    if (sn.UserID == 13627 || sn.UserID == 13772) ReportID = 10083;
                }
            }

            // Switch Between Driver Utilization Report and Vehicle Utilization Report
            if (ReportID == 10084 && this.cboFleet.Visible && this.cboVehicle.Enabled)
            {
                ReportID = (this.optReportBased.Items[1].Enabled && this.optReportBased.Items[1].Selected)? 10087 : 10084;
            }

            if (ReportID == 10091)
            {
                if (this.tblReportFieldOption.Visible)
                {
                    // Report w. Idling Time
                    if (this.chkIdleTimeOption.Checked && !this.chkPTOTimeOption.Checked) ReportID = 10104;
                    // Report w. PTO Time
                    if (!this.chkIdleTimeOption.Checked && this.chkPTOTimeOption.Checked) ReportID = 10105;
                    // Report w. both time of Idling and PTO
                    if (this.chkIdleTimeOption.Checked && this.chkPTOTimeOption.Checked) ReportID = 10106;
                }
            }
            // Report layout: { 1=Normal | 2=Drilldown } -- Available only for reports w. Drilldown edition.
            if (this.trReportLayout.Visible && this.rblReportLayout.Visible && this.rblReportLayout.Enabled)
            {
                switch (ReportID) { 
                    case 10112:
                        ReportID = (this.rblReportLayout.SelectedValue == "2") ? 10113 : 10112;
                        break;
                    case 10114:
                        ReportID = (this.rblReportLayout.SelectedValue == "2") ? 10115 : 10114;
                        break;
                    case 10116:
                        ReportID = (this.rblReportLayout.SelectedValue == "2") ? 10117 : 10116;
                        break;
                    default:
                        break;
                }
            }

            #endregion

            #region Basic Parameters

            //sn.Report.XmlDOC = "";
            sn.Report.GuiId = (Int16) ReportID;
            sn.Report.XmlParams = "";
            sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedItem.Value);
            sn.Report.ReportType = cboReports.SelectedItem.Value;

            // Basic parameters
            sbp.Append("reportid: " + ReportID.ToString() + delimitor);                          // this.cboReports.SelectedItem.Value + ", ");
            sbp.Append("reportformat: " + cboFormat.SelectedItem.Text + delimitor);              // PDF; EXCEL; WORD;....   .SelectedValue.ToString()
            sbp.Append("reportformatcode: " + cboFormat.SelectedItem.Value + delimitor);         // 1;   2;     3;   ....   .SelectedValue.ToString()
            // Application Logon User
            sbp.Append("userid: " + sn.UserID + delimitor);

            //Time zone
            //sbp.Append("timezone: GMT" + sn.User.TimeZone.ToString() + delimitor);

            // User language
            //sbp.Append("language: " + sn.SelectedLanguage + delimitor);       //  SystemFunctions.GetStandardLanguageCode(HttpContext.Current) + ", ");

            // Organization
            sbp.Append("organization: " + sn.User.OrganizationId + delimitor);

            // Date range
            string df = txtFrom.SelectedDate.Value.ToString("yyyy/MM/dd") + " " + cboHoursFrom.SelectedDate.Value.ToString("hh:mm:ss tt");
            string dt = txtTo.SelectedDate.Value.ToString("yyyy/MM/dd") + " " + cboHoursTo.SelectedDate.Value.ToString("hh:mm:ss tt");

            sn.Report.FromDate = df;    // from.ToString();
            sn.Report.ToDate = dt;      // to.ToString();

            sbp.Append("datefrom: " + df + delimitor);
            if (this.cboSaturdays.Visible)
                sbp.Append("dateto: " + this.cboSaturdays.SelectedItem.Value + delimitor); 
            else
                sbp.Append("dateto: " + dt + delimitor);

            //sbp.Append("unitofvolume: " + sn.User.VolumeUnits + delimitor);
            //sbp.Append("unitofspeed: " + sn.User.UnitOfMes + delimitor);

            #endregion

            #region Fleet - Vehicle
            // Hierarchy ~ Fleet
            if (organizationHierarchy.Visible)
            {
                sn.Report.OrganizationHierarchyNodeCode = OrganizationHierarchyNodeCode.Value;
                
                if (Request.Form["OrganizationHierarchyFleetId"] != "")
                {
                    sn.Report.FleetName = "";
                    string fleets = Request.Form["OrganizationHierarchyFleetId"].ToString();
                    //sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
                    sbp.Append("fleetid:  " + fleets + delimitor);   //Request.Form["OrganizationHierarchyFleetId"]
                    if (isMultiFleet(fleets))
                    {
                        sn.Report.IsFleet = false;
                        sn.Report.FleetId = 0;
                    }
                    else if (isNumericInteger(fleets))
                    {
                        sn.Report.IsFleet = true;
                        sn.Report.FleetId = StringToInt(fleets);
                    }
                    else
                    {
                        sn.Report.IsFleet = false;
                        sn.Report.FleetId = 0;
                    }
                }

                if (Request.Form["hidOrganizationHierarchyNodeCode"] != "")
                    sbp.Append("nodecode:  " + Request.Form["hidOrganizationHierarchyNodeCode"] + delimitor);

                if (Request.Form["OrganizationHierarchyBoxId"] != "")
                {
                    sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + delimitor);       // Box ID

                    sbp.Replace("10054", "10040").Replace("10039", "10040");
                }

                sn.Report.LicensePlate = "";

            }
            else
            {
                if (cboFleet.Visible && cboFleet.Enabled)
                {
                    sn.Report.IsFleet = true;
                    sn.Report.FleetName = this.cboFleet.SelectedItem.Text;
                    if ((ReportID == 10112 || ReportID == 10113) && this.cboFleet.SelectedIndex == 0)
                        sbp.Append("fleetid:  0" + delimitor);
                    else
                        sbp.Append("fleetid:  " + this.cboFleet.SelectedItem.Value + delimitor);
                }

                if (this.cboVehicle.Visible && this.cboVehicle.Enabled && this.cboVehicle.SelectedIndex > 0) 
                {
                    sn.Report.LicensePlate = this.cboVehicle.SelectedItem.Value;
                    sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + delimitor);                       // License Plate
                    sbp.Replace("10054", "10040").Replace("10039", "10040");
                }
            }
            #endregion

            #region Fleet Violation Reports of Details and Summary

            if (this.tblViolationReport.Visible)
            {
                sbp.Append("violationmask: " + GetViolationMaskString() + delimitor);

                // Fleet Violation Detail
                if (this.cboViolationSpeed.Visible && this.cboViolationSpeed.Enabled)
                    sbp.Append("speedlimitation: " + GetViolationSpeed(this.cboViolationSpeed.SelectedItem.Value) + delimitor);

                if (this.tblPoints.Visible)
                {
                    // Fleet Violation Summary
                    if (IsValidViolationPoint(this.txtSpeed120.Text))
                        sbp.Append("over120: " + this.txtSpeed120.Text + delimitor);
                    else
                        sbp.Append("over120: 10, ");                         //Default

                    if (IsValidViolationPoint(this.txtSpeed130.Text))
                        sbp.Append("over130: " + this.txtSpeed130.Text + delimitor);
                    else
                        sbp.Append("over130: 20, ");                         //Default

                    if (IsValidViolationPoint(this.txtSpeed140.Text))
                        sbp.Append("over140: " + this.txtSpeed140.Text + delimitor);
                    else
                        sbp.Append("over140: 50, ");                         //Default

                    if (IsValidViolationPoint(this.txtAccHarsh.Text))
                        sbp.Append("accharsh: " + this.txtAccHarsh.Text + delimitor);
                    else
                        sbp.Append("accharsh: 10, ");                         //Default

                    if (IsValidViolationPoint(this.txtAccExtreme.Text))
                        sbp.Append("accextreme: " + this.txtAccExtreme.Text + delimitor);
                    else
                        sbp.Append("accextreme: 20, ");                         //Default

                    if (IsValidViolationPoint(this.txtBrakingExtreme.Text))
                        sbp.Append("brakiextreme: " + this.txtBrakingExtreme.Text + delimitor);
                    else
                        sbp.Append("brakiextreme: 20, ");                         //Default

                    if (IsValidViolationPoint(this.txtBrakingHarsh.Text))
                        sbp.Append("brakharsh: " + this.txtBrakingHarsh.Text + delimitor);
                    else
                        sbp.Append("brakharsh: 10, ");                         //Default

                    if (IsValidViolationPoint(this.txtSeatBelt.Text))
                        sbp.Append("seatbelt: " + this.txtSeatBelt.Text + delimitor);
                    else
                        sbp.Append("seatbelt: 10, ");                         //Default
                }
            }

            #endregion

            #region Speed Violation Reports
            if (tblRoadSpeed.Visible)
            {

                // all if unchecked.
                if (chkIsPostedOnly.Visible)                        // && chkIsPostedOnly.Enabled)
                {
                    if (chkIsPostedOnly.Checked)
                        sbp.Append("postedonly: 1" + delimitor);
                    else
                        sbp.Append("postedonly: 2" + delimitor);              //  0, ");
                }

                if (cboRoadSpeedDelta.Visible && cboRoadSpeedDelta.Enabled)
                    sbp.Append("roadspeeddelta: " + cboRoadSpeedDelta.SelectedItem.Value + delimitor);
            }
            #endregion

            #region Fleet Utilization

            // For CN, implemented later
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

            // New for CN, implemented later
            //if (tblColorFilter.Visible)
            //{
            //    if (txtColorFilter.Visible && txtColorFilter.Enabled)
            //        sbp.Append("colorfilter: " + txtColorFilter.Text + ", ");
            //}

            #endregion

            #region Driver
            if (ddlDrivers.Visible && ddlDrivers.Enabled)
            {
                sn.Report.DriverId = (this.ddlDrivers.SelectedValue.ToString() != "") ? Convert.ToInt32(this.ddlDrivers.SelectedValue.ToString()) : 0;
                sbp.Append("driverid: " + sn.Report.DriverId.ToString() + delimitor);
            }
            else if (this.ucReportDriver.Visible)
            {
                // Replace Trip Summary by Vehicle with Trip Summary by Driver
                sbp.Replace("10075", "10082");
                // Driver Utilization Report and Driver issued
                if (ReportID == 10084 && this.chkAllDrivers.Visible && !this.chkAllDrivers.Checked)
                    sbp.Append("driverid: " + this.ucReportDriver.SelectedDriverId.ToString() + delimitor);
                else
                    sbp.Append("driverid: " + this.ucReportDriver.SelectedDriverId.ToString() + delimitor);
            }
            else if (this.ucMantainDrivers.Visible)
            {
                // Replace Trip Summary by Vehicle with Trip Summary by Driver
                sbp.Replace("10075", "10082");
                // Driver Utilization Report for individual Driver
                if (ReportID == 10084 && this.chkAllDrivers.Visible && !this.chkAllDrivers.Checked)
                    sbp.Append("driverid: " + this.ucReportDriver.SelectedDriverId.ToString() + delimitor);
                else
                    sbp.Append("driverid: " + this.ucMantainDrivers.SelectedDriverId.ToString() + delimitor);
            }
            #endregion

            #region Landmark Reports
            // Standard Landmark Name
            if (ddlLandmarks.Visible && ddlLandmarks.Enabled)
            {
                sn.Report.LandmarkName = this.ddlLandmarks.SelectedValue.ToString();
                sbp.Append("landmarkname: " + sn.Report.LandmarkName.ToString() + delimitor);
            }
            // Standard Landmark ID
            if (ddlServiceLandmarks.Visible && ddlServiceLandmarks.Enabled)
            {
                string landmarkid = StringToInt(this.ddlServiceLandmarks.SelectedValue.ToString()).ToString();
                sbp.Append("landmarkid: " + landmarkid + delimitor);
            }
            // Service Landmark Category
            if (this.rcbLandmarkCategory.Visible)
                sbp.Append("categoryid: " + this.rcbLandmarkCategory.SelectedValue + delimitor);
            // Service Landmark Name
            if (this.rcbServiceLandmarkList.Visible)
                sbp.Append("landmarkname: " + this.rcbServiceLandmarkList.SelectedItem.Text + delimitor);
            // Service Landmark ID
            if (this.rcbServiceLandmarkList.Visible)
                sbp.Append("landmarkid: " + this.rcbServiceLandmarkList.SelectedValue + delimitor);
            #endregion

            #region Trip Report 
            //Trip sensor = {3:Ignition | 11:Tractor Power | 8:PTO}
            if (optEndTrip.Visible && optEndTrip.Enabled)
                sbp.Append("triptype: " + optEndTrip.SelectedItem.Value.ToString() + delimitor);

            if (this.tblOptions1.Visible)
            {
                int mask = 0;           // 63. all(max)
                if (chkIncludeStreetAddress.Visible && chkIncludeStreetAddress.Enabled & chkIncludeStreetAddress.Checked) mask += 1;          // 1. Address
                if (chkIncludeSensors.Visible && chkIncludeSensors.Enabled && chkIncludeSensors.Checked) mask += 2;          // 2. Sensor
                if (chkIncludePosition.Visible && chkIncludePosition.Enabled && chkIncludePosition.Checked) mask += 4;          // 4. Position - Box Message In Type = 1 / Scheduled Update
                if (chkIncludeIdleTime.Visible && chkIncludeIdleTime.Enabled && chkIncludeIdleTime.Checked) mask += 8;          // 8. Idling
                if (chkShowStorePosition.Visible && chkShowStorePosition.Enabled && chkShowStorePosition.Checked) mask += 16;          // 16. Store Position
                if (mask > 0) sbp.Append("includemask: " + mask.ToString() + delimitor);
            }
            #endregion

            //standardReport.optEndTrip = optEndTrip.SelectedItem.Value;
            #region History Report
            //if (this.tblHistoryOptions.Visible) { }
            //chkHistIncludeCoordinate
            //chkHistIncludeSensors
            //chkHistIncludeInvalidGPS
            //chkHistIncludePositions
            #endregion

            // Assistance parameters
            #region Assistance Parameters
            //Show Store Position
            if (chkShowStorePosition.Visible)
                sbp.Append("incaddress: " + ((chkShowStorePosition.Checked) ? "1" : "0") + delimitor);
            // Stop & Idling in seconds: 5:300 | 10:600 | ......
            if (cboStopSequence.Visible)
                sbp.Append("minduration: " + cboStopSequence.SelectedItem.Value.ToString() + delimitor);
            // Stop & Idling: 0:Stop | 1:Idling | 2:Both
            if (optStopFilter.Visible)
                sbp.Append("remark: " + optStopFilter.SelectedItem.Value.ToString() + delimitor);

            // sensorNum = 3 (default)
            if (this.optEndTrip.Visible && this.optEndTrip.Enabled)
            {
                if (optEndTrip.SelectedIndex >= 0)
                    sbp.Append("sensornumber: " + optEndTrip.SelectedValue + delimitor);
                else
                    sbp.Append("sensornumber: 3" + delimitor);
            }

            if (this.txtMasterDetal.Visible && this.txtMasterDetal.Enabled)
            { 
                if(isNumeric(this.txtMasterDetal.Text))
                    sbp.Append("delta: " + this.txtMasterDetal.Text + delimitor);
            }

            if(this.trObservationTime.Visible && this.cboObservationTime.Enabled)
                sbp.Append("reporttime: " + this.cboObservationTime.SelectedDate.Value.ToString("hh:mm:ss tt") + delimitor);
            
            #endregion 

            if (sbp.Length > 0)
                return "{" + sbp.ToString() + "}";
            else
                return "";

        }

        /// <summary>
        /// Refer to CreateReportParams() for details
        /// </summary>
        /// <returns></returns>
        public bool ReportingServiceValidation()
        {
            string msg = "";

            this.lblMessage.Visible = false;
            this.lblMessage.Text = "";

            OrganizationHierarchyPath = getPathByNodeCode(OrganizationHierarchyNodeCode.Value);
            IniHierarchyPath = true;

            try
            {
                string tmp = this.cboReports.SelectedValue.ToString();
                // Vehicle: Box (hierarchy) or License Plate (fleet)
                if (!IsValidVehicle(tmp))
                    msg = (string)base.GetLocalResourceObject("valSelectVehicle");      //this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                // Fleet:
                else if (!IsValidFleet(tmp) && trFleet.Visible)
                    msg = (string)base.GetLocalResourceObject("valFleetMessage");
                //Landmark
                else if (!IsValidLandmark(tmp))
                    msg = (string)base.GetLocalResourceObject("valSelectLandmark");
                //Driver
                else if (!IsValidDriver(tmp))
                    msg = (string)base.GetLocalResourceObject("valDriver");
                //Geozone
                else if (!IsValidGeozone(tmp))
                    msg = (string)base.GetLocalResourceObject("ddlGeozones_Item_0");
                //DateFr & DateTo
                else if (!IsValidDateTime(out tmp))
                    msg = tmp;
                //Observation time required
                else if (this.trObservationTime.Visible && this.cboObservationTime.Enabled && this.cboObservationTime.ToString() == "")
                    msg = "Please enter observation time.";
                //No validation required
                else
                    msg = "";
            }
            catch (NullReferenceException Ex)
            {
                msg = Ex.Message;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                msg = Ex.Message;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
            finally{
                if (!string.IsNullOrEmpty(msg))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = msg;
                }
            }
            return !this.lblMessage.Visible;
        }

        /// <summary>
        /// Load landmarks into ddl
        /// </summary>
        private void LoadLandmarks()
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = "";

            using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
            {

                //if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                //    if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //        //RedirectToLogin();
                //        return;
                //    }


                //if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public (sn.UserID, sn.SecId, sn.User.OrganizationId,false, ref xmlResult), false))
                //    if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //        //RedirectToLogin();
                //        return;
                //    }

                //if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), false))
                //    if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //        RedirectToLogin();
                //        return;
                //    }

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                dsLandmarks = org.GetOrganizationLandmark_Public(sn.UserID, sn.User.OrganizationId, false);

            }

            //if (String.IsNullOrEmpty(xmlResult))
            //{
            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            //    return;
            //}

            // dsLandmarks.ReadXml(new StringReader(xmlResult));

            this.ddlLandmarks.Items.Clear();

            if (Util.IsDataSetValid(dsLandmarks))
            {
                this.ddlLandmarks.DataSource = dsLandmarks;
                this.ddlLandmarks.DataBind();
                this.ddlLandmarks.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlLandmarks_Item_0").ToString(), "-1"));
            }
            else
                this.ddlLandmarks.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlLandmarks_NoAvailable").ToString(), "-100"));


            if ((sn.Report.LandmarkName != "") && (sn.Report.LandmarkName != " -1"))
                ddlLandmarks.SelectedIndex = ddlLandmarks.Items.IndexOf(ddlLandmarks.Items.FindItemByValue(sn.Report.LandmarkName.ToString()));

        }

        private void InitialLandmarkEventReport(){
            // Category List
            if (this.LoadLandmarkCategories())
                this.tblLandmarkCategory.Visible = true;
            else
                this.tblLandmarkCategory.Visible = false;
            // Service Landmark of Category
            if (this.LoadServiceLandmarkByCategory())
            {
                this.tblServiceLandmarks.Visible = true;
                this.lblServiceLandmarkList.Visible = true;
                this.rcbServiceLandmarkList.Visible = true;
            }
            else
            {
                this.tblServiceLandmarks.Visible = true;
            }
            // Landmark List Option
            //this.tblLandmarkListOption.Visible = true;
            //this.rblLandmarkListOption.SelectedIndex = 0;
            // Landmark Report Group Option
            //this.tblLandmarkReportGroup.Visible = true;
            //this.rblLandmarkReportGroup.SelectedIndex = 0;
            // Report Field Option
            this.tblReportFieldOption.Visible = true;
            this.chkIdleTimeOption.Checked = false;
            this.chkPTOTimeOption.Visible = false;
            this.chkPTOTimeOption.Checked = false;
        }

        #region Landmark Category List
        /// <summary>
        /// 
        /// </summary>
        private bool LoadLandmarkCategories()
        {
            return LoadLandmarkCategories(sn.UserID, sn.User.OrganizationId);
        }

        /// <summary>
        /// Load landmarks into ddl
        /// </summary>
        private bool LoadLandmarkCategories(int UserID, int OrganizationID)
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = "";
            int iRows = 0;
            this.ddlServiceLandmarks.Items.Clear();

            try
            {
                using (ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization())
                {
                    if ((int)InterfaceError.NoError == dbo.GetLandmarkServiceCategoryList(UserID, UserID.ToString(), OrganizationID, ref xmlResult))
                    {
                        dsLandmarks.ReadXml(new StringReader(xmlResult));

                        this.rcbLandmarkCategory.DataSource = dsLandmarks; //dsReports.Tables[0].DefaultView;
                        this.rcbLandmarkCategory.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                iRows = this.rcbLandmarkCategory.Items.Count;
            }
            return (iRows > 0);
        }
        #endregion

        protected void LandmarkCategorySelection_Changed(object sender, RadComboBoxSelectedIndexChangedEventArgs e) {
            this.LoadServiceLandmarkByCategory(this.StringToInt(this.rcbLandmarkCategory.SelectedValue));
        }

        #region Service Landmark by Category
        /// <summary>
        /// 
        /// </summary>
        private bool LoadServiceLandmarkByCategory()
        {
            return LoadServiceLandmarkByCategory(this.StringToInt(this.rcbLandmarkCategory.SelectedValue));
        }

        /// <summary>
        /// 
        /// </summary>
        private bool LoadServiceLandmarkByCategory(int CategoryID)
        {
            return LoadServiceLandmarkByCategory(sn.UserID, sn.User.OrganizationId, CategoryID);
        }

        /// <summary>
        /// Load landmarks into ddl
        /// </summary>
        private bool LoadServiceLandmarkByCategory(int UserID, int OrganizationID, int CategoryID)
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = "";
            int iRows = 0;
            this.ddlServiceLandmarks.Items.Clear();

            try
            {
                using (ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization())
                {
                    if ((int)InterfaceError.NoError == dbo.GetServiceLandmarksByCategory(UserID, UserID.ToString(), OrganizationID, CategoryID, ref xmlResult))
                    {
                        dsLandmarks.ReadXml(new StringReader(xmlResult));

                        this.rcbServiceLandmarkList.DataSource = dsLandmarks; //dsReports.Tables[0].DefaultView;
                        this.rcbServiceLandmarkList.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                //do nothing
            }
            finally
            {
                iRows = this.rcbServiceLandmarkList.Items.Count;
            }
            return (iRows > 0);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void LoadServiceLandmarks()
        {
            LoadServiceLandmarks(this.cboReports.SelectedItem.Value);
        }

        /// <summary>
        /// Load landmarks into ddl
        /// </summary>
        private void LoadServiceLandmarks(string ReportID)
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = ReportID;
            this.ddlServiceLandmarks.Items.Clear();

            try
            {
                using (ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization())
                {
                    //if ((int)InterfaceError.NoError == dbo.GetOrganizationLandmarksXMLByOrganizationId(-sn.UserID, (-sn.UserID).ToString(), -sn.User.OrganizationId, ref xmlResult))
                    if ((int)InterfaceError.NoError == dbo.GetServiceLandmarksByCategory(sn.UserID, (sn.UserID).ToString(), sn.User.OrganizationId, 1004, ref xmlResult))
                    {
                        dsLandmarks.ReadXml(new StringReader(xmlResult));

                        this.ddlServiceLandmarks.DataSource = dsLandmarks; //dsReports.Tables[0].DefaultView;
                        this.ddlServiceLandmarks.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally {
                // Table of Landmark controls
                this.tblLandmarkOptions.Visible = true;
                // Dropdown of Standard Landmarks
                this.ddlLandmarks.Visible = false;
                this.lblLandmarkCaption.Visible = false;
                // Dropdown of Landmarks w. Service Time
                this.ddlServiceLandmarks.Visible = true;
                this.ddlServiceLandmarks.Enabled = true;
                this.lblServiceLandmark.Visible = true;
                // Time of From and To
                this.cboHoursFrom.Enabled = true;
                this.cboHoursTo.Enabled = true;
            }
        }

        private void DateTimeShow(bool visibility) {
            this.DateTimeLabel.Visible = visibility;
            this.DateTimeEntry.Visible = visibility;
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
        /// 
        /// </summary>
        /// <param name="fleetId"></param>
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
                            cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                    return;
                }

                dsVehicle.ReadXml(new StringReader(xml));

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));

                if ((sn.Report.VehicleId != 0) && (sn.Report.VehicleId != -1))
                    cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindItemByValue(sn.Report.VehicleId.ToString()));


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
        /// 
        /// </summary>
        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));

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

        /// <summary>
        /// Get user reports dataset from session, if not valid - use web method
        /// </summary>
        private void GetUserReportsTypes()
        {
            string xml = "";

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

           ////////xml = xml.Replace("34", "10040");         // Prov/state Mileage Detail for Vehicle
        ////////xml = xml.Replace("34", "10039");           // Prov/state Mileage Detail for Fleet (default)
        //////ReplaceReportGUIID("34", "10039", ref xml);
        ////////xml = xml.Replace("36", "10041");           // Prov/state Mileage Summary for Fleet
        //////ReplaceReportGUIID("36", "10041", ref xml);

        //////if (sn.User.OrganizationId != 952)          // G4S
        //////{
        //////    //xml = xml.Replace("39", "10002");           // Activity Summary Report per Vehicle
        //////    ReplaceReportGUIID("39", "10002", ref xml);
        //////    //xml = xml.Replace("38", "10011");           // Activity Summary Report for Organization
        //////    ReplaceReportGUIID("38", "10011", ref xml);
        //////    //xml = xml.Replace("88", "10017");           // Idling Summary Report
        //////    ReplaceReportGUIID("88", "10017", ref xml);
        //////    //xml = xml.Replace("87", "10018");           // Idling Detail Report
        //////    ReplaceReportGUIID("87", "10018", ref xml);
        //////}

        //////switch (sn.User.OrganizationId)
        //////{
        //////    case 951:                                                   // UP
        //////        xml = xml.Replace("Violation", "Infraction");
        //////        //xml = xml.Replace("17", "127");
        //////        ReplaceReportGUIID("17", "127", ref xml);
        //////        //xml = xml.Replace("20", "130");
        //////        ReplaceReportGUIID("20", "130", ref xml);
        //////        //xml = xml.Replace("39", "10002");                     // Activity Summary Report per Vehicle
        //////        //ReplaceReportGUIID("10002", "10048", ref xml);        // MF Vehicle Activity Summary
        //////        ReplaceReportGUIID("10002", "10048", ref xml);             // MF Vehicle Activity Summary
        //////        //xml = xml.Replace("71", "10010");                     // Good vs BadIdling 
        //////        ReplaceReportGUIID("71", "10010", ref xml);
        //////        //xml = xml.Replace("87", "10018");                     // Idling Detail Report
        //////        //ReplaceReportGUIID("10018", "10051", ref xml);        // Idling detail (MF)
        //////        ReplaceReportGUIID("10018", "10051", ref xml);             // Idling detail (MF)
        //////        //xml = xml.Replace("17", "10013");                     // Fleet Violation Details Report
        //////        //ReplaceReportGUIID("17", "10049", ref xml);           // Fleet Violation Detail (MF)
        //////        //xml = xml.Replace("17", "127");                         // Fleet Violation Details Report
        //////        //ReplaceReportGUIID("17", "127", ref xml);
        //////        //xml = xml.Replace("20", "130");
        //////        //ReplaceReportGUIID("20", "130", ref xml);
        //////        //xml = xml.Replace("63", "10020");                     // IVMR
        //////        ReplaceReportGUIID("63", "10020", ref xml);
        //////        //xml = xml.Replace("104", "10015");                    // Speed Violation Details Report by Road Speed
        //////        ReplaceReportGUIID("104", "10056", ref xml);            // MF Speed Violation Detail by Road Speed
        //////        //xml = xml.Replace("97", "10019");                     // Speed Violation Summary by Road Speed
        //////        ReplaceReportGUIID("97", "10057", ref xml);             // MF Speed Violation Summary by Road Speed
        //////        //xml = xml.Replace("28", "10043");                     // Driver Infraction Summary 
        //////        ReplaceReportGUIID("28", "10043", ref xml);
        //////        //xml = xml.Replace("34", "10039");                     // Prov/state Mileage Detail of Fleet
        //////        //ReplaceReportGUIID("34", "10054", ref xml);             // MF P/S Mileage Detail of Fleet
        //////        ReplaceReportGUIID("10039", "10054", ref xml);        // MF P/S Mileage Detail of Fleet
        //////        ////xml = xml.Replace("34", "10040");                     // Prov/state Mileage Detail for Vehicle
        //////        ////ReplaceReportGUIID("10040", "10054", ref xml);        // MF P/S Mileage Detail of Vehicle
        //////        //xml = xml.Replace("36", "10041");                     // Prov/state Mileage Summary for Fleet
        //////        //ReplaceReportGUIID("36", "10055", ref xml);             // MF P/S Mileage Summary of Fleet
        //////        ReplaceReportGUIID("10041", "10055", ref xml);        // MF P/S Mileage Summary of Fleet
        //////        ReplaceReportGUIID("4", "10062", ref xml);              // Stop & Idling
        //////        ReplaceReportGUIID("2", "10063", ref xml);              // Alarms 
        //////        break;



        //////    case 999988:                                    // TDSB
        //////        //xml = xml.Replace("63", "10020");           // IVMR
        //////        ReplaceReportGUIID("63", "10020", ref xml);
        //////        break;

        //////    case 655:                                       // Edgeoil field
        //////        //xml = xml.Replace("28", "10043");           // Driver Infraction Summary
        //////        ReplaceReportGUIID("28", "10043", ref xml);
        //////        break;

        //////    case 489:                                       // Graham Construction
        //////        //xml = xml.Replace("63", "10020");           // IVMR
        //////        ReplaceReportGUIID("63", "10020", ref xml);
        //////        break;

        //////    case 999673:                                    // Falcon Technologies and Service
        //////        //xml = xml.Replace("63", "10020");           // IVMR
        //////        ReplaceReportGUIID("63", "10020", ref xml);
        //////        break;

        //////    case 999756:                                    //BMR
        //////        ReplaceReportGUIID("63", "10020", ref xml); // IVMR
        //////        break;

        //////    case 999722:                                    //BMR
        //////        ReplaceReportGUIID("63", "10020", ref xml); // IVMR
        //////        break;

        //////    case 999994:                                    // Superior Plus Winroc
        //////        xml = xml.Replace("Violation", "Infraction");
        //////        //xml = xml.Replace("63", "10020");           // IVMR
        //////        ReplaceReportGUIID("63", "10020", ref xml);
        //////        break;

        //////    case 1000026:                                     // Bell Canada Inc
        //////        //xml = xml.Replace("39", "10002");           // Activity Summary Report per Vehicle
        //////        ReplaceReportGUIID("10002", "10048", ref xml);// Vehicle Activity Summary (MF)
        //////        //xml = xml.Replace("87", "10018");           // Idling Detail Report
        //////        ReplaceReportGUIID("10018", "10051", ref xml);// Idling detail (MF)
        //////        //xml = xml.Replace("17", "10013");           // Fleet Violation Details Report
        //////        //xml = xml.Replace("17", "10049");           // Fleet Violation Detail (MF)
        //////        ReplaceReportGUIID("17", "10049", ref xml);
        //////        //xml = xml.Replace("20", "10014");           // Fleet Violation Summary Report (MF)
        //////        //xml = xml.Replace("20", "10061");           // Fleet Violation Summary Report (MF)
        //////        ReplaceReportGUIID("20", "10061", ref xml);
        //////        //xml = xml.Replace("34", "10039");           // Prov/state Mileage Detail for Fleet (default)
        //////        xml = xml.Replace("10039", "10054");          // Prov./State Mileage Detail of Fleet (MF)
        //////        //xml = xml.Replace("36", "10041");           // Prov/state Mileage Summary for Fleet
        //////        xml = xml.Replace("10041", "10055");          // Prov./State Mileage Summary of Fleet (MF)
        //////        //xml = xml.Replace("104", "10015");          // Speed Violation Detail by Road Speed
        //////        //xml = xml.Replace("104", "10056");          // Speed Violation Detail by Road Speed (MF)
        //////        ReplaceReportGUIID("104", "10056", ref xml);
        //////        //xml = xml.Replace("97", "10019");           // Speed Violation Summary by Road Speed
        //////        //xml = xml.Replace("97", "10057");           // Speed Violation Summary by Road Speed (MF)
        //////        ReplaceReportGUIID("97", "10057", ref xml);
        //////        break;

        //////    case 1000051:  //LIRR     
        //////    case 1000076:  //Metro North
        //////        ReplaceReportGUIID("4", "10062", ref xml);              // Stop & Idling		
        //////        break;

        //////    case 999630:  //MTS ALLstream
        //////        ReplaceReportGUIID("63", "10020", ref xml);      //IMVR
        //////        ReplaceReportGUIID("4", "10062", ref xml);              // Stop & Idling		
        //////        break;

        //////}



            dsReports.ReadXml(new StringReader(xml));

            sn.Report.UserReportsDataSet = dsReports;

            this.cboReports.DataSource = dsReports; //dsReports.Tables[0].DefaultView;
            this.cboReports.DataBind();

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
                this.ddlDrivers.Items.Add(new ListItem(base.GetLocalResourceObject("ddlDrivers_Item_0").ToString(), "-1"));
				this.ddlDrivers.DataSource = dsDrivers;
                this.ddlDrivers.DataBind();
            }
            else
			{
                //this.ddlDrivers.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlDrivers_NoAvailable").ToString(), "-100"));
                this.ddlDrivers.Items.Add(new ListItem(base.GetLocalResourceObject("ddlDrivers_NoAvailable").ToString(), "-100"));
            }
			

            if ((sn.Report.DriverId != 0) && (sn.Report.DriverId != -1))
                ddlDrivers.SelectedValue = sn.Report.DriverId.ToString();
			else
                ddlDrivers.SelectedValue = "-1";

        }

        protected void ddlReport_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (ddlReport.SelectedIndex == 1)
                Response.Redirect("frmReportMasterExtended_new.aspx");
            else if (ddlReport.SelectedIndex == 2)
                Response.Redirect("frmMyReports.aspx");
            else if (ddlReport.SelectedIndex == 3)
                Response.Redirect("frmSSRS.aspx");

          

        }

        protected void cmdPreviewFleetMaintenanceReport_Click(object sender, EventArgs e)
        {
            //this.BusyReport.Visible = true;
            standardReport = new clsStandardReport();
            GenerateReportParameters();

            //One Time Report
            if (hidSubmitType.Value == "1")
            {
                standardReport.CreateReportParams_MaintenanceReport();
                clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                if (!genReport.CallReportService(sn, this, null, standardReport.keyValue))
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = Resources.Const.Reports_LoadFailed;
                    return;
                }
                RadTabStrip1.FindTabByValue("1").Selected = true;
                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);

            }


            if (hidSubmitType.Value == "2")
            {

                if (!CreateReportParams()) return;


                clsXmlUtil xmlDoc = new clsXmlUtil(sn.Report.XmlDOC);
                xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
                xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
                xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); // new 2008 - 05 - 05
                xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
                xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));

                using (ServerReports.Reports reportProxy = new ServerReports.Reports())
                {
                    lblMessage.Visible = true;
                    if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                        if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning,
                               "Schedule report failed. User: " + sn.UserID.ToString() + " Form:frmReportScheduler.aspx"));
                            ShowMessage(lblMessage, this.GetLocalResourceObject("resScheduleFailed").ToString(), Color.Red);
                            return;
                        }
                }
                ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);

            }

            //My Report
            if (hidSubmitType.Value == "3")
            {
                standardReport.CreateReportParams_MaintenanceReport();
                string xmlDOC = sn.Report.XmlDOC;
                string reportName = clsAsynGenerateReport.PairFindValue("ReportName", xmlDOC);
                string reportDescription = clsAsynGenerateReport.PairFindValue("ReportDescription", xmlDOC);

                clsUserReportParams userReportProperty = new clsUserReportParams();
                userReportProperty.ReportName = reportName;
                userReportProperty.ReportDescription = reportDescription;
                userReportProperty.XmlParams = standardReport.CreateCustomProperty();
                clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                if (!genReport.CallReportService(sn, this, userReportProperty, standardReport.keyValue))
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = Resources.Const.Reports_LoadFailed;
                    return;
                }

                RadTabStrip1.FindTabByValue("1").Selected = true;
                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                //sn.Report 
            }
            //Response.Redirect("frmReportViewer.aspx");
        }

        /// <summary>
        /// Create parameters for clsStandardReport class
        /// </summary>
        private void GenerateReportParameters()
        {
            standardReport.sn = sn;
            standardReport.basepage = this;

            //----
            standardReport.cboReports = cboReports.SelectedValue;

            if (cboReports.SelectedValue == "21" && chkShowDriver.Checked)
                standardReport.cboReports = "131";


            standardReport.chkDoor = chkDoor.Checked;
            standardReport.cboDoorPeriod = cboDoorPeriod.SelectedItem.Value;
            standardReport.chkDriverDoorExc = chkDriverDoorExc.Checked;
            standardReport.chkPassengerDoorExc = chkPassengerDoorExc.Checked;
            standardReport.chkSideHopperDoorExc = chkSideHopperDoorExc.Checked;
            standardReport.chkRearHopperDoorExc = chkRearHopperDoorExc.Checked;
            standardReport.chkSOSMode = chkSOSMode.Checked;
            standardReport.cboSOSLimit = cboSOSLimit.SelectedItem.Value;
            standardReport.chkTAR = chkTAR.Checked;
            standardReport.chkImmobilization = chkImmobilization.Checked;
            standardReport.chkDriverDoor = chkDriverDoor.Checked;
            standardReport.chkLeash = chkLeash.Checked;
            standardReport.chkHistIncludeCoordinate = chkHistIncludeCoordinate.Checked; //Checked
            standardReport.chkHistIncludeSensors = chkHistIncludeSensors.Checked;   //Checked
            standardReport.chkHistIncludeInvalidGPS = chkHistIncludeInvalidGPS.Checked;//Checked
            standardReport.chkHistIncludePositions = chkHistIncludePositions.Checked;//Checked
            standardReport.chkIncludeStreetAddress = chkIncludeStreetAddress.Checked;
            standardReport.chkIncludeSensors = chkIncludeSensors.Checked;
            standardReport.chkIncludePosition = chkIncludePosition.Checked;
            standardReport.chkShowStorePosition = chkShowStorePosition.Checked;
            standardReport.cboStopSequence = cboStopSequence.SelectedItem.Value;
            standardReport.optStopFilter = optStopFilter.SelectedValue;
            standardReport.optStopFilter_0 = optStopFilter.Items[0].Selected;
            standardReport.optStopFilter_1 = optStopFilter.Items[1].Selected;
            standardReport.optStopFilter_2 = optStopFilter.Items[2].Selected;
            standardReport.chkSpeedViolation = chkSpeedViolation.Checked;
            standardReport.cboViolationSpeed = cboViolationSpeed.SelectedValue;
            standardReport.chkHarshAcceleration = chkHarshAcceleration.Checked;
            standardReport.chkExtremeAcceleration = chkExtremeAcceleration.Checked;
            standardReport.chkHarshBraking = chkHarshBraking.Checked; ;
            standardReport.chkExtremeBraking = chkExtremeBraking.Checked;
            standardReport.chkSeatBeltViolation = chkSeatBeltViolation.Checked;
            standardReport.ddlLandmarks = ddlLandmarks.SelectedValue;
            standardReport.ddlGeozones = ddlGeozones.SelectedValue;
            standardReport.ddlDrivers = ddlDrivers.SelectedValue;

            if (ShowOrganizationHierarchy && optMaintenanceBased.SelectedIndex == 0 && hidOrganizationHierarchyFleetId.Value != "")
            {
                standardReport.cboMaintenanceFleet = hidOrganizationHierarchyFleetId.Value;
                standardReport.cboMaintenanceFleet_Name = hidOrganizationHierarchyFleetName.Value;
            }
            else
            {
                standardReport.cboMaintenanceFleet = cboMaintenanceFleet.SelectedValue;
                standardReport.cboMaintenanceFleet_Name = cboMaintenanceFleet.SelectedItem.Text;
            }

            standardReport.cboFleetReportFormat = cboFleetReportFormat.SelectedValue;
            standardReport.chkIncludeIdleTime = chkIncludeIdleTime.Checked;
            standardReport.chkIncludeSummary = chkIncludeSummary.Checked;
            standardReport.cboFromDayH = cboFromDayH.SelectedDate;
            standardReport.cboToDayH = cboToDayH.SelectedDate;
            standardReport.chkWeekend = chkWeekend.Checked;
            standardReport.cboWeekEndFromH = cboWeekEndFromH.SelectedDate;
            standardReport.cboWeekEndToH = cboWeekEndToH.SelectedDate;
            standardReport.chkExcBattery = chkExcBattery.Checked;
            standardReport.chkExcTamper = chkExcTamper.Checked;
            standardReport.chkExcPanic = chkExcPanic.Checked;
            standardReport.chkExcKeypad = chkExcKeypad.Checked;
            standardReport.chkExcGPS = chkExcGPS.Checked;
            standardReport.chkExcAVL = chkExcAVL.Checked;
            standardReport.chkExcLeash = chkExcLeash.Checked;
            standardReport.chkIncCurTARMode = chkIncCurTARMode.Checked;
            standardReport.txtSpeed120 = txtSpeed120.Text;
            standardReport.txtAccHarsh = txtAccHarsh.Text;
            standardReport.txtBrakingExtreme = txtBrakingExtreme.Text;
            standardReport.txtSpeed130 = txtSpeed130.Text;
            standardReport.txtAccExtreme = txtAccExtreme.Text;
            standardReport.txtSeatBelt = txtSeatBelt.Text;
            standardReport.txtSpeed140 = txtSpeed140.Text;
            standardReport.txtBrakingHarsh = txtBrakingHarsh.Text;
            standardReport.txtReverseSpeed = txtReverseSpeed.Text;
            standardReport.txtReverseDistance = txtReverseDistance.Text;
            standardReport.txtHighRail = txtHighRail.Text;
            standardReport.optEndTrip = optEndTrip.SelectedItem.Value;
            standardReport.txtFrom = txtFrom.SelectedDate;
            standardReport.cboHoursFrom = cboHoursFrom.SelectedDate;
            standardReport.txtTo = txtTo.SelectedDate;
            standardReport.cboHoursTo = cboHoursTo.SelectedDate;
            standardReport.txtOver4  = txtOver4.Text;
            standardReport.txtOver10 = txtOver10.Text;
            standardReport.txtOver15 = txtOver15.Text;

            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
                standardReport.cboFleet = Request.Form["OrganizationHierarchyFleetId"];
            else
                standardReport.cboFleet = cboFleet.SelectedItem.Value; //cboFleet.SelectedItem


            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyBoxId"] != "")
            {
                int ibox = StringToInt(Request.Form["OrganizationHierarchyBoxId"].ToString());
                string xml = "";

                if (ibox > 0)
                {
                    ServerDBVehicle.DBVehicle v = new ServerDBVehicle.DBVehicle();
                    if (v.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, ibox, ref xml) == 0)
                    {
                        //standardReport.cboVehicle = Request.Form["OrganizationHierarchyBoxId"];
                        standardReport.cboVehicle = xmlStringValueByTag(xml, "LicensePlate"); //Request.Form["OrganizationHierarchyBoxId"];
                        standardReport.organizationHierarchyBoxId = Request.Form["OrganizationHierarchyBoxId"];
                    }
                }
            }
            else
            {
                standardReport.cboVehicle = cboVehicle.SelectedValue; //cboVehicle.SelectedValue
                standardReport.organizationHierarchyBoxId = "";
            }



            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
                standardReport.organizationHierarchyFleetId = Request.Form["OrganizationHierarchyFleetId"];
            else
                standardReport.organizationHierarchyFleetId = "";
            

            standardReport.cboFormat = cboFormat.SelectedValue;
            standardReport.cboFleet_Name = cboFleet.SelectedItem.Text;

            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyVehicleDescription"] != "")
                standardReport.cboVehicle_Name = Request.Form["OrganizationHierarchyVehicleDescription"];
            else
                standardReport.cboVehicle_Name = cboVehicle.SelectedItem.Text;

            standardReport.keyValue = GenerateKeyValue();
            standardReport.chkLocker1 = chkLocker1.Checked;
            standardReport.chkLocker2 = chkLocker2.Checked;
            standardReport.chkLocker3 = chkLocker3.Checked;
            standardReport.chkLocker4 = chkLocker4.Checked;
            standardReport.chkLocker5 = chkLocker5.Checked;
            standardReport.chkLocker6 = chkLocker6.Checked;
            standardReport.chkLocker7 = chkLocker7.Checked;
            standardReport.chkLocker8 = chkLocker8.Checked;
            standardReport.chkLocker9 = chkLocker9.Checked;
            standardReport.chkShowDriver = chkShowDriver.Checked;

            standardReport.chkIsPostedOnly = chkIsPostedOnly.Checked;

            standardReport.chkReverseSpeed = chkReverseSpeed.Checked;
            standardReport.chkReverseDistance = chkReverseDistance.Checked;
            standardReport.chkHighRail  = chkHighRail.Checked;

            //HOS Audit Report 2014.01.27
            standardReport.chkWorkShiftViolation = chkWorkShiftViolation.Checked;;
            standardReport.chkDailyViolation = chkDailyViolation.Checked;
            standardReport.chkOffDutyViolation = chkOffDutyViolation.Checked;
            standardReport.chkCycleViolation = chkCycleViolation.Checked;
            standardReport.chkPreTripNotDone = chkPreTripNotDone.Checked;
            standardReport.chkPostTripNotDone = chkPostTripNotDone.Checked;
            standardReport.chkDrivingWithDefect = chkDrivingWithDefect.Checked;
            standardReport.chkDriverWithoutSigned = chkDriverWithoutSigned.Checked;
            standardReport.chkLogsNotReceive = chkLogsNotReceive.Checked;
            if (cboReports.SelectedItem.Value == "127" )
            {
                standardReport.chkHyRailReverseSpeed_Extended  = chkHyRailReverseSpeed_Extended.Checked;
                standardReport.chkHyRailSpeed_Extended = chkHyRailSpeed_Extended.Checked;
                standardReport.chkReverseSpeed_Extended = chkReverseSpeed_Extended.Checked;
                standardReport.chkSpeedViolation_Extended  = chkSpeedViolation_Extended.Checked;
                standardReport.cboViolationSpeed_Extended = cboViolationSpeed_Extended.SelectedValue;
                standardReport.chkAcceleration_Extended = chkAcceleration_Extended.Checked;
                standardReport.chkAcceleration_Extended = chkAcceleration_Extended.Checked;
                standardReport.chkBraking_Extended  = chkBraking_Extended.Checked; ;
                standardReport.chkBraking_Extended  = chkBraking_Extended.Checked;
                standardReport.chkSeatBelt_Extended  = chkSeatBelt_Extended.Checked;
                standardReport.chkPostedSpeed_Extended = chkPostedSpeed_Extended.Checked;
                

            }


              if (cboReports.SelectedItem.Value == "130")
            {
                standardReport.chkHyRailReverseSpeed_Extended = HyRailReverseSpeed_ExtendedSummary.Checked;
                standardReport.chkHyRailSpeed_Extended = HyRailSpeed_ExtendedSummary.Checked;
                standardReport.chkReverseSpeed_Extended = ReverseSpeed_ExtendedSummary.Checked;
                standardReport.chkSpeedViolation_Extended = SpeedViolation_ExtendedSummary.Checked;
                standardReport.cboViolationSpeed_Extended = cboViolationSpeed_Extended.SelectedValue;
                standardReport.chkAcceleration_Extended = Acceleration_ExtendedSummary.Checked;
                standardReport.chkBraking_Extended = Braking_ExtendedSummary.Checked; ;
                standardReport.chkPostedSpeed_Extended = chkPostedSpeed_ExtendedSummary.Checked;
		       standardReport.chkSeatBelt_Extended = SeatBelt_ExtendedSummary.Checked;

            }


              //if (cboReports.SelectedItem.Value == "132")
              //{
              //    standardReport.chkSpeedViolation = chkSpeedViolation_Extended.Checked;
              //    standardReport.cboViolationSpeed = cboViolationSpeed_Extended.SelectedValue;
              //    standardReport.chkExtremeAcceleration  = chkExtremeAcceleration.Checked;
              //    standardReport.chkHarshAcceleration  = chkHarshAcceleration .Checked;
              //    standardReport.chkExtremeBraking=  chkExtremeBraking.Checked; ;
              //    standardReport.chkHarshBraking = chkHarshBraking.Checked; ;
              //    standardReport.chkSeatBeltViolation  = chkSeatBelt_Extended.Checked;
              //    standardReport.chkHighRail  = chkReverseSpeed.Checked;
              //}


              //if (cboReports.SelectedItem.Value == "133")
              //{
              //    standardReport.chkSpeedViolation = chkSpeedViolation_Extended.Checked;
              //    standardReport.cboViolationSpeed = cboViolationSpeed_Extended.SelectedValue;
              //    standardReport.chkExtremeAcceleration = chkExtremeAcceleration.Checked;
              //    standardReport.chkHarshAcceleration = chkHarshAcceleration.Checked;
              //    standardReport.chkExtremeBraking = chkExtremeBraking.Checked; ;
              //    standardReport.chkHarshBraking = chkHarshBraking.Checked; ;
              //    standardReport.chkSeatBeltViolation = chkSeatBelt_Extended.Checked;
              //    standardReport.chkHighRail = chkReverseSpeed.Checked;

              //}

              if (cboReports.SelectedItem.Value == "30")
                  sn.Report.XmlParams = chkShowOdometer.Checked.ToString();     
            

            standardReport.cboRoadSpeedDelta = cboRoadSpeedDelta.SelectedItem.Value;
            standardReport.OrganizationHierarchyNodeCode = Request.Form["OrganizationHierarchyNodeCode"];
            sn.Report.ReportType = cboReports.SelectedValue;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdShow_Click(object sender, System.EventArgs e)
        {

	        string message = "";

            int ReportID = Convert.ToInt16(this.cboReports.SelectedValue);

            if (ReportID == 0 || ReportID == 10074){
                if (this.optReportBased.Enabled && this.optReportBased.SelectedIndex == 2)
                    ReportID = 10083;
            }

            if (ReportID > 10000)
            {
                ReportingServices();
            }
            else if (ReportID == 126) // Salt Spreader
            {
                CreateResponsePage("SSRSReports/frmSaltSpreaderSummaryReport.aspx");
            }
            else
            {	
            standardReport = new clsStandardReport();
            GenerateReportParameters();
            if (!CreateReportParams()) return;
            //One Time Report
            if (hidSubmitType.Value == "1")
            {
                clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                if (!genReport.CallReportService(sn, this, null, standardReport.keyValue))
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = Resources.Const.Reports_LoadFailed;

                    return;
                }
                RadTabStrip1.FindTabByValue("1").Selected = true;
                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
            }
            //Schedule Report
            if (hidSubmitType.Value == "2")
            {
                clsXmlUtil xmlDoc = new clsXmlUtil(sn.Report.XmlDOC);
                xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
                xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
                xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat); // new 2008 - 05 - 05
                xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
                xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));

                using (ServerReports.Reports reportProxy = new ServerReports.Reports())
                {
                    lblMessage.Visible = true;
                    if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                        if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning,
                               "Schedule report failed. User: " + sn.UserID.ToString() + " Form:frmReportScheduler.aspx"));
                            ShowMessage(lblMessage, this.GetLocalResourceObject("resScheduleFailed").ToString(), Color.Red);
                            return;
                        }
                }
                ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);

            }

            if (hidSubmitType.Value == "3")
            {
                string xmlDOC = sn.Report.XmlDOC;
                string reportName = clsAsynGenerateReport.PairFindValue("ReportName", xmlDOC);
                string reportDescription = clsAsynGenerateReport.PairFindValue("ReportDescription", xmlDOC);

                clsUserReportParams userReportProperty = new clsUserReportParams();
                userReportProperty.ReportName = reportName;
                userReportProperty.ReportDescription = reportDescription;
                userReportProperty.XmlParams = standardReport.CreateCustomProperty();

                clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                if (!genReport.CallReportService(sn, this, userReportProperty, standardReport.keyValue))
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = Resources.Const.Reports_LoadFailed;
                    return;
                }
                RadTabStrip1.FindTabByValue("1").Selected = true;
                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
            }
          }
        }


        /// <summary>
        /// Create report params and redirect to the next page
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        private Boolean CreateReportParams()
        {
            Boolean ret = true;
            try
            {
                string strFromDate = "", strToDate = "";
                DateTime from, to;
                const string dateFormat = "MM/dd/yyyy HH:mm:ss";

                this.lblMessage.Text = "";

                # region Validation


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

                    if (((this.optReportBased.SelectedIndex == 1 && this.cboVehicle.SelectedIndex == 0)
                        || (this.optReportBased.SelectedIndex == 0 && this.OrganizationHierarchyBoxId.Value == ""))
                        && (this.cboReports.SelectedValue == "40" || this.cboReports.SelectedValue == "63" || this.cboReports.SelectedValue == "10020"))
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectVehicle");
                        return false;
                    }
                }

                else if (this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "40" || this.cboReports.SelectedValue == "63" || this.cboReports.SelectedValue == "10020"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectVehicle");
                    return false;
                }


                if (this.ddlLandmarks.SelectedIndex == 0 && (this.cboReports.SelectedValue == "21" || this.cboReports.SelectedValue == "23" || this.cboReports.SelectedValue == "41" || this.cboReports.SelectedValue == "10066" || this.cboReports.SelectedValue == "10067"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectLandmark");
                    return false;
                }

                if (this.cboReports.SelectedValue != "19" && this.cboReports.SelectedValue != "25" && this.cboReports.SelectedValue != "38" && this.cboReports.SelectedValue != "53" && this.cboReports.SelectedValue != "88")
                {
                    if (this.cboFleet.SelectedIndex == 0 && !ShowOrganizationHierarchy && optReportBased.SelectedIndex != 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("valFleetMessage");
                        return false;
                    }
                }

                // check driver
                if (this.cboReports.SelectedValue == "25" || this.cboReports.SelectedValue == "26" || this.cboReports.SelectedValue == "27" || this.cboReports.SelectedValue == "28" || this.cboReports.SelectedValue == "53")
                {
                    if (this.ddlDrivers.SelectedIndex == 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("valDriver");
                        return false;
                    }
                }

                if ((this.ddlGeozones.SelectedIndex == 0) && (this.cboReports.SelectedValue == "22"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("ddlGeozones_Item_0");
                    return false;
                }

                strFromDate = txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + String.Format("{0:t}", cboHoursFrom.SelectedDate.Value);
                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                //    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 0)
                //    strFromDate = this.txtFrom.Text + " " + "12:00 AM";

                //if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                //    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + String.Format("{0:t}", this.cboHoursTo.SelectedDate.Value);
                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                //    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 0)
                //    strToDate = this.txtTo.Text + " " + "12:00 AM";

                //if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                //    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

                from = Convert.ToDateTime(strFromDate, ci);
                to = Convert.ToDateTime(strToDate, ci);

                if (from >= to)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
                    return false;
                }
                else
                {
                    this.lblMessage.Visible = false;
                    this.lblMessage.Text = "";
                }

                if (((this.cboVehicle.SelectedIndex == 0 && this.cboReports.SelectedValue == "3") ||
                    this.cboVehicle.SelectedIndex == -1) && cboVehicle.Visible == true) //&& cboVehicle.Visible == true added by devin
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                    return false;
                }
                else
                {
                    this.lblMessage.Visible = false;
                    this.lblMessage.Text = "";
                }

                TimeSpan ts = to - from;
                int reportDaysLimit = 31;

                if (standardReport.cboReports == "10002" || standardReport.cboReports == "10011" || standardReport.cboReports == "10020")
                {
                    reportDaysLimit = 100;
                }

                if (ts.Days > reportDaysLimit)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = string.Format(GetLocalResourceObject("MessageRptDateRange").ToString(), reportDaysLimit);
                    return false;
                }





                DataSet ds = new DataSet();
                string xmlResult = "";

                using (CrystalRpt.CrystalRpt cr = new CrystalRpt.CrystalRpt())
                {
                    if (objUtil.ErrCheck(cr.OrganizationHistoryDateRangeValidation(sn.UserID, sn.SecId, from.ToString(), to.ToString(), ref xmlResult), false))
                        if (objUtil.ErrCheck(cr.OrganizationHistoryDateRangeValidation(sn.UserID, sn.SecId, from.ToString(), to.ToString(), ref xmlResult), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " OrganizationHistoryDateRangeValidation:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return false;
                        }
                }

                if (String.IsNullOrEmpty(xmlResult))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "OrganizationHistoryDateRangeValidation:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return false;
                }


                ds.ReadXml(new StringReader(xmlResult));
                if (ds.Tables[0].Rows[0]["InValidCall"].ToString() == "1")
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Please decrease report date/time range! The from date should be greather than:" + System.DateTime.Now.AddDays(-Convert.ToInt16(ds.Tables[0].Rows[0]["MaximumDays"].ToString())).ToShortDateString();
                    return false;
                }
                # endregion



                if (this.cboReports.SelectedValue == "130")

                    sn.Report.TmpData = this.txtSpeed_65_MPH_ExtSummary.Text + "," + this.txtSpeed_75_MPH_ExtSummary.Text + "," + this.txtSpeed_80_MPH_ExtSummary.Text + "," + this.txtSpeed4_OverPosted_ExtSummary.Text + "," + this.txt_Speed10_OverPosted_ExtSummary.Text
                        + "," + this.txt_Speed15_OverPosted_ExtSummary.Text + "," + this.txtBraking_ExtSummary.Text + "," + this.txtAcceleration_ExtSummary.Text + "," + this.txtSeatBelt_ExtSummary.Text + "," + this.txtReverseSpeed_ExtSummary.Text+","+this.txtHyRail_Reverse_Speed_ExtSummary.Text+","+this.txtHyRailSpeed_ExtSummary.Text ;

  
                ret = standardReport.CreateReportParams(cboVehicle);

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
            return ret;
            //Response.Redirect(redirectUrl);
        }



        protected void btnLogin_Click(object sender, EventArgs e)
        {
            RedirectToLogin();
        }

        //For execute
        protected void cmdShowMyReport_Click(object sender, System.EventArgs e)
        {
            standardReport = new clsStandardReport();
            GenerateReportParameters();
            if (!CreateReportParams()) return;
            clsAsynGenerateReport genReport = new clsAsynGenerateReport();
            if (!genReport.CallReportService(sn, this, null, standardReport.keyValue))
            {
                lblMessage.Visible = true;
                lblMessage.Text = Resources.Const.Reports_LoadFailed;
                return;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "frmMyReports_Backto_Repository", "Sys.Application.add_load(parent.frmMyReports_Backto_Repository)", true);

        }

        //For execute and update
        protected void cmdShowMyReportUpdate_Click(object sender, System.EventArgs e)
        {
            standardReport = new clsStandardReport();
            GenerateReportParameters();
            if (!CreateReportParams()) return;

            clsUserReportParams userReportProperty = new clsUserReportParams();
            userReportProperty.ReportRepositoryId = Request.QueryString["id"].ToString();
            userReportProperty.XmlParams = standardReport.CreateCustomProperty();

            clsAsynGenerateReport genReport = new clsAsynGenerateReport();
            if (!genReport.CallReportService(sn, this, userReportProperty, standardReport.keyValue))
            {
                lblMessage.Visible = true;
                lblMessage.Text = Resources.Const.Reports_LoadFailed;
                return;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "frmMyReports_Backto_Repository", "Sys.Application.add_load(parent.frmMyReports_Backto_Repository_u)", true);
        }

        //For execute MaintenanceReportMyReport
        protected void cmdFleetMaintenanceReportMyReport_Click(object sender, System.EventArgs e)
        {
            //this.BusyReport.Visible = true;
            standardReport = new clsStandardReport();
            GenerateReportParameters();

            standardReport.CreateReportParams_MaintenanceReport();

            clsAsynGenerateReport genReport = new clsAsynGenerateReport();
            if (!genReport.CallReportService(sn, this, null, standardReport.keyValue))
            {
                lblMessage.Visible = true;
                lblMessage.Text = Resources.Const.Reports_LoadFailed;
                return;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "frmMyReports_Backto_Repository", "Sys.Application.add_load(parent.frmMyReports_Backto_Repository)", true);

        }

        //For execute and update MaintenanceReportMyReport
        protected void cmdFleetMaintenanceReportMyReportUpdate_Click(object sender, System.EventArgs e)
        {
            //this.BusyReport.Visible = true;
            standardReport = new clsStandardReport();
            GenerateReportParameters();

            standardReport.CreateReportParams_MaintenanceReport();


            clsUserReportParams userReportProperty = new clsUserReportParams();
            userReportProperty.XmlParams = standardReport.CreateCustomProperty();
            userReportProperty.ReportRepositoryId = Request.QueryString["id"].ToString();
            clsAsynGenerateReport genReport = new clsAsynGenerateReport();
            if (!genReport.CallReportService(sn, this, userReportProperty, standardReport.keyValue))
            {
                lblMessage.Visible = true;
                lblMessage.Text = Resources.Const.Reports_LoadFailed;
                return;
            }

            //sn.Report 
            ScriptManager.RegisterStartupScript(this, this.GetType(), "frmMyReports_Backto_Repository", "Sys.Application.add_load(parent.frmMyReports_Backto_Repository_u)", true);

        }

        /// <summary>
        /// Fill all controls 
        /// </summary>
        private void FillControlsByparameters()
        {
            try
            {
                cboReports.FindItemByValue(standardReport.cboReports).Selected = true;
                cboReports_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { }
            if (cboFleet.Visible)
            {
                try
                {
                    cboFleet.FindItemByValue(standardReport.cboFleet).Selected = true;
                    cboFleet_SelectedIndexChanged(null, null);
                }
                catch (Exception ex) { }
            }

            chkDoor.Checked = standardReport.chkDoor;
            try
            {
                cboDoorPeriod.FindItemByValue(standardReport.cboDoorPeriod).Selected = true;
            }
            catch (Exception ex) { }

            chkDriverDoorExc.Checked = standardReport.chkDriverDoorExc;
            chkPassengerDoorExc.Checked = standardReport.chkPassengerDoorExc;
            chkSideHopperDoorExc.Checked = standardReport.chkSideHopperDoorExc;
            chkRearHopperDoorExc.Checked = standardReport.chkRearHopperDoorExc;
            chkSOSMode.Checked = standardReport.chkSOSMode;
            try
            {
                cboSOSLimit.FindItemByValue(standardReport.cboSOSLimit).Selected = true;
            }
            catch (Exception ex) { }
            chkTAR.Checked = standardReport.chkTAR;
            chkImmobilization.Checked = standardReport.chkImmobilization;
            chkDriverDoor.Checked = standardReport.chkDriverDoor;
            chkLeash.Checked = standardReport.chkLeash;
            chkHistIncludeCoordinate.Checked = standardReport.chkHistIncludeCoordinate; //Checked
            chkHistIncludeSensors.Checked = standardReport.chkHistIncludeSensors;   //Checked
            chkHistIncludeInvalidGPS.Checked = standardReport.chkHistIncludeInvalidGPS; //Checked
            chkHistIncludePositions.Checked = standardReport.chkHistIncludePositions;//Checked
            chkIncludeStreetAddress.Checked = standardReport.chkIncludeStreetAddress;
            chkIncludeSensors.Checked = standardReport.chkIncludeSensors;
            chkIncludePosition.Checked = standardReport.chkIncludePosition;
            chkShowStorePosition.Checked = standardReport.chkShowStorePosition;
            try
            {
                cboStopSequence.FindItemByValue(standardReport.cboStopSequence).Selected = true;
            }
            catch (Exception ex) { }
            optStopFilter.Items[0].Selected = standardReport.optStopFilter_0;
            optStopFilter.Items[1].Selected = standardReport.optStopFilter_1;
            optStopFilter.Items[2].Selected = standardReport.optStopFilter_2;
            chkSpeedViolation.Checked = standardReport.chkSpeedViolation;
            try
            {
                cboViolationSpeed.FindItemByValue(standardReport.cboViolationSpeed).Selected = true;
            }
            catch (Exception ex) { }
            chkHarshAcceleration.Checked = standardReport.chkHarshAcceleration;
            chkExtremeAcceleration.Checked = standardReport.chkExtremeAcceleration;
            chkHarshBraking.Checked = standardReport.chkHarshBraking;
            chkExtremeBraking.Checked = standardReport.chkExtremeBraking;
            chkSeatBeltViolation.Checked = standardReport.chkSeatBeltViolation;
            try
            {
                ddlLandmarks.FindItemByValue(standardReport.ddlLandmarks).Selected = true;
            }
            catch (Exception ex) { }

            try
            {
                ddlGeozones.FindItemByValue(standardReport.ddlGeozones).Selected = true;
            }
            catch (Exception ex) { }
            try
            {
                //ddlDrivers.FindItemByValue(standardReport.ddlDrivers).Selected = true;
                ddlDrivers.SelectedValue = standardReport.ddlDrivers;
            }
            catch (Exception ex) { }

            try
            {
                cboMaintenanceFleet.FindItemByValue(standardReport.cboMaintenanceFleet).Selected = true;
            }
            catch (Exception ex) { }

            try
            {
                cboFleetReportFormat.FindItemByValue(standardReport.cboFleetReportFormat).Selected = true;
            }
            catch (Exception ex) { }
            chkIncludeIdleTime.Checked = standardReport.chkIncludeIdleTime;
            chkIncludeSummary.Checked = standardReport.chkIncludeSummary;
            cboFromDayH.SelectedDate = standardReport.cboFromDayH;
            cboToDayH.SelectedDate = standardReport.cboToDayH;
            chkWeekend.Checked = standardReport.chkWeekend;
            chkWeekend_CheckedChanged(null, null);

            cboWeekEndFromH.SelectedDate = standardReport.cboWeekEndFromH;
            cboWeekEndToH.SelectedDate = standardReport.cboWeekEndToH;
            chkExcBattery.Checked = standardReport.chkExcBattery;
            chkExcTamper.Checked = standardReport.chkExcTamper;
            chkExcPanic.Checked = standardReport.chkExcPanic;
            chkExcKeypad.Checked = standardReport.chkExcKeypad;
            chkExcGPS.Checked = standardReport.chkExcGPS;
            chkExcAVL.Checked = standardReport.chkExcAVL;
            chkExcLeash.Checked = standardReport.chkExcLeash;
            chkIncCurTARMode.Checked = standardReport.chkIncCurTARMode;
            txtSpeed120.Text = standardReport.txtSpeed120;
            txtAccHarsh.Text = standardReport.txtAccHarsh;
            txtBrakingExtreme.Text = standardReport.txtBrakingExtreme;
            txtSpeed130.Text = standardReport.txtSpeed130;
            txtAccExtreme.Text = standardReport.txtAccExtreme;
            txtSeatBelt.Text = standardReport.txtSeatBelt;
            txtSpeed140.Text = standardReport.txtSpeed140;
            txtBrakingHarsh.Text = standardReport.txtBrakingHarsh;
            txtReverseSpeed.Text = standardReport.txtReverseSpeed;
            txtReverseDistance.Text = standardReport.txtReverseSpeed;
            txtHighRail.Text = standardReport.txtHighRail;

            //HOS Audit Report 2014.01.27
            chkWorkShiftViolation.Checked = standardReport.chkWorkShiftViolation;
            chkDailyViolation.Checked = standardReport.chkDailyViolation;
            chkOffDutyViolation.Checked = standardReport.chkOffDutyViolation;
            chkCycleViolation.Checked = standardReport.chkCycleViolation;
            chkPreTripNotDone.Checked = standardReport.chkPreTripNotDone;
            chkPostTripNotDone.Checked = standardReport.chkPostTripNotDone;
            chkDrivingWithDefect.Checked = standardReport.chkDrivingWithDefect;
            chkDriverWithoutSigned.Checked = standardReport.chkDriverWithoutSigned;
            chkLogsNotReceive.Checked = standardReport.chkLogsNotReceive;
            try
            {
                optEndTrip.Items.FindByValue(standardReport.optEndTrip).Selected = true;
            }
            catch (Exception ex) { }
            txtFrom.SelectedDate = standardReport.txtFrom;
            cboHoursFrom.SelectedDate = standardReport.cboHoursFrom;
            txtTo.SelectedDate = standardReport.txtTo;
            cboHoursTo.SelectedDate = standardReport.cboHoursTo;
            try
            {
                cboVehicle.FindItemByValue(standardReport.cboVehicle).Selected = true; //cboVehicle.SelectedValue
            }
            catch (Exception ex) { }
            try
            {
                cboFormat.FindItemByValue(standardReport.cboFormat).Selected = true;
            }
            catch (Exception ex) { }

            chkLocker1.Checked = standardReport.chkLocker1;
            chkLocker2.Checked = standardReport.chkLocker2;
            chkLocker3.Checked = standardReport.chkLocker3;
            chkLocker4.Checked = standardReport.chkLocker4;
            chkLocker5.Checked = standardReport.chkLocker5;
            chkLocker6.Checked = standardReport.chkLocker6;
            chkLocker7.Checked = standardReport.chkLocker7;
            chkLocker8.Checked = standardReport.chkLocker8;
            chkLocker9.Checked = standardReport.chkLocker9;
            chkShowDriver.Checked = standardReport.chkShowDriver;

            chkReverseSpeed.Checked = standardReport.chkReverseSpeed;
            chkReverseDistance.Checked = standardReport.chkReverseDistance;
            chkHighRail.Checked = standardReport.chkHighRail;
            chkIsPostedOnly.Checked = standardReport.chkIsPostedOnly;
            chkSpeedViolation_Extended.Checked = standardReport.chkSpeedViolation_Extended;
            try
            {
                cboRoadSpeedDelta.FindItemByValue(standardReport.cboRoadSpeedDelta).Selected = true; //cboVehicle.SelectedValue
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Generate key value for report repository table
        /// </summary>
        /// <param name="isFleetMaintenanceReport"></param>
        /// <returns></returns>
        private string GenerateKeyValue()
        {
            bool isFleetMaintenanceReport = false;
            if (this.cboReports.SelectedValue == "10") isFleetMaintenanceReport = true;
            if (isFleetMaintenanceReport)
            {
                if (ShowOrganizationHierarchy && optMaintenanceBased.SelectedIndex == 0 && hidOrganizationHierarchyFleetId.Value != "")
                {
                    string keyvalue_fleet = (string)base.GetLocalResourceObject("keyvalue_fleet");
                    return string.Format("<b>{0}</b>={1}", keyvalue_fleet, hidOrganizationHierarchyFleetName.Value);                    
                    
                }
                else if (cboMaintenanceFleet.Visible && cboMaintenanceFleet.Enabled)
                {
                    string keyvalue_fleet = (string)base.GetLocalResourceObject("keyvalue_fleet");
                    return string.Format("<b>{0}</b>={1}", keyvalue_fleet, cboMaintenanceFleet.SelectedItem.Text.Trim());
                }
                else return string.Empty;
            }
            else
            {

                if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0)
                {
                    string fid = Request.Form["OrganizationHierarchyFleetId"];
                    if (!string.IsNullOrEmpty(fid))
                    { 
                        if(isMultiFleet(fid))
                            sn.Report.FleetId = 0;
                        else if(isNumeric(fid))
                            sn.Report.FleetId = Convert.ToInt32(fid);
                        else
                            sn.Report.FleetId = 0;
                    }
                }
                else
                    sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);

                string keyvalue_fleet = (string)base.GetLocalResourceObject("keyvalue_fleet");
                string keyvalue_vehicle = (string)base.GetLocalResourceObject("keyvalue_vehicle");
                string keyvalue_landmark = (string)base.GetLocalResourceObject("keyvalue_landmark");
                string keyvalue_driver = (string)base.GetLocalResourceObject("keyvalue_driver");
                string keyvalue_geozone = (string)base.GetLocalResourceObject("keyvalue_geozone");
                string keyvalue_startdate = (string)base.GetLocalResourceObject("keyvalue_startdate");
                string keyvalue_enddate = (string)base.GetLocalResourceObject("keyvalue_enddate");
                StringBuilder sb = new StringBuilder();
                //if (cboFleet.Visible && cboFleet.Enabled && cboFleet.SelectedIndex > 0 && cboFleet.Items.Count > 0)

                if (isMultiFleet(Request.Form["OrganizationHierarchyFleetId"]))
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_fleet, "Multi-Fleet:" + Request.Form["OrganizationHierarchyFleetId"]));
                else
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_fleet, sn.Report.FleetId.ToString()));





                if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyVehicleDescription"] != "")
                {
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_vehicle, Request.Form["OrganizationHierarchyVehicleDescription"]));
                }
                else
                {
                    if (cboVehicle.Visible && cboVehicle.Enabled && cboVehicle.Items.Count > 0)
                    {
                        if (cboVehicle.SelectedIndex >= 0)
                            sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_vehicle, cboVehicle.SelectedItem.Text));
                        else
                            sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_vehicle, cboVehicle.Items[0].Text));
                    }
                }

                if (ddlLandmarks.Visible && ddlLandmarks.Enabled && ddlLandmarks.SelectedIndex > 0 && ddlLandmarks.Items.Count > 0)
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_landmark, ddlLandmarks.SelectedItem.Text));

                if (ddlDrivers.Visible && ddlDrivers.Enabled && ddlDrivers.SelectedIndex > 0 && ddlDrivers.Items.Count > 0)
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_driver, ddlDrivers.SelectedItem.Text));

                if (ddlGeozones.Visible && ddlGeozones.Enabled && ddlGeozones.SelectedIndex > 0 && ddlGeozones.Items.Count > 0)
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_geozone, ddlGeozones.SelectedItem.Text));

                if (txtFrom.Visible)
                {
                    string startDate = txtFrom.SelectedDate.Value.ToString("M/d/yyyy") + " " + String.Format("{0:HH:mm}", cboHoursFrom.SelectedDate.Value);
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_startdate, startDate));
                }

                if (txtTo.Visible)
                {
                    string endDate = txtTo.SelectedDate.Value.ToString("M/d/yyyy") + " " + String.Format("{0:HH:mm}", cboHoursTo.SelectedDate.Value);
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_enddate, endDate));
                }
                return sb.ToString();

            }
        }

        protected void optReportBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportBasedOption();
        }

        protected void optMaintenanceBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            MaintenanceReportBasedOption();
        }

        private void ReportBasedOption()
        {
            if (cboReports.SelectedItem != null)
            {
                string ReportID = cboReports.SelectedItem.Value;

                if (ReportID == "10075" || ReportID == "10082" || ReportID == "0" || ReportID == "10074" || ReportID == "10083")
                {
                    this.vehicleSelectOption.Visible = true;
                }
                else if (ReportID == "10084" || ReportID == "10002" || ReportID == "10048" || ReportID == "10018" || ReportID == "10051")
                {
                    this.vehicleSelectOption.Visible = true;
                }
            }

            if (this.vehicleSelectOption.Visible)
            {
                switch (optReportBased.SelectedItem.Value)
                {
                    case "1":   // Fleet

                        trFleet.Visible = true;
                       // FleetVehicleShow(true);
                        this.trReportDriver.Visible = false;
                        this.chkAllDrivers.Visible = false;
                        this.organizationHierarchy.Visible = false;
                        sn.Report.OrganizationHierarchySelected = false;

                        break;

                    case "2":   // Driver

                        trFleet.Visible = false;
                       // FleetVehicleShow(false);
                        this.trReportDriver.Visible = true;
                        organizationHierarchy.Visible = false;
                        sn.Report.OrganizationHierarchySelected = false;

                        if (cboReports.SelectedItem.Value == "10084")
                            this.chkAllDrivers.Visible = true;
                        else
                            this.chkAllDrivers.Visible = false;

                        break;
                    
                    default:    // Hierarchy

                        this.trFleet.Visible = false;
                        //FleetVehicleShow(false);
                        this.trReportDriver.Visible = false;
                        this.chkAllDrivers.Visible = false;
                        this.organizationHierarchy.Visible = true;
                        sn.Report.OrganizationHierarchySelected = true;

                        break;
                }
            }
        }

        private void MaintenanceReportBasedOption()
        {
            if (optMaintenanceBased.SelectedItem.Value == "1") 
            {
                trMaintenanceHierarchy.Visible = false;
                trMaintenanceFleet.Visible = true;
                trMaintenanceDriver.Visible = false;
            }
            else if (optMaintenanceBased.SelectedItem.Value == "2")
            {
                trMaintenanceHierarchy.Visible = false;
                trMaintenanceFleet.Visible = false;
                trMaintenanceDriver.Visible = true;

            }
            else
            {
                trMaintenanceHierarchy.Visible = true;
                trMaintenanceFleet.Visible = false;
                trMaintenanceDriver.Visible = false;
            }
        }

        private void MaintenanceReportBasedOption(int SelectedItemIndex)
        {
            switch (SelectedItemIndex)
            {
                case 1:
                    trMaintenanceHierarchy.Visible = false;
                    trMaintenanceFleet.Visible = true;
                    trMaintenanceDriver.Visible = false;
                    break;
                case 2:
                    trMaintenanceHierarchy.Visible = false;
                    trMaintenanceFleet.Visible = false;
                    trMaintenanceDriver.Visible = true;
                    break;
                default:
                    trMaintenanceHierarchy.Visible = true;
                    trMaintenanceFleet.Visible = false;
                    trMaintenanceDriver.Visible = false;
                    break;
            }
        }

        private void HideHierarchy()
        {
            trFleet.Visible = true;
            organizationHierarchy.Visible = false;
            this.optReportBased.SelectedIndex = 1;
            vehicleSelectOption.Visible = false;
            hiddenHierarchy = true;
        }

        protected void btnAfterCreate_Click(object sender, EventArgs e)
        {
            ReportCriteria();
        }


        /// <summary>
        /// Build parameters string in json format 
        /// </summary>
        /// <returns></returns>
        public string GenerateReportParameters_JSON(int ReportID)      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {
            ReportID = Math.Abs(ReportID);

            StringBuilder sbp = new StringBuilder();

            // Basic parameters
            sbp.Append("reportid: " + ReportID.ToString() + ", ");                      // this.cboReports.SelectedItem.Value + ", ");
            sbp.Append("reportformat: " + cboFormat.SelectedItem.Text + ", ");          // PDF; EXCEL; WORD;....   .SelectedValue.ToString()
            sbp.Append("reportformatcode: " + cboFormat.SelectedItem.Value + ", ");     // 1;   2;     3;   ....   .SelectedValue.ToString()
            // Application Logon User
            sbp.Append("userid: " + sn.UserID + ", ");
            //Time zone
            sbp.Append("timezone: GMT" + sn.User.TimeZone.ToString() + ", ");
            // User language
            sbp.Append("language: " + sn.SelectedLanguage + ", ");       //  SystemFunctions.GetStandardLanguageCode(HttpContext.Current) + ", ");
            // Organization
            sbp.Append("organization: " + sn.User.OrganizationId + ", ");

//            // Fleet
//            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
//                sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
//            else if (cboFleet.Visible && cboFleet.Enabled)
//                sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
//            else
//                sbp.Append("");

//            // Vehicle ID
//            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyBoxId"] != "")
//                sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + ", ");       // Box ID
//            else if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
//                sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
//            else
//                sbp.Append("");

//            // Vehicle Name
//            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyVehicleDescription"] != "")
//                sbp.Append("vehiclename: " + Request.Form["OrganizationHierarchyVehicleDescription"] + ", ");   //standardReport.cboVehicle_Name = Request.Form["OrganizationHierarchyVehicleDescription"];
//            else if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
//                sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
//            else
//                sbp.Append("");

            // Hierarchy ~ Fleet
            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0)
            {
                if (Request.Form["OrganizationHierarchyFleetId"] != "")
                {
                    sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
                    sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
                }

                if (Request.Form["OrganizationHierarchyBoxId"] != "")
                    sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + ", ");       // Box ID
                else 
                { // if Mileage Detail Report for Vehicle ()default, switch to for Fleet.
                    if (ReportID == 10040)
                        sbp.Replace("10040", "10039");
                }

                if (Request.Form["OrganizationHierarchyVehicleDescription"] != "")
                    sbp.Append("vehiclename: " + Request.Form["OrganizationHierarchyVehicleDescription"] + ", ");   //standardReport.cboVehicle_Name
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
                    if (this.cboVehicle.SelectedItem.Value == "0")
                    {// Switch to Mileage detail for Fleet if if for Vehicle.
                        if (ReportID == 10040)
                            sbp.Replace("10040", "10039");
                    }
                    else
                    {
                        sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
                        sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
                    }
                }
            }

            // Date range
            string df = txtFrom.SelectedDate.Value.ToString("yyyy/MM/dd") + " " + cboHoursFrom.SelectedDate.Value.ToString("hh:mm:ss tt");
            string dt = txtTo.SelectedDate.Value.ToString("yyyy/MM/dd") + " " + cboHoursTo.SelectedDate.Value.ToString("hh:mm:ss tt");

            sbp.Append("datefrom: " + df + ", ");
            //sbp.Append("datefrom: " + Functions.FormattedDateTimeString(txtFrom.SelectedDate.ToString(), "yyyy/MM/dd hh:mm:ss") + ", ");
            sbp.Append("dateto: " + dt + ", ");
            //sbp.Append("dateto: " + Functions.FormattedDateTimeString(txtTo.SelectedDate.ToString(), "yyyy/MM/dd hh:mm:ss") + ", ");

            sbp.Append("unitofvolume: " + sn.User.VolumeUnits + ", ");
            sbp.Append("unitofspeed: " + sn.User.UnitOfMes + ", ");


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

            #region Fleet Utilization

            //if (tblIdlingCost.Visible)
            //{
            //    if (txtCost.Visible && txtCost.Enabled)
            //    {
            //        if (isNumeric(txtCost.Text))
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

            // sensorNum = 3 (default)
            if (this.optEndTrip.Visible && this.optEndTrip.Enabled)
            {
                if (optEndTrip.SelectedIndex >= 0)
                    sbp.Append("sensornumber: " + optEndTrip.SelectedValue + ", ");
                else
                    sbp.Append("sensornumber: 3, ");
            }

            if (sbp.Length > 0)
                return "{" + sbp.ToString() + "}";
            else
                return "";

        }


        public bool GetReportDetail(string msReportID)
        {
            string msMessage = "";
            try
            {
                string cnStr = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(cnStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_ReportDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ReportID", SqlDbType.Int);
                        command.Parameters["@ReportID"].Value = msReportID;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //msReportCategory = reader[0].ToString();
                                //msReportUri = reader[1].ToString();
                                //msReportPath = reader[2].ToString();
                                //msReportName = reader[3].ToString();
                                //msReportPage = reader[4].ToString();
                                //msReportType = reader[5].ToString();
                                msMessage = "";
                            }
                            else
                            {
                                //msReportCategory = "";
                                //msReportUri = "";
                                //msReportPath = "";
                                //msReportName = "";
                                //msReportPage = "";
                                //msReportType = "";
                                //msMessage = "Report not found";
                            }
                        }
                    }
                }
            }
            catch (SqlException Ex)
            {
                msMessage = Ex.Message.ToString();
            }
            catch (Exception Ex)
            {
                msMessage = Ex.Message.ToString();
            }
            finally
            {

            }

            return (msMessage == string.Empty) ? true : false;
        }

        /// <summary>
        /// for async call parameter
        /// </summary>
        /// <param name="iResult"></param>
        private void wsCallback(IAsyncResult iResult) { }

        private string GetControlObjectValue_int(string objValue, string defValue) {
                return (this.isNumeric(objValue))? objValue: defValue;
        }

        /// <summary>
        /// Get violation mask integer number 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetViolationExtendedMaskNumber()
        {
            int intCriteria = 0;

            
            if (this.chkSpeedViolation_Extended.Checked)                                                             // (mbViolationOverSpeed)
                intCriteria |= 0x0001;
           
            if (this.chkAcceleration_Extended.Checked)                                                          // (mbViolationHarshAcceleration) 
                 intCriteria |= 0x0002;
         
            if (this.chkBraking_Extended.Checked)                                                               // (mbViolationHarshBraking)
                   intCriteria |= 0x0004;
          
           
            if (this.chkSeatBelt_Extended.Checked)                                                        // (mbViolationSeatBelt)
                intCriteria |= 0x0008;


            
            if (this.chkHyRailReverseSpeed_Extended.Checked)                                                        // (mbViolationSeatBelt)
                intCriteria |= 0x0010;

            if (this.chkHyRailSpeed_Extended.Checked)                                                        // (mbViolationSeatBelt)
                intCriteria |= 0x0020;

            if (this.chkReverseSpeed_Extended.Checked)                                                        // (mbViolationSeatBelt)
                intCriteria |= 0x0040;

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
        /// Get violation mask string
        /// </summary>
        /// <returns></returns>
        private string GetViolationMaskString(int ReportID)
        {
            string mask = "";

            switch (ReportID) { 
                case 10110:
                    mask = GetViolationMask_RagFatigue().ToString();
                    break;
                default:
                    mask = GetViolationMaskNumber().ToString();
                    break;
            }

            return mask;
        }

        /// <summary>
        /// 
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
                    if (sn.User.UnitOfMes == 1)
                        speed = "140";
                    else
                        speed = "145";  // 90
                    break;
                case "7":
                    speed = "130";  //80
                    break;
                case "6":
                    speed = "125";  //77
                    break;
                case "5":
                    speed = "120"; //75
                    break;
                case "4":
                    speed = "115"; //71
                    break;
                case "3":
                    speed = "110";  //68
                    break;
                case "2":
                    speed = "105";  //65
                    break;
                case "1":
                default:
                    speed = "100";  //62
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

        public string xmlStringValueByTag(string xmlString, string xmlTag) 
        {
            StringReader XML = new StringReader(xmlString);
            DataSet dsPref = new DataSet();
            dsPref.ReadXml(XML);
            DataRow dr = dsPref.Tables[0].Rows[0];
            return dr[xmlTag].ToString();
        }

        public bool isMultiFleet(string Fleets) {
            if(isNumericInteger(Fleets))
                return false; 
            else
                return IsValidFleetIDString(Fleets);
        }

        /// <summary>
        /// Creates frameset page with url and back button
        /// </summary>
        /// <param name="url"></param>
        private void CreateResponsePage(string url)
        {
            StringBuilder pageBuilder =
               new StringBuilder("<html><frameset id=\"TopFrame\" rows=\"*,24px\" frameSpacing=\"0\" border=\"0\" bordercolor=\"gray\" frameBorder=\"0\">");
            pageBuilder.AppendFormat("<frame name=\"report\" src='{0}' scrolling=auto  frameborder=\"0\" noresize />", url);
            pageBuilder.AppendLine("<frame name=\"reportback\" src=\"\" scrolling=\"no\" frameborder=\"1\" noresize /></frameset></html>");

            Response.Write(pageBuilder.ToString());
            //return pageBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="NumberStyle"></param>
        /// <returns></returns>
        private bool IsMultiHierarchyReport(string Report) 
        {
            bool isMultiFleetReport = false;

            switch (Report)
            { 
                case "10048":
                case "10049":                // MF Fleet Violation Detail
                case "10051":                // MF Idling detail
                case "10054":                // MF P/S Mileage Detail of Fleet
                case "10055":                // MF P/S Mileage Summary of Fleet
                case "10056":                // MF Speed Violation Detail by Road Speed
                case "10057":                // MF Speed Violation Summary by Road Speed
                case "10061":                // Fleet Violation Summary Report
                case "10062":                // Stop & Idling
                case "10063":                // Alarms
                case "127":                  // UP Fleet Infraction Details
                case "130":                  // UP Fleet Infraction Summary   
                case "132":                  
                case "133":
                case "103":
                    isMultiFleetReport = true;
                    break;
                default:
                    isMultiFleetReport = false;
                    break;
            }
            return isMultiFleetReport;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="NumberStyle"></param>
        /// <returns></returns>
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
        public bool isNumericInteger(string val)
        {
            var regex = new Regex(@"^-*[0-9]+$");
            return regex.IsMatch(val);
        }

        public bool isTime(string val)
        {
            var regex = new Regex(@"^-*[0-9]+$");
            return regex.IsMatch(val);
        }

        /// <summary>
        /// if this.cboReports.SelectedValue == "25" 
        /// || this.cboReports.SelectedValue == "26"
        /// || this.cboReports.SelectedValue == "27"
        /// || this.cboReports.SelectedValue == "28"
        /// || this.cboReports.SelectedValue == "53"
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        private bool IsValidDriver(string Report) {
            // check driver
            //
            //{
            //    if (this.ddlDrivers.SelectedIndex == 0)
            //    {
            //        this.lblMessage.Visible = true;
            //        this.lblMessage.Text = (string)base.GetLocalResourceObject("valDriver");
            //        return false;
            //    }
            //}
            return true;
        }

        /// <summary>
        /// if(this.cboReports.SelectedValue != "19" 
        /// && this.cboReports.SelectedValue != "25"
        /// && this.cboReports.SelectedValue != "38"
        /// && this.cboReports.SelectedValue != "53"
        /// && this.cboReports.SelectedValue != "88"
        /// && this.cboReports.SelectedValue != "10011"
        /// && this.cboReports.SelectedValue != "10017")
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        private bool IsValidFleet(string Report)
        {
            if ("|19|25|38|53|88|10011|10017|10018|".Contains(Report))
            {
                //Fleet list is visible and enabled
                if (this.cboFleet.Visible && this.cboFleet.Enabled && this.cboFleet.SelectedIndex > 0)
                    return true;
                // Hierarchy tree is visible and enabled
                else if (ShowOrganizationHierarchy && organizationHierarchy.Visible){
                    // Hierarchy Fleet assigned
                    if (Request.Form["OrganizationHierarchyFleetId"] != "")
                        return true;
                    // Hierarchy node assigned
                    else if (Request.Form["hidOrganizationHierarchyNodeCode"] != "")
                        return true;
                    else
                        return false;
                
                }
                // fleet not required
                else
                    return true;
            }
            else 
                return true;
        }

        private bool IsValidFleetIDString(string Fleets)
        {
            if (!Fleets.Contains(",")) {
                return isNumeric(Fleets);
            }
            else
            {
                string[] fleet = Fleets.Split(',');
                foreach (string f in fleet)
	            {
                    if (!string.IsNullOrEmpty(f) && !isNumeric(f)) return false;
	            }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsValidDateTime(out string Message) 
        {
            if (this.txtTo.Visible && this.txtTo.Enabled)
            {
                int reportDaysLimit = 31;

                if ("{10052|10034|10094}".IndexOf(this.cboReports.SelectedValue) > 0)
                    reportDaysLimit = 367;
                else if ("{10002|10011|10020}".IndexOf(this.cboReports.SelectedValue) > 0)
                    reportDaysLimit = 100;
                else 
                    reportDaysLimit = 31;

                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                DateTime datefr = Convert.ToDateTime(this.txtFrom.SelectedDate.Value.ToString(DateFormat) + " " + String.Format("{0:t}", cboHoursFrom.SelectedDate.Value), ci);
                DateTime dateto = Convert.ToDateTime(this.txtTo.SelectedDate.Value.ToString(DateFormat) + " " + String.Format("{0:t}", this.cboHoursTo.SelectedDate.Value), ci);
                TimeSpan dtDiff = dateto - datefr;
                int Hours = (int)(dateto - datefr).TotalHours;

                if (Hours < 1 && cboHoursFrom.Enabled && cboHoursFrom.Visible)
                    Message = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
                else if (dtDiff.Days < 1)
                    Message = (string)base.GetLocalResourceObject("lblMessage_GenerateReportForMoreThanOneDay");
                else if (dtDiff.Days > reportDaysLimit)
                    Message = string.Format(GetLocalResourceObject("MessageRptDateRange").ToString(), reportDaysLimit);
                else
                    Message = "";
            }
            else
            {
                Message = "";
            }
            return string.IsNullOrEmpty(Message);
        }

        /// <summary>
        /// this.cboReports.SelectedValue == "22"
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        private bool IsValidGeozone(string Report) {
            //if ((this.ddlGeozones.SelectedIndex == 0) && ())
            //{
            //    this.lblMessage.Visible = true;
            //    this.lblMessage.Text = ;
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsValidLandmark(string Report)
        {
            // LandMark
            //if (this.ddlLandmarks.SelectedIndex == 0 && (this.cboReports.SelectedValue == "21" || this.cboReports.SelectedValue == "23" || this.cboReports.SelectedValue == "41"))
            //if (this.ddlLandmarks.SelectedIndex == 0 && ("|21|23|41|".IndexOf(this.cboReports.SelectedValue) > 0))
            //{
            //    if (this.ddlLandmarks.SelectedIndex == 0)
            //    {
            //        //this.lblMessage.Visible = true;
            //        //this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectLandmark");
            //        //return false;

            //    }
            //}

            return true;
        }

        /// <summary>
        /// Box / License plate
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        private bool IsValidVehicle(string Report) {
            //if (((this.cboVehicle.SelectedIndex == 0 && this.cboReports.SelectedValue == "3") ||
            //    this.cboVehicle.SelectedIndex == -1) && cboVehicle.Visible == true) //&& cboVehicle.Visible == true added by devin
            //{
            //    this.lblMessage.Visible = true;
            //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
            //    return false;
            //}
            if ("|3|40|63|10020|".Contains(Report))
            {
                if (organizationHierarchy.Visible && this.OrganizationHierarchyBoxId.Value == "")
                    return false;
                else if (this.cboVehicle.Visible && this.cboVehicle.Enabled && this.cboVehicle.SelectedIndex == 0)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        /// <summary>
        /// Replace CR GUI w. BI GUI
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="crid"></param>
        /// <param name="biid"></param>
        /// <returns></returns>
        private void ReplaceReportGUIID(string crid, string biid, ref string xmlReport){
            xmlReport = xmlReport.Replace(String.Format("<GuiId>{0}</GuiId>", crid), String.Format("<GuiId>{0}</GuiId>", biid));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int StringToInt(string value)
        {
            return StringToInt(value, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int StringToInt(string value, int DefaulValue)
        {
            int i = 0;
            if (int.TryParse(value, out i))
                return i;
            else
                return DefaulValue;
        }

        private string getPathByNodeCode(string defaultnodecode)
        {
            string[] ss = defaultnodecode.Split(',');
            List<string> pathList = new List<string>();
            foreach (string s in ss)
            {
                string p = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, s).Trim('/');
                string[] ps = p.Split('/');
                List<string> tmp = new List<string>(ps);
                
                ps = tmp.ToArray();

                foreach (string s1 in ps)
                {
                    int pos = pathList.FindIndex(f => f == s1);
                    if (pos < 0)
                    {
                        pathList.Add(s1);
                    }
                }

            }
            return String.Join("/", pathList.ToArray());
        }

        private void getSaturdays()
        {
            string dtformat = ((CurrentUICulture.IndexOf("fr") == 0) ? "dd/MM/yyyy" : "MM/dd/yyyy");

            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime start = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);

            this.cboSaturdays.Items.Clear();

            while (end > start)     //Descending
            {
                if (end.DayOfWeek == DayOfWeek.Saturday)
                {
                    //saturdays.Add(new DateTime(start.Year, start.Month, start.Day));
                    this.cboSaturdays.Items.Add(new RadComboBoxItem(new DateTime(end.Year, end.Month, end.Day).ToString(dtformat), new DateTime(end.Year, end.Month, end.Day).ToString("yyyy-MM-dd")));
                    end = end.AddDays(-7);
                }
                else
                {
                    end = end.AddDays(-1);
                }
            }
        }
    }
}
