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
using System.Text;

namespace SentinelFM
{

    public partial class GeoZone_Landmarks_frmGeozoneLandmarkMapFrame : SentinelFMBasePage
    {

        public string strVehicleData = null;
        public string strLandmarkData = null;
        public string strGeoZoneData = null;
        public string strUnitOfMes = "Km";
        public string token = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            VirtualEarth.LandmarkPin landmark;
            string icon = "";
            strUnitOfMes = sn.User.UnitOfMes == 1 ? "Km" : "Mi";
            StringBuilder ladmarkString = new StringBuilder();
            token = clsUtility.ValidateVEToken();  


            //if (!sn.Map.ReloadVirtualEarthData)
            //    return;

            #region Landmark Data

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
                    ladmarkString.Append(landmark.ToString());
                }

                strLandmarkData = ladmarkString.ToString();

            }
            #endregion

            #region Geozones
            if (sn.Map.ShowGeoZone)
            {
                if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                    DsGeoZone_Fill();
                if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                    DrawGeoZones();

            }

            #endregion

            sn.Map.ReloadVirtualEarthData = false;

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
                            geozone.pointList = rowItem1["Latitude"] + ";" + rowItem1["Longitude"] + ";" + rowItem2["Latitude"] + ";" + rowItem2["Longitude"];

                        }
                        else //Polygon
                        {

                            geozone.numPoints =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count;
                            foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[tableName].Rows)
                            {
                                geozone.pointList += rowItem["Latitude"] + ";" + rowItem["Longitude"] + ";";
                            }

                            geozone.pointList = geozone.pointList.Substring(0, geozone.pointList.Length - 1);
                        }

                        geozoneString.Append(geozone.ToString());

                    }
                }

                strGeoZoneData = geozoneString.ToString();

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
        protected void cmdLandmark_Click(object sender, EventArgs e)
        {
            try
            {
               sn.GeoZone.ImgConfPath = "";
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
               sn.GeoZone.AddMode = false;
               sn.GeoZone.EditMode = false;
                Response.Redirect("frmLandmark.aspx");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
        protected void cmdGeoZone_Click(object sender, EventArgs e)
        {
            try
            {
               sn.GeoZone.ImgConfPath = "";
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
               sn.GeoZone.AddMode = false;
               sn.GeoZone.EditMode = false;
                Response.Redirect("frmGeoZone.aspx");
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
        protected void cmdVehicleGeoZone_Click(object sender, EventArgs e)
        {
            try
            {
                sn.Landmark.AddLandmarkMode = false;
                sn.Landmark.EditLandmarkMode = false;
               sn.GeoZone.AddMode = false;
               sn.GeoZone.EditMode = false;
                sn.Cmd.BoxId = 0;
                Response.Redirect("frmVehicleGeoZoneAss.htm");

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }
    }
}
