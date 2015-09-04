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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Text.RegularExpressions;


namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmVehicleMap.
    /// </summary>
    public partial class frmLSDmap : SentinelFMBasePage
    {
        
        public VLF.MAP.LSDProxy map;
        public int imageW = 600;
        public int imageH = 325;
        public int AutoRefreshTimer = 60000;
        



        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                try
                {
                    if (Request.QueryString["clientWidth"] != null)
                    {
                        sn.Map.ScreenWidth = Convert.ToInt32(Request.QueryString["clientWidth"]);
                        imageW = Convert.ToInt32(sn.Map.ScreenWidth) - 270;
                        sn.Map.MapResize = true;

                    }
                    else
                    {
                        if (sn.User.ScreenWidth != 0)
                        {
                            if (sn.Map.MapResize)
                                imageW = Convert.ToInt32(sn.Map.ScreenWidth) - 270;
                            else
                                imageW = Convert.ToInt32(sn.User.ScreenWidth) - 480;
                        }
                    }
                }
                catch
                {
                    imageW = Convert.ToInt32(sn.User.ScreenWidth) - 480;
                }


                //imageW = Convert.ToInt32(sn.User.ScreenWidth * 0.75);

                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;

                // create ClientMapProxy only for mapping
                string mapExternalPath = ConfigurationSettings.AppSettings["GeoMicroMapExternalPath"];
                string mapInternalPath = ConfigurationSettings.AppSettings["GeoMicroMapInternalPath"];
                map = new VLF.MAP.LSDProxy(imageW, imageH, mapExternalPath, mapExternalPath);

                
                if (map == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }
                map.MapWidth = imageW;
                map.MapHeight = imageH;


                if (sn.Map.DisabledLSDLayers != null)
                    map.DisableLayers(sn.Map.DisabledLSDLayers);


                if (!Page.IsPostBack)
                {
                    DateTime dt = System.DateTime.Now;

                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map started ->  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));


                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleMapForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    GuiSecurity(this);
                    //Clear Tooltips
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    if (sn.Map.DisabledLSDLayers != null)
                    {
                        for (int i = 0; i < sn.Map.DisabledLSDLayers.Length; i++)
                            for (int chkItems = 0; chkItems < 4; chkItems++)
                                if (Convert.ToInt16(chkLayers.Items[chkItems].Value) == Convert.ToInt16(sn.Map.DisabledLSDLayers[i]))
                                    chkLayers.Items[chkItems].Selected = false;
                    }


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



                    TimeSpan currDuration;
                    // Redraw map by coorinates
                    if ((CoordInX != null) && (CoordInX != "0"))
                    {
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);

                        currDuration = new TimeSpan(System.DateTime.Now.Ticks - dt.Ticks);
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map ended <- Duration: " + currDuration.TotalSeconds.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                        return;
                    }

                    // Redraw map on "Map It" and autorefresh	
                    if ((sn != null) && (sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
                        LoadVehiclesMap();
                    else
                        LoadDefaultMap();

                    currDuration = new TimeSpan(System.DateTime.Now.Ticks - dt.Ticks);
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles map ended <- Duration: " + currDuration.TotalSeconds.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                }

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
                RetrievesMapStateFromViewState(sn, map);

                map.Zoom(zoomIn);
                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);



                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {

                    LoadDefaultMap();
                    return;

                }


                sn.Map.ImgPath = url;
                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
                RetrievesMapStateFromViewState(sn, map);

                map.Zoom(mapZoomLevel);
                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);




                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;

                }



                if (url != "")
                    sn.Map.ImgPath = url;

                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
                SavesMapStateToViewState(sn, map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
                DrawLadmarks();
                if ((sn != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
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
                                if (dr["Speed"].ToString() != "0")
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
                    RetrievesMapStateFromViewState(sn, map);
                }

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);



                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {

                    LoadDefaultMap();
                    return;

                }


                if (url != "")
                    sn.Map.ImgPath = url;

                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
                RetrievesMapStateFromViewState(sn, map);
                map.Pan(mapDirection);
                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);







                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;

                }



                if (url != "")
                    sn.Map.ImgPath = url;

                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void btnMoveNorth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
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
                DrawLadmarks();



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

                                if (dr["Speed"].ToString() != "0")
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
                            CreateVehiclesTooltip(sn, map);
                            SetColorZoomLevel(map);
                            return;
                        }
                    }


                }

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);






                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;
                }


                if (url != "")
                    sn.Map.ImgPath = url;

                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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




       

        private void SetColorZoomLevel(LSDProxy map)
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
                        this.btnMaxZoom.BackColor = Color.Gray;
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
                    RetrievesMapStateFromViewState(sn, map);
                    map.Zoom(zoomIn);
                    SavesMapStateToViewState(sn, map);
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

        protected void cmdDisplay_Click(object sender, EventArgs e)
        {
            try
            {
/*
                
                Regex meridian=new Regex("[4-5-6]");
                if  (!meridian.IsMatch(this.txtMeridian.Text))
                    sn.MessageText = "Invalid meridian info. (Meridian: 4,5 and 6) ";


                Regex range = new Regex("[1-30]");
                if (!range.IsMatch(this.txtRange.Text))
                    sn.MessageText += Environment.NewLine  + "Invalid range info. (Range: 1-30)";

                Regex township = new Regex("[1-126]");
                if (!township.IsMatch(this.txtTownship.Text))
                    sn.MessageText += Environment.NewLine + "Invalid township info. (Township: 1-126)";


                Regex section = new Regex("[1-36]");
                if (!section.IsMatch(this.txtSection.Text))
                    sn.MessageText += Environment.NewLine + "Invalid section info. (Section: 1-36)";
*/

                sn.MessageText = "";

                try
                {
                    int meridian = Convert.ToInt16(this.txtMeridian.Text);
                    if ( meridian <4  || meridian > 6) 
                        sn.MessageText = "Invalid meridian info. (Meridian: 4,5 and 6) ";
                }
                catch (Exception ex)
                {
                    sn.MessageText = "Invalid meridian info. (Meridian: 4,5 and 6) ";
                }



                try
                {
                    int range = Convert.ToInt16(this.txtRange.Text);
                    if (range<1   || range > 30)
                        sn.MessageText += Environment.NewLine + "Invalid range info. (Range: 1-30)";
                }
                catch (Exception ex)
                {
                    sn.MessageText += Environment.NewLine + "Invalid range info. (Range: 1-30)";

                }


                try
                {
                    int township = Convert.ToInt16(this.txtTownship.Text);
                    if (township<1 || township > 126)
                        sn.MessageText += Environment.NewLine + "Invalid township info. (Township: 1-126)";
                }
                catch (Exception ex)
                {
                    sn.MessageText += Environment.NewLine + "Invalid township info. (Township: 1-126)";

                }


                try
                {
                    int section = Convert.ToInt16(this.txtSection.Text);
                    if (section<1 || section > 36)
                        sn.MessageText += Environment.NewLine + "Invalid section info. (Section: 1-36)";
                }
                catch (Exception ex)
                {
                    sn.MessageText += Environment.NewLine + "Invalid section info. (Section: 1-36)";

                }


                if (sn.MessageText != "")
                {
                    ShowErrorMessage();
                    return; 
                }
 
                VLF.MAP.LSDProxy geomap = new VLF.MAP.LSDProxy("");
                string address = VLF.MAP.LSDProxy.ToLSD(Convert.ToInt32(this.txtMeridian.Text), Convert.ToInt32(this.txtTownship.Text), Convert.ToInt32(this.txtRange.Text), Convert.ToInt32(this.txtSection.Text), this.cboQuarter.SelectedItem.Value);
                double lat = 0;
                double lot = 0;
                using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
                {
                    dbs.GetLatitudeLongitudeByLSD(sn.UserID,sn.SecId, address, ref lat, ref lot);
                }
                
                RetrievesMapStateFromViewState(sn, map);


                map.MapCenterLongitude = lot;
                map.MapCenterLatitude = lat;


                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                if (url != "")
                    sn.Map.ImgPath = url;

                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }
        protected void cmdUpdateView_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList disableLayersArr = new ArrayList();
                for (int i = 0; i < this.chkLayers.Items.Count; i++)
                {
                    if (!chkLayers.Items[i].Selected)
                        disableLayersArr.Add(Convert.ToInt32(chkLayers.Items[i].Value));
                }

                //VLF.MAP.LSDProxy geomap = new VLF.MAP.LSDProxy("");
                //if (chkLayers.Items.Count > 0)
                //{
                //    int[] disableLayers = ((int[])(disableLayersArr.ToArray(typeof(int))));
                //    geomap.DisableLayers(disableLayers);
                //}

                RetrievesMapStateFromViewState(sn, map);

                if (chkLayers.Items.Count > 0)
                {
                    map.DisableLayers(((int[])(disableLayersArr.ToArray(typeof(int)))));
                    sn.Map.DisabledLSDLayers = (int[]) disableLayersArr.ToArray(typeof(int));
                }
                else
                    sn.Map.DisabledLSDLayers = null;   

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                if (url != "")
                    sn.Map.ImgPath = url;

                SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        public void SavesMapStateToViewState(SentinelFMSession sn, LSDProxy map)
        {
            sn.Map.XInCoord = map.MapCenterLongitude;
            sn.Map.YInCoord = map.MapCenterLatitude;
            sn.Map.MapScale = map.MapScale;
            sn.Map.ImageDistance = map.ImageDistance;
            sn.Map.DrawAllVehicles = map.DrawAllVehicles;
            sn.Map.SouthWestCorner = map.SouthWestCorner;
            sn.Map.NorthEastCorner = map.NorthEastCorner;

        }



        public void RetrievesMapStateFromViewState(SentinelFMSession sn, LSDProxy map)
        {


            map.Vehicles.Clear();
            map.Landmarks.Clear();
            map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
            map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;

            //set Show checkboxes
            if ((sn.Map.DsFleetInfo != null) &&
                (sn.Map.DsFleetInfo.Tables.Count > 0) &&
                (sn.Map.DsFleetInfo.Tables[0].Rows.Count > 0))
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
                            //add vehicles
                            if (dr["Speed"].ToString() != "0")
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + dr["IconTypeName"].ToString().TrimEnd() + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["Description"].ToString().TrimEnd()));
                            else
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + dr["IconTypeName"].ToString().TrimEnd() + ".ico", dr["Description"].ToString().TrimEnd()));
                        }

                    }
                }
            }


            // add landmarks
            DrawLadmarks();

            map.MapCenterLongitude = Convert.ToDouble(sn.Map.XInCoord);
            map.MapCenterLatitude = Convert.ToDouble(sn.Map.YInCoord);
            map.MapScale = sn.Map.MapScale;
            map.SouthWestCorner = sn.Map.SouthWestCorner;
            map.NorthEastCorner = sn.Map.NorthEastCorner;


            map.ImageDistance = sn.Map.ImageDistance;

            if ((map.MapCenterLongitude == 0) && (map.MapCenterLatitude == 0))
            {
                map.DrawAllVehicles = true;
            }
            else
            {
                map.DrawAllVehicles = Convert.ToBoolean(sn.Map.DrawAllVehicles);
                map.MapScale = Convert.ToDouble(sn.Map.MapScale);
            }


        }

        public void CreateVehiclesTooltip(SentinelFMSession sn, LSDProxy map)
        {


            sn.Map.VehiclesToolTip = "";
            sn.Map.VehiclesMappings = "";


            //Table Vehicles with coordinates
            DataTable tblToolTips = new DataTable();
            DataRow tblRow;

            DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
            tblToolTips.Columns.Add(colDescription);
            DataColumn colX = new DataColumn("colX", Type.GetType("System.Double"));
            tblToolTips.Columns.Add(colX);
            DataColumn colY = new DataColumn("colY", Type.GetType("System.Double"));
            tblToolTips.Columns.Add(colY);

            //Create Vehicles Tooltip description
            for (int i = 0; i < map.Vehicles.Count; i++)
            {
                if ((map.Vehicles[i].X != -1) && (map.Vehicles[i].Y != -1))
                {

                    DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("Description='" + map.Vehicles[i].IconLabel.ToString().TrimEnd() + "'");
                    if (drArr == null || drArr.Length == 0)
                        continue;
                    DataRow rowItem = drArr[0];


                    //foreach(DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    //{
                    //   if (rowItem["Description"].ToString().TrimEnd()==map.Vehicles[i].IconLabel.ToString().TrimEnd())
                    //   {

                    //Tooltip description
                    if (sn.Map.VehiclesToolTip.Length == 0)
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"].ToString()) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            sn.Map.VehiclesToolTip += "<B>" + rowItem["Description"].ToString().TrimEnd() + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]: Position Old</B><BR> <FONT style='TEXT-DECORATION: underline'  > Address:</FONT>" + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > Status:</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() +  "<BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "<B>" + rowItem["Description"].ToString().TrimEnd() + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><BR> <FONT style='TEXT-DECORATION: underline'  > Address:</FONT>" + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > Status:</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() +  "<BR>\",";

                    }
                    else
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"].ToString()) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            sn.Map.VehiclesToolTip += "\"<B>" + rowItem["Description"].ToString().TrimEnd() + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]: Position Old</B><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > Status:</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + ", "  + "<BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "\"<B>" + rowItem["Description"].ToString().TrimEnd() + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > Status:</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + ", " +"<BR>\",";
                    }

                    if (map.Vehicles.Count > 0)
                    {
                        tblRow = tblToolTips.NewRow();
                        tblRow["Description"] = map.Vehicles[i].IconLabel;
                        tblRow["colX"] = map.Vehicles[i].X;
                        tblRow["colY"] = map.Vehicles[i].Y;
                        tblToolTips.Rows.Add(tblRow);
                    }
                    //      break;
                    //   }
                    //}
                }
            }



            if (sn.Map.ShowLandmark)
            {
                //Create Landmark Tooltip description
                for (int i = 0; i < map.Landmarks.Count; i++)
                {


                    if ((map.Landmarks[i].X != -1) && (map.Landmarks[i].Y != -1))
                    {
                        if (sn.Map.VehiclesToolTip.Length == 0)
                            sn.Map.VehiclesToolTip += "<FONT style='COLOR: #084f7f'>" + "<FONT style='TEXT-DECORATION: underline' >" + "Landmark:" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT><BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "\"<FONT style='COLOR: #084f7f'>" + "<FONT style='TEXT-DECORATION: underline' >" + "Landmark:" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT><BR>\",";


                        tblRow = tblToolTips.NewRow();
                        tblRow["Description"] = map.Landmarks[i].IconLabel;
                        tblRow["colX"] = map.Landmarks[i].X;
                        tblRow["colY"] = map.Landmarks[i].Y;
                        tblToolTips.Rows.Add(tblRow);
                    }
                }
            }


            if ((sn.Map.VehiclesToolTip.Length > 0) && (sn.Map.VehiclesToolTip.Substring(sn.Map.VehiclesToolTip.Length - 1, 1) == ","))
                sn.Map.VehiclesToolTip = sn.Map.VehiclesToolTip.Substring(0, sn.Map.VehiclesToolTip.Length - 2);


            //Create ToolTip Matrix
            System.Collections.SortedList Matrix = new System.Collections.SortedList();
            int x = 0;
            int y = 0;
            int key = 0;
            for (int i = 0; i < tblToolTips.Rows.Count; i++)
            {
                x = Convert.ToInt32(Convert.ToInt32(tblToolTips.Rows[i]["colX"]) / 10);
                y = Convert.ToInt32(Convert.ToInt32(tblToolTips.Rows[i]["colY"]) / 10);
                key = (++x) * 100 + (++y);
                if (!Matrix.ContainsKey(key))
                {
                    Matrix.Add(key, i + 1);
                }
                else
                {
                    Matrix[key] += "|" + (i + 1);
                }
            }


            double StartX = 0;
            double EndX = 0;
            double StartY = 0;
            double EndY = 0;
            string strVehiclesList = "";

            foreach (int MatrixKey in Matrix.Keys)
            {
                x = (MatrixKey / 100 - 1) * 10;
                y = (MatrixKey % 100 - 1) * 10;

                StartX = x - 8;
                EndX = x + 8;
                StartY = y - 8;
                EndY = y + 8;

                strVehiclesList = Matrix[MatrixKey].ToString();

                sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
            }

        }


        private void ShowErrorMessage()
        {
            //Create pop up message
            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='Message';";
            strUrl = strUrl + " var w=370;";
            strUrl = strUrl + " var h=50;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);}";

            strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";

            Response.Write(strUrl);
        }

        private void DrawLadmarks()
        {
            if (sn.Map.ShowLandmark)
            {
                if ((sn.DsLandMarks != null) &&
                    (sn.DsLandMarks.Tables.Count > 0) &&
                    (sn.DsLandMarks.Tables[0].Rows.Count > 0))
                {

                    foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                    {

                        if (sn.Map.ShowLandmarkname && sn.Map.ShowLandmarkRadius)
                            map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                        else if (sn.Map.ShowLandmarkname)
                            map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                        else if (sn.Map.ShowLandmarkRadius)
                            map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", ""));
                        else
                            map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", ""));
                    }


                    //foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                    //{
                    //    if (sn.Map.ShowLandmarkname)
                    //        map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                    //    else
                    //        map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", ""));

                    //}
                }
            }
        }
    }
}
