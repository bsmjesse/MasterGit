using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Globalization;

namespace SentinelFM
{

    public partial class MapNew_frmDrawLandmarkGeozone : System.Web.UI.Page
    {
        public string JAVASCRIPT;
        public string DRAW_OBJECT = "[]";
        public string OBJECT_TYPE = string.Empty;

        public string tooltip_Pan_Map = string.Empty;
        public string tooltip_Add_a_landmark_Circle = string.Empty;
        public string tooltip_Draw_a_Polygon = string.Empty;
        public string tooltip_Modify_feature = string.Empty;

        public string MapSearchData = string.Empty;

        protected SentinelFMSession sn = null;
        VLF.DAS.Logic.LandmarkPointSetManager landPointMgr;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);
            
            string FormName = Request.QueryString["FormName"];
            if (!string.IsNullOrEmpty(FormName))
            {
                if (FormName == "Landmark")
                {
                    OBJECT_TYPE = "Landmark";

                    Dictionary<string, string> lankMark = new Dictionary<string, string>();
                    lankMark.Add("type", "Landmark");
                    if (sn.Landmark.X != 0 && sn.Landmark.Y != 0)
                    {                        
                        lankMark.Add("id", "1");
                        lankMark.Add("lat", sn.Landmark.Y.ToString());
                        lankMark.Add("lon", sn.Landmark.X.ToString());
                        lankMark.Add("desc", sn.Landmark.LandmarkName == null ? "newlandmark" : sn.Landmark.LandmarkName.ToString().Trim());
                        lankMark.Add("radius", sn.Landmark.Radius.ToString());

                        string coords = "[]";
                        if (sn.Landmark.Radius.ToString() == "-1")
                        {
                            if (sn.Landmark.DsLandmarkDetails == null)
                            {
                                DataSet ds =
                                    landPointMgr.GetLandmarkPointSetByLandmarkName(sn.Landmark.LandmarkName, sn.User.OrganizationId);
                                if (ds.Tables[0].Rows.Count > 0)
                                {

                                    DataRow[] drs = new DataRow[ds.Tables[0].Rows.Count];
                                    ds.Tables[0].Rows.CopyTo(drs, 0);
                                    coords = CreatePointsetString(drs);
                                }
                            }
                            else
                            {
                                if (sn.Landmark.DsLandmarkDetails.Rows.Count > 0)
                                {

                                    DataRow[] drs = new DataRow[sn.Landmark.DsLandmarkDetails.Rows.Count];
                                    sn.Landmark.DsLandmarkDetails.Rows.CopyTo(drs, 0);
                                    coords = CreatePointsetString(drs);
                                }
                            }
                        }
                        lankMark.Add("coords", coords);

                        //string icon = "Landmark.ico";
                        //lankMark.Add("icon", icon);
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;

                        DRAW_OBJECT = js.Serialize(lankMark);
                    }

                    if (DRAW_OBJECT == "[]")
                    {
                        lankMark.Add("desc", "newlandmark");

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;

                        DRAW_OBJECT = js.Serialize(lankMark);
                    }
                }

                if (FormName == "Geozone")
                {
                    //isDrawGeozoneMap = "1";
                    //DrawGeozoneData = DrawOneGeoZone();
                    OBJECT_TYPE = "Geozone";
                    DrawOneGeoZone();
                }

                if (sn.Map.MapSearch) //for search
                {
                    MapSearchData = sn.History.MapCenterLatitude + "," + sn.History.MapCenterLongitude;
                    sn.Map.MapSearch = false;                    
                }

                
                //return;
            }

