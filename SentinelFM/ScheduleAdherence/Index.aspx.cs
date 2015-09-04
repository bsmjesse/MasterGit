using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Configuration;
using VLF.DAS.Logic;

public partial class ScheduleAdherence_Index : System.Web.UI.Page
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
        if (!HaveSASetting())
        {
            Response.Redirect("frmReasonCodeList.aspx");
        }

    }

    private bool HaveSASetting()
    {
        int organizationId = sn.User.OrganizationId;
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet ds = db.GetSASetting(organizationId);
        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            return false;
        else
            return true;
    }

    protected string ReportDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        }
    }
}