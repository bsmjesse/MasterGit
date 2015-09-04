using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
    public class VehicleEquipmentAssignment : TblGenInterfaces
    {
        /// <summary>
        /// Provides interfaces to VehicleEquipmentAssignment table.
        /// </summary>
      public VehicleEquipmentAssignment(SQLExecuter sqlExec)
            : base("VehicleEquipmentAssignment", sqlExec)
      {
      }

      /// <summary>
      /// Add VehicleEquipmentAssignment
      /// </summary>
      /// <param name="VehicleId"></param>
      /// <param name="EquipmentMedias"></param>
      /// <returns></returns>
      public string AddVehicleEquipmentAssignment(string VehicleIds, string EquipmentMedias)
      {
          string rowsAffected = string.Empty ;
          try
          {
              string sql = "VehicleEquipmentAssignment_Add";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@VehicleIds ", SqlDbType.VarChar, VehicleIds, -1);
              sqlExec.AddCommandParam("@EquipmentMedias", SqlDbType.VarChar, EquipmentMedias);
              sqlExec.AddCommandParam("@RETURN_VALUE", SqlDbType.VarChar, ParameterDirection.ReturnValue, string.Empty);
              sqlExec.SPExecuteNonQuery(sql);
              if (sqlExec.ReadCommandParam("@RETURN_VALUE") != null) rowsAffected = sqlExec.ReadCommandParam("@RETURN_VALUE").ToString();
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to add VehicleEquipmentAssignment. VehicleID:" + VehicleIds + " " + EquipmentMedias;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to add VehicleEquipmentAssignment. VehicleID:" + VehicleIds + " " + EquipmentMedias;
              throw new DASException(prefixMsg + " " + objException.Message);
          }

          return rowsAffected;
      }

      /// <summary>
      /// Delete Vehicle EquipmentAssignment
      /// </summary>
      /// <param name="VehicleId"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public int DeleteVehicleEquipmentAssignment(int AssignmentId)
      {
          int rowsAffected = 0;
          try
          {
              string sql = "VehicleEquipmentAssignment_Delete";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@AssignmentId", SqlDbType.Int, AssignmentId);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to delete VehicleEquipmentAssignment. AssignmentId=" + AssignmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to delete VehicleEquipmentAssignment. AssignmentId=" + AssignmentId;
              throw new DASException(prefixMsg + " " + objException.Message);
          }

          return rowsAffected;
      }

      /// <summary>
      /// Update VehicleEquipmentAssignment
      /// </summary>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <param name="EquipmentId"></param>
      /// <param name="MediaId"></param>
      /// <returns></returns>
      public int UpdateVehicleEquipmentAssignment(int EquipmentMediaAssigmentId,int EquipmentId, int MediaId)
      {
          int rowsAffected = 0;
          try
          {
              string sql = "VehicleEquipmentAssignment_Update";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.Int, EquipmentMediaAssigmentId);
              sqlExec.AddCommandParam("@EquipmentId", SqlDbType.Int, EquipmentId);
              sqlExec.AddCommandParam("@MediaId", SqlDbType.Int, MediaId);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to update VehicleEquipmentAssignment. EquipmentMediaAssigmentId:" + EquipmentMediaAssigmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to update VehicleEquipmentAssignment. EquipmentMediaAssigmentId:" + EquipmentMediaAssigmentId;
              throw new DASException(prefixMsg + " " + objException.Message);
          }

          return rowsAffected;
      }

      /// <summary>
      /// Get Vehicle EquipmentAssignment By Vehicle ID
      /// </summary>
      /// <param name="VehicleId"></param>
      /// <returns></returns>
      public DataSet GetVehicleEquipmentAssignmentByID(Int64 VehicleId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetVehicleEquipmentAssignmentByID";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, VehicleId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Vehicle EquipmentAssignment by VehicleId=" + VehicleId.ToString() + ".";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Vehicle EquipmentAssignment by VehicleId=" + VehicleId.ToString() + ".";

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;

      }

      /// <summary>
      /// Get Vehicles By EquipmentAssignment
      /// </summary>
      /// <param name="OrganizationId"></param>
      /// <returns></returns>
      public DataSet GetVehiclesByEquipmentAssignment(int OrganizationId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetVehiclesByEquipmentAssignment";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, OrganizationId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Vehicles EquipmentAssignment by OrganizationId" + OrganizationId.ToString() + ".";
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Vehicles EquipmentAssignment by OrganizationId" + OrganizationId.ToString() + ".";

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;

      }

      /// <summary>
      /// Get Vehicle Equipment Factors By VehicleId and EquipmentMediaAssigmentId
      /// </summary>
      /// <param name="VehicleId"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetVehicleEquipmentFactorsById(int AssignmentId, int UserId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetVehicleEquipmentFactorsById";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@AssignmentId", SqlDbType.Int, AssignmentId);
              sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Vehicle Equipment Factors By AssignmentId=" + AssignmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Vehicle Equipment Factors By AssignmentId=" + AssignmentId;

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;

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
          int rowsAffected = 0;
          try
          {
              string sql = "VehicleEquipmentAssignment_UpdateFactors";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@VehicleIds", SqlDbType.VarChar, VehicleIds, -1);
              if (Factor1.HasValue) sqlExec.AddCommandParam("@Factor1", SqlDbType.VarChar, Factor1.Value.ToString());
              else sqlExec.AddCommandParam("@Factor1", SqlDbType.VarChar, DBNull.Value);

              if (Factor2.HasValue) sqlExec.AddCommandParam("@Factor2", SqlDbType.VarChar, Factor2.Value.ToString());
              else sqlExec.AddCommandParam("@Factor2", SqlDbType.VarChar, DBNull.Value);

              if (Factor3.HasValue) sqlExec.AddCommandParam("@Factor3", SqlDbType.VarChar, Factor3.Value.ToString());
              else sqlExec.AddCommandParam("@Factor3", SqlDbType.VarChar, DBNull.Value);

              if (Factor4.HasValue) sqlExec.AddCommandParam("@Factor4", SqlDbType.VarChar, Factor4.Value.ToString());
              else sqlExec.AddCommandParam("@Factor4", SqlDbType.VarChar, DBNull.Value);

              if (Factor5.HasValue) sqlExec.AddCommandParam("@Factor5", SqlDbType.VarChar, Factor5.Value.ToString());
              else sqlExec.AddCommandParam("@Factor5", SqlDbType.VarChar, DBNull.Value);

              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.VarChar, EquipmentMediaAssigmentId.ToString());
              sqlExec.AddCommandParam("@UnitOfMeasureId", SqlDbType.Int, UnitOfMeasureId);
              sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);
              rowsAffected = sqlExec.SPExecuteNonQuery(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to update Vehicle Equipment Assignment. VehicleIds=" + VehicleIds;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to update Vehicle Equipment Assignment. VehicleIds=" + VehicleIds;
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return rowsAffected;
      }

      /// <summary>
      /// Get Vehicles Equipment Assignment By FleetId
      /// </summary>
      /// <param name="FleetId"></param>
      /// <returns></returns>
      public DataSet GetVehiclesEquipmentAssignmentByFleetId(int FleetId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetVehiclesEquipmentAssignmentByFleetId";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Vehicle Equipment Assignment By FleetId=" + FleetId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Vehicle Equipment Assignment By FleetId=" + FleetId;

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
      }

      /// <summary>
      /// Get Vehicles By FleetId and EquipmentMediaAssigmentId
      /// </summary>
      /// <param name="FleetId"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetVehiclesByFleetIdandEquipmentMediaAssigmentId(int FleetId, int EquipmentMediaAssigmentId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetVehiclesByFleetIdandEquipmentMediaAssigmentId";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.Int, EquipmentMediaAssigmentId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Vehicles By FleetId and EquipmentMediaAssigmentId. FleetId=" + FleetId + " EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Vehicles By FleetId and EquipmentMediaAssigmentId. FleetId=" + FleetId + " EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId;

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
      }
     
      /// <summary>
      /// Get Fleet By EquipmentMediaAssigmentId
      /// </summary>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetFleetByEquipmentMediaAssigmentId(int UserID, int EquipmentMediaAssigmentId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetFleetByEquipmentMediaAssigmentId";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.Int, EquipmentMediaAssigmentId);
              sqlExec.AddCommandParam("@UserID", SqlDbType.Int, UserID);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Fleet By EquipmentMediaAssigmentId. EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId + " UserID=" + UserID;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Fleet By EquipmentMediaAssigmentId. EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId + " UserID=" + UserID; ;

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
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
          string rowsAffected = "";
          try
          {
              string sql = "VehicleEquipmentAssignment_AssignNew";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@VehicleIds", SqlDbType.VarChar, VehicleIds, -1);

              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.VarChar, EquipmentMediaAssigmentId);

              if (Factor1.HasValue) sqlExec.AddCommandParam("@Factor1", SqlDbType.VarChar, Factor1.Value);
              else sqlExec.AddCommandParam("@Factor1", SqlDbType.VarChar, DBNull.Value);

              if (Factor2.HasValue) sqlExec.AddCommandParam("@Factor2", SqlDbType.VarChar, Factor2.Value);
              else sqlExec.AddCommandParam("@Factor2", SqlDbType.VarChar, DBNull.Value);

              if (Factor3.HasValue) sqlExec.AddCommandParam("@Factor3", SqlDbType.VarChar, Factor3.Value);
              else sqlExec.AddCommandParam("@Factor3", SqlDbType.VarChar, DBNull.Value);

              if (Factor4.HasValue) sqlExec.AddCommandParam("@Factor4", SqlDbType.VarChar, Factor4.Value);
              else sqlExec.AddCommandParam("@Factor4", SqlDbType.VarChar, DBNull.Value);

              if (Factor5.HasValue) sqlExec.AddCommandParam("@Factor5", SqlDbType.VarChar, Factor5.Value);
              else sqlExec.AddCommandParam("@Factor5", SqlDbType.VarChar, DBNull.Value);

              sqlExec.AddCommandParam("@UnitOfMeasureId", SqlDbType.Int, UnitOfMeasureId);
              sqlExec.AddCommandParam("@UserId", SqlDbType.Int, UserId);

              sqlExec.AddCommandParam("@RETURN_VALUE", SqlDbType.VarChar, ParameterDirection.ReturnValue, string.Empty);
              sqlExec.SPExecuteNonQuery(sql);

              if (sqlExec.ReadCommandParam("@RETURN_VALUE") != null) rowsAffected = sqlExec.ReadCommandParam("@RETURN_VALUE").ToString();

          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to Assign New Vehicle Equipment Assignment. VehicleIds=" + VehicleIds + " EquipmentMediaAssigmentId= " + EquipmentMediaAssigmentId.ToString();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to Assign New Vehicle Equipment Assignment. VehicleIds=" + VehicleIds + " EquipmentMediaAssigmentId= " + EquipmentMediaAssigmentId.ToString();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return rowsAffected;
      }


      /// <summary>
      /// GetVehiclesWithNoEquipmentAssignmentById
      /// </summary>
      /// <param name="FleetId"></param>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetVehiclesWithNoEquipmentAssignmentById(int FleetId, int EquipmentMediaAssigmentId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetVehiclesWithNoEquipmentAssignmentById";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, FleetId);
              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.Int, EquipmentMediaAssigmentId);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Vehicles With No EquipmentAssignment By Id. FleetId=" + FleetId + " EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Vehicles With No EquipmentAssignment By Id. FleetId=" + FleetId + " EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId;

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
      }

      /// <summary>
      /// GetFleetWithNoEquipmentAssignmentById
      /// </summary>
      /// <param name="EquipmentMediaAssigmentId"></param>
      /// <returns></returns>
      public DataSet GetFleetWithNoEquipmentAssignmentById(int UserID, int EquipmentMediaAssigmentId)
      {
          DataSet sqlDataSet = null;
          try
          {

              string sql = "GetFleetWithNoEquipmentAssignmentById";

              sqlExec.ClearCommandParameters();

              sqlExec.AddCommandParam("@EquipmentMediaAssigmentId", SqlDbType.Int, EquipmentMediaAssigmentId);
              sqlExec.AddCommandParam("@UserID", SqlDbType.Int, UserID);

              //Executes SQL statement
              sqlDataSet = sqlExec.SPExecuteDataset(sql);
          }
          catch (SqlException objException)
          {
              string prefixMsg = "Unable to retrieve Fleet With No EquipmentAssignment By Id. EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId + " UserID=" + UserID;
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              string prefixMsg = "Unable to retrieve Fleet With No EquipmentAssignment By Id. EquipmentMediaAssigmentId=" + EquipmentMediaAssigmentId + " UserID=" + UserID;

              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return sqlDataSet;
      }
    }
}
