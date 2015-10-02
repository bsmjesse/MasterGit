using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Maintenance_ListMessage : System.Web.UI.UserControl
{
    public string affectGroupStr1 = "This action will affect (1) group, continue?";
    public string affectGroupStr2 = "This action will affect (n) groups, continue?";

    public string overwriteStr1 = "This action will overwrite the service assignment values for (1) box, continue?";
    public string overwriteStr2 = "This action will overwrite the service assignment values for (n) boxes, continue?";
    public string assignStr1 = "This action will delete the service assignments for (1) box, continue?";
    public string assignStr2 = "This action will delete the service assignments for (n) boxes, continue?";
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("BoxId");
        dt.Columns.Add("Description");
        gdVehicle.DataSource = dt;
        gdVehicle.DataBind();
    }
}