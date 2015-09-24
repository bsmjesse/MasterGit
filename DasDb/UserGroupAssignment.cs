using System;
using System.Collections;	// for ArrayList
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfUserGroupAssignment table.
	/// </summary>
	public class UserGroupAssignment: TblConnect2TblsWithoutRules
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public UserGroupAssignment(SQLExecuter sqlExec) : base ("vlfUserGroupAssignment",sqlExec)
		{
		}
		/// <summary>
		/// Add new permission to user group.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user alredy exists in the group</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AssignUserToGroup(int userId,short userGroupId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName +
				" (UserId,UserGroupId) VALUES ( {0},{1} )",	userId,userGroupId);
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
				string prefixMsg = "Unable to add new user '" + userId + "' to the group '" + userGroupId + "'.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new user '" + userId + "' to the group '" + userGroupId + "'.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new user '" + userId + "' to the group '" + userGroupId + "'.";
				throw new DASAppDataAlreadyExistsException(prefixMsg + " This user already exists.");
			}
		}		
		/// <summary>
		/// Deletes existing user group assignment.
		/// </summary>
		/// <param name="userGroupId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if group does not exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteGroupAssignment(short userGroupId)
		{
			return DeleteRowsByIntField("UserGroupId",userGroupId, "user group id");
		}

        /// <summary>
        /// Deletes existing user assignment.
        /// </summary>
        /// <param name="userId"></param> 
        /// <param name="userGroupId"></param> 
        /// <returns>rows affected</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if user or group does not exists.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public int DeleteUserAssignment(int UserId, short UserGroupId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserGroupId", SqlDbType.VarChar, ParameterDirection.Input, UserGroupId);
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupUserAssignment_Delete");
            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to delete user id=" + UserId.ToString() + " from user group=" + UserGroupId.ToString();
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to delete user id=" + UserId.ToString() + " from user group=" + UserGroupId.ToString();
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

		/// <summary>
		/// Deletes existing user assignment from all user groups.
		/// </summary>
		/// <param name="userId"></param> 
		/// <returns>rows affected</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user or group does not exists.</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteUserAssignments(int userId)
		{
			int rowsAffected = 0;
			// 1. Prepares SQL statement
			string sql = "DELETE FROM " + tableName + 
				" WHERE UserId=" + userId;
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
				string prefixMsg = "Unable to delete user id=" + userId + " from all user groups.";
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete user id=" + userId + " from all user groups.";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Returns all user assigned to the group. 
		/// </summary>
		/// <param name="currUserId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUsersByUserGroup(int currUserId,short userGroupId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT DISTINCT vlfUser.UserId,UserName,FirstName,LastName" +
							" FROM vlfUser,vlfPersonInfo,vlfUserGroupAssignment" + 
							" WHERE vlfUserGroupAssignment.UserGroupId=" + userGroupId +
							" AND vlfUserGroupAssignment.UserId=vlfUser.UserId" +
							" AND vlfUser.PersonId=vlfPersonInfo.PersonId"+
                            " AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE()) AND vlfUser.OrganizationId =(SELECT OrganizationId FROM vlfUser WHERE UserId=" + currUserId + ") and (UserName not like 'support1_%' and UserName not like 'hgi_%') ORDER BY LastName, FirstName";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}			
		/// <summary>
		/// Checks if user assigned to the group
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>true/false</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool IsUserAssignedToUserGroup(int userId,short userGroupId)
		{
			int retResult = 0;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT count(UserId) FROM vlfUserGroupAssignment" +
							" WHERE UserGroupId=" + userGroupId +
							" AND UserId=" + userId;
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				retResult = Convert.ToInt32(sqlExec.SQLExecuteScalar(sql));
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user assignments by group id " + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user assignments by group id " + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(retResult > 0)
				return true;
			else
				return false;
		}			
		/// <summary>
		/// Returns all user assigned to the group in current organization. 
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="organizationId"></param>
		/// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUnassignedUsersToUserGroup(short userGroupId,int organizationId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
                string sql = "SELECT UserId,UserName,FirstName,LastName" +
                            " FROM vlfUser,vlfPersonInfo" +
                            " WHERE UserId NOT IN (SELECT UserId FROM " + tableName + " WHERE UserGroupId=" + userGroupId + ")" +
                            " AND OrganizationId=" + organizationId +
                            " AND vlfUser.PersonId=vlfPersonInfo.PersonId " +
                            " AND (ExpiredDate IS NULL OR ExpiredDate > GETDATE())" +
                            " AND (UserName not like 'support1_%' and UserName not like 'hgi_%')" +
                            "ORDER BY LastName, FirstName ";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve users by group id " + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}			

		/// <summary>
		/// Returns all user assigned to the group. 
		/// </summary>
		/// <param name="currUserId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>ArrayList of users ids assigned to the group</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetUsersArrayByGroup(int currUserId,short userGroupId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetUsersByUserGroup(currUserId,userGroupId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToInt32(currRow["UserId"]));
				}
			}
			return resultList;
		}			
		/// <summary>
		/// Returns all groups included this user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [UserGroupId],[UserGroupName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAssignedGroupsByUser(int userId)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT DISTINCT vlfUserGroup.UserGroupId,UserGroupName FROM vlfUserGroup," + tableName + 
							" WHERE " + tableName + ".UserId=" + userId +
							" AND (" + tableName + ".UserGroupId=vlfUserGroup.UserGroupId)";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user groups by user id " + userId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user groups by user id " + userId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}			
		/// <summary>
		/// Returns all groups included this user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>ArrayList of groups ids included this user</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public ArrayList GetGroupsArrayByUser(int userId)
		{
			ArrayList resultList = null;
			DataSet sqlDataSet = GetAssignedGroupsByUser(userId);
			if((sqlDataSet != null)&&(sqlDataSet.Tables[0].Rows.Count > 0))
			{
				resultList = new ArrayList(sqlDataSet.Tables[0].Rows.Count);
				foreach(DataRow currRow in sqlDataSet.Tables[0].Rows)
				{
					resultList.Add(Convert.ToInt16(currRow["UserGroupId"]));
				}
			}
			return resultList;
		}			
		/// <summary>
		/// Returns all groups included users from current organization. 
		/// </summary>
		/// <param name="organizationName"></param>
		/// <returns>DataSet [UserGroupId],[UserGroupName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllUserGroupsByOrganizationName(string organizationName)
		{
			DataSet resultDataSet = null;
			try
			{
				// 1. Prepares SQL statement
				string sql = "SELECT DISTINCT vlfUserGroup.UserGroupId,UserGroupName FROM vlfUserGroup," + tableName + 
					" WHERE " + tableName + ".UserId IN " + "(SELECT UserId FROM vlfUser,vlfOrganization WHERE OrganizationName='" + organizationName.Replace("'","''") + "' AND vlfOrganization.OrganizationId=vlfUser.OrganizationId)" +
					" AND (" + tableName + ".UserGroupId=vlfUserGroup.UserGroupId)";
				if(sqlExec.RequiredTransaction())
				{
					// 2. Attaches SQL to transaction
					sqlExec.AttachToTransaction(sql);
				}
				// 3. Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user groups by Organization Name " + organizationName + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user groups by Organization Name " + organizationName + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

        /// <summary>
        /// Updates User Group Assignment
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UsergroupsParams"></param>
        /// <returns></returns>
        public int  UpdateUserGroupAssignmnet(int UserId, string UsergroupsParams)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@UsergroupsParams", SqlDbType.VarChar, ParameterDirection.Input, UsergroupsParams);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupAssignmnet_Update");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to update user group assignment - user={0}, UserGroups={1}. ", UserId.ToString(), UsergroupsParams);
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to update user group assignment - user={0}, UserGroups={1}. ", UserId.ToString(), UsergroupsParams);
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Adds User Group Assignment
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UsergroupsParams"></param>
        /// <returns></returns>
        public int AddUserGroupAssignmnet(int UserId, int UsergroupId)
        {
            int rowsAffected = 0;
            try
            {
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, ParameterDirection.Input, UserId);
                sqlExec.AddCommandParam("@UsergroupId", SqlDbType.Int, ParameterDirection.Input, UsergroupId);

                rowsAffected = sqlExec.SPExecuteNonQuery("usp_UserGroupAssignmnet_Add");
            }
            catch (SqlException objException)
            {
                string prefixMsg = String.Format("Unable to add user group assignment - user={0}, UserGroup={1}. ", UserId.ToString(), UsergroupId.ToString());
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = String.Format("Unable to add user group assignment - user={0}, UserGroup={1}. ", UserId.ToString(), UsergroupId.ToString());
                throw new DASException(prefixMsg + " " + objException.Message);
            }

            return rowsAffected;
        }
					
	}
}
