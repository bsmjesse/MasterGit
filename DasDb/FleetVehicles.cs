using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using System.Text ;
namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfFleetVehicles table.
	/// </summary>
	public class FleetVehicles : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public FleetVehicles(SQLExecuter sqlExec) : base ("vlfFleetVehicles",sqlExec)
		{
		}
		/// <summary>
		/// Add new vehicle to fleet.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="vehicleId"></param>
		/// <returns>Int64</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle alredy exists in the fleet.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddVehicleToFleet(int fleetId,Int64 vehicleId)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( FleetId, VehicleId) VALUES ( {0}, {1})", fleetId, vehicleId);
			try
			{
                if (sqlExec.RequiredTransaction())
                {
                    //ttach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new 'FleetId:" + fleetId + " VehicleId:" + vehicleId + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new 'FleetId:" + fleetId + " VehicleId:" + vehicleId + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new 'FleetId:" + fleetId + " VehicleId:" + vehicleId + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The vehicle already exists in the fleet.");
			}	
			return rowsAffected;
		}		
		/// <summary>
		/// Purge existing vehicle from all fleets.
		/// </summary>
		/// <remarks>
		/// Cannot delete from default fleet.
		/// </remarks>
		/// <param name="vehicleId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int PurgeVehicleFromAllFleets(Int64 vehicleId)
		{
			//return DeleteRowsByIntField("VehicleId",vehicleId, "vehicle id");		
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfFleetVehicles WHERE vlfFleetVehicles.VehicleId=" + vehicleId;
			//string sql = "DELETE FROM vlfFleetVehicles WHERE VehicleId=" + vehicleId;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete vehicle id=" + vehicleId + " from all fleets.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete vehicle id=" + vehicleId + " from all fleets.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}

		/// <summary>
		/// Delete existing vehicle from the fleets.
		/// </summary>
		/// <remarks>
		/// Cannot delete from default fleet.
		/// </remarks>
		/// <param name="fleetId"></param> 
		/// <param name="vehicleId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteVehicleFromFleet(int fleetId,Int64 vehicleId)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			//string sql = "DELETE FROM " + tableName + 
			//			" WHERE FleetId=" + fleetId + 
			//			" AND VehicleId=" + vehicleId;
			string sql = "DELETE FROM vlfFleetVehicles"+
				" WHERE vlfFleetVehicles.VehicleId=" + vehicleId +
				" AND vlfFleetVehicles.FleetId=" + fleetId +
				" AND vlfFleetVehicles.FleetId NOT IN (SELECT vlfFleet.FleetId FROM vlfFleet WHERE vlfFleet.FleetName = '" + VLF.CLS.Def.Const.defaultFleetName + "')";
			
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete vehicle id:" + vehicleId + " from fleet:" + fleetId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete vehicle id:" + vehicleId + " from fleet:" + fleetId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Delete existing vehicle assignments from the fleet.
		/// </summary>
		/// <param name="fleetId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllVehiclesFromFleet(int fleetId)
		{
			return DeleteRowsByIntField("FleetId",fleetId, "fleet id");		
		}
		/// <summary>
		/// Retrieves array of fleets by vehicle id, in case of empty result returns null.
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="userId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns> array of fleets</returns>
		public int[] GetListOfFleetsByVehicleId(Int64 vehicleId,int userId)
		{
			int[] resultList = null;
			DataSet sqlDataSet = null;
			int index = 0;
			//Prepares SQL statement
			try
			{
				sqlDataSet = GetFleetsInfoByVehicleId(vehicleId,userId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of fleets by vehicle id=" + vehicleId + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of fleets by vehicle id=" + vehicleId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new int[sqlDataSet.Tables[0].Rows.Count];
				//Retrieves info from Table[0].[0][0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList[index++] = Convert.ToInt32(currRow["FleetId"]);
				}
			}
			return resultList;
		}
		/// <summary>
		/// Retrieves array of vehicles in the fleet, in case of empty result returns null.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns> array of vehicles</returns>
		public Int64[] GetListOfVehiclesByFleetId(int fleetId)
		{
			Int64[] resultList = null;
			DataSet sqlDataSet = null;
			int index = 0;
			//Prepares SQL statement
			try
			{
				sqlDataSet = GetVehiclesInfoByFleetId(fleetId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of vehicles by fleet id=" + fleetId + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of vehicles by fleet id=" + fleetId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new Int64[sqlDataSet.Tables[0].Rows.Count];
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList[index++] = Convert.ToInt64(currRow["VehicleId"]);
				}
			}
			return resultList;
		}

        
        /// <summary>
        ///       Returns ManagerName by VehicleId
        /// </summary>        
        /// <param name="featureMask">VehicleId </param>
        /// <returns>XML [ManagerName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetManagerNameByVehicleId(int vehicleId)
        {
            DataSet resultDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT DISTINCT ManagerName from vlfvehicleinfo where VehicleId = " + vehicleId ;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve manager name  for vehicle id " + vehicleId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve manager name  for vehicle id " + vehicleId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
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
             int rowsAffected = 0;
            try
            {
                //Prepares SQL statement
                string sql = "update vlfvehicleinfo set ManagerName = '" + ManagerName + "' where VehicleId in (" + AssignesVehiclesList + ")";
                
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to set manager name  for vehicle ids " + AssignesVehiclesList + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to set manager name  for vehicle ids " + AssignesVehiclesList + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return rowsAffected;
        }

		/// <summary>
		/// Returns vehicle information by fleet id. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehiclesInfoByFleetId(int fleetId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT LicensePlate,BoxId,vlfVehicleAssignment.VehicleId,VinNum,MakeName,ModelName,VehicleTypeName,StateProvince,ModelYear,Color,vlfVehicleInfo.Description," + tableName + ".FleetId,vlfFleet.FleetName  " + 
					" FROM vlfVehicleAssignment,vlfVehicleInfo,vlfMakeModel,vlfMake,vlfModel,vlfVehicleType,vlfFleet, " + tableName +
					" WHERE ("  + tableName + ".FleetId=" + fleetId  + ")" +
					" AND (" + tableName + ".vehicleId=vlfVehicleAssignment.VehicleId)" +
					" AND (vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId)" +
					" AND (vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId)" +
					" AND (vlfMakeModel.MakeId=vlfMake.MakeId)" +
					" AND (vlfMakeModel.ModelId=vlfModel.ModelId)" +
					" AND (vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId)" +
                    " AND (" + tableName + ".FleetId=vlfFleet.FleetId)" +
                    " ORDER BY vlfVehicleInfo.Description";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		
		/// <summary>
		/// Returns vehicle information by fleet ids. 
		/// </summary>
		/// <param name="fleetId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehiclesInfoByMultipleFleetIds(string fleetIds)
		{
			DataSet resultDataSet = null;
			try
			{
				string sql = "sp_GetVehiclesInfoByMultipleFleetIds";
				SqlParameter[] sqlParams = new SqlParameter[1];
				sqlParams[0] = new SqlParameter("@fleetIds", fleetIds);
				//Executes SQL statement
				resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);				
			}
			catch (SqlException objException) 
			{
                string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetIds + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
                string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetIds + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}




        /// <summary>
        /// Returns vehicle information 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesInfoByFleetId_New(int fleetId)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                resultDataSet = sqlExec.SPExecuteDataset("GetVehiclesInfoByFleetId");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesInfoByFleetId by fleetId " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve GetVehiclesInfoByFleetId  by fleetId " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }





        /// <summary>
        /// Returns vehicle peripherals by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesPeripheralInfoByFleetId(int fleetId)
        {
            DataSet resultDataSet = null;
            sqlExec.ClearCommandParameters();
            try
            {
                sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
                resultDataSet = sqlExec.SPExecuteDataset("GetVehiclesPeripheralInfoByFleetId");
                return resultDataSet;

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve VehiclesPeripherals by fleetId " + fleetId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve VehiclesPeripherals  by fleetId " + fleetId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
		
		
		/// <summary>
		/// Returns vehicles last known position information by fleet id. 
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
		/// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
		/// [FwTypeId],[Dormant],[DormantDateTime]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetVehiclesLastKnownPositionInfo(int fleetId,int userId)
		{
			DataSet resultDataSet = null;
			try
			{
                string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int DECLARE @Unit real DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
					" SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
					" IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
					" IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving"+
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0"+


					" SELECT DISTINCT vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.VehicleId,vlfBox.BoxId,"+
					"CASE WHEN vlfBox.LastValidDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastValidDateTime) END AS OriginDateTime,"+
					"ISNULL(vlfBox.LastLatitude,0) AS Latitude,"+
					"ISNULL(vlfBox.LastLongitude,0) AS Longitude,"+
					"CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit,1) END AS Speed,"+
					"ISNULL(vlfBox.LastHeading,0) AS Heading,"+
					"ISNULL(vlfBox.LastSensorMask,0) AS SensorMask,"+
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +


					"ISNULL(vlfVehicleInfo.[Description],'') AS [Description],"+
					"CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed,"+
					"CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime,"+
					"GeoFenceEnabled,vlfVehicleInfo.IconTypeId,IconTypeName,vlfVehicleType.VehicleTypeId,"+
					"ISNULL(vlfVehicleType.VehicleTypeName,'') AS VehicleTypeName,"+
                    "CASE WHEN vlfBox.LastStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastStatusDateTime) END AS LastStatusDateTime," +
					"ISNULL(LastStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSensor,"+
					"ISNULL(LastStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSpeed,"+
                    "CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime," +
					"ISNULL(PrevStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSensor,"+
					"ISNULL(PrevStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSpeed," +
					"vlfFirmware.FwTypeId,"+
					"ISNULL(Dormant,0) AS Dormant," +
                    "CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.DormantDateTime) END AS DormantDateTime" +
               " FROM vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfIconType ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId WHERE vlfFleetVehicles.FleetId=" + fleetId +
					" ORDER BY vlfVehicleInfo.[Description]";

				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

        /// <summary>
        /// Returns vehicles last known position information by fleet id or list of vehicle ids (comma separated)
        /// or list of box ids (comma separated) or list of vehicle descriptions (comma separated). 
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
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetSelectedVehiclesLastKnownPositionInfo(int userId, string sFleetIDs, string sVehicleIDs, string sBoxIDs, string sVehicleDescriptions, string sOptionalOutputFields)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = "usp_SelectedVehiclesLastPositon_Get";
                SqlParameter[] sqlParams = new SqlParameter[6];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@FleetIDs", sFleetIDs);
                sqlParams[2] = new SqlParameter("@VehicleIDs", sVehicleIDs);
                sqlParams[3] = new SqlParameter("@BoxIDs", sBoxIDs);
                sqlParams[4] = new SqlParameter("@VehicleDescriptions", sVehicleDescriptions);
                sqlParams[5] = new SqlParameter("@OptionalOutputFields", sOptionalOutputFields);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "SqlException in GetSelectedVehiclesLastKnownPositionInfo: userId=" + userId + ", FleetIDs=" + sFleetIDs + ", VehicleIDs=" + sVehicleIDs + ", BoxIDs=" + sBoxIDs + ", VehicleDescriptions=" + sVehicleDescriptions + ", OptionalOutputFields=" + sOptionalOutputFields + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Exception in GetSelectedVehiclesLastKnownPositionInfo: userId=" + userId + ", FleetIDs=" + sFleetIDs + ", VehicleIDs=" + sVehicleIDs + ", BoxIDs=" + sBoxIDs + ", VehicleDescriptions=" + sVehicleDescriptions + ", OptionalOutputFields=" + sOptionalOutputFields + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        /// <summary>
        /// Returns vehicles last known position information by XML vehicle list. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehiclesXML"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByVehiclesXML(int userId, string vehiclesXML)
        {
            DataSet resultDataSet = null;
            try
            {

                string sql = "sp_GetVehiclesLastKnownPositionInfoByVehiclesXML";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@doc",  vehiclesXML);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by VehiclesXML " + vehiclesXML + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by VehiclesXML " + vehiclesXML + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }



        /// <summary>
        /// Returns vehicles last known position information by XML vehicle list. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehiclesXML"></param>
        /// <param name="language"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByVehiclesXML_New(int userId, string vehiclesXML,string language)
        {
            DataSet resultDataSet = null;
            try
            {

                string sql = "sp_GetVehiclesLastKnownPositionInfoByVehiclesXML_new";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@doc", vehiclesXML);
                sqlParams[2] = new SqlParameter("@language", language);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by VehiclesXML " + vehiclesXML + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by VehiclesXML " + vehiclesXML + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
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
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfo_SP_NewTZ(int fleetId, int userId, string language)
        {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
            DataSet resultDataSet = null;
            try
            {
                string sql = "sp_GetVehiclesLastKnownPosition_NewTimeZone";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@language", language);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
      /// Returns vehicles last known position information by fleet id. 
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
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfo_SP(int fleetId, int userId, string language)
      {
         string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
         DataSet resultDataSet = null;
         try
         {
             string sql = "sp_GetVehiclesLastKnownPosition";
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@fleetId", fleetId);
            sqlParams[2] = new SqlParameter("@language", language);
            //Executes SQL statement
            resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
        /// Returns vehicles last known position information by multiple fleet id. 
        /// </summary>
        /// <param name="fleetIds"></param>
        /// <param name="userId"></param>
        /// <returns>
        /// DataSet 
        /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
        /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByMultipleFleets_SP_NewTZ(string fleetIds, int userId, string language)
        {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetIds + ". ";
            DataSet resultDataSet = null;
            try
            {
                string sql = "sp_GetVehiclesLastKnownPositionByMultipleFleets_NewTimeZone";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetIds", fleetIds);
                sqlParams[2] = new SqlParameter("@language", language);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
      /// Returns vehicles last known position information by multiple fleet id. 
      /// </summary>
      /// <param name="fleetIds"></param>
      /// <param name="userId"></param>
      /// <returns>
      /// DataSet 
      /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
      /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
      /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
      /// [IconTypeId],[IconTypeName],[VehicleTypeName],
      /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesLastKnownPositionInfoByMultipleFleets_SP(string fleetIds, int userId, string language)
      {
         string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetIds + ". ";
         DataSet resultDataSet = null;
         try
         {
             string sql = "sp_GetVehiclesLastKnownPositionByMultipleFleets";
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@fleetIds", fleetIds);
            sqlParams[2] = new SqlParameter("@language", language);
            //Executes SQL statement
            resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
        /// Returns vehicles last known position information by fleet id. 
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
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetVehiclesChangedPositionInfoByLang_NewTZ(int fleetId, int userId, string language, DateTime lastchecked)
        {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
            DataSet resultDataSet = null;
            try
            {
                string sql = "usp_GetVehiclesChangedPosition_NewTimeZone";
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@language", language);
                sqlParams[3] = new SqlParameter("@lastchecked", SqlDbType.DateTime);
                sqlParams[3].Value = lastchecked;
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
      /// Returns vehicles last known position information by fleet id. 
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
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehiclesChangedPositionInfoByLang(int fleetId, int userId, string language,DateTime lastchecked)
      {
         string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
         DataSet resultDataSet = null;
         try
         {
             string sql = "usp_GetVehiclesChangedPosition";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@fleetId", fleetId);
            sqlParams[2] = new SqlParameter("@language", language);
            sqlParams[3] = new SqlParameter("@lastchecked",SqlDbType.DateTime);
            sqlParams[3].Value = lastchecked;
            //Executes SQL statement
            resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
      /// Returns vehicles last known position information by multiple fleet id. 
      /// </summary>
      /// <param name="fleetIds"></param>
      /// <param name="userId"></param>
      /// <returns>
      /// DataSet 
      /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
      /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
      /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
      /// [IconTypeId],[IconTypeName],[VehicleTypeName],
      /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehiclesChangedPositionInfoByMultipleFleetsByLang_NewTZ(string fleetIds, int userId, string language, DateTime lastchecked)
      {
          string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetIds + ". ";
          DataSet resultDataSet = null;
          try
          {
              string sql = "usp_GetVehiclesChangedPositionByMultipleFleets_NewTimeZone";
              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@userId", userId);
              sqlParams[1] = new SqlParameter("@fleetIds", fleetIds);
              sqlParams[2] = new SqlParameter("@language", language);
              sqlParams[3] = new SqlParameter("@lastchecked", SqlDbType.DateTime);
              sqlParams[3].Value = lastchecked;
              //Executes SQL statement
              resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
      /// Returns vehicles last known position information by multiple fleet id. 
      /// </summary>
      /// <param name="fleetIds"></param>
      /// <param name="userId"></param>
      /// <returns>
      /// DataSet 
      /// [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
      /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
      /// [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
      /// [IconTypeId],[IconTypeName],[VehicleTypeName],
      /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehiclesChangedPositionInfoByMultipleFleetsByLang(string fleetIds, int userId, string language,DateTime lastchecked)
      {
         string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetIds + ". ";
         DataSet resultDataSet = null;
         try
         {
             string sql = "usp_GetVehiclesChangedPositionByMultipleFleets";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@fleetIds", fleetIds);
            sqlParams[2] = new SqlParameter("@language", language);
            sqlParams[3] = new SqlParameter("@lastchecked",SqlDbType.DateTime);
            sqlParams[3].Value = lastchecked;
            //Executes SQL statement
            resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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

      // <summary>
      /// Returns vehicles last known position information by fleet id. 
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
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <comment>
      ///      this is a join of in memory tables
      /// </comment>
      public DataSet GetVehiclesLastKnownPositionInfo_SP(int fleetId, int userId)
      {
          string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
          DataSet resultDataSet = null;
          try
          {
              string sql = "sp_GetVehiclesLastKnownPosition";
              SqlParameter[] sqlParams = new SqlParameter[3];
              sqlParams[0] = new SqlParameter("@userId", userId);
              sqlParams[1] = new SqlParameter("@fleetId", fleetId);
              sqlParams[2] = new SqlParameter("@language", "en");
              //Executes SQL statement
              resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
      /// 
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="fromDateTime"></param>
      /// <param name="toDateTime"></param>
      /// <returns>
      ///       <boxId> <OriginDateTime> <BoxMsgInTypeId> <CustomProp><VehicleID><VinNum>
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>       
      public DataSet GetIdlingDurationForFleetId(int fleetId, int userId, DateTime fromDateTime, DateTime toDateTime)
      {
         DataSet dsResult = null;
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
         sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
         sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);

         try
         {
            dsResult = sqlExec.SPExecuteDataset("sp_GetIdlingDurationForFleet3");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("GetIdlingDurationForFleetId failed ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("GetIdlingDurationForFleetId failed " + objException.Message);
         }


         return dsResult;
      }
	  
	  /// <summary>
      /// 
      /// </summary>
      /// <param name="fleetIds"></param>
      /// <param name="fromDateTime"></param>
      /// <param name="toDateTime"></param>
      /// <returns>
      ///       <boxId> <OriginDateTime> <BoxMsgInTypeId> <CustomProp><VehicleID><VinNum>
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>       
      public DataSet GetIdlingDurationForMultipleFleetIds(string fleetIds, int userId, DateTime fromDateTime, DateTime toDateTime)
      {
         DataSet dsResult = null;
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@fleetIds", SqlDbType.Int, fleetIds);
         sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
         sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
         sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);

         try
         {
            dsResult = sqlExec.SPExecuteDataset("sp_GetIdlingDurationForMultipleFleets3");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("GetIdlingDurationForFleetId failed ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("GetIdlingDurationForFleetId failed " + objException.Message);
         }


         return dsResult;
      }
	  
		/// <summary>
		/// Returns fleets information by vehicle id. 
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="userId"></param>
		/// <returns>DataSet [FleetId],[OrganizationName],[FleetName],[Description],[OrganizationId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetsInfoByVehicleId(Int64 vehicleId,int userId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				/*string sql = "SELECT DISTINCT vlfFleet.FleetId,vlfOrganization.OrganizationName,vlfFleet.FleetName, vlfFleet.Description, vlfOrganization.OrganizationId"+
					" FROM vlfFleet,vlfFleetVehicles,vlfOrganization";
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
					sql += ",vlfFleetUsers";
				sql += " WHERE vlfFleetVehicles.VehicleId=" + vehicleId +
					" AND vlfFleet.FleetId=vlfFleetVehicles.FleetId"+
					" AND vlfFleet.OrganizationId=vlfOrganization.OrganizationId";
				if(userId != VLF.CLS.Def.Const.unassignedIntValue)
				{
					sql += " AND vlfFleet.FleetId=vlfFleetUsers.FleetId";
					sql += " AND vlfFleetUsers.UserId=" + userId;
				}
				sql += " ORDER BY vlfFleet.FleetName";*/
				if(userId == VLF.CLS.Def.Const.unassignedIntValue)
					userId = 0;
				
				string sql = "sp_GetFleetsInfoByVehicleId";
				
				SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@VehicleId", vehicleId);
                sqlParams[1] = new SqlParameter("@UserId",  userId);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
				
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve fleets info by vehicle id " + vehicleId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve fleets info by vehicle id " + vehicleId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Retrieve unassigned to any fleet vehicles for current organization.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnassingToFleetsVehiclesInfo(int organizationId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT LicensePlate,BoxId,vlfFleetVehicles.VehicleId,"+
					"vlfVehicleInfo.VinNum,MakeName,ModelName,VehicleTypeName,"+
					"StateProvince,ModelYear,Color,vlfVehicleInfo.Description,CostPerMile"+
					" FROM vlfFleetVehicles INNER JOIN vlfVehicleAssignment ON vlfFleetVehicles.VehicleId=vlfVehicleAssignment.VehicleId"+
					" INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
					" INNER JOIN vlfMakeModel ON vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId"+
					" INNER JOIN vlfMake ON vlfMakeModel.MakeId=vlfMake.MakeId"+
					" INNER JOIN vlfModel ON vlfMakeModel.ModelId=vlfModel.ModelId"+
					" INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId"+
					" INNER JOIN vlfFleet ON vlfFleetVehicles.FleetId=vlfFleet.FleetId"+
					" WHERE vlfFleet.OrganizationId=" + organizationId +
					" AND vlfVehicleInfo.VehicleId NOT IN"+
					" (SELECT DISTINCT vlfFleetVehicles.VehicleId"+
					" FROM vlfFleetVehicles INNER JOIN vlfFleet ON vlfFleetVehicles.FleetId=vlfFleet.FleetId"+
					" WHERE vlfFleet.OrganizationId=" + organizationId + 
					" AND vlfFleet.FleetName<>'" + VLF.CLS.Def.Const.defaultFleetName + "')" +
					" ORDER BY vlfVehicleInfo.Description";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve unassigned to any fleet vehicles. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve unassigned to any fleet vehicles. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Retrieve all active vehicles info that unassigned to current fleet for current organization.
		/// </summary>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
		/// <param name="organizationId"></param>
		/// <param name="fleetId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnassingToFleetVehiclesInfo(int organizationId, int fleetId)
		{
			DataSet resultDataSet = null;
			try
			{
                string sql = "SELECT DISTINCT vlfVehicleAssignment.LicensePlate,vlfVehicleAssignment.BoxId,"
                            + " vlfVehicleAssignment.VehicleId,vlfVehicleInfo.VinNum,vlfMake.MakeName,vlfModel.ModelName,"
                            + " vlfVehicleType.VehicleTypeName,vlfVehicleInfo.StateProvince,vlfVehicleInfo.ModelYear,"
                            + " vlfVehicleInfo.Color,vlfVehicleInfo.Description,vlfVehicleInfo.CostPerMile "
                            + " FROM vlfVehicleAssignment "
                            + " INNER JOIN vlfBox WITH (nolock) ON vlfVehicleAssignment.BoxId = vlfBox.BoxId "
                            + " INNER JOIN vlfVehicleInfo WITH (nolock) ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId "
                            + " INNER JOIN vlfMakeModel ON vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId "
                            + " INNER JOIN vlfMake ON vlfMakeModel.MakeId=vlfMake.MakeId "
                            + " INNER JOIN vlfModel ON vlfMakeModel.ModelId=vlfModel.ModelId "
                            + " INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId "
                            + " WHERE vlfBox.OrganizationId= " + organizationId
                            + " AND vlfVehicleAssignment.VehicleId NOT IN ( "
                            + " SELECT DISTINCT vlfFleetVehicles.VehicleId "
                            + " FROM vlfFleetVehicles WITH (nolock) INNER JOIN vlfFleet ON vlfFleetVehicles.FleetId=vlfFleet.FleetId "
                            + " WHERE vlfFleet.OrganizationId= " + organizationId
                            + " AND vlfFleetVehicles.FleetId= " + fleetId
                            + " ) ORDER BY vlfVehicleInfo.Description ";

                /// Query Replace By Wisam: if the vehicle is not assigned to any fleet,
                /// there is no way to see it using this query
                /// 
				///string sql = "SELECT DISTINCT LicensePlate,BoxId,vlfFleetVehicles.VehicleId,"+
				///	"vlfVehicleInfo.VinNum,MakeName,ModelName,VehicleTypeName,"+
				///	"StateProvince,ModelYear,Color,vlfVehicleInfo.Description,CostPerMile"+
				///	" FROM vlfFleetVehicles INNER JOIN vlfVehicleAssignment ON vlfFleetVehicles.VehicleId=vlfVehicleAssignment.VehicleId"+
				///	" INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId=vlfVehicleInfo.VehicleId"+
				///	" INNER JOIN vlfMakeModel ON vlfVehicleInfo.MakeModelId=vlfMakeModel.MakeModelId"+
				///	" INNER JOIN vlfMake ON vlfMakeModel.MakeId=vlfMake.MakeId"+
				///	" INNER JOIN vlfModel ON vlfMakeModel.ModelId=vlfModel.ModelId"+
				///	" INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId=vlfVehicleType.VehicleTypeId"+
				///	" INNER JOIN vlfFleet ON vlfFleetVehicles.FleetId=vlfFleet.FleetId"+
				///	" WHERE vlfFleet.OrganizationId=" + organizationId +
				///	" AND vlfVehicleInfo.VehicleId NOT IN"+
				///	" (SELECT DISTINCT vlfFleetVehicles.VehicleId"+
				///	" FROM vlfFleetVehicles INNER JOIN vlfFleet ON vlfFleetVehicles.FleetId=vlfFleet.FleetId"+
				///	" WHERE vlfFleet.OrganizationId=" + organizationId + 
				///	" AND vlfFleetVehicles.FleetId=" + fleetId + ")" +
				///	" ORDER BY vlfVehicleInfo.Description";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve unassigned to organization=" + organizationId + " fleet=" + fleetId + " vehicles info. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve unassigned to organization=" + organizationId + " fleet=" + fleetId + " vehicles info. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Retrieves array of vehicles in the fleet, in case of empty result returns null.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="vehicleId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns> true if vehicle exist, otherwise false</returns>
		public bool IsVehicleExistInFleet(int fleetId,Int64 vehicleId)
		{
			bool isExist = false;
			//Prepares SQL statement
			try
			{
				string sql = "SELECT COUNT(*) FROM vlfFleetVehicles"+
					" WHERE FleetId=" + fleetId + " AND VehicleId=" + vehicleId;
				if(Convert.ToInt16(sqlExec.SQLExecuteScalar(sql)) > 0)
					isExist = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve fleet/vehicle assignments for fleet=" + fleetId + "  vehicleId=" + vehicleId + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve fleet/vehicle assignments for fleet=" + fleetId + "  vehicleId=" + vehicleId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return isExist;
		}


      /// <summary>
      /// Returns Box-Peripheral information by fleet id. 
      /// </summary>
      /// <param name="fleetId"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetPeripheralsInfoByFleetId(int fleetId)
      {
         DataSet resultDataSet = null;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT  dbo.vlfPeripheralBoxAssigment.BoxId, dbo.vlfPeripheralBoxAssigment.PeripheralId, dbo.vlfPeripheralBoxAssigment.TypeId, dbo.vlfPeripheralTypes.TypeName " +
                       " FROM         dbo.vlfFleetVehicles INNER JOIN " +
                      " dbo.vlfVehicleAssignment ON dbo.vlfFleetVehicles.VehicleId = dbo.vlfVehicleAssignment.VehicleId INNER JOIN " +
                      " dbo.vlfPeripheralBoxAssigment ON dbo.vlfVehicleAssignment.BoxId = dbo.vlfPeripheralBoxAssigment.BoxId INNER JOIN " +
                      " dbo.vlfPeripheralTypes ON dbo.vlfPeripheralBoxAssigment.TypeId = dbo.vlfPeripheralTypes.TypeId " +
                " WHERE dbo.vlfFleetVehicles.FleetId=" + fleetId;
                
            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

      /// <summary>
      /// Returns vehicles last known position information with Peripheral  by fleet id . 
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
      /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
      /// [FwTypeId],[Dormant],[DormantDateTime]
      /// </returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetVehiclesLastKnownPositionInfoWithPeripheral(int fleetId, int userId)
      {
         DataSet resultDataSet = null;
         try
         {
            string sql = " DECLARE @ResolveLandmark int DECLARE @Timezone int DECLARE @Unit real DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
           " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
           " IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
           " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +


           " SELECT DISTINCT vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.VehicleId,vlfBox.BoxId," +
           "CASE WHEN vlfBox.LastValidDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastValidDateTime) END AS OriginDateTime," +
           "ISNULL(vlfBox.LastLatitude,0) AS Latitude," +
           "ISNULL(vlfBox.LastLongitude,0) AS Longitude," +
           "CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit,1) END AS Speed," +
           "ISNULL(vlfBox.LastHeading,0) AS Heading," +
           "ISNULL(vlfBox.LastSensorMask,0) AS SensorMask," +
                "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +


           "ISNULL(vlfVehicleInfo.[Description],'') AS [Description]," +
           "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
           "CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime," +
           "GeoFenceEnabled,vlfVehicleInfo.IconTypeId,IconTypeName,vlfVehicleType.VehicleTypeId," +
           "ISNULL(vlfVehicleType.VehicleTypeName,'') AS VehicleTypeName," +
                "CASE WHEN vlfBox.LastStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastStatusDateTime) END AS LastStatusDateTime," +
           "ISNULL(LastStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSensor," +
           "ISNULL(LastStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSpeed," +
                "CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime," +
           "ISNULL(PrevStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSensor," +
           "ISNULL(PrevStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSpeed," +
           "vlfFirmware.FwTypeId," +
           "ISNULL(Dormant,0) AS Dormant," +
           " CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.DormantDateTime) END AS DormantDateTime," +
           " CASE WHEN PeripheralId IS NOT NULL THEN 1 ELSE 0 END  PeripheralFlag "+
           " FROM vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfIconType ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId LEFT JOIN  dbo.vlfPeripheralBoxAssigment ON dbo.vlfBox.BoxId = dbo.vlfPeripheralBoxAssigment.BoxId WHERE vlfFleetVehicles.FleetId=" + fleetId +
           " ORDER BY vlfVehicleInfo.[Description]";

            //Executes SQL statement
            resultDataSet = sqlExec.SQLExecuteDataset(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve vehicles info by fleet id " + fleetId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return resultDataSet;
      }

        /// <summary>
        ///    Returns complete vehicle information by fleet id
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public DataSet GetVehiclesFullInfobyFleetId(int fleetId)
        {
            DataSet resultDataSet = null;
            try
            {

                string sql = "GetVehiclesInfoInFleet";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info (GetVehiclesFullInfobyFleetId) by fleetId " + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info (GetVehiclesFullInfobyFleetId) by fleetId " + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>       
        public DataSet GetFleetDiagnosticReport(int fleetId, int userId, DateTime fromDateTime, DateTime toDateTime)
        {
            DataSet dsResult = null;
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, fromDateTime);
            sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, toDateTime);
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);

            try
            {
                dsResult = sqlExec.SPExecuteDataset("sp_FleetDiagnosticReport");
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("GetFleetDiagnosticReport failed ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("GetFleetDiagnosticReport failed " + objException.Message);
            }


            return dsResult;
        }


        public DataSet GetVehiclesLastKnownPositionCustomInfo(int userId, string filterHierarchy, string filterFleet, string filterUDF, string filterCustomOutputFields)//filterUDF1->UserName (abc@bsm.com)
        {
            DataSet resultDataSet = null;
            //vlfVehicleInfo.VehicleId
            string defaultFields = "SELECT DISTINCT vlfVehicleInfo.Description AS VehicleId, ISNULL(vlfBox.LastLatitude,0) AS Latitude, ISNULL(vlfBox.LastLongitude,0) AS Longitude, CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime";
            //if (!string.IsNullOrEmpty(filterCustomOutputFields))
            //    filterCustomOutputFields = string.Format("{0}", filterCustomOutputFields);

            try
            {
                string sqlOrg = (userId == 11296 ? " DECLARE @OrganizationId int SET @OrganizationId=999956" : string.Format(" DECLARE @OrganizationId int SELECT @OrganizationId=OrganizationId FROM vlfUser WHERE userid={0}", userId));
                string sqlHeader = " DECLARE @ResolveLandmark int DECLARE @Timezone int DECLARE @Unit real DECLARE @DayLightSaving int SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone + sqlOrg +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0";


                //string sqlSelector = " SELECT DISTINCT vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.VehicleId,vlfBox.BoxId," +
                //"CASE WHEN vlfBox.LastValidDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastValidDateTime) END AS OriginDateTime," +
                //"ISNULL(vlfBox.LastLatitude,0) AS Latitude," +
                //"ISNULL(vlfBox.LastLongitude,0) AS Longitude," +
                //"CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit,1) END AS Speed," +
                //"ISNULL(vlfBox.LastHeading,0) AS Heading," +
                //"ISNULL(vlfBox.LastSensorMask,0) AS SensorMask," +
                //"ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +


                //"ISNULL(vlfVehicleInfo.[Description],'') AS [Description]," +
                //"CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                //"CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime," +
                //"GeoFenceEnabled,vlfVehicleInfo.IconTypeId,IconTypeName,vlfVehicleType.VehicleTypeId," +
                //"ISNULL(vlfVehicleType.VehicleTypeName,'') AS VehicleTypeName," +
                //"CASE WHEN vlfBox.LastStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastStatusDateTime) END AS LastStatusDateTime," +
                //"ISNULL(LastStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSensor," +
                //"ISNULL(LastStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSpeed," +
                //"CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime," +
                //"ISNULL(PrevStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSensor," +
                //"ISNULL(PrevStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSpeed," +
                //"vlfFirmware.FwTypeId," +
                //"ISNULL(Dormant,0) AS Dormant," +
                //"CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.DormantDateTime) END AS DormantDateTime";
                string udfStr = string.Format(",DBO.UDFUserNameInfoByFirstLastName ({0},{1}) AS Field1", userId, "vlfVehicleInfo.Field1");

                if (filterCustomOutputFields.Contains(",Field1"))
                    filterCustomOutputFields = filterCustomOutputFields.Replace(",Field1", udfStr);

                string sqlSelector = string.Format("{0} {1}", defaultFields, filterCustomOutputFields);

                string sqlFrom = " FROM vlfBox WITH (NOLOCK) INNER JOIN " +
	                             " vlfVehicleAssignment WITH(NOLOCK) ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN " + 
	                             " vlfFleetVehicles WITH(NOLOCK) ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN " + 
	                             " vlfVehicleInfo WITH(NOLOCK) ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN " +
	                             " vlfIconType WITH(NOLOCK) ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId INNER JOIN " +
	                             " vlfVehicleType WITH(NOLOCK) ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId INNER JOIN " + 
	                             " vlfFirmwareChannels WITH(NOLOCK) ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN " + 
	                             " vlfFirmware WITH(NOLOCK) ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN " + 
	                             " vlfFleet WITH(NOLOCK) ON vlfFleetVehicles.FleetId = vlfFleet.FleetId INNER JOIN " +  
                                 " vlfFleetUsers WITH(NOLOCK) ON vlfFleetUsers.FleetId = vlfFleetVehicles.FleetId ";
                //if(!filterUDF.Contains("F2"))
                //    sqlFrom = " FROM vlfBox WITH (NOLOCK) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfIconType ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN  vlfFleet ON vlfFleetVehicles.FleetId = vlfFleet.FleetId ";
                //else
                //    sqlFrom = " FROM vlfBox WITH (NOLOCK) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfIconType ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN  vlfFleet ON vlfFleetVehicles.FleetId = vlfFleet.FleetId ";

                //string sqlWhere = SQLWhere(filterHierarchy, filterFleet, filterUDF);

                //if (string.IsNullOrEmpty(sqlWhere))
                //{
                //    throw new DASException("Invalid WHERE Clause.");
                //}
                ////sqlWhere = string.Format(" WHERE {0}", sqlWhere);
                //sqlWhere = string.Format(" WHERE {0} AND (vlfvehicleinfo.OrganizationId=@OrganizationId)", sqlWhere);
                //WHERE     vlfFleet.NodeCode IN ('5ZE','5ZE1','5ZE13)'

                string sqlPreWhere = SQLWhere(filterHierarchy, filterFleet, filterUDF);
                string sqlWhere = string.Empty;

                if (string.IsNullOrEmpty(sqlPreWhere))
                {
                    sqlWhere = " WHERE (vlfFleetUsers.UserId=@userId)";//"WHERE vlfFleetVehicles.FleetId=" + fleetId;
                }
                else
                {
                    sqlWhere = string.Format(" WHERE {0} AND (vlfFleetUsers.UserId=@userId)", sqlPreWhere);//"WHERE vlfFleetVehicles.FleetId=" + fleetId;
                }
                string sqlOrder = " ORDER BY vlfVehicleInfo.VehicleId";

                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(string.Format("{0} {1} {2} {3} {4}", sqlHeader, sqlSelector, sqlFrom, sqlWhere, sqlOrder));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by Hierarchy " + filterHierarchy + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by Hierarchy " + filterHierarchy + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

        string SQLWhere(string filterHierarchy, string filterFleet, string filterUDF)
        {
            string sqlWhere = string.Empty;

            if (!string.IsNullOrEmpty(filterHierarchy) && !string.IsNullOrEmpty(filterFleet) && !string.IsNullOrEmpty(filterUDF)) //[HFU]
            {
                //sqlWhere = string.Format(" (vlfFleet.NodeCode IN ({0})) AND (vlfVehicleInfo.VehicleId IN ({1}))  AND (DBO.UDFUserNameInfoByFleetId(vlfFleetVehicles.FleetId,{2}) <>'')", filterHierarchy, filterFleet, filterUDF);
                sqlWhere = string.Format(" (vlfFleet.NodeCode IN ({0})) AND (vlfVehicleInfo.Description IN ({1})) {2}", filterHierarchy, filterFleet, MapUDF(filterUDF));
                //and dbo.UDFUserNameInfoByFleetId(vlfFleetVehicles.FleetId,'mary.christophe@hydro.qc.ca') <>''
            }
            else if (!string.IsNullOrEmpty(filterHierarchy) && !string.IsNullOrEmpty(filterFleet)) //[HF]
            {
                sqlWhere = string.Format(" (vlfFleet.NodeCode IN ({0})) AND (vlfVehicleInfo.Description IN ({1}))", filterHierarchy, filterFleet);
            }
            else if (!string.IsNullOrEmpty(filterHierarchy) && !string.IsNullOrEmpty(filterUDF)) //[HU]
            {
                //sqlWhere = string.Format(" (vlfFleet.NodeCode IN ({0})) AND (DBO.UDFUserNameInfoByFleetId(vlfFleetVehicles.FleetId,{1}) <>'')", filterHierarchy, filterUDF);
                sqlWhere = string.Format(" (vlfFleet.NodeCode IN ({0})) {1}", filterHierarchy, MapUDF(filterUDF));
            }
            else if (!string.IsNullOrEmpty(filterFleet) && !string.IsNullOrEmpty(filterUDF)) //[FU]
            {
                //sqlWhere = string.Format(" (vlfVehicleInfo.VehicleId IN ({0}))  AND (DBO.UDFUserNameInfoByFleetId(vlfFleetVehicles.FleetId,{1}) <>'')", filterFleet, filterUDF);
                sqlWhere = string.Format(" (vlfVehicleInfo.Description IN ({0}))  {1}", filterFleet, MapUDF(filterUDF));
            }
            else if (!string.IsNullOrEmpty(filterHierarchy)) //[H]
            {
                sqlWhere = string.Format(" (vlfFleet.NodeCode IN ({0}))", filterHierarchy);
            }
            else if (!string.IsNullOrEmpty(filterFleet)) //[F]
            {
                sqlWhere = string.Format(" (vlfVehicleInfo.Description IN ({0}))", filterFleet);
            }
            else if (!string.IsNullOrEmpty(filterUDF)) //[U]
            {
                //sqlWhere = string.Format(" (DBO.UDFUserNameInfoByFleetId(vlfFleetVehicles.FleetId,{0}) <>'')", filterUDF);

                sqlWhere = string.Format(" {0}", MapUDF(filterUDF));
                if (sqlWhere.Trim().Length > 1) sqlWhere = sqlWhere.Substring(4);
                //To Remove AND from the begining
            }

            return sqlWhere;
        }

        string MapUDF(string input)
        {
            string output = "";
            string[] a = null;
            //UDF:{F1=hgi_sfm2000,F2=abc}
            if (input.Trim().Length > 1)
            {
                a = input.Split(',');
                foreach (string str in a)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        switch (str.Substring(0, 2))
                        {
                            case "F1":
                                output += string.Format(" AND (DBO.UDFUserNameInfoByFleetId(vlfFleetVehicles.FleetId,'{0}') <>'')", str.Substring(3));
                                break;
                            case "F2":
                                output += string.Format(" AND (vlfVehicleInfo.Field2='{0}')", str.Substring(3));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return output;
        }

        public void FleetVehiclesBulkCopy(int fleetId)
        {

            try
            {
                string sql = "FleetVehiclesBulkCopy";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                //Executes SQL statement
                sqlExec.SPExecuteNonQuery(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Add FleetVehiclesBulkCopy by fleetId " + fleetId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Add FleetVehiclesBulkCopy by fleetId " + fleetId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }


        }

        //SALMAN Feb 13,2013
        /// <summary>
        /// Returns all users info assigned to the organization. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [UserId],[UserName] (FirstName + LastName)</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUsersNameInfoByOrganization(int userId, int organizationId)
        {
            DataSet resultDataSet = null;
            try
            {
                // 1. Prepares SQL statement
                string sql = "SELECT vlfUser.UserId,RTRIM(FirstName) + ' ' + RTRIM(LastName) AS UserName" +
                    " FROM vlfUser INNER JOIN vlfOrganization ON vlfUser.OrganizationId = vlfOrganization.OrganizationId" +
                    " INNER JOIN vlfPersonInfo ON vlfUser.PersonId = vlfPersonInfo.PersonId" +
                    " WHERE vlfOrganization.OrganizationId =" + organizationId +
                    " AND vlfUser.UserId NOT IN (SELECT vlfUser.UserId FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId WHERE vlfUser.OrganizationId=" + organizationId + " AND (vlfUserGroupAssignment.UserGroupId = 1 OR vlfUserGroupAssignment.UserGroupId = 14))" +
                    " AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE())" +
                    " ORDER BY 2";
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
                string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve users info by organization id " + organizationId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


        /// <summary>
        /// Get organization skills list
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>
        /// DataSet
        /// [SkillId], [SkillName]
        /// </returns>
        public DataSet GetOrganizationSkills(int organizationId)
        {
            string prefixMsg = "Unable to retrieve organization skills list by organization id " + organizationId + ". ";
            DataSet resultDataSet = null;
            try
            {
                string sql = "SELECT SkillId, SkillName FROM vlfOrganizationSkills WHERE OrganizationId=" + organizationId;
                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
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
        /// [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
        /// [FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet VlfFindVehicleDrivesInfo(int fleetId, int userId, string language, int skillId = 0, int vehicleTypeId = -1)
        {
            string prefixMsg = "Unable to retrieve vehicles driver info by fleet id " + fleetId + ". ";
            DataSet resultDataSet = null;
            try
            {
                string sql = "vlfFindVehicleDriversInfo";
                SqlParameter[] sqlParams = new SqlParameter[5];
                sqlParams[0] = new SqlParameter("@fleetId", fleetId);
                sqlParams[1] = new SqlParameter("@userId", userId);
                sqlParams[2] = new SqlParameter("@language", language);
                sqlParams[3] = new SqlParameter("@skillId", skillId);
                sqlParams[4] = new SqlParameter("@vehicleTypeId", vehicleTypeId);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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

        public DataSet GetDevicesLastKnownPosition(string slectionCriteria, int userId)
        {
            DataSet resultDataSet = null;
            try
            {
                string sql = " DECLARE @OrganizationId int SELECT @OrganizationId=OrganizationId FROM vlfUser WHERE UserId=" + userId.ToString() + " DECLARE @ResolveLandmark int DECLARE @Timezone int DECLARE @Unit real DECLARE @DayLightSaving int " +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId.ToString() + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " SELECT @Unit=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId.ToString() + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.MeasurementUnits +
                    " IF @Timezone IS NULL SET @Timezone=0 IF @Unit IS NULL SET @Unit=1 SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId.ToString() + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0 SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + userId.ToString() + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0" +


                    " SELECT DISTINCT vlfVehicleAssignment.LicensePlate,vlfVehicleInfo.VehicleId,vlfBox.BoxId," +
                    "CASE WHEN vlfBox.LastValidDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastValidDateTime) END AS OriginDateTime," +
                    "ISNULL(vlfBox.LastLatitude,0) AS Latitude," +
                    "ISNULL(vlfBox.LastLongitude,0) AS Longitude," +
                    "CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit,1) END AS Speed," +
                    "ISNULL(vlfBox.LastHeading,0) AS Heading," +
                    "ISNULL(vlfBox.LastSensorMask,0) AS SensorMask," +
                    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +


                    "ISNULL(vlfVehicleInfo.[Description],'') AS [Description]," +
                    "CASE WHEN vlfBox.BoxArmed=0 then 'false' ELSE 'true' END AS BoxArmed," +
                    "CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime," +
                    "GeoFenceEnabled,vlfVehicleInfo.IconTypeId,IconTypeName,vlfVehicleType.VehicleTypeId," +
                    "ISNULL(vlfVehicleType.VehicleTypeName,'') AS VehicleTypeName," +
                    "CASE WHEN vlfBox.LastStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.LastStatusDateTime) END AS LastStatusDateTime," +
                    "ISNULL(LastStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSensor," +
                    "ISNULL(LastStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS LastStatusSpeed," +
                    "CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime," +
                    "ISNULL(PrevStatusSensor," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSensor," +
                    "ISNULL(PrevStatusSpeed," + (short)VLF.CLS.Def.Const.unassignedShortValue + ") AS PrevStatusSpeed," +
                    "vlfFirmware.FwTypeId," +
                    "ISNULL(Dormant,0) AS Dormant," +
                    "CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,vlfBox.DormantDateTime) END AS DormantDateTime" +
                    " FROM vlfBox with (nolock) INNER JOIN vlfVehicleAssignment WITH(NOLOCK) ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfFleetVehicles WITH(NOLOCK) ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId INNER JOIN vlfVehicleInfo WITH(NOLOCK) ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId INNER JOIN vlfIconType WITH(NOLOCK) ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId INNER JOIN vlfVehicleType WITH(NOLOCK) ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId INNER JOIN vlfFirmwareChannels WITH(NOLOCK) ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware WITH(NOLOCK) ON vlfFirmwareChannels.FwId = vlfFirmware.FwId " +
                    " INNER JOIN vlfFleetUsers WITH(NOLOCK) ON vlfFleetUsers.FleetId = vlfFleetVehicles.FleetId " +
                    " WHERE " + slectionCriteria + " AND vlfBox.OrganizationId=@OrganizationId AND vlfFleetUsers.UserId=" + userId.ToString() +
                    " ORDER BY vlfVehicleInfo.[Description]";

                //Executes SQL statement
                resultDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by fleet id " + slectionCriteria + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicles info by fleet id " + slectionCriteria + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }


		#region Configuration Methods 
		private string GetConfigParameter( string moduleName, short groupID, string paramName, string defaultValue)
		{
			string paramValue = defaultValue;
			DB.Configuration config = new DB.Configuration(sqlExec);

			// take Module ID in DB
			short moduleID = config.GetConfigurationModuleTypeId(moduleName);
			if( moduleID == VLF.CLS.Def.Const.unassignedShortValue )
			{
				throw new VLF.ERR.DASAppResultNotFoundException("Cannot find '" + moduleName + "' in DB." );
			}

			// get parameter from DB
			try	
			{ 
				paramValue = config.GetConfigurationValue(moduleID,groupID,paramName); 
			}
			catch 
			{	
				// TODO: log error
				//throw new VLF.ERR.DASException("'" + paramName + "' is not set in DB. Setting up default value: " + paramValue.ToString() + ".");
			}
			return paramValue;
		}
		#endregion Configuration Methods 

        //Salman May 29, 2014 (MNR)
        /// <summary>
        /// Returns devices last known position information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="userId"></param>
        /// <returns>
        /// DataSet 
        /// [BoxId],[OriginDateTime],[Latitude],[Longitude],
        /// [Speed],[Heading],[LastCommunicatedDateTime],[CustomProp]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>

        public DataSet GetDeviceLastKnownPositionPreTemplatedInfo_SP(int userId, int fleetId, DateTime lastCommunicatedDateTime)
        {
            string prefixMsg = "Unable to retrieve devices info by fleet id " + fleetId + ". ";
            DataSet resultDataSet = null;
            try
            {
                string sql = "usp_DevicesLastPositon_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@userId", userId);
                sqlParams[1] = new SqlParameter("@fleetId", fleetId);
                sqlParams[2] = new SqlParameter("@lastCommunicatedDateTime", lastCommunicatedDateTime);
                //Executes SQL statement
                resultDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
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
        /// Add new vehicle to fleet.
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="vehicleId"></param>
        /// <returns>Int64</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle alredy exists in the fleet.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddAllVehicleToFleet(int fleetId, string result, int organizationId)
        {
            int rowsAffected = 0;
                        
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);                
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@temptable", SqlDbType.Xml, result);
                
               
                rowsAffected = (int)sqlExec.SPExecuteNonQuery("sp_AssignAllVehicleToFleet");
                

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add new 'FleetId:" + fleetId +  "'.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add new 'FleetId:" + fleetId +  "'.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add new 'FleetId:" + fleetId + "'.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " The vehicle already exists in the fleet.");
            }
            return rowsAffected;
        }		
	}
}
