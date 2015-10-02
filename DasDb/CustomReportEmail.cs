using System;
using System.Collections.Generic;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class CustomReportEmail: TblGenInterfaces
    {
        public CustomReportEmail(SQLExecuter sqlExec)
            : base("CustomReportEmail", sqlExec)
        {
        }
        public int CustomReportEmail_Add(string Email, int OrganizationId, string FleetIds, int UserId, int UID)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Email", SqlDbType.NVarChar, Email);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@FleetIds", SqlDbType.VarChar, FleetIds, -1);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                sqlExec.AddCommandParam("@UID", SqlDbType.Int, UID);
                rowsAffected = sqlExec.SPExecuteNonQuery("[CustomReportEmail_Add]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add customReport Email. OrganizationId =" + OrganizationId + " Email=" + Email;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add customReport Email. OrganizationId =" + OrganizationId + " Email=" + Email;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        public int CustomReportEmail_Delete(long CustomReportEmailID, int OrganizationId, int UID)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@CustomReportEmailID", SqlDbType.BigInt, CustomReportEmailID);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UID", SqlDbType.Int, UID);

                rowsAffected = sqlExec.SPExecuteNonQuery("[CustomReportEmail_Delete]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete customReport Email. CustomReportEmailID =" + CustomReportEmailID + " OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete customReport Email. CustomReportEmailID =" + CustomReportEmailID + " OrganizationId=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        public int CustomReportEmail_Update(string Email, long CustomReportEmailID, int OrganizationId, string FleetIds, int UID)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@CustomReportEmailID", SqlDbType.BigInt, CustomReportEmailID);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@Email", SqlDbType.NVarChar, Email);
                sqlExec.AddCommandParam("@FleetIds", SqlDbType.VarChar, FleetIds, -1);
                sqlExec.AddCommandParam("@UID", SqlDbType.Int, UID);

                rowsAffected = sqlExec.SPExecuteNonQuery("[CustomReportEmail_Update]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update customReport Email. CustomReportEmailID =" + CustomReportEmailID + " OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update customReport Email. CustomReportEmailID =" + CustomReportEmailID + " OrganizationId=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        public int CustomReportEmailFleet_Add(long CustomReportEmailID, string FleetIds, int UID)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@CustomReportEmailID", SqlDbType.BigInt, CustomReportEmailID);
                sqlExec.AddCommandParam("@FleetIds", SqlDbType.VarChar, FleetIds, -1);
                sqlExec.AddCommandParam("@UID", SqlDbType.Int, UID);
                rowsAffected = sqlExec.SPExecuteNonQuery("[CustomReportEmailFleet_Add]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add CustomReportEmailFleet. CustomReportEmailID =" + CustomReportEmailID + " FleetIds=" + FleetIds;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add CustomReportEmailFleet. CustomReportEmailID =" + CustomReportEmailID + " FleetIds=" + FleetIds;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        public int CustomReportEmailMessage_Add(string CustomReportEmailMessage, int OrganizationId, int UID)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@CustomReportEmailMessage", SqlDbType.NVarChar, CustomReportEmailMessage);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UID", SqlDbType.Int, UID);
                rowsAffected = sqlExec.SPExecuteNonQuery("[CustomReportEmailMessage_Add]");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add CustomReportEmailMessaget. CustomReportEmailMessage =" + CustomReportEmailMessage;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add CustomReportEmailMessaget. CustomReportEmailMessage =" + CustomReportEmailMessage;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        public DataSet CustomReportEmail_Get(int OrganizationId, Boolean isMessage)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@isMessage", SqlDbType.Bit, isMessage);

                sqlDataSet = sqlExec.SPExecuteDataset("CustomReportEmail_Get");

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to GetOrganizationCustomReportEmail. OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to GetOrganizationCustomReportEmail. OrganizationId=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet CustomReportEmail_GetHGI_USER(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlDataSet = sqlExec.SPExecuteDataset("CustomReportEmail_GetHGI_USER");

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to CustomReportEmail_GetHGI_USER. OrganizationId=" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to CustomReportEmail_GetHGI_USER. OrganizationId=" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
