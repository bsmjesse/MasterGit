using System;
using System.Web;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using VLF.DAS.Logic;
using VLF.ERRSecurity;
using VLF.CLS.Def.Structures;

namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "http://www.sentinelfm.com")]


    public class DBBox : System.Web.Services.WebService
    {

        public DBBox()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <returns></returns>
        private bool ValidateUserBox(int userId, string SID, int boxId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserBox(userId, boxId);
            }
        }

        #endregion




        #region Firmware info interface


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="firmwareTypeId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="firmwareId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="firmwareId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="hwTypeId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="profileId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="hwTypeId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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
        
        #endregion

    }

}