using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using System.Configuration;
using VLF.PATCH.Logic;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace SentinelFM
{
    public partial class OpenLayerMaps : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        public bool ISSECURE = HttpContext.Current.Request.IsSecureConnection;

        public bool ifShowArcgis = false;
        public bool ifShowGoogleStreets = false;
        public bool ifShowGoogleHybrid = false;
        public bool ifShowBingRoads = false;
        public bool ifShowBingHybrid = false;
        
        public string overlays = string.Empty;
        public string basemapSettings = string.Empty;
        public bool ShowAssignToFleet = false;
        public bool ShowMapHistorySearch = false;
        public bool ShowRouteAssignment = false;

        public bool ShowCallTimer = false;
        public string CallTimerSelections = string.Empty;
        public bool isExtended = false;

        public string LastUpdatedOpenLayerMapJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/OpenLayerMap/OpenLayerMap.js")).ToString("yyyyMMddHHmmss");

        public bool ShowFields = false;
	    public bool ShowAlarmPage = false;
        public string SelectedLanguage = string.Empty;
        public string tooltip_Pan_Map = string.Empty;
        public string tooltip_Add_a_landmark_Circle = string.Empty;
        public string tooltip_Draw_a_Polygon = string.Empty;
        public string tooltip_Modify_feature = string.Empty;
        public string tooltip_UserPreference = string.Empty; //Salman, FR translation
        public string tooltip_PrintMap = string.Empty; // Salman (Mantis# 30??)
        public string ResEdit = string.Empty;


       //res start
        public string Res_alandmarkformmoreoptionsText = string.Empty;
        public string Res_alertDeletePolygonFailedText = string.Empty;
        public string Res_alertEditPolygonFailedText = string.Empty;
        public string Res_alertErrorText = string.Empty;
        public string Res_alertGeozoneLoadFailedText = string.Empty;
        public string Res_alertLandmarkLoadFailedText = string.Empty;
        public string Res_btnDeletePolygonText = string.Empty;
        public string Res_CancelText = string.Empty;
        public string Res_cboDirection_DisableText = string.Empty;
        public string Res_cboDirection_InOutText = string.Empty;
        public string Res_cboDirection_InText = string.Empty;
        public string Res_cboDirection_OutText = string.Empty;
        public string Res_cboGeoZoneSeverity_CriticalText = string.Empty;
        public string Res_cboGeoZoneSeverity_NoAlarmText = string.Empty;
        public string Res_cboGeoZoneSeverity_NotifyText = string.Empty;
        public string Res_cboGeoZoneSeverity_WarningText = string.Empty;
        public string Res_cboTimeZoneGMT = string.Empty;
        public string Res_chkCriticalText = string.Empty;
        public string Res_chkDayLightText = string.Empty;
        public string Res_chkNotifyText = string.Empty;
        public string Res_chkWarningText = string.Empty;
        public string Res_ClosestVehiclesNumOfVehicles = string.Empty;
        public string Res_closeText = string.Empty;
        public string Res_cvCustomSpeedText = string.Empty;
        public string Res_cvDriverText = string.Empty;
        public string Res_cvEquipmentText = string.Empty;
        public string Res_cvHistoryInfoIdTitle = string.Empty;
        public string Res_cvListNameText = string.Empty;
        public string Res_cvLocationTitle = string.Empty;
        public string Res_cvMyHeadingText = string.Empty;
        public string Res_cvTeamLeaderNameText = string.Empty;
        public string Res_cvVehicleStatusText = string.Empty;
        public string Res_errorContentFailedDataFetchText = string.Empty;
        public string Res_geozoneDefaultFormTitle = string.Empty;
        public string Res_geozoneDefaultInfoTitle = string.Empty;
        public string Res_geozoneLandmarkFormTitle = string.Empty;
        public string Res_geozoneLandmarkInfoTitle = string.Empty;
        public string Res_h6CircleContentText = string.Empty;
        public string Res_h6featureVehiclesText = string.Empty;
        public string Res_h6NewGeozoneLandmarlText = string.Empty;
        public string Res_lblCallTimerTitle = string.Empty;
        public string Res_lblContactNameTitle = string.Empty;
        public string Res_lblDefaultSeverityTitle = string.Empty;
        public string Res_lblDirectionTitle = string.Empty;
        public string Res_lblEmailTitle = string.Empty;
        public string Res_lblGeozoneDescriptionTitle = string.Empty;
        public string Res_lblGeozoneNameTitle = string.Empty;
        public string Res_lblLandmarkDescriptionTitle = string.Empty;
        public string Res_lblLandmarkNameTitle = string.Empty;
        public string Res_lbllstAddOptionsGeozoneText = string.Empty;
        public string Res_lblMultipleEmailsText = string.Empty;
        public string Res_lblPhoneTitle = string.Empty;
        public string Res_lblRadiusTitle = string.Empty;
        public string Res_lblTimeZoneTitle = string.Empty;
        public string Res_lnkAssignGeozoneToFleetText = string.Empty;
        public string Res_lnkBackText = string.Empty;
        public string Res_lnkClosestVehicleText = string.Empty;
        public string Res_lnklandmarkhistorysearchText = string.Empty;
        public string Res_lnkloadPopupsendmessageText = string.Empty;
        public string Res_lnkRouteAssignmentText = string.Empty;
        public string Res_lnkShowMapHistoryText = string.Empty;
        public string Res_lstAddOptionsLandmarkText = string.Empty;
        public string Res_lstPublicPrivate_PrivateText = string.Empty;
        public string Res_lstPublicPrivate_PublicText = string.Empty;
        public string Res_messagebarEditLandmarkCircleText = string.Empty;
        public string Res_messagebarGeoLandZoomInText = string.Empty;
        public string Res_messagebarGeozoneNonEditableText = string.Empty;
        public string Res_messagebarSaveChangesText = string.Empty;
        public string Res_messagebarSavePolygonFailedText = string.Empty;
        public string Res_onFeatureSelectContentText = string.Empty;
        public string Res_SaveText = string.Empty;
        public string Res_searchAddressMessageText = string.Empty;
        public string Res_SearchHistoryDateText = string.Empty;
        public string Res_SearchHistoryMinutesText = string.Empty;
        public string Res_SearchHistoryTimeRangeText = string.Empty;
        public string Res_SearchHistoryTimeText = string.Empty;
        public string Res_SearchHistoryTitle = string.Empty;
        public string Res_searchmessageHtmlText = string.Empty;
        public string Res_SearchText = string.Empty;
        public string Res_vehicleText = string.Empty;
        public string Res_vehicleViewMoreLinkText = string.Empty;
        public string Res_WMSInfoControlTitle = string.Empty;
        //Added by devin for default view
        public String IsAdmin = "0"; 
        public String Res_ErrorSave = "Failed to save.";
        public String Res_SaveSuccessfully = "Saved successfully.";
        public String Res_Sure_Set_Default = "Are you sure you want to set default map view?";

        public string Res_By = "By";
        public string Res_All = "All";
        public string Res_Fleet = "Fleet";
        public string Res_Hierarchy = "Hierarchy";
        public string Res_SelectedVehicles = "Selected Vehicles";
        public string Res_SearchHistoryFleetHierarchy = string.Empty;

        //Added by Salman For Google Auto Complete Search Text Box (Transalation)
        public string PlaceHolderText = string.Empty;
        public string Res_DefaultViewGroupWindowText = string.Empty;
        public string Res_DefaultViewGroupBoxLabelUserText = string.Empty;
        public string Res_DefaultViewGroupBoxLabelOrganizationText = string.Empty;
        public string Res_DefaultViewGroupButtonCancelText = string.Empty;
        public string Res_DefaultViewGroupButtonSaveText = string.Empty;
        public string Res_DeleteConfirmation = string.Empty;

        public string Res_CategoryTitle = string.Empty;

	//Added by Rohit Mittal
	public bool ShowMultiColor = false;

        //PublicLandmark option on Add
        public bool ShowPublicLandmarkOption = true;
        //PublicGeoZone option on Add
        public bool ShowPublicGeoZoneOption = true;
        public string GeoServer = string.Empty;
        public string CategoryList = string.Empty;
        
        private string sConnectionString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            ShowAlarmPage = false;
            
            if (sn.User.ControlEnable(sn, 115))
            {
                //if (sn.User.UserGroupId == 1 || sn.User.UserGroupId == 7 || sn.User.UserGroupId == 27 || sn.User.UserGroupId == 36 || sn.User.UserGroupId == 44 || sn.UserID == 12394) // hgi user, Armored Car Security Admin, Dispatcher, NCC, NCC Admin, user: mis45050
                //{
                    ShowAlarmPage = true;
                //}
            }

            GeoServer = ISSECURE ? ConfigurationManager.AppSettings.Get("geoserverSSL") : ConfigurationManager.AppSettings.Get("geoserver");

            ShowAssignToFleet = clsPermission.FeaturePermissionCheck(sn, "GeozoneFleetAssignment");
            ShowMapHistorySearch = true; // clsPermission.FeaturePermissionCheck(sn, "MapAddressHistorySearch"); // give all users map history search feature.

            //if (sn.User.OrganizationId == 1000041 || sn.User.OrganizationId == 1000051 || sn.User.OrganizationId == 1000148)//Mantis# 3620 (Milton's/LIRR's 24H Search) Mantis#11486 (DiCan)
            if ((new List<int>(new int[] { 1000041, 1000051, 1000148, 1000076 })).Contains(sn.User.OrganizationId)) //Mantis# 3620 (Milton's/LIRR's 24H Search) Mantis#11486 (DiCan) Mantis#11620 (Metro North)
            {
                isExtended = true;
            }
            
            if (sn.User.ControlEnable(sn, 16)) IsAdmin = "1"; //Added by devin for default view if is Admin then can do organization setting
            if (sn.User.OrganizationId == 999988)
                ShowFields = true;

            //res binding start
            Res_alandmarkformmoreoptionsText = (string)base.GetLocalResourceObject("alandmarkformmoreoptionsText");
            Res_alertDeletePolygonFailedText = (string)base.GetLocalResourceObject("alertDeletePolygonFailedText");
            Res_alertEditPolygonFailedText = (string)base.GetLocalResourceObject("alertEditPolygonFailedText");
            Res_alertErrorText = (string)base.GetLocalResourceObject("alertErrorText");
            Res_alertGeozoneLoadFailedText = (string)base.GetLocalResourceObject("alertGeozoneLoadFailedText");
            Res_alertLandmarkLoadFailedText = (string)base.GetLocalResourceObject("alertLandmarkLoadFailedText");
            Res_btnDeletePolygonText = (string)base.GetLocalResourceObject("btnDeletePolygonText");
            Res_CancelText = (string)base.GetLocalResourceObject("CancelText");
            Res_cboDirection_DisableText = (string)base.GetLocalResourceObject("cboDirection_DisableText");
            Res_cboDirection_InOutText = (string)base.GetLocalResourceObject("cboDirection_InOutText");
            Res_cboDirection_InText = (string)base.GetLocalResourceObject("cboDirection_InText");
            Res_cboDirection_OutText = (string)base.GetLocalResourceObject("cboDirection_OutText");
            Res_cboGeoZoneSeverity_CriticalText = (string)base.GetLocalResourceObject("cboGeoZoneSeverity_CriticalText");
            Res_cboGeoZoneSeverity_NoAlarmText = (string)base.GetLocalResourceObject("cboGeoZoneSeverity_NoAlarmText");
            Res_cboGeoZoneSeverity_NotifyText = (string)base.GetLocalResourceObject("cboGeoZoneSeverity_NotifyText");
            Res_cboGeoZoneSeverity_WarningText = (string)base.GetLocalResourceObject("cboGeoZoneSeverity_WarningText");
            Res_cboTimeZoneGMT = (string)base.GetLocalResourceObject("cboTimeZoneGMT");
            Res_chkCriticalText = (string)base.GetLocalResourceObject("chkCriticalText");
            Res_chkDayLightText = (string)base.GetLocalResourceObject("chkDayLightText");
            Res_chkNotifyText = (string)base.GetLocalResourceObject("chkNotifyText");
            Res_chkWarningText = (string)base.GetLocalResourceObject("chkWarningText");
            Res_ClosestVehiclesNumOfVehicles = (string)base.GetLocalResourceObject("ClosestVehiclesNumOfVehicles");
            Res_closeText = (string)base.GetLocalResourceObject("closeText");
            Res_cvCustomSpeedText = (string)base.GetLocalResourceObject("cvCustomSpeedText");
            Res_cvDriverText = (string)base.GetLocalResourceObject("cvDriverText");
            Res_cvEquipmentText = (string)base.GetLocalResourceObject("cvEquipmentText");
            Res_cvHistoryInfoIdTitle = (string)base.GetLocalResourceObject("cvHistoryInfoIdTitle");
            Res_cvListNameText = (string)base.GetLocalResourceObject("cvListNameText");
            Res_cvLocationTitle = (string)base.GetLocalResourceObject("cvLocationTitle");
            Res_cvMyHeadingText = (string)base.GetLocalResourceObject("cvMyHeadingText");
            Res_cvTeamLeaderNameText = (string)base.GetLocalResourceObject("cvTeamLeaderNameText");
            Res_cvVehicleStatusText = (string)base.GetLocalResourceObject("cvVehicleStatusText");
            Res_errorContentFailedDataFetchText = (string)base.GetLocalResourceObject("errorContentFailedDataFetchText");
            Res_geozoneDefaultFormTitle = (string)base.GetLocalResourceObject("geozoneDefaultFormTitle");
            Res_geozoneDefaultInfoTitle = (string)base.GetLocalResourceObject("geozoneDefaultInfoTitle");
            Res_geozoneLandmarkFormTitle = (string)base.GetLocalResourceObject("geozoneLandmarkFormTitle");
            Res_geozoneLandmarkInfoTitle = (string)base.GetLocalResourceObject("geozoneLandmarkInfoTitle");
            Res_h6CircleContentText = (string)base.GetLocalResourceObject("h6CircleContentText");
            Res_h6featureVehiclesText = (string)base.GetLocalResourceObject("h6featureVehiclesText");
            Res_h6NewGeozoneLandmarlText = (string)base.GetLocalResourceObject("h6NewGeozoneLandmarlText");
            Res_lblCallTimerTitle = (string)base.GetLocalResourceObject("lblCallTimerTitle");
            Res_lblContactNameTitle = (string)base.GetLocalResourceObject("lblContactNameTitle");
            Res_lblDefaultSeverityTitle = (string)base.GetLocalResourceObject("lblDefaultSeverityTitle");
            Res_lblDirectionTitle = (string)base.GetLocalResourceObject("lblDirectionTitle");
            Res_lblEmailTitle = (string)base.GetLocalResourceObject("lblEmailTitle");
            Res_lblGeozoneDescriptionTitle = (string)base.GetLocalResourceObject("lblGeozoneDescriptionTitle");
            Res_lblGeozoneNameTitle = (string)base.GetLocalResourceObject("lblGeozoneNameTitle");
            Res_lblLandmarkDescriptionTitle = (string)base.GetLocalResourceObject("lblLandmarkDescriptionTitle");
            Res_lblLandmarkNameTitle = (string)base.GetLocalResourceObject("lblLandmarkNameTitle");
            Res_lbllstAddOptionsGeozoneText = (string)base.GetLocalResourceObject("lbllstAddOptionsGeozoneText");
            Res_lblMultipleEmailsText = (string)base.GetLocalResourceObject("lblMultipleEmailsText");
            Res_lblPhoneTitle = (string)base.GetLocalResourceObject("lblPhoneTitle");
            Res_lblRadiusTitle = (string)base.GetLocalResourceObject("lblRadiusTitle");
            Res_lblTimeZoneTitle = (string)base.GetLocalResourceObject("lblTimeZoneTitle");
            Res_lnkAssignGeozoneToFleetText = (string)base.GetLocalResourceObject("lnkAssignGeozoneToFleetText");
            Res_lnkBackText = (string)base.GetLocalResourceObject("lnkBackText");
            Res_lnkClosestVehicleText = (string)base.GetLocalResourceObject("lnkClosestVehicleText");
            Res_lnklandmarkhistorysearchText = (string)base.GetLocalResourceObject("lnklandmarkhistorysearchText");
            Res_lnkloadPopupsendmessageText = (string)base.GetLocalResourceObject("lnkloadPopupsendmessageText");
            Res_lnkRouteAssignmentText = (string)base.GetLocalResourceObject("lnkRouteAssignmentText");
            Res_lnkShowMapHistoryText = (string)base.GetLocalResourceObject("lnkShowMapHistoryText");
            Res_lstAddOptionsLandmarkText = (string)base.GetLocalResourceObject("lstAddOptionsLandmarkText");
            Res_lstPublicPrivate_PrivateText = (string)base.GetLocalResourceObject("lstPublicPrivate_PrivateText");
            Res_lstPublicPrivate_PublicText = (string)base.GetLocalResourceObject("lstPublicPrivate_PublicText");
            Res_messagebarEditLandmarkCircleText = (string)base.GetLocalResourceObject("messagebarEditLandmarkCircleText");
            Res_messagebarGeoLandZoomInText = (string)base.GetLocalResourceObject("messagebarGeoLandZoomInText");
            Res_messagebarGeozoneNonEditableText = (string)base.GetLocalResourceObject("messagebarGeozoneNonEditableText");
            Res_messagebarSaveChangesText = (string)base.GetLocalResourceObject("messagebarSaveChangesText");
            Res_messagebarSavePolygonFailedText = (string)base.GetLocalResourceObject("messagebarSavePolygonFailedText");
            Res_onFeatureSelectContentText = (string)base.GetLocalResourceObject("onFeatureSelectContentText");
            Res_SaveText = (string)base.GetLocalResourceObject("SaveText");
            Res_searchAddressMessageText = (string)base.GetLocalResourceObject("searchAddressMessageText");
            Res_SearchHistoryDateText = (string)base.GetLocalResourceObject("SearchHistoryDateText");
            Res_SearchHistoryMinutesText = (string)base.GetLocalResourceObject("SearchHistoryMinutesText");
            Res_SearchHistoryTimeRangeText = (string)base.GetLocalResourceObject("SearchHistoryTimeRangeText");
            Res_SearchHistoryTimeText = (string)base.GetLocalResourceObject("SearchHistoryTimeText");
            Res_SearchHistoryTitle = (string)base.GetLocalResourceObject("SearchHistoryTitle");
            Res_searchmessageHtmlText = (string)base.GetLocalResourceObject("searchmessageHtmlText");
            Res_SearchText = (string)base.GetLocalResourceObject("SearchText");
            Res_vehicleText = (string)base.GetLocalResourceObject("vehicleText");
            Res_vehicleViewMoreLinkText = (string)base.GetLocalResourceObject("vehicleViewMoreLinkText");
            Res_WMSInfoControlTitle = (string)base.GetLocalResourceObject("WMSInfoControlTitle");
            Res_By = (string)base.GetLocalResourceObject("By");
            Res_Fleet = (string)base.GetLocalResourceObject("Fleet");
            Res_All = (string)base.GetLocalResourceObject("All");
            Res_Hierarchy = (string)base.GetLocalResourceObject("Hierarchy");
            Res_SelectedVehicles = (string)base.GetLocalResourceObject("SelectedVehicles");
            Res_SearchHistoryFleetHierarchy = sn.User.LoadVehiclesBasedOn == "fleet" ? Res_Fleet : Res_Hierarchy;


            SelectedLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToString().ToLower();
            tooltip_Pan_Map = (string)base.GetLocalResourceObject("tooltip_Pan_Map");
            tooltip_Add_a_landmark_Circle = (string)base.GetLocalResourceObject("tooltip_Add_a_landmark_Circle");
            tooltip_Draw_a_Polygon = (string)base.GetLocalResourceObject("tooltip_Draw_a_Polygon");
            tooltip_Modify_feature = (string)base.GetLocalResourceObject("tooltip_Modify_feature");
            tooltip_UserPreference = (string)base.GetLocalResourceObject("tooltip_UserPreference"); //Salman, FR translation
            tooltip_PrintMap = (string)base.GetLocalResourceObject("tooltip_PrintMap"); // Salman (Mantis# 30??)
            ResEdit = (string)base.GetLocalResourceObject("Edit");

            //Salman For Google Auto Complete Search Text Box (Transalation)
            PlaceHolderText = (string)base.GetLocalResourceObject("PlaceHolder.Text");
            Res_DefaultViewGroupWindowText = (string)base.GetLocalResourceObject("DefaultViewGroup.Window.Text");
            Res_DefaultViewGroupBoxLabelUserText = (string)base.GetLocalResourceObject("DefaultViewGroup.BoxLabelUser.Text");
            Res_DefaultViewGroupBoxLabelOrganizationText = (string)base.GetLocalResourceObject("DefaultViewGroup.BoxLabelOrganization.Text");
            Res_DefaultViewGroupButtonCancelText = (string)base.GetLocalResourceObject("DefaultViewGroup.ButtonCancel.Text");
            Res_DefaultViewGroupButtonSaveText = (string)base.GetLocalResourceObject("DefaultViewGroup.ButtonSave.Text");
            Res_DeleteConfirmation = (string)base.GetLocalResourceObject("DeleteConfirmation");

            Res_CategoryTitle = (string)base.GetLocalResourceObject("CategoryTitle");

	    //Added by Rohit Mittal
	    ShowMultiColor = clsPermission.FeaturePermissionCheck(sn, "ShowMultiColor");
            ShowRouteAssignment = clsPermission.FeaturePermissionCheck(sn, "RouteAssignment");

            if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
            {
                //enableTimer = true;
                //trCallTimer.Visible = true;
                ShowCallTimer = true;
                if (!Page.IsPostBack)
                    CboServices_Fill();
            }

            VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
            DataSet allLayers;

            allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID(sn.User.OrganizationId);

            foreach (DataRow dr in allLayers.Tables[0].Rows)
            {
                bool selected = false;
                
                string layername = dr["LayerName"].ToString().Trim().Replace(" ","");
                if (Convert.ToBoolean(dr["Default"].ToString()) || Convert.ToBoolean(dr["Premium"].ToString()))
                {
                    selected = true;
                    if (dr["LayerName"].ToString().Trim() == "Arcgis")
                        ifShowArcgis = true;
                    else if (dr["LayerName"].ToString().Trim() == "Google Streets")
                        ifShowGoogleStreets = true;
                    else if (dr["LayerName"].ToString().Trim() == "Google Hybrid")
                        ifShowGoogleHybrid = true;
                    else if (dr["LayerName"].ToString().Trim() == "Bing Roads")
                        ifShowBingRoads = true;
                    else if (dr["LayerName"].ToString().Trim() == "Bing Hybrid")
                        ifShowBingHybrid = true;
                }
                basemapSettings += layername + ":" + selected.ToString().ToLower() + "," + layername + "Description:'" + dr["Description"].ToString().Trim().Replace("'", "\\\'") + "',";
                
            }
            if (basemapSettings != string.Empty)
                basemapSettings = basemapSettings.Substring(0, basemapSettings.Length - 1);
            basemapSettings = "{" + basemapSettings + "}";

            allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID("Overlay", sn.User.OrganizationId);
            foreach (DataRow dr in allLayers.Tables[0].Rows)
            {
                bool selected = false;
                if (Convert.ToBoolean(dr["Default"].ToString()) || Convert.ToBoolean(dr["Premium"].ToString()))
                    selected = true;

                overlays += dr["LayerName"].ToString().Trim() + ":" + selected.ToString().ToLower() + "," + dr["LayerName"].ToString().Trim() + "Visibility:" + dr["Visibility"].ToString().Trim().ToLower() + ",";
                overlays += dr["LayerName"].ToString().Trim() + "Description:'" + dr["Description"].ToString().Trim().Replace("'", "\\\'") + "',";
            }
            if (overlays != string.Empty)
                overlays = overlays.Substring(0, overlays.Length - 1);
            overlays = "{" + overlays + "}";
            
            if (!Page.IsPostBack)
            {
                //Devin added for default map view
                GetDefaultMapView();



                foreach (DataRow rowItem in sn.User.DsGUIControls.Tables[0].Rows)
                {
                    if (Convert.ToInt32(rowItem["ControlId"]) == 92) //Disable public landmark option 
                        ShowPublicLandmarkOption = false;

                    if (Convert.ToInt32(rowItem["ControlId"]) == 93) //Disable public geozone option 
                        ShowPublicGeoZoneOption = false;  
                    
                }

            }

            getLandmarkCategory();
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

        private void CboServices_Fill()
        {
            try
            {
                //string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.PATCH.Logic.PatchServices _ps = new VLF.PATCH.Logic.PatchServices(sConnectionString);  
                DataSet dsServices = new DataSet();
                dsServices = _ps.GetHardcodedCallTimerServices();
                /*cboServices.DataSource = dsServices;
                cboServices.DataBind();

                //cboFleet.Items.Insert(0, new ListItem((string)this.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
                cboServices.Items.Insert(0, new ListItem("Select a service", "-1"));

                setDefaultCallTimer();*/
                CallTimerSelections = "<select id=\"cboServices\" class=\"RegularText\" name=\"cboServices\">";
                CallTimerSelections += "    <option value=\"-1\">Select a service</option>";
                foreach (DataRow dr in dsServices.Tables[0].Rows)
                {
                    CallTimerSelections += "    <option value=\"" + dr["ServiceConfigId"].ToString() + "\">" + dr["RulesApplied"].ToString() + "</option>";
                }
                CallTimerSelections += "</select>";
                CallTimerSelections = CallTimerSelections.Replace("\"", "\\\"");


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

        private void getLandmarkCategory()
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                DataSet dsOrganization = org.ListOrganizationLandmarkCategory(sn.UserID, sn.User.OrganizationId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];

                CategoryList = "[\"Select a category\", 0, 1]";                
                foreach (DataRow oneRow in dsLandmarkCategory.Rows)
                {
                    
                    CategoryList += ",[\"" + oneRow["MetadataValue"].ToString() + "\"," + oneRow["DomainMetadataId"].ToString() + ", 1]";
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


        //Added By Devin for default map view
        void GetDefaultMapView()
        {
            if (sn.MapCenter == null && sn.MapZoomLevel == null)
            {
                clsUtility objUtil;
                objUtil = new clsUtility(sn);
                string xml = "";
                DataSet dsPref = new DataSet();
                System.IO.StringReader strrXML = null;
                using (ServerDBUser.DBUser dbu = new ServerDBUser.DBUser())
                {
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML_ByUserId(sn.UserID, sn.SecId, sn.UserID, ref xml), false))
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML_ByUserId(sn.UserID, sn.SecId, sn.UserID, ref xml), true))
                        {
                            return;
                        }
                    if (xml == "")
                    {
                        return;
                    }

                    strrXML = new System.IO.StringReader(xml);
                    dsPref.ReadXml(strrXML);
                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {

                        if (Convert.ToInt16(rowItem["PreferenceId"]) == 41)// 41 is Map center ID in [vlfPreference] table
                        {
                            if (rowItem["PreferenceValue"].ToString().Trim() != "")
                            {
                                sn.MapCenter = rowItem["PreferenceValue"].ToString();
                            }
                        }
                        if (Convert.ToInt16(rowItem["PreferenceId"]) == 42)  // 42 is Map center ID in [vlfPreference] table
                        {
                            if (rowItem["PreferenceValue"].ToString().Trim() != "")
                            {
                                sn.MapZoomLevel = rowItem["PreferenceValue"].ToString();
                            }
                        }

                    }
                }

                if (sn.MapCenter == null && sn.MapZoomLevel == null)
                {

                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                            if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                            {
                                return;
                            }
                        if (xml == "")
                        {
                            return;
                        }
                        dsPref = new DataSet();
                        strrXML = new System.IO.StringReader(xml);
                        dsPref.ReadXml(strrXML);
                        foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                        {

                            if (Convert.ToInt16(rowItem["OrgPreferenceId"]) == 15)// 15 is Map center ID in [vlfOrganizationSettingsTypes] table
                            {
                                if (rowItem["PreferenceValue"].ToString().Trim() != "")
                                {
                                    sn.MapCenter = rowItem["PreferenceValue"].ToString();
                                }
                            }
                            if (Convert.ToInt16(rowItem["OrgPreferenceId"]) == 16) // 16 is Zoom Level in [vlfOrganizationSettingsTypes] table
                            {
                                if (rowItem["PreferenceValue"].ToString().Trim() != "")
                                {
                                    sn.MapZoomLevel = rowItem["PreferenceValue"].ToString();
                                }
                            }

                        }
                    }
                }

                if (sn.MapCenter == null) sn.MapCenter = "";
                if (sn.MapZoomLevel == null) sn.MapZoomLevel = "";
            }
        }

        [System.Web.Services.WebMethod]
        public static string SetMapDefaultView(string type, string center, string zoomlevel)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsUtility objUtil;
                objUtil = new clsUtility(sn);

                if (type.ToLower() == "user")
                {
                    using (ServerDBUser.DBUser dbu = new ServerDBUser.DBUser())
                    {
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, sn.UserID, 41, center), false)) // 41 is Map center ID in [vlfPreference] table
                            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, sn.UserID, 41, center), true))
                            {
                                return "0";
                            }
                        if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, sn.UserID, 42, zoomlevel), false)) // 42 is Map center ID in [vlfPreference] table
                            if (objUtil.ErrCheck(dbu.UserPreference_Add_Update_ByUserId(sn.UserID, sn.SecId, sn.UserID, 42, zoomlevel), true))
                            {
                                return "0";
                            }
                    }
                    sn.MapCenter = null;
                    sn.MapZoomLevel = null;
                }

                if (type.ToLower() == "organization")
                {
                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, 15, center), false)) // 15 is Map center ID in [vlfOrganizationSettingsTypes] table
                            if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, 15, center), true))
                            {
                                return "0";
                            }
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, 16, zoomlevel), false)) // 16 is Zoom Level in [vlfOrganizationSettingsTypes] table
                            if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, 16, zoomlevel), true))
                            {
                                return "0";
                            }
                        sn.MapCenter = null;
                        sn.MapZoomLevel = null;

                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePositionResult() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }
    }
}
