using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.IO;
using System.Data;
using System.Configuration;
using VLF.DAS.Logic;
using VLF.PATCH.Logic;

namespace SentinelFM
{
    public partial class NewVehicleList : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        public string alarmDetailPage = "./Map/frmAlarmInfo.aspx";
        public string messageDetailPage = "./Map/frmMessageInfo.aspx";
        public int windowWidth = 400;
        public int windowHeight = 550;
        public bool mapAssets = false;
        public int maxVehiclesOnMap = 500;
        public bool VehicleClustering = false;
        public int VehicleClusteringDistance = 20;
        public int VehicleClusteringThreshold = 5;
        public bool ifShowTheme1 = false;
        public bool ifShowTheme2 = false;
        public bool ifShowArcgis = false;
        public bool ifShowGoogleStreets = false;
        public bool ifShowGoogleHybrid = false;
        public bool ifShowBingRoads = false;
        public bool ifShowBingHybrid = false;
        public string overlays = string.Empty;
        public string defaultMapView = "north";
        public bool showDriverFinderButton = false;
        public bool showDriverColumn = false;
        public bool ShowOrganizationHierarchy;
        public string DefaultOrganizationHierarchyFleetId = "0";
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        public string DefaultFleetName = string.Empty;

        public bool ShowAlarmTab = true;
        public bool ShowMessageTab = true;
        public bool ShowRouteAssignment = false;
        public bool ShowScheduleAdherence = false;
        public bool ShowDashboardView = false;

        public bool IE8orUnder = false;
        public int PageSize = 10000;
        public int HistoryPageSize = 200;
        //public int PageSize = 10;

        public string ExtjsVersion = "extjs-4.1.0";
        //public string ExtjsVersion = "ext-4.2.0.663";

        public string LastUpdatedVehicleListJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/VehicleList.js")).ToString("yyyyMMddHHmmss");
        public bool HistoryEnabled = true;
        public bool HGI = false;
        public bool ShowHistoryDetails = false;
        public bool ShowEngineHours = false;
        public bool ShowOdometer = false;
        public bool ShowPTO = false;
        private int IE8VehicleGridPagesize = 100;
        private int IE8HistoryGridPagesize = 100;
	    public bool MutipleUserHierarchyAssignment;
        public bool BatteryTredingEnabled = false;

        public string CategoryList = "[]";

        public string ResVehicles;
        public string ResAlarms;
        public string ResMessages;
        public string ResGeozoneLandmarks;
        public string ResDispatch;
        public string ResVehiclePagerDisplayMsg;
        public string ResVehiclePagerEmptyMsg;

        public string ResUpdatePositionCommandStatus;
        public string ResGeozones;
        public string ResLandmarks;
        public string ResLocation;
        public string ResTrip;
        public string ResHistory;
        public string ResSearchResult;
        public string ResTrackVehicles;
        public string ResSearch;
        public string ResMaptheselectedvehicle;
        public string ResTrackSelectedVehicleOnSeparateMap;
        public string ResGoogleStreetView;
        public string ResMapSelectedVehicleOnMap;
        public string ResRefreshTheMapAndGridAutomatically;
        public string ResVehicleGeozoneAssignment;
        public string ResSearchHistory;
        public string ResMapTheSelectedHistoryRecords;
        public string ResSendCMDHistoryRecords;
        public string ResSelectVehicles;
        public string ResSelectCommMode;
        public string ResClearAllSelectedVehicles;
        public string ResExport;
        public string ResMapItButMapTheSelectedVehicle;
        public string ResUpdatePositionButtonToolTip;
        public string ResWantToImproveOurMap;
        public string ResClearAllButtonToolTip;
        public string ResFindVehiclesDrivers;
        public string ResLegendDateTimeColor;
        public string ResOrganizationHierarchy;
        public string ResSelectFleet;
        public string ResHistoryOrganizationHierarchyButtonToolTip;
        public string ResHistoryFleetButtonSelectfleet;
        public string ResLabelonoffButtonHideShowLabel;
        public string ResClearSearchBtnHideShowLabel;
        public string ResSendmessageButtonSendMessage;
        public string ResStreetViewButtonGoogleStreetView;
        public string ResTrackitButtonToolTip;
        public string ResDemomapButtonToolTip;

