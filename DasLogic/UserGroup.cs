using System;
using System.Data;			// for DataSet
using System.Collections;	// for ArrayList
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def.Structures;
using VLF.CLS.Def;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
	/// <summary>
	/// Provides interface to user group functionality in database
	/// </summary>
	public class UserGroup : Das
	{
		DB.GroupSecurity groupSecurity = null;
		DB.UserGroup userGroup = null;
		DB.UserGroupAssignment userGroupAssignment = null;

		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
		public UserGroup(string connectionString) : base (connectionString)
		{
			groupSecurity = new DB.GroupSecurity(sqlExec);
			userGroup = new DB.UserGroup(sqlExec);
			userGroupAssignment = new DB.UserGroupAssignment(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region Groups Security Information
		/// <summary>
		/// Adds new group security
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="operationId"></param>
		/// <param name="operationType"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if group security already exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddGroupSecurity(short userGroupId,int operationId,int operationType)
		{
			groupSecurity.AddGroupSecurity(userGroupId,operationId,operationType);
		}		
		/// <summary>
		/// Delete group security
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <returns> int rows affected</returns>
		public int DeleteGroupSecurity(short userGroupId)
		{
			return groupSecurity.DeleteGroupSecurity(userGroupId);
		}
		/// <summary>
		/// Delete group security
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="operationId"></param>
		/// <param name="operationType"></param>
		/// <returns> int rows affected</returns>
		public int DeleteGroupSecurity(short userGroupId,int operationId,int operationType)
		{
			return groupSecurity.DeleteGroupSecurity(userGroupId,operationId,operationType);
		}
		/// <summary>
		/// Retieves group security information. 	
		/// Throw DASException exception in case of error.
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <returns>[UserGroupId],[OperationId],[OperationType]</returns>
		public DataSet GetGroupSecurityInfo(short userGroupId)
		{
			DataSet dsResult = groupSecurity.GetGroupSecurityInfo(userGroupId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GroupSecurity" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}
		/// <summary>
		/// Retieves group security full info
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns>[OperationType],[OperationTypeName],[OperationId],[OperationName],[OperationAction]</returns>
		public DataSet GetGroupSecurityFullInfo(short groupId)
		{
			DataSet dsResult = groupSecurity.GetGroupSecurityFullInfo(groupId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GroupSecurityFullInfo" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}
		/// <summary>
		/// Retieves group security full info
		/// </summary>
		/// <returns>[OperationType],[OperationTypeName],[OperationId],[OperationName],[OperationAction]</returns>
		public DataSet GetAllGroupSecurityFullInfo()
		{
			DataSet dsResult = groupSecurity.GetAllGroupSecurityFullInfo();
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetAllGroupSecurityFullInfo" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}

		/// <summary>
		/// Retieves user controls
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>[ControlId]</returns>
		public DataSet GetUserControls(int userId)
		{
			DataSet dsResult = groupSecurity.GetUserControls(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetUserControls" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}
		/// <summary>
		/// Retieves user reports
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>[ReportTypesId],[ReportTypesName],[GuiId],[GuiName]</returns>
		public DataSet GetUserReports(int userId)
		{
			DataSet dsResult = groupSecurity.GetUserReports(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "GetUserReports" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}

        /// <summary>
        /// Retieves user reports
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>[ReportTypesId],[ReportTypesName],[GuiId],[GuiName]</returns>
        public DataSet GetUserReportsByCategory(int userId,int category)
        {
            DataSet dsResult = groupSecurity.GetUserReportsByCategory(userId,category);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetUserReports";
                }
                dsResult.DataSetName = "UserGroup";
            }
            return dsResult;
        }
		/// <summary>
		/// Authorize user operation
		/// </summary>
		/// <returns>true if user allows  this operation, otherwise return false</returns>
		/// <param name="userId"></param>
		/// <param name="operationType"></param>
		/// <param name="operationId"></param>
      /// 
		public bool AuthorizeOperation(int userId,VLF.CLS.Def.Enums.OperationType operationType,int operationId)
		{
			return groupSecurity.AuthorizeOperation(userId,operationType,operationId);
		}

      /// <summary>
      /// Authorize user operation
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="methodName"></param>
      /// <returns>true if user is auth. for this operation, otherwise - false</returns>
      public bool AuthorizeWebMethod(int userId, string methodName)
      {
         return groupSecurity.AuthorizeWebMethod(userId, methodName);
      }

      /// <summary>
      /// Authorize user operation
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="methodName"></param>
      /// <param name="className"></param>
      /// <returns>true if user is auth. for this operation, otherwise - false</returns>
      public bool AuthorizeWebMethod(int userId, string methodName, string className)
      {
         return groupSecurity.AuthorizeWebMethod(userId, methodName, className);
      }
      
      #endregion

		#region User Groups Information
		/// <summary>
		/// Add new user group.
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <returns>vod</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user group name already exist</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddUserGroup(string userGroupName)
		{
			userGroup.AddUserGroup(userGroupName);
		}
		/// <summary>
		/// Update user group name
		/// </summary>
		/// <param name="oldUserGroupName"></param> 
		/// <param name="newUserGroupName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public void SetUserGroupName(string oldUserGroupName,string newUserGroupName)
		{
			userGroup.SetUserGroupName(oldUserGroupName,newUserGroupName);
		}
		/// <summary>
		/// Update user group name
		/// </summary>
		/// <param name="userGroupId"></param> 
		/// <param name="newUserGroupName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public void SetUserGroupName(short userGroupId,string newUserGroupName)
		{
			userGroup.SetUserGroupName(userGroupId,newUserGroupName);
		}

		/// <summary>
		/// Delete exist user group by name.
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <returns>int rows affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserGroup(string userGroupName)
		{
			short userGroupId = GetUserGroupIdByName(userGroupName);
			return DeleteUserGroup(userGroupId);
		}

        /// <summary>
        /// Adds a setting to all User Groups
        /// </summary>
        /// <param name="userGroupName"></param>
        /// <returns>int rows affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int AddUserGroupSettingsAll(int ParentUserGroupId, int OperationId, int OperationType)
        {
            return userGroup.AddUserGroupSettingsAll(ParentUserGroupId, OperationId, OperationType);
        }

        /// <summary>
        /// Gets Fleets assigned to User Group
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns></returns>
        public DataSet GetFleetsByUserGroup(int UserGroupId, string FleetType)
        {
            return userGroup.GetFleetsByUserGroup(UserGroupId, FleetType);
        }

        /// <summary>
        /// Deletes a setting from all User Groups
        /// </summary>
        /// <param name="userGroupName"></param>
        /// <returns>int rows affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteUserGroupSettingsAll(int ParentUserGroupId, int OperationId, int OperationType)
        {
            return userGroup.DeleteUserGroupSettingsAll(ParentUserGroupId, OperationId, OperationType);
        }

		/// <summary>
		/// Delete exist user group by Id
		/// </summary>
		/// <param name="userGroupId"></param> 
		/// <returns>int rows affected</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int DeleteUserGroup(short userGroupId)
		{
			int rowsAffected = 0;
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Delete all user from the group
				DeleteGroupAssignment(userGroupId);
				// 3. Delete all group security
				DeleteGroupSecurity(userGroupId);
				// 4. Delete user group
				rowsAffected = userGroup.DeleteUserGroup(userGroupId);
				// 5. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 5. Rollback all changes
				rowsAffected = 0;
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Uanable to delete user group ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 5. Rollback all changes
				rowsAffected = 0;
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// user group name by id from "vlfUserGroup" table
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <returns>string</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public string GetUserGroupNameById(short userGroupId)
		{
			return userGroup.GetUserGroupNameById(userGroupId);
		}
		/// <summary>
		/// user group id by name from "vlfUserGroup" table
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <returns>short</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public short GetUserGroupIdByName(string userGroupName)
		{
			return userGroup.GetUserGroupIdByName(userGroupName);
		}
		/// <summary>
		/// Retrieves all user group names.
		/// </summary>
		/// <returns>ArrayList</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public ArrayList GetAllUserGroupNames()
		{
			return userGroup.GetAllUserGroupNames();
		}

		/// <summary>
        /// Retrieves all user groups info allowed for User.
		/// </summary>
		/// <returns>DataSet [UserGroupId],[UserGroupName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllUserGroups(bool includeHgiAdmin)
		{
			return userGroup.GetAllUserGroupsInfo(includeHgiAdmin);
		}

        /// <summary>
        /// Retrieves all user group names.
        /// </summary>
        /// <returns>DataSet [UserGroupId],[UserGroupName][IsBaseGroup][SecurityLevelName]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUserGroupsbyUser(int UserId, bool AllOrganizationGroups)
        {
            return userGroup.GetUserGroupsbyUser(UserId, AllOrganizationGroups);
        }

        /// <summary>
        /// Retrieves all user group names for update.
        /// </summary>
        /// <returns>DataSet [UserGroupId],[UserGroupName][IsBaseGroup][SecurityLevelName]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUserGroupsForUpdateByUser(int UserId)
        {
            return userGroup.GetUserGroupsForUpdateByUser(UserId);
        }

        /// <summary>
        /// Retrieves all user group control settings.
        /// </summary>
        /// <returns>DataSet [ControlId][ControlName][SelectedControlId][FormID][FormName][ControlDescription]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUserGroupControlSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            return userGroup.GetUserGroupControlSettings(UserId, UserGroupId, ParentUserGroupId);
        }

        /// <summary>
        /// Retrieves all user group report settings.
        /// </summary>
        /// <returns>DataSet [ReportTypesId][ReportName][UserGroupName][SelectedReport]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUserGroupReportSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            return userGroup.GetUserGroupReportSettings(UserId, UserGroupId, ParentUserGroupId);
        }

        /// <summary>
        /// Retrieves all user group command settings.
        /// </summary>
        /// <returns>DataSet [BoxCmdOutTypeId][BoxCmdOutType][UserGroupName][SelectedCommand]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUserGroupCommandSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            return userGroup.GetUserGroupCommandSettings(UserId, UserGroupId, ParentUserGroupId);
        }

        /// <summary>
        /// Retrieves User Group Control additional settings
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserGroupId"></param>
        /// <param name="ParentUserGroupId"></param>
        /// <returns></returns>
        public DataSet GetUserGroupControlAddSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            return userGroup.GetUserGroupControlAddSettings(UserId, UserGroupId, ParentUserGroupId);
        }

        /// <summary>
        /// Retrieves User Group Report additional settings
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserGroupId"></param>
        /// <param name="ParentUserGroupId"></param>
        /// <returns></returns>
        public DataSet GetUserGroupReportAddSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            return userGroup.GetUserGroupReportAddSettings(UserId, UserGroupId, ParentUserGroupId);
        }

        /// <summary>
        /// Retrieves User Group Command additional settings
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserGroupId"></param>
        /// <param name="ParentUserGroupId"></param>
        /// <returns></returns>
        public DataSet GetUserGroupCommandAddSettings(int UserId, int UserGroupId, int ParentUserGroupId)
        {
            return userGroup.GetUserGroupCommandAddSettings(UserId, UserGroupId, ParentUserGroupId);
        }

        /// <summary>
        /// Adds User Group Setting
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <param name="OperationId"></param>
        /// <param name="OperationType"></param>
        /// <returns></returns>
        public int AddUserGroupSetting(int UserGroupId, int OperationId, int OperationType)
        {
            return userGroup.AddUserGroupSetting(UserGroupId, OperationId, OperationType);
        }

        /// <summary>
        /// Deletes User Group Setting
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <param name="OperationId"></param>
        /// <param name="OperationType"></param>
        /// <returns></returns>
        public int DeleteUserGroupSetting(int UserGroupId, int OperationId, int OperationType)
        {
            return userGroup.DeleteUserGroupSetting(UserGroupId, OperationId, OperationType);
        }
        
        /// <summary>
        /// Updates User Group settings.
        /// </summary>
        /// <returns>Number of affected rows</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int UpdateUserGroupSettings(int UserGroupId, string CheckboxValuesParams, string CheckboxReportsValuesParams, 
            string CheckboxCommandsValuesParams, string FleetIDs, int OperationType, int UserId, string UserGroupName)
        {
            return userGroup.UpdateUserGroupSettings(UserGroupId, CheckboxValuesParams, CheckboxReportsValuesParams, CheckboxCommandsValuesParams, FleetIDs, 
                OperationType, UserId, UserGroupName);
        }

        /// <summary>
        /// Adds User Group settings.
        /// </summary>
        /// <returns>New UserGroupId</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int AddUserGroupSettings(string CheckboxValuesParams, string CheckboxReportsValuesParams, string CheckboxCommandsValuesParams, 
            string FleetIDs, int OperationType, string UserGroupName, int OrganizationId, int ParentUserGroupId, int UserId)
        {
            return userGroup.AddUserGroupSettings(CheckboxValuesParams, CheckboxReportsValuesParams, CheckboxCommandsValuesParams, FleetIDs, OperationType, 
                UserGroupName, OrganizationId, ParentUserGroupId, UserId);
        }	

        /// <summary>
        /// Retrives Operation Types
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public DataSet GetOperationTypes(int UserId)
        {
            return userGroup.GetOperationTypes(UserId);
        }

        /// <summary>
        /// Retrieves Operation Controls
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OperationType"></param>
        /// <returns></returns>
        public DataSet GetOperationControls(int UserId, int OperationType)
        {
            return userGroup.GetOperationControls(UserId, OperationType);
        }

        /// <summary>
        /// Retrieves organizations with User Groups
        /// </summary>
        /// <returns></returns>
        public DataSet GetOrganizationsWithUserGroups()
        {
            return userGroup.GetOrganizationsWithUserGroups();
        }

        /// <summary>
        /// Retrieves all controls for update.
        /// </summary>
        /// <returns>DataSet [ControlId],[ControlName][FormID][FormName][ControlDescription][ControlIsActive]</returns>
        public DataSet GetControlsForUpdate(int UserId)
        {
            return userGroup.GetControlsForUpdate(UserId);
        }

        /// <summary>
        /// Retrieves all active forms
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>DataSet [FormID][FormName]</returns>
        public DataSet GetForms(int UserId)
        {
            return userGroup.GetForms(UserId);
        }

        /// <summary>
        /// Retrieves form control
        /// </summary>
        /// <param name="ControlId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetControl(int ControlId)
        {
            return userGroup.GetControl(ControlId);
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
            return userGroup.AddControl(ControlName, Description, FormID, ControlURL, ControlIsActive, ControlLangNames, ParentControlId);
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
            return userGroup.UpdateControl(ControlName, Description, FormID, ControlURL, ControlIsActive, ControlLangNames, ControlId, ParentControlId);
        }

        /// <summary>
        /// Deletes User Group
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns>rows affected</returns>
        public int DeleteUserGroup(int UserGroupId)
        {
            return userGroup.DeleteUserGroup(UserGroupId);
        }
        
        /// <summary>
        /// Retrieves User Groups operation access
        /// </summary>
        /// <param name="OperationId"></param>
        /// <param name="OperationType"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="OperationAccess"></param>
        /// <returns></returns>
        public DataSet GetUserGroupsOperationAccess(int OperationId, int OperationType, int OrganizationId, bool OperationAccess)
        {
            return userGroup.GetUserGroupsOperationAccess(OperationId, OperationType, OrganizationId, OperationAccess);
        }

		#endregion

		#region User Groups Assignments
		/// <summary>
		/// Add new user to user group.
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="userName"></param>
		/// <param name="userInfo"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user alredy exists in the group</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddUserToGroup(short userGroupId,string userName,UserInfo userInfo)
		{
			try
			{
				// 1. Begin transaction
				sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
				// 2. Add new user
				DB.User user = new DB.User(sqlExec);
				int userId = user.AddUser(userName,userInfo);
				// 3. Set default user preferences
				// 3.1 set default "MeasurementUnits" to km
				DB.UserPreference userPreference = new DB.UserPreference(sqlExec);
				userPreference.AddUserPreference(userId,(int)VLF.CLS.Def.Enums.Preference.MeasurementUnits,"1");
				// 3.2 set default "TimeZone" to GMT
				userPreference.AddUserPreference(userId,(int)VLF.CLS.Def.Enums.Preference.TimeZone,"0");
				// 3.3 set default "DayLightSaving" do not included
                userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving, "1");
                userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving, Convert.ToInt16(System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)).ToString());
                // 4. Add user to the group
				userGroupAssignment.AssignUserToGroup(userId,userGroupId);
				// 5. Save all changes
				sqlExec.CommitTransaction();
			}
			catch (SqlException objException) 
			{
				// 5. Rollback all changes
				sqlExec.RollbackTransaction();
				Util.ProcessDbException("Unable to add new user to the group ",objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				sqlExec.RollbackTransaction();
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				// 5. Rollback all changes
				sqlExec.RollbackTransaction();
				throw new DASException(objException.Message);
			}
		}
		/// <summary>
		/// Add new user to user group.
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <param name="userName"></param>
		/// <param name="userInfo"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user alredy exists in the group</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AddUserToGroup(string userGroupName,string userName,UserInfo userInfo)
		{
			AddUserToGroup(GetUserGroupIdByName(userGroupName),userName,userInfo);
		}

		/// <summary>
		/// Assign existing user to user group.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user alredy exists in the group</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AssignUserToGroup(int userId,short userGroupId)
		{
			userGroupAssignment.AssignUserToGroup(userId,userGroupId);
		}

        /// <summary>
        /// Updates User Group Assignment
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="UsergroupsParams"></param>
        /// <returns>rows affected</returns>
        public int UpdateUserGroupAssignmnet(int userId, string UsergroupsParams)
        {
            return userGroupAssignment.UpdateUserGroupAssignmnet(userId, UsergroupsParams);
        }


        /// <summary>
        /// Adds User Group Assignment
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="UsergroupId"></param>
        /// <returns>rows affected</returns>
        public int AddUserGroupAssignmnet(int userId, int UsergroupId)
        {
            return userGroupAssignment.AddUserGroupAssignmnet(userId, UsergroupId);
        }

		/// <summary>
		/// Assign existing user to user group.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userGroupName"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if user alredy exists in the group</exception>
		/// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
		public void AssignUserToGroup(int userId,string userGroupName)
		{
			userGroupAssignment.AssignUserToGroup(userId,GetUserGroupIdByName(userGroupName));
		}


      /// <summary>
      /// Create new user, add to user group, assign default fleet
      /// </summary>
      /// <param name="userGroupId"></param>
      /// <param name="userName"></param>
      /// <param name="userInfo"></param>
      /// <returns>void</returns>
      /// <exception cref="DASAppDataAlreadyExistsException">Thrown if user alredy exists in the group</exception>
      /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
      public void AddUserToGroupAssignFleet(short userGroupId, string userName, UserInfo userInfo)
      {
         try
         {
            // 1. Begin transaction
            sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
            // 2. Add new user
            DAS.DB.User user = new VLF.DAS.DB.User(this.sqlExec);
            int userId = user.AddUser(userName, userInfo);
            // 3. Set default user preferences
            // 3.1 set default "MeasurementUnits" to km
            DB.UserPreference userPreference = new DB.UserPreference(sqlExec);
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.MeasurementUnits, "1");
            // 3.2 set default "TimeZone" to GMT
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.TimeZone, "0");
            // 3.3 set default "DayLightSaving" do not included
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.AutoAdjustDayLightSaving, "1");
            userPreference.AddUserPreference(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving, Convert.ToInt16(System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)).ToString());
            // 4. Add user to the group
            userGroupAssignment.AssignUserToGroup(userId, userGroupId);

            // 5. Save all changes
            sqlExec.CommitTransaction();

            if (userGroupId == 1 || userGroupId == 2) // hgi admin and sec. admin only
            {
               // Get fleet ID
               Fleet fleet = new Fleet(this.ConnectionString);
               int fleetId = fleet.GetFleetIdByFleetName(userInfo.organizationId, "All Vehicles");
               // assing user to the default fleet
               if (fleetId > 0 && userId > 0)
                  fleet.AddUserToFleet(fleetId, userId);
            }
         }
         catch (SqlException objException)
         {
            // 5. Rollback all changes
            sqlExec.RollbackTransaction();
            Util.ProcessDbException("Unable to add new user to the group ", objException);
         }
         catch (DASDbConnectionClosed exCnn)
         {
            sqlExec.RollbackTransaction();
            throw new DASDbConnectionClosed(exCnn.Message);
         }
         catch (Exception objException)
         {
            // 5. Rollback all changes
            sqlExec.RollbackTransaction();
            throw new DASException(objException.Message);
         }
      }

		/// <summary>
		/// Delete existing user group assignment.
		/// </summary>
		/// <param name="userGroupId"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if group does not exists.</exception>
		/// <exception cref="DASException">Thrown in all other cases.</exception>
		public int DeleteGroupAssignment(short userGroupId)
		{
			return userGroupAssignment.DeleteGroupAssignment(userGroupId);
		}
		/// <summary>
		/// Delete existing user assignment.
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="userGroupId"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user or group does not exists.</exception>
		/// <exception cref="DASException">Thrown in all other cases.</exception>
		public int DeleteUserAssignment(int userId,short userGroupId)
		{
			return userGroupAssignment.DeleteUserAssignment(userId,userGroupId);
		}
		/// <summary>
		/// Delete existing user assignment.
		/// </summary>
		/// <param name="userId"></param> 
		/// <param name="userGroupName"></param> 
		/// <returns>void</returns>
		/// <exception cref="DASAppResultNotFoundException">Thrown if user or group does not exists.</exception>
		/// <exception cref="DASException">Thrown in all other cases.</exception>
		public int DeleteUserAssignment(int userId,string userGroupName)
		{
			return userGroupAssignment.DeleteUserAssignment(userId,userGroup.GetUserGroupIdByName(userGroupName));
		}

		/// <summary>
		/// Returns all user assigned to the group. 
		/// </summary>
		/// <param name="currUserId"></param>
		/// <param name="userGroupName"></param>
		/// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetUsersByUserGroup(int currUserId,string userGroupName)
		{
			short userGroupId = GetUserGroupIdByName(userGroupName);
			return GetUsersByUserGroup(currUserId,userGroupId);
		}			

		/// <summary>
		/// Returns all user assigned to the group. 
		/// </summary>
		/// <param name="currUserId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetUsersByUserGroup(int currUserId,short userGroupId)
		{
			DataSet dsResult = userGroupAssignment.GetUsersByUserGroup(currUserId,userGroupId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllUsersInTheGroup" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}			
		/// <summary>
		/// Returns all user assigned to the group in current organization. 
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="organizationId"></param>
		/// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllUnassignedUsersToUserGroup(short userGroupId,int organizationId)
		{
			DataSet dsResult = userGroupAssignment.GetAllUnassignedUsersToUserGroup(userGroupId,organizationId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "AllUnassignedToGroupUsers" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}
		/// <summary>
		/// Returns all user assigned to the group. 
		/// </summary>
		/// <param name="userGroupName"></param>
		/// <param name="organizationId"></param>
		/// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAllUnassignedUsersToUserGroup(string userGroupName,int organizationId)
		{
			short userGroupId = GetUserGroupIdByName(userGroupName);
			return GetAllUnassignedUsersToUserGroup(userGroupId,organizationId);
		}
		/// <summary>
		/// Returns all user assigned to the group. 
		/// </summary>
		/// <param name="currUserId"></param>
		/// <param name="userGroupId"></param>
		/// <returns>ArrayList of users ids assigned to the group</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public ArrayList GetUsersArrayByGroup(int currUserId,short userGroupId)
		{
			return userGroupAssignment.GetUsersArrayByGroup(currUserId,userGroupId);
		}			
		/// <summary>
		/// Returns all groups included this user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>DataSet [UserGroupId],[UserGroupName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAssignedGroupsByUser(int userId)
		{
			DataSet dsResult = userGroupAssignment.GetAssignedGroupsByUser(userId);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "UserAllGroups" ;
				}
				dsResult.DataSetName = "UserGroup";
			}
			return dsResult;
		}	
		/// <summary>
		/// Returns all groups included users from current organization. 
		/// </summary>
		/// <param name="organizationName"></param>
		/// <returns>DataSet [UserGroupId],[UserGroupName]</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public DataSet GetAssignedGroupsByOrganizationName(string organizationName)
		{
			DataSet dsResult = userGroupAssignment.GetAllUserGroupsByOrganizationName(organizationName);
			if(dsResult != null) 
			{
				if(dsResult.Tables.Count > 0)
				{
					dsResult.Tables[0].TableName = "OrganizationUserGroups" ;
				}
				dsResult.DataSetName = "UserGroups";
			}
			return dsResult;
		}	
		/// <summary>
		/// Returns all groups included this user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>ArrayList of groups ids included this user</returns>
		/// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
		public ArrayList GetGroupsArrayByUser(int userId)
		{
			return userGroupAssignment.GetGroupsArrayByUser(userId);
		}			
		/// <summary>
		/// Returns total number of groups included this user. 
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>int</returns>
		/// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
		public int GetTotalAssignedGroupsByUser(int userId)
		{
			int retResult = 0;
			DataSet dsResult = userGroupAssignment.GetAssignedGroupsByUser(userId);
			if(dsResult != null)
			{
				retResult = dsResult.Tables[0].Rows.Count;
			}
			return retResult;
		}

        /// <summary>
        /// Retrieves child Controls
        /// </summary>
        /// <param name="ParentControlId"></param>
        /// <returns>Dataset</returns>
        public DataSet GetChildControls(int ParentControlId)
        {
            return userGroup.GetChildControls(ParentControlId);
        }

		#endregion

        #region Operations Assignment

        /// <summary>
        /// 
        /// </summary>
        /// <param name="showAssigned"></param>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public DataSet GetUserGroupOperations(bool showAssigned, Enums.OperationType operationType, int userGroupId)
        {
            DataSet dsResult = null;
            string tableName = "";
            if (showAssigned)
            {
                dsResult = userGroup.GetUserGroupAssignedOperations(operationType, userGroupId);
                tableName = "AssignedOperations";
            }
            else
            {
                dsResult = userGroup.GetUserGroupUnassignedOperations(operationType, userGroupId);
                tableName = "UnassignedOperations";
            }

            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = tableName;
                }
                dsResult.DataSetName = "Operations";
            }

            return dsResult;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <param name="opIds"></param>
        /// <returns></returns>
        public int AssignOperationsToUserGroup(Enums.OperationType operationType, int userGroupId, int[] opIds)
        {
            int rowAffected = 0;
            try
            {                
                // 1. begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                for(int i=0;i<opIds.Length;i++)
                {
                    rowAffected += userGroup.AssignOperationToUserGroup(operationType,userGroupId,opIds[i]);
                }
                sqlExec.CommitTransaction();
            }
            catch
            {
                sqlExec.RollbackTransaction();
                throw;
            }

            return rowAffected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="userGroupId"></param>
        /// <param name="opIds"></param>
        /// <returns></returns>
        public int UnassignOperationsToUserGroup(Enums.OperationType operationType, int userGroupId, int[] opIds)
        {
            int rowAffected = 0;
            try
            {
                // 1. begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                for (int i = 0; i < opIds.Length; i++)
                {
                    rowAffected += userGroup.UnassignOperationToUserGroup(operationType, userGroupId, opIds[i]);
                }
                sqlExec.CommitTransaction();
            }
            catch
            {
                sqlExec.RollbackTransaction();
                throw;
            }

            return rowAffected;
        }
        #endregion
    }
}
