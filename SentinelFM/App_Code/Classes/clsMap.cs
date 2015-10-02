using System;
using System.Data;
using System.IO;
using VLF.MAP;
using VLF.CLS;
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.SessionState;

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using SentinelFM.GeomarkServiceRef;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for clsMap.
    /// </summary>
    public class clsMap
    {
        private int selectedFleetID = 0;
        private Int64 selectedVehicleID = 0;
        private string selectedMultiFleetIDs = string.Empty;

        private double xInCoord = 0;
        private double yInCoord = 0;

        private double xEndCoord = 0;
        private double yEndCoord = 0;
        private bool drawAllVehicles = false;
	private string lastKnownVehicleXML = string.Empty;

        private double mapScale = 0;
        private double imageDistance = 0;
        private int selectedZoomLevelType = 0;
        private bool timerStatus = false;
        private bool mapRefresh = false;
        private bool selectAllVehciles = false;
        private bool reloadMap = false;
        private bool showVehicleName = true;
        private bool showLandmark = true;
        private bool showLandmarkname = true;
        private bool showLandmarkRadius = false;
        private bool showGeoZone = false;
        private bool showDefaultMap = false;
        private bool landmarksOnMap = false;
        private DataSet dsFleetInfo = null;
	private DataSet dsFleetInfoNew = null;
    private DataSet dsLastChangedVehicles = null;
        private DataTable dtSetupSensors = null;
        private string vehiclesToolTip = "";
        private string vehiclesMappings = "";
        private string imgPath = "";
        private string defaultimgPath = "";
        private VLF.MAP.GeoPoint southWestCorner;
        private VLF.MAP.GeoPoint northEastCorner;
        private string alarmsHTML = "";
	private string alarmsXML = "";

        private DateTime lastStatusCheckedAt = DateTime.MinValue;



        private int alarmCount = 0;
        private string messagesHTML = "";
	private string messagesXML = "";

        private int messagesCount = 0;
        private bool mapResize = false;
        private int[] disabledLSDLayers = null;
        private bool reloadVirtualEarthData = true;
        private string avlMapVE = "";
        private string landmarksMapVE = "";
        private string geozonesMapVE = "";
        private string editMapVE = "false";
        private string closeBigMap = "true";
        protected clsUtility objUtil;

        ManualResetEvent mre = new ManualResetEvent(false);
        byte[] bytes = null;
        int numberOfBytesRead = 0;
        int readpos = 0;



        public enum MapEngineType
        {
            NotSet = 0,
            GeoMicro = 1,
            MapPointWeb = 2,
            GeoMicroWeb = 3,
            MapPoint = 4,
            LSD = 5,
            MapsoluteWeb = 6,
            MapsoluteStatic = 7,
            GeoMicroRemoting = 8,
            VirtualEarth = 9,
            Telogis = 10,
        }

        public string LastKnownXML
        {
            get { return lastKnownVehicleXML; }
            set { lastKnownVehicleXML = value; }
        }

        public DateTime LastStatusChecked
        {
            get { return lastStatusCheckedAt; }
            set { lastStatusCheckedAt = value; }
        }

        public string CloseBigMap
        {
            get { return closeBigMap; }
            set { closeBigMap = value; }
        }

        public string EditMapVE
        {
            get { return editMapVE; }
            set { editMapVE = value; }
        }

        public string AvlMapVE
        {
            get { return avlMapVE; }
            set { avlMapVE = value; }
        }

        public string GeozonesMapVE
        {
            get { return geozonesMapVE; }
            set { geozonesMapVE = value; }
        }

        public string LandmarksMapVE
        {
            get { return landmarksMapVE; }
            set { landmarksMapVE = value; }
        }


        public bool ReloadVirtualEarthData
        {
            get { return reloadVirtualEarthData; }
            set { reloadVirtualEarthData = value; }
        }

        public int[] DisabledLSDLayers
        {
            get { return disabledLSDLayers; }
            set { disabledLSDLayers = value; }
        }

        public string AlarmsHTML
        {
            get { return alarmsHTML; }
            set { alarmsHTML = value; }
        }

	public string AlarmsXML
        {
            get { return alarmsXML; }
            set { alarmsXML = value; }
        }

        public int AlarmCount
        {
            get { return alarmCount; }
            set { alarmCount = value; }
        }

        public string MessagesHTML
        {
            get { return messagesHTML; }
            set { messagesHTML = value; }
        }
	
	public string MessagesXML
        {
            get { return messagesXML; }
            set { messagesXML = value; }
        }

        public int MessagesCount
        {
            get { return messagesCount; }
            set { messagesCount = value; }
        }

        public bool LandmarksOnMap
        {
            get { return landmarksOnMap; }
            set { landmarksOnMap = value; }
        }


        public VLF.MAP.GeoPoint SouthWestCorner
        {
            get { return southWestCorner; }
            set { southWestCorner = value; }
        }

        public VLF.MAP.GeoPoint NorthEastCorner
        {
            get { return northEastCorner; }
            set { northEastCorner = value; }
        }


        public bool SelectAllVehciles
        {
            get { return selectAllVehciles; }
            set { selectAllVehciles = value; }
        }

        public bool ShowDefaultMap
        {
            get { return showDefaultMap; }
            set { showDefaultMap = value; }
        }


        public Int64 SelectedVehicleID
        {
            get { return selectedVehicleID; }
            set { selectedVehicleID = value; }
        }

        public bool ShowVehicleName
        {
            get { return showVehicleName; }
            set { showVehicleName = value; }
        }


        public bool ShowLandmark
        {
            get { return showLandmark; }
            set { showLandmark = value; }
        }

        public bool ShowGeoZone
        {
            get { return showGeoZone; }
            set { showGeoZone = value; }
        }

        public bool ShowLandmarkRadius
        {
            get { return showLandmarkRadius; }
            set { showLandmarkRadius = value; }
        }


        public bool ShowLandmarkname
        {
            get { return showLandmarkname; }
            set { showLandmarkname = value; }
        }

        public string ImgPath
        {
            get { return imgPath; }
            set { imgPath = value; }
        }


        public string DefaultImgPath
        {
            get { return defaultimgPath; }
            set { defaultimgPath = value; }
        }

        public string VehiclesToolTip
        {
            get { return vehiclesToolTip; }
            set { vehiclesToolTip = value; }
        }


        public string VehiclesMappings
        {
            get { return vehiclesMappings; }
            set { vehiclesMappings = value; }
        }

        public bool DrawAllVehicles
        {
            get { return drawAllVehicles; }
            set { drawAllVehicles = value; }
        }

        public bool MapRefresh
        {
            get { return mapRefresh; }
            set { mapRefresh = value; }
        }


        public bool ReloadMap
        {
            get { return reloadMap; }
            set { reloadMap = value; }
        }


        public bool MapResize
        {
            get { return mapResize; }
            set { mapResize = value; }
        }

        public DataSet DsFleetInfo
        {
            get { return dsFleetInfo; }
            set { dsFleetInfo = value; }
        }

    public DataSet DsFleetInfoNew
        {
            get { return dsFleetInfoNew; }
            set { dsFleetInfoNew = value; }
        }

    public DataSet DsLastChangedVehicles
    {
        get { return dsLastChangedVehicles; }
        set { dsLastChangedVehicles = value; }
    }



        public DataTable SetupSensors
        {
            get { return dtSetupSensors; }
            set { dtSetupSensors = value; }
        }




        public double XInCoord
        {
            get { return xInCoord; }
            set { xInCoord = value; }
        }

        public double YInCoord
        {
            get { return yInCoord; }
            set { yInCoord = value; }
        }


        public double XEndCoord
        {
            get { return xEndCoord; }
            set { xEndCoord = value; }
        }


        public double YEndCoord
        {
            get { return yEndCoord; }
            set { yEndCoord = value; }
        }

        public double MapScale
        {
            get { return mapScale; }
            set { mapScale = value; }
        }

        public double ImageDistance
        {
            get { return imageDistance; }
            set { imageDistance = value; }
        }

        public bool TimerStatus
        {
            get { return timerStatus; }
            set { timerStatus = value; }
        }

        public int SelectedFleetID
        {
            get { return selectedFleetID; }
            set { selectedFleetID = value; }
        }

        public string SelectedMultiFleetIDs
        {
            get { return selectedMultiFleetIDs; }
            set { selectedMultiFleetIDs = value; }
        }


        public int SelectedZoomLevelType
        {
            get { return selectedZoomLevelType; }
            set { selectedZoomLevelType = value; }
        }


        private Int32 dgVisibleRows = 5;
        public Int32 DgVisibleRows
        {
            get { return dgVisibleRows; }
            set { dgVisibleRows = value; }
        }


        private Int32 dgItemsPerPage = 25;
        public Int32 DgItemsPerPage
        {
            get { return dgItemsPerPage; }
            set { dgItemsPerPage = value; }
        }



        private int screenWidth = 0;
        public int ScreenWidth
        {
            get { return screenWidth; }
            set { screenWidth = value; }
        }


        private int screenHeight = 0;
        public int ScreenHeight
        {
            get { return screenHeight; }
            set { screenHeight = value; }
        }

        private bool mapSearch = false;
        public bool MapSearch
        {
            get { return mapSearch; }
            set { mapSearch = value; }
        }


        private string alarmsCheckSum = "";
        public string AlarmsCheckSum
        {
            get { return alarmsCheckSum; }
            set { alarmsCheckSum = value; }
        }

        // Changes for TimeZone Feature start

        public void LoadAlarmsXML_NewTZ(SentinelFMSession sn, string lang, ref string strAlarmXML, ref int AlarmCount)
        {

            string xml = "";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, lang, ref xml), false))
                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, lang, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);

            AlarmCount = 0;

            StringBuilder _alarmXML = new StringBuilder();

            _alarmXML.Append("<Alarm>");

            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {
                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString() && ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
                {
                    _alarmXML.Append("<AllUserAlarmsInfo>");
                    _alarmXML.Append("<AlarmId>" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "</AlarmId>");
                    _alarmXML.Append("<TimeCreated>" + ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd() + "</TimeCreated>");
                    //Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString()
                    _alarmXML.Append("<AlarmState>" + ds.Tables[0].Rows[i]["AlarmState"].ToString() + "</AlarmState>");
                    _alarmXML.Append("<AlarmLevel>" + ds.Tables[0].Rows[i]["AlarmLevel"].ToString() + "</AlarmLevel>");
                    _alarmXML.Append("<vehicleDescription>" + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "</vehicleDescription>");
                    _alarmXML.Append("<AlarmDescription>" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(";", "<br>") + "</AlarmDescription>");
                    _alarmXML.Append("</AllUserAlarmsInfo>");
                    AlarmCount++;
                }
            }
            _alarmXML.Append("</Alarm>");
            strAlarmXML = _alarmXML.ToString();
        }

        // Changes for TimeZone Feature end

 	public void LoadAlarmsXML(SentinelFMSession sn, string lang, ref string strAlarmXML, ref int AlarmCount)
        {

            string xml = "";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, lang, ref xml), false))
                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, lang, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);

            AlarmCount = 0;

            StringBuilder _alarmXML = new StringBuilder();

            _alarmXML.Append("<Alarm>");      

            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {
                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString() && ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
                {
                    _alarmXML.Append("<AllUserAlarmsInfo>");
                    _alarmXML.Append("<AlarmId>" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "</AlarmId>");
                    _alarmXML.Append("<TimeCreated>" + ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd() + "</TimeCreated>");
                    //Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString()
                    _alarmXML.Append("<AlarmState>" + ds.Tables[0].Rows[i]["AlarmState"].ToString() + "</AlarmState>");
                    _alarmXML.Append("<AlarmLevel>" + ds.Tables[0].Rows[i]["AlarmLevel"].ToString() + "</AlarmLevel>");
                    _alarmXML.Append("<vehicleDescription>" + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "</vehicleDescription>");
                    _alarmXML.Append("<AlarmDescription>" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(";", "<br>") + "</AlarmDescription>");
                    _alarmXML.Append("</AllUserAlarmsInfo>");
                    AlarmCount++;
                }   
            }
            _alarmXML.Append("</Alarm>");
            strAlarmXML = _alarmXML.ToString();            
        }


        public void DSFleetInfoGenerator(SentinelFMSession sn, ref DataSet dsFleetInfo)
        {
            objUtil = new clsUtility(sn);
            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            string strUnitOfMes = sn.User.UnitOfMes == 1 ? " km/h" : " mi/h";
            string[] addresses = null;
            string strStreetAddress = "";

            #region Custom columns
            // PTO
            DataColumn dc = new DataColumn("chkPTO", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            dsFleetInfo.Tables[0].Columns.Add(dc);


            //// Command Status (Update Position)
            //dc = new DataColumn("Updated", Type.GetType("System.Int16"));
            //dc.DefaultValue = false;
            //dsFleetInfo.Tables[0].Columns.Add(dc);


            ////Last Communicated
            //DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
            //dsFleetInfo.Tables[0].Columns.Add(colDateTime);


            //// Vehicle Status
            //dc = new DataColumn("VehicleStatus", Type.GetType("System.String"));
            //dc.DefaultValue = "";
            //dsFleetInfo.Tables[0].Columns.Add(dc);

            //// Vehicle Description
            //dc = new DataColumn("VehicleDesc", Type.GetType("System.String"));
            //dc.DefaultValue = "";
            //dsFleetInfo.Tables[0].Columns.Add(dc);

            //// Show Heading
            //dc = new DataColumn("MyHeading", Type.GetType("System.String"));
            //dc.DefaultValue = "";
            //dsFleetInfo.Tables[0].Columns.Add(dc);


            //// Send Command ProtocolId
            //dc = new DataColumn("ProtocolId", Type.GetType("System.Int16"));
            //dc.DefaultValue = -1;
            //dsFleetInfo.Tables[0].Columns.Add(dc);


            //// Current Status
            //dc = new DataColumn("CurrentStatus", Type.GetType("System.String"));
            //dc.DefaultValue = "";
            //dsFleetInfo.Tables[0].Columns.Add(dc);


            //// Custom Speed
            //dc = new DataColumn("CustomSpeed", Type.GetType("System.String"));
            //dc.DefaultValue = "";
            //dsFleetInfo.Tables[0].Columns.Add(dc);


            //// CustomUrl

            //dc = new DataColumn();
            //dc.ColumnName = "CustomUrl";
            //dc.DataType = Type.GetType("System.String");
            //dc.DefaultValue = "";
            //dsFleetInfo.Tables[0].Columns.Add(dc);

            //// Box
            //dc = new DataColumn("IntBoxId", Type.GetType("System.Int32"));
            //dsFleetInfo.Tables[0].Columns.Add(dc);


            ////Street Address
            //if (dsFleetInfo.Tables[0].Columns.IndexOf("StreetAddress") == -1)
            //{
            //    DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
            //    dsFleetInfo.Tables[0].Columns.Add(colStreetAddress);
            //}

            #endregion

            #region Resolve street address in Batch
            // Disabled this resolve address feature, it threw exception for a long time: No connection could be made because the target machine actively refused it 192.168.9.46:9090 
            // And we changed to another way to resolve address: only resolve address for the vehicles in current page at map page, not to resolve address for all vehicles, it will take too long in some cases.
            //try
            //{
            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Start -- addresses "));

            //    DataRow[] drArrAddress = dsFleetInfo.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

            //    if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
            //        addresses = ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
            //    else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroRemoting")) && drArrAddress.Length > 0)
            //        addresses = ResolveStreetAddressGeoMicroRemotingBatch(drArrAddress);
            //    else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
            //        addresses = ResolveStreetAddressGeoMicroBatch(drArrAddress);

            //    int i = 0;
            //    foreach (DataRow dr in drArrAddress)
            //    {
            //        dr["StreetAddress"] = addresses[i];
            //        i++;
            //    }

            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No addresses #:" + i.ToString()));

            //}
            //catch (Exception Ex)
            //{
            //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
            //}
            #endregion

            foreach (DataRow rowItem in dsFleetInfo.Tables[0].Rows)
            {
                #region PTO


                UInt64 intSensorMask = 0;
                try
                {
                    intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                }
                catch
                {
                }

                UInt64 checkBit = 0x80;
                //check bit for PTO
                if ((intSensorMask & checkBit) != 0 && sn.User.OrganizationId != 343)
                    rowItem["chkPTO"] = "True";
                else
                    rowItem["chkPTO"] = "False";



                #endregion
                #region Store Selected Vehicles
                if (sn.Map.DsFleetInfo != null)
                {
                    DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId='" + rowItem["VehicleId"].ToString() + "'");
                    if (drArr != null && drArr.Length > 0)
                        rowItem["chkBoxShow"] = drArr[0]["chkBoxShow"]; //chkBoxShow
                }
                #endregion

                #region Data Casting // To do - HARD TYPE DATASET
                ////BoxId
                //rowItem["IntBoxId"] = Convert.ToInt32(rowItem["BoxId"]);
                //// OriginDateTime
                //rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"]).ToString(sn.User.DateFormat+" "+sn.User.TimeFormat);
                ////Vehicle Description
                //rowItem["Description"] = rowItem["Description"].ToString().Replace("'", "");
                //rowItem["VehicleDesc"] = rowItem["Description"].ToString(); 
                #endregion

                #region Street Address
                 //Get Street address - if not was resolved in batch
                strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
                if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
                {
                   try
                   {
                         //rowItem["StreetAddress"] = ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                       //rowItem["StreetAddress"] = ResolveStreetAddressTelogis(rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                       
                   }
                   catch (Exception Ex)
                   {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
                   }

                }


                switch (strStreetAddress)
                {
                    case VLF.CLS.Def.Const.addressNA:
                        rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                        break;

                    case VLF.CLS.Def.Const.noGPSData:
                        rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                        break;

                  
                    default:
                        break;
                }

                #endregion

                #region Custom data
                ////CustomUrl 
                //rowItem["CustomUrl"] = "javascript:var w =SensorsInfo('" + rowItem["LicensePlate"].ToString() + "')";
                //// Custom Speed
                //rowItem["CustomSpeed"] = rowItem["Speed"].ToString() + strUnitOfMes;
                //// Heading
                //if (Convert.ToDouble(rowItem["Speed"]) != 0 &&
                //  (rowItem["Heading"] != null) &&
                //  (rowItem["Heading"].ToString() != ""))
                //{
                //    rowItem["CustomSpeed"] += ", " + Heading(rowItem["Heading"].ToString());
                //    rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                //}
                #endregion

                #region Vehicle Status

                //if (Convert.ToInt16(rowItem["VehicleTypeId"].ToString().TrimEnd()) != Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.CarSecurity))
                //{
                //    short vehicleType = Convert.ToInt16(rowItem["VehicleTypeId"].ToString().TrimEnd());
                //    string Speed = rowItem["Speed"].ToString();
                //    bool trailer = (vehicleType == Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.XS_Trailer) ||
                //                     vehicleType == Convert.ToInt16(VLF.CLS.Def.Enums.VehicleType.Trailer));
                //    if (!Convert.ToBoolean(rowItem["LastStatusSensor"])) // Ignition OFF /Untethered
                //    {
                //        rowItem["VehicleStatus"] = trailer ? Resources.Const.VehicleStatus_Untethered :
                //                       Resources.Const.VehicleStatus_Parked;

                //        rowItem["CustomSpeed"] = "0 " + strUnitOfMes;
                //        rowItem["Speed"] = "0";
                //    }
                //    else if (Convert.ToBoolean(rowItem["LastStatusSensor"])) // Ignition ON / Tethered
                //    {
                //        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                //        {
                //            rowItem["VehicleStatus"] = trailer ? Resources.Const.VehicleStatus_Tethered :
                //                                             Resources.Const.VehicleStatus_Ignition_ON;
                //        }
                //        else
                //        {
                //            if ((Speed != "0") && (Speed != ""))
                //                rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_Moving;
                //            else if (Speed == "0")
                //            {
                //                rowItem["VehicleStatus"] = trailer ? Resources.Const.VehicleStatus_Tethered :
                //                                                               Resources.Const.VehicleStatus_Idling;

                //                rowItem["CurrentStatus"] = Resources.Const.VehicleStatus_NA;
                //            }
                //            else // Speed==""
                //            {
                //                rowItem["VehicleStatus"] = "";
                //            }
                //        }
                //    }
                //    else // Speed and Status are not set
                //    {
                //        rowItem["VehicleStatus"] = "";
                //    }

                //    try
                //    {
                //        if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                //        {
                //            rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_NA;
                //            rowItem["CurrentStatus"] = Resources.Const.VehicleStatus_NA;
                //        }

                //        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                //            rowItem["VehicleStatus"] += "*";

                //    }
                //    catch
                //    {
                //    }

                //    if (Convert.ToBoolean(rowItem["Dormant"]))
                //        rowItem["VehicleStatus"] = Resources.Const.VehicleStatus_PowerSave;

                //}
                #endregion
            }
        }


        public string Heading(string heading)
        {
            int intHeading = 0;
            try
            {
                intHeading = Convert.ToInt16(heading);

            }
            catch
            {
                return "";
            }


            try
            {
                if (intHeading > 400)
                    return "";


                if ((intHeading >= 337) || (intHeading < 22))
                    return "N";

                if ((intHeading >= 22) && (intHeading < 67))
                    return "NE";

                if ((intHeading >= 67) && (intHeading < 112))
                    return "E";


                if ((intHeading >= 112) && (intHeading < 157))
                    return "SE";

                if ((intHeading >= 157) && (intHeading < 202))
                    return "S";


                if ((intHeading >= 202) && (intHeading < 247))
                    return "SW";


                if ((intHeading >= 247) && (intHeading < 292))
                    return "W";


                if ((intHeading >= 292) && (intHeading < 337))
                    return "NW";
                else
                    return "";
            }
            catch 
            {
                return "";
            }



        }



        public void SavesMapStateToViewState(SentinelFMSession sn, ClientMapProxy map)
        {
            sn.Map.XInCoord = map.MapCenterLongitude;
            sn.Map.YInCoord = map.MapCenterLatitude;
            sn.Map.MapScale = map.MapScale;
            sn.Map.ImageDistance = map.ImageDistance;
            sn.Map.DrawAllVehicles = map.DrawAllVehicles;
            sn.Map.SouthWestCorner = map.SouthWestCorner;
            sn.Map.NorthEastCorner = map.NorthEastCorner;

        }


        public void DrawLandmarks(SentinelFMSession sn, ref ClientMapProxy map)
        {
            //if (sn.Map.ShowLandmark)
            //{
            if ((sn.DsLandMarks != null) &&
                (sn.DsLandMarks.Tables.Count > 0) &&
                (sn.DsLandMarks.Tables[0].Rows.Count > 0))
            {
                foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                {
                    if (sn.Map.ShowLandmarkname && sn.Map.ShowLandmarkRadius)
                        map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                    else if (sn.Map.ShowLandmarkname)
                        map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                    else if (sn.Map.ShowLandmarkRadius)
                        map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", ""));
                    else if (sn.Map.ShowLandmark)
                        map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", ""));

                }
            }
            //}
        }

        public void RetrievesMapStateFromViewState(SentinelFMSession sn, ClientMapProxy map)
        {


            map.Vehicles.Clear();
            map.Landmarks.Clear();
            map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
            map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;

            //set Show checkboxes
            if ((sn.Map.DsFleetInfo != null) &&
                (sn.Map.DsFleetInfo.Tables.Count > 0) &&
                (sn.Map.DsFleetInfo.Tables[0].Rows.Count > 0))
            {

                foreach (DataRow dr in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (dr["chkBoxShow"].ToString().ToLower() == "true")
                    {

                        if (Convert.ToDateTime(dr["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                        {
                            map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Grey" + dr["IconTypeName"].ToString().TrimEnd() + ".ico", dr["Description"].ToString().TrimEnd()));
                        }
                        else
                        {


                            if (Convert.ToBoolean(dr["chkPTO"]))
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Blue" + dr["IconTypeName"].ToString().TrimEnd() + ".ico", dr["Description"].ToString().TrimEnd()));
                            else if (dr["Speed"].ToString() != "0")
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + dr["IconTypeName"].ToString().TrimEnd() + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["Description"].ToString().TrimEnd()));
                            else
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + dr["IconTypeName"].ToString().TrimEnd() + ".ico", dr["Description"].ToString().TrimEnd()));
           
                        }

                    }
                }
            }


            map.MapCenterLongitude = Convert.ToDouble(sn.Map.XInCoord);
            map.MapCenterLatitude = Convert.ToDouble(sn.Map.YInCoord);
            map.MapScale = sn.Map.MapScale;
            map.SouthWestCorner = sn.Map.SouthWestCorner;
            map.NorthEastCorner = sn.Map.NorthEastCorner;


            map.ImageDistance = sn.Map.ImageDistance;

            if ((map.MapCenterLongitude == 0) && (map.MapCenterLatitude == 0))
            {
                map.DrawAllVehicles = true;
            }
            else
            {
                map.DrawAllVehicles = Convert.ToBoolean(sn.Map.DrawAllVehicles);
                map.MapScale = Convert.ToDouble(sn.Map.MapScale);
            }



            DrawLandmarks(sn, ref map);
        }

        public void CreateVehiclesTooltip(SentinelFMSession sn, ClientMapProxy map)
        {


            sn.Map.VehiclesToolTip = "";
            sn.Map.VehiclesMappings = "";


            //Table Vehicles with coordinates
            DataTable tblToolTips = new DataTable();
            DataRow tblRow;

            DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
            tblToolTips.Columns.Add(colDescription);
            DataColumn colX = new DataColumn("colX", Type.GetType("System.Double"));
            tblToolTips.Columns.Add(colX);
            DataColumn colY = new DataColumn("colY", Type.GetType("System.Double"));
            tblToolTips.Columns.Add(colY);

            
            //Create Vehicles Tooltip description
            for (int i = 0; i < map.Vehicles.Count; i++)
            {
                if ((map.Vehicles[i].X != -1) && (map.Vehicles[i].Y != -1))
                {

                    DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("Description='" + map.Vehicles[i].IconLabel.ToString().TrimEnd().Replace("'", "''") + "'");
                    if (drArr == null || drArr.Length == 0)
                        continue;
                    DataRow rowItem = drArr[0];


                    //foreach(DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    //{
                    //   if (rowItem["Description"].ToString().TrimEnd()==map.Vehicles[i].IconLabel.ToString().TrimEnd())
                    //   {

                    //Tooltip description
                    if (sn.Map.VehiclesToolTip.Length == 0)
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"].ToString()) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            sn.Map.VehiclesToolTip += "<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]: Position Old</B><BR> <FONT style='TEXT-DECORATION: underline'  >" + Resources.Const.map_Address + ":</FONT>" + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><BR> <FONT style='TEXT-DECORATION: underline'  > " + Resources.Const.map_Address + ":</FONT>" + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' >" + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";

                    }
                    else
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"].ToString()) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            sn.Map.VehiclesToolTip += "\"<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]: Position Old</B><BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Address + ":</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "\"<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Address + ":</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";
                    }

                    if (sn.User.IsLSDEnabled && rowItem["LSD"].ToString().Trim()!="")
                        sn.Map.VehiclesToolTip = sn.Map.VehiclesToolTip.Substring(0, sn.Map.VehiclesToolTip.Length - 2) + "<FONT style='TEXT-DECORATION: underline' > LSD:</FONT>" + rowItem["LSD"].ToString().Trim() + "<BR>\",";

                    if (map.Vehicles.Count > 0)
                    {
                        tblRow = tblToolTips.NewRow();
                        tblRow["Description"] = map.Vehicles[i].IconLabel.Replace("'", "''");
                        tblRow["colX"] = map.Vehicles[i].X;
                        tblRow["colY"] = map.Vehicles[i].Y;
                        tblToolTips.Rows.Add(tblRow);
                    }
                    //      break;
                    //   }
                    //}
                }
            }



            if (sn.Map.ShowLandmark)
            {
                //Create Landmark Tooltip description
                for (int i = 0; i < map.Landmarks.Count; i++)
                {


                    if ((map.Landmarks[i].X != -1) && (map.Landmarks[i].Y != -1))
                    {
                        if (sn.Map.VehiclesToolTip.Length == 0)
                            sn.Map.VehiclesToolTip += "<FONT style='COLOR: #084f7f'>" + "<FONT style='TEXT-DECORATION: underline' >" + Resources.Const.map_Landmark + ":" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT><BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "\"<FONT style='COLOR: #084f7f'>" + "<FONT style='TEXT-DECORATION: underline' >" + Resources.Const.map_Landmark + ":" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT><BR>\",";


                        tblRow = tblToolTips.NewRow();
                        tblRow["Description"] = map.Landmarks[i].IconLabel.Replace("'", "''");
                        tblRow["colX"] = map.Landmarks[i].X;
                        tblRow["colY"] = map.Landmarks[i].Y;
                        tblToolTips.Rows.Add(tblRow);
                    }
                }
            }


            if ((sn.Map.VehiclesToolTip.Length > 0) && (sn.Map.VehiclesToolTip.Substring(sn.Map.VehiclesToolTip.Length - 1, 1) == ","))
                sn.Map.VehiclesToolTip = sn.Map.VehiclesToolTip.Substring(0, sn.Map.VehiclesToolTip.Length - 2);


            //Create ToolTip Matrix
            System.Collections.SortedList Matrix = new System.Collections.SortedList();
            int x = 0;
            int y = 0;
            int key = 0;
            for (int i = 0; i < tblToolTips.Rows.Count; i++)
            {
                x = Convert.ToInt32(Convert.ToInt32(tblToolTips.Rows[i]["colX"]) / 10);
                y = Convert.ToInt32(Convert.ToInt32(tblToolTips.Rows[i]["colY"]) / 10);
                key = (++x) * 100 + (++y);
                if (!Matrix.ContainsKey(key))
                {
                    Matrix.Add(key, i + 1);
                }
                else
                {
                    Matrix[key] += "|" + (i + 1);
                }
            }


            double StartX = 0;
            double EndX = 0;
            double StartY = 0;
            double EndY = 0;
            string strVehiclesList = "";
            string areaMaps = "<script type=\"text/javascript\">";

            foreach (int MatrixKey in Matrix.Keys)
            {
                x = (MatrixKey / 100 - 1) * 10;
                y = (MatrixKey % 100 - 1) * 10;

                StartX = x - 8;
                EndX = x + 8;
                StartY = y - 8;
                EndY = y + 8;

                strVehiclesList = Matrix[MatrixKey].ToString().Replace("'", "''");

                /*

                    sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";

        */
                areaMaps += "locationMap[\"" + strVehiclesList + "\"] = [" + string.Format("{0},{1},{2},{3}", StartX, StartY, EndX, EndY) + "];";

            }
