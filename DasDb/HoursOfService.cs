using System;
using System.Collections.Generic;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using System.Text;

namespace VLF.DAS.DB
{

    /// <summary>
    /// Provides interfaces to vlfHoursOfService table.
    /// </summary>
    public class HoursOfService : TblOneIntPrimaryKey
    {
        const string tablename = "vlfHoursOfService";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public HoursOfService(SQLExecuter sqlExec) : base("vlfHoursOfService", sqlExec) { }



        /// <summary>
        /// Add new HoursOfService process record
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="timestamp"></param>       
        /// <param name="state"></param>
        /// <param name="cycle"></param>
        /// <param name="IsSecondDay"></param>
        /// <param name="IsCalculated"></param>
        /// <param name="IsSignedOff"></param>
        public int AddHoursOfService(int driverId,
                                        DateTime timestamp,
                                        Int16 state,
                                        Int16 cycle,
                                        bool IsSecondDay,
                                        bool IsCalculated,
                                        bool IsSignedOff,
                                        int userId, string description, Int16 ruleId)
        {
            HoursOfServiceParams values = new HoursOfServiceParams();
            values.DriverId = driverId;
            values.Timestamp = timestamp;
            values.State = (Enums.HosDutyState)state;
            values.Cycle = (Enums.HosDutyCycle)cycle;
            values.IsSecondDay = IsSecondDay;
            values.IsCalculated = IsCalculated;
            values.IsSignedOff = IsSignedOff;
            values.UserId = userId;
            values.Description  = description;
            values.Rule = (Enums.HosRule)ruleId;
            return AddHoursOfService(values);
        }

        /// <summary>
        /// Add new HoursOfService process record
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int AddHoursOfService(HoursOfServiceParams values)
        {
            int serviceId = 0;
            try
            {
                // 1. Set SQL command
                string sql = "INSERT INTO vlfHoursOfService (DriverId, Timestamp, StateTypeId, Cycle, IsSecondDay,  IsCalculated, IsSignedOff,UserId,HOSDescription,ruleId ) VALUES ( @DriverId, @Timestamp, @State, @Cycle, @IsSecondDay, @IsCalculated, @IsSignedOff,@UserId,@HOSDescription,@ruleId) Select Max(HoursOfServiceId) from vlfHoursOfService";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, values.DriverId);
                sqlExec.AddCommandParam("@Timestamp", SqlDbType.DateTime, values.Timestamp);
                sqlExec.AddCommandParam("@State", SqlDbType.SmallInt, values.State);
                sqlExec.AddCommandParam("@Cycle", SqlDbType.SmallInt, values.Cycle);
                sqlExec.AddCommandParam("@IsSecondDay", SqlDbType.Bit, values.IsSecondDay);
                sqlExec.AddCommandParam("@IsCalculated", SqlDbType.Bit, values.IsCalculated);
                sqlExec.AddCommandParam("@IsSignedOff", SqlDbType.Bit, values.IsSignedOff);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, values.UserId);
                sqlExec.AddCommandParam("@HOSDescription", SqlDbType.VarChar, values.Description.Replace("'","''"));
                sqlExec.AddCommandParam("@ruleId", SqlDbType.SmallInt, values.Rule);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                serviceId = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
                // 5. Clean Calculated flag: 15 days back
                sql = "Update vlfHoursOfService set IsCalculated=0 where DriverId=" + values.DriverId + " and Timestamp > DateAdd(d,-15,'" + values.Timestamp + "') and Timestamp <= '" + values.Timestamp + "'";
                sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' HoursOfService.", values.DriverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' HoursOfService.", values.DriverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }


            return serviceId;
        }


