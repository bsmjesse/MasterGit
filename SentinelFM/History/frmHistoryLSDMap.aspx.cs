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
using System.Configuration;
using System.Globalization;
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Text.RegularExpressions;
using VLF.Reports;
namespace SentinelFM
{
    /// <summary>
    public partial class History_frmHistoryLSDMap  : SentinelFMBasePage
    {
        public static int imageW = 591;
        public static int imageH = 325;
        public static int gridHeight = 300;
        private DataSet dsHistory;
        public VLF.MAP.LSDProxy map;
        public string CoordInX;
        public string CoordInY;
        public string CoordEndX;
        public string CoordEndY;
        public static Int32 ScreenHeight = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                double ver = getInternetExplorerVersion();

                try
                {
                    if (Request.QueryString["clientWidth"] != null)
                    {
                        sn.History.ScreenWidth = Convert.ToInt32(Request.QueryString["clientWidth"]);
                        imageW = Convert.ToInt32(sn.History.ScreenWidth) - 290;
                        sn.History.MapResize = true;
                    }
                    else
                    {
                        if (sn.User.ScreenWidth != 0)
                        {
                            if (sn.History.MapResize)
                                imageW = Convert.ToInt32(sn.History.ScreenWidth) - 290;
                            else
                                imageW = Convert.ToInt32(sn.User.ScreenWidth) - 510;
                        }
                    }
                }
                catch
                {
                    imageW = Convert.ToInt32(sn.User.ScreenWidth) - 510;
                }

                if (ver < 7.0)
                {
                    Int32 dgWidth = 1000;
                    if (imageW > 1000)
                        dgWidth = 1250;
                }

                // create ClientMapProxy only for mapping
                string mapExternalPath = ConfigurationSettings.AppSettings["GeoMicroMapExternalPath"];
                string mapInternalPath = ConfigurationSettings.AppSettings["GeoMicroMapInternalPath"];
                map = new LSDProxy(imageW, imageH, mapExternalPath, mapExternalPath);

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
                    if (sn.Map.DefaultImgPath == "")
                    {
                        string url = map.GetDefaultMap();
                        clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                        sn.Map.DefaultImgPath = url;
                    }

                    if (sn.History.ImgPath == "")
                        sn.History.ImgPath = sn.Map.DefaultImgPath;

                    if (sn.Map.DisabledLSDLayers != null)
                    {
                        for (int i = 0; i < sn.Map.DisabledLSDLayers.Length; i++)
                            for (int chkItems = 0; chkItems < 4; chkItems++)
                                if (Convert.ToInt16(chkLayers.Items[chkItems].Value) == Convert.ToInt16(sn.Map.DisabledLSDLayers[i]))
                                    chkLayers.Items[chkItems].Selected = false;
                    }

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistory, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    GuiSecurity(this);

                    //Clear Tooltips
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";

                    if (sn.History.CarLatitude == "")
                        sn.History.ShowToolTip = true;
                    else
                        sn.History.ShowToolTip = false;

                    if (Request.QueryString["CoordInX"] != null)
                        CoordInX = Request.QueryString["CoordInX"].ToString();
                    if (Request.QueryString["CoordInY"] != null)
                        CoordInY = Request.QueryString["CoordInY"].ToString();
                    if (Request.QueryString["CoordEndX"] != null)
                        CoordEndX = Request.QueryString["CoordEndX"].ToString();
                    if (Request.QueryString["CoordEndY"] != null)
                        CoordEndY = Request.QueryString["CoordEndY"].ToString();

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

