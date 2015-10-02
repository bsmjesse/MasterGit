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
    public partial class History_frmHistoryMapVE : SentinelFMBasePage 
    {
        public string strVehicleData = "";
        public string strLandmarkData = null;
        public string strGeoZoneData = null;
        public string strUnitOfMes = "Km";
        public string token = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            VirtualEarth.AvlHistoryPin  avl;
            VirtualEarth.LandmarkPin landmark;

            string icon = "";
            strUnitOfMes = sn.User.UnitOfMes == 1 ? "Km" : "Mi";
            token = clsUtility.ValidateVEToken(); 
            StringBuilder avlDataString = new StringBuilder();
            StringBuilder ladmarkString = new StringBuilder();
            string IconType = "Circle";
            #region Vehicle Data
            if (sn.History.VehicleId == 0)
                IconType = "Circle";

               if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
               {
                   if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables.Count > 0))
                   {
                       foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                       {
                           if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                           {
                               if (dr["Speed"].ToString() != "0")
                                   icon = "Green" + IconType + dr["MyHeading"].ToString().TrimEnd() + ".gif";
                               else
                                   icon = "Red" + IconType + ".gif";

                               avl = new VirtualEarth.AvlHistoryPin();
                               avl.latitude = Convert.ToDouble(dr["Latitude"]);
                               avl.longitude = Convert.ToDouble(dr["Longitude"]);
                               avl.iconName = icon;
                               avl.speed = dr["speed"].ToString().TrimEnd() + " " + strUnitOfMes + "/h";
                               avl.messageType = dr["BoxMsgInTypeName"].ToString().TrimEnd();
                               avl.address = dr["StreetAddress"].ToString().TrimEnd();
                               avl.timestamp = dr["MyDateTime"].ToString().TrimEnd();
                               avl.description = dr["Description"].ToString().TrimEnd();

                               avlDataString.Append(avl.ToString());
                           }
                       }
                       strVehicleData = avlDataString.ToString();
                   }
               }
               else if (sn.History.DsHistoryInfo.Tables["StopData"] != null)
               {
                      foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                      {
                         TimeSpan TripIndling;
                         TripIndling = new TimeSpan(Convert.ToInt64(dr["StopDurationVal"]) * TimeSpan.TicksPerSecond);
                         double StopDurationVal = TripIndling.TotalMinutes;

                         if (Convert.ToBoolean(dr["chkBoxShow"]))
                         {
                              if (dr["Remarks"].ToString() == "Idling")
                                 icon="Idle.gif";
                              else if  (StopDurationVal < 15)
                                 icon="Stop_3.gif";
                              else if ((StopDurationVal > 15) && (StopDurationVal < 60))
                                 icon="Stop_15.gif";
                              else if (StopDurationVal > 60)
                                 icon="Stop_60.gif";

                               avl = new VirtualEarth.AvlHistoryPin();
                               avl.latitude = Convert.ToDouble(dr["Latitude"]);
                               avl.longitude = Convert.ToDouble(dr["Longitude"]);
                               avl.iconName = icon;
                               avl.speed = "0 " + strUnitOfMes + "/h";
                               avl.messageType = dr["Remarks"].ToString().TrimEnd();
                               avl.address = dr["Location"].ToString().TrimEnd();
                               avl.timestamp = dr["ArrivalDateTime"].ToString().TrimEnd();
                               avl.description = "Duration:" + dr["StopDuration"].ToString().TrimEnd();

                               avlDataString.Append(avl.ToString());
                         }
                      }
                      strVehicleData = avlDataString.ToString();
               }
            
            #endregion

            if (!sn.Map.ReloadVirtualEarthData)
                return; 

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
                        if (Convert.ToInt16(dr["Type"].ToString().TrimEnd()) == Convert.ToInt16(VLF.CLS.Def.Enums.GeozoneType.Rectangle))
                        {
                            geozone.numPoints = 2;
                            DataRow rowItem1 =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0];
                            DataRow rowItem2 =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[1];

                            //VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                            //VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                            //VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);
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

        }
    }

