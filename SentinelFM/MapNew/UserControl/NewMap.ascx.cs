using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using VLF.MAP;
using System.Configuration;

namespace SentinelFM
{
public partial class Map_UserControl_Map : System.Web.UI.UserControl
{
    protected SentinelFMSession sn = null;
    public string DrawGeozoneData = string.Empty;
    public string OneLandmark = string.Empty;
    public string OneGeoZone = string.Empty;
    public string MapData = string.Empty;
    public string LandMarkData = string.Empty;
    public string GeoData = string.Empty;
    public string StartupScript = string.Empty;
    public string isBigMap = "0";
    public string isLandmark_GeoZone = "0";
    public string isFixSize = "0";
    public string isDrawMap = "0";
    public string isDrawGeozoneMap = "0";
    public string MapSearchData = string.Empty;
    public string MapHeight = "0";
    public string MapWidth = "0";
    public string isShowLandMark = "0";
    public string isShowGeoZone = "0";
    public string hasLandmarks = "0";
    public string hasGeozone = "0";

    public string IsShowLandmarkname = "0";
    public string IsShowVehicleName = "0";
    public string IsShowLandmarkRadius = "0";
    public string IsFrench = "0";
    public string failedSetValue = "Fail to save value.";
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    VLF.DAS.Logic.LandmarkPointSetManager landPointMgr ;
    protected void Page_Load(object sender, EventArgs e)
    {
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScript", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            return;
        }
        if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca") IsFrench = "1";
        if (!IsPostBack)
        {
            if (sn.Landmark.DsLandmarkPointDetails == null)
            {
                sn.Landmark.DsLandmarkPointDetails = landPointMgr.GetLandmarkPointSetByOrganizationId(sn.User.OrganizationId).Tables[0];
            }
            if (sn.Map.MapSearch) //for search
            {
                MapSearchData = GenerateSearchMapData("1", sn.History.MapCenterLatitude, sn.History.MapCenterLongitude, string.Empty);
                //MapSearchData = GenerateSearchMapData("1", "33.8986", "-118.013", string.Empty);
                //map.MapCenterLongitude = Convert.ToDouble(sn.History.MapCenterLongitude);
                //map.MapCenterLatitude = Convert.ToDouble(sn.History.MapCenterLatitude);
                //map.ImageDistance = Convert.ToDouble(sn.History.ImageDistance);
                //map.MapScale = Convert.ToDouble(sn.History.MapScale);
                //map.DrawAllVehicles = false;
                sn.Map.MapSearch = false;

                if (Request.QueryString["Landmark_GeoZone"] == null && Request.QueryString["FormName"] == null )
                    return;
            }
            string landmarkId = Request.QueryString["LandmarkId"];
            if (!string.IsNullOrEmpty(landmarkId))
            {
                OneLandmark = DrawOneLandMark(landmarkId);
                //isFixSize = "1";
                //MapHeight = "500";
                return;
            }

            string geoZoneId = Request.QueryString["GeoZoneId"];
            if (!string.IsNullOrEmpty(geoZoneId))
            {
                try
                {
                    OneGeoZone = DrawOneGeoZone(Int16.Parse(geoZoneId));
                    //isFixSize = "1";
                    //MapHeight = "550";
                }
                catch (Exception ex) { }
                return;
            }

            string FormName = Request.QueryString["FormName"];
            if (!string.IsNullOrEmpty(FormName))
            {
                if (FormName == "Landmark")
                {
                    isDrawMap = "1";

                    if (sn.Landmark.X != 0 && sn.Landmark.Y != 0)
                    {
                        Dictionary<string, string> lankMark = new Dictionary<string, string>();
                        lankMark.Add("id", "1");
                        lankMark.Add("lat", sn.Landmark.Y.ToString());
                        lankMark.Add("lon", sn.Landmark.X.ToString());
                        lankMark.Add("desc", sn.Landmark.LandmarkName == null ? string.Empty : sn.Landmark.LandmarkName.ToString().Trim());
                        lankMark.Add("rad", sn.Landmark.Radius.ToString());

                        string coords = "[]";
                        if (sn.Landmark.Radius.ToString() == "-1" )
                        {
                            DataSet ds =
                                landPointMgr.GetLandmarkPointSetByLandmarkName(sn.Landmark.LandmarkName, sn.User.OrganizationId);
                            coords = CreatePointsetString(ds.Tables[0]);
                        }
                        lankMark.Add("coords", coords);

                        string icon = "Landmark.ico";
                        lankMark.Add("icon", icon);
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;

                        OneLandmark = js.Serialize(lankMark);
                    }
                }

                if (FormName == "Geozone")
                {
                    isDrawGeozoneMap = "1";
                    DrawGeozoneData = DrawOneGeoZone();
                }

                //isFixSize = "1";
                return;
            }


            LandMarkData = DrawLandMark();
            if (LandMarkData != "") hasLandmarks = "1";
            GeoData = DrawGeoZone();
            if (GeoData != "") hasGeozone = "1";

            if (Request.QueryString["Landmark_GeoZone"] != null)
            {
                isLandmark_GeoZone = "1";
                //isFixSize = "1";
                //MapHeight = "520";
                //MapWidth = "850";
                if (sn.Map.ShowLandmark) isShowLandMark = "1";
                if (sn.Map.ShowGeoZone) isShowGeoZone = "1";

            }
            GetMapOption();
        }
        if (Request.QueryString["isBig"] != null)
        { 
            isBigMap = "1";
        }

