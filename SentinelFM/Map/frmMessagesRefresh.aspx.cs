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
using VLF.CLS;

namespace SentinelFM
{
   public partial class Map_frmMessagesRefresh : SentinelFMBasePage
   {
      
      protected System.Web.UI.WebControls.Label lblTotalAlarms;
      

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {

            if (!Page.IsPostBack)
            {
                MessagesList_Fill_NewTZ();
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


      // Changes for TimeZone Feature start

      private void MessagesList_Fill_NewTZ()
      {
          try
          {
              string str = "";
              sn.Map.LoadMessages_NewTZ(sn, ref str);

              if (sn.Map.MessagesHTML == "" || sn.Map.MessagesHTML != str)
              {
                  sn.Map.MessagesHTML = str;
                  Response.Write("<script language='javascript'> parent.frmMessageRotator.location.href='frmmessagerotating.aspx' </script>");
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

      private void MessagesList_Fill()
      {
         try
         {
            string str = "";
            sn.Map.LoadMessages(sn, ref str);
  
            if (sn.Map.MessagesHTML == "" || sn.Map.MessagesHTML != str)
            {
               sn.Map.MessagesHTML = str;
               Response.Write("<script language='javascript'> parent.frmMessageRotator.location.href='frmmessagerotating.aspx' </script>");
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
