using System;
using System.Collections;	      // for SortedList
using System.Collections.Generic;
using System.Data;			      // for DataSet
using System.Data.SqlClient;	   // for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
//using VLF.CLS.Def;
//using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to Work Orders functionality in database
    /// </summary>
    public class WorkOrders : Das
    {
        private DB.WorkOrders iWorkOrders = null;

        #region General Interfaces
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public WorkOrders(string connectionString)
            : base(connectionString)
        {
            iWorkOrders = new DB.WorkOrders(sqlExec);
        }

        /// <summary>
        /// Distructor
        /// </summary>
        public new void Dispose()
        {
            // forcing collection
            base.Dispose();
        }
        #endregion

        #region WorkOrders Configuration Interfaces
        /// <summary>
        /// Add new Job.
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="UserId"></param>
        /// <param name="VehicleId"></param>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <param name="Location"></param>
        /// <param name="Status"></param>
        /// <param name="Envelop"></param>
        /// <param name="UpdatedDateTime"></param>
        /// <param name="ScheduledDateTime"></param>
        /// <param name="Notes"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if Job already exists.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddJob(long JobId, int UserId, long VehicleId, double Latitude, double Longitude, string Location, short Status, string Envelop, DateTime ScheduledDateTime, string Notes, int ServiceId)
        {
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new box
                iWorkOrders.AddJob(JobId, UserId, VehicleId, Latitude, Longitude, Location, Status, Envelop, ScheduledDateTime, Notes,ServiceId);

                // 2. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 3. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new Job ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 5. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }
        #endregion
    }
}
