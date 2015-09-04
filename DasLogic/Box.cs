using System;
using System.Collections;	// for SortedList
using System.Data;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;


namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to box functionality in database
	/// </summary>
	public partial class Box : Das
	{
		private BoxCommInfo boxCommInfo = null;
      private DB.MapEngine mapEngine = null;
		private DB.Box box = null;
			
		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public Box(string connectionString) : base (connectionString)
		{
			boxCommInfo = new BoxCommInfo(sqlExec);
			box = new DB.Box(sqlExec);
            mapEngine = new DB.MapEngine(sqlExec);
        }

		/// <summary>
		/// Distructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region Box Outputs Interfaces




         /// <summary>
        /// Get Box MsgIn Types
        /// </summary>
        /// <returns>[BoxMsgInTypeId],[BoxMsgInTypeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>

        public DataSet GetBoxMsgInTypes()
        {
            DB.BoxMsgInType msgtypes = new DB.BoxMsgInType(sqlExec);
            DataSet dsResult = msgtypes.GetBoxMsgInTypes();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "BoxMsgTypes";
                }
                dsResult.DataSetName = "Box";
            }
            return dsResult;
        }
		/// <summary>
		/// Retrieves output info by box id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="userId"></param> 
		/// <returns>DataSet [OutputId][OutputName][OutputAction]</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetBoxOutputsInfo(int boxId,int userId)
		{
			DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
			DataSet dsResult = outputs.GetOutputsInfoByBoxId(boxId,userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxOutputsInfo" ;
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}
		/// <summary>
		/// Add new outputs to the box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="dsOutputsCfg"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if output id and name alredy exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddOutputs(int boxId,short boxHwTypeId,DataSet dsOutputsCfg)
		{
			DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
			outputs.AddOutputs(boxId,boxHwTypeId,dsOutputsCfg);
		}

        [Obsolete("Use SetOutputs(int , short , DataSet , bool) instead")]
        public void SetOutputs(int boxId,short boxHwTypeId,DataSet dsOutputsCfg)
        {
            SetOutputs( boxId, boxHwTypeId, dsOutputsCfg,true);
        }
		/// <summary>
		/// Set new outputs to the box.
		/// </summary>
		/// <remarks>
		/// 1. Delete previous outputs related to the box
		/// 2. Add new outputs
		/// </remarks>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="dsOutputsCfg"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if output id and name alredy exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void SetOutputs(int boxId,short boxHwTypeId,DataSet dsOutputsCfg, bool useTransaction)
		{
			try
			{
				// 1. Begin transaction
                if (useTransaction)
				    sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2. Delete previous user-defined outputs
				DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
				outputs.DeleteOutputsByBoxId(boxId);
				
				// 3. add new user-defined outputs
				outputs.AddOutputs(boxId,boxHwTypeId,dsOutputsCfg);
				
				// 4. Save all changes
                if (useTransaction)
				    sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update user-defined sensors. ";
				// 4. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 4. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update user-defined sensors. ";
				// 4. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Update output information.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="outputId"></param>
		/// <param name="outputName"></param>
		/// <param name="outputAction"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not have info for current output.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateOutput(int boxId,short outputId,string outputName,string outputAction)
		{
			DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
			outputs.UpdateOutput(boxId,outputId,outputName,outputAction);
		}
		/// <summary>
		/// Retrieves max number of supported outputs for specific Hw type.
		/// Note: if box does not exist return 0
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int GetMaxSupportedDefaultOutputsByHwType(short boxHwTypeId)
		{
         DB.BoxHwType hwType = new DB.BoxHwType(sqlExec);
         return hwType.GetMaxOutputsNumById(boxHwTypeId);
		}
		#endregion

		#region Box Sensors Interfaces
		/// <summary>
		/// Retrieves sensors info by box id
		/// </summary>
		/// <param name="boxId"></param> 
		/// <returns>DataSet [SensorId][SensorName][SensorAction][AlarmLevelOn][AlarmLevelOff]</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetBoxSensorsInfo(int boxId)
		{
         DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
			DataSet dsResult = sensors.GetSensorsInfoByBoxId(boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxSensorsInfo" ;
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;

		}
		/// <summary>
		/// Add new sensors to the box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="dsSensorsCfg"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor id and name alredy exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddSensors(int boxId,short boxHwTypeId,DataSet dsSensorsCfg)
		{
			DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
			sensors.AddSensors(boxId,boxHwTypeId,dsSensorsCfg);
		}


        [Obsolete("Use SetSensors(int , short , bool , DataSet , bool) instead")]
        public void SetSensors(int boxId, short boxHwTypeId, DataSet dsSensorsCfg)
        {
            SetSensors(boxId, boxHwTypeId, dsSensorsCfg, true);
        }

		/// <summary>
		/// Set new sensors to the box.
		/// </summary>
		/// <remarks>
		/// 1. Delete previous sensors related to the box
		/// 2. Add new outputs
		/// </remarks>
		/// <param name="boxId"></param>
		/// <param name="boxHwTypeId"></param>
		/// <param name="dsSensorsCfg"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if sensor id and name alredy exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void SetSensors(int boxId,short boxHwTypeId,DataSet dsSensorsCfg,bool useTransaction)
		{
			try
			{
				// 1. Begin transaction
                if (useTransaction)
				    sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2. Delete previous user-defined sensors
				DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
				sensors.DeleteSensorsByBoxId(boxId);
				
				// 3. add new user-defined sensors
				sensors.AddSensors(boxId,boxHwTypeId,dsSensorsCfg);

				// 4. Save all changes
                if (useTransaction)
				    sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update user-defined sensors. ";
				// 4. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 4. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update user-defined sensors. ";
				// 4. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Update sensor information.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="sensorId"></param>
		/// <param name="sensorName"></param>
		/// <param name="sensorAction"></param>
		/// <param name="alarmLevelOn"></param>
		/// <param name="alarmLevelOff"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not have info for current sensor.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateSensor(int boxId,short sensorId,string sensorName,string sensorAction,short alarmLevelOn,short alarmLevelOff)
		{
			DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
			sensors.UpdateSensor(boxId,sensorId,sensorName,sensorAction,alarmLevelOn,alarmLevelOff);
		}

        public void UpdateSensorName(int boxId, short sensorId, string sensorName)
        {
            DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
            sensors.UpdateSensorName(boxId, sensorId, sensorName);
        }
		
		/// <summary>
		/// Delete user-defined sensors for current box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>int rowsAffected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteSensors(int boxId)
		{
			DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
			return sensors.DeleteSensorsByBoxId(boxId);
		}
		/// <summary>
		/// Retrieves max number of supported sensors for specific Hw type.
		/// Note: if box does not exist return 0
		/// </summary>
		/// <param name="boxHwTypeId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int GetMaxSupportedSensorsByHwType(short boxHwTypeId)
		{
         DB.BoxHwType hwType = new DB.BoxHwType(sqlExec);
         return hwType.GetMaxSensorsNumById(boxHwTypeId);
		}
		#endregion

		#region Box Communication Interfaces
		/// <summary>
		/// Retieves box communication info. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <remarks>
		/// TableName	= "BoxCommunicationInfo"
		/// DataSetName = "Box"
		/// </remarks>
		/// <returns>DataSet [CommAddressTypeId][CommAddressValue][CommAddressTypeName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetCommInfoByBoxId(int boxId)
		{
			DataSet dsResult = boxCommInfo.GetCommInfoByBoxId(boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxCommunicationInfo";
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}

		/// <summary>
		///            Get full CommInfo by box Id. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName],[FwChId],[BoxHwTypeId],[BoxProtocolTypeId],[CommModeId],[ChPriority]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <comment>
      ///      this is the most used function whenever commands are sent 
      ///      the only drawback is to find out how many notifications are received per day 
      ///      the test I ran returned - 16971 for '2009/08/20'  and < '2009/08/21'
      ///                              - 15800 for '2009/08/19'  and < '2009/08/20'
      ///         select count(*) from vlfMsgInHst (NOLOCK, INDEX=0)
      ///               where BoxMsgInTypeId = 10 and OriginDateTime > '2009/08/20'  and OriginDateTime < '2009/08/21'
      ///      I wanted to see the maximum per day for all vehicles and the average
      /// 
      ///      the conclusion is that the average updates are 1700 per hour,  1700  / 3600 seconds, every two seconds you have to update
      ///      ==> RT mechanism is much more appropriate
      /// </comment>
		public DataSet GetFullCommInfoByBoxId(int boxId)
		{
			DataSet dsResult = boxCommInfo.GetFullCommInfoByBoxId(boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxCommunicationInfo";
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}

      /// <summary>
      /// Get CommInfo by box Id. 	
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>DataSet [CommAddressTypeId], [CommAddressTypeName], [CommAddressValue]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetBoxCommInfo(int boxId)
      {
         string sql =
            "SELECT vlfCommModeAddressType.CommAddressTypeId, vlfCommAddressType.CommAddressTypeName, vlfBoxCommInfo.CommAddressValue FROM vlfBox with (nolock) INNER JOIN vlfFirmwareChannelReference ON vlfBox.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfCommModeAddressType ON vlfCommMode.CommModeId = vlfCommModeAddressType.CommModeId INNER JOIN vlfCommAddressType ON vlfCommModeAddressType.CommAddressTypeId = vlfCommAddressType.CommAddressTypeId INNER JOIN vlfBoxCommInfo ON vlfBox.BoxId = vlfBoxCommInfo.BoxId AND vlfCommAddressType.CommAddressTypeId = vlfBoxCommInfo.CommAddressTypeId WHERE vlfBox.BoxId = @boxId ORDER BY vlfCommModeAddressType.CommAddressTypeId";

         DataSet dsResult = boxCommInfo.GetRowsBySql(sql, new SqlParameter[] { new SqlParameter("@boxId", boxId) });

         if (Util.IsDataSetValid(dsResult))
         {
            dsResult.Tables[0].TableName = "BoxCommunicationInfo";
            dsResult.DataSetName = "Box";
         }
         return dsResult;
      }

		/// <summary>
		/// Returns only first box. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <returns>CommAddressValue</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public string GetCommInfoAddressValue(int boxId,short commAddressTypeId)
		{
			return boxCommInfo.GetCommInfoAddressValue(boxId,commAddressTypeId);
		}
		/// <summary>
		/// Retrieves CommAddressTypeId by ProtocolType and CmdOutTypeId
		/// </summary>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="boxCmdOutTypeId"></param>
		/// <returns>CommAddressTypeId</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public short GetCommAddressTypeIdByProtocolTypeCmdOutTypeId(short boxProtocolTypeId,short boxCmdOutTypeId)
		{
			DB.BoxProtocolTypeCmdOutType boxCfg = new DB.BoxProtocolTypeCmdOutType(sqlExec);
			return boxCfg.GetCommAddressTypeIdByProtocolTypeCmdOutTypeId(boxProtocolTypeId,boxCmdOutTypeId);
		}
		/// <summary>
		/// Returns only first box id by CommAddressValue. 	
		/// </summary>
		/// <param name="commAddressValue"></param>
		/// <returns>box id</returns>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException in case of unexpected multiple rows result.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int GetBoxIdByCommAddressValue(string commAddressValue)
		{
			return boxCommInfo.GetBoxIdByCommAddressValue(commAddressValue);
		}
		/// <summary>
		/// Returns box configuration info by CommAddressValue. 	
		/// </summary>
		/// <param name="commAddressValue"></param>
		/// <returns>DataSet [BoxId],[BoxHwTypeId],[BoxProtocolTypeId],[CommModeId],[FwChId]</returns>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException in case of unexpected multiple rows result.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetBoxConfigInfoByCommAddressValue(string commAddressValue)
		{
			return boxCommInfo.GetBoxConfigInfoByCommAddressValue(commAddressValue);
		}
		/// <summary>
		/// Retrieves DCL id by box id, ProtocolTypeId and CommModeId
		/// In case of wrong result returns VLF.CLS.Def.Const.unassignedIntValue
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>dcl id</returns>
		public short GetDclId(int boxId, VLF.CLS.Def.Enums.ProtocolTypes boxProtocolTypeId, VLF.CLS.Def.Enums.CommMode commModeId)
		{
			DB.Dcl dcl = new DB.Dcl(sqlExec);
			return dcl.GetDclId(boxId,boxProtocolTypeId,commModeId);
		}

		/// <summary>
		/// Retrieves OTA DCL id by communication mode
		/// </summary>
		/// <param name="commModeId"></param>
		/// <remarks>
		/// If dcl with current configuration does not exist, returns VLF.CLS.Def.Const.unassignedIntValue
		/// </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>dcl id</returns>
		public short GetOTADclId(VLF.CLS.Def.Enums.CommMode commModeId)
		{
			DB.Dcl dcl = new DB.Dcl(sqlExec);
			return dcl.GetOTADclId(commModeId);
		}
		/// <summary>
		/// Retrieves DCL info
		/// </summary>
		/// <param name="dclName"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>DataSet [DclId],[CommModeId],[BoxProtocolGroupId ],[DclName],[Description],
		/// [ServiceState],[PID],[BoxProtocolTypeId],[BoxProtocolTypeName]</returns>
		public DataSet GetDclInfo(string dclName)
		{
			DB.Dcl dcl = new DB.Dcl(sqlExec);
			return dcl.GetDclInfo(dcl.GetDclIdByName(dclName));
		}
		/// <summary>
		/// Retrieves DCL info
		/// </summary>
		/// <param name="dclId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <returns>DataSet [DclId],[CommModeId],[BoxProtocolGroupId ],[DclName],[Description],
		/// [ServiceState],[PID],[BoxProtocolTypeId],[BoxProtocolTypeName]</returns>
		public DataSet GetDclInfo(short dclId)
		{
			DB.Dcl dcl = new DB.Dcl(sqlExec);
			DataSet dsResult = dcl.GetDclInfo(dclId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "DclInfo";
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves Protocol types info by protocol group
		/// </summary>
		/// <param name="commAddressValue"></param>
		/// <returns>DataSet [BoxId],[BoxProtocolTypeId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetBoxCommInfoByCommAddressValue(string commAddressValue)
		{
			DB.Dcl dcl = new DB.Dcl(sqlExec);
			DataSet dsResult = dcl.GetBoxCommInfoByCommAddressValue(commAddressValue);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "ProtocolInfo";
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}
		/// <summary>
		/// Updates box communication info. 	
		/// </summary>
		/// <exception cref="DASAppResultNotFoundException">Throws DASAppResultNotFoundException if current box does not support communication address type.</exception>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Throws DASAppViolatedIntegrityConstraintsException if current box does not contain communication information.</exception>
		/// <exception cref="DASException">Throws in all other error cases.</exception>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <param name="commAddressValue"></param>
		public void UpdateBoxCommInfo(int boxId,short commAddressTypeId,string commAddressValue)
		{
			boxCommInfo.UpdateCommInfo(boxId,commAddressTypeId,commAddressValue);
		}
		/// <summary>
		/// Check if communication info does not assigned to another box
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commInfo"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>true if phone is free, otherwise returns false</returns>
		public bool IsFreeCommInfo(int boxId,short commAddressTypeId,string commInfo)
		{
			return boxCommInfo.IsFreeCommInfo(boxId,commAddressTypeId,commInfo);
		}

		/// <summary>
		/// Get Primary Server Ip. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commModeId"></param>
		/// <returns>serverIp</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public string GetPrimaryServerIp(int boxId,short commModeId)
		{
			DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
			return boxCommInfo.GetPrimaryServerIp(boxId,commModeId);
		}
      /// <summary>
      /// Gets server ip and ports
      /// </summary>
      /// <param name="commId">CommModeId</param>
      /// <param name="protocolId">ProtocolTypeId</param>
      /// <returns></returns>
      public DataSet GetBoxServerConfig(int commId, int protocolId)
      {
         return boxCommInfo.GetBoxServerConfig(commId, protocolId);
      }
		#endregion

		#region Box Configuration Interfaces
		/// <summary>
		/// Retrieves all assigned/free boxes ids.
		/// </summary>
		/// <param name="assigned"></param>
		/// <param name="organizationId"></param>
		/// <returns>ArrayList</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public ArrayList GetAllAssignedBoxIds(bool assigned,int organizationId)
		{
			return box.GetAllAssignedBoxIds(assigned,organizationId);
		}
		/// <summary>
		/// Retrieves all assigned/free boxes info.
		/// </summary>
		/// <remarks>
		/// Assgned is true
		/// Free is false
		/// </remarks>
		/// <returns>DataSet [BoxId],[BoxHwTypeName],[BoxProtocolTypeName],[CommModeName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllAssignedBoxIdsDs(bool assigned,int organizationId)
		{
			return box.GetAllAssignedBoxIdsDs(assigned,organizationId);
		}
        /// <summary>
        /// Retrives box information by communication info
        /// </summary>
        /// <returns>DataSet [BoxId],[CommAddressTypeId],[CommAddressTypeName],[OrganizationId],[OrganizationName],[Description],[FleetName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        /// <param name="commInfo"></param>
        public DataSet GetBoxInfoByCommInfo(string commInfo)
        {
            DataSet dsResult = box.GetBoxInfoByCommInfo(commInfo);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetBoxInfoByCommInfo";
                }
                dsResult.DataSetName = "BoxInfo";
            }
            return dsResult;
        }
		/// <summary>
		/// Retrieves all assigned/free boxes info.
		/// </summary>
		/// <remarks>
		/// Assgned is true
		/// Free is false
		/// </remarks>
		/// <returns>DataSet [BoxId],[BoxHwTypeName],[BoxProtocolTypeName],[CommModeName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllAssignedBoxesInfo(bool assigned,int organizationId)
		{
			return box.GetAllAssignedBoxesInfo(assigned,organizationId);
		}

		/// <summary>
		/// Add new box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="fwChId"></param>
		/// <param name="boxArmed"></param>
		/// <param name="boxActive"></param>
		/// <param name="organizationId"></param>
		/// <param name="dsBoxProtocolMaxMsgs"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if box already exists.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddBox(int boxId,short fwChId,bool boxArmed,bool boxActive,int organizationId,DataSet dsBoxProtocolMaxMsgs)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Add new box
				box.AddBox(boxId,fwChId,boxArmed,boxActive,organizationId);
				// 3. Setup max messages for all protocols related for this box
				if(dsBoxProtocolMaxMsgs != null && dsBoxProtocolMaxMsgs.Tables.Count > 0)
				{
					DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
					foreach(DataRow ittr in dsBoxProtocolMaxMsgs.Tables[0].Rows)
					{
						boxSettings.AddBoxSettings(boxId,
												Convert.ToInt16(ittr["BoxProtocolTypeId"]),
												Convert.ToInt16(ittr["CommModeId"]),
												Convert.ToInt16(ittr["MaxMsgs"]),
												Convert.ToInt16(ittr["MaxTxtMsgs"]));
					}
				}
				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
				organizationId = VLF.CLS.Def.Const.unassignedIntValue;
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add new box ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 7. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
				organizationId = VLF.CLS.Def.Const.unassignedIntValue;
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}

		/// <summary>
		/// Retrives box organization
		/// </summary>
		/// <returns>OrganizationId</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int GetBoxOrganization(int boxId)
		{
			return box.GetBoxOrganization(boxId);
		}

      /// <summary>
      /// Get Box Organization and Vehicle Info
      /// Usage: Management Console -> search box
      /// DataSet: [BoxId], [OrganizationId], [OrganizationName], [VehicleId], [Description]
      /// Error: OrganizationId = -1, OrganizationName = 'Not found'
      /// Unassigned box: VehicleId = -1, Description = 'Not Assigned'
      /// Does not exist: empty DataSet
      /// </summary>
      /// <param name="boxId">Box Id</param>
      /// <returns>DataSet</returns>
      public DataSet GetBoxOrganizationVehicleInfo(int boxId)
      {
         sqlExec.ClearCommandParameters();
         sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
         return sqlExec.SQLExecuteDataset("SELECT dbo.vlfBox.BoxId, ISNULL (dbo.vlfBox.OrganizationId, -1) AS OrganizationId, ISNULL (dbo.vlfOrganization.OrganizationName, 'Not found') AS OrganizationName, ISNULL (dbo.vlfVehicleAssignment.VehicleId, -1) AS VehicleId, ISNULL (vlfVehicleInfo.Description, 'Not Assigned') AS Description from dbo.vlfBox INNER JOIN dbo.vlfOrganization ON dbo.vlfBox.OrganizationId = dbo.vlfOrganization.OrganizationId left outer join dbo.vlfVehicleAssignment on dbo.vlfBox.BoxId = dbo.vlfVehicleAssignment.BoxId left outer join dbo.vlfVehicleInfo on dbo.vlfVehicleAssignment.VehicleId = dbo.vlfVehicleInfo.VehicleId WHERE (dbo.vlfBox.BoxId = @boxId)"); 
      }

		/// <summary>
		/// Update box duration status.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="currCommunicatedDateTime"></param>
		/// <param name="currStatusSensor"></param>
		/// <param name="currStatusSpeed"></param>
		/// <param name="lastCommunicatedDateTime"></param>
		/// <param name="lastStatusSensor"></param>
		/// <param name="lastStatusSpeed"></param>
		/// <param name="prevCommunicatedDateTime"></param>
		/// <param name="prevStatusSensor"></param>
		/// <param name="prevStatusSpeed"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateDurationStatusWithTransaction(int boxId,
				DateTime currCommunicatedDateTime,short currStatusSensor,short currStatusSpeed,
				DateTime lastCommunicatedDateTime,short lastStatusSensor,short lastStatusSpeed,
				DateTime prevCommunicatedDateTime,short prevStatusSensor,short prevStatusSpeed)
		{
			try
			{
				// 1. Begin transaction
//				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Execute function
            //  don't need a transaction as only one thread can update vlfBox for a boxId  - gb 02/06/2011

				UpdateDurationStatus(boxId,currCommunicatedDateTime,currStatusSensor,currStatusSpeed,
                                 lastCommunicatedDateTime,lastStatusSensor,lastStatusSpeed,
                                 prevCommunicatedDateTime,prevStatusSensor,prevStatusSpeed);
				// 3. Save all changes
//				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 3. Rollback all changes
//				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to update box duration",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 3. Rollback all changes
//				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 3. Rollback all changes
//				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}
		/// <summary>
		///         Update box duration status.
		/// </summary>
      ///         this is a waste of time
      ///         3 calls like 
      ///            box.UpdateLastStatusSensor(boxId,currStatusSensor); 
		///		      box.UpdateLastSpeedStatus(boxId,currStatusSpeed);
		///		      box.UpdateLastStatusDateTime(boxId,currCommunicatedDateTime);
      /// 
      ///         can be generically changed to 
      ///         box.Update(string.Format("boxid={0}", boxId),
      ///                    new string[] { 
      ///                        string.Format("LastStatusSensor={0}", currStatusSensor),
      ///                        string.Format("LastSpeedStatus={0}", currStatusSensor),
      ///                        string.Format("LastStatusDateTime='{0}'", currCommunicatedDateTime.ToString()),
      ///                     } );
      /// <comment>
      /// </comment>
		/// <param name="boxId"></param>
		/// <param name="currCommunicatedDateTime"></param>
		/// <param name="currStatusSensor"></param>
		/// <param name="currStatusSpeed"></param>
		/// <param name="lastCommunicatedDateTime"></param>
		/// <param name="lastStatusSensor"></param>
		/// <param name="lastStatusSpeed"></param>
		/// <param name="prevCommunicatedDateTime"></param>
		/// <param name="prevStatusSensor"></param>
		/// <param name="prevStatusSpeed"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
#if OLD
		public void UpdateDurationStatus(int boxId,
			DateTime currCommunicatedDateTime,short currStatusSensor,short currStatusSpeed,
			DateTime lastCommunicatedDateTime,short lastStatusSensor,short lastStatusSpeed,
			DateTime prevCommunicatedDateTime,short prevStatusSensor,short prevStatusSpeed)
		{
/*
         Util.BTrace(Util.INF0, "UpdateDurationStatus -> bid={9} curr (dt={0} statusSensor={1} statusSpeed={2}) last(dt={3} statusSensor={4} statusSpeed={5}) prev(dt={6} statusSensor={7} statusSpeed={8})",
                                 currCommunicatedDateTime,currStatusSensor,currStatusSpeed, 
                                 lastCommunicatedDateTime,lastStatusSensor,lastStatusSpeed,
                                 prevCommunicatedDateTime,prevStatusSensor,prevStatusSpeed
                                 boxId) ;
*/

			#region some information about current message is missing
			if(currStatusSensor == VLF.CLS.Def.Const.unassignedShortValue || currStatusSpeed <= VLF.CLS.Def.Const.unassignedShortValue)
				return;
			#endregion
			#region last state does not exist
			if(	lastStatusSensor == VLF.CLS.Def.Const.unassignedShortValue || 
				lastStatusSpeed == VLF.CLS.Def.Const.unassignedShortValue || 
				lastCommunicatedDateTime <= VLF.CLS.Def.Const.unassignedDateTime)
			{
				box.UpdateLastStatusSensor(boxId,currStatusSensor); 
				box.UpdateLastSpeedStatus(boxId,currStatusSpeed);
				box.UpdateLastStatusDateTime(boxId,currCommunicatedDateTime);
			}
			#endregion
			#region previous state does not exist
			else if(currCommunicatedDateTime.Ticks < lastCommunicatedDateTime.Ticks && 
					prevCommunicatedDateTime <= VLF.CLS.Def.Const.unassignedDateTime)
			{
				if(currStatusSensor == lastStatusSensor && currStatusSpeed == lastStatusSpeed)
				{
					box.UpdateLastSpeedStatus(boxId,currStatusSpeed);
					box.UpdateLastStatusDateTime(boxId,currCommunicatedDateTime);
				}
				else
				{
					box.UpdatePrevStatusSensor(boxId,currStatusSensor);
					box.UpdatePrevSpeedStatus(boxId,currStatusSpeed);
					box.UpdatePrevStatusDateTime(boxId,currCommunicatedDateTime);
				}
			}
			#endregion	
			#region message arrived in the begining (old data)
			else if(currCommunicatedDateTime.Ticks < prevCommunicatedDateTime.Ticks)
				return; // do nothing
			#endregion
			#region message arrived in the middle
			else if(currCommunicatedDateTime.Ticks > prevCommunicatedDateTime.Ticks && 
                 currCommunicatedDateTime.Ticks < lastCommunicatedDateTime.Ticks)
			{
				#region status and speed are the same 
				if(currStatusSensor == lastStatusSensor && currStatusSpeed == lastStatusSpeed)
				{
					box.UpdateLastStatusDateTime(boxId,currCommunicatedDateTime);
					box.UpdateLastSpeedStatus(boxId,currStatusSpeed);
				}
				#endregion
				#region status or speed are different
				else
				{
					box.UpdatePrevStatusDateTime(boxId,currCommunicatedDateTime);
					box.UpdatePrevSpeedStatus(boxId,currStatusSpeed);
					box.UpdatePrevStatusDateTime(boxId,currCommunicatedDateTime);
				}
				#endregion
			}
			#endregion
			#region message arrived at the end
			else if(currCommunicatedDateTime.Ticks > lastCommunicatedDateTime.Ticks)
			{
				#region speed or status are different
				if(currStatusSensor != lastStatusSensor || currStatusSpeed != lastStatusSpeed)
				{
					// [OriginDateTime],[Speed]
					DataSet dsFirst = box.GetNextSensorStatus(boxId,currCommunicatedDateTime);
					#region not last packet in DB
					if(dsFirst != null && dsFirst.Tables.Count > 0 && dsFirst.Tables[0].Rows.Count > 0)
					{
						box.UpdatePrevStatusSensor(boxId,currStatusSensor);
						box.UpdatePrevSpeedStatus(boxId,currStatusSpeed);
						box.UpdatePrevStatusDateTime(boxId,currCommunicatedDateTime);
						
						box.UpdateLastSpeedStatus(boxId,Convert.ToInt16(dsFirst.Tables[0].Rows[0]["Speed"]));
						box.UpdateLastStatusDateTime(boxId,Convert.ToDateTime(dsFirst.Tables[0].Rows[0]["OriginDateTime"]));
					}
					#endregion
					#region last packet in DB
					else
					{
						box.UpdatePrevSpeedStatus(boxId,lastStatusSpeed);
						box.UpdatePrevStatusSensor(boxId,lastStatusSensor);
						box.UpdatePrevStatusDateTime(boxId,lastCommunicatedDateTime);
						
						box.UpdateLastSpeedStatus(boxId,currStatusSpeed);
						box.UpdateLastStatusSensor(boxId,currStatusSensor);
						box.UpdateLastStatusDateTime(boxId,currCommunicatedDateTime);
					}
					#endregion
				}
				#endregion
			}
			#endregion
		}
#else
      public bool GetNextSensorStatus(int boxid, DateTime from, out int speed, out DateTime odt)
      {
         return box.GetNextSensorStatusSP(boxid, from, out speed, out odt);
      }

      public void UpdateDurationStatus(int boxId,
                                       DateTime currCommunicatedDateTime, short currStatusSensor, short currStatusSpeed,
                                       DateTime lastCommunicatedDateTime, short lastStatusSensor, short lastStatusSpeed,
                                       DateTime prevCommunicatedDateTime, short prevStatusSensor, short prevStatusSpeed)
      {
         #region some information about current message is missing
         if (currStatusSensor == VLF.CLS.Def.Const.unassignedShortValue || 
             currStatusSpeed <= VLF.CLS.Def.Const.unassignedShortValue)
           return;
         #endregion

         #region last state does not exist
         if (lastStatusSensor == VLF.CLS.Def.Const.unassignedShortValue    ||
             lastStatusSpeed == VLF.CLS.Def.Const.unassignedShortValue     ||
             lastCommunicatedDateTime <= VLF.CLS.Def.Const.unassignedDateTime)
         {
            box.UpdateLastSensorSpeedDateTime(boxId, 
                                             currStatusSensor, currStatusSpeed, currCommunicatedDateTime);
/*
            box.UpdateLastStatusSensor(boxId, currStatusSensor);
            box.UpdateLastSpeedStatus(boxId, currStatusSpeed);
            box.UpdateLastStatusDateTime(boxId, currCommunicatedDateTime);
 */ 
         }
         #endregion
         #region previous state does not exist
         else if (currCommunicatedDateTime.Ticks < lastCommunicatedDateTime.Ticks &&
                  prevCommunicatedDateTime <= VLF.CLS.Def.Const.unassignedDateTime)
         {
            if (currStatusSensor == lastStatusSensor && 
                currStatusSpeed == lastStatusSpeed)
            {
               box.UpdateLastSensorSpeedDateTime(boxId, 
                                                currStatusSensor, currStatusSpeed, currCommunicatedDateTime);

/*
               box.UpdateLastSpeedStatus(boxId, currStatusSpeed);
               box.UpdateLastStatusDateTime(boxId, currCommunicatedDateTime);
 */ 
            }
            else
            {
               box.UpdatePrevSensorSpeedDateTime(boxId, 
                                                 currStatusSensor,currStatusSpeed, currCommunicatedDateTime);
/*
               box.UpdatePrevStatusSensor(boxId, currStatusSensor);
               box.UpdatePrevSpeedStatus(boxId, currStatusSpeed);
               box.UpdatePrevStatusDateTime(boxId, currCommunicatedDateTime);
 */ 
            }
         }
         #endregion
         #region message arrived in the begining (old data)
         else if (currCommunicatedDateTime.Ticks < prevCommunicatedDateTime.Ticks)
            return; // do nothing
         #endregion
         #region message arrived in the middle
         else if (currCommunicatedDateTime.Ticks > prevCommunicatedDateTime.Ticks &&
                 currCommunicatedDateTime.Ticks < lastCommunicatedDateTime.Ticks)
         {
            #region status and speed are the same
            if (currStatusSensor == lastStatusSensor && currStatusSpeed == lastStatusSpeed)
            {
               box.UpdateLastSensorSpeedDateTime(boxId, 
                                                 currStatusSensor, currStatusSpeed, currCommunicatedDateTime);

/*
               box.UpdateLastStatusDateTime(boxId, currCommunicatedDateTime);
               box.UpdateLastSpeedStatus(boxId, currStatusSpeed);
 */ 
            }
            #endregion
            #region status or speed are different
            else
            {
               box.UpdatePrevSensorSpeedDateTime(boxId, 
                                                 currStatusSensor, currStatusSpeed, currCommunicatedDateTime);
/*
               box.UpdatePrevStatusDateTime(boxId, currCommunicatedDateTime);
               box.UpdatePrevSpeedStatus(boxId, currStatusSpeed);
               box.UpdatePrevStatusDateTime(boxId, currCommunicatedDateTime);
 */ 
            }
            #endregion
         }
         #endregion
         #region message arrived at the end
         else if (currCommunicatedDateTime.Ticks > lastCommunicatedDateTime.Ticks)
         {
            #region speed or status are different
            if (currStatusSensor != lastStatusSensor || currStatusSpeed != lastStatusSpeed)
            {
               #region not last packet in DB
               // [OriginDateTime],[Speed]
               int speed;
               DateTime odt;
               if ( box.GetNextSensorStatusSP(boxId, currCommunicatedDateTime, out speed, out odt))               
               {
                  box.UpdateLastAndPreviousSensorSpeedDateTime(boxId,
                                                               -1, (short)speed, odt,
                                                               currStatusSensor, currStatusSpeed, currCommunicatedDateTime);

/*
                  box.UpdatePrevSensorSpeedDateTime(boxId, 
                                                    currStatusSensor,currStatusSpeed, currCommunicatedDateTime);

                  box.UpdatePrevStatusSensor(boxId, currStatusSensor);
                  box.UpdatePrevSpeedStatus(boxId, currStatusSpeed);
                  box.UpdatePrevStatusDateTime(boxId, currCommunicatedDateTime);

                  box.UpdateLastSensorSpeedDateTime(boxId, -1, (short)speed, odt ) ;

                  box.UpdateLastSpeedStatus(boxId, Convert.ToInt16(dsFirst.Tables[0].Rows[0]["Speed"]));
                  box.UpdateLastStatusDateTime(boxId, Convert.ToDateTime(dsFirst.Tables[0].Rows[0]["OriginDateTime"]));
 */ 
               }
               #endregion
               #region last packet in DB
               else
               {
                  box.UpdateLastAndPreviousSensorSpeedDateTime(boxId, 
                                                               currStatusSensor,currStatusSpeed, currCommunicatedDateTime,
                                                               lastStatusSensor, lastStatusSpeed, lastCommunicatedDateTime);
/*
                  box.UpdatePrevSensorSpeedDateTime(boxId, lastStatusSpeed, 
                                                    lastStatusSensor, lastCommunicatedDateTime); 


                  box.UpdatePrevSpeedStatus(boxId, lastStatusSpeed);
                  box.UpdatePrevStatusSensor(boxId, lastStatusSensor);
                  box.UpdatePrevStatusDateTime(boxId, lastCommunicatedDateTime);
*/
/*
                  box.UpdateLastSensorSpeedDateTime(boxId, currStatusSensor,
                                                 currStatusSpeed, currCommunicatedDateTime);

                  box.UpdateLastSpeedStatus(boxId, currStatusSpeed);
                  box.UpdateLastStatusSensor(boxId, currStatusSensor);
                  box.UpdateLastStatusDateTime(boxId, currCommunicatedDateTime);
 */ 
               }
               #endregion
            }
            #endregion
         }
         #endregion
      }
#endif
		/// <summary>
		/// Update box sensor status.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxArmed"></param>
		/// <param name="sensorMask"></param>
		/// <param name="geoFenceEnabled"></param>
		/// <param name="boxActive"></param>
		/// <param name="currCommunicatedDateTime"></param>
		/// <param name="currStatusSensor"></param>
		/// <param name="currStatusSpeed"></param>
		/// <param name="lastCommunicatedDateTime"></param>
		/// <param name="lastStatusSensor"></param>
		/// <param name="lastStatusSpeed"></param>
		/// <param name="prevCommunicatedDateTime"></param>
		/// <param name="prevStatusSensor"></param>
		/// <param name="prevStatusSpeed"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateStatus(int boxId,bool boxArmed,Int64 sensorMask,bool geoFenceEnabled,bool boxActive,
			DateTime currCommunicatedDateTime,short currStatusSensor,short currStatusSpeed,
			DateTime lastCommunicatedDateTime,short lastStatusSensor,short lastStatusSpeed,
			DateTime prevCommunicatedDateTime,short prevStatusSensor,short prevStatusSpeed)
		{
			try
			{
				// 1. Begin transaction
//				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
            // both updates are protected by the lock at the CommSession level  - gb 02/06/2011
				box.UpdateStatus(boxId,boxArmed,currCommunicatedDateTime,sensorMask,geoFenceEnabled,boxActive);
				UpdateDurationStatus(boxId,
                                 currCommunicatedDateTime,currStatusSensor,currStatusSpeed,
                                 lastCommunicatedDateTime,lastStatusSensor,lastStatusSpeed,
                                 prevCommunicatedDateTime,prevStatusSensor,prevStatusSpeed);
				// 4. Save all changes
//				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 7. Rollback all changes
//				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to update box status",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 7. Rollback all changes
//				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 7. Rollback all changes
//				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}	
		/// <summary>
		/// Update box dormant status.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="originatedDateTime"></param>
		/// <param name="currDormantStatus"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateDormantStatus(int boxId,DateTime originatedDateTime, short currDormantStatus)
		{
			box.UpdateDormantStatus(boxId,originatedDateTime,currDormantStatus);
		}	
		
		/// <summary>
		/// Update box position.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="lastValidDateTime"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdatePosition(int boxId,DateTime lastValidDateTime,double latitude,double longitude)
		{
			box.UpdatePosition(boxId,lastValidDateTime,latitude,longitude);
		}
		/// <summary>
		/// Update box position.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="lastValidDateTime"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="speed"></param>
		/// <param name="heading"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if box does not exist.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdatePosition(int boxId,DateTime lastValidDateTime,double latitude,double longitude,short speed,short heading)
		{
			box.UpdatePosition(boxId,lastValidDateTime,latitude,longitude,speed,heading);
		}
		/// <summary>
		/// Get box last info
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [BoxId],
		/// [OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[StreetAddress],
		/// [LastCommunicatedDateTime],[SensorMask],[BoxArmed],[GeoFenceEnabled],
		/// [LastStatusDateTime],[BoxActive],
		/// [LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetBoxLastInfo(int boxId)
		{
			DataSet dsResult = box.GetBoxLastInfo(boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "LastKnownPosition";
				}
				dsResult.DataSetName = "BoxInfo";
			}
			return dsResult;
		}

      /** \comment   returns only one row !!
       *             could use a reader
       */ 
      public DataSet GetBoxLastInfoSP(int boxId)
      {
         DataSet dsResult = box.GetBoxLastInfoSP(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "LastKnownPosition";
            }
            dsResult.DataSetName = "BoxInfo";
         }
         return dsResult;
      }

      /// <summary>
      ///         modified to fit exactly the needs for SLS only
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns></returns>
      /// <comment>
      ///         returns only one row !!
      /// </comment>
      public DataSet GetBoxLastInfoSPSLS(int boxId)
      {
         DataSet dsResult = box.GetBoxLastInfoSPSLS(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "LastKnownPosition";
            }
            dsResult.DataSetName = "BoxInfo";
         }
         return dsResult;
      }

      public void GetSLSVehicleInfo(int boxId, out string licensePlate,
                                    out short vehicleType,
                                    out DateTime lastValidDateTime)
      {
         box.GetSLSVehicleInfo(boxId, out licensePlate,
                                    out  vehicleType,
                                    out lastValidDateTime);
      }

		/// <summary>
		/// Get box last info
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="boxId"></param>
		/// <returns>DataSet [BoxId],
		/// [OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[StreetAddress],
		/// [LastCommunicatedDateTime],[SensorMask],[BoxArmed],[GeoFenceEnabled],
		/// [LastStatusDateTime],[BoxActive]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetBoxLastInfo(int userId,int boxId)
		{
			DataSet dsResult = box.GetBoxLastInfo(userId,boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "LastKnownPosition";
				}
				dsResult.DataSetName = "BoxInfo";
			}
			return dsResult;
		}	
		/// <summary>
		/// Get box valid GPS datetime
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>[OriginDateTime]/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DateTime GetBoxLastValidGpsDateTime(int boxId)
		{
			return box.GetBoxLastValidGpsDateTime(boxId);
		}
		/// <summary>
		/// Get box last communicated datetime
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>[LastCommunicatedDateTime]/// </returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DateTime GetBoxLastCommunicatedDateTime(int boxId)
		{
			return box.GetBoxLastCommunicatedDateTime(boxId);
		}

      /// <summary>
      /// Get box last communicated datetime and sensor mask
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>DataSet [LastCommunicatedDateTime, Dormant]/// </returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetBoxLastCommunicationInfo(int boxId)
      {
         DataSet dsInfo = new DataSet("BoxInfo");
         dsInfo = box.GetRowsBySql("SELECT LastCommunicatedDateTime, Dormant FROM vlfBox WHERE BoxId = @boxid", new SqlParameter[] { new SqlParameter("@boxid", boxId) });
         if (dsInfo.Tables.Count > 0)
            dsInfo.Tables[0].TableName = "LastCommunicationInfo";

         return dsInfo;
      }
      
      /// <summary>
		/// Returns true if box is armed.
		/// Throw exception in case of error.
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>
		/// true - if box is armed
		/// flase - if not
		/// </returns>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException if box does not exist.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public bool IsArmed(int boxId)
		{
			return box.IsArmed(boxId);
		}		
		/// <summary>
		/// Returns true if box is active.
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>
		/// true - if box is active
		/// flase - if not
		/// </returns>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException if box does not exist.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public bool IsActive(int boxId)
		{
			return box.IsArmed(boxId);
		}

      /// <summary>
      ///      when you switch the channel
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="fwChId"></param>
      /// <param name="protocolTypeId"></param>
      /// <param name="commModeId"></param>
      public void UpdateBoxFwChIdBantek(int boxId, short fwChId, short protocolTypeId, short commModeId)
      {
         try
         {
            Util.BTrace(Util.INF0, "UpdateBoxFwChIdBantek -> box[{0} fwchid[{1}] protocol[{2}] commModeId[{3}]",
                  boxId, fwChId, protocolTypeId, commModeId);

            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Update protocol/commMode
            DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
            boxSettings.UpdateProtocolTypeCommMode(boxId, protocolTypeId, commModeId);
            
            DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
            // 3. Update box configuration type
            box.UpdateBoxFwChId(boxId, fwChId);

				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update box configuration type. ";
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
			
	         Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update box configuration type. ";
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}      
		/// <summary>
		/// Update box fwChId.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="fwChId"></param>
		/// <exception cref="DASAppInvalidValueException">Thrown DASAppInvalidValueException if new configuration has different hardware type.</exception>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppResultNotFoundException if box does not exist.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateBoxFwChId(int boxId,short fwChId)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// Clear old user-defined outputs
				DB.BoxOutputsCfg outputs = new DB.BoxOutputsCfg(sqlExec);
				outputs.DeleteOutputsByBoxId(boxId);

				// Clear old user-defined sensors
				DB.BoxSensorsCfg sensors = new DB.BoxSensorsCfg(sqlExec);
				sensors.DeleteSensorsByBoxId(boxId);
				
				// 2. Delete box all communications info
				DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
				boxCommInfo.DeleteCommInfoByBoxId(boxId);

				// 3. Update box configuration type
				box.UpdateBoxFwChId(boxId,fwChId);

				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update box configuration type. ";
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
			
	         Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update box configuration type. ";
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}
		/// <summary>
		/// Update box fwChId.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="newFwId"></param>
		/// <param name="commModeId"></param>
		/// <param name="isDual"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown DASAppResultNotFoundException if cannot retrieve  fwChId.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateBoxFwChId(int boxId,short newFwId, short commModeId, bool isDual)
		{
         Util.BTrace(Util.INF0, "-- Box.UpdateBoxFwChId -> boxId[{0}] newFwId[{1}] commModeId[{2}] isDual[{3}]", boxId, newFwId, commModeId, isDual);
			#region 1. Retrieve fwChId
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			short fwChId = boxConfig.GetFwChId(newFwId,commModeId,isDual);
			if(fwChId == VLF.CLS.Def.Const.unassignedShortValue)
				throw new DASAppResultNotFoundException("Unable to retrieve firmware Ch Id by boxId=" + boxId + " commModeId=" + commModeId + " isDual=" + isDual);
			#endregion

         Util.BTrace(Util.INF0, "-- Box.UpdateBoxFwChId -> FwChId[{0}]", fwChId);

			#region 2. Update Box Firmware Ch Id
			try
			{
				// 2.1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2.2. Update box configuration type
				box.UpdateBoxFwChId(boxId,fwChId);
				
				#region 2.3. Update box protocol settings
				// 4. Delete old box protocol settings
            DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);        
            boxSettings.DeleteBoxSettings(boxId);
            // 5. Get fwChInfo (BoxProtocolTypeId,CommModeId)
            //[FwChId],[ChPriority],[FwId],[FwName],[ChId],[ChName],
            //[CommModeId],[CommModeName],
            //[BoxProtocolTypeId],[BoxProtocolTypeName],
            //[BoxHwTypeId],[BoxHwTypeName]
            DataSet dsFwChConfig = boxConfig.GetConfigInfo(fwChId);
            if (dsFwChConfig == null || dsFwChConfig.Tables.Count == 0 || dsFwChConfig.Tables[0].Rows.Count == 0)
               throw new DASAppResultNotFoundException("Unable to retrieve firmware Ch configuration by fwChId=" + fwChId);
            // 6. Fill box settings
            foreach (DataRow ittrFwChConfig in dsFwChConfig.Tables[0].Rows)
            {
               boxSettings.AddBoxSettings(boxId,
                  Convert.ToInt16(ittrFwChConfig["BoxProtocolTypeId"]),
                  Convert.ToInt16(ittrFwChConfig["CommModeId"]),
                  0, 0);
            }
            
				#endregion

/*				
				#region 2.4. Purge messages
            DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);           
            // 2.4.1 Retrieve box old messages [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]
            DataSet dsOldMessages = boxMsgSeverity.GetAllSupportedMessagesByBoxId(boxId);
            if (dsOldMessages == null || dsOldMessages.Tables.Count == 0 || dsOldMessages.Tables[0].Rows.Count == 0)
               throw new DASAppResultNotFoundException("Unable to retrieve all old supported messages by box=" + boxId);

            // 2.4.2 Delete box old messages
            boxMsgSeverity.DeleteRecordByBoxId(boxId);

            // 2.4.3 Retrieve box new messages [BoxMsgInTypeId],[BoxMsgInTypeName]
            DataSet dsNewMessages = boxConfig.GetAllSupportedMessagesByFwChId(fwChId);
            if (dsNewMessages == null || dsNewMessages.Tables.Count == 0 || dsNewMessages.Tables[0].Rows.Count == 0)
               throw new DASAppResultNotFoundException("Unable to retrieve all new supported messages by box=" + boxId + " and FwChId=" + fwChId);

            #region 2.4.4 Add message with old alarm settings
            short msgAlarmLevel = 0;
            foreach (DataRow ittrNewMsgs in dsNewMessages.Tables[0].Rows)
            {
               msgAlarmLevel = 0;
               #region 2.4.4.1 Lookup for message old settings
               foreach (DataRow ittrOldMsgs in dsOldMessages.Tables[0].Rows)
               {
                  if (Convert.ToInt16(ittrNewMsgs["BoxMsgInTypeId"]) == Convert.ToInt16(ittrOldMsgs["BoxMsgInTypeId"]))
                  {
                     if (Convert.ToInt16(ittrOldMsgs["AlarmLevel"]) != 0)
                        msgAlarmLevel = Convert.ToInt16(ittrNewMsgs["AlarmLevel"]);
                     break;
                  }
               }
               #endregion 2.4.4.1
               // Add box new message
               boxMsgSeverity.AddMsgSeverity(boxId,
                  (VLF.CLS.Def.Enums.MessageType)Convert.ToInt16(ittrNewMsgs["BoxMsgInTypeId"]),
                  (VLF.CLS.Def.Enums.AlarmSeverity)msgAlarmLevel);
            }
            #endregion 2.4.4

            #endregion 2.4.
*/
            // 2.5. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 2.5. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to update box configuration type. ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 2.5. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 2.5. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException("Unable to update box configuration type. " + objException.Message);
			}
			#endregion
		}
		/// <summary>
		/// Update state(Armed/Disarmed).
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="armed"></param>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppResultNotFoundException if box does not exist.</exception>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public void SetArmed(int boxId,bool armed)
		{
			box.SetArmed(boxId,armed);
		}
		/// <summary>
		/// Update box status
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxActive"></param>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppResultNotFoundException if box does not exist.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void SetActive(int boxId,bool boxActive)
		{
			box.SetActive(boxId,boxActive);
		}
		/// <summary>
      /// Retrieves all supported messages by fwChId
		/// </summary>
		/// <param name="fwChId"></param>
		/// <returns>DataSet [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetAllSupportedMessagesByFwChId(short fwChId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
         DataSet dsResult = boxConfig.GetAllSupportedMessagesByFwChId(fwChId);
			if(dsResult != null)
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxMessages" ;
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}
		/// <summary>
		/// Get box configuration information. 
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet[BoxId],[FwId],[MaxMsgs],[MaxTxtMsgs],
		///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
		///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
		///					[CommModeId],[CommModeName] // box communication mode information
		///					[ChPriority]
		/// </returns>
		/// <remarks> Ordered by "ChPriority" field </remarks>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetBoxConfigInfo(int boxId)
		{
			DataSet dsResult = box.GetBoxConfigInfo(boxId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxConfigInfo";
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}

      /// <summary>
      /// Get box configuration information. 
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>DataSet[BoxId],[FwId],[MaxMsgs],[MaxTxtMsgs],
      ///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
      ///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
      ///					[CommModeId],[CommModeName] // box communication mode information
      ///					[ChPriority]
      /// </returns>
      /// <remarks> Ordered by "ChPriority" field </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetBoxConfigInfoFeatures(int boxId)
      {
         DataSet dsResult = box.GetBoxConfigInfoFeatures(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "BoxConfigInfo";
            }
            dsResult.DataSetName = "Box";
         }
         return dsResult;
      }

      /// <summary>
      /// Get box configuration information excluding dcl
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>DataSet[BoxId],[FwId],[MaxMsgs],[MaxTxtMsgs],
      ///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
      ///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
      ///					[CommModeId],[CommModeName] // box communication mode information
      ///					[ChPriority]
      /// </returns>
      /// <remarks> Ordered by "ChPriority" field </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetBoxConfiguration(int boxId)
      {
         DataSet dsResult = box.GetBoxConfiguration(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "BoxConfigInfo";
            }
            dsResult.DataSetName = "Box";
         }
         return dsResult;
      }

      /// <summary>
      /// Get box configuration information excluding dcl
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>DataSet[BoxId],[FwId],[MaxMsgs],[MaxTxtMsgs],
      ///					[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum] // HW type information
      ///					[BoxProtocolTypeId],[BoxProtocolTypeName] // box protocol type information
      ///					[CommModeId],[CommModeName] // box communication mode information
      ///					[ChPriority]
      /// </returns>
      /// <remarks> Ordered by "ChPriority" field </remarks>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public DataSet GetBoxConfigurationFeatures(int boxId)
      {
         DataSet dsResult = box.GetBoxConfigurationFeatures(boxId);
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "BoxConfigInfo";
            }
            dsResult.DataSetName = "Box";
         }
         return dsResult;
      }

		/// <summary>
		/// Get firmware channel
		/// </summary>
		/// <param name="selectedFwId"></param>
		/// <param name="selectedPrimeCommMode"></param>
		/// <param name="selectedSecCommMode"></param>
		/// <returns>fwChId</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public short GetFwChId(short selectedFwId,short selectedPrimeCommMode,short selectedSecCommMode)
		{
			return box.GetFwChId(selectedFwId,selectedPrimeCommMode,selectedSecCommMode);
		}

      /// <summary>
      /// Get firmware channel id
      /// </summary>
      /// <param name="selectedFwId"></param>
      /// <param name="protocolTypeId"></param>
      /// <returns>fwChId</returns>
      /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      public short GetFwChId(short selectedFwId, short protocolTypeId)
      {
         return box.GetFwChId(selectedFwId, protocolTypeId);
      }

		/// <summary>
		/// Get box firmware info. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [BoxId],[FwId],[FwName],[FwChId],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum],[FwTypeId],[FwLocalPath],[FwOAPPath],[FwDateReleased],[MaxGeozones]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetBoxFwChInfo(int boxId)
		{
			return box.GetBoxFwChInfo(boxId);
		}		
		/// <summary>
		/// Returns box protocol type Ids. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <param name="commandTypeId"></param>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetCommandProtocolTypesInfo(int boxId,int userId,short commandTypeId)
		{
			DB.BoxProtocolTypeCmdOutType boxProtocolTypeCmdOutType = new DB.BoxProtocolTypeCmdOutType(sqlExec);
			return boxProtocolTypeCmdOutType.GetCommandProtocolTypesInfo(boxId,userId,commandTypeId);
		}		
		/// <summary>
		/// Retrieves all supported protocol types for current output
		/// Throws exception in case of wrong result (see TblGenInterfaces class).
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <param name="outputId"></param>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
		public DataSet GetOutputProtocolTypesInfo(int boxId,int userId,short outputId)
		{
			DB.BoxProtocolTypeCmdOutType boxProtocolTypeCmdOutType = new DB.BoxProtocolTypeCmdOutType(sqlExec);
			return boxProtocolTypeCmdOutType.GetOutputProtocolTypesInfo(boxId,userId,outputId);
		}
		/// <summary>
		/// Retrieves secondary communication info
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <param name="commandID"></param>
		/// <returns>DataSet [BoxProtocolTypeId],[CommModeId]</returns>
		public DataSet GetSecondaryCommInfo(int boxId,int userId,short commandID)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			return boxConfig.GetSecondaryCommInfo(boxId,userId,commandID);
		}
        /// <summary>
        /// Retrieves primary communication info
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="userId"></param>
        /// <param name="commandID"></param>
        /// <returns>DataSet [BoxProtocolTypeId],[CommModeId]</returns>
        public DataSet GetPrimaryCommInfo(int boxId, int userId, short commandID)
        {
            DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
            return boxConfig.GetPrimaryCommInfo(boxId, userId, commandID);
        }
		/// <summary>
		/// Checks if both FW have the same primary protocol
		/// </summary>
		/// <param name="oldFwChId "></param>
		/// <param name="newFwChId "></param>
		/// <returns>true if it is same, otherwise returns false </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsSamePrimaryProtocol(short oldFwChId,short newFwChId)
		{
			DB.BoxConfig boxConfig = new DB.BoxConfig(sqlExec);
			return boxConfig.IsSamePrimaryProtocol(oldFwChId,newFwChId);
		}

		#endregion

		#region Box Messages
		/// <summary>
		/// Retrieves alarm info for specific message type
		/// </summary>
		/// <returns>DataSet [AlarmLevel],[AlarmLevelName],[CreateAlarm]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		/// <param name="boxId"></param>
		/// <param name="boxMsgInTypeId"></param>
		public DataSet GetAlarmInfo(int boxId,VLF.CLS.Def.Enums.MessageType boxMsgInTypeId)
		{
         DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
			return boxMsgSeverity.GetAlarmInfo(boxId,boxMsgInTypeId);

		}
		/// <summary>
		/// Check if we should create alarm for current severity
		/// </summary>
		/// <param name="alarmSeverity"></param> 
		/// <returns></returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public bool IsAlarm(VLF.CLS.Def.Enums.AlarmSeverity alarmSeverity)
		{

         DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
			return boxMsgSeverity.IsAlarm(alarmSeverity);

		}
		/// <summary>
		/// Update box message severity
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxMsgInTypeId"></param>
		/// <param name="alarmLevel"></param>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateMsgSeverity(int boxId,VLF.CLS.Def.Enums.MessageType boxMsgInTypeId,VLF.CLS.Def.Enums.AlarmSeverity alarmLevel)
		{
			DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
			boxMsgSeverity.UpdateMsgSeverity(boxId,boxMsgInTypeId,alarmLevel);
		}

        [Obsolete("Use AddMsgSeverity(int , DataSet , bool) instead")]
        public void AddMsgSeverity(int boxId, DataSet dsBoxMessagesSeverity)
        {
            AddMsgSeverity(boxId,dsBoxMessagesSeverity,true);
        }

		/// <summary>
		/// Add new message severity to the box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="dsBoxMessagesSeverity"></param>
		/// <exception cref="DASAppInvalidValueException">Thrown if box messages severities are empty.</exception>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if message severity already exist for this box.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddMsgSeverity(int boxId,DataSet dsBoxMessagesSeverity,bool useTransaction)
		{
			if(	dsBoxMessagesSeverity == null || 
				dsBoxMessagesSeverity.Tables.Count == 0 || 
				dsBoxMessagesSeverity.Tables[0].Rows.Count == 0)
				throw new DASAppInvalidValueException("Box messages severities are empty.");
			DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
			try
			{
				// 1. Begin transaction
                if (useTransaction)
				    sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				
				// 2. Removes messages from previous setup
				boxMsgSeverity.DeleteRecordByBoxId(boxId);
		
				// 3. setut new messages severity
				foreach(DataRow ittr in dsBoxMessagesSeverity.Tables[0].Rows)
				{
					boxMsgSeverity.AddMsgSeverity(boxId,
							(VLF.CLS.Def.Enums.MessageType)Convert.ToInt16(ittr["BoxMsgInTypeId"]),
							(VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt16(ittr["AlarmLevel"]));
				}
				// 6. Save all changes
                if (useTransaction)
				    sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 6. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add box message severity ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 6. Rollback all changes
                if (useTransaction)
				    sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}
		/// <summary>
		/// Retrieves all supported messages by box
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel}</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllSupportedMessagesByBoxId(int boxId)
		{

         DB.BoxMsgSeverity boxMsgSeverity = new DB.BoxMsgSeverity(sqlExec);
			DataSet dsResult = boxMsgSeverity.GetAllSupportedMessagesByBoxId(boxId);
			if(dsResult != null)
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxMessages" ;
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;

		}
		/// <summary>
		/// Retrieves records with empty StreetAddresses
		/// </summary>
		/// <returns>DataSet [BoxId],[Latitude],[Longitude]</returns>
		public DataSet GetEmptyStreetAddress(int cmdTimeOut)
		{
			DataSet dsResult = box.GetEmptyStreetAddress(cmdTimeOut);
			if(dsResult != null)
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetEmptyStreetAddress" ;
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}
      /// <summary>
      /// Retrieves records with empty NearestLandmark
      /// </summary>
      /// <returns>DataSet [BoxId],[Latitude],[Longitude]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <param name="cmdTimeOut"></param>
      public DataSet GetEmptyNearestLandmark()
      {
         DataSet dsResult = box.GetEmptyNearestLandmark();
         if (dsResult != null)
         {
            if (dsResult.Tables.Count > 0)
            {
               dsResult.Tables[0].TableName = "GetEmptyNearestLandmark";
            }
            dsResult.DataSetName = "Box";
         }
         return dsResult;
      }
      /// <summary>
      /// Updates record with nearestLandmark
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="nearestLandmark"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if street address already exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void UpdateNearestLandmark(int boxId, string nearestLandmark)
      {
         box.UpdateNearestLandmark(boxId, nearestLandmark);
      }

		/// <summary>
        /// Updates record with street address and/or nearestLandmark
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="streetAddress"></param>
		/// <param name="cmdTimeOut"></param>
		/// <param name="usageYear"></param>
        /// <param name="usageMonth"></param>
        /// <param name="nearestLandmark"></param>
        /// <param name="mapId"></param>
        public void UpdateStreetAddress(int boxId, string streetAddress, int cmdTimeOut, short usageYear, short usageMonth, string nearestLandmark, short mapId)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Update box street address
                box.UpdateStreetAddress(boxId, streetAddress, cmdTimeOut, nearestLandmark);
				// 3. Add box map usage
                mapEngine.AddMapBoxUsage(boxId, (short)VLF.CLS.Def.Enums.MapTypes.StreetAddress, usageYear, usageMonth, mapId);
				// 4. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add new box ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}

        public void UpdateStreetAddress(int boxId, float lat, float lon, string streetAddress)
        {
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update box street address
                box.UpdateStreetAddress(boxId, lat, lon, streetAddress);
                
                // 3. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new box ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }

      #endregion

		#region Box Protocol Settings
		/// <summary>
		/// Add new box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <param name="maxMsgs"></param>
		/// <param name="maxTxtMsgs"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if box already exists.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddBoxSettings(int boxId,short boxProtocolTypeId,short commModeId,short maxMsgs,short maxTxtMsgs)
		{
			DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
			boxSettings.AddBoxSettings(boxId,boxProtocolTypeId,commModeId,maxMsgs,maxTxtMsgs);
		}
		/// <summary>
		/// Delete existing box.
		/// Throws exception in case of wrong result (see TblOneIntPrimaryKey class).
		/// </summary>
		/// <param name="boxId"></param> 
		public int DeleteBoxSettings(int boxId)
		{
			DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
			return boxSettings.DeleteBoxSettings(boxId);
		}
		/// <summary>
		/// Update box config Id.
		/// Throw exception in case of error.
		///		- DASAppResultNotFoundException
		///		- DASException
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <param name="maxMsgs"></param>
		/// <param name="maxTxtMsgs"></param>
		public void UpdateBoxSettings(int boxId,short boxProtocolTypeId,short commModeId,short maxMsgs,short maxTxtMsgs)
		{
			DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
			boxSettings.UpdateBoxSettings(boxId,boxProtocolTypeId,commModeId,maxMsgs,maxTxtMsgs);
		}
		/// <summary>
		/// Retrieves box max messages per protocol.
		/// </summary>
		/// <returns>DataSet [BoxProtocolTypeId],[CommModeId][MaxMsgs],[MaxTxtMsgs]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetBoxSettingsInfo(int boxId)
		{
			DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
			DataSet dsResult = boxSettings.GetBoxSettingsInfo(boxId);
			if(dsResult != null)
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "BoxSettings" ;
				}
				dsResult.DataSetName = "Box";
			}
			return dsResult;
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="boxProtocolTypeId"></param>
      /// <param name="commModeId"></param>
      public void UpdateProtocolTypeCommMode(int boxId, short boxProtocolTypeId, short commModeId)
      {
         DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
         boxSettings.UpdateProtocolTypeCommMode(boxId, boxProtocolTypeId, commModeId);
      }
		#endregion

		/// <summary>
		/// Retrieves command name by id
		/// </summary>
		/// <param name="boxCmdOutTypeId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public string GetBoxCmdName(short boxCmdOutTypeId)
		{
			DB.BoxCmdOutType boxCmdOutType = new DB.BoxCmdOutType(sqlExec);
			return boxCmdOutType.GetNameById(boxCmdOutTypeId);
		}

      public DataSet GetAllChannelsByBoxId(int boxId)
      {
         return null;
      }

        [Obsolete("Use AddBox(int , short , bool , bool , int , long , DataSet , bool) instead")]
        public void AddBox(int boxId, short fwChId, bool boxArmed, bool boxActive, int organizationId, long features, DataSet dsBoxProtocolMaxMsgs)
        {
            AddBox(boxId, fwChId, boxArmed, boxActive, organizationId, features, dsBoxProtocolMaxMsgs,true);
        }

      /// <summary>
      /// Add a new box with firmware features
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="fwChId"></param>
      /// <param name="boxArmed"></param>
      /// <param name="boxActive"></param>
      /// <param name="organizationId"></param>
      /// <param name="features"></param>
      /// <param name="dsBoxProtocolMaxMsgs"></param>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if box already exists.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void AddBox(int boxId, short fwChId, bool boxArmed, bool boxActive, int organizationId, long features, DataSet dsBoxProtocolMaxMsgs,bool useTransaction)
      {
         try
         {
            // 1. Begin transaction
             if (useTransaction)
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

            // 2. Add a new box
            box.AddBox(boxId, fwChId, boxArmed, boxActive, organizationId, features);

            // 3. Setup max messages for all protocols related for this box
            if (dsBoxProtocolMaxMsgs != null && dsBoxProtocolMaxMsgs.Tables.Count > 0)
            {
               DB.BoxSettings boxSettings = new DB.BoxSettings(sqlExec);
               foreach (DataRow ittr in dsBoxProtocolMaxMsgs.Tables[0].Rows)
               {
                  boxSettings.AddBoxSettings(boxId,
                                    Convert.ToInt16(ittr["BoxProtocolTypeId"]),
                                    Convert.ToInt16(ittr["CommModeId"]),
                                    Convert.ToInt16(ittr["MaxMsgs"]),
                                    Convert.ToInt16(ittr["MaxTxtMsgs"]));
               }
            }
            // 4. Save all changes
             if (useTransaction)
                sqlExec.CommitTransaction();
         }
         catch (SqlException objException)
         {
            // 7. Rollback all changes
            organizationId = VLF.CLS.Def.Const.unassignedIntValue;
             if (useTransaction)
                sqlExec.RollbackTransaction();
            Util.ProcessDbException("Unable to add new box ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            // 7. Rollback all changes
             if (useTransaction)
                sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            // 7. Rollback all changes
             organizationId = VLF.CLS.Def.Const.unassignedIntValue;
             if (useTransaction)                
                sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
      }

      /// <summary>
      /// Update box features mask
      /// </summary>
      /// <param name="boxId">Box ID</param>
      /// <param name="features">Box Feature Mask</param>
      /// <exception cref="DASAppInvalidValueException">Thrown DASAppInvalidValueException if new configuration has different hardware type.</exception>
      /// <exception cref="DASAppWrongResultException">Thrown DASAppResultNotFoundException if box does not exist.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void UpdateBoxFeatures(int boxId, long features)
      {
         string prefixMsg = "Unable to update box features. ";
         try
         {
            // 3. Update box features
            box.UpdateRow("SET FwAttributes1 = @features WHERE BoxId = @boxId", 
               new SqlParameter("@boxId", boxId), new SqlParameter("@features", features));
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (Exception objException)
         {
            throw new DASException(prefixMsg + " " + objException.Message);
         }
      }

      /// <summary>
      /// Checks if Id is available
      /// </summary>
      /// <param name="boxId">Box Id</param>
      /// <returns>True if no box with current Id exists, false if yes</returns>
      public bool IsIDAvailable(int boxId)
      {
         return box.GetRowsByIntField("BoxId", boxId, "Available boxes").Tables[0].Rows.Count == 0;
      }

      /// <summary>
      /// Get org. boxes, primary channels only, usage: Man. Console
      /// </summary>
      /// <param name="assigned">True - assigned, false - unassigned</param>
      /// <param name="organizationId">Org. Id</param>
      /// <returns>Boxes dataset:[vlfBox.BoxId, vlfBox.OrganizationId, vlfFirmwareChannelReference.FwChId, vlfFirmware.FwId, 
      /// vlfFirmware.FwName, vlfBoxHwType.BoxHwTypeId, vlfBoxHwType.BoxHwTypeName, vlfChannels.ChName, 
      /// vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfCommMode.CommModeId, 
      /// vlfCommMode.CommModeName, OAPPort]</returns>
      public DataSet GetBoxesInfo(bool assigned, int organizationId)
      {
         return box.GetBoxesInfo(assigned, organizationId);
      }

      /// <summary>
      /// Get all boxes for a company (assigned and unassigned)
      /// Usage: Man. console
      /// </summary>
      /// <param name="organizationId">Organization Id</param>
      /// <returns>ArrayList of Box Ids</returns>
      public ArrayList GetAllBoxIds(int organizationId)
      {
         ArrayList alist = new ArrayList();
         DataSet ds = box.GetRowsByIntField("OrganizationId", organizationId, "Get all boxes");
         if (Util.IsDataSetValid(ds))
         {
            foreach (DataRow drow in ds.Tables[0].Rows)
               alist.Add(drow["BoxId"]);
         }
         return alist;
      }

      /// <summary>
      /// Move Box to another Organization
      /// </summary>
      /// <param name="userId">Must be hgi user</param>
      /// <param name="boxId">Box Id to move</param>
      /// <param name="newOrgId">New organization Id</param>
      /// <returns>Updated rows</returns>
      public int ChangeOrganization(int userId, int boxId, int newOrgId)
      {
         DAS.DB.User user = new VLF.DAS.DB.User(this.sqlExec);
         if (!user.ValidateHGISuperUser(userId))
            throw new Exception("You don't have permissions to move a box!");
         
         if (IsAssigned(boxId))
            throw new Exception("You can not move assigned box!");

          return box.UpdateRow("SET OrganizationId = @orgId WHERE BoxId = @boxId AND @orgId IN (SELECT OrganizationId FROM vlfOrganization)",
            new SqlParameter("@boxId", boxId), new SqlParameter("@orgId", newOrgId));
      }

      /// <summary>
      /// Check if current box is assigned to a vehicle
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>True if assigned, otherwise - false</returns>
      public bool IsAssigned(int boxId)
      {
         using (Vehicle veh = new Vehicle(this.ConnectionString))
         {
            return veh.IsActiveByBoxId(boxId);
         }
     }


        //Get BoxId based on BlackBerry PIN
        public int GetBoxId4BB(int PIN)
        {
            DAS.DB.Box box = new DAS.DB.Box(this.sqlExec);
            return box.GetBoxId4BB(PIN);
            
        }



        /// <summary>
        /// Add new box communication info.
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="dsCommInfo"></param>
        /// <exception cref="DASException">Thrown DASAppDataAlreadyExistsException if data already exists.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void AddBoxCommInfo(int boxId, DataSet dsCommInfo, bool useTransaction)
        {
            if (dsCommInfo == null || dsCommInfo.Tables.Count == 0) return;
            try
            {
                // 1. Begin transaction
                if (useTransaction)
                    sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

               //DB.BoxCommInfo boxCommInfo = new DB.BoxCommInfo(sqlExec);
                foreach (DataRow ittr in dsCommInfo.Tables[0].Rows)
                {
                    boxCommInfo.DeleteCommInfo(boxId, Convert.ToInt16(ittr["CommAddressTypeId"]));

                    boxCommInfo.AddCommInfo(boxId,
                                            Convert.ToInt16(ittr["CommAddressTypeId"]),
                                            ittr["CommAddressValue"].ToString().Trim());
                }
                // 7. Save all changes
                if (useTransaction)
                    sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 7. Rollback all changes
                if (useTransaction)
                    sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add communication information ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                if (useTransaction)
                    sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 7. Rollback all changes
                if (useTransaction)
                    sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }

        public bool CreateBoxAndVehicle(int organizationId, int boxId, short fwChId, long featureMask, short hwTypeId,
            DataSet dsProtocolMaxMessages, DataSet dsCommInfo, DataSet dsOutputs, DataSet dsSensors, DataSet dsMessages,
            VehicInfo vInfo, string licensePlate)
        {
            bool result = false;

            try
            {
                // 1. begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. add the box
                AddBox(boxId, fwChId, false, true, organizationId, featureMask, dsProtocolMaxMessages, false);
                // 3. add comm info - tran
                AddBoxCommInfo(boxId, dsCommInfo, false);
                // 4. Set outputs for the box - tran
                SetOutputs(boxId, hwTypeId, dsOutputs, false);
                // 5. Set defined sensors for the box - tran
                SetSensors(boxId, hwTypeId, dsSensors, false);
                // 6. Set message severity for the box - tran
                AddMsgSeverity(boxId, dsMessages, false);
                // 7. Vehicle assignment structure contains vehicleId, boxId, lic. plate - tran
                if (!String.IsNullOrEmpty(licensePlate) && !vInfo.Equals(null))
                {                    
                    DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
                    bool vehicleExist = false;
                    // 8. add new vehicle
                    long vehicleId = vehicleInfo.AddVehicleInfo(vInfo, organizationId, ref vehicleExist);
                    if (vehicleId > 1)
                    {
                        VehicAssign vehicleAssign = new VehicAssign();
                        vehicleAssign.vehicleId = vehicleId;
                        vehicleAssign.boxId = boxId;
                        vehicleAssign.licensePlate = licensePlate;

                        // 9. assign vehicle to box
                        DB.VehicleAssignment vehicleAssignment = new DB.VehicleAssignment(sqlExec);                        
                        vehicleAssignment.AddVehicleAssignment(vehicleAssign);

                        // 10. Add vehicle assignment into the history
                        VehicleAssignmentHst vehicleAssignmentHst = new VehicleAssignmentHst(sqlExec);                        
                        vehicleAssignmentHst.AddVehicleAssignment(vehicleAssign);

                        // 11. Add vehicle to default fleet
                        DB.Fleet fleet = new DB.Fleet(sqlExec);
                        int fleetId = fleet.GetFleetIdByFleetName(organizationId, VLF.CLS.Def.Const.defaultFleetName);
                        DB.FleetVehicles fleetVehicles = new DB.FleetVehicles(sqlExec);
                        if (!fleetVehicles.IsVehicleExistInFleet(fleetId, vehicleId))
                            fleetVehicles.AddVehicleToFleet(fleetId, vehicleId);
                    }
                    else
                        throw new Exception("Create new vehicle failed");
                }
                // 12. commit transaction
                sqlExec.CommitTransaction();
                result = true;
            }
            catch
            {
                sqlExec.RollbackTransaction();
                throw;
            }

            return result;
        }


        public int BoxExtraInfo_AddUpdate(int BoxId, DateTime Timestamp, Int16 MsgTypeId, string CustomProp,Int64 SensorMask)
        {
            return box.BoxExtraInfo_AddUpdate(BoxId, Timestamp, MsgTypeId, CustomProp, SensorMask);
        }


        public int BoxCmdHist_Add(int BoxId, int BoxCmdOutTypeId, DateTime DateTimeSent, string CustomProp, Int32 UserId)
        {
            return box.BoxCmdHist_Add(BoxId, BoxCmdOutTypeId, DateTimeSent, CustomProp, UserId);
        }

        public int BoxCmdHist_Update(int BoxId, int BoxCmdOutTypeId, DateTime DateTimeAck)
        {
            return box.BoxCmdHist_Update(BoxId, BoxCmdOutTypeId, DateTimeAck);
        }

        public DataSet GetLastCommunicatedDateTimeFromHistory(DateTime dDate, Int32 boxId)
        {
            return box.GetLastCommunicatedDateTimeFromHistory(dDate, boxId);
        }

     #region MdtOTA
              /// <summary>
        /// Add new MdtOTA process record
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="customProp"></param>
        public void AddMdtOTA(int boxId,Int16 typeId, string customProp)
        {
            MdtOTA mdt = new MdtOTA(sqlExec);
            mdt.AddMdtOTA(boxId,typeId, customProp);
        }
     #endregion

        #region Box Commands Sent and Recieved
        public DataSet GetCmdSend(int userId, int fleetId, string cmdType)
        {
            DB.BoxCmdHist boxCmdHist = new DB.BoxCmdHist(sqlExec);
            DataSet dsResult = boxCmdHist.GetCmdSend(userId, fleetId, cmdType);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "BoxCmdHistory";
                }
                dsResult.DataSetName = "CmdHistory";
            }
            return dsResult;
        }

        public DataSet GetCmdRec(int userId, int fleetId, int msgTypeId)
        {
            DB.BoxCmdHist boxCmdHist = new DB.BoxCmdHist(sqlExec);
            DataSet dsResult = boxCmdHist.GetCmdRec(userId, fleetId, msgTypeId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "BoxCmdHistory";
                }
                dsResult.DataSetName = "CmdHistory";
            }
            return dsResult;
        }
        #endregion
 }
}
