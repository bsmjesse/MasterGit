using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VLF.DAS.Logic;
using VLF.PATCH.Logic;
using SentinelFM;
using SentinelFM.GeomarkServiceRef;

public partial class ScheduleAdherence_frmEditStationMap : System.Web.UI.Page
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

        if (!IsPostBack)
        {
            hdAction.Value = Request["Action"];
            if (hdAction.Value == "add")
            {
                InitNewInstance();
                EditMode(true);
                cmdDelete.Visible = false;
            }
            else
            {
                hdStationId.Value = Request["stationId"];
                BindStationData();
                EditMode(false);
                cmdDelete.Visible = true;
            }
            SetLabel();
        }
    }

    private void EditMode(bool flag)
    {
        if (flag)
        {
            cmdEdit.Visible = false;
            cmdSave.Visible = true;
            lbName.Visible = false;
            txtName.Visible = true;
            lbNumber.Visible = false;
            txtNumber.Visible = true;
            lbRadius.Visible = false;
            txtRadius.Visible = true;
            lbContact.Visible = false;
            txtContact.Visible = true;
            lbPhone.Visible = false;
            txtPhone.Visible = true;
            lbFax.Visible = false;
            txtFax.Visible = true;
            lbEmail.Visible = false;
            txtEmail.Visible = true;
            lbAddress.Visible = false;
            txtAddress.Visible = true;
            lbDescription.Visible = false;
            txtDescription.Visible = true;
            lbTimezone.Visible = false;
            ddl_timezone.Visible = true;
            lbDayLight.Visible = false;
            ck_dayLight.Visible = true;
        }
        else
        {
            cmdEdit.Visible = true;
            cmdSave.Visible = false;
            lbName.Visible = true;
            txtName.Visible = false;
            lbNumber.Visible = true;
            txtNumber.Visible = false;
            lbRadius.Visible = true;
            txtRadius.Visible = false;
            lbContact.Visible = true;
            txtContact.Visible = false;
            lbPhone.Visible = true;
            txtPhone.Visible = false;
            lbFax.Visible = true;
            txtFax.Visible = false;
            lbEmail.Visible = true;
            txtEmail.Visible = false;
            lbAddress.Visible = true;
            txtAddress.Visible = false;
            lbDescription.Visible = true;
            txtDescription.Visible = false;
            lbTimezone.Visible = true;
            ddl_timezone.Visible = false;
            lbDayLight.Visible = true;
            ck_dayLight.Visible = false;
        }
    }

    private void BindStationData()
    {
        txtRadius.Text = Request["radius"];
        lbRadius.Text = txtRadius.Text;
        hdLon.Value = Request["lon"];
        hdLat.Value = Request["lat"];

        int stationId;
        if (!int.TryParse(hdStationId.Value, out stationId)) return;
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStation = db.GetStationById(stationId);
        if (dsStation == null || dsStation.Tables.Count == 0 || dsStation.Tables[0].Rows.Count == 0) return;
        DataRow row = dsStation.Tables[0].Rows[0];
        int landmarkId = int.Parse(row["LandmarkId"].ToString());
        DataSet dsLandmark = db.GetLandmarkById(landmarkId);
        if (dsLandmark == null || dsLandmark.Tables.Count == 0 || dsLandmark.Tables[0].Rows.Count == 0) return;
        DataRow rowLandmark = dsLandmark.Tables[0].Rows[0];

        txtName.Text = row["Name"].ToString();
        lbName.Text = txtName.Text;

        if (row["TypeId"] == DBNull.Value)
           hdType.Value = "";
        else
           hdType.Value = row["TypeId"].ToString();
        hdLandmarkId.Value = row["LandmarkId"].ToString();
        if (row["StationNumber"] == DBNull.Value)
            txtNumber.Text = "";
        else
            txtNumber.Text = row["StationNumber"].ToString();
        lbNumber.Text = txtNumber.Text;
        if (row["ContractName"] == DBNull.Value)
            txtContact.Text = "";
        else
            txtContact.Text = row["ContractName"].ToString();
        lbContact.Text = txtContact.Text;
        if (row["PhoneNumber"] == DBNull.Value)
            txtPhone.Text = "";
        else
            txtPhone.Text = row["PhoneNumber"].ToString();
        lbPhone.Text = txtPhone.Text;
        if (row["FaxNumber"] == DBNull.Value)
            txtFax.Text = "";
        else
            txtFax.Text = row["FaxNumber"].ToString();
        lbFax.Text = txtFax.Text;
        if (row["Address"] == DBNull.Value)
            txtAddress.Text = "";
        else
            txtAddress.Text = row["Address"].ToString();
        lbAddress.Text = txtAddress.Text;
        if (row["EmailAddress"] == DBNull.Value)
            txtEmail.Text = "";
        else
            txtEmail.Text = row["EmailAddress"].ToString();
        lbEmail.Text = txtEmail.Text;
        if (row["Description"] == DBNull.Value)
            txtDescription.Text = "";
        else
            txtDescription.Text = row["Description"].ToString();
        lbDescription.Text = txtDescription.Text;
        hdCurName.Value = rowLandmark["LandmarkName"].ToString();
        if (rowLandmark["TimeZone"] != DBNull.Value)
        {
            ddl_timezone.SelectedValue = rowLandmark["TimeZone"].ToString();
            lbTimezone.Text = "GMT" + (ddl_timezone.SelectedValue == "0" ? "":ddl_timezone.SelectedValue);
        }
        if (rowLandmark["DayLightSaving"] != DBNull.Value)
        {
            ck_dayLight.Checked = bool.Parse(rowLandmark["DayLightSaving"].ToString());
            lbDayLight.Text = "Automatically adjust for daylight savings time:" + ck_dayLight.Checked;
        }
    }

    private void InitNewInstance()
    {
        hdType.Value = Request["type"];
        txtRadius.Text = Request["radius"];
        hdLon.Value = Request["lon"];
        hdLat.Value = Request["lat"];
        hdStationId.Value = "-1";
        hdLandmarkId.Value = "-1";
    }

    private void SetLabel()
    {
        int type = int.Parse(hdType.Value);
        if (type == 1)
        {
            lb_name.Text = "Station Name";
            lb_number.Text = "Station Number";
            if (hdAction.Value == "add")
                h6_title.InnerHtml = "New Station";
            else
                h6_title.InnerHtml = "Edit Station";
        }
        else
        {
            lb_name.Text = "Depot Name";
            lb_number.Text = "Depot Number";
            if (hdAction.Value == "add")
                h6_title.InnerHtml = "New Depot";
            else
                h6_title.InnerHtml = "Edit Depot";
        }
    }

    protected void cmdEdit_Click(object sender, System.EventArgs e)
    {
        EditMode(true);
    }

    protected void cmdSave_Click(object sender, System.EventArgs e)
    {
        string name = txtName.Text;
        string number = txtNumber.Text;
        int typeId = int.Parse(hdType.Value);
        int landmarkId = int.Parse(hdLandmarkId.Value);
        string contact = txtContact.Text;
        string phone = txtPhone.Text;
        string fax = txtFax.Text;
        string email = txtEmail.Text;
        string address = txtAddress.Text;
        string description = txtDescription.Text;
        double lon = double.Parse(hdLon.Value);
        double lat = double.Parse(hdLat.Value);
        int radius = int.Parse(txtRadius.Text);
        short timezone = short.Parse(ddl_timezone.SelectedValue);
        bool dayLight = ck_dayLight.Checked;


        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        PatchLandmarkPointSet patchLandPointMgr = new PatchLandmarkPointSet(SentinelFMConnection);
        if (hdAction.Value == "add")
        {
            landmarkId = patchLandPointMgr.PatchVlfLandmarkPointSet_Add(sn.User.OrganizationId, name, lat, lon, description, contact, phone, radius,
                email, phone, timezone, dayLight, dayLight, "", "", sn.UserID, true, 0);
            db.AddStation(sn.User.OrganizationId, name, landmarkId, typeId, number, description, sn.UserID, contact, phone, fax, address, email);
        }
        else
        {
            patchLandPointMgr.PatchVlfLandmarkPointSet_Update(sn.User.OrganizationId, hdCurName.Value, name, lat, lon, description, contact, phone, radius,
                email, phone, timezone, dayLight, dayLight, "", "", sn.UserID, true);
            db.UpdateStation(int.Parse(hdStationId.Value), name, landmarkId, typeId, number, description, sn.UserID, contact, phone, fax, address, email);
        }
        if (typeId == 1)
            Response.Write("<script language=javascript>parent.FireClick('bt_SaveStation');</script>");
        else
            Response.Write("<script language=javascript>parent.FireClick('bt_SaveDepot');</script>");

    }

    protected void cmdCancel_Click(object sender, System.EventArgs e)
    {
        Response.Write("<script language=javascript>parent.FireClick('bt_EditCancel');</script>");
    }

    protected void cmdDelete_Click(object sender, System.EventArgs e)
    {
        int stationId = int.Parse(hdStationId.Value);
        int landmarkid = int.Parse(hdLandmarkId.Value);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (db.DeleteStation(stationId) == 0)
            return;
        try
        {
            PatchLandmarkPointSet patchLandPointMgr = new PatchLandmarkPointSet(SentinelFMConnection);
            patchLandPointMgr.PatchVlfLandmarkPointSet_Delete(sn.User.OrganizationId, hdCurName.Value);
            GeomarkServiceClient _lc = new GeomarkServiceClient("httpbasic");
            _lc.DeleteFromSpatialTable(landmarkid);
        }
        catch
        {
        }

        int typeId = int.Parse(hdType.Value);
        if (typeId == 1)
            Response.Write("<script language=javascript>parent.FireClick('bt_DeleteStation');</script>");
        else
            Response.Write("<script language=javascript>parent.FireClick('bt_DeleteDepot');</script>");
    }

    protected string ReportDB_ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        }
    }
    protected string SentinelFMConnection
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        }
    }
}