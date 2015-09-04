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
using System.Diagnostics;
 
namespace SentinelFM
{

   public partial class Map_frmFleetInfoSimple : SentinelFMBasePage 
   {

      
      
      public string redirectURL;
      private DataSet dsFleetInfo;
      public VLF.MAP.ClientMapProxy map;
      public VLF.MAP.ClientMapProxy geoMap;
      public string strGeoMicroURL;
      public bool chkShowAutoPostBack = false;
      public int AutoRefreshTimer = 60000;

      private Stopwatch watch = new Stopwatch();
      private Stopwatch Pagewatch = new Stopwatch();
       

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {


            body.Style.Add("overflow", "hidden");
           
            double ver = getInternetExplorerVersion();
            //if (ver < 7.0)
            //   dgFleetInfo.Width = Unit.Pixel(1000);



            if (ver < 7.0)
            {
               try
               {
                  if (Request.QueryString["clientWidth"] != null)
                  {
                     sn.Map.ScreenWidth  = Convert.ToInt32(Request.QueryString["clientWidth"]);

                     if (sn.Map.ScreenWidth > 1000)
                        dgFleetInfo.Width = Convert.ToInt32(sn.User.ScreenWidth) +220;
                     else
                        dgFleetInfo.Width = Unit.Pixel(990);
                  }
                  else
                  {
                     if (sn.Map.ScreenWidth != 0 && sn.Map.ScreenWidth > 1000)
                        dgFleetInfo.Width = Convert.ToInt32(sn.Map.ScreenWidth) + 220;
                     else
                        dgFleetInfo.Width = Unit.Pixel(990);
                  }
               }
               catch
               {
                  dgFleetInfo.Width = Unit.Pixel(990);
               }

                
            }
            
            AutoRefreshTimer = sn.User.GeneralRefreshFrequency;
          

            if (!Page.IsPostBack)
            {
               //DateTime dt = System.DateTime.Now;
                  
               //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles info started ->  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                Pagewatch.Reset();
                Pagewatch.Start();

               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref FleetForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

               try
               {
                   cboRows.SelectedIndex = cboRows.Items.IndexOf(cboRows.Items.FindByValue(sn.Map.DgVisibleRows.ToString()));
                   cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue(sn.Map.DgItemsPerPage.ToString()));
                   dgFleetInfo.Height = Unit.Pixel(50 + Convert.ToInt32(dgFleetInfo.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                   this.dgFleetInfo.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
                   if (sn.User.ShowMapGridFilter == 1)
                       dgFleetInfo.LayoutSettings.AllowFilter = ISNet.WebUI.WebGrid.Filter.Yes;
                   else
                       dgFleetInfo.LayoutSettings.AllowFilter = ISNet.WebUI.WebGrid.Filter.No;
               }
               catch (Exception Ex)
               {
                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Grid Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                   System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                   ExceptionLogger(trace);

               }


               GuiSecurity(this);
               if (sn.Map.MapRefresh)
               {
                  
                  string MapRefresh = mnuOptions.Items[3].Text;
                  mnuOptions.Items[3].Text = MapRefresh.Substring(0, (MapRefresh.Length) - 3) + "[x]";
                  chkShowAutoPostBack = true;
                  Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
                  //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                  //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
               }
               else
               {
                  chkShowAutoPostBack = false;
                  Response.Write("<script language='javascript'> clearTimeout();</script>");
               }

               
               this.tblFleetActions.Visible = true;  
               this.tblWait.Visible = false;



               //Get Vehicles Info
               sn.MessageText = "";
               CboFleet_Fill();
               if (sn.MessageText != "") //No Fleet for user
                  return; 
                  
               FindExistingPreference();

               try
               {
                   if (sn.Map.SelectedFleetID != 0)
                   {
                       cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                       DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                   }
                   else
                   {
                       if (sn.User.DefaultFleet != -1)
                       {
                           cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                           DgFleetInfo_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                           sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                       }
                   }
               }
               catch (Exception Ex)
               {
                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                   System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                   ExceptionLogger(trace);

               }

                 

               //--- Check Results after update position

               sn.MessageText = "";

               if (sn.Cmd.DtUpdatePositionFails.Rows.Count > 0)
                  ShowUpdatePositionsTimeOuts();


               if (sn.Cmd.UpdatePositionSend)
               {
                  ShowUpdatePositionsNotValid();
                  sn.Cmd.UpdatePositionSend = false;
               }


               if (sn.Cmd.DtUpdatePositionFails.Rows.Count > 0)
                  sn.Cmd.DtUpdatePositionFails.Rows.Clear();

               //----------------------------------



               if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
               {
                  ShowErrorMessage();
               }


               //TimeSpan currDuration = new TimeSpan(System.DateTime.Now.Ticks - dt.Ticks);

               //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles info ended <- Duration: " + currDuration.TotalSeconds.ToString()  + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));


               if (sn.Map.ReloadMap)
               {
                   LoadMap();
                   sn.Map.ReloadMap = false;
               }

               if (sn.User.MapType == VLF.MAP.MapType.LSD)
                {
                    MenuItem mnuItem = new MenuItem();
                    mnuItem.Text = "LSD";
                    mnuItem.Value = "LSD";
                    this.mnuOptions.Items.Add(mnuItem);   
                 }

                 Pagewatch.Stop();
                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Map DataGrid Page Load -->(sec):" + Pagewatch.Elapsed.TotalSeconds));


               
            }
         }

         catch (NullReferenceException Ex)

         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

      private void CboFleet_Fill()
      {
         try
         {
            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);  
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));

         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", CboFleet_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }

      }

