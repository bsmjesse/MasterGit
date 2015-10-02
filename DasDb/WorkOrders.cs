using System;
using System.Collections.Generic;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using System.Text;

namespace VLF.DAS.DB
{
    /// <summary>
    /// Provides interfaces to vlfWorkOrders table/entity.
    /// </summary>
    /// <comment> </comment>
    public class WorkOrders : TblOneIntPrimaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public WorkOrders(SQLExecuter sqlExec)
            : base("vlfWorkOrders", sqlExec)
        {
        }

        public void AddJob(long JobId, int UserId, long VehicleId, double Latitude, double Longitude, string Location, short Status, string Envelop, DateTime ScheduledDateTime, string Notes, int ServiceId)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command and add parameters to SQL statement
                string sql = "insert_update_work_orders_t ";
                sql += "'" + JobId + "','" + UserId + "','" + VehicleId + "','" + Latitude + "','" + Longitude + "','" + Location.Replace("'", "''") + "','" +
                    Status + "','" + Envelop.Replace("'", "''") + "','" + ScheduledDateTime + "','" + Notes.Replace("'", "''") + "','" + ServiceId + "'";
                //Status + "','" + Envelop.Replace("'", "''") + "','" + UpdatedDateTime + "','" + ScheduledDateTime + "','" + Notes.Replace("'", "''") + "','" + ServiceId + "'";

                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new '" + JobId + "' Job.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new '" + JobId + "' Job.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new '" + JobId + "' Job.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This Job already exists.");
            }
        }
    }
}