using System;
using System.Data.SqlClient ;	//for SqlException
using System.Data;
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfBoxMsgSeverity table.
	/// </summary>
	public class BoxMsgSeverity : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public BoxMsgSeverity(SQLExecuter sqlExec) : base ("vlfBoxMsgSeverity",sqlExec)
		{
		}

      public bool IsAlarmCreated(int boxId, VLF.CLS.Def.Enums.MessageType boxMsgInTypeId)
      {
//         object[] obj = base.GetValueByFilter("CreateAlarm",
                           
         return false;
      }
		/// <summary>
		///      Retrieves alarm info for specific message type
		/// </summary>
		/// <param name="boxId"></param> 
		/// <param name="boxMsgInTypeId"></param> 
		/// <returns>DataSet [AlarmLevel],[AlarmLevelName],[CreateAlarm]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      /// <comment> 
      ///      it is SOOOOOO STUPID TO RETURN A DATASET FOR TWO FU$^##$% VALUES
      ///      (gb) 2009/10/05
      /// </comment>
		public DataSet GetAlarmInfo(int boxId,VLF.CLS.Def.Enums.MessageType boxMsgInTypeId)
		{
			DataSet alarmInfo = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM vlfBoxMsgSeverity"+
							" INNER JOIN vlfSeverity ON vlfBoxMsgSeverity.AlarmLevel=vlfSeverity.AlarmLevel"+
							" WHERE BoxId=" + boxId + 
							" AND BoxMsgInTypeId=" + (short)boxMsgInTypeId;
				
				//Executes SQL statement
				alarmInfo = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve box=" + boxId + " message=" + boxMsgInTypeId + " severity. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve box=" + boxId + " message=" + boxMsgInTypeId + " severity. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return alarmInfo;
		}
		/// <summary>
		/// Add new message severity to the box.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxMsgInTypeId"></param>
		/// <param name="alarmLevel"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if message severity already exist for this box.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddMsgSeverity(int boxId,VLF.CLS.Def.Enums.MessageType boxMsgInTypeId,VLF.CLS.Def.Enums.AlarmSeverity alarmLevel)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO vlfBoxMsgSeverity ( BoxId, BoxMsgInTypeId,AlarmLevel) VALUES ( {0}, {1}, {2})", boxId,(short)boxMsgInTypeId, (short)alarmLevel);
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
				string prefixMsg = "Unable to add new message=" + boxMsgInTypeId + " to box=" + boxId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new message=" + boxMsgInTypeId + " to box=" + boxId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new message=" + boxMsgInTypeId + " to box=" + boxId;
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The message already exists.");
			}
		}
		/// <summary>
		/// Update box message severity
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="boxMsgInTypeId"></param>
		/// <param name="alarmLevel"></param>
		/// <exception cref="DASAppResultNotFoundException">Thrown if data doesn't exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateMsgSeverity(int boxId,VLF.CLS.Def.Enums.MessageType boxMsgInTypeId,VLF.CLS.Def.Enums.AlarmSeverity alarmLevel)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE vlfBoxMsgSeverity SET AlarmLevel=" + (short)alarmLevel +
					" WHERE BoxId=" + boxId +
					" AND BoxMsgInTypeId=" + (short)boxMsgInTypeId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to update message: " + (short)boxMsgInTypeId + " severity for box="+ boxId; 
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to update message: " + (short)boxMsgInTypeId + " severity for box="+ boxId; 
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to update message: " + (short)boxMsgInTypeId + " severity for box="+ boxId; 
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong box id='" + boxId + "' or message id='" + (short)boxMsgInTypeId + "'.");
			}
		}
		
		/// <summary>
		/// Check if we should create alarm for current severity
		/// </summary>
		/// <param name="alarmSeverity"></param> 
		/// <returns>true in case of alarm, otherwise false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsAlarm(VLF.CLS.Def.Enums.AlarmSeverity alarmSeverity)
		{
			bool isAlarm = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT CreateAlarm FROM vlfSeverity"+
					" WHERE AlarmLevel=" + (short)alarmSeverity;
				
				//Executes SQL statement
				object obj = sqlExec.SQLExecuteScalar(sql);
				if(obj != null)
					isAlarm = Convert.ToBoolean(obj);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve alarm creation rule for alarm level =" + alarmSeverity.ToString();
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve alarm creation rule for alarm level =" + alarmSeverity.ToString();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return isAlarm;
		}
		
		/// <summary>
		/// Delete exist box messages
		/// </summary>
		/// <param name="boxId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteRecordByBoxId(int boxId)
		{
			return DeleteRowsByIntField("BoxId",boxId, "box messages");
		}
		/// <summary>
		/// Retrieves all supported messages by box
		/// </summary>
		/// <param name="boxId"></param>
		/// <returns>DataSet [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllSupportedMessagesByBoxId(int boxId)
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT vlfBoxMsgSeverity.BoxMsgInTypeId, vlfBoxMsgInType.BoxMsgInTypeName, vlfBoxMsgSeverity.AlarmLevel"+
							" FROM vlfBoxMsgSeverity INNER JOIN vlfBoxMsgInType ON vlfBoxMsgSeverity.BoxMsgInTypeId = vlfBoxMsgInType.BoxMsgInTypeId"+
							" WHERE vlfBoxMsgInType.Visible=1 AND vlfBoxMsgSeverity.BoxId=" + boxId +
							" ORDER BY vlfBoxMsgInType.BoxMsgInTypeName";

				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}

				// 3. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of all supported messages by protocol=" + boxId + ".";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of all supported messages by protocol=" + boxId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}
	}
}
