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
    public class SAScheduleGroup : TblOneIntPrimaryKey
    {
 		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
        public SAScheduleGroup(SQLExecuter sqlExec)
            : base("saScheduleGroup", sqlExec)
		{
		}

        private const string AddScheduleGroup_SQL = "INSERT INTO {0}(RSCStationId,ScheduleBeginDate,Duration,CreationDate,LastEditedDatetime,LastEditedUserId,Description) VALUES (@RSCStationId,@ScheduleBeginDate,@Duration,@CreationDate,@LastEditedDatetime,@LastEditedUserId,@Description)";
        private const string GetGroupId_SQL = "Select GroupId from {0} Where RSCStationId=@RSCStationId and ScheduleBeginDate=@ScheduleBeginDate and Duration=@Duration and CreationDate=@CreationDate and LastEditedDatetime=@LastEditedDatetime";
        /// <summary>
        /// Add new ScheduleGroup
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddScheduleGroup(int stationId, DateTime scheduleBeginDate, int duration, DateTime creationDate, DateTime editDatetime, int userId, string description)
        {
            // 1. Prepares SQL statement
            try
            {
                DateTime new_creationDate = DateTime.Parse(creationDate.ToString("yyyy-MM-dd hh:mm:ss"));
                DateTime new_editDatetime = DateTime.Parse(editDatetime.ToString("yyyy-MM-dd hh:mm:ss"));
                // Set SQL command
                string sql = string.Format(AddScheduleGroup_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@RSCStationId", SqlDbType.Int, stationId);
                sqlExec.AddCommandParam("@ScheduleBeginDate", SqlDbType.DateTime, scheduleBeginDate);
                sqlExec.AddCommandParam("@Duration", SqlDbType.Int, duration);
                sqlExec.AddCommandParam("@CreationDate", SqlDbType.DateTime, new_creationDate);
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
                sql = string.Format(GetGroupId_SQL, tableName);
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@RSCStationId", SqlDbType.Int, stationId);
                sqlExec.AddCommandParam("@ScheduleBeginDate", SqlDbType.DateTime, scheduleBeginDate);
                sqlExec.AddCommandParam("@Duration", SqlDbType.Int, duration);
                sqlExec.AddCommandParam("@CreationDate", SqlDbType.DateTime, new_creationDate);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, new_editDatetime);
                if (userId <= 0)
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, userId);
                DataSet ds = sqlExec.SQLExecuteDataset(sql);
                return int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new ScheduleGroup to RSCStationId '" + stationId +
                   " ScheduleBeginDate=" + scheduleBeginDate + ".";
                Util.ProcessDbException(prefixMsg, objException);
                return -1;
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new ScheduleGroup to RSCStationId '" + stationId +
                   " ScheduleBeginDate=" + scheduleBeginDate + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateScheduleGroup_SQL = "Update {0} set Description=@Description,LastEditedDatetime=@LastEditedDatetime,LastEditedUserId=@LastEditedUserId where GroupId = @GroupId";
        public void UpdateScheduleGroup(int groupId, string description, DateTime editDatetime, int userId)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateScheduleGroup_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@GroupId", SqlDbType.Int, groupId);
                if (string.IsNullOrEmpty(description) || "null".Equals(description))
                    sqlExec.AddCommandParam("@description", SqlDbType.Char, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@description", SqlDbType.Char, description);
                sqlExec.AddCommandParam("@LastEditedDatetime", SqlDbType.DateTime, editDatetime);
                if (userId <= 0)
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@LastEditedUserId", SqlDbType.Int, userId);
                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update Schedulegroup to GroupId '" + groupId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update Schedulegroup to GroupId '" + groupId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string GetScheduleGroupsByRSCStationId_SQL = "select * from {0} where RSCStationId = @RSCStationId";
        /// <summary>
        /// Retrieves ReasonCodes by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetScheduleGroupsByRSCStationId(int depotId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetScheduleGroupsByRSCStationId_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@RSCStationId", SqlDbType.Int, depotId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by depotId=" + depotId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by depotId=" + depotId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        private const string GetScheduleGroupById_SQL = "select * from {0} where GroupId = @GroupId";
        /// <summary>
        /// Retrieves Stations by id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetScheduleGroupById(int groupId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetScheduleGroupById_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@GroupId", SqlDbType.Int, groupId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by GroupId=" + groupId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by GroupId=" + groupId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int DeleteScheduleGroupById(int groupId)
        {
            return DeleteRowsByIntField("GroupId", groupId, "Schedule Adherence ScheduleGroup");
        }
    }
}
