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
    public class SASetting : TblOneIntPrimaryKey
    {
        public SASetting(SQLExecuter sqlExec)
            : base("saSetting", sqlExec)
		{
		}

        private const string AddSetting_SQL = "INSERT INTO {0}(OrganizationId,WindowBeforeSeconds,WindowsAfterSeconds,RSCDepartEarlySeconds,RSCDepartLateSeconds,RSCArrivalEarlySeconds,RSCArrivalLateSeconds,StopDepartEarlySeconds,StopDepartLateSeconds,StopArrivalEarlySeconds,StopArrivalLateSeconds) VALUES (@OrganizationId,@WindowBeforeSeconds,@WindowsAfterSeconds,@RSCDepartEarlySeconds,@RSCDepartLateSeconds,@RSCArrivalEarlySeconds,@RSCArrivalLateSeconds,@StopDepartEarlySeconds,@StopDepartLateSeconds,@StopArrivalEarlySeconds,@StopArrivalLateSeconds)";
        public void AddSetting(int organizationId, int winBefore, int winAfter,
            int rscDepartEarly, int rscDepartLate, int rscArrivalEarly, int rscArrivalLate,
            int stopDepartEarly, int stopDepartLate, int stopArrivalEarly, int stopArrivalLate)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(AddSetting_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@WindowBeforeSeconds", SqlDbType.Int, winBefore);
                sqlExec.AddCommandParam("@WindowsAfterSeconds", SqlDbType.Int, winAfter);
                sqlExec.AddCommandParam("@RSCDepartEarlySeconds", SqlDbType.Int, rscDepartEarly);
                sqlExec.AddCommandParam("@RSCDepartLateSeconds", SqlDbType.Int, rscDepartLate);
                sqlExec.AddCommandParam("@RSCArrivalEarlySeconds", SqlDbType.Int, rscArrivalEarly);
                sqlExec.AddCommandParam("@RSCArrivalLateSeconds", SqlDbType.Int, rscArrivalLate);
                sqlExec.AddCommandParam("@StopDepartEarlySeconds", SqlDbType.Int, stopDepartEarly);
                sqlExec.AddCommandParam("@StopDepartLateSeconds", SqlDbType.Int, stopDepartLate);
                sqlExec.AddCommandParam("@StopArrivalEarlySeconds", SqlDbType.Int, stopArrivalEarly);
                sqlExec.AddCommandParam("@StopArrivalLateSeconds", SqlDbType.Int, stopArrivalLate);
                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateSetting_SQL = "update {0} set WindowBeforeSeconds = @WindowBeforeSeconds ,WindowsAfterSeconds = @WindowsAfterSeconds, RSCDepartEarlySeconds = @RSCDepartEarlySeconds, RSCDepartLateSeconds = @RSCDepartLateSeconds,RSCArrivalEarlySeconds=@RSCArrivalEarlySeconds,RSCArrivalLateSeconds=@RSCArrivalLateSeconds,StopDepartEarlySeconds=@StopDepartEarlySeconds,StopDepartLateSeconds=@StopDepartLateSeconds,StopArrivalEarlySeconds=@StopArrivalEarlySeconds,StopArrivalLateSeconds=@StopArrivalLateSeconds where OrganizationId = @OrganizationId";
        public void UpdateSetting(int organizationId, int winBefore, int winAfter,
           int rscDepartEarly, int rscDepartLate, int rscArrivalEarly, int rscArrivalLate,
           int stopDepartEarly, int stopDepartLate, int stopArrivalEarly, int stopArrivalLate)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateSetting_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@WindowBeforeSeconds", SqlDbType.Int, winBefore);
                sqlExec.AddCommandParam("@WindowsAfterSeconds", SqlDbType.Int, winAfter);
                sqlExec.AddCommandParam("@RSCDepartEarlySeconds", SqlDbType.Int, rscDepartEarly);
                sqlExec.AddCommandParam("@RSCDepartLateSeconds", SqlDbType.Int, rscDepartLate);
                sqlExec.AddCommandParam("@RSCArrivalEarlySeconds", SqlDbType.Int, rscArrivalEarly);
                sqlExec.AddCommandParam("@RSCArrivalLateSeconds", SqlDbType.Int, rscArrivalLate);
                sqlExec.AddCommandParam("@StopDepartEarlySeconds", SqlDbType.Int, stopDepartEarly);
                sqlExec.AddCommandParam("@StopDepartLateSeconds", SqlDbType.Int, stopDepartLate);
                sqlExec.AddCommandParam("@StopArrivalEarlySeconds", SqlDbType.Int, stopArrivalEarly);
                sqlExec.AddCommandParam("@StopArrivalLateSeconds", SqlDbType.Int, stopArrivalLate);
                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        private const string UpdateFormat_SQL = "update {0} set ImportFormat = @ImportFormat where OrganizationId = @OrganizationId";
        public void UpdateFileFormat(int organizationId, string format)
        {
            // 1. Prepares SQL statement
            try
            {
                // Set SQL command
                string sql = string.Format(UpdateFormat_SQL, tableName);
                // Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@ImportFormat", SqlDbType.NVarChar, format);
                //Executes SQL statement
                sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new Station to OrganizationId '" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }
    }
}
