using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;	// for ArrayList
using VLF.ERR;

namespace VLF.DAS
{
   /// <summary>
   /// Database Access Service base class
   /// This class ONLY should be inherited
   /// </summary>
   abstract public class Das : IDisposable
   {
      #region Data Members

      /// <summary>
      /// Instance of SQL executer
      /// </summary>
      protected SQLExecuter sqlExec;

      public SQLExecuter SQLLayer
      {
         get { return sqlExec; }
      }
      #endregion

      #region Data Manipulation Functions

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="connectionString"></param>
      public Das(string connectionString)
      {
         try
         {
            sqlExec = new SQLExecuter(connectionString);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (System.Data.SqlClient.SqlException se)
         {
            throw new DASDbException(se.Message);
         }
         catch (TimeoutException te)
         {
            throw new TimeoutException(te.Message);
         }
         catch (Exception e)
         {
            throw new Exception(e.Message);
         }
      }

      /// <summary>
      /// Retrieves connection information to the database
      /// </summary>
      public string ConnectionString
      {
         get
         {
            return sqlExec.ConnectionString;
         }
      }

      #endregion

      #region Destructors
      /// <summary>
      /// Destructor
      /// </summary>
      public void Dispose()
      {
         if (null != sqlExec)
            sqlExec.Dispose();
      }

      #endregion
   }

   /// <summary>
   /// SQL Executer class
   /// </summary>
   public class SQLExecuter : IDisposable
   {
      #region Data Members

      private SqlConnection connection;
      private SqlCommand command;
      private string connectionString;
      private SqlTransaction transUserMan;

      #endregion

      #region Data Manipulation Functions

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="connectionString"></param>
      public SQLExecuter(string connectionString)
      {
         this.connectionString = connectionString;
         connection = new SqlConnection(connectionString);
         connection.Open();
         command = new SqlCommand();
         command.CommandTimeout = 600;          // is in seconds - 10 minutes
         // Set up event handler Listing 3A-2-2
         connection.StateChange += new StateChangeEventHandler(OnStateChange);
         // Set up event handler
         //connection.InfoMessage  += new SqlInfoMessageEventHandler(OnInfoMessage);//OleDbInfoMessageEventHandler(OnInfoMessage);
      }

      //			//--- Event related functions
      //			/// <summary>
      //			/// Listing 3A-6-3
      //			/// Shows Msg on warning received from DB provider
      //			/// </summary>
      //			/// <param name="sender"></param>
      //			/// <param name="args"></param>
      //			protected static void OnInfoMessage(object sender, SqlInfoMessageEventArgs args) 
      //			{
      //				/*
      //				// Loop through all the error messages 
      //				foreach (OleDbError objError in args.Errors) 
      //				{
      //					// Display the error properties
      //					MessageBox.Show("The " + objError.Source + " has raised a warning. " +
      //						" These are the properties :\n"  +
      //						"\nNative Error: " + objError.NativeError.ToString() +
      //						"\nSQL State: " + objError.SQLState +
      //						"\nMessage: " + objError.Message);
      //				}
      //
      //				// Display the message, error code and the source
      //				MessageBox.Show("Info Message: " + args.Message +
      //					"\nInfo Error Code: " + args.ErrorCode.ToString() +
      //					"\nInfo Source: " + args.Source);
      //				*/
      //			}

      /// <summary>
      /// Trap connection state chenges.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="args"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      protected static void OnStateChange(object sender, StateChangeEventArgs args)
      {
         /*
            // Display the original and the current state
            MessageBox.Show("The original connection state was: " + args.OriginalState.ToString() +
               "\nThe current connection state is: " + args.CurrentState.ToString());
         */
         if (args.OriginalState != args.CurrentState
            && (args.CurrentState == ConnectionState.Closed ||
               args.CurrentState == ConnectionState.Broken)
            )
            throw new DASDbConnectionClosed("Connection to database has been " + args.CurrentState.ToString() + ".");
      }

      #endregion

      #region Utility Functions

