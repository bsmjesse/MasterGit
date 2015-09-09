using System;
using System.Data.SqlClient ;	// for SqlException
using System.Data ;			// for DataSet
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interface to vlfAlarm table
	/// </summary>
	public class Alarm: TblGenInterfaces
	{
		#region Public Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Alarm(SQLExecuter sqlExec) : base ("vlfAlarm", sqlExec)
		{
		}
		/// <summary>
		/// Add new alarm.
		/// </summary>
		/// <param name="dateTimeCreated"></param>
		/// <param name="boxId"></param>
		/// <param name="alarmTypeName"></param>
		/// <param name="alarmLevel"></param>
		/// <param name="description"></param>
		/// <returns>int new alarm id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if alarm id and datetime alredy exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddAlarm(DateTime dateTimeCreated,int boxId,string alarmTypeName,int alarmLevel,string description)
		{
         string prefixMsg = "Unable to add new alarm with DateTimeCreated '" + dateTimeCreated +
            ", BoxId=" + boxId + ", AlarmTypeName '" + alarmTypeName + "'.";
         // 1. Get next availible alarm index
         int alarmId = CLS.Def.Const.unassignedIntValue;

#if NEW_ALARMS
         try
         {
            // 2. Prepares SQL statement
            // Set SQL command
            string sql = "AlarmAdd"; // st. proc.
               //String.Format("INSERT INTO vlfAlarm(DateTimeCreated, BoxId, AlarmType, AlarmLevel, Description)" +
               //" VALUES (@DateTimeCreated, @BoxId, @AlarmType, @AlarmLevel, @Description)" +
               //" SELECT IDENT_CURRENT('{0}')", this.tableName);
            // Add parameters to SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@dtCreated", SqlDbType.DateTime, Convert.ToDateTime(dateTimeCreated));
            sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
            //sqlExec.AddCommandParam("@alarmType",SqlDbType.Char,alarmTypeName.Replace("'","''"));
            sqlExec.AddCommandParam("@alarmType", SqlDbType.VarChar, alarmTypeName);
            sqlExec.AddCommandParam("@alarmLevel", SqlDbType.Int, alarmLevel);
            if (description == null)
               sqlExec.AddCommandParam("@description", SqlDbType.VarChar, System.DBNull.Value);
            else
               //sqlExec.AddCommandParam("@Description",SqlDbType.Char,description.Replace("'","''"));
               sqlExec.AddCommandParam("@description", SqlDbType.VarChar, description);

            // 3. Executes SQL statement
            object result = sqlExec.SPExecuteScalar(sql);
            if (result != null)
               alarmId = Convert.ToInt32(result);
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
         return alarmId;
#else
  			int rowsAffected = 0;
         try
         {
            alarmId = GetMaxAlarmIndex() + 1;
            // 2. Prepares SQL statement
            // Set SQL command
            string sql = "INSERT INTO vlfAlarm(AlarmId,DateTimeCreated,BoxId,AlarmType,AlarmLevel,Description) VALUES (@AlarmId,@DateTimeCreated,@BoxId,@AlarmType,@AlarmLevel,@Description)";
            // Add parameters to SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@AlarmId", SqlDbType.Int, alarmId);
            sqlExec.AddCommandParam("@DateTimeCreated", SqlDbType.DateTime, Convert.ToDateTime(dateTimeCreated));
            sqlExec.AddCommandParam("@BoxId", SqlDbType.Int, boxId);
            //sqlExec.AddCommandParam("@AlarmType",SqlDbType.Char,alarmTypeName.Replace("'","''"));
            sqlExec.AddCommandParam("@AlarmType", SqlDbType.Char, alarmTypeName);
            sqlExec.AddCommandParam("@AlarmLevel", SqlDbType.Int, alarmLevel);
            if (description == null)
               sqlExec.AddCommandParam("@Description", SqlDbType.Char, System.DBNull.Value);
            else
               //sqlExec.AddCommandParam("@Description",SqlDbType.Char,description.Replace("'","''"));
               sqlExec.AddCommandParam("@Description", SqlDbType.Char, description);

            // 3. Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
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
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This alarm already exists.");
			}
			return alarmId;
#endif
      }

      /// <summary>
      ///         the extra procedures will allow to save the latitude and longitude as 
      ///         well as other information available at the time
      /// </summary>
      /// <param name="cmfIn"></param>
      /// <param name="alarmLevel"></param>
      /// <param name="description"></param>
      /// <returns></returns>
      public int AddAlarm(CMFIn cmfIn, int alarmLevel, string description)
      {
         int alarmId = CLS.Def.Const.unassignedIntValue;

         if (null != cmfIn)
         {
            string prefixMsg = "Unable to add new alarm with DateTimeCreated '" + cmfIn.originatedDateTime +
               ", BoxId=" + cmfIn.boxID + ", AlarmTypeName '" + cmfIn.messageTypeID.ToString() + "'.";
            // 1. Get next availible alarm index


            try
            {
               // 2. Prepares SQL statement
               // Set SQL command
               string sql = "AlarmAddNew"; // st. proc.
               //String.Format("INSERT INTO vlfAlarm(DateTimeCreated, BoxId, AlarmType, AlarmLevel, Description)" +
               //" VALUES (@DateTimeCreated, @BoxId, @AlarmType, @AlarmLevel, @Description)" +
               //" SELECT IDENT_CURRENT('{0}')", this.tableName);
               // Add parameters to SQL statement
               sqlExec.ClearCommandParameters();
               sqlExec.AddCommandParam("@dtCreated", SqlDbType.DateTime, Convert.ToDateTime(cmfIn.originatedDateTime));
               sqlExec.AddCommandParam("@boxId", SqlDbType.Int, cmfIn.boxID);
               sqlExec.AddCommandParam("@latitude", SqlDbType.Float, cmfIn.latitude);
               sqlExec.AddCommandParam("@longitude", SqlDbType.Float, cmfIn.longitude);
               //sqlExec.AddCommandParam("@alarmType",SqlDbType.Char,alarmTypeName.Replace("'","''"));
               sqlExec.AddCommandParam("@alarmType", SqlDbType.VarChar, cmfIn.messageTypeID.ToString());
               sqlExec.AddCommandParam("@alarmLevel", SqlDbType.Int, alarmLevel);
               if (description == null)
                  sqlExec.AddCommandParam("@description", SqlDbType.VarChar, System.DBNull.Value);
               else
                  //sqlExec.AddCommandParam("@Description",SqlDbType.Char,description.Replace("'","''"));
                  sqlExec.AddCommandParam("@description", SqlDbType.VarChar, description);

               // 3. Executes SQL statement
               object result = sqlExec.SPExecuteScalar(sql);
               if (result != null)
                  alarmId = Convert.ToInt32(result);
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
         }
         return alarmId;
      }

#if NEW_ALARMS
#else
      /// <summary>
      /// Add new alarm with autoincrement id column
      /// </summary>
      /// <param name="dateTimeCreated"></param>
      /// <param name="boxId"></param>
      /// <param name="alarmTypeName"></param>
      /// <param name="alarmLevel"></param>
      /// <param name="description"></param>
      /// <returns>long new alarm id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if alarm id and datetime alredy exists.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public long AddAlarmIdentity(DateTime dateTimeCreated, int boxId, string alarmTypeName, int alarmLevel, string description)
      {
         object result = null;
         string prefixMsg = "Unable to add new alarm with DateTimeCreated '" + dateTimeCreated +
            ", BoxId=" + boxId + ", AlarmTypeName '" + alarmTypeName + "'.";
         // 1. Get next availible alarm index
         long alarmId = CLS.Def.Const.unassignedIntValue;
         try
         {
            // 2. Prepares SQL statement
            // Set SQL command
            string sql = "AlarmAdd";
               //String.Format("INSERT INTO vlfAlarm(DateTimeCreated, BoxId, AlarmType, AlarmLevel, Description)" +
               //" VALUES (@DateTimeCreated, @BoxId, @AlarmType, @AlarmLevel, @Description)" +
               //" SELECT IDENT_CURRENT('{0}')", this.tableName);
            // Add parameters to SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@dtCreated", SqlDbType.DateTime, Convert.ToDateTime(dateTimeCreated));
            sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
            //sqlExec.AddCommandParam("@AlarmType",SqlDbType.Char,alarmTypeName.Replace("'","''"));
            sqlExec.AddCommandParam("@alarmType", SqlDbType.VarChar, alarmTypeName);
            sqlExec.AddCommandParam("@alarmLevel", SqlDbType.Int, alarmLevel);
            if (description == null)
               sqlExec.AddCommandParam("@description", SqlDbType.VarChar, System.DBNull.Value);
            else
               //sqlExec.AddCommandParam("@Description",SqlDbType.Char,description.Replace("'","''"));
               sqlExec.AddCommandParam("@description", SqlDbType.VarChar, description);

            // 3. Executes SQL statement
            result = sqlExec.SPExecuteScalar(sql);
            if (result != null)
               alarmId = Convert.ToInt64(result);
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
         return alarmId;
      }
#endif
		/// <summary>
		/// Delete existing alarm by alarm id.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="alarmId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeleteAlarmByAlarmId(int alarmId)
		{
			return DeleteRowsByIntField("AlarmId",alarmId, "alarm id");		
		}
		/// <summary>
		/// Delete all user alarms.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="userId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllUserAlarms(int userId)
		{
			return DeleteRowsByIntField("UserId",userId, "user id");		
		}
		/// <summary>
		/// Delete all alarms related to the box.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="boxId"></param> 
		/// <param name="where"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteBoxAllAlarms(int boxId,string where)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "DELETE FROM vlfAlarm WHERE BoxId=" + boxId + where;
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
				string prefixMsg = "Unable to delete all box " + boxId + " alarms.";
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete all box " + boxId + " alarms.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		
		/// <summary>
		/// Deletes all alarms in the date time range.
		/// </summary>
		/// <remarks>
		/// 1. boxId == VLF.CLS.Def.Const.unassignedIntValue (deletes for all boxes)
		/// 2. from == VLF.CLS.Def.Const.unassignedDateTime ("from" date time is empty)
		/// 3. to == VLF.CLS.Def.Const.unassignedDateTime ("to" date time is empty)
		/// </remarks>
		/// <param name="boxId"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAlarmsByDateTimeRange(int boxId,DateTime from,DateTime to)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName;
			// msgs retaled to the box
			if(boxId != VLF.CLS.Def.Const.unassignedIntValue)
			{
				sql += " WHERE BoxId=" + boxId;
				if(from != VLF.CLS.Def.Const.unassignedDateTime)
				{
					sql += " AND DateTimeCreated>='" + from + "'";
					if(to != VLF.CLS.Def.Const.unassignedDateTime)
						sql += " AND DateTimeCreated<='" + to + "'";
				}
				else if(to != VLF.CLS.Def.Const.unassignedDateTime)
					sql += " AND DateTimeCreated<='" + to + "'";

			}
    		// all msgs more then "from" less then "to"
			else if(from != VLF.CLS.Def.Const.unassignedDateTime)
			{
				sql += " WHERE DateTimeCreated>='" + from + "'";
				if(to != VLF.CLS.Def.Const.unassignedDateTime)
					sql += " AND DateTimeCreated<='" + to + "'";
			}
			// all msgs less then "to"
			else if(to != VLF.CLS.Def.Const.unassignedDateTime)
				sql += " WHERE DateTimeCreated<='" + to + "'";


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
				string prefixMsg = "Unable to delete by BoxId=" + boxId + " from " + from + " to " + to;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete by BoxId=" + boxId + " from " + from + " to " + to;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves alarm info by alarm id 
		/// </summary>
		/// <returns>
		/// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
		///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
		///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription],
		///			[UserName]
		/// </returns>
		/// <param name="alarmId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAlarmInfoByAlarmId(int alarmId)
		{
			return GetAlarmInfoBy("vlfAlarm.AlarmId",alarmId);
		}

		/// <summary>
		/// Retrieves all box alarms info
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="requestUserId"></param>
		/// <remarks>
		/// 1. boxId == VLF.CLS.Def.Const.unassignedIntValue (does not filter by box id)
		/// 2. from == VLF.CLS.Def.Const.unassignedDateTime ("from" date time is empty)
		/// 3. to == VLF.CLS.Def.Const.unassignedDateTime ("to" date time is empty)
		/// Note:	if information in the history does not exist,
		///			return StreetAddress='Address resolution in progress'
		/// </remarks>
		/// <returns>
		/// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
		///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
		///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllAlarmsInfo(DateTime from,DateTime to,int requestUserId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				// 1. Prepares SQL statement
                //string sql = "DECLARE @ResolveLandmark int SELECT @ResolveLandmark=convert(real,PreferenceValue) FROM vlfUserPreference WHERE UserId=" + requestUserId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.ResolveLandmark + " IF @ResolveLandmark IS NULL SET @ResolveLandmark=0 SELECT DISTINCT vlfAlarm.AlarmId,vlfAlarm.DateTimeCreated,vlfAlarm.BoxId," +
                //    "vlfAlarm.AlarmType,vlfAlarm.AlarmLevel,vlfAlarm.DateTimeAck,"+
                //    "vlfAlarm.DateTimeClosed,vlfAlarm.UserId,"+
                //    "ISNULL(vlfAlarm.[Description],' ') AS AlarmDescription,"+
                //    "ISNULL(vlfVehicleAssignment.LicensePlate,' ') AS LicensePlate,"+
                //    "ISNULL(vlfVehicleAssignment.VehicleId,-1) AS VehicleId,"+
                //    "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +

                //    "ISNULL(vlfMsgInHst.ValidGPS,1) AS ValidGPS,"+
                //    "ISNULL(vlfVehicleInfo.Description,' ') as vehicleDescription"+
                //    " FROM vlfAlarm INNER JOIN vlfVehicleAssignment ON vlfAlarm.BoxId = vlfVehicleAssignment.BoxId"+
                //    " INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId"+
                //    " INNER JOIN vlfFleet ON vlfFleetVehicles.FleetId = vlfFleet.FleetId"+
                //    " INNER JOIN vlfFleetUsers ON vlfFleet.FleetId = vlfFleetUsers.FleetId"+
                //    " INNER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId"+
                //    " LEFT OUTER JOIN vlfMsgInHst ON vlfAlarm.BoxId = vlfMsgInHst.BoxId AND vlfAlarm.DateTimeCreated = vlfMsgInHst.OriginDateTime";
                //// msgs retaled to the box
                //if(requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                //{
                //    sql += " WHERE vlfFleetUsers.UserId=" + requestUserId;
                //    if(from != VLF.CLS.Def.Const.unassignedDateTime)
                //    {
                //        sql += " AND DateTimeCreated>='" + from + "'";
                //        if(to != VLF.CLS.Def.Const.unassignedDateTime)
                //            sql += " AND DateTimeCreated<='" + to + "'";
                //    }
                //    else if(to != VLF.CLS.Def.Const.unassignedDateTime)
                //        sql += " AND DateTimeCreated<='" + to + "'";

                //}
                //    // all msgs more then "from" less then "to"
                //else if(from != VLF.CLS.Def.Const.unassignedDateTime)
                //{
                //    sql += " WHERE DateTimeCreated>='" + from + "'";
                //    if(to != VLF.CLS.Def.Const.unassignedDateTime)
                //        sql += " AND DateTimeCreated<='" + to + "'";
                //}
                //    // all msgs less then "to"
                //else if(to != VLF.CLS.Def.Const.unassignedDateTime)
                //    sql += " WHERE DateTimeCreated<='" + to + "'";

                //sql += " ORDER BY DateTimeCreated DESC";
                ////Executes SQL statement
               sqlExec.ClearCommandParameters(); 
               string  sql = "sp_GetAllAlarmsInfo";
                

                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, Convert.ToDateTime(from));
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, Convert.ToDateTime(from));

                
                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, Convert.ToDateTime(to));
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, Convert.ToDateTime(to));

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        // Changes for TimeZone Feature start

        public DataSet GetAlarmsShortInfo_NewTZ(DateTime from, DateTime to, int requestUserId)
        {
            DataSet sqlDataSet = null;
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetAlarmsShortInfo2_NewTimeZone";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature end


        public DataSet GetAlarmsShortInfo(DateTime from, DateTime to, int requestUserId)
        {
            DataSet sqlDataSet = null;
            try
            {
        
                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetAlarmsShortInfo2";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        // Changes for TimeZone Feature start

        public string GetAlarmsShortInfoCheckSum_NewTZ(DateTime from, DateTime to, int requestUserId)
        {
            string checkSum = "";
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetAlarmsShortInfoCheckSum_NewTimeZone";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                object obj = sqlExec.SPExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    checkSum = Convert.ToString(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return checkSum;
        }

        // Changes for TimeZone Feature end

        public string GetAlarmsShortInfoCheckSum(DateTime from, DateTime to, int requestUserId)
        {
            string checkSum = "";
            try
            {

                ////Executes SQL statement
                sqlExec.ClearCommandParameters();
                string sql = "sp_GetAlarmsShortInfoCheckSum";


                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);


                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                object obj = sqlExec.SPExecuteScalar(sql);
                if (obj != System.DBNull.Value)
                    checkSum = Convert.ToString(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve checksum from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return checkSum;
        }

      /// <summary>
      ///      this function is used to store all new changes in memory and feed up all clients
      ///      asking for NEW/CHANGES in alarms
      ///      this is based on a trigger function
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="requestUserId"></param>
      /// <returns></returns>
      public DataSet GetAllNewAlarmsInfo(DateTime from, DateTime to, int requestUserId)
      {
         DataSet sqlDataSet = null;
         try
         {
               sqlExec.ClearCommandParameters();
               string sql = "GetNewAlarmsInfo";
                

                if (requestUserId != VLF.CLS.Def.Const.unassignedIntValue)
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, requestUserId);
                else
                    sqlExec.AddCommandParam("@UserId", SqlDbType.Int, VLF.CLS.Def.Const.unassignedIntValue);

                if (from != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@From", SqlDbType.DateTime, from);
                else
                    sqlExec.AddCommandParam("1/1/1999", SqlDbType.DateTime, to);

                
                if (to != VLF.CLS.Def.Const.unassignedDateTime)
                    sqlExec.AddCommandParam("@To", SqlDbType.DateTime, to);
                else
                    sqlExec.AddCommandParam("1/1/2099", SqlDbType.DateTime, to);

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info from=" + from + " to=" + to + " for requestUserId=" + requestUserId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
      }
		/// <summary>
		/// retrieves max record index from specific table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetMaxAlarmIndex() 
		{
			int maxRecordIndex = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT MAX(AlarmId) FROM " + tableName;
				if(sqlExec.RequiredTransaction())
				{
					// Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				//Executes SQL statement
				object result = sqlExec.SQLExecuteScalar(sql);
				if(!Convert.IsDBNull(result))
					maxRecordIndex = (int)result;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve max record index from '" + tableName + "' table.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve max record index from '" + tableName + "' table.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}				
			return maxRecordIndex;
		}

		/// <summary>
		/// Update alarm level.
		/// </summary>
		/// <param name="alarmId"></param>
		/// <param name="alarmLevel"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetAlarmLevel(int alarmId,int alarmLevel)
		{
			// 1. validates parameters
			if(	(alarmId == VLF.CLS.Def.Const.unassignedIntValue)||
				(alarmLevel == VLF.CLS.Def.Const.unassignedIntValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: Alarm Id=" +
														alarmId + " Alarm Level=" + alarmLevel);
			}
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + " SET AlarmLevel=" + alarmLevel +
						" WHERE AlarmId=" + alarmId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " alarm level" + alarmLevel + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " alarm level" + alarmLevel + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + "alarm level" + alarmLevel + " .";
				throw new DASAppResultNotFoundException(prefixMsg + " This alarm does not exist.");
			}
		}		
		
		/// <summary>
		/// Set alarm acknowledged date time
		/// </summary>
		/// <param name="alarmId"></param>
		/// <param name="acceptedDateTime"></param>
		/// <param name="userId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AcceptAlarm(int alarmId,DateTime acceptedDateTime,int userId)
		{
			// 1. validates parameters
			if(	(alarmId == VLF.CLS.Def.Const.unassignedIntValue)||
				(acceptedDateTime == VLF.CLS.Def.Const.unassignedDateTime))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: Alarm Id=" +
					alarmId + " accepted date time=" + acceptedDateTime.ToString());
			}
			int rowsAffected = 0;
			string sql = "";
			//Prepares SQL statement
			if(userId != VLF.CLS.Def.Const.unassignedIntValue)
			{
				sql =	"UPDATE " + tableName + 
					" SET DateTimeAck='" + acceptedDateTime.ToString() + "'" +
					" ,UserId=" + userId +
					" WHERE AlarmId=" + alarmId;
			}
			else
			{
				sql =	"UPDATE " + tableName + 
					"' SET DateTimeAck='" + acceptedDateTime.ToString() + "'" +
					" WHERE AlarmId=" + alarmId;
			}
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " accepted date time=" + acceptedDateTime.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " accepted date time=" + acceptedDateTime.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " accepted date time=" + acceptedDateTime.ToString() + ".";
				throw new DASAppResultNotFoundException(prefixMsg + " This alarm does not exist.");
			}
		}


        /// <summary>
        /// Set alarm acknowledged date time
        /// </summary>
        /// <param name="alarmId"></param>
        /// <param name="acceptedDateTime"></param>
        /// <param name="userId"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void AcceptAlarm(int alarmId, DateTime acceptedDateTime, int userId, string notes)
        {
            // 1. validates parameters
            if ((alarmId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (acceptedDateTime == VLF.CLS.Def.Const.unassignedDateTime))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: Alarm Id=" +
                    alarmId + " accepted date time=" + acceptedDateTime.ToString());
            }
            int rowsAffected = 0;
            string sql = "";
            //Prepares SQL statement
            if (userId != VLF.CLS.Def.Const.unassignedIntValue)
            {
                sql = "UPDATE " + tableName +
                    " SET DateTimeAck='" + acceptedDateTime.ToString() + "'" +
                    " ,UserId=" + userId +
                    " ,Notes='" + notes + "'" +
                    " WHERE AlarmId=" + alarmId;
            }
            else
            {
                sql = "UPDATE " + tableName +
                    "' SET DateTimeAck='" + acceptedDateTime.ToString() + "'" +
                    " ,Notes='" + notes + "'" +
                    " WHERE AlarmId=" + alarmId;
            }
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update alarm id " + alarmId + " accepted date time=" + acceptedDateTime.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update alarm id " + alarmId + " accepted date time=" + acceptedDateTime.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update alarm id " + alarmId + " accepted date time=" + acceptedDateTime.ToString() + ".";
                throw new DASAppResultNotFoundException(prefixMsg + " This alarm does not exist.");
            }
        }		
		
		/// <summary>
		/// Set alarm closed date time
		/// </summary>
		/// <param name="alarmId"></param>
		/// <param name="closedDateTime"></param>
		/// <param name="userId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void CloseAlarm(int alarmId,DateTime closedDateTime,int userId)
		{
			// 1. validates parameters
			if(	(alarmId == VLF.CLS.Def.Const.unassignedIntValue)||
				(closedDateTime == VLF.CLS.Def.Const.unassignedDateTime))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: Alarm Id=" +
					alarmId + " closed date time=" + closedDateTime.ToString());
			}
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "";
			if(userId != VLF.CLS.Def.Const.unassignedIntValue)
			{
				sql =	"UPDATE " + tableName + 
					" SET DateTimeClosed='" + closedDateTime.ToString() + "'" +
					" ,UserId=" + userId +
					" WHERE AlarmId=" + alarmId;
			}
			else
			{
				sql =	"UPDATE " + tableName + 
					" SET DateTimeClosed='" + closedDateTime.ToString() + "'" +
					" WHERE AlarmId=" + alarmId;
			}
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " closed date time=" + closedDateTime.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " closed date time=" + closedDateTime.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update alarm id " + alarmId + " closed date time=" + closedDateTime.ToString() + ".";
				throw new DASAppResultNotFoundException(prefixMsg + " This alarm does not exist.");
			}
		}

        /// <summary>
        /// Set alarm closed date time with notes.
        /// </summary>
        /// <param name="alarmId"></param>
        /// <param name="closedDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="notes"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void CloseAlarm(int alarmId, DateTime closedDateTime, int userId, string notes)
        {
            // 1. validates parameters
            if ((alarmId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (closedDateTime == VLF.CLS.Def.Const.unassignedDateTime))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: Alarm Id=" +
                    alarmId + " closed date time=" + closedDateTime.ToString());
            }
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "";
            if (userId != VLF.CLS.Def.Const.unassignedIntValue)
            {
                sql = "UPDATE " + tableName +
                    " SET DateTimeClosed='" + closedDateTime.ToString() + "'" +
                    " ,UserId=" + userId +
                    " ,Notes='" + notes + "'" +
                    " WHERE AlarmId=" + alarmId;
            }
            else
            {
                sql = "UPDATE " + tableName +
                    " SET DateTimeClosed='" + closedDateTime.ToString() + "'" +
                    " ,Notes='" + notes + "'" +
                    " WHERE AlarmId=" + alarmId;
            }
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update alarm id " + alarmId + " closed date time=" + closedDateTime.ToString() + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update alarm id " + alarmId + " closed date time=" + closedDateTime.ToString() + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update alarm id " + alarmId + " closed date time=" + closedDateTime.ToString() + ".";
                throw new DASAppResultNotFoundException(prefixMsg + " This alarm does not exist.");
            }
        }	
		#endregion
		
		#region Protected Interfaces
		/// <summary>
		/// retrieves alarm info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>
		/// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
		///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
		///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription],
		///			[UserName],[Latitude],[Longitude],[Speed],[Heading],
		///			[SensorMask],[IsArmed],[BoxId]
		/// </returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		protected DataSet GetAlarmInfoBy(string searchFieldName,int searchFieldValue)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int SET @ResolveLandmark=0 SELECT vlfAlarm.AlarmId,vlfAlarm.DateTimeCreated,vlfAlarm.BoxId," +
							"vlfAlarm.AlarmType,vlfAlarm.AlarmLevel,vlfAlarm.DateTimeAck,"+
							"vlfAlarm.DateTimeClosed,vlfAlarm.UserId,"+
							"ISNULL(vlfAlarm.[Description],' ') AS AlarmDescription,"+
							"ISNULL(vlfVehicleAssignment.LicensePlate,' ') AS LicensePlate,"+
							"ISNULL(vlfVehicleAssignment.VehicleId,-1) AS VehicleId,"+
                            "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +
							"ISNULL(vlfMsgInHst.ValidGPS,1) AS ValidGPS,"+
							"ISNULL(vlfVehicleInfo.Description,' ') as vehicleDescription,"+
							"ISNULL(UserName,' ') AS UserName,"+
							//"ISNULL(vlfMsgInHst.Latitude,0) AS Latitude,"+
							//"ISNULL(vlfMsgInHst.Longitude,0) AS Longitude,"+
                            "ISNULL(vlfAlarm.Lat,0) AS Latitude," +
                            "ISNULL(vlfAlarm.Long,0) AS Longitude," +
							"ISNULL(vlfMsgInHst.Speed,0) AS Speed,"+
							"ISNULL(vlfMsgInHst.Heading,0) AS Heading,"+
							"ISNULL(vlfMsgInHst.SensorMask,0) AS SensorMask,"+
							"ISNULL(vlfMsgInHst.IsArmed,0) AS IsArmed,"+
							"vlfAlarm.BoxId,"+
                            "ISNULL(vlfMsgInHst.CustomProp,' ') AS CustomProp," +
                            "ISNULL(AlarmLandmarkID,' ') AS AlarmLandmarkID" +
							" FROM vlfAlarm LEFT JOIN vlfUser ON vlfAlarm.UserId=vlfUser.UserId"+
							" LEFT JOIN vlfVehicleAssignment ON vlfAlarm.BoxId=vlfVehicleAssignment.BoxId"+
							" LEFT JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId"+
                            " LEFT JOIN vlfMsgInHst with (nolock) ON vlfAlarm.BoxId=vlfMsgInHst.BoxId" +
							" AND vlfAlarm.DateTimeCreated=vlfMsgInHst.OriginDateTime"+
							" WHERE " + searchFieldName + "=" + searchFieldValue;
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        protected DataSet GetAlarmInfoByNew(string searchFieldName, int searchFieldValue)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @ResolveLandmark int SET @ResolveLandmark=0 SELECT vlfAlarm.AlarmId,vlfAlarm.DateTimeCreated,vlfAlarm.BoxId," +
                            "vlfAlarm.AlarmType,vlfAlarm.AlarmLevel,vlfAlarm.DateTimeAck," +
                            "vlfAlarm.DateTimeClosed,vlfAlarm.UserId," +
                            "ISNULL(vlfAlarm.[Description],' ') AS AlarmDescription," +
                            "ISNULL(vlfVehicleAssignment.LicensePlate,' ') AS LicensePlate," +
                            "ISNULL(vlfVehicleAssignment.VehicleId,-1) AS VehicleId," +
                            "ISNULL(CASE WHEN @ResolveLandmark=0 then vlfMsgInHst.StreetAddress ELSE CASE WHEN vlfMsgInHst.NearestLandmark IS NULL then vlfMsgInHst.StreetAddress ELSE vlfMsgInHst.NearestLandmark END END,'" + VLF.CLS.Def.Const.addressNA + "') AS StreetAddress," +
                            "ISNULL(vlfMsgInHst.ValidGPS,1) AS ValidGPS," +
                            "ISNULL(vlfVehicleInfo.Description,' ') as vehicleDescription," +
                            "ISNULL(UserName,' ') AS UserName," +
                    //"ISNULL(vlfMsgInHst.Latitude,0) AS Latitude,"+
                    //"ISNULL(vlfMsgInHst.Longitude,0) AS Longitude,"+
                            "ISNULL(vlfAlarm.Lat,0) AS Latitude," +
                            "ISNULL(vlfAlarm.Long,0) AS Longitude," +
                            "ISNULL(vlfMsgInHst.Speed,0) AS Speed," +
                            "ISNULL(vlfMsgInHst.Heading,0) AS Heading," +
                            "ISNULL(vlfMsgInHst.SensorMask,0) AS SensorMask," +
                            "ISNULL(vlfMsgInHst.IsArmed,0) AS IsArmed," +
                            "vlfAlarm.BoxId," +
                            "ISNULL(vlfMsgInHst.CustomProp,' ') AS CustomProp," +
                            "ISNULL(AlarmLandmarkID,' ') AS AlarmLandmarkID" +
                            " FROM vlfAlarm LEFT JOIN vlfUser ON vlfAlarm.UserId=vlfUser.UserId" +
                            " LEFT JOIN vlfVehicleAssignment ON vlfAlarm.BoxId=vlfVehicleAssignment.BoxId" +
                            " LEFT JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId" +
                            " LEFT JOIN vlfMsgInHst with (nolock) ON vlfAlarm.BoxId=vlfMsgInHst.BoxId" +
                            " AND vlfAlarm.DateTimeCreated=vlfMsgInHst.OriginDateTime" +
                            " WHERE " + searchFieldName + "=" + searchFieldValue;
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by " + searchFieldName + "=" + searchFieldValue + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
		#endregion
	}
}
