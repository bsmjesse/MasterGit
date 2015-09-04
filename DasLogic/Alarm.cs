using System;
using VLF.ERR;
using VLF.CLS.Def;
using VLF.DAS.DB;
using System.Data;			// for DataSet

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to alarms functionality in database
	/// </summary>
	public class Alarm : Das
	{
		private VLF.DAS.DB.Alarm alarm = null;
		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public Alarm(string connectionString) : base (connectionString)
		{
			alarm = new VLF.DAS.DB.Alarm(sqlExec);
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
		/// Add new alarm.
		/// </summary>
		/// <param name="dateTimeCreated"></param>
		/// <param name="boxId"></param>
		/// <param name="alarmTypeName"></param>
		/// <param name="alarmLevel"></param>
		/// <param name="description"></param>
		/// <returns>int new alarm id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if alarm id and datetime alredy exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int AddAlarm(DateTime dateTimeCreated,int boxId,string alarmTypeName,int alarmLevel,string description)
		{
			return alarm.AddAlarm(dateTimeCreated,boxId,alarmTypeName,alarmLevel,description);
		}

      /// <summary>
      ///      the next step for generating an alarm
      /// </summary>
      /// <param name="cmfIn"></param>
      /// <param name="alarmLevel"></param>
      /// <param name="description"></param>
      /// <returns></returns>
      public int AddAlarm(CMFIn cmfIn, int alarmLevel, string description)
      {
         return alarm.AddAlarm(cmfIn, alarmLevel, description);
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
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
/*       
      public long AddAlarmIdentity(DateTime dateTimeCreated, int boxId, string alarmTypeName, int alarmLevel, string description)
      {
         return alarm.AddAlarmIdentity(dateTimeCreated, boxId, alarmTypeName, alarmLevel, description);
      }
*/      
      /// <summary>
		/// Delete existing alarm by alarm id.
		/// </summary>
		/// <remarks>
		/// 1. boxId == VLF.CLS.Def.Const.unassignedIntValue (deletes for all boxes)
		/// 2. from == VLF.CLS.Def.Const.unassignedDateTime ("from" date time is empty)
		/// 3. to == VLF.CLS.Def.Const.unassignedDateTime ("to" date time is empty)
		/// </remarks>
		/// <returns>void</returns>
		/// <param name="alarmId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeleteAlarmByAlarmId(int alarmId)
		{
			return alarm.DeleteAlarmByAlarmId(alarmId);
		}
		/// <summary>
		/// Deletes all alarms in the date time range.
		/// </summary>
		/// <param name="boxId"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public int DeleteAlarmsByDateTimeRange(int boxId,DateTime from,DateTime to)
		{
			return alarm.DeleteAlarmsByDateTimeRange(boxId,from,to);
		}
		/// <summary>
		/// Retrieves alarm info by alarm id 
		/// </summary>
		/// <returns>
		/// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
		///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
		///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription],
		///			[UserName],[Latitude],[Longitude],[Speed],[Heading],
		///			[SensorMask],[IsArmed],[BoxId]
		/// </returns>
		/// <param name="alarmId"></param>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAlarmInfoByAlarmId(int alarmId)
		{
			DataSet dsResult = alarm.GetAlarmInfoByAlarmId(alarmId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AlarmInfo" ;
				}
				dsResult.DataSetName = "Alarm";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves all alarms info
		/// </summary>
		/// <returns>
		/// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
		///			[DateTimeAck],[UserId],[DateTimeClosed],[Description],
		///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAllAlarmsInfo()
		{
			DataSet dsResult = alarm.GetAllAlarmsInfo(	VLF.CLS.Def.Const.unassignedDateTime,
														VLF.CLS.Def.Const.unassignedDateTime,
														VLF.CLS.Def.Const.unassignedIntValue);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllAlarmsInfo" ;
				}
				dsResult.DataSetName = "Alarm";
			}
			return dsResult;
		}
		
		/// <summary>
		/// Update alarm level.
		/// </summary>
		/// <param name="alarmId"></param>
		/// <param name="alarmLevel"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void SetAlarmLevel(int alarmId,int alarmLevel)
		{
			alarm.SetAlarmLevel(alarmId,alarmLevel);
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
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AcceptAlarm(int alarmId,DateTime acceptedDateTime,int userId)
		{
			alarm.AcceptAlarm(alarmId,acceptedDateTime,userId);
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
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void AcceptAlarm(int alarmId, DateTime acceptedDateTime, int userId,string notes)
        {
            alarm.AcceptAlarm(alarmId, acceptedDateTime, userId,notes);
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
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void CloseAlarm(int alarmId,DateTime closedDateTime,int userId)
		{
			alarm.CloseAlarm(alarmId,closedDateTime,userId);
		}

        /// <summary>
        /// Set alarm closed date time and extra information or notes for this closed alarm.
        /// </summary>
        /// <param name="alarmId"></param>
        /// <param name="closedDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="notes"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void CloseAlarm(int alarmId, DateTime closedDateTime, int userId, string notes)
        {
            alarm.CloseAlarm(alarmId, closedDateTime, userId, notes);
        }

		/// <summary>
		/// Retrieves all box alarms info by filter datetime
		/// </summary>
		/// <remarks>
		/// 1. boxId == VLF.CLS.Def.Const.unassignedIntValue (Get all the boxes)
		/// 2. from == VLF.CLS.Def.Const.unassignedDateTime (Does not filter by "from" date time)
		/// 3. to == VLF.CLS.Def.Const.unassignedDateTime (Does not filter by "to" date time)
		/// </remarks>
		/// <param name="from"></param> 
		/// <param name="to"></param> 
		/// <param name="requestUserId"></param> 
		/// <returns>
		/// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
		///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
		///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription]
		/// </returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetAllAlarmsInfo(DateTime from,DateTime to,int requestUserId)
		{
			DataSet dsResult = alarm.GetAllAlarmsInfo(from,to,requestUserId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllUserAlarmsInfo" ;
				}
				dsResult.DataSetName = "Alarm";
			}
			return dsResult;
		}

        // Changes for TimeZone Feature start

        /// <summary>
        /// GetAlarmsShortInfoCheckSum
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="requestUserId"></param>
        /// <returns></returns>
        public string GetAlarmsShortInfoCheckSum_NewTZ(DateTime from, DateTime to, int requestUserId)
        {
            return alarm.GetAlarmsShortInfoCheckSum_NewTZ(from, to, requestUserId);
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// GetAlarmsShortInfoCheckSum
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="requestUserId"></param>
        /// <returns></returns>
        public string GetAlarmsShortInfoCheckSum(DateTime from, DateTime to, int requestUserId)
        {
            return   alarm.GetAlarmsShortInfoCheckSum(from, to, requestUserId);
        }

        // Changes for TimeZone Feature start

        /// <summary>
        /// Retrieves all box alarms info by filter datetime
        /// </summary>
        /// <remarks>
        /// 1. boxId == VLF.CLS.Def.Const.unassignedIntValue (Get all the boxes)
        /// 2. from == VLF.CLS.Def.Const.unassignedDateTime (Does not filter by "from" date time)
        /// 3. to == VLF.CLS.Def.Const.unassignedDateTime (Does not filter by "to" date time)
        /// </remarks>
        /// <param name="from"></param> 
        /// <param name="to"></param> 
        /// <param name="requestUserId"></param> 
        /// <returns>
        /// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
        ///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
        ///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetAlarmsShortInfo_NewTZ(DateTime from, DateTime to, int requestUserId)
        {
            DataSet dsResult = alarm.GetAlarmsShortInfo_NewTZ(from, to, requestUserId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "AllUserAlarmsInfo";
                }
                dsResult.DataSetName = "Alarm";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves all box alarms info by filter datetime
        /// </summary>
        /// <remarks>
        /// 1. boxId == VLF.CLS.Def.Const.unassignedIntValue (Get all the boxes)
        /// 2. from == VLF.CLS.Def.Const.unassignedDateTime (Does not filter by "from" date time)
        /// 3. to == VLF.CLS.Def.Const.unassignedDateTime (Does not filter by "to" date time)
        /// </remarks>
        /// <param name="from"></param> 
        /// <param name="to"></param> 
        /// <param name="requestUserId"></param> 
        /// <returns>
        /// DataSet [AlarmId],[DateTimeCreated],[BoxId],[AlarmTypeName],[AlarmLevel],
        ///			[DateTimeAck],[UserId],[DateTimeClosed],[AlarmDescription],
        ///			[LicensePlate],[VehicleId],[StreetAddress],[ValidGPS],[vehicleDescription]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetAlarmsShortInfo(DateTime from, DateTime to, int requestUserId)
        {
            DataSet dsResult = alarm.GetAlarmsShortInfo(from, to, requestUserId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "AllUserAlarmsInfo";
                }
                dsResult.DataSetName = "Alarm";
            }
            return dsResult;
        }

		#endregion
	}
}
