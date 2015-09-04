using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Configuration;
using VLF.DAS.Logic;

public partial class Maintenance_AddMCCMaintenance : System.Web.UI.UserControl
{
    public string errorLoad = "Failed to load data.";
    protected void Page_Load(object sender, EventArgs e)
    {
        
        try
        {
            errorLoad = HttpContext.GetGlobalResourceObject("Const", "Error_Load").ToString();
        }
        catch { }

    }
    protected void ddlMCCMaintenanceOperationTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Session["SentinelFMSession"] == null)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            return;
        }

        SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        MCCManager mccMg = new MCCManager(sConnectionString);
        ddlNotificationType.ClearSelection();
        ddlNotificationType.Items.Clear();
        //while (ddlNotificationType.Items.Count >= 1)
        //    ddlNotificationType.Items.RemoveAt(0);

        ddlNotificationType.DataSource = mccMg.GetMCCNotificationType(sn.User.OrganizationId, int.Parse(ddlMCCMaintenanceOperationTypes.SelectedValue), null, string.Empty).Tables[0];
        ddlNotificationType.DataBind();
        ddlNotificationType.SelectedIndex = 0;
        if (ddlMCCMaintenanceOperationTypes.SelectedValue == "3")
        {
            ddlInterval.Visible = true;
            txtInterval.Visible = false;
            pnlDate.Visible = true;
        }
        else
        {
            ddlInterval.Visible = false;
            txtInterval.Visible = true;
            pnlDate.Visible = false;
        }
    }
    protected void chkFixedDate_CheckedChanged(object sender, EventArgs e)
    {
        if (chkFixedDate.Checked)
        {
            ddlInterval.ClearSelection();
            ListItem item = ddlInterval.Items.FindByValue("1");
            if (item != null) item.Enabled = false;
            item = ddlInterval.Items.FindByValue("2");
            if (item != null) item.Enabled = false;
            item = ddlInterval.Items.FindByValue("3");
            if (item != null) item.Enabled = false;
            item = ddlInterval.Items.FindByValue("9");
            if (item != null) item.Enabled = false;
            item = ddlInterval.Items.FindByValue("999");
            if (item != null) item.Enabled = false;
            item = ddlInterval.Items.FindByValue("8");
            if (item != null) item.Selected = true;

            //ddlInterval.Enabled = false;
            chkFixedInterval.Checked = true;
            chkFixedInterval.Enabled = false;
            pnlMonthDay.Visible = true;
        }
        else
        {
            ListItem item = ddlInterval.Items.FindByValue("1");
            if (item != null) item.Enabled = true;
            item = ddlInterval.Items.FindByValue("2");
            if (item != null) item.Enabled = false;
            item = ddlInterval.Items.FindByValue("3");

            item = ddlInterval.Items.FindByValue("9");
            if (item != null) item.Enabled = true;
            item = ddlInterval.Items.FindByValue("999");
            if (item != null) item.Enabled = true;
            
            pnlMonthDay.Visible = false;
            ddlInterval.Enabled = true;
            chkFixedInterval.Enabled = true;
        }
    }
    protected void ddlInterval_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlMonth.Enabled = true;
        if (chkFixedDate.Checked )
        {
            if (ddlInterval.SelectedValue != "8")
            {
                ListItem listItem = ddlMonth.Items.FindByValue("None");
                if (listItem != null)
                {
                    listItem.Enabled = true;
                    ddlMonth.ClearSelection();
                    listItem.Selected = true;
                }
                ddlMonth.Enabled = false;
            }
            else
            {
                ListItem listItem = ddlMonth.Items.FindByValue("None");
                if (listItem != null)
                {
                    listItem.Enabled = false;
                }
                listItem = ddlMonth.Items.FindByValue("1");
                if (listItem != null)
                {
                    ddlMonth.ClearSelection();
                    listItem.Selected = true;

                }
            }
        }
    }
}