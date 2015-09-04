using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace VLF.DAS.Logic
{
    public class FuelCategoryMgr: Das
    {

        VLF.DAS.DB.FuelCategory _fuelCategory;

        public FuelCategoryMgr(string connectionString)
            : base(connectionString)
		{
            _fuelCategory = new VLF.DAS.DB.FuelCategory(sqlExec);
        
        }


        /// <summary>
        /// Delete equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <returns></returns>
        public int FuelCategory_Delete(int FuelTypeID,
                   int OrganizationID)
        {

            return _fuelCategory.FuelCategory_Delete(FuelTypeID,
                   OrganizationID);
        }
        /// <summary>
        /// Update Equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentTypeId"></param>
        /// <returns></returns>
        public int FuelCategory_Update(int FuelTypeID,
                                int OrganizationId,
                                string FuelType,
                                string GHGCategory,
                                string GHGCategoryDesc,
                                float CO2Factor)
        {
            return _fuelCategory.FuelCategory_Update(FuelTypeID,
                                OrganizationId,
                                FuelType,
                                GHGCategory,
                                GHGCategoryDesc,
                                CO2Factor);;
        }
        /// <summary>
        /// Add Equipment 
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentTypeId"></param>
        /// <returns></returns>
        public int FuelCategory_Add(int OrganizationId,
                                string FuelType,
                                string GHGCategory,
                                string GHGCategoryDesc,
                                float CO2Factor)
        {

            return _fuelCategory.FuelCategory_Add(OrganizationId,
                                FuelType,
                                GHGCategory,
                                GHGCategoryDesc,
                                CO2Factor);

        }

        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet FuelCategory_Select(int FuelTypeID,
                                int OrganizationId, Boolean IncludeAssign)
        {
            return _fuelCategory.FuelCategory_Select(FuelTypeID,
                                OrganizationId, IncludeAssign);

        }

        public int FuelCategory_UpdateVehicleCo2(int FuelTypeID,
                               int OrganizationId,
                               Int64 VehicleId
        )
        {
            return _fuelCategory.FuelCategory_UpdateVehicleCo2(FuelTypeID,
                                OrganizationId, VehicleId);

        }
    }
}
