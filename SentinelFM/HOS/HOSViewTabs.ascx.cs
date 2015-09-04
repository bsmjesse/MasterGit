using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Configuration;

public partial class HOS_HOSViewTabs : System.Web.UI.UserControl
{
    public string SelectedControl = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SelectedControl))
        {
            if (FindControl(SelectedControl) is Button)
            {
                ((Button)FindControl(SelectedControl)).CssClass = "selectedbutton";
                ((Button)FindControl(SelectedControl)).OnClientClick = "javascript:return false;";
            }
        }
        SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
        //if (sn.User.OrganizationId == 480) cmdDynamicForms.Visible = true;
        //if (!sn.User.ControlEnable(sn, 84))
        //    cmdLogs.Visible = false;
        if (!sn.User.ControlEnable(sn, 85))
            cmdFuel.Visible = false;
        if (!sn.User.ControlEnable(sn, 86))
            cmdHistory.Visible = false;
        if (!sn.User.ControlEnable(sn, 87))
            cmdDynamicForms.Visible = false;
        try
        {
            if (EnableVehicleInspection(sn.User.OrganizationId, sn.User.UserGroupId))
            {
                cmdVehicleInspection.Visible = true;
            }
            else
            {
                cmdVehicleInspection.Visible = false;
            }
        }
        catch (Exception)
        {
        }
        
    }

    private bool EnableVehicleInspection(int CompanyId, int UserGroupId)
    {
        string rapidLog = ConfigurationManager.AppSettings["RapidLog"];
        if ("1".Equals(rapidLog) && CompanyId == 123 && UserGroupId <= 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}