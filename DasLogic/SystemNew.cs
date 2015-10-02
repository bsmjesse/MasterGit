using System;
using System.Collections;	// for SortedList
using System.Data;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Text;

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to system configuration functionality in database
	/// </summary>
	public partial class SystemConfig  : Das
	{
		Dcl dcl = null;
#if ASL_
		Asl asl = null;
#endif
		DB.Preference preference = null;
		Configuration  sysCfg = null;
        DB.MapEngine mapEngine = null;
        DB.CommandScheduler commandScheduler = null;

		#region General Interfaces
		/// <summary>
		/// Consrtuctor
		/// </summary>
		/// <param name="connectionString"></param>
		public SystemConfig(string connectionString) : base (connectionString)
		{
			dcl = new Dcl(sqlExec);
#if ASL_
			asl = new Asl(sqlExec);
#endif
			preference = new DB.Preference(sqlExec);
			sysCfg = new DB.Configuration(sqlExec);
            mapEngine = new DB.MapEngine(sqlExec);
            commandScheduler = new DB.CommandScheduler(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region BoxConfig Interfaces

		/// <summary>
		/// Get all configuration information. 
		/// </summary>
		/// <returns>DataSet[FwChId],[FwId],[FwName],[ChId],[ChName],[ChPriority],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllConfigInfo()
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetAllConfigInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllConfigInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

      /// <summary>
      ///         checking if two firmware types have the same type of channels defined 
      ///         when you know one of the communication modes
      /// </summary>
      /// <param name="currFWID"></param>
      /// <param name="newFWID"></param>
      /// <param name="currCommMode"></param>
      /// <returns></returns>
      /// <comment>
      ///  this is the query I run to see what <channel, BoxProtocolTypeId, CommModeId > are defined for a specific firmwareID
      //// SELECT     vlfFirmwareChannels.FwChId, vlfFirmware.BoxHwTypeId, vlfFirmware.FwName, vlfFirmware.FwTypeId, vlfFirmwareChannels.ChId, 
      ////                vlfFirmwareType.FwTypeName, vlfBoxHwType.BoxHwTypeName, vlfFirmwareChannels.FwId, vlfFirmwareChannels.ChPriority, vlfChannels.ChName, 
      ////                vlfChannels.CommModeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfBoxProtocolType.BoxProtocolTypeId
      //// FROM         vlfFirmware INNER JOIN
      ////                vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId INNER JOIN
      ////                vlfFirmwareChannels ON vlfFirmware.FwId = vlfFirmwareChannels.FwId INNER JOIN
      ////                vlfFirmwareChannelReference ON vlfFirmwareChannels.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN
      ////                vlfFirmwareType ON vlfFirmware.FwTypeId = vlfFirmwareType.FwTypeId INNER JOIN
      ////                vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN
      ////                vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId
      ////WHERE     (vlfFirmware.FwId = 164)
      /// </comment>
      public bool IsCompatibleFirmware(short currFWID, short newFWID, short currCommMode)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         return boxConfig.IsCompatibleFirmware(currFWID, newFWID, currCommMode);
      }

		/// <summary>
		/// Get all primary channels by FwId
		/// </summary>
		/// <param name="fwId"></param>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName],[FwChId],[ChPriority]</returns>
		/// /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllPrimaryChannelsByFwId(short fwId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetAllPrimaryChannelsByFwId(fwId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetAllPrimaryChannelsByFwId" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

      /// <summary>
      /// Get all the channels by FwId
      /// </summary>
      /// <param name="fwId"></param>
      /// <returns>DataSet [FwChId], [FwId], [ChId], [ChName], [ChPriority]</returns>
      /// /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllChannelsByFwId(short fwId)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetAllChannelsByFwId(fwId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetAllChannelsByFwId";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }

      /// <summary>
      /// Get all the channels by FwId
      /// </summary>
      /// <param name="fwId"></param>
      /// <param name="chPriority"></param>
      /// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName],[FwChId],[ChName]</returns>
      /// /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllChannelsByFwId(short fwId, short chPriority)
      {
         if (chPriority < 0 || chPriority > 1)
         {
            throw new ArgumentException(String.Format("Invalid channel priority: {0}", chPriority));
         }
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetAllChannelsByFwId(fwId, chPriority);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetAllChannelsByFwId";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }	

		/// <summary>
		/// Get all secondary channels by FwId
		/// </summary>
		/// <param name="fwId"></param>
		// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllSecondaryChannelsByFwId(short fwId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetAllSecondaryChannelsByFwId(fwId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetAllSecondaryChannelsByFwId" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}


      /// <summary>
      /// 
      /// </summary>
      /// <param name="protocolTypeID"></param>
      /// <param name="commMode"></param>
      /// <returns></returns>
      public short GetChannelId(short protocolTypeID, short commMode)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         return boxConfig.GetChannelId(protocolTypeID, commMode);
      }
      
      /// <summary>
      ///         extract all channels as [FwId] , [ChId], [ChPriority]
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns></returns>
      public DataSet GetAllChannelsByBoxId(int boxId)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetAllChannelsByBoxId(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetAllChannelsByBoxId";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }


      /// <summary>
      ///         extract all channels as [FwId] , [ChId], [ChPriority]
      /// </summary>
      /// <param name="fwChId"></param>
      /// <returns></returns>
      public DataSet GetAllChannelsByFwChId(short fwChId)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetAllChannelsByFwChId(fwChId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetAllChannelsByFwChId";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }

      /// <summary>
      ///      is the reverse of the previous function <GetAllChannelsByFwChId>
      /// </summary>
      /// <comment>
      ///      this is used to update in real time whenever the box is switching from 
      ///      priamry modem to another - solution for Bantek 
      /// </comment>
      /// <param name="ds"></param>
      /// <returns></returns>
      public short GetFirmwareChannelIdByChannels(DataSet ds)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         return boxConfig.GetFirmwareChannelIdByChannels(ds);
      }

      /// <summary>
      /// Get all secondary channels by FwChId
      /// </summary>
      /// <param name="fwId"></param>
      // <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetAllSecondaryChannelsByFwChId(short fwChId)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetAllSecondaryChannelsByFwChId(fwChId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetAllSecondaryChannelsByFwChId";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }

        /// <summary>
        /// Get all secondary channels by FwId
        /// </summary>
        /// <param name="fwId"></param>
        /// <param name="commModeId"></param>
        // <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllSecondaryChannels(short fwId, short commModeId)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet dsResult = boxConfig.GetAllSecondaryChannels(fwId, commModeId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetAllSecondaryChannels";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

      /// <summary>
		/// Get Firmware Information. 
		/// </summary>
		/// <param name="fwId"></param>
		/// <returns>DataSet [FwChId],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommModeId],[CommModeName],[ChPriority],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFirmwareInfo(short fwId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetFirmwareInfo(fwId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetFirmwareInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

        /// <summary>
        /// Get Firmware Information. 
        /// </summary>
        /// <param name="fwId"></param>
        /// <returns>DataSet [FwId],[BoxHwTypeId],[FwName],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[BoxHwTypeName]
        /// </returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFirmwareInfoOnly(short fwId)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet dsResult = boxConfig.GetFirmwareInfoOnly(fwId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetFirmwareInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

		/// <summary>
		/// Get box configuration information. 
		/// </summary>
		/// <returns>DataSet [FwId],[BoxHwTypeId],[FwName],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[BoxHwTypeName]</returns>
		/// <param name="selectedFwTypeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllFirmwareInfo(short selectedFwTypeId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetAllFirmwareInfo(selectedFwTypeId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllConfigInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

		/// <summary>
		/// Get all firmware types information. 
		/// </summary>
		/// <returns>DataSet[FwTypeId],[FwTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet FirmwareTypes()
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.FirmwareTypes();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllConfigInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

		/// <summary>
		/// Returns all box configuration. 	
		/// </summary>
		/// <param name="fwChId"></param>
		/// <returns>DataSet [FwChId],[ChPriority],[FwId],[FwName],[ChId],[ChName],[CommModeId],[CommModeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[BoxHwTypeId],[BoxHwTypeName]</returns>
		public DataSet GetConfigInfo(short fwChId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetConfigInfo(fwChId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxConfigInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

      /// <summary>
      /// Returns all box configuration. 	
      /// </summary>
      /// <param name="fwChId"></param>
      /// <returns>DataSet [FwChId],[ChPriority],[FwId],[FwName],[ChId],[ChName],[CommModeId],[CommModeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[BoxHwTypeId],[BoxHwTypeName]</returns>
      public DataSet GetConfigInfoByFwId(short fwId)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetConfigInfoByFwId(fwId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "BoxConfigInfo";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }

		/// <summary>
		/// Retieves box default communication info. 	
		/// </summary>
		/// <param name="fwChId"></param>
		/// <remarks>
		/// TableName	= "BoxCommunicationInfo"
		/// DataSetName = "Box"
		/// </remarks>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <returns>DataSet [CommAddressTypeId],[CommAddressTypeName],[CommAddressValue],[FwChId],[BoxHwTypeName],[BoxProtocolTypeId],[CommModeId],[ChId],[ChName],[ChPriority]</returns>
		public DataSet GetDefCommInfo(short fwChId)
		{
			// 1. Retieves box configuration
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			DataSet dsResult = boxConfig.GetDefCommInfo(fwChId);
			if(dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
			{
				dsResult.Tables[0].TableName = "BoxCommunicationInfo";
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}

      /// <summary>
      /// Retieves short box default communication info
      /// </summary>
      /// <param name="fwChId"></param>
      /// <remarks>
      /// TableName	= "BoxCommunicationInfo"
      /// DataSetName = "Box"
      /// </remarks>
      /// <exception cref="DASException">Throws DASException in all error cases.</exception>
      /// <returns>DataSet [CommAddressTypeId],[CommAddressTypeName],[CommAddressValue]</returns>
      public DataSet GetDefaultCommInfo(short fwChId)
      {
         // 1. Retieves box configuration
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         string sql =
            "SELECT vlfCommAddressType.CommAddressTypeId, vlfCommAddressType.CommAddressTypeName, '' AS CommAddressValue FROM vlfCommModeAddressType INNER JOIN vlfFirmwareChannels INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId ON vlfCommModeAddressType.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfCommAddressType ON vlfCommModeAddressType.CommAddressTypeId = vlfCommAddressType.CommAddressTypeId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId WHERE vlfFirmwareChannels.FwChId = @fwChId ORDER BY vlfCommModeAddressType.CommAddressTypeId";

         DataSet dsResult = boxConfig.GetRowsBySql(sql, new SqlParameter[] { new SqlParameter("@fwChId", fwChId) });

         if (Util.IsDataSetValid(dsResult))
         {
            dsResult.Tables[0].TableName = "BoxCommunicationInfo";
            dsResult.DataSetName = "Box";
         }
         return dsResult;
      }

		/// <summary>
		/// Getcommunication address types by communication mode. 
		/// </summary>
		/// <param name="commModeId"></param>
		/// <returns>DataSet [CommAddressTypeId],[CommAddressTypeName]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetCommModeAddressTypesInfo(short commModeId)
		{
			DB.CommModeAddressType commModeAddressType = new DB.CommModeAddressType(sqlExec);
			DataSet dsResult = commModeAddressType.GetCommModeAddressTypesInfo(commModeId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxCommunicationInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

		/// <summary>
		/// Retrieves Protocol types info by protocol group
		/// </summary>
		/// <param name="boxProtocolGroupId"></param>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[Assembly],[ClassName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetProtocolTypesInfoByProtocolGroup(short boxProtocolGroupId)
		{
			DB.Dcl dcl = new DB.Dcl(sqlExec);
			DataSet dsResult = dcl.GetProtocolTypesInfoByProtocolGroup(boxProtocolGroupId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "ProtocolsInfo";
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}
		
		/// <summary>
		/// Retrieves communication mode name by id
		/// </summary>
		/// <param name="commModeId"></param>
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public string GetCommunicationModeName(short commModeId)
		{
			DB.CommMode commMode = new DB.CommMode(sqlExec);
			return commMode.GetNameById(commModeId);
		}

		/// <summary>
		/// Add new box communication info.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <param name="commAddressValue"></param>
		/// <exception cref="DASException">Thrown DASAppDataAlreadyExistsException if data already exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddCommInfo(int boxId, short commAddressTypeId, string commAddressValue)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				
                DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
			
				boxCommInfo.DeleteCommInfo(boxId, commAddressTypeId);
				
				boxCommInfo.AddCommInfo(boxId, commAddressTypeId, commAddressValue);
				// 7. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add communication information ", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}

		/// <summary>
		/// Add new box communication info.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="dsCommInfo"></param>
		/// <exception cref="DASException">Thrown DASAppDataAlreadyExistsException if data already exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddCommInfo(int boxId, DataSet dsCommInfo)
		{
			if(dsCommInfo == null || dsCommInfo.Tables.Count == 0) return;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
				foreach(DataRow ittr in dsCommInfo.Tables[0].Rows)
				{
					boxCommInfo.DeleteCommInfo(boxId, Convert.ToInt16(ittr["CommAddressTypeId"]));
				
					boxCommInfo.AddCommInfo(boxId,
											Convert.ToInt16(ittr["CommAddressTypeId"]),
											ittr["CommAddressValue"].ToString().Trim());
				}
				// 7. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add communication information ", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}

		/// <summary>
		/// Delete box.
		/// </summary>
		/// <remarks>
		/// 1. Delete all communications info related to the box.
		/// 2. Unassign box from the vehicle (if applicable)
		/// 3. Delete all vehicle assignments from the history related to the box
		/// 4. Delete user-define sensors
		/// 5. Delete user-define outputs
		/// 6. Delete all events related to the box
		/// 6. Delete all history information related to the box
		/// 7. Delete box
		/// </remarks>
		/// <param name="boxId"></param>
		/// <exception cref="DASException">Thrown DASAppDataAlreadyExistsException if data already exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeleteBox(int boxId)
		{
			int rowsAffected = 0;
			int commandTimeout = sqlExec.CommandTimeout;
			try
			{
				sqlExec.CommandTimeout = 600;
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
			
				// 2. Delete box all communications info
				DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
                rowsAffected += boxCommInfo.DeleteCommInfoByBoxId(boxId);

				// 3. Unassign box from the vehicle (if applicable)
				string licensePlate = "";
				DB.VehicleAssignment vehicleAssignment = new DB.VehicleAssignment(sqlExec);
				licensePlate = Convert.ToString(vehicleAssignment.GetVehicleAssignmentField("LicensePlate","BoxId",boxId));
				if((licensePlate != VLF.CLS.Def.Const.unassignedStrValue)&&(licensePlate != ""))
				{
					// 3.1. Retrieves vehicle assignment by license plate
					VehicAssign vehicAssign = vehicleAssignment.GetVehicleAssignmentVA(licensePlate);
				
					// 3.2. Try to unassign vehicle from all fleets
					DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
                    rowsAffected += fleetVehicles.PurgeVehicleFromAllFleets(vehicAssign.vehicleId);

					// 3.3. Delete old vehicle assignment info
                    rowsAffected += vehicleAssignment.DeleteVehicleAssignment(licensePlate);
				}

				// 4. Delete all vehicle assignments from the history related to the box
				DB.VehicleAssignmentHst vehicleAssignmentHst = new DB.VehicleAssignmentHst(sqlExec);
                rowsAffected += vehicleAssignmentHst.DeleteAllVehicleAssignmentsForBox(boxId);

				// 5. Delete user-define sensors
				DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
                rowsAffected += sensors.DeleteSensorsByBoxId(boxId);

				// 6. Delete user-define outputs
				DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
                rowsAffected += outputs.DeleteOutputsByBoxId(boxId);

				// 7. Delete all alarms related to the box
				DB.Alarm alarm = new DB.Alarm(sqlExec);
                rowsAffected += alarm.DeleteBoxAllAlarms(boxId, "");

				// 8. Delete box message in history
				DB.MsgIn msgIn = new DB.MsgIn(sqlExec);
                rowsAffected += msgIn.DeleteBoxAllMsgs(boxId, "vlfMsgIn", "");
                //rowsAffected += msgIn.DeleteBoxAllMsgs(boxId, "vlfMsgInHst", ""); has been removed by MV
                rowsAffected += msgIn.DeleteBoxAllMsgs(boxId, "vlfMsgInHstIgnored", "");

				DB.MsgOut msgOut = new DB.MsgOut(sqlExec);
                rowsAffected += msgOut.DeleteBoxAllMsgs(boxId, "vlfMsgOut", "");
                rowsAffected += msgOut.DeleteBoxAllMsgs(boxId, "vlfMsgOutHst", "");

				// 8. Delete box text messages 
				DB.TxtMsgs txtMsg = new DB.TxtMsgs(sqlExec);
                rowsAffected += txtMsg.DeleteBoxAllMsgs(boxId, "");
				
				// 9. Delete box messges
				DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
                rowsAffected += boxMsgSeverity.DeleteRecordByBoxId(boxId);

				// 10. Delete box settings
				DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
                rowsAffected += boxSettings.DeleteBoxSettings(boxId);

				// 11. Delete box map usage
				rowsAffected += mapEngine.DeleteBoxMapUsage(boxId, "");

                // 12. Delete tasks
                rowsAffected += commandScheduler.DeleteTasksByBoxId(boxId);

				// 13. Delete box
				DB.Box box = new DB.Box(sqlExec);
				rowsAffected = box.DeleteRecord(boxId);

				// 14. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
                // Rollback all changes
                rowsAffected = 0;
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Cannot delete box ID " + boxId.ToString(), objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
                // Rollback all changes
                sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
                // Rollback all changes
                rowsAffected = 0;
				sqlExec.RollbackTransaction();
                throw new DASException("Cannot delete box ID " + boxId.ToString() + " " + objException.Message);
			}
			finally
			{
				sqlExec.CommandTimeout = commandTimeout;
			}
			return rowsAffected;
		}

        /// <summary>
        /// Add new firmware.
        /// </summary>
        /// <param name="boxHwTypeId"></param>
        /// <param name="fwName"></param>
        /// <param name="fwTypeId"></param>
        /// <param name="fwLocalPath"></param>
        /// <param name="fwOAPPath"></param>
        /// <param name="fwDateReleased"></param>
        /// <param name="maxGeozones"></param>
        /// <param name="oAPPort"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddFirmware(short boxHwTypeId, string fwName, short fwTypeId, string fwLocalPath, string fwOAPPath, string fwDateReleased, int maxGeozones, short oAPPort)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            boxConfig.AddFirmware(boxHwTypeId, fwName, fwTypeId, fwLocalPath, fwOAPPath, fwDateReleased, maxGeozones, oAPPort);
        }

        /// <summary>
        /// Add new firmware overloaded - includes feature mask
        /// </summary>
        /// <param name="boxHwTypeId"></param>
        /// <param name="fwName"></param>
        /// <param name="fwTypeId"></param>
        /// <param name="fwLocalPath"></param>
        /// <param name="fwOAPPath"></param>
        /// <param name="fwDateReleased"></param>
        /// <param name="maxGeozones"></param>
        /// <param name="oAPPort"></param>
        /// <param name="featureMask"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AddFirmware(short boxHwTypeId, string fwName, short fwTypeId, string fwLocalPath, string fwOAPPath, 
           string fwDateReleased, int maxGeozones, short oAPPort, long featureMask)
        {
           DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
           boxConfig.AddFirmware(boxHwTypeId, fwName, fwTypeId, fwLocalPath, fwOAPPath, fwDateReleased, maxGeozones, oAPPort, featureMask);
        }

        /// <summary>
        /// Update firmware info.
        /// </summary>
        /// <param name="fwId"></param>
        /// <param name="fwLocalPath"></param>
        /// <param name="fwOAPPath"></param>
        /// <param name="maxGeozones"></param>
        /// <param name="oAPPort"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if vehicle with vin number alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateFirmwareInfo(short fwId, string fwLocalPath, string fwOAPPath, int maxGeozones, short oAPPort)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            boxConfig.UpdateFirmwareInfo(fwId, fwLocalPath, fwOAPPath, maxGeozones, oAPPort);
        }
		
        /// <summary>
        /// Add new firmware configuration
        /// </summary>
        /// <param name="tblChannels">Table contains 1 or 2 rows - primary channel(mandatory) and secondary channel</param>
        /// <exception cref="DASAppInvalidValueException">Thrown in in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <returns>True if configuration was successfully added, otherwise - false</returns>
      public bool AddFirmwareCfg(DataTable tblChannels)
      {
         if (tblChannels == null || tblChannels.Rows.Count == 0)
            return false;

         try
         {
            // short chId, short fwId, short chPriority
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet ds = new DataSet();
            ds.Tables.Add(tblChannels);
            short fwchid = boxConfig.GetFirmwareChannelIdByChannels(ds);
            if (fwchid > 0) return false; // configuration found

            /*
            // 0. Bring all the channels for the firmware
            // Table structure [ FwChId, ChId, ChName, FwId, ChPriority ]
            DataSet dsConfig = GetAllChannelsByFwId(Convert.ToInt16(tblChannels.Rows[0]["FwId"]));
            if (Util.IsDataSetValid(dsConfig))
            {
               bool identical = false;
               string primaryFilter = "", secondaryFilter = "";

               // 1. Find a prim. channel - it's the 1st row
               primaryFilter =
                  String.Format("FwId = {0} AND ChId = {1} AND ChPriority = 0", tblChannels.Rows[0]["FwId"], tblChannels.Rows[0]["ChId"]);
               DataRow[] prDBRows = dsConfig.Tables[0].Select(primaryFilter);

               // if the primary is different - doesn't exist
               if (prDBRows.Length > 0)
               {
                  // 2. Single pr. channel is being added
                  if (tblChannels.Rows.Count == 1)
                  {
                     // check if any sec. channel with the same FwChId exists for each primary
                     foreach (DataRow drDBPrimary in prDBRows)
                     {
                        secondaryFilter = String.Format("FwChId = {0} AND ChPriority = 1", drDBPrimary["FwChId"]);

                        DataRow[] secDBRows = dsConfig.Tables[0].Select(secondaryFilter);

                        // if none found - already exists
                        if (secDBRows.Length == 0)
                        {
                           identical = true;
                           break;
                        }
                     }
                  }
                  // 3. Pair of the channels is being added
                  else
                  {
                     // check if the same sec. channel exists for each primary until found
                     foreach (DataRow drDBPrimary in prDBRows)
                     {
                        secondaryFilter = String.Format("FwChId = {0} AND ChId = {1} AND ChPriority = 1",
                           drDBPrimary["FwChId"], tblChannels.Rows[1]["ChId"]);

                        DataRow[] secDBRows = dsConfig.Tables[0].Select(secondaryFilter);

                        // if one found - already exists
                        if (secDBRows.Length == 1)
                        {
                           identical = true;
                           break;
                        }
                     }
                  }
               }
               // config. exists
               if (identical) return false;
            }
            */

            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // get next available fwchid
            short fwChId = boxConfig.AddFwChId();

            // add 1 or 2 channels with a new fwchid
            foreach (DataRow ittr in tblChannels.Rows)
            {
               boxConfig.AddFirmwareCfg(fwChId,
                  Convert.ToInt16(ittr["ChId"]),
                  Convert.ToInt16(ittr["FwId"]),
                  Convert.ToInt16(ittr["ChPriority"]));
            }
            // 9. Save all changes
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
         return true;
      }
 
        /// <summary>
        /// Delete Fw Id.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="fwId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteFirmware(short fwId)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            return boxConfig.DeleteFirmware(fwId);
        }
        
        /// <summary>
        /// Deletes firmware channel
        /// </summary>
        /// <param name="chId"></param>
        /// <param name="fwId"></param>
        /// <returns>rows affected</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteFirmwareCfg(short fwChId)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            return boxConfig.DeleteFirmwareCfg(fwChId);
        }
        
        /// <summary>
        /// Get firmwar channel info
        /// </summary>
        /// <param name="fwId"></param>
        /// <param name="commMode"></param>
        // <returns>DataSet [FwChId],[ChId],[FwId],[ChPriority]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetPrimaryFirmwareChannelInfo(short fwId, short commMode)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet dsResult = boxConfig.GetPrimaryFirmwareChannelInfo(fwId, commMode);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetPrimaryFirmwareChannelInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        #endregion BoxConfig Interfaces

        # region Hardware Type Interfaces
        /// <summary>
        /// Get all hardware types information. 
        /// </summary>
        /// <returns>DataSet[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet HardwareTypes()
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet dsResult = boxConfig.HardwareTypes();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "AllConfigInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
        /// Adds a new hardware type
        /// </summary>
        /// <param name="hwTypeName" type="string"></param>
        /// <param name="maxSensors" type="short"></param>
        /// <param name="maxOutputs" type="short"></param>
        /// <param name="listOutputs" type="ArrayList">List of HardwareOutput objects</param>
        /// <returns>A new int hw type id</returns>
      [Obsolete("Becomes obsolete since using sensor profiles")]
        public short AddHardwareType(string hwTypeName, short maxSensors, short maxOutputs, ArrayList listOutputs, ArrayList listSensors)
        {
            short newId;
            try
            {
                BoxHwType boxHwType = new BoxHwType(sqlExec);
                BoxHwDefaultOutputsCfg defOutput = new BoxHwDefaultOutputsCfg(sqlExec);
                BoxHwDefaultSensorsCfg defSensor = new BoxHwDefaultSensorsCfg(sqlExec);

                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                // add a new record to the table vlfBoxHwType and get a new id
                newId = (short)boxHwType.AddRecord(hwTypeName, Convert.ToSByte(maxSensors), Convert.ToSByte(maxOutputs));

                // add outputs to the table vlfBoxHwDefaultOutputsCfg
                foreach (HardwareOutput item in listOutputs)
                {
                    defOutput.AddOutput(newId, item.outputID, item.outputName, item.outputAction);
                }

                // add sensors to the table vlfBoxHwDefaultSensorsCfg
                foreach (HardwareSensor item in listSensors)
                {
                    defSensor.AddSensor(newId, item.sensorID, item.sensorName, item.sensorAction,
                        item.sensorAlarmOn, item.sensorAlarmOff);
                }

                sqlExec.CommitTransaction();
            }
            catch (Exception exc)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbException("Add Hardware Type Failed\n" + exc.Message);
            }
            return newId;
        }
        
        /// <summary>
        /// Adds a new hardware type
        /// </summary>
        /// <param name="hwTypeName" type="string"></param>
        /// <param name="maxSensors" type="short"></param>
        /// <param name="maxOutputs" type="short"></param>
        /// <param name="listOutputs" type="ArrayList">List of HardwareOutput objects</param>
        /// <returns>A new int hw type id</returns>
        public short AddHardwareType(string hwTypeName, short maxSensors, short maxOutputs, ArrayList listOutputs)
        {
           short newId;
           try
           {
              BoxHwType boxHwType = new BoxHwType(sqlExec);
              BoxHwDefaultOutputsCfg defOutput = new BoxHwDefaultOutputsCfg(sqlExec);

              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

              // add a new record to the table vlfBoxHwType and get a new id
              newId = (short)boxHwType.AddRecord(hwTypeName, Convert.ToSByte(maxSensors), Convert.ToSByte(maxOutputs));

              // add outputs to the table vlfBoxHwDefaultOutputsCfg
              foreach (HardwareOutput item in listOutputs)
              {
                 defOutput.AddOutput(newId, item.outputID, item.outputName, item.outputAction);
              }

              sqlExec.CommitTransaction();
           }
           catch (Exception exc)
           {
              sqlExec.RollbackTransaction();
              throw new DASDbException("Add Hardware Type Failed\n" + exc.Message);
           }
           return newId;
        }

        /// <summary>
        /// Update a hardware type
        /// </summary>
        /// <param name="hwTypeId" type="string"></param>
        /// <param name="hwTypeName" type="string"></param>
        /// <param name="maxSensors" type="short"></param>
        /// <param name="maxOutputs" type="short"></param>
        /// <param name="listOutputs" type="ArrayList">List of HardwareOutput objects</param>
        /// <param name="listSensors" type="ArrayList">List of HardwareSensor objects</param>
      [Obsolete("Becomes obsolete since using sensor profiles")]
        public void UpdateHardwareType(short hwTypeId, string hwTypeName, short maxSensors, short maxOutputs,
           ArrayList listOutputs, ArrayList listSensors)
        {
           try
           {
#if debug
         Util.BTrace(Util.INF0, "-- System.UpdateHardwareType({0})", hwTypeId);
# endif
              BoxHwType boxHwType = new BoxHwType(sqlExec);
              BoxHwDefaultOutputsCfg defOutput = new BoxHwDefaultOutputsCfg(sqlExec);
              BoxHwDefaultSensorsCfg defSensor = new BoxHwDefaultSensorsCfg(sqlExec);

              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@hwTypeName", hwTypeName);
              sqlParams[1] = new SqlParameter("@maxSensors", maxSensors);
              sqlParams[2] = new SqlParameter("@maxOutputs", maxOutputs);
              sqlParams[3] = new SqlParameter("@hwTypeId", hwTypeId);
              boxHwType.UpdateRow(
                 "SET BoxHwTypeName = @hwTypeName, MaxSensorsNum = @maxSensors, MaxOutputsNum = @maxOutputs WHERE BoxHwTypeId = @hwTypeId",
                 sqlParams);

              if (listOutputs != null && listOutputs.Count > 0)
              {
                 // delete outputs from the table vlfBoxHwDefaultOutputsCfg
                 defOutput.DeleteOutputsByHwTypeId(hwTypeId);

                 // add outputs to the table vlfBoxHwDefaultOutputsCfg
                 //DataSet dsResult = new DataSet();
                 foreach (HardwareOutput item in listOutputs)
                 {
                    // try to get output from the table
                    //dsResult = defOutput.GetRowsByPrimaryKey("BoxHwTypeId", hwTypeId, "OutputId", item.outputID);
                    // add only if an output is not there
                    //if (!Util.IsDataSetValid(dsResult)) 
                    defOutput.AddOutput(hwTypeId, item.outputID, item.outputName, item.outputAction);
                 }
              }

              if (listSensors != null && listSensors.Count > 0)
              {
                 // delete sensors from the table vlfBoxHwDefaultSensorsCfg
                 defSensor.DeleteSensorsByHwTypeId(hwTypeId);

                 // add sensors to the table vlfBoxHwDefaultSensorsCfg
                 foreach (HardwareSensor item in listSensors)
                 {
                    // try to get sensor from the table
                    //dsResult = defSensor.GetRowsByPrimaryKey("BoxHwTypeId", hwTypeId, "SensorId", item.sensorID);
                    // add only if a sensor is not there
                    //if (!Util.IsDataSetValid(dsResult))
                    defSensor.AddSensor(hwTypeId, item.sensorID, item.sensorName, item.sensorAction, item.sensorAlarmOn, item.sensorAlarmOff);
                 }
              }

              sqlExec.CommitTransaction();
           }
           catch (Exception exc)
           {
              sqlExec.RollbackTransaction();
              throw new DASDbException("Update Hardware Type Failed\n" + exc.Message);
           }
        }

        /// <summary>
        /// Update a hardware type
        /// </summary>
        /// <param name="hwTypeId" type="string"></param>
        /// <param name="hwTypeName" type="string"></param>
        /// <param name="maxSensors" type="short"></param>
        /// <param name="maxOutputs" type="short"></param>
        /// <param name="listOutputs" type="ArrayList">List of HardwareOutput objects</param>
        /// <param name="listSensors" type="ArrayList">List of HardwareSensor objects</param>
        public void UpdateHardwareType(short hwTypeId, string hwTypeName, short maxSensors, short maxOutputs,
           ArrayList listOutputs)
        {
           try
           {
#if debug
         Util.BTrace(Util.INF0, "-- System.UpdateHardwareType({0})", hwTypeId);
# endif
              BoxHwType boxHwType = new BoxHwType(sqlExec);
              BoxHwDefaultOutputsCfg defOutput = new BoxHwDefaultOutputsCfg(sqlExec);
              BoxHwDefaultSensorsCfg defSensor = new BoxHwDefaultSensorsCfg(sqlExec);

              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

              SqlParameter[] sqlParams = new SqlParameter[4];
              sqlParams[0] = new SqlParameter("@hwTypeName", hwTypeName);
              sqlParams[1] = new SqlParameter("@maxSensors", maxSensors);
              sqlParams[2] = new SqlParameter("@maxOutputs", maxOutputs);
              sqlParams[3] = new SqlParameter("@hwTypeId", hwTypeId);
              boxHwType.UpdateRow(
                 "SET BoxHwTypeName = @hwTypeName, MaxSensorsNum = @maxSensors, MaxOutputsNum = @maxOutputs WHERE BoxHwTypeId = @hwTypeId",
                 sqlParams);

              if (listOutputs != null && listOutputs.Count > 0)
              {
                 // delete outputs from the table vlfBoxHwDefaultOutputsCfg
                 defOutput.DeleteOutputsByHwTypeId(hwTypeId);

                 // add outputs to the table vlfBoxHwDefaultOutputsCfg
                 //DataSet dsResult = new DataSet();
                 foreach (HardwareOutput item in listOutputs)
                 {
                    // try to get output from the table
                    //dsResult = defOutput.GetRowsByPrimaryKey("BoxHwTypeId", hwTypeId, "OutputId", item.outputID);
                    // add only if an output is not there
                    //if (!Util.IsDataSetValid(dsResult)) 
                    defOutput.AddOutput(hwTypeId, item.outputID, item.outputName, item.outputAction);
                 }
              }
              sqlExec.CommitTransaction();
           }
           catch (Exception exc)
           {
              sqlExec.RollbackTransaction();
              throw new DASDbException("Update Hardware Type Failed\n" + exc.Message);
           }
        }

        /// <summary>
        /// Deletes hardware type with related outputs and sensors
        /// </summary>
        /// <param name="hwTypeId" type="short">Hardware Type ID</param>
        /// <returns>Rows affected</returns>
        public int DeleteHardwareType(short hwTypeId)
        {
            int rowsAffected = 0;
            try
            {
                BoxHwType boxHwType = new BoxHwType(sqlExec);
                BoxHwDefaultOutputsCfg defOutput = new BoxHwDefaultOutputsCfg(sqlExec);
                BoxHwDefaultSensorsCfg defSensor = new BoxHwDefaultSensorsCfg(sqlExec);

                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                // delete all the outputs
                rowsAffected += defOutput.DeleteOutputsByHwTypeId(hwTypeId);

                // delete all the sensors
                rowsAffected += defSensor.DeleteSensorsByHwTypeId(hwTypeId);

                // delete the hardware type
                rowsAffected += boxHwType.DeleteRecord(hwTypeId);

                sqlExec.CommitTransaction();
            }
            catch (Exception exc)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbException("Add Hardware Type Failed\n" + exc.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Deletes hardware type with related outputs and sensors
        /// </summary>
        /// <param name="hwTypeName" type="string">Hardware Type Name</param>
        /// <returns>Rows affected</returns>
        public int DeleteHardwareType(string hwTypeName)
        {
            int rowsAffected = 0;
            try
            {
               BoxHwType boxHwType = new BoxHwType(sqlExec);
               //BoxHwDefaultOutputsCfg defOutput = new BoxHwDefaultOutputsCfg(sqlExec);
               //BoxHwDefaultSensorsCfg defSensor = new BoxHwDefaultSensorsCfg(sqlExec);

               // get the type id
               DeleteHardwareType(boxHwType.GetIdByName(hwTypeName.Trim()));
               //sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

               // delete all the outputs
               //rowsAffected += defOutput.DeleteOutputsByHwTypeId(hwTypeId);

               // delete all the sensors
               //rowsAffected += defSensor.DeleteSensorsByHwTypeId(hwTypeId);

               // delete the hardware type
               //rowsAffected += boxHwType.DeleteRecord(hwTypeId);

               //sqlExec.CommitTransaction();
            }
            catch (Exception exc)
            {
               sqlExec.RollbackTransaction();
               throw new DASDbException("Delete Hardware Type Failed\n" + exc.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Retrieves output info by Hw type
        /// </summary>
        /// <param name="boxHwTypeId"></param> 
        /// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetDefaultOutputsInfoByHwTypeId(short boxHwTypeId)
        {
            DB.BoxHwDefaultOutputsCfg defOutputs = new DB.BoxHwDefaultOutputsCfg(sqlExec);
            DataSet dsResult = defOutputs.GetOutputsInfoByHwTypeId(boxHwTypeId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "BoxOutputsInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves all outputs info
        /// </summary>
        /// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
        public DataSet GetOutputsInfo()
        {
            DB.BoxHwDefaultOutputsCfg defOutputs = new DB.BoxHwDefaultOutputsCfg(sqlExec);
            DataSet dsResult = defOutputs.GetAllOutputs();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OutputsInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves sensors info by box HW type
        /// </summary>
        /// <param name="boxHwTypeId"></param> 
        /// <returns>DataSet [SensorId],[SensorName],[SensorAction],[AlarmLevelOn],[AlarmLevelOff]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetDefaultSensorsInfoByHwTypeId(short boxHwTypeId)
        {
            BoxHwDefaultSensorsCfg defSensors = new DB.BoxHwDefaultSensorsCfg(sqlExec);
            DataSet dsResult = defSensors.GetSensorsInfoByHwTypeId(boxHwTypeId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "BoxSensorsInfo";
                }
                dsResult.DataSetName = "System";
            }

            return dsResult;
        }

        /// <summary>
        /// Retrieves all sensors info
        /// </summary>
        /// <param name="alarms" type="boolean">Include or not default alarm levels on and off</param>
        /// <returns>DataSet [SensorId][SensorName][SensorAction]</returns>
      public DataSet GetSensorsInfo(bool alarms)
        {
           DB.Sensor defSensors = new DB.Sensor(sqlExec);
            DataSet dsResult = defSensors.GetAllSensors(alarms);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "SensorsInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves box hardware type id by name from "vlfBoxHwType" table
        /// </summary>
        /// <param name="boxHwTypeName"></param>
        /// <returns>short box Hw type Id</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public short GetHwTypeIdByName(string boxHwTypeName)
        {
            DB.BoxHwType boxHwType = new DB.BoxHwType(sqlExec);
            return boxHwType.GetIdByName(boxHwTypeName);
        }

        # endregion

        #region DCL Interfaces
      public short GetDclCommMode(short dclId)
      {
         return dcl.GetDclCommMode(dclId);
      }
        /// <summary>
		/// Retrieves list of DCL Id by name from "vlfDcl" table
		/// </summary>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <returns> list of dcl ids as list of short int</returns>
		public SortedList GetDclIdList()
		{
			return dcl.GetDclIdList();
		}
		/// <summary>
		/// Retrieves DCL Id by name from "vlfDcl" table
		/// </summary>
		/// <param name="dclName"></param>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <returns>dcl id in as short</returns>
		public short GetDclIdByName(string dclName)
		{
			return dcl.GetDclIdByName(dclName);
		}
		/// <summary>
		/// Retrieves DCL Status by name from "vlfDcl" table
		/// </summary>
		/// <param name="dclName"></param>
		/// <returns>DCL status <see cref="VLF.CLS.Def.Enums.ServiceState"/>.</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public VLF.CLS.Def.Enums.ServiceState GetDclStatusByName(string dclName)
		{
			return dcl.GetDclStatusByName(dclName);
		}
		/// <summary>
		/// Retrieves DCL ids by service state
		/// </summary>
		/// <param name="serviceState"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>DataSet [DclId],[DclName]</returns>
		public DataSet GetDclInfoByStatus(VLF.CLS.Def.Enums.ServiceState serviceState)
		{
			return dcl.GetDclInfoByStatus(serviceState);
		}
		/// <summary>
		/// Add new DCL type.
		/// </summary>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>new dcl id</returns>
		/// <param name="commModeId"></param>
		/// <param name="boxProtocolGroupId"></param>
		/// <param name="dclName"></param>
		/// <param name="description"></param>
		/// <param name="serviceState"></param>
		public short AddDcl(VLF.CLS.Def.Enums.CommMode commModeId,short boxProtocolGroupId,string dclName,string description,VLF.CLS.Def.Enums.ServiceState serviceState)
		{
			return dcl.AddDcl(commModeId,boxProtocolGroupId,dclName,description,serviceState);
		}

		/// <summary>
		/// Update DCL type info.
		/// </summary>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>rows affected</returns>
		/// <param name="dclId"></param>
		/// <param name="commModeId"></param>
		/// <param name="boxProtocolGroupId"></param>
		/// <param name="description"></param>
		/// <param name="serviceState"></param>
		/// <param name="pid"></param>
		public void UpdateDclInfo(short dclId,VLF.CLS.Def.Enums.CommMode commModeId,short boxProtocolGroupId,string description,VLF.CLS.Def.Enums.ServiceState serviceState,short pid)
		{
			dcl.UpdateDclInfo(dclId,commModeId,boxProtocolGroupId,description,serviceState,pid);
		}
		/// <summary>
		/// Delete dcl by id
		/// </summary>
		/// <param name="dclID"></param>
		/// <returns></returns>
		public int DeleteDcl( short dclID )
		{
			return dcl.DeleteDcl(dclID);
		}

		/// <summary>
		/// Cleanup all dcls related info
		/// </summary>
		/// <returns>total number of deleted dcls</returns>
		public int CleanupAllDclsRelatedInfo()
		{
			int rowAffected = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				
				// 2. Cleanup all out messages 
				DB.MsgOut msgOut = new DB.MsgOut(sqlExec);
				msgOut.DeleteAllRecords();

				// 3. Cleanup all dcls info
				rowAffected = dcl.DeleteAllRecords();
				
				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to delete dcls related information ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}			
			return rowAffected;
		}

		#endregion DCL Interfaces

#if ASL_
		#region ASL Interfaces
		/// <summary>
		/// Delete all ACLs.
		/// </summary>
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public virtual int DeleteAllAsls()
		{
			return asl.DeleteAllRecords();
		}
		/// <summary>
		/// Add new ASL type.
		/// Return new ASL id.
		/// </summary>
		/// <param name="aslType"></param>
		/// <param name="aslName"></param>
		/// <param name="description"></param>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <returns> new asl id </returns>
		public short AddAsl(short aslType,string aslName,string description)
		{
			return asl.AddAsl(aslType,aslName,description);
		}
		#endregion ASL Interfaces
#endif
		#region Vehicle Configuration Interfaces
		/// <summary>
		/// Assign make to model
		/// </summary>
		/// <param name="makeId"></param>
		/// <param name="modelId"></param>
		/// <param name="makeName"></param>
		/// <param name="modelName"></param>
		/// <returns></returns>
		public int AssignModelToMake(int makeId,int modelId, string makeName, string modelName)
		{
			int makeModelId = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				if(makeId == VLF.CLS.Def.Const.unassignedIntValue)
				{
					DB.Make make = new DB.Make(sqlExec);
					makeId = make.AddMake(makeName.TrimEnd());
				}
			
				if(modelId == VLF.CLS.Def.Const.unassignedIntValue)
				{
					DB.Model model = new DB.Model(sqlExec);
					modelId = model.AddModel(modelName.TrimEnd());
				}
			
				DB.MakeModel makeModel = new DB.MakeModel(sqlExec);
				makeModelId = makeModel.AddMakeModel(makeId,modelId);
				// 7. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add communication information ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}	
			return makeModelId;
		}
		/// <summary>
		/// Retrieves make name by id from "vlfMake" table
		/// </summary>
		/// <param name="makeId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public string GetMakeNameById(int makeId)
		{
			DB.Make make = new DB.Make(sqlExec);
			return make.GetMakeNameById(makeId);
		}
		/// <summary>
		/// Retrieves all makes names.
		/// </summary>
		/// <returns>ArrayList [string]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public ArrayList GetAllMakesNames()
		{
			DB.Make make = new DB.Make(sqlExec);
			return make.GetAllMakesNames();
		}

		/// <summary>
		/// Retrieves all makes information.
		/// </summary>
		/// <returns>DataSet [MakeId],[MakeName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllMakesInfo()
		{
			DB.Make make = new DB.Make(sqlExec);
			DataSet dsResult = make.GetAllRecords();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllMakes" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

		/// <summary>
		/// Retrieves all model names related to specific make.
		/// </summary>
		/// <param name="makeId"></param>
		/// <returns>ArrayList [string]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public ArrayList GetModelNamesByMakeId(int makeId)
		{
			DB.MakeModel makeModel = new DB.MakeModel(sqlExec);
			return makeModel.GetModelNamesByMakeId(makeId);
		}	
		/// <summary>
		/// Retrieves all model related to specific make.
		/// </summary>
		/// <param name="makeId"></param>
		/// <returns>DataSet [ModelId],[ModelName],[MakeModelId]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetModelsInfoByMakeId(int makeId)
		{
			DB.MakeModel makeModel = new DB.MakeModel(sqlExec);
			DataSet dsResult = makeModel.GetModelsInfoByMakeId(makeId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllMakeModels" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}	

		/// <summary>
		/// Retrieves box Protocol type name by id from "vlfModel" table
		/// </summary>
		/// <param name="modelId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public string GetModelNameById(int modelId)
		{
			DB.Model model = new DB.Model(sqlExec);
			return model.GetModelNameById(modelId);
		}
		/// <summary>
		/// Retrieves all States/Provinces.
		/// </summary>
		/// <returns>DataSet [StateProvinceName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllStateProvinces()
		{
			DB.StateProvince stateProvince = new DB.StateProvince(sqlExec);
			DataSet dsResult = stateProvince.GetAllRecords();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllStatesProvinces" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}	

		/// <summary>
		/// Retrieves all vehicle types.
		/// </summary>
		/// <returns>DataSet [VehicleTypeId],[VehicleTypeName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllVehicleTypes()
		{
			DB.VehicleType vehicleType = new DB.VehicleType(sqlExec);
			DataSet dsResult = vehicleType.GetAllRecords();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllVehicleTypes" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}	

		/// <summary>
		/// Retrieves icons information
		/// </summary>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>DataSet [IconTypeId],[IconTypeName]</returns>
		public DataSet GetIconsInfo()
		{
			DB.IconType iconType = new DB.IconType(sqlExec);
			DataSet dsResult = iconType.GetIconsInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "IconTypes" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}	
		#endregion

		#region Standard Preferences Interfaces
		/// <summary>
		/// Add new preference.
		/// </summary>
		/// <param name="preferenceName"></param>
		/// <param name="preferenceRule"></param>
		/// <returns>int next preference id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if preference alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int AddPreference(string preferenceName,string preferenceRule)
		{
			return preference.AddPreference(preferenceName,preferenceRule);
		}		
		/// <summary>
		/// Delete existing preference.
		/// </summary>
		/// <returns>int</returns>
		/// <param name="preferenceName"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if preference does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeletePreferenceByPreferenceName(string preferenceName)
		{
			return preference.DeletePreferenceByPreferenceName(preferenceName);
		}
		/// <summary>
		/// Delete existing preference.
		/// </summary>
		/// <returns>int</returns>
		/// <param name="preferenceId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if preference id does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeletePreferenceByPreferenceId(int preferenceId)
		{
			return preference.DeletePreferenceByPreferenceId(preferenceId);
		}		
		/// <summary>
		/// Retrieves Preference info
		/// </summary>
		/// <returns>DataSet [PreferenceId], [PreferenceName],[PreferenceRule]</returns>
		/// <param name="preferenceId"></param> 
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetPreferenceInfo(int preferenceId)
		{
			DataSet dsResult = preference.GetPreferenceInfo(preferenceId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "PreferenceInfo" ;
				}
				dsResult.DataSetName = "User";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves Preference info
		/// </summary>
		/// <returns>DataSet [PreferenceId], [PreferenceName],[PreferenceRule]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllPreferencesInfo()
		{
			DataSet dsResult = preference.GetAllRecords();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllPreferencesInfo" ;
				}
				dsResult.DataSetName = "User";
			}
			return dsResult;
		}
		#endregion

		#region System Updates
		/// <summary>
		/// Add new system update
		/// </summary>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int AddSystemUpdate(DateTime systemUpdateDateTime,
									string msg,string msgFr,
									VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType,
									VLF.CLS.Def.Enums.AlarmSeverity severity,string FontColor,Int16 FontBold)
		{
			DB.SystemUpdates sysUpdate = new DB.SystemUpdates(sqlExec);
            return sysUpdate.AddSystemUpdate(systemUpdateDateTime, msg,msgFr, systemUpdateType, severity, FontColor, FontBold);
		}

        /// <summary>
        /// update system update table
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int UpdateSystemUpdateTable(int MsgId,DateTime systemUpdateDateTime,
                                    string msg,string msgFr,
                                    VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType,
                                    VLF.CLS.Def.Enums.AlarmSeverity severity, string FontColor, Int16 FontBold)
        {
            DB.SystemUpdates sysUpdate = new DB.SystemUpdates(sqlExec);
            return sysUpdate.UpdateSystemUpdateTable(MsgId,systemUpdateDateTime, msg,msgFr, systemUpdateType, severity, FontColor, FontBold);
        }

		/// <summary>
		/// Retrieves system updates.
		/// </summary>
		/// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
		/// <param name="userId"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="systemUpdateType"></param>
		public DataSet GetSystemUpdates(int userId,DateTime from,DateTime to,VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType)
		{
			DB.SystemUpdates sysUpdate = new DB.SystemUpdates(sqlExec);
			DataSet dsResult = sysUpdate.GetSystemUpdates(userId,from,to,systemUpdateType);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "SystemUpdatesInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult; 
		}

        /// <summary>
        /// Retrieves system updates.
        /// </summary>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        public DataSet GetFullInfoSystemUpdates(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType)
        {
            DB.SystemUpdates sysUpdate = new DB.SystemUpdates(sqlExec);
            DataSet dsResult = sysUpdate.GetFullInfoSystemUpdates(userId, from, to, systemUpdateType);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "SystemUpdatesInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves system updates.
        /// </summary>
        /// <returns>DataSet [MsgId],[Msg],[SystemUpdateType],[AlarmLevel]</returns>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="systemUpdateType"></param>
        public DataSet GetSystemUpdatesByLang(int userId, DateTime from, DateTime to, VLF.CLS.Def.Enums.SystemUpdateType systemUpdateType, string lang)
        {
            DB.SystemUpdates sysUpdate = new DB.SystemUpdates(sqlExec);
            DataSet dsResult = sysUpdate.GetSystemUpdatesByLang(userId, from, to, systemUpdateType, lang);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "SystemUpdatesInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }
		/// <summary>
		/// Delete system update.
		/// </summary>
		public int DeleteSystemUpdate(int msgId)
		{
			DB.SystemUpdates sysUpdate = new DB.SystemUpdates(sqlExec);			
			return sysUpdate.DeleteSystemUpdate(msgId);
		}
		#endregion

		#region System Configuration
		/// <summary>
		/// Retrieves configuration module types info
		/// </summary>
		/// <returns>DataSet [ModuleId],[ModuleName],[PSW],[IPAddress],[UserName],[Enabled],[MachineName]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <param name="typeId"></param>
		/// <param name="configurationModuleState"></param>
		public DataSet GetConfigurationModuleTypesInfo(short typeId,VLF.CLS.Def.Enums.ConfigurationModuleState configurationModuleState)
		{
			DataSet dsResult = sysCfg.GetConfigurationModuleTypesInfo(typeId,configurationModuleState);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "ConfigurationModuleTypesInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves configuration module type id
		/// </summary>
		/// <param name="moduleTypeName"></param>
		/// <returns>module id</returns>
		/// <remarks>If module does not exist, return VLF.CLS.Def.Const.unassignedShortValue,
		/// otherwise return module id</remarks>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public short GetConfigurationModuleTypeId(string moduleTypeName)
		{
			return sysCfg.GetConfigurationModuleTypeId(moduleTypeName);
		}
		/// <summary>
		/// Retrieves configuration value
		/// </summary>
		/// <returns>string [KeyValue]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <param name="moduleId"></param>
		/// <param name="cfgGroupId"></param>
		/// <param name="keyName"></param>
		public string GetConfigurationValue(short moduleId,short cfgGroupId,string keyName)
		{
			return sysCfg.GetConfigurationValue(moduleId,cfgGroupId,keyName);
		}


     
		/// <summary>
		/// Retrieves module configuration
		/// </summary>
		/// <param name="moduleId"></param>
		/// <returns>DataSet [CfgGroupId],[CfgGroupName],[KeyName],[KeyValue]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public DataSet GetModuleConfiguration(short moduleId)
		{
			DataSet dsResult = sysCfg.GetModuleConfiguration(moduleId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "ModuleConfiguration" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves module configuration structure
		/// </summary>
		/// <param name="typeId"></param>
		/// <returns>DataSet [CfgGroupId],[KeyName],[KeyValue],[CfgGroupName]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public DataSet GetModuleConfigurationStructure(short typeId)
		{
			DataSet dsResult = sysCfg.GetModuleConfigurationStructure(typeId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetModuleConfigurationStructure" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves module type groups.
		/// </summary>
		/// <param name="typeId"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if module already exists.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>DataSet [CfgGroupId],[CfgGroupName]</returns>
		public DataSet GetModuleTypeGroups(short typeId)
		{
			DataSet dsResult = sysCfg.GetModuleTypeGroups(typeId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetModuleTypeGroups" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Add new module.
		/// </summary>
		/// <remarks>
		/// dsModuleSettings structure:
		/// [CfgGroupId]	short,
		/// [KeyName]		string,
		/// [KeyValue]		string
		/// </remarks>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if module already exists.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <param name="typeId"></param>
		/// <param name="moduleName"></param>
		/// <param name="enabled"></param>
		/// <param name="dsModuleSettings"></param>
		public short AddModule(short typeId,string moduleName,bool enabled,DataSet dsModuleSettings)
		{
			short moduleId = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Add new box
				moduleId = sysCfg.AddModule(typeId,moduleName,enabled);
				// 3. Setup settings for new module
				if(dsModuleSettings != null && dsModuleSettings.Tables.Count > 0)
				{
					foreach(DataRow ittr in dsModuleSettings.Tables[0].Rows)
					{
						sysCfg.AddModuleSettings(moduleId,
												Convert.ToInt16(ittr["CfgGroupId"]),
												ittr["KeyName"].ToString(),
												ittr["KeyValue"].ToString());
					}
				}
				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
				moduleId = VLF.CLS.Def.Const.unassignedIntValue;
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add new module ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
				moduleId = VLF.CLS.Def.Const.unassignedIntValue;
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
			return moduleId;
		}
		/// <summary>
		/// Update module status.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="enable"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if module does not exist.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateModuleStatus(short moduleId,bool enable)
		{
			sysCfg.UpdateModuleStatus(moduleId,enable);
		}
		/// <summary>
		/// Update module settings.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="cfgGroupId"></param>
		/// <param name="keyName"></param>
		/// <param name="keyValue"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if module does not exist.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateModuleSettings(short moduleId,short cfgGroupId,string keyName,string keyValue)
		{
			sysCfg.UpdateModuleSettings(moduleId,cfgGroupId,keyName,keyValue);
		}
		/// <summary>
		/// Delete module and cleanup all module settings.
		/// </summary>
		/// <param name="moduleId"></param>
		/// <exception cref="DASException">Thrown in all exception cases.</exception>
		public int DeleteModule(short moduleId)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Delete module settings
				sysCfg.DeleteModuleSettings(moduleId);
				// 3. Delete module
				rowsAffected = sysCfg.DeleteModule(moduleId);
				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
				rowsAffected = 0;
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to delete module ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
				rowsAffected = 0;
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves configuration module types
		/// </summary>
		/// <returns>DataSet [TypeId],[TypeName]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public DataSet GetConfigurationModuleTypesInfo()
		{
			DataSet dsResult = sysCfg.GetConfigurationModuleTypesInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "ConfigurationGroups" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves computer modules information
		/// </summary>
		/// <param name="moduleTypeId"></param>
		/// <param name="computerIp"></param>
		/// <returns>DataSet [ModuleId],[ModuleName],[Enabled],[TypeId],[TypeName]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		/// <remarks>If moduleTypeId==VLF.CLS.Def.Const.unassignedIntValue, 
		/// then doesn't include moduleTypeId into the filter </remarks>
		public DataSet GetComputerModulesInfo(short moduleTypeId,string computerIp)
		{
			DataSet dsResult = sysCfg.GetComputerModulesInfo(moduleTypeId,computerIp);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "ComputerModulesInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves configuration value
		/// </summary>
		/// <param name="moduleName"></param>
		/// <param name="groupID"></param>
		/// <param name="paramName"></param>
		/// <returns>[KeyValue]</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if module name does not exist.</exception>
		public string GetConfigParameter(string moduleName, short groupID, string paramName)
		{
			// take Module ID in DB
			short moduleID = GetConfigurationModuleTypeId(moduleName);
			if( moduleID == VLF.CLS.Def.Const.unassignedShortValue )
			{
				throw new DASAppResultNotFoundException("Cannot find '" + moduleName + "' in DB." );
			}
			return GetConfigurationValue(moduleID,groupID,paramName); 
		}

        /// <summary>
        /// Get All Map Engines
        /// </summary>
        /// <returns></returns>
        public DataSet GetMapEngines()
        {
            DB.Organization org = new DB.Organization(sqlExec);
            DataSet dsResult = org.GetMapEngines();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "MapEngines";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
		/// Get all map engines info
		/// </summary>
        /// <returns>DataSet [MapGroupId],[MapGroupName],[Priority],[MapId],[MapEngineName],[Path],[ExternalPath]</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAllMapEnginesInfo()
		{
			DB.Organization org = new DB.Organization(sqlExec);
			DataSet dsResult = org.GetAllMapEnginesInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllMapEnginesInfo";
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

        /// <summary>
        /// Get all map engines short info
        /// </summary>
        /// <returns>DataSet [MapGroupId],[MapGroupName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllMapEnginesShortInfo()
        {
            DB.Organization org = new DB.Organization(sqlExec);
            DataSet dsResult = org.GetAllMapEnginesShortInfo();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetAllMapEnginesShortInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
        /// Get all geo code engines
        /// </summary>
        /// <returns></returns>
        public DataSet GetGeoCodeEngines()
        {
            DB.Organization org = new DB.Organization(sqlExec);
            DataSet dsResult = org.GetGeoCodeEngines();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GeoCodeEngines";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }
        
        /// <summary>
		/// Get all GeoCode engines info
		/// </summary>
        /// <returns>DataSet [GeoCodeGroupId],[GeoCodeGroupName],[Priority],[GeoCodeId],[GeoCodeEngineName],[Path]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllGeoCodeEnginesInfo()
		{
			DB.Organization org = new DB.Organization(sqlExec);
			DataSet dsResult = org.GetAllGeoCodeEnginesInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllGeoCodeEnginesInfo";
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

        /// <summary>
        /// Get all GeoCode engines short info
        /// </summary>
        /// <returns>DataSet [GeoCodeGroupId],[GeoCodeGroupName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllGeoCodeEnginesShortInfo()
        {
            DB.Organization org = new DB.Organization(sqlExec);
            DataSet dsResult = org.GetAllGeoCodeEnginesShortInfo();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetAllGeoCodeEnginesShortInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves Channels info. 	
        /// </summary>
        /// <returns>DataSet [ChId],[ChName],[BoxProtocolTypeId],[CommModeId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetChannelsInfo()
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet dsResult = boxConfig.GetChannelsInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "GetChannelsInfo";
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

        /// <summary>
        /// Retrieves Communication Modes info. 	
        /// </summary>
        /// <returns>DataSet [CommModeId],[CommModeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetCommModesInfo()
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            DataSet dsResult = boxConfig.GetCommModesInfo();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetCommModesInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }
        #endregion

		#region Day Light Saving
		/// <summary>
		/// Set DayLight Savings.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDayLightSaving(bool dayLightSaving)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Set fleet email DayLightSaving
				DB.FleetEmails fleetEmails = new DB.FleetEmails(sqlExec);
				fleetEmails.SetDayLightSaving(dayLightSaving);
				// 3. Set organization geozone DayLightSaving
				DB.OrganizationGeozone organizationGeozone = new DB.OrganizationGeozone(sqlExec);
				organizationGeozone.SetDayLightSaving(dayLightSaving);
				// 4. Set vehicle info DayLightSaving
				DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
				vehicleInfo.SetDayLightSaving(dayLightSaving);
				// 5. Set user preference DayLightSaving
				DB.UserPreference userPreference = new DB.UserPreference(sqlExec);
				userPreference.SetDayLightSaving(dayLightSaving);
				// 6. Set configuration DayLightSaving
				DB.Configuration configuration = new DB.Configuration(sqlExec);
				configuration.SetDayLightSaving(dayLightSaving);
                // 7. Set MsgNotification DayLightSaving
                DB.BoxMsgInType msgNotification = new DB.BoxMsgInType(sqlExec);
                msgNotification.SetDayLightSaving(dayLightSaving);
                // 8. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 8. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Uanable to delete organization ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 8. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}
		/// <summary>
		/// Set AutoAdjustDayLightSaving.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="vehicleId"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetVehicleAutoAdjustDayLightSaving(Int64 vehicleId,bool autoAdjustDayLightSaving,bool dayLightSaving)
		{
			DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
			vehicleInfo.SetAutoAdjustDayLightSaving(vehicleId,autoAdjustDayLightSaving,dayLightSaving);
		}
		/// <summary>
		/// Set AutoAdjustDayLightSaving.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="fleetId"></param>
		/// <param name="email"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetFleetAutoAdjustDayLightSaving(int fleetId,string email,bool autoAdjustDayLightSaving,bool dayLightSaving)
		{
			DB.FleetEmails fleetEmails = new DB.FleetEmails(sqlExec);
			fleetEmails.SetAutoAdjustDayLightSaving(fleetId,email,autoAdjustDayLightSaving,dayLightSaving);
		}
		/// <summary>
		/// Set AutoAdjustDayLightSaving.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="organizationId"></param>
		/// <param name="geozoneId"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetOrganizationGeozoneAutoAdjustDayLightSaving(int organizationId,short geozoneId,bool autoAdjustDayLightSaving,bool dayLightSaving)
		{
			DB.OrganizationGeozone organizationGeozone = new DB.OrganizationGeozone(sqlExec);
			organizationGeozone.SetAutoAdjustDayLightSaving(organizationId,geozoneId,autoAdjustDayLightSaving,dayLightSaving);
		}
        #endregion

		#region AMS
		/// <summary>
		/// Add new task
		/// </summary>
		/// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException information already exist</exception>
		/// <exception cref="DASException">Throws DASException in all other error cases.</exception>
		/// <returns> current task id or -1 in case of error</returns>
		/// <param name="userId"></param>
		/// <param name="time"></param>
		/// <param name="boxID"></param>
		/// <param name="commandID"></param>
		/// <param name="customProp"></param>
		/// <param name="protocolType"></param>
		/// <param name="commMode"></param>
		/// <param name="transmissionPeriod"></param>
		/// <param name="transmissionInterval"></param>
		/// <param name="usingDualMode"></param>
		public Int64 AddTask(int userId, DateTime time, int boxID, short commandID, string customProp, 
							short protocolType, short commMode,Int64 transmissionPeriod,
							int transmissionInterval,bool usingDualMode)
		{
			return commandScheduler.AddTask(userId,time,boxID,commandID,customProp,protocolType,commMode,transmissionPeriod,transmissionInterval,usingDualMode);
		}
		
		/// <summary>
		/// Delete existing task
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="taskId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if task does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeleteTask(Int64 taskId)
		{
			return commandScheduler.DeleteTask(taskId);
		}
		/// <summary>
		/// Reschedule task
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="transmissionPeriod"></param>
		/// <param name="transmissionInterval"></param>
		/// <param name="usingDualMode"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public int ReScheduledTask(Int64 taskId, Int64 transmissionPeriod,int transmissionInterval,bool usingDualMode)
		{
			return commandScheduler.ReScheduledTask(taskId,transmissionPeriod,transmissionInterval,usingDualMode);
		}
		/// <summary>
		/// Get user tasks
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <returns>DataSet [TaskId],[RequestDateTime],[BoxId],[UserId],[BoxCmdOutTypeId],
		/// [BoxProtocolTypeId],[CommModeId],[TransmissionPeriod],[TransmissionInterval],
		/// [CustomProp],[LastDateTimeSent],[UsingDualMode],[VehicleId],
		/// [Description],[LicensePlate]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public DataSet GetUserTasks(int userId)
		{
			DataSet dsResult = commandScheduler.GetUserTasks(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserTasks" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Update last DateTime sent
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="lastDateTimeSent"></param>
		/// <returns></returns>
		public int UpdateTaskLastDateTimeSent(Int64 taskId,DateTime lastDateTimeSent)
		{
			return commandScheduler.UpdateLastDateTimeSent(taskId,lastDateTimeSent);
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="taskId"></param>
      /// <param name="val"></param>
      /// <returns></returns>
      public bool UpdateScheduledTask(Int64 taskId, byte val, DateTime lastDateTimeSent, DateTime lastDateTimeTouched)
		{
         return commandScheduler.UpdateScheduledTask(taskId, val, lastDateTimeSent, lastDateTimeTouched);
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="taskId"></param>
      /// <param name="lastDateTimeSent"></param>
      public void AddTaskInHistory(Int64 taskId, DateTime lastDateTimeSent)
      {
         commandScheduler.AddTaskInHistory(taskId, lastDateTimeSent);
      }
      /// <summary>
      ///         clear the RequestStatus from 
      /// </summary>
      public void InitScheduledTasks()
      {
         commandScheduler.InitScheduledTasks();
      }
      
		/// <summary>
		/// Get currently scheduled tasks
		/// </summary>
		/// <returns>DataSet [TaskId],[RequestDateTime],[BoxId],[UserId],[BoxCmdOutTypeId],
		/// [BoxProtocolTypeId],[CommModeId],[TransmissionPeriod],[TransmissionInterval],
		/// [CustomProp],[LastDateTimeSent],[UsingDualMode]</returns>
		/// <exception cref="DASException">Throws DASException in all error cases.</exception>
		public DataSet GetCurrentlyScheduledTasks()
		{
         DataSet dsResult = commandScheduler.GetCurrentlyScheduledTasks();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "CurrentlyScheduledTasks" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}

      public DataSet GetCurrentlyScheduledTasks(int max)
      {
         DataSet dsResult = commandScheduler.GetCurrentlyScheduledTasks(max);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "CurrentlyScheduledTasks";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }

      public DataSet GetCurrentlyScheduledTasksExcept(int max, int[] boxIDs)
      {
         DataSet dsResult = commandScheduler.GetCurrentlyScheduledTasksExcept(max, boxIDs);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "CurrentlyScheduledTasks";
            }
            dsResult.DataSetName = "System";
         }
         return dsResult;
      }
        /// <summary>
        /// Reset Box Cmds Scheduled DateTime
        /// </summary>
        /// <param name="boxId"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int ResetBoxCmdsScheduledDateTime(int boxId)
        {
            return commandScheduler.ResetBoxCmdsScheduledDateTime(boxId);
        }



        /// <summary>
        ///        Retrieve Scheduled task history
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="fleetId"></param>
        /// <param name="boxId"></param>
        public DataSet  GetSheduledTasksHistory(DateTime fromDate, DateTime toDate, Int32 fleetId, Int32 boxId)
        {
           return commandScheduler.GetSheduledTasksHistory(fromDate, toDate, fleetId, boxId);
        }
       
		#endregion

		#region Map Usage
		/// <summary>
		/// Retrieves organization map usage info.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="mapId"></param>
		/// <param name="usageYear"></param>
		/// <param name="usageMonth"></param>
		/// <returns>DataSet [UserType],[UserName_BoxId],[Map],[StreetAddress],[Totals]</returns>
		public DataSet GetOrganizationMapUsageInfo(int organizationId,int mapId,short usageYear,short usageMonth)
		{
			DataSet dsResult = mapEngine.GetOrganizationMapUsageInfo(organizationId,mapId,usageYear,usageMonth);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "OrganizationMapUsageInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves map types.
		/// </summary>
		/// <returns>DataSet [MapTypeId],[MapTypeName]</returns>
		public DataSet GetMapTypes()

		{
			DataSet dsResult = mapEngine.GetMapTypes();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "MapTypes" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;

		}
        /// <summary>
        /// Get all GeoCode engines info
        /// </summary>
        /// <returns>DataSet [GeoCodeGroupId],[GeoCodeGroupName],[Priority],[GeoCodeId],[GeoCodeEngineName],[Path]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetGeoCodeEnginesInfo()
		{
            DB.Organization mapEngine = new DB.Organization(sqlExec);
            DataSet dsResult = mapEngine.GetAllGeoCodeEnginesInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "GetAllGeoCodeEnginesInfo";
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
		/// <summary>
        /// Retrieves box GeoCode group Id
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>GeoCode group Id if exist, otherwise VLF.CLS.Def.Const.unassignedIntValue</returns>
        public int GetBoxGeoCodeGroupId(int boxId)
		{
			return mapEngine.GetBoxGeoCodeGroupId(boxId);
		}
        /// <summary>
        /// Retrieves Organization GeoCode group Id
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>GeoCode group Id if exist, otherwise VLF.CLS.Def.Const.unassignedIntValue</returns>
        public int GetOrganizationGeoCodeGroupId(int organizationId)
       {
           return mapEngine.GetOrganizationGeoCodeGroupId(organizationId);
       }
		/// <summary>
		/// Retrieves box geocode engine id.
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [GeoCodeId],[Path]</returns>
		public DataSet GetBoxGeoCodeEngineInfo(int boxId)
		{
			DataSet dsResult = mapEngine.GetBoxGeoCodeEngineInfo(boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetBoxGeoCodeEngineInfo" ;
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
        /// <summary>
        /// Retrieves box geocode engine id.
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns>DataSet [MapId],[Path],[ExternalPath]</returns>
        public DataSet GetBoxMapEngineInfo(int boxId)
        {
            DataSet dsResult = mapEngine.GetBoxMapEngineInfo(boxId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetBoxMapEngineInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }
		/// <summary>
		/// Retrieves user geocode path
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>Dataset [GeoCodeId],[Path]</returns>
        public DataSet GetUserGeoCodeEngineInfo(int userId)
		{
            DataSet dsResult = mapEngine.GetUserGeoCodeEngineInfo(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
                    dsResult.Tables[0].TableName = "GetUserGeoCodeEngineInfo";
				}
				dsResult.DataSetName = "System";
			}
			return dsResult;
		}
        /// <summary>
        /// Retrieves user GeoCode group Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>GeoCode group Id if exist, otherwise VLF.CLS.Def.Const.unassignedIntValue</returns>
        public int GetUserGeoCodeGroupId(int userId)
        {
            return mapEngine.GetUserGeoCodeGroupId(userId);
        }
        /// <summary>
        /// Retrieves user map info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Dataset [MapId],[Path],[ExternalPath]</returns>
        public DataSet GetUserMapEngineInfo(int userId)
        {
            DataSet dsResult = mapEngine.GetUserMapEngineInfo(userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetUserMapEngineInfo";
                }
                dsResult.DataSetName = "System";
            }
            return dsResult;
        }
		#endregion

		#region Security
		/// <summary>
		/// Chacks box-user authorization
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <returns>true if authorized, otherwise false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool CheckBoxUserAuthorization(int boxId,int userId)
		{
			DB.Security security = new DB.Security(sqlExec);
			return security.CheckBoxUserAuthorization(boxId,userId);
		}
		#endregion

        /// <summary>
        /// Retrieves Communication Modes info. 	
        /// </summary>
        /// <returns>DataSet [CommModeId],[CommModeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetMsgNotificationRecipientInfo(int boxId, short boxMsgInTypeId, short sensorId, string msgDetails)
        {
            DB.BoxMsgInType boxMsgInType = new DB.BoxMsgInType(sqlExec);
            DataSet dsResult = boxMsgInType.GetMsgNotificationRecipientInfo(boxId, boxMsgInTypeId, sensorId, msgDetails);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "MsgNotificationRecepientInfo";
                }
                dsResult.DataSetName = "MsgInType";
            }
            return dsResult;
        }

        /// <summary>
        /// Overloaded - added features mask field
        /// </summary>
        /// <param name="fwId"></param>
        /// <param name="localPath"></param>
        /// <param name="oapPath"></param>
        /// <param name="maxGeozones"></param>
        /// <param name="oapPort"></param>
        /// <param name="featuresMask"></param>
        public void UpdateFirmwareInfo(short fwId, string localPath, string oapPath, int maxGeozones, short oapPort, long featuresMask)
        {
           DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
           StringBuilder sql = new StringBuilder();
           sql.AppendLine("SET FwLocalPath = @localPath, FwOAPPath = @oapPath, MaxGeozones = @maxGeozones, OAPPort = @oapPort, FwAttributes1 = @featuresMask");
           sql.Append("WHERE FwId = @fwId");
           SqlParameter[] sqlParams = new SqlParameter[6];
           sqlParams[0] = new SqlParameter("@localPath", localPath.Replace("'", "''"));
           sqlParams[1] = new SqlParameter("@oapPath", oapPath.Replace("'", "''"));
           sqlParams[2] = new SqlParameter("@maxGeozones", maxGeozones);
           sqlParams[3] = new SqlParameter("@oapPort", oapPort);
           sqlParams[4] = new SqlParameter("@featuresMask", featuresMask);
           sqlParams[5] = new SqlParameter("@fwId", fwId);
           boxConfig.UpdateRow(sql.ToString(), sqlParams);
        }

        /// <summary>
        /// Get Firmware Information - features mask added
        /// </summary>
        /// <param name="fwId"></param>
        /// <returns>DataSet [FwChId],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum],[BoxProtocolTypeId],[BoxProtocolTypeName],
        /// [CommModeId],[CommModeName],[ChPriority],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones],[Features]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetFirmwareInfoFeatures(short fwId)
        {
           DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);

           StringBuilder sql = new StringBuilder(
              "SELECT vlfFirmwareChannels.FwChId, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfBoxHwType.MaxSensorsNum, ");
           sql.Append("vlfBoxHwType.MaxOutputsNum, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, ");
           sql.Append("vlfCommMode.CommModeId, vlfCommMode.CommModeName, vlfFirmwareChannels.ChPriority, vlfFirmware.FwTypeId, ");
           sql.Append("vlfFirmware.FwLocalPath, vlfFirmware.FwOAPPath, vlfFirmware.FwDateReleased, vlfFirmware.MaxGeozones, vlfFirmware.OAPPort, ");
           sql.AppendLine("vlfFirmware.FwAttributes1");
           sql.AppendLine("FROM vlfFirmwareChannels INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId");
           sql.AppendLine("INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId");
           sql.AppendLine("INNER JOIN vlfBoxHwType ON vlfFirmware.BoxHwTypeId = vlfBoxHwType.BoxHwTypeId");
           sql.AppendLine("INNER JOIN vlfBoxProtocolType ON vlfChannels.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId");
           sql.AppendLine("INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId");
           sql.Append("WHERE vlfFirmwareChannels.FwId = @fwId ORDER BY vlfFirmwareChannels.ChPriority");

           SqlParameter[] sqlParams = new SqlParameter[] { new SqlParameter("@fwId", fwId) };
           DataSet dsResult = boxConfig.GetRowsBySql(sql.ToString(), sqlParams);
           if (dsResult != null && dsResult.Tables.Count > 0)
           {
              dsResult.Tables[0].TableName = "FirmwareInfo";
              dsResult.DataSetName = "System";
           }
           return dsResult;
        }

      /// <summary>
      /// Get all the firmware from the table
      /// </summary>
      /// <returns></returns>
      public DataSet GetAllFirmware()
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
#if debug
         Util.BTrace(Util.INF0, "-- System.GetAllFirmware()");
# endif
         return boxConfig.GetRowsBySql("SELECT FwTypeId, FwId, BoxHwTypeId, FwName, FwDateReleased FROM vlfFirmware ORDER BY vlfFirmware.FwDateReleased DESC", null);
         //if (dsFw != null && dsFw.Tables[0] != null)
         //   dsFw.Tables[0].TableName = "Firmware";
         //return dsFw.Tables[0];
      }

      /// <summary>
      /// Add a new sensor
      /// </summary>
      /// <param name="deviceId"></param>
      /// <param name="deviceTypeId"></param>
      /// <param name="sensorName"></param>
      /// <param name="sensorAction"></param>
      /// <returns></returns>
      public int AddSensor(short deviceId, short deviceTypeId, string sensorName, string sensorAction)
      {
         DB.Sensor sensor = new Sensor(this.sqlExec);
         return sensor.AddSensor(deviceId, deviceTypeId, sensorName, sensorAction);
      }

      /// <summary>
      /// Delete Sensor
      /// </summary>
      /// <param name="sensorId">Sensor ID</param>
      /// <returns>rows deleted</returns>
      public int DeleteSensor(short sensorId)
      {
         DB.Sensor sensor = new Sensor(this.sqlExec);
         return sensor.DeleteSensorById(sensorId);
      }

      /// <summary>
      /// Update Sensor
      /// </summary>
      /// <param name="sensorId"></param>
      /// <param name="name"></param>
      /// <param name="action"></param>
      public int UpdateSensor(short sensorId, string name, string action)
      {
         DB.Sensor sensor = new Sensor(this.sqlExec);
         SqlParameter[] sqlParams = new SqlParameter[3];
         sqlParams[0] = new SqlParameter("@name", name);
         sqlParams[1] = new SqlParameter("@action", action);
         sqlParams[2] = new SqlParameter("@sensorId", sensorId);
         return sensor.UpdateRow("SET SensorName = @name, SensorAction = @action WHERE SensorId = @sensorId", sqlParams);
      }

      /// <summary>
      /// Update Firmware Attributes
      /// </summary>
      /// <param name="fwId"></param>
      /// <param name="featureMask"></param>
      /// <returns></returns>
      public int UpdateFirmwareAttributes(short fwId, long featureMask)
      {
         DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         SqlParameter[] sqlParams = new SqlParameter[2];
         sqlParams[0] = new SqlParameter("@fwId", fwId);
         sqlParams[1] = new SqlParameter("@mask", featureMask);
         return boxConfig.UpdateRow("SET FwAttributes1 = @mask WHERE FwId = @fwId", sqlParams);
      }
   }
}
