using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using System.Text;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Base class for all database classes.
	/// </summary>
	public class TblGenInterfaces
	{
		/// <summary>
		/// Table name for datbase class
		/// </summary>
		protected string		   tableName;

		/// <summary>
		/// Instance of SQL executer
		/// </summary>
		protected SQLExecuter	sqlExec;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="sqlExec"></param>
		protected TblGenInterfaces(string tableName, SQLExecuter sqlExec)
		{
			this.tableName	= tableName;
			this.sqlExec	= sqlExec;
		}

      /// <summary>
      ///         Constructor for generic access to tables
      /// </summary>
      /// <param name="connectionString"></param>
      /// <param name="tableName"></param>
      public TblGenInterfaces(string connectionString, string tableName)
      {
         this.tableName = tableName;
         sqlExec = new SQLExecuter(connectionString);
      }

      public string TableName
      {
         get { return tableName; }
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
      public int DeleteRowsByStrField(string fieldName, string fieldValue, string msgPostfix)
		{
         return DeleteRowsByField(fieldName, fieldValue.Trim().Replace("'","''"));
         /*
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + " WHERE " + fieldName + "='" + fieldValue.Trim().Replace("'","''") + "'";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot delete '" + fieldValue + "' " + msgPostfix;
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
                string prefixMsg = "Cannot delete '" + fieldValue + "' " + msgPostfix;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
         */
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

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
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
         return DeleteRowsByField(fieldName, fieldValue);
         /*
			int rowsAffected = 0;
         string prefixMsg = "";
			// 1. Prepares SQL statement
			string sql = String.Format("DELETE FROM {0} WHERE {1} = @param", tableName, fieldName);
			try
			{
            prefixMsg = String.Format("Cannot delete by {0}={1}\nDetails:", msgPostfix, fieldValue);
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam(new SqlParameter("@param", fieldValue));
				if(sqlExec.RequiredTransaction())
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
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
         */
		}

      /// <summary>
      /// Deletes all rows by id field.
      /// </summary>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <remarks>Useful for all fields</remarks>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRowsByField(string fieldName, object fieldValue)
      {
         int rowsAffected = 0;
         string prefixMsg = "";
         // 1. Prepares SQL statement
         string sql = "";
         try
         {
            prefixMsg = 
               String.Format("Cannot delete from {0} by {1} = {2}", this.tableName, fieldName, fieldValue);

            sqlExec.ClearCommandParameters();
            if (fieldValue == null)
               sql = String.Format("DELETE FROM {0} WHERE {1} IS NULL", this.tableName, fieldName);
            else
            {
               sql = String.Format("DELETE FROM {0} WHERE {1} = @param", this.tableName, fieldName);
               sqlExec.AddCommandParameters(new SqlParameter("@param", fieldValue));
            }
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
            throw new DASException(prefixMsg + Environment.NewLine + objException.Message);
         }
         return rowsAffected;
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
         string prefixMsg = "", sql = "DELETE FROM " + this.tableName;
			try
			{
            prefixMsg = "Cannot delete information from '" + this.tableName + "' table.";
            if (sqlExec.RequiredTransaction())
				{
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
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
         string prefixMsg = "";
         try
         {
            prefixMsg = "Cannot retrieve information from '" + tableName + "' table.";
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset("SELECT * FROM " + tblName);
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
         return resultDataSet;
      }

      /// <summary>
      /// Retrieves record count from a table
      /// </summary>
      /// <param name="tblName">Name of a table to count records</param>
      /// <returns>total number of record in the table</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetRecordCount()
      {
         int recordCount = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Cannot retrieve record count from '" + this.tableName + "' table.";
            //Executes SQL statement
            recordCount = (int)sqlExec.SQLExecuteScalar("SELECT COUNT(*) FROM " + this.tableName);
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
         return recordCount;
      }

      /// <summary>
      /// Retrieves record count from a table using a filter:
      /// Column1 = value1, Column2 = value2...
      /// </summary>
      /// <param name="fieldName">Name of a column</param>
      /// <param name="filter">Filter string</param>
      /// <returns>total number of record in the table</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetRecordCount(string fieldName, string filter)
      {
         int recordCount = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Cannot retrieve record count from '" + this.tableName + "' table.";
            //Executes SQL statement
            if (!String.IsNullOrEmpty(fieldName) || !String.IsNullOrEmpty(filter))
               recordCount = (int)sqlExec.SQLExecuteScalar(
                  String.Format("SELECT COUNT({0}) FROM {1} WHERE {2}", fieldName, this.tableName, filter));
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
         return recordCount;
      }

      /// <summary>
      /// Retrieves record count from a table using a filter:
      /// Column1 = value1, Column2 = value2...
      /// </summary>
      /// <param name="filter">Filter string</param>
      /// <returns>total number of record in the table</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetRecordCount(string filter)
      {
         int recordCount = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Cannot retrieve record count from '" + this.tableName + "' table.";
            //Executes SQL statement
            if (!String.IsNullOrEmpty(filter))
               recordCount = (int)sqlExec.SQLExecuteScalar(
                  String.Format("SELECT COUNT(*) FROM {0} WHERE {1}", this.tableName, filter));
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
         return recordCount;
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
         object maxValue = null;
         string prefixMsg = "Cannot retrieve max field [" + fieldName + "] value from [" + tableName + "] table.";
         try
         {
            // 1. Prepares SQL statement
            string sql = String.Format("SELECT MAX({0}) FROM {1}", fieldName, tableName);
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
         object maxValue = null;
         string prefixMsg = "Cannot retrieve max field [" + fieldName + "] value from [" + tableName + "] table.";
         try
         {
            // 1. Prepares SQL statement
            string sql = String.Format("SELECT MAX({0}) FROM {1} WHERE OrganizationId=@orgId", fieldName, this.tableName);
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
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

      # region Get rows methods

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
         return GetRowsByStringField(this.tableName, fieldName, fieldValue, msgPostfix);
      }

      /// <summary>
      /// Get rows by condition
      /// </summary>
      /// <remarks>added 03/08/2007 Max</remarks>
      /// <param name="tblName"></param>
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByStringField(string tblName, string fieldName, string fieldValue, string msgPostfix)
      {
         DataSet dsResult = new DataSet();
         // 1. Prepares SQL statement
         string sql = String.Format("SELECT * FROM {0} WHERE {1}='{2}'", tblName, fieldName, fieldValue.Trim().Replace("'", "''"));
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
      /// <param name="fieldName"></param>
      /// <param name="fieldValue"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByIntField(string fieldName, int fieldValue, string msgPostfix)
      {
         return GetRowsByIntField(this.tableName, fieldName, fieldValue, msgPostfix);
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
         DataSet dsResult = new DataSet();
         // 1. Prepares SQL statement
         string sql = String.Format("SELECT * FROM {0} WHERE {1}={2}", tblName, fieldName, fieldValue);
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
            string prefixMsg = "Cannot get '" + fieldValue.ToString() + "' " + msgPostfix;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Cannot get '" + fieldValue.ToString() + "' " + msgPostfix;
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return dsResult;
      }

      /// <summary>
      ///      this function returns the checksum for a query
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      public long GetTableSignature(string tblName, string filter)
      {
         string prefixMsg = string.Format("Cannot get signature for table [{0}] filter[{1}]", tblName, filter);

         try
         {
            
            // 1. Prepares SQL statement

            sqlExec.ClearCommandParameters();


            string sql = "SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM " + tblName + " " + filter;
          
            //Executes SQL statement
            object obj = sqlExec.SQLExecuteScalar(sql);
            if (obj != System.DBNull.Value)
               return Convert.ToInt64(obj);
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

         throw new ArgumentNullException(prefixMsg);
         

      }
      # endregion

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
      public int AddRow(string sql, params SqlParameter[] paramArray)
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
      public int AddRow(string sql, string table, params SqlParameter[] paramArray)
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
      public int UpdateRow(string sql, params SqlParameter[] paramArray)
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
      public int UpdateRow(string sql, string table, params SqlParameter[] paramArray)
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
      public DataSet GetRowsByFilter(string filter, params SqlParameter[] paramArray)
      {
         return GetRowsByFilter(this.tableName, filter, paramArray);
      }

      /// <summary>
      /// Get rows by filter
      /// WHERE Column1 = @param1 ... AND ColumnN = @paramN
      /// </summary>
      /// <param name="tblName"></param>
      /// <param name="filter"></param>
      /// <param name="paramArray"></param>
      /// <returns>DataSet</returns>
      public DataSet GetRowsByFilter(string tblName, string filter, params SqlParameter[] paramArray)
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
      /// Get rows by filter:
      /// WHERE Column1 = @param1 ... AND ColumnN = @paramN
      /// </summary>
      /// <param name="columnList"></param>
      /// <param name="tblName"></param>
      /// <param name="filter"></param>
      /// <param name="paramArray"></param>
      /// <returns>DataSet</returns>
      public DataSet GetRowsByFilter(string columnList, string tblName, string filter, params SqlParameter[] paramArray)
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

            string sql = String.Format("SELECT {0} FROM {1} {2}", columnList, tblName, filter);

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
      /// Get rows
      /// </summary>
      /// <param name="columnList">List of columns to get</param>
      /// <returns>DataSet</returns>
      public DataSet GetRows(string columnList)
      {
         DataSet dsResult = new DataSet();
         string prefixMsg = "";
         try
         {
            prefixMsg = String.Format("Cannot get rows");
            // 1. Prepares SQL statement

            sqlExec.ClearCommandParameters();

            string sql = "SELECT " + columnList + " FROM " + this.tableName;

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
      public DataSet GetRowsBySql(string sql, params SqlParameter[] paramArray)
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

      /// <summary>
      /// Get value by filter
      /// [Column1 = @param1 ... AND ColumnN = @paramN]
      /// Created: 2008-06-12, Max
      /// </summary>
      /// <param name="columnName">Column name to get a value</param>
      /// <param name="filter">WHERE sql clause content</param>
      /// <param name="paramArray">Sql parameters array</param>
      /// <returns>Result Object - convert accordingly</returns>
      public Object GetValueByFilter(string columnName, string filter, params SqlParameter[] paramArray)
      {
         Object oResult = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = String.Format("Cannot get value by filter: [{0}]", filter);

            // 1. Prepares SQL statement
            sqlExec.ClearCommandParameters();
            if (paramArray != null)
               sqlExec.AddCommandParameters(paramArray);

            string sql = String.Format("SELECT {0} FROM {1} WHERE {2}", columnName, this.tableName, filter);

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            //Executes SQL statement
            oResult = sqlExec.SQLExecuteScalar(sql);
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
         return oResult;
      }

      # endregion
   }

	/// <summary>
	/// Provides interface to tables with one primary key
	/// </summary>
	public class TblOneIntPrimaryKey : TblGenInterfaces
	{
		/// <summary>
		/// Useful only if in the table one primary key.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="sqlExec"></param>
		protected TblOneIntPrimaryKey(string tableName, SQLExecuter sqlExec) : base (tableName, sqlExec)
		{}

		/// <summary>
		/// Retrieves record count from specific table
		/// </summary>
		/// <returns>total number of record in the table</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected int GetRecordCount(string primaryKeyFieldName)
		{
			int recordCount = 0;
         string prefixMsg = "Cannot retrieve record count from '" + tableName + "' table.";
         try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(" + primaryKeyFieldName + ") FROM " + tableName;
				//Executes SQL statement
				recordCount = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return recordCount;
		}

		/// <summary>
		/// Retrieves max record index from a table
		/// </summary>
      /// <param name="primaryKeyFieldName">Field Name</param>
		/// <returns>Max record index</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      [Obsolete("Try to avoid using this method since it gets a data set, consider using TblGenInterfaces.GetMaxValue instead")]
		protected int GetMaxRecordIndex(string primaryKeyFieldName) 
		{
			int maxRecordIndex = 0;
         string prefixMsg = "Cannot retrieve max record index from '" + tableName + "' table.";
         try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT " + primaryKeyFieldName + " FROM " + tableName + " ORDER BY " + primaryKeyFieldName + " DESC";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
				if(resultDataSet.Tables[0].Rows.Count > 0)
				{
					maxRecordIndex = Convert.ToInt32(resultDataSet.Tables[0].Rows[0][0]);
				}
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return maxRecordIndex;
		}

		/// <summary>
		/// Adds new row.
		/// Useful only for tables with specific structure:
		/// 1. field one is PK and int related type.
		/// 2. field two is UNIQUE and char related type.
		/// </summary>
		/// <param name="rowIdFieldName"></param>
		/// <param name="valueFieldName"></param>
		/// <param name="fieldValue"></param>
		/// <param name="msgPostfix"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected int AddNewRow(string rowIdFieldName, string valueFieldName, string fieldValue, string msgPostfix)
		{
			int nextRowId = GetMaxRecordIndex(rowIdFieldName);
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( " + rowIdFieldName + ", " + valueFieldName + ") VALUES ( {0}, '{1}')", ++nextRowId, fieldValue.Trim().Replace("'","''"));
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
				rowsAffected = nextRowId;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot add new '" + fieldValue + "' " + msgPostfix + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Cannot add new '" + fieldValue + "' " + msgPostfix + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Cannot add new '" + fieldValue + "' " + msgPostfix + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The " + msgPostfix + " already exists.");
			}
			return rowsAffected;
		}

        /// <summary>
		/// Returns first appearance of field value by id. 
		/// </summary>
		/// <remarks>
		/// Useful for fields with UNIQUE constrains only.
		/// Useful for string fields only.
		/// </remarks>
		/// <param name="rowIdFieldName"></param>
		/// <param name="rowId"></param>
		/// <param name="valueFieldName"></param>
		/// <param name="msgPostfix"></param>
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected object GetFieldObjValueByRowId(string rowIdFieldName, int rowId, string valueFieldName, string msgPostfix)
		{
			object resultValue = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + valueFieldName + " FROM " + tableName + " WHERE " + rowIdFieldName + "=" + rowId;
				//Executes SQL statement
				DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
				if(resultDataSet.Tables[0].Rows.Count == 1)
				{
					resultValue = resultDataSet.Tables[0].Rows[0][0];
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot retrieve " + valueFieldName + " by id=" + rowId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Cannot retrieve " + valueFieldName + " by id=" + rowId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultValue;
		}	
	}

	/// <summary>
	/// Table structure is: [int],[string]
	/// Primary key is: first field (Id in generaly)
	/// Constraints:   - string field is Unique
	/// </summary>
	public class Tbl2UniqueFields : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="sqlExec"></param>
		protected Tbl2UniqueFields(string tableName, SQLExecuter sqlExec) : base (tableName, sqlExec)
		{
		}

		/// <summary>
		/// Returns first appearance of field value by id. 
		/// </summary>
		/// <remarks>
		/// Useful for fields with UNIQUE constrains only.
		/// Useful for string fields only.
		/// </remarks>
		/// <param name="rowIdFieldName"></param>
		/// <param name="rowId"></param>
		/// <param name="valueFieldName"></param>
		/// <param name="msgPostfix"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected string GetFieldValueByRowId(string rowIdFieldName, int rowId, string valueFieldName, string msgPostfix)
		{
			string resultValue = VLF.CLS.Def.Const.unassignedStrValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + valueFieldName + " FROM " + tableName + " WHERE " + rowIdFieldName + "=" + rowId;
				//Executes SQL statement
				DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
				//Retrieves info from Table[0].[index][0] into resultValue variable if data exists
				/*foreach(DataRow currRow in resultDataSet.Tables[0].Rows)
				{
					foreach(DataColumn currCol in resultDataSet.Tables[0].Columns)
					{
						resultValue = currRow[currCol].ToString();
						++recCount;
						break;
					}
				}
				*/
				if(resultDataSet.Tables[0].Rows.Count == 1)
				{
					//Trim speces at the end of result (all char fields in the database have fixed size)
					resultValue = Convert.ToString(resultDataSet.Tables[0].Rows[0][0]).TrimEnd();
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot retrieve " + valueFieldName + " by id=" + rowId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Cannot retrieve " + valueFieldName + " by id=" + rowId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultValue;
		}	
	}

	/// <summary>
	/// Provedes interfaces for two primary keys tables.
	/// Table structure is: [int],[int],..... 
	/// Primary keys are: ([int],[int])
	/// </summary>
   public class TblTwoPrimaryKeys : TblGenInterfaces
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="tableName"></param>
      /// <param name="sqlExec"></param>
      protected TblTwoPrimaryKeys(string tableName, SQLExecuter sqlExec)
         : base(tableName, sqlExec)
      {
      }

      /// <summary>
      /// Deletes row by two primary keys.
      /// </summary>
      /// <param name="rowFieldName1"></param>
      /// <param name="rowFieldValue1"></param>
      /// <param name="rowFieldName2"></param>
      /// <param name="rowFieldValue2"></param>
      /// <param name="msgPostfix"></param>
      /// <returns>rows affected</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if data deos not exist.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteRowsByFields(string rowFieldName1, int rowFieldValue1, string rowFieldName2, int rowFieldValue2, string msgPostfix)
      {
         int rowsAffected = 0;
         //Prepares SQL statement
         string sql = "DELETE FROM " + tableName + " WHERE " + rowFieldName1 + "="
            + rowFieldValue1 + " and " + rowFieldName2 + "=" + rowFieldValue2;
         try
         {
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Cannot delete by id=" + rowFieldValue1 + ":" + rowFieldValue2 + " " + msgPostfix;
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Cannot delete by id=" + rowFieldValue1 + ":" + rowFieldValue2 + " " + msgPostfix;
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Retrieves array of int values, in case of empty result returns null.
      /// Throws DASException exception in case of error.
      /// </summary>
      /// <param name="rowSearchFieldName"></param>
      /// <param name="rowSearchFieldValue"></param>
      /// <param name="rowResultFieldName"></param>
      /// <param name="msgPostfix"></param>
      /// <returns>array [int]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int[] GetList(string rowSearchFieldName, int rowSearchFieldValue, string rowResultFieldName, string msgPostfix)
      {
         int[] resultList = null;
         DataSet sqlDataSet = null;
         int index = 0;
         //Prepares SQL statement
         try
         {
            //Prepares SQL statement
            string sql = "SELECT " + rowResultFieldName +
               " FROM " + tableName +
               " WHERE " + rowSearchFieldName + "=" + rowSearchFieldValue +
               " ORDER BY " + rowResultFieldName + " ASC";
            //Executes SQL statement
            sqlDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Cannot retrieve list of " + rowResultFieldName + " by " + rowSearchFieldName + "=" + rowSearchFieldValue + " " + msgPostfix + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Cannot retrieve list of " + rowResultFieldName + " by " + rowSearchFieldName + "=" + rowSearchFieldValue + " " + msgPostfix + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if ((sqlDataSet != null) && (sqlDataSet.Tables[0].Rows.Count > 0))
         {
            resultList = new int[sqlDataSet.Tables[0].Rows.Count];
            //Retrieves info from Table[0].[0][0]
            foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
            {
               resultList[index++] = Convert.ToInt32(currRow[0]);
            }
         }
         return resultList;
      }

      /// <summary>
      /// Get rows by primary key of 2 columns
      /// 
      /// \* Max 2007-06-26
      /// </summary>
      /// <param name="fieldName1"></param>
      /// <param name="fieldValue1"></param>
      /// <param name="fieldName2"></param>
      /// <param name="fieldValue2"></param>
      /// <param name="msgPostfix"></param>
      /// <returns></returns>
      public DataSet GetRowsByPrimaryKey(string fieldName1, int fieldValue1, string fieldName2, int fieldValue2)
      {
         DataSet dsResult = new DataSet();
         string prefixMsg = "";
         try
         {
            prefixMsg = String.Format("Cannot get rows by [{0} = {1}] and [{2} = {3}]", 
               fieldName1, fieldValue1, fieldName2, fieldValue2);
            // 1. Prepares SQL statement
            string sql = String.Format("SELECT * FROM {0} WHERE {1}={2} AND {3}={4}",
               this.tableName, fieldName1, fieldValue1, fieldName2, fieldValue2);

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
   }

	/// <summary>
	/// Provides interfaces for connection tables without rules.
	/// Table structure is: [int],[int]
	/// Primary keys are: both
	/// Constraints:   
	/// (otherTbl [1 -- many] currentTBL [many -- 1] otherTbl)
	/// </summary>
	public class TblConnect2TblsWithoutRules : TblTwoPrimaryKeys
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="sqlExec"></param>
		protected TblConnect2TblsWithoutRules(string tableName, SQLExecuter sqlExec) : base (tableName, sqlExec)
		{
		}

      /// <summary>
		/// Adds new row.
		/// </summary>
		/// <param name="rowFieldName1"></param>
		/// <param name="rowFieldValue1"></param>
		/// <param name="rowFieldName2"></param>
		/// <param name="rowFieldValue2"></param>
		/// <param name="msgPostfix"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected void AddNewRow(string rowFieldName1, int rowFieldValue1, string rowFieldName2, int rowFieldValue2, string msgPostfix)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( " + rowFieldName1 + ", " + rowFieldName2 + ") VALUES ( {0}, {1})", rowFieldValue1, rowFieldValue2);
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + " " + rowFieldName2 + ":" + rowFieldValue2 + "' " + msgPostfix + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + " " + rowFieldName2 + ":" + rowFieldValue2 + "' " + msgPostfix + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + " " + rowFieldName2 + ":" + rowFieldValue2 + "' " + msgPostfix + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The " + msgPostfix + " already exists.");
			}		
		}
	}

	/// <summary>
	/// Provedes interfaces for connection tables with rules.
	/// Table structure is: [int],[int],[sting]
	/// Primary keys are: both
	/// Constraints:   
	/// (otherTbl [1 -- many] currentTBL [many -- 1] otherTbl)
	/// </summary>
	public class TblConnect2TblsWithRules : TblTwoPrimaryKeys
	{
      protected string tableDetailsName;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="sqlExec"></param>
		protected TblConnect2TblsWithRules(string tableName, SQLExecuter sqlExec) : base (tableName, sqlExec)
		{
		}

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="tableName"></param>
      /// <param name="sqlExec"></param>
      protected TblConnect2TblsWithRules(string masterTable, string detailsTable, SQLExecuter sqlExec)
         : base(masterTable, sqlExec)
      {
         tableDetailsName = detailsTable;
      }

        /// <summary>
		/// Add new row.
		/// </summary>
		/// <param name="rowFieldName1"></param>
		/// <param name="rowFieldValue1"></param>
		/// <param name="rowFieldName2"></param>
		/// <param name="rowFieldValue2"></param>
		/// <param name="rowFieldName3"></param>
		/// <param name="rowFieldValue3"></param>
		/// <param name="msgPostfix"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected void AddNewRow(string rowFieldName1, int rowFieldValue1, string rowFieldName2, int rowFieldValue2, string rowFieldName3, string rowFieldValue3, string msgPostfix)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( " + rowFieldName1 + 
				", " + rowFieldName2 + ", " + rowFieldName3 + 
				") VALUES ( {0}, {1} , '{2}')", 
				rowFieldValue1, rowFieldValue2, rowFieldValue3.Trim().Replace("'", "''"));
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The " + msgPostfix + " already exists.");
			}		
		}
	}

	/// <summary>
	/// Provides interfaces for connection tables with rules.
	/// Table structure is: [int],[int],[int]
	/// Primary keys are: both
	/// Constraints:   
	/// (otherTbl [1 -- many] currentTBL [many -- 1] otherTbl)
	/// </summary>
	public class TblConnect2TblsWithIntAdditField : TblTwoPrimaryKeys
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="sqlExec"></param>
		protected TblConnect2TblsWithIntAdditField(string tableName, SQLExecuter sqlExec) : base (tableName, sqlExec)
		{
		}

        /// <summary>
		/// Adds new row.
		/// </summary>
		/// <param name="rowFieldName1"></param>
		/// <param name="rowFieldValue1"></param>
		/// <param name="rowFieldName2"></param>
		/// <param name="rowFieldValue2"></param>
		/// <param name="rowFieldName3"></param>
		/// <param name="rowFieldValue3"></param>
		/// <param name="msgPostfix"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected void AddNewRow(string rowFieldName1, int rowFieldValue1, string rowFieldName2, int rowFieldValue2, string rowFieldName3, int rowFieldValue3, string msgPostfix)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( " + rowFieldName1 + 
				", " + rowFieldName2 + ", " + rowFieldName3 + 
				") VALUES ( {0}, {1} , {2})", 
				rowFieldValue1, rowFieldValue2, rowFieldValue3);
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Cannot add new '" + rowFieldName1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The " + msgPostfix + " already exists.");
			}		
		}
	}
}
