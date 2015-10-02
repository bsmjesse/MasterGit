using System;
using System.Data.SqlClient ;	// for SqlException
using System.Data ;			// for DataSet
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfVehicleGeozone table.
	/// </summary>
	public class VehicleGeozone : TblGenInterfaces
	{
		#region Public Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public VehicleGeozone(SQLExecuter sqlExec) : base ("vlfVehicleGeozone",sqlExec)
		{
		}
		/// <summary>
		/// Add new geozone to the vehicle.
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="geozoneId"></param>
		/// <param name="severityId"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddGeozone(Int64 vehicleId,short geozoneId,short severityId,int speed)
		{
			// 1. Prepares SQL statement
			try
			{
				Int64 geozoneNo = GetGeozoneNo(vehicleId,geozoneId);
				// Set SQL command
				string sql = "INSERT INTO " + tableName + "(VehicleId,GeozoneNo,SeverityId,Speed) VALUES ( @VehicleId,@GeozoneNo,@SeverityId,@Speed )";
				// Add parameters to SQL statement
				sqlExec.ClearCommandParameters();
				sqlExec.AddCommandParam("@VehicleId",SqlDbType.BigInt,vehicleId);
				sqlExec.AddCommandParam("@GeozoneNo",SqlDbType.BigInt,geozoneNo);
				sqlExec.AddCommandParam("@SeverityId",SqlDbType.SmallInt,severityId);
                sqlExec.AddCommandParam("@Speed", SqlDbType.Int, speed);
				
				//Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to assign new geozone=" + geozoneId +" to the vehicle=" + vehicleId +  "." ;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to assign new geozone=" + geozoneId +" to the vehicle=" + vehicleId +  "." ;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Sets geozone severity to the vehicle.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="vehicleId"></param>
		/// <param name="geozoneId"></param>
		/// <param name="severityId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetGeozoneSeverity(int organizationId,Int64 vehicleId,short geozoneId,short severityId)
		{
			Int64 geozoneNo = GetGeozoneNo(vehicleId,geozoneId);
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfVehicleGeozone SET SeverityId=" + severityId +
				" WHERE VehicleId=" + vehicleId + " AND GeozoneNo=" + geozoneNo;
			
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
				string prefixMsg = "Unable to update vehicle geozone Id=" + geozoneId + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update vehicle geozone Id=" + geozoneId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update vehicle geozone Id=" + geozoneId + ".";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " This geozone dose not exist.");
			}
		}
		/// <summary>
		/// Deletes all geozones related to specific vehicle.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="vehicleId"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if vehicle does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllGeozones(Int64 vehicleId)
		{
			return DeleteRowsByIntField("VehicleId",vehicleId, "vehicle id");		
		}
		/// <summary>
		/// Deletes geozone from vehicle.
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="geozoneId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteGeozoneFromVehicle(Int64 vehicleId,short geozoneId)
		{
			int rowsAffected = 0;
			Int64 geozoneNo = GetGeozoneNo(vehicleId,geozoneId);
			if(geozoneNo != VLF.CLS.Def.Const.unassignedIntValue)
			{
				// 1. Prepares SQL statement
				string sql = "DELETE FROM " + tableName + 
					" WHERE GeozoneNo=" + geozoneNo +
					" AND VehicleId=" + vehicleId;
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
					string prefixMsg = "Unable to delete geozoneId=" + geozoneId + " from vehicle " + vehicleId;
					Util.ProcessDbException(prefixMsg,objException);
				}
				catch(DASDbConnectionClosed exCnn)
				{
					throw new DASDbConnectionClosed(exCnn.Message);
				}
				catch(Exception objException)
				{
					string prefixMsg = "Unable to delete geozoneId=" + geozoneId + " from vehicle " + vehicleId;
					throw new DASException(prefixMsg + " " + objException.Message);
				}
			}
			return rowsAffected;
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
			int retResult = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT vlfFirmware.MaxGeozones FROM vlfFirmware INNER JOIN vlfFirmwareChannels ON vlfFirmware.FwId = vlfFirmwareChannels.FwId INNER JOIN vlfFirmwareChannelReference ON vlfFirmwareChannels.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId WHERE vlfVehicleAssignment.VehicleId = " + vehicleId;
				//Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != DBNull.Value)
					retResult = Convert.ToInt32(obj);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle max geozones by vehicle Id=" + vehicleId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle max geozones by vehicle Id=" + vehicleId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}
		
		/// <summary>
		/// Retrieves all assigned to vehicle geozones info
		/// </summary>
		/// <returns>
		/// DataSet [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId],
		/// [GeozoneName],[Type],[GeozoneType],[SeverityId],[Description],[BoxId]
		/// </returns>
		/// <param name="vehicleId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllAssignedToVehicleGeozonesInfo(Int64 vehicleId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfVehicleAssignment.VehicleId,vlfVehicleGeozone.GeozoneNo, " +
							"vlfVehicleGeozone.SeverityId,OrganizationId,GeozoneId, " +
							"GeozoneName,Type,GeozoneType,vlfVehicleGeozone.SeverityId,Description, " +
                            "vlfVehicleAssignment.BoxId,ISNULL(SyncDate,'1/1/1970') as SyncDate,vlfVehicleGeozone.Speed, " +
                            "ISNULL(vlfOrganizationGeozone.[Public], 1) AS [Public], ISNULL(vlfOrganizationGeozone.CreateUserID, 0) AS CreateUserID " +
							" FROM vlfVehicleGeozone INNER JOIN vlfOrganizationGeozone ON vlfVehicleGeozone.GeozoneNo=vlfOrganizationGeozone.GeozoneNo " +
							" INNER JOIN vlfVehicleAssignment ON vlfVehicleGeozone.VehicleId=vlfVehicleAssignment.VehicleId " +
							" WHERE vlfVehicleAssignment.VehicleId=" + vehicleId + " ORDER BY GeozoneName";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle geozones info by vehicle Id=" + vehicleId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle geozones info by vehicle Id=" + vehicleId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves all unassigned to vehicle geozones info
		/// </summary>
		/// <returns>
		/// DataSet [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],
		/// [GeozoneType][SeverityId],[Description]
		/// </returns>
		/// <param name="vehicleId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnassignedToVehicleGeozonesInfo(Int64 vehicleId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfOrganizationGeozone.GeozoneNo, " +
							"vlfOrganizationGeozone.OrganizationId, " +
							"GeozoneId,GeozoneName,Type,GeozoneType, " +
							"SeverityId,vlfOrganizationGeozone.Description, " +
                            "ISNULL(vlfOrganizationGeozone.[Public], 1) AS [Public], ISNULL(vlfOrganizationGeozone.CreateUserID, 0) AS CreateUserID " +
							" FROM vlfOrganizationGeozone INNER JOIN vlfVehicleInfo ON vlfOrganizationGeozone.OrganizationId=vlfVehicleInfo.OrganizationId " +
							" WHERE vlfVehicleInfo.VehicleId=" + vehicleId +
							" AND GeozoneNo NOT IN (SELECT GeozoneNo " +
								" FROM vlfVehicleGeozone WHERE VehicleId=" + vehicleId + ") ORDER BY GeozoneName";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve geozones info unassigned to vehicle Id=" + vehicleId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve geozones info unassigned to vehicle Id=" + vehicleId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves vehicle geozones information
		/// </summary>
		/// <returns>
		/// DataSet [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId],
		/// [GeozoneName],[Latitude],[Longitude],[Width],[Height],[Type],[SeverityId],
		/// [Description]
		/// </returns>
		/// <param name="vehicleId"></param> 
		/// <param name="geozoneId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetGeozoneInformation(Int64 vehicleId,short geozoneId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT VehicleId,vlfVehicleGeozone.GeozoneNo,"+
					"vlfVehicleGeozone.SeverityId,OrganizationId,GeozoneId,"+
					"GeozoneName,Latitude,Longitude,Type,Description"+
					" FROM vlfOrganizationGeozone INNER JOIN vlfGeozoneSet ON vlfOrganizationGeozone.GeozoneNo = vlfGeozoneSet.GeozoneNo INNER JOIN vlfVehicleGeozone ON vlfOrganizationGeozone.GeozoneNo = vlfVehicleGeozone.GeozoneNo"+
					" WHERE vlfVehicleGeozone.VehicleId=" + vehicleId +
					" AND vlfOrganizationGeozone.GeozoneId=" + geozoneId;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle " + vehicleId + " geozone " + geozoneId + " info. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle " + vehicleId + " geozone " + geozoneId + " info. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		/// <summary>
		/// Retrieves all assigned vehicles info to geozone
		/// </summary>
		/// <returns>
		/// DataSet [VehicleId],[Description]
		/// </returns>
		/// <param name="organizationId"></param> 
		/// <param name="geozoneId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllAssignedVehiclesInfoToGeozone(int organizationId,short geozoneId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfVehicleInfo.VehicleId, vlfVehicleInfo.Description"+
							" FROM vlfVehicleInfo INNER JOIN vlfVehicleGeozone ON vlfVehicleInfo.VehicleId=vlfVehicleGeozone.VehicleId"+
							" INNER JOIN vlfOrganizationGeozone ON vlfVehicleGeozone.GeozoneNo=vlfOrganizationGeozone.GeozoneNo"+
							" WHERE vlfOrganizationGeozone.GeozoneId=" + geozoneId +
							" AND vlfOrganizationGeozone.OrganizationId=" + organizationId;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle info assigned to geozone=" + geozoneId + " for organization " + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle info assigned to geozone=" + geozoneId + " for organization " + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		
		/// <summary>
		/// Retrieves all unassigned vehicles info to geozone
		/// </summary>
		/// <returns>
		/// DataSet [VehicleId],[Description]
		/// </returns>
		/// <param name="organizationId"></param> 
		/// <param name="geozoneId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnasignedVehiclesInfoToGeozone(int organizationId,short geozoneId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfVehicleInfo.VehicleId,vlfVehicleInfo.Description FROM vlfVehicleInfo"+
							" WHERE OrganizationId=" + organizationId + 
							" AND vlfVehicleInfo.VehicleId NOT IN (SELECT vlfVehicleInfo.VehicleId"+
								" FROM vlfVehicleInfo INNER JOIN vlfVehicleGeozone ON vlfVehicleInfo.VehicleId=vlfVehicleGeozone.VehicleId"+
								" INNER JOIN vlfOrganizationGeozone ON vlfVehicleGeozone.GeozoneNo=vlfOrganizationGeozone.GeozoneNo"+
								" WHERE vlfOrganizationGeozone.GeozoneId=" + geozoneId +
								" AND vlfOrganizationGeozone.OrganizationId=" + organizationId + ")";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle info assigned to geozone=" + geozoneId + " for organization " + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle info assigned to geozone=" + geozoneId + " for organization " + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		
		/// <summary>
		/// Retrieves organization geozone index
		/// </summary>
		/// <param name="vehicleId"></param>
		/// <param name="geozoneId"></param>
		/// <returns>geozone index</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected Int64 GetGeozoneNo(Int64 vehicleId,short geozoneId)
		{
			Int64 geozoneNo = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				string sql = "SELECT vlfOrganizationGeozone.GeozoneNo"+
							" FROM vlfOrganizationGeozone INNER JOIN vlfVehicleInfo ON vlfOrganizationGeozone.OrganizationId = vlfVehicleInfo.OrganizationId"+
							" WHERE vlfVehicleInfo.VehicleId=" + vehicleId +
							" AND vlfOrganizationGeozone.GeozoneId=" + geozoneId;
				//Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != null)
					geozoneNo = Convert.ToInt64(obj);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve vehicle " + vehicleId + " geozone " + geozoneId + " geozone No. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve vehicle " + vehicleId + " geozone " + geozoneId + " geozone No. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return geozoneNo;
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
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetGeozoneInfo(int boxId,short geozoneId)
		{
			DataSet sqlDataSet = null;
			try
			{
				// Prepares SQL statement
				string sql = "SELECT vlfOrganizationGeozone.GeozoneNo, vlfOrganizationGeozone.OrganizationId, vlfOrganizationGeozone.GeozoneId, vlfOrganizationGeozone.GeozoneName, vlfOrganizationGeozone.Type,vlfOrganizationGeozone.GeozoneType, vlfOrganizationGeozone.SeverityId, vlfOrganizationGeozone.Description, vlfOrganizationGeozone.Deleted,"+
					"ISNULL(vlfOrganizationGeozone.Email,' ') AS Email,"+
                    "ISNULL(vlfOrganizationGeozone.Phone,' ') AS Phone," +
					"ISNULL(vlfOrganizationGeozone.TimeZone,0) AS TimeZone,"+
					"ISNULL(vlfOrganizationGeozone.DayLightSaving,0) AS DayLightSaving,"+
					"ISNULL(vlfOrganizationGeozone.FormatType,0) AS FormatType,"+
					"ISNULL(vlfOrganizationGeozone.Notify,0) AS Notify,"+
					"ISNULL(vlfOrganizationGeozone.Warning,0) AS Warning,"+
					"ISNULL(vlfOrganizationGeozone.Critical,0) AS Critical"+
                    " FROM vlfOrganizationGeozone INNER JOIN vlfBox with (nolock) ON vlfOrganizationGeozone.OrganizationId = vlfBox.OrganizationId" +
					" WHERE vlfBox.BoxId=" + boxId + "  AND vlfOrganizationGeozone.GeozoneId=" + geozoneId;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by box Id=" + boxId + " and geozone Id=" + geozoneId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by box Id=" + boxId + " and geozone Id=" + geozoneId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        /// <summary>
        /// Retrieves all assigned geozones to vehicle
        /// </summary>
        /// <returns>
        /// DataSet [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId],
        /// [GeozoneName],[Type],[GeozoneType],[SeverityId],[Description],[BoxId]
        /// </returns>
        /// <param name="vehicleId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllAssignedGeozonesToVehicle(int organizationId, short geozoneId, int userId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @Timezone int"+
					" DECLARE @DayLightSaving int"+
					" SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
					" WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
					" IF @Timezone IS NULL SET @Timezone=0"+
					" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
					" IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
					" SET @Timezone= @Timezone + @DayLightSaving"+
                    " SELECT vlfVehicleAssignment.VehicleId,vlfVehicleGeozone.GeozoneNo," +
                            "GeozoneName,vlfVehicleInfo.Description,vlfVehicleAssignment.BoxId," +
                            " convert(varchar,DATEADD(hour,@Timezone,LastCommunicatedDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,LastCommunicatedDateTime),108) as LastCommunicatedDateTime " +
                            " FROM vlfVehicleGeozone INNER JOIN vlfOrganizationGeozone ON vlfVehicleGeozone.GeozoneNo=vlfOrganizationGeozone.GeozoneNo" +
                            " INNER JOIN vlfVehicleAssignment ON vlfVehicleGeozone.VehicleId=vlfVehicleAssignment.VehicleId" +
                            " INNER JOIN vlfVehicleInfo ON vlfVehicleInfo.VehicleId=vlfVehicleAssignment.VehicleId" +
                            " INNER JOIN vlfBox with (nolock) ON vlfVehicleAssignment.BoxId=vlfBox.BoxId" +
                            " WHERE vlfOrganizationGeozone.OrganizationId=" + organizationId + " AND vlfOrganizationGeozone.GeozoneId=" + geozoneId + " ORDER BY vlfVehicleAssignment.BoxId";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve vehicle geozones info by GeozoneId=" + geozoneId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve vehicle geozones info by GeozoneId=" + geozoneId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
		#endregion

		#region Static Interfaces
		/// <summary>
		/// Returns formatted GeoZone Description
		/// </summary>
		/// <param name="customProp"></param>
		/// <param name="tblGeoZones"></param>
		/// <returns>description</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public static string GetGeoZoneDescription(string customProp,DataTable tblGeoZones)
		{
			string geoZoneDescription = "";
			//dsVehicleSensors
			int geozoneId = Convert.ToInt32(Util.PairFindValue(VLF.CLS.Def.Const.keyGeozoneId,customProp));
			VLF.CLS.Def.Enums.GeoZoneDirection geozoneDirection = (VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(Util.PairFindValue(VLF.CLS.Def.Const.keyGeozoneDir,customProp));
			if(tblGeoZones == null || tblGeoZones.Rows.Count == 0)
			{
				geoZoneDescription = "Undefined GeoZone (" + geozoneId.ToString() + ") - " + geozoneDirection.ToString();
			}
			else
			{
				foreach(DataRow ittr in tblGeoZones.Rows)
				{
					if(geozoneId == Convert.ToInt32(ittr["GeozoneId"]))
					{
						geoZoneDescription = ittr["GeozoneName"].ToString().TrimEnd();
						geoZoneDescription += " - ";
						//geoZoneDescription +=((VLF.CLS.Def.Enums.GeoZoneDirection)Convert.ToInt16(ittr["Type"].ToString().TrimEnd())).ToString();
						geoZoneDescription += geozoneDirection.ToString();
						break;
					}
				}
				if(geoZoneDescription == "")
				{
					geoZoneDescription = "Undefined GeoZone (" + geozoneId.ToString() + ") - " + geozoneDirection.ToString();
				}
			}
			return geoZoneDescription;
		}
		#endregion

      # region History information

      /// <summary>
      /// Get GeoZone Messages
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="licensePlate"></param>
      /// <param name="geozoneNo"></param>
      /// <param name="dtFrom"></param>
      /// <param name="dtTo"></param>
      /// <returns></returns>
      public DataTable GetVehicleGeozoneMessages(int userId, string licensePlate, long geozoneNo, DateTime dtFrom, DateTime dtTo)
      {
         DataTable resultSet = null;
         string prefixMsg = "";
         try
         {
            prefixMsg = "Unable to get info by geozone number=" + geozoneNo + ". ";
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@userId", userId);
            sqlParams[1] = new SqlParameter("@licenseP", licensePlate.Replace("'", "''"));
            sqlParams[2] = new SqlParameter("@geozoneNo", geozoneNo);
            sqlParams[3] = new SqlParameter("@dtFrom", dtFrom);
            sqlParams[4] = new SqlParameter("@dtTo", dtTo);

            // SQL statement
            string sql = "ReportGeozone4Vehicle";
            sqlExec.CommandTimeout = 72000; 
            //Executes SQL statement
            resultSet = sqlExec.SPExecuteDataTable(sql, "GeozoneMessages", sqlParams);
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
         return resultSet;
      }


      public DataSet GetVehicleGeozoneTimeSheet(int userId, int fleetId,  DateTime dtFrom, DateTime dtTo)
      {
          DataSet resultSet =new DataSet();
          string prefixMsg = "";
          try
          {
              
              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@userId", userId);
              sqlParams[1] = new SqlParameter("@fleetId", fleetId);
              sqlParams[2] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[3] = new SqlParameter("@toDate", dtTo);

              // SQL statement
              string sql = "ReportGeozoneTimesheet";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset (sql, sqlParams);
              
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
          return resultSet;
      }

      public DataSet GetVehicleGeozoneTimeSheet(int userId, int fleetId, DateTime dtFrom, DateTime dtTo,Int16 activeVehicles)
      {
          DataSet resultSet = new DataSet();
          string prefixMsg = "";
          try
          {

              SqlParameter[] sqlParams = new SqlParameter[5];
              sqlParams[0] = new SqlParameter("@userId", userId);
              sqlParams[1] = new SqlParameter("@fleetId", fleetId);
              sqlParams[2] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[3] = new SqlParameter("@toDate", dtTo);
              sqlParams[4] = new SqlParameter("@activeVehicles", activeVehicles);

              // SQL statement
              string sql = "ReportGeozoneTimesheet_ActiveVehicles";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
          return resultSet;
      }

      public DataSet GetVehicleGeozoneTimeSheet201107(int userId, int fleetId, DateTime dtFrom, DateTime dtTo)
      {
          DataSet resultSet = new DataSet();
          string prefixMsg = "";
          try
          {

              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@userId", userId);
              sqlParams[1] = new SqlParameter("@fleetId", fleetId);
              sqlParams[2] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[3] = new SqlParameter("@toDate", dtTo);

              // SQL statement
              string sql = "ReportGeozoneTimesheet201107";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
          return resultSet;
      }



      public DataSet GetWorksiteActivityPerDay(int userId, int fleetId, DateTime dtFrom, DateTime dtTo)
      {
          DataSet resultSet = new DataSet();
          string prefixMsg = "";
          try
          {

              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@userId", userId);
              sqlParams[1] = new SqlParameter("@fleetId", fleetId);
              sqlParams[2] = new SqlParameter("@fromDate", dtFrom);
              sqlParams[3] = new SqlParameter("@toDate", dtTo);

              // SQL statement
              string sql = "WorksiteActivityPerDay";
              //Executes SQL statement
              resultSet = sqlExec.SPExecuteDataset(sql, sqlParams);

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
          return resultSet;
      }


    

      # endregion
   }
}
