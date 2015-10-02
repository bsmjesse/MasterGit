using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace SentinelFM
{
    public partial class BatteryTrending_BatteryVehilceList : SentinelFMBasePage
    {
        public int FleetId = 0;
        public string VoltageThreshold = "red";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["fleetId"] != null && Request["fleetId"].Trim() != string.Empty)
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            if (Request["t"] != null && Request["t"].Trim() != string.Empty)
            {
                VoltageThreshold = Request["t"].Trim().ToLower();
            }
        }        
    }
}