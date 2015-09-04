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
/// Summary description for BaseControl
/// </summary>
public class BaseControl: System.Web.UI.UserControl
{
    protected SentinelFMSession sn = null;
    protected clsUtility objUtil;
	public BaseControl()
	{
		//
		// TODO: Add constructor logic here
		//
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
            SentinelFM.SentinelFMSession snMain = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
            if (snMain != null && snMain.UserID != null)
                UserId = snMain.UserID;
            else
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
        }
        catch
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
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

                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - UserId:" + UserId.ToString() + " , Form:" + frmName + " , Error :" + Excp));
            }
            else
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  UserId:" + UserId.ToString() + " , Form:" + frmName + ", Stack:" + strStack));
        }
        catch
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - not exception info,  Form:" + frmName));
        }

        Session.Abandon();
        Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
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

}
