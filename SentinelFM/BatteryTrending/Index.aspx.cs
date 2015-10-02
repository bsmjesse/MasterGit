using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class BatteryTrending_Index : SentinelFMBasePage
    {
        public int FleetId = 0;
        public string FleetName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["fleetId"] != null && Request["fleetId"].Trim() != string.Empty)
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }
            if (FleetId > 0)
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                FleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, FleetId);
            }
        }
    }
}