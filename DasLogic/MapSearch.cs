using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    public class MapSearch : Das
    {
        private VLF.DAS.DB.MapSearch _mapSearch = null;

        public MapSearch(string connectionString)
            : base(connectionString)
        {
            _mapSearch = new VLF.DAS.DB.MapSearch(sqlExec);

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
            return _mapSearch.GetVehicleAreaSearch_NewTZ(Latitude, Longitude, Radius, fromDate, toDate, orgId, LandmarkID, PolygonPoints, FleetIDs, BoxIDs, userId);
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
            return _mapSearch.GetVehicleAreaSearch(Latitude, Longitude, Radius, fromDate, toDate, orgId, LandmarkID, PolygonPoints, FleetIDs, BoxIDs, userId);
        }
    }
}