        string noPrintStyle = "<style media='print' type='text/css'>.NoPrintStyle {display:none; visibility:hidden} </style>";
         
        Literal lit = new Literal();
        lit.Text = noPrintStyle;
        this.Page.Header.Controls.Add(lit);
    }

    private string DrawOneGeoZone()
    {
        try
        {
            if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 0)
            {
                ArrayList geoZones = new ArrayList();

                Dictionary<string, string> geoZone = new Dictionary<string, string>();
                geoZone.Add("id", "1");
                geoZone.Add("desc", string.Empty);
                geoZone.Add("type", "0");


                if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                {

                    DataRow rowItem1 = sn.GeoZone.DsGeoDetails.Tables[0].Rows[0];
                    DataRow rowItem2 = sn.GeoZone.DsGeoDetails.Tables[0].Rows[1];

                    VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                    VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
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
                    geoZones.Add(geoZone);

                }
                else //Polygon
                {

                    //First Point Icon
                    if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count == 1)
                    {
                        DataRow rowItem = sn.GeoZone.DsGeoDetails.Tables[0].Rows[0];
                        geoZone.Add("lat", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                        geoZone.Add("lon", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                        string icon = "StartGeoZone.ico";
                        geoZone.Add("icon", icon);
                        geoZone.Add("coords", string.Empty);
                        geoZones.Add(geoZone);
                    }
                    else
                    {
                        GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count];
                        if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count <= 0) return string.Empty;
                        geoZone.Add("lat", sn.GeoZone.DsGeoDetails.Tables[0].Rows[0]["Latitude"].ToString());
                        geoZone.Add("lon", sn.GeoZone.DsGeoDetails.Tables[0].Rows[0]["Longitude"].ToString());

                        StringBuilder coords = new StringBuilder();
                        coords.Append("[");
                        int i = 0;
                        foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[0].Rows)
                        {
                            if (coords.Length == 1)
                            {
                                coords.Append(
                                  string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                            }
                            else
                            {
                                coords.Append(
                                string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                            }
                            i++;
                        }
                        // TODO: put proper severity
                        coords.Append("]");
                        // TODO: put proper severity
                        geoZone.Add("coords", coords.ToString());
                        geoZones.Add(geoZone);


                    }
                }
                if (geoZones.Count > 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    return js.Serialize(geoZones);

                }
            }

        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }
        return string.Empty;
    }

    private string GenerateSearchMapData(string id, string lat, string lon, string desc)
    {

        Dictionary<string, string> vehicleDic = new Dictionary<string, string>();

        vehicleDic.Add("id", id);
        vehicleDic.Add("lat", lat);
        vehicleDic.Add("lon", lon);
        vehicleDic.Add("desc", desc);

        JavaScriptSerializer js = new JavaScriptSerializer();
        return js.Serialize(vehicleDic);
    }


    private string DrawOneGeoZone(Int16 geoZoneId)
    {
        try
        {

            clsUtility objUtil = new clsUtility(sn);

            StringReader strrXML = null;
            DataSet dsGeoZoneDetails = new DataSet();

            string xml = "";
            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

            if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, geoZoneId, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, geoZoneId, ref xml), true))
                {
                    return string.Empty ;
                }

            if (xml == "")
                return string.Empty ;




            strrXML = new StringReader(xml);
            dsGeoZoneDetails.ReadXml(strrXML);


            Int16 SeverityId = 0;
            Int16 Type = 0;
            Int16 GeozoneType = 0;
            string GeoZoneName = "";
            ArrayList geoZones = new ArrayList();
            foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
            {
                if (Convert.ToInt16(dr["GeoZoneId"].ToString().TrimEnd()) == geoZoneId)
                {
                    SeverityId = Convert.ToInt16(dr["SeverityId"]);
                    Type = Convert.ToInt16(dr["Type"]);
                    GeozoneType = Convert.ToInt16(dr["GeozoneType"]);
                    GeoZoneName = dr["GeoZoneName"].ToString().TrimEnd();
                    break;
                }
            }


            sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)GeozoneType;
            if (dsGeoZoneDetails.Tables[0].Rows.Count > 0)
            {
                Dictionary<string, string> geoZone = new Dictionary<string, string>();
                geoZone.Add("id", "1");
                geoZone.Add("desc", GeoZoneName);
                geoZone.Add("type", "0");

                if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                {

                    DataRow rowItem1 = dsGeoZoneDetails.Tables[0].Rows[0];
                    DataRow rowItem2 = dsGeoZoneDetails.Tables[0].Rows[1];

                    VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                    VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
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
                    geoZones.Add(geoZone);

                }
                else //Polygon
                {

                    if (dsGeoZoneDetails.Tables[0].Rows.Count <= 0) return string.Empty;
                    geoZone.Add("lat", dsGeoZoneDetails.Tables[0].Rows[0]["Latitude"].ToString());
                    geoZone.Add("lon", dsGeoZoneDetails.Tables[0].Rows[0]["Longitude"].ToString());

                    StringBuilder coords = new StringBuilder();
                    coords.Append("[");
                    int i = 0;
                    foreach (DataRow rowItem in dsGeoZoneDetails.Tables[0].Rows)
                    {
                        if (coords.Length == 1)
                        {
                            coords.Append(
                              string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                        }
                        else
                        {
                            coords.Append(
                            string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
                        }
                        i++;
                    }
                    coords.Append("]");
                    // TODO: put proper severity
                    geoZone.Add("coords", coords.ToString());
                    geoZones.Add(geoZone);

                }

                if (geoZones.Count > 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    return js.Serialize(geoZones);

                }


            }
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }

        return string.Empty;
    }

    private string DrawOneLandMark(string landmarkId)
    {
        if ((sn.DsLandMarks != null) &&
            (sn.DsLandMarks.Tables.Count > 0) &&
            (sn.DsLandMarks.Tables[0].Rows.Count > 0))
        {
            int id = 1;
            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (dr["LandmarkId"].ToString().TrimEnd() == landmarkId)
                {
                    Dictionary<string, string> lankMark = new Dictionary<string, string>();
                    lankMark.Add("id", id.ToString());
                    lankMark.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                    lankMark.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                    lankMark.Add("desc", dr["LandmarkName"] is DBNull ? string.Empty : dr["LandmarkName"].ToString().Trim());
                    lankMark.Add("rad", dr["Radius"] is DBNull ? string.Empty : dr["Radius"].ToString());
                    string coords = "[]";
                    if (dr["Radius"] != DBNull.Value && dr["Radius"].ToString() == "-1" && dr["LandmarkName"] != DBNull.Value)
                    {
                            DataSet ds = 
                                landPointMgr.GetLandmarkPointSetByLandmarkName(dr["LandmarkName"].ToString(), sn.User.OrganizationId);
                            coords = CreatePointsetString(ds.Tables[0]);
                    }
                    lankMark.Add("coords", coords);
                    string icon = "Landmark.ico";
                    lankMark.Add("icon", icon);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    return js.Serialize(lankMark);
                }
            }
        }
        return string.Empty;
    }

    private string CreatePointsetString(DataTable dtPoints)
    {
        StringBuilder coords = new StringBuilder();
        coords.Append("[");
        int i = 0;
        foreach (DataRow rowItem in dtPoints.Rows)
        {
            if (coords.Length == 1)
            {
                coords.Append(
                  string.Format("[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
            }
            else
            {
                coords.Append(
                string.Format(",[{0},{1}]", rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString()));
            }
            i++;
        }
        coords.Append("]");
        return coords.ToString();
    }

    private string DrawLandMark()
    {
        ArrayList lankMarks = new ArrayList();
        if ((sn.DsLandMarks != null) &&
            (sn.DsLandMarks.Tables.Count > 0) &&
            (sn.DsLandMarks.Tables[0].Rows.Count > 0))
        {
            int id = 1;
            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                Dictionary<string, string> lankMark = new Dictionary<string, string>();
                try
                {
                    lankMark.Add("id", id.ToString());
                    lankMark.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                    lankMark.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                    string icon = "../Bigicons/Landmark.ico";
                    lankMark.Add("icon", icon);
                    lankMark.Add("rad", dr["Radius"] is DBNull ? string.Empty : dr["Radius"].ToString());
                    if (sn.Map.ShowLandmarkname && sn.Map.ShowLandmarkRadius)
                    {
                        //lankMark.Add("desc", dr["Description"] is DBNull ? string.Empty : dr["Description"].ToString().Trim());
                        //lankMark.Add("rad", dr["Radius"] is DBNull ? string.Empty : dr["Radius"].ToString());
                        lankMark.Add("show", "1");
                    }
                    //map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                    else if (sn.Map.ShowLandmarkname)
                    {
                        //lankMark.Add("desc", dr["Description"] is DBNull ? string.Empty : dr["Description"].ToString().Trim());
                        //lankMark.Add("rad", "0");
                        lankMark.Add("show", "1");
                        //map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                    }
                    else if (sn.Map.ShowLandmarkRadius)
                    {
                        //lankMark.Add("desc", dr["Description"] is DBNull ? string.Empty : dr["Description"].ToString().Trim());
                        lankMark.Add("show", "0");
                        //lankMark.Add("rad", dr["Radius"] is DBNull ? string.Empty : dr["Radius"].ToString());

                }
                //    map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", ""));
                else if (sn.Map.ShowLandmark)
                {
                    lankMark.Add("show", "0");
                    //lankMark.Add("rad", "0");
                    //lankMark.Add("desc", dr["Description"] is DBNull ? string.Empty : dr["Description"].ToString().Trim());

                }
                else
                {
                    lankMark.Add("show", "0");
                    //lankMark.Add("rad", "0");
                    //lankMark.Add("desc", "");
                }
                lankMark.Add("desc", dr["LandmarkName"] is DBNull ? string.Empty : dr["LandmarkName"].ToString().Trim());
                //    map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", ""));
                string coords = "[]";
                if (dr["Radius"] != DBNull.Value && dr["Radius"].ToString() == "-1" && dr["LandmarkName"] != DBNull.Value)
                { 
                    string pointsLandmarkName = dr["LandmarkName"].ToString().Replace("'", "''");
                        if (sn.Landmark.DsLandmarkPointDetails.Rows.Count > 0)
                        {
                    DataTable dtPoints = sn.Landmark.DsLandmarkPointDetails.Select("LandmarkName='" + pointsLandmarkName + "'").CopyToDataTable();
                    if (dtPoints != null)
                    {
                        coords = CreatePointsetString(dtPoints);

                            }
                        }
                    }
                    lankMark.Add("coords", coords);

                lankMarks.Add(lankMark);
                id++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.StackTrace.ToString()));
                }
            }
        }

        if (lankMarks.Count > 0)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            return js.Serialize(lankMarks);
        }
        else return "";

    }

    public void GetService ()
    {
        ((SentinelMapBasePage)this.Page).GetService();

    }

    public void GetToken()
    {
        ((SentinelMapBasePage)this.Page).GetToken();
    }

    private string DrawGeoZone()
    {
        if (sn.Map.ShowGeoZone)
        {
            if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                DsGeoZone_Fill();

            if (sn.GeoZone.DsGeoZone != null && sn.GeoZone.DsGeoDetails != null)
            {
                try
                {

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
                                geoZone.Add("desc", dr["GeoZoneName"].ToString());
                                geoZone.Add("type", "0");
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
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ((SentinelMapBasePage)(this.Page)).ExceptionLogger(trace);

                }

            }
        }
        return string.Empty;
    }

    private void DsGeoZone_Fill()
    {
        try
        {
            sn.GeoZone.DsGeoZone_Fill(sn);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            SentinelMapBasePage basePage = (SentinelMapBasePage)(this.Page);
            basePage.ExceptionLogger(trace);

        }
    }

    private void GetMapOption()
    {
        isShowLandMark = sn.Map.ShowLandmark ? "1" : "0";
        IsShowLandmarkname = sn.Map.ShowLandmarkname ? "1" : "0";
        IsShowVehicleName = sn.Map.ShowVehicleName ? "1" : "0";
        isShowGeoZone = sn.Map.ShowGeoZone ? "1" : "0";
        IsShowLandmarkRadius = sn.Map.ShowLandmarkRadius  ? "1" : "0";
    }
}
}