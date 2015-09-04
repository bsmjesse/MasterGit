using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class DriverFinder_Default : System.Web.UI.Page
    {
        protected SentinelFMSession sn;
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            int organizationId = sn.User.OrganizationId;
            if (organizationId == 0)
            {
                Response.Redirect("../Login.aspx");
            }
        }
    }
}