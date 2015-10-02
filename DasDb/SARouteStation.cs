using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class SARouteStation: TblOneIntPrimaryKey
    {
 		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
        public SARouteStation(SQLExecuter sqlExec)
            : base("saRouteStation", sqlExec)
		{
		}

        private const string AddStation_SQL = "INSERT INTO {0}(RouteId,StationId,DeliveryTypeId,DepartureSchedule_sec,ArrivalSchedule_sec,LastEditedDatetime,LastEditedUserId,Description) VALUES (@RouteId,@StationId,@DeliveryTypeId,@DepartureSchedule_sec,@ArrivalSchedule_sec,@LastEditedDatetime,@LastEditedUserId,@Description)";
        /// <summary>
        /// Add new ScheduleGroup
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddStation(int routeId, int stationId, int deliveryTypeId, int departure_sec, int arrival_sec, 
            DateTime editDatetime, int userId, string description)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(AddStation_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@RouteId", SqlDbType.Int, routeId);
                sqlExec.AddCommandParam("@StationId", SqlDbType.Int, stationId);
                sqlExec.AddCommandParam("@DeliveryTypeId", SqlDbType.Int, deliveryTypeId);
                sqlExec.AddCommandParam("@DepartureSchedule_sec", SqlDbType.Int, departure_sec);
                sqlExec.AddCommandParam("@ArrivalSchedule_sec", SqlDbType.Int, arrival_sec);
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
                string prefixMsg = "Unable to add new RouteStation '" + routeId + ".";
                Util.ProcessDbException(prefixMsg, objException);
                return;
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new RouteStation '" + routeId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateStation_SQL = "Update {0} set StationId=@StationId,DeliveryTypeId=@DeliveryTypeId,DepartureSchedule_sec=@DepartureSchedule_sec,ArrivalSchedule_sec=@ArrivalSchedule_sec,Description=@Description,LastEditedDatetime=@LastEditedDatetime,LastEditedUserId=@LastEditedUserId where RouteStationId = @RouteStationId";
        public void UpdateStation(int routeStationId, int stationId, int deliveryTypeId, int departure_sec, int arrival_sec,
            DateTime editDatetime, int userId, string description)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateStation_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@RouteStationId", SqlDbType.Int, routeStationId);
                sqlExec.AddCommandParam("@StationId", SqlDbType.Int, stationId);
                sqlExec.AddCommandParam("@DeliveryTypeId", SqlDbType.Int, deliveryTypeId);
                sqlExec.AddCommandParam("@DepartureSchedule_sec", SqlDbType.Int, departure_sec);
                sqlExec.AddCommandParam("@ArrivalSchedule_sec", SqlDbType.Int, arrival_sec);
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
                string prefixMsg = "Unable to update RouteStation '" + routeStationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update RouteStation '" + routeStationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string GetRouteStationsByRouteId_SQL = "select * from {0} where RouteId = @RouteId";
        /// <summary>
        /// Retrieves ReasonCodes by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetRouteStationsByRouteId(int routeId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetRouteStationsByRouteId_SQL, tableName);
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

        private const string GetRouteStationById_SQL = "select * from {0} where RouteId = @RouteId";
        /// <summary>
        /// Retrieves Stations by id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetRouteStationById(int routeStationId)
        {
            return GetRowsByIntField("RouteStationId", routeStationId, "Schedule Adherence ScheduleGroup");
        }

        public int DeleteRouteStationById(int routeStationId)
        {
            return DeleteRowsByIntField("RouteStationId", routeStationId, "Schedule Adherence ScheduleGroup");
        }
        public int DeleteRouteStationsByRouteId(int routeId)
        {
            return DeleteRowsByIntField("RouteId", routeId, "Schedule Adherence ScheduleGroup");
        }

        private const string DeleteRouteStationsByScheduleId_SQL = "delete {0} from {0} s join saRoute r on r.routeid = s.routeid where r.scheduleid = {1}";
        public int DeleteRouteStationsByScheduleId(int scheduleId)
        {
            string sql = string.Format(DeleteRouteStationsByScheduleId_SQL, tableName, scheduleId);
            return DeleteRowsBySql(sql, "", "");
        }
    }
}
