using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections;
using Telerik.Web.UI;
public partial class Maintenance_ListMessageAssign_Dt : System.Web.UI.UserControl
{
    public string assignStr1 = "This action will assign service for (1) box, continue?";
    public string assignStr2 = "This action will assign service for (n) boxes, continue?";
    public string deleteStr1 = "This action will delete service assignments for (1) box, continue?";
    public string deleteStr2 = "This action will delete service assignments for (n) boxes, continue?";
    public string emptyData = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("VehicleId");
            dt.Columns.Add("BoxId");
            dt.Columns.Add("Description");
            gdVehicle_Dt.DataSource = dt;
            gdVehicle_Dt.DataBind();
        }
    }
    protected void gdVehicle_Dt_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        RadDatePicker dp = (RadDatePicker)e.Item.FindControl("calValue");
        if (dp != null) dp.SelectedDate = System.DateTime.Now.Date;
    }
}