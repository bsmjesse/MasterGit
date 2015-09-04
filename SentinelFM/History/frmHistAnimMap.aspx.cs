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
using System.Text;
using System.Collections.Generic;

using VLF.Reports;
namespace SentinelFM
{

   public partial class History_frmHistAnimMap : SentinelFMBasePage
   {
      
      


      public static int imageW = 591;
      public static int imageH = 325;
      public static int gridHeight = 300;
      public ClientMapProxy map;
      public string CoordInX;
      public string CoordInY;
      public string CoordEndX;
      public string CoordEndY;
      public static Int32 ScreenHeight = 0;

      string imageArray = "var imgArr = new Array(";
      string mapArray = "var mapInfoArr = new Array(";
      string historyItemArray = " new Array(";
      string historyCollectionArray = "var historyArr = new Array(";




       protected void Page_Load(object sender, EventArgs e)
      {

         try
         {

             #region mapSize

             double ver = getInternetExplorerVersion();

             try
             {
                 if (Request.QueryString["clientWidth"] != null)
                 {
                     sn.History.ScreenWidth = Convert.ToInt32(Request.QueryString["clientWidth"]);
                     imageW = Convert.ToInt32(sn.History.ScreenWidth) - 110;
                     sn.History.MapResize = true;

                 }
                 else
                 {
                     if (sn.User.ScreenWidth != 0)
                     {
                         if (sn.History.MapResize)
                             imageW = Convert.ToInt32(sn.History.ScreenWidth) - 110;
                         else
                             imageW = Convert.ToInt32(sn.User.ScreenWidth) - 330;
                     }
                 }
             }
             catch
             {
                 imageW = Convert.ToInt32(sn.User.ScreenWidth) - 330;
             }



             if (ver < 7.0)
             {

                 Int32 dgWidth = 1000;
                 if (imageW > 1000)
                     dgWidth = 1250;

             }
             #endregion

             #region Create ClientMapProxy only for mapping
             map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
             if (map == null)
             {
                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                 return;
             }
             map.MapWidth = imageW;
             map.MapHeight = imageH;
             #endregion

             if (!Page.IsPostBack)
             {
                 #region Default Settings
                 if (sn.Map.DefaultImgPath == "")
                 {
                     string url = map.GetDefaultMap();
                     clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);
                     sn.Map.DefaultImgPath = url;
                 }

                 if (sn.History.ImgPath == "")
                     sn.History.ImgPath = sn.Map.DefaultImgPath;

                 LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistory, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                 GuiSecurity(this);

                 //Clear Tooltips
                 sn.Map.VehiclesMappings = "";
                 sn.Map.VehiclesToolTip = "";

                 if (sn.History.CarLatitude == "")
                     sn.History.ShowToolTip = true;
                 else
                     sn.History.ShowToolTip = false;


                 #endregion

                 #region Read Coordinate Parameters

                 if (Request.QueryString["CoordInX"] != null)
                     CoordInX = Request.QueryString["CoordInX"].ToString();
                 if (Request.QueryString["CoordInY"] != null)
                     CoordInY = Request.QueryString["CoordInY"].ToString();
                 if (Request.QueryString["CoordEndX"] != null)
                     CoordEndX = Request.QueryString["CoordEndX"].ToString();
                 if (Request.QueryString["CoordEndY"] != null)
                     CoordEndY = Request.QueryString["CoordEndY"].ToString();
                 #endregion

                 #region Check for ZoomIn/ZoomOut menu options
                 string ZoomIn = Request.QueryString["ZoomIn"];

                 if (ZoomIn != null)
                 {
                     if (ZoomIn == "True")
                         ZoomInMap(true, 2);
                     else if (ZoomIn == "False")
                         ZoomInMap(false, 2);

                     return;
                 }
                 #endregion

                 #region Draw Map
                 if (CoordInX != null)
                     RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
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
                 #endregion
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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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

      private void CreateVehiclesTooltip(ClientMapProxy map)
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

            if ((!sn.History.ShowBreadCrumb) || (sn.History.VehicleId==0))           
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
                              sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + " " + rowItem["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                           else
                              sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + " " + rowItem["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                        }
                        else
                        {
                           if (sn.Map.VehiclesToolTip.Length == 0)
                              sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                           else
                              sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + rowItem["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";

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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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
               sn.Map.SavesMapStateToViewState(sn, map);
            }
            ZoomInMap(zoomIn);
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }



      private void RetrievesMapStateFromViewState(ClientMapProxy map)
      {
         try
         {
            map.Vehicles.Clear();
            map.Landmarks.Clear();
            map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
            map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
            map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;
            LoadVehicles();

            if (!String.IsNullOrEmpty(sn.History.MapCenterLongitude))
            {
                    map.MapCenterLongitude = Convert.ToDouble(sn.History.MapCenterLongitude);
                    map.MapCenterLatitude = Convert.ToDouble(sn.History.MapCenterLatitude);
                    map.ImageDistance = Convert.ToDouble(sn.History.ImageDistance);
                    map.MapScale = Convert.ToDouble(sn.History.MapScale);
                    map.SouthWestCorner = new GeoPoint(Convert.ToDouble(sn.History.MapSouthWestCornerLatitude),
                    Convert.ToDouble(sn.History.MapSouthWestCornerLongitude));
                    map.NorthEastCorner = new GeoPoint(Convert.ToDouble(sn.History.MapNorthEastCornerLatitude),
                        Convert.ToDouble(sn.History.MapNorthEastCornerLongitude));
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



      public void SavesMapStateToViewState(ClientMapProxy map)
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
            catch
            {
            }
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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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

            sn.Map.DrawLandmarks(sn, ref map);   


            if (sn.Map.ShowGeoZone)
            {
                if (sn.GeoZone.DsGeoZone == null ||sn.GeoZone.DsGeoDetails == null)
                    DsGeoZone_Fill();

                if (sn.GeoZone.DsGeoZone != null &&sn.GeoZone.DsGeoDetails != null)
                    DrawGeoZones();
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

                  if ((!sn.History.ShowBreadCrumb) || (sn.History.VehicleId==0))           
                  {
                     foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                     {
                        if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                        {
                           if (dr["MsgDetails"].ToString().Contains("PTO"))
                                map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Blue" + IconType + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                           else if (dr["Speed"].ToString() != "0")
                              map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + IconType + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                           else
                              map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + IconType + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                        }
                     }
                  }
                  else if (sn.History.ShowTrips)
                  {
                      if (sn.History.DsHistoryInfo.Tables[0].Rows.Count==0) //no trips 
                          return;
 
                      foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[1].Rows)
                      {
                            if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                            {
                                if (dr["MsgDetails"].ToString().Contains("PTO"))
                                    map.BreadCrumbPoints.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Blue" + sn.History.IconTypeName + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                                else if (dr["Speed"].ToString() != "0")
                                    map.BreadCrumbPoints.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                                else
                                    map.BreadCrumbPoints.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + sn.History.IconTypeName + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                            }
                        }
                  }
                  else
                  {

                     foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                     {
                        if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                        {
                           if (dr["MsgDetails"].ToString().Contains("PTO"))
                               map.BreadCrumbPoints.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Blue" + sn.History.IconTypeName + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                           else if (dr["Speed"].ToString() != "0")
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
                     string icon = "";

                     if (Convert.ToBoolean(dr["chkBoxShow"]))
                     {

                         if (dr["Remarks"].ToString() == "Idling")
                             icon = "Idle.ico";
                         else if (StopDurationVal < 15)
                             icon = "Stop_3.ico";
                         else if ((StopDurationVal > 15) && (StopDurationVal < 60))
                             icon = "Stop_15.ico";
                         else if (StopDurationVal > 60)
                             icon = "Stop_60.ico";

                         map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), icon, dr["StopIndex"].ToString()));
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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }

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
              DataSet dsGeoZoneDetails;
              
              string xml = "";
              string tableName = "";
              ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

              foreach (DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
              {
                  tableName = dr["GeoZoneId"].ToString();
                 sn.GeoZone.GeozoneTypeId = (VLF.CLS.Def.Enums.GeozoneType)Convert.ToInt16(dr["GeozoneType"].ToString().TrimEnd());
                  if (sn.GeoZone.DsGeoDetails!=null &&sn.GeoZone.DsGeoDetails.Tables[tableName] !=null &&sn.GeoZone.DsGeoDetails.Tables[tableName].Rows.Count > 0)
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



      #region Map Animation Data
      private void MapAnimation()
      {
        List<string> images = new List<string>();
        List<HistoryItem> history = new List<HistoryItem>();
        List<string> historyItems = new List<string>();
        #region Load Vehicle Icons
         
          images.Add(string.Format("Blue{0}.ico", sn.History.IconTypeName));
          images.Add(string.Format("Green{0}.ico", sn.History.IconTypeName));
          images.Add(string.Format("Red{0}.ico", sn.History.IconTypeName));
          images.Add(string.Format("Gray{0}.ico", sn.History.IconTypeName));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "N"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "NE"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "E"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "SE"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "S"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "SW"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "W"));
          images.Add(string.Format("Green{0}{1}.ico", sn.History.IconTypeName, "NW"));
          
          #endregion

          for (int i = 0; i < map.Vehicles.Count; i++)
          {
              if (map.Vehicles[i].X > 0 && map.Vehicles[i].Y > 0)
              {
                  HistoryItem vehicleItem = new HistoryItem();
                  vehicleItem.Timestamp = Convert.ToDateTime(map.Vehicles[i].IconLabel);
                  vehicleItem.Latitude  = map.Vehicles[i].Latitude ;
                  vehicleItem.Longitude = map.Vehicles[i].Longitude;
                  vehicleItem.ImageIndex = images.IndexOf(map.Vehicles[i].IconFile.ToString());
                  history.Add(vehicleItem); 
              }

          }

          foreach (HistoryItem item in history)
              historyItems.Add(CreateJShistoryItemArray(item));

          VLF.MAP.GeoPoint upper = new GeoPoint();
          VLF.MAP.GeoPoint lower = new GeoPoint();
          upper = map.SouthWestCorner;
          lower = map.NorthEastCorner;

          double tmpLatitude = upper.Latitude;
          upper.Latitude = lower.Latitude;
          lower.Latitude = tmpLatitude;

          StringBuilder s = new StringBuilder();
          s.AppendLine(CreateJSimageArray(images));
          s.AppendLine(CreateJSmapInfoArray(sn.History.ImgPath,upper.Latitude  , upper.Longitude  , lower.Latitude  , upper.Longitude ));
          s.AppendLine(CreateJShistoryArray(historyItems));

          this.JSInfo.Text = CreateJSscriptContainer(s.ToString());

      }




    
      #endregion


      #region Map Animation Functions
      string CreateJSimageArray(List<string> images)
    {
        StringBuilder s = new StringBuilder(imageArray);
        foreach (string img in images) s.AppendFormat(" \"{0}\",", img);
        s.Remove(s.Length - 1, 1);
        s.AppendLine(");");
        return s.ToString();
    }

    string CreateJSmapInfoArray(string mapImageFilePath, double upperLat, double upperLon, double lowerLat, double lowerLon)
    {
        StringBuilder s = new StringBuilder(mapArray);
        s.AppendFormat(" \"{0}\",", mapImageFilePath);
        s.AppendFormat(" {0},", upperLat);
        s.AppendFormat(" {0},", upperLon);
        s.AppendFormat(" {0},", lowerLat);
        s.AppendFormat(" {0} ", lowerLon);
        s.AppendLine(");");
        return s.ToString();
    }

    string CreateJShistoryItemArray(HistoryItem item)
    {
        StringBuilder s = new StringBuilder(historyItemArray);
        s.AppendFormat(" \"{0}\",", item.Timestamp.ToString("MM/dd/yyyy HH:mm:ss"));
        s.AppendFormat(" {0},", item.Latitude);
        s.AppendFormat(" {0},", item.Longitude);
        s.AppendFormat(" {0} ", item.ImageIndex);
        s.Append(")");
        return s.ToString();
    }


    string CreateJShistoryArray(List<string> history)
    {
        StringBuilder s = new StringBuilder(historyCollectionArray);
        s.AppendLine();
        foreach (string item in history)
            s.AppendFormat(" {0},", item);
        s.Remove(s.Length - 1, 1);
        s.AppendLine(");");
        return s.ToString();
    }

    string CreateJSscriptContainer(string script)
    {
        StringBuilder s = new StringBuilder();
        s.AppendLine("<script type=\"text/javascript\">");
        s.AppendLine(script);
        //s.AppendLine("alert(\"image count:\" + imgArr.length);");
        //s.AppendLine("alert(\"history count:\" + historyArr.length);");
        s.AppendLine("</script>");
        return s.ToString();
    }

    struct HistoryItem
    {
       public DateTime Timestamp;
       public double Latitude;
       public double Longitude;
       public int ImageIndex;
    }
       #endregion

    
   }
}
