using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using VLF.DAS.Logic;
using SentinelFM.ServerDBOrganization;

public partial class ScheduleAdherence_frmReasonCodeList : SABasePage
{
    protected override void PageLoad()
    {
        lbError.Text = "";
        lb_OptionsError.Text = "";
        if (!IsPostBack)
            InitPage();
    }

    private void InitPage()
    {
        DataSet ds = GetSASetting();
        InitSetting(ds);
        BindGVReasonCodeList();
    }

    private DataSet GetSASetting()
    {
        int organizationId = sn.User.OrganizationId;
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        return db.GetSASetting(organizationId);
    }

    private void InitSetting(DataSet ds)
    {
        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            NewSetting();
        else
            BindData(ds);
    }

    private void BindData(DataSet ds)
    {
        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return;
        Table_options.Visible = true;
        DataRow row = ds.Tables[0].Rows[0];
        txtWindowsAfter.Text = CalculatOutput(row, "WindowsAfterSeconds").ToString();
        txtWindowsbefore.Text = CalculatOutput(row, "WindowBeforeSeconds").ToString();
        txtRSCArrivalEarly.Text = CalculatOutput(row, "RSCArrivalEarlySeconds").ToString();
        txtRSCArrivalLate.Text = CalculatOutput(row, "RSCArrivalLateSeconds").ToString();
        txtRSCDepartEarly.Text = CalculatOutput(row, "RSCDepartEarlySeconds").ToString();
        txtRSCDepartLate.Text = CalculatOutput(row, "RSCDepartLateSeconds").ToString();
        txtStopArrivalEarly.Text = CalculatOutput(row, "StopArrivalEarlySeconds").ToString();
        txtStopArrivalLate.Text = CalculatOutput(row, "StopArrivalLateSeconds").ToString();
        txtStopDepartEarly.Text = CalculatOutput(row, "StopDepartEarlySeconds").ToString();
        txtStopDepartLate.Text = CalculatOutput(row, "StopDepartLateSeconds").ToString();
        DDL_FileFormat.SelectedValue = row["ImportFormat"].ToString();
    }

    private int CalculatOutput(DataRow row, string fieldName)
    {
        if (row[fieldName] == DBNull.Value) return 0;
        int seconds = int.Parse(row[fieldName].ToString());
        int minutes = seconds / 60;
        return minutes;
    }

