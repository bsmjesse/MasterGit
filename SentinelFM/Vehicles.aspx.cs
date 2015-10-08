using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using ClosedXML.Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using VLF.CLS.Def;
using VLF.DAS.Logic;
using SentinelFM.GeomarkServiceRef;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SentinelFM
{
    public partial class Vehicles : BaseAjaxPage
    {
        protected SentinelFMSession sn = null;

        public string _xml = "";
        protected clsUtility objUtil;

        private int vlStart;
        private int vlLimit;
        private string filters;
        private string operation;
        private string formattype;

        private string sConnectionString;
        private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

        public bool MutipleUserHierarchyAssignment;

        private bool showDashboardView = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

            sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

            if (sn.User.OrganizationId == 123 || sn.User.OrganizationId == 480)
            {
                showDashboardView = true;
            }

            //MutipleUserHierarchyAssignment = false;
            if (!Page.IsPostBack)
            {
                var request = Request.QueryString["QueryType"];
                if (string.IsNullOrEmpty(request))
                {
                    request = "GetVehiclePosition";
                    sn.Map.LastKnownXML = string.Empty; //Get everything again may be we lost session
                }
                if (request.Equals("GetVehiclePosition", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetVehiclePosition();
                }
                else if (request.Equals("GetAllFleets", StringComparison.CurrentCultureIgnoreCase))
                {
                    //sn.Map.LastKnownXML = string.Empty;
                    Fleets_Fill();
                }
                else if (request.Equals("GetfleetPosition", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetFleetPosition();
                }
                else if (request.Equals("getClosestVehicles", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (GetClosestVehicles()) return;
                }
                else if (request.Equals("getVehiclesInLandmark", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (GetVehiclesInLandmark()) return;
                }
                else if (request.Equals("searchHistoryAddress", StringComparison.CurrentCultureIgnoreCase))
                {
                    SearchHistoryAddress();
                }
                else if (request.Equals("getVehicleInfo_NewTZ", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetVehicleInfo();
                }
                else if (request.Equals("getBoxInfo", StringComparison.CurrentCultureIgnoreCase))
                {
                    var boxId = 0;
                    int.TryParse(Request.QueryString["BoxId"], out boxId);
                    GetBoxInfo(boxId);
                }

                else if (request.Equals("GetFilteredFleet", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetFilteredFleet();
                }
                else if (request.Equals("GetAllFleetForMap", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetAllFleetForMap();
                }
                else if (request.Equals("resolveaddress", StringComparison.CurrentCultureIgnoreCase))
                {
                    ResolveAddress();
                }
                else if (request.Equals("ListVehiclesInLandmarksForDashboard", StringComparison.CurrentCultureIgnoreCase))
                {
                    var landmarkCategoryId = 0;
                    int.TryParse(Request.QueryString["landmarkCategoryId"], out landmarkCategoryId);
                    ListVehiclesInLandmarksForDashboard(landmarkCategoryId);
                }
                else if (request.Equals("GetLandmarkDuration", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetLandmarkDuration();
                }
                else if (request.Equals("getVehilcesByLandmarkId", StringComparison.CurrentCultureIgnoreCase))
                {
                    var landmarkId = 0;
                    int.TryParse(Request.QueryString["landmarkId"], out landmarkId);
                    GetVehilcesByLandmarkId(landmarkId);
                }
            }


            ShowTimeElapsed();
        }

        private void ResolveAddress()
        {
            var rBoxId = 0;
            float rLat = 0;
            float rLon = 0;
            int.TryParse(Request.QueryString["boxId"], out rBoxId);
            float.TryParse(Request.QueryString["lat"], out rLat);
            float.TryParse(Request.QueryString["lon"], out rLon);
            ResolveAddress(rBoxId, rLat, rLon);
        }

        private void GetFilteredFleet()
        {
            var request = Request.QueryString["filters"];
            filters = !string.IsNullOrEmpty(request) ? request : string.Empty;

            vlStart = 0;
            vlLimit = 10000;
            request = Request.QueryString["start"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out vlStart);
                if (vlStart < 0) vlStart = 0;
            }

            request = Request.QueryString["limit"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out vlLimit);
                if (vlLimit <= 0) vlStart = vlLimit;
            }


            request = Request.QueryString["operation"];
            operation = !string.IsNullOrEmpty(request) ? request : "";

            request = Request.QueryString["formattype"];
            formattype = !string.IsNullOrEmpty(request) ? request : "";

            getFilteredFleet();
        }

        private void GetLandmarkDuration()
        {
            var serviceConfigId = 0;
            int.TryParse(Request.QueryString["serviceConfigId"], out serviceConfigId);

            long vehicleId = 0;
            long.TryParse(Request.QueryString["vehicleId"], out vehicleId);

            long landmarkId = 0;
            long.TryParse(Request.QueryString["landmarkId"], out landmarkId);

            var boxId = 0;
            int.TryParse(Request.QueryString["boxId"], out boxId);


            GetLandmarkDuration(serviceConfigId, vehicleId, landmarkId, boxId);
        }

        private void GetVehicleInfo()
        {
            var vehicleId = 0;
            int.TryParse(Request.QueryString["vehicleId"], out vehicleId);
            getVehicleInfo_NewTZ(vehicleId);
        }

        private void SearchHistoryAddress()
        {
            var ifSearchHistoryAddressByWebservice = ConfigurationManager.AppSettings["SearchHistoryAddressByWebservice"];
            //searchHistoryAddress();
            if (ifSearchHistoryAddressByWebservice == null || ifSearchHistoryAddressByWebservice == "0")
                searchHistoryAddressByDasLogic_NewTZ();
            else
                searchHistoryAddressByWebservice_NewTZ();
        }

        private bool GetVehiclesInLandmark()
        {
            var fleetId = 0;
            var request = Request.QueryString["fleetID"] ?? string.Empty;
            int.TryParse(request, out fleetId);

            long landmarkId = 0;
            request = Request.QueryString["landmarkId"] ?? string.Empty;
            long.TryParse(request, out landmarkId);

            if (fleetId <= 0)
            {
                return true;
            }

            getVehiclesInLandmark(fleetId, landmarkId);
            return false;
        }

        private bool GetClosestVehicles()
        {
            var fleetId = 0;
            double lon = 0;
            double lat = 0;
            var radius = 5;
            var numofvehicles = 10;
            var request = Request.QueryString["fleetID"] ?? string.Empty;
            int.TryParse(request, out fleetId);
            if (fleetId <= 0)
            {
                return true;
            }
            request = Request.QueryString["lon"] ?? string.Empty;
            double.TryParse(request, out lon);
            request = Request.QueryString["lat"] ?? string.Empty;
            double.TryParse(request, out lat);
            request = Request.QueryString["radius"] ?? string.Empty;
            int.TryParse(request, out radius);
            request = Request.QueryString["numofvehicles"] ?? string.Empty;
            int.TryParse(request, out numofvehicles);

            getClosestVehicles_NewTZ(fleetId, lon, lat, numofvehicles, radius);
            return false;
        }

        private void GetFleetPosition()
        {
            var request = Request.QueryString["fleetID"];
            if (MutipleUserHierarchyAssignment)
            {
                sn.Map.SelectedMultiFleetIDs = request;
                sn.Map.LastKnownXML = string.Empty;
            }
            else
            {
                if (!string.IsNullOrEmpty(request))
                {
                    var fleetID = 0;
                    int.TryParse(request, out fleetID);
                    if (fleetID > 0)
                    {
                        sn.Map.SelectedFleetID = fleetID;
                        sn.Map.LastKnownXML = string.Empty;
                    }
                }
            }

            vlStart = 0;
            vlLimit = 10000;
            request = Request.QueryString["start"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out vlStart);
                if (vlStart < 0) vlStart = 0;
            }

            request = Request.QueryString["limit"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out vlLimit);
                if (vlLimit <= 0) vlStart = vlLimit;
            }

            FleetVehicles_Fill_NewTZ();
        }

        private void GetVehiclePosition()
        {
            var request = Request.QueryString["fleetID"];
            if (!string.IsNullOrEmpty(request))
            {
                if (MutipleUserHierarchyAssignment)
                {
                    sn.Map.SelectedMultiFleetIDs = request;
                    sn.Map.LastKnownXML = string.Empty;
                }
                else
                {
                    var fleetId = 0;
                    int.TryParse(request, out fleetId);
                    if (fleetId > 0)
                    {
                        sn.Map.SelectedFleetID = fleetId;
                        sn.Map.LastKnownXML = string.Empty;
                    }
                }
            }

            request = Request.QueryString["filters"];
            filters = !string.IsNullOrEmpty(request) ? request : string.Empty;

            vlStart = 0;
            vlLimit = 10000;
            request = Request.QueryString["start"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out vlStart);
                if (vlStart < 0) vlStart = 0;
            }

            request = Request.QueryString["limit"];
            if (!string.IsNullOrEmpty(request))
            {
                int.TryParse(request, out vlLimit);
                if (vlLimit <= 0) vlStart = vlLimit;
            }

            VehicleList_Fill_NewTZ();
        }

        private void Fleets_Fill()
        {
            var xml = "";
            var dbf = new ServerDBFleet.DBFleet();
            objUtil = new clsUtility(sn);

            var dsFleets = sn.User.GetUserFleets(sn);
            xml = dsFleets.GetXml();

            if (xml == "")
                return;

            Response.ContentType = "text/xml";
            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
            //xml = Encoding.UTF8.GetString(data);
            Response.Write(xml.Trim());
        }

        // Changes for TimeZone Feature start
        private void FleetVehicles_Fill_NewTZ()
        {
            try
            {
                StringReader strrXML = null;
                var xml = "";
                var dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                var fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID));
                sn.Map.LastStatusChecked = DateTime.UtcNow;
                //DateTime.Now;

                DataSet dsVehiclesInfo = null;

                var lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");

                if (MutipleUserHierarchyAssignment)
                {
                    var dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString);

                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByMultipleFleets_NewTZ(sn.Map.SelectedMultiFleetIDs, sn.UserID, lng);
                    dbFleet.Dispose();
                    if (dsVehiclesInfo.Tables[0].Rows.Count > 0)
                        sn.Map.LastKnownXML = "<data>yes</data>";
                }
                else
                {
                    var dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString);

                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo_NewTZ(fleetId, sn.UserID, lng);
                    dbFleet.Dispose();
                    if (dsVehiclesInfo.Tables[0].Rows.Count > 0)
                        sn.Map.LastKnownXML = "<data>yes</data>";

                }

                if (string.IsNullOrEmpty(xml) && dsVehiclesInfo == null)
                {
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        getXmlFromDs(sn.Map.DsFleetInfoNew, -1);
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml.Trim());

                    var strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    var dsFleetInfo = new DataSet();

                    if (dsVehiclesInfo == null)
                    {

                        dsFleetInfo.ReadXmlSchema(strPath);
                        dsFleetInfo.ReadXml(strrXML);
                    }
                    else
                    {
                        dsFleetInfo = dsVehiclesInfo.Copy();
                        //Adding  PTO to Dataset
                        var dc = new DataColumn("PTO", Type.GetType("System.String")) { DefaultValue = "Off" };
                        dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                    }

                    if (showDashboardView)
                    {
                        DataColumn dc;
                        //if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalState"))
                        //{
                        //    dc = new DataColumn("OperationalState", Type.GetType("System.String"));
                        //    dc.DefaultValue = "";
                        //    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        //}

                        //if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateNotes"))
                        //{
                        //    dc = new DataColumn("OperationalStateNotes", Type.GetType("System.String"));
                        //    dc.DefaultValue = "";
                        //    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        //}

                        if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkEventId"))
                        {
                            dc = new DataColumn("LandmarkEventId", Type.GetType("System.Int64")) { DefaultValue = 0 };
                            dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        }

                        if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                        {
                            dc = new DataColumn("OperationalStateName", Type.GetType("System.String"));
                            dc.DefaultValue = "";
                            dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        }

                        if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("DurationInLandmarkMin"))
                        {
                            dc = new DataColumn("DurationInLandmarkMin", Type.GetType("System.Int32"));
                            dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        }

                        if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkInDateTime"))
                        {
                            dc = new DataColumn("LandmarkInDateTime", Type.GetType("System.DateTime"));
                            dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        }

                        if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkID"))
                        {
                            dc = new DataColumn("LandmarkID", Type.GetType("System.Int32"));
                            dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        }

                        if (!dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkName"))
                        {
                            dc = new DataColumn("LandmarkName", Type.GetType("System.String"));
                            dc.DefaultValue = "";
                            dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                        }
                    }


                    foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                    {
                        var _m = new clsMap();

                        if (row["StreetAddress"].ToString().Trim() == "")
                        {

                            row["StreetAddress"] = VLF.CLS.Def.Const.addressNA;//"~~~~~~~~~~~~~~~~~~~~~~~";
                        }

                        //Geting and Inserting PTO Value
                        ulong intSensorMask = 0;
                        try
                        {
                            intSensorMask = Convert.ToUInt64(row["SensorMask"]);
                        }
                        catch
                        {
                        }
                        ulong checkBit = 0x80;
                        //check bit for PTO
                        if ((intSensorMask & checkBit) != 0)
                            row["PTO"] = "On";
                        else
                            row["PTO"] = "Off";

                        if (showDashboardView && dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                        {
                            switch (row["OperationalState"].ToString())
                            {
                                case "100":
                                    row["OperationalStateName"] = "Available";
                                    break;
                                case "200":
                                    row["OperationalStateName"] = "Unavailable";
                                    break;
                                default:
                                    row["OperationalStateName"] = "";
                                    break;
                            }
                        }
                    }
                    sn.Map.DsFleetInfoNew = dsFleetInfo;

                    var request = Request.QueryString["mergeData"];
                    if (showDashboardView && !string.IsNullOrEmpty(request) && request.ToLower() == "VehiclesInLandmarks".ToLower())
                    {
                        mergeLandarksToVehicleList();
                    }

                    //xml = dsFleetInfo.GetXml();                        
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        //xml = dsFleetInfo.GetXml();
                        getXmlFromDs(dsFleetInfo, -1);
                    }
                    else
                    {
                        var dstemp = new DataSet();
                        var dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                        dv.Sort = "OriginDateTime DESC";
                        var sortedTable = dv.ToTable();
                        var dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehiclesLastKnownPositionInformation";

                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                var _m = new clsMap();
                                row["StreetAddress"] = _m.ResolveStreetAddressNavteq(row["Latitude"].ToString(), row["Longitude"].ToString());

                                var filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                                var foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                                foreach (DataRow foundRow in foundRows)
                                {
                                    try
                                    {
                                        var index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRow);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorMessage =
                                            VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                                                ex.StackTrace);
                                        LogErrors(ex, errorMessage);
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, errorMessage);
                                    }
                                }
                            }
                        }

                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "Fleet";
                        //xml = dstemp.GetXml();
                        //xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count.ToString() + "</totalCount>");
                        getXmlFromDs(dstemp, dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count);
                    }
                    //}
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    //xml = xml.Replace("&#x0", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone()); 
                    ////byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    ////xml = Encoding.UTF8.GetString(data);
                    //if (lng == "fr")
                    //{
                    //    xml = xml.Replace("<BoxArmed>true</BoxArmed>", "<BoxArmed>voir</BoxArmed>");
                    //    xml = xml.Replace("<BoxArmed>false</BoxArmed>", "<BoxArmed>faux</BoxArmed>");
                    //}
                    //Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");
            }
        }
        // Changes for TimeZone Feature end

        private void FleetVehicles_Fill()
        {
            try
            {
                StringReader strrXML = null;
                var xml = "";
                var dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                //Response.ContentEncoding = Encoding.Default;
                Response.ContentEncoding = Encoding.UTF8;
                var fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                    //Convert.ToInt32(Convert.ToInt32(sn.Map.SelectedFleetID));
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID));
                sn.Map.LastStatusChecked = DateTime.UtcNow;
                //DateTime.Now;

                DataSet dsVehiclesInfo = null;

                var lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");

                if (MutipleUserHierarchyAssignment)
                {
                    var dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString);

                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByMultipleFleets(sn.Map.SelectedMultiFleetIDs, sn.UserID, lng);
                    dbFleet.Dispose();
                    if (dsVehiclesInfo.Tables[0].Rows.Count > 0)
                        sn.Map.LastKnownXML = "<data>yes</data>";
                }
                else
                {
                    var dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString);

                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, sn.UserID, lng);
                    dbFleet.Dispose();
                    if (dsVehiclesInfo.Tables[0].Rows.Count > 0)
                        sn.Map.LastKnownXML = "<data>yes</data>";

                }

                if (string.IsNullOrEmpty(xml) && dsVehiclesInfo == null)
                {
                    if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {

                        getXmlFromDs(sn.Map.DsFleetInfoNew, -1);
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml.Trim());

                    var strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    var dsFleetInfo = new DataSet();


                    dsFleetInfo = dsVehiclesInfo.Copy();
                    //Adding  PTO to Dataset
                    var dc = new DataColumn("PTO", Type.GetType("System.String"));
                    dc.DefaultValue = "Off";
                    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);

                    foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                    {
                        var _m = new clsMap();
                        //if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                        //{
                        //    row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                        //}
                        if (row["StreetAddress"].ToString().Trim() == "")
                        {
                            //var x = row["Latitude"].ToString();
                            //var y = row["Longitude"].ToString();
                            row["StreetAddress"] = VLF.CLS.Def.Const.addressNA;//"~~~~~~~~~~~~~~~~~~~~~~~";
                        }

                        //Geting and Inserting PTO Value
                        ulong intSensorMask = 0;
                        intSensorMask = Convert.ToUInt64(row["SensorMask"]);

                        ulong checkBit = 0x80;
                        //check bit for PTO
                        if ((intSensorMask & checkBit) != 0)
                            row["PTO"] = "On";
                        else
                            row["PTO"] = "Off";
                    }
                    sn.Map.DsFleetInfoNew = dsFleetInfo;
                    //xml = dsFleetInfo.GetXml();                        
                    if (vlStart == 0 && vlLimit == 10000)
                    {
                        //xml = dsFleetInfo.GetXml();
                        getXmlFromDs(dsFleetInfo, -1);
                    }
                    else
                    {
                        var dstemp = new DataSet();
                        var dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                        dv.Sort = "OriginDateTime DESC";
                        var sortedTable = dv.ToTable();
                        var dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        dt.TableName = "VehiclesLastKnownPositionInformation";
                        dstemp.Tables.Add(dt);
                        dstemp.DataSetName = "Fleet";
                        //xml = dstemp.GetXml();
                        //xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count.ToString() + "</totalCount>");
                        getXmlFromDs(dstemp, dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows.Count);
                    }
                    //}
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    //xml = xml.Replace("&#x0", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone()); 
                    ////byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    ////xml = Encoding.UTF8.GetString(data);
                    //if (lng == "fr")
                    //{
                    //    xml = xml.Replace("<BoxArmed>true</BoxArmed>", "<BoxArmed>voir</BoxArmed>");
                    //    xml = xml.Replace("<BoxArmed>false</BoxArmed>", "<BoxArmed>faux</BoxArmed>");
                    //}
                    //Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.StackTrace));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + ex.Message + ex.StackTrace + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");
            }
        }
        // Changes for TimeZone Feature start
        private void VehicleList_Fill_NewTZ()
        {
            try
            {
                StringReader strrXML = null;
                var xml = "";
                var dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                var lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");//CultureInfo.CurrentUICulture.TwoLetterISOLanguageName

                var fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                    //Convert.ToInt32(Convert.ToInt32(sn.Map.SelectedFleetID));
                }
                else if (sn.User.DefaultFleet != -1)
                {
                    fleetId = Convert.ToInt32(sn.User.DefaultFleet);
                    //sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                }

                var multipleFleetIds = string.Empty;
                if (MutipleUserHierarchyAssignment)
                {
                    if (sn.Map.SelectedMultiFleetIDs != string.Empty)
                    {
                        multipleFleetIds = sn.Map.SelectedMultiFleetIDs;
                    }
                    else if (sn.User.PreferNodeCodes != string.Empty)
                    {
                        var ns = sn.User.PreferNodeCodes.Split(',');

                        foreach (var s in ns)
                        {
                            var fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                            multipleFleetIds = multipleFleetIds + "," + fid;
                        }
                        multipleFleetIds = multipleFleetIds.Trim(',');
                        sn.User.PreferFleetIds = multipleFleetIds;
                    }
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID));
                DataSet dsVehiclesInfo = null;
                var dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString);

                if (string.IsNullOrEmpty(sn.Map.LastKnownXML) || sn.Map.LastStatusChecked == null)
                {
                    sn.Map.LastStatusChecked = DateTime.UtcNow;
                    //DateTime.Now;

                    if (MutipleUserHierarchyAssignment)
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByMultipleFleets_NewTZ(multipleFleetIds, sn.UserID, lng);
                        //if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang(sn.UserID, sn.SecId, multipleFleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        //    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang(sn.UserID, sn.SecId, multipleFleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        //    {
                        //        sn.Map.DsFleetInfoNew = null;
                        //        sn.Map.LastKnownXML = string.Empty;
                        //        return;
                        //    }
                    }
                    else
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo_NewTZ(fleetId, sn.UserID, lng);
                        //if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        //    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        //    {
                        //        sn.Map.DsFleetInfoNew = null;
                        //        sn.Map.LastKnownXML = string.Empty;
                        //        return;
                        //    }
                    }

                    if (dsVehiclesInfo != null && dsVehiclesInfo.Tables[0].Rows.Count > 0)
                        sn.Map.LastKnownXML = "<data>yes</data>";

                    if (!string.IsNullOrEmpty(xml))
                    {
                        xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                }
                else
                {
                    var lastCheckedTime = DateTime.UtcNow;

                    if (MutipleUserHierarchyAssignment)
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByMultipleFleetsByLang_NewTZ(multipleFleetIds, sn.UserID, lng, sn.Map.LastStatusChecked);
                        //if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByMultipleFleetsByLang(sn.UserID, sn.SecId, multipleFleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), false))
                        //    if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByMultipleFleetsByLang(sn.UserID, sn.SecId, multipleFleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), true))
                        //    {
                        //        sn.Map.DsFleetInfoNew = null;
                        //        sn.Map.LastKnownXML = string.Empty;
                        //        return;
                        //    }
                    }
                    else
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByLang_NewTZ(fleetId, sn.UserID, lng, sn.Map.LastStatusChecked);
                        //if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByLang(sn.UserID, sn.SecId, fleetId, lng, sn.Map.LastStatusChecked, ref xml), false))
                        //    if (objUtil.ErrCheck(dbf.GetVehiclesChangedPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.Map.LastStatusChecked, ref xml), true))
                        //    {
                        //        sn.Map.DsFleetInfoNew = null;
                        //        sn.Map.LastKnownXML = string.Empty;
                        //        return;
                        //    }
                    }
                    sn.Map.LastStatusChecked = lastCheckedTime;
                    //DateTime.Now;                        
                    if (!string.IsNullOrEmpty(xml))
                    {
                        xml = xml.Replace("VehiclesChangedPositionInformation", "VehiclesLastKnownPositionInformation");
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    }
                }


                if (string.IsNullOrEmpty(xml) && dsVehiclesInfo == null)
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (MutipleUserHierarchyAssignment)
                    {
                        if (sn.User.PreferFleetIds == string.Empty && sn.User.PreferNodeCodes != string.Empty)
                        {
                            var ns = sn.User.PreferNodeCodes.Split(',');
                            var mfids = string.Empty;

                            foreach (var s in ns)
                            {
                                var fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                                mfids = mfids + "," + fid;
                            }
                            mfids = mfids.Trim(',');
                            sn.User.PreferFleetIds = mfids;
                        }

                        if (sn.Map.SelectedMultiFleetIDs == sn.User.PreferFleetIds)
                        {
                            xml = sn.Map.DsFleetInfoNew.GetXml();
                            xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                            //xml = Encoding.UTF8.GetString(data);
                            Response.Write(xml.Trim());
                        }
                    }
                    else if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                        //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                        //xml = Encoding.UTF8.GetString(data);
                        if (lng == "fr")
                        {
                            xml = xml.Replace("<BoxArmed>true</BoxArmed>", "<BoxArmed>voir</BoxArmed>");
                            xml = xml.Replace("<BoxArmed>false</BoxArmed>", "<BoxArmed>faux</BoxArmed>");
                        }
                        Response.Write(xml.Trim());
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    var strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    var dsFleetInfo = new DataSet();

                    if (dsVehiclesInfo == null)
                    {

                        dsFleetInfo.ReadXmlSchema(strPath);
                        dsFleetInfo.ReadXml(strrXML);
                    }
                    else
                    {
                        dsFleetInfo = dsVehiclesInfo.Copy();
                        dsFleetInfo.Tables[0].TableName = "VehiclesLastKnownPositionInformation";
                        //Adding  PTO to Dataset
                        var dc = new DataColumn("PTO", Type.GetType("System.String"));
                        dc.DefaultValue = "Off";
                        dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);

                        sn.Map.DsLastChangedVehicles = dsFleetInfo.Copy();
                    }

                    if (showDashboardView && !dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                    {
                        var dc = new DataColumn("OperationalStateName", Type.GetType("System.String"));
                        dc.DefaultValue = "";
                        dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                    }

                    //xml = dsFleetInfo.GetXml();
                    //sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                    //sn.Map.DsFleetInfoNew = dsFleetInfo;
                    if (sn.Map.DsFleetInfoNew != null)
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            var _m = new clsMap();
                            //if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            //{
                            //    row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            //}

                            //Geting and Inserting PTO Value
                            ulong intSensorMask = 0;
                            try
                            {
                                intSensorMask = Convert.ToUInt64(row["SensorMask"]);
                            }
                            catch
                            {
                            }
                            ulong checkBit = 0x80;
                            //check bit for PTO
                            if ((intSensorMask & checkBit) != 0)
                                row["PTO"] = "On";
                            else
                                row["PTO"] = "Off";

                            var filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                            var foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                            if (foundRows.Length == 0)
                            {
                                // insert here
                                var insertedRow = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].NewRow();
                                insertedRow.ItemArray = row.ItemArray;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Add(insertedRow);
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }
                            else
                            {
                                // update here
                                foreach (DataRow foundRow in foundRows)
                                {
                                    try
                                    {
                                        var index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRow);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxId"] = row["BoxId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastCommunicatedDateTime"] = row["LastCommunicatedDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OriginDateTime"] = row["OriginDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Latitude"] = row["Latitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Longitude"] = row["Longitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Description"] = row["Description"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxArmed"] = row["BoxArmed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["IconTypeName"] = row["IconTypeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleStatus"] = row["VehicleStatus"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["chkBoxShow"] = row["chkBoxShow"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Updated"] = row["Updated"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomUrl"] = row["CustomUrl"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Speed"] = row["Speed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomSpeed"] = row["CustomSpeed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MyHeading"] = row["MyHeading"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ProtocolId"] = row["ProtocolId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SensorMask"] = row["SensorMask"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Driver"] = row["Driver"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ImagePath"] = row["ImagePath"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ConfiguredNum"] = row["ConfiguredNum"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["DriverCardNumber"] = row["DriverCardNumber"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field1"] = row["Field1"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field2"] = row["Field2"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field3"] = row["Field3"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field4"] = row["Field4"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field5"] = row["Field5"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ModelYear"] = row["ModelYear"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MakeName"] = row["MakeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ModelName"] = row["ModelName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleTypeName"] = row["VehicleTypeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VinNum"] = row["VinNum"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ManagerName"] = row["ManagerName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ManagerEmployeeId"] = row["ManagerEmployeeId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StateProvince"] = row["StateProvince"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["PTO"] = row["PTO"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["EngineHours"] = row["EngineHours"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Odometer"] = row["Odometer"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastIgnOnBatV"] = row["LastIgnOnBatV"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastIgnOffBatV"] = row["LastIgnOffBatV"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastBatV"] = row["LastBatV"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalState"] = row["OperationalState"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateNotes"] = row["OperationalStateNotes"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomConfig"] = row["CustomConfig"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleDeviceStatusID"] = row["VehicleDeviceStatusID"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["NearestLandmark"] = row["NearestLandmark"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SAP_number"] = row["SAP_number"];

                                        if (showDashboardView && dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName") && sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                                        {
                                            if (row["OperationalState"].ToString() == "100")
                                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateName"] = "Available";
                                            else if (row["OperationalState"].ToString() == "200")
                                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateName"] = "Unavailable";
                                            else
                                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateName"] = "";
                                        }

                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.StackTrace));
                                    }
                                }
                            }
                        }
                        //xml = dsFleetInfo.GetXml();
                    }
                    else
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            var _m = new clsMap();
                            ulong intSensorMask = 0;
                            try
                            {
                                intSensorMask = Convert.ToUInt64(row["SensorMask"]);
                            }
                            catch
                            {
                            }
                            ulong checkBit = 0x80;
                            //check bit for PTO
                            if ((intSensorMask & checkBit) != 0 && sn.User.OrganizationId != 343)
                                row["PTO"] = "On";
                            else
                                row["PTO"] = "Off";

                            if (showDashboardView && dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                            {
                                if (row["OperationalState"].ToString() == "100")
                                    row["OperationalStateName"] = "Available";
                                else if (row["OperationalState"].ToString() == "200")
                                    row["OperationalStateName"] = "Unavailable";
                                else
                                    row["OperationalStateName"] = "";
                            }
                        }
                        sn.Map.DsFleetInfoNew = dsFleetInfo;
                        //xml = dsFleetInfo.GetXml();
                    }

                    var request = Request.QueryString["mergeData"];
                    if (showDashboardView && !string.IsNullOrEmpty(request) && request.ToLower() == "VehiclesInLandmarks".ToLower())
                    {
                        mergeLandarksToVehicleList();
                    }

                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    getFilteredFleet();
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();

                dbFleet.Dispose();
            }
            catch (NullReferenceException ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.StackTrace));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + ex.Message + ex.StackTrace + "</error>");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + ex.Message + ex.StackTrace + "</error>");
            }
        }

        // Changes for TimeZone Feature end


        private void VehicleList_Fill()
        {
            try
            {
                StringReader strrXML = null;
                var xml = "";
                var dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                var lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");//CultureInfo.CurrentUICulture.TwoLetterISOLanguageName

                var fleetId = 0;
                if (sn.Map.SelectedFleetID != 0)
                {
                    fleetId = Convert.ToInt32(sn.Map.SelectedFleetID);
                }
                else if (sn.User.DefaultFleet != -1)
                {
                    fleetId = Convert.ToInt32(sn.User.DefaultFleet);
                }

                var multipleFleetIds = string.Empty;
                if (MutipleUserHierarchyAssignment)
                {
                    if (sn.Map.SelectedMultiFleetIDs != string.Empty)
                    {
                        multipleFleetIds = sn.Map.SelectedMultiFleetIDs;
                    }
                    else if (sn.User.PreferNodeCodes != string.Empty)
                    {
                        var ns = sn.User.PreferNodeCodes.Split(',');

                        foreach (var s in ns)
                        {
                            var fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                            multipleFleetIds = multipleFleetIds + "," + fid;
                        }
                        multipleFleetIds = multipleFleetIds.Trim(',');
                        sn.User.PreferFleetIds = multipleFleetIds;
                    }
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->VehicleList_Fill - Fleet Id:" + fleetId + ", User Id:" + sn.UserID));
                DataSet dsVehiclesInfo = null;
                var dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString);

                if (string.IsNullOrEmpty(sn.Map.LastKnownXML) || sn.Map.LastStatusChecked == null)
                {
                    sn.Map.LastStatusChecked = DateTime.UtcNow;
                    //DateTime.Now;

                    if (MutipleUserHierarchyAssignment)
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByMultipleFleets(multipleFleetIds, sn.UserID, lng);
                    }
                    else
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, sn.UserID, lng);
                    }

                    if (!string.IsNullOrEmpty(xml))
                    {
                        xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        sn.Map.LastKnownXML = xml;
                    }
                }
                else
                {
                    var lastCheckedTime = DateTime.UtcNow;

                    if (MutipleUserHierarchyAssignment)
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByMultipleFleetsByLang(multipleFleetIds, sn.UserID, lng, sn.Map.LastStatusChecked);
                    }
                    else
                    {
                        dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByLang(fleetId, sn.UserID, lng, sn.Map.LastStatusChecked);
                    }

                    //DateTime.Now;                        
                    sn.Map.LastStatusChecked = lastCheckedTime;
                    if (!string.IsNullOrEmpty(xml))
                    {
                        xml = xml.Replace("VehiclesChangedPositionInformation", "VehiclesLastKnownPositionInformation");
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    }
                }

                if (string.IsNullOrEmpty(xml) && dsVehiclesInfo == null)
                //(xml == "" || xml == null)
                {
                    //sn.Map.DsFleetInfoNew = null;
                    if (MutipleUserHierarchyAssignment)
                    {
                        if (sn.User.PreferFleetIds == string.Empty && sn.User.PreferNodeCodes != string.Empty)
                        {
                            var ns = sn.User.PreferNodeCodes.Split(',');
                            var mfids = string.Empty;

                            foreach (var s in ns)
                            {
                                var fid = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, s);
                                mfids = mfids + "," + fid;
                            }
                            mfids = mfids.Trim(',');
                            sn.User.PreferFleetIds = mfids;
                        }

                        if (sn.Map.SelectedMultiFleetIDs == sn.User.PreferFleetIds)
                        {
                            xml = sn.Map.DsFleetInfoNew.GetXml();
                            xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                            //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                            //xml = Encoding.UTF8.GetString(data);
                            Response.Write(xml.Trim());
                        }
                    }
                    else if (sn.Map.SelectedFleetID == sn.User.DefaultFleet)
                    {
                        xml = sn.Map.DsFleetInfoNew.GetXml();
                        xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                        //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                        //xml = Encoding.UTF8.GetString(data);
                        if (lng == "fr")
                        {
                            xml = xml.Replace("<BoxArmed>true</BoxArmed>", "<BoxArmed>voir</BoxArmed>");
                            xml = xml.Replace("<BoxArmed>false</BoxArmed>", "<BoxArmed>faux</BoxArmed>");
                        }
                        Response.Write(xml.Trim());
                    }
                    else
                        return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    var strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    var dsFleetInfo = new DataSet();

                    if (dsVehiclesInfo == null)
                    {

                        dsFleetInfo.ReadXmlSchema(strPath);
                        dsFleetInfo.ReadXml(strrXML);
                    }
                    else
                    {
                        dsFleetInfo = dsVehiclesInfo.Copy();
                        dsFleetInfo.Tables[0].TableName = "VehiclesLastKnownPositionInformation";
                        //Adding  PTO to Dataset
                        var dc = new DataColumn("PTO", Type.GetType("System.String"));
                        dc.DefaultValue = "Off";
                        dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);
                    }

                    //xml = dsFleetInfo.GetXml();
                    //sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                    //sn.Map.DsFleetInfoNew = dsFleetInfo;
                    if (sn.Map.DsFleetInfoNew != null)
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            var _m = new clsMap();
                            //if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            //{
                            //    row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            //}

                            //Geting and Inserting PTO Value
                            ulong intSensorMask = 0;
                            try
                            {
                                intSensorMask = Convert.ToUInt64(row["SensorMask"]);
                            }
                            catch
                            {
                            }
                            ulong checkBit = 0x80;
                            //check bit for PTO
                            if ((intSensorMask & checkBit) != 0)
                                row["PTO"] = "On";
                            else
                                row["PTO"] = "Off";

                            var filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                            var foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                            if (foundRows.Length == 0)
                            {
                                // insert here
                                var insertedRow = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].NewRow();
                                insertedRow.ItemArray = row.ItemArray;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Add(insertedRow);
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }
                            else
                            {
                                // update here
                                for (var i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        var index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxId"] = row["BoxId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastCommunicatedDateTime"] = row["LastCommunicatedDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OriginDateTime"] = row["OriginDateTime"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Latitude"] = row["Latitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Longitude"] = row["Longitude"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Description"] = row["Description"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["BoxArmed"] = row["BoxArmed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["IconTypeName"] = row["IconTypeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleStatus"] = row["VehicleStatus"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["chkBoxShow"] = row["chkBoxShow"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Updated"] = row["Updated"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomUrl"] = row["CustomUrl"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Speed"] = row["Speed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomSpeed"] = row["CustomSpeed"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MyHeading"] = row["MyHeading"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ProtocolId"] = row["ProtocolId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SensorMask"] = row["SensorMask"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Driver"] = row["Driver"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ImagePath"] = row["ImagePath"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ConfiguredNum"] = row["ConfiguredNum"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["DriverCardNumber"] = row["DriverCardNumber"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field1"] = row["Field1"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field2"] = row["Field2"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field3"] = row["Field3"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field4"] = row["Field4"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Field5"] = row["Field5"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ModelYear"] = row["ModelYear"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["MakeName"] = row["MakeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ModelName"] = row["ModelName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleTypeName"] = row["VehicleTypeName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VinNum"] = row["VinNum"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ManagerName"] = row["ManagerName"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["ManagerEmployeeId"] = row["ManagerEmployeeId"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StateProvince"] = row["StateProvince"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["PTO"] = row["PTO"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["EngineHours"] = row["EngineHours"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["Odometer"] = row["Odometer"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastIgnOnBatV"] = row["LastIgnOnBatV"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastIgnOffBatV"] = row["LastIgnOffBatV"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["LastBatV"] = row["LastBatV"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalState"] = row["OperationalState"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateNotes"] = row["OperationalStateNotes"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["CustomConfig"] = row["CustomConfig"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["VehicleDeviceStatusID"] = row["VehicleDeviceStatusID"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["NearestLandmark"] = row["NearestLandmark"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["SAP_number"] = row["SAP_number"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                                    }
                                }
                            }
                        }
                        //xml = dsFleetInfo.GetXml();
                    }
                    else
                    {
                        foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                        {
                            var _m = new clsMap();
                            //if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            //{
                            //    row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                            //}

                            //Geting and Inserting PTO Value
                            ulong intSensorMask = 0;
                            try
                            {
                                intSensorMask = Convert.ToUInt64(row["SensorMask"]);
                            }
                            catch
                            {
                            }
                            ulong checkBit = 0x80;
                            //check bit for PTO
                            if ((intSensorMask & checkBit) != 0 && sn.User.OrganizationId != 343)
                                row["PTO"] = "On";
                            else
                                row["PTO"] = "Off";
                        }
                        sn.Map.DsFleetInfoNew = dsFleetInfo;
                        //xml = dsFleetInfo.GetXml();
                    }
                    sn.Map.DsFleetInfo = sn.Map.DsFleetInfoNew;
                    //byte[] data = Encoding.Default.GetBytes(xml.Trim());
                    //xml = Encoding.UTF8.GetString(data);
                    //xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    //if (lng == "fr")
                    //{
                    //    xml = xml.Replace("<BoxArmed>true</BoxArmed>", "<BoxArmed>voir</BoxArmed>");
                    //    xml = xml.Replace("<BoxArmed>false</BoxArmed>", "<BoxArmed>faux</BoxArmed>");
                    //}
                    //Response.Write(xml.Trim());
                    //getXmlFromDs(dsFleetInfo, -1);
                    getFilteredFleet();
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();

                dbFleet.Dispose();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");
            }
        }

        private void getFilteredFleet()
        {
            try
            {
                //string xml = "";
                if (sn.Map.DsFleetInfo != null)
                {
                    //StringReader strrXML = new StringReader(sn.Map.DsFleetInfo.GetXml().Trim());
                    //string strPath = Server.MapPath("./Datasets/FleetInfo.xsd");          
                    //DataSet dstemp = new DataSet();
                    //dstemp.ReadXmlSchema(strPath);
                    //dstemp.ReadXml(strrXML);
                    var dstemp = new DataSet();
                    dstemp = sn.Map.DsFleetInfo.Copy();

                    var sortedTable = dstemp.Tables[0];
                    //Edited by Rohit Mittal                    
                    var intialRowCount = sortedTable.Rows.Count;
                    var filterarray = filters.Split(',');
                    if (!string.IsNullOrEmpty(filters) && filterarray.Length > 0)
                    {
                        foreach (var s in filterarray)
                        {
                            if (string.IsNullOrEmpty(s))
                                continue;
                            var filtercol = s.Split(':')[0];
                            var filtervalue = s.Split(':')[1].Replace("\"", "");
                            if (filtervalue.Contains("type int") || filtervalue.Contains("type float"))
                            {
                                if (filtervalue.Contains("lt"))
                                {
                                    var col = filtervalue.Substring(filtervalue.LastIndexOf("lt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " <" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("gt"))
                                {
                                    var col = filtervalue.Substring(filtervalue.LastIndexOf("gt")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(filtercol + " >" + col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("eq"))
                                {
                                    var col = filtervalue.Substring(filtervalue.LastIndexOf("eq")).Split(' ')[1];
                                    sortedTable = sortedTable.Select(string.Format("{0} = {1}", filtercol, col)).CopyToDataTable();
                                }

                            }
                            else if (filtervalue.Contains("type date"))
                            {
                                if (filtervalue.Contains("before"))
                                {
                                    var col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("before")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("after"))
                                {

                                    var col = filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("after")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }
                                if (filtervalue.Contains("on"))
                                {
                                    var col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 23:59:59" + "#";
                                    col += " AND " + filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 00:00:00" + "#";
                                    sortedTable = sortedTable.Select(col).CopyToDataTable();
                                }

                            }
                            else
                            {
                                if (sortedTable.Columns[filtercol].DataType == typeof(bool))
                                {
                                    sortedTable = sortedTable.Select(string.Format("{0} = '{1}'", filtercol, filtervalue)).CopyToDataTable();
                                }
                                else if (filtercol == "OperationalStateName")
                                    sortedTable = sortedTable.Select(string.Format("{0} LIKE '{1}%'", filtercol, filtervalue)).CopyToDataTable();
                                else
                                    sortedTable = sortedTable.Select(string.Format("{0} LIKE '%{1}%'", filtercol, filtervalue)).CopyToDataTable();
                            }
                        }
                    }
                    var request = Request.QueryString["sorting"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        sortedTable.DefaultView.Sort = request.Split(',')[0] + " " + request.Split(',')[1];
                        sortedTable = sortedTable.DefaultView.ToTable();
                    }
                    else
                    {
                        sortedTable.DefaultView.Sort = "OriginDateTime DESC";
                        sortedTable = sortedTable.DefaultView.ToTable();
                    }
                    var finalRowCount = sortedTable.Rows.Count;
                    var resolveAddress = false;
                    if (finalRowCount > vlStart && operation != "Export")
                    {
                        sortedTable = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
                        resolveAddress = true;
                    }
                    else if (operation != "Export")
                    {
                        sortedTable = sortedTable.AsEnumerable().Take(vlLimit).CopyToDataTable();
                        resolveAddress = true;
                    }
                    sortedTable.TableName = "VehiclesLastKnownPositionInformation";

                    if (resolveAddress)
                    {
                        foreach (DataRow row in sortedTable.Rows)
                        {
                            if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                            {
                                var _m = new clsMap();
                                row["StreetAddress"] = _m.ResolveStreetAddressNavteq(row["Latitude"].ToString(), row["Longitude"].ToString());

                                var filter = string.Format("BoxId = '{0}'", row["BoxId"]);
                                var foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
                                for (var i = 0; i < foundRows.Length; i++)
                                {
                                    try
                                    {
                                        var index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["StreetAddress"] = row["StreetAddress"];
                                        sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                                    }
                                }
                            }

                        }
                    }

                    if (operation == "Export" && !string.IsNullOrEmpty(formattype))
                    {
                        request = Request.QueryString["columns"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            exportDatatable(sortedTable, formattype, request);
                            return;
                        }
                    }

                    if (dstemp.Tables.CanRemove(dstemp.Tables[0]))
                    {
                        dstemp.Tables.Remove(dstemp.Tables[0]);
                    }
                    dstemp.Tables.Add(sortedTable);
                    dstemp.DataSetName = "Fleet";
                    //xml = dstemp.GetXml();
                    //if (!String.IsNullOrEmpty(filters) && filterarray.Length > 0)
                    //    xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + finalRowCount.ToString() + "</totalCount>");
                    //else
                    //    xml = xml.Replace("<Fleet>", "<Fleet><totalCount>" + intialRowCount.ToString() + "</totalCount>");

                    var totalCount = 0;
                    if (!string.IsNullOrEmpty(filters) && filterarray.Length > 0)
                        totalCount = finalRowCount;
                    else
                        totalCount = intialRowCount;

                    Response.ContentType = "text/xml";
                    Response.ContentEncoding = Encoding.UTF8;
                    //Response.Write(xml.Trim());
                    getXmlFromDs(dstemp, totalCount);
                    //Response.End();
                }
            }
            catch (Exception Ex)
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");
            }
        }

        private void GetAllFleetForMap()
        {
            try
            {
                var request = string.Empty;
                request = Request.QueryString["data"];
                var dstemp = new DataSet();
                if (!string.IsNullOrEmpty(request) && request == "new")
                    dstemp = sn.Map.DsLastChangedVehicles.Copy();
                else
                    dstemp = sn.Map.DsFleetInfo.Copy();

                var sortedTable = dstemp.Tables[0];
                request = Request.QueryString["filters"];
                if (string.IsNullOrEmpty(request))
                    request = "";
                var filterarray = request.Split(',');
                if (!string.IsNullOrEmpty(request) && filterarray.Length > 0)
                {
                    foreach (var s in filterarray)
                    {
                        if (string.IsNullOrEmpty(s))
                            continue;
                        var filtercol = s.Split(':')[0];
                        var filtervalue = s.Split(':')[1].Replace("\"", "");
                        if (filtervalue.Contains("type int") || filtervalue.Contains("type float"))
                        {
                            if (filtervalue.Contains("lt"))
                            {
                                var col = filtervalue.Substring(filtervalue.LastIndexOf("lt")).Split(' ')[1];
                                sortedTable = sortedTable.Select(filtercol + " <" + col).CopyToDataTable();
                            }
                            if (filtervalue.Contains("gt"))
                            {
                                var col = filtervalue.Substring(filtervalue.LastIndexOf("gt")).Split(' ')[1];
                                sortedTable = sortedTable.Select(filtercol + " >" + col).CopyToDataTable();
                            }
                            if (filtervalue.Contains("eq"))
                            {
                                var col = filtervalue.Substring(filtervalue.LastIndexOf("eq")).Split(' ')[1];
                                sortedTable = sortedTable.Select(string.Format("{0} = {1}", filtercol, col)).CopyToDataTable();
                            }

                        }
                        else if (filtervalue.Contains("type date"))
                        {
                            if (filtervalue.Contains("before"))
                            {
                                var col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("before")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                sortedTable = sortedTable.Select(col).CopyToDataTable();
                            }
                            if (filtervalue.Contains("after"))
                            {

                                var col = filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("after")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + "#";
                                sortedTable = sortedTable.Select(col).CopyToDataTable();
                            }
                            if (filtervalue.Contains("on"))
                            {
                                var col = filtercol + " < #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 23:59:59" + "#";
                                col += " AND " + filtercol + " > #" + DateTime.ParseExact(filtervalue.Substring(filtervalue.LastIndexOf("on")).Split(' ')[1], sn.User.DateFormat, null).ToString("MM/dd/yyyy") + " 00:00:00" + "#";
                                sortedTable = sortedTable.Select(col).CopyToDataTable();
                            }

                        }
                        else
                        {
                            if (sortedTable.Columns[filtercol].DataType == typeof(bool))
                            {
                                sortedTable = sortedTable.Select(string.Format("{0} = '{1}'", filtercol, filtervalue)).CopyToDataTable();
                            }
                            else if (filtercol == "OperationalStateName")
                                sortedTable = sortedTable.Select(string.Format("{0} LIKE '{1}%'", filtercol, filtervalue)).CopyToDataTable();
                            else
                                sortedTable = sortedTable.Select(string.Format("{0} LIKE '%{1}%'", filtercol, filtervalue)).CopyToDataTable();
                        }
                    }
                }
                if (!sn.User.ShowRetiredVehicles && sortedTable.Rows.Count > 0)
                {
                    sortedTable = sortedTable.Select(" VehicleDeviceStatusID <> 3 ").CopyToDataTable();
                }
                sortedTable = sortedTable.Select(" Latitude <> 0 AND Latitude <> 90 AND Latitude <> -90 ").CopyToDataTable();
                sortedTable = getIcon(sortedTable);
                var view = new System.Data.DataView(sortedTable);
                sortedTable = view.ToTable("VehiclesLastKnownPositionInformation", false, "BoxId", "OriginDateTime", "Latitude", "Longitude", "Description", "Driver", "icon", "ImagePath");
                if (dstemp.Tables.CanRemove(dstemp.Tables[0]))
                {
                    dstemp.Tables.Remove(dstemp.Tables[0]);
                }
                dstemp.Tables.Add(sortedTable);
                dstemp.DataSetName = "Fleet";
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.UTF8;
                if (sortedTable.Rows.Count > 0)
                {
                    try
                    {
                        var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("vehiclesfile1_*.xml");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromHours(10))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + Ex.StackTrace + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                    }
                    var filename = Server.MapPath("TempReports/") + "vehiclesfile1_" + sn.UserID + "_" + Session.SessionID + "_" + Guid.NewGuid() + ".xml";
                    dstemp.WriteXml(filename);
                    var doc = new XmlDocument();
                    doc.Load(filename);
                    Response.Write(JsonConvert.SerializeXmlNode(doc));
                }
                else
                {
                    var doc = new XmlDocument();
                    doc.LoadXml("<Fleet><VehiclesLastKnownPositionInformation></VehiclesLastKnownPositionInformation></Fleet>");
                    //Response.Write(JsonConvert.SerializeXmlNode(doc));
                    Response.Write("{\"Fleet\":{\"VehiclesLastKnownPositionInformation\":[]}}");
                }

            }
            catch (Exception Ex)
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + Ex.StackTrace + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        private void ResolveAddress(int rBoxId, float rLat, float rLon)
        {
            try
            {
                var address = "";
                if (rLat != 0 && rLon != 0)
                {
                    var _m = new clsMap();
                    address = _m.ResolveStreetAddressNavteq(rLat.ToString(), rLon.ToString());
                    Response.Write(address);
                }
                else
                    Response.Write("Invalid Address");

                if (rBoxId > 0 && address != "")
                {
                    VLF.DAS.Logic.Box _box = null;
                    _box = new VLF.DAS.Logic.Box(sConnectionString);
                    _box.UpdateStreetAddress(rBoxId, rLat, rLon, address);
                }

            }
            catch (Exception e)
            {
            }
        }

        private void getXmlFromDs(DataSet ds, int c)
        {
            var lng = ((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en");
            Response.Write("<" + ds.DataSetName + "><totalCount>" + c + "</totalCount>");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Response.Write("<" + ds.Tables[0].TableName + ">");
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    Response.Write("<" + column.ColumnName + ">");
                    var v = "";
                    if (column.DataType.Name == "DateTime")
                        v = string.Format("{0:yyyy-MM-ddTHH:mm:ss.ff}", dr[column]);
                    else
                    {
                        v = dr[column].ToString();
                        if (lng == "fr")
                        {
                            if (column.ColumnName == "BoxArmed")
                            {
                                v = v.Replace("true", "voir");
                                v = v.Replace("false", "faux");
                            }
                        }
                        v = v.Replace("&#x0", "").Replace("&", "&amp;").Replace("\0", string.Empty);
                    }
                    //byte[] data = Encoding.Default.GetBytes(v);
                    //v = Encoding.UTF8.GetString(data);
                    Response.Write(RemoveInvalidXmlChars(v));
                    Response.Write("</" + column.ColumnName + ">");
                }
                Response.Write("</" + ds.Tables[0].TableName + ">");
            }
            Response.Write("</" + ds.DataSetName + ">");
        }

        private string RemoveInvalidXmlChars(string text)
        {
            var re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        private string getExcelDateFormat()
        {
            var dformat = "yyyy-mm-dd";
            string tformat;
            if (sn.User.DateFormat == "dd/MM/yyyy")
                dformat = "dd/mm/yyyy";
            else if (sn.User.DateFormat == "d/M/yyyy")
                dformat = "d/m/yyyy";
            else if (sn.User.DateFormat == "dd/MM/yy")
                dformat = "dd/mm/yy";
            else if (sn.User.DateFormat == "d/M/yy")
                dformat = "d/m/yy";
            else if (sn.User.DateFormat == "d MMM yyyy")
                dformat = "d mmmm yyyy";
            else if (sn.User.DateFormat == "MM/dd/yyyy")
                dformat = "mm/dd/yyyy";
            else if (sn.User.DateFormat == "M/d/yyyy")
                dformat = "m/d/yyyy";
            else if (sn.User.DateFormat == "MM/dd/yy")
                dformat = "mm/dd/yy";
            else if (sn.User.DateFormat == "M/d/yy")
                dformat = "m/d/yy";
            else if (sn.User.DateFormat == "MMMM d yy")
                dformat = "mmmm d yy";
            else if (sn.User.DateFormat == "yyyy/MM/dd")
                dformat = "yyyy/mm/dd";

            if (sn.User.TimeFormat == "hh:mm:ss tt")
                tformat = "h:mm:ss AM/PM";
            else
                tformat = "h:mm:ss";

            return dformat + " " + tformat;
        }

        private void exportDatatable(DataTable dt, string formatter, string columns)
        {
            try
            {
                var exceldtformat = getExcelDateFormat();

                if (formatter == "csv")
                {

                    var sresult = new StringBuilder();
                    sresult.Append("sep=,");
                    sresult.Append(Environment.NewLine);
                    var header = string.Empty;
                    foreach (var column in columns.Split(','))
                    {
                        var s = column.Split(':')[0];
                        header += "\"" + s + "\",";
                    }
                    header = header.Substring(0, header.Length - 1);
                    sresult.Append(header);
                    sresult.Append(Environment.NewLine);

                    foreach (DataRow row in dt.Rows)
                    {
                        var data = string.Empty;
                        foreach (var column in columns.Split(','))
                        {
                            var s = string.Empty;
                            if (column.Split(':')[1] == "LatLon")
                            {
                                s = row["Latitude"] + "," + row["Longitude"];
                            }
                            else
                                s = row[column.Split(':')[1]].ToString();
                            if (column.Split(':')[1] == "OriginDateTime")
                                s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                        }
                        data = data.Substring(0, data.Length - 1);
                        sresult.Append(data);
                        sresult.Append(Environment.NewLine);
                    }

                    Response.Clear();
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", "vehicles"));
                    Response.Charset = Encoding.GetEncoding("iso-8859-1").BodyName;
                    Response.ContentType = "application/csv";
                    Response.ContentEncoding = Encoding.GetEncoding("iso-8859-1");
                    Response.Write(sresult.ToString());
                    Response.Flush();
                }
                else if (formatter == "excel2003")
                {
                    var wb = new HSSFWorkbook();
                    var ws = wb.CreateSheet("Sheet1");
                    var cellstyle1 = wb.CreateCellStyle();
                    var cellstyle2 = wb.CreateCellStyle();
                    var cellstyle3 = wb.CreateCellStyle();
                    var cellstyle4 = wb.CreateCellStyle();
                    var cellstyle5 = wb.CreateCellStyle();
                    cellstyle1.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle3.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    cellstyle5.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    var palette = wb.GetCustomPalette();
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index, (byte)123, (byte)178, (byte)115);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.YELLOW.index, (byte)239, (byte)215, (byte)0);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index, (byte)255, (byte)166, (byte)74);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.ROSE.index, (byte)222, (byte)121, (byte)115);
                    palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.INDIGO.index, (byte)99, (byte)125, (byte)165);
                    cellstyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index;
                    cellstyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
                    cellstyle3.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index;
                    cellstyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ROSE.index;
                    cellstyle5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;
                    var row = ws.CreateRow(0);
                    foreach (var column in columns.Split(','))
                    {
                        var s = column.Split(':')[0];
                        row.CreateCell(row.Cells.Count).SetCellValue(s);
                    }
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var data = string.Empty;
                        var rowData = ws.CreateRow(i + 1);
                        foreach (var column in columns.Split(','))
                        {
                            if (column.Split(':')[1] == "OriginDateTime")
                            {
                                var currentDate = DateTime.Now.ToUniversalTime();
                                DateTime recordDate;

                                var datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                //rowData.CreateCell(rowData.Cells.Count).SetCellValue(datadate.Replace("[br]", Environment.NewLine));
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()));
                                recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                                var diffDate = currentDate.Subtract(recordDate);

                                if (diffDate.TotalHours < 24)
                                {
                                    cellstyle1.WrapText = true;
                                    cellstyle1.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle1.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle1;
                                }
                                else if (diffDate.TotalHours < 48)
                                {
                                    cellstyle2.WrapText = true;
                                    cellstyle2.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle2.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle2;
                                }
                                else if (diffDate.TotalHours < 72)
                                {
                                    cellstyle3.WrapText = true;
                                    cellstyle3.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle3.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle3;
                                }
                                else if (diffDate.TotalHours < 168)
                                {
                                    cellstyle4.WrapText = true;
                                    cellstyle4.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle4.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle4;
                                }
                                else if (diffDate.TotalHours > 168)
                                {
                                    cellstyle5.WrapText = true;
                                    cellstyle5.VerticalAlignment = VerticalAlignment.TOP;
                                    cellstyle5.DataFormat = wb.CreateDataFormat().GetFormat(exceldtformat);
                                    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle5;
                                }

                            }
                            else if (column.Split(':')[1] == "LatLon")
                            {
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i]["Latitude"] + "," + dt.Rows[i]["Longitude"]);
                            }
                            else
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine));

                        }
                    }

                    for (var i = 0; i < columns.Split(',').Length; i++)
                    {
                        try
                        {
                            ws.AutoSizeColumn(i);
                        }
                        catch { }
                    }

                    var httpResponse = Response;
                    httpResponse.Clear();
                    //httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //httpResponse.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", "Vehicle"));
                    Response.AddHeader("Content-Type", "application/Excel");
                    Response.ContentType = "application/force-download";
                    Response.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xls", "vehicles"));

                    using (var memoryStream = new MemoryStream())
                    {
                        wb.Write(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }

                    //HttpContext.Current.Response.End();
                }
                else if (formatter == "excel2007")
                {
                    try
                    {
                        var wb = new XLWorkbook();
                        var ws = wb.Worksheets.Add("Sheet1");
                        foreach (var column in columns.Split(','))
                        {
                            var s = column.Split(':')[0];
                            ws.Cell(1, ws.Row(1).CellsUsed().Count() + 1).Value = s;
                        }

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var data = string.Empty;
                            var iColumn = 1;
                            foreach (var column in columns.Split(','))
                            {
                                ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;

                                if (column.Split(':')[1] == "OriginDateTime")
                                {
                                    var currentDate = DateTime.Now.ToUniversalTime();
                                    DateTime recordDate;


                                    ws.Cell(i + 2, iColumn).DataType = XLCellValues.DateTime;
                                    ws.Cell(i + 2, iColumn).Value = dt.Rows[i][column.Split(':')[1]];
                                    ws.Cell(i + 2, iColumn).Style.DateFormat.Format = exceldtformat;

                                    var datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                    //ws.Cell(i + 2, iColumn).Value = "'" + datadate.Replace("[br]", Environment.NewLine);
                                    recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                                    var diffDate = currentDate.Subtract(recordDate);

                                    if (diffDate.TotalHours < 24)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#7BB273");
                                    }
                                    else if (diffDate.TotalHours < 48)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFD700");
                                    }
                                    else if (diffDate.TotalHours < 72)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFA64A");
                                    }
                                    else if (diffDate.TotalHours < 168)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#DE7973");
                                    }
                                    else if (diffDate.TotalHours > 168)
                                    {
                                        ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#637DA5");
                                    }

                                }
                                else if (column.Split(':')[1] == "LatLon")
                                {
                                    ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i]["Latitude"] + "," + dt.Rows[i]["Longitude"];
                                }
                                else
                                    ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);

                                iColumn++;
                            }
                        }

                        //ws.Rows().Style.Alignment.SetWrapText();
                        //ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        //ws.Columns().AdjustToContents();

                        try
                        {
                            var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.xlsx");
                            foreach (var file in files)
                            {
                                if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                                {
                                    File.Delete(file.FullName);
                                }
                            }
                        }
                        catch { }

                        Response.Clear();
                        Response.AddHeader("Content-Type", "application/Excel");
                        Response.ContentType = "application/force-download";
                        Response.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xlsx", "vehicles"));
                        var filemame = string.Format(@"{0}.xlsx", Guid.NewGuid());
                        wb.SaveAs(Server.MapPath("TempReports/") + filemame);
                        Response.TransmitFile("TempReports/" + filemame);
                    }
                    //Peter Editted
                    catch (Exception Ex)
                    {
                        Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                    }

                }
            }
            //Peter Editted
            catch (Exception Ex)
            {
                Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature start

        private void getClosestVehicles_NewTZ(int fleetId, double lon, double lat, int searchNumVehicles, int searchRadius)
        {
            try
            {
                StringReader strrXML = null;

                var dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->getClosestVehicles - Fleet Id:" + fleetId + ", User Id:" + sn.UserID));

                var fleet = new ServerDBFleet.DBFleet();
                var xml = string.Empty;
                if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), false))
                    if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                }


                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    var strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    var dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);

                    var dc = new DataColumn("distance", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);

                    foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                    {
                        var _m = new clsMap();
                        //if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                        //{
                        //    row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                        //}

                        var distance = DistanceAlgorithm.DistanceBetweenPlaces(lon, lat, double.Parse(row["Longitude"].ToString()), double.Parse(row["Latitude"].ToString()));
                        row["distance"] = distance;
                    }

                    var dstemp = new DataSet();
                    var dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                    dv.Sort = "distance";
                    var sortedTable = dv.ToTable();
                    var dt = sortedTable.AsEnumerable().Skip(0).Take(searchNumVehicles).CopyToDataTable();
                    dt.TableName = "VehiclesLastKnownPositionInformation";
                    dstemp.Tables.Add(dt);
                    dstemp.DataSetName = "Fleet";
                    xml = dstemp.GetXml();

                    //xml = dsFleetInfo.GetXml();

                    xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature end

        private void getClosestVehicles(int fleetId, double lon, double lat, int searchNumVehicles, int searchRadius)
        {
            try
            {
                StringReader strrXML = null;

                var dbf = new ServerDBFleet.DBFleet();
                objUtil = new clsUtility(sn);

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->getClosestVehicles - Fleet Id:" + fleetId + ", User Id:" + sn.UserID));

                var fleet = new ServerDBFleet.DBFleet();
                var xml = string.Empty;
                if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), false))
                    if (objUtil.ErrCheck(fleet.GetVehiclesLastKnownPositionInfoNearestToLatLon(sn.UserID, sn.SecId, sn.User.OrganizationId, fleetId, searchRadius, lat, lon, ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                }


                if (string.IsNullOrEmpty(xml))
                //(xml == "" || xml == null)
                {
                    return;
                }
                else
                {
                    strrXML = new StringReader(xml);

                    var strPath = Server.MapPath("./Datasets/FleetInfo.xsd");
                    var dsFleetInfo = new DataSet();
                    dsFleetInfo.ReadXmlSchema(strPath);
                    dsFleetInfo.ReadXml(strrXML);

                    var dc = new DataColumn("distance", Type.GetType("System.Double"));
                    dc.DefaultValue = 0;
                    dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Columns.Add(dc);

                    foreach (DataRow row in dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Rows)
                    {
                        var _m = new clsMap();
                        //if (row["StreetAddress"].ToString().Trim() == VLF.CLS.Def.Const.addressNA)
                        //{
                        //    row["StreetAddress"] = _m.ResolveStreetAddressTelogis(row["Latitude"].ToString(), row["Longitude"].ToString());
                        //}

                        var distance = DistanceAlgorithm.DistanceBetweenPlaces(lon, lat, double.Parse(row["Longitude"].ToString()), double.Parse(row["Latitude"].ToString()));
                        row["distance"] = distance;
                    }

                    var dstemp = new DataSet();
                    var dv = dsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
                    dv.Sort = "distance";
                    var sortedTable = dv.ToTable();
                    var dt = sortedTable.AsEnumerable().Skip(0).Take(searchNumVehicles).CopyToDataTable();
                    dt.TableName = "VehiclesLastKnownPositionInformation";
                    dstemp.Tables.Add(dt);
                    dstemp.DataSetName = "Fleet";
                    xml = dstemp.GetXml();

                    //xml = dsFleetInfo.GetXml();

                    xml = xml.Trim().Replace("<br>", "").Trim().Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                    Response.Write(xml.Trim());
                }
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- VehicleList_Fill in Vehicles aspx User ID:" + sn.UserID));
                //Response.End();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        private void getVehiclesInLandmark(int fleetId, long landmarkId)
        {
            var dstemp = new DataSet();

            var _cs = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

            using (var dbVehicle = new VLF.DAS.Logic.Vehicle(_cs))
            {
                dstemp = dbVehicle.ListVehiclesInLandmarkByFleet(sn.UserID, sn.User.OrganizationId, fleetId, landmarkId);
            }

            var _in = "(";
            for (var i = 0; i < dstemp.Tables[0].Rows.Count - 1; i++)
            {
                if (i > 0)
                    _in += ",";
                _in += dstemp.Tables[0].Rows[i]["VehicleID"].ToString();
            }
            _in += ")";

            var dv = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].DefaultView;
            var sortedTable = dv.ToTable();
            var dt = new DataTable();
            var vehicles = sortedTable.Select(string.Format("VehicleID IN {0}", _in));
            if (vehicles.Length > 0)
                dt = vehicles.CopyToDataTable();

            var dsresult = new DataSet();

            dt.TableName = "VehiclesLastKnownPositionInformation";
            dsresult.Tables.Add(dt);
            dsresult.DataSetName = "Fleet";

            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;
            getXmlFromDs(dsresult, dt.Rows.Count);
        }

        // Changes for TimeZone Feature start
        private void searchHistoryAddressByWebservice_NewTZ()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var dbhistory = new ServerDBHistory.DBHistory();
                //dbhistory.get

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                var SearchHistoryDateTime = Request["SearchHistoryDateTime"];
                var SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"], out lon);
                double.TryParse(Request["lat"], out lat);
                int.TryParse(Request["SearchHistoryTimeRange"], out SearchHistoryTimeRange);
                double.TryParse(Request["radius"], out radius);
                var PolygonPoints = Request["mapSearchPointSets"];

                var dtfrom = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                var dtto = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                var fleetIds = Request["FleetIds"] == null ? "" : Request["FleetIds"];
                var boxIds = Request["BoxIds"] == null ? "" : Request["BoxIds"];

                /*string xml = "<Fleet>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>939393</BoxId>";
                xml += "<VehicleId>44562</VehicleId>";
                xml += "<LicensePlate>LP_939393</LicensePlate>";
                xml += "<Description>939393-QingQing</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>200200</BoxId>";
                xml += "<VehicleId>31320</VehicleId>";
                xml += "<LicensePlate>LP_200200</LicensePlate>";
                xml += "<Description>Ravi_3000_Test_Unit</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "</Fleet>";*/
                dbhistory.Timeout = -1;
                var xml = string.Empty;
                //if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, fleetIds, boxIds, ref xml), false))
                //    if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, fleetIds, boxIds, ref xml), true))
                //    {
                //        return;
                //    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                Response.Write("<error>" + Ex + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex + "</error>");
            }
        }

        // Changes for TimeZone Feature end

        private void searchHistoryAddressByWebservice()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var dbhistory = new ServerDBHistory.DBHistory();
                //dbhistory.get

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                var SearchHistoryDateTime = Request["SearchHistoryDateTime"];
                var SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"], out lon);
                double.TryParse(Request["lat"], out lat);
                int.TryParse(Request["SearchHistoryTimeRange"], out SearchHistoryTimeRange);
                double.TryParse(Request["radius"], out radius);
                var PolygonPoints = Request["mapSearchPointSets"];

                var dtfrom = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                var dtto = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                var fleetIds = Request["FleetIds"] == null ? "" : Request["FleetIds"];
                var boxIds = Request["BoxIds"] == null ? "" : Request["BoxIds"];

                /*string xml = "<Fleet>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>939393</BoxId>";
                xml += "<VehicleId>44562</VehicleId>";
                xml += "<LicensePlate>LP_939393</LicensePlate>";
                xml += "<Description>939393-QingQing</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>200200</BoxId>";
                xml += "<VehicleId>31320</VehicleId>";
                xml += "<LicensePlate>LP_200200</LicensePlate>";
                xml += "<Description>Ravi_3000_Test_Unit</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "</Fleet>";*/
                dbhistory.Timeout = -1;
                var xml = string.Empty;
                //if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, fleetIds, boxIds, ref xml), false))
                //    if (objUtil.ErrCheck(dbhistory.GetVehicleAreaSearch(sn.UserID, sn.SecId, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, 0, PolygonPoints, fleetIds, boxIds, ref xml), true))
                //    {
                //        return;
                //    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                Response.Write("<error>" + Ex + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex + "</error>");
            }
        }

        // Changes for TimeZone Feature start

        private void searchHistoryAddressByDasLogic_NewTZ()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var dbhistory = new ServerDBHistory.DBHistory();
                //dbhistory.get

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                var SearchHistoryDateTime = Request["SearchHistoryDateTime"];
                var SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"], out lon);
                double.TryParse(Request["lat"], out lat);
                int.TryParse(Request["SearchHistoryTimeRange"], out SearchHistoryTimeRange);
                double.TryParse(Request["radius"], out radius);
                var PolygonPoints = Request["mapSearchPointSets"];

                var dtfrom = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                var dtto = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                var fleetIds = Request["FleetIds"] == null ? "" : Request["FleetIds"];
                var boxIds = Request["BoxIds"] == null ? "" : Request["BoxIds"];

                /*string xml = "<Fleet>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>939393</BoxId>";
                xml += "<VehicleId>44562</VehicleId>";
                xml += "<LicensePlate>LP_939393</LicensePlate>";
                xml += "<Description>939393-QingQing</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>200200</BoxId>";
                xml += "<VehicleId>31320</VehicleId>";
                xml += "<LicensePlate>LP_200200</LicensePlate>";
                xml += "<Description>Ravi_3000_Test_Unit</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "</Fleet>";*/
                dbhistory.Timeout = -1;
                var xml = string.Empty;
                if (objUtil.ErrCheck(GetVehicleAreaSearch_NewTZ(sn.UserID, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, fleetIds, boxIds, 0, PolygonPoints, ref xml), false))
                    if (objUtil.ErrCheck(GetVehicleAreaSearch_NewTZ(sn.UserID, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, fleetIds, boxIds, 0, PolygonPoints, ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone_NewTZ()).Replace("-04:00", getUserTimezone_NewTZ());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                Response.Write("<error>" + Ex + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex + "</error>");
            }
        }

        // Changes for TimeZone Feature end

        private void searchHistoryAddressByDasLogic()
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var dbhistory = new ServerDBHistory.DBHistory();
                //dbhistory.get

                objUtil = new clsUtility(sn);

                double lon = 0;
                double lat = 0;
                var SearchHistoryDateTime = Request["SearchHistoryDateTime"];
                var SearchHistoryTimeRange = 60;
                double radius = 2000;
                double.TryParse(Request["lon"], out lon);
                double.TryParse(Request["lat"], out lat);
                int.TryParse(Request["SearchHistoryTimeRange"], out SearchHistoryTimeRange);
                double.TryParse(Request["radius"], out radius);
                var PolygonPoints = Request["mapSearchPointSets"];

                var dtfrom = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(-SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");
                var dtto = DateTime.ParseExact(SearchHistoryDateTime, sn.User.DateFormat + " HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(SearchHistoryTimeRange).ToString("MM/dd/yyyy HH:mm:ss");

                var fleetIds = Request["FleetIds"] == null ? "" : Request["FleetIds"];
                var boxIds = Request["BoxIds"] == null ? "" : Request["BoxIds"];

                /*string xml = "<Fleet>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>939393</BoxId>";
                xml += "<VehicleId>44562</VehicleId>";
                xml += "<LicensePlate>LP_939393</LicensePlate>";
                xml += "<Description>939393-QingQing</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "<VehiclesHistoryAddressInformation>";
                xml += "<BoxId>200200</BoxId>";
                xml += "<VehicleId>31320</VehicleId>";
                xml += "<LicensePlate>LP_200200</LicensePlate>";
                xml += "<Description>Ravi_3000_Test_Unit</Description>";
                xml += "</VehiclesHistoryAddressInformation>";
                xml += "</Fleet>";*/
                dbhistory.Timeout = -1;
                var xml = string.Empty;
                if (objUtil.ErrCheck(GetVehicleAreaSearch(sn.UserID, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, fleetIds, boxIds, 0, PolygonPoints, ref xml), false))
                    if (objUtil.ErrCheck(GetVehicleAreaSearch(sn.UserID, lat, lon, radius, dtfrom, dtto, sn.User.OrganizationId, fleetIds, boxIds, 0, PolygonPoints, ref xml), true))
                    {
                        return;
                    }

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = xml.Trim().Replace("<br>", "").Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
                }
                Response.Write(xml);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                Response.Write("<error>" + Ex + "</error>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                Response.Write("<error>" + Ex + "</error>");
            }
        }

        // Changes for TimeZone Feature start

        private void getVehicleInfo_NewTZ(int vehicleId)
        {
            Response.ContentType = "text/html";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var ds = new DataSet();
                var xml = "";
                var dbv = new ServerDBVehicle.DBVehicle();

                var vehicleInfo = new Dictionary<string, string>();

                objUtil = new clsUtility(sn);

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, vehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, vehicleId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(new StringReader(xml));

                if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(vehicleId), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(vehicleId), ref xml), true))
                    {
                        return;
                    }

                if (xml != "")
                {

                    var dsInfo = new DataSet();
                    dsInfo.ReadXml(new StringReader(xml));

                    if (dsInfo.Tables.Count > 0 && dsInfo.Tables[0].Rows.Count > 0)
                    {
                        vehicleInfo.Add("Field1", dsInfo.Tables[0].Rows[0]["Field1"].ToString());
                        vehicleInfo.Add("Field2", dsInfo.Tables[0].Rows[0]["Field2"].ToString());
                        vehicleInfo.Add("Field3", dsInfo.Tables[0].Rows[0]["Field3"].ToString());
                        vehicleInfo.Add("Field4", dsInfo.Tables[0].Rows[0]["Field4"].ToString());
                        vehicleInfo.Add("Field5", dsInfo.Tables[0].Rows[0]["Field5"].ToString());
                    }
                }

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                Response.Write(js.Serialize(vehicleInfo));



            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature end

        private void getVehicleInfo(int vehicleId)
        {
            Response.ContentType = "text/html";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var ds = new DataSet();
                var xml = "";
                var dbv = new ServerDBVehicle.DBVehicle();

                var vehicleInfo = new Dictionary<string, string>();

                objUtil = new clsUtility(sn);

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, vehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, vehicleId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(new StringReader(xml));


                if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(vehicleId), ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleAdditionalInfoXML(sn.UserID, sn.SecId, Convert.ToInt64(vehicleId), ref xml), true))
                    {
                        return;
                    }

                if (xml != "")
                {

                    var dsInfo = new DataSet();
                    dsInfo.ReadXml(new StringReader(xml));

                    if (dsInfo.Tables.Count > 0 && dsInfo.Tables[0].Rows.Count > 0)
                    {
                        vehicleInfo.Add("Field1", dsInfo.Tables[0].Rows[0]["Field1"].ToString());
                        vehicleInfo.Add("Field2", dsInfo.Tables[0].Rows[0]["Field2"].ToString());
                        vehicleInfo.Add("Field3", dsInfo.Tables[0].Rows[0]["Field3"].ToString());
                        vehicleInfo.Add("Field4", dsInfo.Tables[0].Rows[0]["Field4"].ToString());
                        vehicleInfo.Add("Field5", dsInfo.Tables[0].Rows[0]["Field5"].ToString());
                    }
                }

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                Response.Write(js.Serialize(vehicleInfo));



            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        private void GetBoxInfo(int BoxId)
        {
            Response.ContentType = "text/html";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                var ds = new DataSet();
                ds.Tables.Add(sn.Map.DsFleetInfo.Tables["VehiclesLastKnownPositionInformation"].Select("BoxId=" + BoxId).CopyToDataTable());
                DataTable tblSensors;
                var dtResult = this.GetAllSensorsForVehicle(ds.Tables[0].Rows[0]["LicensePlate"].ToString(), true);
                if ((dtResult != null) && (dtResult.Rows.Count > 0))
                {
                    tblSensors = ds.Tables[0];

                    // move over all sensors and set current status
                    short snsId = 0;
                    var slashIndex = 0;
                    var objRow = new object[3];
                    var fldAction = "";
                    ulong checkBit = 1, shift = 1;
                    foreach (DataRow ittr in dtResult.Rows)
                    {
                        try
                        {
                            if (!ittr["SensorName"].ToString().TrimEnd().Contains(VLF.CLS.Def.Const.keySensorNotInUse))
                            {
                                tblSensors.Columns.Add(ittr["SensorName"].ToString(), typeof(string));
                                snsId = Convert.ToInt16(ittr["SensorId"]);

                                // if not AVL sensor, ignore
                                if ((snsId & (short)Enums.ReeferBase) > 0)
                                    continue;

                                checkBit = shift << (snsId - 1);

                                fldAction = ittr["SensorAction"].ToString().TrimEnd();
                                slashIndex = fldAction.IndexOf("/");
                                if (slashIndex < 1)
                                {
                                    // wrong sensors format in the database (should be action1/action2)
                                    //continue;
                                    //snsAction = "Invalid";
                                    tblSensors.Rows[0][ittr["SensorName"].ToString()] = "Invalid";
                                }
                                else
                                {
                                    if ((Convert.ToUInt64(ds.Tables[0].Rows[0]["SensorMask"]) & checkBit) == 0)
                                        tblSensors.Rows[0][ittr["SensorName"].ToString()] = fldAction.Substring(slashIndex + 1).TrimEnd();
                                    else
                                        tblSensors.Rows[0][ittr["SensorName"].ToString()] = fldAction.Substring(0, slashIndex).TrimEnd();
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                }

                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    if (col.ColumnName == "OriginDateTime" || col.ColumnName == "LandmarkInDateTime")
                        dict.Add(col.ColumnName, ds.Tables[0].Rows[0][col].ToString());
                    else
                        dict[col.ColumnName] = ds.Tables[0].Rows[0][col];
                }

                if (showDashboardView)
                {
                    if (ds.Tables[0].Columns.Contains("LandmarkID") && ds.Tables[0].Rows[0]["LandmarkID"].ToString().Trim() != "")
                    {
                        long landmarkId = 0;
                        long.TryParse(ds.Tables[0].Rows[0]["LandmarkID"].ToString(), out landmarkId);

                        long vehicleId = 0;
                        long.TryParse(ds.Tables[0].Rows[0]["VehicleId"].ToString(), out vehicleId);

                        var boxId = 0;
                        int.TryParse(ds.Tables[0].Rows[0]["BoxId"].ToString(), out boxId);

                        if (landmarkId > 0 && vehicleId > 0)
                        {
                            var _vehicle = new VLF.DAS.Logic.Vehicle(sConnectionString);
                            var dsService = _vehicle.GetServiceConfigurationsByLandmarkAndVehicle(sn.User.OrganizationId, vehicleId, landmarkId);
                            var serviceList = new string[dsService.Tables[0].Rows.Count, 2];
                            for (var i = 0; i < dsService.Tables[0].Rows.Count; i++)
                            {
                                serviceList[i, 0] = dsService.Tables[0].Rows[i]["ServiceConfigID"].ToString();
                                serviceList[i, 1] = dsService.Tables[0].Rows[i]["ServiceConfigName"].ToString();
                            }

                            dict.Add("ServiceConfigurations", serviceList);

                            var dictVehicleAvailableEmailSetting = GetVehicleAvailableEmailSetting(int.Parse(serviceList[0, 0]), vehicleId, landmarkId, boxId);
                            //int landmarkDuration = serviceList.Length > 1 ? getDuration(int.Parse(serviceList[0,0]), vehicleId, landmarkId, boxId) : -1;
                            var landmarkDuration = -1;
                            int.TryParse(dictVehicleAvailableEmailSetting["PeriodicEmailDurationInMinute"], out landmarkDuration);
                            if (landmarkDuration > 0)
                            {
                                landmarkDuration = landmarkDuration / 60;
                            }
                            dict.Add("LandmarkDuration", landmarkDuration);
                            dict.Add("ShouldSendEmailImmediately", dictVehicleAvailableEmailSetting["ShouldSendEmailImmediately"].ToLower());

                        }
                    }

                }

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                Response.Write(js.Serialize(dict));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        private int GetDuration(int serviceConfigId, long vehicleId, long landmarkId, int boxId)
        {
            var rvDuration = -1;

            try
            {
                var clientGeomarkService = new GeomarkServiceClient("httpbasic");
                rvDuration = clientGeomarkService.GetPostpone(boxId, (int)landmarkId, serviceConfigId);
                if (rvDuration == -1)
                {
                    rvDuration = 24;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }

            return rvDuration;
        }

        private Dictionary<string, string> GetVehicleAvailableEmailSetting(int serviceConfigId, long vehicleId, long landmarkId, int boxId)
        {
            var dictVehicleAvailableEmailSetting = new Dictionary<string, string>();

            try
            {
                var clientGeomarkService = new GeomarkServiceClient("httpbasic");
                dictVehicleAvailableEmailSetting = clientGeomarkService.GetVehicleAvailableEmailSetting(boxId, (int)landmarkId, serviceConfigId);
                if (dictVehicleAvailableEmailSetting == null)
                {
                    dictVehicleAvailableEmailSetting = new Dictionary<string, string>();
                    dictVehicleAvailableEmailSetting.Add("PeriodicEmailDurationInMinute", "-1");
                    dictVehicleAvailableEmailSetting.Add("ShouldSendEmailImmediately", "True");
                }

                if (!dictVehicleAvailableEmailSetting.ContainsKey("PeriodicEmailDurationInMinute"))
                {
                    dictVehicleAvailableEmailSetting.Add("PeriodicEmailDurationInMinute", "-1");
                }

                if (!dictVehicleAvailableEmailSetting.ContainsKey("ShouldSendEmailImmediately"))
                {
                    dictVehicleAvailableEmailSetting.Add("ShouldSendEmailImmediately", "True");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }

            return dictVehicleAvailableEmailSetting;
        }

        private DataTable getIcon(DataTable dt)
        {
            var posExpireDate = DateTime.Now.Add(new TimeSpan(0, -Convert.ToInt32(sn.User.PositionExpiredTime), 0));
            if (!dt.Columns.Contains("icon"))
            {
                var dc = new DataColumn("icon", typeof(string));
                dt.Columns.Add(dc);
            }
            if (!dt.Columns.Contains("ImagePath"))
            {
                var dc = new DataColumn("ImagePath", typeof(string));
                dt.Columns.Add(dc);
            }
            foreach (DataRow row in dt.Rows)
            {
                string newIcon;
                if (!string.IsNullOrEmpty(row["ImagePath"].ToString()))
                {
                    if (Convert.ToDateTime(row["OriginDateTime"]) < posExpireDate)
                        newIcon = "Grey";
                    else
                    {
                        if (Convert.ToInt32(row["CustomSpeed"]) != 0)
                            newIcon = "Green";
                        else
                            newIcon = "Red";
                        if (row["VehicleStatus"].ToString().IndexOf("Idling") > -1 || row["VehicleStatus"].ToString().IndexOf("Moteur au ralenti") > -1)
                            newIcon = "Orange";
                        if (row["PTO"].ToString() == "On")
                            newIcon = "Blue";
                    }

                    var bicon = row["ImagePath"].ToString().Replace("\\", "/");
                    if (bicon.Split('/').Length > 1)
                        newIcon = bicon.Split('/')[0] + "/" + newIcon + bicon.Split('/')[1];
                    else
                        newIcon = newIcon + bicon;
                    row["ImagePath"] = newIcon;
                }
                else
                {
                    if (Convert.ToDateTime(row["OriginDateTime"]) < posExpireDate)
                        newIcon = "Grey" + row["IconTypeName"] + ".ico";
                    else
                    {
                        if (Convert.ToInt32(row["CustomSpeed"]) != 0)
                            newIcon = "Green" + row["IconTypeName"] + row["MyHeading"] + ".ico";
                        else
                            newIcon = "Red" + row["IconTypeName"] + ".ico";
                        if (row["VehicleStatus"].ToString().IndexOf("Idling") > -1 || row["VehicleStatus"].ToString().IndexOf("Moteur au ralenti") > -1)
                            newIcon = "Orange" + row["IconTypeName"] + ".ico";
                        if (row["PTO"].ToString() == "On")
                            newIcon = "Blue" + row["IconTypeName"] + ".ico";
                    }
                    row["ImagePath"] = "";
                }
                row["Icon"] = newIcon;
            }
            return dt;
        }

        private void GetVehilcesByLandmarkId(int landmarkId)
        {
            try
            {
                var dstemp = new DataSet();
                dstemp = sn.Map.DsFleetInfoNew.Copy();

                var sortedTable = dstemp.Tables[0];
                sortedTable = sortedTable.Select("LandmarkID=" + landmarkId).CopyToDataTable();

                sortedTable = getIcon(sortedTable);
                var view = new System.Data.DataView(sortedTable);
                sortedTable = view.ToTable("VehiclesLastKnownPositionInformation", false, "BoxId", "OriginDateTime", "Latitude", "Longitude", "Description", "Driver", "icon", "ImagePath");
                if (dstemp.Tables.CanRemove(dstemp.Tables[0]))
                {
                    dstemp.Tables.Remove(dstemp.Tables[0]);
                }
                dstemp.Tables.Add(sortedTable);
                dstemp.DataSetName = "Fleet";
                Response.ContentType = "text/html";
                Response.ContentEncoding = Encoding.UTF8;
                if (sortedTable.Rows.Count > 0)
                {
                    try
                    {
                        var files = new DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("vehiclesfile1_*.xml");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromHours(10))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + Ex.StackTrace + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                    }
                    var filename = Server.MapPath("TempReports/") + "vehiclesfile1_" + sn.UserID + "_" + Session.SessionID + "_" + Guid.NewGuid() + ".xml";
                    dstemp.WriteXml(filename);
                    var doc = new XmlDocument();
                    doc.Load(filename);
                    Response.Write(JsonConvert.SerializeXmlNode(doc));
                }
                else
                {
                    var doc = new XmlDocument();
                    doc.LoadXml("<Fleet><VehiclesLastKnownPositionInformation></VehiclesLastKnownPositionInformation></Fleet>");
                    //Response.Write(JsonConvert.SerializeXmlNode(doc));
                    Response.Write("{\"Fleet\":{\"VehiclesLastKnownPositionInformation\":[]}}");
                }

            }
            catch (Exception Ex)
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<error>" + Ex.Message + Ex.StackTrace + "</error>");

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + Ex.StackTrace + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
            }
        }

        public static string GetLocalResourceValue(string key)
        {
            var path = HttpContext.Current.Server.MapPath("App_LocalResources/frmFleetInfoNew.aspx");
            return clsAsynGenerateReport.GetResourceObject(path, key);
        }

        protected DataTable GetAllSensorsForVehicle(string licensePlate, bool getAllSensors)
        {
            var dsSensors = new DataSet("Sensors");
            var dtSensors = new DataTable("BoxSensors");
            try
            {
                var xml = "";
                objUtil = new clsUtility(sn);
                var dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                           "No Sensors for vehicle: " + licensePlate + " User:" + sn.UserID + " Form:" + Page.GetType().Name));
                    }

                if (string.IsNullOrEmpty(xml))
                {
                    return null;
                }

                dsSensors.ReadXml(new StringReader(xml));
                dtSensors.Columns.Add("SensorId", typeof(short));
                dtSensors.Columns.Add("SensorName", typeof(string));
                dtSensors.Columns.Add("SensorAction", typeof(string));
                dtSensors.Columns.Add("AlarmLevelOn", typeof(short));
                dtSensors.Columns.Add("AlarmLevelOff", typeof(short));

                if (VLF.CLS.Util.IsDataSetValid(dsSensors))
                {
                    foreach (DataRow rowSensor in dsSensors.Tables[0].Rows)
                    {
                        if (!getAllSensors)
                            if (Convert.ToInt16(rowSensor["SensorId"]) > VLF.CLS.Def.Enums.ReeferBase)
                                continue;
                        dtSensors.ImportRow(rowSensor);
                    }
                }
            }
            catch { }

            return dtSensors;
        }

        protected DataSet GetVehicleOperationalState(int userId, int orgId, long vehicleId)
        {
            var dbVehicle = new Vehicle(sConnectionString);
            var dsResult = dbVehicle.GetVehicleOperationalState(userId, orgId, vehicleId);
            return dsResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = false, XmlSerializeString = false)]
        public static string UpdatePosition(string boxIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || string.IsNullOrEmpty(sn.UserName)) return "-1";


            var replyMesgs = new StringBuilder();
            try
            {
                long sessionTimeOut = 0;
                var objUtil = new clsUtility(sn);

                var successBox = new StringBuilder();
                var failedBox = new StringBuilder();
                var cmdSent = false;
                var replyStr = string.Empty;
                if (boxIDs == string.Empty) return "1";

                var boxIDArr = boxIDs.Split(',');
                var dbl = new LocationMgr.Location();
                foreach (var boxId in boxIDArr)
                {
                    if (!string.IsNullOrEmpty(boxId))
                    {
                        short ProtocolId = -1;
                        short CommModeId = -1;
                        var errMsg = "";
                        if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(boxId), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                            if (errMsg == "")
                            {
                                if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(boxId), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                                {
                                    if (errMsg != "")
                                    {
                                        sn.MessageText = errMsg;
                                        replyStr = errMsg;
                                    }
                                    else
                                    {
                                        sn.MessageText = GetLocalResourceValue("sn_MessageText_SendCommandFailedError") + ": ";
                                        replyStr = errMsg;
                                    }

                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " User:" + sn.UserID + "Web method: UpdatePosition() Page:Vehicles.aspx"));
                                }
                            }
                            else
                            {
                                replyStr = errMsg;
                            }
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            //replyStr = "Command sent to box successfully";
                            if (successBox.ToString().Length > 0)
                            {
                                successBox.Append("," + boxId);
                            }
                            else
                                successBox.Append(boxId);
                        }
                        else
                        {
                            if (failedBox.ToString().Length > 0)
                            {
                                failedBox.Append("," + boxId);
                            }
                            else
                                failedBox.Append(boxId);
                        }
                    }
                }
                if (successBox.ToString().Length > 0)
                {
                    replyMesgs.AppendLine("Vehicle " + successBox + " has received updateposition command successfully.");
                }
                if (failedBox.ToString().Length > 0)
                {
                    replyMesgs.AppendLine("Vehicle " + failedBox + " didn't received updateposition command. Error occured... Try again...");
                }

                //return "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message + " User:" + sn.UserID + "Web method: UpdatePosition() Page:Vehicles.aspx"));
                var trace = new System.Diagnostics.StackTrace(ex, true);
                replyMesgs.AppendLine("Error occured please contact BSM for this error...");
                //return "0";
            }
            return SerializeToJson(replyMesgs.ToString());
        }

        public static string SerializeToJson(object o)
        {
            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return js.Serialize(o);
        }

        public void GetLandmarkDuration(int serviceConfigId, long vehicleId, long landmarkId, int boxId)
        {
            Response.ContentType = "text/html";
            Response.ContentEncoding = Encoding.UTF8;

            var dict = new Dictionary<string, object>();

            SentinelFMSession sn = null;
            if (HttpContext.Current.Session["SentinelFMSession"] != null)
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];

            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

            if (sn == null || sn.User == null || string.IsNullOrEmpty(sn.UserName) || sn.UserID <= 0)
            {
                dict.Add("status", 500);
                dict.Add("message", "Your session is timeout, please login again.");
                Response.Write(SerializeToJson(dict));
            }

            var dictVehicleAvailableEmailSetting = GetVehicleAvailableEmailSetting(serviceConfigId, vehicleId, landmarkId, boxId);
            //int landmarkDuration = getDuration(serviceConfigId, vehicleId, landmarkId, boxId);

            var landmarkDuration = -1;
            int.TryParse(dictVehicleAvailableEmailSetting["PeriodicEmailDurationInMinute"], out landmarkDuration);
            if (landmarkDuration > 0)
            {
                landmarkDuration = landmarkDuration / 60;
            }
            dict.Add("landmarkDuration", landmarkDuration);
            dict.Add("ShouldSendEmailImmediately", dictVehicleAvailableEmailSetting["ShouldSendEmailImmediately"].ToLower());

            dict.Add("status", 200);
            //dict.Add("landmarkDuration", landmarkDuration);
            Response.Write(SerializeToJson(dict));
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
        }

        // Changes for TimeZone Feature start
        private string getUserTimezone_NewTZ()
        {
            return "";

            if (sn.User.NewFloatTimeZone >= 0 && sn.User.NewFloatTimeZone < 10)
                return "+0" + sn.User.NewFloatTimeZone + ":00";
            else if (sn.User.NewFloatTimeZone >= 10)
                return "+" + sn.User.NewFloatTimeZone + ":00";
            else if (sn.User.NewFloatTimeZone < 0 && sn.User.NewFloatTimeZone > -10)
                return "-0" + Math.Abs(sn.User.NewFloatTimeZone) + ":00";
            else
                return sn.User.NewFloatTimeZone + ":00";
        }
        // Changes for TimeZone Feature end

        private string getUserTimezone()
        {
            return "";

            if (sn.User.TimeZone >= 0 && sn.User.TimeZone < 10)
                return "+0" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone >= 10)
                return "+" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone < 0 && sn.User.TimeZone > -10)
                return "-0" + Math.Abs(sn.User.TimeZone) + ":00";
            else
                return sn.User.TimeZone + ":00";
        }

        // Changes for TimeZone Feature start
        private int GetVehicleAreaSearch_NewTZ(int userId, double Latitude, double Longitude, double Radius,
                    string fromDate, string toDate, int orgId, string fleetIds, string boxIds, int LandmarkID, string PolygonPoints, ref string xml)
        {

            try
            {

                var ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                DataSet dsResult = null;

                using (var ms = new MapSearch(ConnnectionString))
                {

                    dsResult = ms.GetVehicleAreaSearch_NewTZ(Latitude, Longitude, Radius, fromDate, toDate, orgId, LandmarkID, PolygonPoints, fleetIds, boxIds, userId);

                    //if (Util.IsDataSetValid(dsResult))
                    if (dsResult.Tables[0].Rows.Count > 0)
                        xml = dsResult.GetXml();
                    else
                        return 12;
                }

                return 0;
            }
            catch (Exception Ex)
            {
                return 1;
            }
        }


        // Changes for TimeZone Feature end

        private int GetVehicleAreaSearch(int userId, double Latitude, double Longitude, double Radius,
                    string fromDate, string toDate, int orgId, string fleetIds, string boxIds, int LandmarkID, string PolygonPoints, ref string xml)
        {

            try
            {

                var ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                DataSet dsResult = null;

                using (var ms = new MapSearch(ConnnectionString))
                {

                    dsResult = ms.GetVehicleAreaSearch(Latitude, Longitude, Radius, fromDate, toDate, orgId, LandmarkID, PolygonPoints, fleetIds, boxIds, userId);

                    //if (Util.IsDataSetValid(dsResult))
                    if (dsResult.Tables[0].Rows.Count > 0)
                        xml = dsResult.GetXml();
                    else
                        return 12;
                }

                return 0;
            }
            catch (Exception Ex)
            {
                return 1;
            }
        }

        private void ListVehiclesInLandmarksForDashboard(int landmarkCategoryId)
        {
            try
            {

                var connnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                DataSet dsResult = null;
                Response.ContentType = "text/xml";
                using (var dbVehicle = new VLF.DAS.Logic.Vehicle(sConnectionString))
                {

                    dsResult = dbVehicle.ListVehiclesInLandmarksForDashboard(sn.UserID, sn.User.OrganizationId, landmarkCategoryId);

                    Response.Write(dsResult.GetXml());
                }


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + "Web method: ListVehiclesInLandmarksForDashboard() Page:Vehicles.aspx"));
                var trace = new System.Diagnostics.StackTrace(Ex, true);
                Response.Write("<Vehicle><VehicleList></VehicleList><Error>" + Ex.Message + "</Error></Vehicle>");
            }
        }

        private Dictionary<string, VehicleLandmarkDashboard> ConvertToDictionary(DataSet inputDataSet)
        {
            var rvDict = new Dictionary<string, VehicleLandmarkDashboard>();
            VehicleLandmarkDashboard oneRecord = null;

            foreach (DataRow oneRow in inputDataSet.Tables[0].Rows)
            {
                if (rvDict.ContainsKey(oneRow["VehicleID"].ToString()) == true)
                {
                    oneRecord = rvDict[oneRow["VehicleID"].ToString()];
                    oneRecord.LandmarkName = string.Format("{0} || {1} ({2})", oneRecord.LandmarkName,
                                                            oneRow["LandmarkName"], oneRow["DurationInLandmarkMin"]);
                }
                else
                {
                    oneRecord = new VehicleLandmarkDashboard();

                    var operationalState = "";
                    if (oneRow["OperationalState"].ToString() == "100")
                        operationalState = "Available";
                    else if (oneRow["OperationalState"].ToString() == "200")
                        operationalState = "Unavailable";

                    //oneRecord.OperationalState = operationalState;
                    //oneRecord.OperationalStateNotes = oneRow["OperationalStateNotes"].ToString();
                    oneRecord.DurationInLandmarkMin = Convert.ToInt32(int.Parse(oneRow["DurationInLandmarkMin"].ToString()) / 60.0).ToString(); //oneRow["DurationInLandmarkMin"].ToString(); // convert to hours
                    oneRecord.LandmarkID = oneRow["LandmarkID"].ToString();
                    oneRecord.LandmarkName = oneRow["LandmarkName"].ToString();
                    oneRecord.VehicleID = oneRow["VehicleID"].ToString();
                    oneRecord.LandmarkEventId = oneRow["ID"].ToString();
                    oneRecord.LandmarkInDateTime = oneRow["LandmarkInDateTime"].ToString();

                    rvDict.Add(oneRecord.VehicleID, oneRecord);
                }
            }

            return rvDict;
        }

        private void mergeLandarksToVehicleList()
        {
            try
            {

                var myConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                DataSet dsResult = null;
                //Response.ContentType = "text/xml";
                using (var dbVehicle = new VLF.DAS.Logic.Vehicle(myConnnectionString))
                {
                    var landmarkCategoryId = 0;
                    int.TryParse(Request.QueryString["landmarkCategoryId"], out landmarkCategoryId);

                    dsResult = dbVehicle.ListVehiclesInLandmarksForDashboard(sn.UserID, sn.User.OrganizationId, landmarkCategoryId);

                    var resultDictionary = ConvertToDictionary(dsResult);
                    VehicleLandmarkDashboard oneRecord = null;

                    //if (resultDictionary.Count > 0) { 
                    for (var i = 0; i < sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Count; i++)
                    {
                        try
                        {
                            //sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalState"] = "";
                            //sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalStateNotes"] = "";
                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["DurationInLandmarkMin"] = 0;
                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkID"] = 0;
                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkName"] = "";
                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkEventId"] = 0;
                            //sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkInDateTime"] = DBNull;

                            if (resultDictionary.ContainsKey(sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["VehicleId"].ToString()) == true)
                            {
                                oneRecord = resultDictionary[sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["VehicleId"].ToString()];

                                //sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalState"] = oneRecord.OperationalState;
                                //sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalStateNotes"] = oneRecord.OperationalStateNotes;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["DurationInLandmarkMin"] = oneRecord.DurationInLandmarkMin;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkID"] = oneRecord.LandmarkID;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkName"] = oneRecord.LandmarkName;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkEventId"] = oneRecord.LandmarkEventId;
                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkInDateTime"] = oneRecord.LandmarkInDateTime;

                                sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                            }

                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
                        }
                    }
                    //}

                    //Response.Write(dsResult.GetXml());
                }


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " User:" + sn.UserID + "Web method: ListVehiclesInLandmarksForDashboard() Page:Vehicles.aspx"));
                var trace = new System.Diagnostics.StackTrace(Ex, true);
                return;
            }
        }

        //private void mergeLandarksToVehicleList()
        //{
        //    try
        //    {

        //        string myConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

        //        DataSet dsResult = null;
        //        //Response.ContentType = "text/xml";
        //        using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(myConnnectionString))
        //        {
        //            int landmarkCategoryId = 0;
        //            int.TryParse(Request.QueryString["landmarkCategoryId"], out landmarkCategoryId);                    

        //            dsResult = dbVehicle.ListVehiclesInLandmarksForDashboard(sn.UserID, sn.User.OrganizationId, landmarkCategoryId);

        //            for (int i = 0; i < sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalState"] = "";
        //                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalStateNotes"] = "";
        //                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["DurationInLandmarkMin"] = 0;
        //                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkID"] = 0;
        //                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkName"] = "";

        //                    for (int j = 0; j < dsResult.Tables[0].Rows.Count;j++ )
        //                    {
        //                        if(dsResult.Tables[0].Rows[j]["VehicleID"].ToString() == sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["VehicleId"].ToString())
        //                        {
        //                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalState"] = dsResult.Tables[0].Rows[j]["OperationalState"];
        //                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["OperationalStateNotes"] = dsResult.Tables[0].Rows[j]["OperationalStateNotes"];
        //                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["DurationInLandmarkMin"] = dsResult.Tables[0].Rows[j]["DurationInLandmarkMin"];
        //                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkID"] = dsResult.Tables[0].Rows[j]["LandmarkID"];
        //                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[i]["LandmarkName"] = dsResult.Tables[0].Rows[j]["LandmarkName"];

        //                            sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
        //                        }
        //                    }

        //                }
        //                catch (Exception Ex)
        //                {
        //                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        //                }
        //            }

        //            //Response.Write(dsResult.GetXml());
        //        }


        //    }
        //    catch (Exception Ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: ListVehiclesInLandmarksForDashboard() Page:Vehicles.aspx"));
        //        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
        //        return;
        //    }
        //}
    }


    class VehicleLandmarkDashboard
    {
        public VehicleLandmarkDashboard()
        {

        }

        //public string OperationalState { get; set; }
        //public string OperationalStateNotes { get; set; }
        public string DurationInLandmarkMin { get; set; }
        public string LandmarkID { get; set; }
        public string LandmarkName { get; set; }
        public string VehicleID { get; set; }
        public string LandmarkEventId { get; set; }
        public string LandmarkInDateTime { get; set; }
    }


    public static class DistanceAlgorithm
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6378.16;

        /// <summary>
        /// This class cannot be instantiated.
        /// </summary>
        //private DistanceAlgorithm() { }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * PIx / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            var dlon = Radians(lon2 - lon1);
            var dlat = Radians(lat2 - lat1);

            var a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            var angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }
    }
}
