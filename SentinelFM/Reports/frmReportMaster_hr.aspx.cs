#region Namespace Reference section

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


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

        public const string tblWidth = "85%";


       public bool ShowOrganizationHierarchy;
       public bool OrganizationHierarchySelectVehicle = false;

       public int DefaultOrganizationHierarchyFleetId = 0;
       public string DefaultOrganizationHierarchyFleetName = string.Empty;
       public string DefaultOrganizationHierarchyNodeCode = string.Empty;
       public string OrganizationHierarchyPath = "";
       public int VehiclePageSize = 10;
       public bool MutipleUserHierarchyAssignment = false;
       public string PreferOrganizationHierarchyNodeCode = string.Empty;
       public bool IniHierarchyPath = false;

       static bool isMaintenance = false;
       static int DOHFleetId = 0;
       static string DOHFleetName = string.Empty;
       static string DOHNodeCode = string.Empty;
       static bool RememberSelection = false;

       private string CurrentUICulture = "en-US";
       private string DateFormat = "MM/dd/yyyy";
       private string TimeFormat = "hh:mm:ss tt";

       private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;
       private VLF.PATCH.Logic.PatchVehicle pveh;

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
             sn.Report.ReportActiveTab = 0;

             #region Initialize Current UI Culture, Date Format, Date Control value.

             CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture.ToString();

             txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
             txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
             //txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
             //txtFrom.DateInput.Culture = System.Globalization.CultureInfo.CurrentUICulture;
             //txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();

             txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
             txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
             //txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
             //txtTo.DateInput.Culture = System.Globalization.CultureInfo.CurrentUICulture;
             //txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();

             if (CurrentUICulture.ToLower().IndexOf("fr") >= 0)
             {
                 DateFormat = "dd/MM/yyyy";
                 TimeFormat = "HH:mm:ss";

                 txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                 txtFrom.DateInput.DateFormat = DateFormat;
                 txtFrom.DateInput.DisplayDateFormat = DateFormat;
                 txtFrom.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy"; 
                 //txtFrom.CultureInfo.CultureName = "fr-FR";

                 txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr; 
                 txtTo.DateInput.DateFormat = DateFormat;
                 txtTo.DateInput.DisplayDateFormat = DateFormat;
                 txtTo.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy"; 
                 //txtTo.CultureInfo.CultureName = "fr-FR";

                 //tblWidth = "85%";
                 //cboFromDayH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                 //cboToDayH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                 //cboWeekEndFromH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                 //cboWeekEndToH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                 cboHoursFrom.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
                 cboHoursTo.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");

             }
             else
             {
                 DateFormat = "MM/dd/yyyy";
                 TimeFormat = "hh:mm:ss tt";

                 txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                 txtFrom.DateInput.DateFormat = DateFormat;
                 txtFrom.DateInput.DisplayDateFormat = DateFormat;
                 txtFrom.Calendar.DayCellToolTipFormat = "dddd MMM dd, yyyy"; 
                 //txtFrom.CultureInfo.CultureName = "en-US";

                 txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                 txtTo.DateInput.DateFormat = DateFormat;
                 txtTo.DateInput.DisplayDateFormat = DateFormat;
                 txtTo.Calendar.DayCellToolTipFormat = "dddd MMM dd, yyyy"; 
                 //txtTo.CultureInfo.CultureName = "en-US";

             }

             string datetime = "";

             if (Request[this.txtFrom.UniqueID] != null)
                 datetime = Convert.ToDateTime(Request[this.txtFrom.UniqueID].ToString()).ToString(DateFormat);                    //this.txtFrom.Text 
            else if (!String.IsNullOrEmpty(sn.Report.FromDate))
                datetime = Convert.ToDateTime(sn.Report.FromDate).ToString(DateFormat);
            else
                 datetime = DateTime.Now.ToString(DateFormat);

             this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, DateFormat, CurrentUICulture);   //Convert.ToDateTime(datetime);       //

             if (Request[this.txtTo.UniqueID] != null)
                datetime = Convert.ToDateTime(Request[this.txtTo.UniqueID].ToString()).ToString(DateFormat);
            else  if (!String.IsNullOrEmpty(sn.Report.ToDate))
                datetime = Convert.ToDateTime(sn.Report.ToDate).ToString(DateFormat);
            else
                 datetime = DateTime.Now.AddDays(1).ToString(DateFormat);

             this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, DateFormat, CurrentUICulture);     //Convert.ToDateTime(datetime);  // ;


             DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

             cboHoursFrom.TimeView.TimeFormat = TimeFormat;
             cboHoursFrom.DateInput.DateFormat = TimeFormat;
             cboHoursFrom.DateInput.DisplayDateFormat = TimeFormat;
             cboHoursFrom.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
             cboHoursFrom.SelectedDate = ConvertStringToDateTime(time.ToString(), DateFormat + " " + TimeFormat);
             //this.cboHoursFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

             cboHoursTo.TimeView.TimeFormat = TimeFormat;
             cboHoursTo.DateInput.DateFormat = TimeFormat;
             cboHoursTo.DateInput.DisplayDateFormat = TimeFormat;
             cboHoursTo.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
             cboHoursTo.SelectedDate = ConvertStringToDateTime(time.ToString(), DateFormat + " " + TimeFormat);
             //this.cboHoursTo.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

             //cboFromDayH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
             //cboToDayH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
             //cboWeekEndFromH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");
             //cboWeekEndToH.TimePopupButton.ToolTip = (string)base.GetLocalResourceObject("TimePopupButton.ToolTip");

             #endregion

            //Show Busy Message
            cmdShow.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
            cmdPreviewFleetMaintenanceReport.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
            this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
            this.BusyReport.Text = (string)base.GetLocalResourceObject("BusyPreparingMessage");

            ShowOrganizationHierarchy = false;
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            pveh = new VLF.PATCH.Logic.PatchVehicle(sConnectionString);
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

                    //defaultnodecode = defaultnodecode ?? string.Empty;
                    defaultnodecode = sn.User.PreferNodeCodes;
                    PreferOrganizationHierarchyNodeCode = defaultnodecode;
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
                    if (!IsPostBack)
                    {
                        if (isMaintenance)
                        {
                            DefaultOrganizationHierarchyFleetId = DOHFleetId;
                            DefaultOrganizationHierarchyFleetName = DOHFleetName;
                            DefaultOrganizationHierarchyNodeCode = DOHNodeCode;
                            hidOrganizationHierarchyNodeCode.Value = DOHNodeCode;
                            hidOrganizationHierarchyFleetId.Value = DOHFleetId.ToString();
                            hidOrganizationHierarchyFleetName.Value = DOHFleetName;
                            isMaintenance = false;
                        }
                        else
                        {
                            DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                            DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                            hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                            hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                            hidOrganizationHierarchyFleetName.Value = DefaultOrganizationHierarchyFleetName;
                            OrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                        }

                        if (sn.Report.OrganizationHierarchyPath != null && sn.Report.OrganizationHierarchyPath != string.Empty)
                            OrganizationHierarchyPath = sn.Report.OrganizationHierarchyPath;
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }

                    if (Page.IsPostBack)
                        OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, Request.Form["OrganizationHierarchyNodeCode"].ToString());

                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                    sn.Report.OrganizationHierarchyPath = OrganizationHierarchyPath;

                     ReportBasedOption();
                }
                else
                {
                    this.organizationHierarchy.Visible = false;
                     this.vehicleSelectOption.Visible = false;
                     this.trFleet.Visible = true;  
                }

                //optMaintenanceBased.SelectedValue = (OHSelected ? "0" : "1");
                //optMaintenanceBased.Items[0].Value = (OHSelected ? "0" : "1");
            if (!Page.IsPostBack)
            {

                 if (ShowOrganizationHierarchy)
                    this.optReportBased.SelectedIndex = 0;
    
               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmReportMaster, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                //GuiSecurity(this);
               if (!(sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 1000151))
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
               clsMisc.cboHoursFill(ref cboFromDayH);
               clsMisc.cboHoursFill(ref cboToDayH);
               clsMisc.cboHoursFill(ref cboWeekEndFromH);
               clsMisc.cboHoursFill(ref cboWeekEndToH);

               //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
               //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;

               this.cboFromDayH.SelectedIndex = 8;
               this.cboToDayH.SelectedIndex = 18;
               this.cboWeekEndFromH.SelectedIndex = 8;
               this.cboWeekEndToH.SelectedIndex = 18;
               CboFleet_Fill();
               cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

               if ((sn.Report.FleetId != 0) && (sn.Report.FleetId!=-1))
               {
                  cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Report.FleetId.ToString()));
                  CboVehicle_Fill(Convert.ToInt32(sn.Report.FleetId));

                  if (sn.Report.LicensePlate!="")
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



               if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 1000151)
               {
                   //removeReportSN("53");
                   //removeReportSN("103");
                   //removeReportSN("63");
                   //removeReportSN("53");
                   //removeReportSN("5");
                   //removeReportSN("89");
                   //removeReportSN("40");
                   //removeReportSN("34");
                   //removeReportSN("36");



                       cboReports.Items.Add(new ListItem("------Rapports sommaires------", "-1"));
                       cboReports.Items.Add(new ListItem("Rapport sommaire de déplacement", "1"));
                       //cboReports.Items.Add(new ListItem("Rapport sommaire sur les excès de vitesse basé sur la vitesse des routes", "97"));
                       cboReports.Items.Add(new ListItem("Rapport sommaire sur les excès de vitesse basé sur la vitesse des routes", "10057")); // Speed Violation Summary Report by Road Speed
                       //cboReports.Items.Add(new ListItem("Rapport sommaire des infractions de la flotte des véhicules", "20"));
                       cboReports.Items.Add(new ListItem("Rapport sommaire des infractions de la flotte des véhicules", "10061"));              // Fleet Violation Summary Report
                       cboReports.Items.Add(new ListItem("Rapport sommaire des sites", "21"));
                       cboReports.Items.Add(new ListItem("Rapport sommaire des géozones", "22"));
                       //cboReports.Items.Add(new ListItem("Rapport sommaire d'activités par véhicule", "10002"));                      // Activity Summary for Vehicle
                       cboReports.Items.Add(new ListItem("Rapport sommaire d'activités par véhicule", "10048"));                        // MF Activity Summary for Vehicle
                       cboReports.Items.Add(new ListItem("Rapport sommaire d'activités pour l'organisation (Hydro-Québec)", "10011"));  // Activity Summary for Organization
                       //cboReports.Items.Add(new ListItem("Rapport sommaire d'activités par véhicule", "39"));
                       //cboReports.Items.Add(new ListItem("Rapport sommaire d'activités pour l'organisation (Hydro-Québec)", "38"));
                       
                   
                       cboReports.Items.Add(new ListItem("------Rapports détaillés------", "-2"));
                       cboReports.Items.Add(new ListItem("Rapport détaillé des déplacements", "0"));
                       //cboReports.Items.Add(new ListItem("Rapport détaillé des infractions de la flotte des véhicules", "17"));
                       cboReports.Items.Add(new ListItem("Rapport détaillé des infractions de la flotte des véhicules", "10049"));              // Fleet Violation Details Report
                       cboReports.Items.Add(new ListItem("Rapport d'opérations en dehors des heures de travail", "8"));
                       cboReports.Items.Add(new ListItem("Rapport messages Garmin", "105"));
                       cboReports.Items.Add(new ListItem("Rapport des historiques", "3"));
                       cboReports.Items.Add(new ListItem("Rapport détaillé des sites", "23"));
                       cboReports.Items.Add(new ListItem("Rapport détaillé des géozones", "30"));

                       if (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 1208)//HGI Admins || Security Administrator
                           cboReports.Items.Add(new ListItem("Rapport détaillé d'accès des usagers", "60"));


                       cboReports.Items.Add(new ListItem("------Rapports d'entretien des véhicules------", "-3"));
                       //cboReports.Items.Add(new ListItem("Rapport sommaire d'activités par véhicule", "10002"));                      // Activity Summary for Vehicle
                       cboReports.Items.Add(new ListItem("Rapport sommaire d'activités par véhicule", "10048"));                        // MF Activity Summary for Vehicle
                       cboReports.Items.Add(new ListItem("Rapport sommaire d'activités pour l'organisation (Hydro-Québec)", "10011"));  // Activity Summary for Organization
                       //cboReports.Items.Add(new ListItem("Rapport sommaire d'activités par véhicule", "39"));
                       //cboReports.Items.Add(new ListItem("Rapport sommaire d'activités pour l'organisation (Hydro-Québec)", "38"));
                       //cboReports.Items.Add(new ListItem("Rapport détaillé de ralenti-moteur", "10018"));                               // Idling Detail Report
                       cboReports.Items.Add(new ListItem("Rapport détaillé de ralenti-moteur", "10051"));                               // Idling Detail Report
                       cboReports.Items.Add(new ListItem("Rapport sommaire des ralentis moteur", "10017"));                             // Idling Summary Report
                       //cboReports.Items.Add(new ListItem("Rapport sommaire des ralentis moteur", "88"));
                       //cboReports.Items.Add(new ListItem("Rapport d'arrêts et de ralentis moteur", "4"));                             // Stop and Idling Duration Report
                       cboReports.Items.Add(new ListItem("Rapport d'arrêts et de ralentis moteur", "10062"));
                       //cboReports.Items.Add(new ListItem("Rapport des alarmes", "2"));                                                // Alarms Report
                       cboReports.Items.Add(new ListItem("Rapport des alarmes", "10063"));
                       cboReports.Items.Add(new ListItem("Rapport de maintenance de la flotte", "10"));
                       cboReports.Items.Add(new ListItem("Sommaire des véhicules avec path de la hiérarchie, site et géo-zone associés", "10025"));
                       cboReports.Items.Add(new ListItem("Rapport d'assignation des usagers aux flottes", "10027"));
                       cboReports.Items.Add(new ListItem("Rapport sur l'état des Véhicules", "69"));

                       //cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue("1"));
                       if (sn.Report.GuiId > 0)
                       {
                           cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
                       }
                       else if (RememberSelection)
                       {
                           RememberSelection = false;
                           cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
                       }
                       else
                           cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue("1"));
                       
                       ReportCriteria();

                       return; 
                   
                   
               }
               else
               {

                   this.cboReports.DataSource = sn.Report.UserReportsDataSet;
                   this.cboReports.DataBind();

               }
               
                   
                //end remove report
                   
                   //cboReports.Items.Add(new ListItem("Geozone Report", "22"));
                   //cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                   //cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
               

               //cboReports.Items.Add(new ListItem("Trip Summary Totals Report per Vehicle", "50"));
               //cboReports.Items.Add(new ListItem("Trip Summary Totals Report per Organization", "51"));
               //cboReports.Items.Add(new ListItem("HOS Details Report per Driver", "53"));




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
                   removeReportSN("104");


               if (sn.User.UserGroupId == 1)
               {
                   //cboReports.Items.Add(new ListItem("BSM-Vehicle Information Data Dump", "54"));
                   AddReportItem(new ListItem("BSM-Vehicle Information Data Dump", "54"));
               }

               
               //cboReports.Items.Add(new ListItem("Transportation Mileage Report", "63"));
               //cboReports.Items.Add(new ListItem("Idling Details Report New", "87"));
               //cboReports.Items.Add(new ListItem("Idling Summary Report New", "88"));
               //cboReports.Items.Add(new ListItem("Trip Summary Report New", "89"));

               cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
               ReportCriteria();
            }
            else
            {
                   if (ShowOrganizationHierarchy)
                   {
                       MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                       if (MutipleUserHierarchyAssignment)
                           MutipleUserHierarchyAssignment = IsMultiHierarchyReport(this.cboReports.SelectedValue);
                   }
            }

            switch (this.cboReports.SelectedValue)
            {
                case "3": // History Report
                    OrganizationHierarchySelectVehicle = true;
                    break;
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
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                     return;
                  }
            }

            if (String.IsNullOrEmpty(xml))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
               return;
            }

            dsVehicle.ReadXml(new StringReader(xml));

            cboVehicle.DataSource = dsVehicle;
            cboVehicle.DataBind();
            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));

            if ((sn.Report.VehicleId != 0) && (sn.Report.VehicleId!=-1))
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
             OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, Request.Form["OrganizationHierarchyNodeCode"].ToString());

            ReportCriteria();

            //if (this.cboReports.SelectedItem.Text.Contains("BSM") || ( cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
            //{
            //    this.organizationHierarchy.Visible = false;
            //    this.vehicleSelectOption.Visible = false;
            //    this.trFleet.Visible = true;
            //}

            if (Convert.ToInt32(cboReports.SelectedItem.Value) > 89 && Convert.ToInt32(cboReports.SelectedItem.Value) != 103 && Convert.ToInt32(cboReports.SelectedItem.Value) < 10000)
            {
                this.cmdSchedule.Enabled = false;
                this.cmdViewScheduled.Enabled = false;
            }
            else
            {
                this.cmdSchedule.Enabled = true;
                this.cmdViewScheduled.Enabled = true;
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


         OrganizationHierarchySelectVehicle = false;

         if (Convert.ToInt16(this.cboReports.SelectedValue) >= 10000)
         {
             this.cboHoursFrom.Enabled = false;
             this.cboHoursTo.Enabled = false;
         }


         switch (this.cboReports.SelectedValue)
         {
            case "0": // Trip Details Report
               this.tblOptions1.Visible = true;
               this.tblOptions2.Visible = true;
               this.chkShowStorePosition.Visible = true;
               this.lblTripReportDesc.Visible = true;
               this.tblIgnition.Visible = true;
               OrganizationHierarchySelectVehicle = true;
               break;
            
            case "1": // Trip Summary Report
               this.lblTripSummaryReportDesc.Visible = true;
               this.chkShowStorePosition.Visible = true;
               this.tblIgnition.Visible = true;
               OrganizationHierarchySelectVehicle = true;	
               break;
            
            case "2": // Alarms Report
            case "10063":
               this.lblAlarmReportDesc.Visible = true;
               break;
            
            case "3": // History Report
               this.tblHistoryOptions.Visible = true;
               this.lblHistoryReportDesc.Visible = true;
               //HideHierarchy();
               this.cboFleet.Enabled = false;
               OrganizationHierarchySelectVehicle = true;
               break;
            
            case "4": // Stop & Idling Duration Report
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
               this.lblIdlingDetailsReportDesc.Visible = true;
               break;

            case "17": // violation details 4 fleet
            case "10013":
            case "10049":           // Fleet Violation Details Report (MF)
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
            case "10014": // violation summary 4 fleet
            case "10061":// violation summary 4 fleet (MF)
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
               //HideHierarchy();
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
           case "10011":

               FleetVehicleShow(false);
               this.tblIgnition.Visible = true;
               break;

       
           

           case "39":                            // Idling Detail Report
           case "50":
           case "62":
           case "87":
           case "10002":
           case "10018":
           case "10048":
           case "10051":
               this.cboVehicle.Enabled = false;
               this.tblIgnition.Visible = true;
               break;
           case "40":
           case "63":   //IVMR
           case "10020":
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
               this.cmdSchedule.Visible = false;
               this.cmdViewScheduled.Visible = false;
               LoadDrivers();
               break;

            case "54":
            case "58":
               FleetVehicleShow(false);
               this.txtTo.Enabled = false ;
               this.txtFrom.Enabled = false;
               this.cboHoursFrom.Enabled = false;
               this.cboHoursTo.Enabled = false;
             break;

            case "59":
            case "61":
            case "70":
            case "69":
           
             this.cboVehicle.Enabled = false;
             break;

            case "60":
             FleetVehicleShow(false);
             tblIgnition.Visible = false;  
             break;

            case "89":
             chkShowDriver.Visible = true;  
             break;

            case "97":          // Speed Violation Summary Report by Road Speed
            case "10057":       // Speed Violation Summary Report by Road Speed (MF)
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
             cboRoadSpeedDelta.Visible = true ;
             lblRoadSpeedDelta.Visible = true ;  
             break;


           case "105": // Garmin Message
             //HideHierarchy();
             break;


           case "10025": 
                FleetVehicleShow(false);
               this.txtTo.Enabled = false ;
               this.txtFrom.Enabled = false;
               this.cboHoursFrom.Enabled = false;
               this.cboHoursTo.Enabled = false;
             break;

           case "10027":
	         HideHierarchy();	
             FleetVehicleShow(false);
             this.txtTo.Enabled = false;
             this.txtFrom.Enabled = false;
             this.cboHoursFrom.Enabled = false;
             this.cboHoursTo.Enabled = false;
             break;

         }

         //if (this.cboReports.SelectedItem.Text.Contains("BSM") || (cboReports.SelectedItem.Value != "39" && cboReports.SelectedItem.Value != "17" && cboReports.SelectedItem.Value != "20" && cboReports.SelectedItem.Value != "97"))
         //{
         //    this.organizationHierarchy.Visible = false;
         //    this.vehicleSelectOption.Visible = false;
         //    this.trFleet.Visible = true;
         //}

         if (ShowOrganizationHierarchy)
         {
             MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

             if (MutipleUserHierarchyAssignment)
                 MutipleUserHierarchyAssignment = IsMultiHierarchyReport(this.cboReports.SelectedValue);
         }
         
         if (maintenanceVehicleSelectOption.Visible)
         {
             this.optMaintenanceBased.SelectedIndex = 0;
             MaintenanceReportBasedOption();
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
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  //RedirectToLogin();
                  return;
               }
         }

         if (String.IsNullOrEmpty(xmlResult))
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  //RedirectToLogin();
                  return;
               }
         }

         if (String.IsNullOrEmpty(xmlResult))
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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

         using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
         {
            
            if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
               if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  //RedirectToLogin();
                  return;
               }
         }

         if (String.IsNullOrEmpty(xmlResult))
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            return;
         }

         dsLandmarks.ReadXml(new StringReader(xmlResult));

         this.ddlLandmarks.Items.Clear();

         if (Util.IsDataSetValid(dsLandmarks))
         {
            this.ddlLandmarks.DataSource = dsLandmarks;
            this.ddlLandmarks.DataBind();
            this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_Item_0").ToString(), "-1"));
         }
         else
            this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_NoAvailable").ToString(), "-100"));


         if ((sn.Report.LandmarkName != "") && (sn.Report.LandmarkName !=" -1"))
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
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     return;
                  }
            }

            if (String.IsNullOrEmpty(xml))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               return;
            }

            
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

         string fleetid;
         string fleetname;

         if (ShowOrganizationHierarchy && optMaintenanceBased.SelectedIndex == 0 && hidOrganizationHierarchyFleetId.Value != "")
         {
             fleetid = hidOrganizationHierarchyFleetId.Value;
             fleetname = hidOrganizationHierarchyFleetName.Value;

             isMaintenance = true;
             DOHFleetId = int.Parse(fleetid);
             DOHFleetName = fleetname;
             DOHNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();             
         }
         else
         {
             fleetid = cboMaintenanceFleet.SelectedValue.ToString();
             fleetname = cboMaintenanceFleet.SelectedItem.Text;

             DOHFleetId = int.Parse(fleetid);
             DOHFleetName = fleetname;
         }

         //OHSelected = (optMaintenanceBased.SelectedValue == "0" ? true : false);
          
          //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetMaintenanceFirstParamName, this.cboMaintenanceFleet.SelectedItem.Value.ToString());
          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetMaintenanceFirstParamName, fleetid);


         sn.Report.XmlParams = xmlParams;
         //sn.Report.FleetId = Convert.ToInt32(this.cboMaintenanceFleet.SelectedItem.Value);
         sn.Report.FleetId = Convert.ToInt32(fleetid);
         //sn.Report.FleetName = this.cboMaintenanceFleet.SelectedItem.Text;
         sn.Report.FleetName = fleetname;
         
         short _guiId;
         short.TryParse(cboReports.SelectedValue, out _guiId);
         sn.Report.GuiId = _guiId;


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
              IniHierarchyPath = true;
              OrganizationHierarchyPath = getPathByNodeCode(OrganizationHierarchyNodeCode.Value);
              string strFromDate = "", strToDate = "";
              DateTime from, to;
              const string dateFormat = "MM/dd/yyyy HH:mm:ss";

              this.lblMessage.Text = "";

              # region Validation


            

              if (this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "40" || this.cboReports.SelectedValue == "63"))
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectVehicle");
                  return;
              }


              if (this.ddlLandmarks.SelectedIndex == 0 && (this.cboReports.SelectedValue == "21" || this.cboReports.SelectedValue == "23" || this.cboReports.SelectedValue == "41"))
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = (string)base.GetLocalResourceObject("valSelectLandmark");
                  return;
              }

            


              if ( this.cboReports.SelectedValue != "19" && this.cboReports.SelectedValue != "25" && this.cboReports.SelectedValue != "38" && this.cboReports.SelectedValue != "53" && this.cboReports.SelectedValue != "88")
              {
                  if (this.cboFleet.SelectedIndex == 0 && !ShowOrganizationHierarchy && optReportBased.SelectedIndex != 0)
                  {
                      this.lblMessage.Visible = true;
                      this.lblMessage.Text = (string)base.GetLocalResourceObject("valFleetMessage");
                      return;
                  }
              }
              // check driver
              if (this.cboReports.SelectedValue == "25" || this.cboReports.SelectedValue == "26" || this.cboReports.SelectedValue == "27" || this.cboReports.SelectedValue == "28"  || this.cboReports.SelectedValue == "53")
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
                  return;
              }
              else
              {
                  this.lblMessage.Visible = false;
                  this.lblMessage.Text = "";
              }

              if ((((this.cboVehicle.SelectedIndex == 0 && this.cboReports.SelectedValue == "3") ||
                  this.cboVehicle.SelectedIndex == -1) && cboVehicle.Visible == true) || (this.cboReports.SelectedValue == "3" && organizationHierarchy.Visible == true && Request.Form["OrganizationHierarchyBoxId"].ToString() == ""))
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


              TimeSpan ts = to - from;
              if (ts.Days > 31)
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = GetLocalResourceObject("MessageRptDateRange").ToString();
                  return; 
              }

              if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
              {
                  //sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
                  string f = Request.Form["OrganizationHierarchyFleetId"].ToString();

                  if (isNumericInteger(f))
                      sn.Report.FleetId = Convert.ToInt32(f);         //Request.Form["OrganizationHierarchyFleetId"]);
                  else
                      sn.Report.FleetId = 0;

              }
              else
                  sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);


              


              DataSet ds = new DataSet();
              string xmlResult="";
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
              if (ds.Tables[0].Rows[0]["InValidCall"].ToString()  == "1")
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = "Please decrease report date/time range! The from date should be greather than:" + System.DateTime.Now.AddDays(-Convert.ToInt16(ds.Tables[0].Rows[0]["MaximumDays"].ToString())).ToShortDateString() ;
                  return; 
              }

              # endregion

              this.BusyReport.Visible = true;

              string xmlParams = "",  convFromDate = "", convToDate = "";

              // 'from' and 'to' datetime incl. user pref. timezone and daylight saving
              convFromDate = from.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
              convToDate = to.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
              sn.Report.IsFleet = CheckIfSingleVehicleNotSelected();// Convert.ToInt16(this.cboVehicle.SelectedValue) < 1 ? true : false; ////Mantis# 3674

              # region Reports
              switch (this.cboReports.SelectedValue)
              {
                  # region Trip Details Report
                  case "0":
                      RememberSelection = true; //Mantis# 2861

                      if (sn.Report.IsFleet)
                      {
                          //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                          ////StringBuilder sb = new StringBuilder();
                          ////for (int i = 1; i < cboVehicle.Items.Count; i++)
                          ////{
                          ////    sb.AppendLine(cboVehicle.Items[i].Value.Trim());
                          ////}

                          StringBuilder sb = new StringBuilder();
                          string FleetIds = "";
                          try
                          {
                              string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                              VLF.DAS.Logic.Fleet dbF = new VLF.DAS.Logic.Fleet(sConnectionString);
                              DataSet dsVehicles=new DataSet();

                              if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyFleetId"].ToString().Trim()))
                                  FleetIds = Request.Form["OrganizationHierarchyFleetId"].ToString().Trim();
                              else
                                  FleetIds = cboFleet.SelectedItem.Value.ToString();

                              dsVehicles = dbF.GetVehiclesInfoByMultipleFleetIds(FleetIds);


                              foreach (DataRow rowItem in dsVehicles.Tables[0].Rows)
                                  sb.AppendLine((rowItem["LicensePlate"].ToString().TrimEnd()));
                              
                          }
                          catch
                          {
                          }

                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, sb.ToString());
                      }
                      else
                          if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()))
                          {
                              //DataSet dsSelectedSingleVehicleInfo = new DataSet();
                              //string xml = string.Empty;
                              //string SelectedSingleVehicleLP = "0";

                              //using (ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle())
                              //{
                              //    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, Convert.ToInt32(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()), ref xml), false))
                              //        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, Convert.ToInt32(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()), ref xml), true))
                              //        {
                              //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for Box:" + Request.Form["OrganizationHierarchyBoxId"].ToString().Trim() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                              //            return;
                              //        }
                              //}

                              //dsSelectedSingleVehicleInfo.ReadXml(new StringReader(xml));
                              //if (Util.IsDataSetValid(dsSelectedSingleVehicleInfo))
                              //{
                              //    //dsSelectedSingleVehicleInfo.Tables[0].
                              //    DataRow drVehicleInfo = dsSelectedSingleVehicleInfo.Tables[0].Rows[0];
                              //    SelectedSingleVehicleLP = (DBNull.Value == drVehicleInfo["LicensePlate"]) ? "0" : drVehicleInfo["LicensePlate"].ToString();
                              //}

                              //Shortest & efficient way
                              string _SelectedSingleVehicleLP = string.Empty;
                              using (VLF.DAS.Logic.Vehicle v = new VLF.DAS.Logic.Vehicle(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
                              {
                                  _SelectedSingleVehicleLP = v.GetLicensePlateByBox(Convert.ToInt32(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()));
                              }

                              //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, SelectedSingleVehicleLP.Trim());
                              xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, _SelectedSingleVehicleLP.Trim());
                          }
                          else
                          {
                              xmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripFirstParamName, this.cboVehicle.SelectedValue.Trim());
                          }


                      xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);

                      break;
                  # endregion
                  # region Trip Summary Report
                  case "1":
                      if (sn.Report.IsFleet)
                      {
                          //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, this.cboFleet.SelectedItem.Value.ToString());
                          ////StringBuilder sb = new StringBuilder();
                          ////for (int i = 1; i < cboVehicle.Items.Count; i++)
                          ////{
                          ////    sb.AppendLine(cboVehicle.Items[i].Value);
                          ////}


                          StringBuilder sb = new StringBuilder();
                          string FleetIds = "";
                          try
                          {
                              string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                              VLF.DAS.Logic.Fleet dbF = new VLF.DAS.Logic.Fleet(sConnectionString);
                              DataSet dsVehicles = new DataSet();

                              if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyFleetId"].ToString().Trim()))
                                  FleetIds = Request.Form["OrganizationHierarchyFleetId"].ToString().Trim();
                              else
                                  FleetIds = cboFleet.SelectedItem.Value.ToString();

                              dsVehicles = dbF.GetVehiclesInfoByMultipleFleetIds(FleetIds);


                              foreach (DataRow rowItem in dsVehicles.Tables[0].Rows)
                                  sb.AppendLine((rowItem["LicensePlate"].ToString().TrimEnd()));

                          }
                          catch
                          {
                          }

                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString());
                      }
                      else
                          if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()))
                          {
                              //DataSet dsSelectedSingleVehicleInfo = new DataSet();
                              //string xml = string.Empty;
                              //string SelectedSingleVehicleLP = "0";

                              //using (ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle())
                              //{
                              //    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, Convert.ToInt32(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()), ref xml), false))
                              //        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, Convert.ToInt32(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()), ref xml), true))
                              //        {
                              //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for Box:" + Request.Form["OrganizationHierarchyBoxId"].ToString().Trim() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                              //            return;
                              //        }
                              //}

                              //dsSelectedSingleVehicleInfo.ReadXml(new StringReader(xml));
                              //if (Util.IsDataSetValid(dsSelectedSingleVehicleInfo))
                              //{
                              //    //dsSelectedSingleVehicleInfo.Tables[0].
                              //    DataRow drVehicleInfo = dsSelectedSingleVehicleInfo.Tables[0].Rows[0];
                              //    SelectedSingleVehicleLP = (DBNull.Value == drVehicleInfo["LicensePlate"]) ? "0" : drVehicleInfo["LicensePlate"].ToString();
                              //}

                              //Shortest & efficient way
                              string _SelectedSingleVehicleLP = string.Empty;
                              using (VLF.DAS.Logic.Vehicle v = new VLF.DAS.Logic.Vehicle(ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString))
                              {
                                  _SelectedSingleVehicleLP = v.GetLicensePlateByBox(Convert.ToInt32(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()));
                              }

                              //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, SelectedSingleVehicleLP.Trim());
                              xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, _SelectedSingleVehicleLP.Trim());
                          }
                          else
                          {
                              xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, this.cboVehicle.SelectedValue.Trim());
                          }


                      xmlParams += CreateTripSummaryParameteres(convFromDate, convToDate);

                      break;
                  # endregion
                  # region Alarms Report
                  case "2":
                      if (sn.Report.IsFleet)
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetAlarmsFirstParamName, sn.Report.FleetId.ToString() );
                      else
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpAlarmFirstParamName, this.cboVehicle.SelectedItem.Value.Trim());

                      xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpAlarmSecondParamName : ReportTemplate.RpFleetAlarmsSecondParamName, convFromDate);
                      xmlParams += ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpAlarmThirdParamName : ReportTemplate.RpFleetAlarmsThirdParamName, convToDate);

                      break;
                  # endregion
                  # region History Report
                  case "3":
                      string vehicleLicensePlate = string.Empty;
                      if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0)
                          vehicleLicensePlate = pveh.GetVehicleLicensePlateByBoxId(int.Parse(Request.Form["OrganizationHierarchyBoxId"].ToString()));
                      else
                          vehicleLicensePlate = cboVehicle.SelectedValue;

                      xmlParams = String.Format("{0};{1};{2};{3};{4}",
                         this.chkHistIncludeCoordinate.Checked.ToString(),
                         this.chkHistIncludeSensors.Checked.ToString(),
                         this.chkHistIncludePositions.Checked.ToString(),
                         this.chkHistIncludeInvalidGPS.Checked.ToString(),
                         vehicleLicensePlate);
                         //this.cboVehicle.SelectedItem.Value.ToString());
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
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, sn.Report.FleetId.ToString());
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
                      sn.Message.FleetId = Convert.ToInt32(sn.Report.FleetId.ToString());
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
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFirstParamName, sn.Report.FleetId.ToString());

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
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetOffHourFirstParamName, sn.Report.FleetId.ToString());
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
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName,sn.Report.FleetId.ToString());
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
                          xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName,sn.Report.FleetId.ToString());
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

                      xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamIncludeSummary,"0");

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
                      xmlParams += chkShowDriver.Checked.ToString()+";";
                      xmlParams += sn.Report.LicensePlate;   
                      break;
                  # endregion
                  #region Road Speed Violation Summary
                  case "97":
                      xmlParams =Convert.ToInt16(this.chkIsPostedOnly.Checked).ToString();
                      break;
                  # endregion

                  #region Road Speed Violation
                  case "104":
                      xmlParams = Convert.ToInt16(this.chkIsPostedOnly.Checked) + ";";  
                      xmlParams += this.cboRoadSpeedDelta.SelectedItem.Value.ToString() + ";";
                      
                      break;
                  # endregion

                  #region Server Report Section

                  default:

                      int pid = this.StringToInt32(this.cboReports.SelectedValue.ToString());

                      if (pid >= 10000)
                      {
                          sn.Report.XmlParams = GenerateReportParameters_JSON(pid);
                      }
                      break;

                  #endregion


              }
                #endregion Reports

              sn.Report.GuiId = Convert.ToInt16(this.cboReports.SelectedValue);
              if (this.StringToInt32(this.cboReports.SelectedValue.ToString()) < 10000)
                  sn.Report.XmlParams = xmlParams;


              //sn.Report.FromDate = strFromDate;
              sn.Report.FromDate = from.ToString();
              //sn.Report.ToDate = strToDate;
              sn.Report.ToDate = to.ToString();
              sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedValue);
              if (this.organizationHierarchy.Visible == true && ShowOrganizationHierarchy)
              {
                  string fleets = Request.Form["OrganizationHierarchyFleetId"].ToString();
                  if (fleets == "" || fleets == string.Empty)
                  {
                      this.lblMessage.Visible = true;
                      this.lblMessage.Text = (string)base.GetLocalResourceObject("valMultipleHierarchy");
                      return;
                  }
              }
              if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
              {
                  //sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
                  string fleets = Request.Form["OrganizationHierarchyFleetId"].ToString();

                  if (isNumericInteger(fleets))
                  {
                      //sn.Report.IsFleet = true;
                      sn.Report.FleetId = StringToInt(fleets);
                      sn.Report.FleetName = "";
                  }
                  else if (isMultiFleet(fleets))
                  {
                      //sn.Report.IsFleet = false;
                      sn.Report.FleetId = 0;
                      sn.Report.FleetName = "Multi Fleets";
                  }
                  else
                  {
                      //sn.Report.IsFleet = false;
                      sn.Report.FleetId = 0;
                      sn.Report.FleetName = "";
                  }
              }
              else
                  sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);

            

              sn.Report.DriverId =this.ddlDrivers.SelectedValue!="" ? Convert.ToInt32(this.ddlDrivers.SelectedValue):0;
              sn.Report.LandmarkName = this.ddlLandmarks.SelectedValue.ToString();
              sn.Report.LicensePlate = this.cboVehicle.SelectedValue;
              sn.Report.FleetName = this.cboFleet.SelectedItem.Text;
              sn.Report.ReportType = cboReports.SelectedValue;
              sn.Report.OrganizationHierarchyNodeCode=Request.Form["OrganizationHierarchyNodeCode"];
              
           }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
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

      bool CheckIfSingleVehicleNotSelected()
      {
          bool ret = false;
          switch (optReportBased.SelectedIndex)
          {
              case 0:
                  ret = string.IsNullOrEmpty(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim());
                  break;
              default:
                  ret = (cboVehicle.SelectedIndex < 1);
                  break;
          }
          return ret; //optReportBased.SelectedIndex == 0 && string.IsNullOrEmpty(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()) ? true : false;
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
            if (reportGuiId == "17" || reportGuiId == "27")
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
                     tmpSpeed = (sn.User.UnitOfMes == 1 ? "120" : "75");
                     break;
                  case "5":
                     tmpSpeed = (sn.User.UnitOfMes == 1 ? "125" : "77");
                     break;
                  case "6":
                     tmpSpeed = (sn.User.UnitOfMes == 1 ? "130" : "80");
                     break;
                  case "7":
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
          
          if (optReportBased.SelectedItem.Value == "0")
          {
              trFleet.Visible = false ;
              organizationHierarchy.Visible = true;
              
              //sn.Report.OrganizationHierarchySelected = true;
              //OHSelected = true;
          }
          else
          {
              trFleet.Visible = true;
              organizationHierarchy.Visible = false;

              //sn.Report.OrganizationHierarchySelected = false;
              //OHSelected = false;
          }
      }

      private void HideHierarchy()
      {
          trFleet.Visible = true;
          organizationHierarchy.Visible = false;
          this.optReportBased.SelectedIndex = 1;
          optBaseTable.Visible = false;     
      }

      private void DeleteReportItem(ListItem NewItem)
      {
        
          for (int index = 0; index < cboReports.Items.Count; index++)
          {
              if (cboReports.Items[index].Value==NewItem.Value)
              {
                  cboReports.Items.Remove(NewItem);
                  break;
              }
          }
         
      }

      private void removeReportSN(string strGui)
      {
              int y=sn.Report.UserReportsDataSet.Tables[0].Rows.Count;

                         for  (int i=0; i<y;i++)
                         {
                             DataRow dr=sn.Report.UserReportsDataSet.Tables[0].Rows[i];

                             if (dr["GuiId"].ToString() == strGui)
                             {
                                 sn.Report.UserReportsDataSet.Tables[0].Rows.Remove(dr);
                                 break;
                             }
                         }
                     
                        
      }
      protected void optMaintenanceBased_SelectedIndexChanged(object sender, EventArgs e)
      {
          MaintenanceReportBasedOption();
      }

      private void MaintenanceReportBasedOption()
      {

          if (optMaintenanceBased.SelectedItem.Value == "0")
          {
              trMaintenanceFleet.Visible = false;
              trMaintenanceHierarchy.Visible = true;
              //sn.Report.OrganizationHierarchySelected = true;
          }
          else
          {
              trMaintenanceFleet.Visible = true;
              trMaintenanceHierarchy.Visible = false;
              //sn.Report.OrganizationHierarchySelected = false;
          }
      }

      public DateTime ConvertStringToDateTime(string DateValue, string Dateformat) {

          return ConvertStringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
      }
      public DateTime ConvertStringToDateTime(string value, string format, string cultureinfo) {
          
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

        #region Reporting Service

        /// <summary>
        /// for async call parameter
        /// </summary>
        /// <param name="iResult"></param>
        private void wsCallback(IAsyncResult iResult) { }

        /// <summary>
        /// Constructing parameter string
        /// </summary>
        /// <param name="ReportID"></param>
        /// <returns></returns>
        public string GenerateReportParameters_JSON(int ReportID)      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {
            ReportID = Math.Abs(ReportID);

            StringBuilder sbp = new StringBuilder();

            #region General Parameters Section

            // Report's parameters
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

            // Date range - Extended no time
            string df = txtFrom.SelectedDate.Value.ToString("yyyy/MM/dd") + " 12:00:00 AM";     //+ this.cboHoursFrom.SelectedDate.Value.ToString("hh:mm:ss tt");
            string dt = txtTo.SelectedDate.Value.ToString("yyyy/MM/dd") + " 12:00:00 AM";       //+ cboHoursTo.SelectedDate.Value.ToString("hh:mm:ss tt");

            sbp.Append("datefrom: " + df + ", ");
            //sbp.Append("datefrom: " + Functions.FormattedDateTimeString(txtFrom.SelectedDate.ToString(), "yyyy/MM/dd hh:mm:ss") + ", ");
            sbp.Append("dateto: " + dt + ", ");
            //sbp.Append("dateto: " + Functions.FormattedDateTimeString(txtTo.SelectedDate.ToString(), "yyyy/MM/dd hh:mm:ss") + ", ");

            sbp.Append("unitofvolume: " + sn.User.VolumeUnits + ", ");
            sbp.Append("unitofspeed: " + sn.User.UnitOfMes + ", ");

            // Hierarchy - Fleet
            if (this.organizationHierarchy.Visible)                                     
            {
                if (optReportBased.SelectedIndex == 0)      // select hierarchy
                {
                    // Multi/Single Fleet
                    if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyFleetId"].ToString().Trim()))
                        sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
                    // Vehicle Box ID
                    if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyBoxId"].ToString().Trim()))
                        sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + ", ");
                    // Vehicle Description (Name)
                    //if (!string.IsNullOrEmpty(Request.Form["OrganizationHierarchyVehicleDescription"].ToString().Trim()))
                    //    sbp.Append("vehiclename: " + Request.Form["OrganizationHierarchyVehicleDescription"] + ", ");   //standardReport.cboVehicle_Name = Request.Form["OrganizationHierarchyVehicleDescription"];
                }
                else
	            {
                    // Single Fleet
                    if (cboFleet.Visible && cboFleet.Enabled)
                        sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
                    // Vehicle License Plate
                    if (this.cboVehicle.Visible && this.cboVehicle.Enabled){
                        sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
                        sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
                    }
	            }
            }
            else   // Single Fleet
            {
                if (cboFleet.Visible && cboFleet.Enabled)
                {
                    sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
                    sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);
                }
                // Vehicle License Plate
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

            // Fleet
            //if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
            //    sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
            //else if (cboFleet.Visible && cboFleet.Enabled)
            //    sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
            //else
            //    sbp.Append("");

            #endregion

            // Vehicle ID
            //if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyBoxId"] != "")
            //    sbp.Append("boxid: " + Request.Form["OrganizationHierarchyBoxId"].ToString() + ", ");       // Box ID
            //else if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
            //    sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");                       // License Plate
            //else
            //    sbp.Append("");

            // Vehicle Name
            //if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyVehicleDescription"] != "")
            //    sbp.Append("vehiclename: " + Request.Form["OrganizationHierarchyVehicleDescription"] + ", ");   //standardReport.cboVehicle_Name = Request.Form["OrganizationHierarchyVehicleDescription"];
            //else if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
            //    sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
            //else
            //    sbp.Append("");

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
                if (chkIsPostedOnly.Visible)                        // && chkIsPostedOnly.Enabled)
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

            //Show Store Position
            if (chkShowStorePosition.Visible)
                sbp.Append("incaddress: " + ((chkShowStorePosition.Checked)? "1" : "0") + ", ");
            // Stop & Idling in seconds: 5:300 | 10:600 | ......
            if (cboStopSequence.Visible)
                sbp.Append("minduration: " + cboStopSequence.SelectedItem.Value.ToString() + ", ");
            // Stop & Idling: 0:Stop | 1:Idling | 2:Both
            if (optStopFilter.Visible)
                sbp.Append("remark: " + optStopFilter.SelectedItem.Value.ToString() + ", ");

            if (sbp.Length > 0)
                return "{" + sbp.ToString() + "}";
            else
                return "";
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

        public bool isMultiFleet(string Fleets)
        {
            if (isNumericInteger(Fleets))
                return true;
            else
                return IsValidFleetIDString(Fleets);
        }

        private bool IsValidFleetIDString(string Fleets)
        {
            if (!Fleets.Contains(","))
            {
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
        /// <param name="Report"></param>
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
                case "10062":                // Stop / Idling Duration Report 
                case "10063":                // Alarm Report
                case "0":
                case "1":
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
}
}
