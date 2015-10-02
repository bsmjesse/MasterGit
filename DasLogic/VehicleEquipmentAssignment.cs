using System;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to driver functionality in database
    /// </summary>
    public class VehicleEquipmentAssignmentManager:Das
    {
        private VLF.DAS.DB.VehicleEquipmentAssignment _vehicleEquipmentAssignment = null;

        public VehicleEquipmentAssignmentManager(string connectionString)
           : base(connectionString)
		{
            _vehicleEquipmentAssignment = new VLF.DAS.DB.VehicleEquipmentAssignment(sqlExec);
        
        }

        /// <summary>
        /// Add VehicleEquipmentAssignment
        /// </summary>
        /// <param name="VehicleId"></param>
        /// <param name="EquipmentMedias"></param>
        /// <returns></returns>
        public string AddVehicleEquipmentAssignment(string VehicleIds, string EquipmentMedias)
        {
            return _vehicleEquipmentAssignment.AddVehicleEquipmentAssignment(VehicleIds, EquipmentMedias);
        }

        /// <summary>
        /// Delete Vehicle EquipmentAssignment
        /// </summary>
        /// <param name="VehicleId"></param>
        /// <param name="EquipmentMediaAssigmentId"></param>
        /// <returns></returns>
        public int DeleteVehicleEquipmentAssignment(int AssignmentId)
        {
            return _vehicleEquipmentAssignment.DeleteVehicleEquipmentAssignment(AssignmentId);
        }

        /// <summary>
        /// Update VehicleEquipmentAssignment
        /// </summary>
        /// <param name="EquipmentMediaAssigmentId"></param>
        /// <param name="EquipmentId"></param>
        /// <param name="MediaId"></param>
        /// <returns></returns>
        public int UpdateVehicleEquipmentAssignment(int EquipmentMediaAssigmentId, int EquipmentId, int MediaId)
        {
            return _vehicleEquipmentAssignment.UpdateVehicleEquipmentAssignment(EquipmentMediaAssigmentId, EquipmentId, MediaId);
        }

        /// <summary>
        /// Get Vehicle EquipmentAssignment By Vehicle ID
        /// </summary>
        /// <param name="VehicleId"></param>
        /// <returns></returns>
        public DataSet GetVehicleEquipmentAssignmentByID(Int64 VehicleId)
        {
            return _vehicleEquipmentAssignment.GetVehicleEquipmentAssignmentByID(VehicleId);
        }

        /// <summary>
        /// Get Vehicles By EquipmentAssignment
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetVehiclesByEquipmentAssignment(int OrganizationId)
        {
            return _vehicleEquipmentAssignment.GetVehiclesByEquipmentAssignment(OrganizationId);
        }
        /// <summary>
        /// Get Vehicle Equipment Factors By VehicleId and EquipmentMediaAssigmentId
        /// </summary>
        /// <param name="VehicleId"></param>
        /// <param name="EquipmentMediaAssigmentId"></param>
        /// <returns></returns>
        public DataSet GetVehicleEquipmentFactorsById(int AssignmentId, int UserId)
        {
            return _vehicleEquipmentAssignment.GetVehicleEquipmentFactorsById(AssignmentId, UserId);
        }


        /// <summary>
        /// Update Vehicle Equipment Assignment Factors
        /// </summary>
        /// <param name="VehicleIds"></param>
        /// <param name="Factor1"></param>
        /// <param name="Factor2"></param>
        /// <param name="Factor3"></param>
        /// <param name="Factor4"></param>
        /// <param name="Factor5"></param>
        /// <param name="EquipmentMediaAssigmentId"></param>
        /// <returns></returns>
        public int VehicleEquipmentAssignment_UpdateFactors(string VehicleIds,
            double? Factor1, double? Factor2, double? Factor3, double? Factor4, double? Factor5, int EquipmentMediaAssigmentId,
            int UnitOfMeasureId, int UserId)
        {
            return _vehicleEquipmentAssignment.VehicleEquipmentAssignment_UpdateFactors(VehicleIds, Factor1, Factor2, Factor3, Factor4, Factor5,
                EquipmentMediaAssigmentId, UnitOfMeasureId, UserId);
        }

      /// <summary>
      /// Get Vehicles Equipment Assignment By FleetId
      /// </summary>
      /// <param name="FleetId"></param>
      /// <returns></returns>
        public DataSet GetVehiclesEquipmentAssignmentByFleetId(int FleetId)
        {
            return _vehicleEquipmentAssignment.GetVehiclesEquipmentAssignmentByFleetId(FleetId);
        }

      /// <summary>
      /// Get Vehicles By FleetId and EquipmentMediaAssigmentId
      /// </summary>
      /// <param name="FleetId"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetVehiclesByFleetIdandEquipmentMediaAssigmentId(int FleetId, int EquipmentMediaAssigmentId)
      {
          return _vehicleEquipmentAssignment.GetVehiclesByFleetIdandEquipmentMediaAssigmentId(FleetId, EquipmentMediaAssigmentId);
      }
     
      /// <summary>
      /// Get Fleet By EquipmentMediaAssigmentId
      /// </summary>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetFleetByEquipmentMediaAssigmentId(int UserID, int EquipmentMediaAssigmentId)
      { 
          return _vehicleEquipmentAssignment.GetFleetByEquipmentMediaAssigmentId(UserID, EquipmentMediaAssigmentId);
      }


      /// <summary>
      /// VehicleEquipmentAssignment_AssignNew
      /// </summary>
      /// <param name="VehicleIds"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <param name="Factor1"></param>
      /// <param name="Factor2"></param>
      /// <param name="Factor3"></param>
      /// <param name="Factor4"></param>
      /// <param name="Factor5"></param>
      /// <returns></returns>
      public string VehicleEquipmentAssignment_AssignNew(string VehicleIds, int EquipmentMediaAssigmentId,
          double? Factor1, double? Factor2, double? Factor3, double? Factor4, double? Factor5,
          int UnitOfMeasureId, int UserId)
      {
          return _vehicleEquipmentAssignment.VehicleEquipmentAssignment_AssignNew(VehicleIds, EquipmentMediaAssigmentId,
              Factor1, Factor2, Factor3, Factor4, Factor5, UnitOfMeasureId, UserId);
      }

      /// <summary>
      /// GetVehiclesWithNoEquipmentAssignmentById
      /// </summary>
      /// <param name="FleetId"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetVehiclesWithNoEquipmentAssignmentById(int FleetId, int EquipmentMediaAssigmentId)
      {
          return _vehicleEquipmentAssignment.GetVehiclesWithNoEquipmentAssignmentById(FleetId, EquipmentMediaAssigmentId);
      }

      /// <summary>
      /// GetFleetWithNoEquipmentAssignmentById
      /// </summary>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetFleetWithNoEquipmentAssignmentById(int UserID, int EquipmentMediaAssigmentId)
      {
          return _vehicleEquipmentAssignment.GetFleetWithNoEquipmentAssignmentById(UserID, EquipmentMediaAssigmentId);
      }
    }
}
