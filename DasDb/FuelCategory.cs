using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.CLS;
using VLF.ERR;

namespace VLF.DAS.DB
{
    public class FuelCategory : TblGenInterfaces
    {
        public FuelCategory(SQLExecuter sqlExec)
            : base("vlfFuelCategory", sqlExec)
        {
        }

        /// <summary>
        /// Delete equipment
        /// </summary>
        /// <param name="EquipmentId"></param>
        /// <returns></returns>
        public int FuelCategory_Delete(int FuelTypeID,
                   int OrganizationID)
        {
            int rowsAffected = 0;
            try
            {
                string sql = "[vlfFuelCategory_Delete]";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@FuelTypeID", SqlDbType.Int, FuelTypeID);
                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, OrganizationID);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Delete FuelCategory. FuelTypeID:" + FuelTypeID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Delete FuelCategory. FuelTypeID:" + FuelTypeID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to Delete FuelCategory. FuelTypeID:" + FuelTypeID;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This FuelCategory does not exist.");
            }
            return rowsAffected;
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
            int rowsAffected = 0;
            try
            {
                string sql = "[vlfFuelCategory_Update]";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FuelTypeID", SqlDbType.Int, FuelTypeID);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@FuelType", SqlDbType.VarChar, FuelType);
                sqlExec.AddCommandParam("@GHGCategory", SqlDbType.VarChar, GHGCategory);
                sqlExec.AddCommandParam("@GHGCategoryDesc", SqlDbType.VarChar, GHGCategoryDesc);
                sqlExec.AddCommandParam("@CO2Factor", SqlDbType.Float, CO2Factor);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update FuelType to OrganizationId:" + OrganizationId + " FuelTypeID:" + FuelTypeID;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update FuelType to OrganizationId:" + OrganizationId + " FuelTypeID:" + FuelTypeID;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update FuelType to OrganizationId:" + OrganizationId + " FuelTypeID:" + FuelTypeID;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This FuelType does not exist.");
            }
            return rowsAffected;
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
            int rowsAffected = 0;
            try
            {
                string sql = "vlfFuelCategory_Add";

                sqlExec.ClearCommandParameters();

                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@FuelType", SqlDbType.VarChar, FuelType);
                sqlExec.AddCommandParam("@GHGCategory", SqlDbType.VarChar, GHGCategory);
                sqlExec.AddCommandParam("@GHGCategoryDesc", SqlDbType.VarChar, GHGCategoryDesc);
                sqlExec.AddCommandParam("@CO2Factor", SqlDbType.Float, CO2Factor);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add FuelCategory to OrganizationId:" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add FuelCategory to OrganizationId:" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add FuelCategory to OrganizationId:" + OrganizationId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This Equipment already exists.");
            }

            return rowsAffected;
        }

        /// <summary>
        /// Get Organization Equipments
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public DataSet FuelCategory_Select(int FuelTypeID,
                                int OrganizationId, bool IncludeAssign)
        {
            DataSet sqlDataSet = null;
            try
            {

                string sql = "vlfFuelCategory_Select";

                sqlExec.ClearCommandParameters();


                sqlExec.AddCommandParam("@FuelTypeID", SqlDbType.Int, FuelTypeID);
                sqlExec.AddCommandParam("@OrganizationID", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@IncludeAssign", SqlDbType.Bit, IncludeAssign);

                //Executes SQL statement
                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve FuelCategory by OrganizationId=" + OrganizationId.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve FuelCategory by OrganizationId=" + OrganizationId.ToString() + ".";

                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;

        }


        public int FuelCategory_UpdateVehicleCo2(int FuelTypeID,
                                int OrganizationId,
                                Int64 VehicleId
          )
        {
            int rowsAffected = 0;
            try
            {
                string sql = "[vlfFuelCategory_UpdateVehicleCo2]";

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FuelTypeID", SqlDbType.Int, FuelTypeID);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
                sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, VehicleId);
                rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update CO2 factor to OrganizationId:" + OrganizationId + " VehicleId:" + VehicleId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update CO2 factor to OrganizationId:" + OrganizationId + " VehicleId:" + VehicleId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update CO2 factor to OrganizationId:" + OrganizationId + " VehicleId:" + VehicleId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This Vehicle does not exist.");
            }
            return rowsAffected;
        }

    }
}

