using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace SentinelFM.Components
{
    public partial class Configuration_Components_ctlMenuTabs : System.Web.UI.UserControl
    {
        public String SelectedControl = string.Empty ;
        public string FleetUrl = string.Empty;
        public string VehicleUrl = string.Empty;
        public string HasOrgCommandName = "true";
        public string IsPolicy = string.Empty;
        public string isJquery = "0";
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isHgiUser = false;
            bool isHydroQuebec = false;
            cmdEquipment.Visible = false;

            
          
            
            if (!string.IsNullOrEmpty(SelectedControl))
            {
                if (FindControl(SelectedControl) is Button)
                {
                    ((Button)FindControl(SelectedControl)).CssClass = "selectedbutton";
                    if (((Button)FindControl(SelectedControl)).ID != btnMaintenance.ID)
                    ((Button)FindControl(SelectedControl)).OnClientClick = "javascript:return false;";
                }
            }
            if (HasOrgCommandName != "false") cmdOrganization.CommandName = "48";
            else cmdOrganization.CommandName = "";
            if (IsPolicy == "true")
            {
                btnPolicies.Visible = true;
                cmdOrganization.Visible = false;
                cmdScheduledTasks.Visible = false;
            }

            if (System.Web.UI.ScriptManager.GetCurrent(this.Page) == null)
            {
                isJquery = "1";
                string relativeUrl = this.ResolveUrl("~/Scripts/jquery-1.4.1.js");
                string myScript = string.Format("<script type='text/javascript' src='{0}'></script>", relativeUrl);
                Literal lit = new Literal();
                lit.Text = myScript;
                this.Page.Header.Controls.Add(lit);
            }
            SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
            bool CmdStatus = false;
            if (sn != null)
            {
                CmdStatus = sn.User.ControlEnable(sn, 59);
                if (sn.UserName.ToLower().Contains("hgi_"))
                {
                    isHgiUser = true;
                }
                else
                {
                    btnServiceAssigment.Visible = false;
                }

                if (sn.User.SuperOrganizationId == 382)
                {
                    btnPanicManagerment.Visible = false;   
                    btnCustomReport.Visible = true;
                    btnWorkinghrs.Visible = true;
                }
                //else
                  //  btnCustomReport.Visible = false;

            }
            if (!CmdStatus) btnWorkinghrs.Visible = false;
            else btnWorkinghrs.Visible = true;

            if (sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955)
            {
                isHydroQuebec = true;
            }

            if (isHydroQuebec || sn.User.OrganizationId == 999630)
            {
                cmdEquipment.Visible = false;
                btnWorkinghrs.Visible = false;
                btnPanicManagerment.Visible = false;
                btnCustomReport.Visible = false;
                btnHOS.Visible = false;

            }

            if (sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999952 || sn.User.OrganizationId == 999954)
            {
                cmdEquipment.Visible = false;
                btnWorkinghrs.Visible = false;
                btnPanicManagerment.Visible = false;
                btnCustomReport.Visible = false;
                btnHOS.Visible = false;
                this.cmdOrganization.Visible = false;   

            }

              
        }

        public string GetMaintenanceUrl(string para)
        {
            return this.ResolveUrl("~/Maintenance/frmMaintenanceNew.aspx") + "?type=" + para;
        }

        protected void cmdFleets_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(FleetUrl)) Response.Redirect("~/Configuration/frmEmails.aspx");
            else Response.Redirect(FleetUrl);
        }
        protected void cmdVehicles_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(VehicleUrl)) Response.Redirect("~/Configuration/frmVehicleInfo.aspx");
            else Response.Redirect(VehicleUrl);
        }
        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Configuration/frmTaskScheduler.aspx");
        }
        protected void cmdUsers_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Configuration/frmUsers.aspx");
        }
        protected void cmdEquipment_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Configuration/Equipment/frmEquipment.aspx");
        }

        protected void cmdServiceAssignment_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Configuration/frmServiceAssignment.aspx");
        }
    }
}