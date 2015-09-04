using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using System.Text;


namespace VLF.DAS.DB
{
   public class TableGenInterfaces
   {
      /// <summary>
      /// Table name for datbase class
      /// </summary>
      protected string tableName;

      /// <summary>
      /// Instance of SQL executer
      /// </summary>
      protected SQLExecuter sqlExec;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="tableName"></param>
      /// <param name="sqlExec"></param>
      protected TableGenInterfaces(string tableName, SQLExecuter sqlExec)
      {
         this.tableName = tableName;
         this.sqlExec = sqlExec;
      }

      /// <summary>
      /// Deletes all rows by field name.
      /// </summary>
      /// <remarks>Useful only to char type fields</remarks>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRowsStrField(string fieldName, object fieldValue, string msgPostfix)
      {
         return DeleteRowsByField(fieldName, fieldValue, msgPostfix);
      }

      public int DeleteRowsByField(string fieldName, object fieldValue, string msgPostfix)
      {
         int rowsAffected = 0;
         // 1. Prepares SQL statement
         string sql = "DELETE FROM " + tableName + " WHERE " + SqlUtil.PairValue(fieldName, fieldValue);
         string prefixMsg = "Cannot delete '" + SqlUtil.PairValue(fieldName, fieldValue) + "' " + msgPostfix;
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Deletes all rows by sql string.
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="prefixMsg"></param>
      /// <param name="msgPostfix"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRowsBySql(string sql, string prefixMsg, string msgPostfix)
      {
         int rowsAffected = 0;
         try
         {
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }

         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Deletes all rows by id field.
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <remarks>Useful for all int related fields</remarks>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRowsByIntField(string fieldName, Int64 fieldValue, string msgPostfix)
      {
         return DeleteRowsByField(fieldName, fieldValue, msgPostfix);
      }

      /// <summary>
      /// Delete all rows.
      /// </summary>
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public virtual int DeleteAllRecords()
      {
         int rowsAffected = 0;
         //Prepares SQL statement
         string sql = "DELETE FROM " + this.tableName;
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               sqlExec.AttachToTransaction(sql);
            }
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Cannot delete information from '" + tableName + "' table.";
            Util.ProcessDbException(prefixMsg, objException);
         }

         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Cannot delete information from '" + tableName + "' table.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      public SqlDataReader GetDataReader(string sql)
      {
         string prefixMsg = string.Format("GetDataReader for {0} from {1}", sql, tableName );
         try
         {
            // 1. Prepares SQL statement
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            return sqlExec.SQLExecuteGetDataReader(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return null;
        
      }

      public SqlDataReader GetRow(string tableName,
                                  string columnName, object value)
      {
         string cmd = string.Format("SELECT * FROM {0} WHERE {1}", tableName, SqlUtil.PairValue(columnName, value));
         return sqlExec.SQLExecuteGetDataReader(cmd);
      }

      public SqlDataReader GetRowCondition(string tableName,
                                  string where)
      {
         string cmd = string.Format("SELECT * FROM {0} WHERE {1}", tableName, where);
         return sqlExec.SQLExecuteGetDataReader(cmd);
      }

      public SqlDataReader GetRow(string tableName,
                                  SqlParameter[] parameters)
      {
         string cmd = string.Format("SELECT * FROM {0} WHERE {1}", tableName, SqlUtil.PairValueAnd(parameters));
         return sqlExec.SQLExecuteGetDataReader(cmd);
      }
      /// <summary>
      /// Retrieves all records from specific table
      /// </summary>
      /// <returns>dataset</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllRecords()
      {
         return GetAllRecords(this.tableName);
      }

      /// <summary>
      /// Retrieves all records from specific table
      /// </summary>
      /// <returns>dataset</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllRecords(string tblName)
      {
         DataSet resultDataSet = null;
         try
         {
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset("SELECT * FROM " + tblName);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Cannot retrieve information from '" + tableName + "' table.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Cannot retrieve information from '" + tableName + "' table.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      public object GetExecuteScalarCondition(string sql, string prefixMsg, string fieldName, object value)
      {
         object maxValue = null;
         try
         {
            // 1. Prepares SQL statement
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql + " WHEN " + SqlUtil.PairValue(fieldName, value));
            }
            // 3. Executes SQL statement
            maxValue = sqlExec.SQLExecuteScalar(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return maxValue;
      }


      public object GetExecuteScalar(string sql, string prefixMsg)
      {
         object maxValue = null;
         try
         {
            // 1. Prepares SQL statement
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            maxValue = sqlExec.SQLExecuteScalar(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return maxValue;
      }

      /// <summary>
      /// Retrieves record count from a table
      /// </summary>
      /// <returns>total number of record in the table</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetRecordCount()
      {
         return GetRecordCount(this.tableName);
      }

      /// <summary>
      /// Retrieves record count from a table
      /// </summary>
      /// <returns>total number of record in the table</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetRecordCount(string tblName)
      {
         return Convert.ToInt32(GetExecuteScalar("SELECT COUNT(*) FROM " + tblName, 
                                                   "Cannot retrieve record count from " + tableName));
      }

      /// <summary>
      /// Retrieves max field value from a table
      /// </summary>
      /// <remarks>added 2007-05-25 Max</remarks>
      /// <param name="fieldName">Field Name</param>
      /// <returns>Max field value object - can be casted later to any type</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public object GetMaxValue(string fieldName)
      {
         string sql = string.Format("SELECT MAX({0}) FROM {1}", fieldName, tableName);
         string msg = string.Format("Cannot retrieve max field={0} from {1}", fieldName, tableName );
         return GetExecuteScalar(sql,msg);
      }


      /// <summary>
      /// Retrieves max field value from a table for entire organization
      /// Table must have OrganizationId field: vlfDriver, vlfFleet, vlfUser, vlfVehicle...
      /// </summary>
      /// <remarks>added 2007-05-29 Max</remarks>
      /// <param name="fieldName">Field Name</param>
      /// <param name="orgId">Organization Id</param>
      /// <returns>Max field value object - can be casted later to any type</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public object GetMaxValueByOrganization(string fieldName, int orgId)
      {
         string prefixMsg = string.Format("Cannot retrieve max field={0} value from table={1}", fieldName, tableName);
         return GetExecuteScalarCondition(fieldName, prefixMsg, "OrganizationId", orgId);
      }

      # region Get rows methods

      public DataSet GetRowsByField(string tblName, string fieldName, object fieldValue, string msgPostfix)
      {
         DataSet dsResult = new DataSet();
         // 1. Prepares SQL statement
         string sql = String.Format("SELECT * FROM {0} WHERE {1}",
                                    tblName,
                                    SqlUtil.PairValue(fieldName, fieldValue));
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            dsResult = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Cannot get '" + fieldValue + "' " + msgPostfix;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Cannot get '" + fieldValue + "' " + msgPostfix;
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }
      /// <summary>
      /// Get rows by condition
      /// </summary>
      /// <remarks>added 03/08/2007 Max</remarks>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByStringField(string fieldName, string fieldValue, string msgPostfix)
      {
         return GetRowsByField(this.tableName, fieldName, fieldValue, msgPostfix);
      }

      /// <summary>
      /// Get rows by condition
      /// </summary>
      /// <remarks>added 03/08/2007 Max</remarks>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByStringField(string tblName, string fieldName, string fieldValue, string msgPostfix)
      {
         return GetRowsByField(tblName, fieldName, fieldValue, msgPostfix);
      }
      /// <summary>
      /// Get rows by condition
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByIntField(string fieldName, int fieldValue, string msgPostfix)
      {
         return GetRowsByField(this.tableName, fieldName, fieldValue, msgPostfix);
      }

      /// <summary>
      /// Get rows by condition
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByIntField(string tblName, string fieldName, int fieldValue, string msgPostfix)
      {
         return GetRowsByField(tblName, fieldName, fieldValue, msgPostfix);
      }

      # endregion

      #region Update queries

     
      #endregion Update queries

      # region Masked Id
      /*
       * Author:      Max
       * Date:        2007-05-25
       * Description: Creation of unique Id for DB split and replication
       *              High 2 bytes have an Org. Id, low 2 bytes have a class Id (driver, fleet, user, vehicle)
       *              Max number: 32767 for signed and 65535 for unsigned
       *              Static methods are available for any DasDb class inheriting table interface
      */

      /// <summary>
      /// Creates id based on a bit mask, must be used for the 1st row only, later an id field can be incremented
      /// </summary>
      /// <returns>New Id</returns>
      public static uint CreateMaskId(ushort orgId, ushort classId)
      {
         uint result = 0;
         result = (uint)(orgId << 0x10) | classId;
         return result;
      }

      /// <summary>
      /// Extract low 2 bytes Id
      /// </summary>
      /// <returns>Class Id</returns>
      public static ushort ExtractLowId(uint maskedId)
      {
         return (ushort)(maskedId & 0xffff);
      }

      /// <summary>
      /// Extract high 2 bytes Id
      /// </summary>
      /// <returns>Class Id</returns>
      public static ushort ExtractHighId(uint maskedId)
      {
         return (ushort)(maskedId >> 0x10);
      }

      /// <summary>
      /// Creates id based on a bit mask
      /// </summary>
      /// <returns>New Id</returns>
      public static int CreateMaskId(short orgId, short classId)
      {
         int result = 0;
         result = (orgId << 0x10) | classId;
         return result;
      }

      /// <summary>
      /// Extract low 2 bytes Id
      /// </summary>
      /// <returns>Class Id</returns>
      public static short ExtractLowId(int maskedId)
      {
         return (short)(maskedId & 0xffff);
      }

      /// <summary>
      /// Extract high 2 bytes Id
      /// </summary>
      /// <returns>Class Id</returns>
      public static short ExtractHighId(int maskedId)
      {
         return (short)(maskedId >> 0x10);
      }

      # endregion

      # region Row methods
      /*
       * Author:      Max
       * Date:        2007-06-27
       * Description: Unique interface for add, get and update rows in a DB table
       *              Column names and values must match the table columns
      */

      /// <summary>
      /// Add a new row
      /// (Column1, ..., ColumnN) VALUES(@param1, ..., @paramN)
      /// </summary>
      /// <param name="sql">SQL statement</param>
      /// <param name="paramArray">SQL parameters array</param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <returns>Rows affected</returns>
      public int AddRow(string sql, SqlParameter[] paramArray)
      {
         return AddRow(sql, this.tableName, paramArray);
      }

      /// <summary>
      /// Add Row
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="table"></param>
      /// <param name="paramArray"></param>
      /// <returns></returns>
      public int AddRow(string sql, string table, SqlParameter[] paramArray)
      {
         sqlExec.ClearCommandParameters();
         if (paramArray != null)
            sqlExec.AddCommandParameters(paramArray);

         if (sqlExec.RequiredTransaction())
         {
            // 2. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         //Executes SQL statement
         return sqlExec.SQLExecuteNonQuery("INSERT " + table + sql);
      }

      /// <summary>
      /// Update row
      /// SET Column1 = @param1, ..., ColumnN = @paramN WHERE ColumnX = @paramX AND ColumnY = @paramY
      /// </summary>
      /// <param name="sql">SQL statement</param>
      /// <param name="paramArray">SQL parameters array</param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if person does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <returns>Rows affected</returns>
      public int UpdateRow(string sql, SqlParameter[] paramArray)
      {
         return UpdateRow(sql, this.tableName, paramArray);
      }

      /// <summary>
      /// Update row
      /// SET Column1 = @param1, ..., ColumnN = @paramN WHERE ColumnX = @paramX AND ColumnY = @paramY
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="table"></param>
      /// <param name="paramArray"></param>
      /// <returns></returns>
      public int UpdateRow(string sql, string table, SqlParameter[] paramArray)
      {
         sqlExec.ClearCommandParameters();
         if (paramArray != null)
            sqlExec.AddCommandParameters(paramArray);
         if (sqlExec.RequiredTransaction())
         {
            // 2. Attach current command SQL to transaction
            sqlExec.AttachToTransaction(sql);
         }
         //Executes SQL statement
         return sqlExec.SQLExecuteNonQuery("UPDATE " + table + " " + sql);
      }

      /// <summary>
      /// Get rows by filter
      /// WHERE Column1 = @param1 ... AND ColumnN = @paramN
      /// </summary>
      /// <param name="filter"></param>
      /// <param name="paramArray"></param>
      /// <returns>DataSet</returns>
      public DataSet GetRowsByFilter(string filter, SqlParameter[] paramArray)
      {
         return GetRowsByFilter(this.tableName, filter, paramArray);
      }

      /// <summary>
      /// Get rows by filter
      /// WHERE Column1 = @param1 ... AND ColumnN = @paramN
      /// </summary>
      /// <param name="filter"></param>
      /// <param name="paramArray"></param>
      /// <returns>DataSet</returns>
      public DataSet GetRowsByFilter(string tblName, string filter, SqlParameter[] paramArray)
      {
         DataSet dsResult = new DataSet();
         string prefixMsg = "";
         try
         {
            prefixMsg = String.Format("Cannot get rows by filter: [{0}]", filter);
            // 1. Prepares SQL statement

            sqlExec.ClearCommandParameters();
            if (paramArray != null)
               sqlExec.AddCommandParameters(paramArray);

            string sql = "SELECT * FROM " + tblName + " " + filter;

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            //Executes SQL statement
            dsResult = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      /// <summary>
      /// Get rows by sql statement
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="paramArray"></param>
      /// <returns>DataSet</returns>
      public DataSet GetRowsBySql(string sql, SqlParameter[] paramArray)
      {
         DataSet dsResult = new DataSet();
         string prefixMsg = "";
         try
         {
            prefixMsg = String.Format("Cannot get rows by sql: [{0}]", sql);
            // 1. Prepares SQL statement

            sqlExec.ClearCommandParameters();
            if (paramArray != null)
               sqlExec.AddCommandParameters(paramArray);

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            //Executes SQL statement
            dsResult = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      # endregion
   }
}
