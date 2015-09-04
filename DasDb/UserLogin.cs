using System;
using System.Data;
using VLF.ERR;
using VLF.CLS;
using System.Data.SqlClient ;	// for SqlException
namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfUserLogin table.
	/// </summary>
	public class UserLogin : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public UserLogin(SQLExecuter sqlExec) : base ("vlfUserLogin",sqlExec)
		{
		}
		/// <summary>
		/// Adds new user login
		/// </summary>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user with this datetime already exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int AddUserLogin(int userId,DateTime loginDateTime,string ip)
		{
			int rowsAffected = 0;
			try
			{
				// Set SQL command
				string sql = "INSERT INTO vlfUserLogin(UserId,LoginDateTime,IP) VALUES ( @UserId,@LoginDateTime,@IP ) ";
				sqlExec.ClearCommandParameters();
				// Add parameters to SQL statement
				sqlExec.AddCommandParam("@UserId",SqlDbType.Int,userId);
				sqlExec.AddCommandParam("@LoginDateTime",SqlDbType.DateTime,loginDateTime);
				if(ip == null)
					sqlExec.AddCommandParam("@IP",SqlDbType.Char,System.DBNull.Value);
				else
					sqlExec.AddCommandParam("@IP",SqlDbType.Char,ip);
				
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add user login Date:'" + loginDateTime.ToString() + "' UserId:" + userId + " IP='" + ip + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add user login Date:'" + loginDateTime.ToString() + "' UserId:" + userId + " IP='" + ip + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add user login Date:'" + loginDateTime.ToString() + "' UserId:" + userId + " IP='" + ip + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " User login already exists.");
			}
			return rowsAffected;
		}	
		/// <summary>
		/// Deletes user logins.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserLogins(int userId)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "DELETE FROM vlfUserLogin WHERE UserId=" + userId;
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
				string prefixMsg = "Unable to delete user " + userId + " logins.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete user " + userId + " logins.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserLogins_NewTZ(int userId, DateTime from, DateTime to)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (userId != VLF.CLS.Def.Const.unassignedIntValue)
                {
                    sqlWhere = " WHERE vlfUserLogin.UserId=" + userId;
                    if (from != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND LoginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                }
                else if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere += " WHERE LoginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere += " WHERE LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                }

                //Prepares SQL statement               

                string sql = "DECLARE @Timezone float" +
                           " DECLARE @DayLightSaving int" +
                           " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                           " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                           " IF @Timezone IS NULL SET @Timezone=0" +
                           " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                           " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                           " SET @Timezone= @Timezone + @DayLightSaving" +

                           " SELECT LoginId,vlfUserLogin.UserId,IP," +
                           "CASE WHEN LoginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),LoginDateTime) END AS LoginDateTime," +
                           "UserName,FirstName,LastName" +
                           " FROM vlfUserLogin INNER JOIN vlfUser ON vlfUserLogin.UserId=vlfUser.UserId" +
                           " INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
                           sqlWhere +
                           " ORDER BY vlfUserLogin.LoginDateTime DESC";

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get user login information: UserId:" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get user login information: UserId:" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
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
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserLogins(int userId, DateTime from, DateTime to)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (userId != VLF.CLS.Def.Const.unassignedIntValue)
                {
                    sqlWhere = " WHERE vlfUserLogin.UserId=" + userId;
                    if (from != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND LoginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                }
                else if (from != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere += " WHERE LoginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        sqlWhere += " AND LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                }
                else if (to != VLF.CLS.Def.Const.unassignedDateTime)
                {
                    sqlWhere += " WHERE LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                }

                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                            " IF @Timezone IS NULL SET @Timezone=0" +
                            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                            " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                            " SET @Timezone= @Timezone + @DayLightSaving" +

                            " SELECT LoginId,vlfUserLogin.UserId,IP," +
                            "CASE WHEN LoginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,LoginDateTime) END AS LoginDateTime," +
                            "UserName,FirstName,LastName" +
                            " FROM vlfUserLogin INNER JOIN vlfUser ON vlfUserLogin.UserId=vlfUser.UserId" +
                            " INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
                            sqlWhere +
                            " ORDER BY vlfUserLogin.LoginDateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get user login information: UserId:" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get user login information: UserId:" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginUserId"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        /// <remarks>
        /// 1. Retrieves info about loginUserId
        /// 2. DateTime changed according to userId (user requested this info)
        /// </remarks>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserLastLogin_NewTZ(int userId, int loginUserId)
        {
            DataSet sqlDataSet = null;
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

                   " SELECT top 2 LoginId,vlfUserLogin.UserId,IP," +
                   "CASE WHEN LoginDateTime IS NULL then '' ELSE DATEADD(minute,(@Timezone * 60),LoginDateTime) END AS LoginDateTime," +
                   "UserName,FirstName,LastName" +
                   " FROM vlfUserLogin INNER JOIN vlfUser ON vlfUserLogin.UserId=vlfUser.UserId" +
                   " INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
                   " WHERE vlfUserLogin.UserId=" + loginUserId +
                   " ORDER BY vlfUserLogin.LoginDateTime DESC";

                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get user last login information: UserId:" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get user last login information: UserId:" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginUserId"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        /// <remarks>
        /// 1. Retrieves info about loginUserId
        /// 2. DateTime changed according to userId (user requested this info)
        /// </remarks>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserLastLogin(int userId, int loginUserId)
        {
            DataSet sqlDataSet = null;
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

                    " SELECT top 2 LoginId,vlfUserLogin.UserId,IP," +
                    "CASE WHEN LoginDateTime IS NULL then '' ELSE DATEADD(hour,@Timezone,LoginDateTime) END AS LoginDateTime," +
                    "UserName,FirstName,LastName" +
                    " FROM vlfUserLogin INNER JOIN vlfUser ON vlfUserLogin.UserId=vlfUser.UserId" +
                    " INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
                    " WHERE vlfUserLogin.UserId=" + loginUserId +
                    " ORDER BY vlfUserLogin.LoginDateTime DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get user last login information: UserId:" + userId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get user last login information: UserId:" + userId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return sqlDataSet;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        ///      Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationUserLogins_NewTZ(int userId, int organizationId, DateTime from, DateTime to)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (organizationId != VLF.CLS.Def.Const.unassignedIntValue)
                {
                    sqlWhere = " WHERE UserName not like 'hgi_%' and vlfUser.OrganizationId=" + organizationId;
                    if (from != VLF.CLS.Def.Const.unassignedDateTime)
                        //sqlWhere += " AND LoginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                        sqlWhere += " AND LoginDateTime>= DATEADD(minute,-(@Timezone * 60),'" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')";
                    //DATEADD(hour,-@Timezone,'04/06/2014 04:00:00.000') AND LoginDateTime<=DATEADD(hour,-@Timezone,'04/12/2014 04:00:00.000')
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        //sqlWhere += " AND LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                        sqlWhere += " AND LoginDateTime<= DATEADD(minute,-(@Timezone * 60),'" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')";

                    //sqlWhere += " AND  IP is not null AND IP <>'N/A' ";

                }


                //Prepares SQL statement               

                string sql = "DECLARE @Timezone float" +
                           " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZoneNew +
                           " IF @Timezone IS NULL SET @Timezone=0" +
                           " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                           " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                           " SET @Timezone= @Timezone + @DayLightSaving" +

                           " SELECT distinct vlfUserLogin.UserId,IP," +
                           "CASE WHEN CAST (LoginDateTime as varchar) IS NULL then '' ELSE CAST (DATEADD(minute,(@Timezone * 60),LoginDateTime) as varchar) END AS LoginDateTime," +
                           "UserName,FirstName,LastName,dbo.FleetsByUserId(vlfUser.UserId) as Fleets " +
                           " FROM vlfUserLogin INNER JOIN vlfUser ON vlfUserLogin.UserId=vlfUser.UserId" +
                           " INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
                           sqlWhere +
                           " ORDER BY 3 DESC";


                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get organization user login information: OrganizationId:" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get organization user login information: OrganizationId:" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
        // Changes for TimeZone Feature end
        /// <summary>
        ///      Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationUserLogins(int userId, int organizationId, DateTime from, DateTime to)
        {
            DataSet sqlDataSet = null;
            try
            {
                string sqlWhere = "";
                if (organizationId != VLF.CLS.Def.Const.unassignedIntValue)
                {
                    sqlWhere = " WHERE UserName not like 'hgi_%' and vlfUser.OrganizationId=" + organizationId;
                    if (from != VLF.CLS.Def.Const.unassignedDateTime)
                        //sqlWhere += " AND LoginDateTime>='" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                        sqlWhere += " AND LoginDateTime>= DATEADD(hour,-@Timezone,'" + from.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')";
                    //DATEADD(hour,-@Timezone,'04/06/2014 04:00:00.000') AND LoginDateTime<=DATEADD(hour,-@Timezone,'04/12/2014 04:00:00.000')
                    if (to != VLF.CLS.Def.Const.unassignedDateTime)
                        //sqlWhere += " AND LoginDateTime<='" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "'";
                        sqlWhere += " AND LoginDateTime<= DATEADD(hour,-@Timezone,'" + to.ToString("MM/dd/yyyy HH:mm:ss.fff") + "')";

                    //sqlWhere += " AND  IP is not null AND IP <>'N/A' ";

                }


                //Prepares SQL statement
                string sql = "DECLARE @Timezone int" +
                            " DECLARE @DayLightSaving int" +
                            " SELECT @Timezone=PreferenceValue FROM vlfUserPreference " +
                            " WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.TimeZone +
                            " IF @Timezone IS NULL SET @Timezone=0" +
                            " SELECT @DayLightSaving=PreferenceValue FROM vlfUserPreference WHERE UserId=" + userId + " AND PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving +
                            " IF @DayLightSaving IS NULL SET @DayLightSaving=0" +
                            " SET @Timezone= @Timezone + @DayLightSaving" +

                            " SELECT distinct vlfUserLogin.UserId,IP," +
                            "CASE WHEN CAST (LoginDateTime as varchar) IS NULL then '' ELSE CAST (DATEADD(hour,@Timezone,LoginDateTime) as varchar) END AS LoginDateTime," +
                            "UserName,FirstName,LastName,dbo.FleetsByUserId(vlfUser.UserId) as Fleets " +
                            " FROM vlfUserLogin INNER JOIN vlfUser ON vlfUserLogin.UserId=vlfUser.UserId" +
                            " INNER JOIN vlfPersonInfo ON vlfUser.PersonId=vlfPersonInfo.PersonId" +
                            sqlWhere +
                            " ORDER BY 3 DESC";
                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get organization user login information: OrganizationId:" + organizationId + ".";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get organization user login information: OrganizationId:" + organizationId + ".";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