      /// <summary>
      /// Add values to SQL statement.
      /// Throws DASAppException exception in case of error.
      /// Note: use SetupCommand function before first call, for set SQL header.
      /// </summary>
      /// <param name="paramName"></param>
      /// <param name="paramType"></param>
      /// <param name="paramValue"></param>
      /// <param name="paramValueSize"></param>
      public void AddCommandParam(string paramName, SqlDbType paramType, string paramValue, int paramValueSize)
      {
         try
         {
            command.Parameters.Add(paramName, paramType, paramValueSize);
            if (paramValue == null)
               command.Parameters[paramName].Value = System.DBNull.Value;
            else
               command.Parameters[paramName].Value = paramValue;
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception excpt)
         {
            throw new DASAppException(excpt.Message + " Unable to add new parameter. Param Name= '" +
                              paramName + "' Param Type= '" + paramType +
                              "' Param Value= '" + paramValue + "'");
         }
      }

      /// <summary>
      /// Add values to SQL statement.
      /// Throws DASAppException exception in case of error.
      /// Note: use SetupCommand function before first call, for set SQL header.
      /// </summary>
      /// <param name="paramName"></param>
      /// <param name="paramType"></param>
      /// <param name="paramValue"></param>
      /// <param name="paramValueSize"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddCommandParam(string paramName, SqlDbType paramType, byte[] paramValue, int paramValueSize)
      {
         try
         {
            command.Parameters.Add(paramName, paramType, paramValueSize);
            if (paramValue == null)
               command.Parameters[paramName].Value = System.DBNull.Value;
            else
               command.Parameters[paramName].Value = paramValue;
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception excpt)
         {
            throw new DASAppException(excpt.Message + " Unable to add new parameter. Param Name= '" +
                              paramName + "' Param Type= '" + paramType +
                              "' Param Value= '" + paramValue + "'");
         }
      }

      /// <summary>
      /// Add values to SQL statement.
      /// Note: use SetupCommand function before first call, for set SQL header.
      /// </summary>
      /// <param name="paramName"></param>
      /// <param name="paramType"></param>
      /// <param name="paramValue"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddCommandParam(string paramName, SqlDbType paramType, object paramValue)
      {
         try
         {
            command.Parameters.Add(paramName, paramType);
            if (paramValue == null)
               command.Parameters[paramName].Value = System.DBNull.Value;
            else
               command.Parameters[paramName].Value = paramValue;
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception excpt)
         {
            throw new DASAppException(excpt.Message + " Unable to add new parameter. Param Name= '" +
                  paramName + "' Param Type= '" + paramType +
                  "' Param Value= '" + paramValue + "'");
         }
      }

      /// <summary>
      ///      Add values to SQL statement.
      ///      Note: use SetupCommand function before first call, for set SQL header.
      /// </summary>
      /// <param name="paramName"></param>
      /// <param name="paramType"></param>
      /// <param name="dir"></param>
      /// <param name="paramValue"></param>
      public void AddCommandParam(string paramName, SqlDbType paramType, ParameterDirection dir, object paramValue)
      {
         try
         {
            SqlParameter param = new SqlParameter(paramName, paramType);
            param.Direction = dir;
            param.Value = (paramValue == null) ? System.DBNull.Value :  paramValue ;
            command.Parameters.Add(param);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception excpt)
         {
            throw new DASAppException(excpt.Message + " Unable to add new parameter. Param Name= '" +
                  paramName + "' Param Type= '" + paramType +
                  "' Param Value= '" + paramValue + "'");
         }
      }


      public void AddCommandParam(string paramName, SqlDbType paramType, ParameterDirection dir, int size, object paramValue)
      {
         try
         {
            SqlParameter param = new SqlParameter(paramName, paramType);
            param.Direction = dir;
            param.Value = (paramValue == null) ? System.DBNull.Value : paramValue;
            param.Size = size;
            command.Parameters.Add(param);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception excpt)
         {
            throw new DASAppException(excpt.Message + " Unable to add new parameter. Param Name= '" +
                  paramName + "' Param Type= '" + paramType +
                  "' Param Value= '" + paramValue + "'");
         }
      }


