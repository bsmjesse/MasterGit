using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfUserLogin table.
	/// </summary>
	public class UserPreference : TblConnect2TblsWithRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public UserPreference(SQLExecuter sqlExec) : base ("vlfUserPreference",sqlExec)
		{
		}
		/// <summary>
		/// Adds new user preference
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <param name="preferenceValue"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddUserPreference(int userId,int preferenceId,string preferenceValue)
		{
			AddNewRow("UserId",userId,"PreferenceId",preferenceId,"PreferenceValue",preferenceValue,"user preference");
		}	
		/// <summary>
		/// Updates user preference
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <param name="preferenceValue"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user preference does not exist.</exception>
		/// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void UpdateUserPreference(int userId,int preferenceId,string preferenceValue)
		{
			// 1. validates parameters
			if(	(userId == VLF.CLS.Def.Const.unassignedIntValue)||
				(preferenceId == VLF.CLS.Def.Const.unassignedIntValue))
			{
				throw new DASAppInvalidValueException("Wrong value for insert SQL: user Id=" +
					userId + " preferenceId=" + preferenceId);
			}
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "UPDATE " + tableName +
				" SET PreferenceValue='" + preferenceValue.Replace("'","''") + "'" +
				" WHERE UserId=" + userId + 
				" AND PreferenceId=" + preferenceId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to update user Id " + userId + " preference " + preferenceId + " with preference value=" + preferenceValue +".";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to update user Id " + userId + " preference " + preferenceId + " with preference value=" + preferenceValue +".";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to update user Id " + userId + " preference " + preferenceId + " with preference value=" + preferenceValue +".";
				throw new DASAppResultNotFoundException(prefixMsg + " This preference does not exist.");
			}
		}
		/// <summary>
		/// Set DayLight Savings.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="dayLightSaving"></param>
		/// <remarks>Set day light savings only to users how has autonatic adjusteble DayLightSaving feature.</remarks>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetDayLightSaving(bool dayLightSaving)
		{
			// 1. Prepares SQL statement
			string sql = "UPDATE vlfUserPreference SET PreferenceValue=" + Convert.ToInt16(dayLightSaving) + " WHERE PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.DayLightSaving + " AND UserId IN (SELECT DISTINCT UserId FROM vlfUserPreference WHERE PreferenceId=" + (short)VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving + " and PreferenceValue=1)";
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
		/// Deletes all user preferences
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserPreferences(int userId)
		{
			return DeleteRowsByIntField("UserId",userId, "user preferences");
		}
		/// <summary>
		/// Deletes user preference
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserPreference(int userId,int preferenceId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + 
						" WHERE UserId=" + userId + 
						" AND PreferenceId=" + preferenceId ;
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
				string prefixMsg = "Unable to delete user preference by user id=" + userId + " preferenceId=" + preferenceId;
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete user preference by user id=" + userId + " preferenceId=" + preferenceId;
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retrieves all user preferences info
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUserPreferencesInfo(int userId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfUser.UserId,vlfPreference.PreferenceId,PreferenceName,PreferenceValue" +
					" FROM vlfUser,vlfUserPreference,vlfPreference" +
					" WHERE vlfUser.UserId=" + userId +
					" AND vlfUser.UserId=vlfUserPreference.UserId" +
					" AND vlfUserPreference.PreferenceId=vlfPreference.PreferenceId" + 
					" ORDER BY PreferenceName";

				
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user preferences by user id=" + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}	
		/// <summary>
		/// Retrieves user preference info
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="preferenceId"></param>
		/// <returns>DataSet [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserPreferenceInfo(int userId,int preferenceId)
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfUser.UserId,vlfPreference.PreferenceId,PreferenceName,PreferenceValue" +
					" FROM vlfUser,vlfUserPreference,vlfPreference" +
					" WHERE vlfUser.UserId=" + userId +
					" AND vlfUser.UserId=vlfUserPreference.UserId" +
					" AND vlfUserPreference.PreferenceId=" + preferenceId + 
					" AND vlfUserPreference.PreferenceId=vlfPreference.PreferenceId" + 
					" ORDER BY PreferenceName";

				
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user preference by user id=" + userId + " preferenceId=" + preferenceId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user preference by user id=" + userId + " preferenceId=" + preferenceId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}	
		/// <summary>
		/// Retrieves all users preferences info
		/// </summary>
		/// <returns>DataSet [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUsersPreferencesInfo()
		{
			DataSet sqlDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT vlfUser.UserId,vlfPreference.PreferenceId,PreferenceName,PreferenceValue" +
					" FROM vlfUser,vlfUserPreference,vlfPreference" +
					" WHERE vlfUser.UserId=vlfUserPreference.UserId" +
					" AND vlfUserPreference.PreferenceId=vlfPreference.PreferenceId" + 
					" ORDER BY PreferenceName";

				
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all users preferences.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all users preferences.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        /// <summary>
        /// Get Users Dashboards
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetUsersDashboards(int userId)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT vlfUserDashboards.JoinId, dbo.vlfDashboardTypes.DashboardName, dbo.vlfDashboardTypes.DashboardID, dbo.vlfDashboardTypes.ContentURL,ISNULL(vlfUserDashboards.FleetId,-1) as FleetId,ISNULL(vlfUserDashboards.PeriodId,-1) as PeriodId, ISNULL(vlfUserDashboards.GridView,0) as GridView,ISNULL(vlfUserDashboards.Threshold,-1) as Threshold " +
                             " FROM  dbo.vlfDashboardTypes INNER JOIN " +
                             " dbo.vlfUserDashboards ON dbo.vlfDashboardTypes.DashboardID = dbo.vlfUserDashboards.DashBoardId " +
                             " where dbo.vlfUserDashboards.UserId="+userId ;



                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Users Dashboards.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Users Dashboards.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }


        /// <summary>
        /// Get Dashboard Types
        /// </summary>
         /// <returns></returns>
        public DataSet GetDashboardTypes()
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql = "SELECT  DashboardID,DashboardName,ContentURL,FleetOption,ThresholdOption,ISNULL(ThresholdUnit,'') as ThresholdUnit,PeriodOption,ISNULL(Description,'') as Description " +
                             " FROM  vlfDashboardTypes ";



                //Executes SQL statement
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve Dashboards.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve Dashboards.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }	
	}
}
