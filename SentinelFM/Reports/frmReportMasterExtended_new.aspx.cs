#region Namespace - Assembly Section

// System Namespaces
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

// BSM Namespaces
using VLF.ERRSecurity;
using VLF.Reports;
using VLF.CLS;

// Third party namespaces
using Telerik.Web.UI;

#endregion

namespace SentinelFM
{
    public partial class Reports_frmReportMasterExtended_new : SentinelFMBasePage
    {

        #region Variable Definition Section

        public string errlblMessage_Text_SelectVehicle = string.Empty;
        public bool ShowOrganizationHierarchy;
        public string OrganizationHierarchyPath = "";
        public bool IniHierarchyPath = false;

        public string errvalHierarchyMessage = string.Empty;

        private string CurrentUICulture = "en-US";
        private string DateFormat = "MM/dd/yyyy";
        private string TimeFormat = "hh:mm:ss tt";

        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        clsExtendedReport extendedReport = null;

        public bool MutipleUserHierarchyAssignment = false;
        public string PreferOrganizationHierarchyNodeCode = string.Empty;
        public int VehiclePageSize = 10;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckTimout();
            try
            {

                sn.Report.ReportActiveTab = 1;



                //Show Busy Message Comment By Devin
                //cmdShow.Attributes.Add("onclick", BusyReport.ShowFunctionCall);

                //this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
                //this.BusyReport.Text = (string)base.GetLocalResourceObject("BusyPreparingMessage");

              //  ShowOrganizationHierarchy = false;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
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

                    //string xml = "";
                    //if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    //    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    //    {
                    //
                    //    }


                    //StringReader strrXML = new StringReader(xml);
                    //DataSet dsPref = new DataSet();
                    //dsPref.ReadXml(strrXML);

                    //foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    //{

                    //    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                    //    {
                    //        string nodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                    //        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                    //        //OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                    //        //Devin added
                    //        if (!Page.IsPostBack)
                    //            OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                    //        else
                    //            OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, Request.Form["OrganizationHierarchyNodeCode"].ToString());

                    //    }
                    //}

                    PreferOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;


                    PreferOrganizationHierarchyNodeCode = PreferOrganizationHierarchyNodeCode ?? string.Empty;
                    if (PreferOrganizationHierarchyNodeCode == string.Empty)
                    {
                        if (sn.RootOrganizationHierarchyNodeCode == string.Empty)
                        {
                            PreferOrganizationHierarchyNodeCode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID, MutipleUserHierarchyAssignment);
                            sn.RootOrganizationHierarchyNodeCode = PreferOrganizationHierarchyNodeCode;
                        }
                        else
                            PreferOrganizationHierarchyNodeCode = sn.RootOrganizationHierarchyNodeCode;
                    }                    
                    
                    ReportBasedOption();
                }
                else
                {
                    this.organizationHierarchy.Visible = false;
                    this.vehicleSelectOption.Visible = false;
                    this.trFleet.Visible = true;
                }



                int i = 0;
                if (!Page.IsPostBack)
                {
                    string displayProcessedStandardReports = ConfigurationManager.AppSettings.Get("displayProcessedStandardReports");
                    if (displayProcessedStandardReports == null || displayProcessedStandardReports == string.Empty || displayProcessedStandardReports.ToLower() == "false")
                        ddlReport.Items.FindItemByValue("3").Visible = false;

                      if (ShowOrganizationHierarchy) optReportBased.Items.FindByValue("0").Selected = true;

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



                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmReportMaster, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    GetUserReportsTypes();
                    try
                    {
                        cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindItemByValue(sn.Report.GuiId.ToString()));
                    }
                    catch
                    {
                    }

                    //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                    //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;


                    if (sn.Report.FromDate != "")
                        this.txtFrom.SelectedDate = Convert.ToDateTime(sn.Report.FromDate);
                    else
                        this.txtFrom.SelectedDate = DateTime.Now;


                    if (sn.Report.ToDate != "")
                        this.txtTo.SelectedDate = Convert.ToDateTime(sn.Report.ToDate);
                    else
                        this.txtTo.SelectedDate = DateTime.Now.AddDays(1);

                    //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy hh:mm");
                    //this.txtTo.Text=DateTime.Now.AddDays(1).ToString("MM/dd/yyyy hh:mm");




                    this.tblSpeedViolation.Visible = false;
                    this.tblCost.Visible = true;
                    this.tblFilter.Visible = false;
                    this.lblLandmarkCaption.Visible = false;
                    this.ddlLandmarks.Visible = false;
                    this.tblPoints.Visible = false;
                    this.tblMediaType.Visible = false;
                    this.tblDriverOptions.Visible = false;
                    this.tblIdlingThreshold.Visible = false;
                    this.tblSpeedThreshold.Visible = false;
  
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

                    this.frmScheduleReportList.iOrganizationID = sn.User.OrganizationId;
                    this.frmScheduleReportList.iSessionUserID = sn.UserID;
                    this.frmScheduleReportList.ParentUserGroupId = sn.User.ParentUserGroupId;
                    this.frmScheduleReportList.UserGroupId = sn.User.UserGroupId;

                    //cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));

                    ReportCriteria();
                    cmdShow.Attributes.Add("OnClick", "javascript:if (!OpenCreateWindow('" + cmdShow.ValidationGroup + "', '" + cmdShowHide.ClientID + "')) return false; ");
                    errlblMessage_Text_SelectVehicle = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                    // if (sn.User.OrganizationId == 1)
                    // {
                    //cboReports.Items.Add(new ListItem("Fleet Monthly Utilization Report", "42"));
                    //cboReports.Items.Add(new ListItem("Average Days Utilized Per Vehicle", "43"));
                    //cboReports.Items.Add(new ListItem("Average Service Hrs for Utilized Vehicle", "44"));
                    //cboReports.Items.Add(new ListItem("Average Engine ON Hrs for Utilized Vehicle", "45"));
                    //cboReports.Items.Add(new ListItem("Unnecessary Idling Hrs. Per Vehicle Per Month", "46"));
                    //cboReports.Items.Add(new ListItem("Total Unnecessary Idling Fuel Costs", "47"));
                    //cboReports.Items.Add(new ListItem("Average Travelled Distance ", "48"));
                    //cboReports.Items.Add(new ListItem("Geozone Report", "22"));
                    //cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                    //cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
                    // }
                    HideAndDispControlsForMyReport();
                    if (ShowOrganizationHierarchy) ReportBasedOption();


