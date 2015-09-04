using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using VLF.DAS.Logic;
using System.Data;
using System.Data.SqlClient;
namespace VLF.ASI
{
    /// <summary>
    /// Summary description for LoggerManager
    /// </summary>
    public class LoggerManager
    {
        public static int RecordUserAction(string moduleName, int userId, int organizationId, string tableName, string primaryWhere, string action,
            string remoteAddr, string applicationName, string description)
        {
            string sql = "sp_logging"; //sp name to execute

            try
            {
                LogEvent logEvent = new LogEvent(LoginManager.GetConnnectionString(userId));
                bool result = logEvent.SaveToLog(moduleName, userId, organizationId, tableName, primaryWhere, action, remoteAddr,
                                   applicationName, sql, description);
                logEvent.Dispose();
                return result ? 1 : 0;
            }
            catch (Exception Ex)
            {
                LogException("<< Record user action : uId={0}, action={2}, moduleName={3}, EXC={1}", userId, Ex.Message, action, moduleName);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        public static int RecordInitialValues(string moduleName, int userId, int organizationId, string tableName, string primaryWhere, string action, string remoteAddr, string applicationName, string description)
        {
            string sql = "sp_LoggingInitialValues"; //sp name to execute

            try
            {
                LogEvent logEvent = new LogEvent(LoginManager.GetConnnectionString(userId));
                bool result = logEvent.SaveToLog(moduleName, userId, organizationId, tableName, primaryWhere, action, remoteAddr,
                                   applicationName, sql, description);
                logEvent.Dispose();
                return result ? 1 : 0;
            }
            catch (Exception Ex)
            {
                LogException("<< Record initial values : uId={0}, action={2}, moduleName={3}, EXC={1}", userId, Ex.Message, action, moduleName);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        public static Int64 GetGeozoneNo(Int64 vehicleId, short geozoneId)
        {
            Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(LoginManager.GetConnnectionString(-1)))
                {
                    string sql = "SELECT vlfOrganizationGeozone.GeozoneNo" +
                            " FROM vlfOrganizationGeozone INNER JOIN vlfVehicleInfo ON vlfOrganizationGeozone.OrganizationId = vlfVehicleInfo.OrganizationId" +
                            " WHERE vlfVehicleInfo.VehicleId=" + vehicleId +
                            " AND vlfOrganizationGeozone.GeozoneId=" + geozoneId;
                    //Executes SQL statement
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    SqlDataReader sr = sqlCommand.ExecuteReader();
                    sr.Read();
                    geozoneNo = Convert.ToInt64(sr[0]);
                }

            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicle " + vehicleId + " geozone " + geozoneId + " geozone No. ";
            }
            return geozoneNo;
        }

        private static void LogException(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }
    }
}