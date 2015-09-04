using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmVehicleFleet.
    /// </summary>
    public partial class frmVehicleFleet : SentinelFMBasePage
    {




        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            bool ShowOrganizationHierarchy = false;
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
            }

            if (ShowOrganizationHierarchy)
            {
                trFleetSelectOption.Visible = true;
            }
            else
            {
                trFleetSelectOption.Visible = false;
            }

            if (!Page.IsPostBack)
            {


                //Devin
                //if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 480)
                //{
                    //btnFuelCategory.Visible = true;
                //}

                if (sn.User.SuperOrganizationId == 382)
                    this.btnEquipmentAssignment.Visible = true;
                else
                    this.btnEquipmentAssignment.Visible = false;

                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleFleetForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);


                if (sn != null && !sn.UserName.ToLower().Contains("hgi_"))
                {
                    HideButtons();
                }
            }
        }

        protected void optAssignBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optAssignBased.SelectedItem.Value == "0")
            {
                trBasedOnNormalFleet.Visible = true;
                trBasedOnHierarchyFleet.Visible = false;
            }
            else
            {
                trBasedOnNormalFleet.Visible = false;
                trBasedOnHierarchyFleet.Visible = true;
            }
        }

        protected void cmdVehicleInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        private void cmdPreference_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmPreference.aspx");
        }



        protected void cmdAlarms_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmAlarms.aspx");
        }

        protected void cmdOutputs_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmOutputs.aspx");
        }

        private void cmdVehicleGeoZone_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleGeoZoneAss.htm");
        }

        private void cmdLandmarks_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        protected void cmdFleets_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        protected void cmdUsers_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }



        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx"); 
        }

        private void HideButtons()
        {

            if (sn.User.OrganizationId == 999630)
            {
                this.cmdAlarms.Visible = false;
                this.cmdOutputs.Visible = false;

            }

        }
}
}
