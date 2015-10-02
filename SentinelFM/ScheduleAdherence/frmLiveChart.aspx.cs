using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;

public partial class ScheduleAdherence_frmLiveChart : System.Web.UI.Page
{
    protected SentinelFMSession sn;

    protected void Page_Load(object sender, EventArgs e)
    {
        sn = Session["SentinelFMSession"] as SentinelFMSession;
        if (sn == null)
        {
            Response.Redirect("../Login.aspx");
            return;
        }
        int organizationId = sn.User.OrganizationId;
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }

    }
}