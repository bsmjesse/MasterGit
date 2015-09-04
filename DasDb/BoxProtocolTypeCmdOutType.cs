using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxProtocolTypeCmdOutType table.
	/// </summary>
	public class BoxProtocolTypeCmdOutType : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxProtocolTypeCmdOutType(SQLExecuter sqlExec) : base ("vlfBoxProtocolTypeCmdOutType",sqlExec)
		{
		}
		/// <summary>
		/// Add new row that connect box protocol type with cmd out type
		/// </summary>
		/// <remarks>
		/// If commAddressTypeId not applicable set VLF.CLS.Def.Const.unassignedIntValue
		/// </remarks>
		/// <param name="boxCmdOutTypeId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="rules"></param>
		/// <param name="commAddressTypeId"></param>
		/// <param name="cmdOutTypeLen"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddRecord(short boxCmdOutTypeId,short boxProtocolTypeId,string rules,short commAddressTypeId,short cmdOutTypeLen)
		{
			int rowsAffected = 0;
			try
			{
				if(! Enum.IsDefined(typeof(VLF.CLS.Def.Enums.CommAddressType),Convert.ToInt32(commAddressTypeId)))
				{
					commAddressTypeId = VLF.CLS.Def.Const.unassignedIntValue;
				}
			}
			catch
			{
				commAddressTypeId = VLF.CLS.Def.Const.unassignedIntValue;
			}
			//Prepares SQL statement
			string sql = "INSERT INTO vlfBoxProtocolTypeCmdOutType (BoxCmdOutTypeId,BoxProtocolTypeId,CommAddressTypeId,Rules,CmdOutTypeLen)"+
						" VALUES ( " + boxCmdOutTypeId + ", " + boxProtocolTypeId + ", " + commAddressTypeId + ", '" + rules.Replace("'","''") + "'," +  cmdOutTypeLen + ")";
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new boxCmdOutTypeId=" + boxCmdOutTypeId + " boxProtocolTypeId=" + boxProtocolTypeId + " commAddressTypeId=" + commAddressTypeId + " rules=" + rules + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new boxCmdOutTypeId=" + boxCmdOutTypeId + " boxProtocolTypeId=" + boxProtocolTypeId + " commAddressTypeId=" + commAddressTypeId + " rules=" + rules + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new boxCmdOutTypeId=" + boxCmdOutTypeId + " boxProtocolTypeId=" + boxProtocolTypeId + " commAddressTypeId=" + commAddressTypeId + " rules=" + rules + ".";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The protocol type with cmd out type for current box already exists.");
			}
		}
		/// <summary>
		/// Delete exist box command out type by Id
		/// </summary>
		/// <param name="boxCmdOutTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxCmdOutType(short boxCmdOutTypeId)
		{
			return DeleteRowsByIntField("BoxCmdOutTypeId",boxCmdOutTypeId, "box command out type");
		}
		/// <summary>
		/// Delete exist box protocol type by Id
		/// </summary>
		/// <param name="boxProtocolTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxProtocolType(short boxProtocolTypeId)
		{
			return DeleteRowsByIntField("BoxProtocolTypeId",boxProtocolTypeId, "box protocol type");
		}
		/// <summary>
		/// Delete exist box protocol type command out type.
		/// </summary>
		/// <param name="boxCmdOutTypeId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxProtocolTypeCmdOutType(short boxCmdOutTypeId,short boxProtocolTypeId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + 
						" WHERE BoxCmdOutTypeId=" + boxCmdOutTypeId +
						" AND BoxProtocolTypeId=" + boxProtocolTypeId;
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
				string prefixMsg = "Unable to delete BoxProtocolType CmdOutType by boxCmdOutTypeId=" + boxCmdOutTypeId + " BoxProtocolTypeId=" + boxProtocolTypeId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete BoxProtocolType CmdOutType by boxCmdOutTypeId=" + boxCmdOutTypeId + " BoxProtocolTypeId=" + boxProtocolTypeId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves record count from vlfBoxProtocolTypeCmdOutType table.
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount();
			}
		}
		/// <summary>
		/// Retrieves CommAddressTypeId by ProtocolType and CmdOutTypeId
		/// </summary>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="boxCmdOutTypeId"></param>
		/// <returns>CommAddressTypeId</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short GetCommAddressTypeIdByProtocolTypeCmdOutTypeId(short boxProtocolTypeId,short boxCmdOutTypeId)
		{
			short commAddressTypeId = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT CommAddressTypeId"+ 
					" FROM vlfBoxProtocolTypeCmdOutType" +
					" WHERE BoxProtocolTypeId=" + boxProtocolTypeId +
					" AND BoxCmdOutTypeId=" + boxCmdOutTypeId;
				
				//Executes SQL statement
				commAddressTypeId = Convert.ToInt16(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve commAddressType by  protocol type=" + boxProtocolTypeId + " and boxCmdOutTypeId=" + boxCmdOutTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve commAddressType by  protocol type=" + boxProtocolTypeId + " and boxCmdOutTypeId=" + boxCmdOutTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return commAddressTypeId;
		}	
		/// <summary>
		/// Retrieves all supported commands by protocol type and user id.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <returns>DataSet [BoxCmdOutTypeId],[BoxCmdOutTypeName],[Rules],[BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllSupportedCommands(int boxId,int userId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT vlfBoxCmdOutType.BoxCmdOutTypeId, vlfBoxCmdOutType.BoxCmdOutTypeName, vlfBoxProtocolTypeCmdOutType.Rules, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfFirmwareChannels.ChPriority FROM vlfBoxProtocolTypeCmdOutType INNER JOIN vlfUserGroupAssignment INNER JOIN vlfGroupSecurity ON vlfUserGroupAssignment.UserGroupId = vlfGroupSecurity.UserGroupId ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfGroupSecurity.OperationId INNER JOIN vlfBoxCmdOutType ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId INNER JOIN vlfBoxProtocolType ON vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN  vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId ON vlfBoxProtocolType.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId WHERE vlfBox.BoxId = " + boxId + " AND vlfUserGroupAssignment.UserId =" + userId + " AND vlfGroupSecurity.OperationType = " + (int)VLF.CLS.Def.Enums.OperationType.Command + " AND vlfBoxProtocolTypeCmdOutType.Visible = 1 ORDER BY vlfBoxCmdOutType.BoxCmdOutTypeName, vlfFirmwareChannels.ChPriority";
			
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve commands by boxId=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve commands by boxId=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        public DataSet GetAllDistinctSupportedCommands(string boxid, int userId)
        {
            DataSet sqlDataSet = null;
			try
			{
				sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@boxid", SqlDbType.VarChar, boxid);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);


                sqlDataSet = sqlExec.SPExecuteDataset("GetAllDistinctSupportedCommands");
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve commands by boxIds=" + boxid + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve commands by boxIds=" + boxid + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

		/// <summary>
		/// Retrieves all supported protocol types for current command
		/// </summary>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <param name="commandTypeId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetCommandProtocolTypesInfo(int boxId,int userId,short commandTypeId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfFirmwareChannels.ChPriority, dbo.vlfChannels.CommModeId FROM vlfBoxProtocolTypeCmdOutType INNER JOIN vlfUserGroupAssignment INNER JOIN vlfGroupSecurity ON vlfUserGroupAssignment.UserGroupId = vlfGroupSecurity.UserGroupId ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfGroupSecurity.OperationId INNER JOIN vlfBoxCmdOutType ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId INNER JOIN vlfBoxProtocolType ON vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId ON vlfBoxProtocolType.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId WHERE vlfBox.BoxId =" + boxId + " AND vlfUserGroupAssignment.UserId = " + userId + " AND vlfGroupSecurity.OperationType = " + (int)VLF.CLS.Def.Enums.OperationType.Command + " AND vlfBoxCmdOutType.BoxCmdOutTypeId =" + commandTypeId + " ORDER BY vlfBoxProtocolType.BoxProtocolTypeName, vlfFirmwareChannels.ChPriority";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve commands by boxId=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve commands by boxId=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}	
		/// <summary>
		/// Retrieves all supported protocol types for current output
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="userId"></param>
		/// <param name="outputId"></param>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[ChPriority]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetOutputProtocolTypesInfo(int boxId,int userId,short outputId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, vlfFirmwareChannels.ChPriority FROM vlfBoxProtocolTypeCmdOutType INNER JOIN vlfBoxProtocolType ON vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId INNER JOIN vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId ON vlfBoxProtocolType.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId INNER JOIN  vlfBoxOutputsCfg ON vlfBox.BoxId = vlfBoxOutputsCfg.BoxId INNER JOIN vlfGroupSecurity ON vlfBoxOutputsCfg.OutputId = vlfGroupSecurity.OperationId INNER JOIN vlfUserGroupAssignment ON vlfGroupSecurity.UserGroupId = vlfUserGroupAssignment.UserGroupId WHERE vlfBox.BoxId=" + boxId + " AND vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = " + (short)VLF.CLS.Def.Enums.CommandType.Output + " AND vlfGroupSecurity.OperationId = " + outputId + " AND vlfGroupSecurity.OperationType=" + (short)VLF.CLS.Def.Enums.OperationType.Output + " AND vlfUserGroupAssignment.UserId = " + userId + " ORDER BY vlfFirmwareChannels.ChPriority";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve commands by boxId=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve commands by boxId=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
	}
}
