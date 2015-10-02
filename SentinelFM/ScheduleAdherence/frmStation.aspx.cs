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

public partial class ScheduleAdherence_frmStation : SABasePage
{
    protected override void PageLoad()
    {
        lbError.Text = "";
        if (!IsPostBack)
        {
            BindGVStationList();
        }
    }

    private void BindGVStationList()
    {
        BindGVStationList(null);

    }

    private void BindGVStationList(int? pageIndex)
    {
        DataView dv = GetAllStations();
        gvStation.DataSource = dv;
        if (pageIndex != null)
            gvStation.PageIndex = pageIndex.Value;
        gvStation.DataBind();
    }

    private DataView GetAllStations()
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        DataSet dsStations = db.GetStationsByOrganizationId(sn.User.OrganizationId);
        // Show DirectionName
        DataColumn dc = new DataColumn();
        dc.ColumnName = "LandMarkName";
        dc.DataType = Type.GetType("System.String");
        dc.DefaultValue = "";
        dsStations.Tables[0].Columns.Add(dc);
        dsStations.Tables[0].Columns.Add("TypeName", Type.GetType("System.String"));
        DataSet dsLandmarks = GetLandmarks();

        foreach (DataRow rowItem in dsStations.Tables[0].Rows)
        {
            long landmarkId = 0;
            if (rowItem["LandmarkId"] != System.DBNull.Value && long.TryParse(rowItem["LandmarkId"].ToString(), out landmarkId))
            {
                DataRow[] landmarks = dsLandmarks.Tables[0].Select("LandmarkId = " + landmarkId);
                if (landmarks == null || landmarks.Length == 0) continue;
                rowItem["LandMarkName"] = landmarks[0]["LandmarkName"];
            }
            int typeId = 0;
            if (rowItem["TypeId"] != System.DBNull.Value && int.TryParse(rowItem["TypeId"].ToString(), out typeId))
            {
                rowItem["TypeName"] = typeId == 1 ? "Station" : "Depot";
            }
        }
        dsStations.Tables[0].DefaultView.Sort = "TypeId desc, Name asc";
        return dsStations.Tables[0].DefaultView;
    }

    protected void cmdAddStation_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("frmEditStation.aspx?action=add");
    }

    private DataSet GetLandmarks()
    {
        if (sn.DsLandMarks != null) return sn.DsLandMarks;
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
            DataSet dsLandmarks = new DataSet();
            dsLandmarks.ReadXml(strrXML);

            sn.DsLandMarks = dsLandmarks;
        }
        catch (NullReferenceException Ex)
        {
            RedirectToLogin();
        }
        catch (Exception Ex)
        {
        }
        return sn.DsLandMarks;
    }

    public override SACategory PageCategory
    {
        get { return SACategory.Station; }
    }

    protected void gvStation_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int stationId = -1;
        int.TryParse(hdStation.Value, out stationId);
        switch (e.CommandName)
        {
            case "cmdEdit":
                Response.Redirect("frmEditStation.aspx?action=edit&stationId=" + stationId);
                break;
            case "cmdDelete":
                DeleteStation(stationId);
                break;
        }
    }

    private void DeleteStation(int stationId)
    {
        ScheduleAdherence db = new ScheduleAdherence(ReportDB_ConnectionString);
        if (db.DeleteStation(stationId) == 0)
            lbError.Text = "Delete fail.";
        BindGVStationList(gvStation.SelectedIndex);
    }

    protected void gvStation_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dataRow = e.Row.DataItem as DataRowView;
            if (dataRow == null) return;
            (e.Row.Cells[e.Row.Cells.Count - 2].Controls[0] as ImageButton).OnClientClick = "EditStation(" + dataRow["StationId"] + ")";
            (e.Row.Cells[e.Row.Cells.Count - 1].Controls[0] as ImageButton).OnClientClick = "if(!DeleteStation(" + dataRow["StationId"] + ", '" + dataRow["Name"] + "')) return false;";
        }
    }

    protected void gvStation_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        BindGVStationList(e.NewPageIndex);
    }
}