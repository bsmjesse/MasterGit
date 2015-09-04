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
using VLF.Reports;

namespace SentinelFM
{
   public partial class History_frmHistExtended : SentinelFMBasePage
   {
      public static int imageW = 641;
      public static int imageH = 325;
      public static int gridHeight = 300;
      private DataSet dsHistory;
      public ClientMapProxy map;
      public string CoordInX;
      public string CoordInY;
      public string CoordEndX;
      public string CoordEndY;
      public static Int32 ScreenHeight = 0;

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
         //this.btnMoveNorth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveNorth_Click);
         //this.btnMoveWest.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveWest_Click);
         //this.btnMoveSouth.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveSouth_Click);
         //this.btnZoomIn.Click += new System.Web.UI.ImageClickEventHandler(this.btnZoomIn_Click);
         //this.btnZoomOut.Click += new System.Web.UI.ImageClickEventHandler(this.btnZoomOut_Click);

      }
      #endregion

      protected override void SavePageStateToPersistenceMedium(object viewState)
      {
         base.SavePageStateToPersistenceMedium(viewState);
         Session["VIEWSTATE"] = viewState;
      }

      protected override object LoadPageStateFromPersistenceMedium()
      {
         return Session["VIEWSTATE"];
      }

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            //Clear IIS cache
            //				Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //				Response.Cache.SetExpires(DateTime.Now);
            if (sn == null || sn.UserName == "")
            {
               RedirectToLogin();
               return;
            }
            double ver = getInternetExplorerVersion();
            try
               {
                  if (Request.QueryString["clientWidth"] != null)
                  {
                      imageW = Convert.ToInt32(Request.QueryString["clientWidth"]) - 270;
                      sn.History.ScreenWidth = imageW;  
                    
                  }
                  else
                  {
                     if (sn.History.ScreenWidth != 0)
                        imageW = sn.History.ScreenWidth;
                     else
                        imageW = Convert.ToInt32(sn.User.ScreenWidth) - 270;
                  }
               }
               catch
               {
                  imageW = Convert.ToInt32(sn.User.ScreenWidth) - 270;
               }

               if (ver < 7.0)
                {
                   //dgStops.Width = Unit.Pixel(990);
                   //dgHistoryDetails.Width = Unit.Pixel(990);

                   Int32 dgWidth = 1000;
                   if (imageW > 1000)
                      dgWidth = 1250;

                   dgStops.Width = Unit.Pixel(dgWidth);
                   dgHistoryDetails.Width = Unit.Pixel(dgWidth);
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

            if (!Page.IsPostBack)
            {  
                  dgStops.LayoutSettings.ClientVisible = false;
                  cboRows.SelectedIndex = cboRows.Items.IndexOf(cboRows.Items.FindByValue(sn.History.DgVisibleRows.ToString()));
                  cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue(sn.History.DgItemsPerPage.ToString()));
                  dgHistoryDetails.Height = Unit.Pixel(107 + Convert.ToInt32(dgHistoryDetails.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                  this.dgHistoryDetails.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

                  dgStops.Height = Unit.Pixel(107 + Convert.ToInt32(dgStops.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                  this.dgStops.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

                  if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                  {
                     dgStops.ClearCachedDataSource();
                     dgStops.RebindDataSource();
                     dgStops.LayoutSettings.ClientVisible = true;
                     dgHistoryDetails.LayoutSettings.ClientVisible = false;
                  }
                  else
                  {
                     dgHistoryDetails.ClearCachedDataSource();
                     dgHistoryDetails.RebindDataSource();
                     dgStops.LayoutSettings.ClientVisible = false;
                     dgHistoryDetails.LayoutSettings.ClientVisible = true;
                  }
             
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

               // Show Legend button

               //if (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
               //   this.cmdStopLegends.Visible = true;
               //else
               //   this.cmdStopLegends.Visible = false;

               if (sn.History.CarLatitude == "")
                  sn.History.ShowToolTip = true;
               else
                  sn.History.ShowToolTip = false;

               FindExistingPreference();

               if (Request.QueryString["VehicleId"] != null)
                  sn.History.VehicleId = Convert.ToInt64(Request.QueryString["VehicleId"]);

               if (Request.QueryString["strFromDate"] != null)
                  sn.History.FromDate = Request.QueryString["strFromDate"].ToString();

               if (Request.QueryString["strToDate"] != null)
                  sn.History.ToDate = Request.QueryString["strToDate"].ToString();

               if (Request.QueryString["VehicleName"] != null)
                  sn.History.VehicleName = Request.QueryString["VehicleName"].ToString();

               if (Request.QueryString["FleetName"] != null)
                  sn.History.FleetName = Request.QueryString["FleetName"].ToString();

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
                  dgHistoryDetails.ClearCachedDataSource();
                  dgHistoryDetails.RebindDataSource();
                  return;
               }

               // Redraw Map
               if (CoordInX != null)
               {
                  if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
                  {
                     dgStops.LayoutSettings.ClientVisible = false;
                     dgHistoryDetails.ClearCachedDataSource();
                     dgHistoryDetails.RebindDataSource();
                  }
                  else
                  {
                     dgHistoryDetails.LayoutSettings.ClientVisible = false;
                     dgHistoryDetails.ClearCachedDataSource();
                     dgHistoryDetails.RebindDataSource();
                  }
                  RedrawMapByXY(CoordInX, CoordInY, CoordEndX, CoordEndY);
               }
               else
               {
                  if ((sn.History.FromDate != "") && (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle))
                  {
                     dgStops.LayoutSettings.ClientVisible = false;
                     dgHistoryDetails.LayoutSettings.ClientVisible = true;
                     dgHistory_Fill_NewTZ(Convert.ToInt64(sn.History.VehicleId), sn.History.FromDate.ToString(), sn.History.ToDate.ToString());
                  }

                  if ((sn.History.FromDate != "") && (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                  {
                     dgStops.LayoutSettings.ClientVisible = true;
                     dgHistoryDetails.LayoutSettings.ClientVisible = false;
                     dgStops_Fill_NewTZ(Convert.ToInt64(sn.History.VehicleId), sn.History.FromDate.ToString(), sn.History.ToDate.ToString());
                  }

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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
         }
      }

      // Changes for TimeZone Feature start
      private void dgHistory_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
      {
          try
          {
              dsHistory = new DataSet();
              StringReader strrXML = null;

              string xml = "";
              ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
              bool RequestOverflowed = false;

              if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLang_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml, ref RequestOverflowed), false))
                  if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLang_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml, ref RequestOverflowed), false))
                  {
                      if (RequestOverflowed)
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;
                          sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                          ShowErrorMessage();
                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                      else
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;

                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                  }

              if (RequestOverflowed)
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                  ShowErrorMessage();
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              strrXML = new StringReader(xml);
              if (xml == "")
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;

                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }
              //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
              //string strPath = MapPath(DataSetPath) + @"\Hist.xsd";
              string strPath = Server.MapPath("../Datasets/Hist.xsd");
              //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, strPath + " Form:DataSetPass"));

