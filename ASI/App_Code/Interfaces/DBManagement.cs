using System;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;

using VLF.CLS;
using VLF.CLS.Def;
using VLF.CLS.Def.Structures;
using VLF.DAS.Logic;
using VLF.ERRSecurity;

/// <summary>
/// Summary description for DBManagement
/// </summary>
/// 
namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "hhttp://www.sentinelfm.com")]

    public class DBManagement : System.Web.Services.WebService
    {

        public DBManagement()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }


        #region Local Functions

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
        /// 
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

        #endregion


        #region Validation

        private bool ValidateUserOrganization(int userId, string SID, int organizationId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                //return dbUser.ValidateUserOrganization(userId, organizationId);
                return dbUser.ValidateUserAccess(userId, Enums.ValidationItem.Validate_Organization, organizationId);
            }
        }

        private bool ValidateUserFleet(int userId, string SID, int fleetId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                //return dbUser.ValidateUserFleet(userId, fleetId);
                return dbUser.ValidateUserAccess(userId, Enums.ValidationItem.Validate_Fleet, fleetId);
            }

        }

        private bool ValidateUserBox(int userId, string SID, int boxId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                //return dbUser.ValidateUserBox(userId, boxId);
                return dbUser.ValidateUserAccess(userId, Enums.ValidationItem.Validate_Box, boxId);
            }
        }

        private bool ValidateUserVehicle(int userId, string SID, Int64 vehicleId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                //return dbUser.ValidateUserVehicle(userId, vehicleId);
                return dbUser.ValidateUserAccess(userId, Enums.ValidationItem.Validate_Vehicle, vehicleId);
            }
        }
        private bool ValidateUserDriver(int userId, string SID, int driverId)
        {
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                //return dbUser.ValidateUserDriver(userId, driverId);
                return dbUser.ValidateUserAccess(userId, Enums.ValidationItem.Validate_Driver, driverId);
            }
        }

        private bool ValidateSuperUser(int userId, string SID)
        {
            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateSuperUser(userId);
            }
        }


        #endregion

        #region General

        [WebMethod]
        public int GetAllStateProvinces(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">>GetAllStateProvinces( uid={0} )", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbSystem.GetAllStateProvinces();
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllStateProvinces : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the List of all Super Organizations in XML format<br />Return 0 if success, Error code if not")]
        public int GetAllSuperOrganizations(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllSuperOrganizations(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsResult = null;


                using (VLF.DAS.Logic.Organization dbOrganization = new Organization(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbOrganization.GetSuperOrganizations();
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllSuperOrganizations : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the features for a firmware, XML Format: [FwChId], [BoxHwTypeId], [BoxHwTypeName], [MaxSensorsNum], [MaxOutputsNum], [BoxProtocolTypeId], [BoxProtocolTypeName], [CommModeId], [CommModeName], [ChPriority], [FwTypeId], [FwLocalPath], [FwOAPPath], [FwDateReleased], [MaxGeozones], [OAPPort], [FwAttributes1]<br />Return 0 if success, Error code if not")]
        public int GetAllChannelByFwId(int userId, string SID, short firmwareId, ref string xml)
        {
            try
            {
                Log(">> GetAllChannelByFwId(userId={0}", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                //Authorization
                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllChannelsByFwId(firmwareId);
                }
                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllChannelByFwId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the features for a firmware, XML Format: [FwChId], [BoxHwTypeId], [BoxHwTypeName], [MaxSensorsNum], [MaxOutputsNum], [BoxProtocolTypeId], [BoxProtocolTypeName], [CommModeId], [CommModeName], [ChPriority], [FwTypeId], [FwLocalPath], [FwOAPPath], [FwDateReleased], [MaxGeozones], [OAPPort], [FwAttributes1]<br />Return 0 if success, Error code if not")]
        public int GetFirmwareInfoFeatures(int userId, string SID, short firmwareId, ref string xml)
        {
            try
            {
                Log(">> GetFirmwareInfoFeatures(userId={0}", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                //Authorization
                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetFirmwareInfoFeatures(firmwareId);
                }
                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFirmwareInfoFeatures : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves the firmwares for a specific type id, XML Format: [FwId], [BoxHwTypeId], [FwName], [FwTypeId], [FwLocalPath], [FwOAPPath], [FwDateReleased], [MaxGeozones], [BoxHwTypeName], [OAPPort]<br />Return 0 if success, Error code if not")]
        public int GetAllFirmwareByType(int userId, string SID, short firmwareTypeId, ref string xml)
        {
            try
            {
                Log(">> GetAllFirmwareByType(userId={0}", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                //Authorization
                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllFirmwareInfo(firmwareTypeId);
                }
                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllFirmwareByType : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the List of all Map Engines in XML format<br />Return 0 if success, Error code if not")]
        public int GetAllMapEnginesInfo(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetSuperOrganizationsXML(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsResult = null;


                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllMapEnginesInfo();
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllMapEnginesInfoXML : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves the List of all Geocode Engines in XML format<br />Return 0 if success, Error code if not")]
        public int GetAllGeocodeEnginesInfo(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllGeocodeEnginesInfoXML(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsResult = null;


                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllGeoCodeEnginesInfo();
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllGeoCodeEnginesInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /*
        [WebMethod(Description = "Return MD5 code for the Data")]
        public int GetMD5HashData(int userId, string SID, string data, ref string md5)
        {
            string result = "";
            try
            {
                Log(">> GetMD5HashData(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                result = VLF.DAS.Logic.User.GetMD5HashData(data);

                if (!String.IsNullOrEmpty(result))
                    md5 = result;

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMD5HashData : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }*/

        [WebMethod(Description = "Retrieves the config Info for a specific fwCh id, XML Format: ")]
        public int GetConfigInfoByFwChId(int userId, string SID, short fwChId, ref string xml)
        {
            try
            {
                Log(">> GetConfigInfoByFwChId(userId={0}, fwChId={1}", userId, fwChId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetConfigInfo(fwChId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetConfigInfoByFwChId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves the firmware types, Xml Format: [FwTypeId], [FwTypeName]<br />Return 0 if success, Error code if not")]
        public int GetAllFirmwareType(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllFirmwareType(userId={0}", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                //Authorization
                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.FirmwareTypes();
                }
                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllFirmwareType : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Return default communication information by  Fw Chnl Id. XML format [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName]")]
        public int GetDefaultCommInfoByFwChnl(int userId, string SID, short fwChnlId, ref string xml)
        {
            try
            {
                Log(">>GetDefaultCommInfoByFwChnl(uid={0}, fwChnlId={1})", userId, fwChnlId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsDefCommInfo = null;
                using (SystemConfig dbSystem = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsDefCommInfo = dbSystem.GetDefaultCommInfo(fwChnlId);
                }


                if (Util.IsDataSetValid(dsDefCommInfo))
                    xml = dsDefCommInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDefaultCommInfoByFwChnl : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int GetAllVehicleMakesInfo(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">>GetAllVehicleMakesInfo( uid={0} )", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbSystem.GetAllMakesInfo();
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllVehicleMakesInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int GetAllVehicleTypes(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllVehicleTypes( uid={0} )", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbSystem.GetAllVehicleTypes();
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;

            }
            catch (Exception Ex)
            {
                LogException("<< GetAllVehicleTypes : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int GetModelsInfoByMakeId(int userId, string SID, int makeId, ref string xml)
        {
            try
            {
                Log(">>GetModelsInfoByMakeId( uid={0}, makeId = {1} )", userId, makeId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbSystem.GetModelsInfoByMakeId(makeId);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetModelsInfoByMakeId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int GetIconsInfo(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">>GetIconsInfo( uid={0} )", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbSystem.GetIconsInfo();
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetIconsInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion  //General


        #region Organization

        [WebMethod(Description = "Add new Organizaton, the new organization Id is return by references <br />Return 0 if success, Error code if not")]
        public int AddNewOrganization(int userId, string SID, int superOrganizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId, ref int newOrgId)
        {
            try
            {
                newOrgId = -1;
                Log(">> AddNewOrganization(userId={0},superOrganizationId = {1}, organizationName = {2}, contact = {3}, address = {4}, description = {5}, logoName = {6}, homePageName = {7}, mapGroupId = {8}, geoCodeGroupId = {9})",
                               userId, superOrganizationId, organizationName, contact, address, description, logoName, homePageName, mapGroupId, geoCodeGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                //Authorization
                using (VLF.DAS.Logic.Organization dbOrg = new Organization(Application["ConnectionString"].ToString()))
                {
                    newOrgId = dbOrg.AddOrganization(superOrganizationId, organizationName, contact, address, description,
                       logoName, homePageName, mapGroupId, geoCodeGroupId);
                    Log(">> the Organization {1} is created!", newOrgId);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddNewOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update an existing Organizaton<br />Return 0 if success, Error code if not")]
        public int UpdateOrganization(int userId, string SID, int organizationId, int superOrganizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            try
            {
                Log(">> UpdateOrganization(userId={0},organizationId = {1}, superOrganizationId = {2}, organizationName = {3}, contact = {4}, address = {5}, description = {6}, logoName = {7}, homePageName = {8}, mapGroupId = {9}, geoCodeGroupId = {10})",
                               userId, organizationId, superOrganizationId, organizationName, contact, address, description, logoName, homePageName, mapGroupId, geoCodeGroupId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                //Authorization
                using (VLF.DAS.Logic.Organization dbOrg = new Organization(Application["ConnectionString"].ToString()))
                {
                    dbOrg.UpdateInfo(superOrganizationId, organizationId, organizationName, contact, address, description,
                       logoName, homePageName, mapGroupId, geoCodeGroupId);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Create New or Update Existing Preferences for an Organizaton<br />Return 0 if success, Error code if not")]
        public int SaveOrganizationPreferences(int userId, string SID, int organizationId, string notificationEmail, int radiusForGps, int maximumReportingInterval, int historyTimerange, int waitingPeriodToGetMessages, int timezone)
        {
            try
            {
                Log(">> SaveOrganizationPreferences(userId={0},organizationId = {1}, notificationEmail = {2}, radiusForGps = {3}, maximumReportingInterval = {4}, historyTimerange = {5}, waitingPeriodToGetMessages = {6}, timezone = {7})",
                               userId, organizationId, notificationEmail, radiusForGps, maximumReportingInterval, historyTimerange, waitingPeriodToGetMessages, timezone);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                //Authorization
                using (VLF.DAS.Logic.Organization dbOrg = new Organization(Application["ConnectionString"].ToString()))
                {

                    //if preferences exists, update them if not create new preferecnes for the organization
                    DataSet dsResult = dbOrg.GetOrganizationPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dbOrg.UpdatePreference(organizationId, notificationEmail, radiusForGps, maximumReportingInterval, historyTimerange, waitingPeriodToGetMessages, timezone);
                    }
                    else //not exists, create it
                    {
                        dbOrg.AddPreference(organizationId, notificationEmail, radiusForGps, maximumReportingInterval, historyTimerange, waitingPeriodToGetMessages, timezone);
                    }
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SaveOrganizationPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update a geozone.")]
        public int UpdateGeozone(int userId, string SID, int organizationId, short geozoneId,
                                string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                                short severityId, string description, string email, string phone, int timeZone,
                                bool dayLightSaving, short formatType, bool notify, bool warning,
                                bool critical, bool autoAdjustDayLightSaving)
        {
            try
            {
                Log(">> UpdateGeozone(uId={0}, orgId={1}, geozoneId={2}, email='{3}', timeZone={4}, dayLightSaving={5}, formatType={6}, notify={7}, warning={8}, critical={9}, geozoneType={10}, autoAdjustDayLightSaving={11}, xmlGeozoneSet='{12}')",
                   userId, organizationId, geozoneId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, geozoneType, autoAdjustDayLightSaving, xmlGeozoneSet);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsGeozoneSet = new DataSet();
                if (xmlGeozoneSet != null && xmlGeozoneSet.TrimEnd() != "")
                {
                    System.IO.StringReader strrXML = new System.IO.StringReader(xmlGeozoneSet.TrimEnd());
                    dsGeozoneSet.ReadXml(strrXML);
                }

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateGeozone(organizationId, geozoneId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, true, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateGeozone :uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update landmark.")]
        public int UpdateLandmark(int userId, string SID, int organizationId, string currLandmarkName,
                                  string newLandmarkName, double newLatitude, double newLongitude,
                                  string newDescription, string newContactPersonName, string newContactPhoneNum,
                                  int radius, string email, string phoneSMS, short timeZone, bool dayLightSaving,
                                  bool autoAdjustDayLightSaving, string streetAddress)
        {
            try
            {
                Log(">> UpdateLandmark(uId={0}, orgId={1}, currLandmarkName={2}, newLandmarkName={3}, newLatitude={4}, newLongitude={5}, email='{6}', timeZone={7}, dayLightSaving={8}, autoAdjustDayLightSaving = {9}, streetAddress = '{10}' )",
                         userId, organizationId, currLandmarkName, newLandmarkName, newLatitude, newLongitude, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                if (newLandmarkName == "")
                    newLandmarkName = VLF.CLS.Def.Const.unassignedStrValue;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateLandmark(organizationId, currLandmarkName, newLandmarkName,
                                         newLatitude, newLongitude, newDescription,
                                         newContactPersonName, newContactPhoneNum, radius,
                                         email, phoneSMS, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress,true);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateLandmark : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves landmarks info by organization id .XML File Format : [OrganizationId],[LandmarkName],[Latitude],[Longitude],[Description],[ContactPersonName],[ContactPhoneNum],[Radius],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],[StreetAddress]")]
        public int GetOrganizationLandmarks(int userId, string SID,
                                         int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationLandmarks (uId={0}, orgId={1} )", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetLandMarksInfoByOrganizationId(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationLandmarks : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves all geozones info by organization id. XML File format:  [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted], [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving] ")]
        public int GetOrganizationGeozones(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozones(uId={0}, orgId={1})", userId, organizationId);

                // Authentication
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorization
                //using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //   if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //      return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //}

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozones(organizationId);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = XmlUtil.GetXmlIncludingNull(dsInfo);
                    //xml = dsInfo.GetXml();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozones : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves a geozone info by organization id and geozone id . XML File format:  [GeozoneNo],[GeozoneId],[Type],[GeozoneType],[SequenceNum],[Latitude],[Longitude] ")]
        public int GetOrganizationGeozoneInfo(int userId, string SID, int organizationId,
                                              short geozoneId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozoneInfo(uId={0}, orgId={1}, geozoneId={2})", userId, organizationId, geozoneId);

                DataSet dsInfo = null;

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozoneInfo(organizationId, geozoneId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozoneInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Organization Preferences")]
        public int GetOrganizationPreferences(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationPreferences(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves Organization info by Organization ID. XML File format: [OrganizationId],[OrganizationName],[Contact],[Address],[Description]")]
        public int GetOrganizationInfoById(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">>GetOrganizationInfoById(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationInfoByOrganizationId(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationInfoById : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Change the organization Id for a Box (Move box to a new organization).")]
        public int ChangeBoxOrganization(int userId, string SID, int boxId, int organizationId)
        {
            try
            {
                Log(">> ChangeBoxOrganization(userId={0}, boxId={1}, orgId={2}", userId, boxId, organizationId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (Box dbb = new Box(Application["ConnectionString"].ToString()))
                {
                    dbb.ChangeOrganization(userId, boxId, organizationId);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< ChangeBoxOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retreive info for all accessibles organizations by a specific user")]
        public int GetAccessibleOrganizationsForCurrentUser(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAccessibleOrganizationsForCurrentUser(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationsInfoListByUser(userId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAccessibleOrganizationsForCurrentUser : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retreive info for all accessibles organizations by a specific user")]
        public int GetAccessibleOrganizationsByUser(int userId, string SID, int usrId, ref string xml)
        {
            try
            {
                Log(">> GetAccessibleOrganizationsForCurrentUser(uId={0},id={1})", userId, usrId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationsInfoListByUser(usrId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAccessibleOrganizationsForCurrentUser : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #endregion


        #region Vehicle

        [WebMethod(Description = "Returns XML of vehicles for a particular Fleet by Fleet ID. XML file format:   [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description] ")]
        public int GetVehiclesInfoByFleet(int userId, string SID, int fleetId, ref string xml)
        {
            // string xml = "";
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesInfoByFleet ( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                Log("<< GetVehiclesInfoByFleet(uId={0}, fleetId={1}, tSpan={2}", userId, fleetId,
                   DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesInfoByFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = @"Returns XML of vehicles for a particular fleet. 
                           XML file format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],
                                            [VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]")]
        public int GetVehiclesInfoByFleetAndFeatures(int userId, string SID, int fleetId,
                                                       long featureMask, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehiclesInfoByFleetAndFeatures( uId={0}, fleetId={1}, mask={2} )",
                                                     userId, fleetId, featureMask);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesInfoByFleetId(fleetId, featureMask);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                Log("<< GetVehiclesInfoByFleetAndFeatures( uId={0}, fleetId={1}, mask={2}, tSpan={3} )",
                                    userId, fleetId, featureMask, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesInfoByFleetAndFeatures : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Return communication information by Box ID and Fw Chnl Id. XML format [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName]")]
        public int GetCommInfoByBoxAndFwChnl(int userId, string SID, int boxId, short fwChnlId, ref string xml)
        {
            try
            {
                Log(">>GetCommInfoByBoxAndFwChnl(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsDefCommInfo = null;
                using (SystemConfig dbSystem = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsDefCommInfo = dbSystem.GetDefaultCommInfo(fwChnlId);
                }

                DataSet dsBoxCommInfo = null;
                using (Box dbBox = new Box(Application["ConnectionString"].ToString()))
                {
                    dsBoxCommInfo = dbBox.GetCommInfoByBoxId(boxId);
                }

                if (Util.IsDataSetValid(dsBoxCommInfo) && Util.IsDataSetValid(dsDefCommInfo))
                {
                    foreach (DataRow defRow in dsDefCommInfo.Tables[0].Rows)
                    {
                        foreach (DataRow boxRow in dsBoxCommInfo.Tables[0].Rows)
                        {
                            if (Convert.ToInt32(defRow["CommAddressTypeId"]) == Convert.ToInt32(boxRow["CommAddressTypeId"]))
                            {
                                defRow["CommAddressValue"] = boxRow["CommAddressValue"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                xml = dsDefCommInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCommInfoByBoxAndFwChnl : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete Box Assignement from a Vehicle")]
        public int DeleteActiveVehicleAssignment(int userId, string SID, int boxId, string description)
        {
            try
            {
                int retVal = (int)InterfaceError.NoError;
                Log(">> DeleteActiveVehicleAssignment(uid={0}, boxId={1})", userId, boxId);

                //if (!ValidateUserBox(userId, SID, boxId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(Application["ConnectionString"].ToString()))
                {

                    if (dbVehicle.DeleteActiveVehicleAssignment(userId, boxId, description) == 0)
                        retVal = 1;// (int)InterfaceError.ZeroRowAffected;
                }

                return retVal;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteActiveVehicleAssignment : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle outputs information by Box ID. XML File format: [OutputId][OutputName][OutputAction]")]
        public int GetVehicleOutputsByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleOutputsByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                string licensePlate = VLF.CLS.Def.Const.unassignedStrValue;
                DataSet dsOutputs = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    licensePlate = dbVehicle.GetLicensePlateByBox(boxId);
                    dsOutputs = dbVehicle.GetVehicleOutputsInfo(licensePlate, userId);
                }

                if (Util.IsDataSetValid(dsOutputs))
                    xml = dsOutputs.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleOutputsByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get All Unassigned Vehicles for an organizaton.XML File format: [VehicleId], [VinNum], [MakeModelId], [MakeName], [ModelName], [VehicleTypeName], [StateProvince], [ModelYear], [Color], [Description], [CostPerMile]")]
        public int GetAllUnassignedVehiclesByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">>GetAllUnassignedVehiclesByOrganization(uid={0}, orgId={1})", userId, organizationId);

                DataSet dsVehicles = null;
                using (Vehicle dbVehicle = new Vehicle(Application["ConnectionString"].ToString()))
                {
                    dsVehicles = dbVehicle.GetAllUnassignedVehiclesInfo(organizationId);
                }

                if (Util.IsDataSetValid(dsVehicles))
                {
                    xml = dsVehicles.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedVehiclesByOrganization : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves vehicle information by Vehicle ID. XML File format:[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]")]
        public int GetVehicleInfoById(int userId, string SID, Int64 vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">>GetVehicleInfoById(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleInfoByVehicleId(vehicleId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xmlResult = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoById : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get vehicle assignment history")]
        public int GetVehicleAssignmentHistoryByDates(int userId, string SID, long vehicleId,
                                                      DateTime from, DateTime to, ref string xmlResult)
        {
            try
            {
                Log(">> GetVehicleAssignmentHistoryByDates(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})",
                                  userId, vehicleId, from, to);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsVehicle = new DataSet();

                    dsVehicle = driver.GetVehicleAssignmentHistory(vehicleId, from, to);

                    if (Util.IsDataSetValid(dsVehicle))
                    {
                        xmlResult = dsVehicle.GetXml();
                    }
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetVehicleAssignmentHistoryByDates : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Update Vehicle Information")]
        public int UpdateVehicleInformation(int userId, string SID, Int64 vehicleId, string xmlVehicleInfo,
           string newLicensePlate, int newBoxId)
        {
            try
            {
                Log(">>UpdateVehicleInformation(uid={0}, vid= {1})", userId, vehicleId);

                string oldLicensePlate = "";
                int oldBoxId = -1;

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                VehicInfo vInfo = (VehicInfo)XmlUtil.FromXml(xmlVehicleInfo, typeof(VehicInfo));

                using (Vehicle dbVehicle = new Vehicle(Application["ConnectionString"].ToString()))
                {
                    DataSet ds = dbVehicle.GetVehicleInfoByVehicleId(vehicleId);
                    if (ds.Tables.Count > 0 || ds.Tables[0].Rows.Count > 0)
                    {
                        oldLicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();
                        oldBoxId = Convert.ToInt32(ds.Tables[0].Rows[0]["BoxId"].ToString());
                    }

                    if (oldBoxId != newBoxId || oldLicensePlate.TrimEnd() != newLicensePlate.TrimEnd())
                        dbVehicle.ChangeVehicleInfo(vInfo, vehicleId, newLicensePlate, newBoxId);
                    else
                        dbVehicle.UpdateVehicleInfo(vInfo, vehicleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleInformation : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Create Vehicle")]
        public int CreateVehicle(int userId, string SID, int organizationId, int boxId,
                            string xmlVehicInfo, string licensePlate, ref Int64 vehicleId)
        {
            int retCode = (int)InterfaceError.NoError;
            try
            {
                Log(">> CreateVehicle(uid={0}, organizationId={1}, boxId={2})", userId, organizationId, boxId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);
                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                VehicInfo vInfo = (VehicInfo)XmlUtil.FromXml(xmlVehicInfo, typeof(VehicInfo));

                using (SystemConfig dbs = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    vehicleId = dbs.CreateVehicle(organizationId, boxId, vInfo, licensePlate);
                }
            }
            catch (Exception Ex)
            {
                LogException("<< CreateVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                retCode = (int)ASIErrorCheck.CheckError(Ex);
            }

            return retCode;
        }

        [WebMethod(Description = "Delete Vehicle.")]
        public int DeleteVehicle(int userId, string SID, Int64 vehicleId)
        {
            try
            {
                Log(">>DeleteVehicle(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.DeleteVehicle(vehicleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Assign a vehicle to box.")]
        public int AssignVehicleToBox(int userId, string SID, string licensePlate, int boxId, Int64 vehicleId)
        {
            try
            {
                Log(">>AssignVehicleToBox(uid={0}, LP={1}, boxId={2}, vid= {3} )", userId, licensePlate, boxId, vehicleId);

                //if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                VLF.CLS.Def.Structures.VehicAssign vehicAssign;
                vehicAssign.boxId = boxId;
                vehicAssign.licensePlate = licensePlate;
                vehicAssign.vehicleId = vehicleId;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AssignVehicle(vehicAssign);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AssignVehicleToBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle information by BoxId ID. XML File format:[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]")]
        public int GetVehicleInfoByBox(int userId, string SID, int BoxId, ref string xmlResult)
        {
            try
            {
                Log(">> GetVehicleInfoByBox(uid={0}, boxId={1})", userId, BoxId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserBox(userId, BoxId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleInfoByBoxId(BoxId);
                    if (Util.IsDataSetValid(dsVehicleInfo))
                        xmlResult = dsVehicleInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoByBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region Box

        [WebMethod(Description = "Retrieves the information for all the unassigned Boxes by Organizaton<br />XML format: <BoxId> ,	<OrganizationId> , <FwChId>, <FwId>, <FwName>, <BoxHwTypeId>, <BoxHwTypeName>, <ChName>, <BoxProtocolTypeId>, <BoxProtocolTypeName>, <CommModeId>, <CommModeName>, <OAPPort><br />Return 0 if success, Error code if not")]
        public int GetAllUnassignedBoxesByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedBoxesByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxes = null;

                using (Box dbBox = new Box(Application["ConnectionString"].ToString()))
                {
                    //dsBoxes = dbBox.GetAllAssignedBoxesInfo(assigned, organizationId);
                    dsBoxes = dbBox.GetBoxesInfo(false, organizationId);
                }

                if (Util.IsDataSetValid(dsBoxes))
                    xml = dsBoxes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedBoxesByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the information for all the assigned Boxes by Organizaton<br />XML Format: <BoxId>, <BoxHwTypeName>, <BoxProtocolTypeId>, <BoxProtocolTypeName>, <CommModeId>, <CommModeName>, <IPExternal>, <PortExternal>, <IPInternal>, <PortInternal>, <ModuleName>, <FwName><br />Return 0 if success, Error code if not")]
        public int GetAllAssignedBoxesByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllAssignedBoxesByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxes = null;

                using (Organization dbOrg = new Organization(Application["ConnectionString"].ToString()))
                {
                    //dsBoxes = dbBox.GetAllAssignedBoxesInfo(assigned, organizationId);
                    dsBoxes = dbOrg.GetAssignedBoxes(organizationId);
                }

                if (Util.IsDataSetValid(dsBoxes))
                    xml = dsBoxes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllAssignedBoxesByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get FwChId supported messages.XML File format: [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]")]
        public int GetAllSupportedMessagesByFwChId(int userId, string SID, short fwChId, string lang, ref string xml)
        {
            try
            {
                Log(">>GetAllSupportedMessagesByFwChId(uid={0}, fwChId={1}, lang={2})", userId, fwChId, lang);

                // if (!ValidateUserBox(userId, SID, boxId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsMessages = null;
                using (Box dbBox = new Box(Application["ConnectionString"].ToString()))
                {
                    dsMessages = dbBox.GetAllSupportedMessagesByFwChId(fwChId);
                }

                if (Util.IsDataSetValid(dsMessages))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(Application["ConnectionString"].ToString());
                        dbl.LocalizationData(lang, "BoxMsgInTypeId", "BoxMsgInTypeName", "MessageType", ref dsMessages);
                    }

                    xml = dsMessages.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllSupportedMessagesByFwChId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Box supported messages.XML File format: [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]")]
        public int GetAllSupportedMessagesByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                string lang = "en";

                Log(">>GetAllSupportedMessagesByBoxId(uid={0}, boxId={1}, lang={2})", userId, boxId, lang);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsMessages = null;
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dsMessages = dbBox.GetAllSupportedMessagesByBoxId(boxId);
                }

                if (Util.IsDataSetValid(dsMessages))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "BoxMsgInTypeId", "BoxMsgInTypeName", "MessageType", ref dsMessages);
                    }

                    xml = dsMessages.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllSupportedMessagesByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "GetDefaultOutputsInfoByHwTypeId. XML File format: [OutputId][OutputName][OutputAction]")]
        public int GetDefaultOutputsInfoByHwTypeId(int userId, string SID, short hwTypeId, ref string xml)
        {
            try
            {
                Log(">>GetDefaultOutputsInfoByHwTypeId(uid={0}, boxId={1})", userId, hwTypeId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsOutputs = null;
                using (SystemConfig dbSystem = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsOutputs = dbSystem.GetDefaultOutputsInfoByHwTypeId(hwTypeId);
                }

                // if (Util.IsDataSetValid(dsOutputs))
                if (ASIErrorCheck.IsAnyRecord(dsOutputs))
                    xml = dsOutputs.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDefaultOutputsInfoByHwTypeId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves the sensors for a specific profile id, XML Format: [ProfileId], [BoxHwTypeId], [ProfileName], [ProfileDescription], [SensorId], [SensorName], [SensorAction], [AlarmLevelOn], [AlarmLevelOff]")]
        public int GetSensorsByProfile(int userId, string SID, short profileId, ref string xml)
        {
            try
            {
                Log(">> GetSensorsByProfile(userId={0}, profileId={1}", userId, profileId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetSensorProfile(profileId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetSensorsByProfile : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the sensors profile for a specific hw type, if the hw type id = -1 all the profiles are returned, XML Format:  [SensorId] [SensorName] [SensorAction] [AlarmLevelOn] [AlarmLevelOff] ")]
        public int GetSensorsProfile(int userId, string SID, short hwTypeId, ref string xml)
        {
            try
            {
                Log(">> GetSensorsProfile(userId={0}, hwTypeId={1}", userId, hwTypeId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    if (hwTypeId > 0)
                        dsResult = dbSc.GetHwTypeSensorProfiles(hwTypeId);
                    else
                        dsResult = dbSc.GetAllSensorProfiles();
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetSensorsProfile : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Return sensors profile for a hwtype and Box ID . XML format :")]
        public int GetSensorProfileByBox(int userId, string SID, short hwTypeId, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetSensorProfileByBox(uid={0}, boxId={1}, HwTypeId={2})", userId, boxId, hwTypeId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsSensors = null;
                DataSet dsProfile = null;
                using (Box dbb = new Box(Application["ConnectionString"].ToString()))
                {
                    dsSensors = dbb.GetBoxSensorsInfo(boxId);
                }
                if (Util.IsDataSetValid(dsSensors))
                {
                    string profileName = "";
                    using (SystemConfig dbs = new SystemConfig(Application["ConnectionString"].ToString()))
                    {
                        profileName = dbs.GetSensorProfileName(hwTypeId, dsSensors.Tables[0]);
                        if ((profileName != null) && (profileName != ""))
                        {
                            dsProfile = dbs.GetSensorProfile(profileName);
                        }
                    }
                }

                if (Util.IsDataSetValid(dsProfile))
                    xml = dsProfile.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetSensorProfileByBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Return sensors information for a Box ID . XML format : [SensorId], [SensorName], [SensorAction], [AlarmLevelOn], [AlarmLevelOff]")]
        public int GetSensorsInfoByBox(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetSensorsInfoByBox(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsSensorsInfo = null;

                using (Box dbb = new Box(Application["ConnectionString"].ToString()))
                {
                    dsSensorsInfo = dbb.GetBoxSensorsInfo(boxId);
                }

                if (Util.IsDataSetValid(dsSensorsInfo))
                    xml = dsSensorsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetSensorsInfoByBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Create Box")]
        public int CreateBox(int userId, string SID, int organizationId, int boxId, short fwChId, long featureMask, short hwTypeId,
            string xmlProtocolMaxMessages, string xmlCommInfo, string xmlOutputs, string xmlSensors, string xmlMessages,
            string xmlVehicInfo, string licensePlate)
        {
            try
            {
                Log(">> CreateBox(uid={0}, organizationId={1}, boxId={2})", userId, organizationId, boxId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);
                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsProtocolMaxMessages = new DataSet();
                StringReader strrXML = new StringReader(xmlProtocolMaxMessages);
                dsProtocolMaxMessages.ReadXml(strrXML);

                DataSet dsCommInfo = new DataSet();
                strrXML = new StringReader(xmlCommInfo);
                dsCommInfo.ReadXml(strrXML);

                DataSet dsOutputs = new DataSet();
                strrXML = new StringReader(xmlOutputs);
                dsOutputs.ReadXml(strrXML);

                DataSet dsSensors = new DataSet();
                strrXML = new StringReader(xmlSensors);
                dsSensors.ReadXml(strrXML);

                DataSet dsMessages = new DataSet();
                strrXML = new StringReader(xmlMessages);
                dsMessages.ReadXml(strrXML);

                VehicInfo vInfo = (VehicInfo)XmlUtil.FromXml(xmlVehicInfo, typeof(VehicInfo));

                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    if (dbSc.CreateBox(organizationId, boxId, fwChId, featureMask, hwTypeId, dsProtocolMaxMessages, dsCommInfo, dsOutputs, dsSensors, dsMessages, vInfo, licensePlate) == false)
                        return (int)InterfaceError.ServerError;
                }
            }
            catch (Exception Ex)
            {
                LogException("<< CreateBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }

            return (int)InterfaceError.NoError;
        }

        [WebMethod(Description = "Delete Box ")]
        public int DeleteBox(int userId, string SID, int boxId)
        {
            try
            {
                int retVal = (int)InterfaceError.NoError;
                Log(">> DeleteBox(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                using (SystemConfig dbSc = new SystemConfig(Application["ConnectionString"].ToString()))
                {
                    if (dbSc.DeleteBox(boxId) == 0)
                        retVal = 1;// (int)InterfaceError.ZeroRowAffected;
                }
                return retVal;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get box configuration information. XML File: [BoxId], [FwId], [FwChId], [BoxHwTypeId], [BoxHwTypeName], [MaxSensorsNum], [MaxOutputsNum], [BoxProtocolTypeId], [BoxProtocolTypeName], [CommModeId], [CommModeName], [ChPriority], [FwTypeId], [FwLocalPath], [FwOAPPath], [FwDateReleased], [MaxGeozones], [MaxMsgs], [MaxTxtMsgs], [OAPPort], [ChId], [ChName], [FwAttributes1]")]
        public int GetBoxConfigFeatures(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetBoxConfigFeatures(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxConfigInfo = null;
                using (Box dbBox = new Box(Application["ConnectionString"].ToString()))
                {
                    dsBoxConfigInfo = dbBox.GetBoxConfigurationFeatures(boxId);
                }

                if (Util.IsDataSetValid(dsBoxConfigInfo))
                    xml = dsBoxConfigInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxConfigFeatures : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #endregion


        #region Driver

        [WebMethod(Description = "Get a driver")]
        public int GetDriverById(int userId, string SID, int driverId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverById(uId={0}, DriverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetDriver(driverId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverById : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Get all drivers for organization")]
        public int GetDriversByOrganization(int userId, string SID, int organizationId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriversByOrganization(uId={0}, OrganizationId={1})", userId, organizationId);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriversForOrganization(organizationId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriversByOrganization : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Create new driver")]
        public int CreateDriver(int userId, string SID, string xmlDriverInfo)
        {
            try
            {
                Log(">> CreateDriver(uId={0}, data={1})", userId, xmlDriverInfo);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DriverInfo dInfo = (DriverInfo)XmlUtil.FromXml(xmlDriverInfo, typeof(DriverInfo));
                using (DriverManager driver = new DriverManager(Application["ConnectionString"].ToString()))
                {
                    driver.CreateDriver(dInfo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< CreateDriver : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Update Driver Information")]
        public int UpdateDriverInformation(int userId, string SID, string xmlDriverInfo)
        {
            try
            {
                Log(">> UpdateDriverInformation(uId={0}, data={1})", userId, xmlDriverInfo);
                DriverInfo dInfo = (DriverInfo)XmlUtil.FromXml(xmlDriverInfo, typeof(DriverInfo));

                if (!ValidateUserDriver(userId, SID, dInfo.driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(Application["ConnectionString"].ToString()))
                {
                    driver.UpdateDriver(dInfo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< UpdateDriverInformation : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }
        [WebMethod(Description = "Delete a driver")]
        public int DeleteDriver(int userId, string SID, int driverId)
        {
            try
            {
                Log(">> DeleteDriver(uId={0}, DriverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.DeleteDriver(driverId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeleteDriver : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get driver assignment history with dates")]
        public int GetDriverAssignmentHistoryByDates(int userId, string SID, int driverId, DateTime from, DateTime to, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverAssignmentHistoryByDates(uId={0}, driverId={1}, dtFrom={2}, dtTo={3})", userId, driverId, from, to);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriverAssignmentHistory(driverId, from, to);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverAssignmentHistoryByDates : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Add driver - vehicle assignment history")]
        public int AddAssignmentHistory(int userId, string SID, int driverId, long vehicleId, string description, DateTime startDt, DateTime endDt)
        {
            int rows = 0;
            try
            {
                Log(">> AddAssignmentHistory(uId={0}, driverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    rows = driver.AddAssignmentHistory(driverId, vehicleId, userId, description, startDt, endDt);
                }

                return (rows > 0) ? (int)InterfaceError.NoError : (int)InterfaceError.InvalidParameter;
            }
            catch (Exception ex)
            {
                LogException("<< AddAssignmentHistory : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Add a new driver assignment")]
        public int AssignDriverToVehicle(int userId, string SID, long vehicleId, int driverId, string description)
        {
            try
            {
                Log(">> AssignDriverToVehicle(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.AddDriverAssignment(userId, vehicleId, driverId, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AssignDriverToVehicle : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Delete a driver assignment")]
        public int DeleteDriverAssignment(int userId, string SID, long vehicleId, int driverId, string description)
        {
            try
            {
                Log(">> DeleteDriverAssignment(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.DeleteActiveDriverAssignment(userId, vehicleId, driverId, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeleteDriverAssignment : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }
        [WebMethod(Description = "Get driver assignment")]
        public int GetDriverAssignment(int userId, string SID, long vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverAssignment(uId={0}, vehicleId = {1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriverActiveAssignment(vehicleId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverAssignment : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }
        [WebMethod(Description = "Get unassigned drivers for organization")]
        public int GetUnassignedDriversByOrganization(int userId, string SID, int organizationId, ref string xmlResult)
        {
            try
            {
                Log(">> GetUnassignedDriversByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetUnassignedDriversForOrganization(organizationId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetUnassignedDriversByOrganization : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }
        #endregion //Driver


        #region Fleet

        [WebMethod(Description = "Assign vehicle to fleet.")]
        public int AddVehicleToFleet(int userId, string SID, int fleetId, Int64 vehicleId)
        {
            try
            {
                Log(">> AddVehicleToFleet(uId={0}, fleetId={1}, vehicleId={2})", userId, fleetId, vehicleId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.AddVehicleToFleet(fleetId, vehicleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicleToFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add a new Fleet.")]
        public int AddFleet(int userId, string SID, string fleetName, int organizationId,
                          string description, ref int fleetId)
        {
            try
            {
                Log(">> AddFleet(uId={0}, fleetName={1}, orgId={2}, description={3})",
                     userId, fleetName, organizationId, description);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                // Authorize
                // TODO: 10 - > DeleteFleet
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 10);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    fleetId = dbFleet.AddFleet(userId, fleetName, organizationId, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Delete existing Fleet.")]
        public int DeleteFleetById(int userId, string SID, int fleetId)
        {
            try
            {
                Log(">> DeleteFleetById( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                // TODO: 11 - > DeleteFleet
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 11);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.DeleteFleetByFleetId(fleetId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteFleetById : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Update fleet information.")]
        public int UpdateFleetInfo(int userId, string SID, int fleetId,
                                 string fleetName, int organizationId, string description)
        {
            try
            {
                Log(">> UpdateFleetInfo(fleetId={0}, fleetName={1} , orgId={2}, description={3})",
                        fleetId, fleetName, organizationId, description);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                // Authorize
                // TODO: 12 - > UpdateFleetInfo
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 12);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.UpdateInfo(fleetId, fleetName, organizationId, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateFleetInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all active vehicles that are unassigned to the current fleet.XML file format: XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]")]
        public int GetAllUnassingVehiclesByFleet(int userId, string SID, int organizationId,
                                                      int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassingVehiclesByFleet(uId={0}, orgId={1}, fleetId={2})",
                                    userId, organizationId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetAllUnassingToFleetVehiclesInfo(organizationId, fleetId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassingVehiclesByFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves fleets info by organization. XML File format:  [organizationId]")]
        public int GetFleetsInfoByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetFleetsInfoByOrganization (uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetFleetsInfoByOrganizationId(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Remove vehicle from fleet.")]
        public int RemoveVehicleFromFleet(int userId, string SID, int fleetId, Int64 vehicleId)
        {
            try
            {
                Log(">> RemoveVehicleFromFleet(uId={0}, fleetId={1}, vehicleId={2})", userId, fleetId, vehicleId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.DeleteVehicleFromFleet(fleetId, vehicleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< RemoveVehicleFromFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion


        #region Person

        [WebMethod(Description = "Retrieves information for a person using the id, XML Format: [PersonId], [DriverLicense], [FirstName], [LastName], [MiddleName], [Birthday], [Address], [City], [StateProvince], [Country], [PhoneNo1], [PhoneNo2], [CellNo], [LicenseExpDate], [LicenseEndorsements], [Height], [Weight], [Gender], [EyeColor], [HairColor], [IdMarks], [Certifications], [Description]")]
        public int GetPersonInfoById(int userId, string SID, string personId, ref string xml)
        {
            try
            {
                Log(">> GetPersonsInfoById(userId={0}, personId={1}", userId, personId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                ///LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (PersonInfo dbSc = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetPersonInfoByPersonId(personId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = XmlUtil.GetXmlIncludingNull(dsResult); //dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetPersonsInfoById : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the list of Persons information, XML Format: [PersonId], [FirstName], [LastName]")]
        public int GetAllPersonsInfoByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllPersonsInfoByOrganization(userId={0}, organizationId={1}", userId, organizationId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                /// LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (PersonInfo dbSc = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllPersonsInfo(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllPersonsInfoByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves the list of Unassigned Persons information, XML Format: [PersonId], [FirstName], [LastName]")]
        public int GetAllUnassignedPersonsInfo(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedPersonsInfo(userId={0}", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsResult = null;

                using (PersonInfo dbSc = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllUnassignedPersons();
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedPersonsInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Create a new PersonInfo")]
        public int CreatePersonInfo(int userId, string SID, string xmlPersonInfo, ref string personId)
        {
            try
            {
                Log(">> CreatePersonInfo(uId={0}, data={1})", userId, xmlPersonInfo);
                PersonInfoStruct dInfo = (PersonInfoStruct)XmlUtil.FromXml(xmlPersonInfo, typeof(PersonInfoStruct));

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pi.AddPerson(ref dInfo);
                    personId = dInfo.personId;
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< CreatePersonInfo : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Delete existing person info")]
        public int DeletePersonById(int userId, string SID, string personId)
        {
            try
            {
                Log(">> DeletePersonById(uId={0}, personId={1})", userId, personId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pi.DeletePersonByPersonId(personId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeletePersonById : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Update existing person info")]
        public int UpdatePersonInfo(int userId, string SID, string xmlPersonInfo)
        {
            try
            {
                Log(">> UpdatePersonInfo(uId={0}, data={1})", userId, xmlPersonInfo);
                PersonInfoStruct dInfo = (PersonInfoStruct)XmlUtil.FromXml(xmlPersonInfo, typeof(PersonInfoStruct));

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pi.UpdateInfo(dInfo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< UpdatePersonInfo : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get All Unassigned persons")]
        public int GetAllUnassignedPersonsIds(int userId, string SID, ref string[] personIds)
        {
            ArrayList pIds = null;
            try
            {
                Log(">> GetAllUnassignedPersonsIds(uId={0})", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pIds = pi.GetAllUnassignedPersonsIds();
                    personIds = new string[pIds.Count];
                    //personIds = pIds.ToArray(int);
                    personIds = pIds.ToArray(typeof(string)) as string[];
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< UpdatePersonInfo : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        #endregion


        #region User

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
        [WebMethod(Description = "update the user group op assignment (user group security) ")]
        public int UpdateUserGroupOperationsAssignment(int userId, string SID, int userGroupId, short operationType, int[] opIdsToAdd, int[] opIdsToRemove)
        {
            try
            {
                Log(">> UpdateUserGroupOperationsAssignment(userGroupId={0}, operationType{1})", userGroupId, operationType);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

                //bool includeHgiAdmin = false;
                DataSet dsOperations = null;
                using (UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
                {
                    if (opIdsToRemove.Length > 0)
                        dbUserGroup.UnassignOperationsToUserGroup((Enums.OperationType)operationType, userGroupId, opIdsToRemove);
                    if (opIdsToAdd.Length > 0)
                        dbUserGroup.AssignOperationsToUserGroup((Enums.OperationType)operationType, userGroupId, opIdsToAdd);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateUserGroupOperationsAssignment : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns all unassigned users to specific group and organization. XML File format: [UserId],[UserName],[FirstName],[LastName]")]
        public int GetAllUnassignedUsersByUserGroupAndOrganization(int currUserId, string SID, short userGroupId, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedUsersByUserGroupAndOrganization ( currUserId = {0}, userGroupId = {1}, organizationId= {2})",
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
                LogException("<< GetAllUnassignedUsersByUserGroupAndOrganization : uId={0}, EXC={1}", currUserId, Ex.Message);
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

        #endregion

    }
}
