using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;

namespace SentinelFM
{
    public partial class frmLandmarkGeoZoneVE : SentinelFMBasePage
    {
        public string strLandmarkData = null;
        public string strGeoZoneData = null;
        public string strUnitOfMes = "Km";
        public string flagEditData = "false";
        public string flagShowLandmarks = "false";
        public string flagShowGeoZones = "false";
        public string token = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Int32 LandmarkId = 0;
            Int32 GeoZoneId = 0;
            token = clsUtility.ValidateVEToken();  
            StringBuilder ladmarkString = new StringBuilder();
            if (Request.QueryString["FormName"] != null)
            {
                flagEditData = "true";
                sn.Landmark.RefreshFormName = Request.QueryString["FormName"];
            }

            #region Read Geozones/Landmarks points
            if (Request.Form["Points"] != null && Request.Form["Points"] != "")
            {
                ReadPointsInfo();
                return;
            }
            #endregion

            #region View Existing Landmark - Read Only
            if (Request.QueryString["LandmarkId"] != null)
            {
                flagShowLandmarks = "true";
                sn.Landmark.RefreshFormName = "Landmark";
                LandmarkId = Convert.ToInt32(Request.QueryString["LandmarkId"]);
                if ((sn.DsLandMarks != null) &&
                  (sn.DsLandMarks.Tables.Count > 0) &&
                  (sn.DsLandMarks.Tables[0].Rows.Count > 0))
                {
                    DataRow[] drArr = sn.DsLandMarks.Tables[0].Select("LandmarkId=" + LandmarkId);
                    if (drArr == null)
                        return;

                    VirtualEarth.LandmarkPin landmark = new VirtualEarth.LandmarkPin();
                    landmark.latitude = Convert.ToDouble(drArr[0]["Latitude"]);
                    landmark.longitude = Convert.ToDouble(drArr[0]["Longitude"]);
                    landmark.description = drArr[0]["LandmarkName"].ToString().TrimEnd().Replace("'", "`");
                    landmark.radius = sn.User.UnitOfMes == 1 ? Convert.ToInt32(drArr[0]["Radius"]) : Convert.ToInt32(Math.Round(Convert.ToInt32(drArr[0]["Radius"]) * 3.28, 0)); ;
                    ladmarkString.Append(landmark.ToString());
                    strLandmarkData = ladmarkString.ToString();

                }

                return;
            }
            #endregion

            #region View Existing Landmark - Edit mode
            if (Request.QueryString["FormName"] != null && Request.QueryString["FormName"] == "Landmark")
            {
                flagShowLandmarks = "true";
                if (sn.Landmark.Y == 0 || sn.Landmark.X == 0)
                    flagShowLandmarks = "false";

                VirtualEarth.LandmarkPin landmark = new VirtualEarth.LandmarkPin();
                landmark.latitude = sn.Landmark.Y;
                landmark.longitude = sn.Landmark.X ;
                landmark.description = sn.Landmark.LandmarkName.ToString().TrimEnd().Replace("'", "`");
                landmark.radius = sn.User.UnitOfMes == 1 ? sn.Landmark.Radius : Convert.ToInt32(Math.Round(sn.Landmark.Radius * 3.28, 0)); ;
                ladmarkString.Append(landmark.ToString());
                strLandmarkData = ladmarkString.ToString();
                return;
            }
            #endregion

            #region View Existing Geozone - Read Only
            if (Request.QueryString["GeoZoneId"] != null)
            {
                flagShowGeoZones = "true";
                sn.Landmark.RefreshFormName = "Geozone";
                GeoZoneId = Convert.ToInt32(Request.QueryString["GeoZoneId"]);
                DrawGeoZone(GeoZoneId);
                return; 
            }
            #endregion

            #region View Existing Geozone - Edit mode
            if (Request.QueryString["FormName"] != null && Request.QueryString["FormName"] == "Geozone")
            {
                flagShowGeoZones = "true";
                if (sn.GeoZone.DsGeoDetails != null &&sn.GeoZone.DsGeoDetails.Tables[0] != null &&sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 0)
                    DrawGeoZonesByDataSet(); 

            }
            #endregion

