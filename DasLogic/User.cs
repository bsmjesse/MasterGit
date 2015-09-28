using System;
using System.Collections ;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using System.Diagnostics;
using System.Reflection;
using VLF.CLS.Def;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to user functionality in database
	/// </summary>
	public class User : Das
	{
		DB.User user = null;
		DB.UserPreference userPreference = null;
		DB.UserLogin userLogin = null;

		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public User(string connectionString) : base (connectionString)
		{
			user = new DB.User(sqlExec);
			userPreference = new DB.UserPreference(sqlExec);
			userLogin = new DB.UserLogin(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region User Interfaces
		/// <summary>
		/// Add new user.
		/// </summary>
		/// <returns>int next user id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		/// <param name="userName"></param>
		/// <param name="userInfo"></param>
		/// <param name="pInfo"></param>
		public int AddNewUser(string userName,UserInfo userInfo,VLF.CLS.Def.Structures.PersonInfoStruct pInfo)
		{
            int userId = VLF.CLS.Def.Const.unassignedIntValue, fleetId = Const.unassignedIntValue, organizationId = Const.unassignedIntValue;
			try
			{
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new person
                DB.PersonInfo personInfo = new DB.PersonInfo(sqlExec);
                personInfo.AddPerson(ref pInfo);
                userInfo.personId = pInfo.personId;
                // 3. Add new user
                userId = user.AddUser(userName, userInfo);
                organizationId = GetOrganizationIdByUserId(userId);
                 //4. Set default user preferences
                OrganizationPreferences op = new OrganizationPreferences(sqlExec);
                DataSet ds = op.GetOrganizationPreferenceByOrganizationIdAndUserId(organizationId);
                // 4.1 set default "MeasurementUnits" to km
                bool datainserted = false;
                if (ds.Tables[0].Rows.Count > 0)
                {                   

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (Convert.ToInt16(dr["PreferenceId"]) == (int)VLF.CLS.Def.Enums.Preference.DayLightSaving)
                        {
                            userPreference.AddUserPreference(userId, (int)dr["PreferenceId"], dr["PreferenceValue"].ToString());
                            datainserted = true;
                        }
                        else
                        {
                            userPreference.AddUserPreference(userId, (int)dr["PreferenceId"], dr["PreferenceValue"].ToString());
                        }
                    }
                }
                if (!datainserted)
                {
                    userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving, Convert.ToInt16(System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)).ToString());
                }

              
               
                // 5. Save all changes
                sqlExec.CommitTransaction();

            // Get fleet ID
            /*
            Fleet fleet = new Fleet(this.ConnectionString);
            fleetId = fleet.GetFleetIdByFleetName(userInfo.organizationId, "All Vehicles");
            // assign user to the default fleet
            if (fleetId > 0 && userId > 0)
               fleet.AddUserToFleet(fleetId, userId);
            */
			}
			catch (SqlException objException) 
			{
				userId = VLF.CLS.Def.Const.unassignedIntValue;
				string prefixMsg = "Unable to add user. ";
				// 5. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				userId = VLF.CLS.Def.Const.unassignedIntValue;
				string prefixMsg = "Unable to add user. ";
				// 5. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return userId;
		}		

		/// <summary>
		/// Add new user.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="userInfo"></param>
		/// <returns>int next user id</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int AddUser(string userName,UserInfo userInfo)
		{
         int userId = VLF.CLS.Def.Const.unassignedIntValue, fleetId = Const.unassignedIntValue;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Add new user
				userId = user.AddUser(userName,userInfo);
				// 3. Set default user preferences
				// 3.1 set default "MeasurementUnits" to km
				userPreference.AddUserPreference(userId,(int)VLF.CLS.Def.Enums.Preference.MeasurementUnits,"1");
				// 3.2 set default "TimeZone" to GMT
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving, "1");
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving, Convert.ToInt16(System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)).ToString());
            // 4. Save all changes
				sqlExec.CommitTransaction();

            // Get fleet ID
            /*
            Fleet fleet = new Fleet(this.ConnectionString);
            fleetId = fleet.GetFleetIdByFleetName(userInfo.organizationId, "All Vehicles");
            // assing user to the default fleet
            if (fleetId > 0 && userId > 0)
               fleet.AddUserToFleet(fleetId, userId);
            */
			}
			catch (SqlException objException) 
			{
				userId = VLF.CLS.Def.Const.unassignedIntValue;
				string prefixMsg = "Unable to add user. ";
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				userId = VLF.CLS.Def.Const.unassignedIntValue;
				string prefixMsg = "Unable to add user. ";
				// 4. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return userId;
		}

      /// <summary>
      /// Create new user and assign 'All Vehicles' fleet
      /// </summary>
      /// <returns>int next user id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <param name="userName"></param>
      /// <param name="userInfo"></param>
      /// <param name="pInfo"></param>
      public int AddNewUserAssignDefaultFleet(string userName, UserInfo userInfo, VLF.CLS.Def.Structures.PersonInfoStruct pInfo)
      {
         int userId = Const.unassignedIntValue, fleetId = Const.unassignedIntValue;
         string prefixMsg = "Unable to add user. ";
         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
            // 2. Add new person
            DB.PersonInfo personInfo = new DB.PersonInfo(sqlExec);
            personInfo.AddPerson(ref pInfo);
            userInfo.personId = pInfo.personId;
            // 3. Add new user
            userId = user.AddUser(userName, userInfo);
            if (userId < 1)
               throw new Exception();
            // 4. Set default user preferences
            // 4.1 set default "MeasurementUnits" to km
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.MeasurementUnits, "1");
            // 4.2 set default "TimeZone" to GMT
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.TimeZone, "0");
            // 4.3 set default "AutoAdjustDayLightSaving" to check
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving, "1");
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving, Convert.ToInt16(System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)).ToString());

            // 5. Save all changes
            sqlExec.CommitTransaction();
            
            // Get fleet ID
            Fleet fleet = new Fleet(this.ConnectionString);
            fleetId = fleet.GetFleetIdByFleetName(userInfo.organizationId, "All Vehicles");
            // assing user to the default fleet
            if (fleetId > 0 && userId > 0)
               fleet.AddUserToFleet(fleetId, userId);
         }
         catch (SqlException objException)
         {
            userId = VLF.CLS.Def.Const.unassignedIntValue;
            // 5. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            userId = VLF.CLS.Def.Const.unassignedIntValue;
            // 5. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return userId;
      }		

      /// <summary>
      /// Create new user and assign 'All Vehicles' fleet
      /// </summary>
      /// <param name="userName"></param>
      /// <param name="userInfo"></param>
      /// <returns>int next user id</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public int AddUserAssignDefaultFleet(string userName, UserInfo userInfo)
      {
         int userId = Const.unassignedIntValue, fleetId = Const.unassignedIntValue;
         string prefixMsg = "Unable to add user. ";
         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
            // 2. Add new user
            userId = user.AddUser(userName, userInfo);
            if (userId < 1)
               throw new Exception();
            // 3. Set default user preferences
            // 3.1 set default "MeasurementUnits" to km
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.MeasurementUnits, "1");
            // 3.2 set default "TimeZone" to GMT
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving, "1");
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving, Convert.ToInt16(System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)).ToString());

            // 4. Save all changes
            sqlExec.CommitTransaction();
            
            // Get fleet ID
            Fleet fleet = new Fleet(this.ConnectionString);
            fleetId = fleet.GetFleetIdByFleetName(userInfo.organizationId, "All Vehicles");
            // assign user to the default fleet
            if (fleetId > 0 && userId > 0)
               fleet.AddUserToFleet(fleetId, userId);
         }
         catch (SqlException objException)
         {
            userId = VLF.CLS.Def.Const.unassignedIntValue;
            // 4. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            userId = VLF.CLS.Def.Const.unassignedIntValue;
            // 4. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(prefixMsg + " " + objException.Message);
         }
         return userId;
      }

      // Changes for TimeZone Feature start
      /// <summary>
      /// Update user information.
      /// </summary>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <param name="userId"></param>
      /// <param name="userName"></param>
      /// <param name="firstName"></param>
      /// <param name="lastName"></param>
      /// <param name="expiredDate"></param>
      public void UpdateInfo_NewTZ(int userId, string userName, string firstName, string lastName, DateTime expiredDate)
      {
          DataSet dsResult = user.GetUserInfoByUserId_NewTZ(userId);
          if (dsResult == null || dsResult.Tables.Count == 0 || dsResult.Tables[0].Rows.Count == 0)
              throw new VLF.ERR.DASAppResultNotFoundException("Unable to retieve user info by user id: " + userId.ToString());

          string personId = dsResult.Tables[0].Rows[0]["PersonId"].ToString().TrimEnd();
          try
          {
              // 1. Begin transaction
              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
              DB.PersonInfo personInfo = new DB.PersonInfo(sqlExec);
              // 2. update person info
              personInfo.UpdateInfo(personId, firstName, lastName);
              // 3. update user name
              user.UpdateInfo(userId, userName, expiredDate);
              // 4. Save all changes
              sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
      }


      // Changes for TimeZone Feature end

      /// <summary>
      /// Update user information.
      /// </summary>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <param name="userId"></param>
      /// <param name="userName"></param>
      /// <param name="firstName"></param>
      /// <param name="lastName"></param>
      /// <param name="expiredDate"></param>
      public void UpdateInfo(int userId, string userName, string firstName, string lastName, DateTime expiredDate)
      {
          DataSet dsResult = user.GetUserInfoByUserId(userId);
          if (dsResult == null || dsResult.Tables.Count == 0 || dsResult.Tables[0].Rows.Count == 0)
              throw new VLF.ERR.DASAppResultNotFoundException("Unable to retieve user info by user id: " + userId.ToString());

          string personId = dsResult.Tables[0].Rows[0]["PersonId"].ToString().TrimEnd();
          try
          {
              // 1. Begin transaction
              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
              DB.PersonInfo personInfo = new DB.PersonInfo(sqlExec);
              // 2. update person info
              personInfo.UpdateInfo(personId, firstName, lastName);
              // 3. update user name
              user.UpdateInfo(userId, userName, expiredDate);
              // 4. Save all changes
              sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
      }

      // Changes for TimeZone Feature start
      /// <summary>
      /// Update user information.
      /// </summary>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <param name="userId"></param>
      /// <param name="userName"></param>
      /// <param name="firstName"></param>
      /// <param name="lastName"></param>
      /// <param name="expiredDate"></param>
      public void UpdateInfo_NewTZ(int userId, string userName, string firstName, string lastName, DateTime expiredDate, string status)
      {
          DataSet dsResult = user.GetUserInfoByUserId_NewTZ(userId);
          if (dsResult == null || dsResult.Tables.Count == 0 || dsResult.Tables[0].Rows.Count == 0)
              throw new VLF.ERR.DASAppResultNotFoundException("Unable to retieve user info by user id: " + userId.ToString());

          string personId = dsResult.Tables[0].Rows[0]["PersonId"].ToString().TrimEnd();
          try
          {
              // 1. Begin transaction
              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
              DB.PersonInfo personInfo = new DB.PersonInfo(sqlExec);
              // 2. update person info
              personInfo.UpdateInfo(personId, firstName, lastName);
              // 3. update user name
              user.UpdateInfoStatus(userId, userName, expiredDate, status);
              // 4. Save all changes
              sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
      }


      // Changes for TimeZone Feature end

      /// <summary>
      /// Update user information.
      /// </summary>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
      /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      /// <param name="userId"></param>
      /// <param name="userName"></param>
      /// <param name="firstName"></param>
      /// <param name="lastName"></param>
      /// <param name="expiredDate"></param>
      public void UpdateInfo(int userId, string userName, string firstName, string lastName, DateTime expiredDate, string status)
      {
          DataSet dsResult = user.GetUserInfoByUserId(userId);
          if (dsResult == null || dsResult.Tables.Count == 0 || dsResult.Tables[0].Rows.Count == 0)
              throw new VLF.ERR.DASAppResultNotFoundException("Unable to retieve user info by user id: " + userId.ToString());

          string personId = dsResult.Tables[0].Rows[0]["PersonId"].ToString().TrimEnd();
          try
          {
              // 1. Begin transaction
              sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
              DB.PersonInfo personInfo = new DB.PersonInfo(sqlExec);
              // 2. update person info
              personInfo.UpdateInfo(personId, firstName, lastName);
              // 3. update user name
              user.UpdateInfoStatus(userId, userName, expiredDate, status);
              // 4. Save all changes
              sqlExec.CommitTransaction();
          }
          catch (SqlException objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              Util.ProcessDbException(prefixMsg, objException);
          }
          catch (DASDbConnectionClosed exCnn)
          {
              sqlExec.RollbackTransaction();
              throw new DASDbConnectionClosed(exCnn.Message);
          }
          catch (Exception objException)
          {
              userId = VLF.CLS.Def.Const.unassignedIntValue;
              string prefixMsg = "Unable to update user info. ";
              // 4. Rollback all changes
              sqlExec.RollbackTransaction();
              throw new DASException(prefixMsg + " " + objException.Message);
          }
      }		

        //Changes
      /// <summary>
      /// Update Ameco user status.
      /// </summary>
      /// <param name="userInfo"></param>
      /// <param name="userName"></param>
      ///  <param name="expiredDate"></param>
      ///  <param name="status"></param>
      /// <returns>void</returns>   
      /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
      
      public void UpdateAmecoUserStatus(int userId, string userName, DateTime expiredDate, string status)
      {
          user.UpdateAmecoUserStatus(userId, userName, expiredDate, status);
      }
        //Changes


		/// <summary>
		/// Update user information.
		/// </summary>
		/// <param name="userInfo"></param>
		/// <param name="userName"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateInfo(UserInfo userInfo,string userName)
		{
			user.UpdateInfo(userInfo,userName);
		}		
		
        public void UpdateUserInfo(UserInfo userInfo)
        {
            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                user.UpdateUserInfo(userInfo);
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to update user info. ";
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                //userInfo.userId = VLF.CLS.Def.Const.unassignedIntValue;
                string prefixMsg = "Unable to update user info. ";
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }


        /// <summary>
		/// Delete existing user.
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserByDriverLicense(string driverLicense)
		{
			int rowsAffected = 0;
			int userId = GetUserIdByDriverLicense(driverLicense);
			rowsAffected = DeleteUserByUserId(userId);
			return rowsAffected;
		}
		/// <summary>
		/// Delete existing user.
		/// </summary>
		/// <param name="userName"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserByUserName(string userName)
		{
			int rowsAffected = 0;
			int userId = GetUserIdByUserName(userName);
			rowsAffected = DeleteUserByUserId(userId);
			return rowsAffected;
		}
     
		/// <summary>
		/// Delete existing user.
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>Rows Affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
      /// <comment>
      ///         this function should be moved on the server side as all the references at the user table
      ///         can grow in time
      ///         therefore I decided to disable the user instead of deleting all his foreign references
      /// </comment>
		public int DeleteUserByUserId(int userId)
		{
         int rowsAffected = 1;

         try
         {
            string userName = user.GetUserNameByUserId(userId);
            user.UpdateInfo(userId, userName, DateTime.Now.AddDays(-1));
         }
         catch (Exception objException)
         {
            rowsAffected = 0;
            string prefixMsg = "Unable to delete user. " + userId.ToString();
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         return rowsAffected;
      }

/*
      public int TryDeletingUser(int userId)
      {

			int rowsAffected = 0;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

				// 2. Delete user from all UserGroups (vlfUserGroupAssignment)
				DB.UserGroupAssignment userGroup = new DB.UserGroupAssignment(sqlExec);
            rowsAffected += userGroup.DeleteUserAssignments(userId);

				// 3. Delete user from all fleets (vlfFleetUsers)
				DB.FleetUsers fleets = new DB.FleetUsers(sqlExec);
            rowsAffected += fleets.DeleteUserFromAllFleets(userId);

				// 4. Delete all users alarms
				DB.Alarm alarms = new DB.Alarm(sqlExec);
            rowsAffected += alarms.DeleteAllUserAlarms(userId);

				// 5. Delete all trusted persons related to this user
				DB.TrustedPerson trustPer = new DB.TrustedPerson(sqlExec);
            rowsAffected += trustPer.DeleteTrustedPersonsByUserId(userId);

				// 6. Delete all user preferences
            rowsAffected += userPreference.DeleteUserPreferences(userId);

				// 7. Delete user logins
				DB.UserLogin userLogin = new DB.UserLogin(sqlExec);
            rowsAffected += userLogin.DeleteUserLogins(userId);

				// 8. Delete user map usage
				DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
				rowsAffected += mapEngine.DeleteUserMapUsage(userId);

				// 9. Delete user logins
				DB.TxtMsgs txtMsgs = new DB.TxtMsgs(sqlExec);
            rowsAffected += txtMsgs.DeleteAllUserMsgs(userId);

				// 10. Delete user
				rowsAffected += user.DeleteUserByUserId(userId); 

				// 10. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				rowsAffected = 0;
				string prefixMsg = "Unable to delete user. ";
				// 9. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				rowsAffected = 0;
				string prefixMsg = "Unable to delete user. ";
				// 9. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;  
		}		
*/

		/// <summary>
		/// Retrieves user contact info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [Birthday],[PIN],[Address],[City],[StateProvince],[Country],[PhoneNo1],[PhoneNo2],[CellNo]
		/// </returns>
		/// <param name="userId"></param> 
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetContactInfoByUserId(int userId)
		{
			DataSet dsResult = user.GetContactInfoByUserId(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserContactInfo" ;
				}
				dsResult.DataSetName = "User";
			}
			return dsResult;
		}
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <returns>
        /// DataSet 
        /// [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],
        /// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
        /// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]
        /// </returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetUserInfoByUserId_NewTZ(int userId)
        {
            DataSet dsResult = user.GetUserInfoByUserId_NewTZ(userId);
            return PrepareUserInfoDataSet(dsResult);
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <returns>
        /// DataSet 
        /// [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],
        /// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
        /// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]
        /// </returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetUserInfoByUserId(int userId)
        {
            DataSet dsResult = user.GetUserInfoByUserId(userId);
            return PrepareUserInfoDataSet(dsResult);
        }

        /// <summary>
        /// return only user info from table vlfUser
        /// person info not returned
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetUserInfoById(int userId)
        {
            DataSet dsResult = user.GetUserInfoById(userId);
            return PrepareUserInfoDataSet(dsResult);
        }
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user expiration datetime
        /// </summary>
        /// <param name="userId"></param> 
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public void CheckUserExpiration_NewTZ(int userId)
        {
            DateTime expirationDate = user.GetUserExpirationDate_NewTZ(userId);

            if (expirationDate != VLF.CLS.Def.Const.unassignedDateTime)	// not permanent user
            {
                DateTime now = DateTime.Now;
                // check if user has not been allowed access sysstem yet
                if (now.Year > expirationDate.Year ||
                    (now.Year == expirationDate.Year && now.DayOfYear >= expirationDate.DayOfYear))
                {
                    throw new VLF.ERR.ASIUserExpired("User " + userId + " is expired.");
                }
            }
        }

        // Chnages for TimeZone Feature end

        /// <summary>
        /// Retrieves user expiration datetime
        /// </summary>
        /// <param name="userId"></param> 
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public void CheckUserExpiration(int userId)
        {
            DateTime expirationDate = user.GetUserExpirationDate(userId);

            if (expirationDate != VLF.CLS.Def.Const.unassignedDateTime)	// not permanent user
            {
                DateTime now = DateTime.Now;
                // check if user has not been allowed access sysstem yet
                if (now.Year > expirationDate.Year ||
                    (now.Year == expirationDate.Year && now.DayOfYear >= expirationDate.DayOfYear))
                {
                    throw new VLF.ERR.ASIUserExpired("User " + userId + " is expired.");
                }
            }
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <returns>
        /// DataSet 
        /// [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],
        /// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
        /// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]
        /// </returns>
        /// <param name="userName"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetUserInfoByUserName_NewTZ(string userName)
        {
            DataSet dsResult = user.GetUserInfoByUserName_NewTZ(userName);
            return PrepareUserInfoDataSet(dsResult);
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <returns>
        /// DataSet 
        /// [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],
        /// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
        /// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]
        /// </returns>
        /// <param name="userName"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetUserInfoByUserName(string userName)
        {
            DataSet dsResult = user.GetUserInfoByUserName(userName);
            return PrepareUserInfoDataSet(dsResult);
        }	

        //Changes
        /// <summary>
        /// Retrieves user Id
        /// </summary>
        /// <returns>        
        /// [UserId]
        /// </returns>
        /// <param name="userName" "organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public int GetAmecoUserIdByUserName(string userName, int organizationId)
        {
            int dsResult = user.GetAmecoUserIdByUserName(userName, organizationId);
            return dsResult;
        }	
	
        //Changes
	
		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],
		/// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
		/// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName]
		/// </returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public DataSet GetUserInfoByDriverLicense(string driverLicense)
		{
			DataSet dsResult = user.GetUserInfoByDriverLicense(driverLicense);
			return PrepareUserInfoDataSet(dsResult);
		}		
		/// <summary>
		/// Returns organization id by user Id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int GetOrganizationIdByUserId(int userId)
		{
			return user.GetOrganizationIdByUserId(userId);
		}		
		/// <summary>
		/// Returns user name by user Id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public string GetUserNameByUserId(int userId)
		{
			return user.GetUserNameByUserId(userId);
		}		
		/// <summary>
		/// Returns user name by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public string GetUserNameByDriverLicense(string driverLicense)
		{
			return user.GetUserNameByDriverLicense(driverLicense);
		}		
		/// <summary>
		/// Returns organization id by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int GetOrganizationIdByDriverLicense(string driverLicense)
		{
			return user.GetOrganizationIdByDriverLicense(driverLicense);
		}		
		/// <summary>
		/// Returns password by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public string GetPasswordByDriverLicense(string driverLicense)
		{
			return user.GetPasswordByDriverLicense(driverLicense);
		}		
		/// <summary>
		/// Returns user id by user name. 	
		/// </summary>
		/// <param name="userName"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int GetUserIdByUserName(string userName)
		{
			return user.GetUserIdByUserName(userName);
		}		
		/// <summary>
		/// Validate user. 	
		/// </summary>
		/// <param name="userName"></param> 
		/// <param name="password"></param> 
		/// <remarks>
		/// In case of invalid data, returns -1
		/// </remarks>
		/// <returns>user id</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int ValidateUser(string userName,string password)
		{
			return user.ValidateUser(userName,password);
		}



      /// <summary>
      /// Validate user. 	
      /// </summary>
      /// <param name="userName"></param> 
      /// <param name="password"></param> 
      /// <remarks>
      /// In case of invalid data, returns -1
      /// </remarks>
      /// <returns>user id</returns>
      /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
      public int ValidateUserMD5(string userName, string password)
      {
         return user.ValidateUserMD5(userName,password);
      }		

		/// <summary>
		/// Check if user assigned to the group
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>true/false</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public bool IsUserAssignedToUserGroup(int userId,short userGroupId)
		{
			DB.UserGroupAssignment userGroupAssign = new DB.UserGroupAssignment(sqlExec);
			return userGroupAssign.IsUserAssignedToUserGroup(userId,userGroupId);
		}			
		
		/// <summary>
		/// Returns user id by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public int GetUserIdByDriverLicense(string driverLicense)
		{
			return user.GetUserIdByDriverLicense(driverLicense);
		}		
		/// <summary>
		/// Returns driver license by user name. 	
		/// </summary>
		/// <param name="userName"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public string GetDriverLicenseByUserName(string userName)
		{
			return user.GetDriverLicenseByUserName(userName);
		}		
		/// <summary>
		/// Returns driver license by user id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public string GetDriverLicenseByUserId(int userId)
		{
			return user.GetDriverLicenseByUserId(userId);
		}		
		/// <summary>
		/// Retrieves users boxes by user id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>ArrayList [int]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public ArrayList GetUserBoxes(int userId)
		{
			return user.GetUserBoxes(userId);
		}
		/// <summary>
		/// Update password by user id
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="password"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if unabled to update information.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void SetPasswordByUserId(string password,int userId)
		{
			user.SetPasswordByUserId(password,userId);
		}


      /// Update password by user id
      /// </summary>
      /// <param name="userId"></param> 
      /// <param name="password"></param> 
      /// <returns>void</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if unabled to update information.</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void SetHashPasswordByUserId(string password, int userId)
      {
         user.SetHashPasswordByUserId(GetMD5HashData(password), userId);
      }

		/// <summary>
		/// Update organization id by user id
		/// </summary>
		/// <param name="organizationId"></param> 
		/// <param name="userId"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public void SetOrganizationIdByUserId(int organizationId,int userId)
		{
			user.SetOrganizationIdByUserId(organizationId,userId);
		}
		/// <summary>
		/// Update organization id by user name
		/// </summary>
		/// <param name="organizationId"></param> 
		/// <param name="userName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public void SetOrganizationIdByUserName(int organizationId,string userName)
		{
			user.SetOrganizationIdByUserName(organizationId,userName);
		}
		/// <summary>
		/// Update user name by user id
		/// </summary>
		/// <param name="userName"></param> 
		/// <param name="userId"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public void SetUserNameByUserId(string userName,int userId)
		{
			user.SetUserNameByUserId(userName,userId);
		}
		/// <summary>
		/// Update password by user name
		/// </summary>
		/// <param name="password"></param> 
		/// <param name="userName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public void SetPasswordByUserName(string password,string userName)
		{
			user.SetPasswordByUserName(password,userName);
		}
		/// <summary>
		/// Adds additional information to result DataSet
		/// - Adds TableName = "UserInfo"
		/// - Adds DataSetName = "User"
		/// - Retrieves organization name by organization id
		/// - Adds "OrganizationName" column 
		/// - Deletes "OrganizationId" column
		/// </summary>
		/// <param name="userInfo"></param>
		/// <returns></returns>
		protected DataSet PrepareUserInfoDataSet(DataSet userInfo)
		{
			if(userInfo != null) 
			{
				if(userInfo.Tables.Count > 0)
				{
				// Adds TableName = "UserInfo"
					userInfo.Tables[0].TableName = "UserInfo" ;
				}
				// Adds DataSetName = "User"
				userInfo.DataSetName = "User";
			}

			/* organization name already included,organization id will stay in result
			// Retrieves organization name by organization id
			int organizationId = Convert.ToInt32(userInfo.Tables[0].Rows[0]["OrganizationId"]);
			Organization organization = new Organization(sqlExec);
			string organizationName = organization.GetOrganizationNameByOrganizationId(organizationId);
			
			// Adds "OrganizationName" column
			// Create new DataColumn, set DataType, ColumnName and add to DataTable.    
			DataColumn newColumn = new DataColumn();
			newColumn.DataType = Type.GetType("System.String");
			newColumn.ColumnName = "OrganizationName";
			newColumn.ReadOnly = true;
			newColumn.Unique = false;
			// Add the Column to the DataColumnCollection.
			userInfo.Tables[0].Columns.Add(newColumn);
			// or
			//userInfo.Tables[0].Columns.Add("OrganizationName",System.Type.GetType("System.String"));
			userInfo.Tables[0].Rows[0]["OrganizationName"] = organizationName;

			// Deletes "OrganizationId" column
			if(userInfo.Tables[0].Columns.Contains("OrganizationId"))
			{
				userInfo.Tables[0].Columns.Remove("OrganizationId");
			}
			*/
			return userInfo;
		}
		#endregion

		#region User Preferences Interfaces
		/// <summary>
		/// Add new user preference
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <param name="preferenceValue"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public void AddUserPreference(int userId,int preferenceId,string preferenceValue)
		{
			userPreference.AddUserPreference(userId,preferenceId,preferenceValue);
		}	
		/// <summary>
		/// Update user preference
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <param name="preferenceValue"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user preference does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void UpdateUserPreference(int userId,int preferenceId,string preferenceValue)
		{
			userPreference.UpdateUserPreference(userId,preferenceId,preferenceValue);
		}
		/// <summary>
		/// Delete all user preferences
		/// </summary>
		/// <param name="userId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserPreferences(int userId)
		{
			return userPreference.DeleteUserPreferences(userId);
		}
		/// <summary>
		/// Delete user preference
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserPreference(int userId,int preferenceId)
		{
			return userPreference.DeleteUserPreference(userId,preferenceId);
		}
		/// <summary>
		/// Retrieves all user preferences info
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllUserPreferencesInfo(int userId)
		{
			DataSet dsResult = userPreference.GetAllUserPreferencesInfo(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserPreferencesInfo" ;
				}
				dsResult.DataSetName = "User";
			}
			return dsResult;
		}	
		/// <summary>
		/// Retrieves user preference info
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <returns>DataSet [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetUserPreferenceInfo(int userId,int preferenceId)
		{
			DataSet dsResult = userPreference.GetUserPreferenceInfo(userId,preferenceId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserPreferenceInfo" ;
				}
				dsResult.DataSetName = "User";
			}
			return dsResult;
		}	
		/// <summary>
		/// Retrieves all users preferences info
		/// </summary>
		/// <returns>DataSet [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllUsersPreferencesInfo()
		{
			DataSet dsResult = userPreference.GetAllUsersPreferencesInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllUsersPreferenceInfo" ;
				}
				dsResult.DataSetName = "User";
			}
			return dsResult;
		}
		/// <summary>
		/// Retrieves all user preferences types 
		/// </summary>
		/// <returns>DataSet [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllUserPreferencesTypes()
		{
			DB.Preference preference = new DB.Preference(sqlExec);
			DataSet dsResult = preference.GetAllRecords();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserPreferencesTypes" ;
				}
				dsResult.DataSetName = "UserPreferences";
			}
			return dsResult;
		}


        // Changes for TimeZone Feature start

        /// <summary>
        /// Adjust GMT time to user location time
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gmtDateTime"></param>
        /// <returns></returns>
        public DateTime GetUserLocalTime_NewTZ(int userId, DateTime gmtDateTime)
        {
            float timeZone = 0;
            //[UserId], [PreferenceId], [PreferenceName], [PreferenceValue]
            DataSet dsPrefInfo = GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.TimeZoneNew);
            if (dsPrefInfo != null && dsPrefInfo.Tables.Count > 0 && dsPrefInfo.Tables[0].Rows.Count > 0)
            {
                timeZone = Convert.ToSingle(dsPrefInfo.Tables[0].Rows[0]["PreferenceValue"]);
                dsPrefInfo = GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving);
                if (dsPrefInfo != null && dsPrefInfo.Tables.Count > 0 && dsPrefInfo.Tables[0].Rows.Count > 0)
                    timeZone += Convert.ToInt16(dsPrefInfo.Tables[0].Rows[0]["PreferenceValue"]);
            }
            if ((gmtDateTime == VLF.CLS.Def.Const.unassignedDateTime) ||
                (timeZone > 13) ||
                (timeZone < -12) ||
                (gmtDateTime.Ticks + timeZone * TimeSpan.TicksPerHour <= VLF.CLS.Def.Const.unassignedDateTime.Ticks))
                return gmtDateTime;
            else
                return gmtDateTime.AddHours(timeZone);
        }

        // Changes for TimeZone Feature end

		/// <summary>
		/// Adjust GMT time to user location time
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="gmtDateTime"></param>
		/// <returns></returns>
		public DateTime GetUserLocalTime(int userId,DateTime gmtDateTime)
		{
			short timeZone = 0;
			//[UserId], [PreferenceId], [PreferenceName], [PreferenceValue]
			DataSet dsPrefInfo = GetUserPreferenceInfo(userId,(int)VLF.CLS.Def.Enums.Preference.TimeZone);
			if(dsPrefInfo != null && dsPrefInfo.Tables.Count > 0 && dsPrefInfo.Tables[0].Rows.Count > 0)
			{
				timeZone = Convert.ToInt16(dsPrefInfo.Tables[0].Rows[0]["PreferenceValue"]);
				dsPrefInfo = GetUserPreferenceInfo(userId,(int)VLF.CLS.Def.Enums.Preference.DayLightSaving);
				if(dsPrefInfo != null && dsPrefInfo.Tables.Count > 0 && dsPrefInfo.Tables[0].Rows.Count > 0)
					timeZone += Convert.ToInt16(dsPrefInfo.Tables[0].Rows[0]["PreferenceValue"]);
			}
			if(	(gmtDateTime == VLF.CLS.Def.Const.unassignedDateTime)||
				(timeZone > 13) ||
				(timeZone < -12) ||
				(gmtDateTime.Ticks + timeZone*TimeSpan.TicksPerHour <= VLF.CLS.Def.Const.unassignedDateTime.Ticks))
				return gmtDateTime;
			else
				return gmtDateTime.AddHours(timeZone);
		}

      /// <summary>
      /// Get User Preference
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="preferenceId"></param>
      /// <returns>Result Object: convert to required type</returns>
      public object GetUserPreferenceValue(int userId, int preferenceId)
      {
         DataSet dsPref = new DataSet();
         dsPref = GetUserPreferenceInfo(userId, preferenceId);
         if (Util.IsDataSetValid(dsPref))
            return dsPref.Tables[0].Rows[0]["PreferenceValue"];
         else
            return null;
      }

      /// <summary>
      /// Get User TimeZone and DayLightSaving preference
      /// </summary>
      /// <param name="userId"></param>
      /// <returns>TimeZone + DayLightSaving</returns>
      public int GetUserTimeZoneDayLightSaving(int userId)
      {
         int tz = 0, dls = 0;
         object result = null;
         result = GetUserPreferenceValue(userId, (int)Enums.Preference.TimeZone);
         if (result != null)
            Int32.TryParse(result.ToString(), out tz);
         result = GetUserPreferenceValue(userId, (int)Enums.Preference.DayLightSaving);
         if (result != null)
            Int32.TryParse(result.ToString(), out dls);
         return tz + dls;
      }


        /// <summary>
        /// Get Users Dashboards
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetUsersDashboards(int userId)
        {
            DataSet dsResult = userPreference.GetUsersDashboards(userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UsersDashboards";
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }



       

        /// <summary>
        /// Get Dashboard Types
        /// </summary>
        /// <returns></returns>
        public DataSet GetDashboardTypes()
        {
            DataSet dsResult = userPreference.GetDashboardTypes();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "Dashboards";
                }
                dsResult.DataSetName = "Dashboards";
            }
            return dsResult;
        }	
		#endregion

		#region User Logins

		/// <summary>
		/// Adds new user login
		/// </summary>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int AddUserLogin(int userId, DateTime loginDateTime, string IP, string LoginUserSecId)
		{
            return userLogin.AddUserLogin(userId, loginDateTime, IP, LoginUserSecId);
		}

        /// <summary>
        /// Adds new user login
        /// </summary>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int AddUserLoginExtended(int userId, string IP, int LoginUserId, string LoginUserSecId)
        {
            return userLogin.AddUserLoginExtended(userId, IP, LoginUserId, LoginUserSecId);
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        public DataSet GetUserLogins_NewTZ(int userId, DateTime from, DateTime to)
        {
            DataSet dsResult = userLogin.GetUserLogins_NewTZ(userId, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserLoginsInfo";
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end



        /// <summary>
        /// Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        public DataSet GetUserLogins(int userId, DateTime from, DateTime to)
        {
            DataSet dsResult = userLogin.GetUserLogins(userId, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserLoginsInfo";
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Get User Last Login
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginUserId"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        public DataSet GetUserLastLogin_NewTZ(int userId, int loginUserId)
        {
            DataSet dsResult = userLogin.GetUserLastLogin_NewTZ(userId, loginUserId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserLastLogin";
                    if (dsResult.Tables[0].Rows.Count > 0)
                        dsResult.Tables[0].Rows.RemoveAt(0);
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end
        /// <summary>
        /// Get User Last Login
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginUserId"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        public DataSet GetUserLastLogin(int userId, int loginUserId)
        {
            DataSet dsResult = userLogin.GetUserLastLogin(userId, loginUserId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UserLastLogin";
                    if (dsResult.Tables[0].Rows.Count > 0)
                        dsResult.Tables[0].Rows.RemoveAt(0);
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }
		#endregion

		#region Authorization functions
		/// <summary>
		/// Validate user alarm. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="alarmId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserAlarm(int userId,int alarmId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserAlarm(userId,alarmId);
		}


        /// <summary>
        /// Validate user alarm. 	
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="alarmId"></param>
        /// <returns>true if valid, otherwise returns false</returns>
        /// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public bool ValidateUserAlarmOne(int userId, int alarmId)
        {
            #region ValidateUserCall
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            user.ValidateUserCall(userId, methodBase.Name, methodBase.ReflectedType.Name);
            #endregion
            return user.ValidateUserAlarm(userId, alarmId);
        }	
		/// <summary>
		/// Validate user fleet. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="fleetId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserFleet(int userId,int fleetId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserFleet(userId,fleetId);
		}

        public bool ValidateUserFleetOne(int userId, int fleetId)
        {
            #region ValidateUserCall
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            user.ValidateUserCall(userId, methodBase.Name, methodBase.ReflectedType.Name);
            #endregion
            return user.ValidateUserFleet(userId, fleetId);
        }

      /// <summary>
      /// Validate user driver
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="driverId"></param>
      /// <returns>true if valid, otherwise returns false</returns>
      /// <exception cref="DASAuthorizationException">Thrown if user is unauthorized to use this method.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool ValidateUserDriver(int userId, int driverId)
      {
         #region ValidateUserCall
         StackTrace stackTrace = new StackTrace();
         StackFrame stackFrame = stackTrace.GetFrame(2);
         MethodBase methodBase = stackFrame.GetMethod();
         user.ValidateUserCall(userId, methodBase.Name, methodBase.ReflectedType.Name);
         #endregion
         return user.ValidateUserDriver(userId, driverId);
      }	
		/// <summary>
		/// Validate user organization. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="organizationId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserOrganization(int userId,int organizationId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserOrganization(userId,organizationId);
		}	
		/// <summary>
		/// Validate user vehicle. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="vehicleId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserVehicle(int userId,Int64 vehicleId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserVehicle(userId,vehicleId);
		}	
		/// <summary>
		/// Validate user license plate. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="licensePlate"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserLicensePlate(int userId,string licensePlate)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			//StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserLicensePlate(userId,licensePlate);
		}

        public bool ValidateUserLicensePlateOne(int userId, string licensePlate)
        {
            #region ValidateUserCall
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            //StackFrame stackFrame = stackTrace.GetFrame(2);
            MethodBase methodBase = stackFrame.GetMethod();
            user.ValidateUserCall(userId, methodBase.Name, methodBase.ReflectedType.Name);
            #endregion
            return user.ValidateUserLicensePlate(userId, licensePlate);
        }	
		/// <summary>
		/// Validate user box Id. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="boxId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserBox(int userId,int boxId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserBox(userId,boxId);
		}


        /// <summary>
        /// Validate user box Id. 	
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="boxId"></param>
        /// <returns>true if valid, otherwise returns false</returns>
        /// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public bool ValidateUserBoxOne(int userId, int boxId)
        {
            #region ValidateUserCall
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            user.ValidateUserCall(userId, methodBase.Name, methodBase.ReflectedType.Name);
            #endregion
            return user.ValidateUserBox(userId, boxId);
        }	

		/// <summary>
		/// Validate user msg. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="msgId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserMsg(int userId,int msgId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserMsg(userId,msgId);
		}	
		/// <summary>
		/// Validate user organization name. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="organizationName"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserOrganizationName(int userId,string organizationName)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserOrganizationName(userId,organizationName);
		}	
		/// <summary>
		/// Validate HGI super user. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateHGISuperUser(int userId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateHGISuperUser(userId);
		}	
		/// <summary>
		/// Validate super user. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateSuperUser(int userId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateSuperUser(userId);
		}


        /// <summary>
        /// Validate super user. 	
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true if valid, otherwise returns false</returns>
        /// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public bool ValidateSuperUserOne(int userId)
        {
            #region ValidateUserCall
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            user.ValidateUserCall(userId, methodBase.Name, methodBase.ReflectedType.Name);
            #endregion
            return user.ValidateSuperUser(userId);
        }	

		/// <summary>
		/// Validate user group security. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserGroupSecurity(int userId,int userGroupId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserGroupSecurity(userId,userGroupId);
		}	
		/// <summary>
		/// Validate user preference. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASAuthorizationException">Thrown if user is unauthorized using this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserPreference(int userId,int preferenceId)
		{
			#region ValidateUserCall
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);
			MethodBase methodBase = stackFrame.GetMethod();
			user.ValidateUserCall(userId,methodBase.Name,methodBase.ReflectedType.Name);
			#endregion
			return user.ValidateUserPreference(userId,preferenceId);
		}	
		#endregion

		/// <summary>
		/// Get all user vehicles active assignment information
		/// </summary>
		/// <remarks>
		/// TableName	= "UserAllActiveVehiclesInfo"
		/// DataSetName = "Vehicle"
		/// </remarks>
		/// <exception cref="DASException">Thrown DASException in exception cases.</exception>
		/// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],
		/// [MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
		/// [ModelYear],[Color],[Description],[CostPerMile],
		/// [IconTypeId],[IconTypeName]</returns>
		public DataSet GetUserAllVehiclesActiveInfo(int userId)
		{
			DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
			// 1. Retrieves all vehicle assignments
			DataSet dsResult = vehicleInfo.GetUserAllVehiclesActiveInfo(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserAllActiveVehiclesInfo" ;
				}
				dsResult.DataSetName = "Vehicle";
			}
			return dsResult;
		}

		/// <summary>
		/// Add map/user usage
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="mapTypeId"></param>
		/// <param name="usageYear"></param>
        /// <param name="usageMonth"></param>
        /// <param name="mapId"></param>
        /// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public int AddMapUserUsage(int userId, short mapTypeId, short usageYear, short usageMonth, short mapId)
		{
			DB.MapEngine mapEngine = new DB.MapEngine(sqlExec);
            return mapEngine.AddMapUserUsage(userId, mapTypeId, usageYear, usageMonth, mapId);
		}

        /// <summary>
        /// Add/Update User Preference
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="preferenceId"></param>
        /// <param name="preferenceValue"></param>
        /// <returns></returns>
       public int UserPreference_Add_Update(int userId, int preferenceId, string preferenceValue)
        {
            return user.UserPreference_Add_Update(userId, preferenceId, preferenceValue); ;
        }

       /// <summary>
       /// Validate GeoZone Updatable By User
       /// </summary>
       /// <param name="organizationId"></param>
       /// <param name="userId"></param>
       public int ValidateGeoZoneUpdatableByUser(int organizationId, int userId, short geozoneId)
       {
           return user.ValidateGeoZoneUpdatableByUser(organizationId, userId, geozoneId);
       }

       /// <summary>
       /// Validate Landmark Updatable By User
       /// </summary>
       /// <param name="organizationId"></param>
       /// <param name="userId"></param>
       public int ValidateLandmarkUpdatableByUser(int organizationId, int userId, long landmarkId)
       {
           return user.ValidateLandmarkUpdatableByUser(organizationId, userId, landmarkId);
       }

      /// <summary>
      /// Get all organization users list of id's
      /// </summary>
      /// <param name="organizationId"></param>
      /// <returns></returns>
      internal List<int> GetOrganizationUsers(int organizationId)
      {
         List<int> userList = new List<int>();
         int userId;
         DataSet dsUsers = user.GetOrganizationUsers(organizationId);
         if (!Util.IsDataSetValid(dsUsers))
            return null;
         foreach (DataRow userRow in dsUsers.Tables[0].Rows)
         {
            if (Int32.TryParse(userRow["UserId"].ToString(), out userId))
               userList.Add(userId);
         }
         return userList;
      }

      /// <summary>
      /// Change Current User Name
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="newUserName">New User Name</param>
      public int ChangeUserName(int userId, string newUserName)
      {
         return user.UpdateRow("SET UserName = @userName WHERE UserId = @userId", 
            new SqlParameter("@userId", userId), new SqlParameter("@userName", newUserName));
      }

      /// <summary>
      /// Get MD5 Hash Data
      /// </summary>
      /// <param name="data">Data</param>
      public static string GetMD5HashData(string data)
       {
         // Generate Hash Password
         MD5CryptoServiceProvider oMD5 = new MD5CryptoServiceProvider();
         byte[] bPassword = System.Text.Encoding.ASCII.GetBytes(data);
         oMD5.ComputeHash(bPassword);
         bPassword = oMD5.Hash;


         // Create a new Stringbuilder to collect the bytes
         // and create a string.
         StringBuilder sBuilder = new StringBuilder();



         // Loop through each byte of the hashed data 
         // and format each one as a hexadecimal string.
         for (int i = 0; i < bPassword.Length; i++)
         {
            sBuilder.Append(bPassword[i].ToString("x2"));
         }

         // Return the hexadecimal string.
         return sBuilder.ToString();

         //return Convert.ToBase64String(bPassword);


      }

        public bool ValidateUserAccess(int userId, Enums.ValidationItem item, object value)
        {
            return user.ValidateUserAccess(userId, item, value);
        }


        public int ValidateLockedUser(int userId)
        {
            string status = null;
            try
            {
                DataSet dsResult = null;
                user.RetrieveLockedUser(userId, ref dsResult);
                DataTable dt = dsResult.Tables[0];
                status = Convert.ToString(dt.Rows[0]["UserStatus"] != DBNull.Value ? dt.Rows[0]["UserStatus"] : "");
                switch (status)
                {
                    case "Locked":
                        return 1;
                    case "Deactivated":
                        return 2;
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public void SetUserStatus(int userId, string status)
        {
            try
            {
                user.SetUserStatus(userId, status);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Gets all active languages
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>XML File format: [LanguageID][CultureName][Language]</returns>
        public DataSet GetLanguages(int UserId)
        {
            return user.GetLanguages(UserId);
        }

        public DataSet GetDateTimeFormat()
        {
            return user.GetDateTimeFormat();
        }

        public DataSet GetDefaultDateTimeFormat(int organizationId)
        {
            return user.GetDefaultDateTimeFormat(organizationId);
        }

        
	
        //get all email
        public DataSet GetAllEmail(int userId)
        {
            DataSet dsResult = user.GetAllEmail();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "AllEmails";
                }
                dsResult.DataSetName = "Email";
            }
            return dsResult;
        }

        // Add email
        public int AddEmail(int userId, string emaill)
        {
             return user.AddEmail(userId, emaill);
        }


   }
}
