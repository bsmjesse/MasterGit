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
    public class SASchedule : TblOneIntPrimaryKey
    {
        public SASchedule(SQLExecuter sqlExec)
            : base("saSchedule", sqlExec)
		{
		}

        private const string AddSchedule_SQL = "INSERT INTO {0}(GroupId,ScheduleBeginDate,LastEditedDatetime,LastEditedUserId) VALUES (@GroupId,@ScheduleBeginDate,@LastEditedDatetime,@LastEditedUserId)";
        /// <summary>
        /// Add new ScheduleGroup
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddSchedule(int groupId, DateTime scheduleDate, DateTime editDatetime, int userId)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(AddSchedule_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@GroupId", SqlDbType.Int, groupId);
                sqlExec.AddCommandParam("@ScheduleBeginDate", SqlDbType.DateTime, scheduleDate);
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
                string prefixMsg = "Unable to add new ScheduleGroup to groupId '" + groupId +
                   " ScheduleDate=" + scheduleDate + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new ScheduleGroup to groupId '" + groupId +
                   " ScheduleDate=" + scheduleDate + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string GetSchedulesByGroupId_SQL = "select * from {0} where GroupId = @GroupId";
        /// <summary>
        /// Retrieves ReasonCodes by GroupId 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSchedulesByGroupId(int groupId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetSchedulesByGroupId_SQL, tableName);
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

        public int DeleteSchedleByGroupId(int groupId)
        {
            return DeleteRowsByIntField("GroupId", groupId, "Schedule Adherence ScheduleGroup");
        }
    }
}
