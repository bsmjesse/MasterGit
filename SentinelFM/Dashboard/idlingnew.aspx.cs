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
    public string strDefaultFleetName = "All Vehicles";
    string strFilterExp = "FleetId='{0}'";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            //DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'"); // Salman Mantis #6913 (July 11, 2014)
            DataRow[] drArr = dsFleets.Tables[0].Select(string.Format(strFilterExp, sn.User.DefaultFleet));
            if (drArr.Length > 0)
            {
                strDefaultFleet = drArr[0]["FleetId"].ToString();
                strDefaultFleetName = drArr[0]["FleetName"].ToString();
            }
            else
            {
                strDefaultFleet = dsFleets.Tables[0].Rows[0]["FleetId"].ToString();
                strDefaultFleetName = dsFleets.Tables[0].Rows[0]["FleetName"].ToString() + " ";
            }
        }
    }
}