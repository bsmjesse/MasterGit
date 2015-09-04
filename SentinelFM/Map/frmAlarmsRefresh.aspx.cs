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
 

namespace SentinelFM
{
   public partial class Map_frmAlarmsRefresh : SentinelFMBasePage
   {
      public string strAlarms;
      
      

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
           

            if (!Page.IsPostBack)
               AlarmsList_Fill_NewTZ();
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

      // Changes for TimeZone Feature start

      private void AlarmsList_Fill_NewTZ()
      {
          try
          {
              string str = "";
              int AlarmCount = 0;

              sn.Map.LoadAlarms_NewTZ(sn, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
              if (sn.Map.AlarmsHTML == "" || sn.Map.AlarmsHTML != str)
              {
                  sn.Map.AlarmsHTML = str;
                  sn.Map.AlarmCount = AlarmCount;
                  Response.Write("<script language='javascript'> parent.frmAlarmRotator.location.href='frmalarmrotating.aspx' </script>");
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
          }
      }

      // Changes for TimeZone Feature end
      private void AlarmsList_Fill()
      {
         try
         {
            string str = "";
            int AlarmCount = 0;

            sn.Map.LoadAlarms(sn,CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref str, ref AlarmCount);
            if (sn.Map.AlarmsHTML == "" || sn.Map.AlarmsHTML != str)
            {
               sn.Map.AlarmsHTML = str;
               sn.Map.AlarmCount = AlarmCount; 
               Response.Write("<script language='javascript'> parent.frmAlarmRotator.location.href='frmalarmrotating.aspx' </script>");
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
