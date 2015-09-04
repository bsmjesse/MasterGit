using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfFleetEmails table
	/// </summary>
	public class FleetEmails : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public FleetEmails(SQLExecuter sqlExec) : base ("vlfFleetEmails",sqlExec)
		{
		}
		/// <summary>
		/// Add email to fleet.
		/// </summary>
		/// <param name="fleetId"></param>
		/// <param name="email"></param>
		/// <param name="timeZone"></param>
		/// <param name="dayLightSaving"></param>
		/// <param name="formatType"></param>
		/// <param name="notify"></param>
		/// <param name="warning"></param>
		/// <param name="critical"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if email alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddEmail(int fleetId, string email, string phone, short timeZone, short dayLightSaving,
                            short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance)
		{
			int rowsAffected = 0;
			// 1. validates parameters
			if(	(fleetId == VLF.CLS.Def.Const.unassignedIntValue)||
				(email == VLF.CLS.Def.Const.unassignedStrValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: fleet Id=" + 
					fleetId + " email=" + email);
			}

			// 2. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName
                                    + " (FleetId,Email,Phone,TimeZone,DayLightSaving,FormatType,Notify,Warning,Critical,autoAdjustDayLightSaving,Maintenance)"
                                    + " VALUES ( {0}, '{1}', '{2}', {3}, {4}, {5}, {6}, {7},{8},{9},{10})",
									fleetId,
									email.Replace("'","''"),
                                    phone.Replace("'", "''"),
									timeZone,
									dayLightSaving,
									formatType,
									Convert.ToInt16(notify),
									Convert.ToInt16(warning),
									Convert.ToInt16(critical),
                                    Convert.ToInt16(autoAdjustDayLightSaving), Convert.ToInt16(Maintenance));
			try
			{
				// 4. Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add mail to fleet.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add mail to fleet.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add mail to fleet.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This email already exists.");
			}
			return rowsAffected;
		}



        /// <summary>
        /// Save Fleet Email, add DriverMessage Column
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="Maintenance"></param>
        /// <returns></returns>
        public int SaveEmail(int fleetId, string email, string phone, short timeZone, short dayLightSaving,
                           short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool DriverMessage, bool autosubscription, bool reminder)
        {
            int rowsAffected = 0;
            // 1. validates parameters
            if ((fleetId == VLF.CLS.Def.Const.unassignedIntValue) ||
                (email == VLF.CLS.Def.Const.unassignedStrValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: fleet Id=" +
                    fleetId + " email=" + email);
            }

            // 2. Prepares SQL statement
            string sql = string.Format("INSERT INTO " + tableName
                                    + " (FleetId,Email,Phone,TimeZone,DayLightSaving,FormatType,Notify,Warning,Critical,autoAdjustDayLightSaving,Maintenance,DriverMessage, autosubscription, Reminder)"
                                    + " VALUES ( {0}, '{1}', '{2}', {3}, {4}, {5}, {6}, {7},{8},{9},{10},{11},{12},{13})",
                                    fleetId,
                                    email.Replace("'", "''"),
                                    phone.Replace("'", "''"),
                                    timeZone,
                                    dayLightSaving,
                                    formatType,
                                    Convert.ToInt16(notify),
                                    Convert.ToInt16(warning),
                                    Convert.ToInt16(critical),
                                    Convert.ToInt16(autoAdjustDayLightSaving), 
                                    Convert.ToInt16(Maintenance),
                                    Convert.ToInt16(DriverMessage),
                                    Convert.ToInt16(autosubscription),
                                    Convert.ToInt16(reminder)
                                    );
            try
            {
                // 4. Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add mail to fleet.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to add mail to fleet.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to add mail to fleet.";
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This email already exists.");
            }
            return rowsAffected;
        }

		/// <summary>
		/// Update organization email.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="fleetId"></param>
		/// <param name="oldEmail"></param>
		/// <param name="newEmail"></param>
		/// <param name="timeZone"></param>
		/// <param name="dayLightSaving"></param>
		/// <param name="formatType"></param>
		/// <param name="notify"></param>
		/// <param name="warning"></param>
		/// <param name="critical"></param>
		/// <param name="autoAdjustDayLightSaving"></param>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if organization does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateEmail(int fleetId,string oldEmail,string newEmail,string phone,short timeZone,
								short dayLightSaving,short formatType,bool notify,
								bool warning,bool critical,bool autoAdjustDayLightSaving,bool Maintenance, bool autosubscription, bool reminder)
		{
			// 1. validates parameters
            //if(	(fleetId == VLF.CLS.Def.Const.unassignedIntValue)||
            //    (oldEmail == VLF.CLS.Def.Const.unassignedStrValue)||
            //    (newEmail == VLF.CLS.Def.Const.unassignedStrValue))
            //{
            //    throw new DASAppInvalidValueException("Wrong value for insert SQL: FleetId=" +
            //        fleetId + " oldEmail=" + newEmail + " newEmail=" + newEmail);
            //}

            if (fleetId == VLF.CLS.Def.Const.unassignedIntValue)
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: FleetId=" +
                    fleetId + " oldEmail=" + newEmail + " newEmail=" + newEmail);
            }

			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName + 
				" SET Email='" + newEmail.Replace("'","''") + "'" + 
                ", Phone='" + phone.Replace("'","''") + "'" + 
				",TimeZone=" + timeZone +
				", DayLightSaving=" + dayLightSaving +
				", FormatType=" + formatType +
				", Notify=" + Convert.ToInt16(notify) + 
				", Warning=" + Convert.ToInt16(warning) + 
				", Critical=" + Convert.ToInt16(critical) +
				", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
                ", Maintenance=" + Convert.ToInt16(Maintenance) +
                ", autosubscription=" + Convert.ToInt16(autosubscription) +
                ", Reminder=" + Convert.ToInt16(reminder) +
				" WHERE FleetId=" + fleetId + 
				" AND Email='" + oldEmail.Replace("'","''") + "'";
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update old email=" + oldEmail + " for fleet id " + fleetId + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update old email=" + oldEmail + " for fleet id " + fleetId + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update old email=" + oldEmail + " for fleet id " + fleetId + ".";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
			}
		}



        /// <summary>
        /// DriverMessage Column Added
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        /// <param name="phone"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="Maintenance"></param>

        public void ModifyEmail(int fleetId, string oldEmail, string newEmail, string phone, short timeZone,
                                short dayLightSaving, short formatType, bool notify,
                                bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool DriverMessage, bool autosubscription, bool reminder)
        {
            // 1. validates parameters
            //if(	(fleetId == VLF.CLS.Def.Const.unassignedIntValue)||
            //    (oldEmail == VLF.CLS.Def.Const.unassignedStrValue)||
            //    (newEmail == VLF.CLS.Def.Const.unassignedStrValue))
            //{
            //    throw new DASAppInvalidValueException("Wrong value for insert SQL: FleetId=" +
            //        fleetId + " oldEmail=" + newEmail + " newEmail=" + newEmail);
            //}

            if (fleetId == VLF.CLS.Def.Const.unassignedIntValue)
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: FleetId=" +
                    fleetId + " oldEmail=" + newEmail + " newEmail=" + newEmail);
            }

            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE " + tableName +
                " SET Email='" + newEmail.Replace("'", "''") + "'" +
                ", Phone='" + phone.Replace("'", "''") + "'" +
                ",TimeZone=" + timeZone +
                ", DayLightSaving=" + dayLightSaving +
                ", FormatType=" + formatType +
                ", Notify=" + Convert.ToInt16(notify) +
                ", Warning=" + Convert.ToInt16(warning) +
                ", Critical=" + Convert.ToInt16(critical) +
                ", AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
                ", Maintenance=" + Convert.ToInt16(Maintenance) +
                ", DriverMessage=" + Convert.ToInt16(DriverMessage) +
                ", autosubscription=" + Convert.ToInt16(autosubscription) +
                ", Reminder=" + Convert.ToInt16(reminder) +
                " WHERE FleetId=" + fleetId +
                " AND Email='" + oldEmail.Replace("'", "''") + "'";
            try
            {
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update old email=" + oldEmail + " for fleet id " + fleetId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update old email=" + oldEmail + " for fleet id " + fleetId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to update old email=" + oldEmail + " for fleet id " + fleetId + ".";
                throw new DASAppViolatedIntegrityConstraintsException(prefixMsg);
            }
        }

		/// <summary>
		/// Set DayLight Savings.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="dayLightSaving"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDayLightSaving(bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfFleetEmails SET DayLightSaving=" + Convert.ToInt16(dayLightSaving) + " WHERE AutoAdjustDayLightSaving=1";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update dayLightSaving=" + dayLightSaving.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update dayLightSaving=" + dayLightSaving.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
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
		public void SetAutoAdjustDayLightSaving(int fleetId,string email,bool autoAdjustDayLightSaving,bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfFleetEmails SET AutoAdjustDayLightSaving=" + Convert.ToInt16(autoAdjustDayLightSaving) +
				" ,DayLightSaving=" + Convert.ToInt16(dayLightSaving) + 
				" WHERE FleetId=" + fleetId + " AND Email='" + email.Replace("'","''") + "'";
			try
			{
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update autoAdjustDayLightSaving=" + autoAdjustDayLightSaving.ToString() + " dayLightSaving=" + dayLightSaving.ToString() + " fleetId=" + fleetId.ToString() + " email=" + email.ToString() + ".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update autoAdjustDayLightSaving=" + autoAdjustDayLightSaving.ToString() + " dayLightSaving=" + dayLightSaving.ToString() + " fleetId=" + fleetId.ToString() + " email=" + email.ToString() + ".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
		}

		/// <summary>
		/// Delete existing email from fleet.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="fleetId"></param> 
		/// <param name="email"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if fleetId does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteEmailFromFleet(int fleetId,string email)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + 
				" WHERE FleetId=" + fleetId +
				" AND Email='" + email.Replace("'","''") + "'";
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
				string prefixMsg = "Unable to delete existing email=" + email + " from fleet=" + fleetId;
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete existing email=" + email + " from fleet=" + fleetId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Delete all emails from fleet.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="fleetId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if fleet id does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteAllEmailsFromFleet(int fleetId)
		{
			return DeleteRowsByIntField("FleetId",fleetId, "fleet id");		
		}
		/// <summary>
		/// Retrieves fleet emails
		/// </summary>
		/// <returns>DataSet [FleetId],[FleetMame],[Email],[TimeZone],
		/// [DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]</returns>
		/// <param name="fleetId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetFleetEmails(int fleetId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfFleet.FleetId,FleetName,Email,ISNULL(Phone,'') as Phone,TimeZone,"+
							"CASE WHEN DayLightSaving=1 then 'true' ELSE 'false' END AS DayLightSaving,"+
							"FormatType,"+
							"CASE WHEN Notify=1 then 'true' ELSE 'false' END AS Notify,"+
							"CASE WHEN Warning=1 then 'true' ELSE 'false' END AS Warning,"+
                            "CASE WHEN Critical=1 then 'true' ELSE 'false' END AS Critical," +
                            "CASE WHEN DriverMessage=1 then 'true' ELSE 'false' END AS DriverMessage," +
							"CASE WHEN AutoAdjustDayLightSaving=1 then 'true' ELSE 'false' END AS AutoAdjustDayLightSaving,"+
                            "CASE WHEN ISNULL(Maintenance,0)=1 then 'true' ELSE 'false' END AS Maintenance," +
                            "CASE WHEN ISNULL(autosubscription,0)=1 then 'true' ELSE 'false' END AS autosubscription," +
                            "CASE WHEN ISNULL(Reminder,0)=1 then 'true' ELSE 'false' END AS Reminder" +
					" FROM vlfFleet," + tableName +
					" WHERE " + tableName + ".FleetId=" + fleetId +
					" AND vlfFleet.FleetId=" + tableName + ".FleetId ORDER BY Email";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve email for fleet=" + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve email for fleet=" + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}


        /// <summary>
        ///         returns the information needed to send emails to the fleet email addresses
        ///         for messages with a certain severity
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="alarmSeverity"></param>
        /// <returns>
        ///   [Email][FleetId][Phone][TimeZone][DayLightSaving][FormatType]
        ///   [Notify][Warning][Critical][AutoAdjustDayLightSaving]
        /// </returns>
        /// <comment>
        ///      for alarmSeverity = -1
        ///      it returns all fleet email info for any alarmSeverity > 0 (1|2|3)
        /// </comment>
        public DataSet GetFleetInfo4AlarmSeverity(int boxId, int alarmSeverity)
        {
            string prefixMsg = string.Format("Unable to retrieve fleet Info for boxId={0} and alarmSeverity={1}",
                           boxId, alarmSeverity);

            DataSet resultDataSet = null;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
                sqlExec.AddCommandParam("@alarmSeverity", SqlDbType.Int, alarmSeverity);
                resultDataSet = sqlExec.SPExecuteDataset("GetFleetInfo4AlarmSeverity");
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
            return resultDataSet;
        }

	}
}
