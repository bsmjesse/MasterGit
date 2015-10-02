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
    /// Provides interfaces to vlfPeripheralBoxAssignment table.
    /// </summary>
    public class PeripheralBoxAssignment : TblOneIntPrimaryKey
    {
        const string tablename = "vlfPeripheralBoxAssignment";

        /*
        BoxId	int	
        PeripheralId	nvarchar(50)	
        TypeId	int	        
         * * */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public PeripheralBoxAssignment(SQLExecuter sqlExec) : base("vlfPeripheralBoxAssignment", sqlExec) { }




        /// <summary>
        /// new PeripheralBoxAssignment
        /// </summary>
        /// <param name="boxIdentifier"></param>
        /// <param name="peripheralIdentifier"></param>
        /// <param name="peripheralType"></param>
        public void AssignPeripheralToBox(int boxIdentifier, int peripheralIdentifier, int peripheralType)
        {
            int rowsAffected = 0;
            try
            {
                /*
                  here we go....
                    1) Using a transaction; check to see if the current assignment exists
                            if current assignment does exist do nothing  
                            else
                                a) remove the current peripheral to box assignment
                                b) remove the current box to peripheral assignment
                                c) insert the new box assignment record
                                d) insert the new box assignment record into PeripheralBoxAssigntmentHst
                */

                // 1. Check if record exists...

                if (!CheckForPeripheralBoxAssignment(boxIdentifier, peripheralIdentifier, peripheralType))
                {

                    // if no...then...

                    // 2. Start Transaction
                    sqlExec.BeginTransaction();

                    // 3. Remove Box / Peripheral Type record
                    string sql = string.Format("DELETE FROM {0} WHERE BoxId = @BoxId AND TypeId = @TypeId", tableName);
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxIdentifier);
                    sqlExec.AddCommandParam("@TypeId", SqlDbType.Int, peripheralType);
                    if (sqlExec.RequiredTransaction())
                    {
                        // 4. Attach current command SQL to transaction
                        sqlExec.AttachToTransaction(sql);
                    }
                    rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

                    // 5. Remove Box / Peripheral record
                    sql = string.Format("DELETE FROM {0} WHERE BoxId = @BoxId AND PeripheralId = @PeripheralId", tableName);
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxIdentifier);
                    sqlExec.AddCommandParam("@PeripheralId", SqlDbType.Int, peripheralIdentifier);
                    if (sqlExec.RequiredTransaction())
                    {
                        // 6. Attach current command SQL to transaction
                        sqlExec.AttachToTransaction(sql);
                    }
                    rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

                    // 6. Remove Box / Peripheral record
                    sql = string.Format("INSERT INTO {0} ( BoxId, PeripheralId, TypeId) VALUES ( @BoxId, @PeripheralId, @TypeId)", tableName);
                    sqlExec.ClearCommandParameters();
                    sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxIdentifier);
                    sqlExec.AddCommandParam("@PeripheralId", SqlDbType.Int, peripheralIdentifier);
                    sqlExec.AddCommandParam("@TypeId", SqlDbType.Int, peripheralType);
                    if (sqlExec.RequiredTransaction())
                    {
                        // 7. Attach current command SQL to transaction
                        sqlExec.AttachToTransaction(sql);
                    }
                    rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

                    sqlExec.CommitTransaction();

                }
            }
            catch (SqlException objException)
            {
                sqlExec.RollbackTransaction();
                string prefixMsg = string.Format(""); //Unable to add new PeripheralBoxAssignment with serial number '{0}'.", serialNum);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                sqlExec.RollbackTransaction();
                string prefixMsg = string.Format(""); //Unable to add new PeripheralBoxAssignment with serial number '{0}'.", serialNum);
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }
        }


        public bool CheckForPeripheralBoxAssignment(int boxIdentifier, int peripheralIdentifier, int peripheralType)
        {
            try
            {
                // 1. Set SQL command
                string sql = string.Format("SELECT  COUNT(*) FROM vlfPeripheralBoxAssigment WHERE BoxId = @BoxId AND PeripheralId = @PeripheralId AND TypeId = @TypeId", tableName);

                // 2. Add parameters to SQL statement
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxIdentifier);
                sqlExec.AddCommandParam("@PeripheralId", SqlDbType.Int, peripheralIdentifier);
                sqlExec.AddCommandParam("@TypeId", SqlDbType.Int, peripheralType);

                return (int)sqlExec.SQLExecuteScalar(sql) == 1;

            }
            catch (SqlException objException)
            {
                string prefixMsg = string.Format("Unable to retrieve PeripheralBoxAssignment count.");
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = string.Format("Unable to retrieve PeripheralBoxAssignment count.");
                throw new DASException(string.Format("{0} {1}", prefixMsg, objException.Message));
            }

            return false;
        }

    }

    public class PeripheralBoxAssignmentClass
    {
        /// <summary>
        /// BoxIdentifier
        /// </summary>
        public int BoxIdentifier;
        /// <summary>
        /// PeripheralIdentifier
        /// </summary>
        public int PeripheralIdentifier;
        /// <summary>
        /// PeripheralType
        /// </summary>
        public int PeripheralType;
    }





}
