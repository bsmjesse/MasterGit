using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for OleDbDataReader
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxCommInfo table.
	/// </summary>
	public class BoxCommInfo : TblGenInterfaces
   {
      /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxCommInfo(SQLExecuter sqlExec) : base ("vlfBoxCommInfo",sqlExec)
		{
		}
		/// <summary>
		/// Add new box communication info.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <param name="commAddressValue"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown DASAppDataAlreadyExistsException if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddCommInfo(int boxId,short commAddressTypeId,string commAddressValue)
		{
			ValidateCommAddressValue(commAddressTypeId,commAddressValue);
			ValidateCommAddressTypeByBoxId(boxId,commAddressTypeId);
			if(commAddressValue.TrimEnd() != "" && commAddressTypeId != (short)VLF.CLS.Def.Enums.CommAddressType.IP && commAddressTypeId != (short)VLF.CLS.Def.Enums.CommAddressType.Port && IsFreeCommInfo(boxId,commAddressTypeId,commAddressValue) == false)
				throw new DASAppDataAlreadyExistsException("Validation warning. Phone number " + commAddressValue + " alredy assigned to another box. ");
			
			string prefixMsg = "Unable to add new communication info for box '" + boxId + "'.";		
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName 
				+ " (BoxId,CommAddressTypeId,CommAddressValue)"
				+ " VALUES ( {0}, {1}, '{2}')",
				boxId,commAddressTypeId,commAddressValue.Replace("'","''"));
			int rowsAffected = 0;
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

			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The box communication info already exists.");
			}
		}		
		/// <summary>
		/// Update box communication info.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <param name="commAddressValue"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if data already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateCommInfo(int boxId,short commAddressTypeId,string commAddressValue)
		{
			ValidateCommAddressValue(commAddressTypeId,commAddressValue);
			ValidateCommAddressTypeByBoxId(boxId,commAddressTypeId);
			if(commAddressValue != "" && commAddressTypeId != (short)VLF.CLS.Def.Enums.CommAddressType.IP && commAddressTypeId != (short)VLF.CLS.Def.Enums.CommAddressType.Port && IsFreeCommInfo(boxId,commAddressTypeId,commAddressValue) == false)
				throw new DASAppDataAlreadyExistsException("Validation warning. Phone number " + commAddressValue + " alredy assigned to another box. ");
			
			if(	(commAddressValue == VLF.CLS.Def.Const.unassignedStrValue)||
				(boxId == VLF.CLS.Def.Const.unassignedIntValue)||
				(commAddressTypeId == VLF.CLS.Def.Const.unassignedIntValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: Box Id=" + boxId + 
														" CommAddressTypeId=" + commAddressTypeId + 
														" CommAddressValue=" + commAddressValue.TrimEnd());
			}
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET CommAddressValue='" + commAddressValue.TrimEnd().Replace("'","''") + "'" +
				" WHERE BoxId=" + boxId + " AND CommAddressTypeId=" + commAddressTypeId;
			
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update communication info for box '" + boxId + "'.";		
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update communication info for box '" + boxId + "'.";		
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update communication info for box '" + boxId + "'.";		
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
			}
		}	
		/// <summary>
		/// Delete existing box communication info.
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="commAddressTypeId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteCommInfo(int boxId,short commAddressTypeId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + " WHERE BoxId =" + boxId + 
				" AND CommAddressTypeId=" + commAddressTypeId;
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
				string prefixMsg = "Unable to delete by BoxId=" + boxId 
					+ " and CommAddressTypeId=" + commAddressTypeId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete by BoxId=" + boxId 
					+ " and CommAddressTypeId=" + commAddressTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Delete existing box communication info.
		/// </summary>
		/// <param name="boxId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteCommInfoByBoxId(int boxId)
		{
			int rowsAffected = 0;
			string prefixMsg = "Unable to delete by BoxId=" + boxId + ". ";
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + " WHERE BoxId =" + boxId;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
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
			return rowsAffected;
		}
		
		/// <summary>
		/// Returns only first box. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetCommInfoByBoxId(int boxId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfBoxCommInfo.CommAddressTypeId,CommAddressValue,CommAddressTypeName" +
							" FROM vlfBoxCommInfo,vlfCommAddressType" + 
							" WHERE BoxId=" + boxId +
							" AND vlfBoxCommInfo.CommAddressTypeId=vlfCommAddressType.CommAddressTypeId";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

      /*
            /// <summary>
            /// Returns only first box. 	
            /// </summary>
            /// <param name="boxId"></param>
              /// <returns>DataSet [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName],[FwChId],[BoxHwTypeId],[BoxProtocolTypeId],[CommModeId],[ChPriority],[OAPPort]</returns>
            /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
            /// <exception cref="DASException">Thrown in all other exception cases.</exception>
            public DataSet GetFullCommInfoByBoxId(int boxId)
            {
               DataSet resultDataSet = null;
               try
               {
                  sqlExec.ClearCommandParameters();() ;
                  //Prepares SQL sp adding params
                  sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);            
                  //Executes SQL statement
                  resultDataSet = sqlExec.SPExecuteDataset("sp_GetFullCommInfoByBoxId");
               }
               catch (SqlException objException)
               {
                  string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
                  Util.ProcessDbException(prefixMsg, objException);
               }
               catch (DASDbConnectionClosed exCnn)
               {
                  throw new DASDbConnectionClosed(exCnn.Message);
               }
               catch (Exception objException)
               {
                  string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
                  throw new DASException(prefixMsg + " " + objException.Message);
               }
               return resultDataSet;
            }
       */

      /// <summary>
      /// Returns only first box. 	
      /// </summary>
      /// <param name="boxId"></param>
      /// <returns>DataSet [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName],[FwChId],[BoxHwTypeId],[BoxProtocolTypeId],[CommModeId],[ChPriority],[OAPPort]</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public DataSet GetFullCommInfoByBoxId(int boxId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
            string sql = "SELECT vlfCommModeAddressType.CommAddressTypeId, vlfBoxCommInfo.CommAddressValue,vlfCommAddressType.CommAddressTypeName, vlfFirmwareChannels.FwChId,vlfFirmware.BoxHwTypeId, vlfChannels.BoxProtocolTypeId, vlfChannels.CommModeId, vlfFirmwareChannels.ChPriority, OAPPort, vlfBox.BoxSequenceNum FROM vlfBox with (nolock) INNER JOIN vlfFirmwareChannelReference ON vlfBox.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfCommModeAddressType ON vlfCommMode.CommModeId = vlfCommModeAddressType.CommModeId INNER JOIN vlfCommAddressType ON vlfCommModeAddressType.CommAddressTypeId = vlfCommAddressType.CommAddressTypeId INNER JOIN vlfBoxCommInfo ON vlfBox.BoxId = vlfBoxCommInfo.BoxId AND vlfCommAddressType.CommAddressTypeId = vlfBoxCommInfo.CommAddressTypeId WHERE vlfBox.BoxId=" + boxId + " ORDER BY vlfCommModeAddressType.CommAddressTypeId";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
 
		/// <summary>
		/// Returns only first box. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <returns>CommAddressValue</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetCommInfoAddressValue(int boxId,short commAddressTypeId)
		{
			string commAddressValue = "";
			try
			{
				//Prepares SQL statement
				string sql = "SELECT CommAddressValue" +
					" FROM vlfBoxCommInfo,vlfCommAddressType" + 
					" WHERE BoxId=" + boxId +
					" AND vlfCommAddressType.CommAddressTypeId=" + commAddressTypeId +
					" AND vlfBoxCommInfo.CommAddressTypeId=vlfCommAddressType.CommAddressTypeId";
				//Executes SQL statement
				object retresult = sqlExec.SQLExecuteScalar(sql);
				if(retresult != null)
					commAddressValue = retresult.ToString().Trim();
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve commAddressValue by box id=" + boxId + " and commAddressTypeId=" + commAddressTypeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve commAddressValue by box id=" + boxId + " and commAddressTypeId=" + commAddressTypeId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return commAddressValue;
		}
		/// <summary>
		/// Returns only first box id by CommAddressValue. 	
		/// </summary>
		/// <param name="commAddressValue"></param>
		/// <returns>box id</returns>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException in case of unexpected multiple rows result.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetBoxIdByCommAddressValue(string commAddressValue)
		{
			int boxId = VLF.CLS.Def.Const.unassignedIntValue;
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT BoxId FROM vlfBoxCommInfo" + 
					" WHERE CommAddressValue='" + commAddressValue.Replace("'","''") + "'";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
			{
				if(resultDataSet.Tables[0].Rows.Count == 1)
				{
					boxId = Convert.ToInt32(resultDataSet.Tables[0].Rows[0][0]);
				}
				else
				{
					throw new DASAppWrongResultException("Unexpected multiple rows result.");
				}
			}
			return boxId;
		}
		/// <summary>
		/// Returns box configuration info by CommAddressValue. 	
		/// </summary>
		/// <param name="commAddressValue"></param>
        /// <returns>DataSet [BoxId],[BoxHwTypeId],[BoxProtocolTypeId],[CommModeId],[FwChId],[OAPPort]</returns>
		/// <exception cref="DASAppWrongResultException">Thrown DASAppWrongResultException in case of unexpected multiple rows result.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetBoxConfigInfoByCommAddressValue(string commAddressValue)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
            string sql = "SELECT vlfBox.BoxId, vlfFirmware.BoxHwTypeId, vlfChannels.BoxProtocolTypeId, vlfChannels.CommModeId, vlfFirmwareChannels.FwChId, OAPPort FROM vlfBox with (nolock) INNER JOIN vlfFirmwareChannelReference ON vlfBox.FwChId = vlfFirmwareChannelReference.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfBoxCommInfo ON vlfBox.BoxId = vlfBoxCommInfo.BoxId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfCommAddressType ON vlfBoxCommInfo.CommAddressTypeId = vlfCommAddressType.CommAddressTypeId INNER JOIN vlfCommModeAddressType ON vlfCommMode.CommModeId = vlfCommModeAddressType.CommModeId AND vlfCommAddressType.CommAddressTypeId = vlfCommModeAddressType.CommAddressTypeId WHERE vlfBoxCommInfo.CommAddressValue = '" + commAddressValue.Replace("'", "''") + "'";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box info by commAddressValue=" + commAddressValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box info by commAddressValue=" + commAddressValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Prevents inconsistent insert of the "commAddressTypeId" field to the 
		/// "vlfBoxCommInfo" table by checking valid dependency in the 
		/// "vlfCommModeAddressType" table.
		/// In case of inconsistency throws exception.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commAddressTypeId"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if communication address type doesn't match to box configuration.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		private void ValidateCommAddressTypeByBoxId(int boxId,short commAddressTypeId)
		{
			// 1. Prepares SQL statement
         string sql = "SELECT COUNT(*) FROM vlfBox with (nolock) INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId INNER JOIN vlfCommMode ON vlfChannels.CommModeId = vlfCommMode.CommModeId INNER JOIN vlfCommModeAddressType ON vlfCommMode.CommModeId = vlfCommModeAddressType.CommModeId WHERE vlfBox.BoxId=" + boxId + " AND vlfCommModeAddressType.CommAddressTypeId=" + commAddressTypeId;				 
			int recordCount = 0;
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				recordCount = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Validation warning. Wrong communication address type=" 
					+ commAddressTypeId + " for box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Validation warning. Wrong communication address type=" 
					+ commAddressTypeId + " for box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if(recordCount <= 0) 
			{
				string prefixMsg = "Validation warning. Wrong communication address type=" 
					+ commAddressTypeId + " for box id=" + boxId + ". ";
				throw new DASAppResultNotFoundException(prefixMsg);
			}
		}
		/// <summary>
		/// Validate commAddressValue by commAddressValue
		/// </summary>
		/// <param name="commAddressTypeId"></param>
		/// <param name="commAddressValue"></param>
		private void ValidateCommAddressValue(short commAddressTypeId,string commAddressValue)
		{
			commAddressValue = commAddressValue.Trim();
			if(commAddressValue.Length == 0)
				return;
			
			switch((VLF.CLS.Def.Enums.CommAddressType)commAddressTypeId)
			{
				#region PhoneNum
				case VLF.CLS.Def.Enums.CommAddressType.PhoneNum:
					if(commAddressValue.Length > 16)
						throw new VLF.ERR.DASAppInvalidValueException("The phone number (" + commAddressValue + ") length exceeded maximal 16 characters.");
					try
					{
						Int64 val = Convert.ToInt64(commAddressValue);
					}
					catch
					{
						throw new VLF.ERR.DASAppInvalidValueException("The phone number (" + commAddressValue + ") must contain numerical characters only.");
					}
					break;
				#endregion
				#region IP
				case VLF.CLS.Def.Enums.CommAddressType.IP:
					if(commAddressValue.Length > 15)
						throw new VLF.ERR.DASAppInvalidValueException("The IP address (" + commAddressValue + ") length exceeded maximal 15 characters.");
					
					try
					{
						System.Net.IPAddress val = System.Net.IPAddress.Parse(commAddressValue);
					}
					catch(System.FormatException e)
					{
						throw new VLF.ERR.DASAppInvalidValueException("Incorrect IP address (" + commAddressValue + ") format.");
					}
					break;
				#endregion
				#region Port
				case VLF.CLS.Def.Enums.CommAddressType.Port:
					if(commAddressValue.Length > 5)
						throw new VLF.ERR.DASAppInvalidValueException("The Port (" + commAddressValue + ") length exceeded maximal 5 characters.");
					
					try
					{
						UInt16 val = Convert.ToUInt16(commAddressValue);
					}
					catch
					{
						throw new VLF.ERR.DASAppInvalidValueException("Incorrect Port (" + commAddressValue + ") value.");
					}
					break;
				#endregion
				#region Mins
				case VLF.CLS.Def.Enums.CommAddressType.PMin:
				case VLF.CLS.Def.Enums.CommAddressType.Min1:
				case VLF.CLS.Def.Enums.CommAddressType.Min2:
				case VLF.CLS.Def.Enums.CommAddressType.Min3:
				case VLF.CLS.Def.Enums.CommAddressType.Min4:
				case VLF.CLS.Def.Enums.CommAddressType.Min5:
				case VLF.CLS.Def.Enums.CommAddressType.Min6:
				case VLF.CLS.Def.Enums.CommAddressType.Min7:
				case VLF.CLS.Def.Enums.CommAddressType.Min8:
				case VLF.CLS.Def.Enums.CommAddressType.Min9:
					if(commAddressValue.Length != 10)
						throw new VLF.ERR.DASAppInvalidValueException("The " + ((VLF.CLS.Def.Enums.CommAddressType)commAddressTypeId).ToString() +  " (" + commAddressValue + ") must contain 10 digits.");
					
					try
					{
						Int64 val = Convert.ToInt64(commAddressValue);
					}
					catch
					{
						throw new VLF.ERR.DASAppInvalidValueException("Incorrect " + ((VLF.CLS.Def.Enums.CommAddressType)commAddressTypeId).ToString() + " (" + commAddressValue + ") value.");
					}
					break;
					#endregion
				#region ESN
				case VLF.CLS.Def.Enums.CommAddressType.ESN:
					if(commAddressValue.Length != 11)
						throw new VLF.ERR.DASAppInvalidValueException("The ESN (" + commAddressValue + ") must contain 11 digits.");
					
					try
					{
						Int64 val = Convert.ToInt64(commAddressValue);
					}
					catch
					{
						throw new VLF.ERR.DASAppInvalidValueException("Incorrect ESN (" + commAddressValue + ") value.");
					}
					break;
				#endregion
				case VLF.CLS.Def.Enums.CommAddressType.DeviceID:
					break;
				default:
					break;
			}
		}
		/// <summary>
		/// Check if communication info does not assigned to another box
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commInfo"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns>true if phone is free, otherwise returns false</returns>
		public bool IsFreeCommInfo(int boxId,short commAddressType,string commInfo)
		{
			bool isFree = false;
			//Prepares SQL statement
			string sql = "SELECT COUNT(*) FROM vlfBoxCommInfo WHERE CommAddressTypeId=" + commAddressType + 
						" AND CommAddressValue='" + commInfo + "'" +
						" AND BoxId<>" + boxId;				 
			int recordCount = 0;
			try
			{
				//Executes SQL statement
				recordCount = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Validation warning for box id=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Validation warning for box id=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if(recordCount == 0) 
			{
				isFree = true;
			}
			return isFree;
		}

		/// <summary>
		/// Get Primary Server Ip. 	
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="commModeId"></param>
		/// <returns>serverIp</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetPrimaryServerIp(int boxId,short commModeId)
		{
			string serverIp = "";
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(255) ;
				//Prepares SQL statement
                string sql = "SELECT vlfConfiguration.KeyValue FROM vlfFirmwareChannelReference INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId INNER JOIN vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId INNER JOIN vlfConfigurationModules INNER JOIN vlfDcl ON vlfConfigurationModules.ModuleName = vlfDcl.DclName INNER JOIN vlfConfiguration ON vlfConfigurationModules.ModuleId = vlfConfiguration.ModuleId INNER JOIN vlfBoxlProtocolGroupAssignment ON vlfDcl.BoxProtocolGroupId = vlfBoxlProtocolGroupAssignment.BoxProtocolGroupId INNER JOIN vlfChannels ON vlfBoxlProtocolGroupAssignment.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId AND vlfDcl.CommModeId = vlfChannels.CommModeId ON vlfFirmwareChannels.ChId = vlfChannels.ChId WHERE vlfBox.BoxId=" + boxId + " AND vlfConfiguration.KeyName='Server IP Address' AND vlfChannels.CommModeId<>" + commModeId;
				//Executes SQL statement
				object retresult = sqlExec.SQLExecuteScalar(sql);
				if(retresult != null)
					serverIp = retresult.ToString().TrimEnd();
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve primary comm mode by box id=" + boxId + " and commModeId=" + commModeId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = " Unable to retrieve primary comm mode by box id=" + boxId + " and commModeId=" + commModeId + ". ";
				throw new DASDbConnectionClosed(prefixMsg + objException.Message);
			}
			return serverIp;
		}

      /// <summary>
      ///      Get box - server configuration
      /// </summary>
      /// <param name="commModeId"></param>
      /// <param name="protocolTypeId"></param>
      /// <returns></returns>
      public DataSet GetBoxServerConfig(int commModeId, int protocolTypeId)
      {
         DataSet dsResult = null;
         try
         {
            //Prepares SQL statement
            // string sql = "SELECT IPExternal, PortExternal, IPInternal, PortInternal FROM ServerConfigCommProtocol WHERE CommModeID=" + commModeId + " AND BoxProtocolTypeID=" + protocolTypeId;
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@commmodeid", SqlDbType.SmallInt, commModeId);
            sqlExec.AddCommandParam("@protocoltypeid", SqlDbType.SmallInt, protocolTypeId);
            //Executes SQL statement
            // dsResult = sqlExec.SQLExecuteDataset(sql);
            dsResult = sqlExec.SPExecuteDataset("BoxServerConfigGet");
         }
         catch (SqlException se)
         {
            Util.ProcessDbException("Stored procedure error", se);
            throw se;
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         return dsResult;
      }
	}
}
