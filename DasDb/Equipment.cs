using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to Equipment table.
   /// </summary>
   
    public class Equipment: TblGenInterfaces
    {
      public Equipment(SQLExecuter sqlExec) : base("Equipment", sqlExec)
      {
      }

      /// <summary>
      /// Delete equipment
      /// </summary>
      /// <param name="EquipmentId"></param>
      /// <returns></returns>
      public int DeleteEquipment(int EquipmentId)
      {
          int rowsAffected = 0;
          try
          {
              string sql = "Equipment_Delete";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@EquipmentId", SqlDbType.Int, EquipmentId);
              sqlExec.AddCommandParam("@RETURN_VALUE", SqlDbType.Int, ParameterDirection.ReturnValue, 0);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
              if (sqlExec.ReadCommandParam("@RETURN_VALUE") != null && sqlExec.ReadCommandParam("@RETURN_VALUE").ToString() == "-1") return -1;
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to Delete Equipment. EquipmentId:" + EquipmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to Delete Equipment. EquipmentId:" + EquipmentId;
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          if (rowsAffected == 0)
          {
              string prefixMsg = "Unable to Delete Equipment. EquipmentId:" + EquipmentId;
              throw new DASAppDataAlreadyExistsException(prefixMsg + " This Equipment does not exist.");
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
      public int UpdateEquipment(int EquipmentId, int OrganizationId, string Description, int EquipmentTypeId)
      {
          int rowsAffected = 0;
          try
          {
              string sql = "Equipment_Update";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@EquipmentId", SqlDbType.Int, EquipmentId);
              sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);
              sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, Description);
              sqlExec.AddCommandParam("@EquipmentTypeId", SqlDbType.SmallInt, EquipmentTypeId);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to update Equipment to OrganizationId:" + OrganizationId + " EquipmentId:" + EquipmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to update Equipment to OrganizationId:" + OrganizationId + " EquipmentId:" + EquipmentId;
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          if (rowsAffected == 0)
          {
              string prefixMsg = "Unable to update Equipment to OrganizationId:" + OrganizationId + " EquipmentId:" + EquipmentId; 
              throw new DASAppDataAlreadyExistsException(prefixMsg + " This Equipment does not exist.");
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
      public int AddEquipment(int OrganizationId, string Description, int EquipmentTypeId)
      {
         int rowsAffected = 0;
         try
            {
              string sql = "Equipment_Add";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int , OrganizationId);
              sqlExec.AddCommandParam("@Description", SqlDbType.VarChar , Description);
              sqlExec.AddCommandParam("@EquipmentTypeId", SqlDbType.SmallInt, EquipmentTypeId);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add Equipment to OrganizationId:" + OrganizationId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add Equipment to OrganizationId:" + OrganizationId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add Equipment to OrganizationId:" + OrganizationId;
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This Equipment already exists.");
            }

            return rowsAffected;
      }

    /// <summary>
    /// Get Organization Equipments
    /// </summary>
    /// <param name="OrganizationId"></param>
    /// <returns></returns>
      public DataSet GetOrganizationEquipments(int OrganizationId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetOrganizationEquipments";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
           }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Equipments by OrganizationId=" + OrganizationId.ToString() + ".";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
               string prefixMsg = "Unable to retrieve Equipments by OrganizationId=" + OrganizationId.ToString() + ".";
            
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;

      }

        /// <summary>
        /// Get Equipments Types
        /// </summary>
        /// <returns></returns>
      public DataSet GetEquipmentTypes()
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetEquipmentTypes";

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Equipments Types";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Equipments Types";

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;

      }

    }
}
