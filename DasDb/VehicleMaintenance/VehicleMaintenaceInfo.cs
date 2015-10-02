using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using System.Collections;	// for ArrayList
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Text;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Provides interfaces to vlfVehicleMaintenaceInfo table.
   /// </summary>
   [Obsolete("Class is obsolete, vlfVehicleServices table is used instead, class Vehicle")]
   public class VehicleMaintenaceInfo : TblOneIntPrimaryKey
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public VehicleMaintenaceInfo(SQLExecuter sqlExec)
         : base("vlfVehicleMaintenance", sqlExec)
      {
      }

      /// <summary>
      /// Add new vehicle maintenanceinfo.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastSrvOdo"></param>
      /// <param name="maxSrvInterval"></param>
      /// <param name="lastSrvEngHrs"></param>
      /// <param name="engHrsSrvInterval"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="nextServiceDescription"></param>
      /// <returns>true if succeeded, otherwise false</returns>
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
         int rowsAffected = 0;
         // 1. Prepares SQL statement
         string sql = string.Format("INSERT INTO vlfVehicleMaintenance (VehicleId,LastSrvOdo,CurrentOdo,MaxSrvInterval,LastSrvEngHrs,CurrentEngHrs,EngHrsSrvInterval,Email,TimeZone,DayLightSaving,AutoAdjustDayLightSaving,NextServiceDescription) VALUES ( {0},{1},{2},{3},{4},{5},{6},'{7}',{8},{9},{10},'{11}')",
         vehicleId, lastSrvOdo, currentOdo, maxSrvInterval, lastSrvEngHrs, currentEngHrs, engHrsSrvInterval, email.Replace("'", "''"),
         timeZone, dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription.Replace("'", "''"));
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to add new vehicle=" + vehicleId + " maintenance information.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to add new vehicle=" + vehicleId + " maintenance information.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to add new vehicle=" + vehicleId + " maintenance information.";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle maintenance information already exists.");
         }
      }

      /// <summary>
      /// Add new vehicle maintenance history.
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
      public void AddVehicleMaintenanceHst(Int64 vehicleId, int userId, DateTime serviceDateTime, string serviceDescription, double serviceOdo)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to add new vehicle=" + vehicleId + " maintenance history.";

         // 1. Prepares SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
         sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@srvcDate", SqlDbType.DateTime, serviceDateTime);
         sqlExec.AddCommandParam("@srvcDescr", SqlDbType.VarChar, serviceDescription.Replace("'", "''"));
         sqlExec.AddCommandParam("@serviceOdo", SqlDbType.Float, serviceOdo);
         string sql = 
            "INSERT INTO vlfVehicleMaintenanceHst (VehicleId, UserId, ServiceDateTime, ServiceDescription, ServiceOdo) VALUES (@vehicleId, @userId, @srvcDate, @srvcDescr, @serviceOdo)";
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle maintenance information already exists.");
         }
      }

      /// <summary>
      /// Add new vehicle maintenance history incl. engine service
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="srvcDateTime"></param>
      /// <param name="srvcDescription"></param>
      /// <param name="srvcEngine"></param>
      /// <returns>true if succeeded, otherwise false</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddVehicleMaintenanceEngineHst(Int64 vehicleId, 
                                                 int userId, 
                                                 DateTime serviceDateTime, 
                                                 string serviceDescription, 
                                                 int srvcEngine)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to add new vehicle=" + vehicleId + " maintenance history.";

         // 1. Prepares SQL statement
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
         sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@srvcDate", SqlDbType.DateTime, serviceDateTime);
         sqlExec.AddCommandParam("@srvcDescr", SqlDbType.VarChar, serviceDescription.Replace("'", "''"));
         sqlExec.AddCommandParam("@engHrs", SqlDbType.Int, srvcEngine);
         string sql = 
            "INSERT INTO vlfVehicleMaintenanceHst (VehicleId, UserId, ServiceDateTime, ServiceDescription, ServiceEngHours) VALUES (@vehicleId, @userId, @srvcDate, @srvcDescr, @engHrs)";
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 5. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }

            // 6. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle maintenance information already exists.");
         }
      }

      /// <summary>
      /// Deletes existing vehicle maintenance info.
      /// </summary>
      /// <param name="vehicleId"></param> 
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteVehicleMaintenanceInfo(Int64 vehicleId)
      {
         return DeleteRowsByIntField("VehicleId", vehicleId, "vehicle id");
      }

      /// <summary>
      /// Deletes existing vehicle maintenance history.
      /// </summary>
      /// <param name="vehicleId"></param> 
      /// <returns>rows affected</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int DeleteVehicleMaintenanceHst(Int64 vehicleId)
      {
         int rowsAffected = 0;
         // 1. Prepares SQL statement
         string sql = "DELETE FROM vlfVehicleMaintenanceHst WHERE VehicleId=" + vehicleId;
         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to delete by vehicleId " + vehicleId + " history.";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to delete by vehicleId " + vehicleId + " history.";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Returns vehicle maintenance information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="innerSet">True returns inner join result set, False returns outer join result set</param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <remarks>2008-02-28 Max - added second parameter to get both types of result set</remarks>
      public DataSet GetVehicleMaintenanceInfo(Int64 vehicleId, bool innerSet)
      {
         DataSet resultDataSet = new DataSet();
         string prefixMsg = "Unable to retrieve vehicle maintenance info for vehicle " + vehicleId + ". ";
         try
         {
            // 1. Prepares SQL statement
            // LEFT OUTER JOIN always returns result even if the vehicle is not in the vlfVehicleMaintenance table
            // INNER JOIN returns result only if the vehicle is in the vlfVehicleMaintenance table
            string joinString = innerSet ? "INNER" : "LEFT OUTER";
            StringBuilder sql = new StringBuilder("SELECT BoxId, vlfVehicleInfo.VehicleId, Description,");
            sql.AppendLine("CASE WHEN LastSrvOdo IS NULL then 0 ELSE CAST(ROUND(LastSrvOdo, 2) AS decimal(10,0)) END AS LastSrvOdo,");
            sql.AppendLine("CASE WHEN CurrentOdo IS NULL then 0 ELSE CAST(ROUND(CurrentOdo, 2) AS decimal(10,0)) END AS CurrentOdo,");
            sql.AppendLine("CASE WHEN MaxSrvInterval IS NULL then 0 ELSE CAST(ROUND(MaxSrvInterval, 2) AS decimal(10,0)) END AS MaxSrvInterval,");
            sql.AppendLine("ISNULL(LastSrvEngHrs,0) AS LastSrvEngHrs,");
            sql.AppendLine("ISNULL(CurrentEngHrs,0) AS CurrentEngHrs,");
            sql.AppendLine("ISNULL(EngHrsSrvInterval,0) AS EngHrsSrvInterval,");
            sql.AppendLine("ISNULL(vlfVehicleMaintenance.Email, ' ') AS Email,");
            sql.AppendLine("ISNULL(vlfVehicleMaintenance.TimeZone, 0) AS TimeZone,");
            sql.AppendLine("ISNULL(vlfVehicleMaintenance.DayLightSaving, 0) AS DayLightSaving,");
            sql.AppendLine("ISNULL(vlfVehicleMaintenance.AutoAdjustDayLightSaving, 0) AS AutoAdjustDayLightSaving,");
            sql.AppendLine("ISNULL(vlfVehicleMaintenance.NextServiceDescription, ' ') as NextServiceDescription, VehicleTypeId");
            sql.AppendLine("FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId");
            sql.AppendFormat("{0} JOIN vlfVehicleMaintenance ON vlfVehicleInfo.VehicleId = vlfVehicleMaintenance.VehicleId", joinString);
            sql.AppendLine();
            sql.AppendFormat("WHERE vlfVehicleInfo.VehicleId = {0}", vehicleId);
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql.ToString());
            }
            // 3. Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }


      // Changes for TimeZone Feature start

      /// <summary>
      /// Returns vehicle maintenance information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="innerSet">True returns inner join result set, False returns outer join result set</param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <remarks>2008-02-28 Max - added second parameter to get both types of result set</remarks>
      public DataSet GetVehicleMaintenanceInfo_NewTZ(Int64 vehicleId, bool innerSet, int userId)
      {
          DataSet resultDataSet = new DataSet();
          string prefixMsg = "Unable to retrieve vehicle maintenance info for vehicle " + vehicleId + ". ";
          try
          {
              // 1. Prepares SQL statement
              // LEFT OUTER JOIN always returns result even if the vehicle is not in the vlfVehicleMaintenance table
              // INNER JOIN returns result only if the vehicle is in the vlfVehicleMaintenance table
              string joinString = innerSet ? "INNER" : "LEFT OUTER";
              StringBuilder sql = new StringBuilder("DECLARE @unitOfMes float ");
              sql.AppendFormat("select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={0} and PreferenceId=0", userId);
              sql.AppendLine("SELECT BoxId, vlfVehicleInfo.VehicleId, Description,");
              sql.AppendLine("CASE WHEN LastSrvOdo IS NULL then 0 ELSE CAST(ROUND(LastSrvOdo*@unitOfMes, 2) AS decimal(10,0)) END AS LastSrvOdo,");
              sql.AppendLine("CASE WHEN CurrentOdo IS NULL then 0 ELSE CAST(ROUND(CurrentOdo*@unitOfMes, 2) AS decimal(10,0)) END AS CurrentOdo,");
              sql.AppendLine("CASE WHEN MaxSrvInterval IS NULL then 0 ELSE CAST(ROUND(MaxSrvInterval, 2) AS decimal(10,0)) END AS MaxSrvInterval,");
              sql.AppendLine("Round(ISNULL(cast(LastSrvEngHrs as float)/60,0),2) AS LastSrvEngHrs,");
              sql.AppendLine("Round(ISNULL(cast(CurrentEngHrs as float)/60,0),2) AS CurrentEngHrs,");
              sql.AppendLine("Round(ISNULL(cast(EngHrsSrvInterval as float)/60,0),2) AS EngHrsSrvInterval,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.Email, ' ') AS Email,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.TimeZoneNew, 0) AS TimeZone,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.DayLightSaving, 0) AS DayLightSaving,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.AutoAdjustDayLightSaving, 0) AS AutoAdjustDayLightSaving,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.NextServiceDescription, ' ') as NextServiceDescription, VehicleTypeId");
              sql.AppendLine("FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId");
              sql.AppendFormat("{0} JOIN vlfVehicleMaintenance ON vlfVehicleInfo.VehicleId = vlfVehicleMaintenance.VehicleId", joinString);
              sql.AppendLine();
              sql.AppendFormat("WHERE vlfVehicleInfo.VehicleId = {0}", vehicleId);
              if (sqlExec.RequiredTransaction())
              {
                  // 2. Attaches SQL to transaction
                  sqlExec.AttachToTransaction(sql.ToString());
              }
              // 3. Executes SQL statement
              resultDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultDataSet;
      }

      // Changes for TimeZone Feature end

      /// <summary>
      /// Returns vehicle maintenance information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="innerSet">True returns inner join result set, False returns outer join result set</param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <remarks>2008-02-28 Max - added second parameter to get both types of result set</remarks>
      public DataSet GetVehicleMaintenanceInfo(Int64 vehicleId, bool innerSet,int userId)
      {
          DataSet resultDataSet = new DataSet();
          string prefixMsg = "Unable to retrieve vehicle maintenance info for vehicle " + vehicleId + ". ";
          try
          {
              // 1. Prepares SQL statement
              // LEFT OUTER JOIN always returns result even if the vehicle is not in the vlfVehicleMaintenance table
              // INNER JOIN returns result only if the vehicle is in the vlfVehicleMaintenance table
              string joinString = innerSet ? "INNER" : "LEFT OUTER";
              StringBuilder sql = new StringBuilder("DECLARE @unitOfMes float ");
              sql.AppendFormat("select @unitOfMes=PreferenceValue from vlfUserPreference where UserId={0} and PreferenceId=0",userId);
              sql.AppendLine("SELECT BoxId, vlfVehicleInfo.VehicleId, Description,");
              sql.AppendLine("CASE WHEN LastSrvOdo IS NULL then 0 ELSE CAST(ROUND(LastSrvOdo*@unitOfMes, 2) AS decimal(10,0)) END AS LastSrvOdo,");
              sql.AppendLine("CASE WHEN CurrentOdo IS NULL then 0 ELSE CAST(ROUND(CurrentOdo*@unitOfMes, 2) AS decimal(10,0)) END AS CurrentOdo,");
              sql.AppendLine("CASE WHEN MaxSrvInterval IS NULL then 0 ELSE CAST(ROUND(MaxSrvInterval, 2) AS decimal(10,0)) END AS MaxSrvInterval,");
              sql.AppendLine("Round(ISNULL(cast(LastSrvEngHrs as float)/60,0),2) AS LastSrvEngHrs,");
              sql.AppendLine("Round(ISNULL(cast(CurrentEngHrs as float)/60,0),2) AS CurrentEngHrs,");
              sql.AppendLine("Round(ISNULL(cast(EngHrsSrvInterval as float)/60,0),2) AS EngHrsSrvInterval,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.Email, ' ') AS Email,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.TimeZone, 0) AS TimeZone,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.DayLightSaving, 0) AS DayLightSaving,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.AutoAdjustDayLightSaving, 0) AS AutoAdjustDayLightSaving,");
              sql.AppendLine("ISNULL(vlfVehicleMaintenance.NextServiceDescription, ' ') as NextServiceDescription, VehicleTypeId");
              sql.AppendLine("FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId");
              sql.AppendFormat("{0} JOIN vlfVehicleMaintenance ON vlfVehicleInfo.VehicleId = vlfVehicleMaintenance.VehicleId", joinString);
              sql.AppendLine();
              sql.AppendFormat("WHERE vlfVehicleInfo.VehicleId = {0}", vehicleId);
              if (sqlExec.RequiredTransaction())
              {
                  // 2. Attaches SQL to transaction
                  sqlExec.AttachToTransaction(sql.ToString());
              }
              // 3. Executes SQL statement
              resultDataSet = sqlExec.SQLExecuteDataset(sql.ToString());
          }
          catch (SqlException objException)
          {
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              throw new DASException(prefixMsg + " " + objException.Message);
          }
          return resultDataSet;
      }


      
      /// <summary>
      /// Returns fleet maintenance information.
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <returns>DataSet [BoxId],[VehicleId],[Description],
      /// [LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
      /// [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],
      /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
      /// [LicensePlate],[ModelYear],[MakeName],[ModelName],
      /// [NextServiceDescription],[VehicleTypeId]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetFleetMaintenanceInfo(int fleetId, int userId)
      {
         DataSet resultDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "DECLARE @Unit real SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits + " IF @Unit IS NULL SET @Unit=1" +
                " SELECT  vlfVehicleAssignment.BoxId,vlfVehicleInfo.VehicleId,Description," +
                "CASE WHEN vlfVehicleMaintenance.LastSrvOdo IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,CAST(ROUND(vlfVehicleMaintenance.LastSrvOdo * @Unit,1) AS decimal(10,0))) END AS LastSrvOdo," +
                "CASE WHEN vlfVehicleMaintenance.CurrentOdo IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,CAST(ROUND(vlfVehicleMaintenance.CurrentOdo * @Unit,1) AS decimal(10,0))) END AS CurrentOdo," +
                "CASE WHEN vlfVehicleMaintenance.MaxSrvInterval IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,CAST(ROUND(vlfVehicleMaintenance.MaxSrvInterval * @Unit,1) AS decimal(10,0))) END AS MaxSrvInterval," +
               /*"CASE WHEN vlfVehicleMaintenance.MaxSrvInterval IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,ROUND(vlfVehicleMaintenance.MaxSrvInterval * @Unit,1)) END AS MaxSrvInterval," +*/
                "Round(ISNULL(cast(LastSrvEngHrs as float),0)/60,2) AS LastSrvEngHrs," +
                "Round(ISNULL(cast(CurrentEngHrs as float),0)/60,2) AS CurrentEngHrs," +
                "Round(ISNULL(cast(EngHrsSrvInterval as float),0)/60,2) AS EngHrsSrvInterval," +
                "ISNULL(vlfVehicleMaintenance.Email,' ') AS Email," +
                "ISNULL(vlfVehicleMaintenance.TimeZone,0) AS TimeZone," +
                "ISNULL(vlfVehicleMaintenance.DayLightSaving,0) AS DayLightSaving," +
                "ISNULL(vlfVehicleMaintenance.AutoAdjustDayLightSaving,0) AS AutoAdjustDayLightSaving," +
                "vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.ModelYear, vlfMake.MakeName, vlfModel.ModelName," +
                "NextServiceDescription,VehicleTypeId" +
                " FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfFleetVehicles ON vlfVehicleInfo.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfMakeModel ON vlfVehicleInfo.MakeModelId = vlfMakeModel.MakeModelId INNER JOIN vlfMake ON vlfMakeModel.MakeId = vlfMake.MakeId INNER JOIN vlfModel ON vlfMakeModel.ModelId = vlfModel.ModelId LEFT OUTER JOIN vlfVehicleMaintenance ON vlfVehicleInfo.VehicleId = vlfVehicleMaintenance.VehicleId WHERE vlfFleetVehicles.FleetId=" + fleetId;
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve vehicles maintenance info for fleet " + fleetId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve vehicles maintenance info for fleet " + fleetId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Checks if vehicle maintenance information exists.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>true if exist, otherwisw false</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool IsVehicleMaintenanceInfoExist(Int64 vehicleId)
      {
         bool retResult = false;
         try
         {
            // 1. Prepares SQL statement
            string sql = "SELECT COUNT(*) FROM vlfVehicleMaintenance WHERE VehicleId=" + vehicleId;
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            if ((int)sqlExec.SQLExecuteScalar(sql) > 0)
               retResult = true;
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve vehicle maintenance info for vehicle " + vehicleId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve vehicle maintenance info for vehicle " + vehicleId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return retResult;
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
         int rowsAffected = 0;
         // 1. Prepares SQL statement
         string sql = "UPDATE vlfVehicleMaintenance SET LastSrvOdo=" + lastSrvOdo + ",NextServiceDescription='" + nextServiceDescription.Replace("'", "''") + "' WHERE VehicleId=" + vehicleId;

         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " LastSrvOdo=" + lastSrvOdo + " UpdateVehicleMaintenanceInfo=" + nextServiceDescription + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " LastSrvOdo=" + lastSrvOdo + " UpdateVehicleMaintenanceInfo=" + nextServiceDescription + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " LastSrvOdo=" + lastSrvOdo + " UpdateVehicleMaintenanceInfo=" + nextServiceDescription + ".";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
         }
      }

      /// <summary>
      /// Updates vehicle next service description.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastSrvOdo"></param>
      /// <param name="lastEngine"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleMaintenanceInfo(Int64 vehicleId, double lastSrvOdo, int lastEngine, string nextServiceDescription)
      {
         int rowsAffected = 0;
         // 1. Prepares SQL statement
         string sql = "UPDATE vlfVehicleMaintenance SET LastSrvOdo=" + lastSrvOdo + ",LastSrvEngHrs=" + lastEngine +
            ",NextServiceDescription='" + nextServiceDescription.Replace("'", "''") + "' WHERE VehicleId=" + vehicleId;

         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " LastSrvOdo=" + lastSrvOdo + " UpdateVehicleMaintenanceInfo=" + nextServiceDescription + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " LastSrvOdo=" + lastSrvOdo + " UpdateVehicleMaintenanceInfo=" + nextServiceDescription + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " LastSrvOdo=" + lastSrvOdo + " UpdateVehicleMaintenanceInfo=" + nextServiceDescription + ".";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
         }
      }

      /// <summary>
      ///    Updates vehicle current Odo information.
      /// </summary>
      /// <comment>
      ///    added the feature to insert the record if is not there - gb 2006/09/6
      /// </comment>
      /// <param name="vehicleId"></param>
      /// <param name="currentOdo"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleCurrOdoInfo(Int64 vehicleId, double currentOdo)
      {
         int rowsAffected = 0;
#if false
           
           // 1. Prepares SQL statement
           string sql = "UPDATE vlfVehicleMaintenance SET CurrentOdo=" + currentOdo + " WHERE VehicleId=" + vehicleId;

            try
            {
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 3. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update vehicle=" + vehicleId + " current Odo=" + currentOdo + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update vehicle=" + vehicleId + " current Odo=" + currentOdo + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update vehicle=" + vehicleId + " current Odo=" + currentOdo + ".";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
            }
#else
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@CurrentOdo", SqlDbType.Float, currentOdo);

            // 3. Executes SQL store procedure 
            rowsAffected = sqlExec.SPExecuteNonQuery("sp_UpdateVehicleCurrOdometer");
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " current Odo=" + currentOdo + ".";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " current Odo=" + currentOdo + ".";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to update vehicle=" + vehicleId + " current Odo=" + currentOdo + ".";
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This box already exists.");
         }

#endif
      }

      public int IncrementBoxCurrentEngHrsInfo(int boxId, int currentEngHrs)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to increment vehicle=" + boxId.ToString() + " current engine hours=" + currentEngHrs.ToString() + ".";

         try
         {
            //if (sqlExec.RequiredTransaction())
            //{
            // 2. Attach current command SQL to transaction
            //sqlExec.AttachToTransaction(sql);
            //}
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@EngineHours", SqlDbType.Int, currentEngHrs);

            // 3. Executes SQL store procedure 
            rowsAffected = sqlExec.SPExecuteNonQuery("BoxEngineHoursIncrement");

            // 3. Executes SQL statement
            //rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return rowsAffected;
      }

      public int IncrementVehicleCurrentEngHrsInfo(Int64 vehicleId, int currentEngHrs)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to increment vehicle=" + vehicleId.ToString() + " current engine hours=" + currentEngHrs.ToString() + ".";

         try
         {
            //if (sqlExec.RequiredTransaction())
            //{
            // 2. Attach current command SQL to transaction
            //sqlExec.AttachToTransaction(sql);
            //}
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@EngineHours", SqlDbType.Int, currentEngHrs);

            // 3. Executes SQL store procedure 
            rowsAffected = sqlExec.SPExecuteNonQuery("VehicleEngineHoursIncrement");

            // 3. Executes SQL statement
            //rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return rowsAffected;
      }

      public int UpdateBoxOdometer(int boxId, double currentOdo)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to update boxId=" + boxId.ToString() + " current odometer=" + currentOdo.ToString() + ".";
         // 1. Prepares SQL statement
         //string sql = "UPDATE vlfVehicleMaintenance SET CurrentEngHrs = CurrentEngHrs + " + currentEngHrs.ToString() + " WHERE VehicleId=" + vehicleId;

         try
         {
            //if (sqlExec.RequiredTransaction())
            //{
            // 2. Attach current command SQL to transaction
            //sqlExec.AttachToTransaction(sql);
            //}
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@Odometer", SqlDbType.Float, currentOdo);

            // 3. Executes SQL store procedure 
            rowsAffected = sqlExec.SPExecuteNonQuery("BoxOdometerUpdate");

            // 3. Executes SQL statement
            //rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         //if (rowsAffected == 0)
         //{
         //   throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle does not exist.");
         //}
         return rowsAffected;
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
         int rowsAffected = 0;
         string prefixMsg = "Unable to update vehicle=" + vehicleId.ToString() + " current engine hours=" + currentEngHrs.ToString() + ".";
         // 1. Prepares SQL statement
         //string sql = "UPDATE vlfVehicleMaintenance SET CurrentEngHrs = CurrentEngHrs + " + currentEngHrs.ToString() + " WHERE VehicleId=" + vehicleId;

         try
         {
            //if (sqlExec.RequiredTransaction())
            //{
               // 2. Attach current command SQL to transaction
               //sqlExec.AttachToTransaction(sql);
            //}
               sqlExec.ClearCommandParameters();
               sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, vehicleId);
               sqlExec.AddCommandParam("@EngineHours", SqlDbType.Int, currentEngHrs);

               // 3. Executes SQL store procedure 
               rowsAffected = sqlExec.SPExecuteNonQuery("VehicleEngineHoursUpdate");
            
            // 3. Executes SQL statement
            //rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         //if (rowsAffected == 0)
         //{
         //   throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle does not exist.");
         //}
         return rowsAffected;
      }

      /// <summary>
      /// Updates vehicle current engine hours + idling hours information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="currentEngHrs"></param>
      /// <param name="idlingHours"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int UpdateVehicleHoursInfo(Int64 vehicleId, int currentEngHrs, int idlingHours)
      {
         int rowsAffected = 0;
         string prefixMsg = String.Format("Error updating Vehicle={0} Engine hours={1} Idling hours={2}",
            vehicleId, currentEngHrs, idlingHours);

         try
         {
            // 1. Prepares SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@VehicleId", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@CurrentEngineHours", SqlDbType.Int, currentEngHrs);
            sqlExec.AddCommandParam("@CurrentIdlingHours", SqlDbType.Int, idlingHours);

            string sql = "UpdateVehicleHours";

            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SPExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return rowsAffected;
      }

      /// <summary>
      /// Updates vehicle maintenance history.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="serviceDateTime"></param>
      /// <param name="serviceDescription"></param>
      /// <param name="serviceOdo"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateVehicleMaintenanceHst(Int64 vehicleId, 
                                              DateTime serviceDateTime, 
                                              string serviceDescription, 
                                              double serviceOdo)
      {
         int rowsAffected = 0;
         string prefixMsg = "Unable to update vehicle=" + vehicleId + " history.";
         // 1. Prepares SQL statement
         string sql = "UPDATE vlfVehicleMaintenanceHst SET ServiceDateTime='" + serviceDateTime.ToString() + "',ServiceDescription='" + serviceDescription.Replace("'", "''") + "',ServiceOdo=" + serviceOdo + " WHERE VehicleId=" + vehicleId;

         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle does not exist.");
         }
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
         int rowsAffected = 0;
         string prefixMsg = "Unable to update vehicle=" + vehicleId + " information.";
         // 1. Prepares SQL statement
         string sql = "UPDATE vlfVehicleMaintenance SET LastSrvOdo=" + lastSrvOdo +
             ",CurrentOdo=" + currentOdo +
             ",MaxSrvInterval=" + maxSrvInterval +
             ",Email='" + email.Replace("'", "''") +
             "',TimeZone=" + timeZone +
             ",DayLightSaving=" + dayLightSaving +
             ",AutoAdjustDayLightSaving=" + autoAdjustDayLightSaving +
             ",NextServiceDescription='" + nextServiceDescription.Replace("'", "''") +
             "' WHERE VehicleId=" + vehicleId;

         try
         {
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attach current command SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle doesn't exist.");
         }
      }

      /// <summary>
      /// Updates vehicle maintenance information + engine and idling hours
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastEngHours"></param>
      /// <param name="currentEngHours"></param>
      /// <param name="intervalEngHours"></param>
      /// <param name="email"></param>
      /// <param name="timeZone"></param>
      /// <param name="dayLightSaving"></param>
      /// <param name="autoAdjustDayLightSaving"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <exception cref="DASDbException">Thrown if there is no entry for the vehicle in the table</exception>
      public void UpdateVehicleEngineMaintenanceInfo(Int64 vehicleId,
         int lastEngHours, int currentEngHours, int intervalEngHours,
         string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving,
         string nextServiceDescription)
      {
         int rowsAffected = 0, lastIdling = 0, currIdling = 0;
         string sql = "";
         string prefixMsg = "Unable to update vehicle=" + vehicleId + " information.";

         try
         {
            // get idling hours
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
            sql = "SELECT ISNULL(LastSrvIdlingHrs, 0) AS LastSrvIdlingHrs, ISNULL(CurrentIdlingHrs, 0) AS CurrentIdlingHrs FROM vlfVehicleMaintenance WHERE VehicleId=@vehicleId";

            DataSet dsIdling = sqlExec.SQLExecuteDataset(sql);

            if (!Util.IsDataSetValid(dsIdling)) 
            {
               throw new DASDbException("The vehicle doesn't have maintenance entry: " + vehicleId.ToString());
            }

            lastIdling = Convert.ToInt32(dsIdling.Tables[0].Rows[0]["LastSrvIdlingHrs"].ToString());
            currIdling = Convert.ToInt32(dsIdling.Tables[0].Rows[0]["CurrentIdlingHrs"].ToString());
            lastIdling += currIdling;

            // 2. Attach current command SQL to transaction
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@lastEngHours", SqlDbType.Int, lastEngHours);
            //sqlExec.AddCommandParam("@currentEngHours", SqlDbType.Int, currentEngHours);
            sqlExec.AddCommandParam("@intervalEngHours", SqlDbType.Int, intervalEngHours);
            sqlExec.AddCommandParam("@email", SqlDbType.VarChar, email.Replace("'", "''"));
            sqlExec.AddCommandParam("@timeZone", SqlDbType.Int, timeZone);
            sqlExec.AddCommandParam("@dayLightSaving", SqlDbType.SmallInt, dayLightSaving);
            sqlExec.AddCommandParam("@autoAdjustDayLightSaving", SqlDbType.SmallInt, autoAdjustDayLightSaving);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, nextServiceDescription.Replace("'", "''"));
            sqlExec.AddCommandParam("@lastIdling", SqlDbType.Int, lastIdling);
            sqlExec.AddCommandParam("@currIdling", SqlDbType.Int, 0);

            sql = "UPDATE vlfVehicleMaintenance SET LastSrvEngHrs=@lastEngHours, EngHrsSrvInterval=@intervalEngHours," +
               "Email=@email, TimeZone=@timeZone, DayLightSaving=@dayLightSaving, AutoAdjustDayLightSaving=@autoAdjustDayLightSaving," +
               "NextServiceDescription=@descr, LastSrvIdlingHrs=@lastIdling, CurrentIdlingHrs=@currIdling" +
               " WHERE VehicleId=@vehicleId";

            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle doesn't exist.");
         }
      }

      /// <summary>
      /// Updates vehicle maintenance last information
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <param name="lastEngHours"></param>
      /// <param name="nextServiceDescription"></param>
      /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <exception cref="DASDbException">Thrown if there is no entry for the vehicle in the table</exception>
      public void UpdateVehicleEngineMaintenanceInfo(Int64 vehicleId, int lastEngHours, string nextServiceDescription)
      {
         int rowsAffected = 0, lastIdling = 0, currIdling = 0;
         string sql = "";
         string prefixMsg = "Unable to update vehicle=" + vehicleId + " information.";

         try
         {
            // get idling hours
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
            sql = "SELECT ISNULL(LastSrvIdlingHrs, 0) AS LastSrvIdlingHrs, ISNULL(CurrentIdlingHrs, 0) AS CurrentIdlingHrs FROM vlfVehicleMaintenance WHERE VehicleId=@vehicleId";

            DataSet dsIdling = sqlExec.SQLExecuteDataset(sql);

            if (!Util.IsDataSetValid(dsIdling))
            {
               throw new DASDbException("The vehicle doesn't have maintenance entry: " + vehicleId.ToString());
            }

            lastIdling = Convert.ToInt32(dsIdling.Tables[0].Rows[0]["LastSrvIdlingHrs"].ToString());
            currIdling = Convert.ToInt32(dsIdling.Tables[0].Rows[0]["CurrentIdlingHrs"].ToString());
            lastIdling += currIdling;

            // 2. Attach current command SQL to transaction
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@vehicleId", SqlDbType.BigInt, vehicleId);
            sqlExec.AddCommandParam("@lastEngHours", SqlDbType.Int, lastEngHours);
            sqlExec.AddCommandParam("@descr", SqlDbType.VarChar, nextServiceDescription.Replace("'", "''"));
            sqlExec.AddCommandParam("@lastIdling", SqlDbType.Int, lastIdling);
            sqlExec.AddCommandParam("@currIdling", SqlDbType.Int, 0);

            sql = "UPDATE vlfVehicleMaintenance SET LastSrvEngHrs=@lastEngHours, NextServiceDescription=@descr, LastSrvIdlingHrs=@lastIdling, CurrentIdlingHrs=@currIdling" +
               " WHERE VehicleId=@vehicleId";

            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         if (rowsAffected == 0)
         {
            throw new DASAppDataAlreadyExistsException(prefixMsg + " This vehicle doesn't exist.");
         }
      }

      /// <summary>
      /// Returns vehicle maintenance information.
      /// </summary>
      /// <param name="vehicleId"></param>
      /// <returns>DataSet [VehicleId],[ServiceDateTime],[ServiceDescription],[ServiceOdo]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehicleMaintenanceHistory(Int64 vehicleId, int userId)
      {
         DataSet resultDataSet = null;
         try
         {
            // 1. Prepares SQL statement
            string sql = "DECLARE @Unit real SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits + " IF @Unit IS NULL SET @Unit=1 SELECT VehicleId, CASE WHEN ServiceDateTime IS NULL then CONVERT(VARCHAR,'1/1/0001') ELSE CONVERT(VARCHAR,ServiceDateTime,101) +' '+ convert(varchar,ServiceDateTime,108) END AS ServiceDateTime, ServiceDescription, CASE WHEN ServiceOdo IS NULL then convert(nvarchar,0) ELSE convert(nvarchar,CAST(ROUND(ServiceOdo * @Unit,1) AS decimal(10,0))) END AS ServiceOdo, ServiceEngHours, UserId FROM vlfVehicleMaintenanceHst WHERE VehicleId=" + vehicleId + " ORDER BY ServiceDateTime DESC";
            if (sqlExec.RequiredTransaction())
            {
               // 2. Attaches SQL to transaction
               sqlExec.AttachToTransaction(sql);
            }
            // 3. Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve vehicle maintenance history for vehicle " + vehicleId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve vehicle maintenance history for vehicle " + vehicleId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

     
   }
}
