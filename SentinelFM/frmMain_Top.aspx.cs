using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Globalization;
public partial class frmMain_Top : SentinelFMBasePage
{
    public string Report_Type = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {


	 /// Set the Title of the page
        if (sn.SelectedLanguage.ToLower().Contains("en"))
            Page.Title = "Fleet Management & Security";
        else if (sn.SelectedLanguage.ToLower().Contains("fr"))
            Page.Title = "Gestion de flotte et sécurité";        


        Report_Type = clsReportMessage.Report_Type;

	string cScript="function pageLoad(sender, eventArgs) {setInterval('frmMain_KeepAlive()', 60*1000);}";

         if (!(sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 ))
           ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "KeepAlive", cScript, true);





         if ((sn.User.OrganizationId == 999630) || (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 1000060 || sn.User.OrganizationId == 1000051 || sn.User.OrganizationId == 1000065 || sn.User.OrganizationId == 952))
        {
            if (Request.Url.ToString().ToLower().Contains("http:"))
            {
                string script = "try {if (window.location.href.toLowerCase().indexOf('https:') < 0) {window.location.href = window.location.href.toLowerCase().replace(/http:/, 'https:');}} catch(err){};";
                ScriptManager scriptManager = ScriptManager.GetCurrent(this);
                if (scriptManager != null && scriptManager.IsInAsyncPostBack)
                {
                    //if a MS AJAX request, use the Scriptmanager class
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "httpsscript", script, true);
                }
                else
                {
                    //if a standard postback, use the standard ClientScript method
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "httpsscript", script, true);
                }
            }
        }


    }
}