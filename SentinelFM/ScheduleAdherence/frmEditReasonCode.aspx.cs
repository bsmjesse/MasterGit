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

public partial class ScheduleAdherence_frmEditReasonCode : SABasePage
{
    protected override void PageLoad()
    {
        if (!IsPostBack)
        {
            hdAction.Value = Request["Action"];
            if (hdAction.Value != "add")
            {
                hdReasonCodeId.Value = Request["reasonCodeId"];
                BindReasonCodeData();
            }
        }
    }

    public override SACategory PageCategory
    {
        get { return SACategory.ReasonCode; }
    }

    private void BindReasonCodeData()
    {
        int ReasonCodeId = int.Parse(hdReasonCodeId.Value);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsReasonCode = db.GetReasonCodeById(ReasonCodeId);
        if (dsReasonCode == null || dsReasonCode.Tables.Count == 0 || dsReasonCode.Tables[0].Rows.Count == 0) return;
        DataRow row = dsReasonCode.Tables[0].Rows[0];
        txtReasonCode.Text = row["ReasonCode"].ToString();
        if (txtReasonCode.Text == "1")
            txtReasonCode.Enabled = false;
        txtDescription.Text = row["Description"].ToString();
    }

    protected void cmdSave_Click(object sender, System.EventArgs e)
    {
        string reasonCode = txtReasonCode.Text;
        string description = txtDescription.Text;

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (hdAction.Value == "add")
            db.AddReasonCode(sn.User.OrganizationId, reasonCode, description, sn.UserID);
        else
            db.UpdateReasonCode(int.Parse(hdReasonCodeId.Value), reasonCode, description, sn.UserID);

        Response.Redirect("frmReasonCodeList.aspx");
    }

    protected void cmdCancel_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("frmReasonCodeList.aspx");
    }
}