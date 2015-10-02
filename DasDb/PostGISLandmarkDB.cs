using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace VLF.DAS.DB
{
    public class PostGISLandmarkDB
    {
        private static NpgsqlConnection conn = null;
        //private static string ConnStr = ConfigurationManager.AppSettings["SpatialDB"];       
        public static bool SaveRoute(string connStr, string routePoints, int organizationId, string landmarkName, string contactPerson, int buffer, string email, string contactNo, int timeZone, int daylightSaving, string routerLink, string wayPoints, string description, double latitude, double longitude, ref int landmarkid, int createUserId)
        {
            string sql = "INSERT INTO \"vlfLandmarks\" (LandmarkType, OrganizationId, LandmarkName, Latitude, Longitude, Description, ContactPersonName, ContactPhoneNum, Radius, Email, Phone, TimeZone, DayLightSaving, AutoAdjustDayLightSaving, StreetAddress, CreatedDateTime, AssociatedLandmark, CreateUserID, geom, LandmarkClusterKey, AcrossTo, Layer, OriginalLandmarkid, UpdatedDateTime, routerlink, waypoints) VALUES" +
                             "(@landmarkType, @organizationId, @landmarkName, @latitude, @longitude, @description, @contactPersonName, @contactPhoneNum, @radius, @email, @phone, @timeZone, @dayLightSaving, @autoAdjustDayLightSaving,  @streetAddress, @createdDateTime, @associateLandmark, @createUserId, {0}, {1}, {2}, @layer, @originallandmarkid, @updateddatetime, @routerlink, @waypoints)";
            if (landmarkid > 0)
            {
                sql = "UPDATE \"vlfLandmarks\" SET LandmarkName=@landmarkName, Latitude=@latitude, Longitude=@longitude, " +
                "Description=@description, ContactPersonName=@contactPersonName, ContactPhoneNum=@contactPhoneNum, Radius=@radius, Email=@email, Phone=@phone, TimeZone=@timeZone, " +
                "StreetAddress=@streetAddress, " +
                "geom={0}, LandmarkClusterKey={1}, AcrossTo={2}, Layer=@layer, UpdatedDateTime=@updateddatetime, routerlink=@routerlink, waypoints=@waypoints WHERE originallandmarkid=@landmarkid";
            }            

            landmarkid = SaveTOSQLDb(organizationId, landmarkName, contactPerson, buffer, email, contactNo, timeZone, daylightSaving, description, latitude, longitude, createUserId, landmarkid);

            if (landmarkid > 0)
            {
                if (!routerLink.Contains("landmarkid"))
                {
                    routerLink = string.Format("{0}&landmarkid={1}", routerLink, landmarkid);
                }
            }
            //string routerGeometry = string.Format("ST_LineStringFromWKB(ST_AsBinary(ST_GeomFromText('LINESTRING({0})')), 4326)", routePoints);
            string routerGeometry = string.Format("ST_GeomFromText('LINESTRING({0})', 4326)", routePoints);
            string bufferedRouter = string.Format("bufferedline({0}, {1})", routerGeometry, buffer);
            //string bufferedRouter = string.Format("ST_Buffer_Meters({0}, {1})", routerGeometry, buffer);
           
            string clusterKey = string.Format("ST_Distance(ST_Transform(ST_GeomFromText('POINT(-79.58650145667923 43.67789052257779)',4326),2163), ST_Transform({0},2163))", bufferedRouter);
            string acrossTo = string.Format("ST_MaxDistance(ST_Transform(ST_GeomFromText('POINT(-79.58650145667923 43.67789052257779)',4326),2163), ST_Transform({0},2163))", bufferedRouter);
            string processedSql = string.Format(sql, bufferedRouter, clusterKey, acrossTo);
            try
            {
                using (NpgsqlConnection myConn = new NpgsqlConnection(connStr))
                {    
                    myConn.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(processedSql, myConn))
                    {
                        command.Parameters.AddWithValue("@landmarkType", "ROUTE");
                        command.Parameters.AddWithValue("@organizationId", organizationId);
                        command.Parameters.AddWithValue("@landmarkName", landmarkName);
                        command.Parameters.AddWithValue("@latitude", latitude);
                        command.Parameters.AddWithValue("@longitude", longitude);
                        command.Parameters.AddWithValue("@description", description);

                        command.Parameters.AddWithValue("@contactPersonName",contactPerson);
                        command.Parameters.AddWithValue("@contactPhoneNum", contactNo);
                        command.Parameters.AddWithValue("@radius", buffer);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@phone", null);
                        command.Parameters.AddWithValue("@timeZone", timeZone);

                        command.Parameters.AddWithValue("@dayLightSaving", daylightSaving);
                        command.Parameters.AddWithValue("@autoAdjustDayLightSaving",1);
                        command.Parameters.AddWithValue("@streetAddress", null);

                        command.Parameters.AddWithValue("@createdDateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@updateddatetime", DateTime.Now);
                        command.Parameters.AddWithValue("@associateLandmark", 0);
                        command.Parameters.AddWithValue("@createUserId", createUserId);
                        command.Parameters.AddWithValue("@layer", 0);

                        command.Parameters.AddWithValue("@landmarkClusterKey", clusterKey);
                        command.Parameters.AddWithValue("@acrossTo", acrossTo);
                        command.Parameters.AddWithValue("@originallandmarkid", landmarkid);
                        command.Parameters.AddWithValue("@routerlink", routerLink);
                        command.Parameters.AddWithValue("@waypoints", wayPoints);
                        if (landmarkid > 0)
                        {
                            command.Parameters.AddWithValue("@landmarkid", landmarkid);
                        }
                        int result = command.ExecuteNonQuery();
                        myConn.Close();
                        return result > -1;
                    }
                }
            }
            catch (Exception exception)
            {                
                throw new Exception(exception.Message);
            }
            return true;

        }

        public static bool DeleteRoute(string connStr, int landmarkId)
        {
            if (!DeleteSQLDb(landmarkId))
                return false;
            int serviceConfigId = ServiceAssignmentDB.MappingExist(landmarkId);
            if (serviceConfigId > 0)
            {
                ServiceAssignmentDB.DeleteServiceConfigMapping(serviceConfigId, landmarkId);
            }
            string sql = "delete from \"vlfLandmarks\" where \"originallandmarkid\" = @landmarkId";
            try
            {
                using (NpgsqlConnection myConn = new NpgsqlConnection(connStr))
                {
                    myConn.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, myConn))
                    {
                        command.Parameters.AddWithValue("@landmarkId", landmarkId);
                        int result = command.ExecuteNonQuery();
                        myConn.Close();
                        return result > -1;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static DataTable GetLandmarkInfoById(string connStr, int landmarkId)
        {
            if (landmarkId.Equals(0))
            {
                return null;
            }
            try
            {
                DataTable dataTable = new DataTable();
                using (NpgsqlConnection myConn = new NpgsqlConnection(connStr))
                {
                    myConn.Open();
                    string sql = "SELECT * FROM \"vlfLandmarks\" WHERE originallandmarkId=@landmarkId";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, myConn))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@landmarkId", landmarkId);
                        NpgsqlDataAdapter sqlDataAdapter = new NpgsqlDataAdapter(command);
                        
                        sqlDataAdapter.Fill(dataTable);                        
                    }
                    myConn.Close();
                }
                return dataTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public static bool DeleteSQLDb(int landmarkId)
        {
            try
            {
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string sql =
                        "delete from vlfLandmark where landmarktype = 'ROUTE' and landmarkid = @landmarkId";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@landmarkid", landmarkId);
                        sqlCommand.ExecuteNonQuery();
                        connection.Close();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static int SaveTOSQLDb(int organizationId, string landmarkName, string contactPerson, int buffer, string email, string contactNo, int timeZone, int daylightSaving, string description, double latitude, double longitude, int createUserId, int landmarkId)
        {
            try
            {
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string sql =
                        "INSERT INTO vlfLandmark (OrganizationId, LandmarkName, Latitude, Longitude, Description, ContactPersonName, ContactPhoneNum, Radius, Email, Phone, TimeZone, DayLightSaving, CreateUserId, LandmarkType) "
                        + "VALUES (@organizationid, @landmarkName, @latitude, @longitude, @description, @contactPersonName, @contactPhoneNum, @radius, @email, @ContactPhoneNum, @timeZone, @daylightSaving, @createUserId, @landmarkType);SELECT SCOPE_IDENTITY()";
                    if (landmarkId > 0)
                    {
                        sql =
                            "UPDATE vlfLandmark SET OrganizationId=@organizationId, LandmarkName=@landmarkName, Latitude=@latitude, Longitude=@longitude, Radius=@radius, Description=@description, ContactPersonName=@contactPersonName, ContactPhoneNum=@contactPhoneNum, Email=@email, TimeZone=@timeZone, DayLightSaving=@daylightSaving, CreateUserId=@createUserId, LandmarkType=@landmarkType WHERE LandmarkId=@landmarkId";
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@organizationid", organizationId);
                        sqlCommand.Parameters.AddWithValue("@landmarkName", landmarkName);
                        sqlCommand.Parameters.AddWithValue("@latitude", latitude);
                        sqlCommand.Parameters.AddWithValue("@longitude", longitude);
                        sqlCommand.Parameters.AddWithValue("@description", description);
                        sqlCommand.Parameters.AddWithValue("@contactPersonName", contactPerson);
                        sqlCommand.Parameters.AddWithValue("@radius", buffer);
                        sqlCommand.Parameters.AddWithValue("@email", email);
                        sqlCommand.Parameters.AddWithValue("@contactPhoneNum", contactNo);
                        sqlCommand.Parameters.AddWithValue("@timeZone", timeZone);
                        sqlCommand.Parameters.AddWithValue("@DayLightSaving", 1);
                        sqlCommand.Parameters.AddWithValue("@AutoAjustDayLightSaving", 1);
                        sqlCommand.Parameters.AddWithValue("@createUserId", createUserId);
                        sqlCommand.Parameters.AddWithValue("@landmarkType", "ROUTE");
                        if (landmarkId > 0)
                        {
                            sqlCommand.Parameters.AddWithValue("@landmarkId", landmarkId);
                            sqlCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            landmarkId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                        }                        
                        connection.Close();
                        return landmarkId;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }            
        }

        public static DataTable RetrieveFromDB(Dictionary<string, string> conditions, int organizationid, int userid, ref int totalCount)
        {
            try
            {
                //IList<string> landmarkids = GetUserGroupRoutesId(userid);               
                IList<string> sColumns = new List<string>(new string[] { "landmarkname", "contactpersonname", "createddatetime" });
                string querySql = "SELECT * FROM \"vlfLandmarks\" WHERE landmarktype='ROUTE' AND organizationid={4} " +
                                  //"AND originallandmarkid in ({5}) " +
                                  "{1} {0} LIMIT {3} OFFSET {2}";                
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
                        if (conditions[string.Format("bSortable_{0}", Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {                            
                            switch (conditions[string.Format("iSortCol_{0}", i)])
                            {
                                case "0":
                                    tmpOrder = string.Format("{0} {1}", sColumns[0],
                                                             (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                  ? "ASC"
                                                                  : "DESC"));
                                    break;
                                case "1":
                                    tmpOrder = string.Format("{0} {1}", sColumns[1],
                                                             (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                  ? "ASC"
                                                                  : "DESC"));
                                    break;
                                case "2":
                                    tmpOrder = string.Format("{0} {1}", sColumns[2],
                                                             (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                  ? "ASC"
                                                                  : "DESC"));
                                    break;
                            }
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

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            if (sColumns[i].Equals("createddatetime"))
                            {
                                continue;
                            }
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }

                string countQueryString = "SELECT COUNT(1) FROM \"vlfLandmarks\" WHERE landmarktype='ROUTE' AND organizationid={1} {0}";
                countQueryString = string.Format(countQueryString, sWhere, organizationid);

                DataTable dataTable = new DataTable();
                querySql = string.Format(querySql, sOrder, sWhere, sLimit, sLength, organizationid);
                IList<Dictionary<string, string>> results= new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
                {

                    using (NpgsqlCommand sqlCommandCount = new NpgsqlCommand(countQueryString, connection))
                    {
                        connection.Open();
                        sqlCommandCount.CommandType = CommandType.Text;
                        totalCount = Convert.ToInt32(sqlCommandCount.ExecuteScalar());
                    }

                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(querySql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();   
                        }                        
                        sqlCommand.CommandType = CommandType.Text;
                        NpgsqlDataAdapter sqlDataAdapter = new NpgsqlDataAdapter(sqlCommand);                        
                        sqlDataAdapter.Fill(dataTable);                        
                    }
                    connection.Close();
                }
                return dataTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }            
        }

        public static DataTable RetrieveRoutesFromServiceDB(Dictionary<string, string> conditions, int organizationid, int userid, ref int totalCount)
        {
            SqlConnection connection = null;
            try
            {
                string countSql = "SELECT COUNT(1) AS NUM FROM vlfLandmark " +
                                  "LEFT JOIN vlfServiceRouteMappings ON LandmarkId = RouteId " +
                                  "LEFT JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceRouteMappings.ServiceConfigId " +
                                  "WHERE LandmarkType= 'ROUTE' AND vlfLandmark.OrganizationId={0} {1}";

                string coreSql =
                    "SELECT vlfServiceConfigurations.ServiceConfigID, vlfServiceConfigurations.ServiceConfigName, vlfLandmark.LandmarkName, vlfLandmark.LandmarkId, CreatedDate, IsAssigned = ( " +
                    "CASE " +
                    "WHEN (SELECT COUNT(1) AS num FROM vlfServiceAssignments WHERE vlfServiceAssignments.ServiceConfigID=vlfServiceRouteMappings.ServiceConfigId AND vlfServiceAssignments.Deleted = 0)> 0 THEN 1 " +
                    "ELSE 0 " +
                    "END " +
                    "), " +
                    "IsExpired = ( " +
                    "CASE " +
                    "WHEN vlfServiceConfigurations.ExpiredDate IS NULL THEN 0 " +
                    "ELSE 1 " +
                    "END " +
                    "), " +
                    "UserName = ( " +
                    "CASE " +
                    "WHEN vlfUser.UserName IS NULL THEN (SELECT UserName FROM vlfUser WHERE vlfUser.UserId = vlfLandmark.CreateUserID) " +
                    "ELSE vlfUser.UserName " +
                    "END " +
                    "), ROW_NUMBER() OVER ({1}) AS RowNum FROM vlfLandmark " +
                    "LEFT JOIN vlfServiceRouteMappings ON LandmarkId = RouteId " +
                    "LEFT JOIN vlfServiceConfigurations ON vlfServiceConfigurations.ServiceConfigID = vlfServiceRouteMappings.ServiceConfigId " +
                    "LEFT JOIN vlfUser ON vlfServiceConfigurations.UserID = vlfUser.UserId " +
                    "WHERE LandmarkType= 'ROUTE' AND vlfLandmark.OrganizationId={0} {2}";

                string querySql = ";;WITH ResultSets AS({0})" +
                                  " SELECT * FROM ResultSets WHERE RowNum >= {1} AND RowNum < {1} + {2};";
                IList<string> sColumns =
                    new List<string>(new string[]
                    {
                        "ServiceConfigName", "LandmarkName", "ContactPersonName", "CreatedDatetime", "IsAssigned",
                        "IsExpired"
                    });

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
                            switch (conditions[string.Format("iSortCol_{0}", i)])
                            {
                                case "0":
                                    tmpOrder = string.Format("{0} {1}", sColumns[0],
                                        (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                            ? "ASC"
                                            : "DESC"));
                                    break;
                                case "1":
                                    tmpOrder = string.Format("{0} {1}", sColumns[1],
                                        (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                            ? "ASC"
                                            : "DESC"));
                                    break;
                                case "2":
                                    tmpOrder = string.Format("{0} {1}", sColumns[2],
                                        (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                            ? "ASC"
                                            : "DESC"));
                                    break;
                            }
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
                            if (sColumns[i].Equals("createddatetime"))
                            {
                                continue;
                            }
                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i],
                                    conditions["sSearch"]);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i],
                                    conditions["sSearch"]);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < sColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") &&
                            !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            if (sColumns[i].Equals("createddatetime"))
                            {
                                continue;
                            }
                            sWhere += string.Format("AND LOWER({0}) LIKE LOWER('%{1}%')", sColumns[i],
                                conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }


                string countQueryString = string.Format(countSql, organizationid, sWhere);
                string coreQueryString = string.Format(coreSql, organizationid, sOrder, sWhere);
                DataTable dataTable = new DataTable();
                querySql = string.Format(querySql, coreQueryString, sLimit, sLength);
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (connection = new SqlConnection(connStr))
                {

                    using (SqlCommand sqlCommandCount = new SqlCommand(countQueryString, connection))
                    {
                        connection.Open();
                        sqlCommandCount.CommandType = CommandType.Text;
                        SqlDataReader dataReader = sqlCommandCount.ExecuteReader();
                        dataReader.Read();
                        totalCount = Convert.ToInt32(dataReader["NUM"]);
                        dataReader.Close();
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(querySql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        sqlCommand.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                return dataTable;
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


        public static IList<string> GetUserGroupRoutesId(int userId)
        {
            IList<string> results = new List<string>();
            string sql =
                "SELECT LandmarkId FROM vlfLandmark INNER JOIN vlfUserGroupAssignment ON CreateUserID = UserId WHERE LandmarkType='ROUTE' AND UserGroupId IN (SELECT UserGroupId from vlfUserGroupAssignment WHERE UserId=@userId)";
            SqlConnection connection = null;
            try
            {
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@userId", userId);
                        SqlDataReader dataReader = sqlCommand.ExecuteReader();
                        while (dataReader.Read())
                        {
                            results.Add(Convert.ToString(dataReader["LandmarkId"]));
                        }
                    }
                    connection.Close();
                }
                return results;
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

        public static DataTable GetBsmLandmarksAndGeozonesFromDB(int organizationId, string input)
        {
            string sql = "SELECT LandmarkName AS name,  CAST(vlfLandmark.Latitude AS nvarchar) + ',' + CAST(vlfLandmark.Longitude AS nvarchar) as latlon FROM vlfLandmark WHERE OrganizationId=@organizationId AND ISNULL(LandmarkType, -1) <> 'ROUTE' AND (LandmarkName LIKE '%{0}%' OR ISNULL([Description], -1) LIKE '%{0}%') " +
                        " UNION " +
                        "SELECT og.GeozoneName AS name, (SELECT TOP 1 CAST(vlfGeozoneSet.Latitude AS nvarchar) + ',' + CAST(vlfGeozoneSet.Longitude AS nvarchar) FROM vlfGeozoneSet WHERE GeozoneNo = og.GeozoneNo) as latlon FROM vlfOrganizationGeozone og " +
                        "WHERE og.OrganizationId=@organizationId AND (og.GeozoneName LIKE '%{0}%' OR ISNULL(og.[Description], -1) LIKE '%{0}%')";
            SqlConnection connection = null;
            try
            {
                string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
                sql = string.Format(sql, input);
                DataTable dataTable = new DataTable();
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@organizationId", organizationId);                        
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                return dataTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    if(connection.State != ConnectionState.Closed)
                    {
                        connection.Close(); 
                    }
                }
            }
        }

        public static bool UpdateViolationDispute(int vid, string notes)
        {
            SqlConnection connection = null;
            string sql = "UPDATE evtViolations SET IsEvent=2, Notes=@notes WHERE ID=@vid";
            try
            {
                string connStr = ConfigurationSettings.AppSettings["DBReportConnectionString"];                               
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@notes", notes);
                        command.Parameters.AddWithValue("@vid", vid);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();                    
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
            return false;
        }

        public static bool CreateNewDisputePoints(int vid, double lat, double lon, int speed, int currentValue, int vehicleValue, int metric,
            string notes, int organizationId, string streetAddress, string emailsList, int correctionid = 0, int nid = 0)
        {            
            if (organizationId.Equals(0))
            {
                return false;
            }
            NpgsqlConnection myConn = null;
            string productionConnStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
            string sql = "INSERT INTO \"pointspeedcorrections\" (latitude, longitude, speed, metric, notes, correctionid, organizationid, notificationid, streetaddress, emailslist, created, modified, deleted, currentvalue, vehiclevalue, geom, nid) VALUES (@latitude, @longitude, @speed, @metric, @notes, @correctionid, @organizationid, @vid, @streetaddress, @emailslist, @created, @modified, @deleted, @currentvalue, @vehiclevalue, {0}, @nid)";
            string geomPoint = string.Format("ST_GeomFromText('POINT({0} {1})', 4326)", lon, lat);
            sql = string.Format(sql, geomPoint);
            try
            {

                using (myConn = new NpgsqlConnection(productionConnStr))
                {
                    myConn.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, myConn))
                    {                        
                        command.Parameters.AddWithValue("@latitude", lat);
                        command.Parameters.AddWithValue("@longitude", lon);
                        command.Parameters.AddWithValue("@speed", speed);
                        command.Parameters.AddWithValue("@metric", metric);
                        command.Parameters.AddWithValue("@notes", notes);
                        command.Parameters.AddWithValue("@correctionid", correctionid);
                        command.Parameters.AddWithValue("@organizationid", organizationId);
                        command.Parameters.AddWithValue("@vid", vid);
                        command.Parameters.AddWithValue("@streetaddress", streetAddress);
                        command.Parameters.AddWithValue("@emailslist", emailsList);
                        command.Parameters.AddWithValue("@created", DateTime.Now);
                        command.Parameters.AddWithValue("@modified", DateTime.Now);
                        command.Parameters.AddWithValue("@deleted", 0);
                        command.Parameters.AddWithValue("@currentvalue", currentValue);
                        command.Parameters.AddWithValue("@vehiclevalue",vehicleValue);
                        command.Parameters.AddWithValue("@nid", nid);
                        command.ExecuteNonQuery();
                    }
                    myConn.Close();
                }
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }            
            return false;
        }


        public static string GetMessage(int nid)
        {
            string message = string.Empty;
            if (nid > 0)
            {
                SqlConnection connection = null;
                string sql = "SELECT Message FROM evtNotifications WHERE NotificationID=@nid";               
                try
                {
                    string connStr = ConfigurationSettings.AppSettings["DBReportConnectionString"];
                    using (connection = new SqlConnection(connStr))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@nid", nid);
                            SqlDataReader sqlDataReader = command.ExecuteReader();
                            sqlDataReader.Read();
                            message = Convert.ToString(sqlDataReader["Message"]);
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
            }
            
            return message;
        }

        public static bool SendNotification(int organizationId, int boxId, int vehicleId, int landmarkId, string objects, int objectId, string emaillist, string subject, string body)
        {
            SqlConnection connection = null;
            bool result = false;
            //emaillist = "yjiang@bsmwireless.com";
            string sql = "INSERT INTO evtNotifications (EventID, FleetID, OrganizationID, BoxID, Date, EmailList, Subject, Message, Status, ServiceConfigId, VehicleId, LandmarkId) VALUES(0, @fleetId, @organizationId, @boxId, GETDATE(), @emailsList, @subject, @message, 0, 0, @vehicleId, @landmarkId);";
            try
            {
                string connStr = ConfigurationSettings.AppSettings["DBReportConnectionString"];
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fleetId", ("Fleet".Equals(objects) ? objectId : 0));
                        command.Parameters.AddWithValue("@organizationId", organizationId);
                        command.Parameters.AddWithValue("@boxId", boxId);
                        command.Parameters.AddWithValue("@objectId", objectId);
                        command.Parameters.AddWithValue("@emailsList", emaillist);
                        command.Parameters.AddWithValue("@subject", subject);
                        command.Parameters.AddWithValue("@message", body);
                        command.Parameters.AddWithValue("@vehicleId", vehicleId);
                        command.Parameters.AddWithValue("@landmarkId", landmarkId);
                        result = command.ExecuteNonQuery() > 0;

                    }
                    connection.Close();
                }
                return result;
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


        public static string GetEmailsList(int organizationId, int serviceConfigId, int boxId, int landmarkId, string objects, int objectId)
        {
            SqlConnection connection = null;
            string emailList = string.Empty;
            string sql = "select EmailsList FROM [dbo].[GetEmailsListByServiceConfigId] (@organizationId, @serviceConfigId, @boxId, @objects, @objectId, @landmarkId);";
            try
            {
                string connStr = ConfigurationSettings.AppSettings["DBReportConnectionString"];
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@organizationId", organizationId);
                        command.Parameters.AddWithValue("@serviceConfigId", serviceConfigId);
                        command.Parameters.AddWithValue("@objects", objects);
                        command.Parameters.AddWithValue("@objectId", objectId);
                        command.Parameters.AddWithValue("@landmarkId", landmarkId);
                        command.Parameters.AddWithValue("@boxId", boxId);
                        SqlDataReader sqlDataReader = command.ExecuteReader();
                        sqlDataReader.Read();
                        emailList = Convert.ToString(sqlDataReader["EmailsList"]);
                    }
                    connection.Close();
                }
                return emailList;
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

        public static DataTable GetViolation(int vid)
        {
            SqlConnection connection = null;
            int oid = 0;
            DataTable dataTable = new DataTable();
            try
            {
                string connStr = ConfigurationSettings.AppSettings["DBReportConnectionString"];
                string sql = "SELECT * from evtViolations WHERE ID=@vid";
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("vid", vid);                        
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                    return dataTable;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message + ": " + exception.StackTrace);
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


        public static bool ThereIsThingAssigned(int organizationId, int routeId)
        {
            SqlConnection connection = null;
            string connStr = ConfigurationSettings.AppSettings["SentinelFMConnection"];
            string sql = "SELECT vlfServiceConfigurations.* FROM vlfServiceConfigurations " +
                         "INNER JOIN vlfServiceAssignments ON vlfServiceAssignments.ServiceConfigID = vlfServiceConfigurations.ServiceConfigID " +
                         "WHERE vlfServiceConfigurations.ServiceID = 4 AND vlfServiceConfigurations.OrganizationID=@organizationId;";
            try
            {
                using (connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@organizationId", organizationId);
                        command.CommandType = CommandType.Text;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string rulesApplied = Convert.ToString(reader["RulesApplied"]);
                            if (rulesApplied.Contains(Convert.ToString(routeId)))
                            {
                                connection.Close();
                                return true;
                            }
                        }

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
            return false;
        }

        public static DataTable GetDisputePointsDb(int organizationId = 0, string filter = null, int pid = 0)
        {
            NpgsqlConnection connection = null;
            try
            {
                DataTable dataTable = new DataTable();
                string sql = "SELECT * FROM \"pointspeedcorrections\" {0}";
                if (pid > 0)
                {
                    sql = string.Format(sql,
                        pid > 0
                            ? string.Format("WHERE id={1}", organizationId, pid)
                            : string.Empty);
                }
                else if (organizationId > 0)
                {
                    sql = string.Format(sql,
                        organizationId > 0 ? string.Format("WHERE organizationid={0}", organizationId) : string.Empty);
                }                

                if (!string.IsNullOrEmpty(filter))
                {
                    if (filter.Equals("Dismissed"))
                    {
                        sql = string.Format("{0} AND deleted=1", sql);    
                    }
                    else if (filter.Equals("Accepted"))
                    {
                        sql = string.Format("{0} AND correctionid>0 AND deleted=0", sql);
                    }
                    else if (filter.Equals("Disputed"))
                    {
                        sql = string.Format("{0} AND correctionid=0 AND deleted=0", sql);
                    }

                }
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, connection))
                    {                       
                        sqlCommand.CommandType = CommandType.Text;
                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(sqlCommand);
                        npgsqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                return dataTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }            
        }

        public static bool DeleteDisputePointDb(int pointId, string comment = null)
        {
            NpgsqlConnection connection = null;
            bool result = false;
            try
            {
                DataTable dataTable = new DataTable();
                string sql = "UPDATE \"pointspeedcorrections\" SET correctionid=-1, modified=@modified, deleted=1, comments=@comment WHERE id=@id";                
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@modified", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@comment", comment);
                        sqlCommand.Parameters.AddWithValue("@id", pointId);                        
                        result = sqlCommand.ExecuteNonQuery() > 0;
                    }
                    connection.Close();
                }
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static DataTable DeleteCorrectedSegment(int correctionId)
        {
            NpgsqlConnection connection = null;
            bool deleted = false;
            string storedIds = "-1";
            try
            {
                DataTable affectedRows = new DataTable();
                string sql = "DELETE FROM \"speedcorrections\" WHERE id=@correctionid";
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@correctionid", correctionId);
                        deleted = sqlCommand.ExecuteNonQuery() > 0;
                    }
                    if (deleted)
                    {
                        string storeAffectedPoints = "SELECT id FROM \"pointspeedcorrections\" where correctionid=@correctionid AND deleted=0";

                        using (NpgsqlCommand storedCommand = new NpgsqlCommand(storeAffectedPoints, connection))
                        {
                            storedCommand.CommandType = CommandType.Text;
                            storedCommand.Parameters.AddWithValue("@correctionid", correctionId);
                            NpgsqlDataReader npgsqlDataReader = storedCommand.ExecuteReader();

                            while (npgsqlDataReader.Read())
                            {
                                if (storedIds.Equals("-1"))
                                {
                                    storedIds = Convert.ToString(npgsqlDataReader["id"]);
                                }
                                else
                                {
                                    storedIds += "," + Convert.ToString(npgsqlDataReader["id"]);
                                }
                            }
                            npgsqlDataReader.Close();
                        }

                        sql = string.Format("UPDATE \"pointspeedcorrections\" SET modified=@modified, deleted=1 WHERE id IN ({0})", storedIds);
                        using (NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, connection))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@modified", DateTime.Now);
                            sqlCommand.ExecuteNonQuery();
                        }        

                        string selectedAffectedPoints = string.Format("SELECT * FROM \"pointspeedcorrections\" where id in ({0})", storedIds);
                        using (NpgsqlCommand affectedCommand = new NpgsqlCommand(selectedAffectedPoints, connection))
                        {
                            affectedCommand.CommandType = CommandType.Text;
                            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(affectedCommand);
                            npgsqlDataAdapter.Fill(affectedRows);
                        }
                    }
                    connection.Close();
                }
                return affectedRows;
            }
            catch (Exception exception)
            {
                
                throw new Exception(exception.Message);
            }            
        }

        public static DataTable SaveSpeedCorrection(string connStr, string routePoints, int organizationId, string landmarkName, string contactPerson, int buffer, string email, string contactNo, int timeZone, int daylightSaving, string routerLink, string wayPoints, string description, double latitude, double longitude, ref int correctionid, int createUserId, int speed, int disputeId, string disputeComment = null)
        {
            string sql = "INSERT INTO \"speedcorrections\" (organizationId, geom, routelink, waypoints, routename, contactperson, email, phone, buffer, description, speed) VALUES" +
                             "( @organizationId, {0}, @routerlink, @waypoints, @routename, @contactperson, @email, @phone, @buffer, @description, @speed) RETURNING id";
            if (correctionid > 0)
            {
                sql = "UPDATE \"speedcorrections\" SET organizationId=@organizationId, routename=@routename, " +
                "geom={0}, routelink=@routerlink, waypoints=@waypoints, contactperson=@contactperson, email=@email, phone=@phone, buffer=@buffer, description=@description, speed=@speed WHERE id=@correctionid";
            }
            string routerGeometry = string.Format("ST_GeomFromText('LINESTRING({0})', 4326)", routePoints);
            string bufferedRouter = string.Format("bufferedline({0}, {1})", routerGeometry, buffer);
            string processedSql = string.Format(sql, bufferedRouter);
            int result = correctionid;
            DataTable affectedRows = new DataTable();
            try
            {
                using (NpgsqlConnection myConn = new NpgsqlConnection(connStr))
                {
                    myConn.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(processedSql, myConn))
                    {
                        command.Parameters.AddWithValue("@organizationId", organizationId);
                        command.Parameters.AddWithValue("@routename", landmarkName);
                        command.Parameters.AddWithValue("@buffer", buffer);
                        command.Parameters.AddWithValue("@contactperson", contactPerson);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@phone", contactNo);                        
                        command.Parameters.AddWithValue("@routerlink", routerLink);
                        command.Parameters.AddWithValue("@waypoints", wayPoints);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@speed", speed);                        
                        if (correctionid > 0)
                        {
                            command.Parameters.AddWithValue("@correctionid", correctionid);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            NpgsqlDataReader idReader = command.ExecuteReader();
                            idReader.Read();
                            result = Convert.ToInt32(idReader["id"]);
                            idReader.Close();
                        }
                        
                    }
                    
                    string storedIds = "-1";
                    if (correctionid > 0)
                    {
                        string storeAffectedPoints = "SELECT id FROM \"pointspeedcorrections\" where correctionid=@correctionid";

                        using (NpgsqlCommand storedCommand = new NpgsqlCommand(storeAffectedPoints, myConn))
                        {
                            storedCommand.CommandType = CommandType.Text;
                            storedCommand.Parameters.AddWithValue("@correctionid", result);
                            NpgsqlDataReader npgsqlDataReader = storedCommand.ExecuteReader();

                            while (npgsqlDataReader.Read())
                            {
                                if (storedIds.Equals("-1"))
                                {
                                    storedIds = Convert.ToString(npgsqlDataReader["id"]);
                                }
                                else
                                {
                                    storedIds += "," + Convert.ToString(npgsqlDataReader["id"]);
                                }
                            }
                            npgsqlDataReader.Close();
                        }
                    }


                    string resetSql = "update \"pointspeedcorrections\" SET correctionid=0, speed=0, modified=@modified where correctionid=@correctionid AND deleted=0";

                    using (NpgsqlCommand resetCommand = new NpgsqlCommand(resetSql, myConn))
                    {
                        resetCommand.CommandType = CommandType.Text;
                        resetCommand.Parameters.AddWithValue("@correctionid", result);
                        resetCommand.Parameters.AddWithValue("@modified", DateTime.Now);
                        resetCommand.ExecuteNonQuery();
                    }

                    string selectSql =
                        "select ps.id from \"speedcorrections\" AS pc, \"pointspeedcorrections\" AS ps WHERE pc.id=@correctionid AND ST_Within( ST_Transform(ST_GeomFromText('POINT('|| ps.longitude ||' '|| ps.latitude ||')', 4326), 2163), ST_Transform(pc.geom, 2163))";
                    string ids = "-1";
                    using (NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, myConn))
                    {
                        selectCommand.CommandType = CommandType.Text;
                        selectCommand.Parameters.AddWithValue("@correctionid", result);
                        NpgsqlDataReader npgsqlDataReader = selectCommand.ExecuteReader();
                        
                        while (npgsqlDataReader.Read())
                        {
                            if (ids.Equals("-1"))
                            {
                                ids = Convert.ToString(npgsqlDataReader["id"]);
                            }
                            else
                            {
                                ids += "," + Convert.ToString(npgsqlDataReader["id"]);
                            }
                        }
                        npgsqlDataReader.Close();
                    }

                    string updateSql = string.Format("update \"pointspeedcorrections\" SET correctionid=@correctionid, speed=@speed, modified=@modified where id in ({0}) AND deleted=0", ids);

                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateSql, myConn))
                    {
                        updateCommand.CommandType = CommandType.Text;
                        updateCommand.Parameters.AddWithValue("@correctionid", result);
                        updateCommand.Parameters.AddWithValue("@speed", speed);
                        updateCommand.Parameters.AddWithValue("@modified", DateTime.Now);
                        updateCommand.ExecuteNonQuery();
                    }

                    string selectedAffectedPoints = string.Format("SELECT * FROM \"pointspeedcorrections\" where id in ({0}) AND deleted=0", (storedIds.Equals("-1") ? ids : storedIds));
                    using (NpgsqlCommand affectedCommand = new NpgsqlCommand(selectedAffectedPoints, myConn))
                    {
                        affectedCommand.CommandType = CommandType.Text;
                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(affectedCommand);
                        npgsqlDataAdapter.Fill(affectedRows);                        
                    }                    
                    myConn.Close();
                    correctionid = result;
                    return affectedRows;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static DataTable GetCorrectionRoute(string connStr, int correctionId)
        {
            string sql = "SELECT * FROM \"speedcorrections\" WHERE id=@id";
            try
            {
                DataTable dtTable = new DataTable();
                using (NpgsqlConnection myConn = new NpgsqlConnection(connStr))
                {
                    myConn.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, myConn))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", correctionId);                        
                        NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(command);
                        npgsqlDataAdapter.Fill(dtTable);
                    }                    
                    myConn.Close();
                }
                return dtTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static DataTable RetrieveDisputesFromDB(Dictionary<string, string> conditions, int organizationid, ref int totalCount)
        {
            try
            {
                //IList<string> landmarkids = GetUserGroupRoutesId(userid);               
                IList<string> sColumns = new List<string>(new string[] { "notificationid", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "created", "modified", "id" });
                IList<string> whereColumns = new List<string>(new string[] { "notes", "streetaddress", "emailslist", "notificationid", "created", "modified" });
                string querySql = "SELECT * FROM \"pointspeedcorrections\" WHERE organizationid={4} " +                    
                                  "{1} {0} LIMIT {3} OFFSET {2}";
                //paging
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                        if (Convert.ToInt32(conditions["iDisplayStart"]) < 0)
                        {
                            sLimit = "0";
                        }
                    }
                }
                //Ordering
                string sOrder = "";    
                string sWhere = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";                   
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (conditions[string.Format("bSortable_{0}", Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            if (string.IsNullOrEmpty(sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])]))
                            {
                                continue;
                            }

                            if (sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])].Equals("id"))
                            {
                                tmpOrder = string.Format("correctionid {0}",
                                                          (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                               ? "ASC"
                                                               : "DESC"));                           
                            }
                            else
                            {
                                tmpOrder = string.Format("{0} {1}", sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])],
                                                            (conditions[string.Format("sSortDir_{0}", i)].Equals("asc")
                                                                 ? "ASC"
                                                                 : "DESC"));    
                            }
                            
                           
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
                
                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere += " AND ({0})";
                        string tmpWhere = "";
                        for (int i = 0; i < whereColumns.Count; i++)
                        {                                                

                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER(CAST({0} AS TEXT)) LIKE LOWER('%{1}%')", whereColumns[i], conditions["sSearch"]);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER(CAST({0} AS TEXT)) LIKE LOWER('%{1}%')", whereColumns[i], conditions["sSearch"]);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < whereColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER(CAST({0} AS TEXT)) LIKE LOWER('%{1}%')", whereColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }
                if (conditions.ContainsKey("filter"))
                {
                    switch (conditions["filter"])
                    {
                        case "Disputed":
                            sWhere += " AND correctionid=0 AND deleted=0 ";
                            break;
                        case "Accepted":
                            sWhere += " AND correctionid>0 AND deleted=0 ";
                            break;
                        case "Dismissed":
                            sWhere += " AND deleted=1 ";
                            break;
                    }
                }

                string countQueryString = "SELECT COUNT(1) FROM \"pointspeedcorrections\" WHERE organizationid={1} {0}";
                countQueryString = string.Format(countQueryString, sWhere, organizationid);

                DataTable dataTable = new DataTable();
                
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
                {

                    using (NpgsqlCommand sqlCommandCount = new NpgsqlCommand(countQueryString, connection))
                    {
                        connection.Open();
                        sqlCommandCount.CommandType = CommandType.Text;
                        totalCount = Convert.ToInt32(sqlCommandCount.ExecuteScalar());
                        if (Convert.ToInt32(sLength) < 0)
                        {
                            sLength = Convert.ToString(totalCount);
                        }
                    }
                    querySql = string.Format(querySql, sOrder, sWhere, sLimit, sLength, organizationid);
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(querySql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        sqlCommand.CommandType = CommandType.Text;
                        NpgsqlDataAdapter sqlDataAdapter = new NpgsqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                return dataTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static DataTable RetrieveSegmentsFromDB(Dictionary<string, string> conditions, int organizationid, ref int totalCount)
        {
            try
            {
                //IList<string> landmarkids = GetUserGroupRoutesId(userid);               
                IList<string> sColumns = new List<string>(new string[] { "routename", string.Empty, "created", "expired", "speed", string.Empty, "AssignedNum" });
                IList<string> whereColumns = new List<string>(new string[] { "routename", "contactperson", "created", "expired", "speed" });
                string querySql = "SELECT *, (select count(1) FROM \"pointspeedcorrections\" pd WHERE pd.correctionid=pc.id) as AssignedNum FROM \"speedcorrections\" pc WHERE organizationid in (0, {4}) " +
                                  "{1} {0} LIMIT {3} OFFSET {2}";
                //paging
                string sLimit = "";
                string sLength = "";
                if (conditions.ContainsKey("iDisplayStart") && conditions.ContainsKey("iDisplayLength"))
                {
                    if (!Convert.ToInt32(conditions["iDisplayStart"]).Equals(-1))
                    {
                        sLimit = Convert.ToString(conditions["iDisplayStart"]);
                        sLength = Convert.ToString(conditions["iDisplayLength"]);
                        if (Convert.ToInt32(conditions["iDisplayStart"]) < 0)
                        {
                            sLimit = "0";
                        }
                    }
                }
                //Ordering
                string sOrder = "";
                string sWhere = "";
                if (conditions.ContainsKey("iSortCol_0"))
                {
                    sOrder = "ORDER BY {0}";
                    string sortedColumns = "";
                    for (int i = 0; i < Convert.ToInt32(conditions["iSortingCols"]); i++)
                    {
                        string tmpOrder = "";
                        if (conditions[string.Format("bSortable_{0}", Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)]))] == "true")
                        {
                            if (string.IsNullOrEmpty(sColumns[Convert.ToInt32(conditions[string.Format("iSortCol_{0}", i)])]))
                            {
                                continue;
                            }

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

                if (conditions.ContainsKey("sSearch"))
                {
                    if (!string.IsNullOrEmpty(conditions["sSearch"]))
                    {
                        sWhere += " AND ({0})";
                        string tmpWhere = "";
                        for (int i = 0; i < whereColumns.Count; i++)
                        {

                            if (string.IsNullOrEmpty(tmpWhere))
                            {
                                tmpWhere = string.Format("LOWER(CAST({0} AS TEXT)) LIKE LOWER('%{1}%')", whereColumns[i], conditions["sSearch"]);
                            }
                            else
                            {
                                tmpWhere += string.Format(" OR LOWER(CAST({0} AS TEXT)) LIKE LOWER('%{1}%')", whereColumns[i], conditions["sSearch"]);
                            }
                        }

                        sWhere = string.Format(sWhere, tmpWhere);
                    }
                }

                for (int i = 0; i < whereColumns.Count; i++)
                {
                    if (conditions.ContainsKey(string.Format("bSearchable_{0}", i)))
                    {
                        if (conditions[string.Format("bSearchable_{0}", i)].Equals("true") && !string.IsNullOrEmpty(conditions[string.Format("sSearch_{0}", i)]))
                        {
                            sWhere += string.Format("AND LOWER(CAST({0} AS TEXT)) LIKE LOWER('%{1}%')", whereColumns[i], conditions[string.Format("sSearch_{0}", i)]);
                        }
                    }
                }               

                string countQueryString = "SELECT COUNT(1) FROM \"speedcorrections\" WHERE organizationid in (0, {0}) {1}";
                countQueryString = string.Format(countQueryString, organizationid, sWhere);

                DataTable dataTable = new DataTable();

                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
                {

                    using (NpgsqlCommand sqlCommandCount = new NpgsqlCommand(countQueryString, connection))
                    {
                        connection.Open();
                        sqlCommandCount.CommandType = CommandType.Text;
                        totalCount = Convert.ToInt32(sqlCommandCount.ExecuteScalar());
                        if (Convert.ToInt32(sLength) < 0)
                        {
                            sLength = Convert.ToString(totalCount);
                        }
                    }
                    querySql = string.Format(querySql, sOrder, sWhere, sLimit, sLength, organizationid);
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(querySql, connection))
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        sqlCommand.CommandType = CommandType.Text;
                        NpgsqlDataAdapter sqlDataAdapter = new NpgsqlDataAdapter(sqlCommand);
                        sqlDataAdapter.Fill(dataTable);
                    }
                    connection.Close();
                }
                return dataTable;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static bool UpdateComment(int disputeId, string disputeComment)
        {
            bool result = false;
            try
            {
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();
                    string commentSql = "update \"pointspeedcorrections\" SET comments=@comments where id=@id";
                    using (NpgsqlCommand commentCommand = new NpgsqlCommand(commentSql, connection))
                    {
                        commentCommand.CommandType = CommandType.Text;
                        commentCommand.Parameters.AddWithValue("@comments", disputeComment);
                        commentCommand.Parameters.AddWithValue("@id", disputeId);
                        result = commentCommand.ExecuteNonQuery() > 0;
                    }
                    connection.Close();
                    return result;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static Dictionary<string, string> AlreadyCreatedForDispute(int organizationId, double lat, double lng)
        {
            string selectSql =
                    "select pc.* from \"speedcorrections\" AS pc WHERE pc.organizationid=@organiztionid AND ST_Within( ST_Transform(ST_GeomFromText('POINT('|| @lng ||' '|| @lat ||')', 4326), 2163), ST_Transform(pc.geom, 2163))";
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                string connStr = ConfigurationSettings.AppSettings["SpatialDBProduction"];
                using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();
                    using (NpgsqlCommand selectCommand = new NpgsqlCommand(selectSql, connection))
                    {
                        selectCommand.CommandType = CommandType.Text;
                        selectCommand.Parameters.AddWithValue("@organiztionid", organizationId);
                        selectCommand.Parameters.AddWithValue("@lng", lng);
                        selectCommand.Parameters.AddWithValue("@lat", lat);
                        NpgsqlDataReader npgsqlDataReader = selectCommand.ExecuteReader();

                        while (npgsqlDataReader.Read())
                        {                            
                            result.Add("id", Convert.ToString(npgsqlDataReader["id"]));
                            result.Add("speed", Convert.ToString(npgsqlDataReader["speed"]));
                            result.Add("description", Convert.ToString(npgsqlDataReader["description"])); 
                        }
                    }
                    connection.Close();                    
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return result;

        }
    }    
}
