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

    public partial class ReeferMap : System.Web.UI.Page
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
        public bool ifShowNavteq = false;
        public string overlays = string.Empty;
        public string defaultMapView = "north";
        public bool showDriverFinderButton = false;
        public bool showDriverColumn = false;
        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        public string DefaultFleetName = string.Empty;

        public bool ShowAlarmTab = true;

        public bool IE8orUnder = false;
        public int PageSize = 10000;
        public int HistoryPageSize = 200;
        //public int PageSize = 10;

        public string ExtjsVersion = "extjs-4.1.0";
        //public string ExtjsVersion = "ext-4.2.0.663";

        public string LastUpdatedVehicleListJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/VehicleList_Reefer.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedViolationsJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/Violations.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedReeferImpactJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/ReeferImpact.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedReeferMaintenanceJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/ReeferMaintenance.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedReeferEventJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/ReeferEvent.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedReeferAlarmJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/ReeferAlarm.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedReeferCommandHistoryJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/ReeferCommandHistory.js")).ToString("yyyyMMddHHmmss");
        public string LastUpdatedReeferReeferHistoryJs = System.IO.File.GetLastWriteTime(System.Web.HttpContext.Current.Server.MapPath("~/Scripts/NewMap/ReeferReeferHistory.js")).ToString("yyyyMMddHHmmss");

        public bool ViolationTabAtNewMapPage = false;

        public bool HistoryEnabled = true;

        private int IE8VehicleGridPagesize = 100;
        private int IE8HistoryGridPagesize = 100;

        public string ReeferDashboardDefaultTimeRange;
        public string DisplayZone2_3Temperatures = "0";
        public string TemperatureType = "";

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

                SentinelFMSession sn = null;
                sn = (SentinelFMSession)Session["SentinelFMSession"];

                ReeferDashboardDefaultTimeRange = ConfigurationManager.AppSettings["ReeferDashboardDefaultTimeRange"];
                TemperatureType = sn.User.TemperatureType;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                // exclude MTS users
                //if (browser.Type.ToUpper().Contains("IE") && browser.MajorVersion <= 8 && sn.User.OrganizationId != 999630)
                //{
                IE8orUnder = true;
                //}

                HistoryEnabled = sn.User.ControlEnable(sn, 41) ? true : false;

                ViolationTabAtNewMapPage = clsPermission.FeaturePermissionCheck(sn, "ViolationTabAtNewMapPage");

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
                        if (d == "north" || d == "south" || d == "west" || d == "east")
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
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                }
                else
                {
                    DefaultFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, sn.User.DefaultFleet);
                }

                VLF.PATCH.Logic.MapLayersManager ml = new VLF.PATCH.Logic.MapLayersManager(sConnectionString);
                DataSet allLayers;

                allLayers = ml.GetAllMapLayersWithDefaultPremiumByOrganizationID(sn.User.OrganizationId);

                foreach (DataRow dr in allLayers.Tables[0].Rows)
                {
                    if (Convert.ToBoolean(dr["Default"].ToString()) || Convert.ToBoolean(dr["Premium"].ToString()))
                    {
                        if (dr["LayerName"].ToString().Trim() == "Theme 1")
                            ifShowTheme1 = true;
                        else if (dr["LayerName"].ToString().Trim() == "Theme 2")
                            ifShowTheme2 = true;
                        else if (dr["LayerName"].ToString().Trim() == "Arcgis")
                            ifShowArcgis = true;
                        else if (dr["LayerName"].ToString().Trim() == "Google Streets")
                            ifShowGoogleStreets = true;
                        else if (dr["LayerName"].ToString().Trim() == "Google Hybrid")
                            ifShowGoogleHybrid = true;
                        else if (dr["LayerName"].ToString().Trim() == "Bing Roads")
                            ifShowBingRoads = true;
                        else if (dr["LayerName"].ToString().Trim() == "Bing Hybrid")
                            ifShowBingHybrid = true;
                        else if (dr["LayerName"].ToString().Trim() == "Navteq")
                            ifShowNavteq = true;
                    }
                }


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

                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                        {

                        }
                    if (xml != "")
                    {
                        DataSet dsOrganizationPref = new DataSet();
                        StringReader strrOrgXML = new System.IO.StringReader(xml);
                        dsOrganizationPref.ReadXml(strrOrgXML);
                        foreach (DataRow rowItem in dsOrganizationPref.Tables[0].Rows)
                        {

                            if (Convert.ToInt16(rowItem["OrgPreferenceId"]) == 26)  // 26 is Reefer map - Display zone 2 & 3 temperatures in [vlfOrganizationSettingsTypes] table
                            {
                                if (rowItem["PreferenceValue"].ToString().Trim() != "")
                                {
                                    DisplayZone2_3Temperatures = rowItem["PreferenceValue"].ToString();
                                }
                            }                            

                        }
                    }
                }


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
    }
}