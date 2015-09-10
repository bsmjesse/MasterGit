<%@ WebService Language="C#" Class="NewMapGeozoneLandmark" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.EnterpriseServices;
using System.Text;
using System.Data.Common;
using Telerik.Web.UI;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using SentinelFM;
using VLF.CLS;
using System.Collections;
using Telerik.Web.UI.GridExcelBuilder;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.IO;
using VLF.MAP;
using SentinelFM.ServerDBOrganization;
using VLF.PATCH.Logic;
using VLF.ERRSecurity;
using VLF.ERR;
using VLF.CLS.Def;
using SentinelFM.GeomarkServiceRef;
using System.Resources;
using System.Xml;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class NewMapGeozoneLandmark  : System.Web.Services.WebService {
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    VLF.DAS.Logic.LandmarkPointSetManager landPointMgr = null;
    VLF.PATCH.Logic.PatchLandmarkPointSet patchLandPointMgr = null;
    VLF.PATCH.Logic.PatchServices _patchServices = null;
    VLF.PATCH.Logic.PatchLandmark _patchLandmark = null;
    
    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }


    
    
    public class VehicleAvailabilityPieModel
    {
        public int OperationState { get; set; }
        public string OperationStateDisplayText { get; set; }
        public string name { get; set; }
        public string FleetName { get; set; }
        public int Percentage { get; set; }
        public string PercentageDisplayText { get; set; }
        public int NumberOfVehicles { get; set; }
        public int y { get; set; }
    }
    
    public class ManagerVehiclesAvailabilityModel
    {
        public string name {get; set;}
        public int numAvailable {get;set;}
        public int numUnavailable {get;set;}
        public double percAvailable {get;set;}
        public double percUnavailable {get;set;}
        public int numTotal {get;set;}
        public int x { get; set; }
        public double y { get; set; }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false, XmlSerializeString = false)]
    public List<VehicleAvailabilityPieModel> GetVehicleAvailabilityByFleetForPieChart(int FleetId)
    {
        List<VehicleAvailabilityPieModel> rvList = new List<VehicleAvailabilityPieModel>();
        VehicleAvailabilityPieModel availablePieModel = null, unavailablePieModel = null;

        SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
        string fleetName = "";

        clsUtility objUtil;
        objUtil = new clsUtility(sn);


        try
        {
            DataSet dsResult = null;

            using (VLF.DAS.Logic.Fleet dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString))
            {
                dsResult = dbFleet.GetFleetInfoByFleetId(FleetId);
                DataTable oneDT = dsResult.Tables[0];
                if (oneDT.Rows.Count > 0)
                {
                    fleetName = oneDT.Rows[0]["FleetName"].ToString();
                }
            }

            availablePieModel = new VehicleAvailabilityPieModel()
            {
                FleetName = fleetName,
                OperationState = 100,
                OperationStateDisplayText = "Available",
                name = "Available"

            };

            unavailablePieModel = new VehicleAvailabilityPieModel()
            {
                FleetName = fleetName,
                OperationState = 200,
                OperationStateDisplayText = "Unavailable",
                name = "Unavailable"
            };

            string xml = "";

            SentinelFM.ServerDBVehicle.DBVehicle dbv = new SentinelFM.ServerDBVehicle.DBVehicle();
            if (objUtil.ErrCheck(dbv.GetVehicleAvailabilityByManagerForDashboard(sn.UserID, sn.User.OrganizationId, FleetId, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleAvailabilityByManagerForDashboard(sn.UserID, sn.User.OrganizationId, FleetId, sn.SecId, ref xml), true))
                {
                    
                }

            if (xml != "")
            {
                DataSet dsAvailability = new DataSet();
                
                System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                string strPath = Server.MapPath("../Datasets/VehicleAvailabilityByManager.xsd");
                dsAvailability.ReadXmlSchema(strPath);
                dsAvailability.ReadXml(strrXML);

                if (dsAvailability.Tables[0].Rows.Count > 0)
                {
                    int totalNumberOfRecords = dsAvailability.Tables[0].AsEnumerable()
                           .Sum(x => x.Field<int>("Total"));

                    int availableCount = dsAvailability.Tables[0].AsEnumerable()
                           .Sum(x => x.Field<int>("NumberOfAvailable"));

                    availablePieModel.NumberOfVehicles = availableCount;
                    availablePieModel.y = availableCount;
                    availablePieModel.Percentage = (int)Math.Round(((availableCount / (double)totalNumberOfRecords) * 100), 0);
                    unavailablePieModel.NumberOfVehicles = totalNumberOfRecords - availableCount;
                    unavailablePieModel.y = totalNumberOfRecords - availableCount;
                    unavailablePieModel.Percentage = 100 - availablePieModel.Percentage;

                    availablePieModel.PercentageDisplayText = string.Format("{0}%", availablePieModel.Percentage.ToString());
                    unavailablePieModel.PercentageDisplayText = string.Format("{0}%", unavailablePieModel.Percentage.ToString());
                    rvList.Add(availablePieModel);
                    rvList.Add(unavailablePieModel);
                }
            }
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                Ex.Message.ToString() + " User ID: " + sn.UserID.ToString() + ", Fleet ID:" + FleetId.ToString() + 
                ", Method: GetVehicleAvailabilityByFleetForPieChart() in NewMapGeozoneLandmark.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
        }
        
        return rvList;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = false, XmlSerializeString = false)]
    public XmlDocument GetManagerVehiclesAvailability(int FleetId)
    {

        SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
        clsUtility objUtil;
        objUtil = new clsUtility(sn);
        
        XmlDocument xmlDoc = new XmlDocument();
        DataSet ds = new DataSet("Fleet");

        try
        {
            string xml = "";

            SentinelFM.ServerDBVehicle.DBVehicle dbv = new SentinelFM.ServerDBVehicle.DBVehicle();
            if (objUtil.ErrCheck(dbv.GetVehicleAvailabilityByManagerForDashboard(sn.UserID, sn.User.OrganizationId, FleetId, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleAvailabilityByManagerForDashboard(sn.UserID, sn.User.OrganizationId, FleetId, sn.SecId, ref xml), true))
                {
                    
                }

            if (xml != "")
            {
                DataSet dsAvailability = new DataSet();

                System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                string strPath = Server.MapPath("../Datasets/VehicleAvailabilityByManager.xsd");
                dsAvailability.ReadXmlSchema(strPath);
                dsAvailability.ReadXml(strrXML);

                dsAvailability.Tables[0].Columns.Add(new DataColumn("PercAvailable", Type.GetType("System.Double")));
                dsAvailability.Tables[0].Columns.Add(new DataColumn("PercUnavailable", Type.GetType("System.Double")));
                
                foreach (DataRow dr in dsAvailability.Tables[0].Rows)
                {
                    if (dr["ManagerName"].ToString().Trim() == "")
                    {
                        dr["ManagerName"] = "Unassigned";
                    }
                    dr["PercAvailable"] = (Convert.ToDouble(dr["NumberOfAvailable"]) / Convert.ToDouble(dr["Total"])) * 100d;
                    dr["PercUnavailable"] = (Convert.ToDouble(dr["NumberOfUnavailable"]) / Convert.ToDouble(dr["Total"])) * 100d;
                    dsAvailability.Tables[0].AcceptChanges();
                }


                DataView dv = dsAvailability.Tables[0].DefaultView;
                dv.Sort = "ManagerName";
                DataTable sortedTable = dv.ToTable();
                sortedTable.TableName = "ManagerVehiclesAvailability";
                ds.Tables.Add(sortedTable);

                xmlDoc.LoadXml(ds.GetXml());

                XmlNode newElem = xmlDoc.CreateNode("element", "totalCount", "");
                newElem.InnerText = ds.Tables["ManagerVehiclesAvailability"].Rows.Count.ToString();
                XmlElement root = xmlDoc.DocumentElement;
                root.AppendChild(newElem);
                
            }
            
            return xmlDoc;


        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                Ex.Message.ToString() + " User ID: " + sn.UserID.ToString() + ", Fleet ID:" + FleetId.ToString() +
                ", Method: GetManagerVehiclesAvailability() in NewMapGeozoneLandmark.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            xmlDoc.LoadXml("<Fleet><ManagerVehiclesAvailability></ManagerVehiclesAvailability></Fleet>");
            return xmlDoc;
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false, XmlSerializeString = false)]
    public string GetManagerVehiclesAvailabilityJSON(int FleetId)
    {

        SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
        clsUtility objUtil;
        objUtil = new clsUtility(sn);
        
        XmlDocument xmlDoc = new XmlDocument();
        DataSet ds = new DataSet("Fleet");

        List<ManagerVehiclesAvailabilityModel> series1 = new List<ManagerVehiclesAvailabilityModel>();
        List<ManagerVehiclesAvailabilityModel> series2 = new List<ManagerVehiclesAvailabilityModel>();
        List<ManagerVehiclesAvailabilityModel> series3 = new List<ManagerVehiclesAvailabilityModel>();

        Dictionary<string, List<ManagerVehiclesAvailabilityModel>> returnList = new Dictionary<string, List<ManagerVehiclesAvailabilityModel>>();

        try
        {
            string xml = "";

            SentinelFM.ServerDBVehicle.DBVehicle dbv = new SentinelFM.ServerDBVehicle.DBVehicle();
            if (objUtil.ErrCheck(dbv.GetVehicleAvailabilityByManagerForDashboard(sn.UserID, sn.User.OrganizationId, FleetId, sn.SecId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleAvailabilityByManagerForDashboard(sn.UserID, sn.User.OrganizationId, FleetId, sn.SecId, ref xml), true))
                {
                    
                }

            if (xml != "")
            {
                DataSet dsAvailability = new DataSet();

                System.IO.StringReader strrXML = new System.IO.StringReader(xml);
                string strPath = Server.MapPath("../Datasets/VehicleAvailabilityByManager.xsd");
                dsAvailability.ReadXmlSchema(strPath);
                dsAvailability.ReadXml(strrXML);

                dsAvailability.Tables[0].Columns.Add(new DataColumn("PercAvailable", Type.GetType("System.Double")));
                dsAvailability.Tables[0].Columns.Add(new DataColumn("PercUnavailable", Type.GetType("System.Double")));

                foreach (DataRow dr in dsAvailability.Tables[0].Rows)
                {
                    if (dr["ManagerName"].ToString().Trim() == "")
                    {
                        dr["ManagerName"] = "Unassigned";
                    }
                    dr["PercAvailable"] = (Convert.ToDouble(dr["NumberOfAvailable"]) / Convert.ToDouble(dr["Total"])) * 100d;
                    dr["PercUnavailable"] = (Convert.ToDouble(dr["NumberOfUnavailable"]) / Convert.ToDouble(dr["Total"])) * 100d;
                    dsAvailability.Tables[0].AcceptChanges();
                }


                DataView dv = dsAvailability.Tables[0].DefaultView;
                dv.Sort = "ManagerName";
                DataTable sortedTable = dv.ToTable();

                int x = 1;

                foreach (DataRow dr in sortedTable.Rows)
                {
                    ManagerVehiclesAvailabilityModel mva = new ManagerVehiclesAvailabilityModel();
                    mva.name = dr["ManagerName"].ToString().Trim();
                    mva.numAvailable = Convert.ToInt32(dr["NumberOfAvailable"]);
                    mva.numUnavailable = Convert.ToInt32(dr["NumberOfUnavailable"]);
                    mva.percAvailable = Convert.ToDouble(dr["PercAvailable"]);
                    mva.percUnavailable = Convert.ToDouble(dr["PercUnavailable"]);
                    mva.numTotal = Convert.ToInt32(dr["Total"]);
                    mva.x = x;
                    mva.y = mva.percAvailable;

                    x++;

                    if (mva.percAvailable >= 90)
                        series1.Add(mva);
                    else if (mva.percAvailable < 90 && mva.percAvailable >= 75)
                        series2.Add(mva);
                    else
                        series3.Add(mva);

                }

                returnList.Add("series1", series1);
                returnList.Add("series2", series2);
                returnList.Add("series3", series3);
            }

        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                Ex.Message.ToString() + " User ID: " + sn.UserID.ToString() + ", Fleet ID:" + FleetId.ToString() +
                ", Method: GetManagerVehiclesAvailability() in NewMapGeozoneLandmark.asmx"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);            
        }

        JavaScriptSerializer js = new JavaScriptSerializer();
        js.MaxJsonLength = int.MaxValue;
        return js.Serialize(returnList);         
    }
    

        
    // Changes for TimeZone Feature start


    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public string GetAllGoezons_NewTZ()
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill_NewTZ(sn);


            if (sn.GeoZone.DsGeoZone != null && sn.GeoZone.DsGeoDetails != null)
            {
                try
                {
                    Resources.Const.Culture = new System.Globalization.CultureInfo(sn.SelectedLanguage);
                    DataSet myDataSet = sn.GeoZone.DsGeoZone;
                    LocalizeAddress(sn.SelectedLanguage, ref myDataSet);

                    string tableName = "";

                    ArrayList geoZones = new ArrayList();
                    int index = 0;
                    foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                    {
                        tableName = dr["GeoZoneId"].ToString();
                        sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                        if (sn.GeoZone.DsGeoDetails != null && sn.GeoZone.DsGeoDetails.Tables[tableName] != null && sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count > 0)
                        {
                            try
                            {
                                Dictionary<string, string> geoZone = new Dictionary<string, string>();
                                geoZone.Add("id", tableName);
                                geoZone.Add("geozoneName", dr["GeoZoneName"].ToString());
                                geoZone.Add("desc", dr["Description"].ToString());
                                geoZone.Add("type", "0");
                                geoZone.Add("Assigned", dr["Assigned"].ToString());
                                geoZone.Add("Direction", Enum.GetName(typeof(Enums.GeoZoneDirection), int.Parse(dr["Type"].ToString())).Replace("In", Resources.Const.GZ_Direction_In)
                                                                                                       .Replace("Out", Resources.Const.GZ_Direction_Out)
                                                                                                       .Replace("In/Out", Resources.Const.GZ_Direction_In_out)
                                                                                                       .Replace("Disable", Resources.Const.GZ_Direction_Disable));
                                geoZone.Add("SeverityName", dr["SeverityName"].ToString());
                                geoZone.Add("Public", dr["Public"].ToString().ToLower());
                                if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                                {

                                    DataRow rowItem1 = sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0];
                                    DataRow rowItem2 = sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[1];

                                    VLF.MAP.GeoPoint geopoint1 = new VLF.MAP.GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                                    VLF.MAP.GeoPoint geopoint2 = new VLF.MAP.GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                                    VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);

                                    geoZone.Add("lat", center.Latitude.ToString());
                                    geoZone.Add("lon", center.Longitude.ToString());
                                    String coords = string.Format("[[{0},{1}],[{2},{3}],[{4},{5}],[{6},{7}]]",
                                           rowItem1["Latitude"].ToString(),
                                           rowItem1["Longitude"].ToString(),

                                           rowItem1["Latitude"].ToString(),
                                           rowItem2["Longitude"].ToString(),


                                           rowItem2["Latitude"].ToString(),
                                           rowItem2["Longitude"].ToString(),

                                           rowItem2["Latitude"].ToString(),
                                           rowItem1["Longitude"].ToString()

                                        );

                                    geoZone.Add("coords", coords);
                                    //map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd()));
                                    geoZones.Add(geoZone);
                                    index = index + 1;
                                }
                                else //Polygon
                                {

                                    //GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count];
                                    //int i = 0;
                                    if (sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count <= 0) continue;
                                    geoZone.Add("lat", sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0]["Latitude"].ToString());
                                    geoZone.Add("lon", sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0]["Longitude"].ToString());

                                    StringBuilder coords = new StringBuilder();
                                    coords.Append("[");
                                    foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[tableName].Rows)
                                    {
                                        if (coords.Length == 1)
                                            coords.Append(
                                                string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                        else
                                            coords.Append(
                                                string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                    }
                                    coords.Append("]");
                                    geoZone.Add("coords", coords.ToString());
                                    geoZones.Add(geoZone);
                                    // TODO: put proper severity
                                    //map.Polygons.Add(new PoligonIcon(points, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd(), true));
                                    index = index + 1;

                                }
                            }
                            catch (Exception ex) { }
                        }

                    }
                    //if (geoZones.Count > 6) break;                        
                    // geoZones.RemoveAt(7);
                    //ArrayList al = new ArrayList();
                    //al.Add(geoZones[7]);
                    ///geoZones = al;
                    //geoZones.RemoveAt(7);
                    //geoZones.RemoveAt(6);
                    //geoZones.RemoveAt(6);
                    if (geoZones.Count > 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;
                        return js.Serialize(geoZones);

                    }
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    // Changes for TimeZone Feature end
    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public string GetAllGoezons()
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);


            if (sn.GeoZone.DsGeoZone != null && sn.GeoZone.DsGeoDetails != null)
            {
                try
                {
                    Resources.Const.Culture = new System.Globalization.CultureInfo(sn.SelectedLanguage);
                    DataSet myDataSet = sn.GeoZone.DsGeoZone;
                    LocalizeAddress(sn.SelectedLanguage, ref myDataSet);

                    string tableName = "";

                    ArrayList geoZones = new ArrayList();
                    int index = 0;
                    foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                    {
                        tableName = dr["GeoZoneId"].ToString();
                        sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                        if (sn.GeoZone.DsGeoDetails != null && sn.GeoZone.DsGeoDetails.Tables[tableName] != null && sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count > 0)
                        {
                            try
                            {
                                Dictionary<string, string> geoZone = new Dictionary<string, string>();
                                geoZone.Add("id", tableName);
                                geoZone.Add("geozoneName", dr["GeoZoneName"].ToString());
                                geoZone.Add("desc", dr["Description"].ToString());
                                geoZone.Add("type", "0");
                                geoZone.Add("Assigned", dr["Assigned"].ToString());
                                geoZone.Add("Direction", Enum.GetName(typeof(Enums.GeoZoneDirection), int.Parse(dr["Type"].ToString())).Replace("In", Resources.Const.GZ_Direction_In)
                                                                                                       .Replace("Out", Resources.Const.GZ_Direction_Out)
                                                                                                       .Replace("In/Out", Resources.Const.GZ_Direction_In_out)
                                                                                                       .Replace("Disable", Resources.Const.GZ_Direction_Disable));
                                geoZone.Add("SeverityName", dr["SeverityName"].ToString());
                                geoZone.Add("Public", dr["Public"].ToString().ToLower());
                                if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                                {

                                    DataRow rowItem1 = sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0];
                                    DataRow rowItem2 = sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[1];

                                    VLF.MAP.GeoPoint geopoint1 = new VLF.MAP.GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                                    VLF.MAP.GeoPoint geopoint2 = new VLF.MAP.GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                                    VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);

                                    geoZone.Add("lat", center.Latitude.ToString());
                                    geoZone.Add("lon", center.Longitude.ToString());
                                    String coords = string.Format("[[{0},{1}],[{2},{3}],[{4},{5}],[{6},{7}]]",
                                           rowItem1["Latitude"].ToString(),
                                           rowItem1["Longitude"].ToString(),

                                           rowItem1["Latitude"].ToString(),
                                           rowItem2["Longitude"].ToString(),


                                           rowItem2["Latitude"].ToString(),
                                           rowItem2["Longitude"].ToString(),

                                           rowItem2["Latitude"].ToString(),
                                           rowItem1["Longitude"].ToString()

                                        );

                                    geoZone.Add("coords", coords);
                                    //map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd()));
                                    geoZones.Add(geoZone);
                                    index = index + 1;
                                }
                                else //Polygon
                                {

                                    //GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count];
                                    //int i = 0;
                                    if (sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count <= 0) continue;
                                    geoZone.Add("lat", sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0]["Latitude"].ToString());
                                    geoZone.Add("lon", sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0]["Longitude"].ToString());

                                    StringBuilder coords = new StringBuilder();
                                    coords.Append("[");
                                    foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[tableName].Rows)
                                    {
                                        if (coords.Length == 1)
                                            coords.Append(
                                                string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                        else
                                            coords.Append(
                                                string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                                    }
                                    coords.Append("]");
                                    geoZone.Add("coords", coords.ToString());
                                    geoZones.Add(geoZone);
                                    // TODO: put proper severity
                                    //map.Polygons.Add(new PoligonIcon(points, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd(), true));
                                    index = index + 1;

                                }
                            }
                            catch (Exception ex) { }
                        }

                    }
                    //if (geoZones.Count > 6) break;                        
                    // geoZones.RemoveAt(7);
                    //ArrayList al = new ArrayList();
                    //al.Add(geoZones[7]);
                    ///geoZones = al;
                    //geoZones.RemoveAt(7);
                    //geoZones.RemoveAt(6);
                    //geoZones.RemoveAt(6);
                    if (geoZones.Count > 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;
                        return js.Serialize(geoZones);

                    }
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }            

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    // Changes for TimeZone Feature start

    private void DsGeoZone_Fill_NewTZ(SentinelFMSession sn)
    {
        try
        {
            sn.GeoZone.DsGeoDetails = new DataSet();
            sn.GeoZone.DsGeoZone_Fill_NewTZ(sn);

        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }

    // Changes for TimeZone Feature end

    private void DsGeoZone_Fill(SentinelFMSession sn)
    {
        try
        {
            sn.GeoZone.DsGeoDetails = new DataSet();
            sn.GeoZone.DsGeoZone_Fill(sn);
            
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }


    // Changes for TimeZone Feature start

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string AddNewGeozoneLandmark_NewTZ()
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName) || sn.UserID <= 0)
        {
            _r.status = 500;
            _r.message = "Your session is timeout, please login again.";
            return new JavaScriptSerializer().Serialize(_r);
        }

        string xml = "";
        SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
        if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
            {
                _r.status = 500;
                _r.message = "Failed.";
                return new JavaScriptSerializer().Serialize(_r);
            }


        if (xml == "")
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
            {
                if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                {
                    _r.status = 500;
                    _r.message = "Failed. You have \"View Only\" rights";
                    return new JavaScriptSerializer().Serialize(_r);
                }
            }
        }


        bool enableTimer = false;
        //if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
        if (sn.User.ControlEnable(sn, 102))
        {
            enableTimer = true;
        }

        string pointSets = HttpUtility.HtmlDecode(HttpContext.Current.Request["pointSets"]);
        string centerPoint = HttpUtility.HtmlDecode(HttpContext.Current.Request["centerPoint"]);

        bool isNew = Convert.ToInt16(HttpContext.Current.Request["isNew"] ?? "0") == 1;
        bool metaOnly = Convert.ToInt16(HttpContext.Current.Request["metaOnly"] ?? "0") == 1;

        if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 0) // Landmark
        {

            clsMap mp = new clsMap();
            //landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
            patchLandPointMgr = new VLF.PATCH.Logic.PatchLandmarkPointSet(sConnectionString);
            _patchServices = new VLF.PATCH.Logic.PatchServices(sConnectionString);

            string oldLandmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["oldLandmarkName"]);
            string landmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkName"]);
            string txtX = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtX"]);
            string txtY = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtY"]);
            string txtLandmarkDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkDesc"]);
            string txtContactName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtContactName"]);
            string txtPhone = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhone"]);
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]);
            string txtPhoneSMS = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhoneSMS"] ?? string.Empty);
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            string strAddress = mp.ResolveStreetAddressTelogis(txtY, txtX);
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            int radius = Convert.ToInt32(HttpContext.Current.Request["txtRadius"] ?? "-1");
            bool isPublic = (HttpContext.Current.Request["lstPublicPrivate"] ?? "1").Trim() == "1";

            string orginalAppliedServices = HttpContext.Current.Request["orginalAppliedServices"] ?? string.Empty;
            string updatedAppliedServices = HttpContext.Current.Request["updatedAppliedServices"] ?? string.Empty;
            string newAppliedServices = HttpContext.Current.Request["newAppliedServices"] ?? string.Empty;

            /*string speedOperator = HttpContext.Current.Request["polygonspeedOperator"] ?? "-1";

            _r.status = 500;
            _r.message = "Speed Operator:" + speedOperator;
            return new JavaScriptSerializer().Serialize(_r);*/

            long oldLandmarkCategoryId = Convert.ToInt64(HttpContext.Current.Request["txtOldCategory"] ?? "0");
            long selectedLandmarkCategoryId = Convert.ToInt64(HttpContext.Current.Request["ddlCategory"] ?? "0");

            if (landmarkName.Trim() == "")
            {
                _r.status = 500;
                _r.message = "Landmark name is required.";
                return new JavaScriptSerializer().Serialize(_r);
            }

            if (selectedLandmarkCategoryId == 0)
            {
                _r.status = 500;
                _r.message = "Category is required.";
                return new JavaScriptSerializer().Serialize(_r);
            }

            if (radius > 0)
                pointSets = string.Empty;

            try
            {
                if (isNew)
                {
                    _r.message = "Sucessfully created: " + landmarkName;
                    _r.isNew = 1;
                    int landmarkid = patchLandPointMgr.PatchVlfLandmarkPointSet_Add_NewTZ(sn.User.OrganizationId, landmarkName,
                                 Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                 txtLandmarkDesc,
                                 txtContactName,
                                 txtPhone,
                                 radius,
                                 txtEmail,
                                 txtPhoneSMS,
                                 Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                 pointSets, sn.UserID, isPublic, selectedLandmarkCategoryId
                                 );


                    SentinelFM.ServerDBUser.DBUser _dbu = new SentinelFM.ServerDBUser.DBUser();
                    int result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Add", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + landmarkName + " Added By: " + sn.LoginUserID);

                }
                else
                {
                    _r.message = "The changes are saved.";
                    _r.isNew = 0;
                    if (metaOnly)
                    {
                        long newCategoryId = 0;
                        if (oldLandmarkCategoryId != selectedLandmarkCategoryId)
                        {
                            newCategoryId = selectedLandmarkCategoryId;
                        }

                        patchLandPointMgr.PatchVlfLandmarkMetaData_Update_NewTZ(sn.User.OrganizationId, oldLandmarkName,
                                landmarkName,
                                Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                txtLandmarkDesc,
                                txtContactName,
                                txtPhone,
                                radius,
                                txtEmail,
                                txtPhoneSMS,
                                Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                sn.UserID, isPublic,
                                newCategoryId
                                );
                    }
                    else
                    {
                        patchLandPointMgr.PatchVlfLandmarkPointSet_Update_NewTZ(sn.User.OrganizationId, oldLandmarkName,
                                    landmarkName,
                                    Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                    txtLandmarkDesc,
                                    txtContactName,
                                    txtPhone,
                                    radius,
                                    txtEmail,
                                    txtPhoneSMS,
                                    Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                    pointSets, sn.UserID, isPublic
                                    );

                    }

                    int landmarkid = patchLandPointMgr.PatchGetLandmarkIdByLandmarkName(sn.User.OrganizationId, landmarkName);
                    _patchServices.DeleteAssignmentByLandmarkId(sn.User.OrganizationId, landmarkid);

                    if (updatedAppliedServices.Trim() != string.Empty)
                    {

                        string[] servicelist = updatedAppliedServices.Trim().Split(new string[] { "==++==" }, StringSplitOptions.None);
                        string toadd = string.Empty;
                        foreach (string s in servicelist)
                        {
                            string[] a1 = s.Split(new string[] { "|**|" }, StringSplitOptions.None);
                            if (a1.Length == 5)
                            {
                                int serviceid = int.Parse(a1[0]);
                                string name = HttpUtility.UrlDecode(a1[1]);
                                string rulesapplied = a1[2];
                                string recepients = a1[3];
                                string subjects = HttpUtility.UrlDecode(a1[4]);

                                if (serviceid <= 0)
                                {
                                    serviceid = _patchServices.InsertNewService(name, rulesapplied);
                                }

                                _patchServices.AssignServiceToLandmark(sn.User.OrganizationId, landmarkid, serviceid, recepients, subjects);

                            }
                        }
                    }

                    SentinelFM.ServerDBUser.DBUser _dbu = new SentinelFM.ServerDBUser.DBUser();
                    int result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + landmarkName + " Updated By: " + sn.LoginUserID);
                }

                _r.status = 200;
                _r.objectType = radius > 0 ? "LandmarkCircle" : "Landmark";
                _r.isPublic = isPublic.ToString().ToLower();
                sn.DsLandMarks = null;
            }
            catch (Exception e)
            {
                _r.status = 500;
                if (e.ToString().Contains("Cannot insert duplicate key row"))
                    _r.message = "Landmark with this name already exist";
                else
                    _r.message = "";
            }
            _r.geozonelandmarkname = landmarkName;

            if (enableTimer)
            {
                int serviceConfigId = Convert.ToInt32(HttpContext.Current.Request["cboServices"]);

                VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                int landmarkId = _lankmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, landmarkName);
                if (landmarkId > 0)
                {
                    VLF.PATCH.Logic.PatchServices _ps = new VLF.PATCH.Logic.PatchServices(sConnectionString);
                    _ps.DeleteHardcodedCallTimerServices(sn.User.OrganizationId, landmarkId);

                    if (serviceConfigId > 0)
                        _ps.AssignServiceToLandmark(sn.User.OrganizationId, landmarkId, serviceConfigId, "4160129305@e.pagenet.ca;mnancharla@bsmwireless.com;", "[VehicleDescription] Stop Exception: [ServiceName], occurred at [EVENT_TIME] Location: [LANDMARK_NAME]. [GOOGLE_LINK]");
                }
            }
        }
        else if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 1)    // Geozone
        {
            string txtGeoZoneName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneName"]);
            short cboDirection = Convert.ToInt16(HttpContext.Current.Request["cboDirection"] ?? "0");
            short geozoneType = 2;
            short cboGeoZoneSeverity = Convert.ToInt16(HttpContext.Current.Request["cboGeoZoneSeverity"] ?? "0");
            string txtGeoZoneDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneDesc"] ?? "");
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"] ?? "");
            string txtPhone = "";
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            bool chkWarning = HttpContext.Current.Request["chkWarning"] != null;
            bool chkCritical = HttpContext.Current.Request["chkCritical"] != null;
            bool chkNotify = HttpContext.Current.Request["chkNotify"] != null;

            bool isPublic = (HttpContext.Current.Request["lstGeozonePublicPrivate"] ?? "1").Trim() == "1";

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            if (txtGeoZoneName.Trim() == "")
            {
                _r.status = 500;
                _r.message = "Geozone name is required.";
                return new JavaScriptSerializer().Serialize(_r);
            }

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            _r.status = 200;
            _r.objectType = "Geozone";
            _r.isPublic = isPublic.ToString().ToLower();
            _r.geozonelandmarkname = txtGeoZoneName;

            if (isNew)
            {
                _r.message = "Sucessfully created: " + txtGeoZoneName;
                if (objUtil.ErrCheck(AddGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToSingle(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, 0, isPublic), false))
                    if (objUtil.ErrCheck(AddGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToSingle(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, 0, isPublic), true))
                    {
                        _r.status = 500;
                        _r.message = "Failed. Please check if Gezone name is already exist.";
                    }
            }
            else
            {
                _r.message = "The changes is saved.";
                short geozoneId = Convert.ToInt16(HttpContext.Current.Request["oid"]);
                if (objUtil.ErrCheck(UpdateGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToSingle(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, isPublic), false))
                    if (objUtil.ErrCheck(UpdateGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToSingle(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, isPublic), true))
                    {
                        _r.status = 500;
                        _r.message = "Failed. Please check if Gezone name is already exist.";
                    }
            }

            sn.GeoZone.DsGeoZone = null;
            sn.GeoZone.DsGeoDetails = new DataSet();
        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet=true)]
    public string AddNewGeozoneLandmark()
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        SentinelFM.ServerDBUser.DBUser _dbu = new SentinelFM.ServerDBUser.DBUser();
        clsUtility objUtil;
        objUtil = new clsUtility(sn);
        int result = 0;
        _results _r = new _results();
        int landmarkid = 0;

        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName) || sn.UserID <= 0)
        {
            _r.status = 500;
            _r.message = "Your session is timeout, please login again.";
            return new JavaScriptSerializer().Serialize(_r);
        }

        string xml = "";
        SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
        if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId,ref xml), false))
            if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
            {
                _r.status = 500;
                _r.message = "Failed.";
                return new JavaScriptSerializer().Serialize(_r);
            }


        if (xml == "")
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
            {
                if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                {
                    _r.status = 500;
                    _r.message = "Failed. You have \"View Only\" rights";
                    return new JavaScriptSerializer().Serialize(_r);
                }
            }
        }


        bool enableTimer = false;
        //if ((sn.User.UserGroupId == 1 || sn.User.UserGroupId == 2 || sn.User.UserGroupId == 7 || sn.UserID == 11967) && (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)) //Hgi and Security Administrator user 
        if (sn.User.ControlEnable(sn, 102))
        {
            enableTimer = true;
        }

        string pointSets = HttpUtility.HtmlDecode(HttpContext.Current.Request["pointSets"]);
        string centerPoint = HttpUtility.HtmlDecode(HttpContext.Current.Request["centerPoint"]);

        bool isNew = Convert.ToInt16(HttpContext.Current.Request["isNew"] ?? "0") == 1;
        bool metaOnly = Convert.ToInt16(HttpContext.Current.Request["metaOnly"] ?? "0") == 1;

        if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 0) // Landmark
        {

            clsMap mp = new clsMap();
            //landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
            patchLandPointMgr = new VLF.PATCH.Logic.PatchLandmarkPointSet(sConnectionString);
            _patchServices = new VLF.PATCH.Logic.PatchServices(sConnectionString);

            string oldLandmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["oldLandmarkName"]);
            string landmarkName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkName"]);
            string txtX = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtX"]);
            string txtY = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtY"]);
            string txtLandmarkDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtLandmarkDesc"]);
            string txtContactName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtContactName"]);
            string txtPhone = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhone"]);
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]);
            string txtPhoneSMS = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtPhoneSMS"]??string.Empty);
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            string strAddress = mp.ResolveStreetAddressTelogis(txtY, txtX);
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            int radius = Convert.ToInt32(HttpContext.Current.Request["txtRadius"] ?? "-1");
            bool isPublic = (HttpContext.Current.Request["lstPublicPrivate"] ?? "1").Trim() == "1";

            string orginalAppliedServices = HttpContext.Current.Request["orginalAppliedServices"] ?? string.Empty;
            string updatedAppliedServices = HttpContext.Current.Request["updatedAppliedServices"] ?? string.Empty;
            string newAppliedServices = HttpContext.Current.Request["newAppliedServices"] ?? string.Empty;

            /*string speedOperator = HttpContext.Current.Request["polygonspeedOperator"] ?? "-1";

            _r.status = 500;
            _r.message = "Speed Operator:" + speedOperator;
            return new JavaScriptSerializer().Serialize(_r);*/

            long oldLandmarkCategoryId = Convert.ToInt64(HttpContext.Current.Request["txtOldCategory"] ?? "0");
            long selectedLandmarkCategoryId = Convert.ToInt64(HttpContext.Current.Request["ddlCategory"] ?? "0");

            if (landmarkName.Trim() == "")
            {
                _r.status = 500;
                _r.message = "Landmark name is required.";
                return new JavaScriptSerializer().Serialize(_r);
            }

            if (selectedLandmarkCategoryId == 0)
            {
                _r.status = 500;
                _r.message = "Category is required.";
                return new JavaScriptSerializer().Serialize(_r);
            }

            if (radius > 0)
                pointSets = string.Empty;

            try
            {
                if (isNew)
                {
                    _r.message = "Sucessfully created: " + landmarkName;
                    _r.isNew = 1;
                    landmarkid = patchLandPointMgr.PatchVlfLandmarkPointSet_Add(sn.User.OrganizationId, landmarkName,
                                 Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                 txtLandmarkDesc,
                                 txtContactName,
                                 txtPhone,
                                 radius,
                                 txtEmail,
                                 txtPhoneSMS,
                                 Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                 pointSets, sn.UserID, isPublic, selectedLandmarkCategoryId
                                 );

                    result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Add", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + landmarkName + " Added By: " + sn.LoginUserID);

                }
                else
                {
                    _r.message = "The changes are saved.";
                    _r.isNew = 0;
                    if (metaOnly)
                    {
                        long newCategoryId = 0;
                        if (oldLandmarkCategoryId != selectedLandmarkCategoryId)
                        {
                            newCategoryId = selectedLandmarkCategoryId;
                        }

                        landmarkid = patchLandPointMgr.PatchGetLandmarkIdByLandmarkName(sn.User.OrganizationId, landmarkName);

                        result = _dbu.RecordInitialValues("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + txtLandmarkDesc + " Updated By: " + sn.LoginUserID);

                        patchLandPointMgr.PatchVlfLandmarkMetaData_Update(sn.User.OrganizationId, oldLandmarkName,
                                landmarkName,
                                Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                txtLandmarkDesc,
                                txtContactName,
                                txtPhone,
                                radius,
                                txtEmail,
                                txtPhoneSMS,
                                Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                sn.UserID, isPublic,
                                newCategoryId
                                );

                        result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + landmarkName + " Updated By: " + sn.LoginUserID);
                    }
                    else
                    {
                        landmarkid = patchLandPointMgr.PatchGetLandmarkIdByLandmarkName(sn.User.OrganizationId, landmarkName);

                        result = _dbu.RecordInitialValues("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + txtLandmarkDesc + " Updated By: " + sn.LoginUserID);

                        patchLandPointMgr.PatchVlfLandmarkPointSet_Update(sn.User.OrganizationId, oldLandmarkName,
                                    landmarkName,
                                    Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                    txtLandmarkDesc,
                                    txtContactName,
                                    txtPhone,
                                    radius,
                                    txtEmail,
                                    txtPhoneSMS,
                                    Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                    pointSets, sn.UserID, isPublic
                                    );


                        result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + landmarkName + " Updated By: " + sn.LoginUserID);

                    }


                    _patchServices.DeleteAssignmentByLandmarkId(sn.User.OrganizationId, landmarkid);

                    if (updatedAppliedServices.Trim() != string.Empty)
                    {

                        string[] servicelist = updatedAppliedServices.Trim().Split(new string[] { "==++==" }, StringSplitOptions.None);
                        string toadd = string.Empty;
                        foreach (string s in servicelist)
                        {
                            string[] a1 = s.Split(new string[] { "|**|" }, StringSplitOptions.None);
                            if (a1.Length == 5)
                            {
                                int serviceid = int.Parse(a1[0]);
                                string name = HttpUtility.UrlDecode(a1[1]);
                                string rulesapplied = a1[2];
                                string recepients = a1[3];
                                string subjects = HttpUtility.UrlDecode(a1[4]);

                                if (serviceid <= 0)
                                {
                                    serviceid = _patchServices.InsertNewService(name, rulesapplied);
                                }

                                _patchServices.AssignServiceToLandmark(sn.User.OrganizationId, landmarkid, serviceid, recepients, subjects);

                            }
                        }
                    }
                }

                _r.status = 200;
                _r.objectType = radius > 0 ? "LandmarkCircle" : "Landmark";
                _r.isPublic = isPublic.ToString().ToLower();
                sn.DsLandMarks = null;
            }
            catch (Exception e) { 
                _r.status = 500;
                if (e.ToString().Contains("Cannot insert duplicate key row"))
                    _r.message = "Landmark with this name already exist";
                else
                    _r.message = "";
            }
            _r.geozonelandmarkname = landmarkName;

            if (enableTimer)
            {
                int serviceConfigId = Convert.ToInt32(HttpContext.Current.Request["cboServices"]);

                VLF.PATCH.Logic.PatchLandmark _lankmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
                int landmarkId = _lankmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, landmarkName);
                if (landmarkId > 0)
                {
                    VLF.PATCH.Logic.PatchServices _ps = new VLF.PATCH.Logic.PatchServices(sConnectionString);
                    _ps.DeleteHardcodedCallTimerServices(sn.User.OrganizationId, landmarkId);

                    if (serviceConfigId > 0)
                        _ps.AssignServiceToLandmark(sn.User.OrganizationId, landmarkId, serviceConfigId, "4160129305@e.pagenet.ca;mnancharla@bsmwireless.com;", "[VehicleDescription] Stop Exception: [ServiceName], occurred at [EVENT_TIME] Location: [LANDMARK_NAME]. [GOOGLE_LINK]");
                }
            }
        }
        else if (Int16.Parse(HttpContext.Current.Request["lstAddOptions"]) == 1)    // Geozone
        {
            string txtGeoZoneName = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneName"]);
            short cboDirection = Convert.ToInt16(HttpContext.Current.Request["cboDirection"] ?? "0");
            short geozoneType = 2;
            short cboGeoZoneSeverity = Convert.ToInt16(HttpContext.Current.Request["cboGeoZoneSeverity"] ?? "0");
            string txtGeoZoneDesc = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtGeoZoneDesc"]??"");
            string txtEmail = HttpUtility.HtmlDecode(HttpContext.Current.Request["txtEmail"]??"");
            string txtPhone = "";
            string cboTimeZone = HttpContext.Current.Request["cboTimeZone"];
            bool chkDayLight = HttpContext.Current.Request["chkDayLight"] != null;
            bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
            bool chkWarning = HttpContext.Current.Request["chkWarning"] != null;
            bool chkCritical = HttpContext.Current.Request["chkCritical"] != null;
            bool chkNotify = HttpContext.Current.Request["chkNotify"] != null;

            bool isPublic = (HttpContext.Current.Request["lstGeozonePublicPrivate"] ?? "1").Trim() == "1";

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            if (txtGeoZoneName.Trim() == "")
            {
                _r.status = 500;
                _r.message = "Geozone name is required.";
                return new JavaScriptSerializer().Serialize(_r);
            }

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            _r.status = 200;
            _r.objectType = "Geozone";
            _r.isPublic = isPublic.ToString().ToLower();
            _r.geozonelandmarkname = txtGeoZoneName;

            if (isNew)
            {
                _r.message = "Sucessfully created: " + txtGeoZoneName;
                if (objUtil.ErrCheck(AddGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, 0, isPublic), false))
                    if (objUtil.ErrCheck(AddGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, 0, isPublic), true))
                    {
                        _r.status = 500;
                        _r.message = "Failed. Please check if Gezone name is already exist.";
                    }
            }
            else
            {
                _r.message = "The changes is saved.";
                short geozoneId = Convert.ToInt16(HttpContext.Current.Request["oid"]);
                if (objUtil.ErrCheck(UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, isPublic), false))
                    if (objUtil.ErrCheck(UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, geozoneId, txtGeoZoneName, cboDirection, geozoneType, pointsXml, cboGeoZoneSeverity, txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, isPublic), true))
                    {
                        _r.status = 500;
                        _r.message = "Failed. Please check if Gezone name is already exist.";
                    }
            }

            sn.GeoZone.DsGeoZone = null;
            sn.GeoZone.DsGeoDetails = new DataSet();
        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    // Changes for TimeZone Feature start
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EditGeozoneLandmark_NewTZ(string otype, string oid, string pointSets, string centerpoint, string newradius)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        int result = 0;
        int _landmarkid = 0;
        SentinelFM.ServerDBUser.DBUser _dbu = new SentinelFM.ServerDBUser.DBUser();
        VLF.PATCH.Logic.PatchLandmark _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        string xml = "";
        SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
        if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
            {
                _r.status = 500;
                _r.message = "Failed.";
                return new JavaScriptSerializer().Serialize(_r);
            }


        if (xml == "")
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
            {
                if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                {
                    _r.status = 500;
                    _r.message = "Failed. You have \"View Only\" rights";
                    return new JavaScriptSerializer().Serialize(_r);
                }
            }
        }

        pointSets = HttpUtility.HtmlDecode(pointSets).Replace("%7C", "|");
        oid = HttpUtility.UrlDecode(oid);
        oid = HttpUtility.HtmlDecode(oid);

        if (otype.ToLower() == "landmark") // Landmark
        {
            if (sn.DsLandMarks == null)
                DgLandmarks_Fill_NewTZ(sn);

            _r.status = 200;
            _r.message = pointSets;
            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
                {
                    string txtX = dr["Longitude"].ToString().TrimEnd();
                    string txtY = dr["Latitude"].ToString().TrimEnd();
                    string txtLandmarkDesc = dr["Description"].ToString().TrimEnd();
                    string txtContactName = dr["ContactPersonName"].ToString().TrimEnd();
                    string txtPhone = dr["ContactPhoneNum"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhoneSMS = dr["Phone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    string strAddress = dr["StreetAddress"].ToString().TrimEnd();
                    bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    int radius = -1;// Convert.ToInt32(dr["Radius"]);

                    if (centerpoint.Trim() != "" && centerpoint.Split(',').Length > 1)
                    {
                        txtX = centerpoint.Split(',')[1];
                        txtY = centerpoint.Split(',')[0];
                    }

                    try
                    {
                        _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, oid);

                        //check if landmark audit log already exists in the AuditLogs table
                        result = _dbu.RecordInitialValues("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + txtLandmarkDesc + " Updated By: " + sn.LoginUserID);

                        landPointMgr.vlfLandmarkPointSet_Update_NewTZ(sn.User.OrganizationId, oid,
                                        oid,
                                        Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                        txtLandmarkDesc,
                                        txtContactName,
                                        txtPhone,
                                        radius,
                                        txtEmail,
                                        txtPhoneSMS,
                                        Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                        pointSets, true, -10
                                     );

                        //update AuditLogs table with update
                        result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + txtLandmarkDesc + " Updated By: " + sn.LoginUserID);

                        _r.status = 200;
                        _r.objectType = "Landmark";
                        sn.DsLandMarks = null;
                    }
                    catch(Exception ex) { 
                        _r.status = 500;
                        _r.message = ex.Message;
                    }
                    break;
                }
            }
            _r.geozonelandmarkname = oid;
        }
        else if (otype.ToLower().Trim() == "landmarkcircle") // Landmark
        {
            if (sn.DsLandMarks == null)
                DgLandmarks_Fill_NewTZ(sn);
            pointSets = "";
            _r.status = 200;
            _r.message = pointSets;
            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
                {
                    string txtX = dr["Longitude"].ToString().TrimEnd();
                    string txtY = dr["Latitude"].ToString().TrimEnd();
                    string txtLandmarkDesc = dr["Description"].ToString().TrimEnd();
                    string txtContactName = dr["ContactPersonName"].ToString().TrimEnd();
                    string txtPhone = dr["ContactPhoneNum"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhoneSMS = dr["Phone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    string strAddress = dr["StreetAddress"].ToString().TrimEnd();
                    bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    int radius = Convert.ToInt32(dr["Radius"]);

                    if (centerpoint.Trim() != "")
                    {
                        txtX = centerpoint.Split(',')[1];
                        txtY = centerpoint.Split(',')[0];
                        int.TryParse(newradius, out radius);
                    }

                    try
                    {
                        _landmarkid = _landmark.GetLandmarkIdByLandmarkName(sn.User.OrganizationId, txtLandmarkDesc);

                        //check if landmark audit log already exists in the AuditLogs table
                        result = _dbu.RecordInitialValues("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + txtLandmarkDesc + " Updated By: " + sn.LoginUserID);

                        landPointMgr.vlfLandmarkPointSet_Update_NewTZ(sn.User.OrganizationId, oid,
                                        oid,
                                        Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                        txtLandmarkDesc,
                                        txtContactName,
                                        txtPhone,
                                        radius,
                                        txtEmail,
                                        txtPhoneSMS,
                                        Convert.ToSingle(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                        pointSets, true, -10
                                     );

                        //update AuditLogs table with update
                        result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + _landmarkid.ToString(), "Edit", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + txtLandmarkDesc + " Updated By: " + sn.LoginUserID);

                        _r.status = 200;
                        _r.objectType = "Landmark";
                        sn.DsLandMarks = null;
                    }
                    catch (Exception ex)
                    {
                        _r.status = 500;
                        _r.message = ex.Message;
                    }
                    break;
                }
            }
            _r.geozonelandmarkname = oid;
        }
        else if (otype.ToLower() == "geozone") // Geozone
        {
            _r.status = 200;

            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill_NewTZ(sn);

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["GeozoneName"].ToString().TrimEnd())
                {
                    string geozoneId = dr["GeozoneId"].ToString().TrimEnd();
                    string txtGeoZoneName = dr["GeozoneName"].ToString().TrimEnd();
                    string cboDirection = dr["Type"].ToString().TrimEnd();
                    string typeId = dr["GeozoneType"].ToString().TrimEnd();
                    string cboGeoZoneSeverity = dr["SeverityId"].ToString().TrimEnd();
                    string txtGeoZoneDesc = dr["Description"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhone = dr["Phone"].ToString().TrimEnd();
                    string cboTimeZone = dr["TimeZoneNew"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    bool DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                    bool chkNotify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                    bool chkWarning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                    bool chkCritical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());

                    if (objUtil.ErrCheck(UpdateGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToSingle(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, null), false))
                        if (objUtil.ErrCheck(UpdateGeozone_NewTZ(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToSingle(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, null), true))
                        {
                            _r.status = 500;
                        }
                    _r.geozonelandmarkname = txtGeoZoneName;
                    sn.GeoZone.DsGeoZone = null;
                    sn.GeoZone.DsGeoDetails = new DataSet();
                    break;
                }
            }

        }
        return new JavaScriptSerializer().Serialize(_r);
    }


    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string EditGeozoneLandmark(string otype, string oid, string pointSets)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        string xml = "";
        SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
        if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
            {
                _r.status = 500;
                _r.message = "Failed.";
                return new JavaScriptSerializer().Serialize(_r);
            }


        if (xml == "")
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
            {
                if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                {
                    _r.status = 500;
                    _r.message = "Failed. You have \"View Only\" rights";
                    return new JavaScriptSerializer().Serialize(_r);
                }
            }
        }

        pointSets = HttpUtility.HtmlDecode(pointSets).Replace("%7C", "|");
        oid = HttpUtility.UrlDecode(oid);
        oid = HttpUtility.HtmlDecode(oid);

        if (otype.ToLower() == "landmark") // Landmark
        {
            if (sn.DsLandMarks == null)
                DgLandmarks_Fill(sn);
            
            _r.status = 200;
            _r.message = pointSets;
            clsMap mp = new clsMap();
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
                {
                    string txtX = dr["Longitude"].ToString().TrimEnd();
                    string txtY = dr["Latitude"].ToString().TrimEnd();
                    string txtLandmarkDesc = dr["Description"].ToString().TrimEnd();
                    string txtContactName = dr["ContactPersonName"].ToString().TrimEnd();
                    string txtPhone = dr["ContactPhoneNum"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhoneSMS = dr["Phone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    string strAddress = dr["StreetAddress"].ToString().TrimEnd();
                    bool DayLightSaving = Convert.ToBoolean(objUtil.IsDayLightSaving(chkDayLight));
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    int radius = Convert.ToInt32(dr["Radius"]);

                    try
                    {
                        landPointMgr.vlfLandmarkPointSet_Update(sn.User.OrganizationId, oid,
                                        oid,
                                        Convert.ToDouble(txtY), Convert.ToDouble(txtX),
                                        txtLandmarkDesc,
                                        txtContactName,
                                        txtPhone,
                                        radius,
                                        txtEmail,
                                        txtPhoneSMS,
                                        Convert.ToInt16(cboTimeZone), DayLightSaving, chkDayLight, strAddress,
                                        pointSets, true, -10
                                     );

                        _r.status = 200;
                        _r.objectType = "Landmark";
                        sn.DsLandMarks = null;
                    }
                    catch { _r.status = 500; }
                    break;
                }
            }
            _r.geozonelandmarkname = oid;
        }
        else if (otype.ToLower() == "geozone") // Geozone
        {
            _r.status = 200;
            
            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);

            string[] points = pointSets.Split(',');
            string pointsXml = "<NewDataSet>";

            DBOrganization dbo = new DBOrganization();
            for (int i = 0; i < points.Length; i++)
            {
                pointsXml = pointsXml + "<Table1><Latitude>" + points[i].Split('|')[0] + "</Latitude><Longitude>" + points[i].Split('|')[1] + "</Longitude>";
                pointsXml = pointsXml + "<SequenceNum>" + (i + 1).ToString() + "</SequenceNum></Table1>";
            }
            pointsXml = pointsXml + "</NewDataSet>";

            foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
            {
                if (oid.TrimEnd() == dr["GeozoneName"].ToString().TrimEnd())
                {
                    string geozoneId = dr["GeozoneId"].ToString().TrimEnd();
                    string txtGeoZoneName = dr["GeozoneName"].ToString().TrimEnd();
                    string cboDirection = dr["Type"].ToString().TrimEnd();
                    string typeId = dr["GeozoneType"].ToString().TrimEnd();
                    string cboGeoZoneSeverity = dr["SeverityId"].ToString().TrimEnd();
                    string txtGeoZoneDesc = dr["Description"].ToString().TrimEnd();
                    string txtEmail = dr["Email"].ToString().TrimEnd();
                    string txtPhone = dr["Phone"].ToString().TrimEnd();
                    string cboTimeZone = dr["TimeZone"].ToString().TrimEnd();
                    bool chkDayLight = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                    bool DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                    bool chkNotify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                    bool chkWarning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                    bool chkCritical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());

                    if (objUtil.ErrCheck(UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, null), false))
                        if (objUtil.ErrCheck(UpdateGeozone(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(geozoneId), txtGeoZoneName, Convert.ToInt16(cboDirection), Convert.ToInt16(typeId), pointsXml, Convert.ToInt16(cboGeoZoneSeverity), txtGeoZoneDesc, txtEmail, txtPhone, Convert.ToInt32(cboTimeZone), DayLightSaving, 0, chkNotify, chkWarning, chkCritical, chkDayLight, null), true))
                        {
                            _r.status = 500;
                        }
                    _r.geozonelandmarkname = txtGeoZoneName;
                    sn.GeoZone.DsGeoZone = null;
                    sn.GeoZone.DsGeoDetails = new DataSet();
                    break;
                }
            }
        
        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string DeleteGeozoneLandmark(string otype, string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        string xml = "";
        SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
        if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbu.GetAssignedGroupsByUser(sn.UserID, sn.SecId, ref xml), true))
            {
                _r.status = 500;
                _r.message = "Failed.";
                return new JavaScriptSerializer().Serialize(_r);
            }


        if (xml == "")
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            if (System.Xml.Linq.XDocument.Parse(xml).Root.Elements("UserAllGroups").Count() == 1)
            {
                if (System.Xml.Linq.XDocument.Parse(xml).Root.Element("UserAllGroups").Element("UserGroupName").Value.ToString() == "View Only")
                {
                    _r.status = 500;
                    _r.message = "Failed. You have \"View Only\" rights";
                    return new JavaScriptSerializer().Serialize(_r);
                }
            }
        }

        oid = HttpUtility.HtmlDecode(oid);

        if (otype.ToLower() == "landmark") // Landmark
        {
            _r.status = 200;
            patchLandPointMgr = new VLF.PATCH.Logic.PatchLandmarkPointSet(sConnectionString);
            int landmarkid = patchLandPointMgr.PatchGetLandmarkIdByLandmarkName(sn.User.OrganizationId, oid);
            
            SentinelFM.ServerDBUser.DBUser _dbu = new SentinelFM.ServerDBUser.DBUser();
            try
            {

                GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");
                bool rsd = _lc.DeleteFromSpatialTable(landmarkid);

                if (rsd)
                {
                    landPointMgr.vlfLandmarkPointSet_Delete(sn.User.OrganizationId, oid);
                    sn.DsLandMarks = null;
                }
                else
                {
                    _r.status = 500;
                }                

                int result = _dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Delete", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + oid + "; Deleted By: " + sn.LoginUserID);                
                _r.message = "Sucessfully deleted the landmark.";
            }
            catch (Exception ex)
            {
                //_dbu.RecordUserAction("Landmark", sn.LoginUserID, sn.User.OrganizationId, "vlfLandmark", "LandmarkId=" + landmarkid.ToString(), "Delete", this.Context.Request.UserHostAddress, "MapNew/NewMapGeozoneLandmark.asmx", "Landmark " + oid + " Exception:" + ex.ToString() + "; Deleted By: " + sn.LoginUserID);
                _r.status = 500;
            }
        }
        else if (otype.ToLower() == "geozone") // Geozone
        {
            _r.status = 200;
            _r.message = "Sucessfully deleted the geozone.";
            DBOrganization dbo = new DBOrganization();
            if (objUtil.ErrCheck(dbo.DeleteGeozoneFromOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(oid)), false))
                if (objUtil.ErrCheck(dbo.DeleteGeozoneFromOrganization(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(oid)), true))
                {
                    _r.status = 500;
                    _r.message = "Failed to delete the geozone.";
                }
            
            sn.GeoZone.DsGeoZone = null;
            sn.GeoZone.DsGeoDetails = new DataSet();

        }
        return new JavaScriptSerializer().Serialize(_r);
    }

    // Changes for TimeZone Feature start
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string GetAllLandmarks_NewTZ()
    {
        string status0 = "{\"status\":0}";
        string status1 = "{\"status\":1}";
        string status_1 = "{\"status\":-1}";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return status_1;

        if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
            return status_1;

        try
        {
            _patchLandmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
            DataSet landmarkWithPoints = _patchLandmark.PatchGetLandmarksInfoWithPointsByOrganizationIdUserId_NewTZ(sn.User.OrganizationId, sn.UserID);

            if (landmarkWithPoints != null)
            {
                try
                {

                    string tableName = "";

                    ArrayList landmarks = new ArrayList();
                    foreach (DataRow dr in landmarkWithPoints.Tables[0].Rows)
                    {
                        tableName = dr["LandmarkName"].ToString();

                        Dictionary<string, string> landmark = new Dictionary<string, string>();
                        landmark.Add("id", tableName);
                        int radius = Int32.Parse(dr["Radius"].ToString());
                        landmark.Add("lat", dr["Latitude"].ToString());
                        landmark.Add("lon", dr["Longitude"].ToString());
                        landmark.Add("radius", radius.ToString());
                        landmark.Add("LandmarkName", tableName);
                        landmark.Add("LandmarkId", dr["lid"].ToString());
                        landmark.Add("Public", dr["Public"].ToString().ToLower());
                        landmark.Add("desc", dr["Description"].ToString());
                        landmark.Add("StreetAddress", dr["StreetAddress"].ToString());
                        landmark.Add("Email", dr["Email"].ToString());
                        landmark.Add("ContactPhoneNum", dr["ContactPhoneNum"].ToString());
                        landmark.Add("CategoryId", dr["CategoryId"].ToString());
                        landmark.Add("CategoryName", dr["CategoryName"].ToString());

                        if (radius > 0)
                        {
                            landmarks.Add(landmark);
                        }
                        else if (radius == -1 && dr["Points"] != null)
                        {
                            try
                            {
                                landmark.Add("coords", dr["Points"].ToString());
                                landmarks.Add(landmark);

                            }
                            catch { }
                        }

                    }
                    if (landmarks.Count > 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;
                        return js.Serialize(landmarks);

                    }
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }

            ////if (sn.DsLandMarks == null || sn.Landmark.DsLandmarkPointDetails == null)
            //    DgLandmarks_Fill(sn);

            //if (sn.DsLandMarks != null && sn.DsLandmarkPoints != null)
            //{
            //    try
            //    {

            //        string tableName = "";

            //        ArrayList landmarks = new ArrayList();
            //        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            //        {
            //            tableName = dr["LandmarkName"].ToString();

            //            Dictionary<string, string> landmark = new Dictionary<string, string>();
            //            landmark.Add("id", tableName);
            //            int radius = Int32.Parse(dr["Radius"].ToString());
            //            landmark.Add("lat", dr["Latitude"].ToString());
            //            landmark.Add("lon", dr["Longitude"].ToString());
            //            landmark.Add("radius", radius.ToString());
            //            landmark.Add("LandmarkName", tableName);
            //            landmark.Add("Public", dr["Public"].ToString().ToLower());
            //            landmark.Add("desc", dr["Description"].ToString());
            //            landmark.Add("StreetAddress", dr["StreetAddress"].ToString());
            //            landmark.Add("Email", dr["Email"].ToString());
            //            landmark.Add("ContactPhoneNum", dr["ContactPhoneNum"].ToString());

            //            if (radius > 0)
            //            {
            //                landmarks.Add(landmark);
            //            }
            //            else if (sn.DsLandmarkPoints != null && sn.DsLandmarkPoints.Tables[tableName] != null && sn.DsLandmarkPoints.Tables[tableName].Rows.Count > 2)
            //            {
            //                try
            //                {

            //                    if (radius == -1)   //Polygon
            //                    {
            //                        if (sn.DsLandmarkPoints.Tables[tableName].Rows.Count <= 2) continue;                                    

            //                        StringBuilder coords = new StringBuilder();
            //                        coords.Append("[");
            //                        foreach (DataRow rowItem in sn.DsLandmarkPoints.Tables[tableName].Rows)
            //                        {
            //                            if (coords.Length == 1)
            //                                coords.Append(
            //                                    string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
            //                            else
            //                                coords.Append(
            //                                    string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
            //                        }
            //                        coords.Append("]");
            //                        landmark.Add("coords", coords.ToString());                                                                           
            //                        landmarks.Add(landmark);
            //                    }
            //                }
            //                catch  { }
            //            }

            //        }
            //        if (landmarks.Count > 0)
            //        {
            //            JavaScriptSerializer js = new JavaScriptSerializer();
            //            js.MaxJsonLength = int.MaxValue;
            //            return js.Serialize(landmarks);

            //        }
            //    }
            //    catch (NullReferenceException Ex)
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            //    }

            //    catch (Exception Ex)
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            //        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

            //    }

            //}         

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return status0;
        }
        return status1;
    }


    // Changes for TimeZone Feature end
    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string GetAllLandmarks()
    {
        string status0 = "{\"status\":0}";
        string status1 = "{\"status\":1}";
        string status_1 = "{\"status\":-1}";
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) 
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
            return status_1;
        
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName))
            return status_1;

        try
        {
            _patchLandmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
            DataSet landmarkWithPoints = _patchLandmark.PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(sn.User.OrganizationId, sn.UserID);

            if (landmarkWithPoints != null )
            {
                try
                {

                    string tableName = "";

                    ArrayList landmarks = new ArrayList();
                    foreach (DataRow dr in landmarkWithPoints.Tables[0].Rows)
                    {
                        tableName = dr["LandmarkName"].ToString();

                        Dictionary<string, string> landmark = new Dictionary<string, string>();
                        landmark.Add("id", tableName);
                        int radius = Int32.Parse(dr["Radius"].ToString());
                        landmark.Add("lat", dr["Latitude"].ToString());
                        landmark.Add("lon", dr["Longitude"].ToString());
                        landmark.Add("radius", radius.ToString());
                        landmark.Add("LandmarkName", tableName);
                        landmark.Add("LandmarkId", dr["lid"].ToString());
                        landmark.Add("Public", dr["Public"].ToString().ToLower());
                        landmark.Add("desc", dr["Description"].ToString());
                        landmark.Add("StreetAddress", dr["StreetAddress"].ToString());
                        landmark.Add("Email", dr["Email"].ToString());
                        landmark.Add("ContactPhoneNum", dr["ContactPhoneNum"].ToString());
                        landmark.Add("CategoryId", dr["CategoryId"].ToString());
                        landmark.Add("CategoryName", dr["CategoryName"].ToString());

                        if (radius > 0)
                        {
                            landmarks.Add(landmark);
                        }
                        else if (radius == -1 && dr["Points"] != null)
                        {
                            try
                            {
                                landmark.Add("coords", dr["Points"].ToString());
                                landmarks.Add(landmark);
                                
                            }
                            catch { }
                        }

                    }
                    if (landmarks.Count > 0)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;
                        return js.Serialize(landmarks);

                    }
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

                }

            }   
            
            ////if (sn.DsLandMarks == null || sn.Landmark.DsLandmarkPointDetails == null)
            //    DgLandmarks_Fill(sn);

            //if (sn.DsLandMarks != null && sn.DsLandmarkPoints != null)
            //{
            //    try
            //    {

            //        string tableName = "";

            //        ArrayList landmarks = new ArrayList();
            //        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            //        {
            //            tableName = dr["LandmarkName"].ToString();

            //            Dictionary<string, string> landmark = new Dictionary<string, string>();
            //            landmark.Add("id", tableName);
            //            int radius = Int32.Parse(dr["Radius"].ToString());
            //            landmark.Add("lat", dr["Latitude"].ToString());
            //            landmark.Add("lon", dr["Longitude"].ToString());
            //            landmark.Add("radius", radius.ToString());
            //            landmark.Add("LandmarkName", tableName);
            //            landmark.Add("Public", dr["Public"].ToString().ToLower());
            //            landmark.Add("desc", dr["Description"].ToString());
            //            landmark.Add("StreetAddress", dr["StreetAddress"].ToString());
            //            landmark.Add("Email", dr["Email"].ToString());
            //            landmark.Add("ContactPhoneNum", dr["ContactPhoneNum"].ToString());

            //            if (radius > 0)
            //            {
            //                landmarks.Add(landmark);
            //            }
            //            else if (sn.DsLandmarkPoints != null && sn.DsLandmarkPoints.Tables[tableName] != null && sn.DsLandmarkPoints.Tables[tableName].Rows.Count > 2)
            //            {
            //                try
            //                {
                               
            //                    if (radius == -1)   //Polygon
            //                    {
            //                        if (sn.DsLandmarkPoints.Tables[tableName].Rows.Count <= 2) continue;                                    

            //                        StringBuilder coords = new StringBuilder();
            //                        coords.Append("[");
            //                        foreach (DataRow rowItem in sn.DsLandmarkPoints.Tables[tableName].Rows)
            //                        {
            //                            if (coords.Length == 1)
            //                                coords.Append(
            //                                    string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
            //                            else
            //                                coords.Append(
            //                                    string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
            //                        }
            //                        coords.Append("]");
            //                        landmark.Add("coords", coords.ToString());                                                                           
            //                        landmarks.Add(landmark);
            //                    }
            //                }
            //                catch  { }
            //            }

            //        }
            //        if (landmarks.Count > 0)
            //        {
            //            JavaScriptSerializer js = new JavaScriptSerializer();
            //            js.MaxJsonLength = int.MaxValue;
            //            return js.Serialize(landmarks);

            //        }
            //    }
            //    catch (NullReferenceException Ex)
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            //    }

            //    catch (Exception Ex)
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            //        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

            //    }

            //}         

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return status0;
        }
        return status1;
    }

    // Changes for TimeZone Feature start

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetGeozoneById_NewTZ(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
            DsGeoZone_Fill_NewTZ(sn);

        _geozone_NewTZ _g = new _geozone_NewTZ();

        foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["GeoZoneName"].ToString().TrimEnd())
            {
                _g.GeozoneNo = Convert.ToInt32(dr["GeozoneNo"].ToString().TrimEnd());
                _g.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _g.GeozoneId = Convert.ToInt32(dr["GeozoneId"].ToString().TrimEnd());
                _g.GeozoneName = dr["GeozoneName"].ToString().TrimEnd();
                _g.Direction = Convert.ToInt32(dr["Type"].ToString().TrimEnd());
                _g.GeozoneType = Convert.ToInt32(dr["GeozoneType"].ToString().TrimEnd());
                _g.SeverityId = Convert.ToInt32(dr["SeverityId"].ToString().TrimEnd());
                _g.Description = dr["Description"].ToString().TrimEnd();
                _g.Email = dr["Email"].ToString().TrimEnd();
                _g.Phone = dr["Phone"].ToString().TrimEnd();
                _g.TimeZone = Convert.ToSingle(dr["TimeZone"].ToString().TrimEnd());
                _g.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _g.Notify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                _g.Warning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                _g.Critical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());
                _g.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());

                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_g);
    }


    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetGeozoneById(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        
        if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill(sn);

        _geozone _g = new _geozone();
        
        foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["GeoZoneName"].ToString().TrimEnd())
            {
                _g.GeozoneNo = Convert.ToInt32(dr["GeozoneNo"].ToString().TrimEnd());
                _g.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _g.GeozoneId = Convert.ToInt32(dr["GeozoneId"].ToString().TrimEnd());
                _g.GeozoneName = dr["GeozoneName"].ToString().TrimEnd();
                _g.Direction = Convert.ToInt32(dr["Type"].ToString().TrimEnd());
                _g.GeozoneType = Convert.ToInt32(dr["GeozoneType"].ToString().TrimEnd());
                _g.SeverityId = Convert.ToInt32(dr["SeverityId"].ToString().TrimEnd());
                _g.Description = dr["Description"].ToString().TrimEnd();
                _g.Email = dr["Email"].ToString().TrimEnd();
                _g.Phone = dr["Phone"].ToString().TrimEnd();
                _g.TimeZone = Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd());
                _g.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _g.Notify = Convert.ToBoolean(dr["Notify"].ToString().TrimEnd());
                _g.Warning = Convert.ToBoolean(dr["Warning"].ToString().TrimEnd());
                _g.Critical = Convert.ToBoolean(dr["Critical"].ToString().TrimEnd());
                _g.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());

                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_g);
    }

    // Changes for TimeZone Feature start


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetLandmarkById_NewTZ(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        if (sn.DsLandMarks == null)
            DgLandmarks_Fill_NewTZ(sn);

        _landmark_NewTZ _l = new _landmark_NewTZ();

        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
            {
                _l.LandmarkId = Convert.ToInt32(dr["LandmarkId"].ToString().TrimEnd());
                _l.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _l.LandmarkName = dr["LandmarkName"].ToString().TrimEnd();
                _l.Latitude = Convert.ToDouble(dr["Latitude"].ToString().TrimEnd());
                _l.Longitude = Convert.ToDouble(dr["Longitude"].ToString().TrimEnd());
                _l.Description = dr["Description"].ToString().TrimEnd();
                _l.ContactPersonName = dr["ContactPersonName"].ToString().TrimEnd();
                _l.ContactPhoneNum = dr["ContactPhoneNum"].ToString().TrimEnd();
                _l.Radius = Convert.ToInt32(dr["Radius"].ToString().TrimEnd());
                _l.Email = dr["Email"].ToString().TrimEnd();
                _l.Phone = dr["Phone"].ToString().TrimEnd();
                _l.TimeZone = Convert.ToSingle(dr["TimeZone"].ToString().TrimEnd());
                _l.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _l.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                _l.StreetAddress = dr["StreetAddress"].ToString().TrimEnd();

                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_l);
    }

    // Changes for TimeZone Feature end

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetLandmarkById(string oid)
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        if (sn.DsLandMarks == null)
            DgLandmarks_Fill(sn);
        
        _landmark _l = new _landmark();            
            
        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
        {
            if (oid.TrimEnd() == dr["LandmarkName"].ToString().TrimEnd())
            {
                _l.LandmarkId = Convert.ToInt32(dr["LandmarkId"].ToString().TrimEnd());
                _l.OrganizationId = Convert.ToInt32(dr["OrganizationId"].ToString().TrimEnd());
                _l.LandmarkName = dr["LandmarkName"].ToString().TrimEnd();
                _l.Latitude = Convert.ToDouble(dr["Latitude"].ToString().TrimEnd());
                _l.Longitude = Convert.ToDouble(dr["Longitude"].ToString().TrimEnd());
                _l.Description = dr["Description"].ToString().TrimEnd();
                _l.ContactPersonName = dr["ContactPersonName"].ToString().TrimEnd();
                _l.ContactPhoneNum = dr["ContactPhoneNum"].ToString().TrimEnd();
                _l.Radius = Convert.ToInt32(dr["Radius"].ToString().TrimEnd());
                _l.Email = dr["Email"].ToString().TrimEnd();
                _l.Phone = dr["Phone"].ToString().TrimEnd();
                _l.TimeZone = Convert.ToInt32(dr["TimeZone"].ToString().TrimEnd());
                _l.DayLightSaving = Convert.ToBoolean(dr["DayLightSaving"].ToString().TrimEnd());
                _l.AutoAdjustDayLightSaving = Convert.ToBoolean(dr["AutoAdjustDayLightSaving"].ToString().TrimEnd());
                _l.StreetAddress = dr["StreetAddress"].ToString().TrimEnd();                
                
                break;
            }
        }
        return new JavaScriptSerializer().Serialize(_l);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string GetLandmarkGeometry(string landmarkId)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        ArrayList landmarks = new ArrayList();

        string[] arr = landmarkId.Split(',');

        SentinelFM.GeomarkServiceRef.GeomarkServiceClient t = new GeomarkServiceClient("httpbasic");
        foreach (string s in arr)
        {
            int lid = 0;
            int.TryParse(s.Trim(), out lid);

            if (lid > 0)
            {
                string landmarkpoints = t.GetLandmarkGeometry(sn.User.OrganizationId, lid);
                landmarks.Add(landmarkpoints);
            }
        }

        //string status_1 = "{\"status\":-1, organizationId:" + organizationId.ToString() + ", landmarkId:" + landmarkId.ToString() + ", landmarkPoints:\"" + landmarkpoints + "\"}";

        //return status_1;
        //return landmarkpoints;
        if (landmarks.Count > 0)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            return js.Serialize(landmarks);

        }
        
        return "-1";
        
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string GetLandmarkGeometryWithVehicles(string landmarkId, string serviceConfigId)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        ArrayList landmarks = new ArrayList();        

        string[] arr = landmarkId.Split(',');

        SentinelFM.GeomarkServiceRef.GeomarkServiceClient t = new GeomarkServiceClient("httpbasic");

        int sid = int.Parse(serviceConfigId);
        
        foreach (string s in arr)
        {
            int lid = 0;
            int.TryParse(s.Trim(), out lid);
            _routesWithVehicles rv = new _routesWithVehicles();

            if (lid > 0)
            {
                string landmarkpoints = t.GetLandmarkGeometry(sn.User.OrganizationId, lid);
                rv.RoutePoints = landmarkpoints;
                Dictionary<string, IList<int>> vehicles = VLF.DAS.Logic.ServiceAssignment.GetSelectedFleetsAndVehicles(sid, "Vehicle");
                List<string> vehicleIds = new List<string>();
                foreach (var vehicle in vehicles)
                {
                    foreach (int v in vehicle.Value)
                    {
                        vehicleIds.Add(v.ToString());
                    }
                }

                ArrayList vehiclesLastKnowInfo = new ArrayList();

                if (vehicleIds.Count > 0)
                {
                    DataRow[] drArr = sn.Map.DsFleetInfoNew.Tables[0].Select("VehicleId IN (" + String.Join(",", vehicleIds.ToArray()) + ")");
                    
                    foreach (DataRow rowItem in drArr)
                    {
                        _vehicleLastPosition vp = new _vehicleLastPosition();
                        vp.VehicleId = Convert.ToInt64(rowItem["VehicleId"]);
                        vp.Description = rowItem["Description"].ToString();
                        vp.lon = Convert.ToDouble(rowItem["Longitude"].ToString());
                        vp.lat = Convert.ToDouble(rowItem["Latitude"].ToString());
                        vehiclesLastKnowInfo.Add(vp);
                    }
                    
                }
                rv.Vehicles = vehiclesLastKnowInfo;
                //landmarks.Add(landmarkpoints);
                landmarks.Add(rv);
            }
        }

        //string status_1 = "{\"status\":-1, organizationId:" + organizationId.ToString() + ", landmarkId:" + landmarkId.ToString() + ", landmarkPoints:\"" + landmarkpoints + "\"}";

        //return status_1;
        //return landmarkpoints;
        if (landmarks.Count > 0)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            return js.Serialize(landmarks);

        }
        VLF.DAS.Logic.ServiceAssignment sanmnt = new VLF.DAS.Logic.ServiceAssignment();

        return "-1";

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetGeoInfo(double lon, double lat)
    {
        GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");
        Dictionary<string,string> d = _lc.ReverseGeoCoder(lon, lat);

        _geoInfo _g = new _geoInfo();
        _g.StreetAddress = d["StreetAddress"];
        _g.IsPostedSpeed = bool.Parse(d["IsPostedSpeed"]);
        _g.RoadSpeed = d["RoadSpeed"] + " " + d["SpeedTypeZone"].Replace("KilometersPerHour", "km/h").Replace("MilesPerHour", "mph") + (_g.IsPostedSpeed ? "(Posted)":"");
        _g.SpeedCategory = d["SpeedCategory"];
        _g.FunctionClass = d["FunctionClass"];
        
        return new JavaScriptSerializer().Serialize(_g);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string SaveVehicleOperationalState()
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName) || sn.UserID <= 0)
        {
            _r.status = 500;
            _r.message = "Your session is timeout, please login again.";
            return new JavaScriptSerializer().Serialize(_r);
        }
        
        int operationState = 0;
        int.TryParse(HttpContext.Current.Request["OperationalState"], out operationState);
        
        long vehicleId = 0;
        long.TryParse(HttpContext.Current.Request["vehicleId"], out vehicleId);

        long boxId = 0;
        long.TryParse(HttpContext.Current.Request["boxId"], out boxId);

        int duration = 0;
        int.TryParse(HttpContext.Current.Request["LandmarkDuration"], out duration);

        int landmarkId = 0;
        int.TryParse(HttpContext.Current.Request["landmarkId"], out landmarkId);
        
        string operationStateNotes = HttpContext.Current.Request["OperationalStateNotes"];
        
        int OperationalStateServiceConfigId = 0;
        int.TryParse(HttpContext.Current.Request["OperationalStateServiceConfigId"], out OperationalStateServiceConfigId);

        string myConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        VLF.DAS.Logic.Vehicle _vehicle = new VLF.DAS.Logic.Vehicle(myConnnectionString);
        int result = _vehicle.SaveVehicleOperationalState(sn.UserID, sn.User.OrganizationId, vehicleId, operationState, operationStateNotes);
        
        if(result > 0)
        {
            _r.status = 200;
            _r.message = "Succeed.";

            if (OperationalStateServiceConfigId > 0)
            {
                try
                {
                    if (operationState == 200)
                    {
                        duration = 0;
                    }
                    GeomarkServiceClient clientGeomarkService = new GeomarkServiceClient("httpbasic");
                    int updatePostponeResult = clientGeomarkService.UpdatePostpone((int)boxId, landmarkId, OperationalStateServiceConfigId, duration);
                }
                catch (Exception ex)
                {
                    _r.status = 500;
                    _r.message = "Failed to update the postpone duration.";
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.ToString() + " User:" + sn.UserID.ToString() + " Form:NewMapGeozoneLandmark.asmx"));
                }
            }

            
            string filter = string.Format("VehicleId = '{0}'", vehicleId);
            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
            for (int i = 0; i < foundRows.Length; i++)
            {
                try
                {
                    string stroperationalState = "";
                    if (operationState == 100)
                        stroperationalState = "Available";
                    else if (operationState == 200)
                        stroperationalState = "Unavailable";

                    int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalState"] = operationState.ToString();
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateName"] = stroperationalState;
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateNotes"] = operationStateNotes;
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }
            }
            
            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }        
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public string BulkUpdateVehicleOperationalState()
    {
        SentinelFMSession sn = null;
        sn = (SentinelFMSession)Session["SentinelFMSession"];

        clsUtility objUtil;
        objUtil = new clsUtility(sn);

        _results _r = new _results();

        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName) || sn.UserID <= 0)
        {
            _r.status = 500;
            _r.message = "Your session is timeout, please login again.";
            return new JavaScriptSerializer().Serialize(_r);
        }

        int operationState = 0;
        int.TryParse(HttpContext.Current.Request["OperationalState"], out operationState);

        string SelectedVehicles = HttpContext.Current.Request["SelectedVehicles"].ToString();
        string chkUpdateNotes = HttpContext.Current.Request["chkUpdateNotes"].ToString();
        string operationNotes = HttpContext.Current.Request["operationNotes"].ToString();

        if (chkUpdateNotes != "on")
            operationNotes = "";

        string myConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        VLF.DAS.Logic.Vehicle _vehicle = new VLF.DAS.Logic.Vehicle(myConnnectionString);
        int totalUpdated = 0;
        string[] vehicles = SelectedVehicles.Split(',');
        foreach (string _v in vehicles)
        {
            totalUpdated += _vehicle.SaveVehicleOperationalState(sn.UserID, sn.User.OrganizationId, int.Parse(_v), operationState, operationNotes);
        }
        
        if (totalUpdated > 0)
        {
            _r.status = 200;
            _r.message = "Succeed.";

            string filter = string.Format("VehicleId IN ({0})", SelectedVehicles);
            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
            for (int i = 0; i < foundRows.Length; i++)
            {
                try
                {
                    string stroperationalState = "";
                    if (operationState == 100)
                        stroperationalState = "Available";
                    else if (operationState == 200)
                        stroperationalState = "Unavailable";

                    int index = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows.IndexOf(foundRows[i]);
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalState"] = operationState.ToString();
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateName"] = stroperationalState;
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Rows[index]["OperationalStateNotes"] = operationNotes;
                    sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].AcceptChanges();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                }
            }

            return new JavaScriptSerializer().Serialize(_r);
        }
        else
        {
            _r.status = 500;
            _r.message = "Failed.";
            return new JavaScriptSerializer().Serialize(_r);
        }
    }

    private class _results
    {
        public int status;
        public string geozonelandmarkname;
        public string objectType;
        public string message;
        public int isNew = 0;
        public string isPublic = "true";
    }

    // Changes for TimeZone Feature start

    private class _landmark_NewTZ
    {
        public int LandmarkId;
        public int OrganizationId;
        public string LandmarkName;
        public double Latitude;
        public double Longitude;
        public string Description;
        public string ContactPersonName;
        public string ContactPhoneNum;
        public int Radius;
        public string Email;
        public string Phone;
        public float? TimeZone;
        public bool DayLightSaving;
        public bool AutoAdjustDayLightSaving;
        public string StreetAddress;
    }

    // Changes for TimeZone Feature end

    private class _landmark
    {
        public int LandmarkId;
        public int OrganizationId;
        public string LandmarkName;
        public double Latitude;
        public double Longitude;
        public string Description;
        public string ContactPersonName;
        public string ContactPhoneNum;
        public int Radius;
        public string Email;
        public string Phone;
        public int? TimeZone;
        public bool DayLightSaving;
        public bool AutoAdjustDayLightSaving;
        public string StreetAddress;
    }


    // Changes for TimeZone Feature start

    private class _geozone_NewTZ
    {
        public int GeozoneNo;
        public int OrganizationId;
        public int GeozoneId;
        public string GeozoneName;
        public int Direction;
        public int GeozoneType;
        public int SeverityId;
        public string Description;
        public string Email;
        public string Phone;
        public float? TimeZone;
        public bool DayLightSaving;
        public bool Notify;
        public bool Warning;
        public bool Critical;
        public bool AutoAdjustDayLightSaving;
    }

    // Changes for TimeZone Feature end
    private class _geozone
    {
        public int GeozoneNo;
        public int OrganizationId;
        public int GeozoneId;
        public string GeozoneName;
        public int Direction;
        public int GeozoneType;
        public int SeverityId;
        public string Description;
        public string Email;
        public string Phone;
        public int? TimeZone;
        public bool DayLightSaving;
        public bool Notify;
        public bool Warning;
        public bool Critical;
        public bool AutoAdjustDayLightSaving; 
    }

    private class _vehicleLastPosition
    {
        public long VehicleId;
        public string Description;
        public double lon;
        public double lat;
    }

    private class _routesWithVehicles
    {
        public string RoutePoints;
        //public _vehicleLastPosition[] Vehicles;
        public ArrayList Vehicles;
    }

    private class _geoInfo
    {
        public string StreetAddress;
        public bool IsPostedSpeed;
        public string RoadSpeed;
        public string SpeedCategory;
        public string FunctionClass;
    }

    // Changes for TimeZone Feature start
    private void DgLandmarks_Fill_NewTZ(SentinelFMSession sn)
    {
        try
        {
            sn.Landmark.DgLandmarks_Fill_NewTZ(sn);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }


    // Changes for TimeZone Feature end
    
    private void DgLandmarks_Fill(SentinelFMSession sn)
    {
        try
        {
            sn.Landmark.DgLandmarks_Fill(sn);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:AddNewMapGoezone"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);

        }
    }

    // Changes for TimeZone Feature start

    private int AddGeozone_NewTZ(int userId, string SID, int organizationId,
                             string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                             short severityId, string description, string email, string phone, float timeZone,
                             bool dayLightSaving, short formatType, bool notify,
                             bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool isPublic)
    {
        try
        {
            if (String.IsNullOrEmpty(xmlGeozoneSet))
                return (int)InterfaceError.InvalidParameter;

            DataSet dsGeozoneSet = new DataSet();
            dsGeozoneSet.ReadXml(new System.IO.StringReader(xmlGeozoneSet.TrimEnd()));

            if (!Util.IsDataSetValid(dsGeozoneSet))
                return (int)InterfaceError.InvalidParameter;

            string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            using (PatchOrganizationGeozone dbOrganization = new PatchOrganizationGeozone(sConnectionString))
            {
                dbOrganization.PatchAddGeozone_NewTZ(organizationId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, isPublic, userId);
            }

            return (int)InterfaceError.NoError;
        }
        catch (Exception ex)
        {
            return (int)InterfaceError.ServerError;
        }
    }

    // Changes for TimeZone Feature end

    private int AddGeozone(int userId, string SID, int organizationId,
                             string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                             short severityId, string description, string email, string phone, int timeZone,
                             bool dayLightSaving, short formatType, bool notify,
                             bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool isPublic)
    {
        try
        {
            if (String.IsNullOrEmpty(xmlGeozoneSet))
                return (int)InterfaceError.InvalidParameter;

            DataSet dsGeozoneSet = new DataSet();
            dsGeozoneSet.ReadXml(new System.IO.StringReader(xmlGeozoneSet.TrimEnd()));

            if (!Util.IsDataSetValid(dsGeozoneSet))
                return (int)InterfaceError.InvalidParameter;

            string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            using (PatchOrganizationGeozone dbOrganization = new PatchOrganizationGeozone(sConnectionString))
            {
                dbOrganization.PatchAddGeozone(organizationId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, isPublic, userId);
            }

            return (int)InterfaceError.NoError;
        }
        catch (Exception ex)
        {
            return (int)InterfaceError.ServerError;
        }
    }
    // Changes for TimeZone Feature start
    private int UpdateGeozone_NewTZ(int userId, string SID, int organizationId, short geozoneId,
                               string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                               short severityId, string description, string email, string phone, float timeZone,
                               bool dayLightSaving, short formatType, bool notify, bool warning,
                               bool critical, bool autoAdjustDayLightSaving, bool? isPublic)
    {
        try
        {
            DataSet dsGeozoneSet = new DataSet();
            if (xmlGeozoneSet != null && xmlGeozoneSet.TrimEnd() != "")
            {
                System.IO.StringReader strrXML = new System.IO.StringReader(xmlGeozoneSet.TrimEnd());
                dsGeozoneSet.ReadXml(strrXML);
            }

            string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            using (PatchOrganizationGeozone dbOrganization = new PatchOrganizationGeozone(sConnectionString))
            {
                dbOrganization.PatchUpdateGeozone_NewTZ(organizationId, geozoneId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, isPublic, userId);
            }

            return (int)InterfaceError.NoError;
        }
        catch (Exception ex)
        {
            return (int)InterfaceError.ServerError;
        }
    }


    // Changes for TimeZone Feature end

    private int UpdateGeozone(int userId, string SID, int organizationId, short geozoneId,
                                string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                                short severityId, string description, string email, string phone, int timeZone,
                                bool dayLightSaving, short formatType, bool notify, bool warning,
                                bool critical, bool autoAdjustDayLightSaving, bool? isPublic)
    {
        try
        {
            DataSet dsGeozoneSet = new DataSet();
            if (xmlGeozoneSet != null && xmlGeozoneSet.TrimEnd() != "")
            {
                System.IO.StringReader strrXML = new System.IO.StringReader(xmlGeozoneSet.TrimEnd());
                dsGeozoneSet.ReadXml(strrXML);
            }

            string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            using (PatchOrganizationGeozone dbOrganization = new PatchOrganizationGeozone(sConnectionString))
            {
                dbOrganization.PatchUpdateGeozone(organizationId, geozoneId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, isPublic, userId);
            }

            return (int)InterfaceError.NoError;
        }
        catch (Exception ex)
        {
            return (int)InterfaceError.ServerError;
        }
    }

    private static void LocalizeAddress(string lang, ref DataSet iDataSet)
    {
        Resources.Const.Culture = new System.Globalization.CultureInfo(lang);

        foreach (DataTable dt in iDataSet.Tables)
        {
            if (dt.Columns.Contains("SeverityName"))
                foreach (DataRow dr in dt.Rows)
                {
                    if (lang.Contains("fr"))
                    {
                        dr["SeverityName"] = dr["SeverityName"].ToString().Replace("NoAlarm", Resources.Const.GZ_Severity_NoAlarm)
                                                                          .Replace("Critical", Resources.Const.GZ_Severity_Critical)
                                                                          .Replace("Notify", Resources.Const.GZ_Severity_Notify)
                                                                          .Replace("Warning", Resources.Const.GZ_Severity_Warning);
                    }
                    else if (lang.Contains("en"))
                    {
                        dr["SeverityName"] = dr["SeverityName"].ToString().Replace("Aucune alarme", Resources.Const.GZ_Severity_NoAlarm)
                                                                      .Replace("Critique", Resources.Const.GZ_Severity_Critical)
                                                                      .Replace("Notification", Resources.Const.GZ_Severity_Notify)
                                                                      .Replace("Avertissement", Resources.Const.GZ_Severity_Warning);
                    }
                }
        }
    }    
}
