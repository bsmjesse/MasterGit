using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VLF.CLS;
using VLF.ERR;

namespace VLF.DAS.DB
{
    public class ServiceAssignmentDB
    {
        private static string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];

        public static DataTable GetServices(int organizationId)
        {
            try
            {
                string sql = "SELECT * FROM vlfEventServices " +
                             "INNER JOIN vlfOrganizationServices ON vlfOrganizationServices.ServiceID=vlfEventServices.ServiceID " +
                             "WHERE vlfOrganizationServices.OrganizationID=@organizationId " +
                             "AND vlfOrganizationServices.Status=1 ORDER BY vlfEventServices.ServiceName";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@organizationId", organizationId);
                        sqlCommand.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetServiceInfo(int serviceId)
        {
            try
            {
                string sql = "SELECT * FROM vlfEventServices WHERE ServiceId=@serviceId";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@serviceId", serviceId);
                        sqlCommand.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetServicesAndConfiguredRules(int organizationId, int userId)
        {
            try
            {
                string sql = "SELECT vlfServiceConfigurations.ServiceConfigID, vlfServiceConfigurations.ServiceConfigName, vlfEventServices.ServiceID, vlfEventServices.ServiceName " +
                             "FROM vlfOrganizationServices " +
                             "INNER JOIN vlfEventServices ON vlfEventServices.ServiceID = vlfOrganizationServices.ServiceID " +
                             "INNER JOIN vlfServiceConfigurations ON vlfEventServices.ServiceID = vlfServiceConfigurations.ServiceID AND vlfServiceConfigurations.OrganizationID = vlfOrganizationServices.OrganizationID " +
                             //"INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                             (organizationId.Equals(480) ? "WHERE vlfOrganizationServices.OrganizationID in (@organizationId, 0) " : "WHERE vlfOrganizationServices.OrganizationID = @organizationId ") +
                             //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) " +
                             "AND Status = 1 AND vlfServiceConfigurations.ExpiredDate IS NULL;";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@organizationId", organizationId);
                        //sqlCommand.Parameters.AddWithValue("@userId", userId);
                        sqlCommand.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetRules(int serviceId)
        {
            try
            {
                string sql = "SELECT * FROM vlfEventRules WHERE ServiceId=@serviceId AND Status=1 ORDER BY RuleName";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@serviceId", serviceId);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static int SaveServiceConfig(string expression, string serviceConfigName, int serviceId, int organizationId, int userId, DateTime? expiredDate = null, int serviceConfigId = 0, bool deleteService = false, int isActive = 1, int isReportable = 1)
        {
            try
            {
                //int routeId = GetRouteId(serviceId, expression);
                string sql = "INSERT INTO vlfServiceConfigurations (ServiceID, ServiceConfigName, OrganizationID, IsActive, RulesApplied, CreatedDate, UserID, IsReportable) VALUES (@serviceId, @serviceConfigName, @organizationId, @isactive, @rulesApplied, @createdDate, @userId, @isReportable);SELECT SCOPE_IDENTITY()";
                if (serviceConfigId > 0)
                {
                    sql =
                        "UPDATE vlfServiceConfigurations SET ServiceID=@serviceId, ServiceConfigName=@serviceConfigName, OrganizationID=@organizationId, IsActive=@isactive, RulesApplied=@rulesApplied, CreatedDate=@createdDate, UserID=@userId, IsReportable=@isReportable WHERE ServiceConfigID=@serviceConfigId";
                    if (deleteService)
                    {
                        //sql = "DELETE FROM vlfServiceConfigurations WHERE ServiceConfigID=@serviceConfigId";
                        sql =
                            "UPDATE vlfServiceConfigurations SET ExpiredDate=@expiredDate, UserID=@userId WHERE ServiceConfigID=@serviceConfigId";
                        SaveObjects(serviceConfigId, null, null, null, DateTime.Now, deleteService);
                    }
                }

                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        sqlCommand.CommandType = CommandType.Text;
                        if (!deleteService)
                        {
                            sqlCommand.Parameters.AddWithValue("@serviceId", serviceId);
                            sqlCommand.Parameters.AddWithValue("@serviceConfigName", serviceConfigName);
                            sqlCommand.Parameters.AddWithValue("@organizationId", organizationId);
                            sqlCommand.Parameters.AddWithValue("@rulesApplied", expression);
                            sqlCommand.Parameters.AddWithValue("@createdDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@isactive", isActive);
                            sqlCommand.Parameters.AddWithValue("@isReportable", isReportable); 
                            
                        }
                        sqlCommand.Parameters.AddWithValue("@userId", userId);
                        if (serviceConfigId < 1)
                        {
                            serviceConfigId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                        }
                        else
                        {
                            sqlCommand.Parameters.AddWithValue("@expiredDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@ServiceConfigID", serviceConfigId);
                            sqlCommand.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }

                return serviceConfigId;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }

        public static int SaveNotification(int serviceConfigId, string recipientsList, string emailLevel, string subject, string messageBody)
        {
            string insertSql = "INSERT INTO vlfServiceConfigNotification (ServiceConfigId, RecipientsList, EmailLevel, Subject, Message) VALUES (@serviceConfigId, @recipientsList, @emailLevel, @subject, @messageBody)";
            string updateSql =
                "UPDATE vlfServiceConfigNotification SET ServiceConfigId=@serviceConfigId, RecipientsList=@recipientsList, EmailLevel=@emailLevel, Subject=@subject, Message=@messageBody WHERE ServiceConfigId=@serviceConfigId";
            string selectSql = "SELECT COUNT(*) AS NUM FROM vlfServiceConfigNotification WHERE ServiceConfigId=@serviceConfigId";
            SqlConnection connection = null;
            int result = 0;
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(selectSql, connection))
                    {
                        connection.Open();
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        reader.Read();
                        SqlCommand saveCommand = null;
                        if (Convert.ToInt32(reader["NUM"]) > 0)
                        {
                            saveCommand = new SqlCommand(updateSql, connection);
                        }
                        else
                        {
                            saveCommand = new SqlCommand(insertSql, connection);
                        }
                        reader.Close();
                        saveCommand.CommandType = CommandType.Text;
                        saveCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        saveCommand.Parameters.AddWithValue("@recipientsList", recipientsList);
                        if (string.IsNullOrEmpty(emailLevel))
                        {
                            saveCommand.Parameters.AddWithValue("@emailLevel", DBNull.Value);
                        }
                        else
                        {
                            saveCommand.Parameters.AddWithValue("@emailLevel",emailLevel);
                        }
                        saveCommand.Parameters.AddWithValue("@subject", subject);
                        saveCommand.Parameters.AddWithValue("@messageBody", messageBody);

                        result = saveCommand.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }

        
        public static void SaveObjects(int serviceConfigId, string target = null, IList<int> includeList = null,
            IList<int> excludeList = null, DateTime? createdDatetime = null, bool delete = false)
        {
            SqlConnection connection = null;
            string sql = "SELECT * FROM vlfServiceAssignments WHERE ServiceConfigID=@serviceConfigId AND Objects=@target AND Inclusive=1 AND Deleted=0";
            if (string.IsNullOrEmpty(target))
            {
                sql = "SELECT * FROM vlfServiceAssignments WHERE ServiceConfigID=@serviceConfigId AND Deleted=0";
            }
            else
            {
                
                if (target.ToLower().Equals("fleet"))
                {
                    sql = "SELECT * FROM vlfServiceAssignments WHERE ServiceConfigID=@serviceConfigId AND (Objects='Fleet' OR (Objects='Vehicle' AND  Inclusive=0)) AND Deleted=0";
                }
            }
            
            
            try
            {
                DataTable includes = new DataTable();
                includes.Columns.Add("ServiceConfigID", typeof(int));
                includes.Columns.Add("Objects", typeof(string));
                includes.Columns.Add("ObjectID", typeof(Int64));
                includes.Columns.Add("StDate", typeof(DateTime));
                includes.Columns.Add("EndDate", typeof(DateTime));
                includes.Columns.Add("Inclusive", typeof(bool));
                includes.Columns.Add("Deleted", typeof(int));

                DataTable updates = new DataTable();
                updates.Columns.Add("ServiceConfigID", typeof(int));
                updates.Columns.Add("Objects", typeof(string));
                updates.Columns.Add("ObjectID", typeof(Int64));
                updates.Columns.Add("StDate", typeof(DateTime));
                updates.Columns.Add("EndDate", typeof(DateTime));
                updates.Columns.Add("Inclusive", typeof(bool));
                updates.Columns.Add("Deleted", typeof(int));
                IList<int> cIncludes = new List<int>();
                IList<int> cExcludes = new List<int>();
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        if (!string.IsNullOrEmpty(target))
                        {
                            sqlCommand.Parameters.AddWithValue("@target", target);    
                        }
                        
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader["Inclusive"]).Equals(1))
                            {
                                if (!cIncludes.Contains(Convert.ToInt32(reader["ObjectID"])))
                                {
                                    cIncludes.Add(Convert.ToInt32(reader["ObjectID"]));
                                }
                            }
                            else
                            {
                                if (!cExcludes.Contains(Convert.ToInt32(reader["ObjectID"])))
                                {
                                    cExcludes.Add(Convert.ToInt32(reader["ObjectID"]));
                                }
                            }
                        }
                    }
                    connection.Close();
                    bool fleetRemoval = false;
                    if (includeList != null)
                    {
                        foreach (int include in includeList)
                        {
                            if (!cIncludes.Contains(include))
                            {
                                DataRow dataRow = includes.NewRow();
                                dataRow["ServiceConfigID"] = serviceConfigId;
                                dataRow["Objects"] = target;
                                dataRow["ObjectID"] = Convert.ToInt64(include);
                                dataRow["StDate"] = createdDatetime;
                                dataRow["EndDate"] = DBNull.Value;
                                dataRow["Inclusive"] = true;
                                dataRow["Deleted"] = 0;
                                includes.Rows.Add(dataRow);
                            }
                        }
                        foreach (int cInclude in cIncludes)
                        {

                            if (!includeList.Contains(cInclude))
                            {
                                DataRow dataRow = updates.NewRow();
                                dataRow["ServiceConfigID"] = serviceConfigId;
                                dataRow["Objects"] = target;
                                dataRow["ObjectID"] = Convert.ToInt64(cInclude);
                                //dataRow["StDate"] = createdDatetime;
                                dataRow["EndDate"] = createdDatetime;
                                dataRow["Inclusive"] = false;
                                dataRow["Deleted"] = 1;
                                updates.Rows.Add(dataRow);
                                fleetRemoval = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (int cInclude in cIncludes)
                        {
                            DataRow dataRow = updates.NewRow();
                            dataRow["ServiceConfigID"] = serviceConfigId;
                            dataRow["Objects"] = target;
                            dataRow["ObjectID"] = Convert.ToInt64(cInclude);
                            //dataRow["StDate"] = createdDatetime;
                            dataRow["EndDate"] = createdDatetime;
                            dataRow["Inclusive"] = true;
                            dataRow["Deleted"] = 1;
                            updates.Rows.Add(dataRow);
                            fleetRemoval = true;
                        }
                    }

                    if (excludeList != null)
                    {
                        foreach (int exclude in excludeList)
                        {
                            if (!cExcludes.Contains(exclude))
                            {
                                DataRow dataRow = includes.NewRow();
                                dataRow["ServiceConfigID"] = serviceConfigId;
                                dataRow["Objects"] = "Vehicle";
                                dataRow["ObjectID"] = Convert.ToInt64(exclude);
                                dataRow["StDate"] = createdDatetime;
                                dataRow["EndDate"] = DBNull.Value;
                                dataRow["Inclusive"] = false;
                                dataRow["Deleted"] = 0;
                                includes.Rows.Add(dataRow);
                            }
                        }
                        foreach (int cExclude in cExcludes)
                        {
                            if (!excludeList.Contains(cExclude))
                            {
                                DataRow dataRow = updates.NewRow();
                                dataRow["ServiceConfigID"] = serviceConfigId;
                                dataRow["Objects"] = "Vehicle";
                                dataRow["ObjectID"] = Convert.ToInt64(cExclude);
                                //dataRow["StDate"] = createdDatetime;
                                dataRow["EndDate"] = createdDatetime;
                                dataRow["Inclusive"] = false;
                                dataRow["Deleted"] = 1;
                                updates.Rows.Add(dataRow);
                            }
                        }
                    }
                    else
                    {
                        foreach (int cExclude in cExcludes)
                        {
                            DataRow dataRow = updates.NewRow();
                            dataRow["ServiceConfigID"] = serviceConfigId;
                            dataRow["Objects"] = "Vehicle";
                            dataRow["ObjectID"] = Convert.ToInt64(cExclude);
                            //dataRow["StDate"] = createdDatetime;
                            dataRow["EndDate"] = createdDatetime;
                            dataRow["Inclusive"] = false;
                            dataRow["Deleted"] = 1;
                            updates.Rows.Add(dataRow);
                        }
                    }
                    if (!string.IsNullOrEmpty(target))
                    {
                        if (target.ToLower().Equals("fleet") && fleetRemoval)
                        {
                            foreach (int cInclude in cIncludes)
                            {
                                foreach (int cExclude in cExcludes)
                                {
                                    if (BelongsToFleet(cExclude, cInclude))
                                    {
                                        DataRow dataRow = updates.NewRow();
                                        dataRow["ServiceConfigID"] = serviceConfigId;
                                        dataRow["Objects"] = "Vehicle";
                                        dataRow["ObjectID"] = Convert.ToInt64(cExclude);
                                        //dataRow["StDate"] = createdDatetime;
                                        dataRow["EndDate"] = createdDatetime;
                                        dataRow["Inclusive"] = false;
                                        dataRow["Deleted"] = 1;
                                        updates.Rows.Add(dataRow);
                                    }
                                }
                            }
                        }
                    }
                    

                    BatchSave(includes);
                    BatchSave(updates, true);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static void BatchSave(DataTable includesAndExcludes, bool update = false)
        {
            if (includesAndExcludes.Rows.Count < 1)
            {
                return;
            }
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    if (!update)
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                        {
                            sqlBulkCopy.DestinationTableName = "vlfServiceAssignments";
                            sqlBulkCopy.WriteToServer(includesAndExcludes);
                        }
                    }
                    else
                    {
                        using (SqlCommand sqlCommand = new SqlCommand())
                        {
                            foreach (DataRow dataRow in includesAndExcludes.Rows)
                            {
                                string processedSql = null;
                                if (dataRow["Objects"] != DBNull.Value && dataRow["Objects"] != null)
                                {
                                    string updateSql = "UPDATE dbo.vlfServiceAssignments SET Inclusive={0}, Deleted={1}, Enddate='{5}'"
                                                  +
                                                  " WHERE ServiceConfigID={2} AND Objects='{3}' AND ObjectID={4}";
                                    processedSql = string.Format(updateSql, Convert.ToInt32(dataRow["Inclusive"]), dataRow["Deleted"], dataRow["ServiceConfigId"], dataRow["Objects"], dataRow["ObjectID"], dataRow["EndDate"]);    
                                }
                                else
                                {
                                    string updateSql = "UPDATE dbo.vlfServiceAssignments SET Inclusive={0}, Deleted={1}, Enddate='{4}'"
                                                  +
                                                  " WHERE ServiceConfigID={2} AND ObjectID={3}";
                                    processedSql = string.Format(updateSql, Convert.ToInt32(dataRow["Inclusive"]), dataRow["Deleted"], dataRow["ServiceConfigId"], dataRow["ObjectID"], dataRow["EndDate"]);
                                }
                                
                                sqlCommand.CommandText += processedSql;

                            }
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.Connection = connection;
                            sqlCommand.ExecuteNonQuery();
                        }                                                
                    }
                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }


        public static void SaveSingleAssignment(int serviceConfigId, int oid, string target, DateTime createDateTime)
        {
            SqlConnection connection = null;
            try
            {
                string searchSql =
                    "SELECT COUNT(1) AS NUM FROM vlfServiceAssignments WHERE ServiceConfigId=@serviceConfigId AND Objects=@objects AND ObjectID=@objectId AND Deleted=0";
                string insertionSql =
                    "INSERT INTO vlfServiceAssignments (ServiceConfigId, Objects, ObjectID, StDate, Inclusive) VALUES (@serviceConfigId, @objects, @objectId, @stDate, 1)";
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(searchSql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@objects", target);
                        sqlCommand.Parameters.AddWithValue("@objectId", oid);
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        bool thereis = false;
                        if (reader.Read())
                        {
                            thereis = Convert.ToInt32(reader["NUM"]) > 0;
                        }
                        reader.Close();
                        if (!thereis)
                        {
                            sqlCommand.CommandText = insertionSql;
                            sqlCommand.Parameters.AddWithValue("@stDate", createDateTime);
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static bool BelongsToFleet(int vehicleId, int fleetId)
        {
            string sql =
                "SELECT COUNT(*) AS NUM FROM dbo.vlfFleetVehicles WHERE FleetId=@fleetId AND VehicleId=@vehicleId";
            //string conn = ConfigurationSettings.AppSettings["SentinelFMConnection"];
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@fleetId", fleetId);
                        sqlCommand.Parameters.AddWithValue("@vehicleId", vehicleId);
                        sqlCommand.CommandType = CommandType.Text;
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        reader.Read();
                        if (Convert.ToInt32(reader["NUM"]) > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return false;
        }

        public static DataTable GetConfiguredServices(int serviceId, int organizationId, int userId, string serviceName = null)
        {
            try
            {
                string sql = "SELECT vlfServiceConfigurations.*, vlfServiceConfigNotification.* FROM vlfServiceConfigurations " +
                            "INNER JOIN vlfOrganizationServices ON vlfOrganizationServices.ServiceID=vlfServiceConfigurations.ServiceID " +
                    // "INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +    
                            "LEFT JOIN vlfServiceConfigNotification ON vlfServiceConfigurations.ServiceConfigID = vlfServiceConfigNotification.ServiceConfigId " +
                             "WHERE vlfServiceConfigurations.ServiceId=@serviceId AND vlfServiceConfigurations.OrganizationID = vlfOrganizationServices.OrganizationID " +
                    // "AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) AND vlfServiceConfigurations.OrganizationID=@organizationId " +
                             "AND vlfOrganizationServices.OrganizationID=@organizationId AND vlfServiceConfigurations.ExpiredDate IS NULL";
                if (!string.IsNullOrEmpty(serviceName))
                {
                    string rawSql =
                        "SELECT vlfServiceConfigurations.*, vlfServiceConfigNotification.* FROM vlfServiceConfigurations " +
                         "INNER JOIN vlfOrganizationServices ON vlfOrganizationServices.ServiceID=vlfServiceConfigurations.ServiceID " +
                        // "INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                        "LEFT JOIN vlfServiceConfigNotification ON vlfServiceConfigurations.ServiceConfigID = vlfServiceConfigNotification.ServiceConfigId " +
                        "WHERE  vlfServiceConfigurations.ServiceId=@serviceId AND vlfOrganizationServices.OrganizationID=@organizationId AND vlfServiceConfigurations.OrganizationID = vlfOrganizationServices.OrganizationID  " +
                        // "AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) AND vlfServiceConfigurations.OrganizationID=@organizationId " +
                        "AND vlfServiceConfigurations.ExpiredDate IS NULL " +
                        "AND vlfServiceConfigurations.ServiceConfigName LIKE '%{0}%'";
                    sql = string.Format(rawSql, serviceName);
                }
                if (organizationId.Equals(480))
                {
                    sql += " UNION SELECT vlfServiceConfigurations.*, vlfServiceConfigNotification.* FROM vlfServiceConfigurations LEFT JOIN vlfServiceConfigNotification ON vlfServiceConfigurations.ServiceConfigID = vlfServiceConfigNotification.ServiceConfigId WHERE vlfServiceConfigurations.OrganizationID=0 AND vlfServiceConfigurations.ExpiredDate IS NULL";
                }
                sql += " ORDER BY vlfServiceConfigurations.ServiceConfigName";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@serviceId", serviceId);
                        sqlCommand.Parameters.AddWithValue("@organizationId", organizationId);
                        sqlCommand.Parameters.AddWithValue("@userId", userId);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetConfiguredService(int configuredServiceId)
        {
            try
            {
                string sql = "SELECT * FROM vlfServiceConfigurations LEFT JOIN vlfServiceConfigNotification ON vlfServiceConfigurations.ServiceConfigID = vlfServiceConfigNotification.ServiceConfigId WHERE vlfServiceConfigurations.ServiceConfigID=@serviceconfigId";                

                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@serviceconfigId", configuredServiceId);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetAssignedFleetsAndExcludedVehicles(int selectedConfiguredId, string target)
        {
            try
            {
                string sql = "SELECT * FROM vlfServiceAssignments WHERE ServiceConfigId=@serviceConfigId AND (Objects=@target OR (Objects='Vehicle' AND Inclusive=0)) AND Deleted=0 ORDER BY Objects ASC";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", selectedConfiguredId);
                        sqlCommand.Parameters.AddWithValue("@target", target);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetLandmarkNameByLandmarkId(int landmarkId)
        {
            try
            {
                string sql = "SELECT LandmarkName FROM vlfLandmark WHERE LandmarkId=@landamrkId";
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@landamrkId", landmarkId);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                        DataTable dataTable = dataSet.Tables[0];
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetFilteredResults(Dictionary<string, string> conditions, int organizationId, int userId, ref int totalCount, int serviceId = 0, int serviceConfigId = 0, bool involveDelete = false)
        {
            string columnName = "vlfVehicleInfo.Description";

            if (conditions.ContainsKey("searchCriteria"))
            {
                if (conditions["searchCriteria"].ToLower().Equals("fleet"))
                {
                    columnName = "vlfFleet.FleetName";
                }
                else if (conditions["searchCriteria"].ToLower().Equals("landmark") || conditions["searchCriteria"].ToLower().Equals("route"))
                {
                    columnName = "vlfLandmark.LandmarkName";
                }
            }
           
            
            string serviceCondition = "";
            if (serviceId > 0)
            {
                serviceCondition = string.Format(" AND vlfServiceConfigurations.ServiceID={0} ", serviceId);
            }

            string serviceConfigCondition = "";
            if (serviceConfigId > 0)
            {
                serviceConfigCondition = string.Format(" AND vlfServiceConfigurations.ServiceConfigID={0} ",
                    serviceConfigId);
            }



            bool sSearchCriteriaRoute = conditions.ContainsKey("sCriterialRoute");


            IList<string> sColumns = new List<string>(new string[] { columnName, "vlfServiceConfigurations.ServiceConfigName", "vlfServiceAssignments.Objects", "vlfServiceConfigurations.RulesApplied", "vlfServiceAssignments.Stdate", "vlfServiceAssignments.Enddate", "vlfUser.UserName" });
            IList<string> oColumns = new List<string>(new string[] { "TempResultSets.objectname", "TempResultSets.ServiceConfigName", "TempResultSets.Objects", "TempResultSets.RulesApplied", "TempResultSets.CreatedDate", "TempResultSets.ExpiredDate", "TempResultSets.UserName" });
            string fleetSql = "SELECT " + columnName + " AS objectname, vlfServiceConfigurations.ServiceConfigName, vlfServiceAssignments.Objects, vlfUser.UserName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.Stdate AS CreatedDate, vlfServiceAssignments.Enddate AS ExpiredDate, vlfFleet.FleetId as myobjectid, vlfServiceAssignments.ServiceConfigId, vlfServiceAssignments.Deleted, vlfServiceAssignments.Enddate FROM vlfServiceAssignments " +
                             "INNER JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceAssignments.ServiceConfigID " +
                             "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +                            
                             "INNER JOIN vlfFleet ON vlfServiceAssignments.ObjectId = vlfFleet.FleetId AND vlfFleet.OrganizationId = vlfServiceConfigurations.OrganizationID " +
                             "WHERE vlfServiceConfigurations.OrganizationID={1} " +                              
                              "{2} {3} AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Objects='Fleet' {4} {0} ";
            string vehicleSql = "SELECT " + columnName + " AS objectname, vlfServiceConfigurations.ServiceConfigName, vlfServiceAssignments.Objects, vlfUser.UserName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.Stdate AS CreatedDate, vlfServiceAssignments.Enddate AS ExpiredDate, vlfVehicleInfo.VehicleId as myobjectid, vlfServiceAssignments.ServiceConfigId, vlfServiceAssignments.Deleted, vlfServiceAssignments.Enddate FROM vlfServiceAssignments " +
                             "INNER JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceAssignments.ServiceConfigID " +                             
                             "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                             "INNER JOIN vlfFleetVehicles ON vlfFleetVehicles.FleetId = vlfServiceAssignments.ObjectID " +
                             "INNER JOIN vlfVehicleInfo ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId AND vlfVehicleInfo.OrganizationId = vlfServiceConfigurations.OrganizationID " +
                             "INNER JOIN vlfFleet ON vlfServiceAssignments.ObjectId = vlfFleet.FleetId " +
                             "WHERE vlfServiceConfigurations.OrganizationID={1} " +                               
                                "{2} {3} AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Objects='Fleet' {4} {0} " +
                             " UNION " +
                             "SELECT " + columnName + " AS objectname, vlfServiceConfigurations.ServiceConfigName, vlfServiceAssignments.Objects, vlfUser.UserName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.Stdate AS CreatedDate, vlfServiceAssignments.Enddate AS ExpiredDate,  vlfVehicleInfo.VehicleId as myobjectid, vlfServiceAssignments.ServiceConfigId, vlfServiceAssignments.Deleted, vlfServiceAssignments.Enddate FROM vlfServiceAssignments " +
                             "INNER JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceAssignments.ServiceConfigID " +                             
                             "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                             "INNER JOIN vlfVehicleInfo ON vlfServiceAssignments.ObjectID = vlfVehicleInfo.VehicleId AND vlfVehicleInfo.OrganizationId = vlfServiceConfigurations.OrganizationID " +
                             "WHERE vlfServiceConfigurations.OrganizationID={1} " +                                
                                "{2} {3} AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Objects='Vehicle' {4} {0} ";
            string landmarkSql = "SELECT " + columnName + " AS objectname, vlfServiceConfigurations.ServiceConfigName, vlfServiceAssignments.Objects, vlfUser.UserName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.Stdate AS CreatedDate, vlfServiceAssignments.Enddate AS ExpiredDate, vlfLandmark.LandmarkId as myobjectid, vlfServiceAssignments.ServiceConfigId, vlfServiceAssignments.Deleted, vlfServiceAssignments.Enddate FROM vlfServiceAssignments " +
                             "INNER JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceAssignments.ServiceConfigID " +                        
                             "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                             "INNER JOIN vlfLandmark as vlfLandmark ON vlfLandmark.LandmarkId = vlfServiceAssignments.ObjectID AND vlfLandmark.OrganizationId = vlfServiceConfigurations.OrganizationID " +
                             "WHERE vlfServiceConfigurations.OrganizationID={1} " +                                 
                                 "AND ISNULL(LandmarkType, -1)<>'ROUTE' AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Objects='Landmark' AND vlfServiceAssignments.Deleted=0 {0} ";
            string routeSql = "SELECT " + columnName + " AS objectname, vlfServiceConfigurations.ServiceConfigName, vlfServiceAssignments.Objects, vlfUser.UserName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.Stdate AS CreatedDate, vlfServiceAssignments.Enddate AS ExpiredDate, vlfLandmark.LandmarkId as myobjectid, vlfServiceAssignments.ServiceConfigId, vlfServiceAssignments.Deleted, vlfServiceAssignments.Enddate FROM vlfServiceAssignments " +
                             "INNER JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceAssignments.ServiceConfigID " +                             
                             "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                             "INNER JOIN vlfLandmark as vlfLandmark ON vlfLandmark.LandmarkId = vlfServiceAssignments.ObjectID AND vlfLandmark.OrganizationId = vlfServiceConfigurations.OrganizationID " +
                             "WHERE vlfServiceConfigurations.OrganizationID={1} " +                              
                              "{2} {3} AND ISNULL(LandmarkType, -1)='ROUTE' AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Objects='Landmark' {4} {0} ";

            if (sSearchCriteriaRoute)
            {
                vehicleSql = "SELECT " + columnName + " AS objectname, vlfServiceConfigurations.ServiceConfigName, vlfServiceAssignments.Objects, vlfUser.UserName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.Stdate AS CreatedDate, vlfServiceAssignments.Enddate AS ExpiredDate,  vlfVehicleInfo.VehicleId as myobjectid, vlfServiceAssignments.ServiceConfigId, vlfServiceAssignments.Deleted, vlfServiceAssignments.Enddate FROM vlfServiceAssignments " +                             
                             "INNER JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceAssignments.ServiceConfigID " +                           
                             "INNER JOIN vlfEventServices ON vlfEventServices.ServiceID = vlfServiceConfigurations.ServiceID " +
                             "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                             "INNER JOIN vlfVehicleInfo ON vlfServiceAssignments.ObjectID = vlfVehicleInfo.VehicleId AND vlfVehicleInfo.OrganizationId = vlfServiceConfigurations.OrganizationID " +
                             "WHERE vlfServiceConfigurations.OrganizationID={1} " +                             
                             "{2} {3} AND vlfEventServices.ServiceID = 4 AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Objects='Vehicle' {4} {0} ";
            }
            string coreSql = fleetSql;
            if (conditions["searchCriteria"].ToLower().Equals("vehicle"))
            {
                coreSql = vehicleSql;
            }
            else if (conditions["searchCriteria"].ToLower().Equals("landmark"))
            {
                coreSql = landmarkSql;
            }
            else if (conditions["searchCriteria"].ToLower().Equals("route"))
            {
                coreSql = routeSql;
            }
            string queryString = ";;WITH TempResultSets AS({0})," +
                                 " ResultSets AS (SELECT *, ROW_NUMBER() OVER ({3}) AS RowNum FROM TempResultSets)" +
                                 " SELECT * FROM ResultSets WHERE RowNum >= {1} AND RowNum < {1} + {2};";

            try
            {
                //paging
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                    }
                }

                //Ordering
                string sOrder = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (
                            conditions[
                                string.Format("bSortable_{0}",
                                              Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            tmpOrder = string.Format("{0} {1}", oColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                              (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                   ? "ASC"
                                                                   : "DESC"));
                        }
                        if (string.IsNullOrEmpty(sortedColumns))
                        {
                            sortedColumns = tmpOrder;
                        }
                        else
                        {
                            sortedColumns += "," + tmpOrder;
                        }
                    }
                    sOrder = string.Format(sOrder, sortedColumns);
                }


                //filtering
                string sWhere = "";
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere = "AND ({0})";
                        string tmpWhere = "";
                        int mySearchColumn = 0;
                        if (conditions.ContainsKey("searchColumn"))
                        {
                            mySearchColumn = Convert.ToInt32(conditions["searchColumn"]);
                        }
                        if (mySearchColumn > 0)
                        {
                            tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[mySearchColumn], conditions["sSearch"]);
                        }
                        else
                        {
                            for (int i = 0; i < sColumns.Count; i++)
                            {
                                if (sColumns[i].Equals("createddatetime"))
                                {
                                    continue;
                                }
                                if (string.IsNullOrEmpty(tmpWhere))
                                {
                                    tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                                }
                                else
                                {
                                    tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                                }
                            }
                        }
                        

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }

                string involveDeleteStr = (!involveDelete ? "AND vlfServiceAssignments.Deleted=0" : String.Empty);
                string querySql = string.Format(coreSql, sWhere, organizationId, serviceCondition, serviceConfigCondition, involveDeleteStr);                
                string completeSql = string.Format(queryString, querySql, sLimit, sLength, sOrder);
                DataTable dataTable = new DataTable();
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand commandCount = new SqlCommand(querySql, connection))
                    {

                        commandCount.CommandType = CommandType.Text;

                        SqlDataReader sqlDataReader = commandCount.ExecuteReader();

                        while (sqlDataReader.Read())
                        {
                            totalCount++;
                        }
                        sqlDataReader.Close();


                    }

                    using (SqlCommand command = new SqlCommand(completeSql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetFilteredUnAssignedServices(Dictionary<string, string> conditions, int organizationId, int userId, int serviceId,
                                                     ref int totalCount)
        {
            string coreSql = "SELECT vlfServiceConfigurations.ServiceConfigID, vlfServiceConfigurations.ServiceConfigName, vlfServiceConfigurations.CreatedDate, vlfServiceConfigurations.ExpiredDate, vlfUser.UserName FROM vlfServiceConfigurations " +
                       // "INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                        "LEFT JOIN vlfServiceAssignments  ON vlfServiceAssignments.ServiceConfigID = vlfServiceConfigurations.ServiceConfigId " +
                         "INNER JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                        "WHERE vlfServiceConfigurations.OrganizationID={1} " +
                             //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={3}) AND vlfServiceConfigurations.ServiceID={2} " +
                             "AND Objects IS NULL {0}";
            string queryString = ";;WITH TempResultSets AS({0})," +
                                 " ResultSets AS (SELECT *, ROW_NUMBER() OVER ({3}) AS RowNum FROM TempResultSets)" +
                                 " SELECT * FROM ResultSets WHERE RowNum >= {1} AND RowNum < {1} + {2};";

            IList<string> sColumns = new List<string>(new string[] { "vlfServiceConfigurations.ServiceConfigName", "vlfServiceConfigurations.CreatedDate", "vlfServiceConfigurations.ExpiredDate", "vlfUser.UserName" });
            IList<string> oColumns = new List<string>(new string[] { "TempResultSets.ServiceConfigName", "TempResultSets.CreatedDate", "TempResultSets.ExpiredDate", "TempResultSets.UserName" });
            try
            {
                //paging
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                    }
                }

                //Ordering
                string sOrder = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (
                            conditions[
                                string.Format("bSortable_{0}",
                                              Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            tmpOrder = string.Format("{0} {1}", oColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                              (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                   ? "ASC"
                                                                   : "DESC"));
                        }
                        if (string.IsNullOrEmpty(sortedColumns))
                        {
                            sortedColumns = tmpOrder;
                        }
                        else
                        {
                            sortedColumns += "," + tmpOrder;
                        }
                    }
                    sOrder = string.Format(sOrder, sortedColumns);
                }


                //filtering
                string sWhere = "";
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere = "AND ({0})";
                        string tmpWhere = "";
                        int mySearchColumn = 0;
                        if (conditions.ContainsKey("searchColumn"))
                        {
                            mySearchColumn = Convert.ToInt32(conditions["searchColumn"]);
                        }
                        if (mySearchColumn > 0)
                        {
                            tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[mySearchColumn], conditions["sSearch"]);
                        }
                        else
                        {
                            for (int i = 0; i < sColumns.Count; i++)
                            {
                                if (sColumns[i].Equals("createddatetime"))
                                {
                                    continue;
                                }
                                if (string.IsNullOrEmpty(tmpWhere))
                                {
                                    tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                                }
                                else
                                {
                                    tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                                }
                            }
                        }


                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }

                string querySql = string.Format(coreSql, sWhere, organizationId, serviceId);
                string completeSql = string.Format(queryString, querySql, sLimit, sLength, sOrder);
                DataTable dataTable = new DataTable();
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand commandCount = new SqlCommand(querySql, connection))
                    {
                        commandCount.CommandType = CommandType.Text;

                        SqlDataReader sqlDataReader = commandCount.ExecuteReader();

                        while (sqlDataReader.Read())
                        {
                            totalCount++;
                        }
                        sqlDataReader.Close();
                    }

                    using (SqlCommand command = new SqlCommand(completeSql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }

        }

        public static DataTable GetFilteredLandmarks(Dictionary<string, string> conditions, int organizationId, int userId,
                                                     ref int totalCount)
        {
            IList<string> sColumns = new List<string>(new string[] { "LandmarkId", "LandmarkName" });


            try
            {
                int serviceTypeId = 0;
                if (conditions.ContainsKey("serviceType"))
                {
                    serviceTypeId = Convert.ToInt32(conditions["serviceType"]);
                }

                //paging
                string countSql = "SELECT COUNT(1) AS NUM FROM vlfLandmark " +
                                  //"INNER JOIN vlfUserGroupAssignment ON vlfLandmark.CreateUserID = vlfUserGroupAssignment.UserId " +
                                  "WHERE OrganizationId={0} " +
                                  //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={2}) " + 
                                  (serviceTypeId.Equals(4) ? "AND LandmarkType='ROUTE' " : "AND ISNULL(LandmarkType, -1)<>'ROUTE'") + " {1}";
                string sql = ";;WITH ResultSets AS(SELECT LandmarkId, LandmarkName, ROW_NUMBER() OVER ({3}) AS RowNum FROM vlfLandmark INNER JOIN vlfUserGroupAssignment ON vlfLandmark.CreateUserID = vlfUserGroupAssignment.UserId WHERE OrganizationId={0} AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment) " + (serviceTypeId.Equals(4) ? "AND LandmarkType='ROUTE' " : "AND ISNULL(LandmarkType, -1)<>'ROUTE'") + " {4})" +
                                     " SELECT * FROM ResultSets WHERE RowNum >= {1} AND RowNum < {1} + {2};";
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                    }
                }

                //Ordering
                string sOrder = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (
                            conditions[
                                string.Format("bSortable_{0}",
                                              Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            tmpOrder = string.Format("{0} {1}", sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                              (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                   ? "ASC"
                                                                   : "DESC"));
                        }
                        if (string.IsNullOrEmpty(sortedColumns))
                        {
                            sortedColumns = tmpOrder;
                        }
                        else
                        {
                            sortedColumns += "," + tmpOrder;
                        }
                    }
                    sOrder = string.Format(sOrder, sortedColumns);
                }


                //filtering
                string sWhere = "";
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere = "AND ({0})";
                        string tmpWhere = "";
                        for (int i = 0; i < sColumns.Count; i++)
                        {
                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }

                string querySql = string.Format(countSql, organizationId, sWhere);
                //string countQuerySql = string.Format(countSql, sWhere, organizationId, conditions["searchCriteria"]);
                string completeSql = string.Format(sql, organizationId, sLimit, sLength, sOrder, sWhere);
                DataTable dataTable = new DataTable();
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                //string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand commandCount = new SqlCommand(querySql, connection))
                    {

                        commandCount.CommandType = CommandType.Text;

                        SqlDataReader sqlDataReader = commandCount.ExecuteReader();
                        sqlDataReader.Read();
                        totalCount = Convert.ToInt32(sqlDataReader["NUM"]);
                        sqlDataReader.Close();
                    }

                    using (SqlCommand command = new SqlCommand(completeSql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetFilteredDtcValues(Dictionary<string, string> conditions,
                                                     ref int totalCount)
        {
            IList<string> sColumns = new List<string>(new string[] { "DTCCode", "text" });


            try
            {
                string dtcTypeId = string.Empty;
                if (conditions.ContainsKey("dtcType"))
                {
                    dtcTypeId = conditions["dtcType"];
                }

                //paging
                string countSql = "SELECT COUNT(1) AS NUM FROM DTCCodes " +
                                  "WHERE " +
                                  "DtcTypeId in ({0})"
                                  + " {1}";
                string sql = ";;WITH ResultSets AS(SELECT DTCCode, text, ROW_NUMBER() OVER ({2}) AS RowNum FROM DTCCodes WHERE "
                             + " DtcTypeId in ({3}) {4}) " +
                                     " SELECT * FROM ResultSets WHERE RowNum >= {0} AND RowNum < {0} + {1};";
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                    }
                }

                //Ordering
                string sOrder = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (
                            conditions[
                                string.Format("bSortable_{0}",
                                              Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            tmpOrder = string.Format("{0} {1}", sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                              (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                   ? "ASC"
                                                                   : "DESC"));
                        }
                        if (string.IsNullOrEmpty(sortedColumns))
                        {
                            sortedColumns = tmpOrder;
                        }
                        else
                        {
                            sortedColumns += "," + tmpOrder;
                        }
                    }
                    sOrder = string.Format(sOrder, sortedColumns);
                }


                //filtering
                string sWhere = "";
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere = "AND ({0})";
                        string tmpWhere = "";
                        for (int i = 0; i < sColumns.Count; i++)
                        {
                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }

                string querySql = string.Format(countSql, dtcTypeId, sWhere);
                //string countQuerySql = string.Format(countSql, sWhere, organizationId, conditions["searchCriteria"]);
                string completeSql = string.Format(sql, sLimit, sLength, sOrder, dtcTypeId, sWhere);
                DataTable dataTable = new DataTable();
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                //string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand commandCount = new SqlCommand(querySql, connection))
                    {

                        commandCount.CommandType = CommandType.Text;

                        SqlDataReader sqlDataReader = commandCount.ExecuteReader();
                        sqlDataReader.Read();
                        totalCount = Convert.ToInt32(sqlDataReader["NUM"]);
                        sqlDataReader.Close();
                    }

                    using (SqlCommand command = new SqlCommand(completeSql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        } 

        public static DataTable GetFilteredFleetAndVehicleExceptions(Dictionary<string, string> conditions, int organizationId, int userId,
                                                     ref int totalCount)
        {
            string coreSql = "";
            string sql = "{3} " + ";;WITH ResultSets AS({0})" +
                                 " SELECT * FROM ResultSets WHERE RowNum >= {1} AND RowNum < {1} + {2};";
            string orderSql = ",ROW_NUMBER() OVER ({0}) AS RowNum";
            string cacheVlfFleetVehicles = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  {0} ";
            string tmpcacheVlfFleetVehicles = "";
            string sColumn = "";
            int serviceConfigId = Convert.ToInt32(conditions["serviceConfigId"]);
            if (conditions["searchFor"].ToLower().Equals("fleet"))
            {
                coreSql = "SELECT vlfVehicleInfo.Description as objectname, vlfStateProvince.TimeZone, vlfStateProvince.DayLightSaving, vlfServiceConfigurations.ServiceID, vlfServiceConfigurations.RulesApplied, vlfLandmark.LandmarkName, vlfServiceConfigurations.ServiceConfigName, evtViolations.* {2} FROM evtViolations " +                         
                          "INNER JOIN vlfVehicleInfo ON vlfVehicleInfo.VehicleId = evtViolations.VehicleID " +
                          "INNER JOIN vlfStateProvince ON vlfStateProvince.StateProvince = vlfVehicleInfo.StateProvince " +
                          "INNER JOIN vlfServiceConfigurations ON evtViolations.ServiceConfigId = vlfServiceConfigurations.ServiceConfigID " +
                          //"INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                          "LEFT JOIN vlfLandmark ON evtViolations.LandmarkID = vlfLandmark.LandmarkId AND vlfLandmark.OrganizationId = evtViolations.OrganizationId " +
                          "WHERE evtViolations.OrganizationId = {0} " +      
                          //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={4}) " +
                          "AND evtViolations.VehicleID IN (SELECT f FROM @t)" +
                          "AND evtViolations.ServiceConfigId = {1} {3}";

                sColumn = "vlfVehicleInfo.Description";
                if (conditions.ContainsKey("searchField"))
                {
                    tmpcacheVlfFleetVehicles = "DECLARE @t table (f int); " +
                "INSERT INTO @t (f) " +
                "SELECT VehicleId from vlfFleetVehicles WHERE FleetId={0};";
                    tmpcacheVlfFleetVehicles = string.Format(tmpcacheVlfFleetVehicles, conditions["searchField"]); 
                }
                               
            }
            else if (conditions["searchFor"].ToLower().Equals("vehicle"))
            {
                coreSql = "SELECT vlfVehicleInfo.Description as objectname, vlfStateProvince.TimeZone, vlfStateProvince.DayLightSaving, vlfServiceConfigurations.ServiceID, vlfServiceConfigurations.RulesApplied, vlfLandmark.LandmarkName, vlfServiceConfigurations.ServiceConfigName, evtViolations.* {3} FROM evtViolations " +
                          "INNER JOIN vlfVehicleInfo ON vlfVehicleInfo.VehicleId = evtViolations.VehicleID " +
                          "INNER JOIN vlfStateProvince ON vlfStateProvince.StateProvince = vlfVehicleInfo.StateProvince " +
                          "INNER JOIN vlfServiceConfigurations ON evtViolations.ServiceConfigId = vlfServiceConfigurations.ServiceConfigID " +
                          //"INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                          "LEFT JOIN vlfLandmark ON evtViolations.LandmarkID = vlfLandmark.LandmarkId AND vlfLandmark.OrganizationId = evtViolations.OrganizationId " +
                          "WHERE evtViolations.OrganizationId = {0} " +
                          //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={5}) " +
                          "AND evtViolations.VehicleId = {1} " +
                          "AND evtViolations.ServiceConfigId = {2} {4}";
                sColumn = "vlfVehicleInfo.Description";
            }
            else
            {
                coreSql = "SELECT vlfVehicleInfo.Description as objectname, vlfStateProvince.TimeZone, vlfStateProvince.DayLightSaving, vlfServiceConfigurations.ServiceID, vlfServiceConfigurations.RulesApplied, vlfLandmark.LandmarkName, vlfServiceConfigurations.ServiceConfigName, evtViolations.* {3} FROM evtViolations " +
                          "INNER JOIN vlfLandmark AS vlfLandmark ON vlfLandmark.LandmarkId = evtViolations.LandmarkId " +
                          "INNER JOIN vlfVehicleInfo ON vlfVehicleInfo.VehicleId = evtViolations.VehicleID " +
                          "INNER JOIN vlfStateProvince ON vlfStateProvince.StateProvince = vlfVehicleInfo.StateProvince " +
                          "INNER JOIN vlfServiceConfigurations ON evtViolations.ServiceConfigId = vlfServiceConfigurations.ServiceConfigID " +
                          //"INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                          "LEFT JOIN vlfLandmark ON evtViolations.LandmarkID = vlfLandmark.LandmarkId AND vlfLandmark.OrganizationId = evtViolations.OrganizationId " +
                          "WHERE evtViolations.OrganizationId = {0} " +
                          //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={5}) " +
                          "AND evtViolations.LandmarkId = {1} " +
                          "AND evtViolations.ServiceConfigId = {2} {4}";
                sColumn = "vlfLandmark.LandmarkName";
            }

            IList<string> sColumns = new List<string>(new string[] { sColumn, "CONVERT(VARCHAR(25), evtViolations.StDate, 126)", "evtViolations.Duration", "evtViolations.Speed", "evtViolations.IsExpired" });
            IList<string> oColumns = new List<string>(new string[] { sColumn, "evtViolations.StDate", "evtViolations.Duration", "evtViolations.Speed", "evtViolations.IsExpired" });

            try
            {
                //paging
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                    }
                }

                //Ordering
                string sOrder = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (
                            conditions[
                                string.Format("bSortable_{0}",
                                              Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            tmpOrder = string.Format("{0} {1}", oColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                              (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                   ? "ASC"
                                                                   : "DESC"));
                        }
                        if (string.IsNullOrEmpty(sortedColumns))
                        {
                            sortedColumns = tmpOrder;
                        }
                        else
                        {
                            sortedColumns += "," + tmpOrder;
                        }
                    }
                    sOrder = string.Format(sOrder, sortedColumns);
                }


                //filtering
                string sWhere = "";
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere = "AND ({0})";
                        string tmpWhere = "";
                        for (int i = 0; i < sColumns.Count; i++)
                        {
                            DateTime triedDatetime;
                            string searchVal = conditions["sSearch"];

                            if (sColumns[i].Contains("StDate"))
                            {
                                if (DateTime.TryParse(conditions["sSearch"], out triedDatetime))
                                {
                                    searchVal = triedDatetime.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], searchVal);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], searchVal);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }
                string completeOrderSql = string.Format(orderSql, sOrder);
                cacheVlfFleetVehicles = string.Format(cacheVlfFleetVehicles, tmpcacheVlfFleetVehicles);
                string countQuerySql = cacheVlfFleetVehicles + string.Format(coreSql, organizationId, conditions["searchField"], serviceConfigId, "", sWhere);
                string querySql = string.Format(coreSql, organizationId, conditions["searchField"], serviceConfigId, completeOrderSql, sWhere);
                if (conditions["searchFor"].ToLower().Equals("fleet"))
                {
                    countQuerySql = cacheVlfFleetVehicles +
                                           string.Format(coreSql, organizationId, 
                                               serviceConfigId, "", sWhere);
                    querySql = string.Format(coreSql, organizationId, serviceConfigId,
                        completeOrderSql, sWhere);
                }
                string completeSql = string.Format(sql, querySql, sLimit, sLength, cacheVlfFleetVehicles);
                if (!string.IsNullOrEmpty(sLength))
                {
                    if (sLength.Equals("-1"))
                    {
                        completeSql = countQuerySql;
                    }
                }
                DataTable dataTable = new DataTable();
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["PreproductionSentinelFmDB"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand commandCount = new SqlCommand(countQuerySql, connection))
                    {

                        commandCount.CommandType = CommandType.Text;

                        SqlDataReader sqlDataReader = commandCount.ExecuteReader();

                        while (sqlDataReader.Read())
                        {
                            totalCount++;
                        }
                        sqlDataReader.Close();


                    }

                    using (SqlCommand command = new SqlCommand(completeSql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable GetFilteredConfiguredRoutesFromDb(Dictionary<string, string> conditions, int organizationId, int userId, ref int totalCount)
        {
            IList<string> sColumns = new List<string>(new string[] { "vlfServiceConfigurations.ServiceConfigName", "vlfServiceConfigurations.RulesApplied" });


            try
            {
                int serviceTypeId = 0;
                if (conditions.ContainsKey("serviceType"))
                {
                    serviceTypeId = Convert.ToInt32(conditions["serviceType"]);
                }

                //paging
                string countSql = "SELECT COUNT(1) AS NUM FROM vlfServiceConfigurations " +
                                  "LEFT JOIN vlfServiceAssignments ON vlfServiceAssignments.ServiceConfigID = vlfServiceConfigurations.ServiceConfigID AND vlfServiceAssignments.Objects='Vehicle' " +
                                  //"INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                                  "WHERE vlfServiceConfigurations.OrganizationID={0} " +
                                  //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={2}) " +
                                  "AND vlfServiceConfigurations.ServiceID=4 AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Deleted=0 {1}";
                string sql = ";;WITH ResultSets AS(SELECT vlfServiceConfigurations.ServiceConfigID, vlfServiceConfigurations.ServiceConfigName, vlfServiceConfigurations.RulesApplied, vlfServiceAssignments.ObjectID, ROW_NUMBER() OVER ({3}) AS RowNum FROM vlfServiceConfigurations " +
                             "LEFT JOIN vlfServiceAssignments ON vlfServiceAssignments.ServiceConfigID = vlfServiceConfigurations.ServiceConfigID AND vlfServiceAssignments.Objects='Vehicle' " +
                             //"INNER JOIN vlfUserGroupAssignment ON vlfServiceConfigurations.UserId = vlfUserGroupAssignment.UserId " +
                             "WHERE OrganizationID={0} " +
                             //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId={5}) " +
                             "AND vlfServiceConfigurations.ServiceID=4 AND vlfServiceConfigurations.ExpiredDate IS NULL AND vlfServiceAssignments.Deleted=0 {4})" +
                                     " SELECT * FROM ResultSets WHERE RowNum >= {1} AND RowNum < {1} + {2};";
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                    }
                }

                //Ordering
                string sOrder = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (
                            conditions[
                                string.Format("bSortable_{0}",
                                              Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            tmpOrder = string.Format("{0} {1}", sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                              (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                   ? "ASC"
                                                                   : "DESC"));
                        }
                        if (string.IsNullOrEmpty(sortedColumns))
                        {
                            sortedColumns = tmpOrder;
                        }
                        else
                        {
                            sortedColumns += "," + tmpOrder;
                        }
                    }
                    sOrder = string.Format(sOrder, sortedColumns);
                }


                //filtering
                string sWhere = "";
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere = "AND ({0})";
                        string tmpWhere = "";
                        for (int i = 0; i < sColumns.Count; i++)
                        {
                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions["sSearch"]);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }

                string querySql = string.Format(countSql, organizationId, sWhere);
                //string countQuerySql = string.Format(countSql, sWhere, organizationId, conditions["searchCriteria"]);
                string completeSql = string.Format(sql, organizationId, sLimit, sLength, sOrder, sWhere);
                DataTable dataTable = new DataTable();
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                //string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand commandCount = new SqlCommand(querySql, connection))
                    {

                        commandCount.CommandType = CommandType.Text;

                        SqlDataReader sqlDataReader = commandCount.ExecuteReader();
                        sqlDataReader.Read();
                        totalCount = Convert.ToInt32(sqlDataReader["NUM"]);
                        sqlDataReader.Close();
                    }

                    using (SqlCommand command = new SqlCommand(completeSql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }


        private static int GetRouteId(int serviceId, string expression)
        {
            if (!serviceId.Equals(1))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(expression))
            {
                return 0;
            }
            SqlConnection connection = null;

            try
            {
                string[] expressionS = expression.Split(new string[] { "LandmarkIn" }, StringSplitOptions.None);
                if (expressionS.Count() > 2)
                {
                    return 0;
                }
                int routerId = 0;
                string[] expressionArray = expression.Split(';');
                foreach (string singleExpression in expressionArray)
                {
                    string[] expressionS1 = singleExpression.Split(' ');
                    if (expressionS1[0].Equals("LandmarkIn"))
                    {
                        routerId = Convert.ToInt32(expressionS1[2]);
                    }
                }

                int count = 0;
                string sql =
                    "SELECT COUNT(1) AS NUM FROM vlfLandmark as vlfLandmark WHERE vlfLandmark.LandmarkId=@landmarkId AND LandmarkType='ROUTE'";
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@landmarkId", routerId);
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        count = Convert.ToInt32(reader["NUM"]);
                    }
                }
                connection.Close();
                return count > 0 ? routerId : 0;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static DataTable GetFilteredDataDB(int organizationId, int userId, string dataName, string table)
        {
            SqlConnection connection = null;
            try
            {
                string sql = string.Empty;
                switch (table.ToLower())
                {
                    case "fleet":
                        sql =
                            string.Format(
                                "SELECT DISTINCT FleetName as name, vlfFleet.FleetId as id FROM vlfFleet " +
                                "INNER JOIN vlfFleetUsers ON vlfFleetUsers.FleetId = vlfFleet.FleetId " +                                
                                //"INNER JOIN vlfUserGroupAssignment ON vlfFleetUsers.UserId = vlfUserGroupAssignment.UserId " +                                
                                "WHERE OrganizationId=@organizationId " +
                                //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) " +
                                "AND LOWER(FleetName) LIKE LOWER('%{0}%')",
                                dataName);
                        break;
                    case "vehicle":
                        sql =
                            string.Format(
                                "SELECT DISTINCT [Description] as name, vlfVehicleInfo.VehicleId as id FROM vlfVehicleInfo " +
                                "INNER JOIN vlfFleetVehicles ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId " +
                                "INNER JOIN vlfFleetUsers ON vlfFleetUsers.FleetId = vlfFleetVehicles.FleetId " +
                                //"INNER JOIN vlfUserGroupAssignment ON vlfFleetUsers.UserId = vlfUserGroupAssignment.UserId " +
                                "WHERE OrganizationId=@organizationId " +
                                //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) " +
                                "AND LOWER([Description]) LIKE LOWER('%{0}%')", dataName);
                        break;
                    case "landmark":
                        sql =
                            string.Format(
                                "SELECT DISTINCT LandmarkName as name, LandmarkId as id FROM vlfLandmark " +
                                //"INNER JOIN vlfUserGroupAssignment ON vlfLandmark.CreateUserID = vlfUserGroupAssignment.UserId " +
                                "WHERE OrganizationId=@organizationId " +
                                //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) " +
                                "AND ISNULL(LandmarkType, -1)<>'ROUTE' " +
                                "AND LOWER([LandmarkName]) LIKE LOWER('%{0}%')", dataName);
                        break;
                    case "route":
                        sql =
                            string.Format(
                                "SELECT DISTINCT LandmarkName as name, LandmarkId as id FROM vlfLandmark " +
                                //"INNER JOIN vlfUserGroupAssignment ON vlfLandmark.CreateUserID = vlfUserGroupAssignment.UserId " +
                                "WHERE OrganizationId=@organizationId " +
                                //"AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId) " +
                                "AND LandmarkType='ROUTE' " +
                                "AND LOWER([LandmarkName]) LIKE LOWER('%{0}%')", dataName);
                        break;
                }
                if(!string.IsNullOrEmpty(sql))
                {
                    using (connection = new SqlConnection(connStr))
                    {
                        connection.Open();
                        DataTable dataTable = new DataTable();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@organizationId", organizationId);
                            //command.Parameters.AddWithValue("@userId", userId);
                            command.CommandType = CommandType.Text;
                            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                            sqlDataAdapter.Fill(dataTable);
                        }
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                
                throw new Exception(exception.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return null;
        }

        public static void DeleteServiceAssignment(int serviceConfigId, int objectId, string target)
        {
            SqlConnection connection = null;
            try
            {
                string deleteSql =
                    "DELETE FROM vlfServiceAssignments WHERE ServiceConfigID=@serviceConfigId AND Objects=@target AND ObjectID=@objectId";
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(deleteSql, connection))
                    {

                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        sqlCommand.Parameters.AddWithValue("@objectId", objectId);
                        sqlCommand.Parameters.AddWithValue("@target", target);
                        sqlCommand.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static bool SaveToServiceRouteMapping(int serviceConfigId, int routeId)
        {
            string sql = "INSERT INTO vlfServiceRouteMappings (ServiceConfigId, RouteId) VALUES(@serviceConfigId, @routeId)";            
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                       sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        sqlCommand.Parameters.AddWithValue("@routeId", routeId);
                        return sqlCommand.ExecuteNonQuery() > 0;
                    }
                    
                    
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }         
        }

        public static bool DeleteServiceConfigMapping(int serviceConfigId, int routeId)
        {
            string searchSql = "SELECT RulesApplied FROM vlfServiceConfigurations WHERE ServiceConfigId=@serviceConfigId";
            string deleteSql = "UPDATE vlfServiceAssignments SET Deleted=1 WHERE ServiceConfigId=@serviceConfigId";
            string deleteSqlConfig = "UPDATE vlfServiceConfigurations SET ExpiredDate=@expiredDate WHERE ServiceConfigId=@serviceConfigId";
            string sql = "DELETE FROM vlfServiceRouteMappings WHERE ServiceConfigId=@serviceConfigId AND RouteId=@routeId";
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(searchSql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        sqlCommand.CommandType = CommandType.Text;                        
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        reader.Read();
                        string rulesApplied = Convert.ToString(reader["RulesApplied"]);
                        string testString = string.Format("LandmarkOut = {0};LandmarkOutNow = 1;", routeId);
                        if (rulesApplied.Equals(testString))
                        {
                            reader.Close();                            
                            sqlCommand.CommandText = deleteSql;                            
                            //sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);                            
                            sqlCommand.ExecuteNonQuery();
                            reader.Close();
                            sqlCommand.CommandText = deleteSqlConfig;
                            sqlCommand.Parameters.AddWithValue("@expiredDate", DateTime.Now);
                            sqlCommand.ExecuteNonQuery();
                        }
                        reader.Close();
                        sqlCommand.Parameters.Clear();
                        sqlCommand.CommandText = sql;
                        sqlCommand.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        sqlCommand.Parameters.AddWithValue("@routeId", routeId);
                        sqlCommand.ExecuteNonQuery();
                    }                                        
                }
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static int MappingExist(int routeId)
        {
            string searchSql =
                "SELECT ServiceConfigId FROM vlfServiceRouteMappings WHERE RouteId=@routeId";
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(searchSql, connection))
                    {                        
                        sqlCommand.Parameters.AddWithValue("@routeId", routeId);
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["ServiceConfigId"]) > 0)
                            {
                                return Convert.ToInt32(reader["ServiceConfigId"]);
                            }    
                        }
                                                                                      
                    }
                }
                return 0;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static DataSet GetColorRuleByVehicle(string vehicle)
        {
            DataSet sqlDataSet = null;
            SQLExecuter sqlExec = new SQLExecuter(connStr);
            string prefixMsg = "Unable to retrieve rules for Vehicle. ";
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ServiceId", SqlDbType.Int, 5);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.Int, Convert.ToInt32(vehicle));

                sqlDataSet = sqlExec.SPExecuteDataset("GetColorRuleByVehicle");
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
            return sqlDataSet;
        }
    }
}
