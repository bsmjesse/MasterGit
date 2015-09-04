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
    /// Provides interfaces to vlfMdtOTA table.
    /// </summary>
    public class MdtOTA : TblOneIntPrimaryKey
    {
        const string tablename = "vlfMdtOTA";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public MdtOTA(SQLExecuter sqlExec) : base("vlfMdtOTA", sqlExec) { }

        /// <summary>
        /// Add new MdtOTA process record
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="customProp"></param>
        public void AddMdtOTA(int boxId, Int16 typeId, string customProp) { AddMdtOTA(boxId, typeId, new Json(customProp)); }


        /// <summary>
        /// Add new MdtOTA process record
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="customProp"></param>
        public void AddMdtOTA(int boxId, Int16 typeId, Json customProp)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("INSERT INTO {0}(BoxId,TypeId, CustomProp, RequestDateTime,LastCommunicated) VALUES ( @BoxId, @TypeId, @CustomProp, @RequestDateTime,@LastCommunicated )", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@TypeId", SqlDbType.SmallInt, typeId);
                sqlExec.AddCommandParam("@CustomProp", SqlDbType.VarChar, customProp.ToString());
                sqlExec.AddCommandParam("@RequestDateTime", SqlDbType.DateTime, DateTime.UtcNow);
                sqlExec.AddCommandParam("@LastCommunicated", SqlDbType.DateTime, DateTime.UtcNow);

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
                string prefixMsg = string.Format("Unable to add new '{0}' MdtOTA.", boxId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' MdtOTA.", boxId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to add new '{0}' MdtOTA.", boxId);
                throw new DASAppDataAlreadyExistsException(string.Format("{0}, This box already exists.", prefixMsg));
            }
        }

        /// <summary>
        /// Remove existing MdtOTA record process by token
        /// </summary>
        /// <param name="token"></param>
        public void RemoveMdtOTA(string token)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("DELETE FROM {0} WHERE Token = @Token", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Token", SqlDbType.VarChar, token);

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
                string prefixMsg = string.Format("Unable to remove '{0}' MdtOTA.", token);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' MdtOTA.", token);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to remove '{0}' MdtOTA.", token);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }

        }

        /// <summary>
        /// Delete existing MdtOTA processes.
        /// Throws exception in case of wrong result (see TblOneIntPrimaryKey class).
        /// </summary>
        /// <param name="boxId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int RemoveAllMdtOTAByBoxId(int boxId)
        {
            return DeleteRowsByIntField("BoxId", boxId, "box");
        }

        /// <summary>
        /// Get MdtOTA process status
        /// </summary>
        /// <param name="processId"></param>
        /// <returns>status percentage</returns>
        public int GetMdtOTAStatus(int processId)
        {
            int percent = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT Status FROM {0} WHERE MdtOtaId = @ProcessId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ProcessId", SqlDbType.Int, processId);

                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 4. Executes SQL statement
                percent = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve '{0}' MdtOTA status.", processId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve '{0}' MdtOTA status.", processId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return percent;

        }



        /// <summary>
        /// Get MdtOTA process status
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="typeId"></param>
        /// <returns>status percentage</returns>
        public DataSet GetMdtsByFleetId(int fleetId, Int16 typeId)
        {
            DataSet dsResult = new DataSet();
            try
            {

                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@typeId", typeId);
                dsResult = sqlExec.SPExecuteDataset("sp_GetMdtsByFleetId", sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve '{0}' Mdts by fleetid.", fleetId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve '{0}' Mdts by fleetid.", fleetId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return dsResult;

        }

        /// <summary>
        /// Retrieves list of MdtOTAParmas for new MdtOTA processes
        /// </summary>
        /// <returns>List of MdtOTAParams</returns>
        public SortedList<long, MdtOTAParams> GetNewMdtOTAProcesses()
        {
            SortedList<long, MdtOTAParams> processes = new SortedList<long, MdtOTAParams>();
            DataSet ds = null;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT MdtOtaId, BoxId, CustomProp, RequestDateTime FROM {0} WHERE Status=-1", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();

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
                            MdtOTAParams p = new MdtOTAParams();
                            p.ProcessIdentifier = Convert.ToInt32(dt.Rows[i]["MdtOtaId"]);
                            p.BoxIdentifier = Convert.ToInt32(dt.Rows[i]["BoxId"]);
                            p.CustomProp = new Json(Convert.ToString(dt.Rows[i]["CustomProp"]));
                            DateTime timestamp = Convert.ToDateTime(dt.Rows[i]["RequestDateTime"]);
                            long ticks = timestamp.Ticks;
                            while (processes.ContainsKey(ticks))
                                ticks++;

                            processes.Add(ticks, p);
                        }
                    }
                }
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve new MdtOTA processes.");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve new MdtOTA processes.");
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return processes;

        }

        /// <summary>
        /// Cleans vlfMdtOTA table of all stale process not completed within 1 hour...
        /// </summary>
        /// <returns>number of stale records cleaned</returns>
        public int CleanMdtOTATable()
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = "UPDATE vlfMdtOTA SET Status = -100 WHERE (Status BETWEEN 0 AND 99) AND (DATEDIFF(hour, LastCommunicated, GETDATE()) > 1)";

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();

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
                string prefixMsg = "Unable to clean MdtOTA";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to clean MdtOTA";
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            return rowsAffected;
        }

        /// <summary>
        /// Update status of MdtOTA process
        /// </summary>
        /// <param name="token"></param>
        /// <param name="percent"></param>
        /// <param name="bytesIn"></param>
        /// <param name="bytesOut"></param>
        public void UpdateMdtOTAStatus(string token, int percent)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("UPDATE {0} SET Status = @Status, LastCommunicated = @LastCommunicated WHERE Token = @Token", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@Token", SqlDbType.VarChar, token);
                sqlExec.AddCommandParam("@Status", SqlDbType.Int, percent);
                sqlExec.AddCommandParam("@LastCommunicated", SqlDbType.DateTime, DateTime.UtcNow);


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
                string prefixMsg = string.Format("Unable to update '{0}' MdtOTA status.", token);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtOTA status.", token);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtOTA status.", token);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }
        }


        /// <summary>
        /// Update status of MdtOTA process
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="percent"></param>
        /// <param name="bytesIn"></param>
        /// <param name="bytesOut"></param>
        public void UpdateMdtOTAStatus(int processId, int percent)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("UPDATE {0} SET Status = @Status, LastCommunicated = @LastCommunicated WHERE MdtOtaId = @ProcessId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ProcessId", SqlDbType.Int, processId);
                sqlExec.AddCommandParam("@Status", SqlDbType.Int, percent);
                sqlExec.AddCommandParam("@LastCommunicated", SqlDbType.DateTime, DateTime.UtcNow);


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
                string prefixMsg = string.Format("Unable to update '{0}' MdtOTA status.", processId);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtOTA status.", processId);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to update '{0}' MdtOTA status.", processId);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by token not found.", prefixMsg));
            }
        }

        /// <summary>
        /// Update the MdtOTA token field
        /// 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="token"></param>
        public void UpdateMdtOTAToken(int processId, string token)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Set SQL command
                string sql = string.Format("UPDATE {0} SET Token = @Token, Status = 0 WHERE MdtOtaId = @ProcessId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ProcessId", SqlDbType.Int, processId);
                sqlExec.AddCommandParam("@Token", SqlDbType.VarChar, token);

                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to processId '{0}' MdtOTA token '{1}'.", processId, token);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to processId '{0}' MdtOTA token '{1}'.", processId, token);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = string.Format("Unable to processId '{0}' MdtOTA token '{1}'.", processId, token);
                throw new DASAppResultNotFoundException(string.Format("{0}, record specified by ProcessId not found.", prefixMsg));
            }
        }
    }



    public struct MdtOTAParams
    {
        /// <summary>
        /// processIdentifier
        /// </summary>
        public int ProcessIdentifier;

        /// <summary>
        /// BoxIdentifier
        /// </summary>
        public int BoxIdentifier;

        /// <summary>
        /// CustomProp
        /// </summary>
        public Json CustomProp;
    }



}
