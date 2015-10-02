using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace SentinelFM
{
    public partial class frmorganizationskills : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        protected void cmdDriverSkill_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmdriverskill.aspx");
        }
    }
}