        public string ResClearAllButtonText;
        public string ResFindVehiclesDriversButtonText;
        public string ResLegendButtonText;
        //Added by Rohit Mittal for Trip Colors
        public string ResHisTripColorsButtonText;
        public string ResHisTripColors;
        public bool ShowMultiColor = false;
        public string ResClearSearchBtnText;
        public string ResSendmessageButtonText;
        public string ResStreetViewButtonText;
        public string ResStreetviewAlertMessage;
        public string ResStreetviewAlertTitle;
        public string ResTrackitButtonText;
        public string ResDemomapButtonText;
        public string ResSearchMapButtonText;
        public string ResAlarmsPagerdisplayMsg;
        public string ResAlarmsPageremptyMsg;
        public string ResMessagesPagerdisplayMsg;
        public string ResMessagesPageremptyMsg;
        public string ResHistoryPagerdisplayMsg;
        public string ResHistoryPageremptyMsg;
        public string ResHistoryPageralert;
        public string ResHistorygridalertMessage;
        public string ResHistorygridalertTitle;
        public string ResFinditmenuText;
        public string ResTrackitmenuText;
        public string ResStreetViewMenuText;
        public string ResStreetViewMenualertMessage;
        public string ResStreetViewMenualertTitle;
        public string ResUpdatePositionMenuText;
        public string ResClearAllMenuText;
        public string ResExportToCsvButtonText;
        public string ResExportToExcel2003ButtonText;
        public string ResExportToExcel2007ButtonText;
        public string ResExportHistoryToCsvButtonText;
        public string ResExportHistoryToExcel2003ButtonText;
        public string ResExportHistoryToExcel2007ButtonText;
        public string ResExportHistoryStopToCsvButtonText;
        public string ResExportHistoryStopToExcel2003ButtonText;
        public string ResExportHistoryStopToExcel2007ButtonText;
        public string ResExportHistoryTripToCsvButtonText;
        public string ResExportHistoryTripToExcel2003ButtonText;
        public string ResExportHistoryTripToExcel2007ButtonText;
        public string ResExportHisotryAddressToCsvButtonText;
        public string ResExportHistoryAddressToExcel2003ButtonText;
        public string ResExportHistoryAddressToExcel2007ButtonText;
        public string ResvgUnitIDText;
        public string ResvgDriverText;
        public string ResvgDescriptionText;
        public string ResvgStatusText;
        public string ResvgSpeedText;
        public string ResvgDateTimeText;
        public string ResvgAddressText;
        public string ResvgArmedText;
        public string ResvgDriverCardNumberText;
        public string ResvgField1Text;
        public string ResvgField2Text;
        public string ResvgField3Text;
        public string ResvgField4Text;
        public string ResvgField5Text;
        public string ResvgModelYearText;
        public string ResvgMakeNameText;
        public string ResvgModelNameText;
        public string ResvgVehicleTypeNameText;
        public string ResvgLicensePlateText;
        public string ResvgVinNumText;
        public string ResvgManagerNameText;
        public string ResvgManagerEmployeeIdText;
        public string ResvgStateProvinceText;
        public string ResvgHistoryText;
        public string ResvgEngineHoursText;
        public string ResvgOdometerText;
        public string ResvgIsRouteAssignedText;
        public string ResdockedItemsActionsText;
        public string ResdockedItemsExportText;
        public string ResalarmgridEmptyText;
        public string ResalarmgridcolumnsAlarmId;
        public string ResalarmgridcolumnsTimeCreated;
        public string ResalarmgridcolumnsAlarmLevel;
        public string ResalarmgridcolumnsAlarmDescription;
        public string ResalarmgridcolumnsvehicleDescription;
        public string ResmessagegridemptyText;
        public string ResmessagegridcolumnsMessageId;
        public string ResmessagegridcolumnsMsgDateTime;
        public string ResmessagegridcolumnsDescription;
        public string ResmessagegridcolumnsMsgBody;
        public string ResmessagegridcolumnsAcknowledged;
        public string ResgeolandmarkgridemptyText;
        public string ResvehiclegeozoneassignmentButtonText;
        public string ResgeozonegridemptyText;
        public string ResgeozonegridcolumnsGeozone;
        public string Resgeozonegridcolumnsdesc;
        public string ResgeozonegridcolumnsDirection;
        public string ResgeozonegridcolumnsSeverityName;
        public string ReslandmarkgridemptyText;
        public string Reslandmarkgridname;
        public string Reslandmarkgriddesc;
        public string ReslandmarkgridStreetAddress;
        public string ReslandmarkgridEmail;
        public string ReslandmarkgridContactPhoneNum;
        public string Reslandmarkgridradius;
        public string ReslandmarkgridCategoryName;
        public string ResbtmHistorySearchText;
        public string ResbtnHistoryMapitText;
        public string ResbtnHistoryLegendText;
        public string ResbtnHistoryMapAllText;
        public string ResbtnHistorySendCMDText;
        public string ReshistoryTypeemptyText;
        public string ResHistoryVehicleListDescription1;
        public string ResHistoryVehicleListDescription2;
        public string ReshistoryVehiclesdisplayField;
        public string ReshistoryVehiclesfieldLabel;
        public string ReshistoryVehiclesEmptyText;
        public string ReshistoryCommModesEmptyText;
        public string ReshistoryMessageCheckBoxboxLabel;
        public string ReshistoryTripRadioshtml;
        public string ReshistoryTripRadiosboxLabel1;
        public string ReshistoryTripRadiosboxLabel2;
        public string ReshistoryTripRadiosboxLabel3;
        public string ResbtnSubmitMessage;
        public string ResbtnSubmitAlert;
        public string ReshistorygridEmptyText;
        public string ReshistorygridBoxId;
        public string ReshistorygridDescription;
        public string ReshistorygridOriginDateTime;
        public string ReshistorygridStreetAddress;
        public string ReshistorygridSpeed;
        public string ReshistorygridBoxMsgInTypeName;
        public string ReshistorygridMsgDetails;
        public string ResAcknowledged;
        public string ReshistoryStopGridEmptyText;
        public string ReshistoryStopGridArrivalDateTime;
        public string ReshistoryStopGridLocation;
        public string ReshistoryStopGridDepartureDateTime;
        public string ReshistoryStopGridStopDuration;
        public string ReshistoryStopGridRemarks;
        public string ReshistoryTripGridEmptyText;
        public string ReshistoryTripGridDescription;
        public string ReshistoryTripGridDepartureTime;
        public string ReshistoryTripGridArrivalTime;
        public string ReshistoryTripGrid_From;
        public string ReshistoryTripGrid_To;
        public string ReshistoryTripGridDuration;
        public string ReshistoryTripGridFuelConsumed;
        public string ReshistoryAddressGridhaUnitID;
        public string ReshistoryAddressGridhaDescription;
        public string ReshistoryAddressGridhaDateTime;
        public string ReshistoryAddressGridhaDetails;
        public string ReshidtoryDetailsGridEmptyText;
        public string ReshidtoryDetailsGridBoxId;
        public string ReshidtoryDetailsGridDescription;
        public string ReshidtoryDetailsGridOriginDateTime;
        public string ReshidtoryDetailsGridStreetAddress;
        public string ReshidtoryDetailsGridSpeed;
        public string ReshidtoryDetailsGridBoxMsgInTypeName;
        public string ReshidtoryDetailsGridMsgDetails;
        public string ReshidtoryDetailsGridAcknowledged;
        public string ReshistoryFormEmptyText;