      private void DgFleetInfo_Fill(int fleetId)
      {
         try
         {
            Stopwatch watch = new Stopwatch();
            dsFleetInfo = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            //if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoWithPeripheral  (sn.UserID, sn.SecId, fleetId, ref xml), false))
            //   if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoWithPeripheral(sn.UserID, sn.SecId, fleetId, ref xml), true))
            //   {
            //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "GetVehiclesLastKnownPositionInfo.User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //      return;
            //   }

            //DumpBeforeCall(sn, string.Format("DgFleetInfo_Fill : fleetId = {0}", fleetId));


            watch.Reset();
            watch.Start();
            VLF.DAS.Logic.Fleet dbFleet = new VLF.DAS.Logic.Fleet("Initial Catalog=SentinelFM;Data Source=192.168.9.41;User ID=sa;Password=BSMwireless1;Pooling=true;Max Pool Size=300;");

            dsFleetInfo = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, sn.UserID);
 
            //if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfo(sn.UserID, sn.SecId, fleetId, ref xml), false))
            //   if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfo(sn.UserID, sn.SecId, fleetId, ref xml), true))
            //   {
            //      sn.Map.DsFleetInfo = null;
            //      return;
            //   }

            //if (xml == "")
            //{
            //   sn.Map.DsFleetInfo = null;
            //   return;
            //}

            //strrXML = new StringReader(xml);
            //dsFleetInfo.ReadXml(strrXML);

            watch.Stop();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, 
                VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get Map Screen Data -->Database Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + fleetId));


            watch.Reset();
            watch.Start();

            sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
            sn.Map.DsFleetInfo = dsFleetInfo;

            watch.Stop();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get Map Screen Data Manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + fleetId));


            //LoadNetistixData();
            


         }

         catch (NullReferenceException Ex)
         {
            ExceptionLogger("NullReferenceException " , Ex);
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }



      protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         try
         {

            sn.Map.DsFleetInfo = null;
            dgFleetInfo.RootTable.Rows.Clear();   
            
    
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
               sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
               DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }

            dgFleetInfo.ClearCachedDataSource();
            dgFleetInfo.RebindDataSource();



            if (sn.User.MapType == VLF.MAP.MapType.LSD)
                  Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
              else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                  Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx' </script>");
              else if (sn.User.MapType != VLF.MAP.MapType.MapsoluteWeb)
                  Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
                

         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cboFleet_SelectedIndexChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

     

      protected void cmdMapIT()
      {
         try
         {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {

               sn.MessageText = "";
               SaveShowCheckBoxes();

               dgFleetInfo.ClearCachedDataSource();
               dgFleetInfo.RebindDataSource();

               //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
               //   sn.MapSolute.ClearMapObjects(sn.MapSolute.MapObjectsList);

               bool VehicleSelected = false;
               foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
               {
                  if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                  {
                     VehicleSelected = true;
                     break;
                  }
               }

               if (VehicleSelected)
               {
                   LoadMap();
                  //Check for Old DateTime
                  string strMessage = "";
                  strMessage = CheckDateTime();
                  if (strMessage != "")
                  {

                     sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle1") + ":" + strMessage + " " + (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle2");
                     ShowErrorMessage();
                  }

               }
               else
               {

                  sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                  ShowErrorMessage();
                  return;
               }

            }
            else
            {
               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
               ShowErrorMessage();
            }
         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cmdMapIT--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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


      protected void cmdUpdatePosition()
      {
         try
         {

            SaveShowCheckBoxes();

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataColumn dc;

            dc = new DataColumn("Freq", Type.GetType("System.Int64"));
            dc.DefaultValue = 0;
            dt.Columns.Add(dc);


            bool cmdSent = false;

            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {

               sn.MessageText = "";
               LocationMgr.Location dbl = new LocationMgr.Location();
               bool VehicleSelected = false;
               SaveShowCheckBoxes();
               //Delete old timeouts
               sn.Cmd.DtUpdatePositionFails.Rows.Clear();
               bool ShowTimer = false;
               Int64 sessionTimeOut = 0;

               //DumpBeforeCall(sn, string.Format("frmFleetInfoNew -- cmdUpdatePosition"));

                 
               foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
               {
                  if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                  {
                     short ProtocolId = -1;
                     short CommModeId = -1;
                     string errMsg = "";
                     if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                        if (errMsg == "")
                        {
                           if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                           {
                              if (errMsg != "")
                                 sn.MessageText = errMsg;
                              else
                                 sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SendCommandFailedError") + ": " + rowItem["Description"];

                              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update position failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));

                              DataRow drErr;
                              drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                              drErr["VehicleDesc"] = rowItem["Description"];
                              drErr["Status"] = sn.MessageText;
                              sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                              rowItem["Updated"] = CommandStatus.CommTimeout;

                           }
                        }
                        else
                        {
                           sn.MessageText = errMsg;
                           DataRow drErr;
                           drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                           drErr["VehicleDesc"] = rowItem["Description"];
                           drErr["Status"] = sn.MessageText;
                           sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                           rowItem["Updated"] = CommandStatus.CommTimeout;
                        }


                     DataRow dr = dt.NewRow();

                     if (sessionTimeOut > 0)
                        dr["Freq"] = Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000;
                     else
                        dr["Freq"] = 2000;

                     dt.Rows.Add(dr);

                     rowItem["ProtocolId"] = ProtocolId;
                     sn.Cmd.ProtocolTypeId = ProtocolId;

                     if (cmdSent)
                     {
                        ShowTimer = true;
                     }
                     else
                     {

                        //Create pop up message
                        sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SendCommandToVehicle1") + " :" + rowItem["Description"].ToString().TrimEnd() + " " + (string)base.GetLocalResourceObject("sn_MessageText_SendCommandToVehicle2");

                        DataRow drErr;
                        drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                        drErr["VehicleDesc"] = rowItem["Description"];
                        drErr["Status"] = sn.MessageText;
                        sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                        rowItem["Updated"] = CommandStatus.CommTimeout;

                     }

                     VehicleSelected = true;
                     rowItem["Updated"] = CommandStatus.Sent;
                  }

               }



               //sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";

               //this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
               //this.dgFleetInfo.DataBind();

               if (VehicleSelected)
               {
                  if (ShowTimer)
                  {

                     //this.dgFleetInfo.Visible = false;
                     
                     this.tblWait.Visible = true;
                     this.tblFleetActions.Visible = false;

                     DataRow[] drCollections = null;
                     drCollections = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
                     if (drCollections.Length == 1)
                     {
                        if (sessionTimeOut > 60)
                        {
                           Int64 SessionTime = Convert.ToInt64(Math.Round(sessionTimeOut / 60.0));
                           this.lblUpdatePosition.Text = (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Minutes1") + " " + SessionTime.ToString() + " " + (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Minutes2");
                        }
                        else if (sessionTimeOut == 0)
                           sn.Cmd.GetCommandStatusRefreshFreq = 15000;
                        else
                           this.lblUpdatePosition.Text = (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Seconds1") + " " + sessionTimeOut + " " + (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Seconds2");
                     }

                     try
                     {
                        ds.Tables.Add(dt);
                        DataView dv = ds.Tables[0].DefaultView;
                        dv.Sort = "Freq" + " DESC";
                        sn.Cmd.GetCommandStatusRefreshFreq = Convert.ToInt64(dv[0].Row[0]);
                     }
                     catch
                     {
                        sn.Cmd.GetCommandStatusRefreshFreq = 1000;
                     }

                     sn.Map.TimerStatus = true;
                     sn.Cmd.UpdatePositionSend = true;
                     Response.Write("<script language='javascript'> parent.frmStatus.location.href='frmTimerPosition.aspx' </script>");

                    
                  }
               }
               else
               {

                  sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                  ShowErrorMessage();
                  this.tblWait.Visible = false;
                  return;
               }


            }
            else
            {

               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
               ShowErrorMessage();

               this.tblWait.Visible = false;
            }
         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cmdUpdatePosition--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

  


      private void cmdRefresh_Click(object sender, System.EventArgs e)
      {
         try
         {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
               SaveShowCheckBoxes();
               DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));

               //Check if vehicles selected
               bool VehicleSelected = false;
               foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
               {
                  if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                     VehicleSelected = true;
               }

               if (VehicleSelected)
                   LoadMap();

            }

            else
            {
               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
               ShowErrorMessage();
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cmdRefresh_Click--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

     


      private void FindExistingPreference()
      {
         try
         {

             // Changes for TimeZone Feature start
            if (sn.User.NewFloatTimeZone < 0)
                ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
            else
                ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")";// Changes for TimeZone Feature end
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", FindExistingPreference--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }

      }




      protected void cmdCancelUpdatePos_Click(object sender, System.EventArgs e)
      {
         try
         {
            LocationMgr.Location dbl = new LocationMgr.Location();
            

            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
               if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
               {
                  if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), false))
                     if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), true))
                     {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     }

               }
            }

            CancelCommand(); 

            //Response.Write("<script language='javascript'> parent.frmFleetInfo.location.href='frmFleetInfo.aspx';</script>"); 



         }

         catch (NullReferenceException Ex)
         {
            CancelCommand();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cmdCancelUpdatePos_Click--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }


      private void CancelCommand()
      {
         //this.dgFleetInfo.Visible = true;
        
         this.tblWait.Visible = false;
         this.tblFleetActions.Visible = true;
         sn.Map.TimerStatus = false;
      }


   
  

      protected void chkAutoUpdateChanged(bool Refresh)
      {
         if (Refresh)
         {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {

               sn.Map.MapRefresh = Refresh;
               sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
               chkShowAutoPostBack = Refresh;
               RefreshPosition();
               Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
            }
            else
            {
               chkShowAutoPostBack = Refresh;
               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
               ShowErrorMessage();
               Response.Write("<script language='javascript'> clearTimeout();</script>");
            }

         }
         else
         {
            sn.Map.MapRefresh = false;
            RefreshPosition();
         }

      }


      private void RefreshPosition()
      {
         try
         {

            SaveShowCheckBoxes();
            DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));


            //Check if vehicles selected
            bool VehicleSelected = false;
            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
               if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                  VehicleSelected = true;
            }

            if (VehicleSelected)
                LoadMap();

         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", RefreshPosition--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }




      private void ShowUpdatePositionsTimeOuts()
      {
         try
         {

            sn.MessageText = "";

            // TimeOut Messages
            string strSQL = "Status=" + Convert.ToString((int)CommandStatus.CommTimeout);
            DataRow[] foundRows = sn.Cmd.DtUpdatePositionFails.Select(strSQL);

            if (foundRows.Length > 0)
            {
               sn.MessageText += (string)base.GetLocalResourceObject("sn_MessageText_CommunicationWithVehicle1") + ": ";
               foreach (DataRow rowItem in foundRows)
               {
                  sn.MessageText += rowItem["VehicleDesc"].ToString().TrimEnd() + ",";
               }


               if (sn.MessageText.Length > 0)
                  sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);

               sn.MessageText += " " + (string)base.GetLocalResourceObject("sn_MessageText_CommunicationWithVehicle2");

            }

            //clear filter
            sn.Cmd.DtUpdatePositionFails.Select();

            //Queued Messages 
            strSQL = "Status=" + Convert.ToString((int)CommandStatus.Pending);
            foundRows = sn.Cmd.DtUpdatePositionFails.Select(strSQL);


            if (foundRows.Length > 0)
            {
               sn.MessageText += " " + (string)base.GetLocalResourceObject("sn_MessageText_UpdatePositionForVehicle1") + ": ";
               foreach (DataRow rowItem in foundRows)
               {
                  sn.MessageText += rowItem["VehicleDesc"].ToString().TrimEnd() + ",";
               }

               if (sn.MessageText.Length > 0)
                  sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);

               sn.MessageText += " " + (string)base.GetLocalResourceObject("sn_MessageText_UpdatePositionForVehicle2");
            }



         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsTimeOuts--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }


      private void ShowUpdatePositionsNotValid()
      {
         try
         {
            if (sn.MessageText != "")
               sn.MessageText += "\n_________________________________________\n";

            bool InvalidExists = false;

            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
               if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
               {
                  if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                  {
                     InvalidExists = true;
                     break;
                  }
               }
            }



            //Delay for resolving "Store Position"
            System.Threading.Thread.Sleep(2000);


            if (InvalidExists)
            {
               DgFleetInfo_Fill(Convert.ToInt32(this.cboFleet.SelectedItem.Value));

               bool ShowTitle = false;

               foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
               {
                  if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                  {
                     if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                     {

                        bool UpdatePositionFails = false;

                        foreach (DataRow rw in sn.Cmd.DtUpdatePositionFails.Rows)
                        {
                           if (rw["VehicleDesc"].ToString().TrimEnd() == rowItem["VehicleDesc"].ToString().TrimEnd())
                           {
                              UpdatePositionFails = true;
                              break;
                           }
                        }

                        // If not exist Error message for this vehicle
                        if (!UpdatePositionFails)
                        {
                           if (!ShowTitle)
                           {
                              sn.MessageText += (string)base.GetLocalResourceObject("sn_MessageText_GPSPositionForVehicle1") + ": ";
                              ShowTitle = true;
                           }

                           sn.MessageText += rowItem["Description"].ToString().TrimEnd() + ",";
                        }
                     }
                  }
               }
            }


            if ((sn.MessageText.Length > 0) && (sn.MessageText.Substring(sn.MessageText.Length - 1, 1) == ","))
            {
               sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);
               sn.MessageText += "\n " + (string)base.GetLocalResourceObject("sn_MessageText_GPSPositionForVehicle2");
            }

            sn.Cmd.DtUpdatePositionFails.Rows.Clear();

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsNotValid--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }



      protected void cmdBigMap()
      {
         try
         {
            string strUrl = "";

            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {

               sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
               SaveShowCheckBoxes();

               bool VehicleSelected = false;
               foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
               {
                  if (rowItem["chkBoxShow"].ToString() == "true")
                     VehicleSelected = true;
               }

               if (VehicleSelected)
               {
                  sn.Map.DsFleetInfo.Tables[0].DefaultView.Sort = "chkBoxShow" + " DESC";
                  // dgFleetInfo.CurrentPageIndex = 0;
                  dgFleetInfo.DataSource = sn.Map.DsFleetInfo.Tables[0];
                  dgFleetInfo.DataBind();
                  //dgFleetInfo.SelectedIndex = -1;


                  //Check for Old DateTime
                  string strMessage = "";
                  strMessage = CheckDateTime();
                  if (strMessage != "")
                  {
                     sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle1") + ":" + strMessage + " " + (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle2");
                     ShowErrorMessage();
                  }

               }
               else
               {
                  sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                  ShowErrorMessage();
                  return;
               }

               strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
               strUrl = strUrl + "	var myname='';";
               strUrl = strUrl + " var w=950;";
               strUrl = strUrl + " var h=650;";
               strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
               strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
               strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=auto,fullscreen=yes,menubar=0,'; ";
               strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";
               strUrl = strUrl + " NewWindow('frmBigMapWait.aspx');</script>";
               LoadBigDefaultMap();
               Response.Write(strUrl);
               return;
            }
            else
            {

               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
               ShowErrorMessage();
               return;
            }

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cmdBigMap--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }



      protected void cmdBigDetails()
      {
         try
         {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
               sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
               SaveShowCheckBoxes();
            }

            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='';";
            strUrl = strUrl + " var w=950;";
            strUrl = strUrl + " var h=650;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=auto,fullscreen=yes,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, myname, winprops); }";

            strUrl = strUrl + " NewWindow('frmBigDetailsFrame.htm');</script>";
            Response.Write(strUrl);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cmdBigDetails--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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

      }
      #endregion

      protected void chkShowAllChanged(bool status)
      {
         try
         {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {

               sn.MessageText = "";

               DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));

               foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
               {
                  rowItem["chkBoxShow"] = status;
               }

               this.dgFleetInfo.DataSource = sn.Map.DsFleetInfo;
               this.dgFleetInfo.DataBind();



               if (sn.Map.MapRefresh)
               {
                  sn.Map.DrawAllVehicles = true;
                  chkShowAutoPostBack = true;
               }

            }

            else
            {
               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
               ShowErrorMessage();
            }
         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", chkShowAllChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }


      private void SaveShowCheckBoxes()
      {
          try
          {
              foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
              {
                  rowItem["chkBoxShow"] = false;
                  foreach (string keyValue in dgFleetInfo.RootTable.GetCheckedRows())
                  {
                      if (keyValue == rowItem["VehicleId"].ToString())
                      {
                          rowItem["chkBoxShow"] = true;
                          break;
                      }

                  }
              }
          }
         catch
          {
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



      private string CheckDateTime()
      {
         string OldDateVehicleNames = "";

         foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
         {
            if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
            {
               if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                  OldDateVehicleNames += rowItem["Description"].ToString().TrimEnd() + ",";
            }

         }

         if (OldDateVehicleNames.Length > 0)
            OldDateVehicleNames = OldDateVehicleNames.Substring(0, OldDateVehicleNames.Length - 1);


         return OldDateVehicleNames;

      }


      public void LoadBigDefaultMap()
      {
         try
         {
            // create ClientMapProxy only for mapping
            map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
            if (map == null)
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               return;
            }
            map.Vehicles.Clear();
            map.Landmarks.Clear();
            map.Landmarks.DrawLabels = sn.Map.ShowLandmarkname;
            map.Vehicles.DrawLabels = sn.Map.ShowVehicleName;
            map.GetDefaultBigMap();
            sn.Map.SavesMapStateToViewState(sn, map);
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", LoadBigDefaultMap--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

     

      protected void mnuFleetActions_MenuItemClick(object sender, MenuEventArgs e)
      {
         if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
         {
            sn.Map.SelectedFleetID = Convert.ToInt32(cboFleet.SelectedItem.Value);
            SaveShowCheckBoxes();
            bool vehicleSelected = false;
            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
               if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
               {
                  vehicleSelected = true;
                  break;
               }
            }

            if (!vehicleSelected)
            {
               sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
               ShowErrorMessage();
               return;
            }


            switch (mnuFleetActions.SelectedValue)
            {
               case "1":
                  foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                  {
                     if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                     {
                        string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                        strUrl = strUrl + "	var myname='Message';";
                        strUrl = strUrl + " var w=370;";
                        strUrl = strUrl + " var h=50;";
                        strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                        strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                        strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                        strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);}";

                        strUrl = strUrl + " NewWindow('frmFleetVehicles.aspx');</script>";

                        Response.Write(strUrl);
                        return;
                     }
                  }

                  break;
               case "2":

                  if (cboFleet.SelectedItem.Text.TrimEnd() == Resources.Const.defaultFleetName)
                  {
                     sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_CannotDeleteVehicles") + Resources.Const.defaultFleetName;
                     ShowErrorMessage();
                     return;
                  }

                  
                  ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                  foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                  {
                     if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                     {
                        if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToInt32(rowItem["vehicleId"])), false))
                           if (objUtil.ErrCheck(dbf.DeleteVehicleFromFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToInt32(rowItem["vehicleId"])), false))
                           {
                              return;
                           }
                     }
                  }

                  DgFleetInfo_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                  break;
            }
         }
         else
         {
            sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
            ShowErrorMessage();
            return;
         }
      }

     
      protected void mnuOptions_MenuItemClick(object sender, MenuEventArgs e)
      {
         switch (e.Item.Value)
         {
            case "UpdatePosition":
               cmdUpdatePosition();
               break;
            case "MapIt":
              
               cmdMapIT();

               break;
            case "AutoRefresh":

               if (e.Item.Text.Substring(e.Item.Text.Length - 3) == "[x]")
               {
                  chkAutoUpdateChanged(false);
                  e.Item.Text = e.Item.Text.Substring(0, e.Item.Text.Length - 3) + "[ ]";
               }
               else
               {
                  e.Item.Text = e.Item.Text.Substring(0, (e.Item.Text.Length) - 3) + "[x]";
                  chkAutoUpdateChanged(true);
               }
               break;
           case "LSD":
               {
                   ResolveLSD();
                   dgFleetInfo.ClearCachedDataSource();
                   dgFleetInfo.RebindDataSource();
                   break;
               }
         
         }

      }


      protected void dgFleetInfo_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
         if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables[0] != null))
         {
            e.DataSource = sn.Map.DsFleetInfo;
         }
         else
         {
            e.DataSource = null;
         }
  
      }




      protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
      {
         sn.Map.DgItemsPerPage = Convert.ToInt32(cboGridPaging.SelectedItem.Value);  
         this.dgFleetInfo.LayoutSettings.PagingSize   = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
         dgFleetInfo.ClearCachedDataSource();
         dgFleetInfo.RebindDataSource();
      }
      protected void dgFleetInfo_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
      {
         sn.History.FleetId = Convert.ToInt64(this.cboFleet.SelectedItem.Value);
         sn.History.VehicleId = Convert.ToInt64(e.Row.Cells.GetNamedItem("VehicleId").Text);
         sn.History.FromDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
         sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
         sn.History.FromHours = "08";
         sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();
         
       
      }
      protected void dgFleetInfo_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
      {
          SaveShowCheckBoxes();


          if (e.Column.Name == "History")
         {
            sn.History.DsSelectedData = null;
            sn.History.RedirectFromMapScreen = true;
            Response.Write("<SCRIPT Language='javascript'>parent.parent.main.window.location='../History/frmhistmain.aspx' </SCRIPT>");
         }
         else if (e.Column.Name == "LSD")
         {
             string strAddress = "";
             try
             {
                 using (LocationMgr.Location location = new LocationMgr.Location())
                 {
                     foreach (DataRow dr in sn.Map.DsFleetInfo.Tables[0].Rows)
                     {
                         if (Convert.ToInt32(dr["VehicleId"]) == Convert.ToInt32(sn.History.VehicleId))
                         {
                             try
                             {

                                 if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId,sn.User.OrganizationId,   Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]),ref  strAddress), false))
                                     if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]),ref  strAddress), true))
                                     {
                                         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Cannot Resolve LSD address"));
                                     }

                                 dr["StreetAddress"] = strAddress;

                             }
                             catch (Exception Ex)
                             {
                                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                                 System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                                 ExceptionLogger(trace);

                             }



                             dgFleetInfo.ClearCachedDataSource();
                             dgFleetInfo.RebindDataSource();
                             System.Threading.Thread.Sleep(0);
                             break;
                         }
                     }
                 } 
             }

             catch (Exception Ex)
             {
                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_ButtonClick--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                 System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                 ExceptionLogger(trace);

             }
         }
      }
      protected void dgFleetInfo_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
      {

         
         DataRow[] drCollections = null;
         if (sn.Map.DsFleetInfo != null) 
         {

            string VehicleStatus = e.Row.Cells[7].Text.ToString();
            

            if ((VehicleStatus == Resources.Const.VehicleStatus_Parked) || (VehicleStatus == Resources.Const.VehicleStatus_Parked + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Untethered) || (VehicleStatus == Resources.Const.VehicleStatus_Untethered + "*"))
            {
               e.Row.Cells[7].Style.ForeColor = Color.Red;
            }

            if ((VehicleStatus == Resources.Const.VehicleStatus_Idling) || (VehicleStatus == Resources.Const.VehicleStatus_Idling + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Tethered) || (VehicleStatus == Resources.Const.VehicleStatus_Tethered + "*"))
            {
               e.Row.Cells[7].Style.ForeColor  = Color.DarkOrange;
            }

            if ((VehicleStatus == Resources.Const.VehicleStatus_Moving) || (VehicleStatus == Resources.Const.VehicleStatus_Moving + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON))
            {
               e.Row.Cells[7].Style.ForeColor = Color.Green;
            }


               
             if (!sn.User.ControlEnable(sn, 41))
                e.Row.Cells.GetNamedItem("History").Column.Visible = false;
             else
                e.Row.Cells.GetNamedItem("History").Column.Visible = true;



            //if (sn.User.MapType == VLF.MAP.MapType.LSD)
              // e.Row.Cells.GetNamedItem("LSD").Column.Visible = true;
              //else
              // e.Row.Cells.GetNamedItem("LSD").Column.Visible = false;


               drCollections = sn.Map.DsFleetInfo.Tables[0].Select("BoxId=" + e.Row.Cells[1].Text);

               foreach (DataRow rowItem in drCollections)
               {
                  if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                     e.Row.Checked = true;
                  else
                     e.Row.Checked = false;
               }
            }
      }
      protected void dgFleetInfo_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
         if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
         {
             e.Layout.TextSettings.Language=ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
             if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                e.Layout.TextSettings.UseLanguage = "fr-FR";
             //else
             //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();      
         }
      }
      protected void cboRows_SelectedIndexChanged(object sender, EventArgs e)
      {
         sn.Map.DgVisibleRows =Convert.ToInt32(cboRows.SelectedItem.Value);
         if (Convert.ToInt16(cboRows.SelectedItem.Value)<11)
            dgFleetInfo.Height = Unit.Pixel(70 + Convert.ToInt16(cboRows.SelectedItem.Value)*2 + Convert.ToInt32(dgFleetInfo.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));         
         else
            dgFleetInfo.Height = Unit.Pixel(110 +  + Convert.ToInt16(cboRows.SelectedItem.Value)+ Convert.ToInt32(dgFleetInfo.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));         
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

       private void LoadMap()
       {
           //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
           //    sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
           if (sn.User.MapType == VLF.MAP.MapType.LSD)
               Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
           else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth )
               //Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='frmVehicleMapVE.aspx';</SCRIPT>");
               Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
           else
               Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");

       }


       protected void dgFleetInfo_InitializePostBack(object sender, ISNet.WebUI.WebGrid.PostbackEventArgs e)
       {
           SaveShowCheckBoxes();
       }


   

       private void ResolveLSD()
       {
           SaveShowCheckBoxes(); 
           string[] addresses = null;
           string strAddress = "";
           try
           {

               using (LocationMgr.Location location = new LocationMgr.Location())
               {
                   DataRow[] drArrAddress = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true");

                   foreach (DataRow dr in drArrAddress)
                   {
                       try
                       {


                           if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), ref strAddress), false))
                               if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]),ref  strAddress), true))
                               {
                                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Cannot Resolve LSD address"));
                               }

                            dr["StreetAddress"]=strAddress;
                       }
                       catch
                       {
                       }
                   }
               }

           }
           catch (Exception Ex)
           {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ResolveLSD--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
               System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
               ExceptionLogger(trace);

           }
          
       }
}  
}