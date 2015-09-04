using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using VLF.PATCH.Logic;
using System.Globalization;

namespace SentinelFM
{

    public partial class frmVehicleAvailabilityDashboard : System.Web.UI.Page
    {
        public string FleetName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

            using (VLF.DAS.Logic.Fleet dbFleet = new VLF.DAS.Logic.Fleet(sConnectionString))
            {
                int FleetId = 0;
                int.TryParse(Request["FleetId"], out FleetId);
                SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];

                DataSet dsResult = dbFleet.GetFleetInfoByFleetId(FleetId);
                DataTable oneDT = dsResult.Tables[0];
                if (oneDT.Rows.Count > 0)
                {
                    FleetName = oneDT.Rows[0]["FleetName"].ToString();
                }
            }
        }
    }

}