        public string ResfleetDefaultText;
        public string ResloadingMaskMessage;
        public string RessearchingMaskMessage;
        public string ResmapitButtonText;
        public string ResupdatePositionButtonText;
        public string ResFeedbackButtonText;
        public string ResfleetButtonOpenwindowMessage;
        public string ReshistoryFleetButtonOpenwindowMessage;
        public string ReslabelonoffButtonlabelonoffShowLabel;
        public string ReslabelonoffButtonlabelonoffHideLabel;
        public string ResstreetViewButtonOpenwindowMessage;
        public string RestrackitButtonOpenwindowMessage;
        public string ReshistorygridemptyText2;
        public string RestrackitmenuOpenwindowMessage;
        public string ResstreetViewMenuOpenwindowMessage;
        public string Resgeolandmarkgridcolumnsname;
        public string ResgeolandmarkgridcolumnsType;
        public string ResgeolandmarkgridsetTitle;
        public string ReshistoryDateFromfieldLabel;
        public string ReshistoryDateTofieldLabel;
        public string ReshistoryTypefieldLabel;
        public string ResbtnSubmitAlertTitle;
        public string ReshistoryFleetButtonsetText;
        public string ReshistoryFormAlertTitle;
        public string ReshistoryFormAlertMessage;
        public string ResAllRoutesBtnText;
        public string ResSubmitButtonText;
        public string ResHistoryLinkText;
        public string ResHistoryLocationText;
        public string ResHistoryTypeValues_0;
        public string ResHistoryTypeValues_1;
        public string ResHistoryTypeValues_2;
        public string ResHistoryTypeValues_3;
        public string ResHistoryTypeValues_4;
        public string ResFilterText;
        public string ResbtnVehicleonoffShowDetailsText;
        public string ResbtnVehicleonoffHideDetailsText;
        public string ResbtnVehicleonoffTooltipText;
        public string ResTxtButtonTitleFleetText;
        public string ResTxtButtonTitleHierarchyText;
        public string ResHistoryCommModeNameText;
        public string ResRouteAssignedYes;
        public string ResRouteAssignedNo;
        public string ResgeozonegridcolumnsCurrentAssignment;
        public string ResMultipleHierarchy;
        public string Res_cvColorText;
        public string ResHistoryReplayReplay;
        public string ResHistoryReplayStop;
        public string ResHistoryReplayTooltip;
        public string ResAddressresolutioninprogress;
        public string ResResolveAddressToolTips;
        public string ResResolving;
        public string ResValidGPS;
        public string ResTextTrue;
        public string ResTextFalse;
        public string ResTextNA;

        //public int mapRefreshFrequency = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;

                //if (sn.UserName.ToLower().Contains("g4s"))
                //{
                //alarmDetailPage = "./Map/frmAlarmInfo_G4S.aspx";
                alarmDetailPage = "./Map/AlarmDetails2.aspx";
                windowWidth = 530;
                windowHeight = 320;
                //}

                ResVehicles = GetLocalResourceObject("Vehicles").ToString();
                ResAlarms = GetLocalResourceObject("Alarms").ToString();
                ResMessages = GetLocalResourceObject("Messages").ToString();
                ResGeozoneLandmarks = GetLocalResourceObject("GeozoneLandmarks").ToString();
                ResDispatch = GetLocalResourceObject("Dispatch").ToString();
                ResVehiclePagerDisplayMsg = GetLocalResourceObject("VehiclePagerDisplayMsg").ToString();
                ResVehiclePagerEmptyMsg = GetLocalResourceObject("VehiclePagerEmptyMsg").ToString();

                ResUpdatePositionCommandStatus = GetLocalResourceObject("UpdatePositionReceived.MessageBox.Title").ToString();
                ResGeozones = GetLocalResourceObject("GeoZoneGrid.Title").ToString();
                ResLandmarks = GetLocalResourceObject("LandmarkGrid.Title").ToString();
                ResLocation = GetLocalResourceObject("HistoryLoactionForm.Title").ToString();
                ResTrip = GetLocalResourceObject("HistoryTripRadios.Title").ToString();
                ResHistory = GetLocalResourceObject("HistoryGridForm.Title").ToString();
                ResSearchResult = GetLocalResourceObject("HistoryAddressGrid.Title").ToString();
                ResTrackVehicles = GetLocalResourceObject("TrackVehicles.Title").ToString();
                ResSearch = GetLocalResourceObject("SearchMapButton.ToolTip").ToString();
                ResMaptheselectedvehicle = GetLocalResourceObject("FindItMenu.ToolTip").ToString();
                ResTrackSelectedVehicleOnSeparateMap = GetLocalResourceObject("TrackTtMenu.ToolTip").ToString();
                ResGoogleStreetView = GetLocalResourceObject("StreetViewMenu.ToolTip").ToString();
                ResMapSelectedVehicleOnMap = GetLocalResourceObject("UpdatePositionMenu.ToolTip").ToString();
                ResRefreshTheMapAndGridAutomatically = GetLocalResourceObject("AutoSync.ToolTip").ToString();
                ResVehicleGeozoneAssignment = GetLocalResourceObject("VehicleGeozoneAssignmentButton.ToolTip").ToString();
                ResSearchHistory = GetLocalResourceObject("btnHistorySearch.Tooltip").ToString();
                ResMapTheSelectedHistoryRecords = GetLocalResourceObject("btnHistoryMapit.ToolTip").ToString();
                ResSendCMDHistoryRecords = GetLocalResourceObject("btnHistorySendCMD.ToolTip").ToString();
                ResSelectVehicles = GetLocalResourceObject("ComboBox.ToolTip").ToString();
                ResSelectCommMode = GetLocalResourceObject("HistoryComboBox.ToolTip").ToString();
                ResClearAllSelectedVehicles = GetLocalResourceObject("ClearAllMenu.ToolTip").ToString();
                ResExport = GetLocalResourceObject("ExportToCsvButton.ToolTip").ToString();
                ResMapItButMapTheSelectedVehicle = GetLocalResourceObject("MapitButton.ToolTip").ToString();
                ResUpdatePositionButtonToolTip = GetLocalResourceObject("UpdatePositionButton.ToolTip").ToString();
                ResWantToImproveOurMap = GetLocalResourceObject("FeedbackButton.ToolTip").ToString();
                ResClearAllButtonToolTip = GetLocalResourceObject("ClearAllButton.ToolTip").ToString();
                ResFindVehiclesDrivers = GetLocalResourceObject("FindVehiclesDriversButton.ToolTip").ToString();
                ResLegendDateTimeColor = GetLocalResourceObject("LegendButton.ToolTip").ToString();
                ResOrganizationHierarchy = GetLocalResourceObject("OrganizationHierarchyButton.ToolTip").ToString();
                ResSelectFleet = GetLocalResourceObject("FleetButton.ToolTip").ToString();
                ResHistoryOrganizationHierarchyButtonToolTip = GetLocalResourceObject("HistoryOrganizationHierarchyButton.ToolTip").ToString();
                ResHistoryFleetButtonSelectfleet = GetLocalResourceObject("HistoryFleetButton.ToolTip").ToString();
                ResLabelonoffButtonHideShowLabel = GetLocalResourceObject("LabelonoffButton.ToolTip").ToString();
                ResClearSearchBtnHideShowLabel = GetLocalResourceObject("ClearSearchBtn.ToolTip").ToString();
                ResSendmessageButtonSendMessage = GetLocalResourceObject("SendmessageButton.ToolTip").ToString();
                ResStreetViewButtonGoogleStreetView = GetLocalResourceObject("StreetViewButton.ToolTip").ToString();
                ResTrackitButtonToolTip = GetLocalResourceObject("TrackitButton.ToolTip").ToString();
                ResDemomapButtonToolTip = GetLocalResourceObject("DemomapButton.ToolTip").ToString();

