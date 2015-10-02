using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class DashBoard_idlingnew : SentinelFMBasePage
{
    public string strDefaultFleet = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
            strDefaultFleet = drArr[0]["FleetId"].ToString();
        }
    }
}