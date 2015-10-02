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

public partial class ScheduleAdherence_frmEditStation : SABasePage
{
    protected override void PageLoad()
    {
        if (!IsPostBack)
        {
            hdAction.Value = Request["Action"];
            Landmarks_Fill();
            if (hdAction.Value != "add")
            {
                hdStationId.Value = Request["stationId"];
                BindStationData();
            }
        }
    }

    public override SACategory PageCategory
    {
        get { return SACategory.Station; }
    }

    private DataSet GetLandmarks()
    {
/*        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        return db.GetLandmarksByOrganizationId(sn.SuperOrganizationId);
*/
        DataSet dsLandmarks = new DataSet();
        try
        {
            string xml = "";
            DBOrganization dbo = new DBOrganization();

            if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                if (objUtil.ErrCheck(dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                {
                    return null;
                }

            if (xml == "") return null;
            StringReader strrXML = new StringReader(xml);
            dsLandmarks.ReadXml(strrXML);
        }
        catch (NullReferenceException Ex)
        {
            RedirectToLogin();
        }
        catch (Exception Ex)
        {
        }
        return dsLandmarks;
    }

    private void Landmarks_Fill()
    {
        ddlLandmark.DataSource = GetLandmarks();
        ddlLandmark.DataBind();
    }

    private void BindStationData()
    {
        int stationId = int.Parse(hdStationId.Value);
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStation = db.GetStationById(stationId);
        if (dsStation == null || dsStation.Tables.Count == 0 || dsStation.Tables[0].Rows.Count == 0) return;
        DataRow row = dsStation.Tables[0].Rows[0];
        txtName.Text = row["Name"].ToString(); 
        ddlLandmark.SelectedValue = row["LandmarkId"].ToString();
        if (row["TypeId"] == DBNull.Value)
            ddlType.SelectedValue = "";
        else
            ddlType.SelectedValue = row["TypeId"].ToString();
        if (row["StationNumber"] == DBNull.Value)
            txtNumber.Text = "";
        else
            txtNumber.Text = row["StationNumber"].ToString();         
        if (row["ContractName"] == DBNull.Value)
            txtContact.Text = "";
        else
            txtContact.Text = row["ContractName"].ToString();
        if (row["PhoneNumber"] == DBNull.Value)
            txtPhone.Text = "";
        else
            txtPhone.Text = row["PhoneNumber"].ToString();
        if (row["FaxNumber"] == DBNull.Value)
            txtFax.Text = "";
        else
            txtFax.Text = row["FaxNumber"].ToString();
        if (row["Address"] == DBNull.Value)
            txtAddress.Text = "";
        else
            txtAddress.Text = row["Address"].ToString();
        if (row["EmailAddress"] == DBNull.Value)
            txtEmail.Text = "";
        else
            txtEmail.Text = row["EmailAddress"].ToString();
        if (row["Description"] == DBNull.Value)
            txtDescription.Text = "";
        else
            txtDescription.Text = row["Description"].ToString();
    }

    protected void cmdSave_Click(object sender, System.EventArgs e)
    {
        string name = txtName.Text;
        string number = txtNumber.Text;
        int typeId = int.Parse(ddlType.SelectedValue);
        int landmarkId = int.Parse(ddlLandmark.SelectedValue);
        string contact = txtContact.Text;
        string phone = txtPhone.Text;
        string fax = txtFax.Text;
        string email = txtEmail.Text;
        string address = txtAddress.Text;
        string description = txtDescription.Text;

        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (hdAction.Value == "add")
            db.AddStation(sn.User.OrganizationId, name, landmarkId, typeId, number, description, sn.UserID, contact, phone, fax, address, email);
        else
            db.UpdateStation(int.Parse(hdStationId.Value), name, landmarkId, typeId, number, description, sn.UserID, contact, phone, fax, address, email);

        Response.Redirect("frmStationList.aspx");
    }

    protected void cmdCancel_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("frmStationList.aspx");
    }
}