                ResClearAllButtonText = GetLocalResourceObject("ClearAllButton.Text").ToString();
                ResFindVehiclesDriversButtonText = GetLocalResourceObject("FindVehiclesDriversButton.Text").ToString();
                ResLegendButtonText = GetLocalResourceObject("LegendButton.Text").ToString();
                //Added by Rohit Mittal for Trip Colors
                ResHisTripColorsButtonText = GetLocalResourceObject("HisTripColorsButton.Text").ToString();
                ResHisTripColors = GetLocalResourceObject("HisTripColors.Text").ToString();
                ResClearSearchBtnText = GetLocalResourceObject("ClearSearchBtn.Text").ToString();
                ResSendmessageButtonText = GetLocalResourceObject("SendmessageButton.Text").ToString();
                ResStreetViewButtonText = GetLocalResourceObject("StreetViewButton.Text").ToString();
                ResStreetviewAlertMessage = GetLocalResourceObject("Streetview.alert.Message").ToString();
                ResStreetviewAlertTitle = GetLocalResourceObject("Streetview.alert.Title").ToString();
                ResTrackitButtonText = GetLocalResourceObject("TrackitButton.Text").ToString();
                ResDemomapButtonText = GetLocalResourceObject("DemomapButton.Text").ToString();
                ResSearchMapButtonText = GetLocalResourceObject("SearchMapButton.Text").ToString();
                ResAlarmsPagerdisplayMsg = GetLocalResourceObject("alarmsPager.displayMsg").ToString();
                ResAlarmsPageremptyMsg = GetLocalResourceObject("alarmsPager.emptyMsg").ToString();
                ResMessagesPagerdisplayMsg = GetLocalResourceObject("messagesPager.displayMsg").ToString();
                ResMessagesPageremptyMsg = GetLocalResourceObject("messagesPager.emptyMsg").ToString();
                ResHistoryPagerdisplayMsg = GetLocalResourceObject("historyPager.displayMsg").ToString();
                ResHistoryPageremptyMsg = GetLocalResourceObject("historyPager.emptyMsg").ToString();
                ResHistoryPageralert = GetLocalResourceObject("historyPager.alert").ToString();
                ResHistorygridalertMessage = GetLocalResourceObject("historygrid.alert.Message").ToString();
                ResHistorygridalertTitle = GetLocalResourceObject("historygrid.alert.Title").ToString();
                ResFinditmenuText = GetLocalResourceObject("Finditmenu.Text").ToString();
                ResTrackitmenuText = GetLocalResourceObject("Trackitmenu.Text").ToString();
                ResStreetViewMenuText = GetLocalResourceObject("StreetViewMenu.Text").ToString();
                ResStreetViewMenualertMessage = GetLocalResourceObject("streetViewMenu.alert.Message").ToString();
                ResStreetViewMenualertTitle = GetLocalResourceObject("streetViewMenu.alert.Title").ToString();
                ResUpdatePositionMenuText = GetLocalResourceObject("UpdatePositionMenu.Text").ToString();
                ResClearAllMenuText = GetLocalResourceObject("ClearAllMenu.Text").ToString();
                ResExportToCsvButtonText = GetLocalResourceObject("ExportToCsvButton.Text").ToString();
                ResExportToExcel2003ButtonText = GetLocalResourceObject("ExportToExcel2003Button.Text").ToString();
                ResExportToExcel2007ButtonText = GetLocalResourceObject("ExportToExcel2007Button.Text").ToString();
                ResExportHistoryToCsvButtonText = GetLocalResourceObject("ExportHistoryToCsvButton.Text").ToString();
                ResExportHistoryToExcel2003ButtonText = GetLocalResourceObject("ExportHistoryToExcel2003Button.Text").ToString();
                ResExportHistoryToExcel2007ButtonText = GetLocalResourceObject("ExportHistoryToExcel2007Button.Text").ToString();
                ResExportHistoryStopToCsvButtonText = GetLocalResourceObject("ExportHistoryStopToCsvButton.Text").ToString();
                ResExportHistoryStopToExcel2003ButtonText = GetLocalResourceObject("exportHistoryStopToExcel2003Button.Text").ToString();
                ResExportHistoryStopToExcel2007ButtonText = GetLocalResourceObject("exportHistoryStopToExcel2007Button.Text").ToString();
                ResExportHistoryTripToCsvButtonText = GetLocalResourceObject("exportHistoryTripToCsvButton.Text").ToString();
                ResExportHistoryTripToExcel2003ButtonText = GetLocalResourceObject("exportHistoryTripToExcel2003Button.Text").ToString();
                ResExportHistoryTripToExcel2007ButtonText = GetLocalResourceObject("exportHistoryTripToExcel2007Button.Text").ToString();
                ResExportHisotryAddressToCsvButtonText = GetLocalResourceObject("exportHisotryAddressToCsvButton.Text").ToString();
                ResExportHistoryAddressToExcel2003ButtonText = GetLocalResourceObject("exportHistoryAddressToExcel2003Button.Text").ToString();
                ResExportHistoryAddressToExcel2007ButtonText = GetLocalResourceObject("exportHistoryAddressToExcel2007Button.Text").ToString();
                ResvgUnitIDText = GetLocalResourceObject("vgUnitID.Text").ToString();
                ResvgDriverText = GetLocalResourceObject("vgDriver.Text").ToString();
                ResvgDescriptionText = GetLocalResourceObject("vgDescription.Text").ToString();
                ResvgStatusText = GetLocalResourceObject("vgStatus.Text").ToString();
                ResvgSpeedText = GetLocalResourceObject("vgSpeed.Text").ToString();
                ResvgDateTimeText = GetLocalResourceObject("vgDateTime.Text").ToString();
                ResvgAddressText = GetLocalResourceObject("vgAddress.Text").ToString();
                ResvgArmedText = GetLocalResourceObject("vgArmed.Text").ToString();
                ResvgDriverCardNumberText = GetLocalResourceObject("vgDriverCardNumber.Text").ToString();
                ResvgField1Text = GetLocalResourceObject("vgField1.Text").ToString();
                ResvgField2Text = GetLocalResourceObject("vgField2.Text").ToString();
                ResvgField3Text = GetLocalResourceObject("vgField3.Text").ToString();
                ResvgField4Text = GetLocalResourceObject("vgField4.Text").ToString();
                ResvgField5Text = GetLocalResourceObject("vgField5.Text").ToString();
                ResvgModelYearText = GetLocalResourceObject("vgModelYear.Text").ToString();
                ResvgMakeNameText = GetLocalResourceObject("vgMakeName.Text").ToString();
                ResvgModelNameText = GetLocalResourceObject("vgModelName.Text").ToString();
                ResvgVehicleTypeNameText = GetLocalResourceObject("vgVehicleTypeName.Text").ToString();
                ResvgLicensePlateText = GetLocalResourceObject("vgLicensePlate.Text").ToString();
                ResvgVinNumText = GetLocalResourceObject("vgVinNum.Text").ToString();
                ResvgManagerNameText = GetLocalResourceObject("vgManagerName.Text").ToString();
                ResvgManagerEmployeeIdText = GetLocalResourceObject("vgManagerEmployeeId.Text").ToString();
                ResvgStateProvinceText = GetLocalResourceObject("vgStateProvince.Text").ToString();
                ResvgHistoryText = GetLocalResourceObject("vgHistory.Text").ToString();
                ResvgEngineHoursText = GetLocalResourceObject("vgEngineHours.Text").ToString();
                ResvgOdometerText = GetLocalResourceObject("vgOdometer.Text").ToString();
                ResvgIsRouteAssignedText = GetLocalResourceObject("vgIsRouteAssigned.Text").ToString();
                ResdockedItemsActionsText = GetLocalResourceObject("dockedItems.Actions.Text").ToString();
                ResdockedItemsExportText = GetLocalResourceObject("dockedItems.Export.Text").ToString();
                ResalarmgridEmptyText = GetLocalResourceObject("alarmgrid.EmptyText").ToString();
                ResalarmgridcolumnsAlarmId = GetLocalResourceObject("alarmgrid.columns.AlarmId").ToString();
                ResalarmgridcolumnsTimeCreated = GetLocalResourceObject("alarmgrid.columns.TimeCreated").ToString();
                ResalarmgridcolumnsAlarmLevel = GetLocalResourceObject("alarmgrid.columns.AlarmLevel").ToString();
                ResalarmgridcolumnsAlarmDescription = GetLocalResourceObject("alarmgrid.columns.AlarmDescription").ToString();
                ResalarmgridcolumnsvehicleDescription = GetLocalResourceObject("alarmgrid.columns.vehicleDescription").ToString();
                ResmessagegridemptyText = GetLocalResourceObject("messagegrid.emptyText").ToString();
                ResmessagegridcolumnsMessageId = GetLocalResourceObject("messagegrid.columns.MessageId").ToString();
                ResmessagegridcolumnsMsgDateTime = GetLocalResourceObject("messagegrid.columns.MsgDateTime").ToString();
                ResmessagegridcolumnsDescription = GetLocalResourceObject("messagegrid.columns.Description").ToString();
                ResmessagegridcolumnsMsgBody = GetLocalResourceObject("messagegrid.columns.MsgBody").ToString();
                ResmessagegridcolumnsAcknowledged = GetLocalResourceObject("messagegrid.columns.Acknowledged").ToString();
                ResgeolandmarkgridemptyText = GetLocalResourceObject("geolandmarkgrid.emptyText").ToString();
                ResvehiclegeozoneassignmentButtonText = GetLocalResourceObject("vehiclegeozoneassignmentButton.Text").ToString();
                ResgeozonegridemptyText = GetLocalResourceObject("geozonegrid.emptyText").ToString();
                ResgeozonegridcolumnsGeozone = GetLocalResourceObject("geozonegrid.columns.Geozone").ToString();
                Resgeozonegridcolumnsdesc = GetLocalResourceObject("geozonegrid.columns.desc").ToString();
                ResgeozonegridcolumnsDirection = GetLocalResourceObject("geozonegrid.columns.Direction").ToString();
                ResgeozonegridcolumnsSeverityName = GetLocalResourceObject("geozonegrid.columns.SeverityName").ToString();
                ReslandmarkgridemptyText = GetLocalResourceObject("landmarkgrid.emptyText").ToString();
                Reslandmarkgridname = GetLocalResourceObject("landmarkgrid.name").ToString();
                Reslandmarkgriddesc = GetLocalResourceObject("landmarkgrid.desc").ToString();
                ReslandmarkgridStreetAddress = GetLocalResourceObject("landmarkgrid.StreetAddress").ToString();
                ReslandmarkgridEmail = GetLocalResourceObject("landmarkgrid.Email").ToString();
                ReslandmarkgridContactPhoneNum = GetLocalResourceObject("landmarkgrid.ContactPhoneNum").ToString();
                Reslandmarkgridradius = GetLocalResourceObject("landmarkgrid.radius").ToString();
                ReslandmarkgridCategoryName = GetLocalResourceObject("landmarkgrid.CategoryName").ToString();
                ResbtmHistorySearchText = GetLocalResourceObject("btmHistorySearch.Text").ToString();
                ResbtnHistoryMapitText = GetLocalResourceObject("btnHistoryMapit.Text").ToString();
                ResbtnHistoryLegendText = GetLocalResourceObject("btnHistoryLegend.Text").ToString();
                ResbtnHistoryMapAllText = GetLocalResourceObject("btnHistoryMapAll.Text").ToString();
                ResbtnHistorySendCMDText = GetLocalResourceObject("btnHistorySendCMD.Text").ToString();
                ReshistoryTypeemptyText = GetLocalResourceObject("historyType.emptyText").ToString();
                ResHistoryVehicleListDescription1 = GetLocalResourceObject("HistoryVehicleList.Description1").ToString();
                ResHistoryVehicleListDescription2 = GetLocalResourceObject("HistoryVehicleList.Description2").ToString();
                ReshistoryVehiclesdisplayField = GetLocalResourceObject("historyVehicles.displayField").ToString();
                ReshistoryVehiclesfieldLabel = GetLocalResourceObject("historyVehicles.fieldLabel").ToString();
                ReshistoryVehiclesEmptyText = GetLocalResourceObject("historyVehicles.EmptyText").ToString();
                ReshistoryCommModesEmptyText = GetLocalResourceObject("historyCommModes.EmptyText").ToString();
                ReshistoryMessageCheckBoxboxLabel = GetLocalResourceObject("historyMessageCheckBox.boxLabel").ToString();
                ReshistoryTripRadioshtml = GetLocalResourceObject("historyTripRadios.html").ToString();
                ReshistoryTripRadiosboxLabel1 = GetLocalResourceObject("historyTripRadios.boxLabel1").ToString();
                ReshistoryTripRadiosboxLabel2 = GetLocalResourceObject("historyTripRadios.boxLabel2").ToString();
                ReshistoryTripRadiosboxLabel3 = GetLocalResourceObject("historyTripRadios.boxLabel3").ToString();
                ResbtnSubmitMessage = GetLocalResourceObject("btnSubmit.Message").ToString();
                ResbtnSubmitAlert = GetLocalResourceObject("btnSubmit.Alert").ToString();
                ReshistorygridEmptyText = GetLocalResourceObject("historygrid.EmptyText").ToString();
                ReshistorygridBoxId = GetLocalResourceObject("historygrid.BoxId").ToString();
                ReshistorygridDescription = GetLocalResourceObject("historygrid.Description").ToString();
                ReshistorygridOriginDateTime = GetLocalResourceObject("historygrid.OriginDateTime").ToString();
                ReshistorygridStreetAddress = GetLocalResourceObject("historygrid.StreetAddress").ToString();
                ReshistorygridSpeed = GetLocalResourceObject("historygrid.Speed").ToString();
                ReshistorygridBoxMsgInTypeName = GetLocalResourceObject("historygrid.BoxMsgInTypeName").ToString();
                ReshistorygridMsgDetails = GetLocalResourceObject("historygrid.MsgDetails").ToString();
                ResAcknowledged = GetLocalResourceObject("Acknowledged").ToString();
                ReshistoryStopGridEmptyText = GetLocalResourceObject("historyStopGrid.EmptyText").ToString();
                ReshistoryStopGridArrivalDateTime = GetLocalResourceObject("historyStopGrid.ArrivalDateTime").ToString();
                ReshistoryStopGridLocation = GetLocalResourceObject("historyStopGrid.Location").ToString();
                ReshistoryStopGridDepartureDateTime = GetLocalResourceObject("historyStopGrid.DepartureDateTime").ToString();
                ReshistoryStopGridStopDuration = GetLocalResourceObject("historyStopGrid.StopDuration").ToString();
                ReshistoryStopGridRemarks = GetLocalResourceObject("historyStopGrid.Remarks").ToString();
                ReshistoryTripGridEmptyText = GetLocalResourceObject("historyTripGrid.EmptyText").ToString();
                ReshistoryTripGridDescription = GetLocalResourceObject("historyTripGrid.Description").ToString();
                ReshistoryTripGridDepartureTime = GetLocalResourceObject("historyTripGrid.DepartureTime").ToString();
                ReshistoryTripGridArrivalTime = GetLocalResourceObject("historyTripGrid.ArrivalTime").ToString();
                ReshistoryTripGrid_From = GetLocalResourceObject("historyTripGrid._From").ToString();
                ReshistoryTripGrid_To = GetLocalResourceObject("historyTripGrid._To").ToString();
                ReshistoryTripGridDuration = GetLocalResourceObject("historyTripGrid.Duration").ToString();
                ReshistoryTripGridFuelConsumed = GetLocalResourceObject("historyTripGrid.FuelConsumed").ToString();
                ReshistoryAddressGridhaUnitID = GetLocalResourceObject("historyAddressGrid.haUnitID").ToString();
                ReshistoryAddressGridhaDescription = GetLocalResourceObject("historyAddressGrid.haDescription").ToString();
                ReshistoryAddressGridhaDateTime = GetLocalResourceObject("historyAddressGrid.haDateTime").ToString();
                ReshistoryAddressGridhaDetails = GetLocalResourceObject("historyAddressGrid.haDetails").ToString();
                ReshidtoryDetailsGridEmptyText = GetLocalResourceObject("hidtoryDetailsGrid.EmptyText").ToString();
                ReshidtoryDetailsGridBoxId = GetLocalResourceObject("hidtoryDetailsGrid.BoxId").ToString();
                ReshidtoryDetailsGridDescription = GetLocalResourceObject("hidtoryDetailsGrid.Description").ToString();
                ReshidtoryDetailsGridOriginDateTime = GetLocalResourceObject("hidtoryDetailsGrid.OriginDateTime").ToString();
                ReshidtoryDetailsGridStreetAddress = GetLocalResourceObject("hidtoryDetailsGrid.StreetAddress").ToString();
                ReshidtoryDetailsGridSpeed = GetLocalResourceObject("hidtoryDetailsGrid.Speed").ToString();
                ReshidtoryDetailsGridBoxMsgInTypeName = GetLocalResourceObject("hidtoryDetailsGrid.BoxMsgInTypeName").ToString();
                ReshidtoryDetailsGridMsgDetails = GetLocalResourceObject("hidtoryDetailsGrid.MsgDetails").ToString();
                ReshidtoryDetailsGridAcknowledged = GetLocalResourceObject("hidtoryDetailsGrid.Acknowledged").ToString();
                ReshistoryFormEmptyText = GetLocalResourceObject("historyForm.EmptyText").ToString();
                ResfleetDefaultText = GetLocalResourceObject("fleetDefault.Text").ToString();
                ResloadingMaskMessage = GetLocalResourceObject("loadingMask.Message").ToString();
                RessearchingMaskMessage = GetLocalResourceObject("searchingMask.Message").ToString();
                ResmapitButtonText = GetLocalResourceObject("mapitButton.Text").ToString();
                ResupdatePositionButtonText = GetLocalResourceObject("updatePositionButton.Text").ToString();
                ResFeedbackButtonText = GetLocalResourceObject("FeedbackButton.Text").ToString();
                ResfleetButtonOpenwindowMessage = GetLocalResourceObject("fleetButton.Openwindow.Message").ToString();
                ReshistoryFleetButtonOpenwindowMessage = GetLocalResourceObject("historyFleetButton.Openwindow.Message").ToString();
                ReslabelonoffButtonlabelonoffShowLabel = GetLocalResourceObject("labelonoffButton.labelonoff.ShowLabel").ToString();
                ReslabelonoffButtonlabelonoffHideLabel = GetLocalResourceObject("labelonoffButton.labelonoff.HideLabel").ToString();
                ResstreetViewButtonOpenwindowMessage = GetLocalResourceObject("streetViewButton.Openwindow.Message").ToString();
                RestrackitButtonOpenwindowMessage = GetLocalResourceObject("trackitButton.Openwindow.Message").ToString();
                ReshistorygridemptyText2 = GetLocalResourceObject("historygrid.emptyText2").ToString();
                RestrackitmenuOpenwindowMessage = GetLocalResourceObject("trackitmenu.Openwindow.Message").ToString();
                ResstreetViewMenuOpenwindowMessage = GetLocalResourceObject("streetViewMenu.Openwindow.Message").ToString();
                Resgeolandmarkgridcolumnsname = GetLocalResourceObject("geolandmarkgrid.columns.name").ToString();
                ResgeolandmarkgridcolumnsType = GetLocalResourceObject("geolandmarkgrid.columns.Type").ToString();
                ResgeolandmarkgridsetTitle = GetLocalResourceObject("geolandmarkgrid.setTitle").ToString();
                ReshistoryDateFromfieldLabel = GetLocalResourceObject("historyDateFrom.fieldLabel").ToString();
                ReshistoryDateTofieldLabel = GetLocalResourceObject("historyDateTo.fieldLabel").ToString();
                ReshistoryTypefieldLabel = GetLocalResourceObject("historyType.fieldLabel").ToString();
                ResbtnSubmitAlertTitle = GetLocalResourceObject("btnSubmitAlert.Title").ToString();
                ReshistoryFleetButtonsetText = GetLocalResourceObject("historyFleetButton.setText").ToString();
                ReshistoryFormAlertTitle = GetLocalResourceObject("historyForm.Alert.Title").ToString();
                ReshistoryFormAlertMessage = GetLocalResourceObject("historyForm.Alert.Message").ToString();
                ResAllRoutesBtnText = GetLocalResourceObject("allRoutesBtn.Text").ToString();
                ResSubmitButtonText = GetLocalResourceObject("SubmitBtn.Text").ToString();
                ResHistoryLinkText = GetLocalResourceObject("HistoryLink.Text").ToString();
                ResHistoryLocationText = GetLocalResourceObject("historyLocation.Text").ToString();
                ResHistoryTypeValues_0 = GetLocalResourceObject("historyType_values_0.Text").ToString();
                ResHistoryTypeValues_1 = GetLocalResourceObject("historyType_values_1.Text").ToString();
                ResHistoryTypeValues_2 = GetLocalResourceObject("historyType_values_2.Text").ToString();
                ResHistoryTypeValues_3 = GetLocalResourceObject("historyType_values_3.Text").ToString();
                ResHistoryTypeValues_4 = GetLocalResourceObject("historyType_values_4.Text").ToString();
                ResFilterText = GetLocalResourceObject("MenuFilter.Text").ToString();
                ResbtnVehicleonoffShowDetailsText = GetLocalResourceObject("btnVehicleonoffShowDetails.Text").ToString();
                ResbtnVehicleonoffHideDetailsText = GetLocalResourceObject("btnVehicleonoffHideDetails.Text").ToString();
                ResbtnVehicleonoffTooltipText = GetLocalResourceObject("btnVehicleonoffTooltip.Text").ToString();
                ResTxtButtonTitleFleetText = GetLocalResourceObject("txtButtonTitleFleet.Text").ToString();
                ResTxtButtonTitleHierarchyText = GetLocalResourceObject("txtButtonTitleHierarchy.Text").ToString();
                ResHistoryCommModeNameText = GetLocalResourceObject("historyCommModeName.Text").ToString();
                ResRouteAssignedYes = GetLocalResourceObject("RouteAssignedYes.Text").ToString();
                ResRouteAssignedNo = GetLocalResourceObject("RouteAssignedNo.Text").ToString();
                ResgeozonegridcolumnsCurrentAssignment = GetLocalResourceObject("GeozoneGridColumnsCurrentAssignment.Text").ToString();
                Res_cvColorText = (string)base.GetLocalResourceObject("cvVehicleColorText");
                ResMultipleHierarchy = GetLocalResourceObject("MultipleHierarchy.Text").ToString(); //"Multiple Hierarchies";
                ResHistoryReplayReplay = GetLocalResourceObject("HistoryReplayReplay.Text").ToString();
                ResHistoryReplayStop = GetLocalResourceObject("HistoryReplayStop.Text").ToString();
                ResHistoryReplayTooltip = GetLocalResourceObject("HistoryReplayTooltip.Text").ToString();
                ResAddressresolutioninprogress = GetLocalResourceObject("Addressresolutioninprogress").ToString();
                ResResolveAddressToolTips = GetLocalResourceObject("ResolveAddressToolTips").ToString();
                ResResolving = GetLocalResourceObject("Resolving").ToString();
                ResValidGPS = GetLocalResourceObject("ValidGPS").ToString();
                ResTextTrue = GetLocalResourceObject("Text_True").ToString();
                ResTextFalse = GetLocalResourceObject("Text_False").ToString();
                ResTextNA = GetLocalResourceObject("Text_NA").ToString();

