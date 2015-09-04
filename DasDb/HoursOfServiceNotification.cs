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
    /// Provides interfaces to vlfHoursOfServiceNotification table.
    /// </summary>
    public class HoursOfServiceNotification : TblOneIntPrimaryKey
    {
        const string tablename = "vlfHoursOfServiceNotification";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public HoursOfServiceNotification(SQLExecuter sqlExec) : base("vlfHoursOfServiceNotification", sqlExec) { }

        /// <summary>
        /// Add new AddHoursOfServiceNofication record
        /// </summary>
        /// <param name="parameters"></param>
        public void AddHoursOfServiceNofication(HoursOfServiceNotificationParams parameters)
        {
            AddHoursOfServiceNofication(parameters.DriverId, parameters.Timestamp, parameters.Severity, parameters.ExtendedData);
        }


        /// <summary>
        /// Add new AddHoursOfServiceNofication record
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="timestamp"></param>
        /// <param name="severity"></param>
        /// <param name="extendedData"></param>
        public void AddHoursOfServiceNofication(int driverId, DateTime timestamp, Enums.HosNotificationSeverity severity, string extendedData)
        {
            int rowsAffected = 0;
            try
            {

                // 1. Set SQL command
               // string sql = string.Format("INSERT INTO {0} (DriverId, Timestamp, Severity, ExtendedData) VALUES ( @DriverId, @Timestamp, @Severity, @ExtendedData )", tableName);



                //[HoursOfServiceNotificationId] [int] NOT NULL,
                //[DriverId] [int] NOT NULL,
                //[Timestamp] [datetime] NOT NULL,
                //[Severity] [tinyint] NOT NULL,
                //[ExtendedData] [varchar](50) NOT NULL,

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@Timestamp", SqlDbType.DateTime, timestamp);
                sqlExec.AddCommandParam("@Severity", SqlDbType.TinyInt, (byte)severity);
                sqlExec.AddCommandParam("@ExtendedData", SqlDbType.VarChar, extendedData);

                //if (sqlExec.RequiredTransaction())
                //{
                //    // 3. Attaches SQL to transaction
                //    sqlExec.AttachToTransaction(sql);
                //}
                //// 4. Executes SQL statement
                //rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
                rowsAffected = sqlExec.SPExecuteNonQuery("sp_HoursOfServiceNotification_Add");
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' HoursOfServiceNofication.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' HoursOfServiceNofication.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' HoursOfServiceNofication.", driverId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This box already exists.", prefixMsg));
            }
        }



        /// <summary>
        /// Add new AddHoursOfServiceNofication record
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="timestamp"></param>
        /// <param name="severity"></param>
        /// <param name="extendedData"></param>
        public void AddHoursOfServiceNofication(int HosId, Enums.HosNotificationSeverity severity, Enums.HosException hosException)
        {
            int rowsAffected = 0;
            try
            {

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@HosId", SqlDbType.Int, HosId);
                sqlExec.AddCommandParam("@Severity", SqlDbType.TinyInt, (byte)severity);
                sqlExec.AddCommandParam("@HosException", SqlDbType.TinyInt, (byte)hosException);

                //if (sqlExec.RequiredTransaction())
                //{
                //    // 3. Attaches SQL to transaction
                //    sqlExec.AttachToTransaction(sql);
                //}
                //// 4. Executes SQL statement
                //rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
                rowsAffected = sqlExec.SPExecuteNonQuery("sp_HoursOfServiceNotificationAdd");
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to add new HoursOfServiceNofication.HosId: '{0}'", HosId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new HoursOfServiceNofication.HosId: '{0}'", HosId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new HoursOfServiceNofication.HosId: '{0}'", HosId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This box already exists.", prefixMsg));
            }
        }


        /// <summary>
        /// Remove existing HoursOfServiceNofication records by DriverId
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
                string prefixMsg = string.Format("Unable to remove '{0}' HoursOfServiceNofication.", driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' HoursOfServiceNofication.", driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' HoursOfServiceNofication.", driverId);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Remove existing HoursOfServiceNofication records by DriverId
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="timestamp"></param>
        public void RemoveAllHoursOfServiceNotificationsByDriverId(int driverId, DateTime timestamp)
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
                string prefixMsg = string.Format("Unable to remove HoursOfServiceNofication record {0} for Driver:{1}.", timestamp, driverId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove HoursOfServiceNofication record {0} for Driver:{1}.", timestamp, driverId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove HoursOfServiceNofication record {0} for Driver:{1}.", timestamp, driverId);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Retrieves list of HoursOfServiceNofication for driverID
        /// </summary>
        /// <returns>List of HoursOfServiceParams</returns>
        public SortedList<DateTime, HoursOfServiceNotificationParams> GetHoursOfServiceNotificationsByDriverId(int driverId)
        {
            SortedList<DateTime, HoursOfServiceNotificationParams> hosRecords = new SortedList<DateTime, HoursOfServiceNotificationParams>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT HoursOfServiceNotificationId, DriverId, Timestamp, Severity, ExtendedData FROM {0} WHERE DriverId=@DriverId", tableName);

                //[HoursOfServiceNotificationId] [int] NOT NULL,
                //[DriverId] [int] NOT NULL,
                //[Timestamp] [datetime] NOT NULL,
                //[Severity] [tinyint] NOT NULL,
                //[ExtendedData] [varchar](50) NOT NULL,

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
                            HoursOfServiceNotificationParams p = new HoursOfServiceNotificationParams();
                            p.DriverId = Convert.ToInt32(dt.Rows[i]["DriverId"]);
                            p.Timestamp = Convert.ToDateTime(dt.Rows[i]["Timestamp"]);
                            p.Severity = (Enums.HosNotificationSeverity)Convert.ToInt32(dt.Rows[i]["Severity"]);
                            p.ExtendedData = Convert.ToString(dt.Rows[i]["ExtendedData"]);
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
        /// Retrieves list of HoursOfServiceNofication for driverID for specified date range
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>List of HoursOfServiceParams</returns>
        public SortedList<DateTime, HoursOfServiceNotificationParams> GetHoursOfServiceNotificationsByDriverIdAndDateRange(int driverId, DateTime from, DateTime to)
        {
            SortedList<DateTime, HoursOfServiceNotificationParams> hosRecords = new SortedList<DateTime, HoursOfServiceNotificationParams>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT HoursOfServiceNotificationId, DriverId, Timestamp, Severity, ExtendedData  FROM {0} WHERE DriverId=@DriverId AND (Timestamp BETWEEN @From AND @To)", tableName);

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
                            HoursOfServiceNotificationParams p = new HoursOfServiceNotificationParams();
                            p.DriverId = Convert.ToInt32(dt.Rows[i]["DriverId"]);
                            p.Timestamp = Convert.ToDateTime(dt.Rows[i]["Timestamp"]);
                            p.Severity = (Enums.HosNotificationSeverity)Convert.ToInt32(dt.Rows[i]["Severity"]);
                            p.ExtendedData = Convert.ToString(dt.Rows[i]["ExtendedData"]);

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
        /// Retrieves list of HOS Notifications based on DateTime and Driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public DataSet GetHOSnotificationsByDateTime(int driverId, int userId,  DateTime from, DateTime to)
        {

            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = "sp_GetHOSnotifications";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@DriverId", SqlDbType.Int, driverId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                ds = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve HosNotificationsRecords for Driver:{0}.", driverId);
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

    }




    /// <summary>
    /// HoursOfServiceNotification record structure
    /// </summary>
    public struct HoursOfServiceNotificationParams
    {

        //[HoursOfServiceNotificationId] [int] NOT NULL,
        //[DriverId] [int] NOT NULL,
        //[Timestamp] [datetime] NOT NULL,
        //[Severity] [tinyint] NOT NULL,
        //[ExtendedData] [varchar](50) NOT NULL,
        
        /// <summary>
        /// id
        /// </summary>
        public int Id;

        /// <summary>
        /// DriverId id
        /// </summary>
        public int DriverId;

        /// <summary>
        /// DateTime when notification issuance occurred
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// Severity level of notification
        /// </summary>
        public Enums.HosNotificationSeverity Severity;


        /// <summary>
        /// Description / Information as to the cause of the notification and possible mitigation instructions
        /// </summary>
        public string ExtendedData;
    }



}
