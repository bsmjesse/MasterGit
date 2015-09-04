using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.IO;
using VLF.ERRSecurity;
using VLF.DAS.Logic;
using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;

namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "http://www.sentinelfm.com")]

    /// <summary>
    /// Summary description for DBUser.
    /// </summary>
    public class DBUser : System.Web.Services.WebService
    {
        public DBUser()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code

        //Required by the Web Services Designer 
        private IContainer components = null;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
        ///      by replacing the log calls we can add a UDP sender for logs
        ///      or dynamic filtering 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void Log(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }


        /// <summary>
        ///      the exception should be saved in a separate file or in the Event log of the computer
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void LogException(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }

        private bool ValidateUserPreference(int userId, string SID, int preferenceId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUserVal = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUserVal.ValidateUserPreference(userId, preferenceId);
            }
        }

        private bool ValidateSuperUser(int userId, string SID)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUserVal = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUserVal.ValidateSuperUser(userId);
            }
        }




        #region User Preferences
        /// <summary>
        ///         Retrieves all user preferences info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xml">
        ///   XML File Format: [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]
        /// </param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves all user preferences info. XML File Format: [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]")]
        public int GetUserPreferencesXML(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserPreferencesXML(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetAllUserPreferencesInfo(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserPreferencesXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add a user preferences.")]
        public int AddUserPreference(int userId, string SID, int preferenceId, string preferenceValue)
        {
            try
            {
                Log(">> AddUserPreference(uId={0}, preferenceId={1}, preferenceValue={2})",
                         userId, preferenceId, preferenceValue);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.AddUserPreference(userId, preferenceId, preferenceValue);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserPreference : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update a user preferences.")]
        public int UpdateUserPreference(int userId, string SID, int preferenceId, string preferenceValue)
        {
            try
            {
                Log(">>UpdateUserPreference(uId={0}, preferenceId={1}, preferenceValue={2})",
                         userId, preferenceId, preferenceValue);

                if (!ValidateUserPreference(userId, SID, preferenceId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.UpdateUserPreference(userId, preferenceId, preferenceValue);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateUserPreference : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }





        [WebMethod(Description = "Delete all user preferences.")]
        public int DeleteUserPreferences(int userId, string SID, ref int rowsAffected)
        {
            try
            {
                Log(">> DeleteUserPreferences(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUser.DeleteUserPreferences(userId);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserPreferences : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete a user preference.")]
        public int DeleteUserPreference(int userId, string SID, int preferenceId, ref int rowsAffected)
        {
            try
            {
                Log(">> DeleteUserPreference(uId={0}, preferenceId={1})", userId, preferenceId);

                if (!ValidateUserPreference(userId, SID, preferenceId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUser.DeleteUserPreference(userId, preferenceId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserPreference(uId={0}, preferenceId={1}, EXC={2})", userId, preferenceId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add/Update a user preferences.")]
        public int UserPreference_Add_Update(int userId, string SID, int preferenceId, string preferenceValue)
        {
            try
            {
                Log(">>Add_Update_UserPreference(uId={0}, preferenceId={1}, preferenceValue={2})",
                            userId, preferenceId, preferenceValue);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                //if (!ValidateUserPreference(userId, SID, preferenceId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.UserPreference_Add_Update(userId, preferenceId, preferenceValue);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Add_Update_UserPreference(uId={0}, preferenceId={1}, EXC={2})", userId, preferenceId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region User Interfaces

        [WebMethod(Description = "Returns all groups that user belong to. XML File Format: [UserGroupId],[UserGroupName]")]
        public int GetAssignedGroupsByUser(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAssignedGroupsByUser(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUserGroup.GetAssignedGroupsByUser(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAssignedGroupsByUser: uId={0},  EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves a user info. XML File Format: [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],[OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],[Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]")]
        public int GetUserInfoByUserId(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserInfoByUserId(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetUserInfoByUserId(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserInfoByUserId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="usrId">user Id to get its info</param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves a user info. XML File Format: [UserId],[UserName],[Password],[PersonId],[DriverLicense],[FirstName],[LastName],[OrganizationId],[Birthday],[PIN],[Address],[City],[StateProvince],[Country],[PhoneNo1],[PhoneNo2],[CellNo],[Description],[OrganizationName],[ExpiredDate]")]
        public int GetUserInfoById(int userId, string SID, int usrId, ref string xml)
        {
            try
            {
                Log(">> GetUserInfoById(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetUserInfoByUserId(usrId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = XmlUtil.GetXmlIncludingNull(dsInfo); //dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserInfoById : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Change a user password")]
        public int ChangeUserPasswordByUserName(int userId, string SID, string userName,
                                                        string oldPassword, string newPassword)
        {
            try
            {
                Log(">> ChangeUserPasswordByUserName(userName={0},oldPassword = *****, newPassword = ***** )", userName);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    int validUserId = dbUser.ValidateUserMD5(userName, VLF.DAS.Logic.User.GetMD5HashData(oldPassword));
                    if (validUserId == VLF.CLS.Def.Const.unassignedIntValue)
                        throw new VLF.ERR.ASIAuthenticationFailedException("ChangeUserPasswordByUserName::Authentication for user " + userName + " failed.");

                    dbUser.SetHashPasswordByUserId(newPassword, validUserId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< ChangeUserPasswordByUserName : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Reset Password by SuperUser")]
        public int ResetPasswordBySuperUser(int userId, int userIdUpdated, string SID, string newPassword)
        {
            try
            {
                Log(">> ResetPasswordBySuperUser(uId={0}, forUserId={1})", userId, userIdUpdated);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                //Authorization
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.SetHashPasswordByUserId(newPassword, userIdUpdated);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< ResetPasswordBySuperUser : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add a new user.")]
        public int AddNewUser(int currUserId, string SID, string userName, string personId,
                       string password, string firstName, string lastName, string expiredDate)
        {
            try
            {
                Log(">> AddNewUser(currUserId={0},userName = {1}, personId = {2}, firstName = {3}, lastName = {4}, expiredDate = {5})",
                               currUserId, userName, personId, firstName, lastName, expiredDate);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                //Authorization
                using (VLF.DAS.Logic.User dbUserVal = new User(LoginManager.GetConnnectionString(currUserId)))
                {
                    if (!dbUserVal.ValidateSuperUser(currUserId))
                        return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                    int orgId = dbUserVal.GetOrganizationIdByUserId(currUserId);
                    CreateDBUser(currUserId, orgId, userName, personId, password, firstName, lastName, expiredDate);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddNewUser : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete a user.")]
        public int DeleteUserByUserId(int currUserId, string SID, int userId)
        {
            try
            {
                Log(">> DeleteUserByUserId( currUserId = {0}, userId = {1})", currUserId, userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 19);

                //Authorization
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    if (!dbUser.ValidateSuperUserOne(currUserId))
                        return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                    dbUser.DeleteUserByUserId(userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserByUserId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update a user info.")]
        public int UpdateInfo(int currUserId, string SID, int userId, string userName, string firstName, string lastName, string expiredDate)
        {
            try
            {
                Log(">> UpdateInfo(currUserId={0},userId={1}, userName={2}, firstName={3}, lastName={4}, expiredDate={5})",
                      currUserId, userId, userName, firstName, lastName, expiredDate);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 20);

                //Authorization
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.UpdateInfo(userId, userName, firstName, lastName, Convert.ToDateTime(expiredDate));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateInfo : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update a user info.<br /> same as UpdateInfo but it takes UserInfo structure as parameter")]
        public int UpdateUserInfo(int userId, string SID, string xmlUserInfo)
        {
            try
            {
                UserInfo userInfo = (UserInfo)XmlUtil.FromXml(xmlUserInfo, typeof(UserInfo));
                Log(">> UpdateUserInfo(userId={0}, userName={1})",
                      userId, userInfo.username);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 20);

                //Authorization
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.UpdateUserInfo(userInfo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateUserInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region User/Group assignment

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xmlUserInfo"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add New User (default preferences, assigned to default fleet allVehicles) and assign him to the user group")]
        public int AddNewUserToGroup(int userId, string SID, string xmlUserInfo, short userGroupId)
        {
            try
            {
                Log(">> AddNewUserToGroup(userId = {0},userGroupId = {1}", userId, userGroupId);
                //Authorization
                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                UserInfo userInfo = (UserInfo)XmlUtil.FromXml(xmlUserInfo, typeof(UserInfo));

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dbUserGroup.AddUserToGroupAssignFleet(userGroupId, userInfo.username, userInfo);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddNewUserToGroup : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add a new user to group, assign a default fleet.")]
        public int AddUserToGroup(int userId, string SID, int organizationId, string userName, string personId,
           string password, string firstName, string lastName, string expiredDate, short userGroupId)
        {
            try
            {
                Log(">> AddUserToGroup(userId = {0},userName = {1}, personId = {2}, firstName = {3}, lastName = {4}, expiredDate = {5})",
                       userId, userName, personId, firstName, lastName, expiredDate);

                //Authorization
                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                // create new user and person
                int newUserId = CreateDBUser(userId, organizationId, userName, personId, password, firstName, lastName, expiredDate);

                // assign user to group
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dbUserGroup.AssignUserToGroup(newUserId, userGroupId);
                }

                // assign user to default fleet
                if (userGroupId == 1 || userGroupId == 2)
                {
                    using (Fleet fleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                    {
                        int fleetId = fleet.GetFleetIdByFleetName(organizationId, "All Vehicles");
                        // assign user to the default fleet
                        if (fleetId > 0 && newUserId > 0)
                            fleet.AddUserToFleet(fleetId, newUserId);
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserToGroup : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Assign a user to group.")]
        public int AssignUserToGroup(int currUserId, string SID, int userId, short userGroupId)
        {
            try
            {
                Log(">> AssignUserToGroup(currUserId={0},userId={1},userGroupId={2})", currUserId, userId, userGroupId);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dbUserGroup.AssignUserToGroup(userId, userGroupId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AssignUserToGroup : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Remove a user from group.")]
        public int DeleteUserAssignment(int currUserId, string SID, int userId, short userGroupId)
        {
            try
            {
                Log(">> DeleteUserAssignment(currUserId={0},userId={1},userGroupId={2})", currUserId, userId, userGroupId);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dbUserGroup.DeleteUserAssignment(userId, userGroupId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserAssignment : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all user assigned to the group. XML File format: [UserId],[UserName],[FirstName],[LastName]")]
        public int GetUsersByUserGroup(int currUserId, string SID, short userGroupId, ref string xml)
        {
            try
            {
                Log(">> GetUsersByUserGroup(currUserId={0}, userGroupId={1})", currUserId, userGroupId);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUsersByUserGroup(currUserId, userGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersByUserGroup : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all organization users assigned to the group. XML File format: [UserId],[UserName],[FirstName],[LastName]")]
        public int GetUsersByUserGroupAndOrganization(int currUserId, string SID, short userGroupId, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetUsersByUserGroupAndOrganization(currUserId={0}, userGroupId={1})", currUserId, userGroupId);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                DataSet dsInfo = null;
                using (Organization dbo = new Organization(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbo.GetOrganizationUsersByUserGroup(organizationId, userGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersByUserGroupAndOrganization : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all unassigned users to specific group. XML File format: [UserId],[UserName],[FirstName],[LastName]")]
        public int GetAllUnassignedUsersToUserGroup(int currUserId, string SID, short userGroupId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedUsersToUserGroup ( currUserId = {0}, userGroupId = {1})", currUserId, userGroupId);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUser.GetUserInfoByUserId(currUserId);
                    if (dsInfo == null || dsInfo.Tables.Count == 0 || dsInfo.Tables[0].Rows.Count == 0)
                        throw new VLF.ERR.ASIDataNotFoundException();
                }

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    DataSet dsResult = dbUserGroup.GetAllUnassignedUsersToUserGroup(userGroupId, Convert.ToInt32(dsInfo.Tables[0].Rows[0]["OrganizationId"]));
                    if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                        xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedUsersToUserGroup : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="SID"></param>
        /// <param name="userGroupId"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Returns all unassigned users to specific group and organization. XML File format: [UserId],[UserName],[FirstName],[LastName]")]
        public int GetAllUnassignedUsersToUserGroupByOrganization(int currUserId, string SID, short userGroupId, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedUsersToUserGroupByOrganization ( currUserId = {0}, userGroupId = {1}, organizationId= {2})",
                                 currUserId, userGroupId, organizationId);

                if (!ValidateSuperUser(currUserId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    DataSet dsResult = dbUserGroup.GetAllUnassignedUsersToUserGroup(userGroupId, organizationId);
                    if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                        xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedUsersToUserGroupByOrganization : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all users groups. XML File format: [UserGroupId],[UserGroupName]")]
        public int GetAllUserGroups(int currUserId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllUserGroups(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                bool includeHgiAdmin = false;
                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetAllUserGroups(includeHgiAdmin);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUserGroups : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="SID"></param>
        /// <param name="includeHgiAdmin"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Returns all users groups. XML File format: [UserGroupId],[UserGroupName]")]
        public int GetUserGroups(int currUserId, string SID, bool includeHgiAdmin, ref string xml)
        {
            try
            {
                Log(">> GetUserGroups(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                //bool includeHgiAdmin = false;
                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetAllUserGroups(includeHgiAdmin);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroups : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Returns all users groups. XML File format: [UserGroupId],[UserGroupName]")]
        public int GetAllUserGroupsByLang(int currUserId, string SID, string lang, ref string xml)
        {
            try
            {
                Log(">> GetAllUserGroupsByLang(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                bool includeHgiAdmin = false;
                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetAllUserGroups(includeHgiAdmin);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();


                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(currUserId));
                        dbl.LocalizationData(lang, "UserGroupId", "UserGroupName", "UserGroup", ref dsInfo);
                    }

                    xml = dsInfo.GetXml();
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUserGroupsByLang : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region User Permissions
        [WebMethod]
        public int GetUserPermissionsXML(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserPermissionsXML( userId = {0} )", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);
                // TODO: Implement
                return (int)InterfaceError.NotImplemented;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserPermissionsXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region User Logins

        /// <summary>
        ///      Get the number of logins between certain dates
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="loginUserId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public int GetUserLogins(int userId, string SID, int loginUserId, string fromDateTime, string toDateTime, ref string xml)
        {
            try
            {
                Log(">> GetUserLogins(uId={0}, loginUserId={1}, dtFrom={2}, dtTo={3})",
                         userId, loginUserId, fromDateTime, toDateTime);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DateTime fromDT = VLF.CLS.Def.Const.unassignedDateTime;
                DateTime toDT = VLF.CLS.Def.Const.unassignedDateTime;

                if (!string.IsNullOrEmpty(fromDateTime))
                    fromDT = Convert.ToDateTime(fromDateTime);

                if (string.IsNullOrEmpty(toDateTime))
                    toDT = Convert.ToDateTime(toDateTime);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetUserLogins(loginUserId, fromDT, toDT);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserLogins : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Get User Last Login
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="loginUserId"></param>
        /// <param name="xml"> XML File format: [LoginId],[UserId],[LoginDateTime],[IP], [UserName],[FirstName],[LastName] </param>
        /// <returns></returns>
        [WebMethod(Description = "Get User Last Login. XML File format: [LoginId],[UserId],[LoginDateTime],[IP], [UserName],[FirstName],[LastName]")]
        public int GetUserLastLogin(int userId, string SID, int loginUserId, ref string xml)
        {
            try
            {
                Log(">> GetUserLastLogin(uId={0}, loginUserId={1})", userId, loginUserId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetUserLastLogin(userId, loginUserId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserLastLogin : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Notifications For User")]
        public int GetAllNotificationsForUserId(int userId, string SID, string fromDateTime, string toDateTime, ref string xml)
        {
            try
            {
                Log(">> GetAllNotificationsForUserId(uId={0},  fromDate={1}, toDate={2})", userId, fromDateTime, toDateTime);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Notification dbNotification = new Notification(LoginManager.GetConnnectionString(userId), false))
                {
                    dsInfo = dbNotification.GetAllNotificationsForUserId(userId, Convert.ToDateTime(fromDateTime), Convert.ToDateTime(toDateTime), 0);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllNotificationsForUserId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Users Dashboards")]
        public int GetUsersDashboards(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUsersDashboards(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetUsersDashboards(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersDashboards : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }






        [WebMethod(Description = "Get Dashboards Types")]
        public int GetDashboardTypes(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetDashboardTypes(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetDashboardTypes();
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDashboardTypes : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        //#region User Localization

        //[WebMethod] 
        //public int LocalizationInfo(string lang, string KeyField,string FieldName, string FieldGroup, ref string xml)
        //{
        //    LocalizationLayer.ServerLocalizationLayer dblocal = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
        //    dblocal.LocalizationData(lang, KeyField, FieldName, FieldGroup, ref xml);
        //    return (int)InterfaceError.NoError;
        //}
        //#endregion

        /// <summary>
        ///         Create SentinelFM DB User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="personId"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="expiredDate"> when the user login expires</param>
        /// <returns>New User Id</returns>
        private int CreateDBUser(int currUserId, int organizationId, string userName, string personId, string password, string firstName, string lastName, string expiredDate)
        {
            using (User dbUser = new User(LoginManager.GetConnnectionString(currUserId)))
            {
                VLF.CLS.Def.Structures.UserInfo userInfo = new VLF.CLS.Def.Structures.UserInfo();
                userInfo.organizationId = organizationId;
                userInfo.personId = personId;
                userInfo.password = password;
                userInfo.hashpassword = VLF.DAS.Logic.User.GetMD5HashData(password);
                userInfo.pin = "";
                userInfo.description = "";
                userInfo.expiredDate = Convert.ToDateTime(expiredDate);

                VLF.CLS.Def.Structures.PersonInfoStruct personInfo = new VLF.CLS.Def.Structures.PersonInfoStruct();
                personInfo.birthday = DateTime.Now;
                personInfo.certifications = "";
                personInfo.description = "";
                personInfo.driverLicense = "";
                personInfo.eyeColor = "";
                personInfo.firstName = firstName;
                personInfo.gender = "";
                personInfo.hairColor = "";
                personInfo.height = 0;
                personInfo.idMarks = "";
                personInfo.lastName = lastName;
                personInfo.licenseEndorsements = "";
                personInfo.licenseExpDate = DateTime.Now;
                personInfo.middleName = "";
                personInfo.personId = personId;
                personInfo.userContactInfo.address = "";
                personInfo.userContactInfo.cellNo = "";
                personInfo.userContactInfo.city = "";
                personInfo.userContactInfo.country = "";
                personInfo.userContactInfo.phoneNo1 = "";
                personInfo.userContactInfo.phoneNo2 = "";
                personInfo.userContactInfo.stateProvince = "";
                personInfo.weight = 0;

                return dbUser.AddNewUser(userName, userInfo, personInfo);
            }
        }


        #region UserGroup Operations Assignment

        [WebMethod(Description = "Return the list of the operations assigned/unassigned to a user group")]
        public int GetUserGroupOperationsByType(int userId, string SID, int userGroupId, short operationType, bool showAssigned, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupOperationsByType(userGroupId={0})", userGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                //bool includeHgiAdmin = false;
                DataSet dsOperations = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dsOperations = dbUserGroup.GetUserGroupOperations(showAssigned, (Enums.OperationType)operationType, userGroupId);
                }

                if (Util.IsDataSetValid(dsOperations))
                    xml = dsOperations.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupOperationsByType : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion
    }
}
