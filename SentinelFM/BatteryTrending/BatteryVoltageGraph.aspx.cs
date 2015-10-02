using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class BatteryTrending_BatteryVoltageGraph : SentinelFMBasePage
    {
        public int FleetId = 0;
        public int VehicleId = 0;
        public string VehicleDescription;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["fleetId"] != null && Request["fleetId"].Trim() != string.Empty)
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            if (Request["vehicleId"] != null && Request["vehicleId"].Trim() != string.Empty)
            {
                int.TryParse(Request["vehicleId"], out VehicleId);
            }

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            
            if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                {
                    return;
                }

            if (xml == "")
            {
                return;
            }

            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(xml));
            VehicleDescription = Convert.ToString(ds.Tables[0].Rows[0].ItemArray[11]);
        }
    }
}