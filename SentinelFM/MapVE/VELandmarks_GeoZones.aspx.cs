using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.IO;  
namespace SentinelFM
{
    public partial class VELandmarks_GeoZones : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VirtualEarth.LandmarkPin landmark;
            string icon = "";
            StringBuilder ladmarkString = new StringBuilder();
            Int32 LandmarkId = 0;
            Int32 GeoZoneId = 0;
            sn.Map.GeozonesMapVE = "";
            sn.Map.LandmarksMapVE = "";
            sn.Map.AvlMapVE = "";
            sn.Map.EditMapVE = "false"; 

            #region View Existing Landmark - Read Only
            if (Request.QueryString["LandmarkId"] != null)
            {
                sn.Landmark.RefreshFormName = "Landmark";
                LandmarkId = Convert.ToInt32(Request.QueryString["LandmarkId"]);
                if ((sn.DsLandMarks != null) &&
                  (sn.DsLandMarks.Tables.Count > 0) &&
                  (sn.DsLandMarks.Tables[0].Rows.Count > 0))
                {
                    DataRow[] drArr = sn.DsLandMarks.Tables[0].Select("LandmarkId=" + LandmarkId);
                    if (drArr == null)
                        return;

                    landmark = new VirtualEarth.LandmarkPin();
                    landmark.latitude = Convert.ToDouble(drArr[0]["Latitude"]);
                    landmark.longitude = Convert.ToDouble(drArr[0]["Longitude"]);
                    landmark.description = drArr[0]["LandmarkName"].ToString().TrimEnd().Replace("'", "`");
                    landmark.radius = sn.User.UnitOfMes == 1 ? Convert.ToInt32(drArr[0]["Radius"]) : Convert.ToInt32(Math.Round(Convert.ToInt32(drArr[0]["Radius"]) * 3.28, 0)); ;
                    ladmarkString.Append(landmark.ToVEstring());
                    sn.Map.LandmarksMapVE = ladmarkString.ToString();

                }

                return;
            }
            #endregion

            #region View Existing GeoZone - Read Only
            if (Request.QueryString["GeoZoneId"] != null)
            {
                GeoZoneId = Convert.ToInt32(Request.QueryString["GeoZoneId"]);
                DrawGeoZone(GeoZoneId);
                return;
            }
            #endregion

            #region View all Landmarks

            if ((sn.DsLandMarks != null) &&
                   (sn.DsLandMarks.Tables.Count > 0) &&
                   (sn.DsLandMarks.Tables[0].Rows.Count > 0))
            {

                foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                {

                    landmark = new VirtualEarth.LandmarkPin();
                    landmark.latitude = Convert.ToDouble(dr["Latitude"]);
                    landmark.longitude = Convert.ToDouble(dr["Longitude"]);
                    landmark.description = dr["LandmarkName"].ToString().TrimEnd().Replace("'", "`");
                    landmark.radius = sn.User.UnitOfMes == 1 ? Convert.ToInt32(dr["Radius"]) : Convert.ToInt32(Math.Round(Convert.ToInt32(dr["Radius"]) * 3.28, 0)); ;
                    ladmarkString.Append(landmark.ToVEstring());
                }

                sn.Map.LandmarksMapVE = ladmarkString.ToString();

            }
            #endregion

            #region View all GeoZones
            if (sn.Map.ShowGeoZone)
            {
                if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                    DsGeoZone_Fill();
                if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                    DrawGeoZones();

            }

            #endregion
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
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        private void DrawGeoZones()
        {
            try
            {
                string tableName = "";
                VirtualEarth.GeozoneShape geozone;
                StringBuilder geozoneString = new StringBuilder();

                foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                {
                    tableName = dr["GeoZoneId"].ToString();
                    geozone = new VirtualEarth.GeozoneShape();
                    geozone.description = dr["GeoZoneName"].ToString().TrimEnd();
                    geozone.severity = Convert.ToInt16(dr["SeverityId"]);
                    geozone.type = Convert.ToInt16(dr["Type"].ToString().TrimEnd());
                    if (sn.GeoZone.DsGeoDetails != null &&sn.GeoZone.DsGeoDetails.Tables[tableName] != null &&sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count > 0)
                    {
                        if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                        {
                            geozone.numPoints = 2;
                            DataRow rowItem1 =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0];
                            DataRow rowItem2 =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[1];
                            geozone.pointList = rowItem1["Latitude"] + "^" + rowItem1["Longitude"] + "^" + rowItem2["Latitude"] + "^" + rowItem2["Longitude"];

                        }
                        else //Polygon
                        {

                            geozone.numPoints =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count;
                            foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[tableName].Rows)
                            {
                                geozone.pointList += rowItem["Latitude"] + "^" + rowItem["Longitude"] + "^";
                            }

                            geozone.pointList = geozone.pointList.Substring(0, geozone.pointList.Length - 1);
                        }

                        geozoneString.Append(geozone.ToVEstring());

                    }
                }

                sn.Map.GeozonesMapVE= geozoneString.ToString();

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

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

                DataRow[] drArr =sn.GeoZone.DsGeoZone.Tables[0].Select("GeoZoneId='" + GeoZoneId + "'");
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
                        geozone.pointList = rowItem1["Latitude"] + "^" + rowItem1["Longitude"] + "^" + rowItem2["Latitude"] + "^" + rowItem2["Longitude"];

                    }
                    else //Polygon
                    {

                        geozone.numPoints = dsGeoZoneDetails.Tables[0].Rows.Count;
                        foreach (DataRow rowItem in dsGeoZoneDetails.Tables[0].Rows)
                        {
                            geozone.pointList += rowItem["Latitude"] + "^" + rowItem["Longitude"] + "^";
                        }

                        geozone.pointList = geozone.pointList.Substring(0, geozone.pointList.Length - 1);
                    }

                    geozoneString.Append(geozone.ToVEstring());

                }

                sn.Map.GeozonesMapVE = geozoneString.ToString();

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
    }
}
