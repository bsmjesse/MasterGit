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
using System.Configuration;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;

namespace SentinelFM
{
    public partial class Reports_frmReports_new : SentinelFMBasePage
    {
        public string errvalSelectVehicle = string.Empty;
        public string errvalSelectLandmark = string.Empty;
        public string errvalFleetMessage = string.Empty;
        public string errvalDriver = string.Empty;
        public string errddlGeozones_Item_0 = string.Empty;
        public string errlblMessage_Text_InvalidDate = string.Empty;
        public string errlblMessage_Text_SelectVehicle = string.Empty;
        public string tblWidth = "30%";
        public bool ShowOrganizationHierarchy;
        public string OrganizationHierarchyPath = "";

        clsStandardReport standardReport = null;
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                CheckTimout();
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                sn.Report.ReportActiveTab = 0;

                ShowOrganizationHierarchy = false;
                
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
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                        {

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
                            //Devin added
                            if (!Page.IsPostBack)
                                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                            else
                                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, Request.Form["OrganizationHierarchyNodeCode"].ToString());

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


                    //remove report

                    string strViolationSpeedRoad = "18,489,287,628,254,368,664,343,999647,951,999630";
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

                    cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindItemByValue(sn.Report.GuiId.ToString()));
                    ReportCriteria();



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

            if (sn.User.OrganizationId == 123 || sn.SuperOrganizationId == 382 || sn.User.OrganizationId == 327 || sn.User.OrganizationId == 489 || sn.User.OrganizationId == 622 || sn.User.OrganizationId == 570 || sn.User.OrganizationId == 18 || sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999620 || sn.User.OrganizationId == 698 || sn.User.OrganizationId == 999630)
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
            errvalDriver = (string)base.GetLocalResourceObject("valDriver");
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
            this.tblRoadSpeed.Visible = false;
            // build descr. name
            this.LabelReportDescription.Text = "";
            sn.Report.ReportType = cboReports.SelectedValue;    

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
                    this.cboFleet.Enabled = false;
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
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.lblFleetViolationDetailsReportDesc.Visible = true;
                    this.cboViolationSpeed.Visible = true;
                    break;

                case "19": // idling summary
                case "88":
                    FleetVehicleShow(false);
                    this.lblIdlingSummaryReportDesc.Visible = true;
                    break;

                case "20": // violation summary 4 fleet
                    this.tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.tblPoints.Visible = true;
                    this.lblFleetViolationSummaryReportDesc.Visible = true;
                    break;

                case "21": // landmark summary
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
                    this.cboVehicle.Enabled = false;
                    break;

                case "38":
                case "51":

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
                case "40":
                case "63":
                    this.tblIgnition.Visible = true;
                    break;

                case "41":
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
                    this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = false;
                    lblRoadSpeedDelta.Visible = false;
                    break;
                case "103":
                    tblDriverOptions.Visible = false;
                    FleetVehicleShow(false);
                    break;
                case "104": // ROad Speed Violation
                    this.cboVehicle.Enabled = false;
                    tblRoadSpeed.Visible = true;
                    cboRoadSpeedDelta.Visible = true;
                    lblRoadSpeedDelta.Visible = true;
                    break;


                case "105": // Garmin Message
                    HideHierarchy();
                    break;

            }

            //if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
            //{
            //    this.organizationHierarchy.Visible = false;
            //    this.vehicleSelectOption.Visible = false;
            //    this.trFleet.Visible = true;
            //}

            if (vehicleSelectOption.Visible) this.optReportBased.SelectedIndex = 0;
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


            dsReports.ReadXml(new StringReader(xml));
            //dsReports.Tables[0].DefaultView.Sort = "GUIName";   
            //For Testing begin
            //dsReports.Tables[0].Rows[2]["GuiId"] = "6";
            //end

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
                this.ddlDrivers.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlDrivers_Item_0").ToString(), "-1"));
            }
            else
                this.ddlDrivers.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlDrivers_NoAvailable").ToString(), "-100"));


            if ((sn.Report.DriverId != 0) && (sn.Report.DriverId != -1))
                ddlDrivers.SelectedIndex = ddlDrivers.Items.IndexOf(ddlDrivers.Items.FindItemByValue(sn.Report.DriverId.ToString()));

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
            standardReport.cboMaintenanceFleet = cboMaintenanceFleet.SelectedValue;
            standardReport.cboMaintenanceFleet_Name = cboMaintenanceFleet.SelectedItem.Text;
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
            standardReport.optEndTrip = optEndTrip.SelectedItem.Value;
            standardReport.txtFrom = txtFrom.SelectedDate;
            standardReport.cboHoursFrom = cboHoursFrom.SelectedDate;
            standardReport.txtTo = txtTo.SelectedDate;
            standardReport.cboHoursTo = cboHoursTo.SelectedDate;

            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0)
                standardReport.cboFleet = Request.Form["OrganizationHierarchyFleetId"];
            else
                standardReport.cboFleet = cboFleet.SelectedItem.Value; //cboFleet.SelectedItem

            standardReport.cboVehicle = cboVehicle.SelectedValue; //cboVehicle.SelectedValue
            standardReport.cboFormat = cboFormat.SelectedValue;
            standardReport.cboFleet_Name = cboFleet.SelectedItem.Text;
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
            standardReport.cboRoadSpeedDelta = cboRoadSpeedDelta.SelectedItem.Value;
            standardReport.OrganizationHierarchyNodeCode = Request.Form["OrganizationHierarchyNodeCode"];
            sn.Report.ReportType = cboReports.SelectedValue;
        }

        protected void cmdShow_Click(object sender, System.EventArgs e)
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


                if (this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "40" || this.cboReports.SelectedValue == "63"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectVehicle");
                    return false;
                }


                if (this.ddlLandmarks.SelectedIndex == 0 && (this.cboReports.SelectedValue == "21" || this.cboReports.SelectedValue == "23" || this.cboReports.SelectedValue == "41"))
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
                if (ts.Days > 31)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = GetLocalResourceObject("MessageRptDateRange").ToString();
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
                ddlDrivers.FindItemByValue(standardReport.ddlDrivers).Selected = true;
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

            chkIsPostedOnly.Checked = standardReport.chkIsPostedOnly;
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
                if (cboMaintenanceFleet.Visible && cboMaintenanceFleet.Enabled)
                {
                    string keyvalue_fleet = (string)base.GetLocalResourceObject("keyvalue_fleet");
                    return string.Format("<b>{0}</b>={1}", keyvalue_fleet, cboMaintenanceFleet.SelectedItem.Text.Trim());
                }
                else return string.Empty;
            }
            else
            {

                if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0)
                    sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
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
                    sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_fleet, sn.Report.FleetId.ToString()));


                


                if (cboVehicle.Visible && cboVehicle.Enabled && cboVehicle.Items.Count > 0)
                {
                    if (cboVehicle.SelectedIndex >= 0)
                        sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_vehicle, cboVehicle.SelectedItem.Text));
                    else
                        sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_vehicle, cboVehicle.Items[0].Text));
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

        private void ReportBasedOption()
        {

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
            vehicleSelectOption.Visible = false;
        }

        protected void btnAfterCreate_Click(object sender, EventArgs e)
        {            
        }

    }
}
