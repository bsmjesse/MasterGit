using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM.ServerDBFleet;
//using SentinelFM;

namespace SentinelFM
{
    public partial class DriverFinder_GetSurroundedVehicles : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            double lat = Convert.ToDouble(Request.QueryString["lat"] ?? "43.5183333333333");
            double lon = Convert.ToDouble(Request.QueryString["lon"] ?? "-79.8623133333333");
            int fleetId = Convert.ToInt32(Request.QueryString["fleetId"] ?? "0");
            int skillId = Convert.ToInt32(Request.QueryString["skillId"] ?? "0");
            int vehicleTypeId = Convert.ToInt32(Request.QueryString["vehicleTypeId"] ?? "-1");
            DBFleet fleet = new DBFleet();
            string myXmlString = null;
            int result = fleet.GetVehiclesLastKnownPositionInfoWithDriversInfoNearestToLatLon(sn.UserID, sn.SecId, 480, fleetId, 0, lat, lon, skillId, vehicleTypeId,
                                                                  ref myXmlString);

            Response.ContentType = "application/xml; charset=utf-8";
            Response.Write(myXmlString);
            Response.End();
        }
    }
}