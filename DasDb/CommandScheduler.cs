using System;
using System.Data.SqlClient ;	// for SqlException
using System.Data ;			// for DataSet
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfCommandScheduler table.
	/// </summary>
	public class CommandScheduler : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public CommandScheduler(SQLExecuter sqlExec) : base ("vlfCommandScheduler",sqlExec)
		{
		}
		/// <summary>
		/// Add new task
		/// </summary>
		/// <returns> current task id or -1 in case of error</returns>
		/// <param name="userId"></param>
		/// <param name="time"></param>
		/// <param name="boxId"></param>
		/// <param name="commandID"></param>
		/// <param name="customProp"></param>
		/// <param name="protocolType"></param>
		/// <param name="commMode"></param>
		/// <param name="transmissionPeriod"></param>
		/// <param name="transmissionInterval"></param>
		/// <param name="usingDualMode"></param>
		/// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException information already exist.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 AddTask(int userId, DateTime time, int boxId, short commandID, string customProp, 
							short protocolType, short commMode,Int64 transmissionPeriod,
							int transmissionInterval,bool usingDualMode)
		{
			Int64 rowsAffected = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				// Set SQL command
				string sql = "INSERT INTO vlfCommandScheduler(RequestDateTime,BoxId,UserId,BoxCmdOutTypeId,BoxProtocolTypeId,CommModeId,TransmissionPeriod,TransmissionInterval,CustomProp,UsingDualMode) VALUES ( @RequestDateTime,@BoxId,@UserId,@BoxCmdOutTypeId,@BoxProtocolTypeId,@CommModeId,@TransmissionPeriod,@TransmissionInterval,@CustomProp,@UsingDualMode) Select Max(MsgId) from vlfTxtMsgs";
				sqlExec.ClearCommandParameters();
				// Add parameters to SQL statement
				sqlExec.AddCommandParam("@RequestDateTime",SqlDbType.DateTime,time);
				sqlExec.AddCommandParam("@BoxId",SqlDbType.Int,boxId);
				sqlExec.AddCommandParam("@UserId",SqlDbType.Int,userId);
				sqlExec.AddCommandParam("@BoxCmdOutTypeId",SqlDbType.SmallInt,commandID);
				sqlExec.AddCommandParam("@BoxProtocolTypeId",SqlDbType.SmallInt,protocolType);
				sqlExec.AddCommandParam("@CommModeId",SqlDbType.SmallInt,commMode);
				sqlExec.AddCommandParam("@TransmissionPeriod",SqlDbType.BigInt,transmissionPeriod);
				sqlExec.AddCommandParam("@TransmissionInterval",SqlDbType.Int,transmissionInterval);
				if(customProp == null)
					sqlExec.AddCommandParam("@CustomProp",SqlDbType.VarChar,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@CustomProp",SqlDbType.VarChar,customProp);
				sqlExec.AddCommandParam("@UsingDualMode",SqlDbType.SmallInt,Convert.ToInt16(usingDualMode));
				
				//Executes SQL statement
				object currentTaskId = sqlExec.SQLExecuteScalar(sql);
				if(currentTaskId != null)
					rowsAffected = Convert.ToInt64(currentTaskId);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to schedule new task.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to schedule new task.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Delete existing task
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="taskId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if task does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteTask(Int64 taskId)
		{
			return DeleteRowsByIntField("TaskId", taskId, "task id");		
		}

        /// <summary>
        /// Delete box tasks
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public int DeleteTasksByBoxId(int boxId)
        {
            return DeleteRowsByIntField("BoxId", boxId, "box id");
        }

		/// <summary>
		/// Reschedule task
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="transmissionPeriod"></param>
		/// <param name="transmissionInterval"></param>
		/// <param name="usingDualMode"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int ReScheduledTask(Int64 taskId, Int64 transmissionPeriod,int transmissionInterval,bool usingDualMode)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE vlfCommandScheduler"+ 
					" SET TransmissionPeriod=" + transmissionPeriod +
					",TransmissionInterval=" +  transmissionInterval + 
					",UsingDualMode=" +  Convert.ToInt16(usingDualMode) + 
					" WHERE TaskId=" + taskId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to reschedule task. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to reschedule task. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				throw new DASAppViolatedIntegrityConstraintsException("Unable to reschedule task. ");
			}
			return rowsAffected;
		}		
		/// <summary>
		/// Get user tasks
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [TaskId],[RequestDateTime],[BoxId],[UserId],[BoxCmdOutTypeId],
		/// [BoxProtocolTypeId],[CommModeId],[TransmissionPeriod],[TransmissionInterval],
		/// [CustomProp],[LastDateTimeSent],[UsingDualMode],[VehicleId],
		/// [Description],[LicensePlate]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserTasks(int userId)
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "DECLARE @Timezone int"+
					" DECLARE @DayLightSaving int"+
					" SELECT @Timezone=PreferenceValue FROM vlfUserPreference"+
					" WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.TimeZone +
					" IF @Timezone IS NULL SET @Timezone=0"+
					" SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId="+ (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
					" IF @DayLightSaving IS NULL SET @DayLightSaving=0"+
					" SET @Timezone= @Timezone + @DayLightSaving"+
					
					" SELECT TaskId,"+
					" CASE WHEN RequestDateTime IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,RequestDateTime),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,RequestDateTime),108) END as RequestDateTime,"+
					" vlfCommandScheduler.BoxId, UserId, vlfCommandScheduler.BoxCmdOutTypeId, BoxProtocolTypeId,"+
					" CommModeId, TransmissionPeriod, TransmissionInterval,"+
					" RTRIM(CustomProp) AS CustomProp,"+
					" CASE WHEN LastDateTimeSent IS NULL THEN 'N/A' ELSE convert(varchar,DATEADD(hour,@Timezone,LastDateTimeSent),101) + ' ' + convert(varchar,DATEADD(hour,@Timezone,LastDateTimeSent),108) END as LastDateTimeSent,"+
					" UsingDualMode,vlfVehicleInfo.VehicleId,"+
					" RTRIM(vlfVehicleInfo.Description) AS Description,"+ 
                    " RTRIM(vlfVehicleAssignment.LicensePlate) AS LicensePlate,"+
					" RTRIM(BoxCmdOutTypeName) AS BoxCmdOutTypeName "+
					" FROM vlfVehicleAssignment INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId"+
					" INNER JOIN vlfCommandScheduler ON vlfVehicleAssignment.BoxId = vlfCommandScheduler.BoxId"+
					" INNER JOIN vlfBoxCmdOutType ON vlfCommandScheduler.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId"+
					" WHERE vlfCommandScheduler.UserId="+ userId +
					" ORDER BY RequestDateTime DESC";
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve user=" + userId + " tasks.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve user=" + userId + " tasks. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}
        /// <summary>
        /// Reset Box Cmds Scheduled DateTime
        /// </summary>
        /// <param name="boxId"></param>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int ResetBoxCmdsScheduledDateTime(int boxId)
        {
            int rowsAffected = 0;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @tasks int SELECT @tasks=COUNT(*) from vlfCommandScheduler where BoxId=" + boxId + " IF @tasks>0 UPDATE vlfCommandScheduler SET RequestDateTime='" + DateTime.Now + "' WHERE BoxId=" + boxId;
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to reset RequestDateTime for box " + boxId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to reset RequestDateTime for box " + boxId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                throw new DASAppViolatedIntegrityConstraintsException("Unable to reset RequestDateTime for box " + boxId + ". ");
            }
            return rowsAffected;
        }
		/// <summary>
		/// Update last DateTime sent
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="lastDateTimeSent"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int UpdateLastDateTimeSent(Int64 taskId,DateTime lastDateTimeSent)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE vlfCommandScheduler"+ 
					" SET LastDateTimeSent='" + lastDateTimeSent.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
					" WHERE TaskId=" + taskId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update task LastDateTimeSent. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update task LastDateTimeSent. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}

      /// <summary>
      ///      put value 0 for RequestStatus 
      /// </summary>
      public void InitScheduledTasks()
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = "UPDATE vlfCommandScheduler SET RequestStatus= 0";
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update task RequestStatus. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update task RequestStatus. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         
      }

      /// <summary>
      ///         
      /// </summary>
      /// <param name="taskId"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>       
      public bool UpdateScheduledTask(Int64 taskId, byte value, DateTime lastDateTimeSent,DateTime lastDateTimeTouched)
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = "UPDATE vlfCommandScheduler SET RequestStatus=" + value.ToString() + 
               ", LastDateTimeSent='" + lastDateTimeSent.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
               ", LastDateTimeTouched='" + lastDateTimeTouched.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'" +
               " WHERE TaskId=" + taskId;
            //Executes SQL statement
            Util.BTrace(Util.INF0, "UpdateScheduledTask -> {0}", sql);
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to update task RequestStatus. ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to update task RequestStatus. ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return (rowsAffected == 1);
      }
		/// <summary>
		/// Get currently scheduled tasks
		/// </summary>
		/// <returns>DataSet [TaskId],[RequestDateTime],[BoxId],[UserId],[BoxCmdOutTypeId],
		/// [BoxProtocolTypeId],[CommModeId],[TransmissionPeriod],[TransmissionInterval],
		/// [CustomProp],[LastDateTimeSent],[UsingDualMode]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetCurrentlyScheduledTasks()
		{
			DataSet sqlDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = " SELECT * FROM vlfCommandScheduler"+
					" WHERE RequestDateTime<=GETDATE()"+
					" AND (LastDateTimeSent IS NULL OR (LastDateTimeSent IS NOT NULL AND GETDATE() > DATEADD(second, TransmissionInterval, LastDateTimeSent)))";
				// 2. Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				Util.ProcessDbException("Unable to retieve currently scheduled tasks.", objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				throw new DASException("Unable to retieve currently scheduled tasks. " + objException.Message);
			}
			// 3. Return result
			return sqlDataSet;
		}

      /// <summary>
      ///         this returns ONLY records with RequestStatus = 0 (INIT)
      ///         using sp <sp_GetScheduledTasks>
      /// </summary>
      /// <param name="max"></param>
      /// <returns></returns>
      public DataSet GetCurrentlyScheduledTasks(int max)
      {
         DataSet sqlDataSet = null;
         try
         {
            sqlExec.ClearCommandParameters();
            //Prepares SQL statement
            sqlExec.AddCommandParam("@cnt", SqlDbType.Int, max);
            //Executes SQL statement
            sqlDataSet = sqlExec.SPExecuteDataset("sp_GetScheduledTasks");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("CommandScheduler.GetCurrentlyScheduledTasks (1)->", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("CommandScheduler.GetCurrentlyScheduledTasks (2)->" + objException.Message);
         }

         return sqlDataSet;
      }


      /// <summary>
      ///         this returns ONLY records with RequestStatus = 0 (INIT)
      ///         using sp <sp_GetScheduledTasksExcept>
      ///         this is used to not schedule commands for boxes undergoing OTA 
      /// </summary>
      /// <param name="max"></param>
      /// <returns></returns>
      public DataSet GetCurrentlyScheduledTasksExcept(int max, int[] boxIDs)
      {
         DataSet sqlDataSet = null;
         try
         {
            sqlExec.ClearCommandParameters();
            //Prepares SQL statement
            sqlExec.AddCommandParam("@cnt", SqlDbType.Int, max);
            sqlExec.AddCommandParam("@notIn", SqlDbType.VarChar, Util.ArrayToString<int>(boxIDs));
            //Executes SQL statement
            sqlDataSet = sqlExec.SPExecuteDataset("sp_GetScheduledTasksExcept");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("CommandScheduler.GetCurrentlyScheduledTasksExcept (1)->", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("CommandScheduler.GetCurrentlyScheduledTasksExcept (2)->" + objException.Message);
         }

         return sqlDataSet;
      }
      /// <summary>
      ///         move the task from vlfCommandScheduler to vlfCommandSchedulerHst
      ///         if dtAckReceived is DateTime.MinValue the command was unsuccessful, otherwise you could link
      ///         MsgOutHst based on <boxId, protocoltype, commandId>
      ///         with 
      ///         MsgInHst based on <boxId, dtAckReceived>
      /// </summary>
      /// <param name="taskId"></param>
      /// <param name="dtAckReceived"></param>
      public void AddTaskInHistory(Int64 taskId, DateTime dtAckReceived)
      {
         DataSet sqlDataSet = null;
         try
         {
            sqlExec.ClearCommandParameters();
            //Prepares SQL statement
            sqlExec.AddCommandParam("@TaskId", SqlDbType.BigInt, taskId);
            sqlExec.AddCommandParam("@MsgInDateTime", SqlDbType.DateTime, dtAckReceived);
            //Executes SQL statement
            sqlDataSet = sqlExec.SPExecuteDataset("sp_AddScheduledTask2History");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("CommandScheduler.AddTaskInHistory (1)->", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("CommandScheduler.AddTaskInHistory (2)->" + objException.Message);
         }

      }


      /// <summary>
      ///        Retrieve Scheduled task history
      /// </summary>
      /// <param name="fromDate"></param>
      /// <param name="toDate"></param>
      /// <param name="fleetId"></param>
      /// <param name="boxId"></param>
      public DataSet GetSheduledTasksHistory(DateTime fromDate, DateTime toDate, Int32 fleetId, Int32 boxId)
      {
         DataSet sqlDataSet = null;
         try
         {
            sqlExec.ClearCommandParameters();
            //Prepares SQL statement
            sqlExec.AddCommandParam("@FromDate", SqlDbType.DateTime, fromDate);
            sqlExec.AddCommandParam("@ToDate", SqlDbType.DateTime, toDate);
            sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            
            //Executes SQL statement
            sqlDataSet = sqlExec.SPExecuteDataset("[sp_GetCommandSchedulerHst_New]");
         }
         catch (SqlException objException)
         {
            Util.ProcessDbException("CommandScheduler.GetSheduledTasksHistory (1)->", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            throw new DASException("CommandScheduler.GetSheduledTasksHistory (2)->" + objException.Message);
         }

         return sqlDataSet;

      }
	}
}
