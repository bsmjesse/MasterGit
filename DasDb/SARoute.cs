using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class SARoute : TblOneIntPrimaryKey
    {
 		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
        public SARoute(SQLExecuter sqlExec)
            : base("saRoute", sqlExec)
		{
		}

        private const string AddRoute_SQL = "INSERT INTO {0}(ScheduleId,Name,VehicleId,RSCDepartureTime_sec,RSCArrivalTime_sec,LastEditedDatetime,LastEditedUserId,Description) VALUES (@ScheduleId,@Name,@VehicleId,@RSCDepartureTime_sec,@RSCArrivalTime_sec,@LastEditedDatetime,@LastEditedUserId,@Description)";
        private const string GetRouteId_SQL = "Select RouteId from {0} Where ScheduleId=@ScheduleId and Name=@Name and LastEditedDatetime=@LastEditedDatetime";
        /// <summary>
        /// Add new ScheduleGroup
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddRoute(int scheduleId, string name, int? vehicleId, int? departure_sec, int? arrival_sec, 
            DateTime editDatetime, int userId, string description)
        {
            // 1. Prepares SQL statement
            try
            {
                DateTime new_editDatetime = DateTime.Parse(editDatetime.ToString("yyyy-MM-dd hh:mm:ss"));
                // Set SQL command
                string sql = string.Format(AddRoute_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ScheduleId", SqlDbType.Int, scheduleId);
                sqlExec.AddCommandParam("@Name", SqlDbType.Char, name);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, vehicleId);
                sqlExec.AddCommandParam("@RSCDepartureTime_sec", SqlDbType.Int, departure_sec);
                sqlExec.AddCommandParam("@RSCArrivalTime_sec", SqlDbType.Int, arrival_sec);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, new_editDatetime);
                if (userId <= 0)
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, userId);
                if (string.IsNullOrEmpty(description) || "null".Equals(description))
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, null);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);

                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);

                //Retrieve GroupId
                sql = string.Format(GetRouteId_SQL, tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ScheduleId", SqlDbType.Int, scheduleId);
                sqlExec.AddCommandParam("@Name", SqlDbType.Char, name);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, new_editDatetime);
                DataSet ds = sqlExec.SQLExecuteDataset(sql);
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Route to ScheduleId '" + scheduleId +
                   " Name=" + name + ".";
                Util.ProcessDbException(prefixMsg, objException);
                return -1;
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Route to ScheduleId '" + scheduleId +
                   " Name=" + name + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateRoute_SQL = "Update {0} set Name=@Name,VehicleId=@VehicleId,RSCDepartureTime_sec=@RSCDepartureTime_sec,RSCArrivalTime_sec=@RSCArrivalTime_sec,Description=@Description,LastEditedDatetime=@LastEditedDatetime,LastEditedUserId=@LastEditedUserId where RouteId = @RouteId";
        public void UpdateRoute(int routeId, string name, int? vehicleId, int? departure_sec, int? arrival_sec,
            DateTime editDatetime, int userId, string description)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateRoute_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@RouteId", SqlDbType.Int, routeId);
                sqlExec.AddCommandParam("@Name", SqlDbType.Char, name);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, vehicleId);
                sqlExec.AddCommandParam("@RSCDepartureTime_sec", SqlDbType.Int, departure_sec);
                sqlExec.AddCommandParam("@RSCArrivalTime_sec", SqlDbType.Int, arrival_sec);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, editDatetime);
                if (userId <= 0)
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, userId);
                if (string.IsNullOrEmpty(description) || "null".Equals(description))
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, null);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update Route to RouteId '" + routeId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update Route to RouteId '" + routeId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string GetRoutesByScheduleId_SQL = "select * from {0} where ScheduleId = @ScheduleId";
        /// <summary>
        /// Retrieves ReasonCodes by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetRoutesByScheduleId(int scheduleId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetRoutesByScheduleId_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@ScheduleId", SqlDbType.Int, scheduleId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by scheduleId=" + scheduleId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by scheduleId=" + scheduleId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        private const string GetRouteById_SQL = "select * from {0} where RouteId = @RouteId";
        /// <summary>
        /// Retrieves Stations by id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetRouteById(int routeId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetRouteById_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@RouteId", SqlDbType.Int, routeId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by routeId=" + routeId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by routeId=" + routeId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int DeleteRouteById(int routeId)
        {
            return DeleteRowsByIntField("RouteId", routeId, "Schedule Adherence ScheduleGroup");
        }

        public int DeleteRouteByScheduleId(int scheduleId)
        {
            return DeleteRowsByIntField("ScheduleId", scheduleId, "Schedule Adherence ScheduleGroup");
        }
    }
}
