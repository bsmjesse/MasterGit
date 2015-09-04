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

    public partial class MapNew_frmViewLandmarkGeozone : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        VLF.DAS.Logic.LandmarkPointSetManager landPointMgr;
        public string DRAW_OBJECT = string.Empty;
        public string CAPTION = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            landPointMgr = new VLF.DAS.Logic.LandmarkPointSetManager(sConnectionString);

            if (!IsPostBack)
            {
                string landmarkId = Request.QueryString["LandmarkId"];
                if (!string.IsNullOrEmpty(landmarkId))
                {
                    DRAW_OBJECT = DrawOneLandMark(landmarkId);
                    return;
                }

                string geoZoneId = Request.QueryString["GeoZoneId"];
                if (!string.IsNullOrEmpty(geoZoneId))
                {
                    try
                    {
                        DRAW_OBJECT = DrawOneGeoZone(Int16.Parse(geoZoneId));                        
                    }
                    catch (Exception ex) { }
                    return;
                }
            }
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
                        

                        string radius = dr["Radius"] is DBNull ? string.Empty : dr["Radius"].ToString();

                        CAPTION = "<b>" + (string)base.GetLocalResourceObject("txtLandmark") + ":</b> " + (dr["LandmarkName"] is DBNull ? string.Empty : dr["LandmarkName"].ToString().Trim());
                        try
                        {
                            if (radius != string.Empty && int.Parse(radius) > 0)
                                CAPTION += " <b>" + (string)base.GetLocalResourceObject("txtRadius") + ":</b> " + radius;
                        }
                        catch { }
                        
                        Dictionary<string, string> lankMark = new Dictionary<string, string>();
                        lankMark.Add("id", id.ToString());
                        lankMark.Add("type", "Landmark");
                        lankMark.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                        lankMark.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                        lankMark.Add("desc", dr["LandmarkName"] is DBNull ? string.Empty : dr["LandmarkName"].ToString().Trim());
                        lankMark.Add("radius", radius);
                        string coords = "[]";
                        if (dr["Radius"] != DBNull.Value && dr["Radius"].ToString() == "-1" && dr["LandmarkName"] != DBNull.Value)
                        {
                            DataSet ds =
                                landPointMgr.GetLandmarkPointSetByLandmarkName(dr["LandmarkName"].ToString(), sn.User.OrganizationId);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow[] drs = new DataRow[ds.Tables[0].Rows.Count];
                                ds.Tables[0].Rows.CopyTo(drs, 0);
                                coords = CreatePointsetString(drs);
                            }
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
                        return string.Empty;
                    }

                if (xml == "")
                    return string.Empty;

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
                    CAPTION = "<b>" + (string)base.GetLocalResourceObject("txtGeozone") + ":</b> " + GeoZoneName;
                    
                    Dictionary<string, string> geoZone = new Dictionary<string, string>();
                    geoZone.Add("id", "1");
                    geoZone.Add("desc", GeoZoneName);
                    geoZone.Add("type", "Geozone");
                    geoZone.Add("radius", "-1");

                    string coords = "[]";
                    if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    {

                        DataRow rowItem1 = dsGeoZoneDetails.Tables[0].Rows[0];
                        DataRow rowItem2 = dsGeoZoneDetails.Tables[0].Rows[1];

                        VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                        VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                        VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);

                        geoZone.Add("lat", center.Latitude.ToString());
                        geoZone.Add("lon", center.Longitude.ToString());

                        coords = string.Format("POLYGON(({0} {1},{2} {3},{4} {5},{6} {7}))",
                               rowItem1["Longitude"].ToString(),
                               rowItem1["Latitude"].ToString(),

                               rowItem2["Longitude"].ToString(),
                               rowItem1["Latitude"].ToString(),


                               rowItem2["Longitude"].ToString(),
                               rowItem2["Latitude"].ToString(),
                               
                               rowItem1["Longitude"].ToString(),
                               rowItem2["Latitude"].ToString()                           

                            );

                        
                        //geoZones.Add(geoZone);

                    }
                    else //Polygon
                    {
                        
                        if (dsGeoZoneDetails.Tables[0].Rows.Count <= 0) return string.Empty;
                        geoZone.Add("lat", dsGeoZoneDetails.Tables[0].Rows[0]["Latitude"].ToString());
                        geoZone.Add("lon", dsGeoZoneDetails.Tables[0].Rows[0]["Longitude"].ToString());

                        if (dsGeoZoneDetails.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] drs = new DataRow[dsGeoZoneDetails.Tables[0].Rows.Count];
                            dsGeoZoneDetails.Tables[0].Rows.CopyTo(drs, 0);
                            coords = CreatePointsetString(drs);
                        }

                        // TODO: put proper severity

                    }

                    geoZone.Add("coords", coords);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    return js.Serialize(geoZone);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            return string.Empty;
        }

        private string CreatePointsetString(DataRow[] dtPoints)
        {
            StringBuilder coords = new StringBuilder();
            coords.Append("POLYGON((");
            //int i = 0;
            foreach (DataRow rowItem in dtPoints)
            {
                coords.Append(string.Format("{0} {1},", rowItem["Longitude"].ToString(), rowItem["Latitude"].ToString() ));
            }
            coords.Remove(coords.Length - 1, 1);
            coords.Append("))");
            return coords.ToString();
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