            tooltip_Pan_Map = (string)base.GetLocalResourceObject("tooltip_Pan_Map");
            tooltip_Add_a_landmark_Circle = (string)base.GetLocalResourceObject("tooltip_Add_a_landmark_Circle");
            tooltip_Draw_a_Polygon = (string)base.GetLocalResourceObject("tooltip_Draw_a_Polygon");
            tooltip_Modify_feature = (string)base.GetLocalResourceObject("tooltip_Modify_feature");

        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                string FormName = Request.QueryString["FormName"];
                if (!string.IsNullOrEmpty(FormName))
                {
                    if (FormName == "Landmark")
                    {
                        if (sn.Landmark.DsLandmarkDetails == null)
                        {
                            sn.Landmark.DsLandmarkDetails = new DataTable();

                            sn.Landmark.DsLandmarkDetails.Columns.Add("Latitude", typeof(System.Double));
                            sn.Landmark.DsLandmarkDetails.Columns.Add("Longitude", typeof(System.Double));
                        }
                        sn.Landmark.DsLandmarkDetails.Rows.Clear();
                        if (pointSets.Value.Trim() != string.Empty)
                        {
                            string[] points = pointSets.Value.Trim().Split(',');
                            if (isCircle.Value == "1")
                            {
                                //string[] xyValues = hidPoints.Value.Trim().Split('|');
                                sn.Landmark.Y = double.Parse(latitude.Value);
                                sn.Landmark.X = double.Parse(longitude.Value);
                                sn.Landmark.Radius = (int)(double.Parse(radius.Value));
                                sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.None;
                            }
                            else
                            {
                                sn.Landmark.Radius = -1;

                                double minX = 999999;
                                double maxX = -999999;
                                double minY = 999999;
                                double maxY = -999999;

                                foreach (string myPoints in points)
                                {
                                    if (myPoints.Trim() != string.Empty)
                                    {
                                        string[] latLon = myPoints.Split('|');
                                        DataRow dr1 = sn.Landmark.DsLandmarkDetails.NewRow();
                                        dr1["Latitude"] = double.Parse(latLon[0]);
                                        dr1["Longitude"] = double.Parse(latLon[1]);
                                        if (minY > (double)dr1["Latitude"]) minY = (double)dr1["Latitude"];
                                        if (maxY < (double)dr1["Latitude"]) maxY = (double)dr1["Latitude"];

                                        if (minX > (double)dr1["Longitude"]) minX = (double)dr1["Longitude"];
                                        if (maxX < (double)dr1["Longitude"]) maxX = (double)dr1["Longitude"];

                                        sn.Landmark.DsLandmarkDetails.Rows.Add(dr1);
                                    }
                                }

                                sn.Landmark.LandmarkType = VLF.CLS.Def.Enums.GeozoneType.Polygon;

                                sn.Landmark.Y = (minY + maxY) / 2;
                                sn.Landmark.X = (minX + maxX) / 2;
                            }

                            JAVASCRIPT = "self.close(); window.opener.ReFreshWindow();";
                        }
                    }

                    else if (FormName == "Geozone")
                    {
                        sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();
                        if (pointSets.Value.Trim() != string.Empty)
                        {
                            string[] points = pointSets.Value.Trim().Split(',');
                            foreach (string myPoints in points)
                            {
                                if (myPoints.Trim() != string.Empty)
                                {
                                    string[] latLon = myPoints.Split('|');
                                    DataRow dr1 = sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                                    dr1["Latitude"] = double.Parse(latLon[0]);
                                    dr1["Longitude"] = double.Parse(latLon[1]);
                                    dr1["SequenceNum"] = sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                                    sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr1);
                                }
                            }
                            
                            //sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(this.GeoZoneOptions.SelectedItem.Value);
                            if (isCircle.Value == "1")
                                sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.None;
                            else
                                sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Polygon;

                            JAVASCRIPT = " window.opener.document.forms[0].txtMapMessage.value='" + (string)base.GetLocalResourceObject("txtMapMessage_Value_OnWindowOpen") + "';";
                            JAVASCRIPT += " self.close();";
                            //RadAjaxManager1.ResponseScripts.Add(strJavaScript);
                        }
                    }
                }
                
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //RadAjaxManager1.ResponseScripts.Add("alert('" + errorSave + "')");
            }
        }

        private string CreatePointsetString(DataRow[] dtPoints)
        {
            StringBuilder coords = new StringBuilder();
            coords.Append("POLYGON((");
            //int i = 0;
            foreach (DataRow rowItem in dtPoints)
            {
                coords.Append(string.Format("{0} {1},", rowItem["Longitude"].ToString(), rowItem["Latitude"].ToString()));
            }
            coords.Remove(coords.Length - 1, 1);
            coords.Append("))");
            return coords.ToString();
        }

        private string DrawOneGeoZone()
        {
            try
            {
                Dictionary<string, string> geoZone = new Dictionary<string, string>();
                geoZone.Add("type", "Geozone");                    

                if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 0)
                {   

                    //Dictionary<string, string> geoZone = new Dictionary<string, string>();
                    geoZone.Add("id", "1");
                    //geoZone.Add("desc", string.Empty);
                    geoZone.Add("desc", (sn.GeoZone.Name == null || sn.GeoZone.Name.Trim() == string.Empty) ? "newgeozone" : sn.GeoZone.Name.Trim());
                    
                    if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    {

                        DataRow rowItem1 = sn.GeoZone.DsGeoDetails.Tables[0].Rows[0];
                        DataRow rowItem2 = sn.GeoZone.DsGeoDetails.Tables[0].Rows[1];

                        VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                        VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                        VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);

                        geoZone.Add("lat", center.Latitude.ToString());
                        geoZone.Add("lon", center.Longitude.ToString());

                        String coords = string.Format("POLYGON(({0} {1},{2} {3},{4} {5},{6} {7}))",
                            rowItem1["Longitude"].ToString(),   
                            rowItem1["Latitude"].ToString(),

                            rowItem2["Longitude"].ToString(),
                            rowItem1["Latitude"].ToString(),


                            rowItem2["Longitude"].ToString(),
                            rowItem2["Latitude"].ToString(),
                               
                           rowItem1["Longitude"].ToString(),
                           rowItem2["Latitude"].ToString()
                        );

                        geoZone.Add("coords", coords);
                        //geoZones.Add(geoZone);

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = int.MaxValue;

                        DRAW_OBJECT = js.Serialize(geoZone);

                    }
                    else //Polygon
                    {

                        if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 1)
                        {
                            GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count];
                            if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count <= 0) return string.Empty;
                            geoZone.Add("lat", sn.GeoZone.DsGeoDetails.Tables[0].Rows[0]["Latitude"].ToString());
                            geoZone.Add("lon", sn.GeoZone.DsGeoDetails.Tables[0].Rows[0]["Longitude"].ToString());

                            DataRow[] drs = new DataRow[sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count];
                            sn.GeoZone.DsGeoDetails.Tables[0].Rows.CopyTo(drs, 0);
                            string coords = CreatePointsetString(drs);
                            
                            geoZone.Add("coords", coords.ToString());
                            //geoZones.Add(geoZone);

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            js.MaxJsonLength = int.MaxValue;

                            DRAW_OBJECT = js.Serialize(geoZone);


                        }
                    }
                    
                }

                if (DRAW_OBJECT == "[]")
                {
                    geoZone.Add("desc", "newgeozone");

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;

                    DRAW_OBJECT = js.Serialize(geoZone);
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
            return string.Empty;
        }

        protected override void InitializeCulture()
        {

            SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
            if (snMain == null || snMain.User == null || String.IsNullOrEmpty(snMain.UserName))
            {
                return;

            }

            if (snMain.SelectedLanguage != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new
                    CultureInfo(snMain.SelectedLanguage);

                System.Threading.Thread.CurrentThread.CurrentCulture = new
                 CultureInfo("en-US");

                base.InitializeCulture();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new
                   CultureInfo("en-US");

                base.InitializeCulture();
            }
        }
    }
}