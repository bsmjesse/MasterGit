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
    public partial class frmAlarmRotatingServerCall : System.Web.UI.Page    
   {
      public string strAlarms;
      
      
      private Stopwatch Pagewatch = new Stopwatch();
      private Stopwatch watch = new Stopwatch();
      public string _xml = "";
      public string _checksum = "";
      public string headerColor = "#009933";
      protected SentinelFMSession sn = null;
      protected clsUtility objUtil;

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {

             if (sn.User.ViewAlarmScrolling == 0)
                 return; 
            
               sn = (SentinelFMSession)Session["SentinelFMSession"];
               AlarmsList_Fill_NewTZ();

       
         }
         catch (System.Threading.ThreadAbortException Ex)
         {

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

      private void AlarmsList_Fill_NewTZ()
      {
          try
          {

              if (sn.UserID == 0)
                  return;



              ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
              float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

              watch.Reset();
              watch.Start();

              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "--> GetAlarmsShortInfoXMLByLang_NewTZ User ID:" + sn.UserID));

              // _checksum = "";
              // alarms.GetAlarmsShortInfoCheckSumCompleted +=
              //new ServerAlarms.GetAlarmsShortInfoCheckSumCompletedEventHandler(GetAlarmsXML);
              // alarms.GetAlarmsShortInfoCheckSumAsync(sn.UserID, sn.SecId, timeZone, _checksum);

              GetAlarmsXML_NewTZ();
          }
          catch (System.Threading.ThreadAbortException Ex) {
              
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
      
             if (sn.UserID == 0)
                 return;


            
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

            watch.Reset();
            watch.Start();

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "--> GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID));

           // _checksum = "";
           // alarms.GetAlarmsShortInfoCheckSumCompleted +=
           //new ServerAlarms.GetAlarmsShortInfoCheckSumCompletedEventHandler(GetAlarmsXML);
           // alarms.GetAlarmsShortInfoCheckSumAsync(sn.UserID, sn.SecId, timeZone, _checksum);

            GetAlarmsXML();
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

      void GetAlarmsXML_NewTZ()
      {
          ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
          float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);
          objUtil = new clsUtility(sn);


          sn.Map.AlarmsCheckSum = _checksum;
          if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), false))
              if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), true))
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

                  if (ds.Tables[0].Rows[i]["vehicleDescription"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                     && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                  {
                      if (ds.Tables[0].Rows[i]["AlarmDescription"].ToString().Contains("DTC codes:")) //DTC Code
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                          strStyle,
                          ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                          Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                          ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                          ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(",", "<br>"));
                      }
                      else
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                          strStyle,
                          ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                          Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                          ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                          ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", "<br>"));
                      }

                  }
                  else
                  {
                      if (ds.Tables[0].Rows[i]["AlarmDescription"].ToString().Contains("DTC codes:")) //DTC Code
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                            strStyle,
                            ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                            Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                            ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                            ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(";", "<br>"));
                      }
                      else
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                               strStyle,
                               ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                               Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                               ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                               ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", "<br>"));
                      }
                  }
                  //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                  AlarmCount++;
              }

              //----------------------

          }

          strAlarms = strBuild.ToString();
          sn.Map.AlarmsHTML = strAlarms;
          sn.Map.AlarmCount = AlarmCount;

          Response.Write(sn.Map.AlarmsHTML + "~" + sn.Map.AlarmCount + "~" + sn.Map.AlarmsCheckSum);

          watch.Stop();
          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID + ", Database Call(sec):" + Math.Round(watch.Elapsed.TotalSeconds, 2)));
          Response.End();
      }

      // Changes for TimeZone Feature end

      void GetAlarmsXML()
      {
          ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
          Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);
          objUtil = new clsUtility(sn);


              sn.Map.AlarmsCheckSum = _checksum;
              if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), false))
                  if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), true))
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

                  if (ds.Tables[0].Rows[i]["vehicleDescription"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                     && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                  {
                      if (ds.Tables[0].Rows[i]["AlarmDescription"].ToString().Contains("DTC codes:")) //DTC Code
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                          strStyle,
                          ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                          Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                          ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                          ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(",", "<br>"));
                      }
                      else
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                          strStyle,
                          ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                          Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                          ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                          ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", "<br>"));
                      }

                  }
                  else
                  {
                      if (ds.Tables[0].Rows[i]["AlarmDescription"].ToString().Contains("DTC codes:")) //DTC Code
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                            strStyle,
                            ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                            Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                            ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                            ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(";", "<br>"));
                      }
                      else
                      {
                          strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                               strStyle,
                               ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                               Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                               ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                               ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", "<br>"));
                      }
                  }
                  //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                  AlarmCount++;
              }

              //----------------------

          }

          strAlarms = strBuild.ToString();
          sn.Map.AlarmsHTML = strAlarms;
          sn.Map.AlarmCount = AlarmCount;

          Response.Write(sn.Map.AlarmsHTML + "~" + sn.Map.AlarmCount + "~" + sn.Map.AlarmsCheckSum);

          watch.Stop();
          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID + ", Database Call(sec):" + Math.Round(watch.Elapsed.TotalSeconds, 2)));
          Response.End();  
      }

        void GetAlarmsXML(Object source, ServerAlarms.GetAlarmsShortInfoCheckSumCompletedEventArgs  e)
        {
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
            // Changes for TimeZone Feature start
            float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);// Changes for TimeZone Feature end
            objUtil = new clsUtility(sn);

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

            _checksum  = e.checksum;
            if (_checksum == null || _checksum == "")
                return;

            if (sn.Map.AlarmsCheckSum != _checksum)
            {
                sn.Map.AlarmsCheckSum = _checksum;
                if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), false))
                    if (objUtil.ErrCheck(alarms.GetAlarmsShortInfoXMLByLang_NewTZ(sn.UserID, sn.SecId, timeZone, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref _xml), true))
                    {
                        return;
                    }
            }
            else
            {
                Response.Write(sn.Map.AlarmsHTML + "~" + sn.Map.AlarmCount + "~" + sn.Map.AlarmsCheckSum);  
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

                    if (ds.Tables[0].Rows[i]["vehicleDescription"].ToString() == VLF.CLS.Def.Enums.AlarmState.New.ToString()
                       && ds.Tables[0].Rows[i]["AlarmLevel"].ToString() == VLF.CLS.Def.Enums.AlarmSeverity.Critical.ToString())
                    {
                        if (ds.Tables[0].Rows[i]["AlarmDescription"].ToString().Contains("DTC codes:")) //DTC Code
                        {
                            strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                            strStyle,
                            ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                            Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                            ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                            ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(",","<br>"));
                        }
                        else
                        {
                            strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> <IMG alt='' border=0  src='../images/exclameanim.gif'> {2} {3}<br>[{4}]</u></a></p><br>",
                            strStyle,
                            ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                            Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                            ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                            ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", "<br>"));
                        }
                        
                    }
                    else
                    {
                        if (ds.Tables[0].Rows[i]["AlarmDescription"].ToString().Contains("DTC codes:")) //DTC Code
                        {
                            strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                              strStyle,
                              ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                              Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                              ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                              ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", " ").Replace(";", "<br>"));
                        }
                        else
                        {
                            strBuild.AppendFormat("<p><a href='#' {0} onclick=NewWindow('{1}')> {2} {3}<br>[{4}]</u></a></p><br>",
                                 strStyle,
                                 ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd(),
                                 Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString(),
                                 ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd(),
                                 ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd().Replace("\n", "<br>"));
                        }
                    }
                    //str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + ds.Tables[0].Rows[i]["AlarmId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(ds.Tables[0].Rows[i]["TimeCreated"].ToString().TrimEnd(), new CultureInfo("en-US")).ToLongTimeString() + " " + ds.Tables[0].Rows[i]["vehicleDescription"].ToString().TrimEnd() + "<br>" + "[" + ds.Tables[0].Rows[i]["AlarmDescription"].ToString().TrimEnd() + "]" + "</u></a></p><br>");

                    AlarmCount++;
                }

                //----------------------

            }

            strAlarms = strBuild.ToString();
            sn.Map.AlarmsHTML = strAlarms;
            sn.Map.AlarmCount = AlarmCount;
            
            Response.Write(sn.Map.AlarmsHTML + "~" + sn.Map.AlarmCount+"~"+sn.Map.AlarmsCheckSum);  

            watch.Stop();
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetAlarmsShortInfoXMLByLang User ID:" + sn.UserID+", Database Call(sec):" + Math.Round(watch.Elapsed.TotalSeconds,2)));
            Response.End();  
        }



        protected void Page_Init(object sender, EventArgs e)
        {

            sn = (SentinelFMSession)Session["SentinelFMSession"];

            if (sn.User.MenuColor != "")
                headerColor = sn.User.MenuColor;



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

