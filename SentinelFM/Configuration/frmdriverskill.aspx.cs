using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using VLF.DAS.Logic;

namespace SentinelFM
{
    public partial class frmdriverskill : Page
    {
        protected SentinelFMSession sn;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                
            }

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            int organizationId = sn.User.OrganizationId;
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            Organization organization = new Organization(sConnectionString);
            DataSet results = organization.GetOrganizationSkills(organizationId);

            skillsList.DataSource = results.Tables[0];
            skillsList.DataTextField = "SkillName";
            skillsList.DataValueField = "SkillId";
            skillsList.DataBind();

            DriverManager driver = new DriverManager(sConnectionString);
            DataSet driversResult = driver.GetDriversForOrganization(organizationId);
            DriversList.DataSource = driversResult.Tables[0];
            DriversList.DataTextField = "FullName";
            DriversList.DataValueField = "DriverId";
            DriversList.DataBind();
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
            Response.Redirect("frmVehicleInfo.aspx");
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
        protected void btnManageSkills_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmorganizationskills.aspx");
        }

        protected void cmdDriverSkill_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmdriverskill.aspx");
        }
}
}