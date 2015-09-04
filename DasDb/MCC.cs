using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to MCC table.
    /// </summary>
    public class MCC: TblGenInterfaces
    {
        public MCC(SQLExecuter sqlExec)
            : base("MCCGroup", sqlExec)
      {
      }

        /// <summary>
        /// Delete equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <returns></returns>
        public int DeleteMCCGroup(long MccId, int OrganizationId, int UserId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCGroup_Delete";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete MCCGroup. MccId:" + MccId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete MCCGroup. MccId:" + MccId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to Delete MCCGroup. MccId:" + MccId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCGroup does not exist.");
            }
            return rowsAffected;
        }

        /// <summary>
        /// UpdateMcc
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccName"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public int UpdateMCCGroup(int OrganizationId, string MccName, long MccId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCGroup_Update";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@MccName", SqlDbType.NVarChar, MccName);
                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update MCCGroup to OrganizationId:" + OrganizationId + " MCCId:" + MccId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update MCCGroup to OrganizationId:" + OrganizationId + " MCCId:" + MccId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update MCCGroup to OrganizationId:" + OrganizationId + " MCCId:" + MccId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCGroup does not exist.");
            }
            return rowsAffected;
        }


        /// <summary>
        /// AddMCC
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccName"></param>
        /// <returns></returns>
        public int AddMCCGroup(int OrganizationId, string MccName)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCGroup_Add";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@MccName", SqlDbType.NVarChar, MccName);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add MCCGroup to OrganizationId:" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add MCCGroup to OrganizationId:" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add MCCGroup to OrganizationId:" + OrganizationId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCGroup already exists.");
            }

            return rowsAffected;
        }

        /// <summary>
        /// GetOrganizationMCC
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationMCCGroup(int OrganizationId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "GetOrganizationMCCGroup";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MCCGroup by OrganizationId=" + OrganizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MCCGroup by OrganizationId=" + OrganizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        /// <summary>
        /// MCCMaintenanceAssigment_Add
        /// </summary>
        /// <param name="MccId"></param>
        /// <param name="MaintenanceIds"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int MCCMaintenanceAssigment_Add(long MccId, string MaintenanceIds, int OrganizationId, string vehicleIDs)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCMaintenanceAssigment_Add";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                sqlExec.AddCommandParam("@MaintenanceIds", SqlDbType.VarChar, MaintenanceIds, -1);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@vehicleIDs", SqlDbType.VarChar, vehicleIDs, -1);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to assign Maintenances to MCCId:" + MccId + " MaintenanceIds=" + MaintenanceIds;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to assign Maintenances to MCCId:" + MccId + " MaintenanceIds=" + MaintenanceIds;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// MccServiceAssigment_Delete
        /// </summary>
        /// <param name="MccId"></param>
        /// <param name="ServiceTypeIDs"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int MCCMaintenanceAssigment_Delete(long MccId, string MaintenanceIds, int OrganizationId, int UserID)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCMaintenanceAssigment_Delete";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                sqlExec.AddCommandParam("@MaintenanceIds", SqlDbType.VarChar, MaintenanceIds, -1);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, UserID);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete MCCMaintenanceAssigment MCCId:" + MccId + " MaintenanceIds=" + MaintenanceIds;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete MCCMaintenanceAssigment MCCId:" + MccId + " @MaintenanceIds=" + MaintenanceIds;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// GetMccServiceAssigments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceAssigment(int? OrganizationId, long? MccId, string operationTypeStr, int UserId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(operationTypeStr))
                {
                   operationTypeStr = "," + operationTypeStr;
                   operationTypeStr = operationTypeStr.Replace("OperationTypeID", "MCCMaintenances.OperationTypeID");
                }
                string sqlMaintenance = string.Format(
                           "Select MCCMaintenances.*, MCCNotificationType.Description as NotificationDescription, " +
                           "MaintenanceFrequency.FrequencyName, " +
                           "Case MCCMaintenances.OperationTypeID " +
                           "when 1 then CEILING(MCCMaintenances.interval*@unitOfMes) when 2 then MCCMaintenances.interval/60 else MCCMaintenances.interval end as IntervalDesc, " + 
                           "MCCNotificationType.Notification1, MCCNotificationType.Notification2, MCCNotificationType.Notification3 {0} " +
                           "from MCCMaintenances " +
                           "left join MCCNotificationType on MCCMaintenances.NotificationTypeID=MCCNotificationType.NotificationID and (MCCNotificationType.OrganizationId={1} or MCCNotificationType.OrganizationId = 0 ) " +
                           "left join TimespanConventionTypes on TimespanConventionTypes.TimespanId =  MCCMaintenances.TimespanId " + 
                           "left join MaintenanceFrequency on MaintenanceFrequency.FrequencyID = MCCMaintenances.FrequencyID " +
                           "where (MCCMaintenances.OrganizationId={1} ) and (MCCMaintenances.Active is null or MCCMaintenances.Active = 1) ", operationTypeStr, OrganizationId
                    );
                if (MccId.HasValue)
                {
                    sql = string.Format("DECLARE @unitOfMes float; " +
                               "select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={2} and PreferenceId=0; " + 
                               "Select a.MccId, b.MccName, c.* " +
                               "from MCCMaintenanceAssigment a  " +
                               "inner join MCCGroup b on a.MccId = b.MccId " +
                               "inner join ({0}) as c ON a.MaintenanceId = c.MaintenanceId " +
                               "where a.MccId = {1} and (a.Active is null or a.Active = 1) order by Description", sqlMaintenance,
                               MccId, UserId
                        );
                }
                else
                {
                    sql = string.Format("DECLARE @unitOfMes float; " +
                               "select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={2} and PreferenceId=0; " + 
                               "Select a.OrganizationId, a.MccId, a.MccName, c.*" +
                               "from MCCGroup  a  " +
                               "left join MCCMaintenanceAssigment b on a.MccId = b.MccId and " +
                               "(b.Active is null or b.Active = 1) and " + 
                               "b.MaintenanceId in (select MaintenanceId from MCCMaintenances where MCCMaintenances.OrganizationId={1} and (MCCMaintenances.Active is null or MCCMaintenances.Active = 1) ) " +
                               "left join ({0}) as c ON b.MaintenanceId = c.MaintenanceId " +
                               "where a.OrganizationId={1}  and (a.Active is null or a.Active = 1) order by MccName, Description", sqlMaintenance,
                               OrganizationId, UserId
                        );

                }
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MCCMaintenanceAssigment ";
                if (OrganizationId.HasValue)
                    prefixMsg = prefixMsg + "By OrganizationId=" + OrganizationId;
                if (MccId.HasValue)
                    prefixMsg = prefixMsg + "By MccId=" + MccId;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MCCMaintenanceAssigment ";
                if (OrganizationId.HasValue)
                    prefixMsg = prefixMsg + "By OrganizationId=" + OrganizationId;
                if (MccId.HasValue)
                    prefixMsg = prefixMsg + "By MccId=" + MccId;


                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// GetMccUnAssignedService
        /// </summary>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceUnAssigment(int OrganizationId, long MccId, string operationTypeStr, int UserId)
        {
            DataSet sqlDataSet = null;
            try
            {
                if (!string.IsNullOrEmpty(operationTypeStr))
                {
                    operationTypeStr = "," + operationTypeStr;
                    operationTypeStr = operationTypeStr.Replace("OperationTypeID", "a.OperationTypeID");
                }

                string sql = string.Empty;
                sql = string.Format("DECLARE @unitOfMes float; " +
                           "select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={3} and PreferenceId=0; " + 
                           "Select a.*,  MCCNotificationType.Description as NotificationDescription, " +
                           "MaintenanceFrequency.FrequencyName, " +
                           "Case a.OperationTypeID " +
                           "when 1 then CEILING(a.interval*@unitOfMes) when 2 then a.interval/60 else a.interval end as IntervalDesc, " + 
                           "MCCNotificationType.Notification1, MCCNotificationType.Notification2, MCCNotificationType.Notification3 {0} " +
                          "from MCCMaintenances a " +
                          "left join MCCNotificationType on a.NotificationTypeID=MCCNotificationType.NotificationID and (MCCNotificationType.OrganizationId={1} or MCCNotificationType.OrganizationId = 0 )" +
                          "left join TimespanConventionTypes on TimespanConventionTypes.TimespanId =  a.TimespanId " +
                          "left join MaintenanceFrequency on MaintenanceFrequency.FrequencyID = a.FrequencyID " +
                          "where (a.OrganizationId = {1} ) and (a.Active is null or a.Active = 1) " +
                          "and a.MaintenanceId not in ( Select MaintenanceId from MCCMaintenanceAssigment where MccId = {2} and (Active is null or Active = 1) ) order by a.Description",
                          operationTypeStr, OrganizationId,
                          MccId, UserId
                   );
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MCCMaintenanceUnAssigment By MccId=" + MccId + " OrganizationId=" + OrganizationId; 
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MCCMaintenanceUnAssigment By MccId=" + MccId + " OrganizationId=" + OrganizationId;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// DeleteMCCMaintenances
        /// </summary>
        /// <param name="MaintenanceId"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public int DeleteMCCMaintenances(long MaintenanceId, int OrganizationId, int UserId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCMaintenances_Delete";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MaintenanceId", SqlDbType.BigInt, MaintenanceId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete MCCMaintenances. MaintenanceId:" + MaintenanceId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete MCCMaintenances. MaintenanceId:" + MaintenanceId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to Delete MCCMaintenances. MaintenanceId:" + MaintenanceId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCMaintenances does not exist.");
            }
            return rowsAffected;
        }

        /// <summary>
        /// UpdateMCCMaintenances
        /// </summary>
        /// <param name="MaintenanceId"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="FrequencyID"></param>
        /// <param name="Interval"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int UpdateMCCMaintenances(int MaintenanceId, int OrganizationId,
            int OperationTypeID, int? NotificationTypeID, int FrequencyID, int Interval, 
            string Description, int? TimespanId, Boolean FixedInterval, int userID,
            string FixedServiceDate, Boolean FixedDate, int FixedDueDate)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCMaintenances_Update";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MaintenanceId", SqlDbType.Int, MaintenanceId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@OperationTypeID", SqlDbType.Int, OperationTypeID);
                sqlExec.AddCommandParam("@NotificationTypeID", SqlDbType.Int, NotificationTypeID);
                sqlExec.AddCommandParam("@FrequencyID", SqlDbType.Int, FrequencyID);
                sqlExec.AddCommandParam("@Interval", SqlDbType.Int, Interval);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, Description);
                sqlExec.AddCommandParam("@TimespanId", SqlDbType.Int, TimespanId);
                sqlExec.AddCommandParam("@FixedInterval", SqlDbType.Bit, FixedInterval);
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userID);

                sqlExec.AddCommandParam("@FixedServiceDate", SqlDbType.VarChar, FixedServiceDate);
                sqlExec.AddCommandParam("@FixedDate", SqlDbType.Bit, FixedDate);
                sqlExec.AddCommandParam("@FixedDueDate", SqlDbType.Int, FixedDueDate);

                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update MCCMaintenances to OrganizationId:" + OrganizationId + " MaintenanceId:" + MaintenanceId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update MCCMaintenances to OrganizationId:" + OrganizationId + " MaintenanceId:" + MaintenanceId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update MCCMaintenances to OrganizationId:" + OrganizationId + " MaintenanceId:" + MaintenanceId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCMaintenances does not exist.");
            }
            return rowsAffected;
        }


        /// <summary>
        /// AddMCCMaintenances
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="FrequencyID"></param>
        /// <param name="Interval"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int AddMCCMaintenances(int OrganizationId, int OperationTypeID, int? NotificationTypeID, 
            int FrequencyID, int Interval, string Description, int? TimespanId, 
            Boolean FixedInterval, int userID,
            string FixedServiceDate, Boolean FixedDate)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCMaintenances_Add";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@OperationTypeID", SqlDbType.Int, OperationTypeID);
                sqlExec.AddCommandParam("@NotificationTypeID", SqlDbType.Int, NotificationTypeID);
                sqlExec.AddCommandParam("@FrequencyID", SqlDbType.Int, FrequencyID);
                sqlExec.AddCommandParam("@Interval", SqlDbType.Int, Interval);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, Description);
                sqlExec.AddCommandParam("@TimespanId", SqlDbType.Int, TimespanId);
                sqlExec.AddCommandParam("@FixedInterval", SqlDbType.Bit, FixedInterval);
                sqlExec.AddCommandParam("@userID", SqlDbType.Int, userID);

                sqlExec.AddCommandParam("@FixedServiceDate", SqlDbType.VarChar, FixedServiceDate);
                sqlExec.AddCommandParam("@FixedDate", SqlDbType.Bit, FixedDate);

                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add MCCMaintenances to OrganizationId:" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add MCCMaintenances to OrganizationId:" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add MCCMaintenances to OrganizationId:" + OrganizationId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCMaintenances already exists.");
            }

            return rowsAffected;
        }

        /// <summary>
        /// GetMCCMaintenances
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="operationTypeStr"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenances(int OrganizationId, string operationTypeStr, int userID)
        {
            if (!string.IsNullOrEmpty(operationTypeStr))
            {
                operationTypeStr = "," + operationTypeStr;
                operationTypeStr = operationTypeStr.Replace("OperationTypeID", "MCCMaintenances.OperationTypeID");
            }
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Empty;
                sql = string.Format("DECLARE @unitOfMes float; " +
	                       "select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={2} and PreferenceId=0; " +
                           "Select MCCMaintenances.*, MCCNotificationType.Description as NotificationDescription, dbo.ConvertMonthDay(MCCMaintenances.FixedServiceDate) as FixedServiceDate_1, " +
                           "MaintenanceFrequency.FrequencyName, " + 
                           "Case MCCMaintenances.OperationTypeID " +
                           "when 1 then CEILING(MCCMaintenances.interval*@unitOfMes) when 2 then MCCMaintenances.interval/60 else MCCMaintenances.interval end as IntervalDesc, " + 
                           "MCCNotificationType.Notification1, MCCNotificationType.Notification2, MCCNotificationType.Notification3 {0} " +
                           "from MCCMaintenances " +
                           "left join MCCNotificationType on MCCMaintenances.NotificationTypeID=MCCNotificationType.NotificationID and (MCCNotificationType.OrganizationId={1} or MCCNotificationType.OrganizationId = 0)" +
                           "left join TimespanConventionTypes on TimespanConventionTypes.TimespanId =  MCCMaintenances.TimespanId " +
                           "left join MaintenanceFrequency on MaintenanceFrequency.FrequencyID = MCCMaintenances.FrequencyID " +
                           "where MCCMaintenances.OrganizationId={1} and (MCCMaintenances.Active is null or MCCMaintenances.Active = 1) Order by  MCCMaintenances.Description", operationTypeStr, OrganizationId, userID
                    );
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MCCMaintenances by OrganizationId=" + OrganizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MCCMaintenances by OrganizationId=" + OrganizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        /// <summary>
        /// AddMCCNotificationType
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="Notification1"></param>
        /// <param name="Notification2"></param>
        /// <param name="Notification3"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int AddMCCNotificationType(int OrganizationID, int OperationTypeID, int Notification1,
                   int Notification2, int Notification3, string Description)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCNotificationType_Add";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationID);
                sqlExec.AddCommandParam("@OperationTypeID", SqlDbType.Int, OperationTypeID);
                sqlExec.AddCommandParam("@Notification1", SqlDbType.Int, Notification1);
                sqlExec.AddCommandParam("@Notification2", SqlDbType.Int, Notification2);
                sqlExec.AddCommandParam("@Notification3", SqlDbType.Int, Notification3);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, Description);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add MCCNotificationType to OrganizationId:" + OrganizationID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add MCCNotificationType to OrganizationId:" + OrganizationID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add MCCNotificationType to OrganizationId:" + OrganizationID;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCNotificationType already exists.");
            }

            return rowsAffected;
        }

        /// <summary>
        /// DeleteMCCNotificationType
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="NotificationID"></param>
        /// <returns></returns>
        public int DeleteMCCNotificationType(int OrganizationID, int NotificationID)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCNotificationType_Delete";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationID);
                sqlExec.AddCommandParam("@NotificationID", SqlDbType.Int, NotificationID);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete MCCNotificationType  NotificationID:" + NotificationID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete MCCNotificationType  NotificationID:" + NotificationID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to Delete MCCNotificationType  NotificationID:" + NotificationID;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCNotificationType does not exist.");
            }

            return rowsAffected;
        }

        /// <summary>
        /// UpdateMCCNotificationType
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="NotificationID"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="Notification1"></param>
        /// <param name="Notification2"></param>
        /// <param name="Notification3"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int UpdateMCCNotificationType(int OrganizationID, int NotificationID, int OperationTypeID, int Notification1,
                   int Notification2, int Notification3, string Description)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MCCNotificationType_Update";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationID);
                sqlExec.AddCommandParam("@NotificationID", SqlDbType.Int, NotificationID);
                sqlExec.AddCommandParam("@OperationTypeID", SqlDbType.Int, OperationTypeID);
                sqlExec.AddCommandParam("@Notification1", SqlDbType.Int, Notification1);
                sqlExec.AddCommandParam("@Notification2", SqlDbType.Int, Notification2);
                sqlExec.AddCommandParam("@Notification3", SqlDbType.Int, Notification3);
                sqlExec.AddCommandParam("@Description", SqlDbType.NVarChar, Description);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update MCCNotificationType to NotificationID:" + NotificationID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update MCCNotificationType to NotificationID:" + NotificationID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update MCCNotificationType to NotificationID:" + NotificationID;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This MCCNotificationType does not exist.");
            }

            return rowsAffected;
        }

        /// <summary>
        /// GetMCCNotificationType
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationTypeID"></param>
        /// <param name="NotificationID"></param>
        /// <param name="operationTypeStr"></param>
        /// <returns></returns>
        public DataSet GetMCCNotificationType(int OrganizationId, int? OperationTypeID, int? NotificationID , string operationTypeStr)
        {
            if (!string.IsNullOrEmpty(operationTypeStr)) operationTypeStr = "," + operationTypeStr;
            DataSet sqlDataSet = null;

            try
            {

                string sql = string.Empty;
                if (OperationTypeID.HasValue)
                {
                    sql = string.Format("Select * {0} " +
                               "from MCCNotificationType  " +
                               "where (OrganizationId={1} or OrganizationId = 0 ) and OperationTypeID = {2}", operationTypeStr, OrganizationId, OperationTypeID
                        );
                }

                if (NotificationID.HasValue)
                {
                    sql = string.Format("Select * {0} " +
                               "from MCCNotificationType  " +
                               "where (OrganizationId={1} or OrganizationId = 0 ) and NotificationID = {2}", operationTypeStr, OrganizationId, NotificationID
                        );

                }

                if (!OperationTypeID.HasValue && !NotificationID.HasValue)
                {
                    sql = string.Format("Select * {0} " +
                               "from MCCNotificationType  " +
                               "where (OrganizationId={1} or OrganizationId = 0 )", operationTypeStr, OrganizationId
                        );
                }

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string errorCondition = "";
                if (OperationTypeID.HasValue)
                {
                    errorCondition = " OperationTypeID=" + OperationTypeID.ToString();
                }

                if (NotificationID.HasValue)
                {
                    errorCondition = " NotificationID=" + NotificationID.ToString();
                }
                string prefixMsg = "Unable to retrieve GetMCCNotificationType by OrganizationId=" + OrganizationId.ToString() + errorCondition + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string errorCondition = "";
                if (OperationTypeID.HasValue)
                {
                    errorCondition = " OperationTypeID=" + OperationTypeID.ToString();
                }

                if (NotificationID.HasValue)
                {
                    errorCondition = " NotificationID=" + NotificationID.ToString();
                }
                string prefixMsg = "Unable to retrieve GetMCCNotificationType by OrganizationId=" + OrganizationId.ToString() + errorCondition + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        /// <summary>
        /// Get Vehicle Maintenance Services
        /// </summary>
        /// <param name="FleetId"></param>
        /// <param name="VehicleId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServices(int FleetId, long VehicleId, int UserId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceGetVehicleServices";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, VehicleId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceVehicleServices by FleetId=" + FleetId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceVehicleServices by FleetId=" + FleetId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }





        /// <summary>
        /// Get Vehicle Maintenance Report
        /// </summary>
        /// <param name="FleetId"></param>
        /// <param name="VehicleId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceVehicleReport(int FleetId,  int UserId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceVehicleReport";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceVehicleReport by FleetId=" + FleetId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceVehicleReport by FleetId=" + FleetId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }


        /// <summary>
        /// GetMCCMaintenanceByMccIdandVehicleID
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <param name="operationTypeStr"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceByMccIdandVehicleID(int OrganizationId, long MccId, string operationTypeStr, string vehicleIds)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(operationTypeStr))
                {
                    operationTypeStr = "," + operationTypeStr;
                    operationTypeStr = operationTypeStr.Replace("OperationTypeID", "MCCMaintenances.OperationTypeID");
                }

                string vehicleStr = string.Empty;
                if (!string.IsNullOrEmpty(vehicleIds))
                {
                    vehicleStr = string.Format(", (SELECT top 1 MaintenanceID FROM VehicleMaintenance where VehicleID in ({0}) and MaintenanceID={1}) AS isAssigned ",
                                                  vehicleIds, "a.MaintenanceId"); 
                }
                else vehicleStr = string.Format(", null as isAssigned ");

                string sqlMaintenance = string.Format("Select MCCMaintenances.*, MCCNotificationType.Description as NotificationDescription, " +
                           "MCCNotificationType.Notification1, MCCNotificationType.Notification2, MCCNotificationType.Notification3 {0} " +
                           "from MCCMaintenances " +
                           "left join MCCNotificationType on MCCMaintenances.NotificationTypeID=MCCNotificationType.NotificationID and ( MCCNotificationType.OrganizationId={1} or MCCNotificationType.OrganizationId = 0 )" +
                           "where (MCCMaintenances.OrganizationId={1}) and (MCCMaintenances.Active is null or MCCMaintenances.Active = 1) ", operationTypeStr, OrganizationId
                    );
                sql = string.Format("Select a.MccId, c.* {0} " +
                               "from MCCMaintenanceAssigment a  " +
                               "inner join ({1}) as c ON a.MaintenanceId = c.MaintenanceId " +
                               "where a.MccId = {2} and (a.Active is null or a.Active = 1) order by Description, OperationType, NotificationDescription", vehicleStr, sqlMaintenance,
                               MccId
                        );
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetMCCMaintenance By MccId =" + MccId.ToString() + " and VehicleID =" + vehicleIds;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetMCCMaintenance By MccId =" + MccId.ToString() + " and VehicleID =" + vehicleIds;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// VehicleMaintenanceAssignment
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <param name="MaintenancesList"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public int VehicleMaintenanceAssignment(string VehiclesList, string MaintenancesList, int MccId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "VehicleMaintenanceAssignment";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@VehiclesList", SqlDbType.VarChar, VehiclesList, - 1);
                sqlExec.AddCommandParam("@MaintenancesList", SqlDbType.VarChar, MaintenancesList);
                sqlExec.AddCommandParam("@MccId", SqlDbType.Int, MccId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Assign VehicleMaintenance VehiclesList=" + VehiclesList + " MaintenancesList=" + MaintenancesList + " MccId=" + MccId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Assign VehicleMaintenance VehiclesList=" + VehiclesList + " MaintenancesList=" + MaintenancesList + " MccId=" + MccId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// MaintenanceGetVehicleServices
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServices(string VehiclesList, int UserId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "MaintenanceGetVehicleServices";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@VehiclesList", SqlDbType.VarChar, VehiclesList, -1);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);


                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve VehicleMaintenance VehiclesList=" + VehiclesList + " VehiclesList=" + VehiclesList;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve VehicleMaintenance VehiclesList=" + VehiclesList + " VehiclesList=" + VehiclesList;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;

        }

        /// <summary>
        /// MaintenanceClose
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="MaintenanceID"></param>
        /// <param name="ServiceValue"></param>
        /// <returns></returns>
        public int MaintenanceClose(int UserId, long VehicleId, int MaintenanceID, int ServiceValue, DateTime MaintenanceDateTime, Int64 MccId, int dueValue)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MaintenanceClose";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, VehicleId);
                sqlExec.AddCommandParam("@MaintenanceID", SqlDbType.Int, MaintenanceID);
                sqlExec.AddCommandParam("@serviceValue", SqlDbType.Int, ServiceValue);
                sqlExec.AddCommandParam("@dueValue", SqlDbType.Int, dueValue);
                sqlExec.AddCommandParam("@MaintenanceDateTime", SqlDbType.DateTime, MaintenanceDateTime);
                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Close Maintenance UserId=" + UserId + " vehicleId=" + VehicleId + " ServiceValue=" + ServiceValue.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Close Maintenance UserId=" + UserId + " vehicleId=" + VehicleId + " ServiceValue=" + ServiceValue.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }



        /// <summary>
        /// GetTimespanConventionTypes()
        /// </summary>
        /// <returns></returns>
        public DataSet GetTimespanConventionTypes()
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "Select * from TimespanConventionTypes";
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve TimespanConventionTypes";

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve TimespanConventionTypes";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// GetMaintenanceFrequency
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaintenanceFrequency()
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "Select * from MaintenanceFrequency";
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceFrequency";

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceFrequency";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// GetMCCMaintenanceByMccId
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <param name="operationTypeStr"></param>
        /// <returns></returns>
        public DataSet GetMCCMaintenanceByMccId(int OrganizationId, long MccId, string operationTypeStr, int UserId)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(operationTypeStr))
                {
                    operationTypeStr = "," + operationTypeStr;
                    operationTypeStr = operationTypeStr.Replace("OperationTypeID", "MCCMaintenances.OperationTypeID");
                }


                string sqlMaintenance = string.Format(
                           "Select MCCMaintenances.*, MCCNotificationType.Description as NotificationDescription, dbo.ConvertMonthDay(MCCMaintenances.FixedServiceDate) as FixedServiceDate_1," +
                           "MaintenanceFrequency.FrequencyName, " +
                           "Case MCCMaintenances.OperationTypeID " +
                           "when 1 then CEILING(MCCMaintenances.interval*@unitOfMes) when 2 then MCCMaintenances.interval/60 else MCCMaintenances.interval end as IntervalDesc, " + 
                           "MCCNotificationType.Notification1, MCCNotificationType.Notification2, MCCNotificationType.Notification3 {0} " +
                           "from MCCMaintenances " +
                           "left join MCCNotificationType on MCCMaintenances.NotificationTypeID=MCCNotificationType.NotificationID and ( MCCNotificationType.OrganizationId={1} or MCCNotificationType.OrganizationId = 0 )" +
                           "left join TimespanConventionTypes on TimespanConventionTypes.TimespanId =  MCCMaintenances.TimespanId " +
                           "left join MaintenanceFrequency on MaintenanceFrequency.FrequencyID = MCCMaintenances.FrequencyID " +
                           "where (MCCMaintenances.OrganizationId={1} ) and (MCCMaintenances.Active is null or MCCMaintenances.Active = 1) ", operationTypeStr, OrganizationId
                    );

                sql = string.Format("DECLARE @unitOfMes float; " +
               "select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={2} and PreferenceId=0; " + 
               "Select a.MccId, c.* " +
               "from MCCMaintenanceAssigment a  " +
               "inner join ({0}) as c ON a.MaintenanceId = c.MaintenanceId " +
               "where a.MccId = {1}  and (a.Active is null or a.Active = 1)  order by Description, OperationType, NotificationDescription", sqlMaintenance,
               MccId, UserId
                );

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetMCCMaintenance By MccId =" + MccId.ToString() ;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetMCCMaintenance By MccId =" + MccId.ToString() ;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// GetVehicleAssignedMCC
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="maintenanceIds"></param>
        /// <returns></returns>
        public DataSet GetVehicleAssignedMCC(int fleetId, string maintenanceIds, long MccId)
        {
            DataSet sqlDataSet = null;
            int maintenanceIdsnum = maintenanceIds.Split(',').Length;
            try
            {
                string sql = string.Format("Select a.VehicleId, b.Description from vlfFleetVehicles a " + 
                                  "inner join vlfVehicleInfo b on a.VehicleId = b.VehicleId where a.FleetId = {0} and " +
                                  "{1} =  (Select count(VehicleId) from VehicleMaintenance where VehicleId= a.VehicleId and MaintenanceID in ({2}) and MccId = {3}) order by Description"
                                  , fleetId, maintenanceIdsnum, maintenanceIds, MccId
                    );

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;                
        }


        public DataSet GetVehicleAssignedMCCMultiFleet(string fleetId, string maintenanceIds, long MccId)
        {
            DataSet sqlDataSet = null;
            int maintenanceIdsnum = maintenanceIds.Split(',').Length;
            try
            {
                string sql = string.Format("Select a.VehicleId, b.Description from vlfFleetVehicles a " +
                                  "inner join vlfVehicleInfo b on a.VehicleId = b.VehicleId where a.FleetId in ({0}) and " +
                                  "{1} =  (Select count(VehicleId) from VehicleMaintenance where VehicleId= a.VehicleId and MaintenanceID in ({2}) and MccId = {3}) order by Description"
                                  , fleetId, maintenanceIdsnum, maintenanceIds, MccId
                    );

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        /// <summary>
        /// GetVehicleUnAssignedMCC
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="maintenanceIds"></param>
        /// <returns></returns>
        public DataSet GetVehicleUnAssignedMCC(int fleetId, string maintenanceIds, long MccId)
        {
            int maintenanceIdsnum = maintenanceIds.Split(',').Length;
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Format("Select a.VehicleId, b.Description from vlfFleetVehicles a " +
                                  "inner join vlfVehicleInfo b on a.VehicleId = b.VehicleId where a.FleetId = {0} and " +
                                  "{1} <>  (Select count(VehicleId) from VehicleMaintenance where VehicleId = a.VehicleId and MaintenanceID in ({2}) and MccId = {3}) order by Description"
                                  , fleetId, maintenanceIdsnum, maintenanceIds, MccId
                    );

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleUnAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleUnAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        public DataSet GetVehicleUnAssignedMCCMultiFleet(string fleetId, string maintenanceIds, long MccId)
        {
            int maintenanceIdsnum = maintenanceIds.Split(',').Length;
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Format("Select a.VehicleId, b.Description from vlfFleetVehicles a " +
                                  "inner join vlfVehicleInfo b on a.VehicleId = b.VehicleId where a.FleetId in ({0}) and " +
                                  "{1} <>  (Select count(VehicleId) from VehicleMaintenance where VehicleId = a.VehicleId and MaintenanceID in ({2}) and MccId = {3}) order by Description"
                                  , fleetId, maintenanceIdsnum, maintenanceIds, MccId
                    );

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleUnAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehicleUnAssignedMCC By fleetId =" + fleetId + " maintenanceIds=" + maintenanceIds;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        /// <summary>
        /// MaintenanceVehiclesUnAssignment
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <param name="MaintenanceList"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int MaintenanceVehiclesUnAssignment(string VehiclesList, string MaintenanceList, int UserID, long MccId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "MaintenanceVehiclesUnAssignment";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@VehiclesList", SqlDbType.VarChar, VehiclesList , - 1);
                sqlExec.AddCommandParam("@MaintenanceList", SqlDbType.VarChar, MaintenanceList, -1);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, UserID);
                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to MaintenanceVehiclesUnAssignment UserId=" + UserID + " VehiclesList=" + VehiclesList + " MaintenanceList=" + MaintenanceList;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to MaintenanceVehiclesUnAssignment UserId=" + UserID + " VehiclesList=" + VehiclesList + " MaintenanceList=" + MaintenanceList;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }

        /// <summary>
        /// MaintenanceGetVehicleServicesHistory
        /// </summary>
        /// <param name="VehiclesList"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServicesHistory(string VehiclesList, int UserId, DateTime BeginDate)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = "MaintenanceGetVehicleServicesHistory";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@VehiclesList", SqlDbType.VarChar, VehiclesList, -1);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                sqlExec.AddCommandParam("@BeginDate", SqlDbType.DateTime, BeginDate);
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetVehicleServicesHistory VehiclesList=" + VehiclesList ;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetVehicleServicesHistory VehiclesList=" + VehiclesList;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;

        }

        /// <summary>
        /// VehicleMaintenanceUpdateComment
        /// </summary>
        /// <param name="MaintenanceID"></param>
        /// <param name="VehicleId"></param>
        /// <param name="Comments"></param>
        /// <returns></returns>
        public int VehicleMaintenanceUpdateComment(int MaintenanceID, long VehicleId, string Comments, int OrganizationId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "VehicleMaintenanceUpdateComment";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MaintenanceID", SqlDbType.Int, MaintenanceID);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, VehicleId);
                sqlExec.AddCommandParam("@Comments", SqlDbType.VarChar, Comments);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to VehicleMaintenanceUpdateComment MaintenanceID=" + MaintenanceID + " VehicleId=" + VehicleId.ToString() + " Comments=" + Comments;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to VehicleMaintenanceUpdateComment MaintenanceID=" + MaintenanceID + " VehicleId=" + VehicleId.ToString() + " Comments=" + Comments;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;

        }
         
        /// <summary>
        /// GetMccGroupByName(string name, int OrganizationId)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetMccGroupByName(string name, int OrganizationId, long? Mccid)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Empty;
                if (Mccid == null)
                {
                    sql = string.Format(" Select * from MCCGroup where OrganizationId ={0} and MccName='{1}' and (MCCGroup.Active is null or MCCGroup.Active = 1) Order by MccName ", OrganizationId, name.Replace("'", "''"));
                }
                else
                {
                    sql = string.Format(" Select * from MCCGroup where OrganizationId ={0} and MccName='{1}' and MccId <> {2} and (MCCGroup.Active is null or MCCGroup.Active = 1) Order by MccName ", OrganizationId, name.Replace("'", "''"), Mccid.Value);
                }
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetMccGroupByName By OrganizationId =" + OrganizationId.ToString() + " MccName=" + name;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetMccGroupByName By OrganizationId =" + OrganizationId.ToString() + " MccName=" + name;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// GetMCCMaintenanceByVehicles
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="MccId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetServicesByVehicles(int OrganizationId, string VehiclesList, int UserID)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceGetServicesByVehicles";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@VehiclesList", SqlDbType.VarChar, VehiclesList, -1);
                sqlExec.AddCommandParam("@UserID", SqlDbType.Int, UserID);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetServicesByVehicles by OrganizationId=" + OrganizationId.ToString() + " VehicleIDs=" + @VehiclesList;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetServicesByVehicles by OrganizationId=" + OrganizationId.ToString() + " VehicleIDs=" + @VehiclesList;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        /// <summary>
        /// MaintenanceGetVehiclesByMaintenanceId
        /// </summary>
        /// <param name="MaintenanceId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehiclesByMaintenanceId(int MaintenanceId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceGetVehiclesByMaintenanceId";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MaintenanceId", SqlDbType.Int, MaintenanceId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetVehiclesByMaintenanceId by MaintenanceId=" + MaintenanceId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetVehiclesByMaintenanceId by MaintenanceId=" + MaintenanceId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        ///
        public DataSet MaintenanceGetMccGroupByMaintenanceId(int MaintenanceId,int OrganizationId )
        {
            DataSet sqlDataSet = null;
            try
            {
                string sql = string.Empty;
                sql = "Select Distinct a.MccName from MCCGroup a " +
                    "inner join MCCMaintenanceAssigment on a.MccId = MCCMaintenanceAssigment.MccId " +
                    "and (a.Active is null or a.Active = 1) and a.OrganizationId={0} and " +
                    "MCCMaintenanceAssigment.MaintenanceId={1} and (MCCMaintenanceAssigment.Active is null or MCCMaintenanceAssigment.Active = 1) ";
                sql = string.Format(sql, OrganizationId, MaintenanceId);
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetMccGroupByMaintenanceId By MaintenanceId =" + MaintenanceId.ToString() + " OrganizationId=" + OrganizationId.ToString();

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetMccGroupByMaintenanceId By MaintenanceId =" + MaintenanceId.ToString() + " OrganizationId=" + OrganizationId.ToString();

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet MaintenanceGetVehiclesByMccId(int MccId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceGetVehiclesByMccId";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetVehiclesByMccId by MccId=" + MccId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetVehiclesByMccId by MccId=" + MccId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

        public DataSet MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId(long MccId, string MaintenanceIds)
        { 
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceGetUnAssignedVehiclesByMccIDAndMaintenanceId";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                sqlExec.AddCommandParam("@MaintenanceIds", SqlDbType.VarChar, MaintenanceIds, -1);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId by MccId=" + MccId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId by MccId=" + MccId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId(long MccId, string MaintenanceIds)
        { 
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MccId", SqlDbType.BigInt, MccId);
                sqlExec.AddCommandParam("@MaintenanceIds", SqlDbType.VarChar, MaintenanceIds, -1);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId by MccId=" + MccId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceGetAssignedVehiclesByMccIDAndMaintenanceId by MccId=" + MccId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet MaintenanceBoxUserSettings_Get(int FleetId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "MaintenanceBoxUserSettings_Get";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceBoxUserSettings_Get by FleetId=" + FleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceBoxUserSettings_Get by FleetId=" + FleetId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet MaintenanceGetOperationReport(DateTime dt, int OrganizationId)
        {
            DataSet sqlDataSet = null;

            try
            {
                string sql = "MaintenanceGetOperationReport";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@MinDate", SqlDbType.DateTime, dt);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetOperationReport ";

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetOperationReport ";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet Maintenance_GetVehiclesEngineHours(string VehiclesList)
        {
            DataSet sqlDataSet = null;

            try
            {
                string sql = "Maintenance_GetVehiclesEngineHours";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@VehiclesList", SqlDbType.VarChar, VehiclesList, -1);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Maintenance_GetVehiclesEngineHours by VehiclesList= " + VehiclesList;

                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Maintenance_GetVehiclesEngineHours by VehiclesList= " + VehiclesList;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;        
        }

        public int Maintenance_UpdateEngineHours(long VehicleId, int CurrentEngineHours, int UserId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "Maintenance_UpdateEngineHours";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, VehicleId);
                sqlExec.AddCommandParam("@CurrentEngineHours", SqlDbType.Int, CurrentEngineHours);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Maintenance_UpdateEngineHours VehicleId=" + VehicleId.ToString() + " CurrentEngineHours=" + CurrentEngineHours.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Maintenance_UpdateEngineHours VehicleId=" + VehicleId.ToString() + " CurrentEngineHours=" + CurrentEngineHours.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }



        /// <summary>
        /// Get Vehicle Maintenance Services
        /// </summary>
        /// <param name="FleetId"></param>
        /// <param name="VehicleId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet MaintenanceGetVehicleServices_DashBoard(int UserId, int FleetId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "DashBoard_Maintenance";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceVehicleServices by UserId=" + UserId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve MaintenanceVehicleServices by UserId=" + UserId.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }

    }
}
