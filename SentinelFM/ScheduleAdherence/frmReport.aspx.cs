using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using VLF.DAS.Logic;
using SentinelFM;

public partial class ScheduleAdherence_frmReport : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    public string DefaultRoute = "";
    public string DefaultVehicle = "";
    public string DefaultStation = "";
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

        if (!IsPostBack)
        {
            GetParameter();
            if (!HaveSASetting())
            {
                Response.Redirect("frmReasonCodeList.aspx");
            }
        }
    }

    private void GetParameter()
    {
        if (Request["Route"] != null)
            DefaultRoute = Request["Route"];
        if (Request["Vehicle"] != null)
            DefaultVehicle = Request["Vehicle"];
        if (Request["Station"] != null)
            DefaultStation = Request["Station"];
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