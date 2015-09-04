using System;
using System.Collections.Generic;
using System.Text;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Data;
using System.Data.SqlClient;

namespace VLF.DAS.DB
{
    public class MapSearch : TblGenInterfaces
    {
        public MapSearch(SQLExecuter sqlExec)
            : base("", sqlExec)
        {
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Gets list of vehicles in polygon for selected time frame and organization
        /// </summary>
        /// <param name="Latitude">Map point latitude</param>
        /// <param name="Longitude">Map point longitude</param>
        /// <param name="Radius">Polygon radius</param>
        /// <param name="fromDate">Date from (UTC format)</param>
        /// <param name="toDate">Date to (UTC format)</param>
        /// <param name="orgId">Organization ID</param>
        /// <param name="LandmarkID">Existing landmark</param>
        /// <returns>List of vehicles</returns>
        public DataSet GetVehicleAreaSearch_NewTZ(double Latitude, double Longitude, double Radius, string fromDate, string toDate,
                    int orgId, int LandmarkID, string PolygonPoints, string FleetIDs, string BoxIDs, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[11];

                sqlParams[0] = new SqlParameter("@Latitude", Latitude);
                sqlParams[1] = new SqlParameter("@Longitude", Longitude);
                sqlParams[2] = new SqlParameter("@Radius", Radius);
                sqlParams[3] = new SqlParameter("@fromDate", fromDate);
                sqlParams[4] = new SqlParameter("@toDate", toDate);
                sqlParams[5] = new SqlParameter("@orgId", orgId);
                sqlParams[6] = new SqlParameter("@LandmarkID", LandmarkID);
                sqlParams[7] = new SqlParameter("@PolygonPoints", PolygonPoints);
                sqlParams[8] = new SqlParameter("@FleetIDs", FleetIDs);
                sqlParams[9] = new SqlParameter("@BoxIDs", BoxIDs);
                sqlParams[10] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "usp_VehicleAreaSearch_Get";
                //Executes SQL statement
                sqlExec.CommandTimeout = 600;
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Gets list of vehicles in polygon for selected time frame and organization
        /// </summary>
        /// <param name="Latitude">Map point latitude</param>
        /// <param name="Longitude">Map point longitude</param>
        /// <param name="Radius">Polygon radius</param>
        /// <param name="fromDate">Date from (UTC format)</param>
        /// <param name="toDate">Date to (UTC format)</param>
        /// <param name="orgId">Organization ID</param>
        /// <param name="LandmarkID">Existing landmark</param>
        /// <returns>List of vehicles</returns>
        public DataSet GetVehicleAreaSearch(double Latitude, double Longitude, double Radius, string fromDate, string toDate,
                    int orgId, int LandmarkID, string PolygonPoints, string FleetIDs, string BoxIDs, int userId)
        {
            DataSet resultSet = new DataSet();
            string prefixMsg = "";
            try
            {
                SqlParameter[] sqlParams = new SqlParameter[11];

                sqlParams[0] = new SqlParameter("@Latitude", Latitude);
                sqlParams[1] = new SqlParameter("@Longitude", Longitude);
                sqlParams[2] = new SqlParameter("@Radius", Radius);
                sqlParams[3] = new SqlParameter("@fromDate", fromDate);
                sqlParams[4] = new SqlParameter("@toDate", toDate);
                sqlParams[5] = new SqlParameter("@orgId", orgId);
                sqlParams[6] = new SqlParameter("@LandmarkID", LandmarkID);
                sqlParams[7] = new SqlParameter("@PolygonPoints", PolygonPoints);
                sqlParams[8] = new SqlParameter("@FleetIDs", FleetIDs);
                sqlParams[9] = new SqlParameter("@BoxIDs", BoxIDs);
                sqlParams[10] = new SqlParameter("@userId", userId);

                // SQL statement
                string sql = "usp_VehicleAreaSearch_Get";
                //Executes SQL statement
                sqlExec.CommandTimeout = 600;
                resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
            return resultSet;
        }
    }
}
