using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;
using System.Text;
using System.Collections.Generic;

namespace VLF.DAS.Logic
{
    public class BatteryTrending : VLF.DAS.Das
    {
        DB.BatteryTrending batteryTrending = null;

        #region General Interfaces

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public BatteryTrending(string connectionString)
            : base(connectionString)
        {
            batteryTrending = new DB.BatteryTrending(sqlExec);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }

        #endregion

        public DataSet GetBatterySummaryByFleetId(int fleetId)
        {
            return batteryTrending.GetBatterySummaryByFleetId(fleetId);
        }

        public DataSet GetCurrentOrLastTripBatteryByFleet(int fleetId, string category, int userId)
        {
            return batteryTrending.GetCurrentOrLastTripBatteryByFleet(fleetId, category, userId);
        }

        // Changes for TimeZone Feature start

        public DataSet GetLastKnownBatteryByFleet_NewTZ(int fleetId, string category, int userId)
        {
            return batteryTrending.GetLastKnownBatteryByFleet_NewTZ(fleetId, category, userId);
        }

        // Changes for TimeZone Feature end

        public DataSet GetLastKnownBatteryByFleet(int fleetId, string category, int userId)
        {
            return batteryTrending.GetLastKnownBatteryByFleet(fleetId, category, userId);
        }

        // Changes for TimeZone Feature start

        public DataSet GetBatteryTrendingByVehicleId_NewTZ(int vehicleId, string dateTimeFrom, string dateTimeTo, int userId)
        {
            return batteryTrending.GetBatteryTrendingByVehicleId_NewTZ(vehicleId, dateTimeFrom, dateTimeTo, userId);
        }

        // Changes for TimeZone Feature end

        public DataSet GetBatteryTrendingByVehicleId(int vehicleId, string dateTimeFrom, string dateTimeTo, int userId)
        {
            return batteryTrending.GetBatteryTrendingByVehicleId(vehicleId, dateTimeFrom, dateTimeTo, userId);
        }

    }
}
