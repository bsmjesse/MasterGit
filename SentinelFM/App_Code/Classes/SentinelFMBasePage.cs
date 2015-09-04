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
using System.Globalization;
using System.Text;
using System.IO;
using System.Diagnostics;
using VLF.CLS;
/// <summary>
/// Summary description for SentinelFMBasePage
/// </summary>
public class SentinelFMBasePage : System.Web.UI.Page     
{


    protected SentinelFMSession sn = null;
    protected clsUtility objUtil;
    public SentinelFMBasePage()
    {
        //this.ShowErrorsAtClient = false;
        //this.ThrowExceptionsAtClient = true;

    }






    protected override void OnLoad(EventArgs e)
    {
        try
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);
            sn = (SentinelFMSession)Session["SentinelFMSession"];

            objUtil = new clsUtility(sn);

        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form: " + Page.GetType().Name));

        }

        base.OnLoad(e);
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
            if (snMain != null && snMain.UserID != null && snMain.UserID != 0)
                UserId = snMain.UserID;
            else
            {
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session is null, SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " IP:" + Request.ServerVariables["remote_addr"]));
                return;
            }
        }
        catch
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName + " IP:" + Request.ServerVariables["remote_addr"]));
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

                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - UserId:" + UserId.ToString() + ", SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName + " , Error :" + Excp + " IP:" + Request.ServerVariables["remote_addr"]));
            }
            else
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  UserId:" + UserId.ToString() + ", SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Form:" + frmName + ", Stack:" + strStack + " IP:" + Request.ServerVariables["remote_addr"]));
        }
        catch
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - not exception info, SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " ,  Form:" + frmName + ", Stack:" + strStack + " IP:" + Request.ServerVariables["remote_addr"]));
        }

        Session.Abandon();
        //By Devin
        if (ScriptManager.GetCurrent(this) != null)
        {
            string relativeUrl = this.ResolveUrl("~/login.aspx");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScript", "<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('" + relativeUrl + "','_top')</SCRIPT>", false);
        }
        else 
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
        return;
    }

    protected override void InitializeCulture()
    {

        SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
        if (snMain == null || snMain.User == null || String.IsNullOrEmpty(snMain.UserName))
        {
            CreateSession();
            snMain = (SentinelFMSession)Session["SentinelFMSession"];

        }

        if (snMain.SelectedLanguage != null)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new
                CultureInfo(snMain.SelectedLanguage);

            System.Threading.Thread.CurrentThread.CurrentCulture = new
             CultureInfo("en-US");

            base.InitializeCulture();
        }
        else
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new
               CultureInfo("en-US");

            base.InitializeCulture();
        }
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
               String.Format("Local Resource [{0}] not found. {1} {2} {3}", resourceName, ex.Message, Page.GetType().Name, ex.StackTrace)));
        }
        return result;
    }

    public void DumpBeforeCall(SentinelFMSession sn, string msg)
    {

        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo,
            VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info,
            string.Format("DumpBeforeCall --> sn={0} userId={1} username = {2} form={3} *** {4}",
                               null == sn ? "NULL" : Session.SessionID, sn.UserID, sn.UserName, Page.GetType().Name, msg)));

    }

    public void ExceptionLogger(System.Diagnostics.StackTrace trace)
    {
        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
            VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Method: " + trace.GetFrame(0).GetMethod().Name + ", Line: " + trace.GetFrame(0).GetFileLineNumber() + ", Column: " + trace.GetFrame(0).GetFileColumnNumber() + " User:" + sn.UserID.ToString() + " Form: " + Page.GetType().Name));
    }

    public void ExceptionLogger(string message, Exception ex)
    {
        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                       VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                       message + ", Exception: " + ex.Message + " ---" + ex.StackTrace.ToString()));
    }

    public void ShowMessage(Label label, string text, System.Drawing.Color color)
    {
        label.ForeColor = color;
        label.Text = text;
    }



    public static void BTraceLong(int verbosity_, string format, params object[] objects)
    {
        try
        {

            StringBuilder strDynamic = new StringBuilder();
            strDynamic.AppendFormat(format, objects);
            //            Trace.WriteLine(strDynamic.ToString()) ;
            VLF.CLS.Def.Enums.TraceSeverity sev = VLF.CLS.Def.Enums.TraceSeverity.Info;
            if (verbosity_ == Util.ERR0)
                System.Diagnostics.Trace.WriteLine(TraceFormatLong(VLF.CLS.Def.Enums.TraceSeverity.Error, strDynamic.ToString()));
            else if (verbosity_ == Util.WARN0)
                System.Diagnostics.Trace.WriteLine(TraceFormatLong(VLF.CLS.Def.Enums.TraceSeverity.Warning, strDynamic.ToString()));
            else System.Diagnostics.Trace.WriteLine(TraceFormatLong(VLF.CLS.Def.Enums.TraceSeverity.Info, strDynamic.ToString()));
        }
        catch (Exception exc)
        {
            System.Diagnostics.Trace.WriteLine("-- Util.BTrace -> WRONG FORMATTED DATA ---" + exc.Message);
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {

        sn = (SentinelFMSession)Session["SentinelFMSession"];
        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            CreateSession();

        objUtil = new clsUtility(sn);


        try
        {
            HtmlGenericControl hgc = new HtmlGenericControl("style");
            hgc.Attributes.Add("type", "text/css");
            //Devin changed
            String filePath = HttpContext.Current.Server.MapPath("~/GlobalStyle.css");
            //End
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            if (sn.User.MenuColor != "")
                content = content.Replace("#009933", sn.User.MenuColor);
            if (sn.User.ConfigTabBackColor != "")
                content = content.Replace("#fffff0", sn.User.ConfigTabBackColor);


            hgc.InnerText = content;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            hgc.RenderControl(hw);
            //By Devin

            if (IsPostBack && (ScriptManager.GetCurrent(this) != null))
            {
                if (ScriptManager.GetCurrent(this).IsInAsyncPostBack) return;
            }

            //if (Request.Url.ToString().ToLower().Contains("/mapnew/"))
            //{
                Page.Header.Controls.Add(
                    new LiteralControl(sb.ToString())
                );
            //}
            //else 
            //    Response.Write(sb.ToString());
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form: " + Page.GetType().Name));
        }

    }

    public static string TraceFormatLong(VLF.CLS.Def.Enums.TraceSeverity severity, string msg)
    {
        return DateTime.Now.ToString("M/d/yyyy HH:mm:ss.fff") + "> " + VLF.CLS.Def.Enums.TraceSeverityMessage[(int)severity] + ":: " + msg;
    }

    public string GetResourceString(string id)
    {
        string value = string.Empty;
        object x = this.GetLocalResourceObject(id);
        if (x == null) return "????";
        if (x.GetType() == typeof(string))
            value = (string)x;
        return value;
    }


    protected void CreateSession()
    {
        if (Page.Request.QueryString["sn"] != null)
        {
            SentinelFM.SentinelFMSessionBase snb = SentinelFM.SentinelFMSession.GetSessionBaseByKey((string)Page.Request.QueryString["sn"]);
            sn = new SentinelFMSession(snb);
            if (sn == null)
                return;
            Session["SentinelFMSession"] = sn;
        }
        else if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.Query != null && Page.Request.UrlReferrer.Query.StartsWith("?sn="))
        {

            string qs = Page.Request.UrlReferrer.Query.Substring(4);
            SentinelFM.SentinelFMSessionBase snb = SentinelFM.SentinelFMSession.GetSessionBaseByKey(qs);
            sn = new SentinelFMSession(snb);
            if (sn == null)
                return;
            Session["SentinelFMSession"] = sn;

        }
        else
        {
            RedirectToLogin();
            return;
        }

    }

    ///  <summary>

    /// Sets a user's Locale based on the browser's Locale setting. If no setting

    /// is provided the default Locale is used.

    /// </summary>


    //By Devin
    /// <summary>
    /// Get String for javascript, convert quotes character
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string GetScriptEscapeString(string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        string newQuote = @"\" + @"""" ;
        string newQuote1 = @"\" + @"'";
        return str.Replace("\"", newQuote).Replace("'", newQuote1);
    }


    //Devin for resize script
    public string SetResizeScript(string gridID)
    {
        //Must follow this format, because it is defined in control
        string resizeNameStr = string.Format("{0}($find('{1}'), null)", "_gridCreate_" + gridID + "_1", gridID);
        string setResizeScript = string.Format("{0}; setTimeout(\"{1}\",500);", resizeNameStr, resizeNameStr);
        return setResizeScript;
    }

}
