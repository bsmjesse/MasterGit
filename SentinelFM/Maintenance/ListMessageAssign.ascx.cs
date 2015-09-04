using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections;

public partial class Maintenance_ListMessageAssign : System.Web.UI.UserControl
{
    public string assignStr1 = "This action will assign service for (1) box, continue?";
    public string assignStr2 = "This action will assign service for (n) boxes, continue?";
    public string deleteStr1 = "This action will delete service assignments for (1) box, continue?";
    public string deleteStr2 = "This action will delete service assignments for (n) boxes, continue?";
    public string emptyData = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("VehicleId");
        dt.Columns.Add("BoxId");
        dt.Columns.Add("Description");
        JavaScriptSerializer js = new JavaScriptSerializer();
        js.MaxJsonLength = int.MaxValue;
        
        ArrayList vehicles = new ArrayList();
        Dictionary<string, string> vehicle = new Dictionary<string, string>();
        vehicle.Add("BoxId", "");
        vehicle.Add("Description", "");
        vehicle.Add("dtStart", "");
        vehicles.Add(vehicle);
        emptyData = js.Serialize(vehicles);
        gdVehicle.DataSource = dt;
        gdVehicle.DataBind();
    }
}