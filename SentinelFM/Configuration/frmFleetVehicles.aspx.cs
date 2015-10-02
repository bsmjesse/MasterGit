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
	/// Summary description for frmFleetVehicles.
	/// </summary>
	public partial class frmFleetVehicles : SentinelFMBasePage
	{
        public bool IsHydroQuebec = false;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956)
                IsHydroQuebec = true;
            else
                IsHydroQuebec = false;

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
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmEmails, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);

                if (IsHydroQuebec || sn.User.OrganizationId == 952)
                {
                    cmdDriverSkill.Visible = false;
                }
            }
		}

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


        protected void optReportBased_SelectedIndexChanged(object sender, EventArgs e)
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

        protected void cmdFleetMng_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmFleets.aspx"); 
		}

		protected void cmdEmails_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmEmails.aspx"); 
		}

		protected void cmdVehicles_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleInfo.aspx") ;
		}

		protected void cmdFleetUsers_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmFleetUsers.aspx"); 
		}

		protected void cmdUsers_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmUsers.aspx"); 
		}
        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx"); 
        }

        protected void cmdDriverSkill_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmdriverskill.aspx");
        }

}
}
