using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using System.Threading;		// for Thread.Sleep
using System.Collections;	// for ArrayList
using System.Xml ;			// for XML components
using VLF.ERR;
using VLF.DAS.DB;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Text;
using VLF.CLS.Def;


namespace VLF.DAS.Logic
{
   /// <summary>
   /// Provides interface to vehicle functionality in database
   /// </summary>
   public partial class Vehicle : Das
   {
      DB.BoxSensorsCfg boxSnsCfg = null;
      DB.BoxOutputsCfg boxOutputsCfg = null;
      DB.VehicleAssignment vehicleAssignment = null;
      DB.BoxProtocolTypeCmdOutType boxProtocolTypeCmdOutType = null;
      DB.Box box = null;
      DB.VehicleInfo vehicleInfo = null;
      DB.VehicleGeozone vehicleGeozone = null;
      DB.VehicleMaintenaceInfo vehicleMaintenance = null;
      DB.VehicleServices vehicleServices = null;

      #region General Interfaces
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="connectionString"></param>
      public Vehicle(string connectionString) : base(connectionString)
      {
         boxSnsCfg = new DB.BoxSensorsCfg(sqlExec);
         boxOutputsCfg = new DB.BoxOutputsCfg(sqlExec);
         vehicleAssignment = new DB.VehicleAssignment(sqlExec);
         boxProtocolTypeCmdOutType = new DB.BoxProtocolTypeCmdOutType(sqlExec);
         box = new DB.Box(sqlExec);
         vehicleInfo = new DB.VehicleInfo(sqlExec);
         vehicleGeozone = new DB.VehicleGeozone(sqlExec);
         vehicleMaintenance = new DB.VehicleMaintenaceInfo(sqlExec);
         vehicleServices = new DB.VehicleServices(sqlExec);
      }
      /// <summary>
      /// Destructor
      /// </summary>
      public new void Dispose()
      {
         base.Dispose();
      }
      #endregion

