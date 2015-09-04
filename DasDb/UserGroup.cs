using System;
using System.Collections;	//for ArrayList
using System.Data;			// for DataSet
using System.Data.SqlClient;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfUserGroup table.
	/// </summary>
	public class UserGroup : Tbl2UniqueFields
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public UserGroup(SQLExecuter sqlExec) : base ("vlfUserGroup",sqlExec)
		{
		}
		/// <summary>
		/// Add new user group.
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <returns>vod</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user group name already exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddUserGroup(string userGroupName)
		{
			AddNewRow("UserGroupId","UserGroupName",userGroupName,"user group");
		}
		/// <summary>
		/// Updates user group name
		/// </summary>
		/// <param name="oldUserGroupName"></param> 
		/// <param name="newUserGroupName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetUserGroupName(string oldUserGroupName,string newUserGroupName)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET UserGroupName='" + newUserGroupName.Replace("'","''") + "'" +
					" WHERE UserGroupName='" + oldUserGroupName.Replace("'","''") + "'";
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set new user group name" + newUserGroupName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set new user group name" + newUserGroupName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set new user group name" + newUserGroupName + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user group name.");
			}
		}
		/// <summary>
		/// Updates user group name
		/// </summary>
		/// <param name="userGroupId"></param> 
		/// <param name="newUserGroupName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void SetUserGroupName(short userGroupId,string newUserGroupName)
		{
			int rowsAffected = 0;
			try
			{
				//Prepares SQL statement
				string sql = "UPDATE " + tableName + 
					" SET UserGroupName='" + newUserGroupName.Replace("'","''") + "'" +
					" WHERE UserGroupId=" + userGroupId;
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg =	"Unable to set new user group name" + newUserGroupName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg =	"Unable to set new user group name" + newUserGroupName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			//Throws exception in case of wrong result
			if(rowsAffected == 0) 
			{
				string prefixMsg =	"Unable to set new user group name" + newUserGroupName + ". ";
				throw new DASAppResultNotFoundException(prefixMsg + " Wrong user group name.");
			}
		}
		/// <summary>
		/// Deletes exist user group by name.
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user group name does not exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserGroup(string userGroupName)
		{
			return DeleteRowsByStrField("UserGroupName",userGroupName, "user group");
		}
		/// <summary>
		/// Delete exist user group by Id
		/// </summary>
		/// <param name="userGroupId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user group id does not exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public int DeleteUserGroup(short userGroupId)
		{
			return DeleteRowsByIntField("UserGroupId",userGroupId, "user group");
		}
		/// <summary>
		/// Retrieves record count of "vlfUserGroup" table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 RecordCount
		{
			get
			{
				return GetRecordCount("UserGroupId");
			}
		}
		/// <summary>
		/// Retrieves max record index from "vlfUserGroup" table
		/// </summary>
		/// <returns>int</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public Int64 MaxRecordIndex
		{
			get
			{
				return GetMaxRecordIndex("UserGroupId");
			}
		}
		/// <summary>
		/// Gets user group name by id
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public string GetUserGroupNameById(short userGroupId)
		{
			return GetFieldValueByRowId("UserGroupId",userGroupId,"UserGroupName","user group");
		}
		/// <summary>
		/// Gets user group id by name
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <returns>short</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public short GetUserGroupIdByName(string userGroupName)
		{
			short resultValue = VLF.CLS.Def.Const.unassignedIntValue;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT UserGroupId FROM " + tableName + 
								" WHERE UserGroupName='" + userGroupName.Replace("'","''") + "'";
				//Executes SQL statement
				DataSet resultDataSet = sqlExec.SQLExecuteDataset(sql);
				if(resultDataSet.Tables[0].Rows.Count == 1)
				{
					//Trim speces at the end of result (all char fields in the database have fixed size)
					resultValue = Convert.ToInt16(resultDataSet.Tables[0].Rows[0][0]);
				}
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user group id by user group name=" + userGroupName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user group id by user group name=" + userGroupName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultValue;
		}
		/// <summary>
		/// Retrieves all user group names.
		/// </summary>
		/// <returns>ArrayList</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetAllUserGroupNames()
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = null;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT UserGroupName" + 
					" FROM " + tableName +
					" ORDER BY UserGroupName ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of user group names.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of user group names.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				//Retrieves info from Table[0].[0][0]
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToString(currRow[0]).TrimEnd());
				}
			}
			return resultList;
		}

		/// <summary>
		/// Retrieves all user groups info.
		/// </summary>
		/// <returns>DataSet [UserGroupId],[UserGroupName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUserGroupsInfo(bool includeHgiAdmin)
		{
			DataSet sqlDataSet = null;
			//Prepares SQL statement
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM vlfUserGroup";
				if(includeHgiAdmin == false)
                    sql += " WHERE UserGroupId<>1 and UserGroupId<>14";
				sql += " ORDER BY UserGroupName ASC";
				//Executes SQL statement
				sqlDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve list of user groups.";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve list of user groups.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return sqlDataSet;
		}

        /// <summary>
        /// Retrieves all user groups info allowed for User.
        /// </summary>
        /// <returns>DataSet [UserGroupId],[UserGroupName][IsBaseGroup][SecurityLevelName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupsbyUser(int UserId, bool AllOrganizationGroups)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupsByUser_Get";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@UserId", UserId);
                sqlParams[1] = new SqlParameter("@AllOrganizationGroups", AllOrganizationGroups);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of user groups.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of user groups.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves all user groups info allowed for update by User.
        /// </summary>
        /// <returns>DataSet [UserGroupId],[UserGroupName][IsBaseGroup][SecurityLevelName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupsForUpdateByUser(int UserId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupsForUpdateByUser_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of user groups.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of user groups.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Group Control settings
        /// </summary>
        /// <returns>DataSet [ControlId],[ControlName][ControlURL][SelectedControlId][FormID][FormURL][FormDisplayOrder][FormName][ControlDescription]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupControlSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupControlSettings_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@ParentUserGroupId", ParentUserGroupId);
                sqlParams[2] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve User Group Control settings.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve User Group Control settings.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Group Report settings
        /// </summary>
        /// <returns>DataSet [ReportTypesId],[ReportName][UserGroupName][SelectedReport]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupReportSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupReportSettings_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@ParentUserGroupId", ParentUserGroupId);
                sqlParams[2] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve User Group Report settings.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve User Group Report settings.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Group Command settings
        /// </summary>
        /// <returns>DataSet [BoxCmdOutTypeId][BoxCmdOutType][UserGroupName][SelectedCommand]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupCommandSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupCommandSettings_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@ParentUserGroupId", ParentUserGroupId);
                sqlParams[2] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve User Group Command settings.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve User Group Command settings.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Group Control additional settings
        /// </summary>
        /// <returns>DataSet [ControlId][ControlDescription]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupControlAddSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupControlAddSettings_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@ParentUserGroupId", ParentUserGroupId);
                sqlParams[2] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve User Group Control additional settings.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve User Group Control additional settings.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Group Report additional settings
        /// </summary>
        /// <returns>DataSet [ReportTypesId][ReportName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupReportAddSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupReportAddSettings_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@ParentUserGroupId", ParentUserGroupId);
                sqlParams[2] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve User Group Report additional settings.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve User Group Report additional settings.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Group Command additional settings
        /// </summary>
        /// <returns>DataSet [BoxCmdOutTypeId][BoxCmdOutType]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupCommandAddSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupCommandAddSettings_Get";
                SqlParameter[] sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@ParentUserGroupId", ParentUserGroupId);
                sqlParams[2] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve User Group Command additional settings.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve User Group Command additional settings.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves Operation Types
        /// </summary>
        /// <returns>DataSet [OperationType], [OperationTypeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOperationTypes(int UserId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_OperationTypes_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get operation types.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get operation types.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves Operation Controls
        /// </summary>
        /// <returns>DataSet [ControlID][ControlName]/returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOperationControls(int UserId, int OperationType)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_OperationControls_Get";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@OperationType", OperationType);
                sqlParams[1] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get operation controls.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get operation controls.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves Organizations with User Groups
        /// </summary>
        /// <returns>DataSet [OrganizationId][OrganizationName]/returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationsWithUserGroups()
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_OrganizationsWithUserGroups_Get";

                sqlDataSet = sqlExec.SPExecuteDataset(sql);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get organizations with user groups.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get organizations with user groups.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves User Groups operation access
        /// </summary>
        /// <returns>DataSet [UserGroupId][UserGroupName]/returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserGroupsOperationAccess(int OperationId, int OperationType, int OrganizationId, bool OperationAccess)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_UserGroupsOperationAccess_Get";
                SqlParameter[] sqlParams = new SqlParameter[4];
                sqlParams[0] = new SqlParameter("@OperationId", OperationId);
                sqlParams[1] = new SqlParameter("@OperationType", OperationType);
                sqlParams[2] = new SqlParameter("@OrganizationId", OrganizationId);
                sqlParams[3] = new SqlParameter("@OperationAccess", OperationAccess);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to get User Groups operation access.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to get User Groups operation access.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Adds User Group Setting
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <param name="OperationId"></param>
        /// <param name="OperationType"></param>
        public int AddUserGroupSetting(int UserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.Int, ParameterDirection.Input, UserGroupId);
                sqlExec.AddCommandParam("@OperationId", SqlDbType.Int, ParameterDirection.Input, OperationId);
                sqlExec.AddCommandParam("@OperationType", SqlDbType.Int, ParameterDirection.Input, OperationType);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupSetting_Add");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add user group setting - UserGroupId={0}, OperationId={1}, OperationType={2}. ", UserGroupId.ToString(), OperationId.ToString(), OperationType.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add user group setting - UserGroupId={0}, OperationId={1}, OperationType={2}. ", UserGroupId.ToString(), OperationId.ToString(), OperationType.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Deletes User Group Setting
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <param name="OperationId"></param>
        /// <param name="OperationType"></param>
        public int DeleteUserGroupSetting(int UserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.Int, ParameterDirection.Input, UserGroupId);
                sqlExec.AddCommandParam("@OperationId", SqlDbType.Int, ParameterDirection.Input, OperationId);
                sqlExec.AddCommandParam("@OperationType", SqlDbType.Int, ParameterDirection.Input, OperationType);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupSetting_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to delete user group setting - UserGroupId={0}, OperationId={1}, OperationType={2}. ", UserGroupId.ToString(), OperationId.ToString(), OperationType.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to delete user group setting - UserGroupId={0}, OperationId={1}, OperationType={2}. ", UserGroupId.ToString(), OperationId.ToString(), OperationType.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Updates User Group Settings
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <param name="CheckboxValuesParams"></param>
        /// <param name="OperationType"></param>
        /// <param name="UserId"></param>
        /// <param name="UserGroupName"></param>
        public int UpdateUserGroupSettings(int UserGroupId, string CheckboxValuesParams, string CheckboxReportsValuesParams, 
            string CheckboxCommandsValuesParams, string FleetIDs, int OperationType, int UserId, string UserGroupName)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.Int, ParameterDirection.Input, UserGroupId);
                sqlExec.AddCommandParam("@CheckboxValuesParams", SqlDbType.VarChar, ParameterDirection.Input, CheckboxValuesParams);
                sqlExec.AddCommandParam("@CheckboxReportsValuesParams", SqlDbType.VarChar, ParameterDirection.Input, CheckboxReportsValuesParams);
                sqlExec.AddCommandParam("@CheckboxCommandsValuesParams", SqlDbType.VarChar, ParameterDirection.Input, CheckboxCommandsValuesParams);
                sqlExec.AddCommandParam("@FleetIDs", SqlDbType.VarChar, ParameterDirection.Input, FleetIDs);
                sqlExec.AddCommandParam("@OperationType", SqlDbType.Int, ParameterDirection.Input, OperationType);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@UserGroupName", SqlDbType.VarChar, ParameterDirection.Input, UserGroupName);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupSettings_Update");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update user group settings - user={0}, UserGroupId={1}, CheckboxValuesParams={2}. ", UserId.ToString(), UserGroupId.ToString(), CheckboxValuesParams);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update user group settings - user={0}, UserGroupId={1}, CheckboxValuesParams={2}. ", UserId.ToString(), UserGroupId.ToString(), CheckboxValuesParams);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Adds User Group Settings
        /// </summary>
        /// <param name="CheckboxValuesParams"></param>
        /// <param name="OperationType"></param>
        /// <param name="UserGroupName"></param>
        /// <param name="SecurityLevel"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="ParentUserGroupId"></param>
        /// <param name="UserId"></param>
        public int AddUserGroupSettings(string CheckboxValuesParams, string CheckboxReportsValuesParams, string CheckboxCommandsValuesParams,
            string FleetIDs, int OperationType, string UserGroupName, int OrganizationId, int ParentUserGroupId, int UserId)
        {
            int UserGroupId = 0;
            try
            {	
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@CheckboxValuesParams", SqlDbType.VarChar, ParameterDirection.Input, CheckboxValuesParams);
                sqlExec.AddCommandParam("@CheckboxReportsValuesParams", SqlDbType.VarChar, ParameterDirection.Input, CheckboxReportsValuesParams);
                sqlExec.AddCommandParam("@CheckboxCommandsValuesParams", SqlDbType.VarChar, ParameterDirection.Input, CheckboxCommandsValuesParams);
                sqlExec.AddCommandParam("@FleetIDs", SqlDbType.VarChar, ParameterDirection.Input, FleetIDs);
                sqlExec.AddCommandParam("@OperationType", SqlDbType.Int, ParameterDirection.Input, OperationType);
                sqlExec.AddCommandParam("@UserGroupName", SqlDbType.VarChar, ParameterDirection.Input, UserGroupName);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, ParameterDirection.Input, OrganizationId);
                sqlExec.AddCommandParam("@ParentUserGroupId", SqlDbType.Int, ParameterDirection.Input, ParentUserGroupId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.SmallInt, ParameterDirection.Output, 4, UserGroupId);

                int res = sqlExec.SPExecuteNonQuery("usp_UserGroupSettings_Add");

                UserGroupId = (DBNull.Value == sqlExec.ReadCommandParam("@UserGroupId")) ?
                              UserGroupId : Convert.ToInt32(sqlExec.ReadCommandParam("@UserGroupId"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add user group settings - user={0}, UserGroupId={1}, CheckboxValuesParams={2}. ", UserId.ToString(), UserGroupId.ToString(), CheckboxValuesParams);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add user group settings - user={0}, UserGroupId={1}, CheckboxValuesParams={2}. ", UserId.ToString(), UserGroupId.ToString(), CheckboxValuesParams);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return UserGroupId;
        }

        /// <summary>
        /// Gets Fleets assigned to User Group
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns></returns>
        public DataSet GetFleetsByUserGroup(int UserGroupId, string FleetType)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_FleetsByUserGroup_Get";
                SqlParameter[] sqlParams = new SqlParameter[2];
                sqlParams[0] = new SqlParameter("@UserGroupId", UserGroupId);
                sqlParams[1] = new SqlParameter("@FleetType", FleetType);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve UserGroup Fleets.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve UserGroup Fleets.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves all user groups info allowed for update by User.
        /// </summary>
        /// <returns>DataSet [ControlId],[ControlName][FormID][FormName][ControlDescription][ControlIsActive]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetControlsForUpdate(int UserId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_ControlsForUpdate_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of controls.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of controls.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves all active forms
        /// </summary>
        /// <returns>DataSet [FormID][FormName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetForms(int UserId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_Forms_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@UserId", UserId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of forms.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of forms.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Retrieves form control
        /// </summary>
        /// <param name="ControlId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetControl(int ControlId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_Control_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@ControlId", ControlId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve form control.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve form control.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// Adds new form control
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="Description"></param>
        /// <param name="FormID"></param>
        /// <param name="ControlURL"></param>
        /// <param name="ControlIsActive"></param>
        /// <param name="ControlLangNames"></param>
        /// <returns>ControlId</returns>
        public int AddControl(string ControlName, string Description, int FormID, string ControlURL, bool ControlIsActive, string ControlLangNames, int ParentControlId)
        {
            int ControlId = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ControlName", SqlDbType.VarChar, ParameterDirection.Input, ControlName);
                sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, ParameterDirection.Input, Description);
                sqlExec.AddCommandParam("@FormID", SqlDbType.Int, ParameterDirection.Input, FormID);
                sqlExec.AddCommandParam("@ControlURL", SqlDbType.VarChar, ParameterDirection.Input, ControlURL);
                sqlExec.AddCommandParam("@ControlIsActive", SqlDbType.Bit, ParameterDirection.Input, Convert.ToByte(ControlIsActive));
                sqlExec.AddCommandParam("@ControlLangNames", SqlDbType.VarChar, ParameterDirection.Input, ControlLangNames);
                sqlExec.AddCommandParam("@ParentControlId", SqlDbType.Int, ParameterDirection.Input, ParentControlId);
                sqlExec.AddCommandParam("@ControlId", SqlDbType.Int, ParameterDirection.Output, 4, ControlId);

                int res = sqlExec.SPExecuteNonQuery("usp_Control_Add");

                ControlId = (DBNull.Value == sqlExec.ReadCommandParam("@ControlId")) ?
                              ControlId : Convert.ToInt32(sqlExec.ReadCommandParam("@ControlId"));
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add control - ControlName={0}, FormID={1}.", ControlName, FormID.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add control - ControlName={0}, FormID={1}.", ControlName, FormID.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return ControlId;
        }

        /// <summary>
        /// Updates form control
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="Description"></param>
        /// <param name="FormID"></param>
        /// <param name="ControlURL"></param>
        /// <param name="ControlIsActive"></param>
        /// <param name="ControlLangNames"></param>
        /// <param name="ControlId"></param>
        /// <returns>rows affected</returns>
        public int UpdateControl(string ControlName, string Description, int FormID, string ControlURL, bool ControlIsActive, string ControlLangNames, int ControlId, int ParentControlId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ControlName", SqlDbType.VarChar, ParameterDirection.Input, ControlName);
                sqlExec.AddCommandParam("@Description", SqlDbType.VarChar, ParameterDirection.Input, Description);
                sqlExec.AddCommandParam("@FormID", SqlDbType.Int, ParameterDirection.Input, FormID);
                sqlExec.AddCommandParam("@ControlURL", SqlDbType.VarChar, ParameterDirection.Input, ControlURL);
                sqlExec.AddCommandParam("@ControlIsActive", SqlDbType.Bit, ParameterDirection.Input, Convert.ToByte(ControlIsActive));
                sqlExec.AddCommandParam("@ControlLangNames", SqlDbType.VarChar, ParameterDirection.Input, ControlLangNames);
                sqlExec.AddCommandParam("@ControlId", SqlDbType.Int, ParameterDirection.Input, ControlId);
                sqlExec.AddCommandParam("@ParentControlId", SqlDbType.Int, ParameterDirection.Input, ParentControlId);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_Control_Update");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update control - ControlId={0}, ControlName={1}.", ControlId.ToString(), ControlName);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update control - ControlId={0}, ControlName={1}.", ControlId.ToString(), ControlName);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Deletes User Group
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns>rows affected</returns>
        public int DeleteUserGroup(int UserGroupId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.Int, ParameterDirection.Input, UserGroupId);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroup_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to delete User Group - UserGroupId={0}.", UserGroupId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable todelete User Group - UserGroupId={0}.", UserGroupId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Adds a setting to all User Groups
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns>rows affected</returns>
        public int AddUserGroupSettingsAll(int ParentUserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ParentUserGroupId", SqlDbType.Int, ParameterDirection.Input, ParentUserGroupId);
                sqlExec.AddCommandParam("@OperationId", SqlDbType.Int, ParameterDirection.Input, OperationId);
                sqlExec.AddCommandParam("@OperationType", SqlDbType.Int, ParameterDirection.Input, OperationType);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupSettingsAll_Add");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add setting to all User Groups - ParentUserGroupId={0}.", ParentUserGroupId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add setting to all User Groups - ParentUserGroupId={0}.", ParentUserGroupId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Deletes a setting from all User Groups
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns>rows affected</returns>
        public int DeleteUserGroupSettingsAll(int ParentUserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@ParentUserGroupId", SqlDbType.Int, ParameterDirection.Input, ParentUserGroupId);
                sqlExec.AddCommandParam("@OperationId", SqlDbType.Int, ParameterDirection.Input, OperationId);
                sqlExec.AddCommandParam("@OperationType", SqlDbType.Int, ParameterDirection.Input, OperationType);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupSettingsAll_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to delete setting from all User Groups - ParentUserGroupId={0}.", ParentUserGroupId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to delete setting from all User Groups - ParentUserGroupId={0}.", ParentUserGroupId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public DataSet GetUserGroupAssignedOperations(Enums.OperationType operationType, int userGroupId)
        {
            DataSet sqlDataSet = null;
            string idName = "";
            string fieldName = "";
            string tableName = "";

            switch(operationType)
            {
                case Enums.OperationType.Command:
                    idName = "BoxCmdOutTypeId";
                    fieldName = "BoxCmdOutTypeName";
                    tableName = "vlfBoxCmdOutType";
                    break;
                case Enums.OperationType.Output:
                    idName = "OutputId";
                    fieldName = "OutputName";
                    tableName = "vlfOutput";
                    break;
                case Enums.OperationType.Gui:
                    idName = "ControlId";
                    fieldName = "ControlName";
                    tableName = "vlfGuiControls";
                    break;
                case Enums.OperationType.Repors:
                    idName = "ReportTypesId";
                    fieldName = "ReportTypesName";
                    tableName = "vlfReportTypes";
                    break;
                case Enums.OperationType.WebMethod:
                    idName = "MethodId";
                    fieldName = "MethodName";
                    tableName = "vlfWebMethods";
                    break;
                default:
                    return null;                    
            }

            try
            {
                string sql = "SELECT CAST(op." + idName + " AS INTEGER) AS OpId, op." + fieldName + " AS OpName "
                           + " FROM vlfOperationType ot, vlfGroupSecurity gs, " + tableName + " as op "
                           + " WHERE gs.UserGroupId = " + userGroupId.ToString() + " AND gs.OperationId=op." + idName
                           + " AND gs.OperationType=ot.OperationType AND gs.OperationType=" + ((short)operationType).ToString()
                           + " ORDER BY 2, 1";
                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of Assigned operations.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of Assigned operations.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public DataSet GetUserGroupUnassignedOperations(Enums.OperationType operationType, int userGroupId)
        {
            DataSet sqlDataSet = null;
            string idName = "";
            string fieldName = "";
            string tableName = "";

            switch (operationType)
            {
                case Enums.OperationType.Command:
                    idName = "BoxCmdOutTypeId";
                    fieldName = "BoxCmdOutTypeName";
                    tableName = "vlfBoxCmdOutType";
                    break;
                case Enums.OperationType.Output:
                    idName = "OutputId";
                    fieldName = "OutputName";
                    tableName = "vlfOutput";
                    break;
                case Enums.OperationType.Gui:
                    idName = "ControlId";
                    fieldName = "ControlName";
                    tableName = "vlfGuiControls";
                    break;
                case Enums.OperationType.Repors:
                    idName = "ReportTypesId";
                    fieldName = "ReportTypesName";
                    tableName = "vlfReportTypes";
                    break;
                case Enums.OperationType.WebMethod:
                    idName = "MethodId";
                    fieldName = "MethodName";
                    tableName = "vlfWebMethods";
                    break;
                default:
                    return null;
            }

            try
            {
                string sql = "SELECT CAST(" + idName + " AS INTEGER) AS OpID, " + fieldName + " As OpName "
                           + " FROM " + tableName + " EXCEPT "
                           + " SELECT CAST(op." + idName + " AS INTEGER) AS OpID, op." + fieldName + " As OpName "
                           + " FROM vlfOperationType ot, vlfGroupSecurity gs, " + tableName + " as op"
                           + " WHERE gs.UserGroupId=" + userGroupId.ToString() + " AND gs.OperationId=op." + idName
                           + " AND gs.OperationType=ot.OperationType AND gs.OperationType=" + ((short)operationType).ToString();

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
            }

            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve list of Unassigned operations.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve list of Unassigned operations.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <param name="opId"></param>
        /// <returns></returns>
        public int AssignOperationToUserGroup(Enums.OperationType operationType, int userGroupId,  int opId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = String.Format("INSERT INTO vlfGroupSecurity (UserGroupId,OperationId,OperationType) VALUES ({0},{1},{2});",
                                                userGroupId, opId, (short)operationType);
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to Assign Operation: " + opId + " to Group: " + userGroupId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to Assign Operation: " + opId + " to Group: " + userGroupId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <param name="opId"></param>
        /// <returns></returns>
        public int UnassignOperationToUserGroup(Enums.OperationType operationType, int userGroupId, int opId)
        {
            int rowsAffected = 0;
            try
            {
                string sql = String.Format("DELETE FROM vlfGroupSecurity WHERE UserGroupId={0} AND OperationId={1} AND OperationType={2};",
                                                userGroupId, opId, (short)operationType);
                if (sqlExec.RequiredTransaction())
                {
                    // 3. Attaches SQL to transaction
                    sqlExec.AttachToTransaction(sql);
                }
                rowsAffected = sqlExec.SQLExecuteNonQuery(sql);

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to UnAssign Operation: " + opId + " from Group: " + userGroupId;
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to UnAssign Operation: " + opId + " from Group: " + userGroupId;
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Retrieves child Controls
        /// </summary>
        /// <param name="ControlId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetChildControls(int ParentControlId)
        {
            DataSet sqlDataSet = null;
            //Prepares SQL statement
            try
            {
                string sql = "usp_ChildControls_Get";
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@ParentControlId", ParentControlId);

                sqlDataSet = sqlExec.SPExecuteDataset(sql, sqlParams);
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve child controls.";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve child controls.";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