              try
              {
                  dsHistory.ReadXmlSchema(strPath);
                  dsHistory.ReadXml(strrXML);
              }
              catch (Exception Ex)
              {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DataSet Path: " + strPath + " frmHistExtended.aspx"));
              }

              int RowsCount = dsHistory.Tables[0].Rows.Count - 1;
              sn.MessageText = "";

              if (dsHistory.Tables[0].Columns.IndexOf("StreetAddress") == -1)
              {
                  DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                  dsHistory.Tables[0].Columns.Add(colStreetAddress);
              }

              DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
              dsHistory.Tables[0].Columns.Add(colDateTime);
              // Show Heading
              DataColumn dc = new DataColumn();
              dc.ColumnName = "MyHeading";
              dc.DataType = Type.GetType("System.String");
              dc.DefaultValue = "";
              dsHistory.Tables[0].Columns.Add(dc);

              // DataGrid Key

              dc = new DataColumn();
              dc.ColumnName = "dgKey";
              dc.DataType = Type.GetType("System.String");
              dc.DefaultValue = "";
              dsHistory.Tables[0].Columns.Add(dc);

              // CustomUrl

              dc = new DataColumn();
              dc.ColumnName = "CustomUrl";
              dc.DataType = Type.GetType("System.String");
              dc.DefaultValue = "";
              dsHistory.Tables[0].Columns.Add(dc);

              dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
              dc.DefaultValue = true;
              dsHistory.Tables[0].Columns.Add(dc);

              int i = 0;
              string strStreetAddress = "";

              UInt64 checkBit = 0;
              Int16 bitnum = 31;
              UInt64 shift = 1;
              UInt64 intSensorMask = 0;

              foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
              {
                  //Key 
                  rowItem["dgKey"] = i.ToString();
                  //CustomUrl 
                  rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";
                  // Date
                  rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());
                  // Heading
                  if (dsHistory.Tables[0].Columns.IndexOf("Speed") != -1)
                  {
                      if ((rowItem["Speed"].ToString() != "0") && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.defNA))
                      {
                          if ((rowItem["Heading"] != null) &&
                              (rowItem["Heading"].ToString() != "") && (rowItem["Heading"].ToString() != VLF.CLS.Def.Const.blankValue))
                          {
                              rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                          }
                      }
                  }
                  if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling)))
                      rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                  i++;

                  if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                        && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                  {
                      rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
                  }

                  strStreetAddress = rowItem["StreetAddress"].ToString().Trim();

                  switch (strStreetAddress)
                  {
                      case VLF.CLS.Def.Const.addressNA:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                          break;

                      case VLF.CLS.Def.Const.noGPSData:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                          break;

                      case VLF.CLS.Def.Const.noValidAddress:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                          break;

                      default:
                          break;
                  }
                  //Disable Speed for Store Position
                  try
                  {
                      //Test for wrong Sensor Mask
                      try
                      {
                          intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                      }
                      catch
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
                      }

                      checkBit = shift << bitnum;
                      //check bit for store position 
                      if ((intSensorMask & checkBit) == checkBit)
                      {
                          rowItem["MyHeading"] = "";
                          rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                      }
                  }
                  catch
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error to disable speed for SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
                  }
              }
              sn.History.DsHistoryInfo = dsHistory;
              dgHistoryDetails.ClearCachedDataSource();
              dgHistoryDetails.RebindDataSource();

              xml = "";
              ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
              //Get Vehicle IconType
              if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), false))
                  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), true))
                  {
                      return;
                  }

              strrXML = new StringReader(xml);
              if (xml == "")
                  return;

              DataSet dsVehicle = new DataSet();
              dsVehicle.ReadXml(strrXML);

              sn.History.IconTypeName = dsVehicle.Tables[0].Rows[0]["IconTypeName"].ToString().TrimEnd();

              if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
              {
                  string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                  strUrl = strUrl + "	var myname='Message';";
                  strUrl = strUrl + " var w=370;";
                  strUrl = strUrl + " var h=50;";
                  strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                  strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                  strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                  strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                  strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                  Response.Write(strUrl);
              }
          }
          catch (NullReferenceException Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
              RedirectToLogin();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
          }
      }
      // Changes for TimeZone Feature end

      private void dgHistory_Fill(Int64 VehicleId, string strFromDate, string strToDate)
      {
          try
          {
              dsHistory = new DataSet();
              StringReader strrXML = null;

              string xml = "";
              ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
              bool RequestOverflowed = false;

              if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLang(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml, ref RequestOverflowed), false))
                  if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLang(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml, ref RequestOverflowed), false))
                  {
                      if (RequestOverflowed)
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;
                          sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                          ShowErrorMessage();
                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                      else
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;

                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                  }

              if (RequestOverflowed)
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                  ShowErrorMessage();
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              strrXML = new StringReader(xml);
              if (xml == "")
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;

                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }
              //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
              //string strPath = MapPath(DataSetPath) + @"\Hist.xsd";
              string strPath = Server.MapPath("../Datasets/Hist.xsd");
              //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, strPath + " Form:DataSetPass"));

              try
              {
                  dsHistory.ReadXmlSchema(strPath);
                  dsHistory.ReadXml(strrXML);
              }
              catch (Exception Ex)
              {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "DataSet Path: " + strPath + " frmHistExtended.aspx"));
              }

              int RowsCount = dsHistory.Tables[0].Rows.Count - 1;
              sn.MessageText = "";

              if (dsHistory.Tables[0].Columns.IndexOf("StreetAddress") == -1)
              {
                  DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                  dsHistory.Tables[0].Columns.Add(colStreetAddress);
              }

              DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
              dsHistory.Tables[0].Columns.Add(colDateTime);
              // Show Heading
              DataColumn dc = new DataColumn();
              dc.ColumnName = "MyHeading";
              dc.DataType = Type.GetType("System.String");
              dc.DefaultValue = "";
              dsHistory.Tables[0].Columns.Add(dc);

              // DataGrid Key

              dc = new DataColumn();
              dc.ColumnName = "dgKey";
              dc.DataType = Type.GetType("System.String");
              dc.DefaultValue = "";
              dsHistory.Tables[0].Columns.Add(dc);

              // CustomUrl

              dc = new DataColumn();
              dc.ColumnName = "CustomUrl";
              dc.DataType = Type.GetType("System.String");
              dc.DefaultValue = "";
              dsHistory.Tables[0].Columns.Add(dc);

              dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
              dc.DefaultValue = true;
              dsHistory.Tables[0].Columns.Add(dc);

              int i = 0;
              string strStreetAddress = "";

              UInt64 checkBit = 0;
              Int16 bitnum = 31;
              UInt64 shift = 1;
              UInt64 intSensorMask = 0;

              foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
              {
                  //Key 
                  rowItem["dgKey"] = i.ToString();
                  //CustomUrl 
                  rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";
                  // Date
                  rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());
                  // Heading
                  if (dsHistory.Tables[0].Columns.IndexOf("Speed") != -1)
                  {
                      if ((rowItem["Speed"].ToString() != "0") && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.defNA))
                      {
                          if ((rowItem["Heading"] != null) &&
                              (rowItem["Heading"].ToString() != "") && (rowItem["Heading"].ToString() != VLF.CLS.Def.Const.blankValue))
                          {
                              rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                          }
                      }
                  }
                  if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)) ||
                      (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling)))
                      rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                  i++;

                  if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                        && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                  {
                      rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
                  }

                  strStreetAddress = rowItem["StreetAddress"].ToString().Trim();

                  switch (strStreetAddress)
                  {
                      case VLF.CLS.Def.Const.addressNA:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                          break;

                      case VLF.CLS.Def.Const.noGPSData:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                          break;

                      case VLF.CLS.Def.Const.noValidAddress:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                          break;

                      default:
                          break;
                  }
                  //Disable Speed for Store Position
                  try
                  {
                      //Test for wrong Sensor Mask
                      try
                      {
                          intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                      }
                      catch
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
                      }

                      checkBit = shift << bitnum;
                      //check bit for store position 
                      if ((intSensorMask & checkBit) == checkBit)
                      {
                          rowItem["MyHeading"] = "";
                          rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                      }
                  }
                  catch
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error to disable speed for SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
                  }
              }
              sn.History.DsHistoryInfo = dsHistory;
              dgHistoryDetails.ClearCachedDataSource();
              dgHistoryDetails.RebindDataSource();

              xml = "";
              ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
              //Get Vehicle IconType
              if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), false))
                  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), true))
                  {
                      return;
                  }

              strrXML = new StringReader(xml);
              if (xml == "")
                  return;

              DataSet dsVehicle = new DataSet();
              dsVehicle.ReadXml(strrXML);

              sn.History.IconTypeName = dsVehicle.Tables[0].Rows[0]["IconTypeName"].ToString().TrimEnd();

              if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
              {
                  string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                  strUrl = strUrl + "	var myname='Message';";
                  strUrl = strUrl + " var w=370;";
                  strUrl = strUrl + " var h=50;";
                  strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                  strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                  strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                  strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                  strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                  Response.Write(strUrl);
              }
          }
          catch (NullReferenceException Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
              RedirectToLogin();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
          }
      }

      // Changes for TimeZone Feature start
      private void dgStops_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
      {
          try
          {
              dsHistory = new DataSet();
              StringReader strrXML = null;

              string xml = "";

              DataSet ds = new DataSet();
              ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

              if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                  {
                      sn.History.ImgPath = sn.Map.DefaultImgPath;
                      sn.Map.VehiclesMappings = "";
                      sn.Map.VehiclesToolTip = "";
                      return;
                  }

              if (xml == "")
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              strrXML = new StringReader(xml);
              ds.ReadXml(strrXML);

              string LicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();

              string xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, LicensePlate) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopFourthParamName, true.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopFifthParamName, sn.History.ReportstopDuration.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopSixthParamName, sn.History.ShowStops.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopSeventhParamName, sn.History.ShowIdle.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopEighthParamName, "3");

              ServerReports.Reports rpt = new ServerReports.Reports();

              bool RequestOverflowed = false;
              bool OutMaxOverflowed = false;
              int TotalSqlRecords = 0;
              int OutMaxRecords = 0;

              xml = "";

              if (objUtil.ErrCheck(rpt.GetXml_NewTZ(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), false))
                  if (objUtil.ErrCheck(rpt.GetXml_NewTZ(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), true))
                  {
                      if (RequestOverflowed)
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;
                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                      else
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;
                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                  }

              if (RequestOverflowed)
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              strrXML = new StringReader(xml);

              if (xml == "")
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              dsHistory.ReadXml(strrXML);

              if (dsHistory.Tables.IndexOf("StopData") == -1)
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
              dc.DefaultValue = true;
              dsHistory.Tables["StopData"].Columns.Add(dc);

              foreach (DataRow rowItem in dsHistory.Tables["StopData"].Rows)
              {
                  // Date
                  if (rowItem["StopDuration"].ToString().TrimEnd() == "00:00:00")
                      rowItem["StopDuration"] = VLF.CLS.Def.Const.blankValue;

                  rowItem["ArrivalDateTime"] = Convert.ToDateTime(rowItem["ArrivalDateTime"].ToString());

                  if (Convert.ToDateTime(rowItem["DepartureDateTime"]) == VLF.CLS.Def.Const.unassignedDateTime)
                      rowItem["DepartureDateTime"] = VLF.CLS.Def.Const.blankValue;
                  else
                      rowItem["DepartureDateTime"] = Convert.ToDateTime(rowItem["DepartureDateTime"].ToString());
              }
              sn.History.DsHistoryInfo = dsHistory;
              dgStops.ClearCachedDataSource();
              dgStops.RebindDataSource();
          }
          catch (NullReferenceException Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
              RedirectToLogin();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
          }
      }
      // Changes for TimeZone Feature end

      private void dgStops_Fill(Int64 VehicleId, string strFromDate, string strToDate)
      {
          try
          {
              dsHistory = new DataSet();
              StringReader strrXML = null;

              string xml = "";

              DataSet ds = new DataSet();
              ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

              if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                  if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                  {
                      sn.History.ImgPath = sn.Map.DefaultImgPath;
                      sn.Map.VehiclesMappings = "";
                      sn.Map.VehiclesToolTip = "";
                      return;
                  }

              if (xml == "")
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              strrXML = new StringReader(xml);
              ds.ReadXml(strrXML);

              string LicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();

              string xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, LicensePlate) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopFourthParamName, true.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopFifthParamName, sn.History.ReportstopDuration.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopSixthParamName, sn.History.ShowStops.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopSeventhParamName, sn.History.ShowIdle.ToString()) +
                  ReportTemplate.MakePair(ReportTemplate.RpStopEighthParamName, "3");

              ServerReports.Reports rpt = new ServerReports.Reports();

              bool RequestOverflowed = false;
              bool OutMaxOverflowed = false;
              int TotalSqlRecords = 0;
              int OutMaxRecords = 0;

              xml = "";

              if (objUtil.ErrCheck(rpt.GetXml(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), false))
                  if (objUtil.ErrCheck(rpt.GetXml(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), true))
                  {
                      if (RequestOverflowed)
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;
                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                      else
                      {
                          sn.History.ImgPath = sn.Map.DefaultImgPath;
                          sn.Map.VehiclesMappings = "";
                          sn.Map.VehiclesToolTip = "";
                          return;
                      }
                  }

              if (RequestOverflowed)
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              strrXML = new StringReader(xml);

              if (xml == "")
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              dsHistory.ReadXml(strrXML);

              if (dsHistory.Tables.IndexOf("StopData") == -1)
              {
                  sn.History.ImgPath = sn.Map.DefaultImgPath;
                  sn.Map.VehiclesMappings = "";
                  sn.Map.VehiclesToolTip = "";
                  return;
              }

              DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
              dc.DefaultValue = true;
              dsHistory.Tables["StopData"].Columns.Add(dc);

              foreach (DataRow rowItem in dsHistory.Tables["StopData"].Rows)
              {
                  // Date
                  if (rowItem["StopDuration"].ToString().TrimEnd() == "00:00:00")
                      rowItem["StopDuration"] = VLF.CLS.Def.Const.blankValue;

                  rowItem["ArrivalDateTime"] = Convert.ToDateTime(rowItem["ArrivalDateTime"].ToString());

                  if (Convert.ToDateTime(rowItem["DepartureDateTime"]) == VLF.CLS.Def.Const.unassignedDateTime)
                      rowItem["DepartureDateTime"] = VLF.CLS.Def.Const.blankValue;
                  else
                      rowItem["DepartureDateTime"] = Convert.ToDateTime(rowItem["DepartureDateTime"].ToString());
              }
              sn.History.DsHistoryInfo = dsHistory;
              dgStops.ClearCachedDataSource();
              dgStops.RebindDataSource();
          }
          catch (NullReferenceException Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
              RedirectToLogin();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
         }
      }

      protected void btnZoomIn_Click(object sender, System.EventArgs e)
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

      protected void btnZoomOut_Click(object sender, System.EventArgs e)
      {
         this.btnZoomIn.BackColor = Color.White;
         this.btnCountryLevel.BackColor = Color.White;
         this.btnMaxZoom.BackColor = Color.White;
         this.btnRegionLevel1.BackColor = Color.White;
         this.btnRegionLevel2.BackColor = Color.White;
         this.btnStreetLevel1.BackColor = Color.White;
         this.btnStreetLevel2.BackColor = Color.White;
         this.btnZoomOut.BackColor = Color.Gray;

         ZoomInMap(false, 2);
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
         }
      }

      private void cmdChangeCriteria_Click(object sender, System.EventArgs e)
      {
         Response.Redirect("frmHistoryCriteria.aspx");
      }

      protected void cmdRecenter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
      {
         sn.Map.SelectedZoomLevelType = 0;

         if ((sn != null) && (map.MapCenterLatitude != 0) && (map.MapCenterLongitude != 0))
            LoadVehiclesMap();
         else
            LoadDefaultMap();
      }

      private void dgHistoryDetails_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
      {
         if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
         {
            if (sn.User.UnitOfMes == 1)
               ViewState["UnitOfMes"] = "km";
            if (sn.User.UnitOfMes == 0.6214)
               ViewState["UnitOfMes"] = "mi";
            if ((e.Item.Cells[3].Text != VLF.CLS.Def.Const.blankValue) && (e.Item.Cells[3].Text != VLF.CLS.Def.Const.defNA.ToString()))
               e.Item.Cells[3].Text += " " + ViewState["UnitOfMes"] + "/h ";
            if ((e.Item.Cells[10].Text.TrimEnd() != "") && (e.Item.Cells[10].Text.TrimEnd() != "&nbsp;"))
               e.Item.Cells[3].Text += ", " + e.Item.Cells[10].Text.TrimEnd();

            try
            {
                if (!clsUtility.IsNumeric(e.Item.Cells[4].Text.ToString().TrimEnd())
               || !clsUtility.IsNumeric(e.Item.Cells[5].Text.ToString().TrimEnd())
               || (Convert.ToDouble(e.Item.Cells[4].Text.ToString().TrimEnd()) == 0)
               || (Convert.ToDouble(e.Item.Cells[5].Text.ToString().TrimEnd()) == 0))
               {
                  e.Item.Cells[11].Enabled = false;
                  return;
               }
            }
            catch  {}
         }
      }

      private void btnZoomOut_Click(object sender, System.Web.UI.ImageClickEventArgs e)
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

      private void btnZoomIn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
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

      protected void cmdCenterMap_Click(object sender, System.EventArgs e)
      {
         try
         {
            if ((sn == null) || (sn.UserName == ""))
            {
               RedirectToLogin();
            }

            if (sn.History.DsHistoryInfo == null)
            {
               return;
            }
            // Retrieves Map from GeoMicro
            // restore map states
            RetrievesMapStateFromViewState(map);
            map.DrawAllVehicles = true;

            sn.History.ShowToolTip = true;

            LoadVehiclesMap();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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

                     sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + 1 + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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
            sn.Map.DrawLandmarks(sn, ref map);   

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
                  if (!sn.History.ShowBreadCrumb)
                  {
                     foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                     {
                        if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue))
                        {
                           if (dr["Speed"].ToString() != "0")
                              map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
                           else
                              map.Vehicles.Add(new MapIcon(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), "Red" + sn.History.IconTypeName + ".ico", dr["OriginDateTime"].ToString().TrimEnd()));
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
         }
      }

      private void FindExistingPreference()
      {
         try
         {
            if (sn.User.UnitOfMes == 1)
               ViewState["UnitOfMes"] = "km";
            if (sn.User.UnitOfMes == 0.6214)
               ViewState["UnitOfMes"] = "mi";
            //Changes for TimeZone Feature start
            if (sn.User.NewFloatTimeZone < 0)
               ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
            else
                ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")";//Changes for TimeZone Feature end
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
         }
      }

      private void cmdShowAll_Click(object sender, System.EventArgs e) {}

      private string Heading(string heading)
      {
         return sn.Map.Heading(heading);
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
                  this.btnZoomIn.BackColor = Color.White;
                  this.btnCountryLevel.BackColor = Color.White;
                  this.btnMaxZoom.BackColor = Color.Gray;
                  this.btnRegionLevel1.BackColor = Color.White;
                  this.btnRegionLevel2.BackColor = Color.White;
                  this.btnStreetLevel1.BackColor = Color.White;
                  this.btnStreetLevel2.BackColor = Color.White;
                  this.btnZoomOut.BackColor = Color.White;
               }
               break;
            case VLF.MAP.MapZoomLevel.StreetLevelNorm5:
            case VLF.MAP.MapZoomLevel.StreetLevelNorm6:
            case VLF.MAP.MapZoomLevel.StreetLevelLow:
            case VLF.MAP.MapZoomLevel.StreetLevelLow2:
               {
                  this.btnZoomIn.BackColor = Color.White;
                  this.btnCountryLevel.BackColor = Color.White;
                  this.btnMaxZoom.BackColor = Color.White;
                  this.btnRegionLevel1.BackColor = Color.White;
                  this.btnRegionLevel2.BackColor = Color.White;
                  this.btnStreetLevel1.BackColor = Color.Gray;
                  this.btnStreetLevel2.BackColor = Color.White;
                  this.btnZoomOut.BackColor = Color.White;
               }
               break;
            case VLF.MAP.MapZoomLevel.StreetLevelMin:
            case VLF.MAP.MapZoomLevel.StreetLevelMin2:
            case VLF.MAP.MapZoomLevel.RegionLevelMax:
            case VLF.MAP.MapZoomLevel.RegionLevelMax2:
            case VLF.MAP.MapZoomLevel.RegionLevelHigh:
            case VLF.MAP.MapZoomLevel.RegionLevelHigh2:
               {
                  this.btnZoomIn.BackColor = Color.White;
                  this.btnCountryLevel.BackColor = Color.White;
                  this.btnMaxZoom.BackColor = Color.White;
                  this.btnRegionLevel1.BackColor = Color.White;
                  this.btnRegionLevel2.BackColor = Color.White;
                  this.btnStreetLevel1.BackColor = Color.White;
                  this.btnStreetLevel2.BackColor = Color.Gray;
                  this.btnZoomOut.BackColor = Color.White;
               }
               break;
            case VLF.MAP.MapZoomLevel.RegionLevelNorm:
            case VLF.MAP.MapZoomLevel.RegionLevelNorm2:
            case VLF.MAP.MapZoomLevel.RegionLevelLow:
            case VLF.MAP.MapZoomLevel.RegionLevelLow2:
               {
                  this.btnZoomIn.BackColor = Color.White;
                  this.btnCountryLevel.BackColor = Color.White;
                  this.btnMaxZoom.BackColor = Color.White;
                  this.btnRegionLevel1.BackColor = Color.Gray;
                  this.btnRegionLevel2.BackColor = Color.White;
                  this.btnStreetLevel1.BackColor = Color.White;
                  this.btnStreetLevel2.BackColor = Color.White;
                  this.btnZoomOut.BackColor = Color.White;
               }
               break;
            case VLF.MAP.MapZoomLevel.RegionLevelMin:
            case VLF.MAP.MapZoomLevel.RegionLevelMin2:
            case VLF.MAP.MapZoomLevel.CountryLevelMax:
            case VLF.MAP.MapZoomLevel.CountryLevelMax2:
            case VLF.MAP.MapZoomLevel.CountryLevelHigh:
            case VLF.MAP.MapZoomLevel.CountryLevelHigh2:
               {
                  this.btnZoomIn.BackColor = Color.White;
                  this.btnCountryLevel.BackColor = Color.White;
                  this.btnMaxZoom.BackColor = Color.White;
                  this.btnRegionLevel1.BackColor = Color.White;
                  this.btnRegionLevel2.BackColor = Color.Gray;
                  this.btnStreetLevel1.BackColor = Color.White;
                  this.btnStreetLevel2.BackColor = Color.White;
                  this.btnZoomOut.BackColor = Color.White;
               }
               break;
            case VLF.MAP.MapZoomLevel.CountryLevelNorm:
            case VLF.MAP.MapZoomLevel.CountryLevelNorm2:
            case VLF.MAP.MapZoomLevel.CountryLevelLow:
            case VLF.MAP.MapZoomLevel.CountryLevelLow2:
            case VLF.MAP.MapZoomLevel.CountryLevelMin:
            case VLF.MAP.MapZoomLevel.CountryLevelMin2:
               {
                  this.btnZoomIn.BackColor = Color.White;
                  this.btnCountryLevel.BackColor = Color.Gray;
                  this.btnMaxZoom.BackColor = Color.White;
                  this.btnRegionLevel1.BackColor = Color.White;
                  this.btnRegionLevel2.BackColor = Color.White;
                  this.btnStreetLevel1.BackColor = Color.White;
                  this.btnStreetLevel2.BackColor = Color.White;
                  this.btnZoomOut.BackColor = Color.White;
               }
               break;
            default:
               break;
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

            if (!sn.History.ShowBreadCrumb)
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

               sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
         }
      }

      protected void dgStops_SelectedIndexChanged(object sender, System.EventArgs e) {}

      private void cmdShowAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
      {
         sn.Map.VehiclesMappings = "";
         sn.Map.VehiclesToolTip = "";
         sn.History.CarLatitude = "";
         sn.History.CarLongitude = "";
         sn.History.MapCenterLongitude = "";
         sn.History.MapCenterLatitude = "";
         sn.History.ShowToolTip = true;
         LoadVehiclesMap();
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

            sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
         }
      }

       protected void cmdStopLegends_Click(object sender, System.Web.UI.ImageClickEventArgs e)
      {
         string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
         strUrl = strUrl + "	var myname='Legend';";
         strUrl = strUrl + " var w=240;";
         strUrl = strUrl + " var h=230;";
         strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
         strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
         strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
         strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

         strUrl = strUrl + " NewWindow('frmHistoryLegend.aspx');</script>";
         Response.Write(strUrl);
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
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
            LoadVehiclesMap();
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
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            VLF.ERR.LOG.LogFile(ConfigurationSettings.AppSettings["LogFolder"], "SentinelFM", "Error:" + Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx");
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
    
      protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.dgStops.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
         this.dgHistoryDetails.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
         sn.History.DgItemsPerPage = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

         if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            dgStops.ClearCachedDataSource();
            dgStops.RebindDataSource();
         }
         else
         {
            dgHistoryDetails.ClearCachedDataSource();
            dgHistoryDetails.RebindDataSource();
         }
      }

      protected void dgHistoryDetails_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
      {
         sn.History.CarLatitude = e.Row.Cells.GetNamedItem("Latitude").Text;
         sn.History.CarLongitude = e.Row.Cells.GetNamedItem("Longitude").Text;
         sn.History.CarSpeed = e.Row.Cells.GetNamedItem("Speed").Text;
         sn.History.CarHistoryDate = e.Row.Cells.GetNamedItem("MyDateTime").Text;
         sn.History.CarMessageType = e.Row.Cells.GetNamedItem("BoxMsgInTypeName").Text;
         sn.History.CarAddress = e.Row.Cells.GetNamedItem("StreetAddress").Text;
         sn.History.OriginDateTime = e.Row.Cells.GetNamedItem("MyDateTime").Text;
         try
         {
            sn.History.Heading = e.Row.Cells.GetNamedItem("MyHeading").Text;
         }
         catch
         {
            sn.History.Heading = "";
         }
         dgHistoryDetails.ClientAction.RefreshModifiedControls();
      }

      protected void dgStops_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
         if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
            && (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            e.DataSource = sn.History.DsHistoryInfo.Tables["StopData"];
         }
      }

      protected void dgStops_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
      {
         sn.Map.VehiclesToolTip = "";
         sn.Map.VehiclesMappings = "";

         sn.History.StopIndex = Convert.ToInt32(e.Row.Cells.GetNamedItem("StopIndex").Text);
         sn.History.CarLatitude = e.Row.Cells.GetNamedItem("Latitude").Text;
         sn.History.CarLongitude = e.Row.Cells.GetNamedItem("Longitude").Text;
         sn.History.StopStatus = e.Row.Cells.GetNamedItem("Remarks").Text;
         sn.History.StopDate = e.Row.Cells.GetNamedItem("ArrivalDateTime").Text;
         sn.History.StopDuration = e.Row.Cells.GetNamedItem("StopDuration").Text;
         sn.History.StopAddress = e.Row.Cells.GetNamedItem("Location").Text;

         sn.History.CarSpeed = "0";
         sn.History.OriginDateTime = "";
         sn.History.Heading = "";
         sn.History.ShowToolTip = false;
         sn.History.StopDurationVal = Convert.ToInt32(e.Row.Cells.GetNamedItem("StopDurationVal").Text);

         TimeSpan TripIndling;
         TripIndling = new TimeSpan(Convert.ToInt64(sn.History.StopDurationVal) * TimeSpan.TicksPerSecond);
         sn.History.StopDurationVal = TripIndling.TotalMinutes;
      }

      protected void dgHistoryDetails_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
      {
         if (e.Column.Name == "MapIt")
         {
            try
            {
               sn.Map.VehiclesToolTip = "";
               sn.Map.VehiclesMappings = "";

               sn.History.ShowToolTip = false;

               if ((sn.History.CarLatitude == "0") || (sn.History.CarLongitude == "0") || (sn.History.CarLatitude == VLF.CLS.Def.Const.blankValue))
               {
                  LoadDefaultMap();
                  return;
               }

               map.Vehicles.DrawLabels = false;
               map.Vehicles.Clear();
               map.Landmarks.Clear();
               map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
               map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
               map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;

               if ((sn.History.CarSpeed == VLF.CLS.Def.Const.blankValue) || (sn.History.CarSpeed == "0") || (sn.History.CarSpeed == "0 km/h") || (sn.History.CarSpeed == "0 mi/h"))
                  map.Vehicles.Add(new MapIcon(Convert.ToDouble(sn.History.CarLatitude), Convert.ToDouble(sn.History.CarLongitude), "Red" + sn.History.IconTypeName + ".ico", sn.History.OriginDateTime));
               else
                  map.Vehicles.Add(new MapIcon(Convert.ToDouble(sn.History.CarLatitude), Convert.ToDouble(sn.History.CarLongitude), "Green" + sn.History.IconTypeName + sn.History.Heading + ".ico", sn.History.OriginDateTime));

              sn.Map.DrawLandmarks(sn, ref map);   

               map.DrawAllVehicles = false;
               map.Zoom(VLF.MAP.MapZoomLevel.StreetLevelNorm);
               map.MapCenterLongitude = Convert.ToDouble(sn.History.CarLongitude);
               map.MapCenterLatitude = Convert.ToDouble(sn.History.CarLatitude);

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
               //Creating ToolTip
               sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.CarHistoryDate + "]:</B><FONT style='COLOR: Purple'>" + sn.History.CarMessageType + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.CarAddress + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + sn.History.CarSpeed + "<BR>\",";
               //Table Vehicles with coordinates
               DataTable tblToolTips = new DataTable();
               DataRow tblRow;

               DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
               tblToolTips.Columns.Add(colDescription);
               DataColumn colX = new DataColumn("colX", Type.GetType("System.Double"));
               tblToolTips.Columns.Add(colX);
               DataColumn colY = new DataColumn("colY", Type.GetType("System.Double"));
               tblToolTips.Columns.Add(colY);

               if (map.Vehicles.Count > 0)
               {
                  tblRow = tblToolTips.NewRow();
                  tblRow["Description"] = "";
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

                  sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
               }
               //-- End Tooltip
               SavesMapStateToViewState(map);
               SetColorZoomLevel(map);
            }
            catch (NullReferenceException Ex)
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
               RedirectToLogin();
            }
            catch (Exception Ex)
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
            }
         }
      }

      protected void dgHistoryDetails_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
         if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
       && !(sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            e.DataSource = sn.History.DsHistoryInfo;
         }
      }

      protected void dgStops_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
      {
         if (e.Column.Name == "MapIt")
         {
            try
            {
               sn.Map.VehiclesToolTip = "";
               sn.Map.VehiclesMappings = "";

               TimeSpan TripIndling;
               TripIndling = new TimeSpan(Convert.ToInt64(sn.History.StopDurationVal) * TimeSpan.TicksPerSecond);
               sn.History.StopDurationVal = TripIndling.TotalMinutes;

               if (sn.History.CarLatitude == "0")
               {
                  LoadDefaultMap();
                  return;
               }

               map.Vehicles.Clear();
               map.Landmarks.Clear();
               map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
               map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
               map.BreadCrumbPoints.DrawLabels = sn.History.ShowBreadCrumb;

               if (sn.History.ShowStopSqNum)
                  map.Vehicles.DrawLabels = true;
               else
                  map.Vehicles.DrawLabels = false;

               if (sn.History.StopStatus == "Idling")
               {
                  map.Vehicles.Add(new MapIcon(Convert.ToDouble(sn.History.CarLatitude), Convert.ToDouble(sn.History.CarLongitude), "Idle.ico", sn.History.StopIndex.ToString()));
               }
               else
               {
                  if (sn.History.StopDurationVal < 15)
                     map.Vehicles.Add(new MapIcon(Convert.ToDouble(sn.History.CarLatitude), Convert.ToDouble(sn.History.CarLongitude), "Stop_3.ico", sn.History.StopIndex.ToString()));
                  if ((sn.History.StopDurationVal > 15) && (sn.History.StopDurationVal < 60))
                     map.Vehicles.Add(new MapIcon(Convert.ToDouble(sn.History.CarLatitude), Convert.ToDouble(sn.History.CarLongitude), "Stop_15.ico", sn.History.StopIndex.ToString()));
                  if (sn.History.StopDurationVal > 60)
                     map.Vehicles.Add(new MapIcon(Convert.ToDouble(sn.History.CarLatitude), Convert.ToDouble(sn.History.CarLongitude), "Stop_60.ico", sn.History.StopIndex.ToString()));
               }

               sn.Map.DrawLandmarks(sn, ref map);   
               map.DrawAllVehicles = false;
               map.Zoom(VLF.MAP.MapZoomLevel.StreetLevelNorm);
               map.MapCenterLongitude = Convert.ToDouble(sn.History.CarLongitude);
               map.MapCenterLatitude = Convert.ToDouble(sn.History.CarLatitude);

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
               //Creating ToolTip
               sn.Map.VehiclesToolTip += "<B>" + " [" + sn.History.StopDate + "]:</B><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + sn.History.StopAddress + "<BR>\",";
               //Table Vehicles with coordinates
               DataTable tblToolTips = new DataTable();
               DataRow tblRow;

               DataColumn colDescription = new DataColumn("Description", Type.GetType("System.String"));
               tblToolTips.Columns.Add(colDescription);
               DataColumn colX = new DataColumn("colX", Type.GetType("System.Double"));
               tblToolTips.Columns.Add(colX);
               DataColumn colY = new DataColumn("colY", Type.GetType("System.Double"));
               tblToolTips.Columns.Add(colY);

               if (map.Vehicles.Count > 0)
               {
                  tblRow = tblToolTips.NewRow();
                  tblRow["Description"] = "";
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

                  sn.Map.VehiclesMappings += "<area onmouseover=\"javascript:toolTip('" + strVehiclesList + "');document.frmHistory.imgMap.style.cursor='crosshair'\"  onmouseout=\"javascript:hide();document.frmHistory.imgMap.style.cursor='default'\" shape=\"RECT\" coords=\"" + StartX.ToString() + "," + StartY.ToString() + "," + EndX.ToString() + "," + EndY.ToString() + "\">";
               }
               //-- End Tooltip
               sn.History.ImgPath = url;

               SavesMapStateToViewState(map);
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
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " frmHistExtended.aspx"));
            }
         }
      }

      protected void dgHistoryDetails_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
      {
         try
         {
            ISNet.WebUI.WebGrid.WebGridCellCollection cells = e.Row.Cells;
            //if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record && (!IsNumeric(cells.GetNamedItem("Latitude").Text.TrimEnd())
            //|| !IsNumeric(cells.GetNamedItem("Longitude").Text.TrimEnd())
            //|| (Convert.ToDouble(cells.GetNamedItem("Latitude").Text.TrimEnd()) == 0)
            //|| (Convert.ToDouble(cells.GetNamedItem("Longitude").Text.TrimEnd()) == 0)))
            //{
            //   cells.GetNamedItem("MapIt").Column.Visible = false;
            //}
            //else
            //{
            //   cells.GetNamedItem("MapIt").Column.Visible = true;
            //}
            DataRow[] drCollections = null;
            if (sn.History.DsHistoryInfo != null)
            {
               drCollections = sn.History.DsHistoryInfo.Tables[0].Select("dgKey like '" + e.Row.Cells[1].Text + "'");

               foreach (DataRow rowItem in drCollections)
               {
                  e.Row.Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
               }
            }
         }
         catch  {}
      }

      protected void dgStops_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
      {
         try
         {
            ISNet.WebUI.WebGrid.WebGridCellCollection cells = e.Row.Cells;
            //if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record && (!IsNumeric(cells.GetNamedItem("Latitude").Text.TrimEnd())
            //|| !IsNumeric(cells.GetNamedItem("Longitude").Text.TrimEnd())
            //|| (Convert.ToDouble(cells.GetNamedItem("Latitude").Text.TrimEnd()) == 0)
            //|| (Convert.ToDouble(cells.GetNamedItem("Longitude").Text.TrimEnd()) == 0)))
            //{
            //   cells.GetNamedItem("MapIt").Column.Visible = false;
            //}
            //else
            //{
            //   cells.GetNamedItem("MapIt").Column.Visible = true;
            //}
            DataRow[] drCollections = null;
            if (sn.History.DsHistoryInfo  != null)
            {
               drCollections = sn.History.DsHistoryInfo.Tables[0].Select("StopIndex like '" + e.Row.Cells[1].Text + "'");

               foreach (DataRow rowItem in drCollections)
               {
                  e.Row.Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
               }
            }
         }
         catch {}
      }

      protected void dgHistoryDetails_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
         if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
         {
            e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
               e.Layout.TextSettings.UseLanguage = "fr-FR";
            //else
            //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
         }
      }

      protected void dgStops_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
         if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
         {
            e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
               e.Layout.TextSettings.UseLanguage = "fr-FR";
            //else
            //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
         }
      }

      protected void cboRows_SelectedIndexChanged(object sender, EventArgs e)
      {
         sn.History.DgVisibleRows = Convert.ToInt32(cboRows.SelectedItem.Value);
         dgHistoryDetails.Height = Unit.Pixel(107 + Convert.ToInt32(dgHistoryDetails.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
         dgStops.Height = Unit.Pixel(107 + Convert.ToInt32(dgStops.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));          
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

      protected void cmdMapSelected_Click(object sender, EventArgs e)
      {
         SaveShowCheckBoxes();
         LoadVehiclesMap(); 
      }
       
      private void SaveShowCheckBoxes()
      {
         if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
            {
               rowItem["chkBoxShow"] = false;
               foreach (string keyValue in dgStops.RootTable.GetCheckedRows())
               {
                  if (keyValue == rowItem["StopIndex"].ToString())
                  {
                     rowItem["chkBoxShow"] = true;
                     break;
                  }
               }
            }
         }
         else
         {
            foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
            {
               rowItem["chkBoxShow"] = false;
               foreach (string keyValue in dgHistoryDetails.RootTable.GetCheckedRows())
               {
                  if (keyValue == rowItem["dgKey"].ToString())
                  {
                     rowItem["chkBoxShow"] = true;
                     break;
                  }
               }
            }
         }
      }
   }
}
