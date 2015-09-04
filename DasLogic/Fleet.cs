using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;
using System.Text;
using System.Collections.Generic;
//using VLF.CLS;

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to fleet functionality in database
	/// </summary>
	public class Fleet : Das
	{
		FleetVehicles fleetVehicles = null;
		FleetUsers fleetUsers = null;
		DB.Fleet fleet = null;
		DB.VehicleAssignment vehicleAssignment = null;
		DB.FleetEmails fleetEmails = null;
		DB.VehicleInfo vehicleInfo = null;
	
		#region General Interfaces

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
        public Fleet(string connectionString)
            : base(connectionString)
		{
			fleetVehicles = new FleetVehicles(sqlExec);
			fleetUsers = new FleetUsers(sqlExec);
			fleet = new DB.Fleet(sqlExec);
			fleetVehicles = new FleetVehicles(sqlExec);
			vehicleAssignment = new DB.VehicleAssignment(sqlExec);
			fleetEmails = new FleetEmails(sqlExec);
			vehicleInfo = new DB.VehicleInfo(sqlExec);
		}

		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}

		#endregion

		#region Fleet/Vehicle Interfaces
		/// <summary>
		/// Add new vehicle to fleet.
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="fleetId"></param>
		/// <returns>Int</returns>
		/// <exception cref="DASAppWrongResultException">Thrown if unable to retrieve organization info for current fleet.</exception>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if fleet and vehicle do no assigned to same organization..</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int AddVehicleToFleet(int fleetId, Int64 vehicleId)
		{
			DataSet dsFleetInfo = fleet.GetFleetInfoByFleetId(fleetId);
            if (dsFleetInfo == null || dsFleetInfo.Tables.Count == 0 || dsFleetInfo.Tables[0].Rows.Count == 0)
				throw new DASAppWrongResultException("Unable to retrieve organization info for current fleet.");
			
			int vehicleOrganizationId = vehicleInfo.GetVehicleOrganization(vehicleId);
            if (Convert.ToInt32(dsFleetInfo.Tables[0].Rows[0]["OrganizationId"]) != vehicleOrganizationId)
				throw new DASAppViolatedIntegrityConstraintsException("Fleet and vehicle do no assigned to same organization.");

            /// make sure the vehicle is not already assigned to the fleet
            //DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
            if (!fleetVehicles.IsVehicleExistInFleet(fleetId, vehicleId))
                return fleetVehicles.AddVehicleToFleet(fleetId, vehicleId);

            return 0;
		}		
		/// <summary>
		/// Delete existing vehicle from the fleets.
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <param name="vehicleId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in exception cases.</exception>
        public int DeleteVehicleFromFleet(int fleetId, Int64 vehicleId)
		{
            return fleetVehicles.DeleteVehicleFromFleet(fleetId, vehicleId);
		}
		/// <summary>
		/// Delete existing vehicle assignments from the fleet and the fleet.
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in exception cases.</exception>
		public int DeleteAllVehiclesFromFleet(int fleetId)
		{
			return fleetVehicles.DeleteAllVehiclesFromFleet(fleetId);
		}
		/// <summary>
		/// Retreive unassigned to any fleet vehicles.
		/// </summary>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetAllUnassingToFleetsVehiclesInfo(int organizationId)
		{
			DataSet dsResult = fleetVehicles.GetAllUnassingToFleetsVehiclesInfo(organizationId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "VehiclesInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		/// <summary>
		/// Retreive all active vehicles info that unassigned to current fleet.
		/// </summary>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetAllUnassingToFleetVehiclesInfo(int organizationId, int fleetId)
		{
            DataSet dsResult = fleetVehicles.GetAllUnassingToFleetVehiclesInfo(organizationId, fleetId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "VehiclesInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}

        
        /// <summary>
        ///       Returns ManagerName by VehicleId
        /// </summary>
        /// <param name="fleetId"> OrganizationId</param>
        /// <param name="featureMask">VehicleId </param>
        /// <returns>XML [ManagerName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetManagerNameByVehicleId(int VehicleId)
        {
            DataSet dsResult = fleetVehicles.GetManagerNameByVehicleId(VehicleId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "ManagerNameInfo";
                }
                dsResult.DataSetName = "Manager";
            }
            return dsResult;
        }

       
        /// <summary>
        ///       Returns Rows affected after setting manager name to AssignedVehiclesList
        /// </summary>        
        /// <param name="AssignedVehiclesList">AssignedVehiclesList</param>
        /// <param name="ManagerName">ManagerName</param>
        /// <returns>rowsAffected
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int SetManagerNameByVehicleId(string AssignesVehiclesList, string ManagerName)
        {
            int rowsAffected = fleetVehicles.SetManagerNameByVehicleId(AssignesVehiclesList, ManagerName);

            return rowsAffected;
        }
		/// <summary>
		/// Returns vehicle information by fleet id. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetVehiclesInfoByFleetId(int fleetId)
		{
			DataSet dsResult = fleetVehicles.GetVehiclesInfoByFleetId(fleetId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "VehiclesInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		
		/// <summary>
		/// Returns vehicle information by fleet ids. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetVehiclesInfoByMultipleFleetIds(string fleetIds)
		{
			DataSet dsResult = fleetVehicles.GetVehiclesInfoByMultipleFleetIds(fleetIds);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "VehiclesInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}


        /// <summary>
        /// Returns vehicle peripherals by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesPeripheralInfoByFleetId(int fleetId)
        {
            DataSet dsResult = fleetVehicles.GetVehiclesPeripheralInfoByFleetId(fleetId);
            return dsResult;
        }
		
		
      /// <summary>
      /// Returns vehicle information by fleet id filtered by feature mask
      /// </summary>
      /// <param name="fleetId">Fleet Id</param>
      /// <param name="featureMask">Firmware feature mask inherited by a box</param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public DataSet GetVehiclesInfoByFleetId(int fleetId, long featureMask)
      {
         StringBuilder sql = new StringBuilder();
         sql.AppendLine("SELECT DISTINCT dbo.vlfVehicleAssignment.LicensePlate, dbo.vlfVehicleAssignment.BoxId, dbo.vlfVehicleAssignment.VehicleId, dbo.vlfVehicleInfo.VinNum, dbo.vlfMake.MakeName, dbo.vlfModel.ModelName, dbo.vlfVehicleType.VehicleTypeName, dbo.vlfVehicleInfo.StateProvince, dbo.vlfVehicleInfo.ModelYear, dbo.vlfVehicleInfo.Color, dbo.vlfVehicleInfo.Description, dbo.vlfFleetVehicles.FleetId, dbo.vlfFleet.FleetName");
         sql.AppendLine("FROM dbo.vlfVehicleAssignment INNER JOIN dbo.vlfFleetVehicles ON dbo.vlfVehicleAssignment.VehicleId = dbo.vlfFleetVehicles.VehicleId");
         sql.AppendLine("INNER JOIN dbo.vlfVehicleInfo ON dbo.vlfVehicleAssignment.VehicleId = dbo.vlfVehicleInfo.VehicleId");
         sql.AppendLine("INNER JOIN dbo.vlfMakeModel ON dbo.vlfVehicleInfo.MakeModelId = dbo.vlfMakeModel.MakeModelId");
         sql.AppendLine("INNER JOIN dbo.vlfMake ON dbo.vlfMakeModel.MakeId = dbo.vlfMake.MakeId");
         sql.AppendLine("INNER JOIN dbo.vlfModel ON dbo.vlfMakeModel.ModelId = dbo.vlfModel.ModelId");
         sql.AppendLine("INNER JOIN dbo.vlfVehicleType ON dbo.vlfVehicleInfo.VehicleTypeId = dbo.vlfVehicleType.VehicleTypeId");
         sql.AppendLine("INNER JOIN dbo.vlfFleet ON dbo.vlfFleetVehicles.FleetId = dbo.vlfFleet.FleetId");
         sql.AppendLine("INNER JOIN dbo.vlfBox ON dbo.vlfVehicleAssignment.BoxId = dbo.vlfBox.BoxId");
         sql.AppendLine("WHERE (dbo.vlfFleetVehicles.FleetId = @fleetId) AND (dbo.IsFlagOn(dbo.vlfBox.FwAttributes1, @featureMask) = 1)");
         sql.Append("ORDER BY vlfVehicleInfo.Description");

         SqlParameter[] sqlParams = 
            new SqlParameter[] { new SqlParameter("@fleetId", fleetId), new SqlParameter("@featureMask", featureMask) };

         DataSet dsResult = fleetVehicles.GetRowsBySql(sql.ToString(), sqlParams);

         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "VehiclesInformation";
            }
            dsResult.DataSetName = "Fleet";
         }
         return dsResult;
      }
        /// <summary>
        /// Returns complete vehicle information by fleet id
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public DataSet GetVehiclesFullInfobyFleetId(int fleetId)
        {
            DataSet dsResult = fleetVehicles.GetVehiclesFullInfobyFleetId(fleetId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehiclesInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }


      /// <summary>
      /// 
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="fromDateTime"></param>
      /// <param name="toDateTime"></param>
      /// <returns>
      ///      [BoxId],[LicensePlate],[VehicleDescription],[VehicleId],[VinNum],[OriginDateTime],[BoxMsgInTypeId], [CustomProp]
      /// </returns>
      public DataSet GetIdlingDurationForFleetId(int fleetId, int userId, DateTime fromDateTime, DateTime toDateTime)
      {
         return fleetVehicles.GetIdlingDurationForFleetId(fleetId, userId, fromDateTime, toDateTime);
      }
	  
	  /// <summary>
      /// 
      /// </summary>
      /// <param name="fleetIds"></param>
      /// <param name="fromDateTime"></param>
      /// <param name="toDateTime"></param>
      /// <returns>
      ///      [BoxId],[LicensePlate],[VehicleDescription],[VehicleId],[VinNum],[OriginDateTime],[BoxMsgInTypeId], [CustomProp]
      /// </returns>
      public DataSet GetIdlingDurationForMultipleFleetIds(string fleetIds, int userId, DateTime fromDateTime, DateTime toDateTime)
      {
         return fleetVehicles.GetIdlingDurationForMultipleFleetIds(fleetIds, userId, fromDateTime, toDateTime);
      }

        // Changes for TimeZone Feature start
      /// <summary>
      /// Returns vehicles last known position information by fleet id. 
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <param name="language"></param>
      /// <returns>
      /// DataSet 
      /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
      /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
      /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
      /// [IconTypeId],[IconTypeName],[VehicleTypeName],
      /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehiclesLastKnownPositionInfo_NewTZ(int fleetId, int userId, string language)
      {
          //DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
          DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo_SP_NewTZ(fleetId, userId, language);
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

        // Changes for TimeZone Feature end
	  
		/// <summary>
		/// Returns vehicles last known position information by fleet id. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="userId"></param>
        /// <param name="language"></param>
		/// <returns>
		/// DataSet 
		/// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
		/// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
		/// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
		/// [IconTypeId],[IconTypeName],[VehicleTypeName],
		/// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
		/// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
		/// [FwTypeId],[Dormant],[DormantDateTime]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfo(int fleetId, int userId, string language)
		{
			//DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
            DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo_SP(fleetId, userId, language);
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
        
        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns vehicles last known position information by multiple fleet ids. 
        /// </summary>
        /// <param name="fleetIds"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByMultipleFleets_NewTZ(string fleetIds, int userId, string language)
        {
            DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfoByMultipleFleets_SP_NewTZ(fleetIds, userId, language);
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
        // Changes for TimeZone Feature end
		
		/// <summary>
		/// Returns vehicles last known position information by multiple fleet ids. 
		/// </summary>
		/// <param name="fleetIds"></param>
		/// <param name="userId"></param>
        /// <param name="language"></param>
		/// <returns>
		/// DataSet 
		/// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
		/// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
		/// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
		/// [IconTypeId],[IconTypeName],[VehicleTypeName],
		/// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
		/// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
		/// [FwTypeId],[Dormant],[DormantDateTime]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByMultipleFleets(string fleetIds, int userId, string language)
		{
			DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfoByMultipleFleets_SP(fleetIds, userId, language);
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
        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns vehicles last known position information (if it is changed) by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesChangedPositionInfoByLang_NewTZ(int fleetId, int userId, string language, DateTime lastChecked)
        {
            //DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
            DataSet dsResult = fleetVehicles.GetVehiclesChangedPositionInfoByLang_NewTZ(fleetId, userId, language, lastChecked);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehiclesChangedPositionInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end


        /// <summary>
        /// Returns vehicles last known position information (if it is changed) by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesChangedPositionInfoByLang(int fleetId, int userId, string language, DateTime lastChecked)
        {
            //DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
            DataSet dsResult = fleetVehicles.GetVehiclesChangedPositionInfoByLang(fleetId, userId, language, lastChecked);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehiclesChangedPositionInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns vehicles last known position information (if it is changed) by multiple fleet ids. 
        /// </summary>
        /// <param name="fleetIds"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesChangedPositionInfoByMultipleFleetsByLang_NewTZ(string fleetIds, int userId, string language, DateTime lastChecked)
        {
            //DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
            DataSet dsResult = fleetVehicles.GetVehiclesChangedPositionInfoByMultipleFleetsByLang_NewTZ(fleetIds, userId, language, lastChecked);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehiclesChangedPositionInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature end
		
		/// <summary>
        /// Returns vehicles last known position information (if it is changed) by multiple fleet ids. 
        /// </summary>
        /// <param name="fleetIds"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesChangedPositionInfoByMultipleFleetsByLang(string fleetIds, int userId, string language, DateTime lastChecked)
        {
            //DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
            DataSet dsResult = fleetVehicles.GetVehiclesChangedPositionInfoByMultipleFleetsByLang(fleetIds, userId, language, lastChecked);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehiclesChangedPositionInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfo(int fleetId, int userId)
        {
            //DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId,userId);
            DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfo(fleetId, userId);
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
        /// Returns vehicles last known position information by fleet id or list of vehicle ids (comma separated)
        /// or list of box ids (comma separated) or list of vehicle descriptions (comma separated). 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetSelectedVehiclesLastKnownPositionInfo(int userId, string sFleetIDs, string sVehicleIDs, string sBoxIDs, string sVehicleDescriptions, string sOptionalOutputFields)
        {
            DataSet dsResult = fleetVehicles.GetSelectedVehiclesLastKnownPositionInfo(userId, sFleetIDs, sVehicleIDs, sBoxIDs, sVehicleDescriptions, sOptionalOutputFields);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "SelectedVehiclesLastKnownPositionInfo";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns vehicles last known position information by vehicles XMLS. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByVehiclesXML(int userId, string vehiclesXML)
        {
            DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfoByVehiclesXML(userId, vehiclesXML);
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
        /// Returns vehicles last known position information by vehicles XMLS. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByVehiclesXML_New(int userId, string vehiclesXML, string language)
        {
            DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfoByVehiclesXML_New(userId, vehiclesXML, language);
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
		/// Returns fleets information by vehicle id. 
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="userId"></param>
		/// <returns>DataSet [FleetId],[OrganizationName],[FleetName],[Description],[OrganizationId]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetFleetsInfoByVehicleId(Int64 vehicleId, int userId)
		{
            DataSet dsResult = fleetVehicles.GetFleetsInfoByVehicleId(vehicleId, userId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "FleetsInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		/// <summary>
		/// Returns fleets information by vehicle id. 
		/// </summary>
		/// <param name="licensePlate"></param>
		/// <param name="userId"></param>
		/// <returns>DataSet [FleetId],[OrganizationName],[FleetName],[Description],[OrganizationId]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetFleetsInfoByLicensePlate(string licensePlate, int userId)
		{
            Int64 vehicleId = (Int64)vehicleAssignment.GetVehicleAssignmentField("VehicleId", "LicensePlate", licensePlate);
            return GetFleetsInfoByVehicleId(vehicleId, userId);
		}
		/// <summary>
		/// Retrieves array of vehicles in the fleet, in case of empty result returns null.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns> array of vehicles</returns>
		public Int64[] GetListOfVehiclesByFleetId(int fleetId)
		{
			return fleetVehicles.GetListOfVehiclesByFleetId(fleetId);
		}
		/// <summary>
		/// Retrieves array of fleets by vehicle id, in case of empty result returns null.
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns> array of fleets</returns>
        public int[] GetListOfFleetsByVehicleId(Int64 vehicleId, int userId)
		{
            return fleetVehicles.GetListOfFleetsByVehicleId(vehicleId, userId);
		}
		/// <summary>
		/// Returns total number of vehicles in the fleet . 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int GetTotalVehiclesByFleetId(int fleetId)
		{
			int totalVehicles = 0;
			DataSet dsResult = fleetVehicles.GetVehiclesInfoByFleetId(fleetId);
            if (dsResult != null && dsResult.Tables.Count > 0)
			{
				totalVehicles = dsResult.Tables[0].Rows.Count;
			}
			return totalVehicles;
		}
		/// <summary>
		/// Returns total number of fleets by vehicle id. 
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="userId"></param>
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int GetTotalFleetsByVehicleId(Int64 vehicleId, int userId)
		{
			int totalFleets = 0;
            DataSet dsResult = fleetVehicles.GetFleetsInfoByVehicleId(vehicleId, userId);
            if (dsResult != null && dsResult.Tables.Count > 0)
			{
				totalFleets = dsResult.Tables[0].Rows.Count;
			}
			return totalFleets;
		}
		/// <summary>
		/// Gets all vehicles active assignment configuration information for current fleet
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [Description],[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetAllActiveVehiclesCfgInfo(int fleetId)
		{
			DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
			DataSet dsResult = vehicleInfo.GetFleetAllActiveVehiclesCfgInfo(fleetId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "GetFleetAllActiveVehiclesCfgInfo";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
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
            VehicleMaintenaceInfo vehicleMaintenaceInfo = new VehicleMaintenaceInfo(sqlExec);
            return vehicleMaintenaceInfo.GetFleetMaintenanceInfo(fleetId, userId);
        }
        /// <summary>
        /// Returns fleet maintenance history.
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>DataSet [VehicleId],[ServiceDateTime],[ServiceDescription],[ServiceOdo]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFleetMaintenanceHistory(int userId, int fleetId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@fleetId", fleetId);
            sqlParams[1] = new SqlParameter("@userId", userId);
            return this.sqlExec.SPExecuteDataset("sp_FleetGetServicesHistory", sqlParams);
        }

      /// <summary>
      /// Get vehicles short info;
      /// Usage: vehicles drop down list or combobox
      /// </summary>
      /// <param name="fleetId">Fleet ID</param>
      /// <param name="sortByFieldName">Column name to sort a result set</param>
      /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[Description]</returns>
      public DataSet GetVehicles(int fleetId, string sortByFieldName)
      {
         StringBuilder sql = new StringBuilder();
         sql.AppendLine("SELECT LicensePlate, vlfVehicleAssignment.VehicleId AS VehicleId, vlfVehicleAssignment.BoxId AS BoxId, [Description]");
         sql.AppendLine("FROM vlfVehicleAssignment");
         sql.AppendLine("INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId");
         sql.AppendLine("INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId");
         sql.AppendLine("WHERE (vlfFleetVehicles.FleetId = @fleetId)");
         sql.AppendFormat("ORDER BY {0}", sortByFieldName.Trim());
         return fleetVehicles.GetRowsBySql(sql.ToString(), new SqlParameter("@fleetId", fleetId));
      }


      public DataSet GetVehiclesByFleetUser(int userId, int fleetId)
      {
          SqlParameter[] sqlParams = new SqlParameter[2];
          sqlParams[0] = new SqlParameter("@userId", userId);
          sqlParams[1] = new SqlParameter("@fleetId", fleetId);
          return this.sqlExec.SPExecuteDataset("GetVehiclesByFleetUser", sqlParams);
      }


      /// <summary>
      /// Get fleet vehicles
      /// Usage: Man. Console - export to excel file for updating vehicles info
      /// </summary>
      /// <param name="fleetId"></param>
      /// <returns></returns>
      public DataSet GetFleetVehicles(int fleetId)
      {
         StringBuilder sql = new StringBuilder();
         sql.AppendLine("SELECT [Box_ID],[Description],[VIN],[Plate],[Vehicle_Type],[Make],[Model],[Year],[Color],[Field1],[Field2],[Field3],[Field4]");
         sql.AppendLine("FROM [View_FleetVehicles]");
         sql.AppendLine("WHERE [FleetId] = @fleetId");
         return fleetVehicles.GetRowsBySql(sql.ToString(), new SqlParameter("@fleetId", fleetId));
      }

      #endregion

		#region Fleet/User Interfaces
		/// <summary>
		/// Add user to fleet
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public void AddUserToFleet(int fleetId, int userId)
		{
            fleetUsers.AddUserToFleet(fleetId, userId);
		}	
		/// <summary>
		/// Delete exist user from all fleets
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserFromAllFleets(int userId)
		{
			return fleetUsers.DeleteUserFromAllFleets(userId);
		}
		/// <summary>
		/// Delete all users from the fleet and fleet
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteAllUsersFromFleet(int fleetId)
		{
			return fleetUsers.DeleteAllUsersFromFleet(fleetId);
		}
		/// <summary>
		/// Delete exist user from the fleet
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <param name="userId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteUserFromFleet(int fleetId, int userId)
		{
            return fleetUsers.DeleteUserFromFleet(fleetId, userId);
		}
		/// <summary>
		/// Returns fleets information by user id. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetFleetsInfoByUserId(int userId)
		{
			DataSet dsResult = fleetUsers.GetFleetsInfoByUserId(userId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "FleetsInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		/// <summary>
		/// Returns unassigned fleets information to current user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetUnassignedFleetsInfoByUserId(int userId)
		{
			DataSet dsResult = fleetUsers.GetUnassignedFleetsInfoByUserId(userId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "FleetsInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}

		/// <summary>
		/// Returns fleets information by organizationName. 
		/// </summary>
		/// <param name="organizationName"></param>
		/// <returns>DataSet [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetFleetsInfoByOrganizationName(string organizationName)
		{
			DataSet dsResult = fleet.GetFleetsInfoByOrganizationName(organizationName);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "FleetsInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		/// <summary>
		/// Returns users info by fleet id. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],[ContactInfo],[OrganizationId]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetUsersInfoByFleetId(int fleetId)
		{
			DataSet dsResult = fleetUsers.GetUsersInfoByFleetId(fleetId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "UsersInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		/// <summary>
		/// Retieves all users (except HGIAdmin user group) unassigned to the fleet.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],[ContactInfo],[OrganizationId]</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public DataSet GetUnassignedUsersInfoByFleetId(int fleetId)
		{
			DataSet dsResult = fleetUsers.GetUnassignedUsersInfoByFleetId(fleetId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "UsersInformation";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}
		/// <summary>
		/// Returns total number of fleets assigned to this user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int GetTotalFleetsByUserId(int userId)
		{
			int totalFleets = 0;
			DataSet dsResult = fleetUsers.GetFleetsInfoByUserId(userId);
            if (dsResult != null && dsResult.Tables.Count > 0)
			{
				totalFleets = dsResult.Tables[0].Rows.Count;
			}
			return totalFleets;
		}
		/// <summary>
		/// Returns total number of users assigned to this fleet. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int GetTotalUsersByFleetId(int fleetId)
		{
			int totalUsers = 0;
			DataSet dsResult = fleetUsers.GetUsersInfoByFleetId(fleetId);
            if (dsResult != null && dsResult.Tables.Count > 0)
			{
				totalUsers = dsResult.Tables[0].Rows.Count;
			}
			return totalUsers;
		}
		#endregion

		#region Fleet Interfaces

		/// <summary>
		/// Add new fleet.
		/// - Add new fleet
		/// - Assign user to the fleet
		/// </summary>
		/// <param name="fleetName"></param>
		/// <param name="organizationId"></param>
		/// <param name="description"></param>
		/// <returns>int next fleet id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int AddFleet(string fleetName, int organizationId, string description)
		{
			int fleetId = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Add new fleet
                fleetId = fleet.AddFleet(fleetName, organizationId, description);
				// TODO: ??? fleetUsers.AddUserToFleet(fleetId,userId);
				// 8. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
            {
				// 8. Rollback all changes
				fleetId = VLF.CLS.Def.Const.unassignedIntValue;
				sqlExec.RollbackTransaction();
                Util.ProcessDbException("Uanable to add new fleet ", objException);
			}
            catch (DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
            catch (Exception objException)
			{
				// 8. Rollback all changes
				fleetId = VLF.CLS.Def.Const.unassignedIntValue;
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
			return fleetId;
		}

      /// <summary>
      /// Add new fleet.
      /// - Add new fleet
      /// - Assign user to the fleet
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="fleetName"></param>
      /// <param name="organizationId"></param>
      /// <param name="description"></param>
      /// <returns>int next fleet id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public int AddFleet(int userId, string fleetName, int organizationId, string description)
      {
         int fleetId = VLF.CLS.Def.Const.unassignedIntValue;
         try
         {
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);            
            fleetId = fleet.AddFleet(userId, fleetName, organizationId, description);
            sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            fleetId = VLF.CLS.Def.Const.unassignedIntValue;
            sqlExec.RollbackTransaction();
            Util.ProcessDbException("Uanable to add new fleet ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            fleetId = VLF.CLS.Def.Const.unassignedIntValue;
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            fleetId = VLF.CLS.Def.Const.unassignedIntValue;
            sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
         return fleetId;
      }		

		/// <summary>
		/// Update fleet information.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="fleetName"></param>
		/// <param name="organizationId"></param>
		/// <param name="description"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateInfo(int fleetId, string fleetName, int organizationId, string description)
		{
            fleet.UpdateInfo(fleetId, fleetName, organizationId, description);
		}		
		
		/// <summary>
		/// Delete existing fleets
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteFleetByFleetId(int fleetId)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Deletes all fleet vehicle assignments
                rowsAffected += DeleteAllVehiclesFromFleet(fleetId);
				// 3. Deletes all fleet users
                rowsAffected += DeleteAllUsersFromFleet(fleetId);
				// 4. Deletes all emails from fleet
                rowsAffected += DeleteAllEmailsFromFleet(fleetId);
				// 5. Delete fleet
                rowsAffected += fleet.DeleteFleetByFleetId(fleetId);
				// 6. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// Rollback all changes
                rowsAffected = 0;
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Cannot delete the fleet ID " + fleetId.ToString(), objException);
			}
            catch (DASDbConnectionClosed exCnn)
			{
				// Rollback all changes
                rowsAffected = 0;
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
            catch (Exception objException)
			{
				// Rollback all changes
                rowsAffected = 0;
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
			return rowsAffected;
		}

        /// <summary>
        /// Delete existing organization fleets
        /// </summary>
        /// <param name="orgId"></param> 
        /// <returns>Rows Affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteOrganizationFleets(int orgId)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Get all org. fleet ids
                DataSet dsFleets = fleet.GetFleetsByOrganizationId(orgId);

                // 2. Delete all fleets with their dependencies
                if (dsFleets != null && dsFleets.Tables.Count > 0 && dsFleets.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rowFleet in dsFleets.Tables[0].Rows)
                    {
                        rowsAffected += DeleteFleetByFleetId(Convert.ToInt32(rowFleet["FleetId"]));
                    }
                }

                // 3. Delete all fleets if still there are some
                rowsAffected += fleet.DeleteOrganizationFleets(orgId);
            }
            catch (SqlException objException)
            {
                // Rollback all changes
                rowsAffected = 0;
                Util.ProcessDbException("Cannot delete fleets for Organization ID " + orgId.ToString(), objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                // Rollback all changes
                rowsAffected = 0;
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // Rollback all changes
                rowsAffected = 0;
                throw new DASException(objException.Message);
            }
            return rowsAffected;
        }

		/// <summary>
		/// Retrieves Fleet info
		/// </summary>
		/// <returns>DataSet [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]</returns>
		/// <param name="fleetId"></param> 
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetFleetInfoByFleetId(int fleetId)
		{
			DataSet dsResult = fleet.GetFleetInfoByFleetId(fleetId);
            if (dsResult != null && dsResult.Tables[0].Rows.Count > 0)
			{
                dsResult.Tables[0].TableName = "FleetInformation";
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}

		/// <summary>
		/// Check if current fleet is default
		/// </summary>
		/// <returns>true if default,otherwise false</returns>
		/// <param name="fleetId"></param> 
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public bool IsDefaultFleet(int fleetId)
		{
			bool retResult = false;
			DataSet dsResult = fleet.GetFleetInfoByFleetId(fleetId);
            if (dsResult != null &&
				dsResult.Tables.Count > 0 && 
				dsResult.Tables[0].Rows.Count > 0 &&
				dsResult.Tables[0].Rows[0]["FleetName"].ToString().TrimEnd() == VLF.CLS.Def.Const.defaultFleetName)
					retResult = true;
			return retResult;
		}

		/// <summary>
		/// Returns fleet id by fleet name.
		/// </summary>
		/// <param name="organizationId"></param> 
		/// <param name="fleetName"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int GetFleetIdByFleetName(int organizationId, string fleetName)
		{
			return fleet.GetFleetIdByFleetName(organizationId, fleetName);
		}

		/// <summary>
		/// Returns fleet name by fleet Id. 	
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public string GetFleetNameByFleetId(int fleetId)
		{
			return fleet.GetFleetNameByFleetId(fleetId);
		}

        /// <summary>
        /// Get Fleet Services
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="servicesFlag"></param>
        /// <returns></returns>
        public DataTable GetFleetServices(long fleetId, short servicesFlag)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@fleetId", fleetId);
            sqlParams[1] = new SqlParameter("@flag", servicesFlag);
            return this.sqlExec.SPExecuteDataTable("FleetGetAllServices", "VehicleServices", sqlParams);
            //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId", sqlParams).Tables[0];
        }


        /// <summary>
        /// Get Fleet Services
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="servicesFlag"></param>
        /// <returns></returns>
        public DataTable GetFleetServices(long fleetId, short servicesFlag, int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@fleetId", fleetId);
            sqlParams[1] = new SqlParameter("@flag", servicesFlag);
            sqlParams[2] = new SqlParameter("@userId", userId);
            return this.sqlExec.SPExecuteDataTable("FleetGetAllServices_New", "VehicleServices", sqlParams);
            //return vehicleServices.GetRowsByFilter("WHERE VehicleId = @vehicleId", sqlParams).Tables[0];
        }

        /// <summary>
        /// Get Fleet Services
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="operTypeId"></param>
        /// <param name="servicesFlag"></param>
        /// <returns></returns>
        public DataTable GetFleetServices(long fleetId, short operTypeId, short servicesFlag, int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@fleetId", fleetId);
            sqlParams[1] = new SqlParameter("@operTypeId", operTypeId);
            sqlParams[2] = new SqlParameter("@flag", servicesFlag);
            sqlParams[3] = new SqlParameter("@userId", userId);
            return this.sqlExec.SPExecuteDataTable("FleetGetServices_New", "VehicleServices", sqlParams);
        }


		

         /// <summary>
        /// Get Fleet Services History
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public DataSet GetFleetServicesHistory(long fleetId)
        {
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@fleetId", fleetId);
            return this.sqlExec.SPExecuteDataset("FleetGetServicesHistory", sqlParams);
        }


        /// <summary>
        /// Get Fleet Services History
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public DataSet GetFleetServicesHistory(long fleetId, int userId)
        {
            SqlParameter[] sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("@fleetId", fleetId);
            sqlParams[1] = new SqlParameter("@userId", userId);
            return this.sqlExec.SPExecuteDataset("FleetGetServicesHistory_New", sqlParams);
        }


		#endregion

		#region Fleet Emails Interfaces
				/// <summary>
		/// Add email to fleet.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="email"></param>
		/// <param name="timeZone"></param>
		/// <param name="dayLightSaving"></param>
		/// <param name="formatType"></param>
		/// <param name="notify"></param>
		/// <param name="warning"></param>
		/// <param name="critical"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if email alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int AddEmail(int fleetId, string email, string phone, short timeZone, short dayLightSaving,
                            short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance)
		{
            return fleetEmails.AddEmail(fleetId, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance);
		}

        /// <summary>
        /// Save email to fleet, add DriverMessage column 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="Maintenance"></param>
        /// <param name="DriverMessage"></param>
        /// <returns></returns>
        public int SaveEmail(int fleetId, string email, string phone, short timeZone, short dayLightSaving,
                            short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool DriverMessage, bool autosubscription, bool reminder)
        {
            return fleetEmails.SaveEmail(fleetId, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance, DriverMessage, autosubscription, reminder);
        }
		

			/// <summary>
		/// Update fleet email.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="oldEmail"></param>
		/// <param name="newEmail"></param>
		/// <param name="timeZone"></param>
		/// <param name="dayLightSaving"></param>
		/// <param name="formatType"></param>
		/// <param name="notify"></param>
		/// <param name="warning"></param>
		/// <param name="critical"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if fleet does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateEmail(int fleetId,string oldEmail,string newEmail,string phone,short timeZone,
								short dayLightSaving,short formatType,bool notify,
                                bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool autosubscription, bool reminder)
		{
            fleetEmails.UpdateEmail(fleetId, oldEmail, newEmail, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance, autosubscription, reminder);
		}

        /// <summary>
        /// Update fleet email with DriverMessage
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        /// <param name="phone"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="Maintenance"></param>
        /// <param name="DriverMessage"></param>
        public void ModifyEmail(int fleetId, string oldEmail, string newEmail, string phone, short timeZone,
                        short dayLightSaving, short formatType, bool notify,
                        bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool DriverMessage, bool autosubscription, bool reminder)
        {
            fleetEmails.ModifyEmail(fleetId, oldEmail, newEmail, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance, DriverMessage, autosubscription, reminder);
        }		
		

		/// <summary>
		/// Delete existing email from fleet.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="fleetId"></param> 
		/// <param name="email"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if fleet does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int DeleteEmailFromFleet(int fleetId, string email)
		{
            return fleetEmails.DeleteEmailFromFleet(fleetId, email);
		}
		/// <summary>
		/// Delete all emails from fleet.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="fleetId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if fleet does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeleteAllEmailsFromFleet(int fleetId)
		{
			return fleetEmails.DeleteAllEmailsFromFleet(fleetId);
		}
		/// <summary>
		/// Retrieves fleet emails
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>DataSet [FleetId],[FleetMame],[Email],[TimeZone],
		/// [DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetFleetEmails(int fleetId)
		{
			DataSet dsResult = fleetEmails.GetFleetEmails(fleetId);
            if (dsResult != null)
			{
                if (dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "FleetEmails";
				}
				dsResult.DataSetName = "Fleet";
			}
			return dsResult;
		}

      /// <summary>
      ///      Retrieves fleet emails where email != "" and alarmSeverity = true 
      ///      where alarmSeverity could be Notify / Warning / Critical
      /// </summary>
      /// <param name="fleetId"></param> 
      /// <returns>DataSet 
      ///   [Email][FleetId][Phone][TimeZone][DayLightSaving][FormatType]
      ///   [Notify][Warning][Critical][AutoAdjustDayLightSaving]
      /// </returns>
      /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
      public DataSet GetFleetInfo4AlarmSeverity(int vehicleId, int alarmSeverity)
      {
         DataSet dsResult = fleetEmails.GetFleetInfo4AlarmSeverity(vehicleId, alarmSeverity);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "FleetEmails";
            }
            dsResult.DataSetName = "Fleet";
         }
         return dsResult;
      }
		#endregion

      #region Peripherals
       /// <summary>
      /// Returns Box-Peripheral information by fleet id. 
      /// </summary>
      /// <param name="fleetId"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetPeripheralsInfoByFleetId(int fleetId)
      {
         DataSet dsResult = fleetVehicles.GetPeripheralsInfoByFleetId(fleetId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "BoxPeripherals";
            }
            dsResult.DataSetName = "Peripherals";
         }
         return dsResult;
      }

      /// <summary>
      /// Returns vehicles last known position information with peripheral by fleet id. 
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="userId"></param>
      /// <returns>
      /// DataSet 
      /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
      /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
      /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
      /// [IconTypeId],[IconTypeName],[VehicleTypeName],
      /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetVehiclesLastKnownPositionInfoWithPeripheral(int fleetId, int userId)
      {
         DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionInfoWithPeripheral(fleetId, userId);
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
      #endregion

      /// <summary>
      /// Get list of box id
      /// </summary>
      /// <param name="fleetId"></param>
      /// <returns>List of int</returns>
      public List<int> GetFleetBoxes(int fleetId)
      {
         SqlParameter[] sqlParams = new SqlParameter[1];
         sqlParams[0] = new SqlParameter("@fleetId", fleetId);
         string sql = "SELECT dbo.vlfVehicleAssignment.BoxId FROM dbo.vlfFleetVehicles INNER JOIN dbo.vlfVehicleAssignment ON dbo.vlfFleetVehicles.VehicleId = dbo.vlfVehicleAssignment.VehicleId WHERE dbo.vlfFleetVehicles.FleetId = @fleetId ORDER BY dbo.vlfVehicleAssignment.BoxId";
         DataSet dsBoxes = fleet.GetRowsBySql(sql, sqlParams);
         List<int> boxList = new List<int>();
         if (Util.IsDataSetValid(dsBoxes))
         {
                foreach (DataRow drow in dsBoxes.Tables[0].Rows)
            {
               boxList.Add(Convert.ToInt32(drow["BoxId"]));
            }
         }
         return boxList;
      }


          /// <summary>
        /// Get MdtOTA process status
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>status percentage</returns>
        public DataSet GetMdtsByFleetId(int fleetId, Int16 typeId)
        {

            DB.MdtOTA mdt = new DB.MdtOTA(sqlExec);
            DataSet dsResult = mdt.GetMdtsByFleetId(fleetId, typeId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "Mdts";
                }
                dsResult.DataSetName = "FleetMdts";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns fleets information by vehicle id. 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="userId"></param>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetFleetDiagnosticReport(int fleetId, int userId, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet dsResult = fleetVehicles.GetFleetDiagnosticReport(fleetId, userId, fromDateTime, toDateTime);
            return dsResult;
        }


        /// <summary>
        /// Get the list of organization ID
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>
        /// DataSet
        /// [SkillId], [SkillName]
        /// </returns>
        public DataSet GetOrganizationSkillsList(int organizationId)
        {
            DataSet dsResult = fleetVehicles.GetOrganizationSkills(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationSkills";
                }
                dsResult.DataSetName = "Skill";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <param name="language"></param>
        /// <param name="skillId"></param>
        /// <param name="vehicleTypeId"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionDriverInfoBySkill(int fleetId, int userId, string language, int skillId = 0, int vehicleTypeId = -1)
        {
            DataSet dsResult = fleetVehicles.VlfFindVehicleDrivesInfo(fleetId, userId, language, skillId, vehicleTypeId);
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


        // SALMAN Feb 19, 2013
        public DataSet GetVehiclesLastKnownPositionCustomInfo(int userId, string filterHierarchy, string filterFleet, string filterUDF, string filterCustomOutputFields)//filterUDF1->UserName (abc@bsm.com)
        {
            DataSet dsResult = fleetVehicles.GetVehiclesLastKnownPositionCustomInfo(userId, filterHierarchy, filterFleet, filterUDF, filterCustomOutputFields);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "VehiclesLastKnownPositionCustomInformation";
                }
                dsResult.DataSetName = "HierarchyFleet";
            }
            return dsResult;
        }


        // SALMAN July 19, 2013
        /// <summary>
        /// Returns devices last known position information by custom selection (boxId, vehicleId, or vehicleDescription) criteria. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="selectionCriteria"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDevicesLastKnownPosition(int userId, string selectionCriteria)
        {
            DataSet dsResult = fleetVehicles.GetDevicesLastKnownPosition(selectionCriteria, userId);
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

        // SALMAN May 29, 2014

        /// <summary>
        /// Returns devices last known position information for PreTemplate.  
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <param name="lastCommunicatedDateTime"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDeviceLastKnownPositionPreTemplatedInfo_SP(int userId, int fleetId, DateTime lastCommunicatedDateTime)
        {
            DataSet dsResult = fleetVehicles.GetDeviceLastKnownPositionPreTemplatedInfo_SP(userId, fleetId, lastCommunicatedDateTime);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetDeviceLastKnownPositionPreTemplatedInfo_SP";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }


       
        /// <summary>
        /// Add All new vehicle to fleet.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fleetId"></param>
        /// <returns>Int</returns>
        /// <exception cref="DASAppWrongResultException">Thrown if unable to retrieve organization info for current fleet.</exception>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if fleet and vehicle do no assigned to same organization..</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int AddAllVehicleToFleet(int fleetId, string result, int organizationId)
        {

            return fleetVehicles.AddAllVehicleToFleet(fleetId, result, organizationId);


        }
       
   }		
}
