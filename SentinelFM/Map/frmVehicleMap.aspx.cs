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


namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmVehicleMap.
    /// </summary>
    public partial class frmVehicleMap : SentinelFMBasePage
    {
        
        public VLF.MAP.ClientMapProxy map;
        public int imageW=600 ;
        public int imageH =325;
        public int AutoRefreshTimer = 60000;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
               Response.Redirect("../MapNew/frmvehiclemap.aspx");
                    if (Request.QueryString["clientWidth"] != null)
                        sn.User.ScreenWidth = Convert.ToInt32(Request.QueryString["clientWidth"])+220;

                imageW = Convert.ToInt32(sn.User.ScreenWidth) - 310;
               
                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;

                // create ClientMapProxy only for mapping
                map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
                if (map == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }
                map.MapWidth = imageW;
                map.MapHeight = imageH;
                

                if (!Page.IsPostBack)
                {
                   //DateTime dt = System.DateTime.Now;

                   //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map started ->  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));


                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleMapForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    GuiSecurity(this);
                    //Clear Tooltips
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    if (sn.Map.ShowDefaultMap)
                    {
                        sn.Map.ShowDefaultMap = false;
                        LoadDefaultMap();
                        return;
                    }


                    // Get user selected area coordinates
                    string CoordInX = Request.QueryString["CoordInX"];
                    string CoordInY = Request.QueryString["CoordInY"];
                    string CoordEndX = Request.QueryString["CoordEndX"];
                    string CoordEndY = Request.QueryString["CoordEndY"];

                    //Check for ZoomIn/ZoomOut menu options
                    string ZoomIn = Request.QueryString["ZoomIn"];

                    if (ZoomIn != null)
                    {
                        if (ZoomIn == "True")
                        {
                            ZoomInMap(true, 2);

                        }
                        else if (ZoomIn == "False")
                        {
                            ZoomInMap(false, 2);

                        }
                        return;
                    }


                    //Check for refresh

                    if (sn.Map.MapRefresh)
                        Response.Write("<script language='javascript'>window.setTimeout('AutoReloadMap()'," + AutoRefreshTimer.ToString() + ")</script>");
                    else
                        Response.Write("<script language='javascript'> clearTimeout();</script>");

                    // TimeSpan currDuration;
                    // Redraw map by coorinates
                    if ((CoordInX != null) && (CoordInX != "0"))
                    {
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);

                        //currDuration = new TimeSpan(System.DateTime.Now.Ticks - dt.Ticks);
                        //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map ended <- Duration: " + currDuration.TotalSeconds.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                        return;
                    }

                    // Redraw map on "Map It" and autorefresh	
                    if ((sn != null) && (sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
                        LoadVehiclesMap();
                    else
                        LoadDefaultMap();

                     //currDuration = new TimeSpan(System.DateTime.Now.Ticks - dt.Ticks);
                     //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map ended <- Duration: " + currDuration.TotalSeconds.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                }

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

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.btnMoveNorth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveNorth_Click);
            //this.btnMoveEast.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveEast_Click);
            //this.btnMoveWest.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveWest_Click);
            //this.btnMoveSouth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveSouth_Click);

        }
        #endregion

       private void ZoomInMap(bool zoomIn)
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;

                if ((sn == null) || (sn.UserName == ""))
                    RedirectToLogin();


                // Retrieves Map from GeoMicro
                // restore map states
                sn.Map.RetrievesMapStateFromViewState(sn, map);

                if (sn.Map.ShowGeoZone)
                {
                    if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                        DsGeoZone_Fill();

                    if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                        DrawGeoZones();
                }

                map.Zoom(zoomIn);
                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if (map.LastUsedMapID == VLF.MAP.MapType.NotSet)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "CreateMap returned: " + map.GetLastErrorDescription()));
                }
                else
                {
                    
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), false))
                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                        }
                }

                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {

                    LoadDefaultMap();
                    return;

                }


                sn.Map.ImgPath = url;
                sn.Map.SavesMapStateToViewState(sn, map);
                sn.Map.CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
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



        private void ZoomMap(MapZoomLevel mapZoomLevel)
        {
            try
            {
                if ((sn == null) || (sn.UserName == ""))
                    RedirectToLogin();


                // Retrieves Map from GeoMicro
                // restore map states
                sn.Map.RetrievesMapStateFromViewState(sn, map);

                if (sn.Map.ShowGeoZone)
                {
                    if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                        DsGeoZone_Fill();

                    if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                        DrawGeoZones();
                }

                map.Zoom(mapZoomLevel);
                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if (map.LastUsedMapID == VLF.MAP.MapType.NotSet)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "CreateMap returned: " + map.GetLastErrorDescription()));
                }
                else
                {
                    
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), false))
                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                        }
                }


                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;

                }



                if (url != "")
                    sn.Map.ImgPath = url;

                sn.Map.SavesMapStateToViewState(sn, map);
                sn.Map.CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
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

        protected void btnMaxZoom_Click(object sender, System.EventArgs e)
        {
            ZoomMap(MapZoomLevel.StreetLevelHigh);
            SetColorZoomLevel(map);
        }

        protected void btnStreetLevel1_Click(object sender, System.EventArgs e)
        {
            ZoomMap(MapZoomLevel.StreetLevelNorm5);
            SetColorZoomLevel(map);
        }

        protected void btnStreetLevel2_Click(object sender, System.EventArgs e)
        {
            ZoomMap(MapZoomLevel.StreetLevelMin);
            SetColorZoomLevel(map);
        }

        protected void btnRegionLevel1_Click(object sender, System.EventArgs e)
        {
            ZoomMap(MapZoomLevel.RegionLevelNorm);
            SetColorZoomLevel(map);
        }

        protected void btnRegionLevel2_Click(object sender, System.EventArgs e)
        {
            ZoomMap(MapZoomLevel.RegionLevelMin);
            SetColorZoomLevel(map);
        }

        protected void btnCountryLevel_Click(object sender, System.EventArgs e)
        {
            ZoomMap(MapZoomLevel.CountryLevelNorm);
            SetColorZoomLevel(map);
        }

        private void LoadDefaultMap()
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;

                // Set default map coordinates

                map.DrawAllVehicles = false;
                string url = map.GetDefaultMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                sn.Map.ImgPath = url;
                sn.Map.VehiclesMappings = "";
                sn.Map.VehiclesToolTip = "";
                sn.Map.SavesMapStateToViewState(sn, map);
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



        private void LoadVehiclesMap()
        {
            try
            {
        
                bool blnVehicleSelected = false;
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;


                if ((sn != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
                {
                    foreach (DataRow dr in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (dr["chkBoxShow"].ToString().ToLower() == "true")
                        {

                            if (Convert.ToDateTime(dr["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            {
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Grey"+ dr["IconTypeName"].ToString().TrimEnd()+".ico", dr["Description"].ToString().TrimEnd()));
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


                            blnVehicleSelected = true;
                        }
                    }
                }


                if (blnVehicleSelected)
                {
                    map.DrawAllVehicles = true;
                    if (sn.User.MapEngine[0].MapEngineID.ToString() == "MapsoluteStatic")
                    {
                       map.ImageDistance = sn.Map.ImageDistance;
                    }
                   

                }
                else
                {
                    sn.Map.RetrievesMapStateFromViewState(sn, map);
                }


                if (sn.Map.MapSearch)
                {
                    map.MapCenterLongitude = Convert.ToDouble(sn.History.MapCenterLongitude);
                    map.MapCenterLatitude = Convert.ToDouble(sn.History.MapCenterLatitude);
                    map.ImageDistance = Convert.ToDouble(sn.History.ImageDistance);
                    map.MapScale = Convert.ToDouble(sn.History.MapScale);
                    map.DrawAllVehicles = false;
                    sn.Map.MapSearch = false;
                }
                else
                    map.DrawAllVehicles = true;

                sn.Map.DrawLandmarks(sn, ref map);

                if (sn.Map.ShowGeoZone)
                {
                    if (sn.GeoZone.DsGeoZone == null || sn.GeoZone.DsGeoDetails == null)
                        DsGeoZone_Fill();

                    if (sn.GeoZone.DsGeoZone != null && sn.GeoZone.DsGeoDetails != null)
                        DrawGeoZones();
                }

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Start -Map Engine [" + sn.User.MapEngine[0].MapEngineID + "] Map url: Internal [" + sn.User.MapEngine[0].MapEngineLink.ToString() + "],External [" + sn.User.MapEngine[0].MapEngineExternalLink.ToString() + "]"));

                string url =map.CreateMap();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "End - Map url:" + url));

                clsMap.AdjustMapURL(Request.IsSecureConnection,ref url);
                                
                
                if (map.LastUsedMapID == VLF.MAP.MapType.NotSet)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "CreateMap returned: " + map.GetLastErrorDescription()));
                }
                else
                {
                    
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), false))
                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                        }
                }





                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {

                    LoadDefaultMap();
                    return;

                }


                if (url != "")
                    sn.Map.ImgPath = url;

                sn.Map.SavesMapStateToViewState(sn, map);
                sn.Map.CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);

                
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

       protected void btnMoveWest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.West);
        }


        private void MoveMap(MapPanDirection mapDirection)
        {
            try
            {
                if ((sn == null) || (sn.UserName == ""))
                    RedirectToLogin();


                // Retrieves Map from GeoMicro
                // restore map states
                sn.Map.RetrievesMapStateFromViewState(sn, map);

                if (sn.Map.ShowGeoZone)
                {
                    if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                        DsGeoZone_Fill();

                    if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                        DrawGeoZones();
                }

                map.Pan(mapDirection);
                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if (map.LastUsedMapID == VLF.MAP.MapType.NotSet)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "CreateMap returned: " + map.GetLastErrorDescription()));
                }
                else
                {
                    
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), false))
                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                        }
                }





                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;

                }



                if (url != "")
                    sn.Map.ImgPath = url;

                sn.Map.SavesMapStateToViewState(sn, map);
                sn.Map.CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
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

        protected  void btnMoveNorth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.North);
        }

       protected void btnMoveEast_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.East);
        }

       protected void btnMoveSouth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.South);
        }


        private void RedrawMapByXY(string CoordInX, string CoordInY, string CoordEndX, string CoordEndY)
        {
            try
            {

                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;

                sn.Map.DrawLandmarks(sn, ref map);   

                if (sn.Map.ShowGeoZone)
                {
                    if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                        DsGeoZone_Fill();

                    if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                        DrawGeoZones();
                }

                if ((sn != null) && (sn.Map.DsFleetInfo != null))
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

        
                map.DrawAllVehicles = sn.Map.DrawAllVehicles;
                map.MapCenterLongitude = sn.Map.XInCoord;
                map.MapCenterLatitude = sn.Map.YInCoord;
                map.MapScale = sn.Map.MapScale;
                map.ImageDistance = sn.Map.ImageDistance;
                map.SouthWestCorner = sn.Map.SouthWestCorner;
                map.NorthEastCorner = sn.Map.NorthEastCorner;


                if (CoordInX != "0")
                {
                    map.DrawAllVehicles = false;

                    if ((CoordEndX == CoordInX) || (Convert.ToDouble(CoordEndX) - Convert.ToDouble(CoordInX)) < 5 || (Convert.ToDouble(CoordInX) - Convert.ToDouble(CoordEndX)) > 5)
                    {
                        VLF.MAP.GeoPoint geopoint = map.ConvertPxToLatLon(new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordInY)));
                        map.MapCenterLongitude = geopoint.Longitude;
                        map.MapCenterLatitude = geopoint.Latitude;

                        //if (sn.Map.SelectedZoomLevelType == 1)
                        //    map.Zoom(true);

                        //if (sn.Map.SelectedZoomLevelType == 2)
                        //    map.Zoom(false);

                    }
                    else
                    {
                        if (!map.ResizeMap(new ImagePoint(Convert.ToInt32(CoordEndX), Convert.ToInt32(CoordInY)),
                            new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordEndY))))
                        {
                            sn.Map.CreateVehiclesTooltip(sn, map);
                            SetColorZoomLevel(map);
                            return;
                        }
                    }

                   
                }

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if (map.LastUsedMapID == VLF.MAP.MapType.NotSet)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "CreateMap returned: " + map.GetLastErrorDescription()));
                }
                else
                {
                    
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), false))
                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                        }
                }





                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;
                }


                if (url != "")
                    sn.Map.ImgPath = url;

                sn.Map.SavesMapStateToViewState(sn, map);
                sn.Map.CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);

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







        private void cmdBigMap_Click(object sender, System.EventArgs e)
        {
            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='';";
            strUrl = strUrl + " var w=950;";
            strUrl = strUrl + " var h=650;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=auto,fullscreen=yes,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

            strUrl = strUrl + " NewWindow('frmBigMapWait.aspx');</script>";
            Response.Write(strUrl);
        }





        private void SetColorZoomLevel(ClientMapProxy map)
        {

           double mapScale = map.MapScale;

           if (sn.User.MapEngine[0].MapEngineID.ToString() == "MapsoluteStatic")
              mapScale = map.ImageDistance;

           switch (map.GetZoomLevelByScale(mapScale))
            {
                case VLF.MAP.MapZoomLevel.StreetLevelMax:
                case VLF.MAP.MapZoomLevel.StreetLevelMax2:
                case VLF.MAP.MapZoomLevel.StreetLevelHigh:
                case VLF.MAP.MapZoomLevel.StreetLevelHigh2:
                case VLF.MAP.MapZoomLevel.StreetLevelNorm:
                case VLF.MAP.MapZoomLevel.StreetLevelNorm2:
                case VLF.MAP.MapZoomLevel.StreetLevelNorm3:
                case VLF.MAP.MapZoomLevel.StreetLevelNorm4:
                    {

                        this.btnCountryLevel.BackColor = Color.White;
                        this.btnMaxZoom.BackColor = Color.Gray ;
                        this.btnRegionLevel1.BackColor = Color.White;
                        this.btnRegionLevel2.BackColor = Color.White;
                        this.btnStreetLevel1.BackColor = Color.White;
                        this.btnStreetLevel2.BackColor = Color.White;

                    }
                    break;

                case VLF.MAP.MapZoomLevel.StreetLevelNorm5:
                case VLF.MAP.MapZoomLevel.StreetLevelNorm6:
                case VLF.MAP.MapZoomLevel.StreetLevelLow:
                case VLF.MAP.MapZoomLevel.StreetLevelLow2:
                    {

                        this.btnCountryLevel.BackColor = Color.White;
                        this.btnMaxZoom.BackColor = Color.White;
                        this.btnRegionLevel1.BackColor = Color.White;
                        this.btnRegionLevel2.BackColor = Color.White;
                        this.btnStreetLevel1.BackColor = Color.Gray;
                        this.btnStreetLevel2.BackColor = Color.White;

                    }
                    break;

                case VLF.MAP.MapZoomLevel.StreetLevelMin:
                case VLF.MAP.MapZoomLevel.StreetLevelMin2:
                case VLF.MAP.MapZoomLevel.RegionLevelMax:
                case VLF.MAP.MapZoomLevel.RegionLevelMax2:
                case VLF.MAP.MapZoomLevel.RegionLevelHigh:
                case VLF.MAP.MapZoomLevel.RegionLevelHigh2:
                    {


                        this.btnCountryLevel.BackColor = Color.White;
                        this.btnMaxZoom.BackColor = Color.White;
                        this.btnRegionLevel1.BackColor = Color.White;
                        this.btnRegionLevel2.BackColor = Color.White;
                        this.btnStreetLevel1.BackColor = Color.White;
                        this.btnStreetLevel2.BackColor = Color.Gray;

                    }
                    break;



                case VLF.MAP.MapZoomLevel.RegionLevelNorm:
                case VLF.MAP.MapZoomLevel.RegionLevelNorm2:
                case VLF.MAP.MapZoomLevel.RegionLevelLow:
                case VLF.MAP.MapZoomLevel.RegionLevelLow2:
                    {


                        this.btnCountryLevel.BackColor = Color.White;
                        this.btnMaxZoom.BackColor = Color.White;
                        this.btnRegionLevel1.BackColor = Color.Gray;
                        this.btnRegionLevel2.BackColor = Color.White;
                        this.btnStreetLevel1.BackColor = Color.White;
                        this.btnStreetLevel2.BackColor = Color.White;

                    }
                    break;




                case VLF.MAP.MapZoomLevel.RegionLevelMin:
                case VLF.MAP.MapZoomLevel.RegionLevelMin2:
                case VLF.MAP.MapZoomLevel.CountryLevelMax:
                case VLF.MAP.MapZoomLevel.CountryLevelMax2:
                case VLF.MAP.MapZoomLevel.CountryLevelHigh:
                case VLF.MAP.MapZoomLevel.CountryLevelHigh2:
                    {

                        this.btnCountryLevel.BackColor = Color.White;
                        this.btnMaxZoom.BackColor = Color.White;
                        this.btnRegionLevel1.BackColor = Color.White;
                        this.btnRegionLevel2.BackColor = Color.Gray;
                        this.btnStreetLevel1.BackColor = Color.White;
                        this.btnStreetLevel2.BackColor = Color.White;

                    }
                    break;

                case VLF.MAP.MapZoomLevel.CountryLevelNorm:
                case VLF.MAP.MapZoomLevel.CountryLevelNorm2:
                case VLF.MAP.MapZoomLevel.CountryLevelLow:
                case VLF.MAP.MapZoomLevel.CountryLevelLow2:
                case VLF.MAP.MapZoomLevel.CountryLevelMin:
                case VLF.MAP.MapZoomLevel.CountryLevelMin2:
                    {

                        this.btnCountryLevel.BackColor = Color.Gray;
                        this.btnMaxZoom.BackColor = Color.White;
                        this.btnRegionLevel1.BackColor = Color.White;
                        this.btnRegionLevel2.BackColor = Color.White;
                        this.btnStreetLevel1.BackColor = Color.White;
                        this.btnStreetLevel2.BackColor = Color.White;

                    }
                    break;

                default:
                    break;

            }

        }



        private void GuiSecurity(System.Web.UI.Control obj)
        {

            foreach (System.Web.UI.Control ctl in obj.Controls)
            {
                try
                {
                    if (ctl.HasControls())
                        GuiSecurity(ctl);

                    System.Web.UI.WebControls.Button CmdButton = (System.Web.UI.WebControls.Button)ctl;
                    bool CmdStatus = false;
                    if (CmdButton.CommandName != "")
                    {
                        CmdStatus = sn.User.ControlEnable(sn, Convert.ToInt32(CmdButton.CommandName));
                        CmdButton.Enabled = CmdStatus;
                    }

                }
                catch
                {
                }
            }
        }

      

        protected void cmdZoomOut_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 2;
        }



        private void ZoomInMap(bool zoomIn, int level)
        {
            try
            {
                for (int i = 0; i < level; i++)
                {
                    sn.Map.RetrievesMapStateFromViewState(sn, map);

                    if (sn.Map.ShowGeoZone)
                    {
                        if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                            DsGeoZone_Fill();

                        if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                            DrawGeoZones();
                    }
                    map.Zoom(zoomIn);
                    sn.Map.SavesMapStateToViewState(sn, map);
                }
                ZoomInMap(zoomIn);
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }



       protected void cmdRecenter_Click(object sender, ImageClickEventArgs e)
       {
          sn.Map.SelectedZoomLevelType = 0;
          if ((sn != null) && (sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
             LoadVehiclesMap();
          else
             LoadDefaultMap();
       }

       protected void cmdZoomInMain_Click(object sender, ImageClickEventArgs e)
       {
          
          this.btnCountryLevel.BackColor = Color.White;
          this.btnMaxZoom.BackColor = Color.White;
          this.btnRegionLevel1.BackColor = Color.White;
          this.btnRegionLevel2.BackColor = Color.White;
          this.btnStreetLevel1.BackColor = Color.White;
          this.btnStreetLevel2.BackColor = Color.White;
          //this.btnZoomOut.BackColor=Color.Gray;

          ZoomInMap(true, 2);
         

       }
       protected void cmdZoomOutMain_Click(object sender, ImageClickEventArgs e)
       {
          this.btnCountryLevel.BackColor = Color.White;
          this.btnMaxZoom.BackColor = Color.White;
          this.btnRegionLevel1.BackColor = Color.White;
          this.btnRegionLevel2.BackColor = Color.White;
          this.btnStreetLevel1.BackColor = Color.White;
          this.btnStreetLevel2.BackColor = Color.White;
          
          ZoomInMap(false, 2);
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

                StringReader strrXML = null;
                DataSet dsGeoZoneDetails ;
                
                string xml = "";
                string tableName = "";
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                {
                    tableName = dr["GeoZoneId"].ToString();
                   sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                    if (sn.GeoZone.DsGeoDetails != null &&sn.GeoZone.DsGeoDetails.Tables[tableName] != null &&sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count > 0)
                    {
                        if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                        {

                            DataRow rowItem1 =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[0];
                            DataRow rowItem2 =sn.GeoZone.DsGeoDetails.Tables[tableName].Rows[1];

                            VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                            VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                            VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);


                            map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd()));

                        }
                        else //Polygon
                        {

                            GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count];
                            int i = 0;
                            foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[tableName].Rows)
                            {
                                points[i] = new GeoPoint(Convert.ToDouble(rowItem["Latitude"]),
                                    Convert.ToDouble(rowItem["Longitude"]));
                                i++;
                            }
                            // TODO: put proper severity
                            map.Polygons.Add(new PoligonIcon(points, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd(), true));


                        }
                    }
                }
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
