using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;			// Enums

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to maintenance functionality in database
	/// </summary>
	public class Maintenance : Das
	{
		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public Maintenance(string connectionString) : base (connectionString)
		{
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region Public Interfaces
		/// <summary>
		/// CleanUp all data from all tables.
		/// </summary>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public void CleanUpAllTables()
		{
			try
			{
				// 1. begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2. Delete tables
				DeleteAllRecords("vlfBoxMsgSeverity");
				DeleteAllRecords("vlfTxtMsgs");
				DeleteAllRecords("vlfTxtMsgType");
				DeleteAllRecords("vlfMsgInHstIgnored");
				DeleteAllRecords("vlfMsgOutHst");
				DeleteAllRecords("vlfMsgInHst");
				DeleteAllRecords("vlfGroupSecurity");
				DeleteAllRecords("vlfFleetVehicles");
				DeleteAllRecords("vlfFleetUsers");
				DeleteAllRecords("vlfFleetEmails");
				DeleteAllRecords("vlfFleet");
				DeleteAllRecords("vlfDriverAssignmentHst");
				DeleteAllRecords("vlfDriverAssignment");			
				DeleteAllRecords("vlfVehicleAssignmentHst");
				DeleteAllRecords("vlfVehicleAssignment");
				DeleteAllRecords("vlfBoxSensorsCfg");
				DeleteAllRecords("vlfBoxOutputsCfg");
				DeleteAllRecords("vlfBoxHwDefaultSensorsCfg");
				DeleteAllRecords("vlfBoxHwDefaultOutputsCfg");
				DeleteAllRecords("vlfVehicleInfo");
				DeleteAllRecords("vlfMakeModel");
				DeleteAllRecords("vlfModel");
				DeleteAllRecords("vlfMake");
				DeleteAllRecords("vlfMsgIn");
				DeleteAllRecords("vlfMsgOut");
				DeleteAllRecords("vlfImages");
				DeleteAllRecords("vlfVehicleType");
				DeleteAllRecords("vlfBoxCommInfo");
				DeleteAllRecords("vlfAlarm");
				DeleteAllRecords("vlfBox");
				DeleteAllRecords("vlfFirmware");
				DeleteAllRecords("vlfAsl");
				DeleteAllRecords("vlfDcl");
				DeleteAllRecords("vlfBoxProtocolTypeCmdOutType");
				DeleteAllRecords("vlfBoxProtocolTypeMsgInType");
				DeleteAllRecords("vlfBoxCmdOutType");
				DeleteAllRecords("vlfBoxMsgInType");
				DeleteAllRecords("vlfBoxProtocolType");
				DeleteAllRecords("vlfBoxHwType");
				DeleteAllRecords("vlfCommModeAddressType");
				DeleteAllRecords("vlfCommAddressType");
				DeleteAllRecords("vlfCommMode");
				DeleteAllRecords("vlfUserGroupAssignment");
				DeleteAllRecords("vlfUserPreference");
				DeleteAllRecords("vlfUserLogin");
				DeleteAllRecords("vlfUser");
				DeleteAllRecords("vlfUserGroup");
				DeleteAllRecords("vlfLandmark");
				DeleteAllRecords("vlfPreference");	
				DeleteAllRecords("vlfIconType");	
				DeleteAllRecords("vlfConfiguration");	
				DeleteAllRecords("vlfConfigurationGroups");	
				DeleteAllRecords("vlfConfigurationModuleType");	
				DeleteAllRecords("vlfPersonInfo");
				DeleteAllRecords("vlfStateProvince");
				DeleteAllRecords("vlfSensor");
				DeleteAllRecords("vlfOutput");
				DeleteAllRecords("vlfSeverity");
				DeleteAllRecords("vlfOrganization");
				
				// 3. commit transaction
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 3. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add box message severity ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception e)
			{
				// 3. rollback transaction
				sqlExec.RollbackTransaction();
				throw new DASException(e.Message);
			}
		}
		
		/// <summary>
		/// CleanUp all data from all tables.
		/// </summary>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public void CleanUpAllDynamicTables()
		{
			int prevCommandTimeout = sqlExec.CommandTimeout;
			try
			{
				// 1. begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2. Delete tables
				DeleteAllRecords("vlfTxtMsgs");
				DeleteAllRecords("vlfMsgInHstIgnored");
				DeleteAllRecords("vlfMsgOutHst");
				DeleteAllRecords("vlfMsgInHst");
				//DeleteAllRecords("vlfGroupSecurity");
				DeleteAllRecords("vlfFleetVehicles");
				DeleteAllRecords("vlfFleetUsers");
				DeleteAllRecords("vlfFleetEmails");
				DeleteAllRecords("vlfFleet");
				DeleteAllRecords("vlfDriverAssignmentHst");
				DeleteAllRecords("vlfDriverAssignment");			
				DeleteAllRecords("vlfVehicleAssignmentHst");
				DeleteAllRecords("vlfVehicleAssignment");
				DeleteAllRecords("vlfBoxSensorsCfg");
				DeleteAllRecords("vlfBoxOutputsCfg");
				//DeleteAllRecords("vlfBoxHwDefaultSensorsCfg");
				//DeleteAllRecords("vlfBoxHwDefaultOutputsCfg");
				DeleteAllRecords("vlfVehicleInfo");
				//DeleteAllRecords("vlfMakeModel");
				//DeleteAllRecords("vlfModel");
				//DeleteAllRecords("vlfMake");
				DeleteAllRecords("vlfMsgIn");
				DeleteAllRecords("vlfMsgOut");
				DeleteAllRecords("vlfImages");
				//DeleteAllRecords("vlfVehicleType");
				DeleteAllRecords("vlfBoxCommInfo");
				DeleteAllRecords("vlfAlarm");
				DeleteAllRecords("vlfBox");
				DeleteAllRecords("vlfFirmware");
				DeleteAllRecords("vlfAsl");
				DeleteAllRecords("vlfDcl");
				//DeleteAllRecords("vlfBoxProtocolTypeCmdOutType");
				//DeleteAllRecords("vlfBoxProtocolTypeMsgInType");
				//DeleteAllRecords("vlfBoxCmdOutType");
				//DeleteAllRecords("vlfBoxMsgInType");
				//DeleteAllRecords("vlfBoxProtocolType");
				//DeleteAllRecords("vlfBoxHwType");
				//DeleteAllRecords("vlfCommModeAddressType");
				//DeleteAllRecords("vlfCommAddressType");
				//DeleteAllRecords("vlfCommMode");
				DeleteAllRecords("vlfUserGroupAssignment");
				DeleteAllRecords("vlfUserPreference");
				DeleteAllRecords("vlfUserLogin");
				DeleteAllRecords("vlfUser");
				//DeleteAllRecords("vlfUserGroup");
				DeleteAllRecords("vlfLandmark");
				DeleteAllRecords("vlfOrganization");
				//DeleteAllRecords("vlfPreference");		
				DeleteAllRecords("vlfPersonInfo");
				//DeleteAllRecords("vlfStateProvince");
				//DeleteAllRecords("vlfSensor");
				//DeleteAllRecords("vlfOutput");
				//DeleteAllRecords("vlfSeverity");

				// 3. commit transaction
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 3. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add box message severity ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception e)
			{
				// 3. rollback transaction
				sqlExec.RollbackTransaction();
				sqlExec.CommandTimeout = prevCommandTimeout;
				throw new DASException(e.Message);
			}
			sqlExec.CommandTimeout = prevCommandTimeout;
		}
		
		/// <summary>
		/// Fill all static tables with data from enums.
		/// Throws DASException exception in case of wrong result.
		/// </summary>
		public void InitStaticTables()
		{
			try
			{
				// 1. begin transaction
				//sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				
				// 2. Fill static tables
				try{FillTxtMsgTypes();}catch{}
				try{FillBoxMsgTypes();}catch{}
				try{FillBoxCommandTypes();}catch{}
				try{FillBoxProtocolTypes();}catch{}
				try{FillCommModes();}catch{}
				try{FillCommAddressType();}catch{}
				try{FillUserGroup();}catch{}
				try{FillPreferences();}catch{}

				// 3. commit transaction
				//sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 3. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add box message severity ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception e)
			{
				// 3. rollback transaction
				//sqlExec.RollbackTransaction();
				throw new DASException(e.Message);
			}
		}
		
		/// <summary>
		/// Fills vlfBoxProtocolTypeCmdOutType table with protocol specific commands.
		/// </summary>
		/// <param name="boxProtocolType"></param>
		/// <param name="cmdOutTypes"></param>
		/// <param name="commAddressType"></param>
		public void FillBoxProtocolTypeCmdOutType(short boxProtocolType, Enums.CommandType[] cmdOutTypes,short[] commAddressType)
		{
			if(!IsFieldValueExist("vlfBoxProtocolTypeCmdOutType","BoxProtocolTypeId",boxProtocolType))
			{
				short commAddressTypeVal = VLF.CLS.Def.Const.unassignedIntValue;
				short cmdOutTypeLen = 0;
				for(int index=0;index<cmdOutTypes.Length;++index)
				{
					if((commAddressType != null) && (index < commAddressType.Length))
						commAddressTypeVal = commAddressType[index];
					//AddNewRow("vlfBoxProtocolTypeCmdOutType","BoxCmdOutTypeId",Convert.ToInt16(cmdOutTypes[index]),"BoxProtocolTypeId",boxProtocolType,"Rules"," ","box protocol type command out type");
					DB.BoxProtocolTypeCmdOutType protocolTypeCmdOutType = new DB.BoxProtocolTypeCmdOutType(sqlExec);
					cmdOutTypeLen = GetCmdTypeLenByProtocolType(boxProtocolType,(Enums.CommandType)Convert.ToInt16(cmdOutTypes[index]));
					protocolTypeCmdOutType.AddRecord(Convert.ToInt16(cmdOutTypes[index]),boxProtocolType,"",commAddressTypeVal,cmdOutTypeLen);
				}
			}
		}
		/// <summary>
		/// Fills vlfBoxProtocolTypeMsgInType table with protocol specific messages.
		/// </summary>
		/// <param name="boxProtocolType"></param>
		/// <param name="msgInTypes"></param>
		public void FillBoxProtocolTypeMsgInType(short boxProtocolType, Enums.MessageType[] msgInTypes)
		{
			if(!IsFieldValueExist("vlfBoxProtocolTypeMsgInType","BoxProtocolTypeId",boxProtocolType))
			{
				short msgTypeLen = 0;
				for(int index=0;index<msgInTypes.Length;++index)
				{
					DB.BoxProtocolTypeMsgInType protocolTypeMsgInType = new DB.BoxProtocolTypeMsgInType(sqlExec);
					msgTypeLen = GetMsgTypeLenByProtocolType(boxProtocolType,(Enums.MessageType)Convert.ToInt16(msgInTypes[index]));
					protocolTypeMsgInType.AddRecord(Convert.ToInt16(msgInTypes[index]),boxProtocolType,"",msgTypeLen);
				}
			}
		}
		/// <summary>
		/// CleanUp all dynamic data tables.
		/// </summary>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public void CleanUpDynamicTbls()
		{
			try
			{
				// 1. begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2. Delete tables
				DeleteAllRecords("vlfMsgIn");
				DeleteAllRecords("vlfMsgOut");
				//				DeleteAllRecords("vlfAsl");
				DeleteAllRecords("vlfDcl");

				// 3. commit transaction
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 3. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add box message severity ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception e)
			{
				// 3. rollback transaction
				sqlExec.RollbackTransaction();
				throw new DASException(e.Message);
			}
		}
		
		#endregion

		#region Protected Interfaces
		/// <summary>
		/// Delete all rows.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		protected int DeleteAllRecords(string tableName)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to delete information from '" + tableName + "' table.";
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName;
			try
			{
				// 2. Attach SQL to transaction
				sqlExec.AttachToTransaction(sql);
				// 3. Executes SQL statement
				//rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Fills vlfBoxMsgInType table with message id and name.
		/// </summary>
		protected void FillBoxMsgTypes()
		{
			DB.BoxMsgInType msgInType = new DB.BoxMsgInType(sqlExec);
			string boxMsgInTypeName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.MessageType));
			foreach(Enums.MessageType ittr in enmArr)
			{
				boxMsgInTypeName = Enum.GetName(typeof(Enums.MessageType),ittr);
				msgInType.AddRecord(Convert.ToInt16(ittr),boxMsgInTypeName);
			}
		}
		/// <summary>
		/// Fills vlfTxtMsgType table with message id and name.
		/// </summary>
		protected void FillTxtMsgTypes()
		{
			DB.TxtMsgType txtMsgType = new DB.TxtMsgType(sqlExec);
			string txtMsgTypeName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.TxtMsgType));
			foreach(Enums.TxtMsgType ittr in enmArr)
			{
				txtMsgType.AddRecord(Convert.ToInt16(ittr),txtMsgTypeName,txtMsgTypeName);
			}
		}
		/// <summary>
		/// Fills vlfCommMode table with communication mode id and name.
		/// </summary>
		protected void FillCommModes()
		{
			string boxCommModeName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.CommMode));
			foreach(Enums.CommMode ittr in enmArr)
			{
				boxCommModeName = Enum.GetName(typeof(Enums.CommMode),ittr);
				AddNewRow("vlfCommMode","CommModeId",Convert.ToInt16(ittr),"CommModeName",boxCommModeName,"communication mode.");
			}
		}
		/// <summary>
		/// Fills vlfCommAddressType table with communication address type id and name.
		/// </summary>
		protected void FillCommAddressType()
		{
			string boxCommAddressTypeName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.CommAddressType));
			foreach(Enums.CommAddressType ittr in enmArr)
			{
				boxCommAddressTypeName = Enum.GetName(typeof(Enums.CommAddressType),ittr);
				AddNewRow("vlfCommAddressType","CommAddressTypeId",Convert.ToInt16(ittr),"CommAddressTypeName",boxCommAddressTypeName,"communication address type.");
			}
		}

		/// <summary>
		/// Fills vlfBoxCmdOutType table with message id and name.
		/// </summary>
		protected void FillBoxCommandTypes()
		{
			string boxCmdOutTypeName = null;
			DB.BoxCmdOutType cmdOutType = new DB.BoxCmdOutType(sqlExec);
			Array enmArr = Enum.GetValues(typeof(Enums.CommandType));
			foreach(Enums.CommandType ittr in enmArr)
			{
				boxCmdOutTypeName = Enum.GetName(typeof(Enums.CommandType),ittr);
				cmdOutType.AddRecord(Convert.ToInt16(ittr),boxCmdOutTypeName);
			}
		}
		
		/// <summary>
		/// Fills vlfBoxProtocolType table with protocol id and name.
		/// </summary>
		protected void FillBoxProtocolTypes()
		{
			string boxProtocolTypeName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.ProtocolTypes));
			foreach(Enums.ProtocolTypes ittr in enmArr)
			{
				boxProtocolTypeName = Enum.GetName(typeof(Enums.ProtocolTypes),ittr);
				AddNewRow("vlfBoxProtocolType","BoxProtocolTypeId",Convert.ToInt16(ittr),"BoxProtocolTypeName",boxProtocolTypeName,"box protocol type.");
			}
		}

		/// <summary>
		/// Fills vlfUserGroup table with user group id and name.
		/// </summary>
		protected void FillUserGroup()
		{
			string userGroupName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.UserGroup));
			foreach(Enums.UserGroup ittr in enmArr)
			{
				userGroupName = Enum.GetName(typeof(Enums.UserGroup),ittr);
				AddNewRow("vlfUserGroup","UserGroupId",Convert.ToInt16(ittr),"UserGroupName",userGroupName,"user group");
			}
		}

		/// <summary>
		/// Fills vlfPreference table with preference id and name.
		/// </summary>
		protected void FillPreferences()
		{
			string preferenceName = null;
			Array enmArr = Enum.GetValues(typeof(Enums.Preference));
			foreach(Enums.Preference ittr in enmArr)
			{
				preferenceName = Enum.GetName(typeof(Enums.Preference),ittr);
				AddNewRow("vlfPreference","PreferenceId",Convert.ToInt16(ittr),"PreferenceName",preferenceName,"preference");
			}
		}
		/// <summary>
		/// Add new row.
		/// Throws exception in case of wrong result.
		///		- DASException
		///		- DASAppDataAlreadyExistsException
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="rowFieldName1"></param>
		/// <param name="rowFieldValue1"></param>
		/// <param name="rowFieldName2"></param>
		/// <param name="rowFieldValue2"></param>
		/// <param name="rowFieldName3"></param>
		/// <param name="rowFieldValue3"></param>
		/// <param name="msgPostfix"></param>
		protected void AddNewRow(string tableName,string rowFieldName1,short rowFieldValue1,string rowFieldName2,short rowFieldValue2,string rowFieldName3,string rowFieldValue3,string msgPostfix)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to add new '" + rowFieldValue1 + ":" + rowFieldValue1 + "' " + msgPostfix + ".";
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( " + rowFieldName1 + 
				", " + rowFieldName2 + ", " + rowFieldName3 + 
				") VALUES ( {0}, {1} , '{2}')", 
				rowFieldValue1, rowFieldValue2, rowFieldValue3);
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The " + msgPostfix + " already exists.");
			}		
		}
		/// <summary>
		/// Add new row.
		/// Throws exception in case of wrong result.
		///		- DASException
		///		- DASAppDataAlreadyExistsException
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="rowFieldName1"></param>
		/// <param name="rowFieldValue1"></param>
		/// <param name="rowFieldName2"></param>
		/// <param name="rowFieldValue2"></param>
		/// <param name="msgPostfix"></param>
		protected void AddNewRow(string tableName,string rowFieldName1,short rowFieldValue1,string rowFieldName2,string rowFieldValue2,string msgPostfix)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to add new '" + rowFieldValue1 + ":" + rowFieldValue2 + "' " + msgPostfix + ".";
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + " ( " + rowFieldName1 + 
				", " + rowFieldName2 + ") VALUES ( {0}, '{1}')", 
				rowFieldValue1, rowFieldValue2);
			try
			{
				// 2. Attach SQL to transaction
				sqlExec.AttachToTransaction(sql);

				// 3. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				// 6. Rollback all changes
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The " + msgPostfix + " already exists.");
			}		
		}

		/// <summary>
		/// Returns true if field with specific value exists in table.
		/// Throws exception in case of wrong result.
		///		- DASException
		///		- DASAppDataAlreadyExistsException
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		protected bool IsFieldValueExist(string tableName,string fieldName,short fieldValue)
		{
			bool IsExist = false;
			string prefixMsg = "Unable to find '" + fieldName + "':" + fieldValue + " in the '" + tableName + "' table.";
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "SELECT COUNT(*) FROM " + tableName +
				" WHERE (" + fieldName + "=" + fieldValue + ")";

			try
			{
				// 2. Executes SQL statement
				rowsAffected = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected > 0) 
			{
				IsExist = true;
			}	
			return IsExist;
		}

		/// <summary>
		/// Retrieves command len by protocol type
		/// </summary>
		/// <param name="protocolType"></param>
		/// <param name="cmdType"></param>
		protected short GetCmdTypeLenByProtocolType(short protocolType,Enums.CommandType cmdType)
		{
			short cmdLen = 0;
			switch(cmdType)
			{
				case Enums.CommandType.Ack:
					#region Ack size
					if(	(short)Enums.ProtocolTypes.HGIv20 == protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30 == protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.MBv10 == protocolType)
						cmdLen = 1;
					#endregion
					break;
				case Enums.CommandType.Arm:
					#region Arm size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.BoxReset:
					#region BoxReset size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.ClearFences:
					#region ClearFences size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(7 + VLF.CLS.Def.Const.udpHeaderLen);;
					#endregion
					break;
				case Enums.CommandType.ClearMemory:
					#region ClearMemory size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.Disarm:
					#region Disarm size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.VoiceMessage:
					#region VoiceMessage size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(100 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.SecurityCode:
					#region SecurityCode size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(15 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.GetBoxSetup:
					#region GetBoxSetup size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.GetBoxStatus:
					#region GetBoxStatus size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.GetWaypoints:
					#region GetWaypoints size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.IdnAck:
					#region IdnAck size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.Output:
					#region Output size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(7 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(8 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.SetFence:
					#region SetFence size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(41 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(42 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.Setup:
					#region Setup size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(53 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(65 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.UpdatePosition:
					#region UpdatePosition size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType ||
						(short)Enums.ProtocolTypes.HGIv10 == protocolType)
						cmdLen = Convert.ToInt16(5 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.Trace:
					#region Trace size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(6 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.MDTAck:
					#region MDTAck size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(296 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.MDTTextMessage:
					#region MDTTextMessage size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType)
						cmdLen = Convert.ToInt16(296 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.AddGeoZone:
					#region AddGeoZone size
					if(	(short)Enums.ProtocolTypes.HGIv40==protocolType)
						cmdLen = Convert.ToInt16(41 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.CommandType.DeleteGeoZone:
					#region DeleteGeoZone size
					if(	(short)Enums.ProtocolTypes.HGIv40==protocolType)
						cmdLen = Convert.ToInt16(41 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				default:
					cmdLen = VLF.CLS.Def.Const.defaultCmdOutTypeLen;
					break;
			}
			return cmdLen;
		}
		
		/// <summary>
		/// Retrieves msg len by protocol type
		/// </summary>
		/// <param name="protocolType"></param>
		/// <param name="msgType"></param>
		protected short GetMsgTypeLenByProtocolType(short protocolType,Enums.MessageType msgType)
		{
			short msgLen = 0;
			switch(msgType)
			{
				case Enums.MessageType.Ack:
					#region Ack size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(10 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(11 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.Alarm:
					#region Alarm size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.BoxSetup:
					#region BoxSetup size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(70 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.BoxStatus:
					#region BoxStatus size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(50 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.Coordinate:
					#region Coordinate size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.GeoFence:
					#region Fence size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.Identification:
					#region Identification size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(10 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(11 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.IPUpdate:
					#region IPUpdate size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(34 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(35 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.KeyFobArm:
					#region KeyFobArm size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.KeyFobDisarm:
					#region KeyFobDisarm size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.KeyFobPanic:
					#region KeyFobPanic size
					if(	(short)Enums.ProtocolTypes.HGIv30==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.MBAlarm:
					#region MBAlarm size
					if((short)Enums.ProtocolTypes.MBv10==protocolType)
						msgLen = Convert.ToInt16(1 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.PositionUpdate:
					#region PositionUpdate size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.Sensor:
            case Enums.MessageType.SensorExtended:
					#region Sensor size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.Speed:
					#region Speed size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.WaypointDownloadEnd:
					#region WaypointDownloadEnd size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(10 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.StoredPosition:
					#region StoredPosition size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType || 
						(short)Enums.ProtocolTypes.HGIv10==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					else if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(29 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.MDTAck:
					#region MDTAck size
					if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(46 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.MDTResponse:
					#region MDTResponse size
					if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(296 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.MDTSpecialMessage:
					#region MDTSpecialMessage size
					if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(296 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.MDTTextMessage:
					#region MDTTextMessage size
					if((short)Enums.ProtocolTypes.HGIv30==protocolType)
						msgLen = Convert.ToInt16(296 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.Idling:
            case Enums.MessageType.ExtendedIdling:
					#region Idling size
					if(	(short)Enums.ProtocolTypes.HGIv20==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				case Enums.MessageType.GeoZone:
					#region GeoZone size
					if(	(short)Enums.ProtocolTypes.HGIv40==protocolType)
						msgLen = Convert.ToInt16(24 + VLF.CLS.Def.Const.udpHeaderLen);
					#endregion
					break;
				default:
					msgLen = VLF.CLS.Def.Const.defaultMsgInTypeLen;
					break;
			}
			return msgLen;
		}
		#endregion
	}
}
