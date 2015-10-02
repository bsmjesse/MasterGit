using System;
using System.Data ;			   // for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to driver functionality in database
    /// </summary>
    public class EquipmentManager : Das
    {
        private VLF.DAS.DB.Equipment _equipment = null;

        public EquipmentManager(string connectionString) : base(connectionString)
		{
            _equipment = new VLF.DAS.DB.Equipment(sqlExec);
        
        }

        /// <summary>
        /// Add Equipment 
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentTypeId"></param>
        /// <returns></returns>
        /// 
        public int AddEquipment(int OrganizationId, string Description, int EquipmentTypeId)
        {
            return _equipment.AddEquipment(OrganizationId, Description, EquipmentTypeId);
        }

        /// <summary>
        /// Update Equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="Description"></param>
        /// <param name="EquipmentTypeId"></param>
        /// <returns></returns>
        public int UpdateEquipment(int EquipmentId, int OrganizationId, string Description, int EquipmentTypeId)
        {
            return _equipment.UpdateEquipment(EquipmentId, OrganizationId, Description, EquipmentTypeId);
        }

        /// <summary>
        /// Delete Equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <returns></returns>
        public int DeleteEquipment(int EquipmentId)
        {
            return _equipment.DeleteEquipment(EquipmentId);
        }

        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationEquipments(int OrganizationId)
        {
            return _equipment.GetOrganizationEquipments(OrganizationId);
        }

        /// <summary>
        /// Get Equipments Types
        /// </summary>
        /// <returns></returns>
        public DataSet GetEquipmentTypes()
        {
            return _equipment.GetEquipmentTypes();
        }

    }
}
