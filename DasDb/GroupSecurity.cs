using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
	/// <summary>
	/// Provides interfaces to vlfGroupSecuritytable.
	/// </summary>
	public class GroupSecurity : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public GroupSecurity(SQLExecuter sqlExec) : base ("vlfGroupSecurity",sqlExec)
		{
		}					
		/// <summary>
		/// Adds new group security
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="operationId"></param>
		/// <param name="operationType"></param>
		/// <returns>void</returns>
		/// <exception cref="DASAppDataAlreadyExistsException">Thrown if group security already exist</exception>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public void AddGroupSecurity(short userGroupId,int operationId,int operationType)
		{
			//Prepares SQL statement
			string sql = string.Format("INSERT INTO " + tableName 
									+ " (UserGroupId,OperationId,OperationType)"
									+ " VALUES ( {0}, {1}, {2})",
									userGroupId,operationId,operationType);
			int rowsAffected = 0;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to add new group security for group id '" + userGroupId + "'.";		
				Util.ProcessDbException(prefixMsg,objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to add new group security for group id '" + userGroupId + "'.";		
				throw new DASException(prefixMsg + " " + objException.Message);
			}

			if(rowsAffected == 0) 
			{
				string prefixMsg = "Unable to add new group security for group id '" + userGroupId + "'.";		
				throw new DASAppDataAlreadyExistsException(prefixMsg + " The group security already exists.");
			}
		}		
		/// <summary>
		/// Delete group security
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <returns> int rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteGroupSecurity(short userGroupId)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "DELETE FROM " + tableName + " WHERE UserGroupId =" + userGroupId;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete group security " + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete group security " + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Delete group security
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <param name="operationId"></param>
		/// <param name="operationType"></param>
		/// <returns> int rows affected</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public int DeleteGroupSecurity(short userGroupId,int operationId,int operationType)
		{
			int rowsAffected = 0;
			//Prepares SQL statement
			string sql = "DELETE FROM " + tableName + " WHERE UserGroupId =" + userGroupId +
						" AND OperationId=" + operationId + " AND OperationType=" + operationType;
			try
			{
				//Executes SQL statement
				rowsAffected = sqlExec.SQLExecuteNonQuery(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to delete group security " + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg,objException);
			}

			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to delete group security " + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return rowsAffected;
		}
		/// <summary>
		/// Retieves group security information. 	
		/// </summary>
		/// <param name="userGroupId"></param>
		/// <returns>[UserGroupId],[OperationId],[OperationType]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetGroupSecurityInfo(short userGroupId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT * FROM " + tableName + " WHERE UserGroupId=" + userGroupId;
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve group security info by user group " + userGroupId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve group security info by user group " + userGroupId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

		/// <summary>
		/// Retieves group security full info
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns>[OperationType],[OperationTypeName],[OperationId],[OperationName],[OperationAction]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetGroupSecurityFullInfo(short groupId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
				string sql = "DECLARE @UserGroup INT SET @UserGroup=" + groupId +
					" SELECT vlfOperationType.OperationType,vlfOperationType.OperationTypeName,vlfGroupSecurity.OperationId,vlfBoxCmdOutType.BoxCmdOutTypeName AS OperationName,' ' AS OperationAction"+
					" FROM vlfGroupSecurity INNER JOIN vlfBoxCmdOutType ON vlfGroupSecurity.OperationId = vlfBoxCmdOutType.BoxCmdOutTypeId"+
					" INNER JOIN vlfOperationType ON vlfGroupSecurity.OperationType = vlfOperationType.OperationType"+
					" WHERE vlfOperationType.OperationType=2 AND vlfGroupSecurity.UserGroupId=@UserGroup"+
					" UNION SELECT vlfOperationType.OperationType, vlfOperationType.OperationTypeName,vlfGroupSecurity.OperationId, vlfOutput.OutputName AS OperationName,vlfOutput.OutputAction AS OperationAction"+
					" FROM vlfGroupSecurity INNER JOIN vlfOperationType ON vlfGroupSecurity.OperationType = vlfOperationType.OperationType"+
					" INNER JOIN vlfOutput ON vlfGroupSecurity.OperationId = vlfOutput.OutputId"+
					" WHERE vlfOperationType.OperationType=1 AND vlfGroupSecurity.UserGroupId=@UserGroup"+
					" UNION SELECT vlfOperationType.OperationType, vlfOperationType.OperationTypeName, vlfGroupSecurity.OperationId,vlfGuiControls.ControlName AS OperationName,vlfGuiControls.Description AS OperationAction"+
					" FROM vlfGroupSecurity INNER JOIN vlfOperationType ON vlfGroupSecurity.OperationType = vlfOperationType.OperationType"+
					" INNER JOIN vlfGuiControls ON vlfGroupSecurity.OperationId = vlfGuiControls.ControlId"+
					" WHERE vlfOperationType.OperationType=3 AND vlfGroupSecurity.UserGroupId=@UserGroup"+
					" UNION SELECT vlfOperationType.OperationType, vlfOperationType.OperationTypeName,vlfGroupSecurity.OperationId,vlfReportTypes.ReportTypesName AS OperationName,' ' AS OperationAction"+
					" FROM vlfGroupSecurity INNER JOIN vlfOperationType ON vlfGroupSecurity.OperationType = vlfOperationType.OperationType"+
					" INNER JOIN vlfReportTypes ON vlfGroupSecurity.OperationId = vlfReportTypes.ReportTypesId"+
					" WHERE vlfOperationType.OperationType=4 AND vlfGroupSecurity.UserGroupId = @UserGroup AND (NOT vlfReportTypes.GuiId IS NULL)"+
					" ORDER BY vlfOperationType.OperationType,OperationName";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user group=" + groupId + " security info. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user group=" + groupId + " security info. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

		/// <summary>
		/// Retieves group security full info
		/// </summary>
		/// <returns>[OperationType],[OperationTypeName],[OperationId],[OperationName],[OperationAction]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetAllGroupSecurityFullInfo()
		{
			DataSet resultDataSet = null;
			try
			{
				//TODO: Prepares SQL statement
				string sql = "SELECT 1 AS OperationType, 'Output' AS OperationTypeName,OutputId AS OperationId,OutputName AS OperationName,OutputAction AS OperationAction FROM vlfOutput"+
							" UNION SELECT 2 AS OperationType, 'Command' AS OperationTypeName,BoxCmdOutTypeId AS OperationId,BoxCmdOutTypeName AS OperationName,' ' AS OperationAction FROM vlfBoxCmdOutType"+
							" WHERE BoxCmdOutTypeId<>" + (short)VLF.CLS.Def.Enums.CommandType.Ack +
							" AND BoxCmdOutTypeId<>" + (short)VLF.CLS.Def.Enums.CommandType.Output +
							" AND BoxCmdOutTypeId<>" + (short)VLF.CLS.Def.Enums.CommandType.DAck +
							" AND BoxCmdOutTypeId<>" + (short)VLF.CLS.Def.Enums.CommandType.IdnAck +
							" AND BoxCmdOutTypeId<>" + (short)VLF.CLS.Def.Enums.CommandType.EndCall +
							" AND BoxCmdOutTypeId<>" + (short)VLF.CLS.Def.Enums.CommandType.MDTAck +
							" UNION SELECT 3 AS OperationType, 'Gui element' AS OperationTypeName,ControlId AS OperationId,ControlName AS OperationName,vlfGuiControls.Description AS OperationAction FROM vlfGuiControls"+
							" UNION SELECT 4 AS OperationType, 'Reports' AS OperationTypeName,ReportTypesId AS OperationId,ReportTypesName AS OperationName,' ' AS OperationAction FROM vlfReportTypes"+
							" WHERE NOT vlfReportTypes.GuiId IS NULL"+
							" ORDER BY vlfOperationType.OperationType,OperationName";
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve all user groups security info. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve all user groups security info. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}


		#region Additional Security
		/// <summary>
		/// Retieves user controls
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>[ControlId]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserControls(int userId)
		{
			DataSet resultDataSet = null;
			try
			{
				//Prepares SQL statement
                string sql = "SELECT DISTINCT vlfGuiControls.ControlId" +
						" FROM vlfGroupSecurity INNER JOIN vlfGuiControls ON vlfGroupSecurity.OperationId = vlfGuiControls.ControlId"+
						" INNER JOIN vlfUserGroup ON vlfGroupSecurity.UserGroupId = vlfUserGroup.UserGroupId"+
						" INNER JOIN vlfUserGroupAssignment ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId"+
						" WHERE vlfGroupSecurity.OperationType=" + (short)VLF.CLS.Def.Enums.OperationType.Gui + 
						" AND vlfUserGroupAssignment.UserId=" + userId;
				//Executes SQL statement
				resultDataSet = sqlExec.SQLExecuteDataset(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user " + userId + " controls. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user " + userId + " controls. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}
		/// <summary>
		/// Retieves user reports
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>[ReportTypesId],[ReportTypesName],[GuiId],[GuiName]</returns>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public DataSet GetUserReports(int userId)
		{
			DataSet resultDataSet = null;
			try
			{
               //// //Prepares SQL statement
               //// string sql = "SELECT DISTINCT vlfReportTypes.ReportTypesName,GuiId,GuiName,vlfReportTypes.ReportTypesId" +
               ////         " FROM vlfGroupSecurity INNER JOIN vlfUserGroup ON vlfGroupSecurity.UserGroupId = vlfUserGroup.UserGroupId"+
               ////         " INNER JOIN vlfUserGroupAssignment ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId"+
               ////         " INNER JOIN vlfReportTypes ON vlfGroupSecurity.OperationId = vlfReportTypes.ReportTypesId"+
               ////         " WHERE vlfGroupSecurity.OperationType=" + (short)VLF.CLS.Def.Enums.OperationType.Repors + 
               ////         " AND vlfUserGroupAssignment.UserId=" + userId +
               ////         " AND NOT (GuiId IS NULL)"+
               ////         " AND Category=0" +
               ////         " ORDER BY GUIName";
               ////// Executes SQL statement
               //// resultDataSet = sqlExec.SQLExecuteDataset(sql);


                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@category", SqlDbType.Int, 0);

                resultDataSet = sqlExec.SPExecuteDataset("GetUserReports");

			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to retrieve user " + userId + " reports. ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to retrieve user " + userId + " reports. ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			return resultDataSet;
		}

        /// <summary>
        /// Retieves user reports
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="category"></param>
        /// <returns>[ReportTypesId],[ReportTypesName],[GuiId],[GuiName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetUserReportsByCategory(int userId,int category)
        {
            DataSet resultDataSet = null;
            try
            {
                //////Prepares SQL statement
                ////string sql = "SELECT DISTINCT vlfReportTypes.ReportTypesId,vlfReportTypes.ReportTypesName,GuiId,GuiName" +
                ////        " FROM vlfGroupSecurity INNER JOIN vlfUserGroup ON vlfGroupSecurity.UserGroupId = vlfUserGroup.UserGroupId" +
                ////        " INNER JOIN vlfUserGroupAssignment ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId" +
                ////        " INNER JOIN vlfReportTypes ON vlfGroupSecurity.OperationId = vlfReportTypes.ReportTypesId" +
                ////        " WHERE vlfGroupSecurity.OperationType=" + (short)VLF.CLS.Def.Enums.OperationType.Repors +
                ////        " AND vlfUserGroupAssignment.UserId=" + userId +
                ////        " AND NOT (GuiId IS NULL)" +
                ////        " AND Category=" + category +
                ////        " ORDER BY GUIName";
                //////Executes SQL statement
                ////resultDataSet = sqlExec.SQLExecuteDataset(sql);

                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
                sqlExec.AddCommandParam("@category", SqlDbType.Int, category);

                resultDataSet = sqlExec.SPExecuteDataset("GetUserReports");

            }
            catch (SqlException objException)
            {
                string prefixMsg = "Unable to retrieve user " + userId + " reports. ";
                Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                string prefixMsg = "Unable to retrieve user " + userId + " reports. ";
                throw new DASException(prefixMsg + " " + objException.Message);
            }
            return resultDataSet;
        }

		/// <summary>
		/// Authorize user operation
		/// </summary>
      /// <returns>true if user is authorized for this operation, otherwise return false</returns>
		/// <param name="userId"></param>
		/// <param name="operationType"></param>
		/// <param name="operationId"></param>
		/// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
		/// <exception cref="DASException">Thrown in all other exception cases.</exception>
		public bool AuthorizeOperation(int userId,VLF.CLS.Def.Enums.OperationType operationType,int operationId)
		{
			int rowsFound = 0;
			try
			{
				//Prepares SQL statement
				string sql = "SELECT COUNT(vlfGroupSecurity.OperationId) AS OperationId"+
					" FROM vlfGroupSecurity INNER JOIN vlfUserGroup ON vlfGroupSecurity.UserGroupId = vlfUserGroup.UserGroupId"+
					" INNER JOIN vlfUserGroupAssignment ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId"+
					" WHERE vlfGroupSecurity.OperationType=" + Convert.ToInt32(operationType) + 
					" AND vlfUserGroupAssignment.UserId=" + userId +
					" AND vlfGroupSecurity.OperationId=" + operationId;

				//Executes SQL statement
				rowsFound = (int)sqlExec.SQLExecuteScalar(sql);
			}
			catch (SqlException objException) 
			{
				string prefixMsg = "Unable to authorize user " + userId + " operationType=" + operationType.ToString() + " operationId=" + operationId + ". ";
				Util.ProcessDbException(prefixMsg, objException);
			}
			catch(DASDbConnectionClosed exCnn)
			{
				throw new DASDbConnectionClosed(exCnn.Message);
			}
			catch(Exception objException)
			{
				string prefixMsg = "Unable to authorize user " + userId + " operationType=" + operationType.ToString() + " operationId=" + operationId + ". ";
				throw new DASException(prefixMsg + " " + objException.Message);
			}
			if(rowsFound == 0)
				return false;
			else
				return true;
		}

      /// <summary>
      /// Authorize user - web method
      /// </summary>
      /// <returns>true if user is authorized for this operation, otherwise return false</returns>
      /// <param name="userId"></param>
      /// <param name="methodName"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool AuthorizeWebMethod(int userId, string methodName)
      {
         int rowsFound = 0;
         string prefixMsg = String.Format("User <{0}> is not authorized to run Web Method <{1}>.", userId, methodName);
         try
         {
            //Prepares SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@opName", SqlDbType.VarChar, methodName);
            string sql = "SELECT COUNT(vlfGroupSecurity.OperationId) AS Operation FROM vlfGroupSecurity " + 
               "INNER JOIN vlfUserGroup ON vlfGroupSecurity.UserGroupId = vlfUserGroup.UserGroupId " + 
               "INNER JOIN vlfUserGroupAssignment ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId " + 
               "INNER JOIN vlfOperationType ON vlfGroupSecurity.OperationType = vlfOperationType.OperationType " + 
               "INNER JOIN vlfWebMethods ON vlfWebMethods.MethodId = vlfGroupSecurity.OperationId" +
               "WHERE (vlfUserGroupAssignment.UserId = @userId) AND " +
               "(vlfWebMethods.MethodName = @opName) AND " + 
               "(vlfOperationType.OperationTypeName = 'WebMethod')";

            //Executes SQL statement
            rowsFound = (int)sqlExec.SQLExecuteScalar(sql);
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
            throw new DASException(prefixMsg + " :: " + objException.Message);
         }
         return (rowsFound > 0);
      }

      /// <summary>
      /// Authorize user - web method
      /// </summary>
      /// <returns>true if user is authorized for this operation, otherwise return false</returns>
      /// <param name="userId"></param>
      /// <param name="methodName"></param>
      /// <param name="className"></param>
      /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
      /// <exception cref="DASException">Thrown in all other exception cases.</exception>
      public bool AuthorizeWebMethod(int userId, string methodName, string className)
      {
         int rowsFound = 0;
         string prefixMsg = String.Format("User <{0}> is not authorized to run Web Method <{1}>.", userId, methodName);
         try
         {
            //Prepares SQL statement
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@opName", SqlDbType.VarChar, methodName);
            sqlExec.AddCommandParam("@class", SqlDbType.VarChar, className);
            string sql = "SELECT COUNT(vlfGroupSecurity.OperationId) AS Operation FROM vlfGroupSecurity " +
               "INNER JOIN vlfUserGroup ON vlfGroupSecurity.UserGroupId = vlfUserGroup.UserGroupId " +
               "INNER JOIN vlfUserGroupAssignment ON vlfUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId " +
               "INNER JOIN vlfOperationType ON vlfGroupSecurity.OperationType = vlfOperationType.OperationType " +
               "INNER JOIN vlfWebMethods ON vlfWebMethods.MethodId = vlfGroupSecurity.OperationId" +
               "WHERE vlfUserGroupAssignment.UserId = @userId AND " +
               "vlfWebMethods.MethodName = @opName AND vlfWebMethods.MethodClass = @class AND " +
               "vlfOperationType.OperationTypeName = 'WebMethod'";

            //Executes SQL statement
            rowsFound = (int)sqlExec.SQLExecuteScalar(sql);
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
            throw new DASException(prefixMsg + " :: " + objException.Message);
         }
         return (rowsFound > 0);
      }

      #endregion
	}
}

