using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using VLF.DAS;
using VLF.DAS.DB;

namespace VLF.PATCH.DB
{
    public class PatchLandmark : TblGenInterfaces
    {
        /// <summary>
        /// Provides interfaces to vlfMapLayers table.
        /// </summary>
        public PatchLandmark(SQLExecuter sqlExec)
            : base("vlfLandmark", sqlExec)
        {
        }


        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves public or private landmarks info by organization id, user id
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
        /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
        /// [StreetAddress],[Radius]
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet PatchGetLandmarksInfoByOrganizationIdUserId_NewTZ(int organizationId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                /*string sql = "SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius," +
                         "ISNULL(Email,' ') AS Email," +
                         "ISNULL(Phone,' ') AS Phone," +
                         "ISNULL(TimeZone,0) AS TimeZone," +
                         "ISNULL(DayLightSaving,0) AS DayLightSaving," +
                         "ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                                "ISNULL(StreetAddress,' ') AS StreetAddress,Radius" +
                   " FROM " + tableName +
                   " WHERE OrganizationId=" + organizationId + " ORDER BY LandmarkName";*/

                string sql = "sp_GetLandmarksInfo_NewTimeZone";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@organizationId", organizationId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }



        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves public or private landmarks info by organization id, user id
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
        /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
        /// [StreetAddress],[Radius]
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet PatchGetLandmarksInfoByOrganizationIdUserId(int organizationId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                /*string sql = "SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius," +
                         "ISNULL(Email,' ') AS Email," +
                         "ISNULL(Phone,' ') AS Phone," +
                         "ISNULL(TimeZone,0) AS TimeZone," +
                         "ISNULL(DayLightSaving,0) AS DayLightSaving," +
                         "ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                                "ISNULL(StreetAddress,' ') AS StreetAddress,Radius" +
                   " FROM " + tableName +
                   " WHERE OrganizationId=" + organizationId + " ORDER BY LandmarkName";*/

                string sql = "sp_GetLandmarksInfo";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@organizationId", organizationId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(int organizationId, int userId)
        {
            return PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(organizationId, userId, 0);
        }

        // Changes for TimeZone Feature start

        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId_NewTZ(int organizationId, int userId, long categoryId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                /*string sql = "SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius," +
                         "ISNULL(Email,' ') AS Email," +
                         "ISNULL(Phone,' ') AS Phone," +
                         "ISNULL(TimeZone,0) AS TimeZone," +
                         "ISNULL(DayLightSaving,0) AS DayLightSaving," +
                         "ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                                "ISNULL(StreetAddress,' ') AS StreetAddress,Radius" +
                   " FROM " + tableName +
                   " WHERE OrganizationId=" + organizationId + " ORDER BY LandmarkName";*/

                string sql = "sp_GetLandmarksInfoWithPoints_NewTimeZone";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@organizationId", organizationId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                sqlParams[2] = new SqlParameter("@categoryId", categoryId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end

        public DataSet PatchGetLandmarksInfoWithPointsByOrganizationIdUserId(int organizationId, int userId, long categoryId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                /*string sql = "SELECT OrganizationId,Replace(LandmarkName,char(39)+''+char(39),char(39)) AS LandmarkName,Latitude,Longitude,Replace(Description,char(39)+''+char(39),char(39)) AS Description,ContactPersonName,ContactPhoneNum,Radius," +
                         "ISNULL(Email,' ') AS Email," +
                         "ISNULL(Phone,' ') AS Phone," +
                         "ISNULL(TimeZone,0) AS TimeZone," +
                         "ISNULL(DayLightSaving,0) AS DayLightSaving," +
                         "ISNULL(AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                                "ISNULL(StreetAddress,' ') AS StreetAddress,Radius" +
                   " FROM " + tableName +
                   " WHERE OrganizationId=" + organizationId + " ORDER BY LandmarkName";*/

                string sql = "sp_GetLandmarksInfoWithPoints";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@organizationId", organizationId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                sqlParams[2] = new SqlParameter("@categoryId", categoryId);
                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by organization Id=" + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetAllUnassignedToFleetLandmarksInfo(int organizationId, int fleetId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "sp_GetAllUnassignedToFleetLandmarksInfo";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve AllUnassignedToFleetLandmarksInfo by OrganizationId=" + organizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve AllUnassignedToFleetLandmarksInfo by OrganizationId=" + organizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetAllAssignedToFleetLandmarksInfo(int organizationId, int fleetId)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "sp_GetAllAssignedToFleetLandmarksInfo";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve AllAssignedToFleetLandmarksInfo by OrganizationId=" + organizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve AllAssignedToFleetLandmarksInfo by OrganizationId=" + organizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public int AssignLandmarkToFleet(int organizationId, int landmarkId, int fleetId)
        {
            return AssignObjectToFleet(organizationId, landmarkId, fleetId, "landmark");
        }

        public int AssignObjectToFleet(int organizationId, int objectId, int fleetId, string objectName)
        {
            try
            {
                objectName = objectName.ToLower();
                string sql = "sp_AssignObjectToFleet";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@objectId", SqlDbType.Int, objectId);
                sqlExec.AddCommandParam("@objectName", SqlDbType.VarChar, objectName);
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@assignToAllVehiclesFleet", SqlDbType.Int, 1);


                return sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to AssignObjectToFleet by objectId=" + objectId.ToString() + "; fleetId=" + fleetId.ToString() + "; objectname=" + objectName;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to AssignObjectToFleet by objectId=" + objectId.ToString() + "; fleetId=" + fleetId.ToString() + "; objectname=" + objectName;

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }

        public int UnassignLandmarkFromFleet(int organizationId, int landmarkId, int fleetId)
        {
            try
            {

                string sql = "sp_UnassignObjectFromFleet";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@objectId", SqlDbType.Int, landmarkId);
                sqlExec.AddCommandParam("@objectName", SqlDbType.VarChar, "landmark");
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@unassignFromAllVehiclesFleet", SqlDbType.Int, 1);


                return sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to UnassignLandmarkToFleet by landmarkId=" + landmarkId.ToString() + "; fleetId=" + fleetId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to UnassignLandmarkToFleet by landmarkId=" + landmarkId.ToString() + "; fleetId=" + fleetId.ToString();

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return 0;
        }

        public int GetLandmarkIdByLandmarkName(int organizationId, string LandmarkName)
        {
            string sql = "sp_GetLandmarkIdByLandmarkName";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
            sqlExec.AddCommandParam("@landmarkName", SqlDbType.VarChar, LandmarkName);

            DataSet dt = sqlExec.SPExecuteDataset(sql);
            if (dt.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Tables[0].Rows[0][0].ToString());
            }
            return 0;
            
            
        }

        public int UpdateLandmarkCreater(int landmarkId, int userId)
        {
            string sql = "UPDATE vlfLandmark SET CreateUserID=" + userId + " WHERE LandmarkID=" + landmarkId;
            return sqlExec.SQLExecuteNonQuery(sql);
        }

        public int UnassignObjectFromAllFleets(string objectName, int objectId)
        {
            string sql = "sp_UnassignObjectFromAllFleets";

            sqlExec.ClearCommandParameters();

            sqlExec.AddCommandParam("@ObjectName", SqlDbType.VarChar, objectName);
            sqlExec.AddCommandParam("@ObjectId", SqlDbType.Int, objectId);


            return sqlExec.SPExecuteNonQuery(sql);
        }

        public bool IfObjectAssignedToFleet( int fleetId, string objectName, int objectId)
        {
            string sql = "SELECT * FROM vlfFleetObjects WHERE FleetId=" + fleetId.ToString() + " AND ObjectName='" + objectName + "' AND ObjectId=" + objectId.ToString();
            DataSet dt = sqlExec.SQLExecuteDataset(sql);
            return dt.Tables[0].Rows.Count > 0;
        }
    }
}
