/** \file      DBFleet.cs
 *  \comment   keywords for logs
 *             uid            ->    user id
 *             fleetId        ->    fleet id
 *			   fleetIds	      ->	multiple fleet ids
 *             vehicleId      ->    vehicle Id
 *             driverId       ->    driver Id
 *             orgId          ->    organization Id
 *             dtFrom         ->    date time from
 *             dtTo           ->    date time to
 *             tSpan          ->    time span to execute the web method
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using VLF.ERR;
using VLF.DAS.Logic;
using VLF.CLS;
using System.Configuration;

namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "http://www.sentinelfm.com")]

    /// <summary>
    /// Summary description for DBFleet.
    /// </summary>
    public class DBFleet : System.Web.Services.WebService
    {
        public DBFleet()
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


        #region refactored functions
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
        ///   there are two important keywords 
        ///      -  uid         - user id
        ///      -  tSpan       - how fast was the operation
        ///   most of the time, the string format WILL CONTAIN KEYWORD, tSpan=... which is the time to execute 
        ///   the method name between << and (
        ///   in the same time, this function can send the information in real-time to a server
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void LogFinal(string strFormat, params object[] objects)
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

        /// <summary>
        ///        it replaces some of the common calls to the user's rights layer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        private bool ValidateUserFleet(int userId, string SID, int fleetId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserFleet(userId, fleetId);
            }

        }

        /// <summary>
        ///        it replaces some of the common calls to the user's rights layer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        private bool ValidateUserMultipleFleets(int userId, string SID, string fleetIds)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                try
                {
                    string[] ids = fleetIds.Split(',');
                    foreach (string s in ids)
                    {
                        if (dbUser.ValidateUserFleet(userId, int.Parse(s)) == false)
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        private bool ValidateUserVehicle(int userId, string SID, long vehicleId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserVehicle(userId, vehicleId);
            }

            return true;
        }

        private bool ValidateUserOrganization(int userId, string SID, int orgId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserOrganization(userId, orgId);
            }

            return true;
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


        #endregion  refactored functions

        #region Fleet/Vehicle Interfaces

        [WebMethod(Description = "Returns XML info for Idling messages of vehicles for a particular Fleet by Fleet ID. XML file format:   [BoxId],[LicensePlate],[VehicleDescription],[VehicleId],[VinNum],[OriginDateTime],[Latitude],[Longitude],[BoxMsgInTypeId],[CustomProp]")]
        public int GetVehiclesIdlingDurationByFleetId(int userId, string SID, int fleetId,
                                                      DateTime fromDateTime, DateTime toDateTime, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesIdlingDurationByFleetId(uId={0}, fleetId={1}, dtFrom={2} dtTo={3})",
                         userId, fleetId, fromDateTime, toDateTime);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetIdlingDurationForFleetId(fleetId, userId, fromDateTime, toDateTime);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesIdlingDurationByFleetId(uId={0}, fleetId={1}, tSpan={2}", userId, fleetId,
                            DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesIdlingDurationByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns XML info for Idling messages of vehicles for a particular Fleet by Multiple Fleet IDs. XML file format:   [BoxId],[LicensePlate],[VehicleDescription],[VehicleId],[VinNum],[OriginDateTime],[Latitude],[Longitude],[BoxMsgInTypeId],[CustomProp]")]
        public int GetVehiclesIdlingDurationByMultipleFleetIds(int userId, string SID, string fleetIds,
                                                      DateTime fromDateTime, DateTime toDateTime, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesIdlingDurationByFleetId(uId={0}, fleetIds={1}, dtFrom={2} dtTo={3})",
                         userId, fleetIds, fromDateTime, toDateTime);

                if (!ValidateUserMultipleFleets(userId, SID, fleetIds))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetIdlingDurationForMultipleFleetIds(fleetIds, userId, fromDateTime, toDateTime);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesIdlingDurationByFleetId(uId={0}, fleetIds={1}, tSpan={2}", userId, fleetIds,
                            DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesIdlingDurationByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

       
        /// <summary>
        ///       Returns ManagerName by VehicleId
        /// </summary>
        /// <param name="fleetId"> OrganizationId</param>
        /// <param name="featureMask">VehicleId </param>
        /// <returns>XML [ManagerName]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of ManagerName for Vehicle id. 
                           XML file format: [ManagerName]")]
        public int GetManagerNameByVehicleId(int OrganizationId, int VehicleId,
                                                        ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetManagerNameByVehicleId( organizationId={0}, vehicleId={1}",
                                                     OrganizationId, VehicleId);


                DataSet dsManagerNameInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(OrganizationId)))
                {
                    dsManagerNameInfo = dbFleet.GetManagerNameByVehicleId(VehicleId);
                }

                if (Util.IsDataSetValid(dsManagerNameInfo))
                    xml = dsManagerNameInfo.GetXml();

                LogFinal("<< GetManagerNameByVehicleId(uId={0}, vehicleId={1}, tSpan={2}", OrganizationId, VehicleId,
                    DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetManagerNameByVehicleId : uId={0}, EXC={1}", OrganizationId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        
        /// <summary>
        ///       Returns Rows affected after setting manager name to AssignedVehiclesList
        /// </summary>
        /// <param name="OrganizationId">OrganizationId</param>
        /// <param name="AssignedVehiclesList">AssignedVehiclesList</param>
        /// <param name="ManagerName">ManagerName</param>
        /// <returns>rowsAffected
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of vehicles for a particular fleet. 
                           XML file format: [ManagerName]")]
        public int SetManagerNameByVehicleId(int OrganizationId, string AssignedVehiclesList, string ManagerName, ref int rowsAffected)                                                        
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> SetManagerNameByVehicleId( organizationId={0}, AssignedVehiclesList={1}, ManagerName={2}",
                                                     OrganizationId, AssignedVehiclesList, ManagerName);


                // rowsAffected = 0;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(OrganizationId)))
                {
                    rowsAffected = dbFleet.SetManagerNameByVehicleId(AssignedVehiclesList, ManagerName);
                }



                LogFinal("<< SetManagerNameByVehicleId(uId={0}, AssignesVehiclesList={1}, tSpan={2}", OrganizationId, AssignedVehiclesList,
                    DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetManagerNameByVehicleId : uId={0}, EXC={1}", OrganizationId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        /// Returns vehicle information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles for a particular Fleet by Fleet ID. XML file format:   [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description] ")]
        public int GetVehiclesInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            // string xml = "";
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesInfoXMLByFleetId ( uId={0}, fleetId={1} )", userId, fleetId);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //     return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesInfoXMLByFleetId(uId={0}, fleetId={1}, tSpan={2}", userId, fleetId,
                   DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Returns vehicle information by fleet ids. 
        /// </summary>
        /// <param name="fleetIds"></param>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles for a particular Fleet by Fleet ID. XML file format:   [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description] ")]
        public int GetVehiclesInfoXMLByMultipleFleetIds(int userId, string SID, string fleetIds, ref string xml)
        {
            // string xml = "";
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesInfoXMLByFleetId ( uId={0}, fleetIds={1} )", userId, fleetIds);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesInfoByMultipleFleetIds(fleetIds);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesInfoXMLByFleetId(uId={0}, fleetIds={1}, tSpan={2}", userId, fleetIds,
                   DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        /// <summary>
        /// Returns vehicle peripherals by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with Peripherals by Fleet ID")]
        public int GetVehiclesPeripheralInfoByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesPeripheralInfoByFleetId ( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesPeripheralInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesPeripheralInfoByFleetId(uId={0}, fleetId={1}, tSpan={2}", userId, fleetId,
                   DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfo(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Stopwatch watch = new Stopwatch();
                Log(">> GetVehiclesLastKnownPositionInfo(uId={0}, fleetId={1})", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    watch.Reset();
                    watch.Start();
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, userId);
                    watch.Stop();
                    Log("-- GetVehiclesLastKnownPositionInfo : duration={0}", watch.Elapsed.TotalSeconds);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfo(uId={0}, fleetId={1}, tSpan={2}", userId, fleetId,
                    DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <comment>
        ///      GB -> language is not used !!
        /// </comment>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoByLang_NewTZ(int userId, string SID, int fleetId, string language, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetId);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo_NewTZ(fleetId, userId, language);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1}, tSpan={2} )",
                               userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end
        // Changes for TimeZone Feature start

        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <comment>
        ///      GB -> language is not used !!
        /// </comment>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoByLang(int userId, string SID, int fleetId, string language, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetId);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, userId, language);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1}, tSpan={2} )",
                               userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <comment>
        ///      GB -> language is not used !!
        /// </comment>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang_NewTZ(int userId, string SID, string fleetIds, string language, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetIds);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByMultipleFleets_NewTZ(fleetIds, userId, language);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetIds={1}, tSpan={2} )",
                               userId, fleetIds, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes fopr TimeZone Feature end

        /// <summary>
        /// Returns vehicles last known position information by fleet id. 
        /// </summary>
        /// <comment>
        ///      GB -> language is not used !!
        /// </comment>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang(int userId, string SID, string fleetIds, string language, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetIds);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByMultipleFleets(fleetIds, userId, language);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetIds={1}, tSpan={2} )",
                               userId, fleetIds, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        /// <summary>
        /// Returns vehicles last known position information by vehicles list xml. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoByVehiclesXML(int userId, string SID, string vehiclesXML, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoByVehiclesXML (uId={0}, xml={1})", userId, vehiclesXML);

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserFleet(userId, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByVehiclesXML(userId, vehiclesXML);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByVehiclesXML (uId={0}, xml={1}), tSpan={2}",
                                    userId, vehiclesXML, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoByVehiclesXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        /// <summary>
        /// Returns vehicles last known position information by vehicles list xml. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoByVehiclesXML_New(int userId, string SID, string vehiclesXML, string language, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoByVehiclesXML_New(uId={0}, fleetId={1} )", userId, vehiclesXML);

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserFleet(userId, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoByVehiclesXML_New(userId, vehiclesXML, language);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByVehiclesXML_New : uId={0}, fleetId={1}, tSpan={2}",
                                   userId, vehiclesXML, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoByVehiclesXML_New : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///            Returns vehicles last known position within a certain distance from a landmark
        /// </summary>
        /// <comment>
        ///         GB -> this could be done completely in a store procedure
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fleetId"></param>
        /// <param name="fleetId"></param>
        /// <param name="distance"></param>
        /// <param name="landmarkName"></param>
        /// <param name="xml" ref ></param>
        ///<returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Landmark Name , distance from Landmark and Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[Description]")]
        public int GetVehiclesLastKnownPositionInfoNearestToLandmark(int userId, string SID, int organizationId,
                                                         int fleetId, int distance, string landmarkName, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoNearestToLandmark( uId={0}, organizationId={1}, fleetId={2}, distance={3}, landmarkName={4})",
                                 userId, organizationId, fleetId, distance, landmarkName);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {

                    // 1. Retrieve fleet last known position
                    // [LicensePlate],[VehicleId],[BoxId],[OriginDateTime],
                    // [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[StreetAddress],
                    // [Description],[BoxArmed],[LastCommunicatedDateTime],[GeoFenceEnabled],
                    // [IconTypeId],[IconTypeName],[VehicleTypeName],
                    // [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],
                    // [PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed].
                    // [FwTypeId],[Dormant],[DormantDateTime]
                    DataSet dsFleetLocation = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, userId, "en");
                    if (dsFleetLocation != null && dsFleetLocation.Tables.Count > 0 && dsFleetLocation.Tables[0].Rows.Count > 0)
                    {
                        // 2. Retrieve landmark location (lat,lon)
                        using (Organization queryOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                        {
                            double landmarkLat = 0;
                            double landmarkLon = 0;
                            queryOrganization.GetLandmarkLocation(organizationId, landmarkName, ref landmarkLat, ref landmarkLon);
                            VLF.MAP.GeoPoint landmarkCoord = new VLF.MAP.GeoPoint(landmarkLat, landmarkLon);
                            VLF.MAP.GeoPoint currVehicleCoord = new VLF.MAP.GeoPoint();

                            int distanceBetweenGPS = 0;

                            #region Create map engine instance
                            VLF.MAP.ClientMapProxy map = null;
                            using (SystemConfig qSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                            {
                                DataSet dsGeoCodeInfo = qSystem.GetUserGeoCodeEngineInfo(userId);
                                if (dsGeoCodeInfo != null && dsGeoCodeInfo.Tables.Count > 0 && dsGeoCodeInfo.Tables[0].Rows.Count > 0)
                                    map = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
                                else
                                    throw new ASIDataNotFoundException("Unable to retrieve map engine info by user=" + userId);
                            }
                            #endregion

                            // 4. Filter vehicles located near to landmark
                            foreach (DataRow ittr in dsFleetLocation.Tables[0].Rows)
                            {
                                currVehicleCoord.Latitude = Convert.ToDouble(ittr["Latitude"]);
                                currVehicleCoord.Longitude = Convert.ToDouble(ittr["Longitude"]);
                                // 5. Calculates distance between landmark and current location
                                distanceBetweenGPS = (int)(Math.Round(map.GetDistance(landmarkCoord, currVehicleCoord)));
                                if (distanceBetweenGPS <= VLF.CLS.Def.Const.unassignedIntValue ||
                                     distanceBetweenGPS > distance)
                                {
                                    ittr.Delete();
                                }
                            }
                            xml = dsFleetLocation.GetXml();
                        }	/// 	using( Organization queryOrganization				

                    } //  if(dsFleetLocation != null

                } // using( Fleet dbFleet 

                LogFinal("<< GetVehiclesLastKnownPositionInfoNearestToLandmark( uId={0}, organizationId={1}, fleetId={2}, distance={3}, landmarkName={4}, tSpan={5})",
                                     userId, organizationId, fleetId, distance, landmarkName, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoNearestToLandmark : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }

        // Changes for TimeZone Feature start

        /// <summary>
        ///            returns vehicle found in proximity of a given location
        /// </summary>
        /// <comment>
        ///         GB -> this could be done completely in a store procedure
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="fleetId"></param>
        /// <param name="distance"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <comment>
        ///         TO REEXAMINE : the operation can be executed on the server side 
        /// </comment>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Landmark Name , distance from Landmark and Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[Description]")]
        public int GetVehiclesLastKnownPositionInfoNearestToLatLon_NewTZ(int userId, string SID, int organizationId,
                                                  int fleetId, int distance, double lat, double lon, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">>GetVehiclesLastKnownPositionInfoNearestToLatLon (uId={0}, orgId={1}, fleetId={2}, distance={3}, ({4},{5}))",
                        userId, organizationId, fleetId, distance, lat, lon);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    // 1. Retrieve fleet last known position
                    DataSet dsFleetLocation = dbFleet.GetVehiclesLastKnownPositionInfo_NewTZ(fleetId, userId, "en");
                    if (Util.IsDataSetValid(dsFleetLocation))
                    {
                        // 2. Retrieve location (lat,lon)
                        VLF.MAP.GeoPoint searchPosition = new VLF.MAP.GeoPoint(lat, lon);
                        VLF.MAP.GeoPoint currVehicleCoord = new VLF.MAP.GeoPoint();

                        int distanceBetweenGPS = 0;

                        #region Create map engine instance

                        VLF.MAP.ClientMapProxy map = null;
                        using (SystemConfig qSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                        {
                            DataSet dsGeoCodeInfo = qSystem.GetUserGeoCodeEngineInfo(userId);
                            if (Util.IsDataSetValid(dsGeoCodeInfo))
                                map = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
                            else
                                throw new ASIDataNotFoundException("Unable to retrieve map engine info by user=" + userId);
                        }

                        #endregion

                        // 4. Filter vehicles located near to landmark
                        foreach (DataRow ittr in dsFleetLocation.Tables[0].Rows)
                        {
                            currVehicleCoord.Latitude = Convert.ToDouble(ittr["Latitude"]);
                            currVehicleCoord.Longitude = Convert.ToDouble(ittr["Longitude"]);
                            // 5. Calculates distance between landmark and current location
                            distanceBetweenGPS = (int)(Math.Round(map.GetDistance(searchPosition, currVehicleCoord)));
                            if ((distanceBetweenGPS <= VLF.CLS.Def.Const.unassignedIntValue) ||
                                (distanceBetweenGPS > distance))
                            {
                                ittr.Delete();
                            }
                        }

                        xml = dsFleetLocation.GetXml();
                    }
                }

                LogFinal("<< GetVehiclesLastKnownPositionInfoNearestToLatLon (uId={0}, orgId={1}, fleetId={2}, distance={3}, ({4},{5}), tSpan={6})",
                       userId, organizationId, fleetId, distance, lat, lon, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoNearestToLatLon : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature end
        /// <summary>
        ///            returns vehicle found in proximity of a given location
        /// </summary>
        /// <comment>
        ///         GB -> this could be done completely in a store procedure
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="fleetId"></param>
        /// <param name="distance"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <comment>
        ///         TO REEXAMINE : the operation can be executed on the server side 
        /// </comment>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Landmark Name , distance from Landmark and Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[Description]")]
        public int GetVehiclesLastKnownPositionInfoNearestToLatLon(int userId, string SID, int organizationId,
                                                  int fleetId, int distance, double lat, double lon, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">>GetVehiclesLastKnownPositionInfoNearestToLatLon (uId={0}, orgId={1}, fleetId={2}, distance={3}, ({4},{5}))",
                        userId, organizationId, fleetId, distance, lat, lon);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    // 1. Retrieve fleet last known position
                    DataSet dsFleetLocation = dbFleet.GetVehiclesLastKnownPositionInfo(fleetId, userId, "en");
                    if (Util.IsDataSetValid(dsFleetLocation))
                    {
                        // 2. Retrieve location (lat,lon)
                        VLF.MAP.GeoPoint searchPosition = new VLF.MAP.GeoPoint(lat, lon);
                        VLF.MAP.GeoPoint currVehicleCoord = new VLF.MAP.GeoPoint();

                        int distanceBetweenGPS = 0;

                        #region Create map engine instance

                        VLF.MAP.ClientMapProxy map = null;
                        using (SystemConfig qSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                        {
                            DataSet dsGeoCodeInfo = qSystem.GetUserGeoCodeEngineInfo(userId);
                            if (Util.IsDataSetValid(dsGeoCodeInfo))
                                map = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
                            else
                                throw new ASIDataNotFoundException("Unable to retrieve map engine info by user=" + userId);
                        }

                        #endregion

                        // 4. Filter vehicles located near to landmark
                        foreach (DataRow ittr in dsFleetLocation.Tables[0].Rows)
                        {
                            currVehicleCoord.Latitude = Convert.ToDouble(ittr["Latitude"]);
                            currVehicleCoord.Longitude = Convert.ToDouble(ittr["Longitude"]);
                            // 5. Calculates distance between landmark and current location
                            distanceBetweenGPS = (int)(Math.Round(map.GetDistance(searchPosition, currVehicleCoord)));
                            if ((distanceBetweenGPS <= VLF.CLS.Def.Const.unassignedIntValue) ||
                                (distanceBetweenGPS > distance))
                            {
                                ittr.Delete();
                            }
                        }

                        xml = dsFleetLocation.GetXml();
                    }
                }

                LogFinal("<< GetVehiclesLastKnownPositionInfoNearestToLatLon (uId={0}, orgId={1}, fleetId={2}, distance={3}, ({4},{5}), tSpan={6})",
                       userId, organizationId, fleetId, distance, lat, lon, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoNearestToLatLon : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Returns the fleets where specified vehicle (vehicleId) is assigned 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns>XML [OrganizationName],[FleetName],[FleetDescription]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of Fleets this vehicle belongs to by Vehicle ID. XML file format [OrganizationName],[FleetName],[FleetDescription]")]
        public int GetFleetsInfoXMLByVehicleId(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetFleetsInfoXMLByVehicleId( uId={0}, vehicleId={1} )", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsFleetsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsFleetsInfo = dbFleet.GetFleetsInfoByVehicleId(vehicleId, userId);
                }

                if (Util.IsDataSetValid(dsFleetsInfo))
                    xml = dsFleetsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoXMLByVehicleId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         Retrieve vehicles unassigned to any fleet
        /// </summary>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns list of vehicles currently unassigned to any Fleet. XML file format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]")]
        public int GetAllUnassingToFleetsVehiclesInfoXML(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassingToFleetsVehiclesInfoXML( uId={0}, orgId={1} )", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetAllUnassingToFleetsVehiclesInfo(organizationId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassingToFleetsVehiclesInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         Retrieve all active vehicles that are unassigned to the current fleet.
        /// </summary>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns all active vehicles that are unassigned to the current fleet.XML file format: XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]")]
        public int GetAllUnassingToFleetVehiclesInfoXML(int userId, string SID, int organizationId,
                                                      int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassingToFleetVehiclesInfoXML(uId={0}, orgId={1}, fleetId={2})",
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
                LogException("<< GetAllUnassingToFleetVehiclesInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Remove vehicle from fleet.")]
        public int DeleteVehicleFromFleet(int userId, string SID, int fleetId, Int64 vehicleId)
        {
            try
            {
                Log(">> DeleteVehicleFromFleet(uId={0}, fleetId={1}, vehicleId={2})", userId, fleetId, vehicleId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetVehicles",
                                                  string.Format("FleetId={0} AND VehicleId={1}", fleetId, vehicleId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({1}) from fleet({0}) - Initial values", fleetId, vehicleId));

                    dbFleet.DeleteVehicleFromFleet(fleetId, vehicleId);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetVehicles",
                                                  string.Format("FleetId={0} AND VehicleId={1}", fleetId, vehicleId),
                                                  "Delete", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle({1}) from fleet({0})", fleetId, vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteVehicleFromFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
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

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetVehicles",
                                                string.Format("FleetId={0} AND VehicleId={1}", fleetId, vehicleId),
                                                "Add", this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                string.Format("Add vehicle({1}) to fleet({0})", fleetId, vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicleToFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///      Returns all vehicles active assignment configuration for the current organization
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>XML [Description],[BoxId],[FwId],[FwName],[FwDateReleased]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of vehicles for a particular Fleet by Fleet ID. XML file format:   
                              [Description],[BoxId],[FwId],[FwName],[FwDateReleased]")]
        public int GetFleetAllActiveVehiclesCfgInfo(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetFleetAllActiveVehiclesCfgInfo(uId={0}, fleetId={1})", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetFleetAllActiveVehiclesCfgInfo(fleetId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetFleetAllActiveVehiclesCfgInfo(uId={0}, fleetId={1}), tSpan={2}",
                         userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetAllActiveVehiclesCfgInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = @"Retrieves vehicle maintenance information . 
                     XML File format :[BoxId],[VehicleId],[Description],[LastSrvOdo],[CurrentOdo],[MaxSrvInterval],
                                      [LastSrvEngHrs],[CurrentEngHrs],[EngHrsSrvInterval],[Email],[TimeZone],
                                      [DayLightSaving],[AutoAdjustDayLightSaving],[LicensePlate],[ModelYear],[MakeName],
                                      [ModelName],[NextServiceDescription],[VehicleTypeId]")]
        public int GetFleetMaintenanceInfoXML(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetFleetMaintenanceInfoXML(uId={0}, fleetId={1})", userId, fleetId);

                // By designe, this method allows user to see all fleets (even not ussigned to this user!!!) maintenace information.
                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsFleetInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsFleetInfo = dbFleet.GetFleetMaintenanceInfo(fleetId, userId);
                }

                if (Util.IsDataSetValid(dsFleetInfo))
                    xml = dsFleetInfo.GetXml();

                LogFinal("<< GetFleetMaintenanceInfoXML(uId={0}, fleetId={1}, tSpan={2})",
                           userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetMaintenanceInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = @"Retrieves vehicle maintenance information . 
                        XML File format :[VehicleId],[ServiceDateTime],[ServiceDescription],[ServiceOdo]")]
        public int GetFleetMaintenanceHistoryXML(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetFleetMaintenanceHistoryXML(uId={0}, fleetId={1})", userId, fleetId);

                // By designe, this method allows user to see all fleets (even not ussigned to this user!!!) maintenace information.
                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsFleetInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsFleetInfo = dbFleet.GetFleetMaintenanceHistory(userId, fleetId);
                }

                if (Util.IsDataSetValid(dsFleetInfo))
                    xml = dsFleetInfo.GetXml();

                LogFinal("<< GetFleetMaintenanceHistoryXML(uId={0}, fleetId={1}, tSpan={2})",
                       userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetMaintenanceHistoryXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///         Retrieves history information by Fleet ID, within a period of time 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fleetId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = @"Retrieves history information by Fleet ID.  
                     XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],
                                     [BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],
                                     [CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],
                                     [CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],
                                     [MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
        public int GetMessagesFromHistoryByFleetId(int userId, string SID, int fleetId, string fromDateTime,
                                                   string toDateTime, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetMessagesFromHistoryByFleetId( uId={0}, fleetId={1} )", userId, fleetId);

                // By designe, this method allows user to see all fleets (even not ussigned to this user!!!) maintenace information.
                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsFleetInfo = null;

                using (MessageQueue dbMsg = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    TimeSpan ts = Convert.ToDateTime(toDateTime) - Convert.ToDateTime(fromDateTime);
                    if (ts.TotalHours < 3)
                        dsFleetInfo = dbMsg.GetMessagesFromHistoryByFleetId(fleetId,
                                                                          Convert.ToDateTime(fromDateTime),
                                                                          Convert.ToDateTime(toDateTime));
                    else
                        dsFleetInfo = dbMsg.GetMessagesFromHistoryByFleetId(fleetId,
                                                                            Convert.ToDateTime(fromDateTime),
                                                                            Convert.ToDateTime(fromDateTime).AddHours(3));
                }

                if (Util.IsDataSetValid(dsFleetInfo))
                    xml = dsFleetInfo.GetXml();

                LogFinal("<< GetMessagesFromHistoryByFleetId( uId={0}, fleetId={1}, tSpan={2} )",
                                   userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMessagesFromHistoryByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///       Returns vehicle information by fleet id, filtered by feature mask
        /// </summary>
        /// <param name="fleetId">Fleet Id</param>
        /// <param name="featureMask">Feature mask</param>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],[VehicleTypeName],
        ///              [StateProvince],[ModelYear],[Color],[Description]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of vehicles for a particular fleet. 
                           XML file format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeName],[ModelName],
                                            [VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description]")]
        public int GetVehiclesInfoXMLByFleetIdFeatures(int userId, string SID, int fleetId,
                                                       long featureMask, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehiclesInfoXMLByFleetIdFeatures( uId={0}, fleetId={1}, mask={2} )",
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

                Log("<< GetVehiclesInfoXMLByFleetIdFeatures( uId={0}, fleetId={1}, mask={2}, tSpan={3} )",
                                    userId, fleetId, featureMask, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesInfoXMLByFleetIdFeatures : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

      
        /// <summary>
        ///          Returns vehicle information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>XML [LicensePlate],[BoxId],[VehicleId],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of vehicles for a particular fleet. 
                                  XML file format: [LicensePlate],[BoxId],[VehicleId],Description]")]
        public int GetVehiclesShortInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehiclesShortInfoXMLByFleetId( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehicles(fleetId, "Description");
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesShortInfoXMLByFleetId( uId={0}, fleetId={1}, tSpan={2} )",
                          userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesShortInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = @"Returns XML of vehicles for a particular fleet. 
                                  XML file format: [LicensePlate],[BoxId],[VehicleId],Description]")]
        public int GetVehiclesByFleetUser(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehiclesByFleetUser( uId={0}, fleetId={1} )", userId, fleetId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesByFleetUser(userId, fleetId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesByFleetUser( uId={0}, fleetId={1}, tSpan={2} )",
                          userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesByFleetUser : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region Fleet Interfaces
        /// <summary>
        ///         Delete existing fleet.
        /// </summary>
        /// <param name="fleetId"></param> 
        /// <returns>Rows Affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Delete existing Fleet.")]
        public int DeleteFleetByFleetId(int userId, string SID, int fleetId)
        {
            try
            {
                Log(">> DeleteFleetByFleetId( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                // TODO: 11 - > DeleteFleet
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 11);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleet",
                                                   string.Format("FleetId={0}", fleetId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Update fleet({0}) - Initial values", fleetId));

                    dbFleet.DeleteFleetByFleetId(fleetId);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleet",
                                                   string.Format("FleetId={0}", fleetId),
                                                   "Delete", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Delete fleet({0})", fleetId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteFleetByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///      Add new fleet.
        /// </summary>
        /// <param name="fleetName"></param>
        /// <param name="organizationId"></param>
        /// <param name="description"></param>
        /// <returns>int next fleet id</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if fleet alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
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

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleet",
                               string.Format("FleetId={0}", fleetId),
                               "Add", this.Context.Request.UserHostAddress,
                               this.Context.Request.RawUrl,
                               string.Format("Add new fleet({0})", fleetId));

                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        //// <summary>
        ///         Retrieves Fleet info
        /// </summary>
        /// <returns>XML [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]</returns>
        /// <param name="fleetId" param size = int (4 bytes)></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        [WebMethod(Description = @"Retrieves Fleet information.
                        XML File format: [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]")]
        public int GetFleetInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetFleetInfoXMLByFleetId( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                // TODO: 9 - > GetFleetInfo
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 9);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetFleetInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///            Update fleet information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fleetId"></param>
        /// <param name="fleetName"></param>
        /// <param name="organizationId"></param>
        /// <param name="description"></param>
        /// <returns></returns>
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
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleet",
                                                   string.Format("FleetId={0}", fleetId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Update fleet({0}) - Initial values", fleetId));

                    dbFleet.UpdateInfo(fleetId, fleetName, organizationId, description);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleet",
                                                   string.Format("FleetId={0}", fleetId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Update fleet({0})", fleetId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateFleetInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region Fleet/User Interfaces
        /// <summary>
        ///         Returns fleets information by user id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>XML [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of Fleets by User ID . 
                        XML [OrganizationName],[FleetId],[FleetName],[FleetDescription]")]
        public int GetFleetsInfoXMLByUserId(int userId, string SID, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetFleetsInfoXMLByUserId(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetFleetsInfoByUserId(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                LogFinal("<< GetFleetsInfoXMLByUserId(uId={0}, tSpan={1})", userId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoXMLByUserId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///       Returns fleets information by user id. 
        ///       it changes the name for "All vehicles" ??     
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>XML [OrganizationName],[FleetId],[FleetName],[FleetDescription]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>      
        [WebMethod(Description = @"Returns XML of Fleets by User ID. 
                              XML [OrganizationName],[FleetId],[FleetName],[FleetDescription]")]
        public int GetFleetsInfoXMLByUserIdByLang(int userId, string SID, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetFleetsInfoXMLByUserIdByLang( uId={0}, lang={1} )", userId, lang);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                // using( Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetFleetsInfoByUserId(userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = dsInfo.GetXml();

                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        Resources.Const.Culture = new System.Globalization.CultureInfo(lang);
                        xml = xml.Replace(VLF.CLS.Def.Const.defaultFleetName, Resources.Const.defaultFleetName);
                    }
                }

                LogFinal("<< GetFleetsInfoXMLByUserIdByLang( uId={0}, lang={1}, tSpan={2} )",
                         userId, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoXMLByUserIdByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///      Returns all users info by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>XML [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],
        ///              [ContactInfo],[OrganizationId]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = @"Returns XML of users assigned to fleet. 
                  XML file format: [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],
                                   [ContactInfo],[OrganizationId]")]
        public int GetUsersInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetUsersInfoXMLByFleetId( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                // TODO: 15 - > ViewFleetUsers
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 15);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetUsersInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        /// Retieves all users unassigned to the fleet
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fleetId"></param>
        /// <returns>XML [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],[ContactInfo],[OrganizationId]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of users , that not assigned to specific fleet. XML file format: [UserId],[UserName],[Password],[DriverLicense],[FirstName],[LastName],[ContactInfo],[OrganizationId]")]
        public int GetUnassignedUsersInfoXMLByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetUnassignedUsersInfoXMLByFleetId( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                // Authorize
                // TODO: 15 - > ViewFleetUsers
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 15);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetUnassignedUsersInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUnassignedUsersInfoXMLByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        /// Get MdtOTA process status. 
        /// </summary>
        /// <param name="fleetid"></param>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Returns XML of Mdts based on FleetId")]
        public int GetMdtsByFleetId(int userId, string SID, int fleetid, Int16 typeId, ref string xml)
        {
            try
            {
                Log(">> GetMdtsByFleetId( uId={0}, fleetid={1} )", userId, fleetid);

                if (!ValidateUserFleet(userId, SID, fleetid))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsFleetsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsFleetsInfo = dbFleet.GetMdtsByFleetId(fleetid, typeId);
                }

                if (Util.IsDataSetValid(dsFleetsInfo))
                    xml = dsFleetsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMdtsByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Assign a user to fleet")]
        public int AddUserToFleet(int userId, string SID, int fleetId, int userIdAddToFleet)
        {
            try
            {
                Log(">> AddUserToFleet(uId={0}, fleetId={1}, userIdAddToFleet={2} )", userId, fleetId, userIdAddToFleet);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                // Authorize
                // TODO: 13 - > AddUserToFleet
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 13);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.AddUserToFleet(fleetId, userIdAddToFleet);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetUsers",
                                                  string.Format("FleetId={0} AND UserId={1}", fleetId, userIdAddToFleet),
                                                  "Add", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Add new user to fleet({0} and UserId({1}))", fleetId, userIdAddToFleet));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserToFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Remove a user from fleet")]
        public int DeleteUserFromFleet(int userId, string SID, int fleetId, int userIdDeleteFromFleet)
        {
            try
            {
                Log(">> DeleteUserFromFleet( uId={0}, fleetId={1}, userIdDeleteFromFleet={2} )",
                        userId, fleetId, userIdDeleteFromFleet);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                // TODO: 14 - > DeleteUserFromFleet
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 14);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetUsers",
                                                  string.Format("FleetId={0} AND UserId={1}", fleetId, userIdDeleteFromFleet),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update user({1}) from fleet({0}) - Initial values", fleetId, userIdDeleteFromFleet));

                    dbFleet.DeleteUserFromFleet(fleetId, userIdDeleteFromFleet);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetUsers",
                                                  string.Format("FleetId={0} AND UserId={1}", fleetId, userIdDeleteFromFleet),
                                                  "Delete", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete user({1}) from fleet({0})", fleetId, userIdDeleteFromFleet));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteUserFromFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod]
        public int GetActivitySummaryPerVehicle(Int32 userId, string SID, int FleetId, string FromDate, string ToDate, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {

                Log(">> GetActivitySummaryPerVehicle(uId={0}, dtFrom={1}, dtTo={2}, FleetId={3})",
                                  userId, FromDate, ToDate, FleetId);


                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet ds = new DataSet();


                using (VLF.DAS.Logic.Report rpt = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(userId)))
                {
                    ds = rpt.GetActivitySummaryReportPerVehicle(userId, FleetId, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), 3);
                    ds.Tables[0].TableName = "ActivitySummaryReportPerBox";
                }

                xml = "";

                if (!Util.IsDataSetValid(ds))
                    return (int)InterfaceError.NoError;
                else
                    xml = ds.GetXml();



                LogFinal("<< GetActivitySummaryPerVehicle(uId={0}, dtFrom={1}, dtTo={2}, FleetId={3}, tSpan={4})",
                                 userId, FromDate, ToDate, FleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetActivitySummaryPerVehicle: uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int GetStateMileagePerFleet(Int32 userId, string SID, int FleetId,
                                           string FromDate, string ToDate, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetStateMileagePerFleet(uId={0}, dtFrom={1}, dtTo={2}, FleetId={3})",
                                  userId, FromDate, ToDate, FleetId);
                // Authenticate 
                if (!ValidateUserFleet(userId, SID, FleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet ds = new DataSet();

                using (VLF.DAS.Logic.Report rpt = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(userId)))
                {
                    ds = rpt.GetStateMileagePerFleet(FleetId, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), 3);
                    ds.Tables[0].TableName = "StateMileageReportPerFleet";
                }

                if (!Util.IsDataSetValid(ds))
                {
                    xml = "";
                    return (int)InterfaceError.NoError;
                }
                else
                {
                    xml = ds.GetXml();
                }

                LogFinal("<< GetStateMileagePerFleet(uId={0}, dtFrom={1}, dtTo={2}, FleetId={3}, tSpan={4})",
                                  userId, FromDate, ToDate, FleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetStateMileagePerFleet: uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        #endregion

        #region Fleet Emails Interfaces
        /// <summary>
        /// Add email to fleet.
        /// </summary>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="fleetId"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if email alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Add a email address to fleet. This email will be notify in case of alarm")]
        public int AddEmail(int userId, string SID, int fleetId, string email, string phone, short timeZone,
              short dayLightSaving, short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance)
        {
            try
            {
                Log(">> AddEmail( uId={0}, fleetId={1}, email={2}, timeZone={3}, dayLightSaving={4}, formatType={5}, notify={6}, warning={7}, critical={8}, autoAdjustDayLightSaving={9} )",
                  userId, fleetId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.AddEmail(fleetId, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                                   string.Format("FleetId={0} AND Email='{1}'", fleetId, email),
                                                   "Add", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl, email);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddEmail : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #region New Fleet Emails Interfaces for SaveEmail with new column DriverMessage
        /// <summary>
        /// Add email to fleet.
        /// </summary>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="fleetId"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if email alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Add a email address to fleet. This email will be notify in case of alarm")]
        public int SaveEmail(int userId, string SID, int fleetId, string email, string phone, short timeZone,
              short dayLightSaving, short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool DriverMessage, bool autosubscription, bool reminder)
        {
            try
            {
                Log(">> AddEmail( uId={0}, fleetId={1}, email={2}, timeZone={3}, dayLightSaving={4}, formatType={5}, notify={6}, warning={7}, critical={8}, drivermessage={9},autoAdjustDayLightSaving={10} )",
                  userId, fleetId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, DriverMessage, autoAdjustDayLightSaving);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.SaveEmail(fleetId, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance, DriverMessage, autosubscription, reminder);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                                   string.Format("FleetId={0} AND Email='{1}'", fleetId, email),
                                                   "Add", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl, email);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddEmail : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion


        [WebMethod(Description = "Modify a email address. This email will be notify in case of alarm")]
        public int ModifyEmail(int userId, string SID, int fleetId, string oldEmail, string newEmail, string phone, short timeZone,
            short dayLightSaving, short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool DriverMessage, bool autosubscription, bool reminder)
        {
            try
            {
                Log(">> UpdateEmail( uId={0}, fleetId={1}, oldEmail={2}, newEmail={3}, timeZone={4}, dayLightSaving={5}, formatType={6}, notify={7}, warning={8}, critical={9}, drivermessage={10}, autoAdjustDayLightSaving={11}, autosubscription={12}, reminder={13} )",
                  userId, fleetId, oldEmail, newEmail, timeZone, dayLightSaving, formatType, notify, warning, critical, DriverMessage, autoAdjustDayLightSaving, autosubscription, reminder);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetEmails",
                                                  string.Format("FleetId={0} AND Email='{1}'", fleetId, oldEmail),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update email for fleet({0}), email({1}) to new email({2}) - Initial values",
                                                                fleetId, oldEmail, newEmail));

                    dbFleet.ModifyEmail(fleetId, oldEmail, newEmail, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance, DriverMessage, autosubscription, reminder);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                                  string.Format("FleetId={0} AND Email='{1}'", fleetId, oldEmail),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update email for fleet({0}), email({1}) to new email({2})",
                                                                fleetId, oldEmail, newEmail));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateEmail : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        /// Update fleet email.
        /// </summary>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="fleetId"></param>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <returns>void</returns>
        /// <exception cref="DASAppViolatedIntegrityConstraintsException">Thrown if fleet does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Update a email address. This email will be notify in case of alarm")]
        public int UpdateEmail(int userId, string SID, int fleetId, string oldEmail, string newEmail, string phone, short timeZone,
            short dayLightSaving, short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool Maintenance, bool autosubscription, bool reminder)
        {
            try
            {
                Log(">> UpdateEmail( uId={0}, fleetId={1}, oldEmail={2}, newEmail={3}, timeZone={4}, dayLightSaving={5}, formatType={6}, notify={7}, warning={8}, critical={9}, autoAdjustDayLightSaving={10}, autosubscription={11}, reminder={12} )",
                  userId, fleetId, oldEmail, newEmail, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, autosubscription, reminder);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetEmails",
                                                  string.Format("FleetId={0} AND Email='{1}'", fleetId, oldEmail),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update email for fleet({0}), email({1}) to new email({2}) - Initial values",
                                                                fleetId, oldEmail, newEmail));

                    dbFleet.UpdateEmail(fleetId, oldEmail, newEmail, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, Maintenance, autosubscription, reminder);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                                  string.Format("FleetId={0} AND Email='{1}'", fleetId, oldEmail),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update email for fleet({0}), email({1}) to new email({2})",
                                                                fleetId, oldEmail, newEmail));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateEmail : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        /// Delete existing email from fleet.
        /// </summary>
        /// <returns>rows affected</returns>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="fleetId"></param> 
        /// <param name="email"></param> 
        /// <exception cref="DASAppResultNotFoundException">Thrown if fleet does not exist</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Remove a email address from fleet.")]
        public int DeleteEmailFromFleet(int userId, string SID, int fleetId, string email)
        {
            try
            {
                Log(">> DeleteEmailFromFleet( uId={0}, fleetId={1}, email={2} )", userId, fleetId, email);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetEmails",
                                                  string.Format("FleetId={0} AND Email='{1}'", fleetId, email),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl, string.Format("Update email({1}) for fleet {0} - Initial values", fleetId, email));

                    dbFleet.DeleteEmailFromFleet(fleetId, email);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                                  string.Format("FleetId={0} AND Email='{1}'", fleetId, email),
                                                  "Delete", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl, string.Format("Delete email({1}) for fleet {0}", fleetId, email));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteEmailFromFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        /// Delete all emails from fleet.
        /// </summary>
        /// <returns>rows affected</returns>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="fleetId"></param> 
        /// <exception cref="DASAppResultNotFoundException">Thrown if fleet id does not exist</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        [WebMethod(Description = "Delete all email addresses from fleet.")]
        public int DeleteAllEmailsFromFleet(int userId, string SID, int fleetId)
        {
            try
            {
                Log(">> DeleteAllEmailsFromFleet( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetEmails",
                                                   string.Format("FleetId={0}", fleetId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl, string.Format("Update all emails for fleet {0} - Initial values", fleetId));

                    dbFleet.DeleteAllEmailsFromFleet(fleetId);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                                   string.Format("FleetId={0}", fleetId),
                                                   "Delete", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl, string.Format("Delete all emails for fleet {0}", fleetId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteAllEmailsFromFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        //// <summary>
        /// Retrieves fleet emails
        /// </summary>
        /// <returns>XML [FleetId],[FleetMame],[Email],[TimeZone],
        /// [DayLightSaving],[FormatType],[Notify],[Warning],[Critical]</returns>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="fleetId" param></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        [WebMethod(Description = "Returns XML of all email addresses assigned to fleet. XML File format:XML [FleetId],[FleetMame],[Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical]")]
        public int GetFleetEmailsXML(int userId, string SID, int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetFleetEmailsXML( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbFleet.GetFleetEmails(fleetId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetEmailsXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Set Daylight Saving adjustment to the notification email")]
        public int SetFleetAutoAdjustDayLightSaving(int userId, string SID, int fleetId, string email, bool autoAdjustDayLightSaving, bool dayLightSaving)
        {
            try
            {
                Log(">> SetFleetAutoAdjustDayLightSaving( fleetId={0}, email='{1}', autoAdjustDayLightSaving={2}, dayLightSaving={3} )",
                        fleetId, email, autoAdjustDayLightSaving, dayLightSaving);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Fleet", userId, 0, "vlfFleetEmails",
                                               string.Format("FleetId={0} AND Email='{1}'", fleetId, email),
                                               "Update", this.Context.Request.UserHostAddress,
                                               this.Context.Request.RawUrl,
                                               string.Format("Create daylight saving for fleet({0}), email({1}) - Initial values",
                                                             fleetId, email));

                    dbSystem.SetFleetAutoAdjustDayLightSaving(fleetId, email, autoAdjustDayLightSaving, dayLightSaving);

                    LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetEmails",
                                               string.Format("FleetId={0} AND Email='{1}'", fleetId, email),
                                               "Update", this.Context.Request.UserHostAddress,
                                               this.Context.Request.RawUrl,
                                               string.Format("Create daylight saving for fleet({0}), email({1})",
                                                             fleetId, email));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetFleetAutoAdjustDayLightSaving : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Fleet All Maintenance Plans")]
        public int FleetMaintenancePlan_GetAll(int userId, string SID, int organizationId, int fleetId, short dueFlag, ref string xmlString)
        {
            try
            {
                Log(">> FleetMaintenancePlan_GetAll(uId={0}, orgId={1}, fleetId={2})", userId, organizationId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbFleet.GetFleetServices(fleetId, dueFlag, userId);
                    if (dtResult != null && dtResult.Rows.Count > 0)
                        xmlString = Util.Table2Xml(dtResult, true, "Service");
                    //if (Util.IsDataSetValid(dsResult))
                    //{
                    //   dsResult.DataSetName = "VehicleMaintenance";
                    //   dsResult.Tables[0].TableName = "MaintenancePlans";
                    //   xmlString = dsResult.GetXml();
                    //}
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< FleetMaintenancePlan_GetAll : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Fleet All Maintenance Plans")]
        public int FleetMaintenancePlan_GetAllByLang(int userId, string SID, int organizationId, int fleetId, short dueFlag, string lang, ref string xmlString)
        {
            try
            {
                Log(">> FleetMaintenancePlan_GetAllByLang(uId={0}, orgId={1}, fleetId={2})", userId, organizationId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbFleet.GetFleetServices(fleetId, dueFlag, userId);
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        DataSet dsResult = new DataSet();
                        DataTable dt = dtResult.Copy();
                        dt.TableName = "Service";
                        dsResult.Tables.Add(dt);
                        if (ASIErrorCheck.IsLangSupported(lang))
                        {
                            LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                            dbl.LocalizationData(lang, "OperationTypeId", "OperationTypeDescription", "ServiceOperationType", ref dsResult);
                        }

                        xmlString = dsResult.GetXml();
                        //xmlString = Util.Table2Xml(dtResult, true, "Service");

                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< FleetMaintenancePlan_GetAllByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        [WebMethod(Description = "Get Fleet's Maintenance Plans By Type")]
        public int FleetMaintenancePlan_GetByType(int userId, string SID, int organizationId, int fleetId, short operationType, short dueFlag, ref string xmlString)
        {
            try
            {
                Log(">> FleetMaintenancePlan_GetByType(uId={0}, orgId={1}, fleetId={2})", userId, organizationId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbFleet.GetFleetServices(fleetId, operationType, userId);
                    if (dtResult != null && dtResult.Rows.Count > 0)
                        xmlString = Util.Table2Xml(dtResult, true, "Service");
                    //if (Util.IsDataSetValid(dsResult))
                    //{
                    //   dsResult.DataSetName = "VehicleMaintenance";
                    //   dsResult.Tables[0].TableName = "MaintenancePlans";
                    //   xmlString = dsResult.GetXml();
                    //}
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< FleetMaintenancePlan_GetByType : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Fleet's Maintenance Plans By Type")]
        public int FleetMaintenancePlan_GetByTypeLang(int userId, string SID, int organizationId, int fleetId, short operationType, short dueFlag, string lang, ref string xmlString)
        {
            try
            {
                Log(">> FleetMaintenancePlan_GetByTypeLang(uId={0}, orgId={1}, fleetId={2})", userId, organizationId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbFleet.GetFleetServices(fleetId, operationType, userId);


                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        DataSet dsResult = new DataSet();
                        DataTable dt = dtResult.Copy();
                        dt.TableName = "Service";
                        dsResult.Tables.Add(dt);
                        if (ASIErrorCheck.IsLangSupported(lang))
                        {
                            LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                            dbl.LocalizationData(lang, "OperationTypeId", "OperationTypeDescription", "ServiceOperationType", ref dsResult);
                        }

                        xmlString = dsResult.GetXml();
                        //xmlString = Util.Table2Xml(dtResult, true, "Service");

                    }
                    else
                        xmlString = "";



                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< FleetMaintenancePlan_GetByTypeLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Fleet Service History")]
        public int FleetGetServicesHistory(int userId, string SID, int fleetId, ref string xmlString)
        {
            try
            {
                Log(">> FleetGetServicesHistory (uId={0}, fleetId={1})", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbFleet.GetFleetServicesHistory(fleetId, userId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "ServicesHistory";
                        dsResult.Tables[0].TableName = "Service";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< FleetGetServicesHistory : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        [WebMethod(Description = "Add New Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlan_AddToFleet(int userId, string SID, int organizationId, int fleetId, int serviceId,
           short operationTypeId, int notificationId, short frequency, int serviceValue, int serviceInterval, int endValue,
           string email, string description, string comments)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_AddToFleet(uid={0}, orgId={1}, serv.Id={2}, descr.={3})",
                                                       userId, organizationId, serviceId, description);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AddServiceToFleet(userId, fleetId, serviceId, operationTypeId, notificationId, frequency,
                                                serviceValue, serviceInterval, endValue, email, description, comments);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_AddToFleet : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        #endregion

        // Changes for TimeZone Feature start

        /// <summary>
        /// Returns vehicles last known position information by vehicles list xml. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesChangedPositionInfoByLang_NewTZ(int userId, string SID, int fleetId, string language, DateTime lastChecked, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesChangedPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetId);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByLang_NewTZ(fleetId, userId, language, lastChecked);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1}, tSpan={2} )",
                               userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesChangedPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature end
        /// <summary>
        /// Returns vehicles last known position information by vehicles list xml. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesChangedPositionInfoByLang(int userId, string SID, int fleetId, string language, DateTime lastChecked, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesChangedPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetId);

                // if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByLang(fleetId, userId, language, lastChecked);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetId={1}, tSpan={2} )",
                               userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesChangedPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Returns vehicles last known position information by vehicles list xml. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesChangedPositionInfoByMultipleFleetsByLang(int userId, string SID, string fleetIds, string language, DateTime lastChecked, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesChangedPositionInfoByLang( uId={0}, fleetId={1} )", userId, fleetIds);

                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesChangedPositionInfoByMultipleFleetsByLang(fleetIds, userId, language, lastChecked);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoByLang( uId={0}, fleetIds={1}, tSpan={2} )",
                               userId, fleetIds, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesChangedPositionInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        /// <summary>
        /// Return the list of organization skills based on organization id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns>
        /// int        
        /// </returns>
        [WebMethod(Description = "Returns XML of organization skills based on organization id")]
        public int GetOrganizationSkills(int userId, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationSkills( organizationId={0}", organizationId);

                // if (!ValidateUserFleet(userId, SID, fleetid))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsFleetsInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsFleetsInfo = dbFleet.GetOrganizationSkillsList(organizationId);
                }

                if (Util.IsDataSetValid(dsFleetsInfo))
                    xml = dsFleetsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMdtsByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///            returns vehicle found in proximity of a given location
        /// </summary>
        /// <comment>
        ///         GB -> this could be done completely in a store procedure
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="fleetId"></param>
        /// <param name="distance"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="skillId"></param>
        /// <param name="vehicleTypeId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <comment>
        ///         TO REEXAMINE : the operation can be executed on the server side 
        /// </comment>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Landmark Name , distance from Landmark and Fleet ID, driver's information. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[Description]")]
        public int GetVehiclesLastKnownPositionInfoWithDriversInfoNearestToLatLon(int userId, string SID, int organizationId,
                                                  int fleetId, int distance, double lat, double lon, int skillId, int vehicleTypeId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">>GetVehiclesLastKnownPositionInfoNearestToLatLon (uId={0}, orgId={1}, fleetId={2}, distance={3}, ({4},{5}))",
                        userId, organizationId, fleetId, distance, lat, lon);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    // 1. Retrieve fleet last known position
                    DataSet dsFleetLocation = dbFleet.GetVehiclesLastKnownPositionDriverInfoBySkill(fleetId, userId, "en", skillId, vehicleTypeId);
                    if (Util.IsDataSetValid(dsFleetLocation))
                    {
                        // 2. Retrieve location (lat,lon)
                        VLF.MAP.GeoPoint searchPosition = new VLF.MAP.GeoPoint(lat, lon);
                        VLF.MAP.GeoPoint currVehicleCoord = new VLF.MAP.GeoPoint();

                        int distanceBetweenGPS = 0;

                        #region Create map engine instance

                        VLF.MAP.ClientMapProxy map = null;
                        using (SystemConfig qSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                        {
                            DataSet dsGeoCodeInfo = qSystem.GetUserGeoCodeEngineInfo(userId);
                            if (Util.IsDataSetValid(dsGeoCodeInfo))
                                map = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));
                            else
                                throw new ASIDataNotFoundException("Unable to retrieve map engine info by user=" + userId);
                        }

                        #endregion

                        // 4. Filter vehicles located near to landmark
                        dsFleetLocation.Tables[0].Columns.Add("GPSDistance");
                        foreach (DataRow ittr in dsFleetLocation.Tables[0].Rows)
                        {
                            currVehicleCoord.Latitude = Convert.ToDouble(ittr["Latitude"]);
                            currVehicleCoord.Longitude = Convert.ToDouble(ittr["Longitude"]);
                            // 5. Calculates distance between landmark and current location
                            distanceBetweenGPS = (int)(Math.Round(map.GetDistance(searchPosition, currVehicleCoord)));
                            if (distance > 0)
                            {
                                if ((distanceBetweenGPS <= VLF.CLS.Def.Const.unassignedIntValue) ||
                                    (distanceBetweenGPS > distance))
                                {
                                    ittr.Delete();
                                }
                            }
                            ittr["GPSDistance"] = distanceBetweenGPS;
                        }

                        xml = dsFleetLocation.GetXml();
                    }
                }

                LogFinal("<< GetVehiclesLastKnownPositionInfoNearestToLatLon (uId={0}, orgId={1}, fleetId={2}, distance={3}, ({4},{5}), tSpan={6})",
                       userId, organizationId, fleetId, distance, lat, lon, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoNearestToLatLon : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // SALMAN Feb 19, 2013
        /// <summary>
        /// Returns vehicles last position information by Hierarchy, Fleet, UserName (UDF1 like abc@bsm.com). 
        /// </summary>
        /// <param name="filterHierarchy"></param>
        /// <returns>
        /// XML File Format: 
        /// [VehicleId],[Latitude],[Longitude],[LastCommunicatedDateTime],
        /// [CustomOutputFields] (like Hierarchy, Field1, Field2)
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position custom information by Hierarchy, Fleet, UserName (UDF1 like abc@bsm.com). XML file format: [VehicleId],[Latitude],[Longitude],[LastCommunicatedDateTime],[CustomOutputFields] (like Hierarchy, Field1, Field2)")]
        public int GetVehiclesLastKnownPositionCustomInfo(int userId, string SID, string filterHierarchy, string filterFleet, string filterUDF1, string filterCustomOutputFields, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Stopwatch watch = new Stopwatch();
                Log(">> GetVehiclesLastKnownPositionCustomInfo(uId={0}, HierarchyId={1})", userId, filterHierarchy);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //if (!ValidateUserHierarchy(userId, SID)) ?????????????????????????????????????????????????????
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    watch.Reset();
                    watch.Start();
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionCustomInfo(userId, filterHierarchy, filterFleet, filterUDF1, filterCustomOutputFields);
                    watch.Stop();
                    Log("-- GetVehiclesLastKnownPositionCustomInfo : duration={0}", watch.Elapsed.TotalSeconds);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionCustomInfo(uId={0}, HierarchyId={1}, tSpan={2}", userId, filterHierarchy,
                    DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionCustomInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // SALMAN Jul 19, 2013
        /// <summary>
        /// Returns devices last position information by selection criteria.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="selectionCriterea"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of devices with last known position information by selection criteria. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetDevicesLastKnownPosition(int userId, string SID, string selectionCriteria, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Stopwatch watch = new Stopwatch();
                Log(">> GetDevicesLastKnownPosition(uId={0}, SelectionCriteria={1})", userId, selectionCriteria);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //if (!ValidateUserHierarchy(userId, SID)) ???????????????????????????????????????????????
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    watch.Reset();
                    watch.Start();
                    dsVehiclesInfo = dbFleet.GetDevicesLastKnownPosition(userId, selectionCriteria);
                    watch.Stop();
                    Log("-- GetDevicesLastKnownPosition : duration={0}", watch.Elapsed.TotalSeconds);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetDevicesLastKnownPosition(uId={0}, SelectionCriteria={1}, tSpan={2}", userId, selectionCriteria,
                    DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDevicesLastKnownPosition : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Gene Aug 15, 2013
        /// <summary>
        /// Returns vehicles last known position information by list of fleet ids or list of vehicle ids
        /// or list of box ids or list of vehicle descriptions. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// FleetId, LicensePlate, VehicleId, BoxId, LastValidDateTime, Latitude, Longitude, Speed, Heading, StreetAddress
        /// SensorMask, BoxArmed, GeoFenceEnabled, IconTypeName, VehicleTypeName
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position optional information by <Filter>FleetIDs:{};VehicleIDs:{};BoxIDs:{};VehicleDescriptions:{};OptionalOutputFields:{}</Filter>. XML Format: [FleetId], [LicensePlate], [VehicleId], [BoxId], [LastValidDateTime], [Latitude], [Longitude], [Speed], [Heading], [StreetAddress], [SensorMask], [BoxArmed], [GeoFenceEnabled], [IconTypeName], [VehicleTypeName]")]
        public int GetSelectedVehiclesLastKnownPositionInfo(int userId, string SID, string sFleetIDs, string sVehicleIDs, string sBoxIDs, string sVehicleDescriptions, string sOptionalOutputFields, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Stopwatch watch = new Stopwatch();
                Log(">> GetSelectedVehiclesLastKnownPositionInfo(uId={0}, FleetIDs={1}, VehicleIDs={2}, BoxIDs={3}, VehicleDescriptions={4}, OptionalOutputFields={5})", userId, sFleetIDs, sVehicleIDs, sBoxIDs, sVehicleDescriptions, sOptionalOutputFields);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    watch.Reset();
                    watch.Start();
                    dsVehiclesInfo = dbFleet.GetSelectedVehiclesLastKnownPositionInfo(userId, sFleetIDs, sVehicleIDs, sBoxIDs, sVehicleDescriptions, sOptionalOutputFields);
                    watch.Stop();
                    Log("-- GetSelectedVehiclesLastKnownPositionInfo : duration={0}", watch.Elapsed.TotalSeconds);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetSelectedVehiclesLastKnownPositionInfo(uId={0}, FleetIDs={1}, VehicleIDs={2}, BoxIDs={3}, VehicleDescriptions={4}, OptionalOutputFields={5}, tSpan={6}",
                    userId, sFleetIDs, sVehicleIDs, sBoxIDs, sVehicleDescriptions, sOptionalOutputFields, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetSelectedVehiclesLastKnownPositionInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #region Peripherals
        /// <summary>
        /// Returns Box-Peripheral information by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>      
        [WebMethod(Description = "Returns XML info for Box-Peripheral information by fleet id")]
        public int GetPeripheralsInfoByFleetId(int userId, string SID, int fleetId, ref string xml)
        {
            try
            {
                Log(">> GetPeripheralsInfoByFleetId(uId={0}, fleetId={1})", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetPeripheralsInfoByFleetId(fleetId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetPeripheralsInfoByFleetId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///         Returns vehicles last known position information with peripheral by fleet id. 
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description],
        /// [IconTypeId],[IconTypeName],[VehicleTypeName],
        /// [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed],[FwTypeId],[Dormant],[DormantDateTime]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns XML of vehicles with last known position information by Fleet ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehiclesLastKnownPositionInfoWithPeripheral(int userId, string SID, int fleetId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfoWithPeripheral( uId={0}, fleetId={1} )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehiclesInfo = null;
                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbFleet.GetVehiclesLastKnownPositionInfoWithPeripheral(fleetId, userId);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehiclesLastKnownPositionInfoWithPeripheral( uId={0}, fleetId={1}, tSpan={2} )",
                            userId, fleetId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfoWithPeripheral : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        // Salman May 27, 2014
        public int GetDevicesLastKnownPositionPreTemplatedInfo(int userId, DateTime lastCommunicatedDateTime, ref string xml)
        {
            int fleetId = GetAllvehicleFleetIdWithoutSID(userId);

            if (fleetId < 0)
            {
                LogException("<< GetVehiclesLastKnownPositionInfo : uId={0}, EXC=FleetId found {1}.", userId, fleetId);
                return (int)InterfaceError.NotFound;
            }

            DateTime dtNow = DateTime.Now;
            try
            {
                Stopwatch watch = new Stopwatch();
                Log(">> GetDevicesLastKnownPositionPreTemplatedInfo(uId={0}, fleetId={1})", userId, fleetId);

                DataSet dsDevicesInfo = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
                using (Fleet dbFleet = new Fleet(sConnectionString))
                {
                    watch.Reset();
                    watch.Start();
                    dsDevicesInfo = dbFleet.GetDeviceLastKnownPositionPreTemplatedInfo_SP(userId, fleetId, lastCommunicatedDateTime);
                    watch.Stop();
                    Log("-- GetDevicesLastKnownPositionPreTemplatedInfo : duration={0}", watch.Elapsed.TotalSeconds);
                }

                if (Util.IsDataSetValid(dsDevicesInfo))
                    xml = dsDevicesInfo.GetXml();

                LogFinal("<< GetDevicesLastKnownPositionPreTemplatedInfo(uId={0}, fleetId={1}, tSpan={2}", userId, fleetId,
                    DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDevicesLastKnownPositionPreTemplatedInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        int GetAllvehicleFleetIdWithoutSID(int userId)
        {
            //FleetId/FleetName: 19698/All Vehicles
            int fleetId = -1;

            try
            {
                switch (userId)
                {
                    case 22908: //MNR (userId coming with pwd and no SID)
                        fleetId = 19698;
                        break;
                    default:
                        break;
                }
            }
            catch { fleetId = -1; }

            return fleetId;
        }

        [WebMethod(Description = "Assign All vehicle in excel to fleet.")]
        public int AddAllVehicleToFleet(int userId, string SID, string result, int fleetId, int organizationId)
        {
            try
            {
                Log(">> AddAllVehicleToFleet(uId={0}, fleetId={1}, AllvehicleId={2})", userId, fleetId, result);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dbFleet.AddAllVehicleToFleet(fleetId, result, organizationId);

                    //LoggerManager.RecordUserAction("Fleet", userId, 0, "vlfFleetVehicles",
                    //                            string.Format("FleetId={0} AND VehicleId={1}", fleetId, vehicleId),
                    //                            "Add", this.Context.Request.UserHostAddress,
                    //                            this.Context.Request.RawUrl,
                    //                            string.Format("Add vehicle({1}) to fleet({0})", fleetId, vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddAllVehicleToFleet : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
    }
}