      /// <summary>
      ///      reads an output param
      /// </summary>
      /// <param name="paramName"></param>
      /// <returns></returns>
      public object ReadCommandParam(string paramName)
      {
         return command.Parameters[paramName].Value;
      }

      /// <summary>
      /// Setup command with full SQL statement include values.
      /// </summary>
      /// <param name="sql"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// Obsolete - use SetupCommand(string cmdText, CommandType cmdType)
/*       
      private void SetupCommand(string sql)
      {
         try
         {
            //ClearCommandParameters();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.Connection = GetConnection();
         }
         catch (Exception e)
         {
            AssertConnectionStatus(e.Message);
            // if connection is OK, then rethrow this exception again.
            throw e;
         }
      }
*/
      /// <summary>
      /// Setup Command with type
      /// </summary>
      /// <param name="cmdText"></param>
      /// <param name="cmdType"></param>
      private void SetupCommand(string cmdText, CommandType cmdType)
      {
         try
         {
            //ClearCommandParameters();
            command.CommandText = cmdText;
            command.CommandType = cmdType;
            command.Connection = GetConnection();          
         }
         catch (Exception e)
         {
            AssertConnectionStatus(e.Message);
            // if connection is OK, then rethrow this exception again.
            throw e;
         }
      }

      /// <summary>
      /// Add SQL parameters array
      /// </summary>
      /// <exception cref="DASAppException">Thrown if connection to database has been closed.</exception>
      public void AddCommandParameters(params SqlParameter[] sqlParams)
      {
         if (command == null) throw new DASAppException("SqlCommand wasn't set properly.");
         ClearCommandParameters();
         command.Parameters.AddRange(sqlParams);
      }

      /// <summary>
      /// Add values Array
      /// </summary>
      /// <exception cref="DASAppException">Thrown if connection to database has been closed.</exception>
      public void AddCommandParameters(Array values)
      {
         if (command == null) throw new DASAppException("SqlCommand wasn't set properly.");
         ClearCommandParameters();
         command.Parameters.AddRange(values);
      }

      /// <summary>
      /// Clear SQLCommand parameters
      /// </summary>
      /// <exception cref="DASAppException">Thrown if connection to database has been closed.</exception>
      public void ClearCommandParameters()
      {
         if (command == null) throw new DASAppException("SqlCommand doesn't setup properly.");
         command.Parameters.Clear();
      }

      /// <summary>
      /// Retrieves database connection information.
      /// </summary>
      public string ConnectionString
      {
         get
         {
            return connectionString;
         }
      }

      /// <summary>
      /// Checks connection status
      /// </summary>
      private void AssertConnectionStatus(string errMessage)
      {
         if (!ConnectionOk())
            throw new DASDbConnectionClosed("Connection to database has been " + connection.State + ". " + errMessage);
      }

      /// <summary>
      /// Checks connection status
      /// </summary>
      private bool ConnectionOk()
      {
         if (connection == null || (connection.State != ConnectionState.Open && (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)))
            return false;
         return true;
      }

      /// <summary>
      /// Gets connection 
      /// </summary>
      /// <remarks>If connection has been closed, try to reopen it again</remarks>
      private SqlConnection GetConnection()
      {
         if (!ConnectionOk())
         {
            try
            {
               connection.Close();
            }
            catch
            {
            }
            // TODO: Log here
            connection = new SqlConnection(connectionString);
            connection.Open();
         }
         return connection;
      }

      /// <summary>
      /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
      /// </summary>
      /// <exception cref="ArgumentException">The property value assigned is less than 0.</exception>
      /// <returns>A value of 0 indicates no limit, and should be avoided in a CommandTimeout because an attempt to execute a command will wait indefinitely.</returns>
      public int CommandTimeout
      {
         get
         {
            return command.CommandTimeout;
         }
         set
         {
            command.CommandTimeout = value;
         }
      }

      /// <summary>
      /// Opens connection
      /// </summary>
      public void OpenConnection()
      {
         if (!ConnectionOk())// try to reconnect
            connection.Open();
         AssertConnectionStatus("Unable to open connection to database.");
      }

