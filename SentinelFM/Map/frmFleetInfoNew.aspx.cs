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
using System.Globalization;
using System.Collections.Generic;
 
namespace SentinelFM
{

   public partial class Map_frmFleetInfoNew : SentinelFMBasePage 
   {


     // System.Net.WebRequest myRequest;
      public string redirectURL;
      private DataSet dsFleetInfo;
      public VLF.MAP.ClientMapProxy map;
      public VLF.MAP.ClientMapProxy geoMap;
      public string strGeoMicroURL;
      public bool chkShowAutoPostBack = false;
      public int AutoRefreshTimer = 60000;

      private Stopwatch watch = new Stopwatch();
      private Stopwatch Pagewatch = new Stopwatch();
      public string _xml;



     

      protected void Page_Load(object sender, System.EventArgs e)
      {
          Response.Redirect("new/frmFleetInfoNew.aspx");
          return;

         try
         {
           System.Diagnostics.Trace.WriteLine("Page_Load->Thread ID:" + System.Threading.Thread.CurrentThread.GetHashCode().ToString());    
             
            //body.Style.Add("overflow", "hidden");
           
            double ver = getInternetExplorerVersion();
            //if (ver < 7.0)
            //   dgFleetInfo.Width = Unit.Pixel(1000);

            if (!sn.User.IsLSDEnabled)
                dgFleetInfo.RootTable.Columns[11].Visible = false;


            

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

            if (!Page.IsPostBack)
            {
               //DateTime dt = System.DateTime.Now;
                  
               //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Map screen - Load vehicles info started ->  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                   
                
                //Pagewatch.Reset();
                //Pagewatch.Start();
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Start-->Map Screen Grid"));

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
               //if (sn.Map.MapRefresh)
               //{
                  
               //   string MapRefresh = mnuOptions.Items[3].Text;
               //   mnuOptions.Items[3].Text = MapRefresh.Substring(0, (MapRefresh.Length) - 3) + "[x]";
               //   chkShowAutoPostBack = true;
               //   Response.Write("<script language='javascript'>window.setTimeout('AutoReloadDetails()'," + AutoRefreshTimer.ToString() + ")</script>");
               //   //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
               //   //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
               //}
               //else
               //{
               //   chkShowAutoPostBack = false;
               //   Response.Write("<script language='javascript'> clearTimeout();</script>");
               //}


              
               
               this.tblFleetActions.Visible = true;  
               this.tblWait.Visible = false;



               //Get Vehicles Info
               sn.MessageText = "";
               CboFleet_Fill();
               if (sn.MessageText != "") //No Fleet for user
                  return;


              string strUnitOfMes = sn.User.UnitOfMes == 1 ? " (km/h)" : " (mph)";
              this.dgFleetInfo.RootTable.Columns[5].Caption = this.dgFleetInfo.RootTable.Columns[5].Caption + strUnitOfMes;       
              FindExistingPreference();



              //// BeginEventHandler bh = new BeginEventHandler(this.BeginGetAsyncData);
              //// EndEventHandler eh = new EndEventHandler(this.EndGetAsyncData);
              //// AddOnPreRenderCompleteAsync(bh, eh);
              //// string address = Request.Url.AbsoluteUri.ToString();
              //// myRequest = System.Net.WebRequest.Create(address);
              ////// myRequest.Credentials = new System.Net.NetworkCredential(sn.UserName, sn.Password);



               try
               {
                   if (sn.Map.SelectedFleetID != 0)
                   {
                       cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                       DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                   }
                   else if (sn.User.DefaultFleet != -1)
                   {
                       cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                       DgFleetInfo_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                       sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                   }
               }
               catch (Exception Ex)
               {
                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                   System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                   ExceptionLogger(trace);

               }





               #region Check Results after update position

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

               #endregion



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


                // this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

                

               
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


      private void Page_PreRenderComplete(object sender, EventArgs e)
      {
          //Pagewatch.Stop();
          //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "End<--Map Screen Grid-->(sec):" + Pagewatch.Elapsed.TotalSeconds));

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
            
            dsFleetInfo = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            watch.Reset();
            watch.Start();  

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));


            //dbf.GetVehiclesLastKnownPositionInfoCompleted +=
            //    new ServerDBFleet.GetVehiclesLastKnownPositionInfoCompletedEventHandler(GetFleetXML);

            //dbf.GetVehiclesLastKnownPositionInfoAsync(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, _xml);


            if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    sn.Map.DsFleetInfo = null;
                    return;
                }

            if (xml == "")
            {
                sn.Map.DsFleetInfo = null;
                return;
            }





            strrXML = new StringReader(xml);
            //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
            //string strPath = MapPath(DataSetPath) + @"\FleetInfo.xsd";

            string strPath = Server.MapPath("../Datasets/FleetInfo.xsd");

            dsFleetInfo.ReadXmlSchema(strPath);
            dsFleetInfo.ReadXml(strrXML);

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));

            sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
            sn.Map.DsFleetInfo = dsFleetInfo;


            //dgFleetInfo.LayoutSettings.AutoFitColumns = true;  

            //dgFleetInfo.ClearCachedDataSource();
            //dgFleetInfo.RebindDataSource();
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
            Int64 sessionTimeOut = 0;
            SaveShowCheckBoxes();

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataColumn dc;

            dc = new DataColumn("Freq", Type.GetType("System.Int64"));
            dc.DefaultValue = 0;
            dt.Columns.Add(dc);


            bool cmdSent = false;

            if (Convert.ToInt32(cboFleet.SelectedItem.Value) == -1)
            {
                sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet");
                ShowErrorMessage();
                this.tblWait.Visible = false;
                return; 
            }

               sn.MessageText = "";
               LocationMgr.Location dbl = new LocationMgr.Location();
               
               SaveShowCheckBoxes();
               bool ShowTimer = false;
               //Delete old timeouts
               sn.Cmd.DtUpdatePositionFails.Rows.Clear();



               DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows); 
               if (drArr == null || drArr.Length == 0)
               {
                   sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                   ShowErrorMessage();
                   this.tblWait.Visible = false;
                   return;
               }

               //DumpBeforeCall(sn, string.Format("frmFleetInfoNew -- cmdUpdatePosition"));


#if NEW_SLS
              


               int[] boxid=new int[drArr.Length];
               short[] protocolType=new short[drArr.Length];
               short[] commMode=new short[drArr.Length];
               bool[] sent = new bool[drArr.Length];
               Int64[] timeOut=new Int64[drArr.Length];
               short[] results = new short[drArr.Length];
               string[] vehicles = new string[drArr.Length];
               

               int i=0;
               foreach (DataRow rowItem in drArr)
                {
                       boxid[i]=Convert.ToInt32(rowItem["BoxId"]);
                       protocolType[i]=-1;
                       commMode[i]=-1;
                       sent[i]=false;
                       timeOut[i]=0;
                       results[i]=0;
                       vehicles[i]=rowItem["Description"].ToString() ;
                       i++;
                }

                if (objUtil.ErrCheck(dbl.SendCommandToMultipleVehicles(sn.UserID, sn.SecId, DateTime.Now,boxid , Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "",false, ref protocolType  , ref commMode  , ref sent, ref timeOut,ref results), false))
                   if (objUtil.ErrCheck(dbl.SendCommandToMultipleVehicles(sn.UserID, sn.SecId, DateTime.Now,boxid , Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "",false, ref protocolType  , ref commMode  , ref sent, ref timeOut,ref results), true ))
                     {
                         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Send update position failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                         return;
                     }


                for (i = 0; i < sent.Length; i++)
                {
                    if (!sent[i])
                    {
                        DataRow drErr;
                        drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                        drErr["VehicleDesc"] = vehicles[i];
                        drErr["Status"] = (string)base.GetLocalResourceObject("sn_MessageText_SendCommandFailedError") + ": " + vehicles[i]; 
                        sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                    }
                    else
                    {
                        sn.Cmd.ArrBoxId.Add(boxid[i]);
                        sn.Cmd.ArrProtocolType.Add(protocolType[i]);
                        sn.Cmd.ArrVehicle.Add(vehicles[i]);   
                        ShowTimer = true;
                    }

                }
                
                 sessionTimeOut = FindMax(timeOut);

                
