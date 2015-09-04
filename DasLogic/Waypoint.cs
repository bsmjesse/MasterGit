using System;
using System.Collections.Generic;
using System.Text;
using System.Data; 

namespace VLF.DAS.Logic
{
    public class Waypoint : Das
    {
        DB.Waypoint wayPoint = null;

        public Waypoint(string connectionString): base(connectionString)
		{
            wayPoint = new DB.Waypoint(sqlExec);
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
            return wayPoint.Waypoint_Add_Update(WaypointId, WaypointName, TypeId, Latitude, Longitude, OrganizationId, Persistent);
        }

          /// <summary>
        /// Get Waypoint Types
        /// </summary>
        /// <returns></returns>
        public DataSet GetWaypointType()
        {
            return wayPoint.GetWaypointType(); 
        }
       
          /// <summary>
        /// Get Organization Waypoints
        /// </summary>
        /// <returns></returns>
        public DataSet GetOrganizationWaypoints(int OrgId)
        {
            return wayPoint.GetOrganizationWaypoints(OrgId);   
        }
    }

}