      #endregion

      #region SQL Execution Functions

      /// <summary>
      /// Get Data Reader
      /// </summary>
      /// <param name="sql"></param>
      /// <returns></returns>
      public SqlDataReader SQLExecuteGetDataReader(string sql)
      {
         SetupCommand(sql, CommandType.Text);
         SqlDataReader retResult = null;
         try
         {
            retResult = command.ExecuteReader(CommandBehavior.CloseConnection);
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {
                  SetupCommand(sql, CommandType.Text);
                  retResult = command.ExecuteReader(CommandBehavior.CloseConnection);
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return retResult;
      }

      /// <summary>
      /// Get Data Reader
      /// </summary>
      /// <param name="sql"></param>
      /// <returns></returns>
      public SqlDataReader SQLExecuteSPGetDataReader(string sql)
      {
          SetupCommand(sql, CommandType.StoredProcedure);
          SqlDataReader retResult = null;
          try
          {
              retResult = command.ExecuteReader();
          }
          catch (Exception e)
          {
              if (!ConnectionOk())// try to reconnect
              {
                  try
                  {
                      SetupCommand(sql, CommandType.StoredProcedure);
                      retResult = command.ExecuteReader();
                          //(CommandBehavior.CloseConnection);
                  }
                  catch (Exception exp)
                  {
                      AssertConnectionStatus(exp.Message);
                      // if connection is OK, then rethrow this exception again.
                      throw exp;
                  }
              }
              else
                  throw e;
          }
          return retResult;
      }
      /// <summary>
      /// Executes the query, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.
      /// </summary>
      /// <param name="sql"></param>
      /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty.</returns>
      /// <exception cref="SqlException">An exception occurred while executing the command against a locked row. This exception is not generated when using Microsoft .NET Framework version 1.0.</exception>
      public object SQLExecuteScalar(string sql)
      {
         SetupCommand(sql, CommandType.Text);
         object retResult = null;
         try
         {
            retResult = command.ExecuteScalar();
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {
                  SetupCommand(sql, CommandType.Text);
                  retResult = command.ExecuteScalar();
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return retResult;
      }

      /// <summary>
      /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
      /// </summary>
      /// <param name="sql"></param>
      /// <returns>The number of rows affected.</returns>
      /// <exception cref="SqlException">An exception occurred while executing the command against a locked row. This exception is not generated when using Microsoft .NET Framework version 1.0.</exception>
      public int SQLExecuteNonQuery(string sql)
      {
         SetupCommand(sql, CommandType.Text);
         int retResult = 0;
         try
         {
            retResult = command.ExecuteNonQuery();
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {
                  SetupCommand(sql, CommandType.Text);
                  retResult = command.ExecuteNonQuery();
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return retResult;
      }

      /// <summary>
      /// Sends the CommandText to the Connection and builds a DataSet
      /// </summary>
      /// <param name="sql"></param>
      /// <returns></returns>
      public DataSet SQLExecuteDataset(string sql)
      {
         SetupCommand(sql, CommandType.Text);
         DataSet dataSet = new DataSet();
         SqlDataAdapter adapter = new SqlDataAdapter();
         try
         {
            adapter.SelectCommand = command;
            adapter.Fill(dataSet);
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {
                  SetupCommand(sql, CommandType.Text);
                  adapter.SelectCommand = command;
                  adapter.Fill(dataSet);
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return dataSet;
      }

      # endregion

      # region Insert bulk data
/*
      /// <summary>
      /// Insert Data into a table
      /// </summary>
      /// <param name="data">Source Table containing data</param>
      /// <param name="tableName">Destination Table name to add data</param>
      public void InsertData(DataTable data, string tableName, SqlBulkCopyOptions options)
      {
         try
         {
            this.OpenConnection();
            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(this.connectionString))
            {
               sqlBulk.BatchSize = 100;
               sqlBulk.DestinationTableName = tableName;
               sqlBulk.WriteToServer(data);
            }
         }
         catch (Exception ex)
         {
            AssertConnectionStatus(ex.Message);
            // if connection is OK, then rethrow this exception again.
            throw ex;
         }
      }

      /// <summary>
      /// Insert Data into a table
      /// </summary>
      /// <param name="data">Source Data rows array containing data</param>
      /// <param name="tableName">Destination Table to add data</param>
      public void InsertData(DataRow[] data, string tableName, SqlBulkCopyOptions options)
      {
         try
         {
            this.OpenConnection();
            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(this.connectionString))
            {
               sqlBulk.BatchSize = 100;
               sqlBulk.DestinationTableName = tableName;
               sqlBulk.WriteToServer(data);
            }
         }
         catch (Exception ex)
         {
            AssertConnectionStatus(ex.Message);
            // if connection is OK, then rethrow this exception again.
            throw ex;
         }
      }
*/
      /// <summary>
      /// Insert Data into a table, using column mappings
      /// </summary>
      /// <param name="data">Source data table</param>
      /// <param name="options">SqlBulkCopyOptions, use null or Default if not known</param>
      /// <param name="tableName">Destination Table to add data</param>
      /// <param name="mappings">SqlBulkCopyColumnMappingCollection, use null if not required</param>
      public void InsertData(DataTable data, string tableName, SqlBulkCopyOptions options, SqlBulkCopyColumnMappingCollection mappings)
      {
         try
         {
            this.OpenConnection();
            if (options == null) options = SqlBulkCopyOptions.Default;
            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(this.connectionString, options))
            {
               sqlBulk.BatchSize = 100;
               if (mappings != null)
               {
                  foreach (SqlBulkCopyColumnMapping mapping in mappings)
                  {
                     sqlBulk.ColumnMappings.Add(mapping);
                  }
               }
               sqlBulk.DestinationTableName = tableName;
               sqlBulk.WriteToServer(data);
            }
         }
         catch (Exception ex)
         {
            AssertConnectionStatus(ex.Message);
            // if connection is OK, then rethrow this exception again.
            throw ex;
         }
      }

      /// <summary>
      /// Insert Data into a table, using column mappings
      /// </summary>
      /// <param name="data">Data rows array containing data</param>
      /// <param name="tableName">Destination Table to add data</param>
      /// <param name="options">SqlBulkCopyOptions, use null or Default if not known</param>
      /// <param name="mappings">SqlBulkCopyColumnMappingCollection, use null if not required</param>
      public void InsertData(DataRow[] data, string tableName, SqlBulkCopyOptions options, SqlBulkCopyColumnMappingCollection mappings)
      {
         try
         {
            this.OpenConnection();
            if (options == null) options = SqlBulkCopyOptions.Default;
            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(this.connectionString, options))
            {
               sqlBulk.BatchSize = 100;
               if (mappings != null)
               {
                  foreach (SqlBulkCopyColumnMapping mapping in mappings)
                  {
                     sqlBulk.ColumnMappings.Add(mapping);
                  }
               }
               sqlBulk.DestinationTableName = tableName;
               sqlBulk.WriteToServer(data);
            }
         }
         catch (Exception ex)
         {
            AssertConnectionStatus(ex.Message);
            // if connection is OK, then rethrow this exception again.
            throw ex;
         }
      }

      /// <summary>
      /// Insert Data into a table, using column mappings
      /// </summary>
      /// <param name="reader">IDataReader containing data (SqlDataReader)</param>
      /// <param name="tableName">Destination Table to add data</param>
      /// <param name="options">SqlBulkCopyOptions, use null or Default if not known</param>
      /// <param name="mappings">SqlBulkCopyColumnMappingCollection, use null if not required</param>
      public void InsertData(IDataReader reader, string tableName, SqlBulkCopyOptions options, SqlBulkCopyColumnMappingCollection mappings)
      {
         try
         {
            this.OpenConnection();
            if (options == null) options = SqlBulkCopyOptions.Default;
            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(this.connectionString, options))
            {
               sqlBulk.BatchSize = 100;
               if (mappings != null)
               {
                  foreach (SqlBulkCopyColumnMapping mapping in mappings)
                  {
                     sqlBulk.ColumnMappings.Add(mapping);
                  }
               }
               sqlBulk.DestinationTableName = tableName;
               sqlBulk.WriteToServer(reader);
            }
         }
         catch (Exception ex)
         {
            AssertConnectionStatus(ex.Message);
            // if connection is OK, then rethrow this exception again.
            throw ex;
         }
      }

      #endregion

      # region SP Execution Functions

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <returns>Dataset</returns>
      public DataSet SPExecuteDataset(string sp_name)
       {
           SetupCommand(sp_name, CommandType.StoredProcedure);
           DataSet dataSet = new DataSet();
           SqlDataAdapter adapter = new SqlDataAdapter(command);
           try
           {
               //adapter.SelectCommand = command;
               adapter.Fill(dataSet);
           }
           catch (Exception e)
           {
               if (!ConnectionOk())// try to reconnect
               {
                   try
                   {

                       SetupCommand(sp_name, CommandType.StoredProcedure);
                       //adapter.SelectCommand = command;
                       adapter.Fill(dataSet);
                   }
                   catch (Exception exp)
                   {
                       AssertConnectionStatus(exp.Message);
                       // if connection is OK, then rethrow this exception again.
                       throw exp;
                   }
               }
               else
                   throw e;
           }
           return dataSet;
       }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <param name="paramList">Array of sql parameters</param>
      /// <returns>Dataset</returns>
      public DataSet SPExecuteDataset(string sp_name, SqlParameter[] paramList)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         DataSet dataSet = new DataSet();
         if (paramList != null) AddCommandParameters(paramList);
         SqlDataAdapter adapter = new SqlDataAdapter(command);
         try
         {
            //adapter.SelectCommand = command;
            adapter.Fill(dataSet);
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {

                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  //adapter.SelectCommand = command;
                  adapter.Fill(dataSet);
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return dataSet;
      }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <param name="tableName">table name</param>
      /// <returns>Table</returns>
      public DataTable SPExecuteDataTable(string sp_name, string tableName)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         DataSet dataSet = new DataSet();
         DataTable dataTable = new DataTable();
         SqlDataAdapter adapter = new SqlDataAdapter(command);
         try
         {
            //adapter.SelectCommand = command;
            adapter.Fill(dataSet, tableName);
            dataTable = dataSet.Tables[tableName];
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {

                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  //adapter.SelectCommand = command;
                  adapter.Fill(dataSet, tableName);
                  dataTable = dataSet.Tables[tableName];
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return dataTable;
      }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <param name="tableName">table name</param>
      /// <param name="paramList">Array of sql parameters</param>
      /// <returns>Table</returns>
      public DataTable SPExecuteDataTable(string sp_name, string tableName, SqlParameter[] paramList)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         DataSet dataSet = new DataSet();
         DataTable dataTable = new DataTable();
         if (paramList != null) AddCommandParameters(paramList);
         SqlDataAdapter adapter = new SqlDataAdapter(command);
         try
         {
            //adapter.SelectCommand = command;
            adapter.Fill(dataSet, tableName);
            dataTable = dataSet.Tables[tableName];
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {

                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  //adapter.SelectCommand = command;
                  adapter.Fill(dataSet, tableName);
                  dataTable = dataSet.Tables[tableName];
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else
               throw e;
         }
         return dataTable;
      }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <returns>Object</returns>
      public object SPExecuteScalar(string sp_name)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         object retResult = null;
         try
         {
            retResult = command.ExecuteScalar();
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {

                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  retResult = command.ExecuteScalar();
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else throw e;
         }
         return retResult;
      }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <param name="paramList">Array of sql parameters</param>
      /// <returns>Object</returns>
      public object SPExecuteScalar(string sp_name, SqlParameter[] paramList)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         object retResult = null;
         if (paramList != null) AddCommandParameters(paramList);
         try
         {
            retResult = command.ExecuteScalar();
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// try to reconnect
            {
               try
               {

                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  retResult = command.ExecuteScalar();
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else throw e;
         }
         return retResult;
      }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <returns>Integer - rows</returns>
      public int SPExecuteNonQuery(string sp_name)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         int retResult = 0;
         try
         {
            retResult = command.ExecuteNonQuery();
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// trying to reconnect
            {
               try
               {
                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  retResult = command.ExecuteNonQuery();
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else throw e;
         }
         return retResult;
      }

      /// <summary>
      /// Execute stored procedure by name
      /// </summary>
      /// <param name="sp_name">sp name</param>
      /// <param name="paramList">Array of sql parameters</param>
      /// <returns>Integer - rows</returns>
      public int SPExecuteNonQuery(string sp_name, SqlParameter[] paramList)
      {
         SetupCommand(sp_name, CommandType.StoredProcedure);
         if (paramList != null) AddCommandParameters(paramList);
         int retResult = 0;
         try
         {
            retResult = command.ExecuteNonQuery();
         }
         catch (Exception e)
         {
            if (!ConnectionOk())// trying to reconnect
            {
               try
               {
                  SetupCommand(sp_name, CommandType.StoredProcedure);
                  retResult = command.ExecuteNonQuery();
               }
               catch (Exception exp)
               {
                  AssertConnectionStatus(exp.Message);
                  // if connection is OK, then rethrow this exception again.
                  throw exp;
               }
            }
            else throw e;
         }
         return retResult;
      }

      # endregion

      #region Transaction Operations Functions

      /// <summary>
      /// Groupping database releted operations - Listing 3A-8
      /// 1. Connection must be valid and open, or the "DASAppException" is thrown.
      /// </summary>
      public void BeginTransaction()
      {
         AssertConnectionStatus("Unable to begin transaction.");
         // Attaches transaction to connection
         transUserMan = connection.BeginTransaction();
      }

      /// <summary>
      /// Groupping database releted operations - Listing 3A-8
      /// 1. Connection must be valid and open, or the "DASAppException" is thrown.
      /// </summary>
      /// <param name="isolationLevel"></param>
      public void BeginTransactionWithIsolationLevel(IsolationLevel isolationLevel)
      {
         AssertConnectionStatus("Unable to begin transaction with isolation level " + isolationLevel);
         // Attaches transaction to connection
         transUserMan = connection.BeginTransaction(isolationLevel);
      }

      /// <summary>
      /// 1. Connection must be valid and open, or the "DASAppException" is thrown.
      /// 2. AttachCommandToTransaction must be called after "BeginTransaction" mathod 
      /// has been called, or the "DASAppException" is thrown.
      /// </summary>
      public void AttachToTransaction(string sql)
      {
         AssertConnectionStatus("Unable to attach SQL statement to transaction.");
         if (transUserMan.Connection == null)
         {
            throw new DASAppException("Unable to attach SQL statement to transaction. Transaction no longer is valid");
         }
         else
         {
            // attach new sql to command.
            command.CommandText = sql;
            // add new sql to transaction.
            command.Transaction = transUserMan;
         }
      }

      /// <summary>
      /// Aborting a manual transaction. Rollback all changes since beginning transaction
      /// - Listing 3A-9
      /// 1. Connection must be valid and open, or the "DASAppException" is thrown.
      /// 2. Rollback must be called after "BeginTransaction" mathod has been called, or
      /// the "DASAppException" is thrown.
      /// </summary>
      public void RollbackTransaction()
      {
         AssertConnectionStatus("Unable to rollback transaction.");
         if (transUserMan.Connection == null)
         {
            throw new DASAppException("Unable to attach SQL statement to transaction. Transaction no longer is valid.");
         }
         else
         {
            // Rollback transaction from pending state.
            transUserMan.Rollback();
         }
      }

      /// <summary>
      /// Commiting a manual transaction. Commit all changes since beginning transaction
      /// - Listing 3A-9
      /// 1. Connection must be valid and open, or the "DASAppException" is thrown.
      /// 2. Commit must be called after "BeginTransaction" mathod has been called, or
      /// the "DASAppException" is thrown.
      /// </summary>
      public void CommitTransaction()
      {
         AssertConnectionStatus("Unable to commit transaction.");
         if (transUserMan.Connection == null)
            throw new DASAppException("Unable to commit transaction. Transaction no longer is valid.");
         else
            // Commit transaction from pending state.
            transUserMan.Commit();
      }

      /// <summary>
      /// Check if command has been initiated with transaction. 
      /// "BeginTransaction" method been called before.
      /// Returns true if command already using transaction.
      /// </summary>
      public bool RequiredTransaction()
      {
         AssertConnectionStatus("Unable to retrieve transaction information.");
         if (transUserMan != null && transUserMan.Connection != null)
            return true;
         return false;
      }

      /// <summary>
      /// Retrieves the transaction isolation level - Listing 3A-13
      /// 1. Connection must be valid and open, or the "DASAppException" is thrown.
      /// 2. GetIsolationLevel must be called after "BeginTransaction" mathod has been called, or
      /// the "DASAppException" is thrown.
      /// </summary>
      /// <returns></returns>
      public IsolationLevel GetIsolationLevel()
      {
         AssertConnectionStatus("Unable to get transaction isolation level.");
         if (transUserMan.Connection == null)
            throw new DASAppException("Unable to get transaction isolation level. Transaction no longer is valid.");
         return transUserMan.IsolationLevel;
      }

      #endregion

      #region Destructors

      private void CleanUp()
      {
         if (command != null)
            command.Dispose();            // gb, 04/08/2011 

         if (transUserMan != null)        // gb, 04/08/2011
            transUserMan.Dispose();

         if (null != connection && connection.State == ConnectionState.Open)
            connection.Close();

         if (null != connection)
            connection.Dispose();         // gb, 04/08/2011 
      }

      /// <summary>
      /// 1. Finalizers are implemented by overriding the Object.Finalize method.
      /// However, types written in C# or C++ implement destructors, which compilers 
      /// turn into an override of Object.Finalize
      /// 2. Allows an Object to attempt to free resources and perform 
      /// other cleanup operations before the Object is reclaimed by garbage collection.
      /// 3. In C#, finalizers are expressed using destructor syntax.
      /// 4. This method is automatically called after an object becomes inaccessible, 
      /// unless the object has been exempted from finalization by a call 
      /// to SuppressFinalize
      /// Note: 
      ///		A type should implement Finalize when it uses unmanaged resources such as 
      ///		file handles or database connections that must be released when 
      ///		the managed object that uses them is reclaimed. 
      /// </summary>      
      ~SQLExecuter()
      {
         this.Dispose();
      }

      public void DisposeNew()
      {
         CleanUp();
         GC.SuppressFinalize(this); //Requests that the system not call the finalizer method for the specified object
      }

      /// <summary>
      /// 1. Use the Dispose method of this interface to explicitly release 
      /// unmanaged resources in conjunction with the garbage collector. 
      /// The consumer of an object CAN call this method when the object is 
      /// no longer needed.
      /// 2. Because the Dispose method must be called explicitly, objects that implement 
      /// IDisposable MUST also implement a finalizer to handle freeing resources 
      /// when Dispose is not called. 
      /// 3. By default, the garbage collector will automatically call an 
      /// object's finalizer prior to reclaiming its memory. However, once the 
      /// Dispose method has been called, it is typically unnecessary for the 
      /// garbage collector to call the disposed object's finalizer. 
      /// To prevent automatic finalization, Dispose implementations can call the 
      /// GC.SuppressFinalize method
      /// 4. The garbage collector does not, by default, call the Dispose method.
      /// However, implementations of the Dispose method can call methods in the GC class 
      /// to customize the finalization behavior of the garbage collector
      /// </summary>
      /// 
      public void Dispose()
      {
         try
         {
            // Perform some cleanup operations here.
            CleanUp();
         }
         catch
         {
         }
         finally
         {
            //Controls the system garbage collector and automatically reclaims unused memory.
//            GC.Collect(); // Forces garbage collection of all generations
            GC.SuppressFinalize(this); //Requests that the system not call the finalizer method for the specified object
         }
      }

      #endregion
   }
}
