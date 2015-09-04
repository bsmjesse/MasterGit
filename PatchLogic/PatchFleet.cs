using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.PATCH.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.PATCH.Logic
{
    public class PatchFleet : VLF.DAS.Das
    {
        private VLF.PATCH.DB.PatchFleet _fleet = null;

        public PatchFleet(string connectionString)
           : base(connectionString)
		{
            _fleet = new VLF.PATCH.DB.PatchFleet(sqlExec);        
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public int AddUserToFleet(int organizationId, int fleetId, int userId, int userIdAddToFleet)
        {
            if(!_fleet.ValidateUserAssignedToFleet(organizationId, userId, fleetId))
                return -1;
            
            return _fleet.AddUserToFleet(fleetId, userIdAddToFleet);
        }

        public int DeleteUserFromFleet(int organizationId, int fleetId, int userId, int userIdAddToFleet)
        {
            if (!_fleet.ValidateUserAssignedToFleet(organizationId, userId, fleetId))
                return -1;

            return _fleet.DeleteUserFromFleet(fleetId, userIdAddToFleet);
        }

        public int AddVehicleToFleet(int organizationId, int fleetId, int userId, int vehicleId)
        {
            if (!_fleet.ValidateUserAssignedToFleet(organizationId, userId, fleetId))
                return -1;

            return _fleet.AddVehicleToFleet(fleetId, vehicleId);
        }

        public int DeleteVehicleFromFleet(int organizationId, int fleetId, int userId, int vehicleId)
        {
            if (!_fleet.ValidateUserAssignedToFleet(organizationId, userId, fleetId))
                return -1;

            return _fleet.DeleteVehicleFromFleet(fleetId, vehicleId);
        }

        public DataSet GetUsersInfoByFleetId(int fleetId)
        {
            return _fleet.GetUsersInfoByFleetId(fleetId);
        }

        public bool ValidateUserAssignedToFleet(int organizationId, int userId, int fleetId)
        {
            return _fleet.ValidateUserAssignedToFleet(organizationId, userId, fleetId);
        }

        public DataSet GetVehiclesInfoByFleetIdByPage(int fleetId, int pageSize, int page)
        {
            return _fleet.GetVehiclesInfoByFleetIdByPage(fleetId, pageSize, page);
        }

        public DataSet GetVehiclesInfoByFleetIdByFilterByPage(int fleetId, int pageSize, int page, string filter)
        {
            return _fleet.GetVehiclesInfoByFleetIdByFilterByPage(fleetId, pageSize, page, filter);
        }

        public int GetVehiclesInfoTotalNumberByFleetId(int fleetId)
        {
            return _fleet.GetVehiclesInfoTotalNumberByFleetId(fleetId);
        }

        public int GetVehiclesInfoTotalNumberByFilterByFleetId(int fleetId, string filter)
        {
            return _fleet.GetVehiclesInfoTotalNumberByFileterFleetId(fleetId, filter);
        }
    }
}
