using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;			
using VLF.ERR;
using VLF.DAS.DB;
using VLF.CLS;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.Logic
{
   /// <summary>
   /// Vehicle Maintenance Interface - obsolete, not in use
   /// </summary>
   public partial class Vehicle
   {
      # region Obsolete methods

      /// <summary>
      /// Add new vehicle maintenanceinfo.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastSrvOdo"></param>
      /// <param name="currentOdo"></param>
      /// <param name="maxSrvInterval"></param>
      /// <param name="lastSrvEngHrs"></param>
      /// <param name="currentEngHrs"></param>
      /// <param name="engHrsSrvInterval"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddVehicleMaintenanceInfo(Int64 vehicleId,
          double lastSrvOdo, double currentOdo, double maxSrvInterval,
          int lastSrvEngHrs, int currentEngHrs, int engHrsSrvInterval,
          string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving,
          string nextServiceDescription)
      {
         vehicleMaintenance.AddVehicleMaintenanceInfo(vehicleId, lastSrvOdo, currentOdo, maxSrvInterval, lastSrvEngHrs, currentEngHrs, engHrsSrvInterval, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription);
      }

      /// <summary>
      /// Add new vehicle odometer maintenance history.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>       
      /// <param name="serviceDateTime"></param>
      /// <param name="serviceDescription"></param>
      /// <param name="serviceOdo"></param>
      /// <returns>true if succeeded, otherwise false</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddVehicleMaintenanceHst(Int64 vehicleId, 
                                           int userId,
                                           DateTime serviceDateTime, 
                                           string serviceDescription, 
                                           double serviceOdo)
      {
         vehicleMaintenance.AddVehicleMaintenanceHst(vehicleId, userId, serviceDateTime, serviceDescription, serviceOdo);
      }

      /// <summary>
      /// Add new vehicle engine maintenance history.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="serviceDateTime"></param>
      /// <param name="serviceDescription"></param>
      /// <param name="srvcEngine"></param>
      /// <returns>true if succeeded, otherwise false</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddVehicleEngineMaintenanceHst(Int64 vehicleId, int userId, DateTime serviceDateTime, string serviceDescription, int srvcEngine)
      {
         vehicleMaintenance.AddVehicleMaintenanceEngineHst(vehicleId, userId, serviceDateTime, serviceDescription, srvcEngine);
      }

      /// <summary>
      /// Returns vehicle maintenance information (excluding not existing).
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenanceInfo(Int64 vehicleId)
      {
         return vehicleMaintenance.GetVehicleMaintenanceInfo(vehicleId, true);
      }
      // Changes for TimeZone Feature start

      /// <summary>
      /// Returns vehicle maintenance information (excluding not existing).
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenanceInfo_NewTZ(Int64 vehicleId, int userId)
      {
          return vehicleMaintenance.GetVehicleMaintenanceInfo_NewTZ(vehicleId, true, userId);
      }

      // Changes for TimeZone Feature end


      /// <summary>
      /// Returns vehicle maintenance information (excluding not existing).
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenanceInfo(Int64 vehicleId,int userId )
      {
          return vehicleMaintenance.GetVehicleMaintenanceInfo(vehicleId, true, userId);
      }

      /// <summary>
      /// Returns vehicle maintenance information (including not existing).
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="include">True to include not initialized vehicle information</param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenance(Int64 vehicleId, bool include)
      {
         return vehicleMaintenance.GetVehicleMaintenanceInfo(vehicleId, include);
      }


      /// <summary>
      /// Returns vehicle maintenance information (including not existing).
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="include">True to include not initialized vehicle information</param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenance(Int64 vehicleId, bool include,int userId )
      {
          return vehicleMaintenance.GetVehicleMaintenanceInfo(vehicleId, include,userId);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="currentOdo"></param>
      public void UpdateBoxOdometer(int boxId, double currentOdo)
      {
         vehicleMaintenance.UpdateBoxOdometer(boxId, currentOdo);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="currentEngHrs"></param>
      /// <returns></returns>
      public int IncrementBoxCurrentEngHrsInfo(int boxId, int currentEngHrs)
      {
         return vehicleMaintenance.IncrementBoxCurrentEngHrsInfo(boxId, currentEngHrs);
      }

      /// <summary>
      /// Updates vehicle current Odo information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="currentOdo"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleCurrOdoInfo(Int64 vehicleId, double currentOdo)
      {
         vehicleMaintenance.UpdateVehicleCurrOdoInfo(vehicleId, currentOdo);
      }

      /// <summary>
      /// Updates vehicle current engine hours information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="currentEngHrs"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int UpdateVehicleCurrentEngHrsInfo(Int64 vehicleId, int currentEngHrs)
      {
         return vehicleMaintenance.UpdateVehicleCurrentEngHrsInfo(vehicleId, currentEngHrs);
      }

      /// <summary>
      /// Updates vehicle current engine hours + idling hours.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="currentEngHrs"></param>
      /// <param name="idlingHours"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int UpdateVehicleHoursInfo(Int64 vehicleId, int currentEngHrs, int idlingHours)
      {
         return vehicleMaintenance.UpdateVehicleHoursInfo(vehicleId, currentEngHrs, idlingHours);
      }

      /// <summary>
      /// Updates vehicle current Odo information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="serviceDateTime"></param>
      /// <param name="serviceDescription"></param>
      /// <param name="serviceOdo"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleMaintenanceHst(Int64 vehicleId, DateTime serviceDateTime, string serviceDescription, double serviceOdo)
      {
         vehicleMaintenance.UpdateVehicleMaintenanceHst(vehicleId, serviceDateTime, serviceDescription, serviceOdo);
      }

      /// <summary>
      /// Updates vehicle maintenance information + odometer
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastSrvOdo"></param>
      /// <param name="currentOdo"></param>
      /// <param name="maxSrvInterval"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleMaintenanceInfo(Int64 vehicleId,
          double lastSrvOdo, double currentOdo, double maxSrvInterval,
          string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving,
          string nextServiceDescription)
      {
         vehicleMaintenance.UpdateVehicleMaintenanceInfo(vehicleId, lastSrvOdo, currentOdo, maxSrvInterval, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription);
      }

      /// <summary>
      /// Updates vehicle next service description.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastSrvOdo"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleMaintenanceInfo(Int64 vehicleId, double lastSrvOdo, string nextServiceDescription)
      {
         vehicleMaintenance.UpdateVehicleMaintenanceInfo(vehicleId, lastSrvOdo, nextServiceDescription);
      }

      /// <summary>
      /// Updates vehicle maintenance information + engine hours
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastSrvOdo"></param>
      /// <param name="currentOdo"></param>
      /// <param name="maxSrvInterval"></param>
      /// <param name="lastSrvEngHrs"></param>
      /// <param name="currentEngHrs"></param>
      /// <param name="engHrsSrvInterval"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASDbException">Thrown if vehicle is not in the maint. table</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleEngineMaintenanceInfo(Int64 vehicleId,
         int lastSrvEngHrs, int currentEngHrs, int engHrsSrvInterval,
         string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving,
         string nextServiceDescription)
      {
         vehicleMaintenance.UpdateVehicleEngineMaintenanceInfo(vehicleId,
            lastSrvEngHrs, currentEngHrs, engHrsSrvInterval,
            email, timeZone, dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription);
      }

      /// <summary>
      /// Updates vehicle next service description.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastEngine"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleEngineMaintenanceInfo(Int64 vehicleId, int lastEngine, string nextServiceDescription)
      {
         vehicleMaintenance.UpdateVehicleEngineMaintenanceInfo(vehicleId, lastEngine, nextServiceDescription);
      }

      /// <summary>
      /// Returns vehicle maintenance information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="userId"></param>
      /// <returns>DataSet [VehicleId],[ServiceDateTime],[ServiceDescription],[ServiceOdo]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenanceHistory(Int64 vehicleId, int userId)
      {
         return vehicleMaintenance.GetVehicleMaintenanceHistory(vehicleId, userId);
      }

      # endregion
   }
}
