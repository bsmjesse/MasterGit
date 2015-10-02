using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using VLF.DAS;
using VLF.DAS.DB;

namespace VLF.PATCH.DB
{
    public class PatchFleet : TblOneIntPrimaryKey
    {
        public PatchFleet(SQLExecuter sqlExec) : base("vlfFleet", sqlExec)
        {
            
        }

        public int AddUserToFleet(int fleetId, int userId)
        {
            int rowsaffected = 0;
            try
            {
                //string sql = " IF NOT EXISTS (SELECT * FROM vlfFleetUsers WHERE FleetId=" + fleetId.ToString() + " AND UserId=" + userId.ToString() + ") INSERT INTO vlfFleetUsers (FleetId, UserId) VALUES(" + fleetId.ToString() + ", " + userId.ToString() + ") ";
                //rowsaffected = sqlExec.SQLExecuteNonQuery(sql);
                string sql = "sp_AddUserToFleet";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

                rowsaffected = sqlExec.SPExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add user to fleet by UserId=" + userId.ToString() + ", FleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add user to fleet by UserId=" + userId.ToString() + ", FleetId=" + fleetId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsaffected;

        }

        public int DeleteUserFromFleet(int fleetId, int userId)
        {
            int rowsaffected = 0;
            try
            {
                string sql = " DELETE FROM vlfFleetUsers WHERE FleetId=" + fleetId.ToString() + " AND UserId=" + userId.ToString();
                rowsaffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete user from fleet by UserId=" + userId.ToString() + ", FleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete user from fleet by UserId=" + userId.ToString() + ", FleetId=" + fleetId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsaffected;

        }

        public int AddVehicleToFleet(int fleetId, Int64 vehicleId)
        {
            int rowsaffected = 0;
            try
            {
                string sql = " IF NOT EXISTS (SELECT * FROM vlfFleetVehicles WHERE FleetId=" + fleetId.ToString() + " AND VehicleId=" + vehicleId.ToString() + ") INSERT INTO vlfFleetVehicles (FleetId, VehicleId) VALUES(" + fleetId.ToString() + ", " + vehicleId.ToString() + ") ";
                rowsaffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add vehicle to fleet by VehicleId=" + vehicleId.ToString() + ", FleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add vehicle to fleet by VehicleId=" + vehicleId.ToString() + ", FleetId=" + fleetId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsaffected;

        }

        public int DeleteVehicleFromFleet(int fleetId, int vehicleId)
        {
            int rowsaffected = 0;
            try
            {
                string sql = " DELETE FROM vlfFleetVehicles WHERE FleetId=" + fleetId.ToString() + " AND VehicleId=" + vehicleId.ToString();
                rowsaffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete vehicle from fleet by VehicleId=" + vehicleId.ToString() + ", FleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete vehicle from fleet by VehicleId=" + vehicleId.ToString() + ", FleetId=" + fleetId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsaffected;

        }

        public bool ValidateUserAssignedToFleet(int organizationId, int userId, int fleetId)
        {
            string sql = "sp_GetUserParentFleetAssignment";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            DataSet dt = sqlExec.SPExecuteDataset(sql);
            return dt.Tables[0].Rows.Count > 0;
        }

        public DataSet GetUsersInfoByFleetId(int fleetId)
        {
            string sql = "sp_GetUsersInfoByFleetId";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);            
        }

        public DataSet GetVehiclesInfoByFleetIdByPage(int fleetId, int pageSize, int page)
        {
            /*string sql = "GetVehiclesInfoByFleetIdByPage";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@PageSize", SqlDbType.Int, pageSize);
            sqlExec.AddCommandParam("@Page", SqlDbType.Int, page);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);*/
            return GetVehiclesInfoByFleetIdByFilterByPage(fleetId, pageSize, page, "");
        }

        public DataSet GetVehiclesInfoByFleetIdByFilterByPage(int fleetId, int pageSize, int page, string filter)
        {
            string sql = "GetVehiclesInfoByFleetIdByPage";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@PageSize", SqlDbType.Int, pageSize);
            sqlExec.AddCommandParam("@Page", SqlDbType.Int, page);
            sqlExec.AddCommandParam("@Filter", SqlDbType.VarChar, filter);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            return sqlExec.SPExecuteDataset(sql);
        }

        public int GetVehiclesInfoTotalNumberByFleetId(int fleetId)
        {
            /*int result = 0;
            string sql = "GetVehiclesInfoTotalNumberByFleetId";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
            {
                int.TryParse(dt.Tables[0].Rows[0][0].ToString(), out result);
            }
            return result;*/
            return GetVehiclesInfoTotalNumberByFileterFleetId(fleetId, "");
        }

        public int GetVehiclesInfoTotalNumberByFileterFleetId(int fleetId, string filter)
        {
            int result = 0;
            string sql = "GetVehiclesInfoTotalNumberByFleetId";
            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@Filter", SqlDbType.VarChar, filter);

            if (sqlExec.RequiredTransaction())
            {
                sqlExec.AttachToTransaction(sql);
            }
            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
            {
                int.TryParse(dt.Tables[0].Rows[0][0].ToString(), out result);
            }
            return result;
        }
    }
}
