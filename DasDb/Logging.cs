using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;
using System.Text.RegularExpressions;

namespace VLF.DAS.DB
{
    public class Logging : IDisposable
    {
        private string connStr { get; set; }
        bool isDisposed = false;
        public Logging(string myConnStr)
        {
            connStr = myConnStr;
        }

        ~Logging()
        {
            Dispose();
        }

        private string GetNewRow(string tableName, string primaryWhere)
        {
            string resultSet = null;
            try
            {
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    StringBuilder sb = new StringBuilder("SELECT * FROM ");
                    sb.Append(tableName + " WHERE ");
                    sb.Append(primaryWhere);
                    SqlCommand sqlCommand = new SqlCommand(sb.ToString(), sConn);
                    sqlCommand.CommandType = CommandType.Text;
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                    DataSet results = new DataSet();
                    adapter.Fill(results, tableName);
                    DataTable dt = results.Tables[tableName];
                    foreach (DataRow dr in dt.Rows)
                    {
                        resultSet = null;
                        foreach (DataColumn column in dt.Columns)
                        {
                            string tempSetStr = string.Format("{0}={1}", column.ColumnName,
                                                           Convert.ToString(dr[column.ColumnName]));
                            if (string.IsNullOrEmpty(resultSet))
                            {
                                resultSet = tempSetStr;
                            }
                            else
                            {
                                resultSet += ";" + tempSetStr;
                            }
                        }
                    }
                    sConn.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            return resultSet;
        }

        public bool SaveLog(string moduleName, int userId, int organizationId, string tableName, string primaryWhere, string action, 
            string remoteAddr, string applicationName, string sql, string description = null)
        {
            int myid = 0;
            string valuesStr = null;

            try
            {
                if (!action.Contains("Delete") && !string.IsNullOrEmpty(primaryWhere))
                {
                    valuesStr = GetNewRow(tableName, primaryWhere);
                }
                else
                {
                    valuesStr = primaryWhere;
                }

                if (!string.IsNullOrEmpty(valuesStr))
                {
                    string[] vals = valuesStr.Split(';');
                    primaryWhere = vals[0];
                }
                else
                    valuesStr = Regex.Replace(primaryWhere, " and ", ";", RegexOptions.IgnoreCase);

                //string sql = "sp_logging";
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sConn);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@moduleName", moduleName);
                    sqlCommand.Parameters.AddWithValue("@organizationId", organizationId);
                    sqlCommand.Parameters.AddWithValue("@tableName", tableName);
                    sqlCommand.Parameters.AddWithValue("@action", action);
                    if (!string.IsNullOrEmpty(primaryWhere))
                    {
                        string processedWhereClauster = primaryWhere.Replace(" AND ", " and ");
                        processedWhereClauster = processedWhereClauster.Replace(" And ", " and ");
                        IList<string> processedWhereClausterArray = processedWhereClauster.Split(new string[] { " and " }, StringSplitOptions.None);

                        if (processedWhereClausterArray.Count > 0)
                        {
                            foreach (string myExp in processedWhereClausterArray)
                            {

                                string processedExp = myExp.Replace(" ", "");
                                string[] processedExpArray = processedExp.Split('=');
                                sqlCommand.Parameters.AddWithValue("@columnName", processedExpArray[0]);
                                sqlCommand.Parameters.AddWithValue("@record_id", processedExpArray[1]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@record_id", 0);
                    }


                    sqlCommand.Parameters.AddWithValue("@updatedby", userId);
                    sqlCommand.Parameters.AddWithValue("@newvalue", valuesStr);
                    sqlCommand.Parameters.AddWithValue("@remoteAddr", remoteAddr);
                    sqlCommand.Parameters.AddWithValue("@applicationName", applicationName);
                    sqlCommand.Parameters.AddWithValue("@description", description);
                    SqlDataReader sr = sqlCommand.ExecuteReader();
                    sr.Read();
                    myid = Convert.ToInt32(sr[0]);
                    sr.Close();
                    sConn.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            return myid > 0;
        }

        public bool ValidatePasswordInLastEight(string newpassword, int userId)
        {
            try
            {
                using (SqlConnection sConn = new SqlConnection(connStr))
                {
                    sConn.Open();
                    string rawSql = "SELECT TOP(8) description FROM dbo.AuditLogs WHERE ColumnName='{0}' AND record_id={1} AND (Action='{2}' OR Action='{3}') AND description in (SELECT DISTINCT description FROM dbo.AuditLogs) ORDER BY Updated DESC";
                    string sql = string.Format(rawSql, "UserId", userId, "Reset Password", "Create User");
                    SqlCommand sqlCommand = new SqlCommand(sql, sConn);
                    sqlCommand.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    SqlDataAdapter sAdapter = new SqlDataAdapter(sqlCommand);
                    sAdapter.Fill(ds, "AuditLogs");
                    DataTable dt = ds.Tables["AuditLogs"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (newpassword.Equals(dr["description"]))
                        {
                            return true;
                        }
                    }
                    sConn.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            return false;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool doingDispose)
        {
            if (!this.isDisposed)
            {
                if (doingDispose)
                {
                    connStr = null;
                    //Release all managed resources
                }
                //Release all Unmanaged resources over here.
                //So if doingDispose is FALSE, then only unmanaged resources will be released.
            }
            this.isDisposed = true;
        }
    }
}