                SentinelFMSession sn = null;
                sn = (SentinelFMSession)Session["SentinelFMSession"];

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                // exclude MTS users
                //if (browser.Type.ToUpper().Contains("IE") && browser.MajorVersion <= 8 && sn.User.OrganizationId != 999630)
                //{
                IE8orUnder = true;
                //}
                //Added by Rohit Mittal for Trip Colors
                ShowMultiColor = clsPermission.FeaturePermissionCheck(sn, "ShowMultiColor");
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                if (sn.User.ControlEnable(sn, 72))
                    ShowRouteAssignment = true;
                else
                    ShowRouteAssignment = false;
                if (sn.User.ControlEnable(sn, 97))
                    BatteryTredingEnabled = true;
                ShowEngineHours = clsPermission.FeaturePermissionCheck(sn, "ShowEngineHours");
                ShowOdometer = clsPermission.FeaturePermissionCheck(sn, "ShowOdometer");
                ShowPTO = clsPermission.FeaturePermissionCheck(sn, "ShowPTO");

                HistoryEnabled = sn.User.ControlEnable(sn, 41) ? true : false;

                if (sn.User.OrganizationId == 123 || sn.User.OrganizationId == 480)
                {
                    ShowDashboardView = true;
                    getLandmarkCategory();
                }

