using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using System.Collections;	// for SortedList
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfDcl table.
	/// </summary>
	public class Dcl: TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Dcl(SQLExecuter sqlExec) : base ("vlfDcl",sqlExec)
		{
		}
		/// <summary>
		/// Add new DCL type.
		/// </summary>
		/// <param name="commModeId"></param>
		/// <param name="boxProtocolGroupId"></param>
		/// <param name="dclName"></param>
		/// <param name="description"></param>
		/// <param name="serviceState"></param>
		/// <returns>new dcl id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short AddDcl(VLF.CLS.Def.Enums.CommMode commModeId,short boxProtocolGroupId,string dclName,string description,VLF.CLS.Def.Enums.ServiceState serviceState)
		{
			int rowsAffected = 0;
			short nextDclId = MaxRecordIndex;
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName + 
				" (DclId,CommModeId,BoxProtocolGroupId,DclName,Description,ServiceState) VALUES ({0},{1},{2},'{3}','{4}',{5})", 
				++nextDclId,Convert.ToInt16(commModeId),boxProtocolGroupId,dclName.Replace("'","''"),description.Replace("'","''"),Convert.ToInt16(serviceState));
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new DCL by DclName=" + dclName + "' and ServiceState=" + serviceState.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new DCL by DclName=" + dclName + "' and ServiceState=" + serviceState.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return nextDclId;
		}	
		/// <summary>
		/// Delete exist DCL type.
		/// </summary>
		/// <param name="dclId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteDcl(short dclId)
		{
			return DeleteRowsByIntField("DclId",dclId, "DCL type");
		}
		/// <summary>
		/// Delete exist DCL type.
		/// </summary>
		/// <param name="dclName"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteDcl(string dclName)
		{
			return DeleteRowsByStrField("DclName",dclName, "DCL type");
		}
		/// <summary>
		/// Update DCL type info.
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="commModeId"></param>
		/// <param name="boxProtocolGroupId"></param>
		/// <param name="description"></param>
		/// <param name="serviceState"></param>
		/// <param name="pid"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateDclInfo(short dclId,VLF.CLS.Def.Enums.CommMode commModeId,short boxProtocolGroupId,string description,VLF.CLS.Def.Enums.ServiceState serviceState,short pid)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET CommModeId=" + Convert.ToInt16(commModeId) + 
				",BoxProtocolGroupId=" + boxProtocolGroupId +
				",Description='"+ description.Replace("'","''") + "'" +
				",ServiceState=" + Convert.ToInt16(serviceState) +
				",PID=" + pid +
				" WHERE DclId=" + dclId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new DCL type='" + dclId + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new DCL type='" + dclId + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new DCL type='" + dclId + "'.";
				throw new DASAppResultNotFoundException(prefixMsg + " The DCL type does not exist.");
			}
		}	
		/// <summary>
		/// Retrieves record count from "vlfDcl" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("DclId");
			}
		}
		/// <summary>
		/// Retrieves max record index from "vlfDcl" table
		/// </summary>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short MaxRecordIndex
		{
			get
			{
				return Convert.ToInt16(GetMaxRecordIndex("DclId"));
			}
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="dclId"></param>
      /// <returns></returns>
      public short GetDclCommMode(short dclId)
      {
         short ret = -1;
         try
         {
            //Prepares SQL statement
            string sql = "SELECT CommModeId from vlfDcl where DclId = " + dclId.ToString() ; 
            //Executes SQL statement
            ret = (short)sqlExec.SQLExecuteScalar(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to retrieve CommMode for DCLId=" + dclId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to retrieve CommMode for DCLId=" + dclId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return (-1);
      }
		/// <summary>
		/// Retrieves DCL info
		/// </summary>
		/// <param name="dclId"></param>
		/// <returns>DataSet [DclId],[CommModeId],[BoxProtocolGroupId ],[DclName],[Description],
		/// [ServiceState],[PID],[BoxProtocolTypeId],[BoxProtocolTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetDclInfo(short dclId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfDcl.*, vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName FROM vlfBoxProtocolType INNER JOIN vlfBoxlProtocolGroupAssignment ON vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId INNER JOIN vlfBoxProtocolGroup ON vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId = vlfBoxProtocolGroup.BoxProtocolGroupId INNER JOIN vlfDcl ON vlfBoxProtocolGroup.BoxProtocolGroupId = vlfDcl.BoxProtocolGroupId WHERE DclId=" + dclId;
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve DCL Id=" + dclId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve DCL Id=" + dclId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

		/// <summary>
		/// Retrieves Protocol types info by protocol group
		/// </summary>
		/// <param name="boxProtocolGroupId"></param>
		/// <remarks>
		/// If Assembly is null, returns empty string ("")
		/// If ClassName is null, returns empty string ("")
		/// </remarks>
		/// <returns>DataSet [BoxProtocolTypeId],[BoxProtocolTypeName],[Assembly],[ClassName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetProtocolTypesInfoByProtocolGroup(short boxProtocolGroupId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfBoxProtocolType.BoxProtocolTypeId, vlfBoxProtocolType.BoxProtocolTypeName, ISNULL(vlfBoxProtocolType.Assembly, '') AS Assembly, ISNULL(vlfBoxProtocolType.ClassName, '') AS ClassName FROM vlfBoxProtocolType INNER JOIN vlfBoxlProtocolGroupAssignment ON vlfBoxProtocolType.BoxProtocolTypeId = vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId WHERE vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId=" + boxProtocolGroupId + " ORDER BY vlfBoxProtocolType.BoxProtocolTypeId";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve protocols info by protocol group=" + boxProtocolGroupId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve protocols info by protocol group=" + boxProtocolGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
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
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT vlfBoxCommInfo.BoxId, vlfChannels.BoxProtocolTypeId FROM vlfBoxCommInfo INNER JOIN vlfBox with (nolock) ON vlfBoxCommInfo.BoxId = vlfBox.BoxId INNER JOIN vlfCommModeAddressType ON vlfBoxCommInfo.CommAddressTypeId = vlfCommModeAddressType.CommAddressTypeId INNER JOIN vlfCommMode ON vlfCommModeAddressType.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfChannels ON vlfCommMode.CommModeId = vlfChannels.CommModeId INNER JOIN vlfFirmwareChannelReference ON vlfBox.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfFirmwareChannels ON vlfChannels.ChId = vlfFirmwareChannels.ChId AND vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId WHERE vlfBoxCommInfo.CommAddressValue = '" + commAddressValue + "'";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve protocol info by commAddressValue=" + commAddressValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve protocol info by commAddressValue=" + commAddressValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

		/// <summary>
		/// Retrieves DCL id by box Id, protocol type and communication mode
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <remarks>
		/// If dcl with current configuration does not exist, returns VLF.CLS.Def.Const.unassignedIntValue
		/// </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>dcl id</returns>
		public short GetDclId(int boxId,VLF.CLS.Def.Enums.ProtocolTypes boxProtocolTypeId,VLF.CLS.Def.Enums.CommMode commModeId)
		{
			short retResult  = VLF.CLS.Def.Const.unassignedIntValue;
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT vlfDcl.DclId FROM vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfBoxlProtocolGroupAssignment INNER JOIN vlfDcl ON vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId = vlfDcl.BoxProtocolGroupId INNER JOIN vlfChannels ON vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId AND vlfDcl.CommModeId = vlfChannels.CommModeId ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfBox.BoxId = " + boxId + " AND vlfChannels.CommModeId =" + Convert.ToInt16(commModeId) + " AND vlfChannels.BoxProtocolTypeId =" + Convert.ToInt16(boxProtocolTypeId);
		
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve DCL id by box Id " + boxId + " ,protocol type " + boxProtocolTypeId + " and communication mode " + commModeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve DCL id by box Id " + boxId + " ,protocol type " + boxProtocolTypeId + " and communication mode " + commModeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if(sqlDataSet != null && sqlDataSet.Tables.Count > 0 && sqlDataSet.Tables[0].Rows.Count > 0)
			{
				retResult = Convert.ToInt16(sqlDataSet.Tables[0].Rows[0][0]);
			}
			return retResult;
		}

		/// <summary>
		/// Retrieves DCL id by protocol type and communication mode
		/// </summary>
		/// <param name="boxProtocolTypeId"></param>
		/// <param name="commModeId"></param>
		/// <remarks>
		/// If dcl with current configuration does not exist, returns VLF.CLS.Def.Const.unassignedIntValue
		/// </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>dcl id</returns>
		public short GetOTADclId(VLF.CLS.Def.Enums.CommMode commModeId)
		{
			short retResult  = VLF.CLS.Def.Const.unassignedShortValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfDcl.DclId FROM vlfBoxlProtocolGroupAssignment INNER JOIN vlfDcl ON vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId = vlfDcl.BoxProtocolGroupId WHERE vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId=" + (short)VLF.CLS.Def.Enums.ProtocolTypes.OTAv10 + " AND vlfDcl.CommModeId =" + Convert.ToInt16(commModeId);
		
				//Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != DBNull.Value)
					retResult = Convert.ToInt16(obj);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve OTA DCL id by commMode " + commModeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve OTA DCL id by commMode " + commModeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}
		/// <summary>
		/// Retrieves DCL ids by service state
		/// </summary>
		/// <param name="serviceState"></param>
		/// <returns>DataSet [DclId],[DclName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetDclInfoByStatus(VLF.CLS.Def.Enums.ServiceState serviceState)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfDcl.DclId,DclName"+
					" FROM vlfDcl WHERE ServiceState=" + Convert.ToInt16(serviceState);
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve DCL ids by ServiceState " + serviceState.ToString() + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve DCL ids by ServiceState " + serviceState.ToString() + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
		
		/// <summary>
		/// Retrieves DCL Id by name from "vlfDcl" table
		/// </summary>
		/// <param name="dclName"></param>
		/// <returns>dcl id</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short GetDclIdByName(string dclName)
		{
			short retResult  = VLF.CLS.Def.Const.unassignedIntValue;
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DclId FROM " + tableName + 
					" WHERE DclName='" + dclName.Replace("'","''") + "'";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve DCL id by name " + dclName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve DCL id by name " + dclName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if((sqlDataSet != null)&&(sqlDataSet.Tables.Count > 0)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				retResult = Convert.ToInt16(sqlDataSet.Tables[0].Rows[0][0]);
			}
			return retResult;
		}

		/// <summary>
		/// Retrieves DCL Status by name from "vlfDcl" table
		/// </summary>
		/// <param name="dclName"></param>
		/// <returns>DCL status <see cref="VLF.CLS.Def.Enums.ServiceState"/>/</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public VLF.CLS.Def.Enums.ServiceState GetDclStatusByName(string dclName)
		{
			VLF.CLS.Def.Enums.ServiceState retResult  = VLF.CLS.Def.Enums.ServiceState.Stopped;
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT ServiceState FROM " + tableName + 
					" WHERE DclName='" + dclName.Replace("'","''") + "'";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve DCL id by name " + dclName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve DCL id by name " + dclName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if((sqlDataSet != null)&&(sqlDataSet.Tables.Count > 0)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				retResult = (VLF.CLS.Def.Enums.ServiceState)Convert.ToInt16(sqlDataSet.Tables[0].Rows[0][0]);
			}
			return retResult;
		}
		/// <summary>
		/// Retrieves list of DCL Id by name from "vlfDcl" table
		/// </summary>
		/// <returns>In case of empty table returns null.</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public SortedList GetDclIdList()
		{
			SortedList lstResult = null;
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DclId,DclName FROM " + tableName + " ORDER BY DclId ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve DCL ids list. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve DCL ids list. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				lstResult = new SortedList();
				//Retrieves info from Table[0].[n][0] and Table[0].[n][1]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					lstResult.Add(Convert.ToInt16(currRow[0]),Convert.ToString(currRow[1]).TrimEnd());
				}
			}
			return lstResult;
		}

		/// <summary>
		/// Updates DCL type id from "vlfDcl" table
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="commModeId"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetCommModeId(short dclId, VLF.CLS.Def.Enums.CommMode commModeId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET CommModeId=" + Convert.ToInt16(commModeId) + 
					" WHERE DclId=" + dclId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new DCL communication mode Id=" + Convert.ToInt16(commModeId) + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new DCL communication mode Id=" + Convert.ToInt16(commModeId) + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new DCL communication mode Id=" + Convert.ToInt16(commModeId) + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " The DCL '" + dclId + "' does not exists.");
			}
		}
		/// <summary>
		/// Updates DCL Protocol Type Id
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="boxProtocolTypeId"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetBoxProtocolTypeId(short dclId, VLF.CLS.Def.Enums.ProtocolTypes boxProtocolTypeId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET BoxProtocolTypeId=" + Convert.ToInt16(boxProtocolTypeId) + 
					" WHERE DclId=" + dclId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new DCL protocol type Id=" + Convert.ToInt16(boxProtocolTypeId) + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new DCL protocol type Id=" + Convert.ToInt16(boxProtocolTypeId) + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new DCL protocol type Id=" + Convert.ToInt16(boxProtocolTypeId) + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " The DCL '" + dclId + "' does not exists.");
			}
		}
		/// <summary>
		/// Updates DCL type name from "vlfDCl" table
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="dclName"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDclTypeName(short dclId, string dclName)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET DclName='" + dclName.Replace("'","''") + "'" + 
					" WHERE DclId=" + dclId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new DCL type name '" + dclName + "'. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new DCL type name '" + dclName + "'. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new DCL type name '" + dclName + "'. ";
				throw new DASAppResultNotFoundException(prefixMsg + " The DCL '" + dclId + "' does not exists.");
			}
		}
		/// <summary>
		/// Updates DCL description in the "vlfDCl" table
		/// </summary>
		/// <param name="dclId"></param>
		/// <param name="description"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if does not exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDclDescription(short dclId, string description)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
							" SET Description='" + description.Replace("'","''") + "'" + 
							" WHERE DclId=" + dclId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to set new DCL description '" + description + "'. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to set new DCL description '" + description + "'. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to set new DCL description '" + description + "'. ";
				throw new DASAppResultNotFoundException(prefixMsg + " The DCL '" + dclId + "' does not exists.");
			}
		}
	}
}
