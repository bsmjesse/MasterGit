using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SentinelFM;

/// <summary>
/// Base Page class for a Web form - put all shared code here
/// </summary>
public class BasePage : SentinelFMBasePage
{
   protected SentinelFMSession sn = null;
   protected clsUtility objUtil;

   public BasePage()
   {
      objUtil = new clsUtility(sn);
   }

   public void RedirectToLogin()
    {
        int UserId = 0;
        string frmName = "";
        string strStack = "";

        try
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            strStack = stackTrace.ToString();

        }
        catch
        {
        }


        //Session Check
        try
        {
            SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
            if (snMain != null && snMain.UserID != null)
                UserId = snMain.UserID;
            else
            {
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session is null, SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName + ", Stack:" + strStack));
                return;
            }
        }
        catch
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName));
        }

        //Form Name
        try
        {
            frmName = Page.GetType().Name;
        }
        catch
        {
        }


        //Get Last Error if exists
        try
        {
            if (Server.GetLastError() != null)
            {
                Exception ex = Server.GetLastError().GetBaseException();

                string Excp = "SentinelFM " +
                    "MESSAGE: " + ex.Message +
                    "\nSOURCE: " + ex.Source +
                    "\nFORM: " + frmName +
                    "\nQUERYSTRING: " + Request.QueryString.ToString() +
                    "\nTARGETSITE: " + ex.TargetSite +
                    "\nSTACKTRACE: " + ex.StackTrace;

                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - UserId:" + UserId.ToString() + ", SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName + " , Error :" + Excp));
            }
            else
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  UserId:" + UserId.ToString() + ", SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName + ", Stack:" + strStack));
        }
        catch
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - not exception info, SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " ,  Form:" + frmName + ", Stack:" + strStack));
        }

        Session.Abandon();
        Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../frmTimeOut.aspx','_top')</SCRIPT>");
        return;
    }

   public void GuiSecurity(System.Web.UI.Control obj)
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

   protected override void OnLoad(EventArgs e)
   {
      try
      {
         //Clear IIS cache
         Response.Cache.SetCacheability(HttpCacheability.NoCache);
         Response.Cache.SetExpires(DateTime.Now);
         sn = (SentinelFMSession)Session["SentinelFMSession"];
         if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
         {
            RedirectToLogin();
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
         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form: " + this.Form.Name));
        
      }

      base.OnLoad(e);
   }

   /// <summary>
   /// Display colored text
   /// </summary>
   /// <param name="label">Label control</param>
   /// <param name="text">Text to display</param>
   /// <param name="color">Text color</param>
   public void ShowMessage(Label label, string text, System.Drawing.Color color)
   {
      label.ForeColor = color;
      label.Text = text;
   }

   /// <summary>
   /// GetLocalResourceObject() wrapper with error handling
   /// </summary>
   /// <param name="resourceName">Key Name in a resource file</param>
   /// <returns>Empty string if not found</returns>
   public string GetResource(string resourceName)
   {
      string result = "";
      try
      {
         result = base.GetLocalResourceObject(resourceName).ToString();
      }
      catch (NullReferenceException ex) // resource does not exist
      {
         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
            VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
            String.Format("Local Resource [{0}] not found. {1} {2}", resourceName, ex.Message, ex.StackTrace)));
      }
      return result;
   }
}