            strUnitOfMes = sn.User.UnitOfMes == 1 ? "Km" : "Mi";

        }
        private void ReadPointsInfo()
       {
            string strJavaScript = "";
            Int32 LandmarkId = 0;
            Int32 GeoZoneId = 0;

            string points = Request.Form["Points"];
            string[] tmpPoints = points.Split(',');

            if (tmpPoints.Length == 0)
            {
                strJavaScript += "<script language='javascript'>self.close();</script>";
                Response.Write(strJavaScript);
                return;
            }

            if (sn.Landmark.RefreshFormName == "Landmark")
            {
                strJavaScript += "<script language='javascript'>";
                strJavaScript += " window.opener.document.forms[0].txtX.value=" + tmpPoints[1] + ";";
                strJavaScript += " window.opener.document.forms[0].txtY.value=" + tmpPoints[0] + ";";
                strJavaScript += " self.close();</script>";
                Response.Write(strJavaScript);

            }
            else if (sn.Landmark.RefreshFormName == "Geozone")
            {
                try
                {
                    if (sn.GeoZone.DsGeoDetails.Tables[0] != null &&sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 0)
                       sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();

                    if (tmpPoints.Length == 4)
                       sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Rectangle;
                    else if (tmpPoints.Length > 4)
                       sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Polygon ;
                    else
                       sn.GeoZone.GeozoneTypeId = VLF.CLS.Def.Enums.GeozoneType.Unknown;

                    int i = 0;
                    while (i < tmpPoints.Length)
                    {
                        DataRow dr =sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                        dr["Latitude"] = tmpPoints[i];
                        dr["Longitude"] = tmpPoints[i+1];
                        dr["SequenceNum"] =sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                       sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr);
                        i += 2;
                    }

                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    strJavaScript += "<script language='javascript'>";
                    strJavaScript += " window.opener.document.forms[0].txtMapMessage.value='Geozone add failed.'; self.close();</script>";
                    return;
                }

                strJavaScript += "<script language='javascript'>";
                strJavaScript += " window.opener.document.forms[0].txtMapMessage.value='Geozone was drawn successfully.'; self.close();</script>";
                Response.Write(strJavaScript);

            }
        }

        private void DrawGeoZone(Int32 GeoZoneId)
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsGeoZoneDetails = new DataSet();
                StringBuilder geozoneString = new StringBuilder();
                string xml = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(GeoZoneId), ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(GeoZoneId), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsGeoZoneDetails.ReadXml(strrXML);

                DataRow[] drArr=sn.GeoZone.DsGeoZone.Tables[0].Select("GeoZoneId='" + GeoZoneId+"'");
                if (drArr == null)
                    return;

                    VirtualEarth.GeozoneShape geozone = new VirtualEarth.GeozoneShape();
                    geozone.description = drArr[0]["GeoZoneName"].ToString().TrimEnd();
                    geozone.severity = Convert.ToInt16(drArr[0]["SeverityId"]);
                    geozone.type = Convert.ToInt16(drArr[0]["Type"].ToString().TrimEnd());
                
                    if (dsGeoZoneDetails != null && dsGeoZoneDetails.Tables[0] != null && dsGeoZoneDetails.Tables[0].Rows.Count > 0)
                    {
                        if (geozone.type == Convert.ToInt16(VLF.CLS.Def.Enums.GeozoneType.Rectangle)) 
                        {
                            geozone.numPoints = 2;
                            DataRow rowItem1 = dsGeoZoneDetails.Tables[0].Rows[0];
                            DataRow rowItem2 = dsGeoZoneDetails.Tables[0].Rows[1];
                            geozone.pointList = rowItem1["Latitude"] + ";" + rowItem1["Longitude"] + ";" + rowItem2["Latitude"] + ";" + rowItem2["Longitude"];

                        }
                        else //Polygon
                        {

                            geozone.numPoints = dsGeoZoneDetails.Tables[0].Rows.Count;
                            foreach (DataRow rowItem in dsGeoZoneDetails.Tables[0].Rows)
                            {
                                geozone.pointList += rowItem["Latitude"] + ";" + rowItem["Longitude"] + ";";
                            }

                            geozone.pointList = geozone.pointList.Substring(0, geozone.pointList.Length - 1);
                        }

                        geozoneString.Append(geozone.ToString());

                    }

                strGeoZoneData = geozoneString.ToString();

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void DrawGeoZonesByDataSet()
        {

            try
            {

                    VirtualEarth.GeozoneShape geozone = new VirtualEarth.GeozoneShape();
                    StringBuilder geozoneString = new StringBuilder();
                    geozone.description =sn.GeoZone.Description  ;
                    geozone.severity =sn.GeoZone.Severity ;
                    geozone.type = Convert.ToInt16(sn.GeoZone.GeozoneTypeId);
                    if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    {
                        geozone.numPoints = 2;
                        DataRow rowItem1 =sn.GeoZone.DsGeoDetails.Tables[0].Rows[0];
                        DataRow rowItem2 =sn.GeoZone.DsGeoDetails.Tables[0].Rows[1];
                        geozone.pointList = rowItem1["Latitude"] + ";" + rowItem1["Longitude"] + ";" + rowItem2["Latitude"] + ";" + rowItem2["Longitude"];

                    }
                    else //Polygon
                    {

                        geozone.numPoints =sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count;
                        foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[0].Rows)
                        {
                            geozone.pointList += rowItem["Latitude"] + ";" + rowItem["Longitude"] + ";";
                        }

                        geozone.pointList = geozone.pointList.Substring(0, geozone.pointList.Length - 1);
                    }

                    geozoneString.Append(geozone.ToString());
                    strGeoZoneData = geozoneString.ToString();


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

    }
}