    private void NewSetting()
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.SaveSASetting(sn.User.OrganizationId, 18000, 18000, 1800, 1800, 3600, 3600,
            3600, 3600, 3600, 3600);
        db.AddReasonCode(sn.User.OrganizationId, "1", "On Time", sn.UserID);
        InitPage();
    }

    protected void cmdSaveFormat_Click(object sender, System.EventArgs e)
    {
        string format;
        if (string.IsNullOrEmpty(DDL_FileFormat.SelectedValue))
            format = null;
        else
            format = DDL_FileFormat.SelectedValue;
        int organizationId = sn.User.OrganizationId;
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.UpdateFileFormat(organizationId, format);
    }

    protected void cmdSave_Click(object sender, System.EventArgs e)
    {
        int organizationId = sn.User.OrganizationId;
        int winBefore = 0;
        if (!int.TryParse(txtWindowsbefore.Text, out winBefore))
        {
            lb_OptionsError.Text = "Before Windows need a number.";
            return;
        }
        winBefore = winBefore * 60;
        int winAfter = 0;
        if (!int.TryParse(txtWindowsAfter.Text, out winAfter))
        {
            lb_OptionsError.Text = "After Windows need a number.";
            return;
        }
        winAfter = winAfter * 60;
        int rscArrivalEarly = 0;
        if (!int.TryParse(txtRSCArrivalEarly.Text, out rscArrivalEarly))
        {
            lb_OptionsError.Text = "Early for Depot arriva need a number.";
            return;
        }
        rscArrivalEarly = rscArrivalEarly * 60;
        int rscArrivalLate = 0;
        if (!int.TryParse(txtRSCArrivalLate.Text, out rscArrivalLate))
        {
            lb_OptionsError.Text = "Late for Depot arrival need a number.";
            return;
        }
        rscArrivalLate = rscArrivalLate * 60;
        int rscDepartEarly = 0;
        if (!int.TryParse(txtRSCDepartEarly.Text, out rscDepartEarly))
        {
            lb_OptionsError.Text = "Early for Depot departure need a number.";
            return;
        }
        rscDepartEarly = rscDepartEarly * 60;
        int rscDepartLate = 0;
        if (!int.TryParse(txtRSCDepartLate.Text, out rscDepartLate))
        {
            lb_OptionsError.Text = "Late for Depot departure need a number.";
            return;
        }
        rscDepartLate = rscDepartLate * 60;
        int stopArrivalEarly = 0;
        if (!int.TryParse(txtStopArrivalEarly.Text, out stopArrivalEarly))
        {
            lb_OptionsError.Text = "Early for Station arriva need a number.";
            return;
        }
        stopArrivalEarly = stopArrivalEarly * 60;
        int stopArrivalLate = 0;
        if (!int.TryParse(txtStopArrivalLate.Text, out stopArrivalLate))
        {
            lb_OptionsError.Text = "Late for Station arriva need a number.";
            return;
        }
        stopArrivalLate = stopArrivalLate * 60;
        int stopDepartEarly = 0;
        if (!int.TryParse(txtStopDepartEarly.Text, out stopDepartEarly))
        {
            lb_OptionsError.Text = "Early for Station depature need a number.";
            return;
        }
        stopDepartEarly = stopDepartEarly * 60;
        int stopDepartLate = 0;
        if (!int.TryParse(txtStopDepartLate.Text, out stopDepartLate))
        {
            lb_OptionsError.Text = "Late for Station depature need a number.";
            return;
        }
        stopDepartLate = stopDepartLate * 60;
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        db.SaveSASetting(organizationId, winBefore, winAfter, rscDepartEarly, rscDepartLate, rscArrivalEarly, rscArrivalLate,
            stopDepartEarly, stopDepartLate, stopArrivalEarly, stopArrivalLate);
    }

    private void BindGVReasonCodeList()
    {
        BindGVReasonCodeList(null);
    }

    private void BindGVReasonCodeList(int? pageIndex)
    {
        DataView dv = GetAllReasonCodes();
        gvReasonCode.DataSource = dv;
        if (pageIndex != null && pageIndex.Value >=0)
            gvReasonCode.PageIndex = pageIndex.Value;
        gvReasonCode.DataBind();
    }

    private DataView GetAllReasonCodes()
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsReasonCodes = db.GetReasonCodesByOrganizationId(sn.User.OrganizationId);
        dsReasonCodes.Tables[0].DefaultView.Sort = "ReasonCode";
        return dsReasonCodes.Tables[0].DefaultView;
    }

    protected void cmdAddReasonCode_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("frmEditReasonCode.aspx?action=add");
    }
 
    public override SACategory PageCategory
    {
        get { return SACategory.ReasonCode; }
    }

    protected void gvReasonCode_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int ReasonCodeId = -1;
        int.TryParse(hdReasonCode.Value, out ReasonCodeId);
        switch (e.CommandName)
        {
            case "cmdEdit":
                Response.Redirect("frmEditReasonCode.aspx?action=edit&ReasonCodeId=" + ReasonCodeId);
                break;
            case "cmdDelete":
                DeleteReasonCode(ReasonCodeId);
                break;
        }
    }

    private void DeleteReasonCode(int ReasonCodeId)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (db.DeleteReasonCode(ReasonCodeId) == 0)
            lbError.Text = "Delete fail.";
        BindGVReasonCodeList();
    }

    protected void gvReasonCode_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dataRow = e.Row.DataItem as DataRowView;
            if (dataRow == null) return;
            (e.Row.Cells[e.Row.Cells.Count - 2].Controls[0] as ImageButton).OnClientClick = "EditReasonCode(" + dataRow["ReasonCodeId"] + ")";
            (e.Row.Cells[e.Row.Cells.Count - 1].Controls[0] as ImageButton).OnClientClick = "if(!DeleteReasonCode(" + dataRow["ReasonCodeId"] + ", '" + dataRow["ReasonCode"] + "')) return false;";
        }
    }

    protected void gvReasonCode_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        BindGVReasonCodeList(e.NewPageIndex);
    }
}