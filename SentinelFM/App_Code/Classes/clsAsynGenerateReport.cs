//This file is for report function and created by Devin
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Diagnostics;
using VLF.CLS;
using VLF.Reports;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Telerik.Web.UI;
using System.IO;
using System.Xml;
using System.Data.SqlClient;
namespace SentinelFM
{

    public class SideMenu
    {
        string _MenuID = string.Empty;
        string _MenuText = string.Empty;
        string _MenuImage = string.Empty;
        string _MenuUrl = string.Empty;
        Boolean _IsSelect = false;
        List<SideMenu> _SubSideMenu = new List<SideMenu>();

        public string MenuID
        {
            get { return _MenuID; }
            set { _MenuID = value; }
        }
        public string MenuText
        {
            get { return _MenuText; }
            set { _MenuText = value; }

        }
        public string MenuImage
        {
            get { return _MenuImage; }
            set { _MenuImage = value; }

        }
        public string MenuUrl
        {
            get { return _MenuUrl; }
            set { _MenuUrl = value; }

        }
        public Boolean IsSelect
        {
            get { return _IsSelect; }
            set { _IsSelect = value; }

        }
        public List<SideMenu> SubSideMenu
        {
            get { return _SubSideMenu; }
            set { _SubSideMenu = value; }

        }
    }
    /// <summary>
    /// This class is for reportmessage
    /// </summary>
    public class clsReportMessage
    {
        public static string Report_Type = "1";
        public static string Message_Type = "2";
        public clsReportMessage() { }


        private string _MessageID;
        public string MessageID
        {
            get { return _MessageID; }
            set { _MessageID = value; }
        }

        private string _MessageName;
        public string MessageName
        {
            get { return _MessageName; }
            set { _MessageName = value; }
        }

        private string _MessageType;
        public string MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        }

        private DateTime _MessageDateTime;
        public DateTime MessageDateTime
        {
            get { return _MessageDateTime; }
            set { _MessageDateTime = value; }
        }

