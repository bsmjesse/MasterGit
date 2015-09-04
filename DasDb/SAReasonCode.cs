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
    public class SAReasonCode : TblOneIntPrimaryKey
    {
 		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
        public SAReasonCode(SQLExecuter sqlExec)
            : base("saReasonCode", sqlExec)
		{
		}

        private const string AddReasonCode_SQL = "INSERT INTO {0}(OrganizationId,ReasonCode,Description,LastEditedDatetime,LastEditedUserId) VALUES (@OrganizationId,@ReasonCode,@Description,@LastEditedDatetime,@LastEditedUserId)";
        /// <summary>
        /// Add new Error Code.
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddReasonCode(int organizationId, string reasonCode, string description, DateTime editDatetime, int userId)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(AddReasonCode_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@ReasonCode", SqlDbType.Char, reasonCode);
                sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);
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
                string prefixMsg = "Unable to add new ReasonCode to OrganizationId '" + organizationId +
                   " ReasonCode=" + reasonCode + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new ReasonCode to OrganizationId '" + organizationId +
                   " ReasonCode=" + reasonCode + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateReasonCode_SQL = "Update {0} set ReasonCode = @ReasonCode, Description=@Description,LastEditedDatetime=@LastEditedDatetime,LastEditedUserId=@LastEditedUserId where ReasonCodeId = @ReasonCodeId";
        public void UpdateReasonCode(int ReasonCodeId, string reasonCode, string description, DateTime editDatetime, int userId)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateReasonCode_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ReasonCodeId", SqlDbType.Int, ReasonCodeId);
                sqlExec.AddCommandParam("@ReasonCode", SqlDbType.Char, reasonCode);
                if (string.IsNullOrEmpty(description))
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
                string prefixMsg = "Unable to update ReasonCode to ReasonCodeId '" + ReasonCodeId +
                   " ReasonCode=" + reasonCode + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update ReasonCode to ReasonCodeId '" + ReasonCodeId +
                   " ReasonCode=" + reasonCode + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string GetReasonCodesByOrganizationId_SQL = "select ReasonCodeId, OrganizationId, ReasonCode, Description, LastEditedDatetime, LastEditedUserId from {0} where OrganizationId = @OrganizationId";
        /// <summary>
        /// Retrieves ReasonCodes by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetReasonCodesByOrganizationId(int organizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetReasonCodesByOrganizationId_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        private const string GetReasonCodeById_SQL = "select ReasonCodeId, OrganizationId, ReasonCode, Description, LastEditedDatetime, LastEditedUserId from {0} where ReasonCodeId = @ReasonCodeId";
        /// <summary>
        /// Retrieves Stations by organization id 
        /// </summary>
        /// <returns>
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetReasonCodeById(int reasonCodeId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = string.Format(GetReasonCodeById_SQL, tableName);
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@ReasonCodeId", SqlDbType.Int, reasonCodeId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by ReasonCodeId=" + reasonCodeId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by ReasonCodeId=" + reasonCodeId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int DeleteReasonCodeById(int ReasonCodeId)
        {
            return DeleteRowsByIntField("ReasonCodeId", ReasonCodeId, "Schedule Adherence ReasonCode");
        }
    }
}