      #region Vehicle Configuration Interfaces
      /// <summary>
      /// Get vehicle sensors information by license plate
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>Structure [SensorId][SensorName][SensorAction][AlarmLevel]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public string[,] GetVehicleSensorsInfoStruct(string licensePlate)
      {
         int boxID = GetBoxIdByLicensePlate(licensePlate);
         return GetVehicleSensorsInfoStruct(boxID);
      }
      /// <summary>
      ///      Get vehicle sensors information by box id 
      /// </summary>
      /// <param name="boxID"></param>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>Structure [SensorId][SensorName][SensorAction][AlarmLevel]</returns>
      /// <comment>
      ///      it leverages the logical functions available at this level
      /// </comment>
      public string[,] GetVehicleSensorsInfoStruct(int boxID)
      {
/*
         string[,] strSupportedSensors = boxSnsCfg.GetSensorsInfoStructByBoxId(boxID);
         return strSupportedSensors;
*/
         string[,] resultArr = null;


         DataSet sqlDataSet = GetVehicleSensorsInfo(boxID);
         if ((sqlDataSet != null) && (sqlDataSet.Tables[0].Rows.Count > 0))
         {
            resultArr = new string[sqlDataSet.Tables[0].Rows.Count, sqlDataSet.Tables[0].Columns.Count];
            int index = 0;
            foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
            {
               resultArr[index, 0] = Convert.ToString(currRow[0]);
               resultArr[index, 1] = Convert.ToString(currRow[1]).TrimEnd();
               resultArr[index, 2] = Convert.ToString(currRow[2]).TrimEnd();
               resultArr[index, 3] = Convert.ToString(currRow[3]);
               resultArr[index, 4] = Convert.ToString(currRow[4]);
               ++index;
            }
         }

         return resultArr;
      }
      /// <summary>
      /// Get vehicle sensors information by license plate
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleSensorsInfo"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <param name="licensePlate"></param>
      /// <returns>DataSet [SensorId][SensorName][SensorAction][AlarmLevel]</returns>
      public DataSet GetVehicleSensorsInfo(string licensePlate)
      {
         int boxID = GetBoxIdByLicensePlate(licensePlate);
         DataSet dsResult = GetVehicleSensorsInfo(boxID);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleSensorsInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      /// <summary>
      /// Get vehicle sensors information by box id 
      /// </summary>
      /// <param name="boxID"></param>
      /// <remarks>
      /// TableName	= "VehicleSensorsInfo"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>DataSet [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
      public DataSet GetVehicleSensorsInfo(int boxID)
      {

         DataSet dsResult = boxSnsCfg.GetSensorsInfoByBoxId(boxID);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleSensorsInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;

      }
      /// <summary>
      /// Get vehicle outputs information by box id 
      /// </summary>
      /// <param name="boxID"></param>
      /// <param name="userId"></param>
      /// <returns>Structure [OutputId][OutputName][OutputAction]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public string[,] GetVehicleOutputsInfoStruct(int boxID, int userId)
      {
         return boxOutputsCfg.GetOutputsInfoStructByBoxId(boxID, userId);
      }
      /// <summary>
      /// Get vehicle outputs information by box id 
      /// </summary>
      /// <param name="boxID"></param>
      /// <param name="userId"></param>
      /// <remarks>
      /// TableName	= "VehicleOutputsInfo"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
      public DataSet GetVehicleOutputsInfo(int boxID, int userId)
      {
         DataSet dsResult = boxOutputsCfg.GetOutputsInfoByBoxId(boxID, userId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleOutputsInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Get vehicle outputs information from  by licence plate and user id
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="userId"></param>
      /// <returns>Structure [OutputId][OutputName][OutputAction]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public string[,] GetVehicleOutputsInfoStruct(string licensePlate, int userId)
      {
         string[,] resultArr = null;
         object boxID = vehicleAssignment.GetVehicleAssignmentField("BoxId", "LicensePlate", licensePlate);
         if (boxID != null)
            resultArr = GetVehicleOutputsInfoStruct(Convert.ToInt32(boxID), userId);
         return resultArr;
      }

      /// <summary>
      /// Get vehicle outputs information by licence plate and user id
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="userId"></param>
      /// <remarks>
      /// TableName	= "VehicleOutputsInfo"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
      public DataSet GetVehicleOutputsInfo(string licensePlate, int userId)
      {
         DataSet dsResult = null;
         object boxID = vehicleAssignment.GetVehicleAssignmentField("BoxId", "LicensePlate", licensePlate);
         if (boxID != null)
         {
            dsResult = GetVehicleOutputsInfo(Convert.ToInt32(boxID), userId);
         }
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleOutputsInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Get vehicle commands information by box Id and user id
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="userId"></param>
      /// <remarks>
      /// TableName	= "VehicleCommandsInfo"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>DataSet [BoxCmdOutTypeId],[BoxCmdOutTypeName],[Rules],[BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
      public DataSet GetVehicleCommandsInfo(int boxId, int userId)
      {
         DataSet dsResult = boxProtocolTypeCmdOutType.GetAllSupportedCommands(boxId, userId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleCommandsInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      /// <summary>
      /// Retrieves all supported protocol types for current command
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="userId"></param>
      /// <param name="commandTypeId"></param>
      /// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetCommandProtocolTypesInfo(int boxId, int userId, short commandTypeId)
      {
         DataSet dsResult = boxProtocolTypeCmdOutType.GetCommandProtocolTypesInfo(boxId, userId, commandTypeId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleCommandProtocolTypesInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Get vehicle commands information by license plate and user id
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="userId"></param>
      /// <returns>Structure [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public string[,] GetVehicleCommandsInfoStruct(string licensePlate, int userId)
      {
         string[,] resultArr = null;
         object boxId = vehicleAssignment.GetVehicleAssignmentField("BoxId", "LicensePlate", licensePlate);
         if (boxId != null)
         {
            try
            {
               resultArr = GetVehicleCommandsInfoStruct(Convert.ToInt32(boxId), userId);
            }
            catch (DASDbConnectionClosed exCnn)
            {
               throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
               throw new DASAppInvalidValueException("Invalid box id. " + objException.Message);
            }
         }
         return resultArr;
      }

      /// <summary>
      /// Get vehicle commands information by license plate and user id
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="userId"></param>
      /// <remarks>
      /// TableName	= "VehicleCommandsInfo"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>DataSet [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules]</returns>
      public DataSet GetVehicleCommandsInfo(string licensePlate, int userId)
      {
         DataSet dsResult = null;
         object boxId = vehicleAssignment.GetVehicleAssignmentField("BoxId", "LicensePlate", licensePlate);
         if (boxId != null)
         {
            try
            {
               dsResult = GetVehicleCommandsInfo(Convert.ToInt32(boxId), userId);
            }
            catch (DASDbConnectionClosed exCnn)
            {
               throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
               throw new DASAppInvalidValueException("Invalid box id. " + objException.Message);
            }
         }
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleCommandsInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      public DataSet GetMultiVehicleCommandsInfo(string Boxid, int userId)
      {
          DataSet dsResult = null;
         
            try
            {
                dsResult = boxProtocolTypeCmdOutType.GetAllDistinctSupportedCommands(Boxid, userId);
               
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASAppInvalidValueException("Invalid box id. " + objException.Message);
            }
          
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "VehicleCommandsInfo";
              }
              dsResult.DataSetName = "Vehicle";
          }
          return dsResult;
      }

      /// <summary>
      /// Get vehicle commands information by license plate and user id
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="userId"></param>
      /// <returns>Structure [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public string[,] GetVehicleCommandsInfoStruct(int boxId, int userId)
      {
         return GetVehicleCommandsInfoStruct(boxId, userId);
      }

      /// <summary>
      /// Gets vehicle active assignment configuration information
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [Description],[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetActiveVehicleCfgInfo(Int64 vehicleId)
      {
         DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
         DataSet dsResult = vehicleInfo.GetActiveVehicleCfgInfo(vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetActiveVehicleCfgInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      #endregion

      #region Vehicle Assignment Interfaces

       // Changes for TimeZone feature start
      /// <summary>
      /// Add new Vehicle.
      ///		- Add new vehicle information into vlfVehicleInfo table.
      ///		- Assign new vehicle with user id, box and license plate (vlfVehicleAssignment table).
      ///		- Backup information into vlfVehicleAssignmentHst table.
      /// </summary>
      /// <param name="vehicInfo"></param>
      /// <param name="vehicAssign"></param>
      /// <param name="organizationId"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if box and vehicle do no assigned to same organization.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public long AddVehicle_NewTZ(VehicInfo vehicInfo, VehicAssign vehicAssign, int organizationId)
      {
          long vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
          DB.Fleet fleet = new DB.Fleet(sqlExec);
          int fleetId = fleet.GetFleetIdByFleetName(organizationId, VLF.CLS.Def.Const.defaultFleetName);

          try
          {
              // 1. Begin transaction
              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

              // 2. Creates VehicleInfo database object
              //VehicleInfo vehicleInfo = new VehicleInfo(sqlExec);

              // 3. Add/Update vehicle info and set vehicle id to assignment structure
              bool vehicleExist = false;
              vehicleId = vehicleInfo.AddVehicleInfo_NewTZ(vehicInfo, organizationId, ref vehicleExist);
              vehicAssign.vehicleId = vehicleId;

              // 4. Check if box and vehicle assigned to same organization
              int vehicleOrganizationId = vehicleInfo.GetVehicleOrganization(vehicleId);
              int boxOrganizationId = box.GetBoxOrganization(vehicAssign.boxId);
              if (boxOrganizationId != vehicleOrganizationId)
                  throw new DASAppViolatedIntegrityConstraintsException("Box and vehicle do no assigned to same organization.");

              // 5. Add vehicle assignment
              vehicleAssignment.AddVehicleAssignment(vehicAssign);

              // 6. Prepares SQL statement to VehicleAssignmentHst table 
              VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);

              // 7. Add vehicle assignment into the history
              vehicleAssignmentHst.AddVehicleAssignment(vehicAssign);

              // 8. Add vehicle to default fleet
              DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
              if (!fleetVehicles.IsVehicleExistInFleet(fleetId, vehicleId))
                  fleetVehicles.AddVehicleToFleet(fleetId, vehicleId);

              if (vehicleExist)
              {
                  vehicleInfo.UpdateVehicleInfo_NewTZ(vehicInfo, vehicAssign.vehicleId, vehicleOrganizationId);
              }

              //////////// 18-01-2007 new code - Max ///////////////////////////
              /*
              // 9. Add entry into the table vlfVehicleMaintenance
              // create a DasLogic.Box object
              Box boxLogic = new Box(this.ConnectionString);
              // get box settings - table vlfBoxSettings
              DataSet dsBoxSettings = boxLogic.GetBoxSettingsInfo(vehicAssign.boxId);
              boxLogic.Dispose();
              if (Util.IsDataSetValid(dsBoxSettings)) // contains data
              {
                 foreach (DataRow row in dsBoxSettings.Tables[0].Rows)
                 {
                    // if the box supports 8.0 protocol
                    if (Convert.ToInt32(row["BoxProtocolTypeId"]) == (int)CLS.Def.Enums.ProtocolTypes.HGIv80)
                    {
                       // insert record with vehicle Id and empty parameters
                       AddVehicleMaintenanceInfo(vehicAssign.vehicleId, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, "");
                       break;
                    }
                 }
              }
              */
              //////////// 18-01-2007 end of new code - Max ////////////////////

              // 10. Save all changes
              sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              // 9. Rollback all changes
              sqlExec.RollbackTransaction();
              Util.ProcessDbException("Unable to add new vehicle ", objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              // 9. Rollback all changes
              sqlExec.RollbackTransaction();
              throw new DASException(objException.Message);
          }
          return vehicleId;
      }
       // Changes for TimeZone Feature end

      /// <summary>
      /// Add new Vehicle.
      ///		- Add new vehicle information into vlfVehicleInfo table.
      ///		- Assign new vehicle with user id, box and license plate (vlfVehicleAssignment table).
      ///		- Backup information into vlfVehicleAssignmentHst table.
      /// </summary>
      /// <param name="vehicInfo"></param>
      /// <param name="vehicAssign"></param>
      /// <param name="organizationId"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if box and vehicle do no assigned to same organization.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public long AddVehicle(VehicInfo vehicInfo, VehicAssign vehicAssign, int organizationId)
      {
         long vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
         DB.Fleet fleet = new DB.Fleet(sqlExec);
         int fleetId = fleet.GetFleetIdByFleetName(organizationId, VLF.CLS.Def.Const.defaultFleetName);

         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Creates VehicleInfo database object
            //VehicleInfo vehicleInfo = new VehicleInfo(sqlExec);

            // 3. Add/Update vehicle info and set vehicle id to assignment structure
            bool vehicleExist = false;
            vehicleId = vehicleInfo.AddVehicleInfo(vehicInfo, organizationId, ref vehicleExist);
            vehicAssign.vehicleId = vehicleId;

            // 4. Check if box and vehicle assigned to same organization
            int vehicleOrganizationId = vehicleInfo.GetVehicleOrganization(vehicleId);
            int boxOrganizationId = box.GetBoxOrganization(vehicAssign.boxId);
            if (boxOrganizationId != vehicleOrganizationId)
               throw new DASAppViolatedIntegrityConstraintsException("Box and vehicle do no assigned to same organization.");

            // 5. Add vehicle assignment
            vehicleAssignment.AddVehicleAssignment(vehicAssign);

            // 6. Prepares SQL statement to VehicleAssignmentHst table 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);

            // 7. Add vehicle assignment into the history
            vehicleAssignmentHst.AddVehicleAssignment(vehicAssign);

            // 8. Add vehicle to default fleet
            DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
            if (!fleetVehicles.IsVehicleExistInFleet(fleetId, vehicleId))
               fleetVehicles.AddVehicleToFleet(fleetId, vehicleId);

            if (vehicleExist)
            {
               vehicleInfo.UpdateVehicleInfo(vehicInfo, vehicAssign.vehicleId, vehicleOrganizationId);
            }

            //////////// 18-01-2007 new code - Max ///////////////////////////
            /*
            // 9. Add entry into the table vlfVehicleMaintenance
            // create a DasLogic.Box object
            Box boxLogic = new Box(this.ConnectionString);
            // get box settings - table vlfBoxSettings
            DataSet dsBoxSettings = boxLogic.GetBoxSettingsInfo(vehicAssign.boxId);
            boxLogic.Dispose();
            if (Util.IsDataSetValid(dsBoxSettings)) // contains data
            {
               foreach (DataRow row in dsBoxSettings.Tables[0].Rows)
               {
                  // if the box supports 8.0 protocol
                  if (Convert.ToInt32(row["BoxProtocolTypeId"]) == (int)CLS.Def.Enums.ProtocolTypes.HGIv80)
                  {
                     // insert record with vehicle Id and empty parameters
                     AddVehicleMaintenanceInfo(vehicAssign.vehicleId, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, "");
                     break;
                  }
               }
            }
            */
            //////////// 18-01-2007 end of new code - Max ////////////////////

            // 10. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            // 9. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException("Unable to add new vehicle ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            // 9. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
         return vehicleId;
      }

      /// <summary>
      /// Updates vehicle additional information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="field1"></param>
      /// <param name="field2"></param>
      /// <param name="field3"></param>
      /// <param name="field4"></param>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if vehicle does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleAdditionalInfo(Int64 vehicleId, string field1, string field2, string field3, string field4, string field5, int? vehicleWeight, string vehicleWtUnit, float? fuelCapacity, float? fuelBurnRate)
      {
          vehicleInfo.UpdateVehicleAdditionalInfo(vehicleId, field1, field2, field3, field4, field5, vehicleWeight, vehicleWtUnit, fuelCapacity, fuelBurnRate);
      }

      /// <summary>
      /// Add new Vehicle.
      ///		- Assign new vehicle with box and license plate (vlfVehicleAssignment table).
      ///		- Backup information into vlfVehicleAssignmentHst table.
      /// </summary>
      /// <param name="vehicAssign"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with same datetime alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void AssignVehicle(VehicAssign vehicAssign)
      {
         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Add vehicle assignment
            vehicleAssignment.AddVehicleAssignment(vehicAssign);

            // 3. Prepares SQL statement to VehicleAssignmentHst table 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);

            // 4. Add vehicle assignment into the history
            vehicleAssignmentHst.AddVehicleAssignment(vehicAssign);

            // 5. Check if box and vehicle assigned to same organization
            int vehicleOrganizationId = vehicleInfo.GetVehicleOrganization(vehicAssign.vehicleId);
            int boxOrganizationId = box.GetBoxOrganization(vehicAssign.boxId);
            if (boxOrganizationId != vehicleOrganizationId)
               throw new DASAppViolatedIntegrityConstraintsException("Box and vehicle do not belong to the same organization.");

            // 6. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            // 6. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException("Uanable to aaaign new vehicle ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            // 6. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
      }

      /// <summary>
      /// Assign existing Vehicle to box and default fleet
      ///		- Assign new vehicle to box and license plate (vlfVehicleAssignment table).
      ///		- Assign new vehicle to a default fleet.
      ///		- Backup information into vlfVehicleAssignmentHst table.
      /// </summary>
      /// <param name="vehicAssign"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with license plate alredy exists</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle assignment with same datetime alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public int AssignVehicleToBoxAndFleet(VehicAssign vehicAssign)
      {
         int result = 0;
         try
         {
            // Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            int vehicleOrgId = vehicleInfo.GetVehicleOrganization(vehicAssign.vehicleId);

            // Check if box and vehicle assigned to same organization
            int boxOrgId = box.GetBoxOrganization(vehicAssign.boxId);
            if (boxOrgId != vehicleOrgId)
               throw new DASAppViolatedIntegrityConstraintsException("Box and vehicle do not belong to the same organization");

            Fleet fleet = new Fleet(this.ConnectionString);
            int defaultFleetId = fleet.GetFleetIdByFleetName(vehicleOrgId, Const.defaultFleetName);
            if (defaultFleetId < 1)
               throw new DASAppViolatedIntegrityConstraintsException("Default fleet does not exist!");

            // Add vehicle assignment
            vehicleAssignment.AddVehicleAssignment(vehicAssign);

            // Prepares SQL statement to VehicleAssignmentHst table 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);

            // Add vehicle assignment into the history
            vehicleAssignmentHst.AddVehicleAssignment(vehicAssign);

            // Add vehicle to default fleet
            result = fleet.AddVehicleToFleet(defaultFleetId, vehicAssign.vehicleId);

            if (result != 1)
               throw new DASDbException("Add Vehicle To Fleet Failed");

            // Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            sqlExec.RollbackTransaction();
            Util.ProcessDbException("Unable to assign new vehicle ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
         return result;
      }

      /// <summary>
      /// Delete vehicle from the system
      /// </summary>
      /// <remarks>
      /// 1. Unassign vehicle from all fleets
      /// 2. Unassign driver from the vehicle
      /// 3. Save old driver/license plate assignment to the history
      /// 4. Delete old vehicle assignment
      /// 5. Save old assignment into the history 
      /// </remarks>
      /// <param name="licensePlate"></param>
      /// <returns>Rows Affected</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public int DeleteActiveVehicleAssignmentByLicensePlate(string licensePlate)
      {
         int rowsAffected = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to disconnect vehicle from the box. ";
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Retrieves vehicle assignment by license plate
            VehicAssign vehicAssign = vehicleAssignment.GetVehicleAssignmentVA(licensePlate);

            // 3. Try to unassign vehicle from all fleets
            FleetVehicles fleetVehicles = new FleetVehicles(sqlExec);
            fleetVehicles.PurgeVehicleFromAllFleets(vehicAssign.vehicleId);

            // 4. Delete vehicle geozones
            VehicleGeozone vehicleGeozone = new VehicleGeozone(sqlExec);
            vehicleGeozone.DeleteAllGeozones(vehicAssign.vehicleId);

            // 5. Unassign driver from the vehicle
            // 5.1 Retrieves driver assignment by license plate
            DriverAssignment driverAssignment = new DriverAssignment(sqlExec);
            DataSet dsDriverAssignment = driverAssignment.GetDriverAssignment(licensePlate);
            if (Util.IsDataSetValid(dsDriverAssignment))
            {
               // 5.2 Unassign driver from license plate
               driverAssignment.DeleteDriverAssignment(licensePlate);

               // 5.3 Save old driver/license plate assignment to the history
               // coming from GetDriverAssignment -> 
               //            [VehicleId]
               //            [DriverId],
               //            [LicensePlate],
               //            [AssigneDateTime],
               //            [Description],
               //            [PersonId]
               DriverAssignmentHst driverAssignmentHst = new DriverAssignmentHst(sqlExec);
               driverAssignmentHst.AddDriverAssignmentHistory(
                        Convert.ToInt32(dsDriverAssignment.Tables[0].Rows[0]["DriverId"].ToString()),
                        Convert.ToInt32(dsDriverAssignment.Tables[0].Rows[0]["VehicleId"].ToString()),
                        0,
                        dsDriverAssignment.Tables[0].Rows[0]["Description"].ToString(),
                        Convert.ToDateTime(dsDriverAssignment.Tables[0].Rows[0]["AssigneDateTime"].ToString()),
                        DateTime.Now);

            }

            // 6. Delete old vehicle assignment info
            rowsAffected = vehicleAssignment.DeleteVehicleAssignment(licensePlate);

            // 7. Save old assignment into the history 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);
            vehicleAssignmentHst.UpdateVehicleAssignment(vehicAssign.boxId);

            // 8. Delete vehicle maintenance information - 25-01-2007 - Max
            /*
            VehicleMaintenaceInfo vehicleMaintenaceInfo = new VehicleMaintenaceInfo(sqlExec);
            if (vehicleMaintenaceInfo.IsVehicleMaintenanceInfoExist(vehicAssign.vehicleId))
            {
               // delete vehicle maintenance history
               vehicleMaintenaceInfo.DeleteVehicleMaintenanceHst(vehicAssign.vehicleId);
               // delete vehicle maintenance entry
               vehicleMaintenaceInfo.DeleteVehicleMaintenanceInfo(vehicAssign.vehicleId);
               // adding vehicle maintenance history info - constraint in a DB 'FK_vlfVehicleMaintenanceHst_vlfVehicleMaintenance'
               //vehicleMaintenaceInfo.AddVehicleMaintenanceHst(
               //   vehicAssign.vehicleId, DateTime.UtcNow, "Vehicle was disconnected from the box: " + vehicAssign.boxId.ToString(), 0);
            }
            */
            ///////////////// end of new code 25-01-2007 /////////////////////

            // 9. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            rowsAffected = 0;
            // 8. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            // 8. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Delete vehicle from the system
      /// </summary>
      /// <remarks>
      /// 1. Unassign vehicle from all fleets
      /// 2. Unassign driver from the vehicle
      /// 3. Save old driver/license plate assignment to the history
      /// 4. Delete old vehicle assignment
      ///	5. Save old assignment into the history 
      /// </remarks>
      /// <param name="userId"></param>
      /// <param name="boxId"></param>
      /// <returns>Rows Affected</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public int DeleteActiveVehicleAssignment(int userId, int boxId, string description)
      {
         int rowsAffected = 0;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to disconnect vehicle from the box. ";
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Retrieves vehicle assignment by license plate
            DataSet dsVehAssign = vehicleAssignment.GetVehicleAssignmentBy("BoxId", boxId);
            if (!Util.IsDataSetValid(dsVehAssign))
               throw new ArgumentNullException("Vehicle assignment is NULL");

            long vehicleId = Convert.ToInt64(dsVehAssign.Tables[0].Rows[0]["VehicleId"]);

            // 3. Try to unassign vehicle from all fleets
            FleetVehicles fleetVehicles = new FleetVehicles(sqlExec);
            fleetVehicles.PurgeVehicleFromAllFleets(vehicleId);

            // 4. Delete vehicle geozones
            VehicleGeozone vehicleGeozone = new VehicleGeozone(sqlExec);
            vehicleGeozone.DeleteAllGeozones(vehicleId);           


            // 5. Unassign driver from the vehicle
           // DriverAssignment driverAssignment = new VLF.DAS.DB.DriverAssignment(sqlExec);
           // driverAssignment.DeleteDriverAssignment(userId, vehicleId, driverId, description);

            using (DriverManager driver = new DriverManager(this.ConnectionString))
            {
               DataSet dsDiver = driver.GetDriverActiveAssignment(vehicleId);
               if (Util.IsDataSetValid(dsDiver))
               {
                  int driverId = Convert.ToInt32(dsDiver.Tables[0].Rows[0]["DriverId"]);
                  rowsAffected += driver.DeleteActiveDriverAssignment(userId, vehicleId, driverId, description);
               }
            }
             
            // 6. Delete old vehicle assignment info
            rowsAffected += vehicleAssignment.DeleteVehicleAssignment(vehicleId);

            // 7. Save old assignment into the history 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);
            vehicleAssignmentHst.UpdateVehicleAssignment(boxId);

            // 8. Delete vehicle maintenance information - 25-01-2007 - Max
            //VehicleMaintenaceInfo vehicleMaintenaceInfo = new VehicleMaintenaceInfo(sqlExec);
            //if (vehicleMaintenaceInfo.IsVehicleMaintenanceInfoExist(vehicleId))
            //{
            //   // delete vehicle maintenance history
            //   vehicleMaintenaceInfo.DeleteVehicleMaintenanceHst(vehicleId);
            //   // delete vehicle maintenance entry
            //   vehicleMaintenaceInfo.DeleteVehicleMaintenanceInfo(vehicleId);
            //   // adding vehicle maintenance history info - constraint in a DB 'FK_vlfVehicleMaintenanceHst_vlfVehicleMaintenance'
            //   //vehicleMaintenaceInfo.AddVehicleMaintenanceHst(
            //   //   vehicAssign.vehicleId, DateTime.UtcNow, "Vehicle was disconnected from the box: " + vehicAssign.boxId.ToString(), 0);
            //}
            ///////////////// end of new code 25-01-2007 /////////////////////

            // 9. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            rowsAffected = 0;
            // 8. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            // 8. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// 	- Delete assign vehicle by license plate (vlfVehicleAssignment table).
      ///		- Backup information into vlfVehicleAssignmentHst table.
      ///	</summary>
      /// <remarks>Note: This method should call only on full system cleanup.</remarks>
      /// <returns>Rows Affected</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public int DeleteAllActiveVehiclesAssignments()
      {
         int rowsAffected = 0;
         try
         {
            // 1. Retrieves license plates list of all active assignments
            ArrayList licensePlatesList = vehicleAssignment.GetAllLicencePlates();

            // 2. Creates VehicleAssignmentHst database object
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);

            // 3. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 4. Delete vehicle assignment info and save it into the history
            foreach (string ittr in licensePlatesList)
            {
               // 5. Retrieves vehicle assignment by license plate
               VehicAssign vehicAssign = vehicleAssignment.GetVehicleAssignmentVA(ittr);

               // 6. Try to unassign vehicle from all fleets
               FleetVehicles fleetVehicles = new FleetVehicles(sqlExec);
               fleetVehicles.PurgeVehicleFromAllFleets(vehicAssign.vehicleId);

               // 7. Delete old vehicle assignment info
               rowsAffected += vehicleAssignment.DeleteVehicleAssignment(ittr);

               // 8. Save old assignment into the history 
               vehicleAssignmentHst.UpdateVehicleAssignment(vehicAssign.boxId);

               // 9. Wait number of timeslices before next insert into the history.
               // Reason: DateTime is primary key in the history. 
               //		  SQL has problems with Datetime.Millisec insert.
               Thread.Sleep(VLF.CLS.Def.Const.nextDateTimeMillisecInterval * 100);
            }

            // 10. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            rowsAffected = 0;
            string prefixMsg = "Unable to delete all vehicles.";
            // 10. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            string prefixMsg = "Unable to delete all vehicles.";
            // 10. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      ///         Retrieves all assigned vehicles license plates.
      /// </summary>
      /// <returns>ArrayList</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public ArrayList GetAllActiveVehiclesLicencePlates()
      {

         // 1. Retrieves all assigned vehicles license plates
         return vehicleAssignment.GetAllLicencePlates();

      }

      /// <summary>
      /// Get all Vehicles active assignment information by license plate as dataset
      /// </summary>
      /// <remarks>
      /// TableName	= "AllActiveVehiclesAssignments"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in exception cases.</exception>
      /// <returns>DataSet [LicensePlate][BoxId][VehicleId]</returns>
      public DataSet GetAllVehiclesActiveAssignments(int organizationId)
      {
         // 1. Retrieves all vehicle assignments
         DataSet dsResult = vehicleAssignment.GetAllVehiclesActiveAssignments(organizationId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllActiveVehiclesAssignments";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      /// <summary>
      /// Get all Vehicles active assignment information by license plate as dataset
      /// </summary>
      /// <remarks>
      /// TableName	= "AllVehiclesHistoryAssignments"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in exception cases.</exception>
      /// <returns>DataSet</returns>
      public DataSet GetAllVehiclesHstAssignments()
      {
         // 1. Creates VehicleAssignmentHst database object
         VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);
         // 2. Retrieves all vehicle assignments
         DataSet dsResult = vehicleAssignmentHst.GetAllRecords();
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllVehiclesHistoryAssignments";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Returns all unassigned vehicles information. 
      /// </summary>
      /// <returns>DataSet [VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllUnassignedVehiclesInfo(int organizationId)
      {
         DataSet dsResult = vehicleInfo.GetAllUnassignedVehiclesInfo(organizationId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllUnassignedVehicles";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Get vehicle active assignment information by license plate
      /// In case of empty result returns null
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleActiveAssignment" -- [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="licensePlate"></param>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <returns>DataSet</returns>
      public DataSet GetVehicleActiveAssignmentByLicensePlate(string licensePlate)
      {

         // 1. Retrieves all vehicle assignments
         DataSet dsResult = vehicleAssignment.GetVehicleAssignmentBy("LicensePlate", licensePlate);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleActiveAssignment";
            }
            dsResult.DataSetName = "Vehicle";
         }

         return dsResult;

      }
      /// <summary>
      /// Get vehicle active assignment information by vehicleId
      /// In case of empty result returns null
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleActiveAssignment"  -- [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="vehicleId"></param>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <returns>DataSet</returns>
      public DataSet GetVehicleActiveAssignmentByVehicleId(Int64 vehicleId)
      {

         // 1. Retrieves all vehicle assignments
         DataSet dsResult = vehicleAssignment.GetVehicleAssignmentBy("VehicleId", vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleActiveAssignment";
            }
            dsResult.DataSetName = "Vehicle";
         }

         return dsResult;

      }
      /// <summary>
      /// Get vehicle active assignment information by box id as dataset
      /// In case of empty result returns null
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleActiveAssignment" -- [LicensePlate],[BoxId],[VehicleId],[AssignedDateTime]
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="boxId"></param>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <returns>DataSet</returns>
      public DataSet GetVehicleActiveAssignmentByBoxId(int boxId)
      {
         // 1. Retrieves vehicle assignment
         DataSet dsResult = vehicleAssignment.GetVehicleAssignmentBy("BoxId", Convert.ToInt64(boxId));
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleActiveAssignment";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;

      }
      /// <summary>
      /// Retrieves box id by license plate
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>int</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public int GetBoxIdByLicensePlate(string licensePlate)
      {
         // 1. Retrieves vehicle assignment
         return Convert.ToInt32(vehicleAssignment.GetVehicleAssignmentField("BoxId", "LicensePlate", licensePlate));

      }
      /// <summary>
      /// Retrieves box id by vehicleId
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>int</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public int GetBoxIDByVehicleId(Int64 vehicleId)
      {

         // 1. Retrieves vehicle assignment
         return Convert.ToInt32(vehicleAssignment.GetVehicleAssignmentField("BoxId", "VehicleId", vehicleId));

      }

      /// <summary>
      ///      Retrieves box id by vehicle description
      /// </summary>
      /// <param name="vehicleDescription">vehicle description</param>
      /// <param name="orgid">Organization id</param>
      /// <returns></returns>
      public int GetBoxIdByVehicleDescription(string vehicleDescription, int orgid)
      {

         int boxId = VLF.CLS.Def.Const.unassignedIntValue;
         long vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
         try
         {
            vehicleId = vehicleInfo.GetVehicleIdByDescription(vehicleDescription, orgid);
            if (vehicleId != CLS.Def.Const.unassignedIntValue) boxId = GetBoxIDByVehicleId(vehicleId);
            return boxId;
         }
         catch
         {
            return VLF.CLS.Def.Const.unassignedIntValue;
         }

      }

      /// <summary>
      /// Retrieves box channels response time by vehicle description
      /// </summary>
      /// <param name="vehicleDescription">vehicle description</param>
      /// <param name="orgid">Organization id</param>
      /// <returns></returns>
      public DataSet GetBoxChannelsByVehicleDescription(string vehicleDescription, int orgid)
      {
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgid);
         sqlExec.AddCommandParam("@vdescr", SqlDbType.VarChar, vehicleDescription);
         return sqlExec.SPExecuteDataset("VehicleGetBoxChannels");
      }

      /// <summary>
      /// Retrieves vehicle id by license plate
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>Int64</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public Int64 GetVehicleIdByLicensePlate(string licensePlate)
      {

         // 1. Retrieves vehicle assignment
         return Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "LicensePlate", licensePlate));

      }
      /// <summary>
      /// Retrieves vehicle id by boxId
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>Int64</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public Int64 GetVehicleIdByBoxId(int boxId)
      {

         // 1. Retrieves vehicle assignment
         return Convert.ToInt64(vehicleAssignment.GetVehicleAssignmentField("VehicleId", "BoxId", boxId));


      }

      /// <summary>
      /// Retrieves license plate by box id 
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>string</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public string GetLicensePlateByBox(int boxId)
      {

         string licensePlate = "";
         // 1. Retrieves vehicle assignment
         licensePlate = Convert.ToString(vehicleAssignment.GetVehicleAssignmentField("LicensePlate", "BoxId", boxId));
         if ((licensePlate != null) && (licensePlate != ""))
            return licensePlate.Trim();
         return licensePlate;

      }
      /// <summary>
      /// Retrieves license plate by vehicle Id
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>string</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public string GetLicensePlateByVehicleId(Int64 vehicleId)
      {

         string licensePlate = "";
         // 1. Retrieves vehicle assignment
         licensePlate = Convert.ToString(vehicleAssignment.GetVehicleAssignmentField("LicensePlate", "VehicleId", vehicleId));
         if ((licensePlate != null) && (licensePlate != ""))
            return licensePlate.Trim();
         return licensePlate;

      }

      /// <summary>
      /// Check active vehicle assignment by box id
      /// If exists return true, otherwise return false
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>bool</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public bool IsActiveByBoxId(int boxId)
      {

         // 1. Retrieves vehicle assignments status
         return vehicleAssignment.IsActiveAssignmentBy("BoxId", boxId);

      }

      /// <summary>
      /// Check active vehicle assignment by license plate
      /// If exists return true, otherwise return false
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>bool</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public bool IsActiveByLicensePlate(string licensePlate)
      {

         // 1. Retrieves vehicle assignments status
         return vehicleAssignment.IsActiveAssignmentBy("LicensePlate", licensePlate);

      }
      #endregion

      #region Vehicle Geozone Interfaces
      /// <summary>
      /// Add new geozone to the vehicle.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="geozoneId"></param>
      /// <param name="severityId"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void AddGeozone(Int64 vehicleId, short geozoneId, short severityId,int speed)
      {
         vehicleGeozone.AddGeozone(vehicleId, geozoneId, severityId,speed );
      }
      /// <summary>
      /// Set geozone severity to the vehicle.
      /// </summary>
      /// <param name="organizationId"></param>
      /// <param name="vehicleId"></param>
      /// <param name="geozoneId"></param>
      /// <param name="severityId"></param>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public void SetGeozoneSeverity(int organizationId, Int64 vehicleId, short geozoneId, short severityId)
      {
         vehicleGeozone.SetGeozoneSeverity(organizationId, vehicleId, geozoneId, severityId);
      }
      /// <summary>
      /// Deletes all geozones related to specific vehicle.
      /// </summary>
      /// <returns>Rows affected</returns>
      /// <param name="vehicleId"></param>
      /// <exception cref="DASAppResultNotFoundException">Thrown if vehicle does not exist</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public int DeleteAllGeozones(Int64 vehicleId)
      {
         return vehicleGeozone.DeleteAllGeozones(vehicleId);
      }
      /// <summary>
      /// Deletes geozone from vehicle.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="geozoneId"></param>
      /// <returns></returns>
      public int DeleteGeozoneFromVehicle(Int64 vehicleId, short geozoneId)
      {
         return vehicleGeozone.DeleteGeozoneFromVehicle(vehicleId, geozoneId);
      }
      /// <summary>
      /// Retrieves all assigned to vehicle geozones info
      /// </summary>
      /// <returns>
      /// DataSet [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId],
      /// [GeozoneName],[Type],[GeozoneType],[SeverityId],[Description],[BoxId]
      /// </returns>
      /// <param name="vehicleId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllAssignedToVehicleGeozonesInfo(Int64 vehicleId)
      {
         DataSet dsResult = vehicleGeozone.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllAssignedToVehicleGeozones";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }


      /// <summary>
      /// Retrieves all assigned geozones to vehicle
      /// </summary>
      /// <returns>
      /// DataSet [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId],
      /// [GeozoneName],[Type],[GeozoneType],[SeverityId],[Description],[BoxId]
      /// </returns>
      /// <param name="vehicleId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
       public DataSet GetAllAssignedGeozonesToVehicle(int organizationId, short geozoneId,int userId)
       {
           DataSet dsResult = vehicleGeozone.GetAllAssignedGeozonesToVehicle(organizationId, geozoneId, userId);
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "AllAssignedToVehicleGeozones";
              }
              dsResult.DataSetName = "Vehicle";
          }
          return dsResult;
      }

      /// <summary>
      /// Retrieves vehicle max geozones
      /// </summary>
      /// <returns>int</returns>
      /// <param name="vehicleId"></param> 
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int GetMaxGeozonesByVehicleId(Int64 vehicleId)
      {
         return vehicleGeozone.GetMaxGeozonesByVehicleId(vehicleId);
      }
      /// <summary>
      /// Retrieves all unassigned to vehicle geozones info
      /// </summary>
      /// <returns>
      /// DataSet [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],
      /// [GeozoneType],[SeverityId],[Description]
      /// </returns>
      /// <param name="vehicleId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllUnassignedToVehicleGeozonesInfo(Int64 vehicleId)
      {
         DataSet dsResult = vehicleGeozone.GetAllUnassignedToVehicleGeozonesInfo(vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllUnassignedToVehicleGeozones";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Retrieves vehicle geozones information
      /// </summary>
      /// <returns>
      /// DataSet [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId],
      /// [GeozoneName],[Latitude],[Longitude],[Type],[SeverityId],
      /// [Description]
      /// </returns>
      /// <param name="vehicleId"></param> 
      /// <param name="geozoneId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetGeozoneInformation(Int64 vehicleId, short geozoneId)
      {
         DataSet dsResult = vehicleGeozone.GetGeozoneInformation(vehicleId, geozoneId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GeozoneInformation";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Retrieves all assigned vehicles info to geozone
      /// </summary>
      /// <returns>
      /// DataSet [VehicleId],[Description]
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <param name="geozoneId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllAssignedVehiclesInfoToGeozone(int organizationId, short geozoneId)
      {
         DataSet dsResult = vehicleGeozone.GetAllAssignedVehiclesInfoToGeozone(organizationId, geozoneId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllAssignedVehiclesInfoToGeozone";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      /// <summary>
      /// Retrieves all unassigned vehicles info to geozone
      /// </summary>
      /// <returns>
      /// DataSet [VehicleId],[Description]
      /// </returns>
      /// <param name="organizationId"></param> 
      /// <param name="geozoneId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllUnasignedVehiclesInfoToGeozone(int organizationId, short geozoneId)
      {
         DataSet dsResult = vehicleGeozone.GetAllUnasignedVehiclesInfoToGeozone(organizationId, geozoneId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllUnassignedVehiclesInfoToGeozone";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }
      /// <summary>
      /// Retrieves geozone info by organization id and geozone Id 
      /// </summary>
      /// <returns>
      /// DataSet [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],
      /// [GeozoneType],[SeverityId],[Description]
      /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical]</returns>
      /// <param name="boxId"></param> 
      /// <param name="geozoneId"></param> 
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetGeozoneInfo(int boxId, short geozoneId)
      {
         DataSet dsResult = vehicleGeozone.GetGeozoneInfo(boxId, geozoneId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "BoxGeozoneInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      #endregion

      #region Vehicles Info Interfaces
      /// <summary>
      /// Delete vehicle from the system
      /// </summary>
      /// <remarks>
      /// 1. Unassign vehicle from all fleets
      /// 2. Unassign driver from the vehicle
      /// 3. Save old driver/license plate assignment to the history
      /// 4. Delete old vehicle assignment
      ///	5. Save old assignment into the history 
      /// </remarks>
      /// <param name="vehicleId"></param>
      /// <comment> changed to NO DELETE sensors/outputs/settings for a boxID - 2006/08/18</comment>
      /// <returns>Rows Affected</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public int DeleteVehicle(Int64 vehicleId)
      {
         int rowsAffected = 0;
         int commandTimeout = sqlExec.CommandTimeout;
         string prefixMsg = "", fmt = "yyyy-MM-dd h:mm:ss tt";
         try
         {
            prefixMsg = "Unable to delete vehicle. ";
            
            sqlExec.CommandTimeout = 600;
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 3. Unassign vehicle from all fleets
            FleetVehicles fleetVehicles = new FleetVehicles(sqlExec);
            rowsAffected += fleetVehicles.PurgeVehicleFromAllFleets(vehicleId);

            // 4. Delete vehicle geozones
            //VehicleGeozone vehicleGeozone = new VehicleGeozone(sqlExec);
            rowsAffected += vehicleGeozone.DeleteAllGeozones(vehicleId);


            // 4.1 Delete Extra Service History
            //VehicleGeozone vehicleGeozone = new VehicleGeozone(sqlExec);
            rowsAffected += vehicleInfo.DeleteVehicleExtraServiceHistory(vehicleId); 

            // 5. Unassign driver from the vehicle
            // 5.1 Retrieves driver assignment by license plate
            DriverAssignment driverAssignment = new DriverAssignment(sqlExec);
            //DataSet dsDriverAssignment = driverAssignment.GetDriverAssignment(licensePlate);
            //if (Util.IsDataSetValid(dsDriverAssignment))
            //{
               // 5.2 Unassign driver from license plate
            rowsAffected += driverAssignment.DeleteRowsByIntField("VehicleId", vehicleId, "Vehicle Id");

               // 5.3 Save old driver/license plate assignment to the history
               //DriverAssignmentHst driverAssignmentHst = new DriverAssignmentHst(sqlExec);
               //driverAssignmentHst.UpdateRow(dsDriverAssignment.Tables[0].Rows[0]["PersonId"].ToString().Trim(), licensePlate);
            //}

            // 6. Delete old vehicle assignment info
            //string licensePlate = GetLicensePlateByVehicleId(vehicleId);
            //VehicAssign vehicAssign = vehicleAssignment.GetVehicleAssignmentBy_VA("VehicleId", vehicleId);
            rowsAffected += vehicleAssignment.DeleteVehicleAssignment(vehicleId);

            // 7. Save old assignment into the history 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);
            //[AssignedDateTime],[LicensePlate],[BoxId],[VehicleId],[DeletedDateTime]
            DataSet dsBoxHst = vehicleAssignmentHst.GetAllVehiclesAssignmentsBy("VehicleId", vehicleId, " ORDER BY AssignedDateTime");
            if (Util.IsDataSetValid(dsBoxHst))
            {
               //DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
               //DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
               DB.Alarm alarm = new DB.Alarm(sqlExec);
               DB.MsgIn msgIn = new DB.MsgIn(sqlExec);
               DB.MsgOut msgOut = new DB.MsgOut(sqlExec);
               DB.TxtMsgs txtMsg = new DB.TxtMsgs(sqlExec);
               DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
               //DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
               DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
               //DB.Box box = new DB.Box(sqlExec);
               foreach (DataRow ittr in dsBoxHst.Tables[0].Rows)
               {
                  int boxId = Convert.ToInt32(ittr["BoxId"]);
                  DateTime assignedDateTime = Convert.ToDateTime(ittr["AssignedDateTime"]);
                  DateTime deletedDateTime = DateTime.Now;
                  if (ittr["DeletedDateTime"] != System.DBNull.Value)
                     deletedDateTime = Convert.ToDateTime(ittr["DeletedDateTime"]);

                  // 10. Delete all alarms related to the box
                  string where = " AND DateTimeCreated BETWEEN '" + assignedDateTime.ToString(fmt) + "' AND '" + deletedDateTime.ToString(fmt) + "'";
                  alarm.DeleteBoxAllAlarms(boxId, where);

                  // 11. Delete box message in history
                  where = " AND DateTimeReceived BETWEEN '" + assignedDateTime.ToString(fmt) + "' AND '" + deletedDateTime.ToString(fmt) + "'";
                  msgIn.DeleteBoxAllMsgs(boxId, "vlfMsgIn", "");
                  //msgIn.DeleteBoxAllMsgs(boxId, "vlfMsgInHst", where); has been removed by MV
                  msgIn.DeleteBoxAllMsgs(boxId, "vlfMsgInHstIgnored", where);

                  where = " AND DateTime BETWEEN '" + assignedDateTime.ToString(fmt) + "' AND '" + deletedDateTime.ToString(fmt) + "'";
                  msgOut.DeleteBoxAllMsgs(boxId, "vlfMsgOut", "");
                  msgOut.DeleteBoxAllMsgs(boxId, "vlfMsgOutHst", where);

                  // 12. Delete box text messages 
                  where = " AND MsgDateTime BETWEEN '" + assignedDateTime.ToString(fmt) + "' AND '" + deletedDateTime.ToString(fmt) + "'";
                  txtMsg.DeleteBoxAllMsgs(boxId, where);

                  // 13. Delete box messages
                  boxMsgSeverity.DeleteRecordByBoxId(boxId);

                  // 15. Delete box map usage
                  if (assignedDateTime.Year == deletedDateTime.Year && assignedDateTime.Month == deletedDateTime.Month)
                     where = " AND UsageYear = " + assignedDateTime.Year + " AND UsageMonth = " + assignedDateTime.Month;
                  else
                     where = " AND UsageYear >= " + assignedDateTime.Year + " AND UsageMonth >= " + assignedDateTime.Month + " AND UsageYear <= " + deletedDateTime.Year + " AND UsageMonth <= " + deletedDateTime.Month;
                  
                  rowsAffected += mapEngine.DeleteBoxMapUsage(boxId, where);
               }
               rowsAffected += vehicleAssignmentHst.DeleteAllVehicleAssignmentsForVehicle(vehicleId);
            }

            // 16, Delete vehicle maintenance information
            /*
            if (vehicleMaintenance.IsVehicleMaintenanceInfoExist(vehicleId))
            {
               rowsAffected += vehicleMaintenance.DeleteVehicleMaintenanceHst(vehicleId);
               rowsAffected += vehicleMaintenance.DeleteVehicleMaintenanceInfo(vehicleId);
            }
            */
            rowsAffected += vehicleServices.DeleteRowsByField("VehicleId", vehicleId);

            // 17. Delete Vehicle Working Hours - obsolete - table is not in use?
            rowsAffected += vehicleInfo.DeleteVehicleWorkingHours(vehicleId);

            // 18. Delete vehicle
            rowsAffected += vehicleInfo.DeleteVehicInfo(vehicleId);

            // 19. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            rowsAffected = 0;
            // 19. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            // 19. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         finally
         {
            sqlExec.CommandTimeout = commandTimeout;
         }
         return rowsAffected;
      }
      /// <summary>
      /// Add new vehicle info.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="weekdayStart"></param>
      /// <param name="weekdayEnd"></param>
      /// <param name="weekendStart"></param>
      /// <param name="weekendEnd"></param>
      /// <returns>new vehicle id</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if vehicle does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddVehicleWorkingHours(Int64 vehicleId, int weekdayStart, int weekdayEnd, int weekendStart, int weekendEnd)
      {
         return vehicleInfo.AddVehicleWorkingHours(vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);
      }
      /// <summary>
      /// Deletes vehicle working hours
      /// </summary>
      /// <param name="vehicleId"></param> 
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteVehicleWorkingHours(Int64 vehicleId)
      {
         return vehicleInfo.DeleteVehicleWorkingHours(vehicleId);
      }
      /// <summary>
      /// Updates vehicle working hours.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="weekdayStart"></param>
      /// <param name="weekdayEnd"></param>
      /// <param name="weekendStart"></param>
      /// <param name="weekendEnd"></param>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if vehicle does not exist.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleWorkingHours(Int64 vehicleId, int weekdayStart, int weekdayEnd, int weekendStart, int weekendEnd)
      {
         vehicleInfo.UpdateVehicleWorkingHours(vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);
      }
      /// <summary>
      /// Returns vehicle working hours. 
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [VehicleId],[WeekdayStart],[WeekdayEnd],[WeekendStart],[WeekendEnd]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleWorkingHours(Int64 vehicleId)
      {
         DataSet dsResult = vehicleInfo.GetVehicleWorkingHours(vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleWorkingHours";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

       // Changes for TimeZone Feature start
      /// <summary>
      /// Update Vehicle and vehicle assignment.
      /// </summary>
      /// <returns>void</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if vehicle assignment does not exist</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box and vehicle do no assigned to same organization.</exception>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <param name="vehicInfo"></param>
      /// <param name="vehicleId"></param>
      /// <param name="newLicensePlate"></param>
      /// <param name="newBoxId"></param>
      public void ChangeVehicleInfo_NewTZ(VehicInfo vehicInfo, Int64 vehicleId, string newLicensePlate, int newBoxId)
      {
          // Check if box and vehicle assigned to same organization
          int vehicleOrganizationId = vehicleInfo.GetVehicleOrganization(vehicleId);
          int boxOrganizationId = box.GetBoxOrganization(newBoxId);
          if (boxOrganizationId != vehicleOrganizationId)
              throw new DASAppViolatedIntegrityConstraintsException("Box and vehicle do not belong to the same organization.");

          //LicensePlate,BoxId,VehicleId,AssignedDateTime
          DataSet dsAssignInfo = GetVehicleActiveAssignmentByVehicleId(vehicleId);
          if (!Util.IsDataSetValid(dsAssignInfo))
          {
              throw new DASAppResultNotFoundException("Unable to find vehicle assignment information by vehicleId: " + vehicleId);
          }

          try
          {
              // 1. Begin transaction
              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

              // 2. Delete old vehicle assignment info
              vehicleAssignment.DeleteVehicleAssignment(vehicleId);

              // 3. Save old assignment into the history 
              VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);
              vehicleAssignmentHst.UpdateVehicleAssignment(Convert.ToInt32(dsAssignInfo.Tables[0].Rows[0]["BoxId"]));

              // 4. Add vehicle assignment into the history and to active vehicles
              VehicAssign vehicAssign;
              vehicAssign.boxId = newBoxId;
              vehicAssign.vehicleId = vehicleId;
              vehicAssign.licensePlate = newLicensePlate;
              vehicleAssignmentHst.AddVehicleAssignment(vehicAssign);
              vehicleAssignment.AddVehicleAssignment(vehicAssign);

              // 5. Update vehicle info
              vehicleInfo.UpdateVehicleInfo_NewTZ(vehicInfo, vehicleId, vehicleOrganizationId);

              // 6. Save all changes
              sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              // 6. Rollback all changes
              sqlExec.RollbackTransaction();
              Util.ProcessDbException("Unable to add new vehicle ", objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              // 6. Rollback all changes
              sqlExec.RollbackTransaction();
              throw new DASException(objException.Message);
          }
      }
       // Changes for TimeZone Feature end
      /// <summary>
      /// Update Vehicle and vehicle assignment.
      /// </summary>
      /// <returns>void</returns>
      /// <exception cref="DASAppResultNotFoundException">Thrown if vehicle assignment does not exist</exception>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box and vehicle do no assigned to same organization.</exception>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <param name="vehicInfo"></param>
      /// <param name="vehicleId"></param>
      /// <param name="newLicensePlate"></param>
      /// <param name="newBoxId"></param>
      public void ChangeVehicleInfo(VehicInfo vehicInfo, Int64 vehicleId, string newLicensePlate, int newBoxId)
      {
         // Check if box and vehicle assigned to same organization
         int vehicleOrganizationId = vehicleInfo.GetVehicleOrganization(vehicleId);
         int boxOrganizationId = box.GetBoxOrganization(newBoxId);
         if (boxOrganizationId != vehicleOrganizationId)
            throw new DASAppViolatedIntegrityConstraintsException("Box and vehicle do not belong to the same organization.");

         //LicensePlate,BoxId,VehicleId,AssignedDateTime
         DataSet dsAssignInfo = GetVehicleActiveAssignmentByVehicleId(vehicleId);
         if (!Util.IsDataSetValid(dsAssignInfo))
         {
            throw new DASAppResultNotFoundException("Unable to find vehicle assignment information by vehicleId: " + vehicleId);
         }

         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Delete old vehicle assignment info
            vehicleAssignment.DeleteVehicleAssignment(vehicleId);

            // 3. Save old assignment into the history 
            VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);
            vehicleAssignmentHst.UpdateVehicleAssignment(Convert.ToInt32(dsAssignInfo.Tables[0].Rows[0]["BoxId"]));

            // 4. Add vehicle assignment into the history and to active vehicles
            VehicAssign vehicAssign;
            vehicAssign.boxId = newBoxId;
            vehicAssign.vehicleId = vehicleId;
            vehicAssign.licensePlate = newLicensePlate;
            vehicleAssignmentHst.AddVehicleAssignment(vehicAssign);
            vehicleAssignment.AddVehicleAssignment(vehicAssign);

            // 5. Update vehicle info
            vehicleInfo.UpdateVehicleInfo(vehicInfo, vehicleId, vehicleOrganizationId);

            // 6. Save all changes
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            // 6. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException("Unable to add new vehicle ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            // 6. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
      }

       // Changes for TimeZone Feature start
      /// <summary>
      /// Update Vehicle.
      /// </summary>
      /// <param name="vehicInfo"></param>
      /// <param name="vehicleId"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public void UpdateVehicleInfo_NewTZ(VehicInfo vehicInfo, Int64 vehicleId)
      {
          vehicleInfo.UpdateVehicleInfo_NewTZ(vehicInfo, vehicleId, vehicleInfo.GetVehicleOrganization(vehicleId));
      }
      // Changes for TimeZone Feature end

      /// <summary>
      /// Update Vehicle.
      /// </summary>
      /// <param name="vehicInfo"></param>
      /// <param name="vehicleId"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public void UpdateVehicleInfo(VehicInfo vehicInfo, Int64 vehicleId)
      {
         vehicleInfo.UpdateVehicleInfo(vehicInfo, vehicleId, vehicleInfo.GetVehicleOrganization(vehicleId));
      }
      /// <summary>
      /// Get all Vehicles information as dataset
      /// </summary>
      /// <remarks>
      /// TableName	= "AllVehiclesInformation"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile]</returns>
      public DataSet GetAllVehiclesInfo()
      {
         DataSet dsResult = vehicleInfo.GetAllVehiclesInfo();
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "AllVehiclesInformation";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;

      }
      /// <summary>
      /// Returns vehicle additional information. 
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [VehicleId],[Field1],[Field2],[Field3],[Field4]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleAdditionalInfo(Int64 vehicleId)
      {

         DataSet dsResult = vehicleInfo.GetVehicleAdditionalInfo(vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleAdditionalInfo";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

       // Chnages for TimeZone Feature start
      /// <summary>
      /// Returns vehicle information by license plate. 
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleInformation"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="licensePlate"></param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
      /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
      /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehicleInfoByLicensePlate_NewTZ(string licensePlate)
      {
          DataSet dsResult = vehicleInfo.GetVehicleInfoByLicensePlate_NewTZ(licensePlate);
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "VehicleInformation";
              }
              dsResult.DataSetName = "Vehicle";
          }
          return dsResult;
      }// Changes for TimeZone Feature end

      /// <summary>
      /// Returns vehicle information by license plate. 
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleInformation"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="licensePlate"></param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
      /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
      /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehicleInfoByLicensePlate(string licensePlate)
      {
         DataSet dsResult = vehicleInfo.GetVehicleInfoByLicensePlate(licensePlate);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleInformation";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

       // Changes for TimeZone Feature start
      /// <summary>
      /// Returns vehicle information by vehicle id. 
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleInformation"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
      /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
      /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehicleInfoByVehicleId_NewTZ(Int64 vehicleId)
      {

          DataSet dsResult = vehicleInfo.GetVehicleInfoByVehicleId_NewTZ(vehicleId);
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "VehicleInformation";
              }
              dsResult.DataSetName = "Vehicle";
          }
          return dsResult;
      }

      // Changes for TimeZone Feature end

      /// <summary>
      /// Returns vehicle information by vehicle id. 
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleInformation"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
      /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
      /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehicleInfoByVehicleId(Int64 vehicleId)
      {

         DataSet dsResult = vehicleInfo.GetVehicleInfoByVehicleId(vehicleId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleInformation";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;
      }

      public short GetVehicleTypeIdByBoxId(int boxId)
      {
         return vehicleInfo.GetVehicleTypeIdByBoxId(boxId);
      }
      /// <summary>
      /// Returns vehicle information by box id. 
      /// </summary>
      /// <remarks>
      /// TableName	= "VehicleInformation"
      /// DataSetName = "Vehicle"
      /// </remarks>
      /// <param name="boxId"></param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],
      /// [MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],
      /// [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]
      /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],
      /// [VehicleTypeId]</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehicleInfoByBoxId(int boxId)
      {
         DataSet dsResult = vehicleInfo.GetVehicleInfoByBoxId(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehicleInformation";
            }
            dsResult.DataSetName = "Vehicle";
         }
         return dsResult;

      }
      /// <summary>
      /// Get state/province by license plate
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>object</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public string GetStateProvince(string licensePlate)
      {

         return vehicleInfo.GetVehicleInfoStrFieldByLicensePlate("StateProvince", licensePlate);

      }

      /// <summary>
      /// Set new State/Province.
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="stateProvince"></param>
      /// <returns>void</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public void SetStateProvince(string licensePlate, string stateProvince)
      {
         vehicleInfo.SetVehicleInfoStrFieldByLicensePlate("StateProvince", stateProvince, licensePlate);
      }

      /// <summary>
      /// Get cost per mile by license plate
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <returns>object</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public double GetCostPerMile(string licensePlate)
      {
         return Convert.ToDouble(vehicleInfo.GetVehicleInfoStrFieldByLicensePlate("CostPerMile", licensePlate));

      }
      /// <summary>
      /// Set new cost per mile.
      /// </summary>
      /// <param name="licensePlate"></param>
      /// <param name="costPerMile"></param>
      /// <returns>void</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public void SetCostPerMile(string licensePlate, double costPerMile)
      {
         vehicleInfo.SetVehicleInfoDoubleFieldByLicensePlate("CostPerMile", costPerMile, licensePlate);
      }
      /// <summary>
      /// Get box last street address
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>street address</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public string GetVehicleLastStreetAddress(int boxId)
      {
         DB.Box box = new DB.Box(sqlExec);
         return box.GetVehicleLastStreetAddress(boxId);
      }
      #endregion


      public int SaveVehicleOperationalState(int userId, int orgId, long vehicleId,
           int operationalState, string notes)
      {
          return vehicleInfo.SaveVehicleOperationalState(userId, orgId, vehicleId, operationalState, notes);
      }

      public DataSet GetVehicleOperationalState(int userId, int orgId, long vehicleId)
      {
          DataSet dsResult = vehicleInfo.GetVehicleOperationalState(userId, orgId, vehicleId);
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "VehicleInfo";
              }
              dsResult.DataSetName = "Vehicle";
          }
          return dsResult;
      }

      public DataSet ListVehiclesInLandmarksForDashboard(int userId, int orgId, long landmarkCategoryId)
      {
          DataSet dsResult = vehicleInfo.ListVehiclesInLandmarksForDashboard(userId, orgId, landmarkCategoryId);
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "VehicleList";
              }
              dsResult.DataSetName = "Vehicle";
          }
          return dsResult;
      }

      public DataSet GetServiceConfigurationsByLandmarkAndVehicle(int orgId, long vehicleId, long landmarkId)
      {
          DataSet dsResult = vehicleInfo.GetServiceConfigurationsByLandmarkAndVehicle(orgId, vehicleId, landmarkId);
          if (dsResult != null)
          {
              if (dsResult.Tables.Count > 0)
              {
                  dsResult.Tables[0].TableName = "ServiceConfigurationList";
              }
              dsResult.DataSetName = "ServiceConfigurations";
          }
          return dsResult;
      }

      /// <summary>
      /// Delete all organization vehicles
      /// </summary>
      /// <param name="orgId" type="int">Organization ID</param>
      /// <returns>Rows affected</returns>
      public int DeleteOrganizationVehicles(int orgId)
      {
         int rowsAffected = 0;
         vehicleInfo = new VehicleInfo(sqlExec);
         DataSet dsVehicles = vehicleInfo.GetOrganizationVehicles(orgId);
         if (Util.IsDataSetValid(dsVehicles))
         {
            try
            {
               foreach (DataRow drVehicle in dsVehicles.Tables[0].Rows)
               {
                  rowsAffected += DeleteVehicle(Convert.ToInt64(drVehicle["VehicleId"]));
               }
            }
            catch (Exception exc)
            {
               rowsAffected = 0;
               throw new Exception(String.Format("Cannot delete vehicles for Organization ID {0}\n", orgId), exc);
            }
         }
         return rowsAffected;
      }

      /// <summary>
      /// Update vehicle info from a datarow
      /// </summary>
      /// <param name="orgId">Organization Id</param>
      /// <param name="drVehicle">Vehicles data: 
      /// [Description, Vin, License, Vehicle_Type, Make, Model, Year, Color, Box Id, Field1, Field2, Field3, Field4]</param>
      /// <returns>Number of rows updated</returns>
      public int UpdateVehicle(int orgId, DataRow drVehicle)
      {
         int rowsAffected = 0;
         try
         {
            if (drVehicle == null || String.IsNullOrEmpty(drVehicle["BOX_ID"].ToString().Trim())) return 0;

            SqlParameter[] paramsVInfo = new SqlParameter[14];
            paramsVInfo[0] = new SqlParameter("@orgId", orgId);
            paramsVInfo[1] = new SqlParameter("@descr", drVehicle["Description"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[2] = new SqlParameter("@vin", drVehicle["Vin"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[3] = new SqlParameter("@license", drVehicle["LICENSE"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[4] = new SqlParameter("@vtype", drVehicle["VEHICLE_TYPE"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[5] = new SqlParameter("@make", drVehicle["MAKE"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[6] = new SqlParameter("@model", drVehicle["MODEL"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[7] = new SqlParameter("@year", drVehicle["Year"]);
            paramsVInfo[8] = new SqlParameter("@color", drVehicle["Color"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[9] = new SqlParameter("@boxid", drVehicle["BOX_ID"]);
            paramsVInfo[10] = new SqlParameter("@field1", drVehicle["Field1"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[11] = new SqlParameter("@field2", drVehicle["Field2"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[12] = new SqlParameter("@field3", drVehicle["Field3"].ToString().Trim().Replace('\'', '"'));
            paramsVInfo[13] = new SqlParameter("@field4", drVehicle["Field4"].ToString().Trim().Replace('\'', '"'));

            rowsAffected = sqlExec.SPExecuteNonQuery("VehicleUpdate", paramsVInfo);
         }
         catch (Exception exc)
         {
            rowsAffected = 0;
            throw new Exception(String.Format("Update vehicles for Organization ID: {0} failed.\n{1}", orgId, exc.Message));
         }
         return rowsAffected;
      }

      /// <summary>
      /// Update vehicle info
      /// </summary>
      /// <param name="orgId"></param>
      /// <param name="boxId"></param>
      /// <param name="vin"></param>
      /// <param name="plate"></param>
      /// <param name="description"></param>
      /// <param name="vehicleType"></param>
      /// <param name="year"></param>
      /// <param name="make"></param>
      /// <param name="model"></param>
      /// <param name="color"></param>
      /// <param name="field1"></param>
      /// <param name="field2"></param>
      /// <param name="field3"></param>
      /// <param name="field4"></param>
      /// <returns>Number of rows updated</returns>
      public int UpdateVehicle(int orgId, int boxId, string vin, string plate, string description, string vehicleType,
         short year, string make, string model, string color, string field1, string field2, string field3, string field4)
      {
         int rowsAffected = 0;
         try
         {
            SqlParameter[] paramsVInfo = new SqlParameter[14];
            paramsVInfo[0] = new SqlParameter("@orgId", orgId);
            paramsVInfo[1] = new SqlParameter("@descr", description.Trim().Replace('\'', '"'));
            paramsVInfo[2] = new SqlParameter("@vin", vin.Trim().Replace('\'', '"'));
            paramsVInfo[3] = new SqlParameter("@license", plate.Trim().Replace('\'', '"'));
            paramsVInfo[4] = new SqlParameter("@vtype", vehicleType.Trim().Replace('\'', '"'));
            paramsVInfo[5] = new SqlParameter("@make", make.Trim().Replace('\'', '"'));
            paramsVInfo[6] = new SqlParameter("@model", model.Trim().Replace('\'', '"'));
            paramsVInfo[7] = new SqlParameter("@year", year);
            paramsVInfo[8] = new SqlParameter("@color", color.Trim().Replace('\'', '"'));
            paramsVInfo[9] = new SqlParameter("@boxid", boxId);
            paramsVInfo[10] = new SqlParameter("@field1", field1.Trim().Replace('\'', '"'));
            paramsVInfo[11] = new SqlParameter("@field2", field2.Trim().Replace('\'', '"'));
            paramsVInfo[12] = new SqlParameter("@field3", field3.Trim().Replace('\'', '"'));
            paramsVInfo[13] = new SqlParameter("@field4", field4.Trim().Replace('\'', '"'));

            rowsAffected = sqlExec.SPExecuteNonQuery("VehicleUpdate", paramsVInfo);
         }
         catch (Exception exc)
         {
            rowsAffected = 0;
            throw new Exception(String.Format("Update vehicles for Organization ID: {0} failed.\n{1}", orgId, exc.Message));
         }
         return rowsAffected;
      }

      /// <summary>
      /// Get MakeModelId
      /// </summary>
      /// <param name="makeId">Make Id</param>
      /// <param name="modelId">Model Id</param>
      /// <returns>MakeModelId</returns>
      public int GetMakeModelId(int makeId, int modelId)
      {
         try
         {
            MakeModel mm = new MakeModel(this.sqlExec);
            return mm.GetMakeModelIdByMakeIdModelId(makeId, modelId);
         }
         catch
         {
            return -1;
         }
      }

      /// <summary>
      /// Get MakeModelId
      /// </summary>
      /// <param name="makeName">Make name</param>
      /// <param name="modelName">Model name</param>
      /// <returns>MakeModelId</returns>
      public int GetMakeModelId(string makeName, string modelName)
      {
         try
         {
            MakeModel mm = new MakeModel(this.sqlExec);
            return mm.GetMakeModelIdByMakeNameModelName(makeName, modelName);
         }
         catch
         {
            return -1;
         }
      }

      /// <summary>
      /// Get VehicleInfo By Description
      /// </summary>
      /// <param name="description">Vehicle Description</param>
      /// <param name="freeText">True if free text search, false if exact search</param>
      /// <returns>DataSet [VehicleId, Description, BoxId, LicensePlate, OrganizationId, OrganizationName, FleetId, FleetName]</returns>
      public DataSet GetVehicleInfoByDescription(string description, bool freeText)
      {
         DataSet dsResult = null;
         try
         {
            SqlParameter[] sqlParam = new SqlParameter[1];
            sqlParam[0] = new SqlParameter("@descr", description);
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT vlfVehicleInfo.VehicleId, RTRIM(vlfVehicleInfo.Description) AS Description,");
            sql.AppendLine("ISNULL(CONVERT(varchar, vlfVehicleAssignment.BoxId), 'N/A') AS BoxId,");
            sql.AppendLine("ISNULL(RTRIM(vlfVehicleAssignment.LicensePlate), 'N/A') AS LicensePlate,");
            sql.AppendLine("vlfVehicleInfo.OrganizationId, RTRIM(vlfOrganization.OrganizationName) AS OrganizationName,");
            sql.AppendLine("ISNULL(CONVERT(varchar, vlfFleet.FleetId), 'N/A') AS FleetId,");
            sql.AppendLine("ISNULL(RTRIM(vlfFleet.FleetName), 'N/A') AS FleetName");
            sql.AppendLine("FROM vlfVehicleInfo LEFT OUTER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId");
            sql.AppendLine("LEFT OUTER JOIN vlfFleetVehicles ON vlfVehicleInfo.VehicleId = vlfFleetVehicles.VehicleId");
            sql.AppendLine("LEFT OUTER JOIN vlfFleet ON vlfFleetVehicles.FleetId = vlfFleet.FleetId");
            sql.AppendLine("INNER JOIN vlfOrganization ON vlfVehicleInfo.OrganizationId = vlfOrganization.OrganizationId");
            sql.Append("WHERE vlfVehicleInfo.Description");
            if (freeText)
               sql.Append(" LIKE '%" + description + "%'");
            else
               sql.Append(" = @descr");
            dsResult = vehicleInfo.GetRowsBySql(sql.ToString(), sqlParam);
         }
         catch (Exception exc)
         {
            throw new Exception(String.Format("Get vehicle by Description: {0} failed.\n{1}", description, exc.Message));
         }
         return dsResult;
      }

       /// <summary>
       /// Get Vehicle Last Known Position Info By BoxId
       /// </summary>
       /// <param name="userId"></param>
       /// <param name="boxId"></param>
       /// <param name="language"></param>
       /// <returns></returns>
       public DataSet GetVehicleLastKnownPositionInfoByBoxId(int userId, int boxId, string language)
       {
           DataSet dsResult = vehicleInfo.GetVehicleLastKnownPositionInfoByBoxId(userId, boxId, language);
           if (dsResult != null)
           {
               if (dsResult.Tables.Count > 0)
               {
                   dsResult.Tables[0].TableName = "VehiclesLastKnownPositionInformation";
               }
               dsResult.DataSetName = "Fleet";
           }
           return dsResult;
       }

       /// <summary>
       /// Update Vehicle/Box BSM Maintenance information
       /// </summary>
       /// <param name="VehicleId"></param>
       /// <param name="Field1"></param>
       /// <param name="Field2"></param>
       /// <param name="Field3"></param>
       /// <param name="Field4"></param>
       /// <param name="Field5"></param>
       /// <returns></returns>
       public int VehicleExtraServiceHistory_Add_Update(Int64 VehicleId, string Field1, string Field2, string Field3, string Field4, string Field5, string Field6, string Field7,
            string Field8, string Field9, string Field10, string Field11, string Field12, string Field13, string Field14, string Field15, string Field16)
       {
           return vehicleInfo.VehicleExtraServiceHistory_Add_Update(VehicleId, Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8, Field9, Field10, Field11, Field12, Field13, Field14, Field15, Field16);           
       }

       /// <summary>
       /// Get Vehicle/Box BSM Extra information like: Status, Firmware
       /// </summary>
       /// <param name="vehicleId"></param>
       /// <returns></returns>
       public DataSet GetVehicleExtraServiceHistoryByVehicleId(Int64 vehicleId)
       {
           return vehicleInfo.GetVehicleExtraServiceHistoryByVehicleId(vehicleId);        
       }

       /// <summary>
       /// Get Vehicle/Box BSM Maintenance information
       /// </summary>
       /// <param name="vehicleId"></param>
       /// <returns></returns>
       public DataSet GetBoxExtraInfo(Int64 vehicleId)
       {
           return vehicleInfo.GetBoxExtraInfo(vehicleId);
       }

       /// <summary>
       /// Returns 3rd party vehicle additional information.
       /// </summary>
       /// <param name="vehicleId"></param>
       /// <returns>DataSet [VehicleId],[Equip#],[EquipCat],[Dates],[Readings]</returns>
       /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
       /// <exception cref="DASException">Thrown in all other exception cases.</exception>
       public DataSet Get3rdPartyVehicleAdditionalInfo(Int64 vehicleId)
       {
           DataSet dsResult = vehicleInfo.Get3rdPartyVehicleAdditionalInfo(vehicleId);//3rdParty
           if (dsResult != null)
           {
               if (dsResult.Tables.Count > 0)
               {
                   dsResult.Tables[0].TableName = "VehicleAdditionalInfo_3rdParty";
               }
               dsResult.DataSetName = "Vehicle_3rdParty";
           }
           return dsResult;
       }

       public void Update3rdPartyVehicleAdditionalInfo(Int64 vehicleId, string EquipNbr, string SAPEquipNbr, string LegacyEquipNbr, string ObjectType, string DOTNbr, string EquipCategory,
                         DateTime? AcquireDate, DateTime? RetireDate, DateTime? SoldDate, string ObjectPrefix, string OwningDistrict, string ProjectNbr,
                         decimal? TotalCtrReading_1, decimal? TotalCtrReading_2, string CtrReadingUom_1, string CtrReadingUom_2, string ShortDesc)
       {
           vehicleInfo.Update3rdPartyVehicleAdditionalInfo(vehicleId, EquipNbr, SAPEquipNbr, LegacyEquipNbr, ObjectType, DOTNbr, EquipCategory, AcquireDate, RetireDate, SoldDate,
                                                   ObjectPrefix, OwningDistrict, ProjectNbr,TotalCtrReading_1, TotalCtrReading_2, CtrReadingUom_1, CtrReadingUom_2, ShortDesc);
       }

       /// <summary>
       /// Swap Vehicle
       /// </summary>
       /// <param name="vehicleId"></param>       
       public int swapVehicle(Int64 vehicleId)
       {
           return vehicleInfo.swapVehicle(vehicleId);
       }

       /// <summary>
       /// Gets vehicle device statuses
       /// </summary>
       /// <param name="UserId"></param>
       /// <param name="VehicleId"></param>
       /// <returns>dataset</returns>
       public DataSet GetVehicleDeviceStatuses(int UserId, int VehicleId)
       {
           return vehicleInfo.GetVehicleDeviceStatuses(UserId, VehicleId); 
       }

       /// <summary>
       /// Updates vehicle device status
       /// </summary>
       /// <param name="VehicleDeviceStatusID"></param>
       /// <param name="StatusDate"></param>
       /// <param name="AuthorizationNo"></param>
       /// <param name="VehicleId"></param>
       /// <param name="UserId"></param>
       /// <returns>updated VehicleId</returns>
       public int UpdateVehicleDeviceStatus(int VehicleDeviceStatusID, string StatusDate, string AuthorizationNo, int VehicleId, int UserId, string Address, double Latitude, double Longitude)
       {
           return vehicleInfo.UpdateVehicleDeviceStatus(VehicleDeviceStatusID, StatusDate, AuthorizationNo, VehicleId, UserId, Address, Latitude, Longitude); 
       }
   }
}
