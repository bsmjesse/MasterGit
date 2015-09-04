using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace SentinelFM.Components
{
    public partial class Configuration_Components_ctlOrganizationSubMenuTabs : System.Web.UI.UserControl
    {
        public String SelectedControl = string.Empty;
        
        private SentinelFMSession sn; 
        
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];

            if (!string.IsNullOrEmpty(SelectedControl))
            {
                if (FindControl(SelectedControl) is Button)
                {
                    ((Button)FindControl(SelectedControl)).CssClass = "selectedbutton";
                    ((Button)FindControl(SelectedControl)).OnClientClick = "javascript:return false;";
                }
            }

            bool isHgiUser = (sn != null && sn.User.UserGroupId == 1) ? true : false;
            
            if (!Page.IsPostBack)
            {
                //Devin
                if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 480)
                {
                    btnFuelCategory.Visible = true;
                }
                
                //if (sn.SuperOrganizationId == 382)
                this.cmdFuel.Enabled = true;

                if (!isHgiUser)
                {
                    cmdMapSubscription.Visible = false;
                    cmdOverlaySubscription.Visible = false;
                }

                if(sn.User.ControlEnable(sn, 94))
                    cmdMapSubscription.Visible = true;

                if (sn.User.ControlEnable(sn, 95))
                    cmdOverlaySubscription.Visible = true;

                HideButtons();                
            }
        }

        protected void cmdSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmOrganizationSettings.aspx");
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
        protected void cmdFuel_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmFuelTranSettings.aspx");
        }
        protected void cmdQueryWindows_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmQueryWindows.aspx");
        }

        private void HideButtons()
        {
            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
            {
                this.cmdPushSettings.Visible = false;
                this.cmdFuel.Visible = false;
                this.cmdPanicMangement.Visible = false;
            }


            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
            {
                this.cmdHierarchy.Visible = true;
                this.cmdHierarchyAssignmentImport.Visible = true;
                this.cmdHierarchyImport.Visible = true;
            }
            else
            {
                this.cmdHierarchy.Visible = false;
                this.cmdHierarchyAssignmentImport.Visible = false;
                this.cmdHierarchyImport.Visible = false;
            }

            if (sn.User.OrganizationId == 999630)
                this.cmdPanicMangement.Visible = false;

            if (sn.User.OrganizationId != 951)
                this.btnQueryWindows.Visible = false;

        }
    }
}