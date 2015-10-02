using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using VLF.DAS.Logic;

public partial class ScheduleAdherence_SAMasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        cmdSchedule.Style[HtmlTextWriterStyle.Width] = "auto";
        Button activeButton = GetCurActiveButton();
        if (activeButton != null)
        {
            activeButton.CssClass = "selectedbutton";
            activeButton.OnClientClick = "javascript:return false;";
        }
    }

    private Button GetCurActiveButton()
    {
        SABasePage saPage = Page as SABasePage;
        if (saPage == null) return null;
        switch (saPage.PageCategory)
        {
            case SABasePage.SACategory.Station:
                return cmdStation;
            case SABasePage.SACategory.Schedule:
                return cmdSchedule;
            case SABasePage.SACategory.Report:
                return cmdReport;
            case SABasePage.SACategory.Chart:
                return cmdChart;
            case SABasePage.SACategory.ReasonCode:
                return cmdReasonCode;
            default:
                return null;
        }
    }
}

public abstract class SABasePage : SentinelFMBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int organizationId = sn.User.OrganizationId;
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }
        if (PageCategory != SACategory.ReasonCode && !HaveSASetting())
        {
            Response.Redirect("frmReasonCodeList.aspx");
        }
        PageLoad();
    }

    protected virtual void PageLoad()
    {
    }

    public abstract SACategory PageCategory
    {
        get;
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

    protected string RealTimeDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        }
    }
    protected string ReportDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        }
    }
    protected string SpatialDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.AppSettings["SpatialDB"];
        }
    }
    public enum SACategory
    {
        Station,
        Schedule,
        Chart,
        ReasonCode,
        Report
    }
}
