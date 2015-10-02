using System;
using System.Data.SqlClient;	// for SqlException
using System.Data;			// for DataSet
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfLandmark table.
   /// </summary>
    public class Waypoint : TblGenInterfaces
    {

        public Waypoint(SQLExecuter sqlExec): base("vlfvlfWaypoint", sqlExec)
       {
         
       }

       /// <summary>
       /// Add or Update Waypoint. Note: Waypoint should be equal to -1 for new waypoint.
       /// </summary>
       /// <param name="WaypointId"></param>
       /// <param name="WaypointName"></param>
       /// <param name="TypeId"></param>
       /// <param name="Latitude"></param>
       /// <param name="Longitude"></param>
       /// <param name="OrganizationId"></param>
       /// <param name="Persistent"></param>
       /// <returns></returns>
        public int Waypoint_Add_Update(int WaypointId, string WaypointName, int TypeId, float Latitude, float Longitude, int OrganizationId, Int16 Persistent)
        {
            int rowsAffected = 0;
            try
            {

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@WaypointId", SqlDbType.Int, WaypointId);
                sqlExec.AddCommandParam("@WaypointName", SqlDbType.VarChar, @WaypointName);
                sqlExec.AddCommandParam("@TypeId", SqlDbType.Int, @TypeId);
                sqlExec.AddCommandParam("@Latitude", SqlDbType.Float, Latitude);
                sqlExec.AddCommandParam("@Longitude", SqlDbType.Float, Latitude);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                rowsAffected = sqlExec.SPExecuteNonQuery("sp_Waypoint_Add_Update");
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add or Update Waypoint=" + WaypointId + " WaypointName='" + WaypointName + "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add or Update Waypoint=" + WaypointId + " WaypointName='" + WaypointName + "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

         return rowsAffected;
        }


        /// <summary>
        /// Get Waypoint Types
        /// </summary>
        /// <returns></returns>
        public DataSet GetWaypointType()
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "select * from vlfWaypointType";

                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve WaypointTypes";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve WaypointTypes";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;

        }


        /// <summary>
        /// Get Organization Waypoints
        /// </summary>
        /// <returns></returns>
        public DataSet GetOrganizationWaypoints(int OrgId)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "select WaypointId,WaypointName,vlfWaypoint.TypeId,vlfWaypointType.TypeName,Latitude,Longitude, StreetAddress,Persistent from  vlfWaypoint INNER JOIN vlfWaypointType ON vlfWaypoint.TypeId=vlfWaypointType.TypeId where OrganizationId=" + OrgId;

                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve WaypointTypes";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve WaypointTypes";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;

        }
        
    }
}
