using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Configuration_frmServiceAssignment : SentinelFMBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void cmdSettings_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmOrganizationSettings.aspx");
    }
    protected void cmdFuel_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmFuelTranSettings.aspx");
    }
    protected void cmdDriver_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmdrivers.aspx");
    }
    protected void cmdUsers_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmUsers.aspx");
    }
    protected void cmdVehicles_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmVehicleInfo.aspx");
    }
    protected void cmdFleets_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmEmails.aspx");
    }

    protected void cmdServiceAssignment_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmServiceAssignment.aspx");
    }

    protected void cmdPushSettings_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmOrganizationPushSettings.aspx");
    }
    protected void cmdScheduledTasks_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmTaskScheduler.aspx");
    }
    protected void cmdPanicMangement_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmPanicManagement.aspx");
    }
    protected void cmdMapSubscription_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmmapsubscription.aspx");
    }
    protected void cmdOverlaySubscription_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmoverlaysubscription.aspx");
    }
    protected void cmdHierarchy_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmorganizationhierarchy.aspx");
    }
    protected void cmdHierarchyImport_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmorganizationhierarchyImport.aspx");
    }
    protected void cmdHierarchyAssignmentImport_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmorganizationhierarchyAssignmentImport.aspx");
    }
}