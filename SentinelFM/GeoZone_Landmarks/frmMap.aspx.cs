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
using System.IO;

namespace SentinelFM.Map_Geo_Land_Zone
{
    /// <summary>
    /// Summary description for frmMap.
    /// </summary>
    public partial class frmMap : SentinelFMBasePage
    {
        
        public ClientMapProxy map;
        public int imageW = 655;
        public int imageH = 361;
        protected System.Web.UI.WebControls.ImageButton cmdSetGeoZone;

        protected System.Web.UI.WebControls.Button cmdCompleteGeoZone;
        


        protected void Page_Load(object sender, System.EventArgs e)
        {


            if (Request.QueryString["FormName"] != null)
            {
                if (Request.QueryString["FormName"].ToString().ToLower() == "landmark")
                {
                    Response.Redirect("../MapNew/frmDrawMap.aspx?FormName=Landmark");
                }
                if (Request.QueryString["FormName"].ToString().ToLower() == "geozone")
                {
                    Response.Redirect("../MapNew/frmDrawGeoZone.aspx?FormName=Geozone");
                }

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

            string CoordInX = Request.QueryString["CoordInX"];
            string CoordInY = Request.QueryString["CoordInY"];
            string CoordEndX = Request.QueryString["CoordEndX"];
            string CoordEndY = Request.QueryString["CoordEndY"];

            if (Request.QueryString["FormName"] != null)
                sn.Landmark.RefreshFormName = Request.QueryString["FormName"];


            if (sn.GeoZone.ImgConfPath == "")
                LoadDefaultMap();


            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMapForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);
                if (sn.Landmark.RefreshFormName == "Geozone")
                {

                    if (sn.GeoZone.AddMode)
                    {
                        this.cmdShowEditGeoZone.Visible = false;
                        this.tblGeoZone.Visible = true;
                    }

                    if (sn.GeoZone.ShowEditGeoZoneTable)
                        this.tblGeoZone.Visible = true;
                    else
                        this.tblGeoZone.Visible = false;


                    CboLandmarks_Fill();
                    if (sn.GeoZone.SetGeoZone)
                    {
                        this.cmdDrawGeoZone.BorderStyle = BorderStyle.Inset;
                        this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_CompleteDrawingGeozone");
                        this.GeoZoneOptions.Enabled = false;
                        this.lblMessage.Visible = true;

                        if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Polygon)
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_ClickToCreateGeozone");
                        else if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DrawDesiredRectangle");


                    }
                    else
                    {

                        cmdDrawGeoZone.BorderStyle = BorderStyle.Outset;
                        this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_StartDrawing");
                        this.lblMessage.Visible = true;

                        if (sn.GeoZone.ShowEditGeoZoneTable)
                            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NavigateMapToGeozone");

                        if (sn.User.MapEngine[0].MapEngineID == VLF.MAP.MapType.GeoMicro)
                            this.GeoZoneOptions.Enabled = false;
                        else
                            this.GeoZoneOptions.Enabled = true;
                    }

                    GeoZoneOptions.SelectedIndex = GeoZoneOptions.Items.IndexOf(GeoZoneOptions.Items.FindByValue(Convert.ToInt16(sn.GeoZone.GeozoneTypeId).ToString()));



                    // Redraw map by coorinates
                    if ((CoordInX == null) || (CoordInX == "0"))
                    {
                        if ((sn.GeoZone.DsGeoDetails != null) &&
                            (sn.GeoZone.DsGeoDetails.Tables.Count > 0))
                        {
                            DrawGeoZones();
                            map.DrawGeoZones = true;
                            map.DrawPolygons = true;

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


                           sn.GeoZone.ImgConfPath = url;
                            SavesMapStateToViewState(map);
                            SetColorZoomLevel(map);

                        }
                    }
                    else
                    {
                        DrawGeoZones();
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
                    }
                }
                else
                {


                    this.cmdShowEditGeoZone.Visible = false;
                    this.tblGeoZone.Visible = false;

                    if ((sn.Landmark.X == 0 && sn.Landmark.Y == 0) && ((CoordInX == null) || (CoordInX == "0")))
                    {
                        LoadDefaultMap();
                        return;
                    }
                    else if ((sn.Landmark.X != 0 && sn.Landmark.Y != 0) && ((CoordInX == null) || (CoordInX == "0")))
                    {
                        RetrievesMapStateFromViewState(map);
                        map.MapCenterLongitude = sn.Landmark.X;
                        map.MapCenterLatitude = sn.Landmark.Y;
                        map.Landmarks.Clear();
                        map.DrawLandmarks = true;
                        map.Landmarks.Add(new MapIcon(sn.Landmark.Y, sn.Landmark.X, "NewLandmark.ico", ""));

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



                       sn.GeoZone.ImgConfPath = url;
                        SavesMapStateToViewState(map);
                        SetColorZoomLevel(map);
                    }
                    else if ((CoordInX != null) || (CoordInX != "0"))
                    {
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
                    }
                }
            }
        }

        private void ZoomInMap(bool zoomIn)
        {
            try
            {

                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;


                // Retrieves Map from GeoMicro
                // restore map states
                RetrievesMapStateFromViewState(map);
                map.Zoom(zoomIn);


                if (sn.Landmark.RefreshFormName == "Geozone")
                    DrawGeoZones();
                else
                    DrawLandMark();


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


               sn.GeoZone.ImgConfPath = url;
                SavesMapStateToViewState(map);
                SetColorZoomLevel(map);
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void cmdClearMap_Click(object sender, System.EventArgs e)
        {
            ViewState["ConfirmDelete"] = "1";
           sn.GeoZone.SetGeoZone = false;
            this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_StartDrawing");
            this.lblMessage.Visible = true;
            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_NavigateMapToGeozone");

            if (sn.User.MapEngine[0].MapEngineID == VLF.MAP.MapType.GeoMicro)
                this.GeoZoneOptions.Enabled = false;
            else
                this.GeoZoneOptions.Enabled = true;

            this.GeoZoneOptions.Enabled = true;

            ClearMap();
        }

        protected void GeoZoneOptions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
           sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(this.GeoZoneOptions.SelectedItem.Value);
        }

        private void ZoomMap(MapZoomLevel mapZoomLevel)
        {
            try
            {

                // Retrieves Map from GeoMicro 
                RetrievesMapStateFromViewState(map);

                map.Zoom(mapZoomLevel);



                if (sn.Landmark.RefreshFormName == "Geozone")
                    DrawGeoZones();
                else
                    DrawLandMark();


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




               sn.GeoZone.ImgConfPath = url;
                SavesMapStateToViewState(map);
                SetColorZoomLevel(map);
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
                map.Vehicles.DrawLabels = false;

                // Set default map coordinates
                map.MapCenterLongitude = -95.6743;
                map.MapCenterLatitude = 47.1623611;
                map.Zoom(VLF.MAP.MapZoomLevel.CountryLevelLow2);

                string url = map.GetDefaultMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
               sn.GeoZone.ImgConfPath = url;
                

                map.DrawAllVehicles = false;
                sn.Map.VehiclesMappings = "";
                sn.Map.VehiclesToolTip = "";
                SavesMapStateToViewState(map);
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

                // Retrieves Map from GeoMicro
                // restore map states
                RetrievesMapStateFromViewState(map);

                map.Pan(mapDirection);

                if (sn.Landmark.RefreshFormName == "Geozone")
                    DrawGeoZones();
                else
                    DrawLandMark();


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


               sn.GeoZone.ImgConfPath = url;
                SavesMapStateToViewState(map);
                //CreateVehiclesTooltip(map);
                SetColorZoomLevel(map);
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
                map.DrawAllVehicles = false;

                if (sn.Landmark.RefreshFormName == "Geozone")
                    DrawGeoZones();


                map.MapCenterLongitude = sn.Map.XInCoord;
                map.MapCenterLatitude = sn.Map.YInCoord;
                map.MapScale = sn.Map.MapScale;
                map.ImageDistance = sn.Map.ImageDistance;

                map.SouthWestCorner = sn.Map.SouthWestCorner;
                map.NorthEastCorner = sn.Map.NorthEastCorner;

                if (CoordInX != "0")
                {

                    if (((CoordEndX == CoordInX) || (Convert.ToDouble(CoordEndX) - Convert.ToDouble(CoordInX)) < Convert.ToInt32(ConfigurationSettings.AppSettings["MaxGeoZonePoints"]) || (Convert.ToDouble(CoordInX) - Convert.ToDouble(CoordEndX)) > Convert.ToInt32(ConfigurationSettings.AppSettings["MaxGeoZonePoints"])) && (!sn.GeoZone.SetGeoZone))
                    {
                        VLF.MAP.GeoPoint geopoint = map.ConvertPxToLatLon(new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordInY)));
                        map.MapCenterLongitude = geopoint.Longitude;
                        map.MapCenterLatitude = geopoint.Latitude;
                        map.Landmarks.Clear();

                        if (sn.Landmark.RefreshFormName == "Landmark")
                        {

                            sn.Landmark.X = geopoint.Longitude;
                            sn.Landmark.Y = geopoint.Latitude;
                            map.Landmarks.Add(new MapIcon(Convert.ToDouble(geopoint.Latitude), Convert.ToDouble(geopoint.Longitude), "NewLandmark.ico", ""));
                        }

                        if (sn.Landmark.RefreshFormName == "Geozone")
                        {
                            if (sn.Map.SelectedZoomLevelType == 1)
                                map.Zoom(true);

                            if (sn.Map.SelectedZoomLevelType == 2)
                                map.Zoom(false);
                        }
                    }
                    else
                    {
                        if (sn.GeoZone.SetGeoZone)
                        {
                            sn.Map.SelectedZoomLevelType = 0;
                            if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                            {

                               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Clear();
                                VLF.MAP.GeoPoint geopoint1 = map.ConvertPxToLatLon(new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordInY)));
                                VLF.MAP.GeoPoint geopoint2 = map.ConvertPxToLatLon(new ImagePoint(Convert.ToInt32(CoordEndX), Convert.ToInt32(CoordEndY)));
                                VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);
                                map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2));

                                DataRow dr1 =sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                                dr1["Longitude"] = geopoint1.Longitude;
                                dr1["Latitude"] = geopoint1.Latitude;
                                dr1["SequenceNum"] =sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr1);

                                DataRow dr2 =sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                                dr2["Longitude"] = geopoint2.Longitude;
                                dr2["Latitude"] = geopoint2.Latitude;
                                dr2["SequenceNum"] =sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr2);

                            }
                            else
                            {

                                if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count == Convert.ToInt32(ConfigurationSettings.AppSettings["MaxGeoZonePoints"]))
                                {
                                    this.lblMessage.Text = Convert.ToInt32(ConfigurationSettings.AppSettings["MaxGeoZonePoints"]).ToString() + " " + (string)base.GetLocalResourceObject("lblMessage_Text_MaximumPointsError");
                                    return;
                                }

                                VLF.MAP.GeoPoint geopoint = map.ConvertPxToLatLon(new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordInY)));
                                DataRow dr =sn.GeoZone.DsGeoDetails.Tables[0].NewRow();
                                dr["Longitude"] = geopoint.Longitude;
                                dr["Latitude"] = geopoint.Latitude;
                                dr["SequenceNum"] =sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count + 1;
                               sn.GeoZone.DsGeoDetails.Tables[0].Rows.Add(dr);

                                map.MapCenterLongitude = geopoint.Longitude;
                                map.MapCenterLatitude = geopoint.Latitude;
                                DrawGeoZones();
                            }
                        }
                        else
                        {
                            if (!map.ResizeMap(new ImagePoint(Convert.ToInt32(CoordEndX), Convert.ToInt32(CoordInY)),
                            new ImagePoint(Convert.ToInt32(CoordInX), Convert.ToInt32(CoordEndY))))
                                return;

                            DrawLandMark();
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
                   sn.GeoZone.ImgConfPath = url;

                SavesMapStateToViewState(map);
                SetColorZoomLevel(map);
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


       protected void btnMoveNorth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.North);
        }

       protected void btnMoveWest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.West);
        }

       protected void btnMoveEast_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.East);
        }

       protected void btnMoveSouth_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.South);
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



        protected void cmdSave_Click(object sender, System.EventArgs e)
        {

            string strJavaScript = "";

           sn.GeoZone.SetGeoZone = false;

            if (sn.Landmark.RefreshFormName == "Landmark")
            {
                strJavaScript += "<script language='javascript'>";
                strJavaScript += " window.opener.document.forms[0].txtX.value=" + sn.Landmark.X.ToString() + ";";
                strJavaScript += " window.opener.document.forms[0].txtY.value=" + sn.Landmark.Y.ToString() + ";";
                strJavaScript += " self.close();</script>";
                Response.Write(strJavaScript);

            }
            if (sn.Landmark.RefreshFormName == "Geozone")
            {

                strJavaScript += "<script language='javascript'>";
                strJavaScript += " window.opener.document.forms[0].txtMapMessage.value='" + (string)base.GetLocalResourceObject("txtMapMessage_Value_OnWindowOpen") + "';";
                strJavaScript += " self.close();</script>";
                Response.Write(strJavaScript);

            }

        }

        protected void cmdCancel_Click(object sender, System.EventArgs e)
        {
            Response.Write("<script language='javascript'>window.close()</script>");
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
            this.btnZoomIn.Click += new System.Web.UI.ImageClickEventHandler(this.btnZoomIn_Click);
            this.btnZoomOut.Click += new System.Web.UI.ImageClickEventHandler(this.btnZoomOut_Click);
            this.btnMoveEast.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveEast_Click);
            this.btnMoveSouth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveSouth_Click);
            this.btnMoveNorth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveNorth_Click);
            this.btnMoveWest.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveWest_Click);

        }
        #endregion


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

       protected void btnZoomOut_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.btnZoomIn.BackColor = Color.Gray;
            this.btnCountryLevel.BackColor = Color.White;
            this.btnMaxZoom.BackColor = Color.White;
            this.btnRegionLevel1.BackColor = Color.White;
            this.btnRegionLevel2.BackColor = Color.White;
            this.btnStreetLevel1.BackColor = Color.White;
            this.btnStreetLevel2.BackColor = Color.White;
            this.btnZoomOut.BackColor = Color.Gray;

            ZoomInMap(false, 2);
        }

       protected void btnZoomIn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.btnZoomIn.BackColor = Color.Gray;
            this.btnCountryLevel.BackColor = Color.White;
            this.btnMaxZoom.BackColor = Color.White;
            this.btnRegionLevel1.BackColor = Color.White;
            this.btnRegionLevel2.BackColor = Color.White;
            this.btnStreetLevel1.BackColor = Color.White;
            this.btnStreetLevel2.BackColor = Color.White;
            this.btnZoomOut.BackColor = Color.White;

            ZoomInMap(true, 2);



        }

        protected void cmdRecenter_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 0;
           sn.GeoZone.SelectLayer = "trans";
           sn.GeoZone.SetGeoZone = false;
        }

        protected void cmdZoomIn_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 1;
           sn.GeoZone.SelectLayer = "trans";
           sn.GeoZone.SetGeoZone = false;
        }

        protected void cmdZoomOut_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 2;
           sn.GeoZone.SelectLayer = "trans";
           sn.GeoZone.SetGeoZone = false;
        }


        private void ZoomInMap(bool zoomIn, int level)
        {

            for (int i = 0; i < level; i++)
            {
                RetrievesMapStateFromViewState(map);
                map.Zoom(zoomIn);
                SavesMapStateToViewState(map);
            }
            ZoomInMap(zoomIn);
        }

        private void DrawGeoZones()
        {

            try
            {
                if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count > 0)
                {



                    if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    {

                        DataRow rowItem1 =sn.GeoZone.DsGeoDetails.Tables[0].Rows[0];
                        DataRow rowItem2 =sn.GeoZone.DsGeoDetails.Tables[0].Rows[1];

                        VLF.MAP.GeoPoint geopoint1 = new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]), Convert.ToDouble(rowItem1["Longitude"]));
                        VLF.MAP.GeoPoint geopoint2 = new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]), Convert.ToDouble(rowItem2["Longitude"]));
                        VLF.MAP.GeoPoint center = VLF.MAP.MapUtilities.GetGeoCenter(geopoint1, geopoint2);
                        
                        map.GeoZones.Add(new GeoZoneIcon(geopoint1, geopoint2));

                    }
                    else //Polygon
                    {

                        //First Point Icon
                        if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count == 1)
                        {
                            map.Landmarks.Clear();

                            DataRow rowItem =sn.GeoZone.DsGeoDetails.Tables[0].Rows[0];
                            map.Landmarks.Add(new MapIcon(Convert.ToDouble(rowItem["Latitude"]), Convert.ToDouble(rowItem["Longitude"]), "StartGeoZone.ico", ""));
                        }
                        else
                        {
                            GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count];
                            int i = 0;
                            foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[0].Rows)
                            {
                                points[i] = new GeoPoint(Convert.ToDouble(rowItem["Latitude"]),
                                                          Convert.ToDouble(rowItem["Longitude"]));
                                i++;
                            }
                            // TODO: put proper severity
                            map.Polygons.Add(new PoligonIcon(points, VLF.CLS.Def.Enums.AlarmSeverity.Critical, VLF.CLS.Def.Enums.GeoZoneDirection.Disable, "",sn.GeoZone.IsGeoZoneComplete));

                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }




        private void CboLandmarks_Fill()
        {
            try
            {

                cboLandmarks.Items.Clear();

                if (sn.DsLandMarks.Tables[0].Rows.Count == 0)
                {
                    StringReader strrXML = null;
                    DataSet dsLandmarks = new DataSet();
                    
                    string xml = "";
                    ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                    if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                        if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                        {
                            return;
                        }

                    if (xml == "")
                    {
                        this.cboLandmarks.DataSource = null;
                        this.cboLandmarks.DataBind();
                        return;
                    }


                    strrXML = new StringReader(xml);
                    dsLandmarks.ReadXml(strrXML);
                    sn.DsLandMarks = dsLandmarks;
                }

                this.cboLandmarks.DataSource = sn.DsLandMarks;
                this.cboLandmarks.DataBind();
                cboLandmarks.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboLandmarks_Item_0"), "-1"));

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cboLandmarks_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
            {
                if (dr["LandmarkName"].ToString().TrimEnd() == this.cboLandmarks.SelectedItem.Value.ToString().TrimEnd())
                {

                    RetrievesMapStateFromViewState(map);
                    map.MapCenterLongitude = Convert.ToDouble(dr["Longitude"]);
                    map.MapCenterLatitude = Convert.ToDouble(dr["Latitude"]);


                    if (sn.User.MapEngine[0].MapEngineID.ToString() == "MapsoluteStatic")
                       map.ImageDistance = Convert.ToDouble(VLF.MAP.MapZoomLevel.StreetLevelHigh2);
                    else
                    {
                       //map.MapScale = map.GetScaleByZoomLevel(VLF.MAP.MapZoomLevel.StreetLevelNorm5);
                       map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", cboLandmarks.SelectedItem.Text ));
                    }

                    
                    map.DrawAllVehicles = false;
                    map.DrawLandmarks = true;
  
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



                   sn.GeoZone.ImgConfPath = url;
                    SavesMapStateToViewState(map);
                    SetColorZoomLevel(map);
                    return;
                }

            }
        }





        public void RetrievesMapStateFromViewState(VLF.MAP.ClientMapProxy map)
        {
            map.MapCenterLongitude = Convert.ToDouble(sn.Map.XInCoord);
            map.MapCenterLatitude = Convert.ToDouble(sn.Map.YInCoord);
            map.MapScale = sn.Map.MapScale;
            map.ImageDistance = sn.Map.ImageDistance;
            map.SouthWestCorner = sn.Map.SouthWestCorner;
            map.NorthEastCorner = sn.Map.NorthEastCorner;

        }

        public void SavesMapStateToViewState(VLF.MAP.ClientMapProxy map)
        {
            sn.Map.XInCoord = map.MapCenterLongitude;
            sn.Map.YInCoord = map.MapCenterLatitude;
            sn.Map.MapScale = map.MapScale;
            sn.Map.ImageDistance = map.ImageDistance;
            sn.Map.SouthWestCorner = map.SouthWestCorner;
            sn.Map.NorthEastCorner = map.NorthEastCorner;
        }




        protected void cmdDrawGeoZone_Click(object sender, System.EventArgs e)
        {

            if (!sn.GeoZone.SetGeoZone)
            {

                if (sn.GeoZone.EditMode)
                {
                    if ((ViewState["ConfirmDelete"] == null) || (ViewState["ConfirmDelete"].ToString() == "1"))
                    {
                        ViewState["ConfirmDelete"] = "0";
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_ReplaceConfirmation");
                        return;
                    }
                }

                ClearMap();

                this.lblMessage.Visible = true;
                this.GeoZoneOptions.Enabled = false;

                if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Polygon)
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_ClickToCreateGeozone");
                else if (sn.GeoZone.GeozoneTypeId == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_DrawDesiredRectangle");

               sn.GeoZone.SetGeoZone = true;
                this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_CompleteDrawing");

                if ((VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(this.GeoZoneOptions.SelectedItem.Value) == VLF.CLS.Def.Enums.GeozoneType.Rectangle)
                {
                   sn.GeoZone.SelectLayer = "geozone";
                   sn.GeoZone.DsGeoDetails.Tables[0].Clear();
                }

               sn.GeoZone.SetGeoZone = true;
                sn.Map.SelectedZoomLevelType = -1;
                cmdDrawGeoZone.BorderStyle = BorderStyle.Inset;

                this.GeoZoneOptions.Enabled = false;
            }
            else
            {


                if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count < 2)
                {
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_MinimumPointsRectangleError");
                    return;
                }

                if ((VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(this.GeoZoneOptions.SelectedItem.Value) == VLF.CLS.Def.Enums.GeozoneType.Polygon)
                {


                    if (sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count < 3)
                    {
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_MinimumPointsPolygonError");
                        return;
                    }

                    map.Landmarks.Clear();
                   sn.GeoZone.IsGeoZoneComplete = true;

                    RetrievesMapStateFromViewState(map);

                    GeoPoint[] points = new GeoPoint[sn.GeoZone.DsGeoDetails.Tables[0].Rows.Count];
                    int i = 0;
                    foreach (DataRow rowItem in sn.GeoZone.DsGeoDetails.Tables[0].Rows)
                    {
                        points[i] = new GeoPoint(Convert.ToDouble(rowItem["Latitude"]),
                            Convert.ToDouble(rowItem["Longitude"]));
                        i++;
                    }
                    // TODO: put proper severity
                    map.Polygons.Add(new PoligonIcon(points, VLF.CLS.Def.Enums.AlarmSeverity.Critical, VLF.CLS.Def.Enums.GeoZoneDirection.Disable, "", true));



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




                   sn.GeoZone.ImgConfPath = url;

                    SavesMapStateToViewState(map);
                    SetColorZoomLevel(map);
                }


                ViewState["ConfirmDelete"] = "1";
               sn.GeoZone.SetGeoZone = false;
                this.cmdDrawGeoZone.Text = (string)base.GetLocalResourceObject("cmdDrawGeozone_Text_StartDrawing");
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "";
                cmdDrawGeoZone.BorderStyle = BorderStyle.Outset;

                if (sn.User.MapEngine[0].MapEngineID == VLF.MAP.MapType.GeoMicro)
                    this.GeoZoneOptions.Enabled = false;
                else
                    this.GeoZoneOptions.Enabled = true;
               sn.GeoZone.SelectLayer = "trans";

            }
        }

        private void DrawLandMark()
        {
            map.Landmarks.Add(new MapIcon(Convert.ToDouble(sn.Landmark.Y), Convert.ToDouble(sn.Landmark.X), "NewLandmark.ico", ""));

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



           sn.GeoZone.ImgConfPath = url;

            SavesMapStateToViewState(map);
            SetColorZoomLevel(map);
        }


        protected void cmdShowEditGeoZone_Click(object sender, System.EventArgs e)
        {
            this.tblGeoZone.Visible = true;
           sn.GeoZone.ShowEditGeoZoneTable = true;

        }

        private void ClearMap()
        {
            this.lblMessage.Text = "";
           sn.GeoZone.DsGeoDetails.Clear();
           sn.GeoZone.IsGeoZoneComplete = false;
            // Retrieves Map from GeoMicro 
            RetrievesMapStateFromViewState(map);

            map.DrawAllVehicles = false;


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



           sn.GeoZone.ImgConfPath = url;
            SavesMapStateToViewState(map);
        }
    }
}
