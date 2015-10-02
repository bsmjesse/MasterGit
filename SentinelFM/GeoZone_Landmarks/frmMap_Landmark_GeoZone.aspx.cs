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
using System.IO;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Configuration;
namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmMap_Landmark_GeoZone.
    /// </summary>
    public partial class frmMap_Landmark_GeoZone : SentinelFMBasePage
    {
        protected System.Web.UI.HtmlControls.HtmlForm Form2;
        protected System.Web.UI.HtmlControls.HtmlForm Configuration;
        protected System.Web.UI.HtmlControls.HtmlForm frmMap;
        
        public ClientMapProxy map;
        

        protected ServerDBOrganization.DBOrganization dbo;
        public int imageW = 655;
        public int imageH = 361;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
Response.Redirect("../MapNew/frmMap_Landmark_GeoZone.aspx?Landmark_GeoZone=1");

                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);


                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;

                }

                // create ClientMapProxy only for mapping
                map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
                if (map == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    return;
                }
                map.MapWidth = imageW;
                map.MapHeight = imageH;

                if (sn.GeoZone.ImgPath == "")
                   sn.GeoZone.ImgPath = sn.Map.DefaultImgPath;

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMap_Landmark_Zone, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    chkShowLandmark.Checked = sn.Map.ShowLandmark;
                    chkShowGeoZone.Checked = sn.Map.ShowGeoZone;
                    //Clear Tooltips

                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

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

                    //Load Geozone

                    DsGeoZone_Fill();


                    // Redraw map by coorinates
                    if ((CoordInX != null) && (CoordInX != "0"))
                    {
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
                        return;
                    }

                    if (((sn.DsLandMarks != null) &&
                        (sn.DsLandMarks.Tables.Count > 0) &&
                        (sn.DsLandMarks.Tables[0].Rows.Count > 0) && sn.Map.ShowLandmark == true) ||

                        ((sn.GeoZone.DsGeoZone != null) &&
                        (sn.GeoZone.DsGeoZone.Tables.Count > 0) &&
                        (sn.GeoZone.DsGeoZone.Tables[0].Rows.Count > 0) && sn.Map.ShowGeoZone == true))

                        LoadVehiclesMap();
                    else
                        LoadDefaultMap();
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
            this.btnZoomIn.Click += new System.Web.UI.ImageClickEventHandler(this.cmdZoomIn_Click);
            this.btnZoomOut.Click += new System.Web.UI.ImageClickEventHandler(this.cmdZoomOut_Click);
            this.btnMoveWest.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveWest_Click);
            this.btnMoveSouth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveSouth_Click);
            this.btnMoveNorth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveNorth_Click);
            this.btnMoveEast.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveEast_Click);

        }
        #endregion

       protected void btnZoomIn_Click(object sender, System.EventArgs e)
        {
            this.btnCountryLevel.ForeColor = Color.White;
            this.btnMaxZoom.ForeColor = Color.White;
            this.btnRegionLevel1.ForeColor = Color.White;
            this.btnRegionLevel2.ForeColor = Color.White;
            this.btnStreetLevel1.ForeColor = Color.White;
            this.btnStreetLevel2.ForeColor = Color.White;

            ZoomInMap(true, 2);

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

       protected void btnZoomOut_Click(object sender, System.EventArgs e)
        {
            this.btnCountryLevel.ForeColor = Color.White;
            this.btnMaxZoom.ForeColor = Color.White;
            this.btnRegionLevel1.ForeColor = Color.White;
            this.btnRegionLevel2.ForeColor = Color.White;
            this.btnStreetLevel1.ForeColor = Color.White;
            this.btnStreetLevel2.ForeColor = Color.White;


            ZoomInMap(false, 2);

        }

       protected void btnMoveNorth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.North);
        }

       protected void btnMoveWest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.West);
        }

       protected void btnMoveSouth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.South);
        }

       protected void btnMoveEast_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.East);
        }

        private void ZoomInMap(bool zoomIn, int level)
        {

            for (int i = 0; i < level; i++)
            {
                RetrievesMapStateFromViewState(map);
                map.Zoom(zoomIn);
                sn.Map.SavesMapStateToViewState(sn, map);
            }
            ZoomInMap(zoomIn);
        }

        private void ZoomInMap(bool zoomIn)
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;

                if ((sn == null) || (sn.UserName == ""))
                {

                    RedirectToLogin();
                }

                // Retrieves Map from GeoMicro
                // restore map states
                RetrievesMapStateFromViewState(map);
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




                //				if ((map.MapCenterLongitude==0) && (map.MapCenterLatitude==0)) 
                //				{
                //					LoadDefaultMap();
                //					return;
                //				}


               sn.GeoZone.ImgPath = url;
                sn.Map.SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(map);
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
                {
                    RedirectToLogin();
                }

                // Retrieves Map from GeoMicro
                // restore map states
                RetrievesMapStateFromViewState(map);

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




                //				if ((map.MapCenterLongitude==0) && (map.MapCenterLatitude==0)) 
                //				{
                //					LoadDefaultMap();
                //					return;
                //				}


               sn.GeoZone.ImgPath = url;
                sn.Map.SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(map);
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


        private void LoadVehiclesMap()
        {
            try
            {

                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.DrawLandmarks = true;
                map.DrawGeoZones = true;
                map.DrawPolygons = true;


                RetrievesMapStateFromViewState(map);


                if ((!sn.Map.ShowGeoZone) && (!sn.Map.ShowLandmark))
                {
                    LoadDefaultMap();
                    return;
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



               sn.GeoZone.ImgPath = url;

                sn.Map.SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(map);
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


        private void LoadDefaultMap()
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;

                // Set default map coordinates
                string url = map.GetDefaultMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

               sn.GeoZone.ImgPath = url;
                map.DrawAllVehicles = false;
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }


        private void MoveMap(MapPanDirection mapDirection)
        {
            try
            {
                if ((sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                }

                // Retrieves Map from GeoMicro
                // restore map states
                RetrievesMapStateFromViewState(map);

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




                //				if ((map.MapCenterLongitude==0) && (map.MapCenterLatitude==0))
                //				{
                //					LoadDefaultMap();
                //					return;
                //				}


               sn.GeoZone.ImgPath = url;
                sn.Map.SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(map);
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


        private void RedrawMapByXY(string CoordInX, string CoordInY, string CoordEndX, string CoordEndY)
        {
            try
            {

                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;

                if (sn.Map.ShowLandmark)
                {
                    if ((sn.DsLandMarks != null) &&
                        (sn.DsLandMarks.Tables.Count > 0) &&
                        (sn.DsLandMarks.Tables[0].Rows.Count > 0))
                    {
                        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                        {
                            if (sn.Map.ShowLandmarkRadius)
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                            else
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                        }
                    }

                }

                if (sn.Map.ShowGeoZone)
                {
                    if ((sn.GeoZone.DsGeoZone != null) &&
                        (sn.GeoZone.DsGeoZone.Tables.Count > 0) &&
                        (sn.GeoZone.DsGeoZone.Tables[0].Rows.Count > 0))
                    {
                        DrawGeoZones();
                    }
                }



                map.MapCenterLongitude = sn.Map.XInCoord;
                map.MapCenterLatitude = sn.Map.YInCoord;
                map.MapScale = sn.Map.MapScale;
                map.ImageDistance = sn.Map.ImageDistance;
                map.SouthWestCorner = sn.Map.SouthWestCorner;
                map.NorthEastCorner = sn.Map.NorthEastCorner;


                if (CoordInX != "0")
                {

                    if ((CoordEndX == CoordInX) || (Convert.ToDouble(CoordEndX) - Convert.ToDouble(CoordInX)) < 5 || (Convert.ToDouble(CoordInX) - Convert.ToDouble(CoordEndX)) > 5)
                    {
                        VLF.MAP.GeoPoint geopoint = map.ConvertPxToLatLon(new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordInY)));
                        map.MapCenterLongitude = geopoint.Longitude;
                        map.MapCenterLatitude = geopoint.Latitude;

                        if (sn.Map.SelectedZoomLevelType == 1)
                            map.Zoom(true);

                        if (sn.Map.SelectedZoomLevelType == 2)
                            map.Zoom(false);

                    }
                    else
                    {
                        if (!map.ResizeMap(new ImagePoint(Convert.ToInt32(CoordEndX), Convert.ToInt32(CoordInY)),
                            new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordEndY))))
                        {
                            return;
                        }
                    }

                }


                map.DrawGeoZones = false;
                map.DrawPolygons = false;
                map.DrawLandmarks = false;

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




                //				if ((map.MapCenterLongitude==0) && (map.MapCenterLatitude==0)) 
                //				{
                //					LoadDefaultMap();
                //					return;
                //				}


                if (url != "")
                   sn.GeoZone.ImgPath = url;

                sn.Map.SavesMapStateToViewState(sn, map);
                CreateVehiclesTooltip(map);
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

        private void CreateVehiclesTooltip(ClientMapProxy map)
        {

            try
            {
                sn.Map.VehiclesToolTip = "";
                sn.Map.VehiclesMappings = "";
                string UnitOfMes = sn.User.UnitOfMes == 1 ? "km/h" : "mi/h";

                //Table Vehicles with coordinates
                DataTable tblToolTips = new DataTable();
                DataRow tblRow;

                DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
                tblToolTips.Columns.Add(colDescription);
                DataColumn colX = new DataColumn("colX", Type.GetType("System.Double"));
                tblToolTips.Columns.Add(colX);
                DataColumn colY = new DataColumn("colY", Type.GetType("System.Double"));
                tblToolTips.Columns.Add(colY);



                if (sn.Map.ShowLandmark)
                {
                    //Create Landmark Tooltip description
                    for (int i = 0; i < map.Landmarks.Count; i++)
                    {


                        if ((map.Landmarks[i].X != -1) && (map.Landmarks[i].Y != -1))
                        {
                            if (sn.Map.VehiclesToolTip.Length == 0)
                                sn.Map.VehiclesToolTip += "<B><FONT style='COLOR: #000066'>" + "<FONT style='TEXT-DECORATION: underline' >" + "Landmark:" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT></B><BR>\",";
                            else
                                sn.Map.VehiclesToolTip += "\"<B><FONT style='COLOR: #000066'>" + "<FONT style='TEXT-DECORATION: underline' >" + "Landmark:" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT></B><BR>\",";


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
                        this.btnCountryLevel.ForeColor = Color.White;
                        this.btnMaxZoom.ForeColor = Color.Yellow;
                        this.btnRegionLevel1.ForeColor = Color.White;
                        this.btnRegionLevel2.ForeColor = Color.White;
                        this.btnStreetLevel1.ForeColor = Color.White;
                        this.btnStreetLevel2.ForeColor = Color.White;
                    }
                    break;

                case VLF.MAP.MapZoomLevel.StreetLevelNorm5:
                case VLF.MAP.MapZoomLevel.StreetLevelNorm6:
                case VLF.MAP.MapZoomLevel.StreetLevelLow:
                case VLF.MAP.MapZoomLevel.StreetLevelLow2:
                    {
                        this.btnCountryLevel.ForeColor = Color.White;
                        this.btnMaxZoom.ForeColor = Color.White;
                        this.btnRegionLevel1.ForeColor = Color.White;
                        this.btnRegionLevel2.ForeColor = Color.White;
                        this.btnStreetLevel1.ForeColor = Color.Yellow;
                        this.btnStreetLevel2.ForeColor = Color.White;
                    }
                    break;

                case VLF.MAP.MapZoomLevel.StreetLevelMin:
                case VLF.MAP.MapZoomLevel.StreetLevelMin2:
                case VLF.MAP.MapZoomLevel.RegionLevelMax:
                case VLF.MAP.MapZoomLevel.RegionLevelMax2:
                case VLF.MAP.MapZoomLevel.RegionLevelHigh:
                case VLF.MAP.MapZoomLevel.RegionLevelHigh2:
                    {

                        this.btnCountryLevel.ForeColor = Color.White;
                        this.btnMaxZoom.ForeColor = Color.White;
                        this.btnRegionLevel1.ForeColor = Color.White;
                        this.btnRegionLevel2.ForeColor = Color.White;
                        this.btnStreetLevel1.ForeColor = Color.White;
                        this.btnStreetLevel2.ForeColor = Color.Yellow;
                    }
                    break;



                case VLF.MAP.MapZoomLevel.RegionLevelNorm:
                case VLF.MAP.MapZoomLevel.RegionLevelNorm2:
                case VLF.MAP.MapZoomLevel.RegionLevelLow:
                case VLF.MAP.MapZoomLevel.RegionLevelLow2:
                    {

                        this.btnCountryLevel.ForeColor = Color.White;
                        this.btnMaxZoom.ForeColor = Color.White;
                        this.btnRegionLevel1.ForeColor = Color.Yellow;
                        this.btnRegionLevel2.ForeColor = Color.White;
                        this.btnStreetLevel1.ForeColor = Color.White;
                        this.btnStreetLevel2.ForeColor = Color.White;

                    }
                    break;




                case VLF.MAP.MapZoomLevel.RegionLevelMin:
                case VLF.MAP.MapZoomLevel.RegionLevelMin2:
                case VLF.MAP.MapZoomLevel.CountryLevelMax:
                case VLF.MAP.MapZoomLevel.CountryLevelMax2:
                case VLF.MAP.MapZoomLevel.CountryLevelHigh:
                case VLF.MAP.MapZoomLevel.CountryLevelHigh2:
                    {
                        this.btnCountryLevel.ForeColor = Color.White;
                        this.btnMaxZoom.ForeColor = Color.White;
                        this.btnRegionLevel1.ForeColor = Color.White;
                        this.btnRegionLevel2.ForeColor = Color.Yellow;
                        this.btnStreetLevel1.ForeColor = Color.White;
                        this.btnStreetLevel2.ForeColor = Color.White;
                    }
                    break;

                case VLF.MAP.MapZoomLevel.CountryLevelNorm:
                case VLF.MAP.MapZoomLevel.CountryLevelNorm2:
                case VLF.MAP.MapZoomLevel.CountryLevelLow:
                case VLF.MAP.MapZoomLevel.CountryLevelLow2:
                case VLF.MAP.MapZoomLevel.CountryLevelMin:
                case VLF.MAP.MapZoomLevel.CountryLevelMin2:
                    {
                        this.btnCountryLevel.ForeColor = Color.Yellow;
                        this.btnMaxZoom.ForeColor = Color.White;
                        this.btnRegionLevel1.ForeColor = Color.White;
                        this.btnRegionLevel2.ForeColor = Color.White;
                        this.btnStreetLevel1.ForeColor = Color.White;
                        this.btnStreetLevel2.ForeColor = Color.White;
                    }
                    break;

                default:
                    break;

            }

        }



        protected void cmdZoomOut_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 2;
        }

        protected void cmdZoomIn_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 1;
        }



        protected void cmdZoomIn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 1;
            ZoomInMap(true, 2);
        }

        protected void cmdZoomOut_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 2;
            ZoomInMap(false, 2);
        }

        protected void chkShowLandmark_CheckedChanged(object sender, System.EventArgs e)
        {
            sn.Map.ShowLandmark = chkShowLandmark.Checked;
            sn.Map.XInCoord = 0;
            sn.Map.YInCoord = 0;
            LoadVehiclesMap();
        }



        protected void cmdLandmark_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmLandmark.aspx");
        }


        private void DsGeoZone_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsGeoZone = new DataSet();
                
                string xml = "";
                dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, "", ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, "", ref xml), true))
                    {
                       sn.GeoZone.DsGeoZone = null;
                    }

                if (xml == "")
                {
                   sn.GeoZone.DsGeoZone = null;
                    return;
                }



                strrXML = new StringReader(xml);
                dsGeoZone.ReadXml(strrXML);



                //// Show SevirityName
                //DataColumn dc = new DataColumn();
                //dc.ColumnName = "SeverityName";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsGeoZone.Tables[0].Columns.Add(dc);



                // Show DirectionName
                DataColumn dc = new DataColumn();
                dc.ColumnName = "DirectionName";
                dc.DataType = Type.GetType("System.String");
                dc.DefaultValue = "";
                dsGeoZone.Tables[0].Columns.Add(dc);




                short enumId = 0;
                string[] Sen = new string[1];

                foreach (DataRow rowItem in dsGeoZone.Tables[0].Rows)
                {
                    //enumId = Convert.ToInt16(rowItem["SeverityId"]);
                    //rowItem["SeverityName"] = Enum.GetName(typeof(VLF.CLS.Def.Enums.AlarmSeverity), (VLF.CLS.Def.Enums.AlarmSeverity)enumId);


                    enumId = Convert.ToInt16(rowItem["Type"]);
                    rowItem["DirectionName"] = Enum.GetName(typeof(VLF.CLS.Def.Enums.GeoZoneDirection), (VLF.CLS.Def.Enums.GeoZoneDirection)enumId);

                }


               sn.GeoZone.DsGeoZone = dsGeoZone;


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

        protected void chkShowGeoZone_CheckedChanged(object sender, System.EventArgs e)
        {
            sn.Map.ShowGeoZone = chkShowGeoZone.Checked;
            sn.Map.XInCoord = 0;
            sn.Map.YInCoord = 0;
            sn.Map.DrawAllVehicles = true;
            LoadVehiclesMap();
        }

        protected void cmdGeoZone_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmGeoZone.aspx");
        }

        private void cmdCenterMap_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            sn.Map.DrawAllVehicles = true;
            sn.Map.ShowLandmark = chkShowLandmark.Checked;
            LoadVehiclesMap();
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

        protected void cmdVehicleGeoZone_Click(object sender, System.EventArgs e)
        {
            sn.Cmd.BoxId = 0;
            Response.Redirect("frmVehicleGeoZoneAss.htm");
        }

        


        private void RetrievesMapStateFromViewState(ClientMapProxy map)
        {
            try
            {
                map.Landmarks.Clear();

                // add landmarks

                if (sn.Map.ShowLandmark)
                {
                    if ((sn.DsLandMarks != null) &&
                        (sn.DsLandMarks.Tables.Count > 0) &&
                        (sn.DsLandMarks.Tables[0].Rows.Count > 0))
                    {
                        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                        {
                            if (sn.Map.ShowLandmarkRadius)
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                            else
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                        }
                    }
                }


                if (sn.Map.ShowGeoZone)
                {
                    if ((sn.GeoZone.DsGeoZone != null) &&
                        (sn.GeoZone.DsGeoZone.Tables.Count > 0) &&
                        (sn.GeoZone.DsGeoZone.Tables[0].Rows.Count > 0))
                    {
                        DrawGeoZones();
                    }
                }


                if ((sn.Map.XInCoord != 0) && (sn.Map.YInCoord != 0))
                {
                    map.Zoom(VLF.MAP.MapZoomLevel.CountryLevelMin);
                    map.MapCenterLongitude = Convert.ToDouble(sn.Map.XInCoord);
                    map.MapCenterLatitude = Convert.ToDouble(sn.Map.YInCoord);
                    map.ImageDistance = sn.Map.ImageDistance;

                    //map.DrawAllVehicles = Convert.ToBoolean(sn.Map.DrawAllVehicles);
                    if (sn.Map.MapScale != 0)
                        map.MapScale = Convert.ToDouble(sn.Map.MapScale);
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                VLF.ERR.LOG.LogFile(ConfigurationSettings.AppSettings["LogFolder"], "SentinelFM", "Error:" + Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name);
            }

        }



        private void DrawGeoZones()
        {
            try
            {

                foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
                {


                    StringReader strrXML = null;
                    DataSet dsGeoZoneDetails = new DataSet();
                    
                    string xml = "";
                    dbo = new ServerDBOrganization.DBOrganization();

                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dr["GeoZoneId"]), ref xml), false))
                        if (objUtil.ErrCheck(dbo.GetOrganizationGeozoneInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt16(dr["GeoZoneId"]), ref xml), true))
                        {
                            continue;
                        }

                    if (xml == "")
                        continue;




                    strrXML = new StringReader(xml);
                    dsGeoZoneDetails.ReadXml(strrXML);
                   sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                    if (dsGeoZoneDetails.Tables[0].Rows.Count > 0)
                    {
                        if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                        {

                            DataRow rowItem1 = dsGeoZoneDetails.Tables[0].Rows[0];
                            DataRow rowItem2 = dsGeoZoneDetails.Tables[0].Rows[1];

                            VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                            VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                            VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);

                            map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2, (VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(dr["SeverityId"]), (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(dr["Type"]), dr["GeoZoneName"].ToString().TrimEnd()));

                        }
                        else //Polygon
                        {

                            GeoPoint[] points = new GeoPoint[dsGeoZoneDetails.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow rowItem in dsGeoZoneDetails.Tables[0].Rows)
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

    }
}