                    // Redraw Map
                    if (CoordInX != null)
                    {
                        RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
                    }
                    else
                    {
                        sn.History.ShowToolTip = true;
                        sn.History.CarLatitude = "";
                        sn.History.CarLongitude = "";

                        //LoadVehiclesMap();
                        RetrievesMapStateFromViewState(map);

                        if ((sn != null) && (map.MapCenterLatitude != 0) && (map.MapCenterLongitude != 0))
                            LoadVehiclesMap();
                        else
                            LoadDefaultMap();
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 1-frmHistMap.aspx"));
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
                map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;
                string url = map.GetDefaultMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                sn.History.ImgPath = url;
                map.DrawAllVehicles = false;
                SavesMapStateToViewState(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 2-frmHistMap.aspx"));
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
                {
                    string str = "";
                    str = "top.document.all('TopFrame').cols='0,*';";
                    Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }
                // Retrieves Map from GeoMicro
                // restore map states
                RetrievesMapStateFromViewState(map);
                map.DrawAllVehicles = false;

                map.Pan(mapDirection);

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if ((map.MapCenterLongitude == 0) && (map.MapCenterLatitude == 0))
                {
                    LoadDefaultMap();
                    return;
                }

                sn.History.ImgPath = url;
                SavesMapStateToViewState(map);

                if (sn.History.CarLatitude == "")
                {
                    CreateVehiclesTooltip(map);
                }
                else
                {
                    //ToolTip
                    sn.Map.VehiclesToolTip = "";
                    sn.Map.VehiclesMappings = "";

                    if (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.StopIndex.ToString() + "]:</B><FONT style='COLOR: Purple'>" + sn.History.StopStatus + " (" + sn.History.StopDuration + ") ," + sn.History.StopDate + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.StopAddress + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);
                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    else
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.CarHistoryDate + "]:</B><FONT style='COLOR: Purple'>" + sn.History.CarMessageType + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.CarAddress + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + sn.History.CarSpeed + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);
                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    CreateLandmarksToolTip();
                }
                SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 3-frmHistMap.aspx"));
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

        private void ZoomMap(MapZoomLevel mapZoomLevel)
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;

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

                if ((map.MapCenterLongitude == 0) && (map.MapCenterLatitude == 0))
                {
                    LoadDefaultMap();
                    return;
                }
                sn.History.ImgPath = url;
                SavesMapStateToViewState(map);

                if (sn.History.CarLatitude == "")
                {
                    CreateVehiclesTooltip(map);
                }
                else
                {
                    //ToolTip
                    sn.Map.VehiclesToolTip = "";
                    sn.Map.VehiclesMappings = "";

                    if (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.StopIndex.ToString() + "]:</B><FONT style='COLOR: Purple'>" + sn.History.StopStatus + " (" + sn.History.StopDuration + ") ," + sn.History.StopDate + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.StopAddress + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);
                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    else
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.CarHistoryDate + "]:</B><FONT style='COLOR: Purple'>" + sn.History.CarMessageType + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.CarAddress + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + sn.History.CarSpeed + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);
                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    CreateLandmarksToolTip();
                }
                //CreateVehiclesTooltip(map);
                SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 4-frmHistMap.aspx"));
            }
        }

        private void CreateLandmarksToolTip()
        {
            //Table Vehicles with coordinates
            DataTable tblToolTips = new DataTable();
            DataRow tblRow;

            DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
            tblToolTips.Columns.Add(colDescription);
            DataColumn colX = new DataColumn("colX", Type.GetType("System.Double"));
            tblToolTips.Columns.Add(colX);
            DataColumn colY = new DataColumn("colY", Type.GetType("System.Double"));
            tblToolTips.Columns.Add(colY);

            tblRow = tblToolTips.NewRow();
            tblRow["Description"] = "";
            if (map.Vehicles.Count > 0)
            {
                tblRow["colX"] = map.Vehicles[0].X;
                tblRow["colY"] = map.Vehicles[0].Y;
                tblToolTips.Rows.Add(tblRow);
            }

            //Create Landmark Tooltip description
            for (int i = 0; i < map.Landmarks.Count; i++)
            {
                if ((map.Landmarks[i].X != -1) && (map.Landmarks[i].Y != -1) && (map.Landmarks[i].X != -0) && (map.Landmarks[i].Y != -0))
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

        private void CreateVehiclesTooltip(LSDProxy map)
        {
            try
            {
                sn.Map.VehiclesToolTip = "";
                sn.Map.VehiclesMappings = "";

                if (!sn.History.ShowToolTip)
                    return;

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

                //Create Vehicles Tooltip description
                //Tooltip description
                VLF.MAP.MapIconCollection iconCollection;

                if ((!sn.History.ShowBreadCrumb) || (sn.History.VehicleId == 0))
                    iconCollection = map.Vehicles;
                else
                    iconCollection = map.BreadCrumbPoints;

                if (iconCollection.Count > 0)
                {
                    if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
                    {
                        for (int i = 0; i < iconCollection.Count; i++)
                        {
                            if ((iconCollection[i].X != -1) && (iconCollection[i].Y != -1))
                            {
                                DataRow[] drArr = sn.History.DsHistoryInfo.Tables[0].Select("OriginDateTime='" + iconCollection[i].IconLabel + "'");
                                if (drArr == null || drArr.Length == 0)
                                    continue;
                                DataRow rowItem = drArr[0];
                                //foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
                                //{
                                //   if (rowItem["OriginDateTime"].ToString().TrimEnd() == iconCollection[i].IconLabel.ToString().TrimEnd())
                                //   {
                                if (rowItem["MsgDetails"].ToString().TrimEnd() != "")
                                {
                                    if (sn.Map.VehiclesToolTip.Length == 0)
                                        sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + " " + rowItem["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                    else
                                        sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + " " + rowItem["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                }
                                else
                                {
                                    if (sn.Map.VehiclesToolTip.Length == 0)
                                        sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                    else
                                        sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["OriginDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                }
                                tblRow = tblToolTips.NewRow();
                                tblRow["Description"] = iconCollection[i].IconLabel;
                                tblRow["colX"] = iconCollection[i].X;
                                tblRow["colY"] = iconCollection[i].Y;
                                tblToolTips.Rows.Add(tblRow);
                                //   }
                                //}
                            }
                        }
                    }
                    else
                    {

                        for (int i = 0; i < map.Vehicles.Count; i++)
                        {
                            if ((iconCollection[i].X != -1) && (iconCollection[i].Y != -1))
                            {
                                DataRow[] drArr = sn.History.DsHistoryInfo.Tables["StopData"].Select("StopIndex='" + map.Vehicles[i].IconLabel + "'");
                                if (drArr == null || drArr.Length == 0)
                                    continue;
                                DataRow rowItem = drArr[0];

                                //foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                                //{
                                //   if (rowItem["StopIndex"].ToString().TrimEnd() == map.Vehicles[i].IconLabel.ToString().TrimEnd())
                                //   {
                                rowItem["Location"] = rowItem["Location"].ToString().TrimEnd().Replace(Convert.ToString(Convert.ToChar(13)), " ").Replace(Convert.ToString(Convert.ToChar(10)), " ");

                                if (sn.Map.VehiclesToolTip.Length == 0)
                                    sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["Remarks"].ToString().TrimEnd() + " (" + rowItem["StopDuration"].ToString().TrimEnd() + ") ," + rowItem["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["Location"].ToString().TrimEnd() + "<BR>\",";
                                else
                                    sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["Remarks"].ToString().TrimEnd() + " (" + rowItem["StopDuration"].ToString().TrimEnd() + ") ," + rowItem["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["Location"].ToString().TrimEnd() + "<BR> \",";


                                tblRow = tblToolTips.NewRow();
                                tblRow["Description"] = rowItem["StopIndex"].ToString().TrimEnd();
                                tblRow["colX"] = iconCollection[i].X;
                                tblRow["colY"] = iconCollection[i].Y;
                                tblToolTips.Rows.Add(tblRow);
                                //   }
                                //}
                            }
                        }
                    }
                }

                //Create Landmark Tooltip description
                for (int i = 0; i < map.Landmarks.Count; i++)
                {
                    if ((map.Landmarks[i].X != -1) && (map.Landmarks[i].Y != -1) && (map.Landmarks[i].X != -0) && (map.Landmarks[i].Y != -0))
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 5-frmHistMap.aspx"));
            }
        }   

        protected void cmdRecenter_Click(object sender, System.EventArgs e)
        {
            sn.Map.SelectedZoomLevelType = 0;
            cmdRecenter.BorderStyle = BorderStyle.Inset;
            cmdRecenter.BackColor = Color.Gray;
        }

        private void ZoomInMap(bool zoomIn, int level)
        {
            try
            {
                for (int i = 0; i < level; i++)
                {
                    RetrievesMapStateFromViewState(map);
                    map.Zoom(zoomIn);
                    SavesMapStateToViewState(map);
                }
                ZoomInMap(zoomIn);
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 6-frmHistMap.aspx"));
            }
        }

        private void RetrievesMapStateFromViewState(LSDProxy map)
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;

                LoadVehicles();

                try
                {
                    map.MapCenterLongitude = Convert.ToDouble(sn.History.MapCenterLongitude);
                    map.MapCenterLatitude = Convert.ToDouble(sn.History.MapCenterLatitude);
                    map.ImageDistance = Convert.ToDouble(sn.History.ImageDistance);
                    map.MapScale = Convert.ToDouble(sn.History.MapScale);
                    map.SouthWestCorner = new GeoPoint(Convert.ToDouble(sn.History.MapSouthWestCornerLatitude),
                    Convert.ToDouble(sn.History.MapSouthWestCornerLongitude));
                    map.NorthEastCorner = new GeoPoint(Convert.ToDouble(sn.History.MapNorthEastCornerLatitude),
                        Convert.ToDouble(sn.History.MapNorthEastCornerLongitude));
                }
                catch { }
                
                if ((map.MapCenterLongitude == 0) && (map.MapCenterLatitude == 0))
                {
                    map.DrawAllVehicles = true;
                }
                else
                {
                    map.DrawAllVehicles = Convert.ToBoolean(sn.History.DrawAllVehicles);
                    map.MapScale = Convert.ToDouble(sn.History.MapScale);

                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                VLF.ERR.LOG.LogFile(ConfigurationSettings.AppSettings["LogFolder"], "SentinelFM", "Error:" + Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 7-frmHistMap.aspx");
            }
        }

        public void SavesMapStateToViewState(LSDProxy map)
        {
            sn.History.MapCenterLongitude = map.MapCenterLongitude.ToString();
            sn.History.MapCenterLatitude = map.MapCenterLatitude.ToString();
            sn.History.MapScale = map.MapScale.ToString();
            sn.History.DrawAllVehicles = map.DrawAllVehicles.ToString();
            sn.History.ImageDistance = map.ImageDistance.ToString();
            sn.History.MapSouthWestCornerLatitude = map.SouthWestCorner.Latitude.ToString();
            sn.History.MapSouthWestCornerLongitude = map.SouthWestCorner.Longitude.ToString();
            sn.History.MapNorthEastCornerLatitude = map.NorthEastCorner.Latitude.ToString();
            sn.History.MapNorthEastCornerLongitude = map.NorthEastCorner.Longitude.ToString();
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

        private float getInternetExplorerVersion()
        {
            // Returns the version of Internet Explorer or a -1
            // (indicating the use of another browser).
            float rv = -1;
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            if (browser.Browser == "IE")
                rv = (float)(browser.MajorVersion + browser.MinorVersion);
            return rv;
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
                catch {}
            }
        }

        private void ZoomInMap(bool zoomIn)
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;

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

                sn.History.ImgPath = url;
                SavesMapStateToViewState(map);

                if (sn.History.CarLatitude == "")
                {
                    CreateVehiclesTooltip(map);
                }
                else
                {
                    //ToolTip
                    sn.Map.VehiclesToolTip = "";
                    sn.Map.VehiclesMappings = "";

                    if (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.StopIndex.ToString() + "]:</B><FONT style='COLOR: Purple'>" + sn.History.StopStatus + " (" + sn.History.StopDuration + ") ," + sn.History.StopDate + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.StopAddress + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);

                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    else
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.CarHistoryDate + "]:</B><FONT style='COLOR: Purple'>" + sn.History.CarMessageType + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.CarAddress + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + sn.History.CarSpeed + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);

                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    CreateLandmarksToolTip();
                }
                //CreateVehiclesTooltip(map);
                SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 8-frmHistMap.aspx"));
            }
        }

        protected void cmdZoomOutMain_Click(object sender, ImageClickEventArgs e)
        {
            //this.btnZoomIn.BackColor=Color.White;
            this.btnCountryLevel.BackColor = Color.White;
            this.btnMaxZoom.BackColor = Color.White;
            this.btnRegionLevel1.BackColor = Color.White;
            this.btnRegionLevel2.BackColor = Color.White;
            this.btnStreetLevel1.BackColor = Color.White;
            this.btnStreetLevel2.BackColor = Color.White;
            //this.btnZoomOut.BackColor=Color.Gray;
            ZoomInMap(false, 2);
        }

        protected void cmdZoomInMain_Click(object sender, ImageClickEventArgs e)
        {
            //this.btnZoomIn.BackColor=Color.Gray;
            this.btnCountryLevel.BackColor = Color.White;
            this.btnMaxZoom.BackColor = Color.White;
            this.btnRegionLevel1.BackColor = Color.White;
            this.btnRegionLevel2.BackColor = Color.White;
            this.btnStreetLevel1.BackColor = Color.White;
            this.btnStreetLevel2.BackColor = Color.White;
            //this.btnZoomOut.BackColor=Color.White;

            ZoomInMap(true, 2);
        }

        private void LoadVehiclesMap()
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;

                if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables.Count > 0) && (sn.History.DsHistoryInfo.Tables[0].Rows.Count > 0))
                {
                    LoadVehicles();
                }
                else
                {
                    LoadDefaultMap();
                    return;
                }

                if (sn.History.MapSearch)
                {
                    map.DrawAllVehicles = false;
                    sn.History.MapSearch = false;
                }
                else
                    map.DrawAllVehicles = true;

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if ((Convert.ToInt32(map.MapCenterLongitude) == 0) && (Convert.ToInt32(map.MapCenterLatitude) == 0))
                {
                    LoadDefaultMap();
                    return;
                }

                sn.History.ImgPath = url;
                SavesMapStateToViewState(map);
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 9-frmHistMap.aspx"));
            }
        }

        private void LoadVehicles()
        {
            try
            {
                map.Vehicles.Clear();
                map.Landmarks.Clear();
                map.BreadCrumbPoints.Clear();
                map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
                map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
                map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;
                string IconType = sn.History.IconTypeName;

                if (sn.History.VehicleId == 0)
                {
                    IconType = "Circle";
                    sn.History.IconTypeName = "Circle";
                }

                if (sn.Map.ShowLandmark)
                {
                    if ((sn.DsLandMarks != null) &&
                        (sn.DsLandMarks.Tables.Count > 0) &&
                        (sn.DsLandMarks.Tables[0].Rows.Count > 0))
                    {
                        foreach (DataRow dr in sn.DsLandMarks.Tables[0].Rows)
                        {
                            // map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"], new System.Globalization.CultureInfo("en-US")), Convert.ToDouble(dr["Longitude"], new System.Globalization.CultureInfo("en-US")), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                            if (sn.Map.ShowLandmarkname && sn.Map.ShowLandmarkRadius)
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                            else if (sn.Map.ShowLandmarkname)
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", dr["LandmarkName"].ToString().TrimEnd()));
                            else if (sn.Map.ShowLandmarkRadius)
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), Convert.ToInt32(dr["Radius"]), "Landmark.ico", ""));
                            else
                                map.Landmarks.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Landmark.ico", ""));
                        }
                    }
                }
                if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables.Count > 0))
                {
                    if (((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
                    ) && (sn.History.ShowStopSqNum))
                        map.Vehicles.DrawLabels = true;
                    else
                        map.Vehicles.DrawLabels = false;

                    map.BreadCrumbPoints.DrawLabels = false;

                    if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
                    {
                        if ((!sn.History.ShowBreadCrumb) || (sn.History.VehicleId == 0))
                        {
                            foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                            {
                                //if (sn.History.DsSelectedData != null && sn.History.DsSelectedData.Tables.Count > 0 && sn.History.DsSelectedData.Tables[0].Rows.Count > 1)
                                //{
                                //   DataRow[] drArr = sn.History.DsSelectedData.Tables[0].Select("VehicleId=" + dr["VehicleId"] );
                                //   IconType = drArr[0]["IconType"].ToString();  
                                //}

                                if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                                {
                                    if (dr["Speed"].ToString() != "0")
                                        map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + IconType + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                                    else
                                        map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + IconType + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                                }
                            }
                        }
                        else
                        {
                            foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                            {
                                if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                                {
                                    if (dr["Speed"].ToString() != "0")
                                        map.BreadCrumbPoints.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                                    else
                                        map.BreadCrumbPoints.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + sn.History.IconTypeName + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                        {
                            TimeSpan TripIndling;
                            TripIndling = new TimeSpan(Convert.ToInt64(dr["StopDurationVal"]) * TimeSpan.TicksPerSecond);
                            double StopDurationVal = TripIndling.TotalMinutes;

                            if (Convert.ToBoolean(dr["chkBoxShow"]))
                            {
                                if (dr["Remarks"].ToString() == "Idling")
                                {
                                    map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Idle.ico", dr["StopIndex"].ToString()));
                                }
                                else
                                {
                                    //map.Vehicles.Add(new   MapIcon( Convert.ToDouble(dr["Latitude"]) ,Convert.ToDouble(dr["Longitude"]),"Stop.ico", "" ));		

                                    if (StopDurationVal < 15)
                                        map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Stop_3.ico", dr["StopIndex"].ToString()));
                                    if ((StopDurationVal > 15) && (StopDurationVal < 60))
                                        map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Stop_15.ico", dr["StopIndex"].ToString()));
                                    if (StopDurationVal > 60)
                                        map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Stop_60.ico", dr["StopIndex"].ToString()));
                                }
                            }
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 10-frmHistMap.aspx"));
            }
        }

        private void RedrawMapByXY(string CoordInX, string CoordInY, string CoordEndX, string CoordEndY)
        {
            try
            {               
                RetrievesMapStateFromViewState(map);
                map.DrawAllVehicles = false;
                map.MapCenterLongitude = Convert.ToDouble(sn.History.MapCenterLongitude);
                map.MapCenterLatitude = Convert.ToDouble(sn.History.MapCenterLatitude);
                map.MapScale = Convert.ToDouble(sn.History.MapScale);
                map.ImageDistance = Convert.ToDouble(sn.History.ImageDistance);
                map.SouthWestCorner = new GeoPoint(Convert.ToDouble(sn.History.MapSouthWestCornerLatitude),
                                                    Convert.ToDouble(sn.History.MapSouthWestCornerLongitude));
                map.NorthEastCorner = new GeoPoint(Convert.ToDouble(sn.History.MapNorthEastCornerLatitude),
                                                    Convert.ToDouble(sn.History.MapNorthEastCornerLongitude));

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
                        CreateVehiclesTooltip(map);
                        SetColorZoomLevel(map);
                        return;
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
                    sn.History.ImgPath = url;

                SavesMapStateToViewState(map);

                if (sn.History.CarLatitude == "")
                {
                    CreateVehiclesTooltip(map);
                }
                else
                {
                    //ToolTip
                    sn.Map.VehiclesToolTip = "";
                    sn.Map.VehiclesMappings = "";

                    if (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.StopIndex.ToString() + "]:</B><FONT style='COLOR: Purple'>" + sn.History.StopStatus + " (" + sn.History.StopDuration + ") ," + sn.History.StopDate + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.StopAddress + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);

                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    else
                    {
                        if (map.Vehicles.Count > 0)
                        {
                            sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.CarHistoryDate + "]:</B><FONT style='COLOR: Purple'>" + sn.History.CarMessageType + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.CarAddress + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + sn.History.CarSpeed + "<BR>\",";

                            string StartX = Convert.ToString(map.Vehicles[0].X - 10);
                            string EndX = Convert.ToString(map.Vehicles[0].X + 10);

                            string StartY = Convert.ToString(map.Vehicles[0].Y + 10);
                            string EndY = Convert.ToString(map.Vehicles[0].Y - 10);

                            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.myform.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.myform.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
                        }
                    }
                    CreateLandmarksToolTip();
                }
                SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " 11-frmHistMap.aspx"));
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

                RetrievesMapStateFromViewState(map);

                if (chkLayers.Items.Count > 0)
                {
                    map.DisableLayers(((int[])(disableLayersArr.ToArray(typeof(int)))));
                    sn.Map.DisabledLSDLayers = (int[])disableLayersArr.ToArray(typeof(int));
                }
                else
                    sn.Map.DisabledLSDLayers = null;

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                if (url != "")
                    sn.History.ImgPath = url;

                SavesMapStateToViewState(map);
                CreateVehiclesTooltip(map);
                SetColorZoomLevel(map);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }

        protected void cmdDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                //sn.MessageText = "";
                //Regex meridian = new Regex("[4-5-6]");
                //if (!meridian.IsMatch(this.txtMeridian.Text))
                //    sn.MessageText = "Invalid meridian info. (Meridian: 4,5 and 6) ";

                //Regex range = new Regex("[1-30]");
                //if (!range.IsMatch(this.txtRange.Text))
                //    sn.MessageText += Environment.NewLine + "Invalid range info. (Range: 1-30)";

                //Regex township = new Regex("[1-126]");
                //if (!township.IsMatch(this.txtTownship.Text))
                //    sn.MessageText += Environment.NewLine + "Invalid township info. (Township: 1-126)";

                //Regex section = new Regex("[1-36]");
                //if (!section.IsMatch(this.txtSection.Text))
                //    sn.MessageText += Environment.NewLine + "Invalid section info. (Section: 1-36)";
                sn.MessageText = "";

                try
                {
                    int meridian = Convert.ToInt16(this.txtMeridian.Text);
                    if (meridian < 4 || meridian > 6)
                        sn.MessageText = "Invalid meridian info. (Meridian: 4,5 and 6) ";
                }
                catch (Exception ex)
                {
                    sn.MessageText = "Invalid meridian info. (Meridian: 4,5 and 6) ";
                }

                try
                {
                    int range = Convert.ToInt16(this.txtRange.Text);
                    if (range < 1 || range > 30)
                        sn.MessageText += Environment.NewLine + "Invalid range info. (Range: 1-30)";
                }
                catch (Exception ex)
                {
                    sn.MessageText += Environment.NewLine + "Invalid range info. (Range: 1-30)";
                }

                try
                {
                    int township = Convert.ToInt16(this.txtTownship.Text);
                    if (township < 1 || township > 126)
                        sn.MessageText += Environment.NewLine + "Invalid township info. (Township: 1-126)";
                }
                catch (Exception ex)
                {
                    sn.MessageText += Environment.NewLine + "Invalid township info. (Township: 1-126)";
                }

                try
                {
                    int section = Convert.ToInt16(this.txtSection.Text);
                    if (section < 1 || section > 36)
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
                //VLF.MAP.LSDProxy geomap = new VLF.MAP.LSDProxy("");
                //string address = VLF.MAP.LSDProxy.ToLSD(Convert.ToInt32(this.txtMeridian.Text), Convert.ToInt32(this.txtTownship.Text), Convert.ToInt32(this.txtRange.Text), Convert.ToInt32(this.txtSection.Text), this.cboQuarter.SelectedItem.Value);
                //double lat = 0;
                //double lot = 0;
                //geomap.GetLatitudeLongitude(address, out lat, out lot);

                VLF.MAP.LSDProxy geomap = new VLF.MAP.LSDProxy("");
                string address = VLF.MAP.LSDProxy.ToLSD(Convert.ToInt32(this.txtMeridian.Text), Convert.ToInt32(this.txtTownship.Text), Convert.ToInt32(this.txtRange.Text), Convert.ToInt32(this.txtSection.Text), this.cboQuarter.SelectedItem.Value);
                double lat = 0;
                double lot = 0;
                using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
                {
                    dbs.GetLatitudeLongitudeByLSD(sn.UserID, sn.SecId, address, ref lat, ref lot);
                }

                RetrievesMapStateFromViewState(map);

                map.MapCenterLongitude = lot;
                map.MapCenterLatitude = lat;

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                if (url != "")
                    sn.History.ImgPath = url;

                SavesMapStateToViewState( map);
                CreateVehiclesTooltip( map);
                SetColorZoomLevel(map);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
    }
}
