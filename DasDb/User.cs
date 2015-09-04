using System;
using System.Collections ;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;

namespace VLF.DAS.DB
{
	
	/// <summary>
	/// Provides interfaces to vlfUser table.
	/// </summary>
	/// <remarks>
	/// See VLF.CLS.Def.Structures.UserInfo for vlfUser table strusture
	/// </remarks>
	public class User : TblOneIntPrimaryKey
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public User(SQLExecuter sqlExec) : base ("vlfUser", sqlExec)
		{
		}
       
        /// <summary>
        /// Add new user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userInfo"></param>
        /// <returns>int next user id</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int AddUser(string userName, UserInfo userInfo)
        {
            int userId = VLF.CLS.Def.Const.unassignedIntValue;
            string prefixMsg = "";
            // 1. validates parameters
            if (String.IsNullOrEmpty(userName.Trim()) ||
            String.IsNullOrEmpty(userInfo.password.Trim()) ||
            String.IsNullOrEmpty(userInfo.personId.Trim()) ||
                (userInfo.organizationId == VLF.CLS.Def.Const.unassignedIntValue))
            {
                throw new DASAppInvalidValueException(
                                                "Wrong value for insert SQL: UserName=" + userName +
                                                " Password=" + userInfo.password +
                    //" HashPassword=" + userInfo.hashpassword + 
                                                " PersonId=" + userInfo.personId +
                                                " OrganizationId=" + userInfo.organizationId +
                                                " UserStatus=" + userInfo.userStatus
                                                );
            }
            int rowsAffected = 0;
            // 2. Get next availible index
            //userId = (int)GetMaxRecordIndex("UserId") + 1;
            object objId = GetMaxValue("UserId");
            if (objId == null)
                throw new DASAppInvalidValueException("Invalid User ID");

            userId = Convert.ToInt32(objId) + 1;

            // 3. Prepares SQL statement
           // string sql = "INSERT INTO vlfUser (UserId,UserName,Password,PersonId,OrganizationId,PIN,Description,HashPassword, UserStatus";
           // if (userInfo.expiredDate != VLF.CLS.Def.Const.unassignedDateTime)
             //   sql += ",ExpiredDate";
           // sql += ") VALUES ( @UserId,@UserName,@Password,@PersonId,@OrganizationId,@PIN,@Description,@HashPassword";
          //  if (userInfo.expiredDate != VLF.CLS.Def.Const.unassignedDateTime)
             //   sql += ",@ExpiredDate";
           // sql += ", @UserStatus)";

            string sql = "INSERT INTO vlfUser (UserId,UserName,Password,PersonId,OrganizationId,PIN,Description,HashPassword, UserStatus";
            if (userInfo.expiredDate != VLF.CLS.Def.Const.unassignedDateTime)
                sql += ",ExpiredDate";
            sql += ") VALUES ( @UserId,@UserName,@Password,@PersonId,@OrganizationId,@PIN,@Description,@HashPassword";
          
            sql += ", @UserStatus";
            if (userInfo.expiredDate != VLF.CLS.Def.Const.unassignedDateTime)
                sql += ",@ExpiredDate";
            sql += ")";
            try
            {
                prefixMsg = "Unable to add new user with personId " + userInfo.personId + " .";
                // Set SQL command
                sqlExec.ClearCommandParameters();
                // Add parameters to SQL statement
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                //if(userName == null)
                //   sqlExec.AddCommandParam("@UserName",SqlDbType.Char,System.DBNull.Value);
                //else
                sqlExec.AddCommandParam("@UserName", SqlDbType.VarChar, userName);
                //if(userInfo.password == null)
                //   sqlExec.AddCommandParam("@Password",SqlDbType.Char,System.DBNull.Value);
                //else
                sqlExec.AddCommandParam("@Password", SqlDbType.VarChar, " ");

                sqlExec.AddCommandParam("@HashPassword", SqlDbType.VarChar, userInfo.hashpassword);
                sqlExec.AddCommandParam("@PersonId", SqlDbType.VarChar, userInfo.personId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, userInfo.organizationId);
                sqlExec.AddCommandParam("@PIN", SqlDbType.VarChar, userInfo.pin);
                if (userInfo.description == null)
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, System.DBNull.Value);
                else
                    sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, userInfo.description);
                
