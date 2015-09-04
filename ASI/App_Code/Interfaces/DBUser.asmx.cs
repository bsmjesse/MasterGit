using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
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



        [WebMethod(Description = "Retrieves all user preferences info. XML File Format: [UserId], [PreferenceId], [PreferenceName], [PreferenceValue]")]
        public int GetUserPreferencesXML_ByUserId(int userId, string SID, int userIdToUpdate, ref string xml)
        {
            try
            {
                Log(">> GetUserPreferencesXML(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetAllUserPreferencesInfo(userIdToUpdate);
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


        [WebMethod(Description = "Add/Update a user preferences.")]
        public int UserPreference_Add_Update_ByUserId(int userId, string SID, int userIdToUpdate, int preferenceId, string preferenceValue)
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
                    dbUser.UserPreference_Add_Update(userIdToUpdate, preferenceId, preferenceValue);
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

        [WebMethod(Description = "Returns all groups that user belong to. XML File Format: [UserGroupId],[UserGroupName]")]
        public int GetAssignedGroupsForSelectedUser(int userId, string SID, int SelectedUserID, ref string xml)
        {
            try
            {
                Log(">> GetAssignedGroupsForSelectedUser(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUserGroup.GetAssignedGroupsByUser(SelectedUserID);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAssignedGroupsForSelectedUser: uId={0},  EXC={1})", userId, Ex.Message);
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
                    if (!(new Regex("[A-Z]").Matches(newPassword).Count >= 1 && new Regex("[a-z]").Matches(newPassword).Count >= 1 && new Regex("[0-9]").Matches(newPassword).Count >= 1))
                    {
                        return Convert.ToInt32(InterfaceError.PasswordNotInRules);
                    }
                    if (ValidatePasswordInLastEight(newPassword, validUserId) < 1)
                    {
                        dbUser.SetHashPasswordByUserId(newPassword, validUserId);
                        dbUser.SetUserStatus(validUserId, "Unlocked");
                        LoggerManager.RecordUserAction("User", userId, 0, "vlfUser", "UserId=" + validUserId, "Reset Password",
                                          this.Context.Request.UserHostAddress, this.Context.Request.RawUrl,
                                          VLF.DAS.Logic.User.GetMD5HashData(newPassword));
                    }
                    else
                    {
                        return (int)InterfaceError.PasswordDuplicatedInLastEight;
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< ChangeUserPasswordByUserName : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Change a user password")]
        public int ChangeUserPasswordByUserName_ByUserId(int userId, string SID, int userIdToUpdate, string userName,
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
                    if (!(new Regex("[A-Z]").Matches(newPassword).Count >= 1 && new Regex("[a-z]").Matches(newPassword).Count >= 1 && new Regex("[0-9]").Matches(newPassword).Count >= 1))
                    {
                        return Convert.ToInt32(InterfaceError.PasswordNotInRules);
                    }
                    if (ValidatePasswordInLastEight(newPassword, validUserId) < 1)
                    {
                        dbUser.SetHashPasswordByUserId(newPassword, validUserId);
                        dbUser.SetUserStatus(validUserId, "Unlocked");
                        LoggerManager.RecordUserAction("User", userId, 0, "vlfUser", "UserId=" + validUserId, "Reset Password",
                                          this.Context.Request.UserHostAddress, this.Context.Request.RawUrl,
                                          VLF.DAS.Logic.User.GetMD5HashData(newPassword));
                    }
                    else
                    {
                        return (int)InterfaceError.PasswordDuplicatedInLastEight;
                    }
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
                    if (!(new Regex("[A-Z]").Matches(newPassword).Count >= 1 && new Regex("[a-z]").Matches(newPassword).Count >= 1 && new Regex("[0-9]").Matches(newPassword).Count >= 1))
                    {
                        return Convert.ToInt32(InterfaceError.PasswordNotInRules);
                    }

                    if (ValidatePasswordInLastEight(newPassword, userIdUpdated) < 1)
                    {
                        dbUser.SetHashPasswordByUserId(newPassword, userIdUpdated);
                        dbUser.SetUserStatus(userIdUpdated, "Locked");
                        LoggerManager.RecordUserAction("User", userId, 0, "vlfUser", "UserId=" + userIdUpdated, "Reset Password",
                                         this.Context.Request.UserHostAddress, this.Context.Request.RawUrl,
                                         VLF.DAS.Logic.User.GetMD5HashData(newPassword));
                    }
                    else
                    {
                        return (int)InterfaceError.PasswordDuplicatedInLastEight;
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< ResetPasswordBySuperUser : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Validate locked user")]
        public int ValidateLockedUser(int userId)
        {
            try
            {
                int result = 0;
                using (VLF.DAS.Logic.User dbUser = new User(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    result = dbUser.ValidateLockedUser(userId);
                }
                return result;
            }
            catch (Exception Ex)
            {
                LogException("<< ResetPasswordBySuperUser : uid={0}, EXC={1}", userId, Ex.Message);
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

                    LoggerManager.RecordInitialValues("User", userId, 0, "vlfUser", "UserId=" + userId, "Delete User",
                              this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, string.Format("Delete user ({0})", userId));

                    dbUser.DeleteUserByUserId(userId);

                    LoggerManager.RecordUserAction("User", userId, 0, "vlfUser", "UserId=" + userId, "Delete User",
                               this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, string.Format("Delete user ({0})", userId));
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

        [WebMethod(Description = "Update a user info.")]
        public int UpdateUserInfoStatus(int currUserId, string SID, int userId, string userName, string firstName, string lastName, string expiredDate, string status)
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
                    dbUser.UpdateInfo(userId, userName, firstName, lastName, Convert.ToDateTime(expiredDate), status);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateInfo : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update a user info.")]
        public int UpdateUserInfoStatusAndGroups(int currUserId, string SID, int userId, string userName, string firstName, string lastName, string expiredDate,
            string status, string UsergroupsParams)
        {
            try
            {
                Log(">> UpdateInfo(currUserId={0},userId={1}, userName={2}, firstName={3}, lastName={4}, expiredDate={5})",
                      currUserId, userId, userName, firstName, lastName, expiredDate);

                //if (!ValidateSuperUser(currUserId, SID))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 20);

                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dbUser.UpdateInfo(userId, userName, firstName, lastName, Convert.ToDateTime(expiredDate), status);
                }

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    int rowsAffected = dbUserGroup.UpdateUserGroupAssignmnet(userId, UsergroupsParams); //assign to multiple User Groups
                    LoggerManager.RecordUserAction("User", userId, 0, "vlfUserGroupAssignment",
                                                    string.Format("UserId={0} AND UserGroupId IN ({1})", userId, UsergroupsParams.Replace(";", ",")),
                                                    "Update",
                                                    this.Context.Request.UserHostAddress,
                                                    this.Context.Request.RawUrl,
                                                    "Update user group assignment");
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

        [WebMethod(Description = "Record user action, it will be triggered after create, delete or update event")]
        public int RecordUserAction(string moduleName, int userId, int organizationId, string tableName, string primaryWhere, string action, string remoteAddr, string applicationName, string description)
        {
            string sql = "sp_logging"; //sp name to execute

            try
            {
                LogEvent logEvent = new LogEvent(LoginManager.GetConnnectionString(userId));
                bool result = logEvent.SaveToLog(moduleName, userId, organizationId, tableName, primaryWhere, action, remoteAddr,
                                    applicationName, sql, description);
                logEvent.Dispose();
                return result ? 1 : 0;
            }
            catch (Exception Ex)
            {
                LogException("<< Record user action: uId={0}, action={2}, moduleName={3}, EXC={1}", userId, Ex.Message, action, moduleName);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Record initial values, it will be triggered before create, delete or update event")]
        public int RecordInitialValues(string moduleName, int userId, int organizationId, string tableName, string primaryWhere, string action, string remoteAddr, string applicationName, string description)
        {
            string sql = "sp_LoggingInitialValues"; //sp name to execute

            try
            {
                LogEvent logEvent = new LogEvent(LoginManager.GetConnnectionString(userId));
                bool result = logEvent.SaveToLog(moduleName, userId, organizationId, tableName, primaryWhere, action, remoteAddr,
                                   applicationName, sql, description);
                logEvent.Dispose();
                return result ? 1 : 0;
            }
            catch (Exception Ex)
            {
                LogException("<< Record initial values: uId={0}, action={2}, moduleName={3}, EXC={1}", userId, Ex.Message, action, moduleName);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Validate user password to check if it exists in the last 8 passwords, if it does, then it will return false")]
        public int ValidatePasswordInLastEight(string newPassword, int userId)
        {
            try
            {
                LogEvent logEvent = new LogEvent(LoginManager.GetConnnectionString(userId));
                bool result = logEvent.ValidatePasswordInLastEight(VLF.DAS.Logic.User.GetMD5HashData(newPassword), userId);
                logEvent.Dispose();
                return result ? 1 : 0;
            }
            catch (Exception Ex)
            {
                LogException("<< Validate user password action : uId={0}, reason={1}, action=validateuserpassword", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get the date-time format to populate dropdown on user preference interface")]
        public int GetDateTimeFormats(int userId, string SID, ref DataSet dsInfo)
        {
            try
            {

                dsInfo = null;
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetDateTimeFormat();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDateTimeFormats : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

       [WebMethod(Description = "Get the default date-time format")]
       public int GetDefaultDateTimeFormats(int userId, int organizationid, string SID, ref DataSet dsInfo)
       {
           try
           {

               dsInfo = null;
               using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
               {
                   dsInfo = dbUser.GetDefaultDateTimeFormat(organizationid);
               }
               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               LogException("<< GetDefaultDateTimeFormats : uId={0},orgId={1}, EXC={2}", userId, organizationid, Ex.Message);
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
                {
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                }

                if (!(new Regex("[A-Z]").Matches(password).Count >= 1 && new Regex("[a-z]").Matches(password).Count >= 1 && new Regex("[0-9]").Matches(password).Count >= 1))
                {
                    return Convert.ToInt32(InterfaceError.PasswordNotInRules);
                }

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                // create new user and person
                int newUserId = CreateDBUser(userId, organizationId, userName, personId, password, firstName, lastName, expiredDate);
                LoggerManager.RecordUserAction("User", userId, organizationId, "vlfUser", "UserId=" + newUserId, "Create User",
                                  this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, VLF.DAS.Logic.User.GetMD5HashData(password));
                // assign user to group
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    dbUserGroup.AssignUserToGroup(newUserId, userGroupId);
                    LoggerManager.RecordUserAction("User", userId, organizationId, "vlfUserGroupAssignment", string.Format("UserId={0} AND UserGroupId={1}", newUserId, userGroupId), "Assign User to group",
                                  this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, "Assign a new user to group");
                }

                // assign user to default fleet
                if (userGroupId == 1 || userGroupId == 2)
                {
                    using (Fleet fleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                    {
                        int fleetId = fleet.GetFleetIdByFleetName(organizationId, "All Vehicles");
                        // assign user to the default fleet
                        if (fleetId > 0 && newUserId > 0)
                        {
                            fleet.AddUserToFleet(fleetId, newUserId);
                            LoggerManager.RecordUserAction("User", userId, organizationId, "vlfFleetUsers", string.Format("FleetId={0} AND UserId={1}", fleetId, newUserId), "Assign User to fleet",
                                   this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, "Assign a new user to a fleet");
                        }
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

        [WebMethod(Description = "Add a new user to group, assign a default fleet.")]
        public int AddUserToGroups(int userId, string SID, int organizationId, string userName, string personId,
           string password, string firstName, string lastName, string expiredDate, string UsergroupsParams)
        {
            try
            {
                Log(">> AddUserToGroup(userId = {0},userName = {1}, personId = {2}, firstName = {3}, lastName = {4}, expiredDate = {5})",
                       userId, userName, personId, firstName, lastName, expiredDate);

                //Authorization
                //if (!ValidateSuperUser(userId, SID))
                //{
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //}

                if (!(new Regex("[A-Z]").Matches(password).Count >= 1 && new Regex("[a-z]").Matches(password).Count >= 1 && new Regex("[0-9]").Matches(password).Count >= 1))
                {
                    return Convert.ToInt32(InterfaceError.PasswordNotInRules);
                }

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                // create new user and person
                int newUserId = CreateDBUser(userId, organizationId, userName, personId, password, firstName, lastName, expiredDate);
                LoggerManager.RecordUserAction("User", userId, organizationId, "vlfUser", "UserId=" + newUserId, "Add",
                                  this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, VLF.DAS.Logic.User.GetMD5HashData(password));
                // assign user to group
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    //dbUserGroup.AssignUserToGroup(newUserId, userGroupId); //assign to only one User Group
                    int rowsAffected = dbUserGroup.UpdateUserGroupAssignmnet(newUserId, UsergroupsParams); //assign to multiple User Groups
                    LoggerManager.RecordUserAction("User", userId, organizationId, "vlfUserGroupAssignment",
                                                    string.Format("UserId={0} AND UserGroupId IN ({1})", newUserId, UsergroupsParams.Replace(";", ",")),
                                                    "Add",
                                                    this.Context.Request.UserHostAddress,
                                                    this.Context.Request.RawUrl,
                                                    "Assign a new user to group");
                }

                if (!String.IsNullOrEmpty(UsergroupsParams))
                    UsergroupsParams = ";" + UsergroupsParams + ";";
                // assign user to default fleet
                if (UsergroupsParams.Contains(";1;") || UsergroupsParams.Contains(";2;"))
                {
                    using (Fleet fleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                    {
                        int fleetId = fleet.GetFleetIdByFleetName(organizationId, "All Vehicles");
                        // assign user to the default fleet
                        if (fleetId > 0 && newUserId > 0)
                        {
                            fleet.AddUserToFleet(fleetId, newUserId);
                            LoggerManager.RecordUserAction("User", userId, organizationId, "vlfFleetUsers",
                                                            string.Format("FleetId={0} AND UserId={1}", fleetId, newUserId),
                                                            "Add",
                                                            this.Context.Request.UserHostAddress,
                                                            this.Context.Request.RawUrl,
                                                            "Assign a new user to a fleet");
                        }
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

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    int rowsAffected = dbUserGroup.AddUserGroupAssignmnet(userId, userGroupId);
                    LoggerManager.RecordUserAction("User", userId, 0, "vlfUserGroupAssignment",
                                                    string.Format("UserId={0} AND UserGroupId={1}", userId.ToString(), userGroupId.ToString()),
                                                    "Add",
                                                    this.Context.Request.UserHostAddress,
                                                    this.Context.Request.RawUrl,
                                                    "Assign a new user to group");
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AssignUserToGroup : uId={0}, EXC={1}", userId.ToString(), Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Remove a user from group.")]
        public int DeleteUserAssignment(int currUserId, string SID, int userId, short userGroupId)
        {
            try
            {
                Log(">> DeleteUserAssignment(currUserId={0},userId={1},userGroupId={2})", currUserId, userId, userGroupId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("User", 0, 0, "vlfUserGroupAssignment",
                                                        string.Format("UserId={0} AND UserGroupId={1}", userId, userGroupId),
                                                        "Update",
                                                        this.Context.Request.UserHostAddress,
                                                        this.Context.Request.RawUrl,
                                                        "Update a user from group - Initial values");

                    dbUserGroup.DeleteUserAssignment(userId, userGroupId);

                    LoggerManager.RecordUserAction("User", userId, 0, "vlfUserGroupAssignment",
                                                    string.Format("UserId={0} AND UserGroupId={1}", userId, userGroupId),
                                                    "Delete",
                                                    this.Context.Request.UserHostAddress,
                                                    this.Context.Request.RawUrl,
                                                    "Delete a user from group");
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

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

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

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);
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

        [WebMethod(Description = "Returns all users groups allowed to view by User. XML File format: [UserGroupId],[UserGroupName],[IsBaseGroup],[SecurityLevelName]")]
        public int GetUserGroupsbyUser(int currUserId, bool AllOrganizationGroups, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupsbyUser(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupsbyUser(currUserId, AllOrganizationGroups);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupsbyUser : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all users groups allowed to update by User. XML File format: [UserGroupId],[UserGroupName],[IsBaseGroup],[SecurityLevelName]")]
        public int GetUserGroupsForUpdateByUser(int currUserId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupsForUpdateByUser(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupsForUpdateByUser(currUserId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupsForUpdateByUser : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all users groups allowed to update by User. XML File format: [ControlId],[ControlName][FormID][FormName][ControlDescription][ControlIsActive]")]
        public int GetControlsForUpdate(int currUserId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetControlsForUpdate(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(currUserId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetControlsForUpdate(currUserId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetControlsForUpdate : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all users groups allowed to update by User. XML File format: [FormID][FormName]")]
        public int GetForms(int currUserId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetForms(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetForms(currUserId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetForms : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns control. XML File format.")]
        public int GetControl(int currUserId, string SID, int ControlId, ref string xml)
        {
            try
            {
                Log(">> GetControl(currUserId={0}, ControlId={1})", currUserId, ControlId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetControl(ControlId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetControl : uId={0}, ControlId={1}, EXC={2}", currUserId, ControlId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns User Group Control settings. XML File format: [ControlId],[ControlName][SelectedControlId][FormID][FormName][ControlDescription]")]
        public int GetUserGroupControlSettings(int currUserId, int UserGroupId, int ParentUserGroupId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupControlSettings(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupControlSettings(currUserId, UserGroupId, ParentUserGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupControlSettings : uId={0}, UserGroupId={1}, EXC={2}", currUserId, UserGroupId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Adds form control.")]
        public int AddControl(int userId, string SID, string ControlName, string Description, int FormID, string ControlURL, bool ControlIsActive, string ControlLangNames, int ParentControlId)
        {
            int ControlId = 0;

            try
            {
                Log(">>AddControl(uId={0}, ControlName={1}, FormID={2})", userId, ControlName, FormID.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    ControlId = dbUserGroup.AddControl(ControlName, Description, FormID, ControlURL, ControlIsActive, ControlLangNames, ParentControlId);
                }

                LoggerManager.RecordUserAction("GuiControl", userId, 0, "vlfGuiControls",
                                                string.Format("ControlId={0}", ControlId),
                                                "Add",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Add form control");

                //return (int)InterfaceError.NoError;
                return ControlId;
            }
            catch (Exception Ex)
            {
                LogException("<< AddControl : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Updates form control.")]
        public int UpdateControl(int userId, string SID, string ControlName, string Description, int FormID, string ControlURL, bool ControlIsActive, string ControlLangNames,
            int ControlId, int ParentControlId)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>UpdateControl(uId={0}, ControlId={1})",
                         userId, ControlId.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                LoggerManager.RecordInitialValues("GuiControl", 0, 0, "vlfGuiControls",
                                                string.Format("ControlId={0}", ControlId),
                                                "Update",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Update form control");

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.UpdateControl(ControlName, Description, FormID, ControlURL, ControlIsActive, ControlLangNames, ControlId, ParentControlId);
                }

                LoggerManager.RecordUserAction("GuiControl", userId, 0, "vlfGuiControls",
                                                string.Format("ControlId={0}", ControlId),
                                                "Update",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Update form control");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateControl : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Deletes User Group.")]
        public int DeleteUserGroup(int userId, string SID, int UserGroupId)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>DeleteUserGroup(uId={0}, UserGroupId={1})", userId, UserGroupId.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                LoggerManager.RecordInitialValues("UserGroup", userId, 0, "vlfUserGroup",
                                                string.Format("UserGroupId={0}", UserGroupId.ToString()),
                                                "Update",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Update User Group");

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.DeleteUserGroup(UserGroupId);
                }

                LoggerManager.RecordUserAction("UserGroup", userId, 0, "vlfUserGroup",
                                                string.Format("UserGroupId={0}", UserGroupId.ToString()),
                                                "Delete",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Delete User Group");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserGroup : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns User Group Control settings. XML File format: [ReportTypesId][ReportName][UserGroupName][SelectedReport]")]
        public int GetUserGroupReportSettings(int currUserId, int UserGroupId, int ParentUserGroupId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupReportSettings(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupReportSettings(currUserId, UserGroupId, ParentUserGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupReportSettings : uId={0}, UserGroupId={1}, EXC={2}", currUserId, UserGroupId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns User Group Control settings. XML File format: [BoxCmdOutTypeId][BoxCmdOutType][UserGroupName][SelectedCommand]")]
        public int GetUserGroupCommandSettings(int currUserId, int UserGroupId, int ParentUserGroupId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupCommandSettings(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupCommandSettings(currUserId, UserGroupId, ParentUserGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupCommandSettings : uId={0}, UserGroupId={1}, EXC={2}", currUserId, UserGroupId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns User Group Control additional settings. XML File format: [ControlId], [ControlDescription]")]
        public int GetUserGroupControlAddSettings(int currUserId, int UserGroupId, int ParentUserGroupId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupControlAddSettings(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupControlAddSettings(currUserId, UserGroupId, ParentUserGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupControlAddSettings : uId={0}, UserGroupId={1}, EXC={2}", currUserId, UserGroupId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns User Group Report additional settings. XML File format: [ReportTypesId], [ReportName]")]
        public int GetUserGroupReportAddSettings(int currUserId, int UserGroupId, int ParentUserGroupId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupReportAddSettings(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupReportAddSettings(currUserId, UserGroupId, ParentUserGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupReportAddSettings : uId={0}, UserGroupId={1}, EXC={2}", currUserId, UserGroupId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns User Group Command additional settings. XML File format: [BoxCmdOutTypeId][BoxCmdOutType]")]
        public int GetUserGroupCommandAddSettings(int currUserId, int UserGroupId, int ParentUserGroupId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupCommandAddSettings(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupCommandAddSettings(currUserId, UserGroupId, ParentUserGroupId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupCommandAddSettings : uId={0}, UserGroupId={1}, EXC={2}", currUserId, UserGroupId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Adds User Group setting.")]
        public int AddUserGroupSetting(int userId, string SID, int UserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>AddUserGroupSetting(uId={0}, UserGroupId={1}, OperationId={2}, OperationType={3})",
                         userId, UserGroupId, OperationId, OperationType);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.AddUserGroupSetting(UserGroupId, OperationId, OperationType);
                }

                LoggerManager.RecordUserAction("GroupSecurity", userId, 0, "vlfGroupSecurity",
                                                string.Format("UserGroupId={0} AND OperationId={1} AND OperationType={2}", UserGroupId, OperationId, OperationType),
                                                "Add",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Add setting to User Group");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserGroupSetting : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Deletes User Group setting.")]
        public int DeleteUserGroupSetting(int userId, string SID, int UserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>DeleteUserGroupSetting(uId={0}, UserGroupId={1}, OperationId={2}, OperationType={3})",
                         userId, UserGroupId, OperationId, OperationType);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                LoggerManager.RecordInitialValues("GroupSecurity", 0, 0, "vlfGroupSecurity",
                                                        string.Format("UserGroupId={0} AND OperationId={1} AND OperationType={2}", UserGroupId, OperationId, OperationType),
                                                        "Update",
                                                        this.Context.Request.UserHostAddress,
                                                        this.Context.Request.RawUrl,
                                                        "Update User Group setting");

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.DeleteUserGroupSetting(UserGroupId, OperationId, OperationType);
                }

                LoggerManager.RecordUserAction("GroupSecurity", userId, 0, "vlfGroupSecurity",
                                                string.Format("UserGroupId={0} AND OperationId={1} AND OperationType={2}", UserGroupId, OperationId, OperationType),
                                                "Delete",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Delete setting from User Group");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserGroupSetting : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Updates User Group settings.")]
        public int UpdateUserGroupSettings(int userId, string SID, int UserGroupId, string CheckboxValuesParams, string CheckboxReportsValuesParams,
           string CheckboxCommandsValuesParams, string FleetIDs, int OperationType, string UserGroupName)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>UpdateUserGroupSettings(uId={0}, UserGroupId={1}, CheckboxValuesParams={2}, OperationType={3})",
                         userId, UserGroupId, CheckboxValuesParams, OperationType);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                LoggerManager.RecordInitialValues("UserGroup", userId, 0, "vlfUserGroup",
                                                string.Format("UserGroupId={0}", UserGroupId),
                                                "Update",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Update User Group");

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.UpdateUserGroupSettings(UserGroupId, CheckboxValuesParams, CheckboxReportsValuesParams, CheckboxCommandsValuesParams, FleetIDs, OperationType, userId, UserGroupName);
                }

                LoggerManager.RecordUserAction("UserGroup", userId, 0, "vlfUserGroup",
                                                string.Format("UserGroupId={0}", UserGroupId),
                                                "Update",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Update User Group");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateUserGroupSettings : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add User Group settings.")]
        public int AddUserGroupSettings(int userId, string SID, string CheckboxValuesParams, string CheckboxReportsValuesParams,
            string CheckboxCommandsValuesParams, string FleetIDs, int OperationType, string UserGroupName, int OrganizationId, int ParentUserGroupId)
        {
            int UserGroupId = 0;

            try
            {
                Log(">>AddUserGroupSettings(uId={0}, UserGroupName={1}, CheckboxValuesParams={2}, OperationType={3})",
                         userId, UserGroupName, CheckboxValuesParams, OperationType);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    UserGroupId = dbUserGroup.AddUserGroupSettings(CheckboxValuesParams, CheckboxReportsValuesParams, CheckboxCommandsValuesParams, FleetIDs, OperationType, UserGroupName, OrganizationId, ParentUserGroupId, userId);
                }

                LoggerManager.RecordUserAction("UserGroup", userId, 0, "vlfUserGroup",
                                                string.Format("UserGroupId={0}", UserGroupId),
                                                "Add",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Add User Group");

                //return (int)InterfaceError.NoError;
                return UserGroupId;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserGroupSettings : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns Fleets by UserGroup. XML File format: [FleetId][NodeCode]")]
        public int GetFleetsByUserGroup(int currUserId, string SID, int UserGroupId, string FleetType, ref string xml)
        {
            try
            {
                Log(">> GetFleetsByUserGroup(currUserId={0}, UserGroupId={1})", currUserId, UserGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetFleetsByUserGroup(UserGroupId, FleetType);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsByUserGroup : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns Operation Types. XML File format: [OperationType][OperationTypeName]")]
        public int GetOperationTypes(int currUserId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetOperationTypes(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetOperationTypes(currUserId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOperationTypes : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns Operation Controls. XML File format: [ControlID][ControlName]")]
        public int GetOperationControls(int currUserId, string SID, int OperationType, ref string xml)
        {
            try
            {
                Log(">> GetOperationControls(currUserId={0}, OperationType={1})", currUserId, OperationType);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetOperationControls(currUserId, OperationType);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOperationControls : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns Operation Controls. XML File format: [OrganizationId][OrganizationName]")]
        public int GetOrganizationsWithUserGroups(int currUserId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationsWithUserGroups(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetOrganizationsWithUserGroups();
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationsWithUserGroups : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns Operation Controls. XML File format: [UserGroupId][UserGroupName]")]
        public int GetUserGroupsOperationAccess(int currUserId, string SID, int OperationId, int OperationType, int OrganizationId, bool OperationAccess, ref string xml)
        {
            try
            {
                Log(">> GetUserGroupsOperationAccess(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetUserGroupsOperationAccess(OperationId, OperationType, OrganizationId, OperationAccess);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupsOperationAccess : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns UserGroupId.")]
        public int GetUserGroupIdByName(int currUserId, string SID, string userGroupName)
        {
            int UserGroupId = 0;

            try
            {
                Log(">> GetUserGroupIdByName(currUserId={0})", currUserId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    UserGroupId = dbUserGroup.GetUserGroupIdByName(userGroupName);
                }

                return UserGroupId;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserGroupIdByName : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns Controls. XML File format: [ControlId]")]
        public int GetChildControls(int currUserId, string SID, int ParentControlId, ref string xml)
        {
            try
            {
                Log(">> GetChildControls(currUserId={0}, ParentControlId={1})", currUserId.ToString(), ParentControlId.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);

                DataSet dsInfo = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsInfo = dbUserGroup.GetChildControls(ParentControlId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetChildControls : uId={0}, ParentControlId={1}, EXC={2}", currUserId.ToString(), ParentControlId.ToString(), Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add a setting to all User Groups.")]
        public int AddUserGroupSettingsAll(int userId, string SID, int ParentUserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>AddUserGroupSettingsAll(uId={0}, ParentUserGroupId={1}, OperationId={2}, OperationType={3})",
                         userId.ToString(), ParentUserGroupId.ToString(), OperationId.ToString(), OperationType.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.AddUserGroupSettingsAll(ParentUserGroupId, OperationId, OperationType);
                }

                LoggerManager.RecordUserAction("GroupSecurity", userId, 0, "vlfGroupSecurity",
                                                string.Format("OperationId={0} AND OperationType={1}", OperationId.ToString(), OperationType.ToString()),
                                                "Add",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Add setting to all User Groups");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserGroupSettingsAll : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete a setting from all User Groups.")]
        public int DeleteUserGroupSettingsAll(int userId, string SID, int ParentUserGroupId, int OperationId, int OperationType)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>DeleteUserGroupSettingsAll(uId={0}, ParentUserGroupId={1}, OperationId={2}, OperationType={3})",
                         userId.ToString(), ParentUserGroupId.ToString(), OperationId.ToString(), OperationType.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbUserGroup.DeleteUserGroupSettingsAll(ParentUserGroupId, OperationId, OperationType);
                }

                LoggerManager.RecordUserAction("GroupSecurity", userId, 0, "vlfGroupSecurity",
                                                string.Format("OperationId={0} AND OperationType={1}", OperationId.ToString(), OperationType.ToString()),
                                                "Delete",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Delete setting from all User Groups");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserGroupSettingsAll : uId={0}, EXC={1}", userId, Ex.Message);
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


        // Changes for TimeZone Feature start
        [WebMethod(Description = "Get Notifications For User")]
        public int GetAllNotificationsForUserId_NewTZ(int userId, string SID, string fromDateTime, string toDateTime, ref string xml)
        {
            try
            {
                Log(">> GetAllNotificationsForUserId(uId={0},  fromDate={1}, toDate={2})", userId, fromDateTime, toDateTime);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Notification dbNotification = new Notification(LoginManager.GetConnnectionString(userId), false))
                {
                    dsInfo = dbNotification.GetAllNotificationsForUserId_NewTZ(userId, Convert.ToDateTime(fromDateTime), Convert.ToDateTime(toDateTime), 0);
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
        // Changes for TimeZone Feature end

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
                userInfo.userStatus = "Locked";

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

        #region User general

        [WebMethod(Description = "Returns Operation Controls. XML File format: [LanguageID][CultureName][Language]")]
        public int GetLanguages(int currUserId, string SID, ref string xml)
        {
            DataSet dsLanguages = null;

            try
            {
                Log(">> GetLanguages(uId={0})", currUserId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(currUserId, SID);


                using (User dbUser = new User(LoginManager.GetConnnectionString(currUserId)))
                {
                    dsLanguages = dbUser.GetLanguages(currUserId);
                }

                if (Util.IsDataSetValid(dsLanguages))
                    xml = dsLanguages.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLanguages : uId={0}, EXC={1}", currUserId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        [WebMethod(Description = "Retrieves all EmailId of all users.")]
        public int GetAllEmailXML(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllEmailXML(uId={0})", userId);

                //Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbUser.GetAllEmail(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllEmailXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update user email.")]
        public int AddEmail(int userId, string SID,string email)
        {
            try
            {
                Log(">>AddEmail(uId={0}, Email={1})",
                         userId, email);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                     dbUser.AddEmail(userId, email);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddEmail : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

    }
}
