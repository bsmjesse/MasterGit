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
public class BasePage : System.Web.UI.Page
{
   protected SentinelFMSession sn = null;
   protected clsUtility objUtil;

   public BasePage()
   {
      objUtil = new clsUtility(sn);
   }

   public void RedirectToLogin()
   {
      Session.Abandon();
      Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../frmAdminLogin.aspx','_top')</SCRIPT>");
      return;
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
         RedirectToLogin();
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
}
