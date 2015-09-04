using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO; 
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Diagnostics; 

namespace SentinelFM
{
    public partial class Map_frmAlarmRotatingCallBack : System.Web.UI.Page, ICallbackEventHandler
   {
      public string strAlarms;
      protected SentinelFMSession sn = null;
      protected clsUtility objUtil;
      private string _callbackArg;
      public string alarmsScreenHight = "105";
      private Stopwatch Pagewatch = new Stopwatch();
      private Stopwatch watch = new Stopwatch();
      public string _xml = "";
      public string _checksum = "";
      public string headerColor = "#009933";
      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            
            //Clear IIS cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);

           
            if (sn.User.ViewMDTMessagesScrolling == 0)
               alarmsScreenHight = "290";

           Pagewatch.Reset();
           Pagewatch.Start();

           System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Alarm Screen Start-->User Id:" + sn.UserID));


           objUtil = new clsUtility(sn);

            // Define a StringBuilder to hold messages to output.
            StringBuilder sb = new StringBuilder();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Define one of the callback script's context.
            // The callback script will be defined in a script block on the page.
            StringBuilder context = new StringBuilder();
            context.Append("function ReceiveServerData(arg)");
            context.Append("{");
            context.Append("var msg_array=arg.split('~');  wholemessage =  msg_array[0]; lblTotalAlarms.innerHTML=msg_array[1];");
            context.Append("}");

            // Define callback references.
            String cbReference = cs.GetCallbackEventReference(this, "arg",
                "ReceiveServerData", context.ToString());
            String callbackScript = "function CallTheServer(arg) { " +
                cbReference + "; }";

            // Register script blocks will perform call to the server.
            cs.RegisterClientScriptBlock(this.GetType(), "CallTheServer",
                callbackScript, true);



            if (!Page.IsPostBack)
            {
                AlarmsList_Fill_NewTZ();
               this.lblTotalAlarms.Text = sn.Map.AlarmCount.ToString();

               if (sn.Map.ReloadMap)
               {
                  
                  //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                  //{
                  //   sn.MapSolute.LoadVehicles(sn, sn.Map.DsFleetInfo.Tables[0], "Map");
                  //   Response.Write("<SCRIPT Language='javascript'>parent.parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx';</SCRIPT>");
                  //}
                  if (sn.User.MapType == VLF.MAP.MapType.LSD)
                      Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
                  else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                      //Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmVehicleMapVE.aspx';</SCRIPT>");
                      Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
                      
                  else
                     Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmvehiclemap.aspx';</SCRIPT>");

                  sn.Map.ReloadMap = false;
               }

               this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            }

