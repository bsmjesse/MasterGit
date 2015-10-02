using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Web.SessionState;


namespace SentinelFM
{
    /// <summary>
    /// Summary description for clsUser.
    /// </summary>

    public class clsUser
    {

       
        ServerDBUser.DBUser dbu;        
        protected clsUtility objUtil;
        private VLF.MAP.MapEngine<VLF.MAP.MapType>[] mapEngine;
        public VLF.MAP.MapEngine<VLF.MAP.MapType>[] MapEngine
        {
            get { return mapEngine; }
            set { mapEngine = value; }
        }

        private VLF.MAP.MapEngine<VLF.MAP.GeoCodeType>[] geoCodeEngine;
        public VLF.MAP.MapEngine<VLF.MAP.GeoCodeType>[] GeoCodeEngine
        {
            get { return geoCodeEngine; }
            set { geoCodeEngine = value; }
        }

        private int defaultFleet = -1;
        public int DefaultFleet
        {
            get { return defaultFleet; }
            set { defaultFleet = value; }
        }

        private double unitOfMes = 0.6214;
        public double UnitOfMes
        {
            get { return unitOfMes; }
            set { unitOfMes = value; }
        }


        private double volumeUnits = 0.26;
        public double VolumeUnits
        {
            get { return volumeUnits; }
            set { volumeUnits = value; }
        }

        private Int16 timeZone = 0;
        public Int16 TimeZone
        {
            get { return timeZone; }
            set { timeZone = value; }
        }

        // Changes for TimeZone feature start
        private float newFloatTimeZone = 0.0f;
        public float NewFloatTimeZone
        {
            get { return newFloatTimeZone; }
            set { newFloatTimeZone = value; }
        }
        // Changes for TimeZone feature end

        private string companyLogo = "";
        public string CompanyLogo
        {
            get { return companyLogo; }
            set { companyLogo = value; }
        }


        private Int16 dayLightSaving = 0;
        public Int16 DayLightSaving
        {
            get { return dayLightSaving; }
            set { dayLightSaving = value; }
        }

        private Int16 showReadMess = 0;
        public Int16 ShowReadMess
        {
            get { return showReadMess; }
            set { showReadMess = value; }
        }

        private Int32 alarmRefreshFrequency = 60000;
        public Int32 AlarmRefreshFrequency
        {
            get { return alarmRefreshFrequency; }
            set { alarmRefreshFrequency = value; }
        }

        private Int32 generalRefreshFrequency = 60000;
        public Int32 GeneralRefreshFrequency
        {
            get { return generalRefreshFrequency; }
            set { generalRefreshFrequency = value; }
        }

        private Int64 positionExpiredTime = VLF.CLS.Def.Const.PositionExpiredTime;
        public Int64 PositionExpiredTime
        {
            get { return positionExpiredTime; }
            set { positionExpiredTime = value; }
        }

        private string firstName = "";
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }


        private string lastName = "";
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }


        private string ipAddr = "";
        public string IPAddr
        {
            get { return ipAddr; }
            set { ipAddr = value; }
        }


        private string organizationName = "";
        public string OrganizationName
        {
            get { return organizationName; }
            set { organizationName = value; }
        }

        private bool driverMessageForward = false;
        public bool DriverMessageForward
        {
            get { return driverMessageForward; }
            set { driverMessageForward = value; }
        }

        private string menuColor = "";
        public string MenuColor
        {
            get { return menuColor; }
            set { menuColor = value; }
        }

        private string configTabBackColor = "";
        public string ConfigTabBackColor
        {
            get { return configTabBackColor; }
            set { configTabBackColor = value; }
        }

        private string temperatureZone = "";
        public string TemperatureZone
        {
            get { return temperatureZone; }
            set { temperatureZone = value; }
        }

        private Int32 organizationId = 0;
        public Int32 OrganizationId
        {
            get { return organizationId; }
            set { organizationId = value; }
        }



        private DataSet dsGUIControls;
        public DataSet DsGUIControls
        {
            get { return dsGUIControls; }
            set { dsGUIControls = value; }
        }



        private int screenWidth = 1024;
        public int ScreenWidth
        {
            get { return screenWidth; }
            set { screenWidth = value; }
        }

        private int screenHeight = 768;
        public int ScreenHeight
        {
            get { return screenHeight; }
            set { screenHeight = value; }
        }

        private string fleetPulseURL;
        public string FleetPulseURL
        {
            get { return fleetPulseURL; }
            set { fleetPulseURL = value; }
        }


        private Int16 viewMDTMessagesScrolling = 0;
        public Int16 ViewMDTMessagesScrolling
        {
            get { return viewMDTMessagesScrolling; }
            set { viewMDTMessagesScrolling = value; }
        }



        private Int16 viewAlarmScrolling = 0;
        public Int16 ViewAlarmScrolling
        {
            get { return viewAlarmScrolling; }
            set { viewAlarmScrolling = value; }
        }


        private Int16 showMapGridFilter = 1;
        public Int16 ShowMapGridFilter
        {
            get { return showMapGridFilter; }
            set { showMapGridFilter = value; }
        }

        private Int16 rememberLastPage = 1;
        public Int16 RemberLastPage
        {
            get { return rememberLastPage; }
            set { rememberLastPage = value; }
        }

        private Int32 userGroupId = 0;
        public Int32 UserGroupId
        {
            get { return userGroupId; }
            set { userGroupId = value; }
        }

        private Int32 parentUserGroupId = 0;
        public Int32 ParentUserGroupId
        {
            get { return parentUserGroupId; }
            set { parentUserGroupId = value; }
        }


        private Int32 superOrganizationId = 1;
        public Int32 SuperOrganizationId
        {
            get { return superOrganizationId; }
            set { superOrganizationId = value; }
        }




        private bool hosEnabled = false;
        public bool HosEnabled
        {
            get { return hosEnabled; }
            set { hosEnabled = value; }
        }


        private DataSet dsDrivers;
        public DataSet DsDrivers
        {
            get { return dsDrivers; }
            set { dsDrivers = value; }
        }



        private DataSet dsDriverHOSnotifications;
        public DataSet DsDriverHOSnotifications
        {
            get { return dsDriverHOSnotifications; }
            set { dsDriverHOSnotifications = value; }
        }


        private DataSet dsDashBoards;
        public DataSet DsDashBoards
        {
            get { return dsDashBoards; }
            set { dsDashBoards = value; }
        }


        private VLF.MAP.MapType mapType = 0;
        public VLF.MAP.MapType MapType
        {
            get { return mapType; }
            set { mapType = value; }
        }

        private Int16 userMapType = 0;
        public Int16 UserMapType
        {
            get { return userMapType; }
            set { userMapType = value; }
        }

        private bool isBingEnabled = false;
        public bool IsBingEnabled
        {
            get { return isBingEnabled; }
            set { isBingEnabled = value; }
        }


        private bool isLSDEnabled = false;
        public bool IsLSDEnabled
        {
            get { return isLSDEnabled; }
            set { isLSDEnabled = value; }
        }


        private bool smsSupport = false;
        public bool SmsSupport
        {
            get { return smsSupport; }
            set { smsSupport = value; }
        }

        private bool customReportsPostback = false;
        public bool CustomReportsPostback
        {
            get { return customReportsPostback; }
            set { customReportsPostback = value; }
        }


        private int customReportsLastSelected = 0;
        public int CustomReportsLastSelected
        {
            get { return customReportsLastSelected; }
            set { customReportsLastSelected = value; }
        }


        private int customReportsCurrentSelected = 0;
        public int CustomReportsCurrentSelected
        {
            get { return customReportsCurrentSelected; }
            set { customReportsCurrentSelected = value; }
        }


        private Int16 vehicleDataSource = 0;
        public Int16 VehicleDataSource
        {
            get { return vehicleDataSource; }
            set { vehicleDataSource = value; }
        }

        private string preferNodeCodes = string.Empty;
        public string PreferNodeCodes
        {
            get { return preferNodeCodes; }
            set { preferNodeCodes = value; }
        }

        private string preferFleetIds = string.Empty;
        public string PreferFleetIds
        {
            get { return preferFleetIds; }
            set { preferFleetIds = value; }
        }

        private string loadVehiclesBasedOn = "fleet";
        public string LoadVehiclesBasedOn
        {
            get { return loadVehiclesBasedOn; }
            set { loadVehiclesBasedOn = value; }
        }

        private string dateFormat = "";
        public string DateFormat
        {
            get { return dateFormat; }
            set { dateFormat = value; }
        }

        private string timeFormat = "";
        public string TimeFormat
        {
            get { return timeFormat; }
            set { timeFormat = value; }
        }

        private string VehicleGrid;
        public string VGrid
        {
            get { return VehicleGrid; }
            set { VehicleGrid = value; }
        }

        private string HistoryGrid;
        public string HGrid
        {
            get { return HistoryGrid; }
            set { HistoryGrid = value; }
        }

        private string VehicleGridActive;
        public string VGridActive
        {
            get { return VehicleGridActive; }
            set { VehicleGridActive = value; }
        }

        private string HistoryGridActive;
        public string HGridActive
        {
            get { return HistoryGridActive; }
            set { HistoryGridActive = value; }
        }

        private int vehicleNotReported = 3;
        public int VehicleNotReported
        {
            get { return vehicleNotReported; }
            set { vehicleNotReported = value; }
        }

        private string eventColumns;
        public string EventColumns
        {
            get { return eventColumns; }
            set { eventColumns = value; }
        }


        private string violationColumns;
        public string ViolationColumns
        {
            get { return violationColumns; }
            set { violationColumns = value; }
        }

        
        private string recordsToFetch;
        public string RecordsToFetch
        {
            get { return recordsToFetch; }
            set { recordsToFetch = value; }
        }


        private bool showRetiredVehicles = true;
        public bool ShowRetiredVehicles
        {
            get { return showRetiredVehicles; }
            set { showRetiredVehicles = value; }
        }


        private string temperatureType = "Fahrenheit";
        public string TemperatureType
        {
            get { return temperatureType; }
            set { temperatureType = value; }
        }

        public DataSet GetUserFleets(SentinelFMSession sn)
        {

            Stopwatch watch = new Stopwatch();

            DataSet dsFleets = new DataSet();
            StringReader strrXML = null;

            dbu = new ServerDBUser.DBUser();
            objUtil = new clsUtility(sn);

            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " No Fleets for User:" + sn.UserID.ToString() + " Form:clsUser "));
                    return null;
                }


            if (xml == "")
                return null;

            strrXML = new StringReader(xml);
            dsFleets.ReadXml(strrXML);
            watch.Stop();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get user fleets(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.UserID));

            ///----Without OH Filter
            //return dsFleets;
            ///----End Without OH Filter
            ///


            DataView FleetView = dsFleets.Tables[0].DefaultView;
            FleetView.RowFilter = "FleetType<>'oh'";
            DataSet ds = new DataSet();
            DataTable dt = FleetView.ToTable();

            ds.DataSetName = "Fleet";
            ds.Tables.Add(dt);

            return ds;


        }

        public void ExistingPreference(SentinelFMSession sn)
        {
            string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

            try
            {
                dbu = new ServerDBUser.DBUser();
                objUtil = new clsUtility(sn);
                string xml = "";
                StringReader strrXML = null;
                DataSet dsUser = new DataSet();
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                if (objUtil.ErrCheck(dbu.GetUserInfoByUserId(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserInfoByUserId(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserInfoByUserId. User:" + sn.UserID.ToString() + " Form:clsUser "));
                    }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    dsUser.ReadXml(strrXML);

                    sn.User.LastName = dsUser.Tables[0].Rows[0]["LastName"].ToString();
                    sn.User.FirstName = dsUser.Tables[0].Rows[0]["FirstName"].ToString();
                    sn.User.OrganizationName = dsUser.Tables[0].Rows[0]["OrganizationName"].ToString();
                    sn.User.OrganizationId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["OrganizationId"]);
                    sn.User.FleetPulseURL = dsUser.Tables[0].Rows[0]["FleetPulseURL"].ToString();
                    sn.User.UserGroupId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["UserGroupId"].ToString());
                    sn.User.HosEnabled = Convert.ToBoolean(dsUser.Tables[0].Rows[0]["HOSenabled"].ToString());
                    sn.User.ParentUserGroupId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["ParentUserGroupId"].ToString());

                }

                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                DataSet dsGeoCodeEngines = new DataSet();
                DataSet dsMapEngines = new DataSet();

                if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetGeoCodeEnginesInfo. User:" + sn.UserID.ToString() + " Form:clsUser "));
                    }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    dsGeoCodeEngines.ReadXml(strrXML);
                    sn.User.GeoCodeEngine = VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeEngines);
                }
                if (objUtil.ErrCheck(dbs.GetUserMapEngineInfoXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetUserMapEngineInfoXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserMapEngineInfoXML. User:" + sn.UserID.ToString() + " Form:clsUser "));
                    }
                if (xml != "")
                {

                    //xml = "<System><GetUserMapEngineInfo><MapId>8</MapId><Path>" + ConfigurationManager.AppSettings["MapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["MapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";

                    //  xml = "<System><GetUserMapEngineInfo><MapId>8</MapId><Path>" + ConfigurationManager.AppSettings["GeoMicroMapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["GeoMicroMapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";


                    //if (sn.User.OrganizationId == 160 || sn.User.OrganizationId == 300)
                    // xml = "<System><GetUserMapEngineInfo><MapId>9</MapId><Path>" + ConfigurationManager.AppSettings["MapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["MapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";


                    //if (sn.User.OrganizationId == 187 || sn.User.OrganizationId == 277 || sn.User.OrganizationId == 234 || sn.User.OrganizationId == 313)
                    //    xml = "<System><GetUserMapEngineInfo><MapId>5</MapId><Path>" + ConfigurationManager.AppSettings["GeoMicroMapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["GeoMicroMapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";


                    //if (sn.User.OrganizationId == 219)
                    // xml = "<System><GetUserMapEngineInfo><MapId>" + ConfigurationManager.AppSettings["DefaultMapId"] + "</MapId><Path>" + ConfigurationManager.AppSettings["MapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["MapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";

                    //xml = "<System><GetUserMapEngineInfo><MapId>" + ConfigurationManager.AppSettings["DefaultMapId"] + "</MapId><Path>" + ConfigurationManager.AppSettings["GeoMicroMapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["GeoMicroMapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";

                    strrXML = new StringReader(xml);
                    dsMapEngines.ReadXml(strrXML);
                    sn.User.MapEngine = VLF.MAP.MapUtilities.ConvertMapsToMapEngine(dsMapEngines);
                    //sn.User.MapType = (VLF.MAP.MapType)Convert.ToInt16(dsMapEngines.Tables[0].Rows[0]["MapId"]);

                    sn.User.UserMapType = Convert.ToInt16(dsMapEngines.Tables[0].Rows[0]["MapId"]);

                    if (sn.User.UserMapType == (Int16)clsMap.MapEngineType.VirtualEarth)
                        sn.User.IsBingEnabled = true;


                }

                ////Company Logo and home Page
                //xml="";
                //if( objUtil.ErrCheck( dbo.GetOrganizationInfoXMLByUserId     ( sn.UserID ,sn.SecId , ref xml ),false ) )
                //    if( objUtil.ErrCheck( dbo.GetOrganizationInfoXMLByUserId    ( sn.UserID , sn.SecId , ref xml ),true ) )
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"GetOrganizationInfoXMLByUserId. User:"+sn.UserID.ToString()+" Form:clsUser "));    
                //    }

                //if (xml == "")
                //{
                //    sn.User.CompanyLogo=ConfigurationManager.AppSettings["DefaultLogo"];
                //    sn.CompanyURL=ConfigurationManager.AppSettings["DefaultCompanyURL"];  
                //}
                //else
                //{

                //    DataSet dsCompany=new DataSet();
                //    strrXML = new StringReader( xml ) ;
                //    dsCompany.ReadXml (strrXML) ;

                //    //try
                //    //{
                //    //    sn.User.CompanyLogo=dsCompany.Tables[0].Rows[0]["LogoName"].ToString().TrimEnd();     
                //    //}
                //    //catch
                //    //{
                //    //    sn.User.CompanyLogo=ConfigurationManager.AppSettings["DefaultLogo"];
                //    //}

                //    try
                //    {
                //        sn.CompanyURL=dsCompany.Tables[0].Rows[0]["HomePageName"].ToString().TrimEnd();
                //    }
                //    catch
                //    {
                //        sn.CompanyURL=ConfigurationManager.AppSettings["DefaultCompanyURL"];  
                //    }

                //}
                xml = "";
                DataSet dsPref = new DataSet();
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserPreferencesXML. User:" + sn.UserID.ToString() + " Form:clsUser "));
                    }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    dsPref.ReadXml(strrXML);

                    Int16 PreferenceId = 0;

                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {
                        PreferenceId = Convert.ToInt16(rowItem["PreferenceId"]);
                        switch (PreferenceId)
                        {
                            case (Int16)VLF.CLS.Def.Enums.Preference.MeasurementUnits:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.UnitOfMes = Convert.ToDouble(rowItem["PreferenceValue"].ToString().TrimEnd());

                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.VolumeUnits:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.VolumeUnits = Convert.ToDouble(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.TimeZone:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                {
                                    sn.User.TimeZone = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                    sn.User.NewFloatTimeZone = Convert.ToSingle(rowItem["PreferenceValue"].ToString().TrimEnd());
                                    //// Changes for TimeZone feature start
                                    //sn.User.NewFloatTimeZone = Convert.ToSingle(rowItem["PreferenceValue"].ToString().TrimEnd());
                                    //sn.User.TimeZone = Convert.ToInt16(sn.User.NewFloatTimeZone);
                                    //// Changes for TimeZone feature end
                                }
                                break;
                            // Changes for TimeZone feature start
                            case (Int16)VLF.CLS.Def.Enums.Preference.TimeZoneNew:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                { 
                                    sn.User.NewFloatTimeZone = Convert.ToSingle(rowItem["PreferenceValue"].ToString().TrimEnd());

                                }
                                break;
                            // Changes for TimeZone feature end

                            case (Int16)VLF.CLS.Def.Enums.Preference.DefaultFleet:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.DefaultFleet = Convert.ToInt32(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.DayLightSaving:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    sn.User.DayLightSaving = 1;
                                else
                                    sn.User.DayLightSaving = 0;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "0")
                                    sn.Map.ShowVehicleName = false;
                                else
                                    sn.Map.ShowVehicleName = true;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowLandmark:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "0")
                                    sn.Map.ShowLandmark = false;
                                else
                                    sn.Map.ShowLandmark = true;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "0")
                                    sn.Map.ShowLandmarkname = false;
                                else
                                    sn.Map.ShowLandmarkname = true;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowBreadCrumbTrail:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    sn.History.ShowBreadCrumb = true;
                                else
                                    sn.History.ShowBreadCrumb = false;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowStopSequenceNo:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    sn.History.ShowStopSqNum = true;
                                else
                                    sn.History.ShowStopSqNum = false;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowGeoZone:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    sn.Map.ShowGeoZone = true;
                                else
                                    sn.Map.ShowGeoZone = false;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkRadius:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    sn.Map.ShowLandmarkRadius = true;
                                else
                                    sn.Map.ShowLandmarkRadius = false;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.ShowReadMessages:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    sn.User.showReadMess = 1;
                                else
                                    sn.User.showReadMess = 0;
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.AlarmRefreshFrequency:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.AlarmRefreshFrequency = Convert.ToInt32(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.GeneralRefreshFrequency:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.GeneralRefreshFrequency = Convert.ToInt32(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.PositionExpiredTime:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.PositionExpiredTime = Convert.ToInt64(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.MapGridDefaultRows:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.Map.DgVisibleRows = Convert.ToInt32(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.HistoryGridDefaultRows:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.History.DgVisibleRows = Convert.ToInt32(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.ViewMDTMessagesScrolling:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.ViewMDTMessagesScrolling = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.ViewAlarmMessagesScrolling:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.ViewAlarmScrolling = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.ShowMapGridFilter:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.ShowMapGridFilter = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.RememberLastPage:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.RemberLastPage = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            //Vehicle Not Reported
                            case (Int16)VLF.CLS.Def.Enums.Preference.VehicleNotReported:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.VehicleNotReported = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.EventColumns:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.EventColumns = Convert.ToString(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.ViolationColumns:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.ViolationColumns = Convert.ToString(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.TemperatureType:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.TemperatureType = Convert.ToString(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.DefaultMapId:
                                if ((rowItem["PreferenceValue"].ToString().TrimEnd() != "-1") && sn.User.MapEngine[0].MapEngineID.ToString().Contains("Mapsolute"))
                                {
                                    //xml = "<System><GetUserMapEngineInfo><MapId>" + rowItem["PreferenceValue"].ToString().TrimEnd() + "</MapId><Path>" + ConfigurationManager.AppSettings["MapInternalPath"] + "</Path><ExternalPath>" + ConfigurationManager.AppSettings["MapExternalPath"] + "</ExternalPath></GetUserMapEngineInfo></System>";

                                    //dsMapEngines.Clear(); 
                                    //strrXML = new StringReader(xml);
                                    //dsMapEngines.ReadXml(strrXML);

                                    //sn.User.MapEngine = VLF.MAP.MapUtilities.ConvertMapsToMapEngine(dsMapEngines);
                                    // sn.User.MapType = (VLF.MAP.MapType)Convert.ToInt16(dsMapEngines.Tables[0].Rows[0]["MapId"]);
                                    sn.User.UserMapType = Convert.ToInt16(dsMapEngines.Tables[0].Rows[0]["MapId"]);
                                    if (sn.User.UserMapType == (Int16)clsMap.MapEngineType.VirtualEarth)
                                        sn.User.IsBingEnabled = true;

                                }
                                break;


                            case (Int16)VLF.CLS.Def.Enums.Preference.DefaultLanguage:

                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                {
                                    string UserCulture = rowItem["PreferenceValue"].ToString().TrimEnd();
                                    if (UserCulture != "")
                                    {
                                        sn.SelectedLanguage = UserCulture;
                                        HttpContext.Current.Session["PreferredCulture"] = UserCulture;
                                        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);

                                    }
                                }
                                else
                                {
                                    sn.SelectedLanguage = "en-US";
                                }


                                break;

                            case (Int16)VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode:
                                sn.User.PreferNodeCodes = rowItem["PreferenceValue"].ToString().TrimEnd();
                                bool MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                                if (MutipleUserHierarchyAssignment)
                                {
                                    if (sn.User.PreferNodeCodes.Trim() == string.Empty)
                                    {
                                        DataSet dsOH = new DataSet();
                                        dsOH = poh.GetOrganizationHierarchyRootByUserID(sn.User.OrganizationId, sn.UserID, true);
                                        string nodecodes = string.Empty;
                                        foreach (DataRow dr in dsOH.Tables[0].Rows)
                                        {
                                            if (nodecodes != string.Empty)
                                                nodecodes = nodecodes + ",";
                                            nodecodes = nodecodes + dr["NodeCode"].ToString();
                                        }
                                        sn.User.PreferNodeCodes = nodecodes;
                                    }

                                    sn.User.PreferNodeCodes = poh.ValidatedNodeCodes(sn.User.OrganizationId, sn.UserID, sn.User.PreferNodeCodes);
                                    string[] ns = sn.User.PreferNodeCodes.Split(',');
                                    string multipleFleetIds = string.Empty;
                                    foreach (string s in ns)
                                    {
                                        if (s.Trim() != string.Empty)
                                        {
                                            int fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                                            multipleFleetIds = multipleFleetIds + "," + fid.ToString();
                                        }
                                    }
                                    multipleFleetIds = multipleFleetIds.Trim(',');
                                    sn.User.PreferFleetIds = multipleFleetIds;
                                }
                                break;

                            case 36:
                                if (rowItem["PreferenceValue"].ToString().ToLower() == "hierarchy")
                                    sn.User.LoadVehiclesBasedOn = "hierarchy";
                                else
                                    sn.User.LoadVehiclesBasedOn = "fleet";
                                break;

                            case 44:
                                if (rowItem["PreferenceValue"].ToString().Trim() == "")
                                    sn.User.DateFormat = "MM/dd/yyyy";
                                else
                                    sn.User.DateFormat = rowItem["PreferenceValue"].ToString();
                                break;

                            case 45:
                                if (rowItem["PreferenceValue"].ToString().Trim() == "")
                                    sn.User.TimeFormat = "HH:mm:ss";
                                else
                                    sn.User.TimeFormat = rowItem["PreferenceValue"].ToString();
                                break;
                            case (Int16)VLF.CLS.Def.Enums.Preference.ShowRetiredVehicles:
                                sn.User.ShowRetiredVehicles = rowItem["PreferenceValue"].ToString() == "1" ? true : false;
                                break;
                        }

                    }
                }

                if (sn.User.DateFormat == string.Empty)
                {
                    sn.User.DateFormat = "MM/dd/yyyy";                   
                }
                if (sn.User.TimeFormat == string.Empty)
                {
                    sn.User.TimeFormat = "HH:mm:ss";
                }

                xml = "";
                /*if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetOrganizationLandmarksXMLByOrganizationId. User:" + sn.UserID.ToString() + " Form:clsUser "));
                    }
				*/

                DataSet _landmarks = null;

                using (VLF.PATCH.Logic.PatchLandmark pog = new VLF.PATCH.Logic.PatchLandmark(sConnectionString))
                {
                    _landmarks = pog.PatchGetLandmarksInfoByOrganizationIdUserId(sn.User.OrganizationId, sn.UserID);

                    if (VLF.CLS.Util.IsDataSetValid(_landmarks))
                    {
                        xml = clsXmlUtil.GetXmlIncludingNull(_landmarks);
                    }
                }

                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    DataSet ds = new DataSet();
                    ds.ReadXml(strrXML);
                    sn.DsLandMarks = ds;
                }
                else
                {
                    sn.DsLandMarks = null;
                }

                if (sn.User.UserGroupId == 27)
                    sn.DsLandMarks = null;

                //sn.DsLandMarks = null;

                //xml = "";


                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                        {
                            //return;
                        }
                }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    DataSet ds = new DataSet();
                    ds.ReadXml(strrXML);

                    Int16 PreferenceId = 0;

                    foreach (DataRow rowItem in ds.Tables[0].Rows)
                    {
                        PreferenceId = Convert.ToInt16(rowItem["OrgPreferenceId"]);
                        switch (PreferenceId)
                        {
                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.HeaderColor:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.MenuColor = rowItem["PreferenceValue"].ToString().TrimEnd();
                                break;

                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.ConfTabBackGround:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.ConfigTabBackColor = rowItem["PreferenceValue"].ToString().TrimEnd();
                                break;

                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.TempZone:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.TemperatureZone = rowItem["PreferenceValue"].ToString().TrimEnd();
                                break;

                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.HomePagePicture:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.HomePagePicture = rowItem["PreferenceValue"].ToString().TrimEnd();
                                break;

                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.InterfacePreference:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "" && clsUtility.IsNumeric(rowItem["PreferenceValue"].ToString().TrimEnd()))
                                    sn.InterfacePrefrence = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.ResovleLSDAddress:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.isLSDEnabled = Convert.ToBoolean(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case (Int16)VLF.CLS.Def.Enums.OrganizationPreference.SMSsupport:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    sn.User.SmsSupport = Convert.ToBoolean(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;

                            case 44:
                                if (sn.User.DateFormat.Trim() == "")
                                {
                                    if (rowItem["PreferenceValue"].ToString().Trim() == "")
                                        sn.User.DateFormat = "";
                                    else
                                        sn.User.DateFormat = rowItem["PreferenceValue"].ToString();
                                }
                                break;

                            case 45:
                                if (sn.User.TimeFormat.Trim() == "")
                                {
                                    if (rowItem["PreferenceValue"].ToString().Trim() == "")
                                        sn.User.TimeFormat = "";
                                    else
                                        sn.User.TimeFormat = rowItem["PreferenceValue"].ToString();
                                }
                                break;
                        }
                    }
                }

                xml = "";
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationColumnsPreferences(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationColumnsPreferences(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                        {
                            return;
                        }
                }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    DataSet ds = new DataSet();
                    ds.ReadXml(strrXML);

                    Int16 PreferenceId = 0;

                    foreach (DataRow rowItem in ds.Tables[0].Rows)
                    {
                        PreferenceId = Convert.ToInt16(rowItem["GridId"]);
                        switch (PreferenceId)
                        {
                            case 1:
                                if (rowItem["ColumnsList"].ToString().TrimEnd() != "")
                                    sn.User.VGrid = "," + rowItem["ColumnsList"].ToString() + ",";
                                else
                                    sn.User.VGrid = "all";
                                if (rowItem["ColumnsActiveList"].ToString().TrimEnd() != "")
                                    sn.User.VGridActive = rowItem["ColumnsActiveList"].ToString() + ",";
                                else
                                    sn.User.VGridActive = "";
                                break;

                            case 2:
                                if (rowItem["ColumnsList"].ToString().TrimEnd() != "")
                                    sn.User.HGrid = rowItem["ColumnsList"].ToString() + ",";
                                else
                                    sn.User.HGrid = "all";
                                if (rowItem["ColumnsActiveList"].ToString().TrimEnd() != "")
                                    sn.User.HGridActive = rowItem["ColumnsActiveList"].ToString() + ",";
                                else
                                    sn.User.HGridActive = "";
                                break;

                        }
                    }
                }
                if (sn.User.DateFormat.Trim() == "" || sn.User.TimeFormat.Trim() == "")
                {
                    try
                    {
                        DataSet dsInfo = null;
                        dbu = new ServerDBUser.DBUser();
                        if (objUtil.ErrCheck(dbu.GetDefaultDateTimeFormats(sn.UserID, sn.User.OrganizationId, sn.SecId, ref dsInfo), false))
                        {
                            if (objUtil.ErrCheck(dbu.GetDefaultDateTimeFormats(sn.UserID, sn.User.OrganizationId, sn.SecId, ref dsInfo), true))
                            {
                            }
                        }
                        if (dsInfo != null)
                        {
                            if (sn.User.DateFormat.Trim() == "")
                                sn.User.DateFormat = dsInfo.Tables[0].Rows[0]["DefDateFormat"].ToString(); // dsInfo.Tables[0].Select("Type= 'Default Date'").CopyToDataTable().Rows[0]["Format"].ToString();
                            if (sn.User.TimeFormat.Trim() == "")
                                sn.User.TimeFormat = dsInfo.Tables[0].Rows[0]["DefTimeFormat"].ToString();//dsInfo.Tables[0].Select("Type= 'Default Time'").CopyToDataTable().Rows[0]["Format"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                
                xml = "";
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSensorPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSensorPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), true))
                        {
                            return;
                        }
                }
                if (xml != "")
                {
                    DataSet dsSensor = new DataSet();
                    strrXML = new StringReader(xml);
                    dsSensor.ReadXml(strrXML);
                    sn.History.DsAssSensors = "";
                    foreach (DataRow r in dsSensor.Tables[0].Rows)
                    {
                        sn.History.DsAssSensors += r[0] + ",";
                    }
                    sn.History.DsAssSensors = sn.History.DsAssSensors.Substring(0, sn.History.DsAssSensors.LastIndexOf(","));
                }
            }

            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsUser "));
            }
        }

        public bool ControlEnable(SentinelFMSession sn, int ControlId)
        {
            try
            {
                bool ControlStatus = false;

                if (sn.User.DsGUIControls != null)
                {
                    foreach (DataRow rowItem in sn.User.DsGUIControls.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(rowItem["ControlId"]) == Convert.ToInt32(ControlId))
                        {
                            ControlStatus = true;
                            break;
                        }
                    }
                }

                if (ControlId == 42 && sn.User.organizationId == 952 && sn.User.UserGroupId == 36)  // hardcoded that G4S NCC user (NCC user group) could see the landmark menu.
                    ControlStatus = true;

                return ControlStatus;
            }
            catch
            {
                return false;
            }
        }

        public void GetGuiControlsInfo(SentinelFMSession sn)
        {
            string xml = "";

            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(dbs.GetUserControls(sn.UserID, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbs.GetUserControls(sn.UserID, sn.SecId, ref xml), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:clsUser "));
                    return;
                }

            if (xml == "")
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:clsUser "));
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);
            sn.User.DsGUIControls = ds;

        }

        static public void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        //public void DashBoardPreferences(SentinelFMSession sn, int JoinId, ref int Threshold, ref bool GridView, ref int FleetId, ref int PeriodId)
        //{
        //    string xml = "";
        //    DataSet ds = new DataSet();
        //    objUtil = new clsUtility(sn);

        //    using (ServerDBUser.DBUser dbUser = new ServerDBUser.DBUser())
        //    {

        //        if (objUtil.ErrCheck(dbUser.GetDashboardUserAssignment(sn.UserID, sn.SecId, ref xml), false))
        //            if (objUtil.ErrCheck(dbUser.GetDashboardUserAssignment(sn.UserID, sn.SecId, ref xml), true))
        //            {
        //                return;
        //            }

        //        if (xml == "")
        //            return;

        //        StringReader strrXML = new StringReader(xml);
        //        ds.ReadXml(strrXML);

        //    }

        //    DataRow[] drArr = ds.Tables[0].Select("JoinId='" + JoinId + "'");
        //    if (drArr == null || drArr.Length == 0)
        //        return;

        //    Threshold = Convert.ToInt32(drArr[0]["Threshold"]);
        //    GridView = Convert.ToBoolean(drArr[0]["GridView"]);
        //    FleetId = Convert.ToInt32(drArr[0]["FleetId"]);
        //    PeriodId = Convert.ToInt32(drArr[0]["PeriodId"]);
        //}

        public clsUser()
        {
            //
            // TODO: Add constructor logic here
            //
        }


    }
}