        /// <summary>
        /// Disable HoursOfService
        /// </summary>
        /// <param name="hoursOfServiceId"></param>
        /// <param name="changedByServiceId"></param>       
        public int DisableHOS(int hoursOfServiceId, int changedByServiceId, int userId)
        {
            int rowsAffected = 0;
            try
            {



                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@HoursOfServiceId", SqlDbType.Int, hoursOfServiceId);
                sqlExec.AddCommandParam("@ChangedByServiceId", SqlDbType.Int, changedByServiceId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

                rowsAffected = sqlExec.SPExecuteNonQuery("sp_DisableHOSentry");
                return rowsAffected;

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to disable '{0}' HoursOfService.", hoursOfServiceId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to disable '{0}' HoursOfService.", hoursOfServiceId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }

            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to disable '{0}' HoursOfService.", hoursOfServiceId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, Unable to disable HoursOfService.", prefixMsg));

            }
            return rowsAffected;
        }

        /// <summary>
        /// Remove existing HoursOfService records by DriverId
        /// </summary>
        /// <param name="driverId"></param>
        public void RemoveAllHoursOfServiceByDriverId(int driverId)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE DriverId = @DriverId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' HoursOfService.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' HoursOfService.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' HoursOfService.", driverId);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Remove existing HoursOfService records by DriverId
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="timestamp"></param>
        public void RemoveAllHoursOfServiceByDriverId(int driverId, DateTime timestamp)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE DriverId=@DriverId AND Timestamp=@Timestamp", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@Timestamp", SqlDbType.DateTime, timestamp);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to remove HoursOfService record {0} for Driver:{1}.", timestamp, driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove HoursOfService record {0} for Driver:{1}.", timestamp, driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove HoursOfService record {0} for Driver:{1}.", timestamp, driverId);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Retrieves list of HoursOfService for driverID
        /// </summary>
        /// <returns>List of HoursOfServiceParams</returns>
        public SortedList<DateTime, HoursOfServiceParams> GetHoursOfService(int driverId)
        {
            SortedList<DateTime, HoursOfServiceParams> hosRecords = new SortedList<DateTime, HoursOfServiceParams>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT HoursOfServiceId, DriverId, Timestamp, StateTypeId as State, Cycle, IsSecondDay, IsCalculated, IsSignedOff  FROM {0} WHERE DriverId=@DriverId AND IsCalculated=0 AND ChangedByServiceId IS NULL", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                ds = sqlExec.SQLExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            HoursOfServiceParams p = new HoursOfServiceParams();
                            p.DriverId = Convert.ToInt32(dt.Rows[i]["DriverId"]);
                            p.Timestamp = Convert.ToDateTime(dt.Rows[i]["Timestamp"]);
                            p.State = (Enums.HosDutyState)Convert.ToInt32(dt.Rows[i]["State"]);
                            p.Cycle = (Enums.HosDutyCycle)Convert.ToInt32(dt.Rows[i]["Cycle"]);
                            p.IsSecondDay = Convert.ToBoolean(dt.Rows[i]["IsSecondDay"]);
                            p.IsCalculated = Convert.ToBoolean(dt.Rows[i]["IsCalculated"]);
                            p.IsSignedOff = Convert.ToBoolean(dt.Rows[i]["IsSignedOff"]); hosRecords.Add(p.Timestamp, p);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return hosRecords;

        }

        /// <summary>
        /// Retrieves list of HoursOfService for driverID for specified date range
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>List of HoursOfServiceParams</returns>
        public SortedList<DateTime, HoursOfServiceParams> GetHoursOfService(int driverId, DateTime from, DateTime to)
        {
            SortedList<DateTime, HoursOfServiceParams> hosRecords = new SortedList<DateTime, HoursOfServiceParams>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT HoursOfServiceId, DriverId, Timestamp, StateTypeId as State, Cycle, IsSecondDay, IsCalculated, IsSignedOff FROM {0} WHERE DriverId=@DriverId AND (Timestamp BETWEEN @From AND @To) AND ChangedByServiceId IS NULL", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                ds = sqlExec.SQLExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            HoursOfServiceParams p = new HoursOfServiceParams();
                            p.DriverId = Convert.ToInt32(dt.Rows[i]["DriverId"]);
                            p.Timestamp = Convert.ToDateTime(dt.Rows[i]["Timestamp"]);
                            p.State = (Enums.HosDutyState)Convert.ToInt32(dt.Rows[i]["State"]);
                            p.Cycle = (Enums.HosDutyCycle)Convert.ToInt32(dt.Rows[i]["Cycle"]);
                            p.IsSecondDay = Convert.ToBoolean(dt.Rows[i]["IsSecondDay"]);
                            p.IsCalculated = Convert.ToBoolean(dt.Rows[i]["IsCalculated"]);
                            p.IsSignedOff = Convert.ToBoolean(dt.Rows[i]["IsSignedOff"]);
                            hosRecords.Add(p.Timestamp, p);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return hosRecords;

        }

        /// <summary>
        /// Retrieves list of HoursOfService for current 14day date range
        /// </summary>
        /// <returns></returns>
        public SortedList<DateTime, HoursOfServiceParams> GetCurrentHoursOfServiceRange(int driverId) { return GetCurrentHoursOfServiceRange(driverId, DateTime.UtcNow); }

        /// <summary>
        /// Retrieves list of HoursOfService for 14day date range
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public SortedList<DateTime, HoursOfServiceParams> GetCurrentHoursOfServiceRange(int driverId, DateTime utc)
        {
            SortedList<DateTime, HoursOfServiceParams> hosRecords = new SortedList<DateTime, HoursOfServiceParams>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT HoursOfServiceId, DriverId, Timestamp, StateTypeId as State, Cycle, IsSecondDay,  IsCalculated, IsSignedOff FROM {0} WHERE (DriverId = @DriverId) AND (Timestamp BETWEEN @From AND @To) AND ChangedByServiceId IS NULL ORDER BY Timestamp", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, utc.AddDays(-14));
                DateTime xdate = utc.Date.AddDays(1);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, xdate);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                ds = sqlExec.SQLExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            HoursOfServiceParams p = new HoursOfServiceParams();
                            p.DriverId = Convert.ToInt32(dt.Rows[i]["DriverId"]);
                            p.Timestamp = Convert.ToDateTime(dt.Rows[i]["Timestamp"]);
                            p.State = (Enums.HosDutyState)Convert.ToInt32(dt.Rows[i]["State"]);
                            p.Cycle = (Enums.HosDutyCycle)Convert.ToInt32(dt.Rows[i]["Cycle"]);
                            p.IsSecondDay = Convert.ToBoolean(dt.Rows[i]["IsSecondDay"]);
                            p.IsCalculated = Convert.ToBoolean(dt.Rows[i]["IsCalculated"]);
                            p.IsSignedOff = Convert.ToBoolean(dt.Rows[i]["IsSignedOff"]);
                            hosRecords.Add(p.Timestamp, p);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve current HosRecords working set.");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve current HosRecords working set.");
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return hosRecords;

        }

        /// <summary>
        /// Retrieves summary of HoursOfService based on DateTime and driver ID
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public SortedList<DateTime, int> GetHOSRefIDs(int driverId, DateTime from, DateTime to)
        {
            SortedList<DateTime, int> hosRefIDs = new SortedList<DateTime, int>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = "GetHOSRefIDs";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@start", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@stop", SqlDbType.DateTime, to);
                ds = sqlExec.SPExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {                            
                            hosRefIDs.Add(Convert.ToDateTime(dt.Rows[i]["StartDate"]),Convert.ToInt32(dt.Rows[i]["Refid"]));
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return hosRefIDs;

        }

        /// <summary>
        /// Retrieves summary of HoursOfService based on DateTime and driver ID
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public SqlDataReader GetHOSSummarybyDateTime(int refid, DateTime refdate)
        {

            SqlDataReader rdr = null;
            try
            {
                // 1. Set SQL command
                string sql = "GetTimeLogData";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@refid", SqlDbType.Int, refid);
                sqlExec.AddCommandParam("@date", SqlDbType.DateTime, refdate);                 
                rdr = sqlExec.SQLExecuteSPGetDataReader(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for RefID:{0}.", refid);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for RefID:{0}.", refid);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return rdr;

        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves list of HoursOfService based on DateTime
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public DataSet GetHOSbyDateTime_NewTZ(int driverId, int userId, DateTime from, DateTime to)
        {

            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = "sp_GetHOSbyDateTime_NewTimeZone";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                ds = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return ds;

        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves list of HoursOfService based on DateTime
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public DataSet GetHOSbyDateTime(int driverId, int userId, DateTime from, DateTime to)
        {

            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = "sp_GetHOSbyDateTime";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                ds = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosRecords for Driver:{0}.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return ds;

        }


        /// <summary>
        /// Get Hours Of Service State Types
        /// </summary>
        /// <returns></returns>
        public DataSet GetHoursOfServiceStateTypes()
        {

            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = "SELECT StateTypeId,StateTypeName from vlfHoursOfServiceStateTypes";
                ds = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve HoursOfServiceStateTypes.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve HoursOfServiceStateTypes.";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return ds;

        }



        /// <summary>
        /// Get Hours Of Service Rules
        /// </summary>
        /// <returns></returns>
        public DataSet GetHoursOfServiceRules()
        {

            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = "SELECT RuleId,RuleName from vlfHoursOfServiceRules";
                ds = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve HoursOfServiceRules.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve HoursOfServiceRules.";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return ds;

        }



        /// <summary>
        /// GetNotCalculatedHOS - 15 days back from the latest not calculated timestamp
        /// </summary>

        public string GetNotCalculatedHOS()
        {
            string xml = string.Empty;
            DataSet ds = null;
            try
            {
                string sql = "sp_GetNotCalculatedHOS";
                sqlExec.ClearCommandParameters();
                ds = sqlExec.SPExecuteDataset(sql);

                if (ds != null && ds.Tables.Count > 0 &&  ds.Tables[0].Rows.Count> 0)
                    xml = ds.GetXml();
                else
                    xml = "";
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetNotCalculatedHOS.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetNotCalculatedHOS.";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return xml;
        }


        /// <summary>
        /// GetNotCalculatedHOS - 15 days back from the latest not calculated timestamp
        /// </summary>

        public void SetCalculatedHOS(int driverId)
        {
            try
            {
                string sql = "sp_SetCalculatedHOS";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                int result = sqlExec.SPExecuteNonQuery(sql);
           }

            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to execute SetCalculatedHOS for Driver:{0}", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to execute SetCalculatedHOS for Driver:{0}", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
        }

        /// <summary>
        /// Validate HOS entry
        /// </summary>

        public void ValidateHOSentry(int driverId, DateTime timestamp, int stateTypeId, ref int existingHoursOfServiceId, ref Int16 flagOverride)
        {
            //try
            //{
            //    using (SqlConnection conn = new SqlConnection(sqlExec.ConnectionString))
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd = new SqlCommand("sp_ValidateHOSentry", conn))
            //        {
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            cmd.Parameters.Add("@DriverId", SqlDbType.Int);
            //            cmd.Parameters["@DriverId"].Value = driverId;

            //            cmd.Parameters.Add("@Timestamp", SqlDbType.DateTime);
            //            cmd.Parameters["@Timestamp"].Value = timestamp;

            //            cmd.Parameters.Add("@StateTypeId", SqlDbType.Bit);
            //            cmd.Parameters["@StateTypeId"].Value = stateTypeId;

            //            cmd.Parameters.Add("@existingHoursOfServiceId", SqlDbType.Int);
            //            cmd.Parameters["@existingHoursOfServiceId"].Direction = ParameterDirection.Output ;

            //            cmd.Parameters.Add("@Override", SqlDbType.Bit);
            //            cmd.Parameters["@Override"].Direction = ParameterDirection.Output;

            //            cmd.ExecuteNonQuery();


            //            flagOverride = Convert.ToInt16(cmd.Parameters["@Override"].Value);
            //            existingHoursOfServiceId = Convert.ToInt32(cmd.Parameters["@existingHoursOfServiceId"].Value);


            //        }
            //    }
            //}



            DataSet ds = null;
            try
            {
                existingHoursOfServiceId = 0;
                flagOverride = 0;
                // 1. Set SQL command
                string sql = "sp_ValidateHOSentry";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@Timestamp", SqlDbType.DateTime, timestamp);
                sqlExec.AddCommandParam("@stateTypeId", SqlDbType.Int, stateTypeId);
                ds = sqlExec.SPExecuteDataset(sql);

                if (ds != null && ds.Tables.Count > 0)
                {
                    flagOverride = Convert.ToInt16(ds.Tables[0].Rows[0]["flagOverride"]);
                    existingHoursOfServiceId = Convert.ToInt32(ds.Tables[0].Rows[0]["existingHoursOfServiceId"]);
                }
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve HoursOfServiceStateTypes.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve HoursOfServiceStateTypes.";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }


        }



        /// <summary>
        /// HoursOfService record structure
        /// 	[HoursOfServiceId] [int] IDENTITY(1,1) NOT NULL,
        /// 	[DriverId] [int] NOT NULL,
        /// 	[Timestamp] [datetime] NOT NULL,
        /// 	[State] [tinyint] NOT NULL,
        /// 	[Cycle] [tinyint] NOT NULL,
        /// 	[IsSecondDay] [bit] NOT NULL,
        /// 	[IsCalculated] [bit] NOT NULL CONSTRAINT [DF_vlfHoursOfService_IsCalculated]  DEFAULT ((0)),
        /// 	[IsSignedOff] [bit] NOT NULL CONSTRAINT [DF_vlfHoursOfService_IsSignedOff]  DEFAULT ((0)),
        /// </summary>
        public struct HoursOfServiceParams
        {
            /// <summary>
            /// id
            /// </summary>
            public int Id;

            /// <summary>
            /// DriverId id
            /// </summary>
            public int DriverId;

            /// <summary>
            /// Timestamp
            /// </summary>
            public DateTime Timestamp;

            /// <summary>
            /// State
            /// </summary>
            public Enums.HosDutyState State;

            /// <summary>
            /// Cycle
            /// </summary>
            public Enums.HosDutyCycle Cycle;

            /// <summary>
            /// Daily calculations for deferrals
            /// </summary>
            public bool IsSecondDay;


            /// <summary>
            /// Value is has been calculated for exceptions
            /// </summary>
            public bool IsCalculated;


            /// <summary>
            /// Value has been signoff by driver (locked in)
            /// </summary>
            public bool IsSignedOff;

            /// <summary>
            /// Added by User Id
            /// </summary>
            public int UserId;
            /// <summary>
            /// Description of HOS
            /// </summary>
            public string Description;

            /// <summary>
            /// Rule
            /// </summary>
            public Enums.HosRule Rule;
        }



    }
}