#else

               
                foreach (DataRow rowItem in drArr)
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

                                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update position failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

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
                       dr["Freq"] = sessionTimeOut;// sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                       dt.Rows.Add(dr);

                       rowItem["ProtocolId"] = ProtocolId;
                       sn.Cmd.ProtocolTypeId = ProtocolId;

                       if (cmdSent)
                           ShowTimer = true;
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

                   }


                try
                {
                    ds.Tables.Add(dt);
                    DataView dv = ds.Tables[0].DefaultView;
                    dv.Sort = "Freq" + " DESC";
                    sessionTimeOut = Convert.ToInt64(dv[0].Row[0]);
                    sn.Cmd.GetCommandStatusRefreshFreq = sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                }
                catch
                {
                    sn.Cmd.GetCommandStatusRefreshFreq = 2000;
                }
             
                  
#endif

                if (ShowTimer)
                {

                    //this.dgFleetInfo.Visible = false;

                    this.tblWait.Visible = true;
                    this.tblFleetActions.Visible = false;
                    sn.Cmd.GetCommandStatusRefreshFreq = sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;

                    if (sessionTimeOut > 60)
                         this.lblUpdatePosition.Text = (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Minutes1") + " " + Convert.ToInt64(Math.Round(sessionTimeOut / 60.0)).ToString() + " " + (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Minutes2");
                        else
                         this.lblUpdatePosition.Text = (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Seconds1") + " " + sessionTimeOut + " " + (string)base.GetLocalResourceObject("lblUpdatePosition_Text_Seconds2");
                    

                    sn.Map.TimerStatus = true;
                    sn.Cmd.UpdatePositionSend = true;
                    Response.Write("<script language='javascript'> parent.frmStatus.location.href='frmTimerPosition.aspx' </script>");


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
                ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")"; // Changes for TimeZone Feature end
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

      
            //Delay for resolving "Store Position"
             //System.Threading.Thread.Sleep(2000);


               bool ShowTitle = false;

               DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
               if (drArr != null || drArr.Length > 0)
               {
                   foreach (DataRow rowItem in drArr)
                   {
                     if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                     {

                        bool UpdatePositionFails = false;

                        foreach (DataRow rw in sn.Cmd.DtUpdatePositionFails.Rows)
                        {
                            if (rw["VehicleDesc"].ToString().TrimEnd() == rowItem["Description"].ToString().TrimEnd())
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
              //foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
              //{
              //    rowItem["chkBoxShow"] = false;
              //    foreach (string keyValue in dgFleetInfo.RootTable.GetCheckedRows())
              //    {
              //        if (keyValue == rowItem["VehicleId"].ToString())
              //        {
              //            rowItem["chkBoxShow"] = true;
              //            break;
              //        }

              //    }
              //}

              foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
              {
                  rowItem["chkBoxShow"] = false;
                  rowItem["Updated"] = false;
              }

              foreach (string keyValue in dgFleetInfo.RootTable.GetCheckedRows())
              {
                  DataRow[] drCollections = null;
                  drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId='" + keyValue+"'");
                  if (drCollections != null && drCollections.Length > 0)
                  {
                      DataRow dRow = drCollections[0];
                      dRow["chkBoxShow"] = true;
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

          try
          {
              // Stopwatch watchFleet = new Stopwatch();

              if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables[0] != null))
              {
                  // watchFleet.Reset();
                  // watchFleet.Start();  
                  e.DataSource = sn.Map.DsFleetInfo;
                  //watchFleet.Stop();
                  //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<--DataGrid Binding:" + watchFleet.Elapsed.TotalSeconds + " ; parameters:" + sn.UserID));

              }
              else
              {
                  e.DataSource = null;
              }
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_InitializeDataSource--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
  
      }




      protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
      {
          try
          {
              sn.Map.DgItemsPerPage = Convert.ToInt32(cboGridPaging.SelectedItem.Value);
              this.dgFleetInfo.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
              dgFleetInfo.ClearCachedDataSource();
              dgFleetInfo.RebindDataSource();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cboGridPaging_SelectedIndexChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }
      protected void dgFleetInfo_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
      {
          try
          {
              
              sn.History.FleetId = Convert.ToInt64(this.cboFleet.SelectedItem.Value);
              //sn.History.VehicleId = Convert.ToInt64(e.Row.Cells.GetNamedItem("VehicleId").Text);


              //sn.History.FromDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
              //sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
              //sn.History.FromHours = "08";
              //sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();


              sn.History.FromDate = DateTime.Now.ToString(sn.User.DateFormat);
              sn.History.ToDate = DateTime.Now.AddDays(1).ToString(sn.User.DateFormat);
              sn.History.FromHours = "0";
              sn.History.ToHours = "0";


              DataRow[] drCollections = null;
              drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId=" + e.Row.Cells.GetNamedItem("VehicleId").Text);
              if (drCollections != null && drCollections.Length > 0)
              {
                  DataRow dRow = drCollections[0];
                  if (e.Row.Cells.GetNamedItem("chkBoxShow").Text.ToLower() == "true")
                      dRow["chkBoxShow"] = false;
                  else
                      dRow["chkBoxShow"] = true;
              }
             
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_RowChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }
      protected void dgFleetInfo_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
      {
          sn.History.VehicleId =Convert.ToInt64(dgFleetInfo.RetrieveClientLastSelectedObject().KeyValue);
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

          try

          {
       
              //  Stopwatch watchFleet = new Stopwatch();

              DataRow[] drCollections = null;
              if (sn.Map.DsFleetInfo != null)
              {

                  //   watchFleet.Reset();
                  //  watchFleet.Start();  

                  string VehicleStatus = e.Row.Cells[7].Text.ToString();


                  if ((VehicleStatus == Resources.Const.VehicleStatus_Parked) || (VehicleStatus == Resources.Const.VehicleStatus_Parked + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Untethered) || (VehicleStatus == Resources.Const.VehicleStatus_Untethered + "*"))
                  {
                      e.Row.Cells[7].Style.ForeColor = Color.Red;
                  }

                  if ((VehicleStatus == Resources.Const.VehicleStatus_Idling) || (VehicleStatus == Resources.Const.VehicleStatus_Idling + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Tethered) || (VehicleStatus == Resources.Const.VehicleStatus_Tethered + "*"))
                  {
                      e.Row.Cells[7].Style.ForeColor = Color.DarkOrange;
                  }

                  if ((VehicleStatus == Resources.Const.VehicleStatus_Moving) || (VehicleStatus == Resources.Const.VehicleStatus_Moving + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON))
                  {
                      e.Row.Cells[7].Style.ForeColor = Color.Green;
                  }



                  if (!sn.User.ControlEnable(sn, 41))
                      e.Row.Cells.GetNamedItem("History").Column.Visible = false;
                  else
                      e.Row.Cells.GetNamedItem("History").Column.Visible = true;


                 
                  
                  
                  
                  //drCollections = sn.Map.DsFleetInfo.Tables[0].Select("BoxId=" + e.Row.Cells[1].Text);

                  //foreach (DataRow rowItem in drCollections)
                  //{
                  //    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                  //        e.Row.Checked = true;
                  //    else
                  //        e.Row.Checked = false;
                  //}


                  drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId=" + e.Row.KeyValue);
                  if (drCollections != null && drCollections.Length > 0)
                  {
                      DataRow dRow = drCollections[0];
                      e.Row.Checked = Convert.ToBoolean(dRow["chkBoxShow"]);


                      //    UInt64 intSensorMask = 0;

                      //    try
                      //    {
                      //         intSensorMask = Convert.ToUInt64(dRow["SensorMask"]);
                      //    }
                      //    catch
                      //    {
                      //    }

                      //UInt64 checkBit = 0x10;
                      //check bit for PTO
                      //if ((intSensorMask & checkBit) != 0)
                      //{
                      //    e.Row.Cells[7].Text = "PTO On";
                      //    e.Row.Cells[7].Style.ForeColor = Color.Black;
                      //}


                  }






                  // watchFleet.Stop();
                  // System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<--DataGrid dgFleetInfo_InitializeRow:" + watchFleet.Elapsed.TotalSeconds + " ; parameters:" + sn.UserID));

              }
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_InitializeRow--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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
           try
           {
               //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
               //    sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
               if (sn.User.MapType == VLF.MAP.MapType.LSD)
                   Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
               else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                   //Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='frmVehicleMapVE.aspx';</SCRIPT>");
                   Response.Write("<SCRIPT Language='javascript'>parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
               else
                   Response.Write("<script language='javascript'> parent.frmVehicleMap.location.href='frmvehiclemap.aspx' </script>");
           }
           catch (Exception Ex)
           {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", LoadMap--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
           }


       }


       protected void dgFleetInfo_InitializePostBack(object sender, ISNet.WebUI.WebGrid.PostbackEventArgs e)
       {
          //SaveShowCheckBoxes();
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


       //IAsyncResult BeginGetAsyncData(Object src, EventArgs args, AsyncCallback cb, Object state)
       //{
       //    Stopwatch pwatch1 = new Stopwatch();
       //    pwatch1.Reset();
       //    pwatch1.Start();
       //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "BeginGetAsyncData - ThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId));

       //    try
       //    {
       //        if (sn.Map.SelectedFleetID != 0)
       //        {
       //            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
       //            DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
       //        }
       //        else if (sn.User.DefaultFleet != -1)
       //        {
       //            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
       //            DgFleetInfo_Fill(Convert.ToInt32(sn.User.DefaultFleet));
       //            sn.Map.SelectedFleetID = sn.User.DefaultFleet;
       //        }

       //        dgFleetInfo.ClearCachedDataSource();
       //        dgFleetInfo.RebindDataSource();
       //    }
       //    catch (Exception Ex)
       //    {
       //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
       //        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
       //        ExceptionLogger(trace);

       //    }

       //    pwatch1.Stop();
       //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->BeginGetAsyncData -->(sec):" + pwatch1.Elapsed.TotalSeconds));


       //    return myRequest.BeginGetResponse(cb, state);
       //}

       //void EndGetAsyncData(IAsyncResult ar)
       //{
       //    Stopwatch pwatch1 = new Stopwatch();
       //    pwatch1.Reset();
       //    pwatch1.Start();

       //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "EndGetAsyncData - ThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId));

       //    System.Net.WebResponse myResponse = myRequest.EndGetResponse(ar);

       //    myResponse.Close();
       //    pwatch1.Stop();
       //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->EndGetAsyncData -->(sec):" + pwatch1.Elapsed.TotalSeconds));

       //}

       //void GetFleetXML(Object source, ServerDBFleet.GetVehiclesLastKnownPositionInfoCompletedEventArgs e)
       //{
       //    dsFleetInfo = new DataSet();
       //    StringReader strrXML = null;

       //    //Validate if key expired
       //    if ((VLF.ERRSecurity.InterfaceError)e.Result ==VLF.ERRSecurity.InterfaceError.PassKeyExpired)
       //    {
       //            SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
       //            string secId = "";
       //            int result = sec.ReloginMD5ByDBName (sn.UserID,sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref secId);
       //            if (result != 0)
       //            {
       //                sn.SecId = secId;
       //                if (sn.Map.SelectedFleetID != 0)
       //                {
       //                    cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
       //                    DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
       //                }
       //                else if (sn.User.DefaultFleet != -1)
       //                {
       //                    cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
       //                    DgFleetInfo_Fill(Convert.ToInt32(sn.User.DefaultFleet));
       //                    sn.Map.SelectedFleetID = sn.User.DefaultFleet;
       //                }


       //            }
       //            return; 
       //    }
           


 
       //    _xml = e.xml;
       //    if (_xml == null || _xml == "")
       //    {
       //        dgFleetInfo.RootTable.Rows.Clear();
       //        sn.Map.DsFleetInfo = null;
       //        return;
       //    }



          



       //    strrXML = new StringReader(_xml);
       //    string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
       //    string strPath = MapPath(DataSetPath) + @"\FleetInfo.xsd";
       //    dsFleetInfo.ReadXmlSchema(strPath);
       //    dsFleetInfo.ReadXml(strrXML);


       //    watch.Stop();
       //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "End Data-->Map Screen Grid -->Database Call(sec):" + watch.Elapsed.TotalSeconds));

           


       //    sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
       //    sn.Map.DsFleetInfo = dsFleetInfo;
       //    dgFleetInfo.ClearCachedDataSource();
       //    dgFleetInfo.RebindDataSource();




       //}

       private Int64 FindMax(Int64[] array)
       {

           Int64 maxValue = array[0];

           for (int i = 1; i < array.Length; i++)
           {

               if (array[i] > maxValue)

                   maxValue = array[i];

           }

           return maxValue;

       }

     
}  
}