areaMaps += "</script>";
            sn.Map.VehiclesMappings = areaMaps;
        }

        // Changes for TimeZone Feature start

        public void LoadAlarms_NewTZ(SentinelFMSession sn, string lang, ref string strAlarmHTML, ref int AlarmCount)
        {

            string xml = "";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, lang, ref xml), false))
                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, lang, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);
            string strStyle = "";



            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {

                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
                    strStyle = "style='{color:green;}'";
                else if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                    strStyle = "style='{color:#C0C0C0;}'";
                else if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                   && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
                    strStyle = "style='{color:DarkGoldenrod;}'";
                else if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                   && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                    strStyle = "style='{color:red;}'";


                // --- Hide closed alarms
                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                {
                    //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");

                    if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                       && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                        strAlarmHTML = strAlarmHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
                    else
                        strAlarmHTML = strAlarmHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                    AlarmCount++;
                }
            }
        }

        // Changes for TimeZone Feature end

        public void LoadAlarms(SentinelFMSession sn, string lang, ref string strAlarmHTML, ref int AlarmCount)
        {

            string xml = "";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, lang, ref xml), false))
                if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, lang, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);
            string strStyle = "";



            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {

                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
                    strStyle = "style='{color:green;}'";
                else if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                    strStyle = "style='{color:#C0C0C0;}'";
                else if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                   && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
                    strStyle = "style='{color:DarkGoldenrod;}'";
                else if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                   && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                    strStyle = "style='{color:red;}'";


                // --- Hide closed alarms
                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                {
                    //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");

                    if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                       && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                        strAlarmHTML = strAlarmHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
                    else
                        strAlarmHTML = strAlarmHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                    AlarmCount++;
                }
            }
        }

        // Changes for TimeZone Feature start
        public void LoadMessages_NewTZ(SentinelFMSession sn, ref string strMessagesHTML)
        {

            StringReader strrXML = null;


            string xml = "";
            string strFromDT = "";
            string strToDT = "";


            strFromDT = DateTime.Now.AddHours(-24 - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
            strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

            ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), false))
                if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);


            string strStyle = "";
            sn.Map.MessagesCount = 0;

            //Unread Messages
            DataRow[] drCollections = null;
            drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
            foreach (DataRow rowItem in drCollections)
            {
                strStyle = "style='{color:#000066;}'";
                strMessagesHTML = strMessagesHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
            }

            if (drCollections != null)
                sn.Map.MessagesCount = drCollections.Length;

            //Read Messages
            if (sn.User.ShowReadMess == 1)
            {
                ds.Tables[0].Select();
                drCollections = ds.Tables[0].Select("UserId<>-1", "", DataViewRowState.CurrentRows);

                foreach (DataRow rowItem in drCollections)
                {
                    strStyle = "style='{color:green;}'";
                    strMessagesHTML = strMessagesHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
                }

                if (drCollections != null)
                    sn.Map.MessagesCount = sn.Map.MessagesCount + drCollections.Length;
            }
        }


        // Changes for TimeZone Feature end


        public void LoadMessages(SentinelFMSession sn, ref string strMessagesHTML)
        {

            StringReader strrXML = null;


            string xml = "";
            string strFromDT = "";
            string strToDT = "";


            strFromDT = DateTime.Now.AddHours(-24 - sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
            strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

            ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
            objUtil = new clsUtility(sn);
            if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT,  ref xml), false))
                if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            strrXML = new StringReader(xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);


            string strStyle = "";
            sn.Map.MessagesCount = 0;

            //Unread Messages
            DataRow[] drCollections = null;
            drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
            foreach (DataRow rowItem in drCollections)
            {
                strStyle = "style='{color:#000066;}'";
                strMessagesHTML = strMessagesHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
            }

            if (drCollections != null)
                sn.Map.MessagesCount = drCollections.Length;

            //Read Messages
            if (sn.User.ShowReadMess == 1)
            {
                ds.Tables[0].Select();
                drCollections = ds.Tables[0].Select("UserId<>-1", "", DataViewRowState.CurrentRows);

                foreach (DataRow rowItem in drCollections)
                {
                    strStyle = "style='{color:green;}'";
                    strMessagesHTML = strMessagesHTML + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
                }

                if (drCollections != null)
                    sn.Map.MessagesCount = sn.Map.MessagesCount + drCollections.Length;
            }
        }


        static public string ResolveStreetAddress(SentinelFMSession sn, string Latitude, string Longitude)
        {

            string tmpAdr = "";

            try
            {

                GeoPoint geoPoint = new GeoPoint();
                geoPoint.Latitude = Convert.ToDouble(Latitude);
                geoPoint.Longitude = Convert.ToDouble(Longitude);

                // create ClientMapProxy only for geocoding
                VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(sn.User.GeoCodeEngine);
                if (geoMap == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form: clsMap"));
                    return "";
                }
                tmpAdr = geoMap.GetStreetAddress(geoPoint);

                return tmpAdr;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
                return "";
            }
        }



        static public string[] ResolveStreetAddressGeoMicroWebBatch(DataRow[] drArrAddress)
        {

            string[] address = null;

            try
            {
                double[] lat = new double[drArrAddress.Length];
                double[] lot = new double[drArrAddress.Length];
                int i = 0;
                foreach (DataRow dr in drArrAddress)
                {
                    lat[i] = Convert.ToDouble(dr["Latitude"]);
                    lot[i] = Convert.ToDouble(dr["Longitude"]);
                    i++;
                }

                GeoMicroWebProxy geomap = new GeoMicroWebProxy("");
                address = geomap.GetStreetAddresses(lat, lot);

                return address;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " Form:clsMap"));
                return null;
            }


        }



        static public string[] ResolveStreetAddressGeoMicroRemotingBatch(DataRow[] drArrAddress)
        {

            string[] address = null;

            try
            {
                double[] lat = new double[drArrAddress.Length];
                double[] lot = new double[drArrAddress.Length];
                int i = 0;
                foreach (DataRow dr in drArrAddress)
                {
                    lat[i] = Convert.ToDouble(dr["Latitude"]);
                    lot[i] = Convert.ToDouble(dr["Longitude"]);
                    i++;
                }

                VLF.MAP.RemoteGeoMicroProxy geomap = new VLF.MAP.RemoteGeoMicroProxy("");
                address = geomap.GetStreetAddresses(lat, lot);

                return address;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " Form:clsMap"));
                return null;
            }


        }


        static public string[] ResolveStreetAddressGeoMicroBatch(DataRow[] drArrAddress)
        {

            string[] address = null;

            try
            {
                double[] lat = new double[drArrAddress.Length];
                double[] lot = new double[drArrAddress.Length];
                int i = 0;
                foreach (DataRow dr in drArrAddress)
                {
                    lat[i] = Convert.ToDouble(dr["Latitude"]);
                    lot[i] = Convert.ToDouble(dr["Longitude"]);
                    i++;
                }

                VLF.MAP.GeoMicroProxy geomap = new VLF.MAP.GeoMicroProxy("http://65.110.160.135/");
                address = geomap.GetStreetAddresses(lat, lot);

                return address;

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " Form:clsMap"));
                return null;
            }


        }




        static public void AdjustMapURL(bool URLSecure, ref string url)
        {
            //string[] tmp;
            //SentinelFMSession sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            //if (sn.User.MapEngine[0].MapEngineID.ToString() != "MapPointWeb")
            //    return;

            //if ((URLSecure) && (url != ""))
            //{
            //    tmp = url.Split('/');
            //    url = ConfigurationManager.AppSettings["MapRelativePath"] + tmp[tmp.Length - 1];
            //}


            if ((URLSecure) && (url != ""))
                url = url.Replace("http", "https");
            
        }



        public void ResolveCooridnatesByAddressTelogis(string address, ref double X, ref double Y, ref string resolvedAddress)
        {

            try
            {

                Resolver.Resolver res = new Resolver.Resolver();
                res.Location(address, ref Y, ref X, ref resolvedAddress);


                // TcpClient c = new TcpClient();
                //// c.Connect(new IPEndPoint(IPAddress.Parse("192.168.9.46"), 44080));
                // c.Connect(new IPEndPoint(Dns.GetHostAddresses("alt.socketstream.sentinelfm.com")[0], 44080));
                // string resultAddress = "";
                // if (c.Connected)
                // {

                //     mre.Reset();
                //     bytes = new byte[2048];
                //     numberOfBytesRead = 0;
                //     readpos = 0;
                //     byte[] data = System.Text.Encoding.ASCII.GetBytes(address);

                //     using (NetworkStream stream = c.GetStream())
                //     {
                //         try
                //         {
                //             stream.ReadTimeout = 5000;
                //             stream.Write(data, 0, data.Length);
                //             if (stream.CanRead)
                //             {
                //                 mre.Reset();
                //                 stream.BeginRead(bytes, readpos, bytes.Length, new AsyncCallback(TcpReadCallback), stream);
                //                 mre.WaitOne();
                //             }
                //             resultAddress = System.Text.Encoding.ASCII.GetString(bytes, 0, readpos);
                //         }
                //         catch
                //         {

                //         }
                //         finally
                //         {
                //             stream.Close();
                //         }
                //     }
                // }


                // if (resultAddress != "")
                // {
                //     string[] tmpAdrr1 = resultAddress.Split('|');
                //     string[] Adrr = tmpAdrr1[0].Split(';');
                //     resolvedAddress = Adrr[0].ToString(); 
                //     X = Convert.ToDouble(Adrr[2]);
                //     Y =  Convert.ToDouble(Adrr[1]);
                // }




            }
            catch
            {
                X = 0;
                Y = 0;
            }
        }




        public string ResolveStreetAddressTelogis(string Latitude, string Longitude)
        {


            try
            {
               // //string streetAddress = VLF3.GeoCoding.RemoteSocketGeoCoder.Get.ResolveStreetAddressTelogis(Latitude, Longitude);

               // string streetAddress = "";
               // //Resolver.Resolver res = new Resolver.Resolver();
               // //res.StreetAddress(Convert.ToDouble(Latitude), Convert.ToDouble(Longitude), ref streetAddress);
                
               // // Changed to use Navteq service to get address instead of Telogis
               // //GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");
               // //Dictionary<string, string> d = _lc.ReverseGeoCoder(double.Parse(Longitude), double.Parse(Latitude));
               //// streetAddress = d["StreetAddress"];


               // GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");

               // GeoCoderNavteqResp d = _lc.NavteqReverseGeoCoder(double.Parse(Latitude), double.Parse(Longitude));
               // streetAddress = d.Address + "," + d.CountryName;
               // _lc.Close();

               // //string streetAddress = "";
               // //string coord = Latitude + "," + Longitude + ";";
               // //TcpClient c = new TcpClient();
               // ////c.Connect(new IPEndPoint(IPAddress.Parse("192.168.9.46"), 44080));
               // //c.Connect(new IPEndPoint(Dns.GetHostAddresses("socketstream.sentinelfm.com")[0], 44080));
               // //if (c.Connected)
               // //{
               // //    mre.Reset();
               // //    bytes = new byte[2048];
               // //    numberOfBytesRead = 0;
               // //    readpos = 0;
               // //    byte[] data = System.Text.Encoding.ASCII.GetBytes(coord);

               // //    using (NetworkStream stream = c.GetStream())
               // //    {
               // //        try
               // //        {
               // //            stream.ReadTimeout = 5000;
               // //            stream.Write(data, 0, data.Length);
               // //            if (stream.CanRead)
               // //            {
               // //                mre.Reset();
               // //                stream.BeginRead(bytes, readpos, bytes.Length, new AsyncCallback(TcpReadCallback), stream);
               // //                mre.WaitOne();
               // //            }
               // //             streetAddress = System.Text.Encoding.ASCII.GetString(bytes, 0, readpos);
               // //        }
               // //        catch 
               // //        {

               // //        }
               // //        finally
               // //        {
               // //            stream.Close();
               // //        }
               // //    }
               // //}

               // return streetAddress;

                return ResolveStreetAddressNavteq(Latitude, Longitude);

            }
            catch
            {
                return "";
            }
        }

        public string ResolveStreetAddressNavteq(string Latitude, string Longitude)
        {


            try
            {
                string streetAddress = "";
                
                GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");

                GeoCoderNavteqResp d = _lc.NavteqReverseGeoCoder(double.Parse(Latitude), double.Parse(Longitude));
                streetAddress = d.Address;
                _lc.Close();

                return streetAddress;

            }
            catch
            {
                return "";
            }
        }


        public void TcpReadCallback(IAsyncResult ar)
        {

            NetworkStream stream = (NetworkStream)ar.AsyncState;
            numberOfBytesRead = stream.EndRead(ar);
            readpos += numberOfBytesRead;
            while (stream.DataAvailable)
            {
                stream.BeginRead(bytes, readpos, bytes.Length, new AsyncCallback(TcpReadCallback), stream);
            }
            mre.Set();
        }

        public clsMap()
        {
        }
    }
}
