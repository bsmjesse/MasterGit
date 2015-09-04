using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class BatteryTrending : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public BatteryTrending(SQLExecuter sqlExec)
            : base("vlfBatteryTrending", sqlExec)
        {
        }

        public DataSet GetBatterySummaryByFleetId(int fleetId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetBatterySummaryByFleetId->Unable to Get Battery Summary By FleetId.";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);

                // SQL statement
                string sql = "[sp_GetBatterySummaryByFleetId]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
                resultSet.DataSetName = "FleetBattery";
                resultSet.Tables[0].TableName = "BatterySummaryInfo";
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }

        public DataSet GetCurrentOrLastTripBatteryByFleet(int fleetId, string category, int userId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetCurrentOrLastTripBatteryByFleet->Unable to Get Battery List By FleetId.";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@category", category);
                sqlParams[2] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "[sp_GetCurrentOrLastTripBatteryByFleet]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
                resultSet.DataSetName = "Fleet";
                resultSet.Tables[0].TableName = "VehiclesBatteryInfo";
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }


        // Changes for TimeZone Feature start

        public DataSet GetLastKnownBatteryByFleet_NewTZ(int fleetId, string category, int userId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetCurrentOrLastTripBatteryByFleet->Unable to Get Battery List By FleetId.";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@category", category);
                sqlParams[2] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "[sp_GetLastKnownBatteryByFleet_NewTimeZone]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
                resultSet.DataSetName = "Fleet";
                resultSet.Tables[0].TableName = "VehiclesBatteryInfo";
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }

        // Changes for TimeZone Feature end

        public DataSet GetLastKnownBatteryByFleet(int fleetId, string category, int userId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetCurrentOrLastTripBatteryByFleet->Unable to Get Battery List By FleetId.";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@category", category);
                sqlParams[2] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "[sp_GetLastKnownBatteryByFleet]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
                resultSet.DataSetName = "Fleet";
                resultSet.Tables[0].TableName = "VehiclesBatteryInfo";
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }

        // Changes for TimeZone Feature start

        public DataSet GetBatteryTrendingByVehicleId_NewTZ(int vehicleId, string dateTimeFrom, string dateTimeTo, int userId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetBatteryTrendingByVehicleId_NewTZ->Unable to Get Battery Trending By vehicleId.";
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
                sqlParams[1] = new SqlParameter("@dateTimeFrom", dateTimeFrom);
                sqlParams[2] = new SqlParameter("@dateTimeTo", dateTimeTo);
                sqlParams[3] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "[sp_GetBatteryTrendingByVehicleId_NewTimeZone]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
                resultSet.DataSetName = "Vehicle";
                resultSet.Tables[0].TableName = "BatteryTrendingInfo";
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }

        // Changes for TimeZone Feature end

        public DataSet GetBatteryTrendingByVehicleId(int vehicleId, string dateTimeFrom, string dateTimeTo, int userId)
        {
            DataSet resultSet = null;
            string prefixMsg = "";
            try
            {
                prefixMsg = "GetBatteryTrendingByVehicleId->Unable to Get Battery Trending By vehicleId.";
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@vehicleId", vehicleId);
                sqlParams[1] = new SqlParameter("@dateTimeFrom", dateTimeFrom);
                sqlParams[2] = new SqlParameter("@dateTimeTo", dateTimeTo);
                sqlParams[3] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "[sp_GetBatteryTrendingByVehicleId]";
                //Executes SQL statement
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);
                resultSet.DataSetName = "Vehicle";
                resultSet.Tables[0].TableName = "BatteryTrendingInfo";
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultSet;
        }
    }
}