            Pagewatch.Stop();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Alarm Screen End<--Alarms Page Load (sec):" + Pagewatch.Elapsed.TotalSeconds + "; User Id:" + sn.UserID));

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }


      }




      public void RaiseCallbackEvent(String eventArgument)
      {
          AlarmsList_Fill_NewTZ();
         eventArgument = sn.Map.AlarmsHTML+"~"+sn.Map.AlarmCount;
      }

      string ICallbackEventHandler.GetCallbackResult()
      {
         return sn.Map.AlarmsHTML + "~" + sn.Map.AlarmCount;
      }

      // Changes for TimeZone Feature start

      private void AlarmsList_Fill_NewTZ()
      {
          try
          {
              //StringBuilder strBuild = new StringBuilder();
              //String str = "";
              //string xml = "";


              if (sn.UserID == 0)
                  return;


              this.lblTotalAlarms.Text = "0";
              ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
              float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

              watch.Reset();
              watch.Start();

              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "--> GetAlarmsShortInfoXMLByLang_NewTZ User ID:" + sn.UserID));


              //if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
              //   if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
              //   {
              //      return;
              //   }

              //alarms.GetAlarmsShortInfoXMLByLangCompleted  +=
              //new ServerAlarms.GetAlarmsShortInfoXMLByLangCompletedEventHandler(GetAlarmsXML);
              //alarms.GetAlarmsShortInfoXMLByLangAsync(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, _xml);

              _checksum = "";
              alarms.GetAlarmsShortInfoCheckSum_NewTZCompleted +=
             new ServerAlarms.GetAlarmsShortInfoCheckSum_NewTZCompletedEventHandler(GetAlarmsXML_NewTZ);
              alarms.GetAlarmsShortInfoCheckSum_NewTZAsync(sn.UserID, sn.SecId, timeZone, _checksum);

              //if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang (sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
              //    if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
              //   {
              //      return;
              //   }

              //if (xml == "")
              //{
              //   return;
              //}

              //StringReader strrXML = new StringReader(xml);

              //DataSet ds = new DataSet();
              //ds.ReadXml(strrXML);



              //string strStyle = "";
              //int AlarmCount = 0;


              //watch.Reset();
              //watch.Start();


              //for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
              //{

              //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
              //   {
              //      strStyle = "style='{color:green;}'";
              //   }

              //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
              //   {
              //      strStyle = "style='{color:#C0C0C0;}'";
              //   }


              //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
              //      && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
              //   {
              //      strStyle = "style='{color:DarkGoldenrod;}'";
              //   }


              //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
              //      && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
              //   {
              //      strStyle = "style='{color:red;}'";
              //   }


              //   // --- Hide closed alarms
              //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
              //   {
              //      //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");

              //       if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
              //          && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
              //           //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
              //           strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
              //           strStyle,
              //           ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
              //           Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
              //           ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
              //           ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd());


              //       else
              //           strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
              //                strStyle,
              //                ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
              //                Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
              //                ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
              //                ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd());

              //         //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

              //      AlarmCount++;
              //   }

              //   //----------------------

              //}

              //strAlarms = strBuild.ToString();

              //watch.Stop();
              //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Building Alarms string Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.UserID));


              ////this.lblTotalAlarms.Text = AlarmCount.ToString();
              //sn.Map.AlarmsHTML = strAlarms;
              //sn.Map.AlarmCount = AlarmCount;
          }
          catch (NullReferenceException Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
      }

      // Changes for TimeZone Feature end
      

      private void AlarmsList_Fill()
      {
         try
         {
            //StringBuilder strBuild = new StringBuilder();
            //String str = "";
            //string xml = "";


             if (sn.UserID == 0)
                 return;


            this.lblTotalAlarms.Text = "0";
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

            watch.Reset();
            watch.Start();

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "--> GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID));


            //if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
            //   if (objUtil.ErrCheck(alarms.GetCurrentAlarmsXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
            //   {
            //      return;
            //   }
            
            //alarms.GetAlarmsShortInfoXMLByLangCompleted  +=
            //new ServerAlarms.GetAlarmsShortInfoXMLByLangCompletedEventHandler(GetAlarmsXML);
            //alarms.GetAlarmsShortInfoXMLByLangAsync(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, _xml);

            _checksum = "";
            alarms.GetAlarmsShortInfoCheckSumCompleted +=
           new ServerAlarms.GetAlarmsShortInfoCheckSumCompletedEventHandler(GetAlarmsXML);
            alarms.GetAlarmsShortInfoCheckSumAsync(sn.UserID, sn.SecId, timeZone, _checksum);
               
            //if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang (sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
            //    if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
            //   {
            //      return;
            //   }

            //if (xml == "")
            //{
            //   return;
            //}

            //StringReader strrXML = new StringReader(xml);

            //DataSet ds = new DataSet();
            //ds.ReadXml(strrXML);

          

            //string strStyle = "";
            //int AlarmCount = 0;


            //watch.Reset();
            //watch.Start();


            //for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            //{

            //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
            //   {
            //      strStyle = "style='{color:green;}'";
            //   }

            //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
            //   {
            //      strStyle = "style='{color:#C0C0C0;}'";
            //   }


            //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
            //      && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
            //   {
            //      strStyle = "style='{color:DarkGoldenrod;}'";
            //   }


            //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
            //      && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
            //   {
            //      strStyle = "style='{color:red;}'";
            //   }


            //   // --- Hide closed alarms
            //   if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
            //   {
            //      //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");

            //       if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
            //          && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
            //           //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
            //           strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
            //           strStyle,
            //           ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
            //           Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
            //           ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
            //           ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd());


            //       else
            //           strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
            //                strStyle,
            //                ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
            //                Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
            //                ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
            //                ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd());

            //         //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

            //      AlarmCount++;
            //   }

            //   //----------------------

            //}

            //strAlarms = strBuild.ToString();

            //watch.Stop();
            //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Building Alarms string Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.UserID));


            ////this.lblTotalAlarms.Text = AlarmCount.ToString();
            //sn.Map.AlarmsHTML = strAlarms;
            //sn.Map.AlarmCount = AlarmCount;
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      // Changes for TimeZone Feature start

      void GetAlarmsXML_NewTZ(Object source, ServerAlarms.GetAlarmsShortInfoCheckSum_NewTZCompletedEventArgs e)
      {
          ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
          // Changes for TimeZone Feature start
          float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);// Changes for TimeZone Feature end


          //Validate if key expired
          if ((VLF.ERRSecurity.InterfaceError)e.Result == VLF.ERRSecurity.InterfaceError.PassKeyExpired)
          {
              SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
              string secId = "";
              int result = sec.ReloginMD5ByDBName(sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref secId);
              if (result != 0)
              {
                  sn.SecId = secId;
                  AlarmsList_Fill_NewTZ();
              }
          }

          _checksum = e.checksum;
          if (_checksum == null || _checksum == "")
              return;

          if (sn.Map.AlarmsCheckSum != _checksum)
          {
              sn.Map.AlarmsCheckSum = _checksum;
              // Changes for TimeZone Feature start
              if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), false))
                  if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), true)) // Changes for TimeZone Feature end
                  {
                      return;
                  }
          }
          else
          {
              return;
          }

          StringBuilder strBuild = new StringBuilder();
          DataSet dsFleetInfo = new DataSet();
          if (_xml == null || _xml == "")
              return;

          StringReader strrXML = new StringReader(_xml);

          DataSet ds = new DataSet();
          ds.ReadXml(strrXML);
          string strStyle = "";
          int AlarmCount = 0;


          if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
              return;


          for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
          {

              if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
              {
                  strStyle = "style='{color:green;}'";
              }

              if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
              {
                  strStyle = "style='{color:#C0C0C0;}'";
              }


              if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                 && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
              {
                  strStyle = "style='{color:DarkGoldenrod;}'";
              }


              if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                 && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
              {
                  strStyle = "style='{color:red;}'";
              }


              // --- Hide closed alarms
              if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
              {
                  //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");

                  if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                     && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                      //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
                      strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                      strStyle,
                      ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                      Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                      ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                      ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", ""));


                  else
                      strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                           strStyle,
                           ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                           Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                           ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                           ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", ""));

                  //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                  AlarmCount++;
              }

              //----------------------

          }

          strAlarms = strBuild.ToString();

          //this.lblTotalAlarms.Text = AlarmCount.ToString();
          sn.Map.AlarmsHTML = strAlarms;
          sn.Map.AlarmCount = AlarmCount;
          this.lblTotalAlarms.Text = AlarmCount.ToString();


          watch.Stop();
          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID + ", Database Call(sec):" + Math.Round(watch.Elapsed.TotalSeconds, 2)));

      }

      // Changes for TimeZone Feature end

        void GetAlarmsXML(Object source, ServerAlarms.GetAlarmsShortInfoCheckSumCompletedEventArgs  e)
        {
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            // Changes for TimeZone Feature start
            short timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);// Changes for TimeZone Feature end


             //Validate if key expired
            if ((VLF.ERRSecurity.InterfaceError)e.Result == VLF.ERRSecurity.InterfaceError.PassKeyExpired)
            {
                SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
                string secId = "";
                int result = sec.ReloginMD5ByDBName(sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref secId);
                if (result != 0)
                {
                    sn.SecId = secId;
                    AlarmsList_Fill();
                }
            }

            _checksum  = e.checksum;
            if (_checksum == null || _checksum == "")
                return;

            if (sn.Map.AlarmsCheckSum != _checksum)
            {
                sn.Map.AlarmsCheckSum = _checksum;
                
                if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), false))
                    if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), true)) // Changes for TimeZone Feature end
                    {
                        return;
                    }
            }
            else
            {
                return; 
            }

            StringBuilder strBuild = new StringBuilder();
            DataSet dsFleetInfo = new DataSet();
            if (_xml == null || _xml == "")
                return;

            StringReader strrXML = new StringReader(_xml);

            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);
            string strStyle = "";
            int AlarmCount = 0;


            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return;


            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {

                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Accepted.ToString())
                {
                    strStyle = "style='{color:green;}'";
                }

                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                {
                    strStyle = "style='{color:#C0C0C0;}'";
                }


                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                   && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Warning.ToString())
                {
                    strStyle = "style='{color:DarkGoldenrod;}'";
                }


                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                   && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                {
                    strStyle = "style='{color:red;}'";
                }


                // --- Hide closed alarms
                if (ds.Tables[0].Rows[i]["AlarmState"].ToString() != VLF.CLS.Def.Enums.AlarmState.Closed.ToString())
                {
                    //str=str+("<p><a href='#' "+ strStyle +" onclick=NewWindow('"+ ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >"+ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd()+" "+ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() +"</u></a></p><br>");

                    if (ds.Tables[0].Rows[i]["AlarmState"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                       && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                        //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + "<IMG alt='' border=0  src='../images/exclameanim.gif'> " + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");
                        strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                        strStyle,
                        ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                        Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                        ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                        ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", ""));


                    else
                        strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                             strStyle,
                             ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                             Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                             ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                             ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n",""));

                    //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                    AlarmCount++;
                }

                //----------------------

            }

            strAlarms = strBuild.ToString();

            //this.lblTotalAlarms.Text = AlarmCount.ToString();
            sn.Map.AlarmsHTML = strAlarms;
            sn.Map.AlarmCount = AlarmCount;
            this.lblTotalAlarms.Text = AlarmCount.ToString();


            watch.Stop();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID+", Database Call(sec):" + Math.Round(watch.Elapsed.TotalSeconds,2)));

        }



        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
           
        }

        protected override void InitializeCulture()
        {

            if (Session["PreferredCulture"] != null)
            {
                string UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }

            if (sn.User.MenuColor != "")
                headerColor = sn.User.MenuColor;

            //HtmlGenericControl hgc = new HtmlGenericControl("style");
            //hgc.Attributes.Add("type", "text/css");

            //String filePath = HttpContext.Current.Server.MapPath("../GlobalStyle.css");

            //StreamReader reader = new StreamReader(filePath);
            //string content = reader.ReadToEnd();
            //reader.Close();
            //if (sn.User.MenuColor != "")
            //    hgc.InnerText = content.Replace("#009933", sn.User.MenuColor);
            //else
            //    hgc.InnerText = content;

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //StringWriter tw = new StringWriter(sb);
            //HtmlTextWriter hw = new HtmlTextWriter(tw);
            //hgc.RenderControl(hw);
            //Response.Write(sb.ToString());

        }



        public void RedirectToLogin()
        {
        
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
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
   }
}