                sqlExec.AddCommandParam("@UserStatus", SqlDbType.VarChar, userInfo.userStatus);
                if (userInfo.expiredDate != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlExec.AddCommandParam("@ExpiredDate", SqlDbType.DateTime, userInfo.expiredDate);
                }
                if (sqlExec.RequiredTransaction())
                {
                    // 4. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                // 5. Executes SQL statement
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
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This user already exists.");
            }
            return userId;
        }	
		
      /// <summary>
		/// Updates user information.
		/// </summary>
		/// <param name="userInfo"></param>
		/// <param name="userId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateInfo(UserInfo userInfo, int userId)
		{
			// 1. validates parameters
			if (userId == VLF.CLS.Def.Const.unassignedIntValue ||
				String.IsNullOrEmpty(userInfo.personId) ||
            String.IsNullOrEmpty(userInfo.hashpassword) ||
				(userInfo.organizationId == VLF.CLS.Def.Const.unassignedIntValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: UserId=" +
					userId + 
					//" Password=" + userInfo.password + 
					" PersonId=" + userInfo.personId +
					" OrganizationId=" + userInfo.organizationId);
			}
         string prefixMsg = "";
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE vlfUser SET PersonId='" + userInfo.personId.Replace("'","''") + "'";
         //if(userInfo.password != "")
         //   sql += " ,Password='" + userInfo.password.Replace("'","''") + "'";

         sql += " ,HashPassword='" + userInfo.hashpassword.Replace("'", "''") + "'";
	
			sql +=", OrganizationId=" + userInfo.organizationId +
				", PIN='" + userInfo.pin.Replace("'","''") + "'" +
				", Description='" + userInfo.description.Replace("'","''") + "'";
			if(userInfo.expiredDate == VLF.CLS.Def.Const.unassignedDateTime)
				sql += ", ExpiredDate=NULL";
			else
				sql += ", ExpiredDate='" + userInfo.expiredDate.ToShortDateString() + "'";
			sql += " WHERE UserId=" + userId;
			try
			{
            prefixMsg = "Unable to update user id " + userId + ". ";
				
				if(sqlExec.RequiredTransaction())
				{
					// 4. Attach current command SQL to transaction
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
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This user already exists.");
			}
		}		
		
      /// <summary>
		/// Updates user information.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userName"></param>
		/// <param name="expiredDate"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateInfo(int userId, string userName, DateTime expiredDate)
		{
			int rowsAffected = 0;
         string prefixMsg = "";
			//Prepares SQL statement
			string sql = "UPDATE vlfUser SET UserName='" + userName.Replace("'","''") + "'";
			if(expiredDate == VLF.CLS.Def.Const.unassignedDateTime)
				sql += ", ExpiredDate=NULL";
			else
				sql += ", ExpiredDate='" + expiredDate + "'";
			sql += " WHERE UserId=" + userId;
			try
			{
            prefixMsg = "Unable to update user name=" + userName + " by id " + userId + " .";
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
			if(rowsAffected == 0) 
			{
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This user already exists.");
			}
		}



        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="expiredDate"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateInfoStatus(int userId, string userName, DateTime expiredDate,string status)
        {
            int rowsAffected = 0;
            string prefixMsg = "";
            //Prepares SQL statement
            string sql = "UPDATE vlfUser SET UserName='" + userName.Replace("'", "''") + "'";
            if (expiredDate == VLF.CLS.Def.Const.unassignedDateTime)
                sql += ", ExpiredDate=NULL";
            else
                sql += ", ExpiredDate='" + expiredDate + "'";

            sql += ", UserStatus='" + status + "'";

            sql += " WHERE UserId=" + userId;
            try
            {
                prefixMsg = "Unable to update user name=" + userName + " by id " + userId + " .";
                //Executes SQL statement
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
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(prefixMsg + " This user already exists.");
            }
        }	

        /// <summary>
		/// Updates user information.
		/// </summary>
		/// <param name="userInfo"></param>
		/// <param name="userName"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user driver license or user name alredy exists.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateInfo(UserInfo userInfo, string userName)
		{
			int userId = GetUserIdByUserName(userName);
			UpdateInfo(userInfo,userId);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        public void UpdateUserInfo(UserInfo userInfo)
        {
            // 1. validates parameters
            if (userInfo.userId == VLF.CLS.Def.Const.unassignedIntValue ||
                String.IsNullOrEmpty(userInfo.personId) ||
            String.IsNullOrEmpty(userInfo.hashpassword) ||
                (userInfo.organizationId == VLF.CLS.Def.Const.unassignedIntValue))
            {
                throw new DASAppInvalidValueException("Wrong value for insert SQL: UserId=" +
                    userInfo.userId +
                    //" Password=" + userInfo.password + 
                    " PersonId=" + userInfo.personId +
                    " OrganizationId=" + userInfo.organizationId);
            }
            string prefixMsg = "";
            int rowsAffected = 0;
            //Prepares SQL statement
            string sql = "UPDATE vlfUser SET PersonId='" + userInfo.personId.Replace("'", "''") + "'";
            
            sql += " ,username ='" + userInfo.username + "'" +
                " ,HashPassword='" + userInfo.hashpassword.Replace("'", "''") + "'" +
                ", OrganizationId=" + userInfo.organizationId +
                ", PIN='" + userInfo.pin.Replace("'", "''") + "'" +
                ", Description='" + userInfo.description.Replace("'", "''") + "'";
            if (userInfo.expiredDate == VLF.CLS.Def.Const.unassignedDateTime)
                sql += ", ExpiredDate=NULL";
            else
                sql += ", ExpiredDate='" + userInfo.expiredDate.ToShortDateString() + "'";
            sql += " WHERE UserId=" + userInfo.userId;
            try
            {
                prefixMsg = "Unable to update user id " + userInfo.userId + ". ";

                if (sqlExec.RequiredTransaction())
                {
                    // 4. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                //Executes SQL statement
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
            if (rowsAffected == 0)
            {
                throw new DASAppDataAlreadyExistsException(prefixMsg + "user does not exist.");
            }
        }


		/// <summary>
		/// Deletes existing user.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if user with driver license does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserByDriverLicense(string driverLicense)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM vlfUser WHERE PersonId=(SELECT PersonId FROM vlfPersonInfo WHERE DriverLicense='" + driverLicense.Replace("'","''") + "')";
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
				string prefixMsg = "Unable to delete user by driver license =" + driverLicense;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete user by driver license =" + driverLicense;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		
        /// <summary>
		/// Deletes existing user.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="userName"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if user with user name not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserByUserName(string userName)
		{
			return DeleteRowsByStrField("UserName",userName, "user name");		
		}
		
        /// <summary>
		/// Deletes existing user.
		/// </summary>
		/// <returns>rows affected</returns>
		/// <param name="userId"></param> 
		/// <exception cref="DASAppResultNotFoundException">Thrown if user with user id not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserByUserId(int userId)
		{
			return DeleteRowsByIntField("UserId", userId, "user id");		
		}
		
        /// <summary>
		/// Retrieves user contact info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [Birthday],[PIN],[Address],[City],[StateProvince],[Country],[PhoneNo1],[PhoneNo2],[CellNo]
		/// </returns>
		/// <param name="userId"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetContactInfoByUserId(int userId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT Birthday,PIN,Address,City,StateProvince,Country,PhoneNo1,PhoneNo2,CellNo" +
					" FROM vlfPersonInfo,vlfUser" + 
					" WHERE UserId=" + userId +
					" AND vlfUser.PersonId=vlfPersonInfo.PersonId";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <returns>
        /// DataSet 
        /// [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],
        /// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
        /// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]
        /// </returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserInfoByUserId_NewTZ(int userId)
        {
            DataSet sqlDataSet = null;

            if (userId < 0)
                return null;

            try
            {

                //Prepares SQL statement 

                string sql = "DECLARE @Timezone float" +
                          " DECLARE @DayLightSaving int" +
                          " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                          " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                          " IF @Timezone IS NULL SET @Timezone=0" +
                          " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                          " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                          " SET @Timezone= @Timezone + @DayLightSaving" +
             " SELECT vlfUser.UserId,UserName,Password,vlfPersonInfo.PersonId,DriverLicense,FirstName,LastName" +
                  ",vlfOrganization.OrganizationId,Birthday,PIN,vlfPersonInfo.Address,City,StateProvince,Country" +
             ",PhoneNo1,PhoneNo2,CellNo,vlfUser.Description,OrganizationName,vlfUserGroupAssignment.UserGroupId,HashPassword, IsNull(vlfOrganization.SuperOrganizationId,1) as SuperOrganizationId " +
             ",CASE WHEN ExpiredDate IS NULL then 'Unlimited' else convert(varchar,DATEADD(minute,(@Timezone * 60),ExpiredDate),101) END AS ExpiredDate, ISNULL(FleetPulseURL,'') as FleetPulseURL,ISNULL(HOSenabled,'False') as  HOSenabled,ISNULL(ParentUserGroupId,vlfUserGroup.UserGroupId) as ParentUserGroupId " +
             " FROM vlfUser INNER JOIN " +
              "vlfOrganization ON vlfUser.OrganizationId=vlfOrganization.OrganizationId INNER JOIN " +
              "vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId LEFT JOIN " +
              "vlfUserGroupAssignment ON vlfUser.UserId=vlfUserGroupAssignment.UserId LEFT JOIN " +
              "vlfUserGroup ON vlfUserGroupAssignment.UserGroupId=vlfUserGroup.UserGroupId " +
              "WHERE vlfUser.UserId=" + userId;

                //" FROM vlfOrganization,vlfPersonInfo,vlfUser,vlfUserGroupAssignment,vlfUserGroup" +
                //" WHERE vlfUser.UserId=" + userId +
                //" AND vlfUser.OrganizationId=vlfOrganization.OrganizationId" +
                //" AND vlfUser.UserId=vlfUserGroupAssignment.UserId" +
                //" AND vlfUserGroupAssignment.UserGroupId=vlfUserGroup.UserGroupId" +
                //" AND vlfUser.PersonId=vlfPersonInfo.PersonId";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <returns>
        /// DataSet 
        /// [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],
        /// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
        /// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]
        /// </returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserInfoByUserId(int userId)
        {
            DataSet sqlDataSet = null;

            if (userId < 0)
                return null;

            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                            " IF @Timezone IS NULL SET @Timezone=0" +
                            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                            " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                            " SET @Timezone= @Timezone + @DayLightSaving" +
               " SELECT vlfUser.UserId,UserName,vlfUser.Email as Email,vlfOrganization.isDisclaimer as isDisclaimer,Password,vlfPersonInfo.PersonId,DriverLicense,FirstName,LastName" +
                    ",vlfOrganization.OrganizationId,Birthday,PIN,vlfPersonInfo.Address,City,StateProvince,Country" +
               ",PhoneNo1,PhoneNo2,CellNo,vlfUser.Description,OrganizationName,vlfUserGroupAssignment.UserGroupId,HashPassword, IsNull(vlfOrganization.SuperOrganizationId,1) as SuperOrganizationId " +
               ",CASE WHEN ExpiredDate IS NULL then 'Unlimited' else convert(varchar,DATEADD(hour,@Timezone,ExpiredDate),101) END AS ExpiredDate, ISNULL(FleetPulseURL,'') as FleetPulseURL,ISNULL(HOSenabled,'False') as  HOSenabled,ISNULL(ParentUserGroupId,vlfUserGroup.UserGroupId) as ParentUserGroupId " +
               " FROM vlfUser INNER JOIN " +
                "vlfOrganization ON vlfUser.OrganizationId=vlfOrganization.OrganizationId INNER JOIN " +
                "vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId LEFT JOIN " +
                "vlfUserGroupAssignment ON vlfUser.UserId=vlfUserGroupAssignment.UserId LEFT JOIN " +
                "vlfUserGroup ON vlfUserGroupAssignment.UserGroupId=vlfUserGroup.UserGroupId " +
                "WHERE vlfUser.UserId=" + userId;
                //" FROM vlfOrganization,vlfPersonInfo,vlfUser,vlfUserGroupAssignment,vlfUserGroup" +
                //" WHERE vlfUser.UserId=" + userId +
                //" AND vlfUser.OrganizationId=vlfOrganization.OrganizationId" +
                //" AND vlfUser.UserId=vlfUserGroupAssignment.UserId" +
                //" AND vlfUserGroupAssignment.UserGroupId=vlfUserGroup.UserGroupId" +
                //" AND vlfUser.PersonId=vlfPersonInfo.PersonId";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Return only the user information
        /// do not include person info or organization
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetUserInfoById(int userId)
        {
            DataSet sqlDataSet = null;

            if (userId < 0)
                return null;

            try
            {
                //Prepares SQL statement
                string sql = "SELECT * FROM vlfUser WHERE UserId=" + userId;
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve info by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Gets person id for the user
        /// </summary>
        /// <param name="userId" type="int"></param>
        /// <returns>string Person ID</returns>
        public string GetPersonId(int userId)
        {
            string pid = "";
            try
            {
                pid = sqlExec.SQLExecuteScalar("SELECT PersonId FROM " + tableName + " WHERE UserId=" + userId.ToString()).ToString();
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve person id by user id=" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve person id by user id=" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return pid;
        }

        // Changes for TimeZone Feature start
        public object GetTimeZone(int userId)
        {
            string GetTimeZoneSQL;
            object objTimeZoneNew;
            DateTime dateTime = VLF.CLS.Def.Const.unassignedDateTime;
            GetTimeZoneSQL = "DECLARE @Timezone float" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                            " SELECT @Timezone ";
            objTimeZoneNew = sqlExec.SQLExecuteScalar(GetTimeZoneSQL);
            if (objTimeZoneNew == DBNull.Value)
            {
                GetTimeZoneSQL = "DECLARE @Timezone float" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                            " SELECT @Timezone ";
                objTimeZoneNew = sqlExec.SQLExecuteScalar(GetTimeZoneSQL);
            }
            return objTimeZoneNew;
        }
    // Changes for TimeZone Feature end


        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user expiration datetime 
        /// </summary>
        /// <returns></returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DateTime GetUserExpirationDate_NewTZ(int userId)
        {
            DateTime dateTime = VLF.CLS.Def.Const.unassignedDateTime;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @Timezone float" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=CAST(PreferenceValue as FLOAT) FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT CASE WHEN ExpiredDate IS NOT NULL then DATEADD(minute,(@Timezone * 60),ExpiredDate) END AS ExpiredDate FROM vlfUser WHERE UserId=" + userId;

                //Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != DBNull.Value)
                    dateTime = Convert.ToDateTime(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve user id=" + userId + " expiration datetime. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve user id=" + userId + " expiration datetime. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dateTime;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user expiration datetime 
        /// </summary>
        /// <returns></returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DateTime GetUserExpirationDate(int userId)
        {
            DateTime dateTime = VLF.CLS.Def.Const.unassignedDateTime;
            try
            {
                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                    " DECLARE @DayLightSaving int" +
                    " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                    " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                    " IF @Timezone IS NULL SET @Timezone=0" +
                    " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                    " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                    " SET @Timezone= @Timezone + @DayLightSaving" +
                    " SELECT CASE WHEN ExpiredDate IS NOT NULL then DATEADD(hour,@Timezone,ExpiredDate) END AS ExpiredDate FROM vlfUser WHERE UserId=" + userId;
                //Executes SQL statement
                object obj = sqlExec.SQLExecuteScalar(sql);
                if (obj != DBNull.Value)
                    dateTime = Convert.ToDateTime(obj);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve user id=" + userId + " expiration datetime. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve user id=" + userId + " expiration datetime. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dateTime;
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserInfoByUserName_NewTZ(string userName)
        {
            return GetUserInfoByUserId_NewTZ(GetUserIdByUserName(userName));
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserInfoByUserName(string userName)
        {
            return GetUserInfoByUserId(GetUserIdByUserName(userName));
        }		
		
        /// <summary>
		/// Retrieves user info
		/// </summary>
		/// <returns>
		/// DataSet 
		/// [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],
		/// [OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],
		/// [Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description]
		/// </returns>
		/// <param name="driverLicense"></param> 
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserInfoByDriverLicense(string driverLicense)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT UserId,UserName,Password,vlfPersonInfo.PersonId,DriverLicense,FirstName,LastName" +
					",vlfOrganization.OrganizationId,Birthday,PIN,vlfPersonInfo.Address,City,StateProvince,Country" +
					",PhoneNo1,PhoneNo2,CellNo,vlfUser.Description,OrganizationName" +
					" FROM vlfOrganization,vlfPersonInfo,vlfUser" + 
					" WHERE vlfPersonInfo.DriverLicense='" + driverLicense.Replace("'","''") + "'" +
					" AND vlfPersonInfo.PersonId=vlfUser.PersonId" +
					" AND vlfUser.OrganizationId=vlfOrganization.OrganizationId";
					
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve info by driver license=" + driverLicense + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        /// <summary>
        /// Returns user Id by organization id 
        /// </summary>
        /// <param name="userId"></param> 
        /// <returns>int</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationUsers(int orgId)
        {
            //int userId = VLF.CLS.Def.Const.unassignedIntValue;
            DataSet dsResult = new DataSet();
            GetUserInfoFieldBy("OrganizationId", "UserId", orgId, ref dsResult);
            return dsResult;
        }

        /// <summary>
		/// Returns organization id by user Id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetOrganizationIdByUserId(int userId)
		{
			int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
			object result = GetUserInfoFieldBy("UserId","OrganizationId",userId);
			if(result != null)
			{
				organizationId = Convert.ToInt32(result);
			}
			return organizationId;
		}		
		
        /// <summary>
		/// Returns user name by user Id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetUserNameByUserId(int userId)
		{
			string userName = VLF.CLS.Def.Const.unassignedStrValue;
			object result = GetUserInfoFieldBy("UserId","UserName",userId);
			if(result != null)
			{
				userName = Convert.ToString(result).TrimEnd();
			}
			return userName;
		}		
		
        /// <summary>
		/// Returns user name by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetUserNameByDriverLicense(string driverLicense)
		{
			string userName = VLF.CLS.Def.Const.unassignedStrValue;
			object result = GetUserInfoFieldByDriverLicense("UserName",driverLicense);
			if(result != null)
			{
				userName = Convert.ToString(result).TrimEnd();
			}
			return userName;
		}		
		
        /// <summary>
		/// Returns organization id by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetOrganizationIdByDriverLicense(string driverLicense)
		{
			int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
			object result = GetUserInfoFieldByDriverLicense("OrganizationId",driverLicense);
			if(result != null)
			{
				organizationId = Convert.ToInt32(result);
			}
			return organizationId;
		}		
		
        /// <summary>
		/// Returns password by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <remarks> "" in case of invalid data, otherwise password </remarks>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetPasswordByDriverLicense(string driverLicense)
		{
			string password = VLF.CLS.Def.Const.unassignedStrValue;
			object result = GetUserInfoFieldByDriverLicense("Password",driverLicense);
			if(result != null)
			{
				password = Convert.ToString(result).TrimEnd();
			}
			return password;
		}		
		
        /// <summary>
		/// Returns user id by user name. 	
		/// </summary>
		/// <param name="userName"></param> 
		/// <remarks> -1 in case of invalid data, otherwise userId </remarks>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetUserIdByUserName(string userName)
		{

            string prefixMsg = string.Format("GetUserIdByUserName EXC: {0}", userName);

            int userId = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserName", SqlDbType.VarChar, userName, 50);

                object obj = sqlExec.SPExecuteScalar("ValidateUserByUserName");
                if (obj != null)
                    userId = Convert.ToInt32(obj);
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
            return userId;
		}
		
        /// <summary>
		/// Returns user id by driver license. 	
		/// </summary>
		/// <param name="driverLicense"></param> 
		/// <returns>int</returns>
		/// <remarks> -1 in case of invalid data, otherwise userId </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int GetUserIdByDriverLicense(string driverLicense)
		{
			int userId = VLF.CLS.Def.Const.unassignedIntValue;
			object result = GetUserInfoFieldByDriverLicense("UserId",driverLicense);
			if(result != null)
			{
				userId = Convert.ToInt32(result);
			}
			return userId;
		}		
		
        /// <summary>
		/// Returns driver license by user name. 	
		/// </summary>
		/// <param name="userName"></param> 
		/// <returns>string</returns>
		/// <remarks> "" in case of invalid data, otherwise driver license </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetDriverLicenseByUserName(string userName)
		{
			string driverLicense = VLF.CLS.Def.Const.unassignedStrValue;			
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DriverLicense" +
					" FROM vlfPersonInfo,vlfUser" + 
					" WHERE vlfUser.UserName='" + userName.Replace("'","''") + "'" +
					" AND vlfUser.PersonId=vlfPersonInfo.PersonId";
					
				//Executes SQL statement
				driverLicense = Convert.ToString(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve driver license by user name=" + userName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve driver license by user name=" + userName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return driverLicense;
		}		
		
        /// <summary>
		/// Returns driver license by user id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>stru=ing</returns>
		/// <remarks> "" in case of invalid data, otherwise driver license </remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetDriverLicenseByUserId(int userId)
		{
			string driverLicense = VLF.CLS.Def.Const.unassignedStrValue;			
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DriverLicense" +
					" FROM vlfPersonInfo,vlfUser" + 
					" WHERE vlfUser.UserId=" + userId +
					" AND vlfUser.PersonId=vlfPersonInfo.PersonId";
					
				//Executes SQL statement
				driverLicense = Convert.ToString(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve driver license by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve driver license by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return driverLicense;
		}		

		/// <summary>
		/// Retrieves users boxes by user id. 	
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>ArrayList [int]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetUserBoxes(int userId)
		{
			ArrayList arrBoxes = null;			
			DataSet dsBoxes = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT DISTINCT vlfVehicleAssignment.BoxId"+
							" FROM vlfFleetUsers INNER JOIN vlfUser ON vlfFleetUsers.UserId=vlfUser.UserId"+
							" INNER JOIN vlfFleetVehicles ON vlfFleetUsers.FleetId=vlfFleetVehicles.FleetId"+
							" INNER JOIN vlfVehicleAssignment ON vlfFleetVehicles.VehicleId=vlfVehicleAssignment.VehicleId"+
							" WHERE vlfUser.UserId="+ userId;
					
				//Executes SQL statement
				dsBoxes = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve users boxes by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve users boxes by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(dsBoxes != null && dsBoxes.Tables.Count > 0 && dsBoxes.Tables[0].Rows.Count > 0)
			{
				arrBoxes = new ArrayList();
				foreach(DataRow ittr in dsBoxes.Tables[0].Rows)
				{
					arrBoxes.Add(Convert.ToInt32(ittr["BoxId"]));
				}
			}
			return arrBoxes;
		}				
		
        /// <summary>
		/// Updates password by user id
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="password"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if unabled to update information.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetPasswordByUserId(string password, int userId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET Password='" + password.Replace("'","''") + 
					"' WHERE UserId=" + userId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set Password=" + password + " by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set Password=" + password + " by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set Password=" + password + " by user id=" + userId + ". ";
				throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " Wrong user id='" + userId  + "'.");
			}
		}



      /// <summary>
      /// Updates password by user id
      /// </summary>
      /// <param name="userId"></param> 
      /// <param name="password"></param> 
      /// <returns>void</returns>
      /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if unabled to update information.</exception>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public void SetHashPasswordByUserId(string password, int userId)
      {
         int rowsAffected = 0;
         try
         {
            //Prepares SQL statement
            string sql = "UPDATE " + tableName +
               " SET HashPassword='" + password.Replace("'", "''") +
                             "', UserStatus='Locked' WHERE UserId=" + userId;
            //Executes SQL statement
            rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
         }
         catch (SqlException objException)
         {
            string prefixMsg = "Unable to set Password=" + password + " by user id=" + userId + ". ";
            Util.ProcessDbException(prefixMsg, objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            string prefixMsg = "Unable to set Password=" + password + " by user id=" + userId + ". ";
            throw new DASException(prefixMsg + " " + objException.Message);
         }

         //Throws exception in case of wrong result
         if (rowsAffected == 0)
         {
            string prefixMsg = "Unable to set Password=" + password + " by user id=" + userId + ". ";
            throw new DASAppViolatedIntegrityConstraintsException(prefixMsg + " Wrong user id='" + userId + "'.");
         }
      }
		/// <summary>
		/// Updates organization id by user id
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="organizationId"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetOrganizationIdByUserId(int organizationId, int userId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET OrganizationId=" + organizationId + " WHERE UserId=" + userId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set OrganizationId=" + organizationId + " by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set OrganizationId=" + organizationId + " by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set OrganizationId=" + organizationId + " by user id=" + userId + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user id='" + userId + ".");
			}
		}

		/// <summary>
		/// Updates user name by user id
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="userName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetUserNameByUserId(string userName, int userId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET UserName='" + userName.Replace("'","''") + 
					"' WHERE UserId=" + userId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to update user name=" + userName + " by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to update user name=" + userName + " by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to update user name=" + userName + " by user id=" + userId + ". ";
				throw new DASAppResultNotFoundException(prefixMsg);
			}
		}

		/// <summary>
		/// Updates organization id by user name
		/// </summary>
		/// <param name="userName"></param> 
		/// <param name="organizationId"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetOrganizationIdByUserName(int organizationId, string userName)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET OrganizationId=" + organizationId + 
					" WHERE UserName='" + userName.Replace("'","''") + "'";
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set OrganizationId=" + organizationId + " by user name=" + userName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set OrganizationId=" + organizationId + " by user name=" + userName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set OrganizationId=" + organizationId + " by user name=" + userName + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user name='" + userName + "'.");
			}
		}

		/// <summary>
		/// Updates password by user name
		/// </summary>
		/// <param name="userName"></param> 
		/// <param name="password"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetPasswordByUserName(string password, string userName)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET Password='" + password.Replace("'","''") + "'" +
					" WHERE UserName='" + userName.Replace("'","''") + "'";
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set Password=" + password + " by user name=" + userName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set Password=" + password + " by user name=" + userName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set Password=" + password + " by user name=" + userName + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user name='" + userName + "'.");
			}
		}

		/// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="resultFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected object GetUserInfoFieldBy(string searchFieldName, string resultFieldName, int searchFieldValue)
		{
			object resultFieldValue = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + resultFieldName + " FROM " + tableName + 
							" WHERE " + searchFieldName + "=" + searchFieldValue;
				//Executes SQL statement
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				resultFieldValue = sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultFieldValue;
		}

        /// <summary>
        /// Retrieves user info
        /// </summary>
        /// <param name="searchFieldName"></param>
        /// <param name="resultFieldName"></param>
        /// <param name="searchFieldValue"></param>
        /// <param name="dsResult" type="DataSet"></param>
        protected void GetUserInfoFieldBy(string searchFieldName, string resultFieldName, int searchFieldValue, ref DataSet dsResult)
        {
            try
            {
                //Prepares SQL statement
                string sql = "SELECT " + resultFieldName + " FROM " + tableName +
                            " WHERE " + searchFieldName + "=" + searchFieldValue;
                //Executes SQL statement
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }
	
        /// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="searchFieldName"></param> 
		/// <param name="resultFieldName"></param> 
		/// <param name="searchFieldValue"></param> 
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		protected object GetUserInfoFieldBy(string searchFieldName, string resultFieldName, string searchFieldValue)
		{
			object resultFieldValue = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + resultFieldName + " FROM " + tableName + 
					" WHERE (" + searchFieldName + "='" + searchFieldValue.Replace("'","''") + "')";
				//Executes SQL statement
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				resultFieldValue = sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by " + searchFieldName + "=" + searchFieldValue + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultFieldValue;
		}
	
        /// <summary>
		/// Retrieves user info
		/// </summary>
		/// <param name="resultFieldName"></param>
		/// <param name="driverLicense"></param>
		/// <returns>object</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		/// <returns></returns>
		protected object GetUserInfoFieldByDriverLicense(string resultFieldName, string driverLicense)
		{
			object resultFieldValue = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT " + resultFieldName + 
							" FROM vlfPersonInfo,vlfUser" +
							" WHERE vlfPersonInfo.DriverLicense='" + driverLicense.Replace("'","''") + "'" +
							" AND vlfPersonInfo.PersonId=vlfUser.PersonId";
				//Executes SQL statement
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attach current command SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				resultFieldValue = sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by driver license=" + driverLicense + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve " + resultFieldName + " by driver license=" + driverLicense + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultFieldValue;
		}

		#region Authorization functions


      /// <summary>
      ///         returns userId OR
      ///				-1 if the user is not found
      ///				-2 if the user ExpiredDate has been reached
      /// </summary>
      /// <param name="userName"></param>
      /// <param name="password"></param>
      /// <param name="hashPassword"></param>
      /// <returns></returns>
      private int ValidateUser(string userName, string password, bool hashPassword)
      {
         string prefixMsg = string.Format("ValidateUser EXC: {0}, {1}, {2}",
                                          userName, password, hashPassword);

         int userId = VLF.CLS.Def.Const.unassignedIntValue;
         try
         {
            sqlExec.ClearCommandParameters();
/*
            SqlParameter[] paramList = { new SqlParameter("@UserName", SqlDbType.VarChar,50, userName),
                                         new SqlParameter("@UserPassword", SqlDbType.VarChar, 256, password),
                                         new SqlParameter("@IsHashPassword", hashPassword)
                                       } ;
*/                  
            sqlExec.AddCommandParam("@UserName", SqlDbType.VarChar, userName, 50);
            sqlExec.AddCommandParam("@UserPassword", SqlDbType.VarChar, password, 256);
            sqlExec.AddCommandParam("@IsHashPassword", SqlDbType.Bit, hashPassword);

            object obj = sqlExec.SPExecuteScalar("ValidateUser");
            if (obj != null) 
                  userId = Convert.ToInt32(obj);
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
         return userId;
      }
        /// <summary>
        /// Validate user. 	
        /// </summary>
        /// <param name="userName"></param> 
        /// <param name="password"></param> 
        /// <remarks> -1 in case of invalid data, otherwise userId </remarks>
        /// <returns>user id</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int ValidateUser(string userName, string password)
        {
           return ValidateUser(userName, password, false);
        }


        /// <summary>
        /// Validate user. 	
        /// </summary>
        /// <param name="userName"></param> 
        /// <param name="password"></param> 
        /// <remarks> -1 in case of invalid data, otherwise userId </remarks>
        /// <returns>user id</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int ValidateUserMD5(string userName, string password)
        {
           return ValidateUser(userName, password, true);
        }

        /// <summary>
		/// Validate user alarm. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="alarmId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserAlarm(int userId,int alarmId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT COUNT(*) FROM vlfFleetUsers INNER JOIN vlfUser ON vlfFleetUsers.UserId = vlfUser.UserId INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId INNER JOIN vlfAlarm INNER JOIN vlfBox with (nolock) ON vlfAlarm.BoxId = vlfBox.BoxId INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId INNER JOIN vlfFleetVehicles ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId ON vlfFleet.FleetId = vlfFleetVehicles.FleetId WHERE vlfUser.UserId=" + userId + " AND vlfAlarm.AlarmId=" + alarmId;
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " alarm=" + alarmId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " alarm=" + alarmId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user fleet. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="fleetId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserFleet(int userId,int fleetId)
		{
			bool retResult = false;
			try
			{
				/*
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfUser INNER JOIN vlfFleetUsers ON vlfUser.UserId = vlfFleetUsers.UserId INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId WHERE vlfUser.UserId=" + userId + " AND vlfFleet.FleetId=" + fleetId;
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
				*/
				
				sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@FleetId", SqlDbType.Int, fleetId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
				
				if((int)sqlExec.SPExecuteScalar("sp_ValidateUserFleet") > 0)
					retResult = true;
                
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " fleet=" + fleetId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " fleet=" + fleetId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user organization. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="organizationId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserOrganization(int userId,int organizationId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfOrganization INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId WHERE vlfUser.UserId=" + userId + " AND vlfOrganization.OrganizationId=" + organizationId;
                /*WISAM
                 * string sql = " SELECT COUNT(*) FROM vlfOrganization " +
                             " INNER JOIN vlfUser ON (vlfOrganization.OrganizationId = vlfUser.OrganizationId) or " +
                             " (vlfOrganization.SuperOrganizationId = vlfUser.OrganizationId) " +
                             " WHERE vlfUser.UserId= " + userId + " AND vlfOrganization.OrganizationId= " + organizationId;
                */
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " organization=" + organizationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " organization=" + organizationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}
      /// <summary>
      /// Validate user driver
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="driverId"></param>
      /// <returns>true if valid, otherwise returns false</returns>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool ValidateUserDriver(int userId, int driverId)
      {
         string prefixMsg = String.Format("Unable to validate user={0} driver={1}.", userId, driverId);
         bool result = false;
         try
         {
            //Prepares SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);
            string sql = "SELECT COUNT(*) FROM vlfUser INNER JOIN vlfDriver ON vlfDriver.OrganizationId = vlfUser.OrganizationId WHERE vlfUser.UserId=@userId AND vlfDriver.DriverId=@driverId";
            /*WISAM
             * string sql = " SELECT COUNT(*) FROM vlfUser " +
                    " INNER JOIN vlfOrganization ON (vlfOrganization.OrganizationId = vlfUser.OrganizationId) or " +
                    " (vlfOrganization.SuperOrganizationId = vlfUser.OrganizationId) " +
                    " INNER JOIN vlfDriver ON vlfDriver.OrganizationId = vlfOrganization.OrganizationId " +                     
                    " WHERE vlfUser.UserId=@userId AND vlfDriver.DriverId=@driverId";
            */
             
             // Executes SQL statement and return true if result > 0
            result = ((int)sqlExec.SQLExecuteScalar(sql) > 0);
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
         return result;
      }
      /// <summary>
		/// Validate user vehicle. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="vehicleId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserVehicle(int userId,Int64 vehicleId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfFleetUsers INNER JOIN vlfUser ON vlfFleetUsers.UserId = vlfUser.UserId INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId INNER JOIN vlfFleetVehicles INNER JOIN vlfVehicleInfo ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId ON vlfFleet.FleetId = vlfFleetVehicles.FleetId WHERE vlfUser.UserId=" + userId + " AND vlfVehicleInfo.VehicleId=" + vehicleId;
                /*WISAM
                 * string sql = " SELECT COUNT(*) FROM vlfVehicleInfo vi " +
             " INNER JOIN vlfOrganization org ON (vi.OrganizationId = org.OrganizationId) " +
             " INNER JOIN vlfUser usr ON ( org.OrganizationId = usr.OrganizationId) OR ( usr.OrganizationId = org.SuperOrganizationId) " +
             " WHERE vi.VehicleId = " + vehicleId +
             " AND usr.UserId = " + userId;
                */
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " vehicle=" + vehicleId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " vehicle=" + vehicleId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user license plate. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="licensePlate"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserLicensePlate(int userId,string licensePlate)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfFleetUsers INNER JOIN vlfUser ON vlfFleetUsers.UserId = vlfUser.UserId INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId INNER JOIN vlfFleetVehicles INNER JOIN vlfVehicleInfo ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId ON vlfFleet.FleetId = vlfFleetVehicles.FleetId INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId WHERE vlfUser.UserId =" + userId + " AND vlfVehicleAssignment.LicensePlate='" + licensePlate + "'";

				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " licensePlate=" + licensePlate + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " licensePlate=" + licensePlate + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user box Id. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="boxId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserBox(int userId,int boxId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
                /// wisam: Replace the SQL, since this will not give access to the user if the box is not assigned to a vehicle int a fleet
                /// TODO: must include all organizations using the super organization also!.
                /// 
				//string sql = "SELECT COUNT(*) FROM vlfFleetUsers INNER JOIN vlfUser ON vlfFleetUsers.UserId = vlfUser.UserId INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId INNER JOIN vlfFleetVehicles INNER JOIN vlfVehicleInfo ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId ON vlfFleet.FleetId = vlfFleetVehicles.FleetId INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId WHERE vlfUser.UserId=" + userId + " AND vlfVehicleAssignment.BoxId=" + boxId;

            string sql = "SELECT COUNT(*) FROM vlfBox with (nolock) INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId INNER JOIN vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId WHERE vlfBox.BoxId = " + boxId + " AND vlfUser.UserId=" + userId;
                /*WISAM
                 * string sql = "SELECT COUNT(*) FROM vlfBox " + 
                             " INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId " +
                             " INNER JOIN vlfUser ON (vlfOrganization.OrganizationId = vlfUser.OrganizationId) " +
                             "  or (vlfOrganization.SuperOrganizationId = vlfUser.OrganizationId) " +
                             " WHERE vlfBox.BoxId = " + boxId + " AND vlfUser.UserId = " + userId ;
                */

				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " boxId=" + boxId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " boxId=" + boxId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user msg. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="msgId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserMsg(int userId,int msgId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfFleetUsers INNER JOIN vlfUser ON vlfFleetUsers.UserId = vlfUser.UserId INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId INNER JOIN vlfFleetVehicles INNER JOIN vlfVehicleInfo ON vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId ON vlfFleet.FleetId = vlfFleetVehicles.FleetId INNER JOIN vlfVehicleAssignment ON vlfVehicleInfo.VehicleId = vlfVehicleAssignment.VehicleId INNER JOIN vlfTxtMsgs ON vlfVehicleAssignment.BoxId = vlfTxtMsgs.BoxId WHERE vlfUser.UserId=" + userId + " AND vlfTxtMsgs.MsgId=" + msgId;
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " msgId=" + msgId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " msgId=" + msgId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user organization name. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="organizationName"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserOrganizationName(int userId,string organizationName)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfUser INNER JOIN vlfOrganization ON vlfUser.OrganizationId = vlfOrganization.OrganizationId WHERE vlfUser.UserId=" + userId + " AND vlfOrganization.OrganizationName = '" + organizationName + "'";
                /*WISAM
                 * string sql = "SELECT COUNT(*) FROM vlfUser " +
                             " INNER JOIN vlfOrganization ON (vlfUser.OrganizationId = vlfOrganization.OrganizationId) or " +
                             " (vlfUser.OrganizationId = vlfOrganization.SuperOrganizationId) "+
                             " WHERE vlfUser.UserId= " + userId + " AND vlfOrganization.OrganizationName = '" + organizationName +"'";
                */

				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " organization name=" + organizationName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " organization name=" + organizationName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate HGI super user. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateHGISuperUser(int userId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId WHERE vlfUser.UserId=" + userId + "  AND vlfUserGroupAssignment.UserGroupId=1";
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate HGISuper user="+ userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate HGISuper user="+ userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate super user. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateSuperUser(int userId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT COUNT(*) "; 
                sql = sql + "FROM vlfUser INNER JOIN "; 
	            sql = sql + "vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId INNER JOIN ";
	            sql = sql + "vlfUserGroup ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId ";
                sql = sql + "WHERE vlfUser.UserId=" + userId.ToString();
                sql = sql + " AND (vlfUserGroupAssignment.UserGroupId IN (1, 2, 7, 14) OR vlfUserGroup.ParentUserGroupId IN (1, 2, 7, 14))";
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate super user="+ userId.ToString() + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate super user="+ userId.ToString() + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;

		}	
		/// <summary>
		/// Validate user group security. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserGroupSecurity(int userId,int userGroupId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfUser INNER JOIN vlfUserGroupAssignment ON vlfUser.UserId = vlfUserGroupAssignment.UserId WHERE vlfUser.UserId=" + userId + " AND vlfUserGroupAssignment.UserGroupId=" + userGroupId;
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " userGroupId=" + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " userGroupId=" + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user preference. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <returns>true if valid, otherwise returns false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool ValidateUserPreference(int userId,int preferenceId)
		{
			bool retResult = false;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(*) FROM vlfUser INNER JOIN vlfUserPreference ON vlfUser.UserId = vlfUserPreference.UserId WHERE vlfUser.UserId=" + userId + " AND vlfUserPreference.PreferenceId=" + preferenceId;
				//Executes SQL statement
				if((int)sqlExec.SQLExecuteScalar(sql) > 0)
					retResult = true;
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to validate user="+ userId + " preferenceId=" + preferenceId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " preferenceId=" + preferenceId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return retResult;
		}	
		/// <summary>
		/// Validate user call. 	
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="methodName"></param>
		/// <exception cref="DASAuthorizationException">Thrown if te user is unauthorized to use this method.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void ValidateUserCall(int userId, string methodName, string methodClass)
		{
			bool retResult = false;
			try
			{
                //Add for Temporary changes
                retResult = true;
                ////Prepares SQL statement
                //string sql = "SELECT COUNT(*) FROM vlfUserGroupAssignment INNER JOIN vlfGroupSecurity ON vlfUserGroupAssignment.UserGroupId = vlfGroupSecurity.UserGroupId INNER JOIN vlfWebMethods ON vlfGroupSecurity.OperationId = vlfWebMethods.MethodId WHERE vlfUserGroupAssignment.UserId=" + 
                //    userId + " AND vlfGroupSecurity.OperationType=" + 
                //    (short)VLF.CLS.Def.Enums.OperationType.WebMethod + 
                //    " AND vlfWebMethods.MethodName='" + methodName + "' AND vlfWebMethods.MethodClass='" + 
                //    methodClass + "'";
                ////Executes SQL statement
                //if((int)sqlExec.SQLExecuteScalar(sql) > 0)
                //    retResult = true;
                //else
                //    Util.BTrace(Util.INF0, "ValidateUserCall Failed -> {0} {1} {2}", userId, methodName, methodClass);

			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to authorize user="+ userId + " method name=" + methodName + " methodClass=" + methodClass + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to validate user="+ userId + " method name=" + methodName + " methodClass=" + methodClass + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(retResult == false)
				throw new DASAuthorizationException("User " + userId + " unauthorized using " + methodName + " methodClass=" + methodClass + ".");
		}

        /// <summary>
        /// Add/Update User Preference
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="preferenceId"></param>
        /// <param name="preferenceValue"></param>
        public int UserPreference_Add_Update(int userId, int preferenceId, string preferenceValue)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@PreferenceId", SqlDbType.Int, preferenceId);
                sqlExec.AddCommandParam("@PreferenceValue", SqlDbType.VarChar, preferenceValue);

                rowsAffected = sqlExec.SPExecuteNonQuery("sp_UserPreference_Add_Update");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to add/update user preferences user=" + userId + " PreferenceId=" + preferenceId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = prefixMsg = "Unable to add/update user preferences user=" + userId + " PreferenceId=" + preferenceId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected; 
        }


        /// <summary>
        /// Validate GeoZone Updatable By User
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        public int ValidateGeoZoneUpdatableByUser(int organizationId, int userId, short geozoneId)
        {
            int isValid = -1;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@geozoneId", SqlDbType.SmallInt, geozoneId);

                isValid = Convert.ToInt32(sqlExec.SPExecuteScalar("usp_ValidateGZUpdatableByUser"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to validate GZ user=" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = prefixMsg = "Unable to validate GZ user=" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return isValid;
        }


        /// <summary>
        /// Validate Landmark Updatable By User
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        public int ValidateLandmarkUpdatableByUser(int organizationId, int userId, long landmarkId)
        {
            int isValid = -1;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@organizationId", SqlDbType.Int, organizationId);
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@landmarkId", SqlDbType.BigInt, landmarkId);

                isValid = Convert.ToInt32(sqlExec.SPExecuteScalar("usp_ValidateLMUpdatableByUser"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to validate LM user=" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = prefixMsg = "Unable to validate LM user=" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return isValid;
        }
		#endregion


        #region GlobalAuthorization

        public bool ValidateUserAccess(int userId, Enums.ValidationItem item, object value)
        {
            bool valid = false;

            string sql = "";
            switch (item)
            {
                case Enums.ValidationItem.Validate_Organization:
                    sql = " SELECT COUNT(*) FROM vlfOrganization " +
                         " INNER JOIN vlfUser ON (vlfOrganization.OrganizationId = vlfUser.OrganizationId) or " +
                         " (vlfOrganization.SuperOrganizationId = vlfUser.OrganizationId) " +
                         " WHERE vlfUser.UserId= " + userId.ToString() + " AND vlfOrganization.OrganizationId= " + value.ToString();
                    break;
                case Enums.ValidationItem.Validate_Box:
                    sql = " SELECT COUNT(*) FROM vlfBox with (nolock)" +
                         " INNER JOIN vlfOrganization ON vlfBox.OrganizationId = vlfOrganization.OrganizationId " +
                         " INNER JOIN vlfUser ON (vlfOrganization.OrganizationId = vlfUser.OrganizationId) " +
                         "              OR (vlfOrganization.SuperOrganizationId = vlfUser.OrganizationId) " +
                         " WHERE vlfBox.BoxId = " + value.ToString() +" AND vlfUser.UserId = " + userId.ToString();
                    break;
                case Enums.ValidationItem.Validate_Vehicle:
                    sql = " SELECT COUNT(*) FROM vlfVehicleInfo vi " +
                        " INNER JOIN vlfOrganization org ON (vi.OrganizationId = org.OrganizationId) " +
                        " INNER JOIN vlfUser usr ON ( org.OrganizationId = usr.OrganizationId) OR ( usr.OrganizationId = org.SuperOrganizationId) " +
                        " WHERE vi.VehicleId = " + value.ToString() + " AND usr.UserId = " + userId.ToString();
                    break;
                case Enums.ValidationItem.Validate_Driver:
                    sql = " SELECT COUNT(*) FROM vlfUser " +
                        " INNER JOIN vlfOrganization ON (vlfOrganization.OrganizationId = vlfUser.OrganizationId) or " +
                        " (vlfOrganization.SuperOrganizationId = vlfUser.OrganizationId) " +
                        " INNER JOIN vlfDriver ON vlfDriver.OrganizationId = vlfOrganization.OrganizationId " +
                        " WHERE vlfUser.UserId = " + userId.ToString() + " AND vlfDriver.DriverId = " + value.ToString();
                    break;
                case Enums.ValidationItem.Validate_Fleet:
                    sql = " SELECT COUNT(*) FROM vlfUser INNER JOIN vlfFleetUsers ON vlfUser.UserId = vlfFleetUsers.UserId " + 
                        " INNER JOIN vlfFleet ON vlfFleetUsers.FleetId = vlfFleet.FleetId " +
                        " WHERE vlfUser.UserId = " + userId.ToString() + " AND vlfFleet.FleetId = " + value.ToString();
                    break;
                default:
                    break;
            }

            if (!String.IsNullOrEmpty(sql))
            {
                try
                {
                    //Executes SQL statement
                    if ( (int)sqlExec.SQLExecuteScalar(sql) > 0 )
                        valid = true;
                }
                catch (SqlException objException)
                {
                    string prefixMsg = "Unable to validate user=" + userId + ", " + item.ToString() + " : " + value + ". ";
                    Util.ProcessDbException(prefixMsg, objException);
                }
                catch (DASDbConnectionClosed exCnn)
                {
                    throw new DASDbConnectionClosed(exCnn.Message);
                }
                catch (Exception objException)
                {
                    string prefixMsg = "Unable to validate user=" + userId + ", " + item.ToString() + " : " + value + ". ";
                    throw new DASException(prefixMsg + " " + objException.Message);
                }
            }

            return valid;
        }




        public void RetrieveLockedUser(int userId, ref DataSet dsResult)
        {
            try
            {
                string sql =
                    "SELECT UserStatus FROM dbo.vlfUser WHERE UserId=@userId";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);

                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to user id: " + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
        }

        public void SetUserStatus(int userId, string status)
        {
            int rowsAffected = 0;
            try
            {
                //Prepares SQL statement
                string sql = "UPDATE " + tableName +
                       " SET UserStatus='" + status + "' WHERE UserId=" + userId;
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to update user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Gets all active languages
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>XML File format: [LanguageID][CultureName][Language]</returns>
        public DataSet GetLanguages(int UserId)
        {
            DataSet sqlDataSet = new DataSet();
            //Prepares SQL statement
            try
            {
                string sql = "usp_Languages_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of languages.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of languages.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetDateTimeFormat()
        {
            DataSet sqlDataSet = new DataSet();
            //Prepares SQL statement
            try
            {
                string sql = "SELECT Format,Type FROM DateTimeFormat";
                

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of format.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of format.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        public DataSet GetDefaultDateTimeFormat(int organizationId)
        {
            DataSet sqlDataSet = new DataSet();
            //Prepares SQL statement
            try
            {
                string sql = "declare @defdateformat varchar(20) declare @deftimeformat varchar(20) set @defdateformat =  (select preferencevalue from vlfOrganizationdefaultpreference where preferenceid = 44 and organizationid=" + organizationId
                + ") set @deftimeformat =  (select preferencevalue from vlfOrganizationdefaultpreference where preferenceid = 45 and organizationid= " + organizationId + ") select isnull(@defdateformat,(select preferencevalue from vlfGlobalDefaultPreference where preferenceid = 44)) as DefDateFormat,"
                + "isnull(@deftimeformat,(select preferencevalue from vlfGlobalDefaultPreference where preferenceid = 45)) as DefTimeFormat";


                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of format.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of format.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
      
        #endregion    

        //Add Email
        public int AddEmail(int userId, string email)
        {
            int rowsAffected = 0;
            try
            {
                //Prepares SQL statement
                string sql = "UPDATE vlfUser" +
                    " SET Email= '" + email + "' WHERE UserId=" + userId;
                //Executes SQL statement
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
                
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to set EmailId=" + email + " by user id=" + userId + ". ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to set EmailId=" + email + " by user id=" + userId + ". ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            //Throws exception in case of wrong result
            if (rowsAffected == 0)
            {
                string prefixMsg = "Unable to set EmailId=" + email + " by user id=" + userId + ". ";
                throw new DASAppResultNotFoundException(prefixMsg + " Wrong user id='" + userId + ".");
            }
            return rowsAffected;
        }

        //Get all emailIds
        public DataSet GetAllEmail()
        {
            DataSet dsResult = new DataSet();
            try
            {
 
                //Prepares SQL statement
                string sql = "SELECT UserId, Email FROM vlfUser";
                            //" WHERE " + searchFieldName + "=" + searchFieldValue;
                //Executes SQL statement
                if (sqlExec.RequiredTransaction())
                {
                    // 2. Attach current command SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                dsResult = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve EmailId.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve EmailId" ;
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return dsResult;
        }	


    }
}
