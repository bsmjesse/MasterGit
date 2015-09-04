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
using System.Globalization;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmBigMap.
    /// </summary>
    public partial class frmBigMap : SentinelFMBasePage
    {
        
        
        public VLF.MAP.ClientMapProxy map;
        //		public string strGeoMicroURL;
        //		public string imgPath=@"..\Images\BigMap.png";	
        public string imgPath = "";
        public int imageW = 840;
        public int imageH = 620;
        public int AutoRefreshTimer = 30000;



        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                try
                {
                    if (Request.QueryString["clientWidth"] != null)
                    {
                        sn.Map.ScreenWidth = Convert.ToInt32(Request.QueryString["clientWidth"]);
                        imageW = Convert.ToInt32(sn.Map.ScreenWidth) - 150;
                        sn.Map.MapResize = true;
                        if (Convert.ToInt32(Request.QueryString["clientHeight"]) > imageH)
                        {
                            imageH = Convert.ToInt32(Request.QueryString["clientHeight"]) - 50;
                            sn.Map.ScreenHeight = Convert.ToInt32(Request.QueryString["clientHeight"]) - 50;
                        }

                    }
                    else
                    {
                        if (sn.User.ScreenWidth != 0)
                        {
                            if (sn.Map.ScreenHeight>700)
                                imageH = Convert.ToInt32(sn.Map.ScreenHeight) - 50;

                            if (sn.Map.MapResize)
                                imageW = Convert.ToInt32(sn.Map.ScreenWidth) - 150;
                        }
                    }
                }
                catch
                {
                    imageW = Convert.ToInt32(sn.User.ScreenWidth) - 200;
                }


                //imageW = Convert.ToInt32(sn.User.ScreenWidth * 0.75);

                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;

                // create ClientMapProxy only for mapping
                map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
                if (map == null)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }
                map.MapWidth = imageW;
                map.MapHeight = imageH;








                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmBigMapForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

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



                    //					if (sn.Map.MapRefresh) 
                    //					{
                    //						this.chkAutoUpdate.Checked=true;
                    //						Response.Write("<script language='javascript'>window.setTimeout('AutoReloadMap()',"+AutoRefreshTimer.ToString()+")</script>") ;
                    //					}
                    //					else
                    //					{
                    //						this.chkAutoUpdate.Checked=false;
                    //						Response.Write("<script language='javascript'> clearTimeout();</script>") ; 
                    //					}


                    //Get Last Vehicles Positions
                    DsFleetInfo_Fill(sn.Map.SelectedFleetID);

                    // Redraw map by coorinates
                    if ((CoordInX != null) && (CoordInX != "0"))
                    {
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
                        return;
                    }

                 

                    // Redraw map on "Map It" and autorefresh	
                    if ((sn != null) && (sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables.Count > 0))
                        LoadVehiclesMap();
                    else
                        LoadDefaultMap();
                }

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
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                sn.Map.DrawLandmarks(sn, ref map);   

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
                            //sn.Map.CreateVehiclesTooltip(sn, map);
                            CreateVehiclesTooltip(sn, map);
                            SetColorZoomLevel(map);
                            return;
                        }
                    }

                   
                }


                string url = map.CreateMap();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo , VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info , "Big map url: " + url));

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



                if (url != "")
                {
                    imgPath = url;
                    ViewState["imgPath"] = imgPath;
                }
                sn.Map.SavesMapStateToViewState(sn, map);
                //sn.Map.CreateVehiclesTooltip(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);

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
                sn.Map.DrawLandmarks(sn, ref map);   


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


                sn.Map.RetrievesMapStateFromViewState(sn, map);

                if (blnVehicleSelected)
                    map.DrawAllVehicles = true;


                string url = map.CreateMap();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Big map url: " + url));

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




                // works only if center coordinates outside of valid values, ex: lat > 90, lon  < 180
                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    if (!((sn.User.MapEngine[0].MapEngineID == VLF.MAP.MapType.GeoMicroWeb) && (map.Vehicles.Count > 1)))
                    {
                        LoadDefaultMap();
                        return;
                    }
                }





                imgPath = url;
                ViewState["imgPath"] = imgPath;
                sn.Map.SavesMapStateToViewState(sn, map);
                //sn.Map.CreateVehiclesTooltip(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        public void LoadDefaultMap()
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;

                // Set default map coordinates
                //				map.MapCenterLongitude = -95.6743;
                //				map.MapCenterLatitude = 47.1623611;
                //				map.Zoom(VLF.MAP.MapZoomLevel.CountryLevelLow2);
                //				imgPath=@"..\Images\BigMap.png";

                map.GetDefaultBigMap();
                string url = map.CreateMap();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Big map url: " + url));
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


                if (url != "")
                {
                    imgPath = url;
                    ViewState["imgPath"] = imgPath;
                }

                ViewState["imgPath"] = imgPath;
                sn.Map.SavesMapStateToViewState(sn, map);

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected  void btnMoveEast_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveMap(MapPanDirection.East);
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


        private void MoveMap(MapPanDirection mapDirection)
        {
            try
            {
                if ((sn == null) || (sn.UserName == ""))
                {

                    string str = "";
                    str = "top.document.all('TopFrame').cols='0,*';";
                    Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }

                // Retrieves Map from GeoMicro
                // restore map states
                sn.Map.RetrievesMapStateFromViewState(sn, map);

                map.Pan(mapDirection);
                string url = map.CreateMap();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Big map url: " + url));
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




                imgPath = url;
                ViewState["imgPath"] = imgPath;
                sn.Map.SavesMapStateToViewState(sn, map);
                //sn.Map.CreateVehiclesTooltip(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void cmdCenterMap_Click(object sender, System.EventArgs e)
        {
            try
            {
                if ((sn == null) || (sn.UserName == ""))
                {
                    string str = "";
                    str = "top.document.all('TopFrame').cols='0,*';";
                    Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }


                if (sn.Map.DsFleetInfo == null)
                {
                    return;
                }

                if ((sn.Map.DsFleetInfo != null) &&
                    (sn.Map.DsFleetInfo.Tables.Count > 0) &&
                    (sn.Map.DsFleetInfo.Tables[0].Rows.Count > 0))
                {
                    // Retrieves Map from GeoMicro
                    // restore map states
                    sn.Map.RetrievesMapStateFromViewState(sn, map);

                    // center all vehicles

                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                        {
                            map.DrawAllVehicles = true;
                            break;
                        }
                    }

                    string url = map.CreateMap();
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Big map url: " + url));

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



                    imgPath = url;
                    ViewState["imgPath"] = imgPath;
                    sn.Map.SavesMapStateToViewState(sn, map);
                    //sn.Map.CreateVehiclesTooltip(sn, map);
                    CreateVehiclesTooltip(sn, map);
                    SetColorZoomLevel(map);
                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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

        protected void cmdRecenter_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 0;
           
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



       protected void btnZoomIn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ZoomInMap(true, 2);
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

                    string str = "";
                    str = "top.document.all('TopFrame').cols='0,*';";
                    Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }

                // Retrieves Map from GeoMicro
                // restore map states
                sn.Map.RetrievesMapStateFromViewState(sn, map);
                map.Zoom(zoomIn);
                string url = map.CreateMap();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Big map url: " + url));

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




                imgPath = url;
                ViewState["imgPath"] = imgPath;
                sn.Map.SavesMapStateToViewState(sn, map);
                //sn.Map.CreateVehiclesTooltip(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
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
                    string str = "";
                    str = "top.document.all('TopFrame').cols='0,*';";
                    Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }

                // Retrieves Map from GeoMicro
                // restore map states
                sn.Map.RetrievesMapStateFromViewState(sn, map);

                map.Zoom(mapZoomLevel);
                string url = map.CreateMap();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Big map url: " + url));
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


                imgPath = url;
                ViewState["imgPath"] = imgPath;
                sn.Map.SavesMapStateToViewState(sn, map);
                //sn.Map.CreateVehiclesTooltip(sn, map);
                CreateVehiclesTooltip(sn, map);
                SetColorZoomLevel(map);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }


        private void DsFleetInfo_Fill(int fleetId)
        {
            try
            {

                DataSet dsFleetInfo = new DataSet();
                
                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    sn.Map.DsFleetInfo = null;
                    return;
                }

                strrXML = new StringReader(xml);
                dsFleetInfo.ReadXml(strrXML);
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;


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
            //this.btnMoveEast.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveEast_Click);
            //this.btnMoveSouth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveSouth_Click);
            //this.btnMoveWest.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveWest_Click);
            //this.btnMoveNorth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveNorth_Click);
            //this.btnZoomIn.Click += new System.Web.UI.ImageClickEventHandler(this.btnZoomIn_Click);
            //this.btnZoomOut.Click += new System.Web.UI.ImageClickEventHandler(this.btnZoomOut_Click);

        }
        #endregion

       protected void btnZoomOut_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ZoomInMap(false, 2);

        }


        private void ZoomInMap(bool zoomIn, int level)
        {

            for (int i = 0; i < level; i++)
            {
                sn.Map.RetrievesMapStateFromViewState(sn, map);
                map.Zoom(zoomIn);
                sn.Map.SavesMapStateToViewState(sn, map);
            }
            ZoomInMap(zoomIn);
        }



        public void CreateVehiclesTooltip(SentinelFMSession sn, ClientMapProxy map)
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

                    DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("Description='" + map.Vehicles[i].IconLabel.ToString().TrimEnd().Replace("'", "''") + "'");
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
                            sn.Map.VehiclesToolTip += "<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]: Position Old</B><BR> <FONT style='TEXT-DECORATION: underline'  >" + Resources.Const.map_Address + ":</FONT>" + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><BR> <FONT style='TEXT-DECORATION: underline'  > " + Resources.Const.map_Address + ":</FONT>" + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' >" + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";

                    }
                    else
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"].ToString()) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            sn.Map.VehiclesToolTip += "\"<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]: Position Old</B><BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Address + ":</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "\"<B>" + rowItem["Description"].ToString().TrimEnd().Replace("'", "''") + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Address + ":</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Speed + ":</FONT> " + rowItem["CustomSpeed"].ToString().TrimEnd() + ", " + "<BR> <FONT style='TEXT-DECORATION: underline' > " + Resources.Const.map_Status + ":</FONT> " + rowItem["VehicleStatus"].ToString().TrimEnd() + "<BR>\",";
                    }

                    if (map.Vehicles.Count > 0)
                    {
                        tblRow = tblToolTips.NewRow();
                        tblRow["Description"] = map.Vehicles[i].IconLabel.Replace("'", "''");
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
                            sn.Map.VehiclesToolTip += "<FONT style='COLOR: #084f7f'>" + "<FONT style='TEXT-DECORATION: underline' >" + Resources.Const.map_Landmark + ":" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT><BR>\",";
                        else
                            sn.Map.VehiclesToolTip += "\"<FONT style='COLOR: #084f7f'>" + "<FONT style='TEXT-DECORATION: underline' >" + Resources.Const.map_Landmark + ":" + "</FONT> " + map.Landmarks[i].IconLabel + "</FONT><BR>\",";


                        tblRow = tblToolTips.NewRow();
                        tblRow["Description"] = map.Landmarks[i].IconLabel.Replace("'", "''");
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
            string areaMaps = "<script type=\"text/javascript\">";

            foreach (int MatrixKey in Matrix.Keys)
            {
                x = (MatrixKey / 100 - 1) * 10;
                y = (MatrixKey % 100 - 1) * 10;

                StartX = x - 8;
                EndX = x + 8;
                StartY = y - 8;
                EndY = y + 8;

                strVehiclesList = Matrix[MatrixKey].ToString().Replace("'", "''");

              
                    sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";

               // areaMaps += "locationMap[\"" + strVehiclesList + "\"] = [" + string.Format("{0},{1},{2},{3}", StartX, StartY, EndX, EndY) + "];";

            }
            areaMaps += "</script>";
            sn.Map.VehiclesMappings = areaMaps;
        }

    }
}