                HGI = sn.User.UserGroupId == 1;

                VLF.PATCH.Logic.PatchGridPagesize ps = new VLF.PATCH.Logic.PatchGridPagesize(sConnectionString);
                DataSet pagesizes = ps.GetPagesizeSettingsByOrganizationId(sn.User.OrganizationId);
                foreach (DataRow dr in pagesizes.Tables[0].Rows)
                {
                    if (dr["Type"].ToString().ToLower() == "vehiclegridpagesize")
                    {
                        IE8VehicleGridPagesize = Convert.ToInt32(dr["PageSize"].ToString());
                    }
                    else if (dr["Type"].ToString().ToLower() == "historygridpagesize")
                    {
                        IE8HistoryGridPagesize = Convert.ToInt32(dr["PageSize"].ToString());
                    }
                    else if (dr["Type"].ToString().ToLower() == "historygridnormalpagesize")
                    {
                        HistoryPageSize = Convert.ToInt32(dr["PageSize"].ToString());
                    }
                }

                if (IE8orUnder)
                {
                    PageSize = IE8VehicleGridPagesize;
                    HistoryPageSize = IE8HistoryGridPagesize;
                }

                string defaultnodecode = string.Empty;

                clsUtility objUtil;
                objUtil = new clsUtility(sn);
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                string xml = "";
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                        mapAssets = false;
                    }
                if (xml == "")
                {
                    mapAssets = false;
                }

                // Hard-coded that only MTS will see Driver Finder button.
                if (sn.User.OrganizationId == 999630)
                {
                    showDriverFinderButton = true;
                    showDriverColumn = true;
                }
                if (sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999618)
                    showDriverColumn = true;

                if (sn.User.UserGroupId == 28)
                    ShowAlarmTab = false;

                if (sn.User.OrganizationId == 1000092) // disabled Alarm tab for G4S & Ryder & Cubeit at vehicle grid since it has alarms window.
                {
                    ShowAlarmTab = false;
                }

                if (ShowAlarmTab == true && sn.User.ViewAlarmScrolling == 0)
                {
                    ShowAlarmTab = false;
                }

                if (ShowMessageTab == true && sn.User.ViewMDTMessagesScrolling == 0)
                {
                    ShowMessageTab = false;
                }
                StringReader strrXML = new StringReader(xml);
                DataSet dsPref = new DataSet();
                dsPref.ReadXml(strrXML);

                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapAssets))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == VLF.CLS.Def.Const.mapAssetsYes)
                            mapAssets = true;
                        else
                            mapAssets = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClustering))
                    {
                        if (Convert.ToInt32(rowItem["PreferenceValue"]) == VLF.CLS.Def.Const.VehicleClusteringYes)
                            VehicleClustering = true;
                        else
                            VehicleClustering = false;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringDistance))
                    {
                        Int32.TryParse(rowItem["PreferenceValue"].ToString(), out VehicleClusteringDistance);
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.VehicleClusteringThreshold))
                    {
                        Int32.TryParse(rowItem["PreferenceValue"].ToString(), out VehicleClusteringThreshold);
                    }

                    /*if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MaxVehiclesOnMap))
                    {
                        Int32.TryParse(rowItem["PreferenceValue"].ToString(), out maxVehiclesOnMap);                        
                    }*/

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultMapView))
                    {
                        string d = rowItem["PreferenceValue"].ToString().ToLower();
                        if (d == "north" || d == "south" || d == "west" || d == "east" || d == "none")
                            defaultMapView = d;
                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                    {
                        defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                    }

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                    {
                        string d = rowItem["PreferenceValue"].ToString().ToLower();
                        if (d == "hierarchy")
                            LoadVehiclesBasedOn = "hierarchy";
                        else
                            LoadVehiclesBasedOn = "fleet";
                    }
                }

                if (!Page.IsPostBack)
                {
                    sn.Map.LastKnownXML = string.Empty; //To force loading of all vehicles on each request.
                    //mapRefreshFrequency = sn.User.GeneralRefreshFrequency;
                    //if (sn.Map.ReloadMap)
                    //{
                    //    if (sn.User.MapType == VLF.MAP.MapType.LSD)
                    //        Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
                    //    else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                    //        Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
                    //    else
                    //        Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmvehiclemap.aspx';</SCRIPT>");

                    //    sn.Map.ReloadMap = false;
                    //}
                }

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                    LoadVehiclesBasedOn = "fleet";
                }

                //LoadVehiclesBasedOn = "hierarchy";

                if (LoadVehiclesBasedOn == "hierarchy")
                {
                    defaultnodecode = defaultnodecode ?? string.Empty;
                    if (defaultnodecode == string.Empty)
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                    //if (MutipleUserHierarchyAssignment && DefaultOrganizationHierarchyFleetId == "0" && sn.User.PreferFleetIds != string.Empty)
                    //if (MutipleUserHierarchyAssignment && sn.User.PreferFleetIds != string.Empty)
                    if (MutipleUserHierarchyAssignment)
                    {
                        DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                        DefaultOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;
                        if (DefaultOrganizationHierarchyFleetId.Trim() == string.Empty)
                            DefaultOrganizationHierarchyFleetName = "";
                        else if (DefaultOrganizationHierarchyFleetId.Contains(','))
                            DefaultOrganizationHierarchyFleetName = ResMultipleHierarchy;
                        else
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    }
                }
                else
                {
                    DefaultFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, sn.User.DefaultFleet).Replace(VLF.CLS.Def.Const.defaultFleetName, ReshistoryFleetButtonsetText);
                }

                VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
                DataSet allLayers;

                allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID("Overlay", sn.User.OrganizationId);
                foreach (DataRow dr in allLayers.Tables[0].Rows)
                {
                    bool selected = false;
                    if (Convert.ToBoolean(dr["Default"].ToString()) || Convert.ToBoolean(dr["Premium"].ToString()))
                        selected = true;

                    overlays += dr["LayerName"].ToString().Trim() + ":" + selected.ToString().ToLower() + "," + dr["LayerName"].ToString().Trim() + "Visibility:" + dr["Visibility"].ToString().Trim().ToLower() + ",";
                }
                if (overlays != string.Empty)
                    overlays = overlays.Substring(0, overlays.Length - 1);
                overlays = "{" + overlays + "}";



            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected override void InitializeCulture()
        {

            if (Session["PreferredCulture"] != null)
            {
                string UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }
        }



        public void RedirectToLogin()
        {
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
        }

        private void getLandmarkCategory()
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                DataSet dsOrganization = org.ListOrganizationLandmarkCategory(sn.UserID, sn.User.OrganizationId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];

                CategoryList = "[\"Select a category\", 0]";
                foreach (DataRow oneRow in dsLandmarkCategory.Rows)
                {

                    CategoryList += ",[\"" + oneRow["MetadataValue"].ToString() + "\"," + oneRow["DomainMetadataId"].ToString() + "]";
                }

                CategoryList = "[" + CategoryList + "]";

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
    }
}