        private string _MessagePath;
        public string MessagePath
        {
            get { return _MessagePath; }
            set { _MessagePath = value; }
        }

    }

    /// <summary>
    /// this class is for rad grid filter
    /// </summary>
    public class clsGridFilterMenu
    {
        public static string PopupCalendarMsg_en = "Open the calendar popup.";
        public static string PopupCalendarMsg_fr = "Ouvrir le calendrier.";

        private List<String> MenuOption = new List<string>();
        //{ "NoFilter","Contains", "DoesNotContain","StartsWith","EndsWith","EqualTo","NotEqualTo","GreaterThan","LessThan","GreaterThanOrEqualTo","LessThanOrEqualTo"
        // };
        public clsGridFilterMenu()
        {
            MenuOption.Add("NoFilter");
            MenuOption.Add("Contains");
            MenuOption.Add("DoesNotContain");
            MenuOption.Add("StartsWith");
            MenuOption.Add("EndsWith");
            MenuOption.Add("EqualTo");
            MenuOption.Add("NotEqualTo");
            MenuOption.Add("GreaterThan");
            MenuOption.Add("LessThan");
            MenuOption.Add("GreaterThanOrEqualTo");
            MenuOption.Add("LessThanOrEqualTo");
            MenuOption.Add("IsEmpty");
            MenuOption.Add("NotIsEmpty");
            MenuOption.Add("IsNull");
            MenuOption.Add("NotIsNull");
        }

        public void PreRender_Grid(RadGrid rg, List<RadDatePicker> datePickers)
        {
            GridFilteringItem item = rg.MasterTableView.GetItems(GridItemType.FilteringItem)[0] as GridFilteringItem;
            foreach (GridColumn gc in rg.Columns)
            {
                AddCultureToCalendar(item[gc.UniqueName], datePickers);
            }
        }
        private void AddCultureToCalendar(Control subControl, List<RadDatePicker> datePickers)
        {
            if (subControl.Controls == null) return;
            foreach (Control ctl in subControl.Controls)
            {
                if (ctl is RadDatePicker)
                {

                    ((RadDatePicker)ctl).
                        Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    ((RadDatePicker)ctl).DateInput.Culture
                         = System.Globalization.CultureInfo.CurrentCulture;
                    ((RadDatePicker)ctl).Calendar.CultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                        ((RadDatePicker)ctl).DatePopupButton.ToolTip = PopupCalendarMsg_fr;

                    datePickers.Add((RadDatePicker)ctl);
                }
                else
                {
                    if (ctl.HasControls()) AddCultureToCalendar(ctl, datePickers);
                }
            }
        }


        public void CreateGridFilterMenu(RadGrid rg)
        {
            rg.Culture = System.Globalization.CultureInfo.CurrentUICulture;
            GridFilterMenu menu = rg.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (MenuOption.Contains(menu.Items[i].Value))
                {
                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                    {
                        switch (menu.Items[i].Text)
                        {
                            case "NoFilter":
                                menu.Items[i].Text = "Aucun filtre";
                                break;
                            case "Contains":
                                //menu.Items[i].Text = "Aucun filtre";
                                break;
                            case "DoesNotContain":
                                //menu.Items[i].Text = "Aucun filtre";
                                break;
                            case "StartsWith":
                                //menu.Items[i].Text = "Aucun filtre";
                                break;
                            case "EndsWith":
                                //menu.Items[i].Text = "Aucun filtre";
                                break;
                            case "EqualTo":
                                menu.Items[i].Text = "Égale à";
                                break;
                            case "NotEqualTo":
                                menu.Items[i].Text = "N’égale pas";
                                break;
                            case "GreaterThan":
                                menu.Items[i].Text = "Plus grand que";
                                break;
                            case "LessThan":
                                menu.Items[i].Text = "Plus petit que ";
                                break;
                            case "GreaterThanOrEqualTo":
                                menu.Items[i].Text = "Plus grand ou égale à";
                                break;
                            case "LessThanOrEqualTo":
                                menu.Items[i].Text = "Plus petit ou égale à";
                                break;
                            case "IsEmpty":
                                //menu.Items[i].Text = "Égale à";
                                break;
                            case "NotIsEmpty":
                                //menu.Items[i].Text = "Égale à";
                                break;
                            case "IsNull":
                                menu.Items[i].Text = "Nul";
                                break;
                            case "NotIsNull":
                                menu.Items[i].Text = "N’est pas nul";
                                break;
                        }
                    }
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }

        }
        public void NoWrapFilterMenu(GridItemEventArgs e)
        {
            if (e.Item is GridFilteringItem)
            {
                foreach (TableCell cell in e.Item.Cells)
                {
                    if (cell.Controls.Count > 0)
                    {
                        cell.Controls.AddAt(0, new LiteralControl("<nobr>"));
                        cell.Controls.Add(new LiteralControl("</nobr>"));
                    }
                }
            }
        }

    }
    [Serializable]
    public class clsCustomProperty
    {
        public clsCustomProperty() { }

        private string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Param;
        public string Param
        {
            get { return _Param; }
            set { _Param = value; }
        }

        private string _From;
        public string From
        {
            get { return _From; }
            set { _From = value; }
        }

        private string _To;
        public string To
        {
            get { return _To; }
            set { _To = value; }
        }

        private string _Format;
        public string Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        private string _FleetId;
        public string FleetId
        {
            get { return _FleetId; }
            set { _FleetId = value; }
        }

        private string _VehicleId;
        public string VehicleId
        {
            get { return _VehicleId; }
            set { _VehicleId = value; }
        }

        private string _FleetName;
        public string FleetName
        {
            get { return _FleetName; }
            set { _FleetName = value; }
        }


        private string _ReportType;
        public string ReportType
        {
            get { return _ReportType; }
            set { _ReportType = value; }
        }


        private string _IsFleet;
        public string IsFleet
        {
            get { return _IsFleet; }
            set { _IsFleet = value; }
        }

        private string _DriverId;
        public string DriverId
        {
            get { return _DriverId; }
            set { _DriverId = value; }
        }

        private string _Landmark;
        public string Landmark
        {
            get { return _Landmark; }
            set { _Landmark = value; }
        }

        private string _License;
        public string License
        {
            get { return _License; }
            set { _License = value; }
        }

        private string _Category;
        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }



    }

    # region clsStandardReport
    /// <summary>
    /// The class is to create parameters for standard report
    /// </summary>
    public class clsStandardReport
    {

        public clsStandardReport()
        {
        }
        //form elements
        public string cboReports = string.Empty;  //cboReports.SelectedValue
        public bool chkDoor = false;
        public string cboDoorPeriod = string.Empty;
        public bool chkDriverDoorExc = false;
        public bool chkPassengerDoorExc = false;
        public bool chkSideHopperDoorExc = false;
        public bool chkRearHopperDoorExc = false;
        public bool chkSOSMode = false;
        public string cboSOSLimit = string.Empty;
        public bool chkTAR = false;
        public bool chkImmobilization = false;
        public bool chkDriverDoor = false;
        public bool chkLeash = false;
        public bool chkHistIncludeCoordinate = false; //Checked
        public bool chkHistIncludeSensors = false;   //Checked
        public bool chkHistIncludeInvalidGPS = false;//Checked
        public bool chkHistIncludePositions = false;//Checked
        public bool chkIncludeStreetAddress = false;
        public bool chkIncludeSensors = false;
        public bool chkIncludePosition = false;
        public bool chkShowStorePosition = false;
        public string cboStopSequence = string.Empty;
        public string optStopFilter = string.Empty;
        public bool optStopFilter_0 = false; //optStopFilter.Items[0].Selected
        public bool optStopFilter_1 = false;//optStopFilter.Items[1].Selected
        public bool optStopFilter_2 = false;//optStopFilter.Items[2].Selected
        public bool chkSpeedViolation = false;
        public string cboViolationSpeed = string.Empty;
        public string cboViolationSpeed_Extended = string.Empty;
        public bool chkHarshAcceleration = false;
        public bool chkExtremeAcceleration = false;
        public bool chkHarshBraking = false;
        public bool chkExtremeBraking = false;
        public bool chkSeatBeltViolation = false;
        public bool chkReverseSpeed = false;
        public bool chkReverseDistance = false;
        public bool chkHighRail = false;
        public bool chkHighRailReverseSpeed = false;
        public bool chkSpeedViolation_Extended=false;                                                             
        public bool chkAcceleration_Extended=false; 
        public bool chkBraking_Extended=false; 
        public bool chkSeatBelt_Extended=false; 
        public bool chkHyRailReverseSpeed_Extended=false; 
        public bool chkHyRailSpeed_Extended=false;
        public bool chkReverseSpeed_Extended = false;
        public bool chkPostedSpeed_Extended = false;
        public string ddlLandmarks = string.Empty;
        public string ddlGeozones = string.Empty;
        public string ddlDrivers = string.Empty;
        public string cboMaintenanceFleet = string.Empty;
        public string cboMaintenanceFleet_Name = string.Empty;
        public string cboFleetReportFormat = string.Empty;
        public bool chkIncludeIdleTime = false;
        public bool chkIncludeSummary = false;
        public DateTime? cboFromDayH = null;
        public DateTime? cboToDayH = null;
        public bool chkWeekend = false;
        public DateTime? cboWeekEndFromH = null;
        public DateTime? cboWeekEndToH = null;
        public bool chkExcBattery = false;
        public bool chkExcTamper = false;
        public bool chkExcPanic = false;
        public bool chkExcKeypad = false;
        public bool chkExcGPS = false;
        public bool chkExcAVL = false;
        public bool chkExcLeash = false;
        public bool chkIncCurTARMode = false;
        public string txtSpeed120 = string.Empty;
        public string txtAccHarsh = string.Empty;
        public string txtBrakingExtreme = string.Empty;
        public string txtSpeed130 = string.Empty;
        public string txtAccExtreme = string.Empty;
        public string txtSeatBelt = string.Empty;
        public string txtSpeed140 = string.Empty;
        public string txtBrakingHarsh = string.Empty;
        public string txtReverseSpeed = string.Empty;
        public string txtReverseDistance = string.Empty;
        public string txtHighRail = string.Empty;
        public string optEndTrip = string.Empty;
        public string txtHighRailReverseSpeed = string.Empty;
        public string txtOver4 = string.Empty;
        public string txtOver10 = string.Empty;
        public string txtOver15 = string.Empty; 
        public DateTime? txtFrom = null;
        public DateTime? cboHoursFrom = null;
        public DateTime? txtTo = null;
        public DateTime? cboHoursTo = null;
        public string cboFleet = string.Empty; //cboFleet.SelectedItem
        public string cboVehicle = string.Empty; //cboVehicle.SelectedValue
        public string cboFormat = string.Empty;
        public string cboFleet_Name = string.Empty;
        public string cboVehicle_Name = string.Empty;
        public string keyValue = string.Empty;
        public bool chkLocker1 = false;
        public bool chkLocker2 = false;
        public bool chkLocker3 = false;
        public bool chkLocker4 = false;
        public bool chkLocker5 = false;
        public bool chkLocker6 = false;
        public bool chkLocker7 = false;
        public bool chkLocker8 = false;
        public bool chkLocker9 = false;
        public bool chkShowDriver = false;
        public bool chkIsPostedOnly = false;

        //HOS Audit Report 2014.01.27
        public bool chkWorkShiftViolation = false;
        public bool chkDailyViolation= false;
        public bool chkOffDutyViolation= false;
        public bool chkCycleViolation= false;
        public bool chkPreTripNotDone= false;
        public bool chkPostTripNotDone= false;
        public bool chkDrivingWithDefect = false;
        public bool chkDriverWithoutSigned = false;
        public bool chkLogsNotReceive = false;

        public string cboRoadSpeedDelta = string.Empty;
        public string OrganizationHierarchyNodeCode;
        public static string category = "0";

        //Elements key 
        //Important: Do not change the k number, you can only add new one
        public string cboReports_N = "K1";
        public string chkDoor_N = "K2";
        public string cboDoorPeriod_N = "K3";
        public string chkDriverDoorExc_N = "K4";
        public string chkPassengerDoorExc_N = "K5";
        public string chkSideHopperDoorExc_N = "K6";
        public string chkRearHopperDoorExc_N = "K7";
        public string chkSOSMode_N = "K8";
        public string cboSOSLimit_N = "K9";
        public string chkTAR_N = "K10";
        public string chkImmobilization_N = "K11";
        public string chkDriverDoor_N = "K12";
        public string chkLeash_N = "K13";
        public string chkHistIncludeCoordinate_N = "K14";
        public string chkHistIncludeSensors_N = "K15";
        public string chkHistIncludeInvalidGPS_N = "K16";
        public string chkHistIncludePositions_N = "K17";
        public string chkIncludeStreetAddress_N = "K18";
        public string chkIncludeSensors_N = "K19";
        public string chkIncludePosition_N = "K20";
        public string chkShowStorePosition_N = "K21";
        public string cboStopSequence_N = "K22";
        public string optStopFilter_N = "K23";
        public string optStopFilter_0_N = "K24";
        public string optStopFilter_1_N = "K25";
        public string optStopFilter_2_N = "K26";
        public string chkSpeedViolation_N = "K27";
        public string cboViolationSpeed_N = "K28";
        public string chkHarshAcceleration_N = "K29";
        public string chkExtremeAcceleration_N = "K30";
        public string chkHarshBraking_N = "K31";
        public string chkExtremeBraking_N = "K32";
        public string chkSeatBeltViolation_N = "K33";
        public string ddlLandmarks_N = "K34";
        public string ddlGeozones_N = "K35";
        public string ddlDrivers_N = "K36";
        public string cboMaintenanceFleet_N = "K37";
        public string cboMaintenanceFleet_Name_N = "K38";
        public string cboFleetReportFormat_N = "K39";
        public string chkIncludeIdleTime_N = "K40";
        public string chkIncludeSummary_N = "K41";
        public string cboFromDayH_N = "K42";
        public string cboToDayH_N = "K43";
        public string chkWeekend_N = "K44";
        public string cboWeekEndFromH_N = "K45";
        public string cboWeekEndToH_N = "K46";
        public string chkExcBattery_N = "K47";
        public string chkExcTamper_N = "K48";
        public string chkExcPanic_N = "K49";
        public string chkExcKeypad_N = "K50";
        public string chkExcGPS_N = "K51";
        public string chkExcAVL_N = "K52";
        public string chkExcLeash_N = "K53";
        public string chkIncCurTARMode_N = "K54";
        public string txtSpeed120_N = "K55";
        public string txtAccHarsh_N = "K56";
        public string txtBrakingExtreme_N = "K57";
        public string txtSpeed130_N = "K58";
        public string txtAccExtreme_N = "K59";
        public string txtSeatBelt_N = "K60";
        public string txtSpeed140_N = "K61";
        public string txtBrakingHarsh_N = "K62";
        public string optEndTrip_N = "K63";
        public string txtFrom_N = "K64";
        public string cboHoursFrom_N = "K65";
        public string txtTo_N = "K66";
        public string cboHoursTo_N = "K67";
        public string cboFleet_N = "K68";
        public string cboVehicle_N = "K69";
        public string cboFormat_N = "K70";
        public string cboFleet_Name_N = "K71";
        public string cboVehicle_Name_N = "K72";
        public string keyValue_N = "K73";
        public string chkLocker1_N = "K74";
        public string chkLocker2_N = "K75";
        public string chkLocker3_N = "K76";
        public string chkLocker4_N = "K77";
        public string chkLocker5_N = "K78";
        public string chkLocker6_N = "K79";
        public string chkLocker7_N = "K80";
        public string chkLocker8_N = "K81";
        public string chkLocker9_N = "K82";
        public string chkShowDriver_N = "K83";
        public string chkIsPostedOnly_N = "K84";
        public string cboRoadSpeedDelta_N = "K85";

        public string OrganizationHierarchyNodeCode_N = "K86";

        public string chkReverseSpeed_N = "K87";
        public string chkReverseDistance_N = "K88";
        public string chkHighRail_N = "K89";

          public string txtReverseSpeed_N="K90";
          public string txtReverseDistance_N = "K91";
          public string txtHighRail_N = "K92";
          public string txtHighRailReverseSpeed_N = "K93";
          public string chkSpeedViolation_Extended_N = "K94";
          public string chkAcceleration_Extended_N = "K95";
          public string chkBraking_Extended_N = "K96";
          public string chkSeatBelt_Extended_N = "K97";
          public string chkHyRailReverseSpeed_Extended_N = "K98";
          public string chkHyRailSpeed_Extended_N = "K99";
         public string cboViolationSpeed_Extended_N = "K100";
         public string chkReverseSpeed_Extended_N = "K101";

         //HOS Audit Report 2014.01.27
         public string chkWorkShiftViolation_N = "K102";
         public string chkDailyViolation_N = "K103";
         public string chkOffDutyViolation_N = "K104";
         public string chkCycleViolation_N = "K105";
         public string chkPreTripNotDone_N = "K106";
         public string chkPostTripNotDone_N = "K107";
         public string chkDrivingWithDefect_N = "K108";
        public static string category_N = "Category";

        public string organizationHierarchyBoxId = "";
        public string organizationHierarchyFleetId = "";
        public string chkPostedSpeed_Extended_N = "K109";
        public string chkDriverWithoutSigned_N = "K110";
        public string chkLogsNotReceive_N = "K111";
        //----
        public SentinelFMSession sn = null;
        public clsUtility objUtil = null;
        public SentinelFMBasePage basepage = null;
        /// <summary>
        /// Create custom property
        /// </summary>
        /// <returns></returns>
        public string CreateCustomProperty()
        {
            StringBuilder spCustomProperty = new StringBuilder();

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboReports_N, cboReports));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkDoor_N, chkDoor.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboDoorPeriod_N, cboDoorPeriod));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkDriverDoorExc_N, chkDriverDoorExc.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkPassengerDoorExc_N, chkPassengerDoorExc.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSideHopperDoorExc_N, chkSideHopperDoorExc.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkRearHopperDoorExc_N, chkRearHopperDoorExc.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSOSMode_N, chkSOSMode.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboSOSLimit_N, cboSOSLimit.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkTAR_N, chkTAR.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkImmobilization_N, chkImmobilization.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkDriverDoor_N, chkDriverDoor.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLeash_N, chkLeash.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHistIncludeCoordinate_N, chkHistIncludeCoordinate.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHistIncludeSensors_N, chkHistIncludeSensors.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHistIncludeInvalidGPS_N, chkHistIncludeInvalidGPS.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHistIncludePositions_N, chkHistIncludePositions.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIncludeStreetAddress_N, chkIncludeStreetAddress.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIncludeSensors_N, chkIncludeSensors.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIncludePosition_N, chkIncludePosition.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkShowStorePosition_N, chkShowStorePosition.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboStopSequence_N, cboStopSequence));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(optStopFilter_N, optStopFilter));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(optStopFilter_0_N, optStopFilter_0.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(optStopFilter_1_N, optStopFilter_1.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(optStopFilter_2_N, optStopFilter_2.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSpeedViolation_N, chkSpeedViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboViolationSpeed_N, cboViolationSpeed));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHarshAcceleration_N, chkHarshAcceleration.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExtremeAcceleration_N, chkExtremeAcceleration.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHarshBraking_N, chkHarshBraking.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExtremeBraking_N, chkExtremeBraking.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSeatBeltViolation_N, chkSeatBeltViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(ddlLandmarks_N, ddlLandmarks));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(ddlGeozones_N, ddlGeozones));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(ddlDrivers_N, ddlDrivers));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboMaintenanceFleet_N, cboMaintenanceFleet));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboMaintenanceFleet_Name_N, cboMaintenanceFleet_Name));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFleetReportFormat_N, cboFleetReportFormat));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIncludeIdleTime_N, chkIncludeIdleTime.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIncludeSummary_N, chkIncludeSummary.ToString()));
            if (cboFromDayH.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFromDayH_N, string.Format("{0:MM/dd/yyyy} {0:t}", cboFromDayH)));
            if (cboToDayH.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboToDayH_N, string.Format("{0:MM/dd/yyyy} {0:t}", cboToDayH)));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkWeekend_N, chkWeekend.ToString()));

            if (cboWeekEndFromH.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboWeekEndFromH_N, string.Format("{0:MM/dd/yyyy} {0:t}", cboWeekEndFromH)));
            if (cboWeekEndToH.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboWeekEndToH_N, string.Format("{0:MM/dd/yyyy} {0:t}", cboWeekEndToH)));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcBattery_N, chkExcBattery.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcTamper_N, chkExcTamper.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcPanic_N, chkExcPanic.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcKeypad_N, chkExcKeypad.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcGPS_N, chkExcGPS.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcAVL_N, chkExcAVL.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExcLeash_N, chkExcLeash.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIncCurTARMode_N, chkIncCurTARMode.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSpeed120_N, txtSpeed120));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtAccHarsh_N, txtAccHarsh));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtBrakingExtreme_N, txtBrakingExtreme));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSpeed130_N, txtSpeed130));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtAccExtreme_N, txtAccExtreme));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSeatBelt_N, txtSeatBelt));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSpeed140_N, txtSpeed140));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtBrakingHarsh_N, txtBrakingHarsh));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(optEndTrip_N, optEndTrip));
            if (txtFrom.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtFrom_N, string.Format("{0:MM/dd/yyyy} {0:t}", txtFrom)));
            if (cboHoursFrom.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboHoursFrom_N, string.Format("{0:MM/dd/yyyy} {0:t}", cboHoursFrom)));
            if (txtTo.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtTo_N, string.Format("{0:MM/dd/yyyy} {0:t}", txtTo)));
            if (cboHoursTo.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboHoursTo_N, string.Format("{0:MM/dd/yyyy} {0:t}", cboHoursTo)));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFleet_N, cboFleet));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFleet_Name_N, cboFleet_Name));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboVehicle_N, cboVehicle));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboVehicle_Name_N, cboVehicle_Name));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFormat_N, cboFormat));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(category_N, category));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(keyValue_N, keyValue));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker1_N, chkLocker1.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker2_N, chkLocker2.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker3_N, chkLocker3.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker4_N, chkLocker4.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker5_N, chkLocker5.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker6_N, chkLocker6.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker7_N, chkLocker7.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker8_N, chkLocker8.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLocker9_N, chkLocker9.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkShowDriver_N, chkShowDriver.ToString()));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkIsPostedOnly_N, chkIsPostedOnly.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboRoadSpeedDelta_N, cboRoadSpeedDelta));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(OrganizationHierarchyNodeCode_N, OrganizationHierarchyNodeCode));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkReverseSpeed_N, chkReverseSpeed.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkReverseDistance_N, chkReverseDistance.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHighRail_N, chkHighRail.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtReverseDistance_N, txtReverseDistance));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtReverseSpeed_N, txtReverseSpeed));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtHighRail_N, txtHighRail));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtHighRailReverseSpeed_N, txtHighRailReverseSpeed.ToString()));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSpeedViolation_Extended_N, chkSpeedViolation_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkAcceleration_Extended_N, chkAcceleration_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkBraking_Extended_N, chkBraking_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSeatBelt_Extended_N, chkSeatBelt_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHyRailReverseSpeed_Extended_N, chkHyRailReverseSpeed_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHyRailSpeed_Extended_N, chkHyRailSpeed_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboViolationSpeed_Extended_N, cboViolationSpeed_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkReverseSpeed_Extended_N, chkReverseSpeed_Extended.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkPostedSpeed_Extended_N, chkPostedSpeed_Extended.ToString()));

            //HOS Audit Report 2014.01.27
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkWorkShiftViolation_N, chkWorkShiftViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkDailyViolation_N, chkDailyViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkOffDutyViolation_N, chkOffDutyViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkCycleViolation_N, chkCycleViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkPreTripNotDone_N, chkPreTripNotDone.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkPostTripNotDone_N, chkPostTripNotDone.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkDrivingWithDefect_N, chkDrivingWithDefect.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkDriverWithoutSigned_N, chkDriverWithoutSigned.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkLogsNotReceive_N , chkLogsNotReceive.ToString()));

            return spCustomProperty.ToString();
        }

        /// <summary>
        /// Get custom property
        /// </summary>
        /// <returns></returns>
        public void GetCustomProperty(string properties)
        {
            if (objUtil == null) objUtil = new clsUtility(sn);
            cboReports = clsAsynGenerateReport.PairFindValue(cboReports_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkDoor_N, properties), out chkDoor);
            cboDoorPeriod = clsAsynGenerateReport.PairFindValue(cboDoorPeriod_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkDriverDoorExc_N, properties), out chkDriverDoorExc);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkPassengerDoorExc_N, properties), out chkPassengerDoorExc);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSideHopperDoorExc_N, properties), out chkSideHopperDoorExc);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkRearHopperDoorExc_N, properties), out chkRearHopperDoorExc);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSOSMode_N, properties), out chkSOSMode);
            cboSOSLimit = clsAsynGenerateReport.PairFindValue(cboSOSLimit_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkTAR_N, properties), out chkTAR);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkImmobilization_N, properties), out  chkImmobilization);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkDriverDoor_N, properties), out  chkDriverDoor);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLeash_N, properties), out chkLeash);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHistIncludeCoordinate_N, properties), out chkHistIncludeCoordinate);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHistIncludeSensors_N, properties), out chkHistIncludeSensors);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHistIncludeInvalidGPS_N, properties), out  chkHistIncludeInvalidGPS);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHistIncludePositions_N, properties), out chkHistIncludePositions);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIncludeStreetAddress_N, properties), out chkIncludeStreetAddress);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIncludeSensors_N, properties), out chkIncludeSensors);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIncludePosition_N, properties), out chkIncludePosition);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkShowStorePosition_N, properties), out chkShowStorePosition);
            cboStopSequence = clsAsynGenerateReport.PairFindValue(cboStopSequence_N, properties);
            optStopFilter = clsAsynGenerateReport.PairFindValue(optStopFilter_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(optStopFilter_0_N, properties), out optStopFilter_0);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(optStopFilter_1_N, properties), out optStopFilter_1);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(optStopFilter_2_N, properties), out optStopFilter_2);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSpeedViolation_N, properties), out chkSpeedViolation);
            cboViolationSpeed = clsAsynGenerateReport.PairFindValue(cboViolationSpeed_N, properties);
            cboViolationSpeed_Extended = clsAsynGenerateReport.PairFindValue(cboViolationSpeed_Extended, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHarshAcceleration_N, properties), out chkHarshAcceleration);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExtremeAcceleration_N, properties), out chkExtremeAcceleration);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHarshBraking_N, properties), out chkHarshBraking);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExtremeBraking_N, properties), out  chkExtremeBraking);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSeatBeltViolation_N, properties), out chkSeatBeltViolation);
            ddlLandmarks = clsAsynGenerateReport.PairFindValue(ddlLandmarks_N, properties);
            ddlGeozones = clsAsynGenerateReport.PairFindValue(ddlGeozones_N, properties);
            ddlDrivers = clsAsynGenerateReport.PairFindValue(ddlDrivers_N, properties);
            cboMaintenanceFleet = clsAsynGenerateReport.PairFindValue(cboMaintenanceFleet_N, properties);
            cboMaintenanceFleet_Name = clsAsynGenerateReport.PairFindValue(cboMaintenanceFleet_Name_N, properties);
            cboFleetReportFormat = clsAsynGenerateReport.PairFindValue(cboFleetReportFormat_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIncludeIdleTime_N, properties), out chkIncludeIdleTime);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIncludeSummary_N, properties), out chkIncludeSummary);
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

            string str_date = clsAsynGenerateReport.PairFindValue(cboFromDayH_N, properties);
            if (!string.IsNullOrEmpty(str_date)) cboFromDayH = Convert.ToDateTime(str_date, ci);
            else cboFromDayH = null;

            str_date = clsAsynGenerateReport.PairFindValue(cboToDayH_N, properties);
            if (!string.IsNullOrEmpty(str_date)) cboToDayH = Convert.ToDateTime(str_date, ci);
            else cboToDayH = null;

            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkWeekend_N, properties), out chkWeekend);

            str_date = clsAsynGenerateReport.PairFindValue(cboWeekEndFromH_N, properties);
            if (!string.IsNullOrEmpty(str_date)) cboWeekEndFromH = Convert.ToDateTime(str_date, ci);
            else cboWeekEndFromH = null;

            str_date = clsAsynGenerateReport.PairFindValue(cboWeekEndToH_N, properties);
            if (!string.IsNullOrEmpty(str_date)) cboWeekEndToH = Convert.ToDateTime(str_date, ci);
            else cboWeekEndToH = null;


            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcBattery_N, properties), out  chkExcBattery);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcTamper_N, properties), out chkExcTamper);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcPanic_N, properties), out chkExcPanic);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcKeypad_N, properties), out chkExcKeypad);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcGPS_N, properties), out chkExcGPS);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcAVL_N, properties), out chkExcAVL);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExcLeash_N, properties), out chkExcLeash);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIncCurTARMode_N, properties), out chkIncCurTARMode);
            txtSpeed120 = clsAsynGenerateReport.PairFindValue(txtSpeed120_N, properties);
            txtAccHarsh = clsAsynGenerateReport.PairFindValue(txtAccHarsh_N, properties);
            txtBrakingExtreme = clsAsynGenerateReport.PairFindValue(txtBrakingExtreme_N, properties);
            txtSpeed130 = clsAsynGenerateReport.PairFindValue(txtSpeed130_N, properties);
            txtAccExtreme = clsAsynGenerateReport.PairFindValue(txtAccExtreme_N, properties);
            txtSeatBelt = clsAsynGenerateReport.PairFindValue(txtSeatBelt_N, properties);
            txtSpeed140 = clsAsynGenerateReport.PairFindValue(txtSpeed140_N, properties);
            txtBrakingHarsh = clsAsynGenerateReport.PairFindValue(txtBrakingHarsh_N, properties);
            optEndTrip = clsAsynGenerateReport.PairFindValue(optEndTrip_N, properties);

            str_date = clsAsynGenerateReport.PairFindValue(txtFrom_N, properties);
            if (!string.IsNullOrEmpty(str_date)) txtFrom = Convert.ToDateTime(str_date, ci);
            else txtFrom = null;

            str_date = clsAsynGenerateReport.PairFindValue(cboHoursFrom_N, properties);
            if (!string.IsNullOrEmpty(str_date)) cboHoursFrom = Convert.ToDateTime(str_date, ci);
            else cboHoursFrom = null;

            str_date = clsAsynGenerateReport.PairFindValue(txtTo_N, properties);
            if (!string.IsNullOrEmpty(str_date)) txtTo = Convert.ToDateTime(str_date, ci);
            else txtTo = null;

            str_date = clsAsynGenerateReport.PairFindValue(cboHoursTo_N, properties);
            if (!string.IsNullOrEmpty(str_date)) cboHoursTo = Convert.ToDateTime(str_date, ci);
            else cboHoursTo = null;

            cboFleet = clsAsynGenerateReport.PairFindValue(cboFleet_N, properties);
            cboVehicle = clsAsynGenerateReport.PairFindValue(cboVehicle_N, properties);
            cboFormat = clsAsynGenerateReport.PairFindValue(cboFormat_N, properties);
            cboFleet_Name = clsAsynGenerateReport.PairFindValue(cboFleet_Name_N, properties);
            cboVehicle_Name = clsAsynGenerateReport.PairFindValue(cboVehicle_Name_N, properties);
            keyValue = clsAsynGenerateReport.PairFindValue(keyValue_N, properties);
            txtReverseSpeed = clsAsynGenerateReport.PairFindValue(txtReverseSpeed_N, properties);
            txtReverseDistance = clsAsynGenerateReport.PairFindValue(txtReverseDistance_N, properties);
            txtHighRail = clsAsynGenerateReport.PairFindValue(txtHighRail_N, properties);
            txtHighRailReverseSpeed = clsAsynGenerateReport.PairFindValue(txtHighRailReverseSpeed_N, properties);

            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker1_N, properties), out chkLocker1);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker2_N, properties), out chkLocker2);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker3_N, properties), out chkLocker3);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker4_N, properties), out chkLocker4);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker5_N, properties), out chkLocker5);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker6_N, properties), out chkLocker6);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker7_N, properties), out chkLocker7);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker8_N, properties), out chkLocker8);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLocker9_N, properties), out chkLocker9);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkShowDriver_N, properties), out chkShowDriver);

            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkIsPostedOnly_N, properties), out chkIsPostedOnly);

            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkReverseSpeed_N, properties), out chkReverseSpeed);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkReverseDistance_N, properties), out chkReverseDistance);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHighRail_N , properties), out chkHighRail );

            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSpeedViolation_Extended_N, properties), out chkSpeedViolation_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkAcceleration_Extended_N, properties), out  chkAcceleration_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkBraking_Extended_N, properties), out chkBraking_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSeatBelt_Extended_N, properties), out chkSeatBelt_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHyRailReverseSpeed_Extended_N, properties), out chkHyRailReverseSpeed_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHyRailSpeed_Extended_N, properties), out chkHyRailSpeed_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkReverseSpeed_Extended_N, properties), out chkReverseSpeed_Extended);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkPostedSpeed_Extended_N, properties), out chkPostedSpeed_Extended);

            

            //HOS Audit Report 2014.01.27
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkWorkShiftViolation_N, properties), out chkWorkShiftViolation);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkDailyViolation_N, properties), out chkDailyViolation);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkOffDutyViolation_N, properties), out chkOffDutyViolation);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkCycleViolation_N, properties), out chkCycleViolation);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkPreTripNotDone_N, properties), out chkPreTripNotDone);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkPostTripNotDone_N, properties), out chkPostTripNotDone);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkDrivingWithDefect_N, properties), out chkDrivingWithDefect);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkDriverWithoutSigned_N, properties), out chkDriverWithoutSigned);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkLogsNotReceive_N, properties), out chkLogsNotReceive);

            cboRoadSpeedDelta = clsAsynGenerateReport.PairFindValue(cboRoadSpeedDelta_N, properties);
            OrganizationHierarchyNodeCode = clsAsynGenerateReport.PairFindValue(OrganizationHierarchyNodeCode_N, properties);
        }


        /// <summary>
        /// Create report params for MaintenanceReport
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        public void CreateReportParams_MaintenanceReport()
        {
            string xmlParams = "";
            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetMaintenanceFirstParamName, this.cboMaintenanceFleet);

            sn.Report.XmlParams = xmlParams;
            sn.Report.FleetId = Convert.ToInt32(this.cboMaintenanceFleet);
            sn.Report.FleetName = cboMaintenanceFleet_Name;
            sn.Report.ReportFormat = Convert.ToInt32(cboFleetReportFormat.ToString());

            sn.Report.ReportType = cboReports;
            sn.Report.IsFleet = true;
        }

        /// <summary>
        /// Create report params 
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        public Boolean CreateReportParams(RadComboBox cboVehicle_Box)
        {
            try
            {
                string strFromDate = "", strToDate = "";
                DateTime from, to;
                const string dateFormat = "MM/dd/yyyy HH:mm:ss";


                strFromDate = txtFrom.Value.ToString("MM/dd/yyyy") + " " + String.Format("{0:t}", cboHoursFrom.Value);

                strToDate = this.txtTo.Value.ToString("MM/dd/yyyy") + " " + String.Format("{0:t}", this.cboHoursTo.Value);

                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

                from = Convert.ToDateTime(strFromDate, ci);
                to = Convert.ToDateTime(strToDate, ci);

                //this.BusyReport.Visible = true;

                string xmlParams = "", convFromDate = "", convToDate = "";

                // 'from' and 'to' datetime incl. user pref. timezone and daylight saving
                convFromDate = from.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
                convToDate = to.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
                sn.Report.IsFleet = this.cboVehicle == "0" ? true : false;
                DataSet dsVehicles=new DataSet() ;

                if (!this.cboFleet.Contains(",")) 
                     dsVehicles = dsVehicle_Fill(Convert.ToInt32(this.cboFleet));

                # region Reports
                switch (this.cboReports)
                {
                    # region Trip Details Report
                    case "0":
                        if (sn.Report.IsFleet)
                        {

                           
                            StringBuilder sb = new StringBuilder();
                            ////for (int i = 1; i < cboVehicle_Box.Items.Count; i++)
                            ////{
                            ////    sb.AppendLine(cboVehicle_Box.Items[i].Value.Trim());
                            ////}

                             foreach (DataRow dr in dsVehicles.Tables[0].Rows)
                            {
                                //sb.AppendLine(dr["LicensePlate"].ToString());
                                sb.AppendLine(dr["LicensePlate"].ToString()+",");
                            }
   
                            //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, sb.ToString());

                            if (sb.ToString().Contains(","))
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, sb.ToString().Substring(0, sb.Length - 1));
                            else
                                xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetDetailedTripFirstParamName, sb.ToString());

	
                        }
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripFirstParamName, this.cboVehicle.Trim());


                        xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);

                        break;
                    # endregion
                    # region Trip Summary Report
                    case "1":
                        if (sn.Report.IsFleet)
                        {
                            

                            StringBuilder sb = new StringBuilder();
                            ////for (int i = 1; i < cboVehicle_Box.Items.Count; i++)
                            ////{
                            ////    sb.AppendLine(cboVehicle_Box.Items[i].Value);
                            ////}

                             foreach (DataRow dr in dsVehicles.Tables[0].Rows)
                            {
                                //sb.AppendLine(dr["LicensePlate"].ToString());
                                sb.AppendLine(dr["LicensePlate"].ToString() + ",");
                            }

                            //xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString());

                             if (sb.ToString().Contains(","))
                                 xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString().Substring(0, sb.Length - 1));
                             else
                                 xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetTripFirstParamName, sb.ToString());
                        }
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpTripFirstParamName, this.cboVehicle.Trim());


                        xmlParams += CreateTripSummaryParameteres(convFromDate, convToDate);

                        break;
                    # endregion
                    # region Alarms Report
                    case "2":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetAlarmsFirstParamName, this.cboFleet.Trim());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpAlarmFirstParamName, this.cboVehicle.Trim());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpAlarmSecondParamName : ReportTemplate.RpFleetAlarmsSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpAlarmThirdParamName : ReportTemplate.RpFleetAlarmsThirdParamName, convToDate);

                        break;
                    # endregion
                    # region History Report
                    case "3":
                        xmlParams = String.Format("{0};{1};{2};{3};{4}",
                           this.chkHistIncludeCoordinate.ToString(),
                           this.chkHistIncludeSensors.ToString(),
                           this.chkHistIncludePositions.ToString(),
                           this.chkHistIncludeInvalidGPS.ToString(),
                           this.cboVehicle.ToString());
                        break;
                    # endregion
                    # region Stop Report
                    case "4":
                        string blnShowsStops = "false";
                        string blnShowsIdles = "false";

                        blnShowsStops = this.optStopFilter_0.ToString();
                        blnShowsIdles = this.optStopFilter_1.ToString();
                        if (optStopFilter_2)
                        {
                            blnShowsStops = "true";
                            blnShowsIdles = "true";
                        }


                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.ToString());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, this.cboStopSequence.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopSixthParamName : ReportTemplate.RpFleetStopSixthParamName, blnShowsStops);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopSeventhParamName : ReportTemplate.RpFleetStopSeventhParamName, blnShowsIdles);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopEighthParamName : ReportTemplate.RpFleetStopEighthParamName, this.optEndTrip.ToString());

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
                            if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, this.cboVehicle, ref xml), false))
                                if (objUtil.ErrCheck(dbv.GetVehicleInfoXML(sn.UserID, sn.SecId, this.cboVehicle, ref xml), true))
                                {
                                    return false;
                                }

                            if (xml != "")
                            {
                                dsVehicle.ReadXml(new StringReader(xml));
                                sn.Message.BoxId = Convert.ToInt32(dsVehicle.Tables[0].Rows[0]["BoxId"]);
                            }
                            else
                            {
                                return false;
                            }

                        }

                        sn.Message.FromDate = convFromDate;
                        sn.Message.ToDate = convToDate;
                        sn.Message.FleetId = Convert.ToInt32(this.cboFleet);
                        sn.Message.FleetName = this.cboFleet_Name; ;
                        sn.Message.VehicleName = this.cboVehicle_Name.ToString();

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
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFirstParamName, this.cboFleet.ToString());

                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFourthParamName, chkSOSMode ? this.cboSOSLimit.ToString() : "-1");
                            if (this.chkDriverDoorExc || this.chkPassengerDoorExc || this.chkRearHopperDoorExc || this.chkSideHopperDoorExc
                                || this.chkLocker1 || this.chkLocker2 || this.chkLocker3 || this.chkLocker4 || this.chkLocker5 || this.chkLocker6
                                || this.chkLocker7 || this.chkLocker8 || this.chkLocker9)
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFifthParamName, this.cboDoorPeriod.ToString());
                            else
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpFleetExceptionFifthParamName, "-1");
                        }
                        else
                        {
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpExceptionFirstParamName, this.cboVehicle.ToString());

                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFourthParamName, chkSOSMode ? this.cboSOSLimit.ToString() : "-1");
                            if (this.chkDriverDoorExc || this.chkPassengerDoorExc || this.chkRearHopperDoorExc || this.chkSideHopperDoorExc
                             || this.chkLocker1 || this.chkLocker2 || this.chkLocker3 || this.chkLocker4 || this.chkLocker5 || this.chkLocker6
                             || this.chkLocker7 || this.chkLocker8 || this.chkLocker9)
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFifthParamName, this.cboDoorPeriod.ToString());
                            else
                                xmlParams += ReportTemplate.MakePair(ReportTemplate.RpExceptionFifthParamName, "-1");

                        }

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionSecondParamName : ReportTemplate.RpFleetExceptionSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionThirdParamName : ReportTemplate.RpFleetExceptionThirdParamName, convToDate);

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionSixthParamName : ReportTemplate.RpFleetExceptionSixthParamName, chkTAR.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionSeventhParamName : ReportTemplate.RpFleetExceptionSeventhParamName, chkImmobilization.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionEightParamName : ReportTemplate.RpFleetExceptionEightParamName, chkDriverDoor.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionNineParamName : ReportTemplate.RpFleetExceptionNineParamName, chkLeash.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionTenParamName : ReportTemplate.RpFleetExceptionTenParamName, chkExcBattery.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionElevenParamName : ReportTemplate.RpFleetExceptionElevenParamName, chkExcTamper.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionTwelveParamName : ReportTemplate.RpFleetExceptionTwelveParamName, chkExcPanic.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionThirteenParamName : ReportTemplate.RpFleetExceptionThirteenParamName, chkExcKeypad.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionFourteenParamName : ReportTemplate.RpFleetExceptionFourteenParamName, chkExcGPS.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionFifteenParamName : ReportTemplate.RpFleetExceptionFifteenParamName, chkExcAVL.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionSixteenParamName : ReportTemplate.RpFleetExceptionSixteenParamName, chkExcLeash.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionSeventeenParamName : ReportTemplate.RpFleetExceptionSeventeenParamName, chkDriverDoorExc.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionEighteenParamName : ReportTemplate.RpFleetExceptionEighteenParamName, chkPassengerDoorExc.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionNineteenParamName : ReportTemplate.RpFleetExceptionNineteenParamName, chkSideHopperDoorExc.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionTwentyParamName : ReportTemplate.RpFleetExceptionTwentyParamName, chkRearHopperDoorExc.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpExceptionTwentyFirstParamName : ReportTemplate.RpFleetExceptionTwentyFirstParamName, this.chkIncCurTARMode.ToString());

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
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetOffHourFirstParamName, this.cboFleet.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpOffHourFirstParamName, this.cboVehicle.ToString());

                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourFourthParamName, chkShowStorePosition.ToString());
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourFifthParamName, String.Format("{0:HH}", this.cboFromDayH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSixthParamName, String.Format("{0:mm}", this.cboFromDayH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourSeventhParamName, String.Format("{0:HH}", this.cboToDayH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourEightParamName, String.Format("{0:mm}", this.cboToDayH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourNineParamName, String.Format("{0:HH}", this.cboWeekEndFromH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourTenParamName, String.Format("{0:mm}", this.cboWeekEndFromH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourElevenParamName, String.Format("{0:HH}", this.cboWeekEndToH));
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RpOffHourTwelveParamName, String.Format("{0:mm}", this.cboWeekEndToH));

                        break;
                    # endregion
                    # region Landmark Activity Report
                    case "9":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet.ToString());
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle.ToString());

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, cboStopSequence.ToString());

                        break;
                    # endregion
                    # region Fleet Maintenace Report
                    case "10":
                        if (sn.Report.IsFleet)
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpFleetStopFirstParamName, this.cboFleet);
                        else
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, this.cboVehicle);

                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopSecondParamName : ReportTemplate.RpFleetStopSecondParamName, convFromDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopThirdParamName : ReportTemplate.RpFleetStopThirdParamName, convToDate);
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopFourthParamName : ReportTemplate.RpFleetStopFourthParamName, chkShowStorePosition.ToString());
                        xmlParams += ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpStopFifthParamName : ReportTemplate.RpFleetStopFifthParamName, cboStopSequence.ToString());

                        break;
                    # endregion
                    # region Violation Report
                    case "17":
                    case "20":
                    case "127":
                    case "132":
                    case "10013":
                    case "10014":
                        xmlParams = CreateViolationParameters(this.cboReports);
                        break;
                    # endregion
                    # region Landmark Summary Report
                    case "21":
                    case "131":
                        if (sn.Report.IsFleet) // fleet
                            //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks);
                        else
                        {
                            // vehicle
                            //xmlParams = String.Format("{0}={1};{2}={3}",
                            //   ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text,
                            //   ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue);

                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks);
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle);
                        }
                        break;
                    # endregion
                    # region Geozone  Reports
                    case "22":
                    case "30":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamGeozone, this.ddlGeozones);

                        if (!sn.Report.IsFleet) // fleet
                        {
                            //xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.Trim());
                        }

                        xmlParams += "|" + sn.Report.XmlParams;  
                        break;
                    # endregion
                    # region Landmark Details Report
                    case "23":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamLandmark, this.ddlLandmarks.Text);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks);
                        if (!sn.Report.IsFleet) // fleet
                        {
                            //  xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.Trim());
                        }

                        break;
                    # endregion
                    # region Inactivity Report
                    case "24":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);
                        if (!sn.Report.IsFleet) // fleet
                        {
                            //xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.Trim());
                        }
                        break;
                    # endregion
                    # region Driver Trip Details Report
                    case "25":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        //xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers);
                        xmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);
                        break;
                    # endregion
                    # region Driver Trip Summary Report
                    case "26":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers);
                        xmlParams += CreateTripSummaryParameteres(convFromDate, convToDate);
                        break;
                    # endregion
                    # region Driver Violation Report
                    case "27":
                    case "28":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamDriverId, this.ddlDrivers.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers);
                        xmlParams += CreateViolationParameters(this.cboReports);
                        break;
                    # endregion
                    #region State Milage Report
                    case "34":
                        //xmlParams = String.Format("{0}={1};", ReportTemplate.RptParamIncludeSummary, "0");

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamIncludeSummary, "0");

                        if (!sn.Report.IsFleet) // fleet
                        {
                            // xmlParams += String.Format(";{0}={1}", ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue.Trim());
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle.Trim());
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
                    case "10011":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);
                        break;
                    # endregion
                    #region Activity Summary Report per Vehicle
                    case "39":
                    case "50":
                    case "62":
                    case "87":
                        //xmlParams = String.Format("{0}={1}", ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);
                        break;
                    # endregion


                    #region HOS Details Report
                    case "53":
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers);
                        //redirectUrl = "./HOS_Report.aspx";
                        break;
                    # endregion


                    #region New Trips Summary Report per Vehicle
                    case "40":
                    case "63":
                        // xmlParams = String.Format("{0}={1};{2}={3}",
                        //       ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue,
                        //       ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);


                        break;
                    # endregion
                    #region Time At Landmark
                    case "41":


                        if (sn.Report.IsFleet) // fleet
                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks);
                        else
                        {

                            xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamLandmark, this.ddlLandmarks);
                            xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle);
                        }
                        break;
                    # endregion
                    //#region HOS Details Report
                    //case "53":
                    //    xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers);
                    //    break;
                    //# endregion

                    #region Idling Summary Report New
                    case "88":
                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.ToString());
                        xmlParams += ReportTemplate.MakePair("OrgId", sn.User.OrganizationId.ToString());
                        break;
                    # endregion

                    #region Trip SUmmary New
                    case "89":
                        xmlParams = "3;";
                        xmlParams += chkShowDriver.ToString() + ";";
                        //xmlParams = chkShowDriver.ToString();
                        xmlParams += sn.Report.LicensePlate;   

                        break;
                    # endregion


                    //HOS Audit report 2014.01.27
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
                          Driven without Signed in = 10
                         */

                        if (chkDriverWithoutSigned)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",10";
                            else
                                xmlParams = "10";
                        }
                        if (chkLogsNotReceive)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",11";
                            else
                                xmlParams = "11";
                        }

                        if (!chkWorkShiftViolation)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",6";
                            else
                                xmlParams = "6";
                        }

                        if (!chkDailyViolation)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",7";
                            else
                                xmlParams = "7";
                        }

                        if (!chkOffDutyViolation)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",8";
                            else
                                xmlParams = "8";
                        }

                        if (!chkCycleViolation)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",9";
                            else
                                xmlParams = "9";
                        }

                        if (!chkPreTripNotDone)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",3";
                            else
                                xmlParams = "3";
                        }
                        if (!chkPostTripNotDone)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",5";
                            else
                                xmlParams = "5";
                        }
                        if (!chkDrivingWithDefect)
                        {
                            if (xmlParams != "")
                                xmlParams = xmlParams + ",4";
                            else
                                xmlParams = "4";
                        }

                        xmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripEighthParamName, xmlParams);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamLicensePlate, this.cboVehicle);
                        xmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamDriverId, this.ddlDrivers);
                        break;
                    # endregion                        
                    #region Road Speed Violation
                    case "104":
                        xmlParams = Convert.ToInt16(this.chkIsPostedOnly) + ";";
                        xmlParams += this.cboRoadSpeedDelta + ";";

                        break;
                    # endregion

                    #region Infraction Summary Report
                    case "130":
                    case "133":
                        xmlParams = CreateViolationParameters(this.cboReports);
                        break;
                    # endregion

                }
                #endregion Reports

                sn.Report.GuiId = Convert.ToInt16(this.cboReports);
                sn.Report.XmlParams = xmlParams;
                //sn.Report.FromDate = strFromDate;
                sn.Report.FromDate = from.ToString();
                //sn.Report.ToDate = strToDate;
                sn.Report.ToDate = to.ToString();
                sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat);
                if (!this.cboFleet.Contains(","))
                {
                    sn.Report.FleetId = Convert.ToInt32(this.cboFleet);
                }
                sn.Report.DriverId = this.ddlDrivers != "" ? Convert.ToInt32(this.ddlDrivers) : 0;
                sn.Report.LandmarkName = this.ddlLandmarks.ToString();
                sn.Report.LicensePlate = this.cboVehicle;
                sn.Report.FleetName = this.cboFleet_Name;
                sn.Report.ReportType = cboReports;
                sn.Report.OrganizationHierarchyNodeCode = OrganizationHierarchyNodeCode;
                
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                if (basepage != null) basepage.RedirectToLogin();
            }
            catch (Exception Ex)
            {
                string page_Name = string.Empty;
                if (basepage != null)
                { page_Name = basepage.GetType().Name; }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + page_Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                if (basepage != null) basepage.ExceptionLogger(trace);

            }
            return true;

        }



        private DataSet dsVehicle_Fill(int fleetId)
        {
            DataSet dsVehicle = new DataSet();
            try
            {
                
                SentinelFMSession sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
                string xml = "";
                clsUtility objUtil = null;
                if (objUtil == null) objUtil = new clsUtility(sn);
                using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
                {
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                        {

                        }
                }

                dsVehicle.ReadXml(new StringReader(xml));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {

            }
            return dsVehicle;
        }


        private RadComboBox CboVehicle_Fill(int fleetId)
        {
            RadComboBox cboVehicleCombo;
            cboVehicleCombo = new RadComboBox();
            cboVehicleCombo.DataTextField = "Description";
            cboVehicleCombo.DataValueField = "LicensePlate";
            string page_Name = string.Empty;
            if (basepage != null)
            { page_Name = basepage.GetType().Name; }

            try
            {
                DataSet dsVehicle = new DataSet();

                string xml = "";

                using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
                {
                    if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + page_Name));
                            cboVehicleCombo.Items.Insert(0, new RadComboBoxItem("Entire Fleet", "0"));
                            return cboVehicleCombo;
                        }
                }

                if (String.IsNullOrEmpty(xml))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + page_Name));
                    return cboVehicleCombo;
                }

                dsVehicle.ReadXml(new StringReader(xml));

                cboVehicleCombo.DataSource = dsVehicle;
                cboVehicleCombo.DataBind();
                cboVehicleCombo.Items.Insert(0, new RadComboBoxItem("Entire Fleet", "0"));

                return cboVehicleCombo;

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                if (basepage != null) basepage.RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + page_Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                if (basepage != null) basepage.ExceptionLogger(trace);

            }
            return cboVehicleCombo;
        }
        /// <summary>
        /// Build Violation Parameters
        /// </summary>
        /// <returns></returns>
        private string CreateViolationParameters(string reportGuiId)
        {
            int intCriteria = 0;

            if (this.chkSpeedViolation)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
            if (this.chkHarshAcceleration)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
            if (this.chkHarshBraking)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
            if (this.chkExtremeAcceleration)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
            if (this.chkExtremeBraking)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
            if (this.chkSeatBeltViolation)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;
            if (this.chkReverseSpeed) 
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_REVERSESPEED;
            if (this.chkReverseDistance )
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_REVERSEDISTANCE;
         
	     if (sn.User.OrganizationId == 999994)
            {
                if (this.chkReverseDistance)
                    intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_REVERSEDISTANCE;
                if (this.chkReverseSpeed)
                    intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_REVERSESPEED;
                if (this.chkHighRail)
                    intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HIGHRAIL;
            }


         if (reportGuiId == "127" || reportGuiId == "130" )
            {
                 intCriteria = 0;
                
                if (this.chkSpeedViolation_Extended)                                                             
                    intCriteria |= 0x0001;

                if (this.chkAcceleration_Extended)                                                          
                    intCriteria |= 0x0002;

              
                if (this.chkBraking_Extended)                                                          
                    intCriteria |= 0x0004;

              
                if (this.chkSeatBelt_Extended)                                                      
                    intCriteria |= 0x0008;

                
                if (this.chkHyRailReverseSpeed_Extended)                                                       
                    intCriteria |= 0x0010;

                if (this.chkHyRailSpeed_Extended)                                                       
                    intCriteria |= 0x0020;

                if (this.chkReverseSpeed_Extended)                                                        
                    intCriteria |= 0x0040;

                if (this.chkPostedSpeed_Extended )                                                        
                    intCriteria |= 0x0080;
            }


            StringBuilder Params = new StringBuilder();

            Params.Append(intCriteria);
            string vSpeed = this.cboViolationSpeed;
            if (reportGuiId == "127" || reportGuiId == "132")
                vSpeed = this.cboViolationSpeed_Extended ;

            // violation summary
            if (reportGuiId == "20" || reportGuiId == "28" || reportGuiId == "130" || reportGuiId == "133")
            {
                if (sn.User.OrganizationId == 999994 || sn.User.OrganizationId ==1000098)
                    Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}",
                       this.txtSpeed120, this.txtSpeed130, this.txtSpeed140, this.txtOver4, this.txtOver10, this.txtOver15,
                       this.txtBrakingHarsh, this.txtBrakingExtreme, this.txtAccHarsh, this.txtAccExtreme,
                       this.txtSeatBelt, this.txtReverseSpeed, this.txtReverseDistance, this.txtHighRail);
                else if (reportGuiId == "130" || reportGuiId == "133")
                {
                    string[] tmp = sn.Report.TmpData.Split(',');
                    Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}",
                    tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], tmp[6], tmp[7], tmp[8], tmp[9], tmp[10], tmp[11]);
                }                       
                else
                  Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7}",
                   this.txtSpeed120, this.txtSpeed130, this.txtSpeed140,
                   this.txtAccExtreme, this.txtAccHarsh, this.txtBrakingExtreme,
                   this.txtBrakingHarsh, this.txtSeatBelt);

                //Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7}",
                //   this.txtSpeed120, this.txtSpeed130, this.txtSpeed140,
                //   this.txtAccExtreme, this.txtAccHarsh, this.txtBrakingExtreme,
                //   this.txtBrakingHarsh, this.txtSeatBelt);
            }
            // violation details
            else if (reportGuiId == "17" || reportGuiId == "27" || reportGuiId == "127" || reportGuiId == "132")
                {
                    Params.Append("*");
                    string tmpSpeed = "";
                    switch (vSpeed)
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

            if (reportGuiId == "127" || reportGuiId == "130" || reportGuiId == "132" || reportGuiId == "133")
            {
                Params.Append("|BoxId=");

                if (organizationHierarchyBoxId != "")
                    Params.Append(organizationHierarchyBoxId);
                else if (this.cboVehicle != "" && this.cboVehicle!="0")
                {
                    try
                    {
                        DataSet dsVehicle = null;
                        string ConnnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                        using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(ConnnectionString))
                        {
                            dsVehicle = dbVehicle.GetVehicleInfoByLicensePlate(this.cboVehicle);
                        }


                        if (Util.IsDataSetValid(dsVehicle))
                            Params.Append(dsVehicle.Tables[0].Rows[0]["BoxId"].ToString());

                    }
                    catch
                    {
                        
                    }
                    
                }


                Params.Append(";OrganizationHierarchyFleetId=");
                Params.Append(organizationHierarchyFleetId);
                Params.Append(";");
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
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpTripSecondParamName : ReportTemplate.RpFleetTripSecondParamName, convFromDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpTripThirdParamName : ReportTemplate.RpFleetTripThirdParamName, convToDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpTripFourthParamName : ReportTemplate.RpFleetTripFourthParamName, chkShowStorePosition.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpTripFifthParamName : ReportTemplate.RpFleetTripFifthParamName, this.optEndTrip.ToString()));
            return sb.ToString();
        }

        /// <summary>
        /// Trip Details Parameteres
        /// </summary>
        /// <param name="xmlParams"></param>
        private string CreateTripDetailsParameteres(string fromDate, string toDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripSecondParamName : ReportTemplate.RpFleetDetailedTripSecondParamName, fromDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripThirdParamName : ReportTemplate.RpFleetDetailedTripThirdParamName, toDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripFourthParamName : ReportTemplate.RpFleetDetailedTripFourthParamName, this.chkIncludeStreetAddress.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripFifthParamName : ReportTemplate.RpFleetDetailedTripFifthParamName, this.chkIncludeSensors.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripSixthParamName : ReportTemplate.RpFleetDetailedTripSixthParamName, this.chkIncludePosition.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripSeventhParamName : ReportTemplate.RpFleetDetailedTripSeventhParamName, this.chkIncludeIdleTime.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripEighthParamName : ReportTemplate.RpFleetDetailedTripEighthParamName, this.chkIncludeSummary.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripNinthParamName : ReportTemplate.RpFleetDetailedTripNinthParamName, this.chkShowStorePosition.ToString()));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripTenthParamName : ReportTemplate.RpFleetDetailedTripTenthParamName, this.optEndTrip.ToString()));
            return sb.ToString();
        }
    }
    # endregion

    # region clsExtendedReport
    /// <summary>
    /// The class is to create parameters for extended report
    /// </summary>
    public class clsExtendedReport
    {
        public string cboReports = string.Empty;
        public string cboViolationSpeed = string.Empty;
        public string txtCost = string.Empty;
        public string txtColorFilter = string.Empty;
        public string optEndTrip = string.Empty; //optEndTrip.SelectedValue
        public bool chkSpeedViolation = false;
        public string DropDownList1 = string.Empty;
        public bool chkHarshAcceleration = false;
        public bool chkHarshBraking = false;
        public bool chkExtremeAcceleration = false;
        public bool chkExtremeBraking = false;
        public bool chkSeatBeltViolation = false;
        public DateTime? txtFrom = null;
        public DateTime? txtTo = null;
        public string cboFleet = string.Empty;
        public string cboFleet_Name = string.Empty;
        public string cboVehicle = string.Empty;
        public string cboFormat = string.Empty;
        public string selectedFleets = string.Empty;
        public string keyValue = string.Empty;

        public string txtSpeed120 = string.Empty;
        public string txtAccHarsh = string.Empty;
        public string txtBrakingExtreme = string.Empty;
        public string txtSpeed130 = string.Empty;
        public string txtAccExtreme = string.Empty;
        public string txtSeatBelt = string.Empty;
        public string txtSpeed140 = string.Empty;
        public string txtBrakingHarsh = string.Empty;
        public string ddlLandmarks = string.Empty;
        public string ddlGeozones = string.Empty;
        public bool chkActiveVehicles = false;

        public string cboIdlingThreshold = string.Empty;
        public string cboSpeedThreshold = string.Empty;
        public string cboMediaType = string.Empty;
        public string ddlDrivers = string.Empty;

        public string OrganizationHierarchyNodeCode = string.Empty;
        public static string category = "1";

        //----
        public SentinelFMSession sn = null;
        public clsUtility objUtil = null;
        public SentinelFMBasePage basepage = null;

        public string cboReports_N = "E1";
        public string cboViolationSpeed_N = "E2";
        public string txtCost_N = "E3";
        public string txtColorFilter_N = "E4";
        public string optEndTrip_N = "E5";
        public string chkSpeedViolation_N = "E6";
        public string DropDownList1_N = "E7";
        public string chkHarshAcceleration_N = "E8";
        public string chkHarshBraking_N = "E9";
        public string chkExtremeAcceleration_N = "E10";
        public string chkExtremeBraking_N = "E11";
        public string chkSeatBeltViolation_N = "E12";
        public string txtFrom_N = "E13";
        public string txtTo_N = "E14";
        public string cboFleet_N = "E15";
        public string cboFleet_Name_N = "E16";
        public string cboVehicle_N = "E17";
        public string cboFormat_N = "E18";
        public string selectedFleets_N = "E19";
        public string keyValue_N = "E20";

        public string txtSpeed120_N = "E21";
        public string txtAccHarsh_N = "E22";
        public string txtBrakingExtreme_N = "E23";
        public string txtSpeed130_N = "E24";
        public string txtAccExtreme_N = "E25";
        public string txtSeatBelt_N = "E26";
        public string txtSpeed140_N = "E27";
        public string txtBrakingHarsh_N = "E28";
        public string ddlLandmarks_N = "E29";
        public string ddlGeozones_N = "E30";
        public string chkActiveVehicles_N = "E31";
        public string OrganizationHierarchyNodeCode_N = "E32" ;
        public string cboIdlingThreshold_N = "E33";
        public string cboSpeedThreshold_N = "E34";
        public string cboMediaType_N = "E35";
        public string ddlDrivers_N = "E36";
        
        public static string category_N = "Category";

        public clsExtendedReport() { }
        /// <summary>
        /// Create custom property
        /// </summary>
        /// <returns></returns>
        public string CreateCustomProperty()
        {
            StringBuilder spCustomProperty = new StringBuilder();
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboReports_N, cboReports));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboViolationSpeed_N, cboViolationSpeed));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtCost_N, txtCost));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtColorFilter_N, txtColorFilter));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(optEndTrip_N, optEndTrip));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSpeedViolation_N, chkSpeedViolation.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(DropDownList1_N, DropDownList1));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHarshAcceleration_N, chkHarshAcceleration.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkHarshBraking_N, chkHarshBraking.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExtremeAcceleration_N, chkExtremeAcceleration.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkExtremeBraking_N, chkExtremeBraking.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkSeatBeltViolation_N, chkSeatBeltViolation.ToString()));
            if (txtFrom.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtFrom_N, string.Format("{0:MM/dd/yyyy} {0:t}", txtFrom)));
            if (txtTo.HasValue) spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtTo_N, string.Format("{0:MM/dd/yyyy} {0:t}", txtTo)));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFleet_N, cboFleet));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFleet_Name_N, cboFleet_Name));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboVehicle_N, cboVehicle));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboFormat_N, cboFormat));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(selectedFleets_N, selectedFleets));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(category_N, category));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(keyValue_N, keyValue));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSpeed120_N, txtSpeed120));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtAccHarsh_N, txtAccHarsh));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtBrakingExtreme_N, txtBrakingExtreme));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSpeed130_N, txtSpeed130));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtAccExtreme_N, txtAccExtreme));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSeatBelt_N, txtSeatBelt));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtSpeed140_N, txtSpeed140));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(txtBrakingHarsh_N, txtBrakingHarsh));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(ddlLandmarks_N, ddlLandmarks));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(ddlGeozones_N, ddlGeozones));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(chkActiveVehicles_N, chkActiveVehicles.ToString()));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(OrganizationHierarchyNodeCode_N, OrganizationHierarchyNodeCode));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboIdlingThreshold_N, cboIdlingThreshold));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboSpeedThreshold_N, cboSpeedThreshold));
            spCustomProperty.Append(clsAsynGenerateReport.MakePair(cboMediaType_N, cboMediaType));

            spCustomProperty.Append(clsAsynGenerateReport.MakePair(ddlDrivers_N, ddlDrivers));

            return spCustomProperty.ToString();
        }

        /// <summary>
        /// Create report params and redirect to the next page
        /// </summary>
        /// <param name="redirectUrl">Web page to redirect to</param>
        public Boolean CreateReportParams()
        {
            try
            {
                string strFromDate = string.Empty;
                string strToDate = string.Empty;
                strFromDate = this.txtFrom.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
                strToDate = this.txtTo.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
                DateTime from, to;
                sn.Report.IsFleet = this.cboVehicle == "0" ? true : false;

                const string dateFormat = "MM/dd/yyyy HH:mm:ss";



                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

                from = Convert.ToDateTime(strFromDate, ci);
                to = Convert.ToDateTime(strToDate, ci);





                # region Reports
                switch (this.cboReports)
                {

                    case "31":
                        sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamVehicleId, this.cboVehicle.Trim());
                        sn.Report.XmlParams += String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost);
                        break;
                    case "32":
                    case "37":
                        //sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                        sn.Report.XmlParams = String.Format("{0};{1}", this.txtCost, this.txtColorFilter);
                        break;
                    case "33":
                    case "35":
                    case "75":
                    case "80":
                    case "81":
                    case "84":
                    case "85":
                        //sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamIncludeSummary, this.cboViolationSpeed.SelectedItem.Value);

                        sn.Report.XmlParams = String.Format("{0};{1}", this.cboViolationSpeed, this.txtColorFilter);

                        break;

                    #region Organization summary
                    case "38":
                        sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);
                        break;
                    # endregion
                    #region Activity Summary Report per Vehicle
                    case "39":
                    case "92":
                        sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);
                        break;
                    # endregion
                    case "47":
                        sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost);
                        break;

                    #region Vehicle summary
                    case "56":
                        sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip);
                        break;
                    # endregion

                    case "57":
                    case "72":
                    case "123":
                        sn.Report.XmlParams = CreateViolationParameters(this.cboReports) +"|"+ sn.Report.TmpData;
                        break;

                    #region New Trips Summary Report per Vehicle
                    case "63":
                        // xmlParams = String.Format("{0}={1};{2}={3}",
                        //       ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue,
                        //       ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);

                        sn.Report.XmlParams = this.cboVehicle.Trim();
                        // sn.Report.XmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);


                        break;
                    # endregion

                    case "69":
                        sn.Report.XmlParams = String.Format("{0};{1}", this.chkActiveVehicles == true ? "1" : "0", sn.User.OrganizationId);
                        break;

                    case "73":
                        sn.Report.XmlParams = String.Format("{0};{1}", this.chkActiveVehicles == true ? "1" : "0", sn.Report.VehicleId);
                        break;


                    #region Worksite Activity Report
                    case "64":
                    case "66":
                    case "67":
                    //case "69":
                    //case "73":
                    case "77":
                    case "79":
                    case "100":
                    case "102":
                        sn.Report.XmlParams = this.chkActiveVehicles == true ? "1" : "0";
                        break;
                    # endregion

                    case "76":
                        sn.Report.XmlParams = sn.User.OrganizationId.ToString();
                        break;

                    case "82":
                    case "86":

                        DataSet dsVehicle = null;
                        string ConnnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                        using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(ConnnectionString))
                        {
                            dsVehicle = dbVehicle.GetVehicleInfoByVehicleId(Convert.ToInt32(this.cboVehicle));
                        }


                        if (Util.IsDataSetValid(dsVehicle))
                        {
                            sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripFirstParamName, dsVehicle.Tables[0].Rows[0]["LicensePlate"].ToString());

                        }

                        string convFromDate = from.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
                        string convToDate = to.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);

                        sn.Report.XmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);
                        break;
                    //#region Timesheet Validation
                    //case "66":
                    //  sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);
                    //  break; 
                    //#endregion
                    //Audit Report


                    #region Road Speed Violation Summary
                    case "107":
                        sn.Report.XmlParams = this.ddlDrivers + ";" + cboIdlingThreshold;
                        break;
                    # endregion

                    #region Driver Violation Report
                    case "110":
                        sn.Report.XmlParams = this.ddlDrivers + ";" + this.cboSpeedThreshold;
                       break;
                    # endregion

                    case "113":
                       sn.Report.XmlParams = this.cboIdlingThreshold;
                       break;
                    case "114":
                       sn.Report.XmlParams = this.ddlDrivers;
                       break;

                    #region Road Speed Violation
                    case "124":
                       sn.Report.XmlParams = String.Format("{0};{1}", chkActiveVehicles == true ? "1" : "0", cboMediaType);
                       break;
                    # endregion
                }
                # endregion Reports

                sn.Report.GuiId = Convert.ToInt16(this.cboReports);
                sn.Report.FromDate = from.ToString();
                sn.Report.ToDate = to.ToString();
                sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat);
                sn.Report.FleetId = Convert.ToInt32(this.cboFleet);
                sn.Report.IsFleet = this.cboVehicle == "0" ? true : false;
                sn.Report.VehicleId = Convert.ToInt32(this.cboVehicle);
                sn.Report.FleetName = this.cboFleet_Name;
                sn.Report.ReportType = cboReports;
                sn.Report.OrganizationHierarchyNodeCode = OrganizationHierarchyNodeCode;

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                if (basepage != null) basepage.RedirectToLogin();
            }
            catch (Exception Ex)
            {
                string page_Name = string.Empty;
                if (basepage != null)
                { page_Name = basepage.GetType().Name; }

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + page_Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                if (basepage != null) basepage.ExceptionLogger(trace);

            }
            return true;
            //Response.Redirect(redirectUrl);
        }


        private string CreateTripDetailsParameteres(string fromDate, string toDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripSecondParamName : ReportTemplate.RpFleetDetailedTripSecondParamName, fromDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripThirdParamName : ReportTemplate.RpFleetDetailedTripThirdParamName, toDate));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripFourthParamName : ReportTemplate.RpFleetDetailedTripFourthParamName, "True"));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripFifthParamName : ReportTemplate.RpFleetDetailedTripFifthParamName, "True"));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripSixthParamName : ReportTemplate.RpFleetDetailedTripSixthParamName, "True"));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripSeventhParamName : ReportTemplate.RpFleetDetailedTripSeventhParamName, "True"));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripEighthParamName : ReportTemplate.RpFleetDetailedTripEighthParamName, "True"));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripNinthParamName : ReportTemplate.RpFleetDetailedTripNinthParamName, "True"));
            sb.Append(ReportTemplate.MakePair(this.cboVehicle == "0" ? ReportTemplate.RpDetailedTripTenthParamName : ReportTemplate.RpFleetDetailedTripTenthParamName, "3"));
            return sb.ToString();

        }

        private string CreateViolationParameters(string reportGuiId)
        {
            int intCriteria = 0;

            if (this.chkSpeedViolation)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
            if (this.chkHarshAcceleration)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
            if (this.chkHarshBraking)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
            if (this.chkExtremeAcceleration)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
            if (this.chkExtremeBraking)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
            if (this.chkSeatBeltViolation)
                intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;

            StringBuilder Params = new StringBuilder();
            Params.Append(intCriteria);

            // violation summary
            if (reportGuiId == "72" || reportGuiId == "123")
            {
                Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7}",
                   this.txtSpeed120, this.txtSpeed130, this.txtSpeed140,
                   this.txtAccExtreme, this.txtAccHarsh, this.txtBrakingExtreme,
                   this.txtBrakingHarsh, this.txtSeatBelt);

            }

            else
            {

                Params.Append("*");
                string tmpSpeed = "";
             switch (this.cboViolationSpeed)
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
        /// Get custom property
        /// </summary>
        /// <returns></returns>
        public void GetCustomProperty(string properties)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            cboReports = clsAsynGenerateReport.PairFindValue(cboReports_N, properties);
            cboViolationSpeed = clsAsynGenerateReport.PairFindValue(cboViolationSpeed_N, properties);
            txtCost = clsAsynGenerateReport.PairFindValue(txtCost_N, properties);
            txtColorFilter = clsAsynGenerateReport.PairFindValue(txtColorFilter_N, properties);
            optEndTrip = clsAsynGenerateReport.PairFindValue(optEndTrip_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSpeedViolation_N, properties), out  chkSpeedViolation);
            DropDownList1 = clsAsynGenerateReport.PairFindValue(DropDownList1_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHarshAcceleration_N, properties), out  chkHarshAcceleration);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkHarshBraking_N, properties), out  chkHarshBraking);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExtremeAcceleration_N, properties), out   chkExtremeAcceleration);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkExtremeBraking_N, properties), out   chkExtremeBraking);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkSeatBeltViolation_N, properties), out   chkSeatBeltViolation);

            string str_date = clsAsynGenerateReport.PairFindValue(txtFrom_N, properties);
            if (!string.IsNullOrEmpty(str_date)) txtFrom = Convert.ToDateTime(str_date, ci);
            else txtFrom = null;

            str_date = clsAsynGenerateReport.PairFindValue(txtTo_N, properties);
            if (!string.IsNullOrEmpty(str_date)) txtTo = Convert.ToDateTime(txtTo, ci);
            else txtTo = null;

            cboFleet = clsAsynGenerateReport.PairFindValue(cboFleet_N, properties);
            cboFleet_Name = clsAsynGenerateReport.PairFindValue(cboFleet_Name_N, properties);
            cboVehicle = clsAsynGenerateReport.PairFindValue(cboVehicle_N, properties);
            cboFormat = clsAsynGenerateReport.PairFindValue(cboFormat_N, properties);
            selectedFleets = clsAsynGenerateReport.PairFindValue(selectedFleets_N, properties);
            keyValue = clsAsynGenerateReport.PairFindValue(keyValue_N, properties);


            txtSpeed120 = clsAsynGenerateReport.PairFindValue(txtSpeed120_N, properties);
            txtAccHarsh = clsAsynGenerateReport.PairFindValue(txtAccHarsh_N, properties);
            txtBrakingExtreme = clsAsynGenerateReport.PairFindValue(txtBrakingExtreme_N, properties);
            txtSpeed130 = clsAsynGenerateReport.PairFindValue(txtSpeed130_N, properties);
            txtAccExtreme = clsAsynGenerateReport.PairFindValue(txtAccExtreme_N, properties);
            txtSeatBelt = clsAsynGenerateReport.PairFindValue(txtSeatBelt_N, properties);
            txtSpeed140 = clsAsynGenerateReport.PairFindValue(txtSpeed140_N, properties);
            txtBrakingHarsh = clsAsynGenerateReport.PairFindValue(txtBrakingHarsh_N, properties);
            ddlLandmarks = clsAsynGenerateReport.PairFindValue(ddlLandmarks_N, properties);
            ddlGeozones = clsAsynGenerateReport.PairFindValue(ddlGeozones_N, properties);
            bool.TryParse(clsAsynGenerateReport.PairFindValue(chkActiveVehicles_N, properties), out  chkActiveVehicles);
            OrganizationHierarchyNodeCode = clsAsynGenerateReport.PairFindValue(OrganizationHierarchyNodeCode_N, properties);


            cboIdlingThreshold = clsAsynGenerateReport.PairFindValue(cboIdlingThreshold_N, properties);
            cboSpeedThreshold = clsAsynGenerateReport.PairFindValue(cboSpeedThreshold_N, properties);
            cboMediaType = clsAsynGenerateReport.PairFindValue(cboMediaType_N, properties);

            cboMediaType = clsAsynGenerateReport.PairFindValue(ddlDrivers_N, properties);
        }
    }
    # endregion

    /// <summary>
    /// This class is for binding Report Repository data to rad grid
    /// </summary>
    public class clsReportRepository : VLF3.Domain.ActiveState.ReportRepository
    {
        private string guiName = string.Empty;
        public string RequestedStr = string.Empty;
        public string CompletedStr = string.Empty;

        public string GuiName
        {
            get { return guiName; }
            set { guiName = value; }
        }


        public clsReportRepository()
        {
        }
    }

    /// <summary>
    ///  This class is for binding User report data to rad grid
    /// </summary>
    public class clsUserReport
    {
        public clsUserReport() { }
        private string _Name;
        public string DateFromStr = string.Empty;
        public string DateToStr = string.Empty;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }


        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }


        private DateTime? _DateFrom;
        public DateTime? DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; }
        }


        private DateTime? _DateTo;
        public DateTime? DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; }
        }

        private string _CustomProp;
        public string CustomProp
        {
            get { return _CustomProp; }
            set { _CustomProp = value; }
        }


        private int _FormatId;
        public int FormatId
        {
            get { return _FormatId; }
            set { _FormatId = value; }
        }


        private long _UserReportId;
        public long UserReportId
        {
            get { return _UserReportId; }
            set { _UserReportId = value; }
        }

        private string _Category;
        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
    }

    /// <summary>
    ///  This class is for binding scheduled report data to rad grid
    /// </summary>
    public class clsScheduledReport
    {
        public clsScheduledReport() { }
        public String DateFromStr = string.Empty;
        public String DateToStr = string.Empty;
        public String StartScheduledDateStr = string.Empty;
        public String EndScheduledDateStr = string.Empty;

        private string _GuiName;
        public string GuiName
        {
            get { return _GuiName; }
            set { _GuiName = value; }
        }

        private DateTime _DateFrom;
        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; }
        }


        private DateTime _DateTo;
        public DateTime DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; }
        }


        private string _FleetName;
        public string FleetName
        {
            get { return _FleetName; }
            set { _FleetName = value; }
        }

        private DateTime _StartScheduledDate;
        public DateTime StartScheduledDate
        {
            get { return _StartScheduledDate; }
            set { _StartScheduledDate = value; }
        }


        private DateTime _EndScheduledDate;
        public DateTime EndScheduledDate
        {
            get { return _EndScheduledDate; }
            set { _EndScheduledDate = value; }
        }


        private string _Status;
        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private long _ReportID;
        public long ReportID
        {
            get { return _ReportID; }
            set { _ReportID = value; }
        }

        private string _DeliveryMethodType;
        public string DeliveryMethodType
        {
            get { return _DeliveryMethodType; }
            set { _DeliveryMethodType = value; }
        }

        private string _DeliveryMethod;
        public string DeliveryMethod
        {
            get { return _DeliveryMethod; }
            set { _DeliveryMethod = value; }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

    }

    /// <summary>
    ///  This class is for binding scheduled report file data to rad grid
    /// </summary>
    public class clsScheduledReportFile
    {
        public clsScheduledReportFile() { }
        private long _RowID;
        public long RowID
        {
            get { return _RowID; }
            set { _RowID = value; }
        }

        private string _ReportFileName;
        public string ReportFileName
        {
            get { return _ReportFileName; }
            set { _ReportFileName = value; }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { _DateCreated = value; }
        }


        private string _GuiName;
        public string GuiName
        {
            get { return _GuiName; }
            set { _GuiName = value; }
        }

        private string _StartScheduledDate;
        public string StartScheduledDate
        {
            get { return _StartScheduledDate; }
            set { _StartScheduledDate = value; }
        }

        private int _DeliveryMethod;
        public int DeliveryMethod
        {
            get { return _DeliveryMethod; }
            set { _DeliveryMethod = value; }
        }
    }

    /// <summary>
    /// This class is for passing data to CallReportService
    /// </summary>
    public class clsUserReportParams
    {
        private string _ReportName;
        public string ReportName
        {
            get { return _ReportName; }
            set { _ReportName = value; }
        }

        private string _ReportDescription;
        public string ReportDescription
        {
            get { return _ReportDescription; }
            set { _ReportDescription = value; }
        }

        private string _XmlParams;
        public string XmlParams
        {
            get { return _XmlParams; }
            set { _XmlParams = value; }
        }

        private string _ReportRepositoryId;
        public string ReportRepositoryId
        {
            get { return _ReportRepositoryId; }
            set { _ReportRepositoryId = value; }
        }

        public clsUserReportParams() { }
    }
    /// <summary>
    ///     This class is to call report service Asynchronously
    /// </summary>

    public class clsAsynGenerateReport
    {
        public static string Seperator = "<S,/>";
        public static string KeySeperator = "=";
        public static string DateTimeFilterFlage = "#datetime";
        public static string DateTimeFilterMin = "19000101";
        public static DateTime DateTimeFilterMinDate = new DateTime(1900, 01, 01);
        public static string DateTimeFilterMax = "25000101";
        public List<int> fleetIdsList = null;
        public clsAsynGenerateReport()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //my data access begin
        public VLF3.Domain.ActiveState.ReportRepository  GetReportRepositoryById(Int64 ReportRepositoryID)
        {
            VLF3.Domain.ActiveState.ReportRepository reportRepository = null;
            DataSet ds = new DataSet();
            using (SqlConnection connection =
new SqlConnection(
   ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "GetReportRepositoryById";

                        SqlParameter para = new SqlParameter("ReportRepositoryID", SqlDbType.BigInt);
                        para.Value = ReportRepositoryID;
                        sqlComment.Parameters.Add(para);

                        sqlComment.CommandTimeout = 600;
                        //connection.Open();
                        DataSet dataSet = new DataSet();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = sqlComment;
                        adapter.Fill(ds);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
                try
                {
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    { 
                        reportRepository = new VLF3.Domain.ActiveState.ReportRepository();
                        if (!(ds.Tables[0].Rows[0]["ReportRepositoryId"] is DBNull))
                            reportRepository.ReportRepositoryId = long.Parse(ds.Tables[0].Rows[0]["ReportRepositoryId"].ToString());

                        if (!(ds.Tables[0].Rows[0]["UserId"] is DBNull))
                            reportRepository.UserId = long.Parse(ds.Tables[0].Rows[0]["UserId"].ToString());


                        if (!(ds.Tables[0].Rows[0]["ReportTypeId"] is DBNull))
                        {
                            reportRepository.ReportType = new VLF3.Domain.ActiveState.ReportType();
                            reportRepository.ReportType.ReportTypeId = int.Parse(ds.Tables[0].Rows[0]["ReportTypeId"].ToString());
                        }
                            

                        if (!(ds.Tables[0].Rows[0]["Path"] is DBNull))
                            reportRepository.Path = ds.Tables[0].Rows[0]["Path"].ToString();

                        if (!(ds.Tables[0].Rows[0]["Start"] is DBNull))
                            reportRepository.Start = double.Parse(ds.Tables[0].Rows[0]["Start"].ToString());

                        if (!(ds.Tables[0].Rows[0]["Period"] is DBNull))
                            reportRepository.Period = double.Parse(ds.Tables[0].Rows[0]["Period"].ToString());

                        if (!(ds.Tables[0].Rows[0]["KeyValues"] is DBNull))
                            reportRepository.KeyValues = ds.Tables[0].Rows[0]["KeyValues"].ToString();

                        if (!(ds.Tables[0].Rows[0]["FormatId"] is DBNull))
                            reportRepository.FormatId = short.Parse(ds.Tables[0].Rows[0]["FormatId"].ToString());

                        if (!(ds.Tables[0].Rows[0]["Requested"] is DBNull))
                            reportRepository.Requested = DateTime.Parse(ds.Tables[0].Rows[0]["Requested"].ToString());

                        if (!(ds.Tables[0].Rows[0]["Completed"] is DBNull))
                            reportRepository.Completed = DateTime.Parse(ds.Tables[0].Rows[0]["Completed"].ToString());

                        if (!(ds.Tables[0].Rows[0]["UpdaterId"] is DBNull))
                            reportRepository.UpdaterId = long.Parse(ds.Tables[0].Rows[0]["UpdaterId"].ToString());

                        if (!(ds.Tables[0].Rows[0]["IsRead"] is DBNull))
                            reportRepository.IsRead = Boolean.Parse(ds.Tables[0].Rows[0]["IsRead"].ToString());

                    }
                }
                catch (Exception ex)
                { return null; }
            }
            return reportRepository;
        }

        public void UpdateReadStatus(Int64 ReportRepositoryID, Boolean IsRead)
        {
            Boolean isSucceed = true;
            using (SqlConnection connection =
new SqlConnection(
   ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "UpdateReadStatus";

                        SqlParameter para = new SqlParameter("ReportRepositoryID", SqlDbType.BigInt);
                        para.Value = ReportRepositoryID;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("IsRead", SqlDbType.Bit);
                        para.Value = IsRead;
                        sqlComment.Parameters.Add(para);
                        connection.Open();
                        sqlComment.ExecuteNonQuery();
                        sqlComment.CommandTimeout = 600;
                        //connection.Open();
                    }
                    catch (Exception ex)
                    {
                        isSucceed = false;
                    }

                    if (connection.State == ConnectionState.Open) connection.Close();
                }

            }
            if (!isSucceed) throw new Exception("Failed to update.");
        }


        public void DeleteReportRepository(Int64 ReportRepositoryID)
        {
            Boolean isSucceed = true;
            using (SqlConnection connection =
new SqlConnection(
   ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "DeleteReportRepository";

                        SqlParameter para = new SqlParameter("ReportRepositoryID", SqlDbType.BigInt);
                        para.Value = ReportRepositoryID;
                        sqlComment.Parameters.Add(para);

                        connection.Open();
                        sqlComment.ExecuteNonQuery();
                        sqlComment.CommandTimeout = 600;
                        //connection.Open();
                    }
                    catch (Exception ex)
                    {
                        isSucceed = false;
                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }

            }
            if (!isSucceed) throw new Exception("Failed to delete.");
        }

        public void DeleteUserReport(Int64 UserReportId)
        {
            Boolean isSucceed = true;
            using (SqlConnection connection =
new SqlConnection(
   ConfigurationManager.ConnectionStrings["InfoStoreConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "DeleteUserReport";

                        SqlParameter para = new SqlParameter("UserReportId", SqlDbType.BigInt);
                        para.Value = UserReportId;
                        sqlComment.Parameters.Add(para);

                        connection.Open();
                        sqlComment.ExecuteNonQuery();
                        sqlComment.CommandTimeout = 600;
                        //connection.Open();
                    }
                    catch (Exception ex)
                    {
                       isSucceed = false;
                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }

            }
            if (!isSucceed) throw new Exception("Failed to delete.");
        }

        public VLF3.Domain.InfoStore.UserReport GetUserReportById(Int64 UserReportId)
        {
            VLF3.Domain.InfoStore.UserReport userReport = null;
            DataSet ds = new DataSet();
            using (SqlConnection connection =
new SqlConnection(
   ConfigurationManager.ConnectionStrings["InfoStoreConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "GetUserReportById";

                        SqlParameter para = new SqlParameter("UserReportId", SqlDbType.BigInt);
                        para.Value = UserReportId;
                        sqlComment.Parameters.Add(para);

                        sqlComment.CommandTimeout = 600;
                        //connection.Open();
                        DataSet dataSet = new DataSet();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = sqlComment;
                        adapter.Fill(ds);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
                try
                {
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        userReport = new VLF3.Domain.InfoStore.UserReport();
                        if (!(ds.Tables[0].Rows[0]["UserReportId"] is DBNull))
                            userReport.UserReportId = long.Parse(ds.Tables[0].Rows[0]["UserReportId"].ToString());

                        if (!(ds.Tables[0].Rows[0]["Name"] is DBNull))
                            userReport.Name = ds.Tables[0].Rows[0]["Name"].ToString();

                        if (!(ds.Tables[0].Rows[0]["Description"] is DBNull))
                            userReport.Description = ds.Tables[0].Rows[0]["Description"].ToString();

                        if (!(ds.Tables[0].Rows[0]["Start"] is DBNull))
                            userReport.Start = double.Parse(ds.Tables[0].Rows[0]["Start"].ToString());

                        if (!(ds.Tables[0].Rows[0]["Period"] is DBNull))
                            userReport.Period = double.Parse(ds.Tables[0].Rows[0]["Period"].ToString());

                        if (!(ds.Tables[0].Rows[0]["CustomProp"] is DBNull))
                            userReport.CustomProp = ds.Tables[0].Rows[0]["CustomProp"].ToString();

                        if (!(ds.Tables[0].Rows[0]["FormatId"] is DBNull))
                            userReport.FormatId = short.Parse(ds.Tables[0].Rows[0]["FormatId"].ToString());

                        if (!(ds.Tables[0].Rows[0]["UserId"] is DBNull))
                        {
                            userReport.User = new VLF3.Domain.InfoStore.User();
                            userReport.User.UserId= long.Parse(ds.Tables[0].Rows[0]["UserId"].ToString());
                        }

                        //if (!(ds.Tables[0].Rows[0]["UpdateDate"] is DBNull))
                        //    userReport.UpdateDate = DateTime.Parse(ds.Tables[0].Rows[0]["UpdateDate"].ToString());

                        //if (!(ds.Tables[0].Rows[0]["UpdaterId"] is DBNull))
                        //    userReport.UpdaterId = long.Parse(ds.Tables[0].Rows[0]["UpdaterId"].ToString());

                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return userReport;
        }
        //my data access end

        public DataSet GetReportRepositoryByReportRepositoryIdandUserId(Int64 reportRepositoryId, Int64 UserId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection =
new SqlConnection(
   ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "GetReportRepositoryByReportRepositoryIdandUserId";

                        SqlParameter para = new SqlParameter("reportRepositoryId", SqlDbType.BigInt);
                        para.Value = reportRepositoryId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("UserId", SqlDbType.BigInt);
                        para.Value = UserId;
                        sqlComment.Parameters.Add(para);


                        sqlComment.CommandTimeout = 600;
                        //connection.Open();
                        DataSet dataSet = new DataSet();
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = sqlComment;
                        adapter.Fill(ds);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            return ds;
        }


        private Int64 SaveReportRepository(Int64 UserId, int ReportTypeId,
                                          double Start, double Period, string KeyValues, int FormatId)
        {
            Int64 isSucceed = -1;
            using (SqlConnection connection =
           new SqlConnection(
               ConfigurationManager.ConnectionStrings["ActiveStateConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {

                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "AddReportRepository";

                        SqlParameter para = new SqlParameter("UserId", SqlDbType.BigInt);
                        para.Value = UserId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("ReportTypeId", SqlDbType.Int);
                        para.Value = ReportTypeId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("Start", SqlDbType.Float);
                        para.Value = Start;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("Period", SqlDbType.Float);
                        para.Value = Period;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("KeyValues", SqlDbType.NVarChar, 1024);
                        para.Value = KeyValues;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("FormatId", SqlDbType.TinyInt);
                        para.Value = FormatId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("UpdaterId", SqlDbType.BigInt);
                        para.Value = UserId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("@RETURN_VALUE", SqlDbType.BigInt);
                        para.Direction = ParameterDirection.ReturnValue;
                        para.Value = 0;
                        sqlComment.Parameters.Add(para);
                        sqlComment.CommandTimeout = 600;
                        connection.Open();
                        sqlComment.ExecuteNonQuery();
                        if (sqlComment.Parameters["@RETURN_VALUE"] != null)
                            isSucceed = Int64.Parse(sqlComment.Parameters["@RETURN_VALUE"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        isSucceed = -1;
                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            return isSucceed;
        }

        private Boolean SaveUserReport(Int64 UserReportId, string Name, string Description,
                                   double Start, double Period, string CustomProp, int FormatId, Int64 UserId)
        {
            Boolean isSucceed = true;
            using (SqlConnection connection =
           new SqlConnection(
               ConfigurationManager.ConnectionStrings["InfoStoreConnectionString"].ConnectionString))
            {
                using (SqlCommand sqlComment = new SqlCommand())
                {
                    try
                    {
                        sqlComment.CommandType = CommandType.StoredProcedure;
                        sqlComment.Connection = connection;
                        sqlComment.CommandText = "AddUserReport";

                        SqlParameter para = new SqlParameter("UserReportId", SqlDbType.BigInt);
                        para.Value = UserReportId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("Name", SqlDbType.NVarChar, 50);
                        para.Value = Name;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("Description", SqlDbType.NVarChar, 255);
                        para.Value = Description;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("Start", SqlDbType.Float);
                        para.Value = Start;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("Period", SqlDbType.Float);
                        para.Value = Period;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("CustomProp", SqlDbType.NVarChar, -1);
                        para.Value = CustomProp;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("FormatId", SqlDbType.TinyInt);
                        para.Value = FormatId;
                        sqlComment.Parameters.Add(para);

                        para = new SqlParameter("UserId", SqlDbType.BigInt);
                        para.Value = UserId;
                        sqlComment.Parameters.Add(para);

                        sqlComment.CommandTimeout = 600;
                        connection.Open();
                        sqlComment.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        isSucceed = false;
                    }
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            return isSucceed;
        }
        /// <summary>
        /// This method is to call report service 
        /// </summary>
        /// <param name="sn">Session sn </param>
        /// <param name="basePage">Base page class from calling page</param>
        public Boolean CallReportService(SentinelFMSession sn, SentinelFMBasePage basePage, clsUserReportParams userRepoProperty, string keyValue)
        {
            Boolean ret = true;
            CrystalRpt.CrystalRpt cr = new CrystalRpt.CrystalRpt();
            cr.Timeout = -1; // infinite time out.
            string ReportType = sn.Report.ReportType;
            string ReportPath = "";
            bool RequestOverflowed = false;
            bool OutMaxOverflowed = false;
            string strUrl = "";
            Stopwatch watch = new Stopwatch();

            int VehicleId = 0;
            Single Cost = 0;
            Int16 sensorNum = 0;
            string LicensePlate = "";
            int Type = 0;
            Int16 Summary = 0;

            string[] tmp;
            string ColorFilter = "";

            watch.Reset();
            watch.Start();

            try
            {
                SecurityManager.SecurityManager securityManager = new SecurityManager.SecurityManager();
                string SID = string.Empty;
                if (sn.SecId != null) SID = sn.SecId;
                //securityManager.ValidateandReloginMD5ByDBName(sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref SID);
                securityManager.ReloginMD5ByDBName(sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref SID);
                sn.SecId = SID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Call SecurityManager.ValidateandReloginMD5ByDBName failed. Ex:" + ex.Message));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            }
            try
            {
                //Save to UserReport database
                if (userRepoProperty != null)
                {
                    //VLF3.Services.InfoStore.UserService uss = VLF3.Services.InfoStore.UserService.GetInstance(sn.UserID);
                    VLF3.Domain.InfoStore.UserReport usrrepo = null;
                    if (!string.IsNullOrEmpty(userRepoProperty.ReportRepositoryId))
                    {
                        usrrepo =  GetUserReportById(long.Parse(userRepoProperty.ReportRepositoryId)); //for update
                    }
                    if (usrrepo == null)
                    {
                        usrrepo = new VLF3.Domain.InfoStore.UserReport(); //for insert

                        usrrepo.UserReportId = -1;

                        usrrepo.Name = userRepoProperty.ReportName;
                        usrrepo.Description = userRepoProperty.ReportDescription;
                    }
                    usrrepo.CustomProp = userRepoProperty.XmlParams;
                    try
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                        DateTime from = Convert.ToDateTime(sn.Report.FromDate, ci);
                        DateTime to = Convert.ToDateTime(sn.Report.ToDate, ci);
                        usrrepo.Start = from.Subtract(System.DateTime.Now.ToUniversalTime().Date).TotalMinutes;
                        usrrepo.Period = to.Subtract(from).TotalMinutes;
                    }
                    catch (Exception ex) { }
                    usrrepo.FormatId = (short)sn.Report.ReportFormat;
                    usrrepo.User = new VLF3.Domain.InfoStore.User();
                    usrrepo.User.UserId = sn.UserID;


                    usrrepo.User.UserName = sn.UserName;
                    if (!SaveUserReport(usrrepo.UserReportId, usrrepo.Name, usrrepo.Description, usrrepo.Start,
                        usrrepo.Period, usrrepo.CustomProp, usrrepo.FormatId, sn.UserID))
                        return false;
                }

                //Save to repository database
                //VLF3.Services.ActiveState.ReportRepositoryService rrs = VLF3.Services.ActiveState.ReportRepositoryService.GetInstance(sn.UserID);
                VLF3.Domain.ActiveState.ReportRepository rr = new VLF3.Domain.ActiveState.ReportRepository();
                rr.FormatId = short.Parse(sn.Report.ReportFormat.ToString());
                //rr.Requested = System.DateTime.Now;
                rr.KeyValues = keyValue;
                rr.UserId = sn.UserID;
                try
                {
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                    DateTime from = Convert.ToDateTime(sn.Report.FromDate, ci);
                    DateTime to = Convert.ToDateTime(sn.Report.ToDate, ci);
                    rr.Start = from.Subtract(rr.Requested.Date).TotalMinutes;
                    rr.Period = to.Subtract(from).TotalMinutes;
                }
                catch (Exception ex) { }
                rr.Completed = null;
                rr.ReportRepositoryId = -1;
                //IList<VLF3.Domain.ActiveState.ReportType> rpts = rrs.GetReportTypeByGuiId(int.Parse(sn.Report.ReportType));
                //if (rpts != null && rpts.Count > 0) rr.ReportType = rpts[0];
                //rr.ReportType.ReportTypeId = Convert.ToInt16(sn.Report.ReportType);
                //List<VLF3.Domain.ActiveState.ReportRepository> rs = (List<VLF3.Domain.ActiveState.ReportRepository>)rrs.GetReportRepositoryByUser(sn.UserID);
                long returnID = SaveReportRepository(sn.UserID, int.Parse(ReportType),
                    rr.Start, rr.Period, rr.KeyValues, rr.FormatId);
                if (returnID == -1) return false;
                ReportPath = returnID.ToString();
                //tesing by devin
                //return true;
                //End
                Int16 tmpValue = 0;
                object asState = null;
                switch (ReportType)
                {
                    # region Trip Details Report
                    case "0":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginTripDetailsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, RequestOverflowed, OutMaxOverflowed, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginTripFleetDetailsReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Trip Summary Report
                    case "1":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginTripSummaryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginTripFleetSummaryReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Alarms Report
                    case "2":

                        // Response.Redirect("Report_Alarms.aspx");

                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginAlarmsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginAlarmsFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region History Report
                    case "3":
                        cr.BeginHistoryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, -1, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        break;
                    # endregion
                    # region Stop Report
                    case "4":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginStopReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginStopFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Messages Report
                    case "5":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginMessagesReport(sn.UserID, sn.SecId, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginMessagesFleetReport(sn.UserID, sn.SecId, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Exception Report
                    case "6":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginExceptionReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginExceptionFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Off Hours Report
                    case "8":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginOffHoursReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginOffHoursFleetReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Landmark Activity Report
                    case "9":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginLandmarkActivityReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, RequestOverflowed, OutMaxOverflowed, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginLandmarkFleetActivityReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, RequestOverflowed, OutMaxOverflowed, ReportPath, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Fleet Maintenance Report
                    case "10":
                        cr.BeginFleetMaintenaceReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        break;
                    # endregion
                    # region Utilization Report
                    case "11":
                        cr.BeginUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Utilization Summary Report
                    case "12":
                        cr.BeginUtilizationSummaryReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Utilization Daily Fleet  Report
                    case "13":
                        cr.BeginUtilizationDailyFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Utilization By VehicleType
                    case "14":
                        cr.BeginUtilizationByVehicleTypeReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Utilization Daily Detail
                    case "15":
                        cr.BeginUtilizationDailyDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Weekly Utilization
                    case "16":
                        cr.BeginUtilizationWeeklyReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Violation details report
                    case "17":
                       // if (!sn.Report.OrganizationHierarchySelected)
                        //{

			 tmp = sn.Report.XmlParams.Split('|');
                            cr.BeginViolationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, tmp[0], sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                        //    cr.BeginViolationReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    # endregion
                    # region Idling Details report
                    case "18":
                    case "10018":
                        cr.BeginIdlingDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Idling Summary report
                    case "19":
                    case "10017":
                        //if (objUtil.ErrCheck(cr.IdlingSummaryOrgReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //    if (objUtil.ErrCheck(cr.IdlingSummaryOrgReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        cr.BeginIdlingSummaryReportByOrgId(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Violation Summary report
                    case "20":
		      tmp = sn.Report.XmlParams.Split('|');
                        //if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //   if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //   {
                        //      //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                        //       ReportFailed();
                        //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                        //      return;
                        //   }

                        string xml = "";
                        //if (!sn.Report.OrganizationHierarchySelected)
                        //{
                            cr.BeginViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, tmp[0], sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                        //    cr.BeginViolationReportWithScore_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    # endregion
                    # region Landmark summary
                    case "21":
                        if (sn.Report.IsFleet)
                        {
                            cr.BeginLandmarkFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginLandmarkVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Geozone summary
                    case "22":
                        if (sn.Report.IsFleet)
                        {
                            cr.BeginGeozoneFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginGeozoneVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Landmark Details Report
                    case "23":
                        if (sn.Report.IsFleet)
                        {
                            cr.BeginLandmarkFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginLandmarkVehicleDetailsReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Inactivity Report
                    case "24":
                        if (sn.Report.IsFleet)
                        {
                            cr.BeginInactivityReport4Fleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginInactivityReport4Vehicle(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    # endregion
                    # region Driver Trip Details Report
                    case "25":
                        cr.BeginDriverTripDetailsReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, RequestOverflowed, OutMaxOverflowed, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Driver Trip Summary Report
                    case "26":
                        cr.BeginDriverTripSummaryReportOneByOne(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, RequestOverflowed, OutMaxOverflowed, wsCallback, asState);
                        break;
                    # endregion
                    # region Driver Violation Details Report
                    case "27":
                        cr.BeginDriverViolationReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Driver Violation Summary Report
                    case "28":
                        cr.BeginDriverViolationSummaryReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Fuel Transaction History
                    case "29":
                        cr.BeginFuelTransactionHistReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region Geozone Details
                    case "30":
                        if (sn.Report.IsFleet)
                        {
                            cr.BeginGeozoneFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                            //cr.GeozoneFleetDetailsReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,ref ReportPath);
                        }
                        else
                        {
                            cr.BeginGeozoneDetailsVehicleReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    #endregion
                    #region ExtendedDailyUtilizationReport
                    case "31":
                        //if (sn.Report.IsFleet)
                        //{
                        //    if (objUtil.ErrCheck(cr.GetExtendedDailyFleetUtilizationReport (sn.UserID, sn.SecId, sn.Report.FleetId , sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //        if (objUtil.ErrCheck(cr.GetExtendedDailyFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //        {
                        //            Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                        //            return;
                        //        }
                        //}
                        //else
                        //{


                        VehicleId = Convert.ToInt32(VLF.CLS.Util.PairFindValue(ReportTemplate.RptParamVehicleId, sn.Report.XmlParams));
                        Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost, sn.Report.XmlParams));


                        cr.BeginGetExtendedDailyVehicleUtilizationReport(sn.UserID, sn.SecId, VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, Cost, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    #endregion
                    #region ExtendedDailyFleetUtilizationReport
                    case "32":
                        Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost, sn.Report.XmlParams));

                        cr.BeginGetExtendedDailyFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region ExtendedSpeedViolationReportReport
                    case "33":
                        tmp = sn.Report.XmlParams.Split(';');
                        Type = Convert.ToInt32(tmp[0]);
                        ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";
                        //Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                        if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                        {
                            int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                            {
                                fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                                i++;
                            }

                            cr.BeginGetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }

                        break;
                    #endregion
                    #region State Mileage Report
                    case "34":
                        Summary = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary, sn.Report.XmlParams));

                        if (sn.Report.IsFleet)
                        {
                            cr.BeginStateMileageReportPerFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate, sn.Report.XmlParams);

                            cr.BeginStateMileageReportPerVehicle(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ReportPath, wsCallback, asState);
                        }
                        break;
                    #endregion
                    #region ExtendedSpeedViolationDetailsReportReport
                    case "35":
                        // Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                        tmp = sn.Report.XmlParams.Split(';');
                        Type = Convert.ToInt32(tmp[0]);
                        ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";

                        if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                        {
                            int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                            {
                                fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                                i++;
                            }

                            cr.BeginGetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        //else
                        //{
                        //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //        if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //        {
                        //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                        //            ReportFailed();
                        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                        //            return;
                        //        }
                        //}
                        break;
                    #endregion
                    #region State Mileage Summary Report
                    case "36":
                        Summary = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary, sn.Report.XmlParams));
                        cr.BeginStateMileageSummaryReportPerFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Summary, sn.Report.ReportFormat, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Fleet Utilization, Idling Details by Vehicle
                    case "37":
                        //Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost,sn.Report.XmlParams));

                        tmp = sn.Report.XmlParams.Split(';');
                        Cost = Convert.ToSingle(tmp[0]);
                        ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";


                        if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                        {
                            int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                            {
                                fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                                i++;
                            }


                            cr.BeginGetCNFleetsUtilizationReport(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Cost, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        //else
                        //{
                        //    if (objUtil.ErrCheck(cr.GetCNFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //        if (objUtil.ErrCheck(cr.GetCNFleetUtilizationReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //        {
                        //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                        //            ReportFailed();
                        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                        //            return;
                        //        }
                        //}
                        break;
                    #endregion
                    #region Activity Summary Report for Organization
                    case "38":

                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                        cr.BeginGetActivitySummaryReportPerOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Activity Summary Report per Vehicle
                    case "39":
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                       // if (!sn.Report.OrganizationHierarchySelected)
                        //{
                            cr.BeginGetActivitySummaryReportPerVehicle(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                         //   cr.BeginGetActivitySummaryReportPerVehicle_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}

                        break;
                    #endregion
                    #region New trips Summary Report per Vehicle
                    case "40":

                        LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate, sn.Report.XmlParams);
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                        cr.BeginGetTripsSummaryReportByLicensePlate(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Time At Landmark
                    case "41":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginGetTimeAtLandmarkReport(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginGetTimeAtLandmarkFleetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    #endregion
                    #region Monthly Utilization Report
                    case "42":

                        cr.BeginGetExtendedMonthlyFleetSummary(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Average Days Utilized Per Vehicle
                    case "43":

                        cr.BeginGetExtendedMonthlyAverageDaysUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region Average Service Hrs for Utilized Vehicle
                    case "44":

                        cr.BeginGetExtendedMonthlyServiceHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Average Engine ON Hrs for Utilized Vehicle
                    case "45":

                        cr.BeginGetExtendedMonthlyEngineOnHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Unnecessary Idling Hrs. Per Vehicle Per Month
                    case "46":

                        cr.BeginGetExtendedMonthlyUnnecessaryIdlingHrsUtilized(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Total Unnecessary Idling Fuel Costs
                    case "47":
                        Cost = Convert.ToSingle(Util.PairFindValue(ReportTemplate.RptParamCost, sn.Report.XmlParams));

                        cr.BeginGetExtendedUnnecessaryIdlingFuelCosts(sn.UserID, sn.SecId, sn.Report.FromDate, Cost, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region Average Travelled Distance
                    case "48":


                        cr.BeginGetExtendedAverageTravelledDistance(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region CN Vehicle Status
                    case "49":


                        cr.BeginGetVehiclesStatusReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Trip Summary Totals Report per Vehicle
                    case "50":
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                        cr.BeginGetTripSummaryTotalsReportperVehicle(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Trip Summary Totals Report per Organization
                    case "51":

                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                        cr.BeginGetTripSummaryTotalsReportperOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Activity Summary Salt Spreader
                    case "52":


                        cr.BeginGetActivitySummarySaltSpreader(sn.UserID, sn.SecId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region HOS Details Report
                    case "53":

                        int DriverId = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamDriverId, sn.Report.XmlParams));
                        //Response.Redirect("..\\HOS\\HOS_Report_.aspx");

                        cr.BeginGetHOSSummaryReport(sn.UserID, sn.SecId, DriverId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Report.GuiId, ReportPath, wsCallback, asState);
                        break;

                    #endregion
                    #region Vehicle Data Dump
                    case "54":
                        cr.BeginVehicleInfoDataDump(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    # region Fuel Transaction Report
                    case "55":
                        cr.BeginFuelTransactionReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region Activity Summary Report per Vehicle (CP)
                    case "56":
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                        cr.BeginGetActivitySummaryReportPerVehicle_CP(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    # region Fleet Violation detail Report
                    case "57":
                        cr.BeginViolationReport_CP(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Fleet Membership Report
                    case "58":
                        cr.BeginFleetMembershipReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Fleet Diagnostic Report
                    case "59":
                        cr.BeginFleetDiagnosticReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region User Login Report
                    case "60":
                        cr.BeginUserLoginsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region State Milage Report Pivot
                    case "61":
                        cr.BeginStateMileageReportPerFleet_StateBased(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    #region Activity Summary Report per Vehicle - Daily
                    case "62":
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                        cr.BeginGetActivitySummaryReportPerVehicle_Daily(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region TransportationMileageReport
                    case "63":
                    case "10020":
                        #region Get License Plate by Vehicle
                        //int vehicleId =Convert.ToInt32( sn.Report.XmlParams);
                        //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                        //string xml="";  

                        //if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , false))
                        //  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , true))
                        //   {
                        //       ReportFailed();
                        //       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetTripsSummaryReportByLicensePlate failed. User:" + sn.UserID.ToString()));
                        //       return;
                        //   }


                        //      if (xml == "")
                        //          {
                        //             return;
                        //          }
                        //      DataSet dsResult = new DataSet();

                        //      System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                        //      dsResult.ReadXml(strrXML);

                        //      LicensePlate = dsResult.Tables[0].Rows[0]["LicensePlate"].ToString();   
                        #endregion

                        sensorNum = 3;

                        LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate, sn.Report.XmlParams);
                        // sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId,sn.Report.XmlParams));



                        cr.BeginGetTransportationMileageReport(sn.UserID, sn.SecId, LicensePlate, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    #region Worksite Activity Report
                    case "64":
                        //#region Get License Plate by Vehicle
                        //int vehicleId = Convert.ToInt32(sn.Report.VehicleId);
                        //ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                        //string xml="";  

                        //if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , false))
                        //  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID,sn.SecId,vehicleId,ref xml)  , true))
                        //   {
                        //       ReportFailed();
                        //       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Worksite Activity Report failed. User:" + sn.UserID.ToString()));
                        //       return;
                        //   }


                        //      if (xml == "")
                        //          {
                        //             return;
                        //          }
                        //      DataSet dsResult = new DataSet();

                        //      System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                        //      dsResult.ReadXml(strrXML);

                        //      LicensePlate = dsResult.Tables[0].Rows[0]["LicensePlate"].ToString();   
                        //   #endregion


                        //LicensePlate = Util.PairFindValue(ReportTemplate.RptParamLicensePlate,sn.Report.XmlParams);


                        //if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(sn.UserID, sn.SecId, 1, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //    if (objUtil.ErrCheck(cr.GetActivityAtLandmarkSummaryReportPerFleetOnTheFly(sn.UserID, sn.SecId, 1, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //    {

                        cr.BeginGetActivityAtLandmarkSummaryReportPerFleet_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion
                    # region Idling  Messages Details  report
                    case "65":
                        cr.BeginIdlingMessagesDetailReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Timesheet Validation Details
                    case "66":
                        tmpValue = Convert.ToInt16(sn.Report.XmlParams);
                        sn.Report.XmlParams = "Details";
                        //if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams  , sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //    if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true ))
                        cr.BeginGeozoneTimeSheetReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, tmpValue, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    # region Timesheet Validation Summary
                    case "67":
                        tmpValue = Convert.ToInt16(sn.Report.XmlParams);
                        sn.Report.XmlParams = "Summary";
                        //if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //    if (objUtil.ErrCheck(cr.GeozoneTimeSheetReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        cr.BeginGeozoneTimeSheetReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, tmpValue, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    # region Vehicle WorkSite Activity Per Day
                    case "68":
                        cr.BeginWorksiteActivityPerDay(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    # region Vehicle Status by LastCommDate
                    case "69":
                        tmp = sn.Report.XmlParams.Split(';'); 
                        if (sn.User.OrganizationId == 570)
                        {
                            cr.BeginGetVehiclesStatusByDateReport_ActiveVehicles(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(tmp[0]), ReportPath, wsCallback, asState);
                        }
                        else
                        {
                            cr.BeginGetVehiclesStatusByDateReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }
                        break;
                    #endregion
                    # region  SensorActivity
                    case "70":
                    case "90":
                    case "91":
                        //if (!sn.Report.OrganizationHierarchySelected)
                        //{
                            cr.BeginGetSensorActivityReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                        //    cr.BeginGetSensorActivityReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    #endregion
                    #region Activity Summary Report per Vehicle Special
                    case "71":
                    case "10010":
                        sensorNum = 3;

                        cr.BeginGetActivitySummaryReportPerVehicle_Special(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    # region Violation Summary report Special
                    case "72":
                        cr.BeginViolationReportWithScore_Special(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Activity Outside Landmark
                    case "73":
                        tmp = sn.Report.XmlParams.Split(';'); 
                        //if (objUtil.ErrCheck(cr.ActivityOutsideLandmarkReport (sn.UserID, sn.SecId, sn.Report.FleetId,sn.Report.VehicleId,  sn.Report.FromDate, sn.Report.ToDate,  sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //    if (objUtil.ErrCheck(cr.ActivityOutsideLandmarkReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        cr.BeginActivityOutsideLandmarkReport_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(tmp[0]), ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Worksite Details Report
                    case "74":
                        cr.BeginActivityInLandmarkReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.VehicleId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    #region ExtendedSpeedViolationDetailsReportReport
                    case "75":
                        // Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                        tmp = sn.Report.XmlParams.Split(';');
                        Type = Convert.ToInt32(tmp[0]);
                        ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";



                        cr.BeginGetExtendedSpeedSummaryViolationsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);


                        //else
                        //{
                        //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //        if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //        {
                        //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                        //            ReportFailed();
                        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                        //            return;
                        //        }
                        //}
                        break;
                    #endregion
                    # region Fleet Membership Report By User
                    case "76":
                        cr.BeginFleetMembershipReportUser(sn.UserID, sn.User.OrganizationId, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    # region Fleet Membership Report Special
                    case "77":
                        cr.BeginBrickmanFleetMembershipReport(sn.UserID, sn.SecId, Convert.ToInt32(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    #region Worksite Activity Report-Winter
                    case "79":
                        cr.BeginGetActivityAtLandmarkSummaryReportPerFleetSnow_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Convert.ToInt16(sn.Report.XmlParams), ReportPath, wsCallback, asState);

                        break;
                    #endregion

                    #region ExtendedSpeedViolationDetailsReportReport_RoadSpeed
                    case "80":
                    case "84":
                        // Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                        tmp = sn.Report.XmlParams.Split(';');
                        Type = Convert.ToInt32(tmp[0]);
                        ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";

                        if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                        {
                            int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                            {
                                fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                                i++;
                            }

                            //if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                            //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type,ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                            //    {

                            cr.BeginGetExtendedSpeedViolationsDetailsReportForFleets_RoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        }
                        //else
                        //{
                        //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                        //        if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsDetailsReportForFleet(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Type, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                        //        {
                        //            //Response.Write("<DIV ID='cache'><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                        //            ReportFailed();
                        //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GeoZoneSummaryReport failed. User:" + sn.UserID.ToString()));
                        //            return;
                        //        }
                        //}
                        break;
                    #endregion

                    #region ExtendedSpeedViolationReportReport_RoadSpeed
                    case "81":
                    case "85":
                        tmp = sn.Report.XmlParams.Split(';');
                        Type = Convert.ToInt32(tmp[0]);
                        ColorFilter = tmp[1] == "" ? "%" : tmp[1] + "%";
                        //Type = Convert.ToInt32(Util.PairFindValue(ReportTemplate.RptParamIncludeSummary,sn.Report.XmlParams));

                        if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0 && sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count > 0)
                        {
                            int[] fleetIds = new int[sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow dr in sn.Misc.DsReportSelectedFleets.Tables[0].Rows)
                            {
                                fleetIds[i] = Convert.ToInt32(dr["FleetId"]);
                                i++;
                            }

                            //if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                            //    if (objUtil.ErrCheck(cr.GetExtendedSpeedViolationsReportForFleets(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))

                            cr.BeginGetExtendedSpeedViolationsReportForFleets_ByRoadSpeed(sn.UserID, sn.SecId, fleetIds, sn.Report.FromDate, sn.Report.ToDate, Type, ColorFilter, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        }

                        break;
                    #endregion

                    # region Trip Details Report with Driver
                    case "82":
                    case "86":
                        if (!sn.Report.IsFleet)
                        {
                            cr.BeginTripDetailsReportDriver(sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, RequestOverflowed, OutMaxOverflowed, ReportPath, wsCallback, asState);
                        }
                        break;
                    # endregion


                    # region Fuel Summary Report
                    case "83":


                        cr.BeginGetActivitySummaryReportPerVehicle_Fuel(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion
                    #region Idling Detail Report New
                    case "87":
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                        cr.BeginGetIdlingDetailsReportNew(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion


                    #region Idling Summary Report New
                    case "88":
                        sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));

                        cr.BeginGetIdlingSummaryReportPerOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion



                    #region Trip Summary Report New
                    case "89":
                        tmp = sn.Report.XmlParams.Split(';');

                        cr.BeginGetTripsSummaryReportNewStructure(sn.UserID, sn.SecId, sn.Report.LicensePlate, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(tmp[0]) ,Convert.ToBoolean(tmp[1]), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;

                    #endregion

                    #region Activity Summary and Green House Gas Report
                    case "92":
                        //sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                        sensorNum = 3;
                        cr.BeginGetActivitySummaryReportPerVehicleGasType(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region Green House Gas Emission Summary Report
                    case "93":
                        //sensorNum = Convert.ToInt16(Util.PairFindValue(ReportTemplate.RptParamSensorId, sn.Report.XmlParams));
                        sensorNum = 3;
                        cr.BeginGetActivitySummaryReportPerVehicleGasTypeSummary(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion

                    #region Maintenance Prev & Next
                    case "94":

                        cr.BeginMaintenanceSpecialReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion


                    # region  Off Road Highrail Mileage
                    case "95":
                        cr.BeginGetHighRailMileageReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion



                    #region Activity Summary Report per Vehicle Special
                    case "96":
                        sensorNum = 3;
                        cr.BeginGetActivitySummaryReportPerVehicle_BadGoodIdling(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sensorNum, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                        break;
                    #endregion


                    #region Speed Violation Summary Report by Road Speed
                    case "97":
                    case "10019":
                        Int16 posted=0;
                        if (sn.Report.XmlParams == "True") posted = 1; else posted = 0;


                        //if (!sn.Report.OrganizationHierarchySelected)
                        //{
                            cr.BeginGetSpeedViolationSummaryReport_RoadSpeed(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, posted, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                        //    cr.BeginGetSpeedViolationSummaryReport_RoadSpeed_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, posted, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    #endregion


                    #region Electronic Invoice-Excel Dump
                    case "98":
                        cr.BeginCNElectronicInvoice(sn.UserID, sn.SecId, sn.Report.ReportFormat, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), ReportPath, wsCallback, asState);

                        break;
                    #endregion

                    #region Maintenance Services
                    case "99":

                        cr.BeginMaintenanceVehicleReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, sn.Report.FleetId, ReportPath, wsCallback, asState);

                        break;
                    #endregion


                    #region Mower Hours Report
                    case "100":
                        cr.BeginGetMowerHoursReport_AciveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion

                    #region Vehicle Start End Odometer/ Eng Hrs Report
                    case "101":
                        cr.BeginVehicleStartEndOdometerEngHrsReport(sn.UserID, sn.SecId, sn.Report.ReportFormat, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), ReportPath, wsCallback, asState);
                        break;
                    #endregion



                    #region Trucks Hours Report
                    case "102":
                        cr.BeginGetTrucksHoursReport_AciveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //cr.BeginGetTrucksHousprsReport_AciveVehicles(sn.UserID, sn.SecId, 4246, sn.Report.FromDate, sn.Report.ToDate, Convert.ToInt16(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion


                    #region Audit Report
                    case "103":
                     System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                     DateTime from = Convert.ToDateTime(sn.Report.FromDate, ci);
                     DateTime to = Convert.ToDateTime(sn.Report.ToDate, ci);
                     string output="";
                     string prefrence = "";
                     if (keyValue.Contains("Multi-Fleet"))
                     {
                         int start = keyValue.IndexOf(":");
                         int stop = keyValue.IndexOf("<b>S");
                         output = keyValue.Substring(start + 1, stop - start - 1);
                         prefrence = "hierarchy";

                     }
                     else
                     {
                         if (sn.Report.OrganizationHierarchyNodeCode != "")
                         {
                             output = sn.Report.FleetId.ToString();
                             prefrence = "hierarchy";
                         }
                         else
                         {
                             output = sn.Report.FleetId.ToString();
                             prefrence = "fleet";
                         }
                     }

                      cr.GetHOSAuditReportForMultiFleets(sn.UserID, sn.SecId, sn.User.OrganizationId, from, to,
                      output, sn.Report.XmlParams, prefrence, 
                      sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath);



                        break;
                    #endregion

                    #region Speed Violation Details Report by Road Speed
                    case "104":
                    case "10015":
                        tmp = sn.Report.XmlParams.Split(';');
                        Int16 PostedSpeedOnly = Convert.ToInt16(tmp[0]);
                        Int32 Delta = Convert.ToInt32(tmp[1]);
                        cr.BeginGetSpeedViolationsDetailsReport_RoadSpeed(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, PostedSpeedOnly, Delta, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion



                    #region Idling Details Driver Report (MTS)
                    case "107":

                        tmp = sn.Report.XmlParams.Split(';');
                        int driverId = Convert.ToInt32(tmp[0]);
                        int IdlingThreshold = Convert.ToInt16(tmp[1]);
                        cr.BeginGetIdlingDriverReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, driverId, IdlingThreshold, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion

                    #region Get User Settings Report
                    case "108":
                        cr.BeginGetUserSettingsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion
                    #region Get Box Settings Report
                    case "109":
                        cr.BeginGetBoxSettingsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion

                    #region Annual Mileage Report
                    case "111":
                        cr.BeginYearOdomEngHoursReport(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion

                    # region User Login Report
                    case "112":
                        cr.BeginUserLoginsReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    # endregion

                    # region Idling Details by Idling
                    case "113":
                          Int16 idlingThreshold = Convert.ToInt16( sn.Report.XmlParams);
                          cr.BeginevtFactEventsByIdling_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), idlingThreshold, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     break;
                    # endregion

                    #region Fuel Consumption Report by Driver
                    case "114":
                        cr.BeginGetTripsSummaryReportNewStructure_ByDriver(sn.UserID, sn.SecId, "0", sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, 3, Convert.ToInt32(sn.Report.XmlParams), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion


                    #region Off Road Report
                    case "115":
                    case "116":

                        //if (!sn.Report.OrganizationHierarchySelected)
                        //{
                            cr.BeginOnOffRoadMiles_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                        //    cr.BeginOnOffRoadMiles_Report_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    #endregion


                    #region Fuel consumption while speeding report
                    case "117":

                        cr.BeginevtViolationFuel_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                            
                        break;
                    #endregion


                    #region Fuel Fraud Report
                    case "118":

                        cr.BeginFuelFraudTransactionReport(sn.UserID, sn.SecId, sn.User.OrganizationId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        break;
                    #endregion


                    #region Speed Distribution Report
                    case "119":
                        //if (!sn.Report.OrganizationHierarchySelected)
                        //{
                            cr.BeginSpeedDistributionReport(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        //else
                        //{
                        //    cr.BeginSpeedDistributionReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //}
                        break;
                    #endregion



               #region Company property speed violation details report
              case "120":
                        //if (!sn.Report.OrganizationHierarchySelected)
                            cr.BeginevtViolationSpeedInLandmark_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                        //else
                         //   cr.BeginevtViolationSpeedInLandmarkHierarchy_Report(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                  break;
              #endregion

                 #region Box Geozones
                 case "121":
                  
                     cr.BeginReportBoxGeozone(sn.UserID, sn.SecId, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     
                     break;
                 #endregion

                 #region Driver Scores by Fleet
                 case "123":
                     //cr.BeginevtDriverViolationsFleet_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     cr.BeginevtDriverViolationsFleet_Report(sn.UserID, sn.SecId, sn.Report.FleetId, Convert.ToDateTime(sn.Report.FromDate), Convert.ToDateTime(sn.Report.ToDate), sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     break;
                 #endregion

                 #region Worksite Activity Snow Report by Media Type
                 case "124":
                     tmp = sn.Report.XmlParams.Split(';');
                     Int16 ActiveVehicles = Convert.ToInt16(tmp[0]);
                     Int16 MediaTypeId = Convert.ToInt16(tmp[1]);

                     cr.BeginGetActivityAtLandmarkSummaryReportPerFleetSnow_MediaType_ActiveVehicles(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ActiveVehicles, MediaTypeId,  ReportPath, wsCallback, asState);
                     
                     break;
                 #endregion

                 case "127":

                    // tmp = sn.Report.XmlParams.Split('|');

                     //cr.BeginViolationReport_NewSafetyMatrix (sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, tmp[0], sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     cr.BeginViolationReport_NewSafetyMatrix(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     //}
                     //}
                     //else
                     //{
                     //    cr.BeginViolationReport_Hierarchy(sn.UserID, sn.SecId, sn.Report.OrganizationHierarchyNodeCode, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     //}
                     break;


                 case "130":
                     //if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), false))
                     //   if (objUtil.ErrCheck(cr.ViolationReportWithScore(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath), true))
                     //   {
                     //      //Response.Write("<DIV ID='cache' ><TABLE style='Z-INDEX: 101; LEFT: 200px; POSITION: absolute; TOP: 200px' WIDTH=400 BGCOLOR=#000000 BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE='Arial, Verdana' SIZE=4><B><BR>" + Resources.Const.Reports_LoadFailed + "<BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>");
                     //       ReportFailed();
                     //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ViolationSummary failed. User:" + sn.UserID.ToString()));
                     //      return;
                     //   }

                    // tmp = sn.Report.XmlParams.Split('|');


                     cr.BeginViolationReportWithScore_NewSafetyMatrix(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     break;


                 # region Landmark summary with Driver
                 case "131":
                     if (sn.Report.IsFleet)
                     {
                         cr.BeginLandmarkFleetReport_Driver (sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                         //cr.LandmarkFleetReport_Driver(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,ref ReportPath);
                     }
                     else
                     {
                         cr.BeginLandmarkVehicleReport_Driver (sn.UserID, sn.SecId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);
                     }
                     break;
                 # endregion



                 case "132":


                     cr.BeginViolationReport_NewSafetyMatrix_WithoutRailData(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                    // cr.ViolationReport_NewSafetyMatrix_WithoutRailData (sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath);

                     break;


                 case "133":

                     cr.BeginViolationReportWithScore_NewSafetyMatrix_WithoutRailData(sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ReportPath, wsCallback, asState);

                     // cr.ViolationReportWithScore_NewSafetyMatrix_WithoutRailData (sn.UserID, sn.SecId, sn.Report.FleetId, sn.Report.FromDate, sn.Report.ToDate, sn.Report.XmlParams, sn.Report.ReportFormat, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref ReportPath);
                     break;

            }
                       


            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "ReportViewer failed. Ex:" + ex.Message));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                if (basePage != null) basePage.ExceptionLogger(trace);
                ret = false;
            }
            return ret;
        }
        private void wsCallback(IAsyncResult iResult)
        {


        }

        /// <summary>
        /// This method is to replace ';' and '=' with special string for value, because ';' and '=' are seperator
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string MakePair(string key, string val)
        {
            if (val == null) val = "";
            string ret = string.Format("{0}{1}{2}{3}", Seperator, key, KeySeperator, val);
            return ret;
        }

        public static string PairFindValue(string key, string src)
        {
            string[] sArray = Regex.Split(src, Seperator);
            string val = string.Empty;
            key = key + KeySeperator;
            if (sArray != null)
            {
                foreach (string str in sArray)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str.StartsWith(key))
                        {
                            if (str.Length > key.Length) val = str.Substring(key.Length);
                            break;
                        }
                    }
                }
            }
            return val;
        }

        public static string GetResourceObject(string pathFile, string key)
        {
            string value = "";
            try
            {
                if (HttpContext.Current.Session["SentinelFMSession"] != null)
                {
                    SentinelFMSession sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];

                    if (sn.SelectedLanguage.ToLower() == "fr-ca")
                        pathFile = pathFile + ".fr.resx";
                    else pathFile = pathFile + ".resx";
                }
                else
                {
                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                        pathFile = pathFile + ".fr.resx";
                    else pathFile = pathFile + ".resx";
                }

                XmlDocument resource = new XmlDocument();
                resource.Load(pathFile);
                XmlNode sValue = resource.SelectSingleNode("root/data[@name='" + key + "']/value");
                if (sValue != null)
                {
                    if (sValue != null)
                    {
                        value = sValue.InnerText;
                    }
                }
            }
            catch
            { }
            return value;
        }

        public static string GetOperationTypeStr_1(DataSet operationsData)
        {
            string operationTypeStr = string.Empty;
            if (operationsData != null && operationsData.Tables.Count > 0 && operationsData.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in operationsData.Tables[0].Rows)
                {
                    if (dr["id"] != DBNull.Value && dr["description"] != DBNull.Value)
                    {
                        string caseStr = string.Format(" When {0} Then '{1}' ", dr["id"].ToString().Replace("'", "''"), dr["description"].ToString().Replace("'", "''"));
                        operationTypeStr = operationTypeStr + caseStr;
                    }
                }
                if (operationTypeStr != string.Empty) operationTypeStr = "case OperationTypeID " + operationTypeStr + " end as OperationType ";
            }
            return operationTypeStr;
        }

        public static string GetOperationTypeStr()
        {
            string path = HttpContext.Current.Server.MapPath("~/Maintenance/App_LocalResources/frmMaintenanceNew.aspx");
            string OperationTypefileName = clsAsynGenerateReport.GetResourceObject(path, "XmlDSOperationType_DataFile");
            DataSet operationsData = new DataSet();
            operationsData.ReadXml(HttpContext.Current.Server.MapPath(OperationTypefileName));
            return clsAsynGenerateReport.GetOperationTypeStr_1(operationsData);
        }

        public static DataTable GetOperationTypeTable()
        {
            string path = HttpContext.Current.Server.MapPath("~/Maintenance/App_LocalResources/frmMaintenanceNew.aspx");
            string OperationTypefileName = clsAsynGenerateReport.GetResourceObject(path, "XmlDSOperationType_DataFile");
            DataSet operationsData = new DataSet();
            operationsData.ReadXml(HttpContext.Current.Server.MapPath(OperationTypefileName));
            if (operationsData != null) return operationsData.Tables[0];
            else return null;
        }


     

    }
}