                    this.btnScoreA.BackColor = System.Drawing.Color.FromArgb(173, 255, 47);
                    this.btnScoreB.BackColor = System.Drawing.Color.FromArgb(255, 165, 0);
                    this.btnScoreC.BackColor = System.Drawing.Color.FromArgb(255, 0, 0);
                }


                if (sn.User.OrganizationId == 951)
                {
                    /*if (this.cboReports.SelectedItem.Value == "111")
                    {
                        this.organizationHierarchy.Visible = false;
                        this.vehicleSelectOption.Visible = false;
                        this.trFleet.Visible = false;
                    }
                    else
                    {
                        this.organizationHierarchy.Visible = true;
                        this.vehicleSelectOption.Visible = true ;
                        this.trFleet.Visible = true;
                    }*/
                }

                CreateErrorMessageForClientValidate();
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

        protected void cboReports_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            try
            {
                ReportBasedOption();
                ReportCriteria();               

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
                            //Temporary Comment
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


                    extendedReport = new clsExtendedReport();
                    if (userReport.CustomProp != null)
                        extendedReport.GetCustomProperty(userReport.CustomProp);

                    DateTime from = DateTime.Now.ToUniversalTime().Date.AddMinutes(userReport.Start);
                    DateTime to = from.AddMinutes(userReport.Period);
                    extendedReport.txtFrom = from.Date;
                    extendedReport.txtTo = to.Date;
                    FillControlsByparameters();
                }
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

        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));


                if (sn.Misc.DsReportAllFleets != null && sn.Misc.DsReportAllFleets.Tables.Count > 0 && sn.Misc.DsReportAllFleets.Tables[0].Rows.Count > 0)
                {
                    sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
                    this.lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
                    lstUnAss.DataBind();
                }
                else
                {
                    dsFleets.Tables[0].DefaultView.Sort = "FleetName";
                    this.lstUnAss.DataSource = dsFleets.Tables[0].DefaultView;
                    lstUnAss.DataBind();
                    sn.Misc.DsReportAllFleets = dsFleets;
                }

                if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
                {
                    this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
                    lstAss.DataBind();
                }

                Boolean isMyreport = false;
                if (Request.QueryString["isMyReport"] != null)
                {
                    if (Request.QueryString["isMyReport"].ToString() == "1")
                    {
                        isMyreport = true;
                    }
                }
                if (!isMyreport) CheckUnassignedList();
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

                //cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                cboVehicle.Items.Insert(0, new RadComboBoxItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
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
            Int16 categoryId = 1;

            if (sn.User.OrganizationId == 123) //CN
                categoryId = 1;
            if (sn.User.OrganizationId == 570) //Brickman
                categoryId = 4;
            else if (sn.SuperOrganizationId == 382 && sn.User.OrganizationId != 999746) //Wex
                categoryId = 2;
            else if (sn.User.OrganizationId == 622) //CP Rail
                categoryId = 3;
            else if (sn.User.OrganizationId == 18) //Aecon
                categoryId = 5;
            else if (sn.User.OrganizationId == 951) //UP
                categoryId = 6;
            else if (sn.User.OrganizationId == 327) //Badger Daylighting Inc
                categoryId = 7;
            else if (sn.User.OrganizationId == 489) //Graham Construction
                categoryId = 8;
            else if (sn.User.OrganizationId == 999620) //Datum Exploration Ltd.
                categoryId = 10;
            else if (sn.User.OrganizationId == 698) //CNTL
                categoryId = 11;
            else if (sn.User.OrganizationId == 999630) //MTSAllstream
                categoryId = 12;
            else if (sn.User.OrganizationId == 999603) //E80 Plus Constructors
                categoryId = 13;
            else if (sn.User.OrganizationId == 999693) //Mr. Rooter of Ottawa
                categoryId = 14;
            else if (sn.User.OrganizationId == 480) //SFM 2000
                categoryId = 15;
            else if (sn.User.OrganizationId == 999620) //SA Exploration (Canada) Ltd (Datum)
                categoryId = 16;
            else if (sn.User.OrganizationId == 999692) //Willbros
                categoryId = 17;
            else if (sn.User.OrganizationId == 999650) //Transport SN
                categoryId = 18;
            else if (sn.User.OrganizationId == 952) //G4S
                categoryId = 19;
            else if (sn.User.OrganizationId == 999695) //VanHoute 
                categoryId = 20;
            else if (sn.User.OrganizationId == 999700) //MTO 
                categoryId = 22;
            else if (sn.User.OrganizationId == 999746) //BATO
                categoryId = 23;
            else if (sn.User.OrganizationId == 999994) //BNSF Railway
                categoryId = 24;
            else if (sn.User.OrganizationId == 1000010) //Sperry
                categoryId = 25;
            else if (sn.User.OrganizationId == 655) //Edge Oil
                categoryId = 26;
            else if (sn.User.OrganizationId == 999988) //TDSB
                categoryId = 27;
            else if (sn.User.OrganizationId == 1000065) //Ameco
                categoryId = 29;
            else if (sn.User.OrganizationId == 563) //Cummins Eastern Canada LP
                categoryId = 32;	
            else if (sn.User.OrganizationId == 1000026) //Bell Canada Inc
                categoryId = 33;	
            else if (sn.User.OrganizationId == 1000051) //LIRR
                categoryId = 34;		
            else if (sn.User.OrganizationId == 1000076) //Metro North
                categoryId = 35;	
            else if (sn.User.OrganizationId ==  1000088) //City of St. John's
                categoryId = 36;
            else if (sn.User.OrganizationId ==  1000096) //Bridges & Tunnels
                categoryId = 37;
            else if (sn.User.OrganizationId ==  1000110) //Bell Aliant
                categoryId = 38;	
            else if (sn.User.OrganizationId ==  1000097) //OmniTrax
                categoryId = 39;
            else if (sn.User.OrganizationId == 1000120) //Beacon Roofing Supply Canada
                categoryId = 40;
            else if (sn.User.OrganizationId == 999722) //Superior Plus Winroc
                categoryId = 41;
            else if (sn.User.OrganizationId == 1000056) //Jean Fournier Inc
                categoryId = 42;
            else if (sn.User.OrganizationId == 342) //Strongco Inc
                categoryId = 43;
            else if (sn.User.OrganizationId == 1000142) //Railworks 1000142/44
                categoryId = 44;
            else if (sn.User.OrganizationId == 999981) // Parkland Industries Ltd
                categoryId = 45;
            else if (sn.User.OrganizationId == 1000144) //Ville De Pointe-Claire (Securite)
                categoryId = 46;
            else if (sn.User.OrganizationId == 999646) //Ville de Vaudreuil-Dorion
                categoryId = 47;
            else if (sn.User.OrganizationId == 1000164) //Town of Georgina
                categoryId = 48;
            else if (sn.User.OrganizationId == 664) //PVS
                categoryId = 49;
            else if (sn.User.OrganizationId == 1000141) //Gazzola
                categoryId = 50;
            else if (sn.User.OrganizationId == 1000176) //Guard-X Inc.
                categoryId = 51;
            else if (sn.User.OrganizationId == 999991) //BSM Test
                categoryId = 52;
            else if (sn.User.OrganizationId == 1000152) //RED-D-ARC USA.
                categoryId = 53;
            else if (sn.User.OrganizationId == 1000170) //RED-D-ARC Canada.
                categoryId = 54;

            if (Util.IsDataSetValid(sn.Report.UserExtendedReportsDataSet))
            {
                this.cboReports.DataSource = sn.Report.UserExtendedReportsDataSet;
                this.cboReports.DataBind();
            }
            else
            {
                DataSet dsReports = new DataSet();

                using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
                {
                    if (objUtil.ErrCheck(dbs.GetUserReportsByCategory(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, categoryId, ref xml), false))
                        if (objUtil.ErrCheck(dbs.GetUserReportsByCategory(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, categoryId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            return;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    // return;
                }
                else
                {
                    ////switch (sn.User.OrganizationId)
////                    {
////                        case 951:                                    // UP   70 70 90 91 10021
////                            xml = xml.Replace("70", "10031");        // Sensor Activity Report
////                            xml = xml.Replace("71", "10033");        // Good / Bad Idling
////                            xml = xml.Replace("129", "10042");       // Operational Log Report
////                            break;

////                        case 952:                                    // G4S : 19 
////                            xml = xml.Replace("71", "10033");        // Truck Utilization Summary Report
////                            break;

////                        case 999994:                                 // 21: 999994 BNSF Railway
////                            xml = xml.Replace("115", "10023");       // Off Road Miles Report (State-Province)
////                            xml = xml.Replace("71", "10033");        // Good / Bad Idling
////                            xml = xml.Replace("70", "10031");        // Sensor Activity Report
////                            break;

////                        case 999692:                                 // 17: 999692: Willbros Unites States Holdings Inc 
////                          xml = xml.Replace("119", "10022");         // Speed Distribution Report
////                          break;

////                        case 1000010:                                // Sperry
////                            xml = xml.Replace("115", "10023");       // Off Road Miles Report (State-Province)
////                            xml = xml.Replace("71", "10033");        // Truck Utilization Summary 
////                            xml = xml.Replace("70", "10031");        // Sensor Activity Report
////                            break;

////                        case 999630: //MTS All Stream
////                            xml = xml.Replace("107", "10044");       // Idling Details Driver Report
////                            xml = xml.Replace("128", "10045");       // Fleet Driver Violation Summary Report
////                            break;

////               case 999746: //Bato
////                            xml = xml.Replace("70", "10031");        // Sensor Activity Report
////                            break;

////                    }

                    dsReports.ReadXml(new StringReader(xml));
                    this.cboReports.DataSource = dsReports;
                    sn.Report.UserExtendedReportsDataSet = dsReports;
                    this.cboReports.DataBind();

                }
            }

        }



        protected void ddlReport_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (ddlReport.SelectedIndex == 0)
                Response.Redirect("frmReports_new.aspx");
            else if (ddlReport.SelectedIndex == 2)
                Response.Redirect("frmMyReports.aspx");
            else if (ddlReport.SelectedIndex == 3)
                Response.Redirect("frmSSRS.aspx");
        }

        /// <summary>
        /// Show/hide controls according to selected report
        /// </summary>
        private void ReportCriteria()
        {


            this.cboVehicle.Enabled = true;
            this.lblVehicleName.Enabled = true;
            FleetVehicleShow(true);

            // build descr. name
            this.LabelReportDescription.Text = "";

            string resourceDescriptionName = "";
            string filter = String.Format("GuiId = '{0}'", this.cboReports.SelectedValue);
            DataRow[] rowsReport = null;
            if (Util.IsDataSetValid(sn.Report.UserExtendedReportsDataSet))
            {
                rowsReport = sn.Report.UserExtendedReportsDataSet.Tables[0].Select(filter);
                if (rowsReport != null && rowsReport.Length > 0)
                {
                    resourceDescriptionName = String.Format("Description_{0}", rowsReport[0]["ReportTypesName"]);
                    this.LabelReportDescription.Text = base.GetResource(resourceDescriptionName);
                    if (String.IsNullOrEmpty(this.LabelReportDescription.Text))
                        this.LabelReportDescription.Text = rowsReport[0]["GuiName"].ToString();
                }
            }
            this.lblToTitle3.Visible = true;
            this.txtTo.Visible = true;
            this.lblToTitle3.Visible = true;
            this.txtTo.Visible = true;
            this.txtFrom.Visible = true;
            this.lblFromTitle3.Visible = true;
            this.cboFleet.Enabled = true;
            this.tblCost.Visible = false;
            this.tblFleets.Visible = false;
            this.tblFilter.Visible = false;
            tblIgnition.Visible = false;
            tblViolationReport.Visible = false;
            this.lblGeozoneCaption.Visible = false;
            this.ddlGeozones.Visible = false;
            this.tblSpeedViolation.Visible = false;
            tblPoints.Visible = false;
            tblViolationReport.Visible = false;
            this.tblMediaType.Visible = false;
            this.tblDriverOptions.Visible = false;
            this.tblIdlingThreshold.Visible = false;
            this.tblSpeedThreshold.Visible = false;
            this.pnlOperationalLogs.Visible = false;
            this.tbVoltageThreshold.Visible = false;
            fldScoreCategory.Visible = false;

            sn.Report.ReportType = cboReports.SelectedValue;

            if (sn.User.OrganizationId == 570)
            {
                this.chkActiveVehicles.Visible = true;
                this.chkActiveVehicles.Enabled = true;
                this.chkActiveVehicles.Checked = true;
            }
            else
                this.chkActiveVehicles.Visible = false;


            switch (this.cboReports.SelectedValue)
            {

                case "29":
                case "55":
                case "118":

                    FleetVehicleShow(false);
                    break;
                case "31":
                    this.cboVehicle.Enabled = true;
                    this.lblVehicleName.Enabled = true;
                    this.tblSpeedViolation.Visible = false;
                    this.tblCost.Visible = true;
                    break;
                case "32":
                case "37":
                    this.tblFleets.Visible = true;
                    this.cboFleet.Enabled = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblSpeedViolation.Visible = false;
                    this.tblCost.Visible = true;
                    this.tblFilter.Visible = true;
                    break;

                case "33":
                case "35":
                case "80":
                case "81":
                case "84":
                case "85":

                    this.tblFleets.Visible = true;
                    this.cboFleet.Enabled = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblSpeedViolation.Visible = true;
                    this.tblCost.Visible = false;
                    this.tblFilter.Visible = true;
                    break;

                case "38":

                    FleetVehicleShow(false);
                    tblIgnition.Visible = true;
                    break;

                case "39":

                    this.cboVehicle.Enabled = false;
                    tblIgnition.Visible = true;
                    break;

                case "42":

                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblCost.Visible = false;
                    break;

                case "43":
                case "44":
                case "45":
                case "46":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.tblCost.Visible = false;
                    break;

                case "47":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.tblCost.Visible = true;
                    break;

                case "49":
                case "58":
                case "76":
                case "77":
                case "121":
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.lblFromTitle3.Visible = false;
                    this.txtFrom.Visible = false;
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.cboFleet.Enabled = false;
                    this.tblCost.Visible = false;
                    break;

                case "52":
                    this.lblToTitle3.Visible = true;
                    this.txtTo.Visible = true;
                    this.lblFromTitle3.Visible = true;
                    this.txtFrom.Visible = true;
                    this.cboVehicle.Enabled = true;
                    this.lblVehicleName.Enabled = true;
                    this.cboFleet.Enabled = true;
                    this.tblCost.Visible = false;
                    break;
                case "56":
                    this.cboVehicle.Enabled = true;
                    tblIgnition.Visible = true;
                    break;

                case "57":
                    this.cboVehicle.Enabled = false;
                    this.tblViolationReport.Visible = true;
                    break;
                case "63":
                    this.cboVehicle.Enabled = true;
                    break;


                case "65":
                case "70":

                case "73":
                case "90":
                case "91":
                case "10021":                                   //Sensor Activity
                case "10031":
                    //this.lblLandmarkCaption.Visible = true ;
                    //this.ddlLandmarks.Visible = true ;
                    this.cboVehicle.Enabled = false;
                    //LoadLandmarks();
                    break;
                case "71":
                case "10032":
                case "10033":
                    this.cboVehicle.Enabled = false;
                    sn.Report.XmlParams = "3";
                    break;
                case "64":
                case "66":
                case "67":
                case "68":
                case "69":
                case "79":
                    // this.lblToTitle3.Visible = false;
                    //this.txtTo.Visible = false;
                    //this.lblFromTitle3.Visible = false;
                    //this.txtFrom.Visible = false;
                    this.cboVehicle.Enabled = false;
                    //  this.lblGeozoneCaption.Visible = true;
                    //  this.ddlGeozones.Visible = true;  
                    break;

                case "72":    // Fleet Violation 
                case "10045": // Fleet Driver Violation
                    trFleet.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.tblPoints.Visible = true;
                    tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    break;

                case "123":  // Driver Scores Fleetwise
                    trFleet.Visible = true;
                    this.cboVehicle.Enabled = false;
                    this.tblPoints.Visible = true;
                    tblViolationReport.Visible = true;
                    this.cboVehicle.Enabled = false;
                    fldScoreCategory.Visible = true;
                    this.txtSeatBelt.Text = "1";
                    this.txtSpeed120.Text = "1";
                    this.txtSpeed130.Text = "1";
                    this.txtSpeed140.Text = "1";
                    this.txtAccExtreme.Text = "1";
                    this.txtAccHarsh.Text = "1";
                    this.txtBrakingExtreme.Text = "1";
                    this.txtBrakingHarsh.Text = "1";
                    break;

                case "75":
                    this.cboVehicle.Enabled = false;
                    this.lblVehicleName.Enabled = false;
                    this.tblSpeedViolation.Visible = true;
                    break;
                case "92":
                case "93":
                case "96":
                case "115":
                case "116":
                case "117":
                case "119":
                case "120":
                case "10023":
                case "10037":
                case "10069":
                case "10034":
                case "10076":
                case "10058":
                case "10052":
                case "10078":
                case "10079":
                case "98":
                case "10094":       // Fleet Vehicle Mileage Summary Report.
                    this.cboVehicle.Enabled = false;
                    break;
                case "94":
                case "99":
                    this.cboVehicle.Enabled = false;
                    this.txtFrom.Visible = false;
                    this.txtTo.Visible = false;
                    break;


                case "100":
                case "102":

                    this.cboFleet.Enabled = false;
                    this.cboVehicle.Enabled = false;
                    break;
             

                case "107":
                    LoadDrivers();
                    tblDriverOptions.Visible = true;
                    tblIdlingThreshold.Visible = true;
                    this.cboVehicle.Enabled = false;
                    break;

                case "10044":
                    this.ddlDrivers.Visible = false;
                    tblDriverOptions.Visible = false;
                    tblIdlingThreshold.Visible = false;
                    this.cboVehicle.Enabled = false;
                    break;

                case "108":
                case "109":

                    this.cboFleet.Enabled = false;
                    this.cboVehicle.Enabled = false;
                    this.lblToTitle3.Visible = false ;
                    this.txtTo.Visible = false;
                    this.lblFromTitle3.Visible = false;
                    this.txtFrom.Visible = false;

                    break;
                case "110":
                    LoadDrivers();
                    tblDriverOptions.Visible = true;
                    tblSpeedThreshold.Visible = true;
                    break;

                case "111":
                    //optReportBased.SelectedIndex = 1;
                    FleetVehicleShow(true);
                    break;
                case "112":
                    FleetVehicleShow(false);
                    break;
                case "113":
                    tblIdlingThreshold.Visible = true;
                    this.cboVehicle.Enabled = false;
                    break;

                case "114":
                    LoadDrivers();
                    tblDriverOptions.Visible = true;
                    this.cboVehicle.Enabled = false;
                    break;

                case "124":
                    this.tblMediaType.Visible = true;
                    this.cboVehicle.Enabled = false;
                    break;

                case "10022":   // Standard Speed Distribution Report
                    break;

                case "10042":                       //Operational Log Report
                    FleetVehicleShow(false);
                    this.pnlOperationalLogs.Visible = true;
                    break;

                case "10070":
                    FleetVehicleShow(true);
                    this.tbVoltageThreshold.Visible = true;
                    break;

                case "10077":
                    this.cboVehicle.Enabled = false;
                    this.lblToTitle3.Visible = false;
                    this.txtTo.Visible = false;
                    this.lblFromTitle3.Visible = false;
                    this.txtFrom.Visible = false;
                    break;

                case "10097":   // Off Road Mile / Idle / PTO 

                    break;
            }

            valCompareDates.Enabled = txtTo.Visible;

            if (ShowOrganizationHierarchy) 
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                if (MutipleUserHierarchyAssignment)
                    MutipleUserHierarchyAssignment = IsMultiHierarchyReport(this.cboReports.SelectedValue);
            }

        }


        private void LoadLandmarks()
        {
            DataSet dsLandmarks = new DataSet();
            string xmlResult = "";

            using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
            {

                if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                    if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //RedirectToLogin();
                        return;
                    }


               
            }

            if (String.IsNullOrEmpty(xmlResult))
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                return;
            }

            dsLandmarks.ReadXml(new StringReader(xmlResult));

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
                this.vehicleSelectOption.Visible = showControls;
                this.organizationHierarchy.Visible = showControls;
                if(showControls)
                    ReportBasedOption();
            }

            if (!showControls)
            {
                this.organizationHierarchy.Visible = false;
                this.vehicleSelectOption.Visible = false;
            }
        }


        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            if (lstUnAss.SelectedIndex < 0) return;

            DataSet ds = sn.Misc.DsReportAllFleets;
            if (sn.Misc.DsReportSelectedFleets == null || sn.Misc.DsReportSelectedFleets.Tables.Count == 0)
                sn.Misc.DsReportSelectedFleets = ds.Clone();

            foreach (ListItem li in lstUnAss.Items)
            {
                if (li.Selected)
                {
                    DataRow dr = sn.Misc.DsReportSelectedFleets.Tables[0].NewRow();
                    dr["FleetId"] = li.Value;
                    dr["FleetName"] = li.Text;
                    sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Add(dr);

                    DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                    ds.Tables[0].Rows.Remove(drColl[0]);
                }

            }

            if (sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
            {
                sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView.Sort = "FleetName";
                this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
                lstAss.DataBind();
            }
            else
            {
                lstAss.Items.Clear();

            }

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "FleetName";
                lstUnAss.DataSource = ds.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {

                lstUnAss.Items.Clear();
            }

            sn.Misc.DsReportAllFleets = ds;

            //CheckUnassignedList();

        }

        private void CheckUnassignedList()
        {

            DataSet ds = sn.Misc.DsReportAllFleets.Copy();
            foreach (ListItem li in lstAss.Items)
            {
                DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                ds.Tables[0].Rows.Remove(drColl[0]);
            }
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].DefaultView.Sort = "FleetName";
                    lstUnAss.DataSource = ds.Tables[0].DefaultView;
                    lstUnAss.DataBind();
                }
                else
                {

                    lstUnAss.Items.Clear();
                }
            }
            else lstUnAss.Items.Clear();
        }
        protected void cmdAddAll_Click(object sender, EventArgs e)
        {
            DataSet ds = sn.Misc.DsReportAllFleets;
            if (sn.Misc.DsReportSelectedFleets == null || sn.Misc.DsReportSelectedFleets.Tables.Count == 0)
                sn.Misc.DsReportSelectedFleets = ds.Clone();

            foreach (ListItem li in lstUnAss.Items)
            {
                DataRow dr = sn.Misc.DsReportSelectedFleets.Tables[0].NewRow();
                dr["FleetId"] = li.Value;
                dr["FleetName"] = li.Text;
                sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Add(dr);

                DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                ds.Tables[0].Rows.Remove(drColl[0]);
            }


            if (sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
            {
                sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView.Sort = "FleetName";
                this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
                lstAss.DataBind();
            }
            else
            {
                lstAss.Items.Clear();

            }

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "FleetName";
                lstUnAss.DataSource = ds.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {

                lstUnAss.Items.Clear();
            }

            sn.Misc.DsReportAllFleets = ds;
        }
        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            if (lstAss.SelectedIndex < 0) return;

            DataSet ds = sn.Misc.DsReportSelectedFleets;
            if (sn.Misc.DsReportAllFleets == null || sn.Misc.DsReportAllFleets.Tables.Count == 0)
                sn.Misc.DsReportAllFleets = ds.Clone();

            foreach (ListItem li in lstAss.Items)
            {
                if (li.Selected)
                {
                    DataRow dr = sn.Misc.DsReportAllFleets.Tables[0].NewRow();
                    dr["FleetId"] = li.Value;
                    dr["FleetName"] = li.Text;


                    sn.Misc.DsReportAllFleets.Tables[0].Rows.Add(dr);

                    DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                    ds.Tables[0].Rows.Remove(drColl[0]);
                }


            }

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "FleetName";
                this.lstAss.DataSource = ds.Tables[0].DefaultView;
                lstAss.DataBind();
            }
            else
            {
                lstAss.Items.Clear();
            }

            if (sn.Misc.DsReportAllFleets.Tables.Count > 0)
            {
                sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
                lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {
                lstUnAss.Items.Clear();
            }

            sn.Misc.DsReportSelectedFleets = ds;
            //CheckUnassignedList();
        }

        protected void cmdRemoveAll_Click(object sender, EventArgs e)
        {

            DataSet ds = sn.Misc.DsReportSelectedFleets;
            if (sn.Misc.DsReportAllFleets == null || sn.Misc.DsReportAllFleets.Tables.Count == 0)
                sn.Misc.DsReportAllFleets = ds.Clone();

            foreach (ListItem li in lstAss.Items)
            {
                DataRow dr = sn.Misc.DsReportAllFleets.Tables[0].NewRow();
                dr["FleetId"] = li.Value;
                dr["FleetName"] = li.Text;


                sn.Misc.DsReportAllFleets.Tables[0].Rows.Add(dr);

                DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                ds.Tables[0].Rows.Remove(drColl[0]);

            }

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].DefaultView.Sort = "FleetName";
                this.lstAss.DataSource = ds.Tables[0].DefaultView;
                lstAss.DataBind();
            }
            else
            {
                lstAss.Items.Clear();
            }

            if (sn.Misc.DsReportAllFleets.Tables.Count > 0)
            {
                sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
                lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {
                lstUnAss.Items.Clear();
            }

            sn.Misc.DsReportSelectedFleets = ds;
        }

        /// <summary>
        /// Create report params and redirect to the next page
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        private Boolean CreateReportParams(string redirectUrl)
        {
            bool ret = true;
            try
            {
                OrganizationHierarchyPath = getPathByNodeCode(OrganizationHierarchyNodeCode.Value);
                IniHierarchyPath = true;

                string strFromDate = string.Empty;
                string strToDate = string.Empty;
                //if (ddlPeriod.SelectedIndex > 0)
                //{
                //    //If user has selected a period
                //    int i_length = ddlPeriod.SelectedValue.Length;
                //    int i_day = 0;
                //    if ddlPeriod.SelectedValue.Substring(i_length.)
                //    strToDate = System.DateTime.Now.ToString("MM/dd/yyyy")  + " 11:59 PM";
                //}
                //else
                //{
                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
                //}
                //lblVehicleName.Text = this.optStopFilter.Items[0].Selected.ToString(); ;
                //return;
                DateTime from, to;
                sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;

                const string dateFormat = "MM/dd/yyyy HH:mm:ss";

		   // check driver
                if (this.cboReports.SelectedValue == "114")
                {
                    if (this.ddlDrivers.SelectedIndex == 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("valDriver");
                        return false;
                    }
                }


                this.lblMessage.Text = "";

                # region Validation

                if (!clsUtility.IsNumeric(this.txtCost.Text))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Cost should be numeric";
                    return false;
                }

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

                if ((this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "3" || this.cboReports.SelectedValue == "82" || this.cboReports.SelectedValue == "86")) ||
                    this.cboVehicle.SelectedIndex == -1)
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
                # endregion

                TimeSpan ts = to - from;
                if (ts.Days > 31 && (this.cboReports.SelectedValue != "10052" && this.cboReports.SelectedValue != "10034" && this.cboReports.SelectedValue != "10094" && this.cboReports.SelectedValue != "10078" && this.cboReports.SelectedValue != "10079")) 
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = GetLocalResourceObject("MessageRptDateRange").ToString();
                    return false;
                }
                else if (ts.Days > 367 && (this.cboReports.SelectedValue == "10052" && this.cboReports.SelectedValue == "10034" && this.cboReports.SelectedValue == "10094" && this.cboReports.SelectedValue == "10078" && this.cboReports.SelectedValue == "10079"))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Please decrease report date range! A maximum of 366 days allowed";
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

                /* Operational Log Report Parameters Validation*/
                if (this.pnlOperationalLogs.Visible)
                {
                    if (!this.chkDriver.Checked && !this.chkFleet.Checked && !this.chkVehicle.Checked && !this.chkUser.Checked)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Module is required for operational log report";
                        return false;
                    }
                    else if (!this.chkUpdate.Checked && !this.chkDelete.Checked && !this.chkAssign.Checked)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Action is required for operational log report";
                        return false;
                    }
                    else
                    {
                        this.lblMessage.Visible = false;
                        this.lblMessage.Text = "";
                    }
                }

                if (this.cboReports.SelectedValue == "123")
                    sn.Report.TmpData = this.txtMileageDivider.Text + "," + this.txtScoreA.Text + "," + this.txtScoreB.Text + "," + this.txtScoreC.Text;

                ret = extendedReport.CreateReportParams();

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



        /// <summary>
        /// Create parameters for clsStandardReport class
        /// </summary>
        private void GenerateReportParameters()
        {
            extendedReport.sn = sn;
            extendedReport.basepage = this;
            extendedReport.cboReports = cboReports.SelectedValue;
            extendedReport.cboViolationSpeed = cboViolationSpeed.SelectedItem.Value;
            extendedReport.txtCost = txtCost.Text;
            extendedReport.txtColorFilter = txtColorFilter.Text;
            extendedReport.optEndTrip = optEndTrip.SelectedValue;
            extendedReport.chkSpeedViolation = chkSpeedViolation.Checked;
            extendedReport.DropDownList1 = DropDownList1.SelectedValue;
            extendedReport.chkHarshAcceleration = chkHarshAcceleration.Checked;
            extendedReport.chkHarshBraking = chkHarshBraking.Checked;
            extendedReport.chkExtremeAcceleration = chkExtremeAcceleration.Checked;
            extendedReport.chkExtremeBraking = chkExtremeBraking.Checked;
            extendedReport.chkSeatBeltViolation = chkSeatBeltViolation.Checked;
            extendedReport.txtFrom = txtFrom.SelectedDate;
            extendedReport.txtTo = txtTo.SelectedDate;


            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"]!="")
                extendedReport.cboFleet = Request.Form["OrganizationHierarchyFleetId"];
            else
                extendedReport.cboFleet = cboFleet.SelectedItem.Value; //cboFleet.SelectedItem


            extendedReport.cboVehicle = cboVehicle.SelectedValue;
            extendedReport.cboFormat = cboFormat.SelectedValue;
            extendedReport.cboFleet_Name = cboFleet.SelectedItem.Text;
            extendedReport.selectedFleets = string.Empty;
            extendedReport.keyValue = GenerateKeyValue();
            if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                {
                    if (dr["FleetId"] != null)
                        extendedReport.selectedFleets = string.Format("{0},{1}", extendedReport.selectedFleets, dr["FleetId"].ToString());
                }

            }

            extendedReport.txtSpeed120 = txtSpeed120.Text.Trim();
            extendedReport.txtAccHarsh = txtAccHarsh.Text.Trim();
            extendedReport.txtBrakingExtreme = txtBrakingExtreme.Text.Trim();
            extendedReport.txtSpeed130 = txtSpeed130.Text.Trim();
            extendedReport.txtAccExtreme = txtAccExtreme.Text.Trim();
            extendedReport.txtSeatBelt = txtSeatBelt.Text.Trim();
            extendedReport.txtSpeed140 = txtSpeed140.Text.Trim();
            extendedReport.txtBrakingHarsh = txtBrakingHarsh.Text.Trim();
            extendedReport.ddlLandmarks = ddlLandmarks.SelectedValue;
            extendedReport.ddlGeozones = ddlGeozones.SelectedValue;
            extendedReport.chkActiveVehicles = chkActiveVehicles.Checked;
            extendedReport.OrganizationHierarchyNodeCode = Request.Form["OrganizationHierarchyNodeCode"];

            extendedReport.cboIdlingThreshold = cboIdlingThreshold.SelectedValue;
            extendedReport.cboSpeedThreshold = cboSpeedThreshold.SelectedValue;
            extendedReport.cboMediaType = cboMediaType.SelectedValue;
            extendedReport.ddlDrivers = ddlDrivers.SelectedValue;
            sn.Report.ReportType = cboReports.SelectedValue;
        }

        /// <summary>
        /// Fill all controls 
        /// </summary>
        private void FillControlsByparameters()
        {
            try
            {
                cboReports.FindItemByValue(extendedReport.cboReports).Selected = true;
                cboReports_SelectedIndexChanged(null, null);
            }
            catch (Exception ex) { }

            if (cboFleet.Visible)
            {
                try
                {
                    cboFleet.FindItemByValue(extendedReport.cboFleet).Selected = true;
                    cboFleet_SelectedIndexChanged(null, null);
                }
                catch (Exception ex) { }
            }
            try
            {
                cboViolationSpeed.FindItemByValue(extendedReport.cboViolationSpeed).Selected = true;
            }
            catch (Exception ex) { }

            txtCost.Text = extendedReport.txtCost;
            txtColorFilter.Text = extendedReport.txtColorFilter;
            try
            {
                optEndTrip.Items.FindByValue(extendedReport.optEndTrip).Selected = true;
            }
            catch (Exception ex) { }
            chkSpeedViolation.Checked = extendedReport.chkSpeedViolation;

            try
            {
                DropDownList1.FindItemByValue(extendedReport.DropDownList1).Selected = true;
            }
            catch (Exception ex) { }

            chkHarshAcceleration.Checked = extendedReport.chkHarshAcceleration;
            chkHarshBraking.Checked = extendedReport.chkHarshBraking;
            chkExtremeAcceleration.Checked = extendedReport.chkExtremeAcceleration;
            chkExtremeBraking.Checked = extendedReport.chkExtremeBraking;
            chkSeatBeltViolation.Checked = extendedReport.chkSeatBeltViolation;
            txtFrom.SelectedDate = extendedReport.txtFrom;
            txtTo.SelectedDate = extendedReport.txtTo;

            try
            {
                cboVehicle.FindItemByValue(extendedReport.cboVehicle).Selected = true;
            }
            catch (Exception ex) { }

            try
            {
                cboFormat.FindItemByValue(extendedReport.cboFormat).Selected = true;
            }
            catch (Exception ex) { }

            string selectedFleets = "";
            if (!string.IsNullOrEmpty(extendedReport.selectedFleets)) selectedFleets = extendedReport.selectedFleets;
            lstAss.Items.Clear();
            if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
                sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Clear();


            if (selectedFleets != string.Empty)
            {
                List<string> selectedFleetsArray = new List<string>(selectedFleets.Split(','));
                foreach (ListItem li in lstUnAss.Items)
                {
                    if (selectedFleetsArray.Contains(li.Value)) li.Selected = true;
                }
                cmdAdd_Click(null, null);
                lstUnAss.ClearSelection();
            }

            txtSpeed120.Text = extendedReport.txtSpeed120;
            txtAccHarsh.Text = extendedReport.txtAccHarsh;
            txtBrakingExtreme.Text = extendedReport.txtBrakingExtreme;
            txtSpeed130.Text = extendedReport.txtSpeed130;
            txtAccExtreme.Text = extendedReport.txtAccExtreme;
            txtSeatBelt.Text = extendedReport.txtSeatBelt;
            txtSpeed140.Text = extendedReport.txtSpeed140;
            txtBrakingHarsh.Text = extendedReport.txtBrakingHarsh;
            chkActiveVehicles.Checked = extendedReport.chkActiveVehicles;
            try
            {
                ddlLandmarks.FindItemByValue(extendedReport.ddlLandmarks).Selected = true; ;
            }
            catch (Exception ex)
            { }

            try
            {
                ddlGeozones.FindItemByValue(extendedReport.ddlGeozones).Selected = true; ;
            }
            catch (Exception ex)
            { }

            try
            {
                cboIdlingThreshold.Items.FindByValue(extendedReport.cboIdlingThreshold).Selected = true;
            }
            catch (Exception ex) { }

            try
            {
                cboSpeedThreshold.Items.FindByValue(extendedReport.cboSpeedThreshold).Selected = true;
            }
            catch (Exception ex) { }

            try
            {
                cboMediaType.Items.FindByValue(extendedReport.cboMediaType).Selected = true;
            }
            catch (Exception ex) { }


            try
            {
                ddlDrivers.Items.FindByValue(extendedReport.ddlDrivers).Selected = true;
            }
            catch (Exception ex) { }

        }

        protected void cmdShow_Click(object sender, System.EventArgs e)
        {
            lblMessage.Text = ""; 
            lblMessage.Visible = false;


            extendedReport = new clsExtendedReport();
            
            GenerateReportParameters();
            
            if (!CreateReportParams("frmReportViewer.aspx")) return;

            int iSelectedReport = Convert.ToInt32(this.cboReports.SelectedValue);

            //Salt Spreader
            if (iSelectedReport == 126)
            {
                CreateResponsePage("SSRSReports/frmSaltSpreaderSummaryReport.aspx");
                return;
            }


            //One Time Report
            if (hidSubmitType.Value == "1")
            {
                if (iSelectedReport < 10000)
                {
                    clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                    if (!genReport.CallReportService(sn, this, null, extendedReport.keyValue))
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = Resources.Const.Reports_LoadFailed;
                        return;
                    }
                
                }
                else
	            {
                    object asState = null;
                    string sjson = GenerateReportParameters_JSON(iSelectedReport);

                    // Asynch Call for enterprise processes
                    ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();
                    IAsyncResult AsyncResult = sr.BeginRenderRepositoryReport(sjson, wsCallback, asState);

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

                if (iSelectedReport < 10000)
                    xmlDoc.CreateNode("XmlParams", sn.Report.XmlParams);
                else
                    xmlDoc.CreateNode("XmlParams",  GenerateReportParameters_JSON(iSelectedReport));

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
                userReportProperty.XmlParams = extendedReport.CreateCustomProperty();

                if (iSelectedReport < 10000)
                {
                    clsAsynGenerateReport genReport = new clsAsynGenerateReport();
                    if (!genReport.CallReportService(sn, this, userReportProperty, extendedReport.keyValue))
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = Resources.Const.Reports_LoadFailed;
                        return;
                    }
                }
                else
                {
                    object asState = null;
                    string sjson = GenerateReportParameters_JSON(iSelectedReport);

                    // Asynch Call for enterprise processes
                    ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();
                    IAsyncResult AsyncResult = sr.BeginRenderRepositoryReport(sjson, wsCallback, asState);
                }
                
                RadTabStrip1.FindTabByValue("1").Selected = true;
                RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                //sn.Report 
            }

        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            RedirectToLogin();
        }

        //For execute MaintenanceReportMyReport
        protected void cmdShowMyReport_Click(object sender, System.EventArgs e)
        {
            extendedReport = new clsExtendedReport();
            GenerateReportParameters();
            if (!CreateReportParams("frmReportViewer.aspx")) return;
            clsAsynGenerateReport genReport = new clsAsynGenerateReport();
            if (!genReport.CallReportService(sn, this, null, extendedReport.keyValue))
            {
                lblMessage.Visible = true;
                lblMessage.Text = Resources.Const.Reports_LoadFailed;
                return;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "frmMyReports_Backto_Repository", "Sys.Application.add_load(parent.frmMyReports_Backto_Repository)", true);

        }

        //For execute and update MaintenanceReportMyReport
        protected void cmdShowMyReportUpdate_Click(object sender, System.EventArgs e)
        {
            extendedReport = new clsExtendedReport();
            GenerateReportParameters();
            if (!CreateReportParams("frmReportViewer.aspx")) return;

            clsUserReportParams userReportProperty = new clsUserReportParams();
            userReportProperty.XmlParams = extendedReport.CreateCustomProperty();
            userReportProperty.ReportRepositoryId = Request.QueryString["id"].ToString();
            clsAsynGenerateReport genReport = new clsAsynGenerateReport();
            if (!genReport.CallReportService(sn, this, userReportProperty, extendedReport.keyValue))
            {
                lblMessage.Visible = true;
                lblMessage.Text = Resources.Const.Reports_LoadFailed;
                return;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "frmMyReports_Backto_Repository", "Sys.Application.add_load(parent.frmMyReports_Backto_Repository_u)", true);

        }

        /// <summary>
        /// Generate key value for report repository table
        /// </summary>
        /// <param name="isFleetMaintenanceReport"></param>
        /// <returns></returns>
        private string GenerateKeyValue()
        {
            string keyvalue_fleet = (string)base.GetLocalResourceObject("keyvalue_fleet");
            string keyvalue_vehicle = (string)base.GetLocalResourceObject("keyvalue_vehicle");
            string keyvalue_startdate = (string)base.GetLocalResourceObject("keyvalue_startdate");
            string keyvalue_enddate = (string)base.GetLocalResourceObject("keyvalue_enddate");

            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0)
            {
                string fid = Request.Form["OrganizationHierarchyFleetId"];
                if (!string.IsNullOrEmpty(fid))
                    sn.Report.FleetId = Convert.ToInt32(fid);                
            }
            else
                sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);

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

            if (txtFrom.Visible)
            {
                string startDate = txtFrom.SelectedDate.Value.ToString("M/d/yyyy");
                sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_startdate, startDate));
            }

            if (txtTo.Visible)
            {
                string endDate = txtTo.SelectedDate.Value.ToString("M/d/yyyy");
                sb.Append(string.Format(" <b>{0}</b>={1}", keyvalue_enddate, endDate));
            }
            return sb.ToString();

        }


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

                //if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), false))
                //    if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //        //RedirectToLogin();
                //        return;
                //    }
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
                this.ddlGeozones.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlGeozones_Item_0").ToString(), "-1"));
            }
            else
                this.ddlGeozones.Items.Insert(0, new RadComboBoxItem(base.GetLocalResourceObject("ddlGeozones_NoAvailable").ToString(), "-100"));
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

        //private void HideHierarchy()
        //{
        //    trFleet.Visible = true;
        //    organizationHierarchy.Visible = false;
        //    this.optReportBased.SelectedIndex = 1;
        //    optBaseTable.Visible = false;
        //}
        //Devin added
        protected void btnAfterCreate_Click(object sender, EventArgs e)
        {
            ReportCriteria();
        }


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
        /// Create error message from resource file
        /// </summary>
        private void CreateErrorMessageForClientValidate()
        {
            errvalHierarchyMessage = (string)base.GetLocalResourceObject("valHierarchyMessage");
        }

        #region Reporting Service Assistance Section

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DateValue"></param>
        /// <param name="Dateformat"></param>
        /// <returns></returns>
        public DateTime ConvertStringToDateTime(string DateValue, string Dateformat)
        {

            return ConvertStringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }
        /// <summary>
        /// 
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

        /// <summary>
        /// for async call parameter
        /// </summary>
        /// <param name="iResult"></param>
        private void wsCallback(IAsyncResult iResult) { }

        /// <summary>
        /// lsz 2013-03-21 for handling SSRS Reports
        /// </summary>
        /// <returns></returns>
        private void ReportingServices()
        {

            extendedReport = new clsExtendedReport();

            if (!CreateReportParams("frmReportViewer.aspx"))
                return;

            //RepositoryServerReports(sjson);
            string sjson = GenerateReportParameters_JSON(Convert.ToInt32(this.cboReports.SelectedValue));
            string message = string.Empty;

            object asState = null;

            switch (hidSubmitType.Value)
            {

                //One Time Report
                //if (hidSubmitType.Value == "1"){
                case "1":

                    #region One Time (Repository) Report
                    {
                        // Asynch Call for enterprise processes
                        ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();
                        IAsyncResult AsyncResult = sr.BeginRenderRepositoryReport(sjson, wsCallback, asState);

                        #region Testing code
                        // Synch call for testing scheduled report
                        //string Url = "";
                        //if (sr.RenderScheduleReport(sn.UserID, "", "2013/01/27", "2013/01/30", sjson,  1, "en", ref Url))
                        //    message = "";
                        //else
                        //    message = sr.Message().ToString();

                        // Synch call for testing repository report
                        //if (sr.RenderRepositoryReport(sjson))
                        //    message = "";
                        //else
                        //    message = sr.Message().ToString();
                        #endregion

                        lblMessage.Visible = false;
                        lblMessage.Text = "";                //this.lblMessage.Text = "Rendering report successfully, please review Repository Tab for detail.";

                        RadTabStrip1.FindTabByValue("1").Selected = true;
                        RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                    }
                    #endregion

                    break;

                //}

                //Schedule Report
                //if (hidSubmitType.Value == "2") {
                case "2":

                    #region Schedule Report
                    {
                        clsXmlUtil xmlDoc = new clsXmlUtil(sn.Report.XmlDOC);
                        xmlDoc.CreateNode("IsFleet", sn.Report.IsFleet);
                        xmlDoc.CreateNode("FleetId", sn.Report.FleetId);
                        xmlDoc.CreateNode("XmlParams", sjson);                      //sn.Report.XmlParams);
                        xmlDoc.CreateNode("GuiId", sn.Report.GuiId);
                        xmlDoc.CreateNode("ReportFormat", sn.Report.ReportFormat);  // new 2008 - 05 - 05
                        xmlDoc.CreateNode("FromDate", Convert.ToDateTime(sn.Report.FromDate, new CultureInfo("en-US")));
                        xmlDoc.CreateNode("ToDate", Convert.ToDateTime(sn.Report.ToDate, new CultureInfo("en-US")));

                        using (ServerReports.Reports reportProxy = new ServerReports.Reports())
                        {
                            lblMessage.Visible = true;
                            if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), false))
                            {
                                if (objUtil.ErrCheck(reportProxy.AddReportSchedule(sn.UserID, sn.SecId, xmlDoc.Xml), true))
                                {
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,
                                        VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning,
                                        "Schedule report failed. User: " + sn.UserID.ToString() + " Form:frmReportScheduler.aspx"));
                                    ShowMessage(lblMessage, this.GetLocalResourceObject("resScheduleFailed").ToString(), Color.Red);
                                    return;
                                }
                                else
                                {
                                    ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);
                                }
                            }
                            else
                            {
                                ShowMessage(this.lblMessage, this.GetLocalResourceObject("lblMessage_Text_ScheduledSuccess").ToString(), Color.Green);
                            }
                        }
                    }
                    #endregion

                    break;
                //}

                //My Reports
                //if (hidSubmitType.Value == "3"){
                case "3":

                    #region My Reports
                    {
                        //string xmlDOC = sn.Report.XmlDOC;
                        //string reportName = clsAsynGenerateReport.PairFindValue("ReportName", xmlDOC);
                        //string reportDescription = clsAsynGenerateReport.PairFindValue("ReportDescription", xmlDOC);

                        //clsUserReportParams userReportProperty = new clsUserReportParams();
                        //userReportProperty.ReportName = reportName;
                        //userReportProperty.ReportDescription = reportDescription;
                        //userReportProperty.XmlParams = standardReport.CreateCustomProperty();

                        // Asynch Call for enterprise processes
                        ReportingServices.ReportingServices sr = new ReportingServices.ReportingServices();
                        IAsyncResult AsyncResult = sr.BeginRenderRepositoryReport(sjson, wsCallback, asState);

                        lblMessage.Visible = false;
                        lblMessage.Text = "";                //this.lblMessage.Text = "Rendering report successfully, please review Repository Tab for detail.";

                        RadTabStrip1.FindTabByValue("1").Selected = true;
                        RadMultiPage1.FindPageViewByID("Repository").Selected = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAfterSubmit", "Sys.Application.add_load(refreshAfterSubmit)", true);
                    }
                    #endregion

                    break;
                //}
            }
        }

        public string GenerateReportParameters_JSON(int ReportID)      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {
            ReportID = Math.Abs(ReportID);

            StringBuilder sbp = new StringBuilder();

            // Basic parameters
            sbp.Append("reportid: " + ReportID.ToString() + ", ");                      // this.cboReports.SelectedItem.Value + ", ");
            sbp.Append("reportformat: " + cboFormat.SelectedItem.Text + ", ");          // PDF; EXCEL; WORD;....   .SelectedValue.ToString()
            sbp.Append("reportformatcode: " + cboFormat.SelectedItem.Value + ", ");     // 1;   2;     3;   ....   .SelectedValue.ToString()
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

            // Fleet
            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
                sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
            else if (cboFleet.Visible && cboFleet.Enabled)
                sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
            else
                sbp.Append("");

            // Vehicle ID
            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyBoxId"] != "")
                sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + ", ");       // Box ID
            else if (this.cboVehicle.Visible && this.cboVehicle.Enabled && this.cboVehicle.SelectedIndex > 0)
            {
                string dvf = this.cboVehicle.DataValueField.ToString().ToLower();
                if (dvf == "vehicleid")
                    sbp.Append("vehicleid: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
                else if (dvf == "boxid")
                    sbp.Append("boxid: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
                else if (dvf == "licenseplate")
                    sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
                else
                    sbp.Append(dvf + ": " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
            }
            else
                sbp.Append("");

            // Vehicle Name
            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyVehicleDescription"] != "")
                sbp.Append("vehiclename: " + Request.Form["OrganizationHierarchyVehicleDescription"] + ", ");   //standardReport.cboVehicle_Name = Request.Form["OrganizationHierarchyVehicleDescription"];
            else if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
                sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
            else
                sbp.Append("");

            // Date range - Extended no time
            string df = txtFrom.SelectedDate.Value.ToString("yyyy/MM/dd") + " 12:00:00 AM";     //+ this.cboHoursFrom.SelectedDate.Value.ToString("hh:mm:ss tt");
            string dt = txtTo.SelectedDate.Value.ToString("yyyy/MM/dd") + " 12:00:00 AM";       //+ cboHoursTo.SelectedDate.Value.ToString("hh:mm:ss tt");

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
                else if(this.DropDownList1.Visible && this.DropDownList1.Enabled)
                    sbp.Append("speedlimitation: " + GetViolationSpeed(this.DropDownList1.SelectedItem.Value) + ", ");
                else
                    sbp.Append("speedlimitation: 100, ");

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
            //if (tblRoadSpeed.Visible)
            //{

            //    // all if unchecked.
            //    if (chkIsPostedOnly.Visible && chkIsPostedOnly.Enabled)
            //    {
            //        if (chkIsPostedOnly.Checked)
            //            sbp.Append("postedonly: 1, ");
            //        else
            //            sbp.Append("postedonly: 2, ");              //  0, ");
            //    }

            //    if (cboRoadSpeedDelta.Visible && cboRoadSpeedDelta.Enabled)
            //        sbp.Append("roadspeeddelta: " + cboRoadSpeedDelta.SelectedItem.Value + ", ");
            //}
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

            #region Sensor Number
            // sensorNum = 3 (default)
            if (this.optEndTrip.Visible && this.optEndTrip.Enabled)
            {
                if (optEndTrip.SelectedIndex >= 0)
                    sbp.Append("sensornumber: " + optEndTrip.SelectedValue + ", ");
                else
                    sbp.Append("sensornumber: 3, ");
            }
            #endregion

            #region Operational Log Report
            if (this.pnlOperationalLogs.Visible)
            {
                string modules = string.Empty;
                string actions = string.Empty;
                string items = string.Empty;

                // Construct modules
                if (this.chkFleet.Checked)
                    modules += "|" + this.chkFleet.AccessKey.ToString();

                if (this.chkVehicle.Checked)
                    modules += "|" + this.chkVehicle.AccessKey.ToString();

                if (this.chkUser.Checked)
                    modules += "|" + this.chkUser.AccessKey.ToString();

                if (this.chkDriver.Checked)
                    modules += "|" + this.chkDriver.AccessKey.ToString();

                sbp.Append("modules: " + modules + ", ");

                if (this.chkUpdate.Checked)
                    actions += "|" + this.chkUpdate.Text;

                if (this.chkAssign.Checked)
                    actions += "|" + this.chkAssign.Text;

                if (this.chkDelete.Checked)
                    actions += "|" + this.chkDelete.Text;

                sbp.Append("actions: " + actions + ", ");
                sbp.Append("updatebyusers: " + this.ddlUpdateUsers.SelectedItem.Value.ToString() + ", ");

            }
            #endregion

            #region Battery Trending Threshold
            if (this.rbVoltageThreshold.Visible && this.rbVoltageThreshold.Enabled)
            {
                switch (this.rbVoltageThreshold.SelectedItem.Value) { 
                    case "1":
                        sbp.Append("fromthres: 0.0000, ");
                        sbp.Append("tothres: 10.9999, ");
                        break;
                    case "2":
                        sbp.Append("fromthres: 11.0000, ");
                        sbp.Append("tothres: 12.5000, ");
                        break;
                    case "3":
                        sbp.Append("fromthres: 12.5999, ");
                        sbp.Append("tothres: 999.0000, ");
                        break;
                    default:
                        sbp.Append("fromthres: 0.0000, ");
                        sbp.Append("tothres: 999.0000, ");
                        break;
                }
            }
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
            this.lblMessage.ForeColor = Color.Transparent;
            this.lblMessage.Text = "";

            try
            {
                string tmp = this.cboReports.SelectedValue.ToString();
                // Vehicle: Box (hierarchy) or License Plate (fleet)
                if (!IsValidVehicle(tmp))
                    msg = (string)base.GetLocalResourceObject("valSelectVehicle");      //this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                // Fleet:
                else if (!IsValidFleet(tmp))
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
            finally
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = msg;
                    this.lblMessage.ForeColor = Color.Red;
                }
            }
            return !this.lblMessage.Visible;
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
        /// Get violation mask string
        /// </summary>
        /// <returns></returns>
        private string GetViolationMaskString()
        {
            return GetViolationMaskNumber().ToString();
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
                case "6":
                    if (sn.User.UnitOfMes == 1)
                        speed = "140";
                    else
                        speed = "145";  // 90
                    break;
                case "5":
                    speed = "130";  //80
                    break;
                case "4":
                    speed = "120"; //75
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

        /// <summary>
        /// if this.cboReports.SelectedValue == "25" 
        /// || this.cboReports.SelectedValue == "26"
        /// || this.cboReports.SelectedValue == "27"
        /// || this.cboReports.SelectedValue == "28"
        /// || this.cboReports.SelectedValue == "53"
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        private bool IsValidDriver(string Report)
        {
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
                else if (ShowOrganizationHierarchy && organizationHierarchy.Visible)
                {
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsValidDateTime(out string Message)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            DateTime datefr = Convert.ToDateTime((this.txtFrom.SelectedDate.Value.ToString(DateFormat) + " 12:00:00 AM"), ci);
            DateTime dateto = Convert.ToDateTime((this.txtTo.SelectedDate.Value.ToString(DateFormat) + " 12:00:00 AM"), ci);
            TimeSpan dtDiff = dateto - datefr;

            Message = "";
            if ("{10078|10079}".IndexOf(this.cboReports.SelectedValue) > 0)
            {
                if (dtDiff.Days < 182)
                    return string.IsNullOrEmpty(Message);
                else {
                    Message = "Please decrease report date range! A maximum of six months allowed";
                    return string.IsNullOrEmpty(Message);
                }
            }
            if (dtDiff.Days < 1)
                Message = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
            else if (dtDiff.Days > 31)
                Message = GetLocalResourceObject("MessageRptDateRange").ToString();
            else
                Message = "";

            return string.IsNullOrEmpty(Message);
        }

        /// <summary>
        /// this.cboReports.SelectedValue == "22"
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        private bool IsValidGeozone(string Report)
        {
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
        private bool IsValidVehicle(string Report)
        {
            //if (((this.cboVehicle.SelectedIndex == 0 && this.cboReports.SelectedValue == "3") ||
            //    this.cboVehicle.SelectedIndex == -1) && cboVehicle.Visible == true) //&& cboVehicle.Visible == true added by devin
            //{
            //    this.lblMessage.Visible = true;
            //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
            //    return false;
            //}
            if ("|3|40|63|10020|".Contains(Report))
            {
                if (organizationHierarchy.Visible && Request.Form["OrganizationHierarchyBoxId"] == "")
                    return false;
                else if (this.cboVehicle.Visible && this.cboVehicle.Enabled && this.cboVehicle.SelectedIndex == 0)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }
        private bool IsMultiHierarchyReport(string Report) 
        {
            bool isMultiFleetReport = false;

            switch (Report)
            { 
                case "10060":      // Speed Distribution Report (MF)
                case "10053":      // Off Road Miles Report (State-Province)
                case "10059":      // Sensor Activity Report
                case "10050":      // Good / Bad Idling Report
                case "10070":       // Battery Trending Report
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

        #endregion  

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
    }
}
