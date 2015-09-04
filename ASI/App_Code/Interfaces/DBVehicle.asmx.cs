/**
 * \comment    keywords for logs, which I try to downsize
 *             uid      -> userId
 *             orgId    -> organizationID
 *             boxId    -> boxId
 *             vId      -> vehicleId
 *             LP       -> licensePlate
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using System.IO;
using VLF.DAS.Logic;
using VLF.CLS;
using System.Text;
using VLF.CLS.Def.Structures;

namespace VLF.ASI.Interfaces
{

    [WebService(Namespace = "http://www.sentinelfm.com")]

    /// <summary>
    /// Summary description for DBVehicle.
    /// </summary>
    public class DBVehicle : System.Web.Services.WebService
    {
        public DBVehicle()
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


        private bool ValidateUserVehicle(int userId, string SID, Int64 vehicleId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserVehicle(userId, vehicleId);
            }
        }


        private bool ValidateUserLicensePlate(int userId, string SID, string licensePlate)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserLicensePlate(userId, licensePlate);
            }
        }


        private bool ValidateUserBox(int userId, string SID, int boxId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);
            //return true;

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserBox(userId, boxId);
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

        #endregion refactored functions

        #region Vehicle Information Interfaces
        // Changes for TimeZone Feature start
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle information by License Plate. XML File format :[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color], [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]")]
        public int GetVehicleInfoXML_NewTZ(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">> GetVehicleInfoXML(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleInfoByLicensePlate_NewTZ(licensePlate);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle information by License Plate. XML File format :[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color], [Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName]")]
        public int GetVehicleInfoXML(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">> GetVehicleInfoXML(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleInfoByLicensePlate(licensePlate);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle additional information by vehicle Id. XML File format :[VehicleId],[Field1],[Field2],[Field3],[Field4]")]
        public int GetVehicleAdditionalInfoXML(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleAdditionalInfoXML(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleAdditionalInfo(vehicleId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleAdditionalInfoXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle additional information for 3rd party by vehicle Id. XML File format :[VehicleId],[Equip#],[EquipCat],[Dates],[Readings]")]
        public int Get3rdPartyVehicleAdditionalInfoXML(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">>GetVehicle3rdPartyAdditionalInfoXML(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.Get3rdPartyVehicleAdditionalInfo(vehicleId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicle3rdPartyAdditionalInfoXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle working hours by vehicle Id. XML File format :[VehicleId],[WeekdayStart],[WeekdayEnd],[WeekendStart],[WeekendEnd]")]
        public int GetVehicleWorkingHoursXML(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetVehicleWorkingHoursXML(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleWorkingHours(vehicleId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleWorkingHoursXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get last odometer by Box ID.")]
        public int GetLastOdometerFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">> GetLastOdometerFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.Odometer);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastOdometerFromHistoryByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get last EEPROM DATA by Box ID.")]
        public int GetLastEEPROMDataFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastOdometerFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.SendEEPROMData);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastOdometerFromHistoryByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xmlResult"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle information by Vehicle ID. XML File format:[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]")]
        public int GetVehicleInfoXMLByVehicleId_NewTZ(int userId, string SID, Int64 vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">>GetVehicleInfoXMLByVehicleId(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleInfoByVehicleId_NewTZ(vehicleId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xmlResult = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xmlResult"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle information by Vehicle ID. XML File format:[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]")]
        public int GetVehicleInfoXMLByVehicleId(int userId, string SID, Int64 vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">>GetVehicleInfoXMLByVehicleId(uid={0}, vId={1})", userId, vehicleId);

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
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="BoxId"></param>
        /// <param name="xmlResult"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves vehicle information by BoxId ID. XML File format:[LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[OrganizationId],[IconTypeId],[IconTypeName],[Email]")]
        public int GetVehicleInfoXMLByBoxId(int userId, string SID, int BoxId, ref string xmlResult)
        {
            try
            {
                Log(">> GetVehicleInfoXMLByBoxId(uid={0}, boxId={1})", userId, BoxId);

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
                LogException("<< GetVehicleInfoXMLByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="oldLicensePlate"></param>
        /// <param name="newLicensePlate"></param>
        /// <param name="color"></param>
        /// <param name="costPerMile"></param>
        /// <param name="description"></param>
        /// <param name="makeModelId"></param>
        /// <param name="modelYear"></param>
        /// <param name="stateProvince"></param>
        /// <param name="vehicleTypeId"></param>
        /// <param name="vinNum"></param>
        /// <param name="vehicleId"></param>
        /// <param name="oldBoxId"></param>
        /// <param name="newBoxId"></param>
        /// <param name="iconTypeId"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a vehicle information.")]
        public int UpdateVehicleInfo_NewTZ(int userId, string SID, string oldLicensePlate,
                                    string newLicensePlate,
                                    string color, double costPerMile,
                                    string description, int makeModelId,
                                    short modelYear, string stateProvince,
                                    short vehicleTypeId, string vinNum,
                                    Int64 vehicleId, int oldBoxId, int newBoxId, short iconTypeId,
                                    string email, string phone, float timeZone, bool dayLightSaving, short formatType,
                                    bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool maintenance, int ServiceConfigID, string _class)
        {
            try
            {
                Log(">>UpdateVehicleInfo(uid={0}, oldLicensePlate={1}, newLicensePlate={2}, vid= {3}, oldBoxId={4}, newBoxId={5}, email = '{6}',timeZone={7}, dayLightSaving={8}, formatType={9}, notify={10}, warning={11}, critical={12}, autoAdjustDayLightSaving={13} )", userId, oldLicensePlate, newLicensePlate, vehicleId, oldBoxId, newBoxId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving);

                if (!ValidateUserLicensePlate(userId, SID, oldLicensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                VLF.CLS.Def.Structures.VehicInfo vehicInfo = new VLF.CLS.Def.Structures.VehicInfo();
                vehicInfo.color = color;
                vehicInfo.costPerMile = costPerMile;
                vehicInfo.description = description;
                vehicInfo.makeModelId = makeModelId;
                vehicInfo.modelYear = modelYear;
                vehicInfo.stateProvince = stateProvince;
                vehicInfo.vehicleTypeId = vehicleTypeId;
                vehicInfo.vinNum = vinNum;
                vehicInfo.iconTypeId = iconTypeId;
                vehicInfo.email = email;
                vehicInfo.phone = phone;
                vehicInfo.timeZoneNew = timeZone;
                vehicInfo.dayLightSaving = Convert.ToInt16(dayLightSaving);
                vehicInfo.formatType = formatType;
                vehicInfo.notify = Convert.ToInt16(notify);
                vehicInfo.warning = Convert.ToInt16(warning);
                vehicInfo.critical = Convert.ToInt16(critical);
                vehicInfo.autoAdjustDayLightSaving = Convert.ToInt16(autoAdjustDayLightSaving);
                vehicInfo.maintenance = Convert.ToInt16(maintenance);
                vehicInfo._class = _class;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet ds = dbVehicle.GetVehicleAdditionalInfo(vehicleId);

                    if (ds.Tables.Count > 0 || ds.Tables[0].Rows.Count > 0)
                    {
                        vehicInfo.field1 = ds.Tables[0].Rows[0]["Field1"].ToString();
                        vehicInfo.field2 = ds.Tables[0].Rows[0]["Field2"].ToString();
                        vehicInfo.field3 = ds.Tables[0].Rows[0]["Field3"].ToString();
                        vehicInfo.field4 = ds.Tables[0].Rows[0]["Field4"].ToString();
                    }
                    else
                    {
                        vehicInfo.field1 = "";
                        vehicInfo.field2 = "";
                        vehicInfo.field3 = "";
                        vehicInfo.field4 = "";
                    }

                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) info", vehicleId));

                    if (oldBoxId != newBoxId ||
                        oldLicensePlate.TrimEnd() != newLicensePlate.TrimEnd())
                        dbVehicle.ChangeVehicleInfo_NewTZ(vehicInfo, vehicleId, newLicensePlate, newBoxId);
                    else
                        dbVehicle.UpdateVehicleInfo_NewTZ(vehicInfo, vehicleId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) info", vehicleId));
                }

                //Salman Mar 06, 2013
                //Update into vlfServiceVehicleAssignment

                string sql = string.Format("usp_vlfServiceVehicleAssignment_Insert_Update {0},{1},{2}", ServiceConfigID, userId, vehicleId);
                using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                {
                    if (scon.State != ConnectionState.Open) scon.Open();
                    using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(sql, scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                    if (scon.State != ConnectionState.Closed) scon.Close();
                }

                return (int)InterfaceError.NoError;

            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="oldLicensePlate"></param>
        /// <param name="newLicensePlate"></param>
        /// <param name="color"></param>
        /// <param name="costPerMile"></param>
        /// <param name="description"></param>
        /// <param name="makeModelId"></param>
        /// <param name="modelYear"></param>
        /// <param name="stateProvince"></param>
        /// <param name="vehicleTypeId"></param>
        /// <param name="vinNum"></param>
        /// <param name="vehicleId"></param>
        /// <param name="oldBoxId"></param>
        /// <param name="newBoxId"></param>
        /// <param name="iconTypeId"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a vehicle information.")]
        public int UpdateVehicleInfo(int userId, string SID, string oldLicensePlate,
                                    string newLicensePlate,
                                    string color, double costPerMile,
                                    string description, int makeModelId,
                                    short modelYear, string stateProvince,
                                    short vehicleTypeId, string vinNum,
                                    Int64 vehicleId, int oldBoxId, int newBoxId, short iconTypeId,
                                    string email, string phone, int timeZone, bool dayLightSaving, short formatType,
                                    bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool maintenance, int ServiceConfigID, string _class)
        {
            try
            {
                Log(">>UpdateVehicleInfo(uid={0}, oldLicensePlate={1}, newLicensePlate={2}, vid= {3}, oldBoxId={4}, newBoxId={5}, email = '{6}',timeZone={7}, dayLightSaving={8}, formatType={9}, notify={10}, warning={11}, critical={12}, autoAdjustDayLightSaving={13} )", userId, oldLicensePlate, newLicensePlate, vehicleId, oldBoxId, newBoxId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving);

                if (!ValidateUserLicensePlate(userId, SID, oldLicensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                VLF.CLS.Def.Structures.VehicInfo vehicInfo = new VLF.CLS.Def.Structures.VehicInfo();
                vehicInfo.color = color;
                vehicInfo.costPerMile = costPerMile;
                vehicInfo.description = description;
                vehicInfo.makeModelId = makeModelId;
                vehicInfo.modelYear = modelYear;
                vehicInfo.stateProvince = stateProvince;
                vehicInfo.vehicleTypeId = vehicleTypeId;
                vehicInfo.vinNum = vinNum;
                vehicInfo.iconTypeId = iconTypeId;
                vehicInfo.email = email;
                vehicInfo.phone = phone;
                vehicInfo.timeZone = timeZone;
                vehicInfo.dayLightSaving = Convert.ToInt16(dayLightSaving);
                vehicInfo.formatType = formatType;
                vehicInfo.notify = Convert.ToInt16(notify);
                vehicInfo.warning = Convert.ToInt16(warning);
                vehicInfo.critical = Convert.ToInt16(critical);
                vehicInfo.autoAdjustDayLightSaving = Convert.ToInt16(autoAdjustDayLightSaving);
                vehicInfo.maintenance = Convert.ToInt16(maintenance);
                vehicInfo._class = _class;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet ds = dbVehicle.GetVehicleAdditionalInfo(vehicleId);

                    if (ds.Tables.Count > 0 || ds.Tables[0].Rows.Count > 0)
                    {
                        vehicInfo.field1 = ds.Tables[0].Rows[0]["Field1"].ToString();
                        vehicInfo.field2 = ds.Tables[0].Rows[0]["Field2"].ToString();
                        vehicInfo.field3 = ds.Tables[0].Rows[0]["Field3"].ToString();
                        vehicInfo.field4 = ds.Tables[0].Rows[0]["Field4"].ToString();
                    }
                    else
                    {
                        vehicInfo.field1 = "";
                        vehicInfo.field2 = "";
                        vehicInfo.field3 = "";
                        vehicInfo.field4 = "";
                    }

                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) info", vehicleId));

                    if (oldBoxId != newBoxId ||
                        oldLicensePlate.TrimEnd() != newLicensePlate.TrimEnd())
                        dbVehicle.ChangeVehicleInfo(vehicInfo, vehicleId, newLicensePlate, newBoxId);
                    else
                        dbVehicle.UpdateVehicleInfo(vehicInfo, vehicleId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) info", vehicleId));
                }

                //Salman Mar 06, 2013
                //Update into vlfServiceVehicleAssignment

                string sql = string.Format("usp_vlfServiceVehicleAssignment_Insert_Update {0},{1},{2}", ServiceConfigID, userId, vehicleId);
                using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                {
                    if (scon.State != ConnectionState.Open) scon.Open();
                    using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(sql, scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                    if (scon.State != ConnectionState.Closed) scon.Close();
                }

                return (int)InterfaceError.NoError;

            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="field1"></param>
        /// <param name="field2"></param>
        /// <param name="field3"></param>
        /// <param name="field4"></param>
        /// <param name="field5"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a vehicle additional information.")]
        public int UpdateVehicleAdditionalInfo(int userId, string SID, Int64 vehicleId,
                         string field1, string field2, string field3, string field4, string field5, int? vehicleWeight, string vehicleWtUnit, float? fuelCapacity, float? fuelBurnRate)
        {
            try
            {
                Log(">>UpdateVehicleAdditionalInfo(uid={0}, vId={1}, field1={2}, field2={3}, field3={4}, field4={5}, field5={6}, VehicleWeight={7}, VehicleWtUnit={8}, FuelCapacity={9}, FuelBurnRate={10})",
                            userId, vehicleId, field1, field2, field3, field4, field5, vehicleWeight, vehicleWtUnit, fuelCapacity, fuelBurnRate);
                // Authenticate & Authorize

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) additional info - Initial values", vehicleId));

                    dbVehicle.UpdateVehicleAdditionalInfo(vehicleId, field1, field2, field3, field4, field5, vehicleWeight, vehicleWtUnit, fuelCapacity, fuelBurnRate);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) additional info", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleAdditionalInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Update 3rd Party vehicle additional information.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="EquipNbr"></param>
        /// <param name="SAPEquipNbr"></param>
        /// <param name="LegacyEquipNbr"></param>
        /// <param name="ObjectType"></param>
        /// <param name="DOTNbr"></param>
        /// <param name="?"></param>
        /// <param name="EquipCategory"></param>
        /// <param name="AcquireDate"></param>
        /// <param name="RetireDate"></param>
        /// <param name="SoldDate"></param>
        /// <param name="ObjectPrefix"></param>
        /// <param name="OwningDistrict"></param>
        /// <param name="ProjectNbr"></param>
        /// <param name="TotalCtrReading_1"></param>
        /// <param name="TotalCtrReading_2"></param>
        /// <param name="CtrReadingUom_1"></param>
        /// <param name="CtrReadingUom_2"></param>
        /// <param name="ShortDesc"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update 3rd Party vehicle additional information.")]
        public int Update3rdPartyVehicleAdditionalInfo(int userId, string SID, Int64 vehicleId,
                         string EquipNbr, string SAPEquipNbr, string LegacyEquipNbr, string ObjectType, string DOTNbr, string EquipCategory,
                         DateTime? AcquireDate, DateTime? RetireDate, DateTime? SoldDate, string ObjectPrefix, string OwningDistrict, string ProjectNbr,
                         decimal? TotalCtrReading_1, decimal? TotalCtrReading_2, string CtrReadingUom_1, string CtrReadingUom_2, string ShortDesc)

        //public int Update3rdPartyVehicleAdditionalInfo(int userId, string SID, Int64 vehicleId,
        //                 string EquipNbr, string SAPEquipNbr, string LegacyEquipNbr, string ObjectType, string DOTNbr, string EquipCategory,
        //                 DateTime AcquireDate, DateTime RetireDate, DateTime SoldDate, string ObjectPrefix, string OwningDistrict, string ProjectNbr,
        //                 decimal? TotalCtrReading_1, decimal? TotalCtrReading_2, string CtrReadingUom_1, string CtrReadingUom_2, string ShortDesc)
        {
            try
            {
                Log(">>Update3rdPartyVehicleAdditionalInfo(uId={0}, vId={1}, EquipNbr={2}, SAPEquipNbr={3}, LegacyEquipNbr={4}, ObjectType={5}, DOTNbr={6}, EquipCategory={7}, AcquireDate={8}, RetireDate={9}, SoldDate={10}, ObjectPrefix={11}, OwningDistrict={12}, ProjectNbr={13}, TotalCtrReading_1={14}, TotalCtrReading_2={15}m CtrReadingUom_1={16},CtrReadingUom_2={17}, ShortDesc={18})",
                            userId, vehicleId, EquipNbr, SAPEquipNbr, LegacyEquipNbr, ObjectType, DOTNbr, EquipCategory, AcquireDate, RetireDate, SoldDate, ObjectPrefix, OwningDistrict, ProjectNbr, TotalCtrReading_1, TotalCtrReading_2, CtrReadingUom_1, CtrReadingUom_2, ShortDesc);
                // Authenticate & Authorize

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle_AdditoionalInfo", userId, 0, "vlfVehicleAdditionalInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update 3rd Party vehicle({0}) additional info - Initial values", vehicleId));

                    dbVehicle.Update3rdPartyVehicleAdditionalInfo(vehicleId, EquipNbr, SAPEquipNbr, LegacyEquipNbr, ObjectType, DOTNbr, EquipCategory, AcquireDate, RetireDate, SoldDate,
                                                                  ObjectPrefix, OwningDistrict, ProjectNbr, TotalCtrReading_1, TotalCtrReading_2, CtrReadingUom_1, CtrReadingUom_2, ShortDesc);

                    LoggerManager.RecordUserAction("Vehicle_AdditoionalInfo", userId, 0, "vlfVehicleAdditionalInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update 3rd Party vehicle({0}) additional info", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Update3rdPartyVehicleAdditionalInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="weekdayStart"></param>
        /// <param name="weekdayEnd"></param>
        /// <param name="weekendStart"></param>
        /// <param name="weekendEnd"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a vehicle additional information.")]
        public int UpdateVehicleWorkingHours(int userId, string SID, Int64 vehicleId, int weekdayStart, int weekdayEnd, int weekendStart, int weekendEnd)
        {
            try
            {
                Log(">>UpdateVehicleWorkingHours(uid={0}, vid= {1}, weekdayStart={2}, weekdayEnd={3}, weekendStart={4}, weekendEnd={5})", userId, vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);
                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleWorkingHours",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle working hours", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) working hours", vehicleId.ToString()));

                    dbVehicle.UpdateVehicleWorkingHours(vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleWorkingHours",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle working hours", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) working hours", vehicleId.ToString()));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleWorkingHours : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// same as UpdateVehicleInfo, but it uses the VehicInfo structure
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="xmlVehicleInfo"></param>
        /// <param name="newLicensePlate"></param>
        /// <param name="newBoxId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update Vehicle Information")]
        public int UpdateVehicleInformation(int userId, string SID, Int64 vehicleId, string xmlVehicleInfo,
           string newLicensePlate, int newBoxId)
        {
            try
            {
                Log(">>UpdateVehicleInformation(uid={0}, vid= {1})", userId, vehicleId);

                string oldLicensePlate = "";
                int oldBoxId = -1;

                if (!ValidateUserLicensePlate(userId, SID, oldLicensePlate))
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

                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) info", vehicleId));

                    if (oldBoxId != newBoxId || oldLicensePlate.TrimEnd() != newLicensePlate.TrimEnd())
                        dbVehicle.ChangeVehicleInfo(vInfo, vehicleId, newLicensePlate, newBoxId);
                    else
                        dbVehicle.UpdateVehicleInfo(vInfo, vehicleId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) info", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleInformation : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        /// <summary>
        ///         add a vehicle to the system
        /// </summary>
        /// <comment>
        ///         GB -> ASK: why is the super user used here ??
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vinNum"></param>
        /// <param name="makeModelId"></param>
        /// <param name="vehicleTypeId"></param>
        /// <param name="stateProvince"></param>
        /// <param name="modelYear"></param>
        /// <param name="color"></param>
        /// <param name="description"></param>
        /// <param name="costPerMile"></param>
        /// <param name="organizationId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="boxId"></param>
        /// <param name="iconTypeId"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a new Vehicle to system.")]
        public int AddVehicle_NewTZ(int userId, string SID, string vinNum, int makeModelId, short vehicleTypeId,
                              string stateProvince, short modelYear, string color, string description,
                              double costPerMile, int organizationId, string licensePlate, int boxId,
                              short iconTypeId, string email, string phone, float timeZone, bool dayLightSaving, short formatType,
                              bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool maintenance, int ServiceConfigID, string _class, ref Int64 vehicleId)
        {
            try
            {
                Log(">>AddVehicle( uid={0}, orgId={1}, LP={2}, boxId={3} , email = '{4}', timeZone={5}, dayLightSaving={6}, formatType={7}, notify={8}, warning={9}, critical={10}, autoAdjustDayLightSaving={11})", userId, organizationId, licensePlate, boxId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                //Authorization
                VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                if (!dbUser.ValidateSuperUserOne(userId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                VLF.CLS.Def.Structures.VehicInfo vehicInfo = new VLF.CLS.Def.Structures.VehicInfo();
                vehicInfo.color = color;
                vehicInfo.costPerMile = costPerMile;
                vehicInfo.description = description;
                vehicInfo.makeModelId = makeModelId;
                vehicInfo.modelYear = modelYear;
                vehicInfo.stateProvince = stateProvince;
                vehicInfo.vehicleTypeId = vehicleTypeId;
                vehicInfo.vinNum = vinNum;
                vehicInfo.iconTypeId = iconTypeId;
                vehicInfo.email = email;
                vehicInfo.phone = phone;
                vehicInfo.timeZoneNew = timeZone;
                vehicInfo.dayLightSaving = Convert.ToInt16(dayLightSaving);
                vehicInfo.formatType = formatType;
                vehicInfo.notify = Convert.ToInt16(notify);
                vehicInfo.warning = Convert.ToInt16(warning);
                vehicInfo.critical = Convert.ToInt16(critical);
                vehicInfo.autoAdjustDayLightSaving = Convert.ToInt16(autoAdjustDayLightSaving);
                vehicInfo.maintenance = Convert.ToInt16(maintenance);
                vehicInfo._class = _class;

                VLF.CLS.Def.Structures.VehicAssign vehicAssign;
                vehicAssign.boxId = boxId;
                vehicAssign.licensePlate = licensePlate;
                vehicAssign.vehicleId = VLF.CLS.Def.Const.unassignedIntValue;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    vehicleId = dbVehicle.AddVehicle_NewTZ(vehicInfo, vehicAssign, organizationId);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Add vehicle", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Add vehicle({0})", vehicleId));
                }

                //Salman Mar 05, 2013
                //Insert into vlfServiceVehicleAssignment
                if (ServiceConfigID > -1) // 40-42 for Posted Speed
                {
                    //string sql = string.Format("INSERT INTO vlfServiceVehicleAssignment (ServiceConfigID,OrganizationId,VehicleId,Stdate,Subjects) VALUES ({0},{1},{2},GETDATE(),'[ServiceName] - for [VehicleDescription] at [Stdate]. The coordinate is [LATITUDE],[LONGITUDE]')", ServiceConfigID, organizationId, vehicleId);
                    string sql = string.Format("usp_vlfServiceVehicleAssignment_Insert_Update {0},{1},{2}", ServiceConfigID, userId, vehicleId);
                    using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                    {
                        if (scon.State != ConnectionState.Open) scon.Open();
                        using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(sql, scon))
                        {
                            scom.ExecuteNonQuery();
                        }
                        if (scon.State != ConnectionState.Closed) scon.Close();
                    }
                }
                return (int)InterfaceError.NoError;

            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end

        /// <summary>
        ///         add a vehicle to the system
        /// </summary>
        /// <comment>
        ///         GB -> ASK: why is the super user used here ??
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vinNum"></param>
        /// <param name="makeModelId"></param>
        /// <param name="vehicleTypeId"></param>
        /// <param name="stateProvince"></param>
        /// <param name="modelYear"></param>
        /// <param name="color"></param>
        /// <param name="description"></param>
        /// <param name="costPerMile"></param>
        /// <param name="organizationId"></param>
        /// <param name="licensePlate"></param>
        /// <param name="boxId"></param>
        /// <param name="iconTypeId"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a new Vehicle to system.")]
        public int AddVehicle(int userId, string SID, string vinNum, int makeModelId, short vehicleTypeId,
                              string stateProvince, short modelYear, string color, string description,
                              double costPerMile, int organizationId, string licensePlate, int boxId,
                              short iconTypeId, string email, string phone, int timeZone, bool dayLightSaving, short formatType,
                              bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, bool maintenance, int ServiceConfigID, string _class, ref Int64 vehicleId)
        {
            try
            {
                Log(">>AddVehicle( uid={0}, orgId={1}, LP={2}, boxId={3} , email = '{4}', timeZone={5}, dayLightSaving={6}, formatType={7}, notify={8}, warning={9}, critical={10}, autoAdjustDayLightSaving={11})", userId, organizationId, licensePlate, boxId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                //Authorization
                VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                if (!dbUser.ValidateSuperUserOne(userId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                vehicleId = VLF.CLS.Def.Const.unassignedIntValue;
                VLF.CLS.Def.Structures.VehicInfo vehicInfo = new VLF.CLS.Def.Structures.VehicInfo();
                vehicInfo.color = color;
                vehicInfo.costPerMile = costPerMile;
                vehicInfo.description = description;
                vehicInfo.makeModelId = makeModelId;
                vehicInfo.modelYear = modelYear;
                vehicInfo.stateProvince = stateProvince;
                vehicInfo.vehicleTypeId = vehicleTypeId;
                vehicInfo.vinNum = vinNum;
                vehicInfo.iconTypeId = iconTypeId;
                vehicInfo.email = email;
                vehicInfo.phone = phone;
                vehicInfo.timeZone = timeZone;
                vehicInfo.dayLightSaving = Convert.ToInt16(dayLightSaving);
                vehicInfo.formatType = formatType;
                vehicInfo.notify = Convert.ToInt16(notify);
                vehicInfo.warning = Convert.ToInt16(warning);
                vehicInfo.critical = Convert.ToInt16(critical);
                vehicInfo.autoAdjustDayLightSaving = Convert.ToInt16(autoAdjustDayLightSaving);
                vehicInfo.maintenance = Convert.ToInt16(maintenance);
                vehicInfo._class = _class;

                VLF.CLS.Def.Structures.VehicAssign vehicAssign;
                vehicAssign.boxId = boxId;
                vehicAssign.licensePlate = licensePlate;
                vehicAssign.vehicleId = VLF.CLS.Def.Const.unassignedIntValue;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    vehicleId = dbVehicle.AddVehicle(vehicInfo, vehicAssign, organizationId);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Add vehicle", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Add vehicle({0})", vehicleId));
                }

                //Salman Mar 05, 2013
                //Insert into vlfServiceVehicleAssignment
                if (ServiceConfigID > -1) // 40-42 for Posted Speed
                {
                    //string sql = string.Format("INSERT INTO vlfServiceVehicleAssignment (ServiceConfigID,OrganizationId,VehicleId,Stdate,Subjects) VALUES ({0},{1},{2},GETDATE(),'[ServiceName] - for [VehicleDescription] at [Stdate]. The coordinate is [LATITUDE],[LONGITUDE]')", ServiceConfigID, organizationId, vehicleId);
                    string sql = string.Format("usp_vlfServiceVehicleAssignment_Insert_Update {0},{1},{2}", ServiceConfigID, userId, vehicleId);
                    using (System.Data.SqlClient.SqlConnection scon = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                    {
                        if (scon.State != ConnectionState.Open) scon.Open();
                        using (System.Data.SqlClient.SqlCommand scom = new System.Data.SqlClient.SqlCommand(sql, scon))
                        {
                            scom.ExecuteNonQuery();
                        }
                        if (scon.State != ConnectionState.Closed) scon.Close();
                    }
                }
                return (int)InterfaceError.NoError;

            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///      it associate with the vehicle the working hours for weekdays, weekends
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="weekdayStart"></param>
        /// <param name="weekdayEnd"></param>
        /// <param name="weekendStart"></param>
        /// <param name="weekendEnd"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add Vehicle Working Hours to system.")]
        public int AddVehicleWorkingHours(int userId, string SID, Int64 vehicleId, int weekdayStart,
                                          int weekdayEnd, int weekendStart, int weekendEnd)
        {
            try
            {
                Log(">>AddVehicleWorkingHours( uid={0}, vid={1}, weekdayStart={2}, weekdayEnd={3}, weekendStart={4}, weekendEnd={5})",
                               userId, vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    vehicleId = dbVehicle.AddVehicleWorkingHours(vehicleId, weekdayStart, weekdayEnd, weekendStart, weekendEnd);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleWorkingHours",
                                                  string.Format("VehicleId={0}", vehicleId.ToString()),
                                                  "Add vehicle working hours", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Add vehicle({0}) working hours", vehicleId.ToString()));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicleWorkingHours : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete vehicle working hours.")]
        public int DeleteVehicleWorkingHours(int userId, string SID, Int64 vehicleId)
        {
            try
            {
                Log(">>DeleteVehicleWorkingHours(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleWorkingHours",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Delete vehicle working hours", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle({0}) working hours", vehicleId));

                    dbVehicle.DeleteVehicleWorkingHours(vehicleId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleWorkingHours",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Delete vehicle working hours", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle({0}) working hours", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteVehicleWorkingHours : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="boxId"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Assign a vehicle to box.")]
        public int AssignVehicle(int userId, string SID, string licensePlate, int boxId, Int64 vehicleId)
        {
            try
            {
                Log(">>AssignVehicle(uid={0}, LP={1}, boxId={2}, vid= {3} )", userId, licensePlate, boxId, vehicleId);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                VLF.CLS.Def.Structures.VehicAssign vehicAssign;
                vehicAssign.boxId = boxId;
                vehicAssign.licensePlate = licensePlate;
                vehicAssign.vehicleId = vehicleId;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AssignVehicle(vehicAssign);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleAssignment",
                                                  string.Format("BoxId={0} AND VehicleId={1} AND LicensePlate='{2}'", boxId, vehicleId, licensePlate),
                                                  "Add vehicle assignment", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Add vehicle({0}) assignment", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AssignVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// dont validate License Plate
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="boxId"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
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
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleAssignment",
                                                  string.Format("BoxId={0} AND VehicleId={1} AND LicensePlate='{2}'", boxId, vehicleId, licensePlate),
                                                  "Add vehicle assignment", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Add vehicle({0}) assignment", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AssignVehicleToBox : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
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
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Delete vehicle", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle({0})", vehicleId));

                    dbVehicle.DeleteVehicle(vehicleId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleInfo",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Delete vehicle", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle({0})", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="vehicleId"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="dayLightSaving"></param>
        /// <returns></returns>
        [WebMethod]
        public int SetVehicleAutoAdjustDayLightSaving(int userId, string SID, Int64 vehicleId, bool autoAdjustDayLightSaving, bool dayLightSaving)
        {
            try
            {
                Log(">>SetVehicleAutoAdjustDayLightSaving( vid= {0}, autoAdjustDayLightSaving={1}, dayLightSaving={2} )", vehicleId, autoAdjustDayLightSaving, dayLightSaving);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    dbSystem.SetVehicleAutoAdjustDayLightSaving(vehicleId, autoAdjustDayLightSaving, dayLightSaving);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetVehicleAutoAdjustDayLightSaving : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region Vehicle Sensors Interfaces

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = @"Retrieves list of all available Sensors including their states by Vehicle License 
                                 Plate. <br>  XML File format: [SensorId][SensorName][SensorAction][AlarmLevel]")]
        public int GetVehicleSensorsXML(int userId, string SID, string licensePlate, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">>GetVehicleSensorsXML(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsSensors = null;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    // 1. Retrieves user-defined box sensors
                    dsSensors = dbVehicle.GetVehicleSensorsInfo(licensePlate);
                    if (dsSensors == null || dsSensors.Tables.Count == 0 || dsSensors.Tables[0].Rows.Count == 0)
                    {
                        // 2. Retrieves default box sensors (if sensor haven't defined yet)
                        short hwTypeId = VLF.CLS.Def.Const.unassignedIntValue;
                        using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                        {
                            DataSet dsBoxConfig = dbBox.GetBoxConfigInfo(GetBoxIDByLicensePlate(licensePlate));
                            if (Util.IsDataSetValid(dsBoxConfig))
                            {
                                // TODO: Today box has only one HW type
                                hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
                            }
                            else
                            {
                                // TODO: write to log (cannot retrieve HW type for specific box)
                                Log("<< GetVehicleSensorsXML : uid={0}, LP={1}, ERR=hardware type not found", userId, licensePlate);
                                return Convert.ToInt32(InterfaceError.InvalidParameter);
                            }
                        }

                        using (SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                        {
                            dsSensors = dbSystemConfig.GetDefaultSensorsInfoByHwTypeId(hwTypeId);
                        }
                    }
                }

                if (Util.IsDataSetValid(dsSensors))
                    xml = dsSensors.GetXml();

                LogFinal("<< GetVehicleSensorsXML(uid={0}, LP={1}, tSpan={2})",
                               userId, licensePlate, DateTime.Now.Subtract(dtNow).TotalMilliseconds);


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleSensorsXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex); ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <comment>
        ///         GB -> what conditions is this 
        ///               if (lang == "fr" && lang != null)
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="lang"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves list of all available Sensors including their states by Vehicle License Plate. XML File format: [SensorId][SensorName][SensorAction][AlarmLevel][lang]")]
        public int GetVehicleSensorsXMLByLang(int userId, string SID, string licensePlate, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">>GetVehicleSensorsXMLByLang(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                short hwTypeId = VLF.CLS.Def.Const.unassignedIntValue;

                // 2. Retrieves default box sensors (if sensor haven't defined yet)
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsBoxConfig = dbBox.GetBoxConfigInfo(GetBoxIDByLicensePlate(licensePlate));
                    if (Util.IsDataSetValid(dsBoxConfig))
                    {
                        // TODO: Today box has only one HW type
                        hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
                    }
                    else
                    {
                        // TODO: write to log (cannot retrieve HW type for specific box)
                        Log("-- GetVehicleSensorsXMLByLang : uid={0} LP={1}, ERROR=GetBoxConfigInfo no hardware type",
                                   userId, licensePlate);
                        return (int)InterfaceError.InvalidParameter;
                    }
                }



                // 1. Retrieves user-defined box sensors
                DataSet dsSensors = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsSensors = dbVehicle.GetVehicleSensorsInfo(licensePlate);
                }

                if (dsSensors == null || dsSensors.Tables.Count == 0 || dsSensors.Tables[0].Rows.Count == 0)
                    using (SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                    {
                        dsSensors = dbSystemConfig.GetDefaultSensorsInfoByHwTypeId(hwTypeId);
                    }

                if (Util.IsDataSetValid(dsSensors))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "SensorId", "SensorName", "Sensors", ref dsSensors);
                        dbl.LocalizationDataAction(lang, "SensorId", "SensorAction", "Sensors", hwTypeId, ref dsSensors);
                    }
                    xml = dsSensors.GetXml();
                }

                LogFinal("<< GetVehicleSensorsXMLByLang(uid={0}, LP={1}, tSpan={2})",
                               userId, licensePlate, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex); ;
            }
        }


        /// <summary>
        ///         Retrieves list of all available Sensors including their states by Box ID
        /// </summary>
        /// <comment>
        ///         GB -> 
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves list of all available Sensors including their states by Box ID.")]
        public int GetVehicleSensorsXMLByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleSensorsXMLByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                string licensePlate = VLF.CLS.Def.Const.unassignedStrValue;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    licensePlate = dbVehicle.GetLicensePlateByBox(boxId);
                }

                if (!string.IsNullOrEmpty(licensePlate))
                    return GetVehicleSensorsXML(userId, SID, licensePlate, ref xml);
                else
                    return Convert.ToInt32(InterfaceError.InvalidParameter);

            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleSensorsXMLByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="boxId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get vehicle Box ID by License Plate.")]
        public int GetBoxIDByLicensePlate(int userId, string SID, string licensePlate, ref int boxId)
        {
            try
            {
                Log(">>GetBoxIDByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                boxId = GetBoxIDByLicensePlate(licensePlate);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxIDByLicensePlate : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex); ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get vehicle Box ID by License Plate.")]
        protected int GetBoxIDByLicensePlate(string licensePlate)
        {
            int boxId = VLF.CLS.Def.Const.unassignedIntValue;

            using (Vehicle dbVehicle = new Vehicle(Application["ConnectionString"].ToString()))
            {
                boxId = dbVehicle.GetBoxIdByLicensePlate(licensePlate);
            }

            return boxId;
        }

        /// <summary>
        ///         Get Box ID by Vehicle Description and organization ID
        ///         01/02/2007 by Max for TMW Integration project
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="SID">Security ID</param>
        /// <param name="vehicleDescription">Vehicle Description (vlfVehicleInfo)</param>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="boxId">Box Id reference</param>
        /// <returns>Error code</returns>
        [WebMethod(Description = "Get vehicle Box ID by Vehicle Description and organization ID")]
        public int GetBoxIDByVehicleDescription(int userId, string SID, string vehicleDescription,
                                                int organizationId, ref int boxId)
        {

            try
            {
                Log(">> GetBoxIDByVehicleDescription(uid={0}, vehicleDescription={1})", userId, vehicleDescription);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    boxId = dbVehicle.GetBoxIdByVehicleDescription(vehicleDescription, organizationId);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxIDByVehicleDescription : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Get Box Channels timeout by Vehicle Description and organization ID
        /// 
        /// \* added 2007-09-10 Max
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="SID">Security ID</param>
        /// <param name="vehicleDescription">Vehicle Description (vlfVehicleInfo)</param>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="xml">Box Id + channel resp. times reference: </param>
        /// Structure: <VehicleBoxChannelsInfo><BoxId>{0}</BoxId><ChannelResponse>{0};{1};{2};</ChannelResponse></VehicleBoxChannelsInfo>
        /// <returns>Error code</returns>
        [WebMethod(Description = "Get vehicle Box channels Info by Vehicle Description and organization ID")]
        public int GetBoxChannelsInfo(int userId, string SID, string vehicleDescription, int organizationId, ref string xml)
        {

            try
            {
                Log(">> GetBoxChannelsInfo(uid={0}, vehicleDescription={1})", userId, vehicleDescription);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsChannels = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsChannels = dbVehicle.GetBoxChannelsByVehicleDescription(vehicleDescription, organizationId);
                }

                StringBuilder sbXml = new StringBuilder("<VehicleBoxChannelsInfo>");
                if (Util.IsDataSetValid(dsChannels))
                {
                    sbXml.AppendFormat("<BoxId>{0}</BoxId><ChannelResponse>", dsChannels.Tables[0].Rows[0]["BoxId"]);
                    for (int i = 0; i < dsChannels.Tables[0].Rows.Count; i++)
                    {
                        sbXml.AppendFormat("{0}", dsChannels.Tables[0].Rows[i]["SessionTimeOut"]);
                        if (i < dsChannels.Tables[0].Rows.Count - 1) // add commas except the last item
                            sbXml.Append(",");
                    }
                    sbXml.Append("</ChannelResponse>");
                    sbXml.Append(String.Format("<UpdateFrequency>{0}</UpdateFrequency>",
                       dsChannels.Tables[0].Rows[0]["UpdateFrequency"]));
                    sbXml.Append("</VehicleBoxChannelsInfo>");
                    xml = sbXml.ToString();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxChannelsInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xmlSensorsSetting"></param>
        /// <returns></returns>
        [WebMethod(Description = "Set new sensors to the box by BoxId.")]
        public int SetVehicleSensorsByBoxId(int userId, string SID, int boxId, string xmlSensorsSetting)
        {
            try
            {
                Log(">> SetVehicleSensorsByBoxId(uid={0}, boxId={1}, xmlSensorsSetting={2} )",
                               userId, boxId, xmlSensorsSetting);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                // 1. Creates dataset from xml
                DataSet dsSensorsCfg = null;
                if (xmlSensorsSetting != null && xmlSensorsSetting != "")
                {
                    dsSensorsCfg = new DataSet();
                    System.IO.StringReader strrXML = new System.IO.StringReader(xmlSensorsSetting);
                    dsSensorsCfg.ReadXml(strrXML);
                }

                // 2. Update sensors info
                DataSet dsBoxConfig = null;
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dsBoxConfig = dbBox.GetBoxConfigInfo(boxId);

                    if (Util.IsDataSetValid(dsBoxConfig))
                    {
                        // TODO: Today box has only one HW type
                        dbBox.SetSensors(boxId, Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]), dsSensorsCfg);
                        LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  null,
                                                  "Set vehicle sensor", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle sensor for box({0} and BoxHwTypeId({1})", boxId, Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"])));
                    }
                    else
                    {
                        // TODO: write to log (cannot retrieve HW type for specific box)
                        Log("-- SetVehicleSensorsByBoxId :uid={0}, boxId={1}, ERROR= GetBoxConfigInfo failed",
                                       userId, boxId);

                        return (int)InterfaceError.InvalidParameter;
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetVehicleSensorsByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Set new sensors to the box by vehicle License Plate
        /// </summary>
        /// <comment>
        ///      GB -> Max got it
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="xmlSensorsSetting"></param>
        /// <returns></returns>
        [WebMethod(Description = "Set new sensors to the box by vehicle License Plate.")]
        public int SetVehicleSensorsByLicencePlate(int userId, string SID, string licensePlate, string xmlSensorsSetting)
        {
            Log(">>SetVehicleSensorsByLicencePlate(uid={0}, LP={1}, xmlSensorsSetting={2} )", userId, licensePlate, xmlSensorsSetting);

            if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            int boxId = VLF.CLS.Def.Const.unassignedIntValue;
            GetBoxIDByLicensePlate(userId, SID, licensePlate, ref boxId);
            return SetVehicleSensorsByBoxId(userId, SID, boxId, xmlSensorsSetting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="sensorId"></param>
        /// <param name="sensorName"></param>
        /// <param name="sensorAction"></param>
        /// <param name="alarmLevelOn"></param>
        /// <param name="alarmLevelOff"></param>
        /// <returns></returns>
        [WebMethod(Description = "Set new sensors information by vehicle License Plate.")]
        public int UpdateSensorByLicencePlate(int userId, string SID, string licensePlate, short sensorId, string sensorName, string sensorAction, short alarmLevelOn, short alarmLevelOff)
        {
            try
            {
                Log(">>UpdateSensorByLicencePlate(uid={0}, LP={1}, sensorId={2}, sensorName={3}, sensorAction={4}, alarmLevelOn={5}, alarmLevelOff={6} )",
                         userId, licensePlate, sensorId, sensorName, sensorAction, alarmLevelOn, alarmLevelOff);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                int boxId = VLF.CLS.Def.Const.unassignedIntValue;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    boxId = dbVehicle.GetBoxIdByLicensePlate(licensePlate);
                }

                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  string.Format("BoxId={0} AND SensorId={1}", boxId, sensorId),
                                                  "Update vehicle sensor", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle sensor({0}) for box({1})", sensorId, boxId));

                    dbBox.UpdateSensor(boxId, sensorId, sensorName, sensorAction, alarmLevelOn, alarmLevelOff);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  string.Format("BoxId={0} AND SensorId={1}", boxId, sensorId),
                                                  "Update vehicle sensor", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle sensor({0}) for box({1})", sensorId, boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateSensorByLicencePlate : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// <param name="userId"></param>
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <param name="sensorId"></param>
        /// <param name="sensorName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Set new sensors information by vehicle License Plate.")]
        public int UpdateSensorNameByLicencePlate(int userId, string licensePlate, short sensorId, string sensorName)
        {
            try
            {
                Log(">>UpdateSensorByLicencePlate(LP={1}, sensorId={2}, sensorName={3})",
                         userId, licensePlate, sensorId, sensorName);

                
                int boxId = VLF.CLS.Def.Const.unassignedIntValue;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    boxId = dbVehicle.GetBoxIdByLicensePlate(licensePlate);
                }

                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  string.Format("BoxId={0} AND SensorId={1}", boxId, sensorId),
                                                  "Update vehicle sensor", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle sensor({0}) for box({1})", sensorId, boxId));

                    dbBox.UpdateSensorName(boxId, sensorId, sensorName);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  string.Format("BoxId={0} AND SensorId={1}", boxId, sensorId),
                                                  "Update vehicle sensor", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle sensor({0}) for box({1})", sensorId, boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateSensorByLicencePlate : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete user-defined sensors for current box by BoxId.")]
        public int DeleteVehicleSensorsByBoxId(int userId, string SID, int boxId)
        {
            try
            {
                Log(">>DeleteVehicleSensorsByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  string.Format("BoxId={0}", boxId),
                                                  "Delete vehicle sensors", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle sensors for box({0})", boxId));

                    dbBox.DeleteSensors(boxId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBoxSensorsCfg",
                                                  string.Format("BoxId={0}", boxId),
                                                  "Delete vehicle sensors", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete vehicle sensors for box({0})", boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteVehicleSensorsByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete user-defined sensors for current box by License Plate.")]
        public int DeleteVehicleSensorsByLicencePlate(int userId, string SID, string licensePlate)
        {
            Log(">>DeleteVehicleSensorsByLicencePlate(uid={0}, LP={1})", userId, licensePlate);

            if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            int boxId = VLF.CLS.Def.Const.unassignedIntValue;
            GetBoxIDByLicensePlate(userId, SID, licensePlate, ref boxId);
            return DeleteVehicleSensorsByBoxId(userId, SID, boxId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="hwTypeId"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        #endregion

        #region Vehicle Commands Interfaces
        [WebMethod(Description = "Retrieves vehicle commands information by vehicle License Plate. XML File format: [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules],[BoxProtocolTypeId],[BoxProtocolTypeName],[ProtocolPriority]")]
        public int GetVehicleCommandsXML(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetVehicleCommandsXML(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsCmd = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsCmd = dbVehicle.GetVehicleCommandsInfo(licensePlate, userId);
                }

                if (Util.IsDataSetValid(dsCmd))
                    xml = dsCmd.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleCommandsXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle commands information by vehicle License Plate. XML File format: [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules],[BoxProtocolTypeId],[BoxProtocolTypeName],[ProtocolPriority][lang]")]
        public int GetVehicleCommandsXMLByLang(int userId, string SID, string licensePlate, string lang, ref string xml)
        {
            try
            {
                Log(">>GetVehicleCommandsXMLByLang(uid={0}, LP={1}, lang={2})", userId, licensePlate, lang);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsCmd = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsCmd = dbVehicle.GetVehicleCommandsInfo(licensePlate, userId);
                }

                if (Util.IsDataSetValid(dsCmd))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsCmd);
                    }

                    xml = dsCmd.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleCommandsXMLByLang : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Retrieves vehicle commands information by Box ID.XML file: [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules],[BoxProtocolTypeId],[BoxProtocolTypeName],[ProtocolPriority]")]
        public int GetVehicleCommandsXMLByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleCommandsXMLByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                string licensePlate = VLF.CLS.Def.Const.unassignedStrValue;
                DataSet dsCmd = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    licensePlate = dbVehicle.GetLicensePlateByBox(boxId);
                    dsCmd = dbVehicle.GetVehicleCommandsInfo(licensePlate, userId);
                }

                if (Util.IsDataSetValid(dsCmd))
                    xml = dsCmd.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleCommandsXMLByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Retrieves command supported protocols. XML File format: [BoxProtocolTypeId],[BoxProtocolTypeName],[ProtocolPriority]")]
        public int GetCommandProtocolTypesInfo(int userId, string SID, int boxId, short commandTypeId, ref string xml)
        {
            try
            {
                Log(">>GetCommandProtocolTypesInfo(uid={0}, boxId={1}, commandTypeId={2} )", userId, boxId, commandTypeId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsCmdProtocols = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsCmdProtocols = dbVehicle.GetCommandProtocolTypesInfo(boxId, userId, commandTypeId);
                }

                if (Util.IsDataSetValid(dsCmdProtocols))
                    xml = dsCmdProtocols.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCommandProtocolTypesInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Retrieves output supported protocols. XML File format: [BoxProtocolTypeId],[BoxProtocolTypeName],[ProtocolPriority]")]
        public int GetOutputProtocolTypesInfo(int userId, string SID, int boxId, short outputId, ref string xml)
        {
            try
            {
                Log(">> GetOutputProtocolTypesInfo(uid={0}, boxId={1}, outputId={2} )", userId, boxId, outputId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsOutProtocols = null;
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dsOutProtocols = dbBox.GetOutputProtocolTypesInfo(boxId, userId, outputId);
                }

                if (Util.IsDataSetValid(dsOutProtocols))
                    xml = dsOutProtocols.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOutputProtocolTypesInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Retrieves MultiProtocol commands by License Plate.")]
        public int GetVehicleMultiProtocolCommandsXML(int userId, string SID, string licensePlate,
                                                      string protocols, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            Log(">>GetVehicleMultiProtocolCommandsXML(uid={0}, LP={1}, protocols={2})", userId, licensePlate, protocols);

            if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            try
            {
                DataSet dsAllCommands = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsAllCommands = dbVehicle.GetVehicleCommandsInfo(licensePlate, userId);
                    if (dsAllCommands != null && dsAllCommands.Tables.Count > 0 && dsAllCommands.Tables[0].Rows.Count > 0)
                    {
                        // 2. Retrieves all protocols related to current box
                        StringReader strrXML = new StringReader(protocols);
                        DataSet dsProtocols = new DataSet();
                        dsProtocols.ReadXml(strrXML);
                        if (dsProtocols != null && dsProtocols.Tables.Count > 0 && dsProtocols.Tables[0].Rows.Count > 1)
                        {
                            // 3. Retrieves only commands supported by all protocols
                            DataSet dsAllMultiProtocolCommands = dsAllCommands.Clone();
                            ArrayList protIndexes = new ArrayList();
                            for (int indCmd = 0; indCmd < dsAllCommands.Tables[0].Rows.Count; ++indCmd)
                            {
                                if (indCmd > 0 &&
                                   Convert.ToInt16(dsAllCommands.Tables[0].Rows[indCmd - 1]["BoxCmdOutTypeId"])
                                   != Convert.ToInt16(dsAllCommands.Tables[0].Rows[indCmd]["BoxCmdOutTypeId"]))
                                {
                                    protIndexes.Clear();
                                }

                                // 4. check if we are interesting in current protocol
                                foreach (DataRow ittr in dsProtocols.Tables[0].Rows)
                                {
                                    if (Convert.ToInt16(dsAllCommands.Tables[0].Rows[indCmd]["BoxProtocolTypeId"]) ==
                                       Convert.ToInt16(ittr["BoxProtocolTypeId"]))
                                    {
                                        protIndexes.Add(indCmd);
                                        break;
                                    }
                                }

                                // Check command supported all required protocols
                                if (protIndexes.Count == dsProtocols.Tables[0].Rows.Count)
                                {
                                    // Copy only first appierence of specific command (privents dupplicated results)
                                    dsAllMultiProtocolCommands.Tables[0].LoadDataRow(dsAllCommands.Tables[0].Rows[(int)protIndexes[0]].ItemArray, false);
                                    protIndexes.Clear();
                                }
                            }
                            // 6. Fill result as XML
                            if (dsAllMultiProtocolCommands != null && dsAllMultiProtocolCommands.Tables.Count > 0 && dsAllMultiProtocolCommands.Tables[0].Rows.Count > 0)
                            {
                                xml = dsAllMultiProtocolCommands.GetXml();
                            }
                        }
                        else
                        {
                            // 3. Fill result as XML
                            if (dsAllCommands != null && dsAllCommands.Tables.Count > 0 && dsAllCommands.Tables[0].Rows.Count > 0)
                            {
                                xml = dsAllCommands.GetXml();
                            }
                        }
                    }
                }

                LogFinal("<< GetVehicleMultiProtocolCommandsXML(uid={0}, LP={1}, protocols={2}), tSpan={3}",
                                 userId, licensePlate, protocols, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle commands information by vehicle License Plate. XML File format: [BoxCmdOutTypeId], [BoxCmdOutTypeName], [Rules],[BoxProtocolTypeId],[BoxProtocolTypeName],[ProtocolPriority][lang]")]
        public int GetMultiVehicleCommandsXMLByLang(int userId, string SID, string Boxid, string lang, ref string xml)
        {
            try
            {
                Log(">>GetVehicleCommandsXMLByLang(uid={0}, LP={1}, lang={2})", userId, Boxid, lang);

                DataSet dsCmd = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsCmd = dbVehicle.GetMultiVehicleCommandsInfo(Boxid, userId);
                }

                if (Util.IsDataSetValid(dsCmd))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "BoxCmdOutTypeId", "BoxCmdOutTypeName", "CommandType", ref dsCmd);
                    }

                    xml = dsCmd.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleCommandsXMLByLang : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region Vehicle Outputs Interfaces
        [WebMethod(Description = @"Retrieves vehicle outputs information by License Plate and User ID. 
                               XML File format: [OutputId][OutputName][OutputAction]")]
        public int GetVehicleOutputsXML(int userId, string SID, string licensePlate, ref string xml)
        {
            return GetVehicleOutputsXMLByLang(userId, SID, licensePlate, "en", ref xml);
        }

        [WebMethod(Description = @"Retrieves vehicle outputs information by License Plate and User ID. 
                           XML File format: [OutputId][OutputName][OutputAction][lang]")]
        public int GetVehicleOutputsXMLByLang(int userId, string SID, string licensePlate, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">>GetVehicleOutputsXMLByLang(uid={0}, LP={1}, lang={2})", userId, licensePlate, lang);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsOutputs = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsOutputs = dbVehicle.GetVehicleOutputsInfo(licensePlate, userId);
                    short hwTypeId;

                    if (Util.IsDataSetValid(dsOutputs))
                    {

                        if (ASIErrorCheck.IsLangSupported(lang))
                        {
                            DataSet dsBoxConfig = null;
                            using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                            {
                                dsBoxConfig = dbBox.GetBoxConfigInfo(dbVehicle.GetBoxIdByLicensePlate(licensePlate));
                            }


                            if (Util.IsDataSetValid(dsBoxConfig))
                            {
                                // TODO: Today box has only one HW type
                                hwTypeId = Convert.ToInt16(dsBoxConfig.Tables[0].Rows[0]["BoxHwTypeId"]);
                            }
                            else
                            {
                                // TODO: write to log (cannot retrieve HW type for specific box)
                                // TODO: write to log (cannot retrieve HW type for specific box)
                                Log("-- GetVehicleOutputsXMLByLang :uid={0}, LP={1}, ERROR= GetBoxConfigInfo failed",
                                               userId, licensePlate);

                                return (int)InterfaceError.InvalidParameter;
                            }

                            LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                            dbl.LocalizationData(lang, "OutputId", "OutputName", "Outputs", hwTypeId, ref dsOutputs);
                            dbl.LocalizationDataAction(lang, "OutputId", "OutputAction", "Outputs", hwTypeId, ref dsOutputs);
                        }

                        xml = dsOutputs.GetXml();
                    }
                }

                LogFinal("<<GetVehicleOutputsXMLByLang(uid={0}, LP={1}, lang={2}, tSpan={3})",
                                  userId, licensePlate, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleOutputsXMLByLang : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle outputs information by Box ID. XML File format: [OutputId][OutputName][OutputAction]")]
        public int GetVehicleOutputsXMLByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleOutputsXMLByBoxId(uid={0}, boxId={1})", userId, boxId);

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
                LogException("<< GetVehicleOutputsXMLByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Update output information by License Plate.")]
        public int UpdateOutputByLicencePlate(int userId, string SID, string licensePlate, short outputId, string outputName, string outputAction)
        {
            try
            {
                Log(">>UpdateOutputByLicencePlate(uid={0}, LP={1}, outId={2}, outName={3}, outAction={4})",
                                     userId, licensePlate, outputId, outputName, outputAction);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                int boxId = VLF.CLS.Def.Const.unassignedIntValue;


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    boxId = dbVehicle.GetBoxIdByLicensePlate(licensePlate);
                }
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfBoxOutputsCfg",
                                                  string.Format("BoxId={0} AND OutputId={1}", boxId, outputId),
                                                  "Update output info by license plate", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update box({0}) output info by license plate", boxId));

                    dbBox.UpdateOutput(boxId, outputId, outputName, outputAction);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBoxOutputsCfg",
                                                  string.Format("BoxId={0} AND OutputId={1}", boxId, outputId),
                                                  "Update output info by license plate", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update box({0}) output info by license plate", boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateOutputByLicencePlate : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region Vehicle Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get Box supported messages.XML File format: [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]")]
        public int GetAllSupportedMessagesByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            return GetAllSupportedMessagesByBoxIdByLang(userId, SID, boxId, "en", ref xml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="lang"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get Box supported messages.XML File format: [BoxMsgInTypeId],[BoxMsgInTypeName],[AlarmLevel]")]
        public int GetAllSupportedMessagesByBoxIdByLang(int userId, string SID, int boxId, string lang, ref string xml)
        {
            try
            {
                Log(">>GetAllSupportedMessagesByBoxIdByLang(uid={0}, boxId={1}, lang={2})", userId, boxId, lang);

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
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="msgInTypeId"></param>
        /// <param name="alarmLevel"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a message severity.")]
        public int UpdateMsgSeverity(int userId, string SID, int boxId, short msgInTypeId, short alarmLevel)
        {
            try
            {
                Log(">>UpdateMsgSeverity(uid={0}, boxId={1}, msgInTypeId={2}, alarmLevel={3})",
                      userId, boxId, msgInTypeId, alarmLevel);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfBoxMsgSeverity",
                                                  string.Format("BoxId={0} AND BoxMsgInTypeId={1}", boxId, msgInTypeId),
                                                  "Update message severity", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update box({0}) message severity", boxId));

                    dbBox.UpdateMsgSeverity(boxId, (VLF.CLS.Def.Enums.MessageType)msgInTypeId,
                                            (VLF.CLS.Def.Enums.AlarmSeverity)alarmLevel);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBoxMsgSeverity",
                                                  string.Format("BoxId={0} AND BoxMsgInTypeId={1}", boxId, msgInTypeId),
                                                  "Update message severity", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update box({0}) message severity", boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateMsgSeverity : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fwChId"></param>
        /// <param name="lang"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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
        #endregion

        #region Vehicle Box Assignment Interface
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="description"></param>
        /// <returns></returns>
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
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleAssignment", null, "Delete vehicle assignment", this.Context.Request.UserHostAddress, this.Context.Request.RawUrl, string.Format("Delete vehicle assignment for box({0})", boxId));

                }

                return retVal;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteActiveVehicleAssignment : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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
        #endregion


        #region Vehicle Geozone Interfaces
        [WebMethod(Description = "Retrieves all assigned to vehicle geozones info. XML File Format: [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId], [GeozoneName],[Type],[GeozoneType],[SeverityId],[Description],[BoxId]")]
        public int GetAllAssignedToVehicleGeozonesInfo(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">>GetAllAssignedToVehicleGeozonesInfo(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbVehicle.GetAllAssignedToVehicleGeozonesInfo(vehicleId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllAssignedToVehicleGeozonesInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves all assigned to vehicle geozones info. XML File Format: [VehicleId],[GeozoneNo],[SeverityId],[OrganizationId],[GeozoneId], [GeozoneName],[Type],[GeozoneType],[SeverityId],[Description],[BoxId]")]
        public int GetAllAssignedGeozonesToVehicle(int userId, string SID, int organizationId, short geozoneId, ref string xml)
        {
            try
            {
                Log(">>GetAllAssignedGeozonesToVehicle(uid={0}, GeozoneNo={1})", userId, geozoneId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbVehicle.GetAllAssignedGeozonesToVehicle(organizationId, geozoneId, userId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllAssignedGeozonesToVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves all unassigned to vehicle geozones info. XML File format: [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],[SeverityId],[Description]")]
        public int GetAllUnassignedToVehicleGeozonesInfo(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">>GetAllUnassignedToVehicleGeozonesInfo(uid={0}, vId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbVehicle.GetAllUnassignedToVehicleGeozonesInfo(vehicleId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedToVehicleGeozonesInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Set geozone severity to the vehicle.")]
        public int SetGeozoneSeverity(int organizationId, int userId, string SID, Int64 vehicleId,
                                      short geozoneId, short severityId)
        {
            try
            {
                Log(">> SetGeozoneSeverity(uid={0}, vId={1}, GZ={2} sev={3})", userId, vehicleId, geozoneId, severityId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.SetGeozoneSeverity(organizationId, vehicleId, geozoneId, severityId);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleGeozone",
                                                   string.Format("VehicleId={0} AND GeozoneNo=", vehicleId, LoggerManager.GetGeozoneNo(vehicleId, geozoneId)),
                                                   "Set geo zone", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Set vehicle({1}) geozone({0})", vehicleId, geozoneId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetGeozoneSeverity : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Delete GeoZone from a vehicle.")]
        public int DeleteGeozoneFromVehicle(int userId, string SID, int boxId, short geozoneId, ref int rowsAffected)
        {
            try
            {
                Log(">>DeleteGeozoneFromVehicle(uid={0}, boxId={1}, GZ={2} )", userId, boxId, geozoneId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                rowsAffected = 0;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    Int64 vehicleId = dbVehicle.GetVehicleIdByBoxId(boxId);

                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleGeozone",
                                                   string.Format("VehicleId={0}", vehicleId),
                                                   "Delete vehicle geo zone", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Delete vehicle({1}) geozone({0})", vehicleId, geozoneId));

                    rowsAffected = dbVehicle.DeleteGeozoneFromVehicle(vehicleId, geozoneId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleGeozone",
                                                   string.Format("VehicleId={0}", vehicleId),
                                                   "Delete vehicle geo zone", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Delete vehicle({1}) geozone({0})", vehicleId, geozoneId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteGeozoneFromVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Delete GeoZone from a vehicle.")]
        public int DeleteGeozoneFromVehicleByVehicleId(int userId, string SID, int vehicleId, short geozoneId, ref int rowsAffected)
        {
            try
            {
                Log(">>DeleteGeozoneFromVehicleByVehicleId(uid={0}, vehicleId={1}, GZ={2} )", userId, vehicleId, geozoneId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                rowsAffected = 0;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {

                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleGeozone",
                                                   string.Format("VehicleId={0}", vehicleId),
                                                   "Delete vehicle geo zone", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Delete vehicle({1}) geozone({0})", vehicleId, geozoneId));

                    rowsAffected = dbVehicle.DeleteGeozoneFromVehicle(vehicleId, geozoneId);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleGeozone",
                                                   string.Format("VehicleId={0}", vehicleId),
                                                   "Delete vehicle geo zone", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Delete vehicle({1}) geozone({0})", vehicleId, geozoneId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteGeozoneFromVehicleByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add GeoZone to a vehicle.")]
        public int AddGeozoneToVehicle(int userId, string SID, int vehicleId, short geozoneId, short severityId, int speed)
        {
            try
            {
                Log(">>AddGeozoneToVehicle(uid={0}, vehicleId={1}, GZ={2} )", userId, vehicleId, geozoneId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AddGeozone(vehicleId, geozoneId, severityId, speed);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleGeozone",
                                                   string.Format("VehicleId={0}", vehicleId),
                                                   "Add vehicle geo zone", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Add vehicle({1}) to geozone({0})", vehicleId, geozoneId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddGeozoneToVehicle : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves a vehicle max geozones number.")]
        public int GetMaxGeozonesByVehicleId(int userId, string SID, Int64 vehicleId, ref int maxGeozones)
        {
            try
            {
                Log(">> GetMaxGeozonesByVehicleId(uid={0}, vid= {1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    maxGeozones = dbVehicle.GetMaxGeozonesByVehicleId(vehicleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMaxGeozonesByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region Box Configuration Interfaces
        /// <returns>XML format [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName]</returns>
        [WebMethod(Description = "Return communication information by Box ID. XML format [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName]")]
        public int GetCommInfoByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetCommInfoByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxCommInfo = null;
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dsBoxCommInfo = dbBox.GetCommInfoByBoxId(boxId);
                }

                if (Util.IsDataSetValid(dsBoxCommInfo))
                    xml = dsBoxCommInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCommInfoByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <returns>XML
        /// [BoxId],[BoxConfigId],
        ///	[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum], // HW type information
        ///	[BoxProtocolTypeId],[BoxProtocolTypeName], // box protocol type information
        ///	[CommModeId],[CommModeName], // box communication mode information
        ///	[ProtocolPriority]
        /// </returns>
        /// <remarks> Ordered by "ProtocolPriority" field </remarks>
        [WebMethod(Description = "Get box configuration information. XML File: [BoxId],[BoxConfigId],[BoxHwTypeId],[BoxHwTypeName],[MaxSensorsNum],[MaxOutputsNum], [BoxProtocolTypeId],[BoxProtocolTypeName],[SessionTimeOut],[CommModeId],[CommModeName],[ProtocolPriority]")]
        public int GetBoxConfigInfo(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetBoxConfigInfo(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxConfigInfo = null;
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dsBoxConfigInfo = dbBox.GetBoxConfigInfo(boxId);
                }

                if (Util.IsDataSetValid(dsBoxConfigInfo))
                    xml = dsBoxConfigInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxConfigInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <returns>XML [Description],[BoxId],[FwId],[FwName],[FwDateReleased]
        /// </returns>
        /// <remarks> Ordered by "ProtocolPriority" field </remarks>
        [WebMethod(Description = "Get box configuration information. XML File: [Description],[BoxId],[FwId],[FwName],[FwDateReleased]")]
        public int GetActiveVehicleCfgInfo(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetActiveVehicleCfgInfo(uid={0}, vehicleID={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxConfigInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsBoxConfigInfo = dbVehicle.GetActiveVehicleCfgInfo(vehicleId);
                }

                if (Util.IsDataSetValid(dsBoxConfigInfo))
                    xml = dsBoxConfigInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetActiveVehicleCfgInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Get Box Last Communication DateTime
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="SID">Security ID</param>
        /// <param name="boxId">Box ID</param>
        /// <param name="dtLastCommunicated">DateTime</param>
        /// <returns>ASI error code</returns>
        [WebMethod(Description = "Get last box communication datetime.")]
        public int GetLastCommunicationByBoxId(int userId, string SID, int boxId, ref DateTime dtLastCommunicated)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetLastCommunicationByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // get last comm. datetime
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dtLastCommunicated = dbBox.GetBoxLastCommunicatedDateTime(boxId);
                }

                LogFinal("<< GetBoxLastCommunicationInfo(uid={0}, boxId={1}, tSpan={2})",
                             userId, boxId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastCommunicationByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last box communication datetime, sensor mask")]
        public int GetLastBoxCommunicationInfo(int userId, string SID, int boxId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetBoxLastCommunicationInfo(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // get last comm. datetime
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    xml = dbBox.GetBoxLastCommunicationInfo(boxId).GetXml();
                }

                LogFinal("<< GetBoxLastCommunicationInfo(uid={0}, boxId={1}, tSpan={2})",
                               userId, boxId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxLastCommunicationInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Returns vehicle last known position information by BoxId. 
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
        [WebMethod(Description = "Returns vehicle last known position information by Box ID. XML file format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description], [IconTypeId],[IconTypeName],[VehicleTypeName], [LastStatusDateTime],[LastStatusSensor],[LastStatusSpeed],[PrevStatusDateTime],[PrevStatusSensor],[PrevStatusSpeed]")]
        public int GetVehicleLastKnownPositionInfoByBoxId(int userId, string SID, int boxId, string language, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetVehicleLastKnownPositionInfoByBoxId(uId={0}, boxId={1} )", userId, boxId);

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserFleet(userId, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsVehiclesInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehiclesInfo = dbVehicle.GetVehicleLastKnownPositionInfoByBoxId(userId, boxId, language);
                }

                if (Util.IsDataSetValid(dsVehiclesInfo))
                    xml = dsVehiclesInfo.GetXml();

                LogFinal("<< GetVehicleLastKnownPositionInfoByBoxId(uId={0}, boxId={1}, tSpan={2} )",
                                userId, boxId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleLastKnownPositionInfoByBoxId : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update a box attributes.")]
        public int UpdateBoxFeatures(int userId, string SID, int boxId, long mask)
        {
            try
            {
                Log(">>UpdateBoxFeatures(uid={0}, boxId={1}, mask={2})", userId, boxId, mask);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfBox",
                                                   string.Format("BoxId={0}", boxId),
                                                   "Update vehicle info", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Update vehicle info for box({0})", boxId));

                    dbBox.UpdateBoxFeatures(boxId, mask);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBox",
                                                   string.Format("BoxId={0}", boxId),
                                                   "Update vehicle info", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Update vehicle info for box({0})", boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateBoxFeatures : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="fwChnlId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fwChnlId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Return default communication information by  Fw Chnl Id. XML format [CommAddressTypeId],[CommAddressValue],[CommAddressTypeName]")]
        public int GetDefaultCommInfoByFwChnl(int userId, string SID, short fwChnlId, ref string xml)
        {
            try
            {
                Log(">>GetDefaultCommInfoByFwChnl(uid={0}, fwChnlId={1})", userId, fwChnlId);
                LoginManager.GetInstance().SecurityCheck(userId, SID);

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

        #endregion

        #region  IDIOTIC WEB FUNCTIONS
        /// <summary>
        ///   
        /// </summary>
        /// <comment>
        ///      
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="licensePlate"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get last controller status by License Plate.")]
        public int GetLastControllerStatusFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastControllerStatusFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                return GetLastControllerStatusFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last controller status by Box ID.")]
        public int GetLastControllerStatusFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastControllerStatusFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.ControllerStatus);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last TAR mode by License Plate.")]
        public int GetLastTARModeFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastTARModeFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);


                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                return GetLastTARModeFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last TAR mode by Box ID.")]
        public int GetLastTARModeFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastTARModeFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.TARMode);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last TAR mode by Box ID.")]
        public int GetLastKeyFobStatusFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastKeyFobStatusFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                return GetLastKeyFobStatusFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Key Fob Status mode by Box ID.")]
        public int GetLastKeyFobStatusFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastKeyFobStatusFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.KeyFobStatus);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last VCR Delay settings by License Palte.")]
        public int GetLastVCROffDelayFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastVCROffDelayFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                return GetLastVCROffDelayFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last VCR Delay settings by Box ID.")]
        public int GetLastVCROffDelayFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastVCROffDelayFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.VCROffDelay);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Setup Settings by License Plate.")]
        public int GetLastBoxSetupFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastBoxSetupFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                return GetLastBoxSetupFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Setup Settings by Box Id.")]
        public int GetLastBoxSetupFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastBoxSetupFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.BoxSetup);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Setup Settings by License Plate, support extended setup.")]
        public int GetLastBoxSetupFromHistoryByLicensePlateExtended(int userId, string SID, string licensePlate, ref string xml, bool isExtendedVersion)
        {
            try
            {
                Log(">>GetLastBoxSetupFromHistoryByLicensePlateExtended(uid={0}, LP={1}, isExtendedVersion={2} )", userId, licensePlate, isExtendedVersion);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                return GetLastBoxSetupFromHistoryByBoxIdExtended(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml, isExtendedVersion);
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxSetupFromHistoryByLicensePlateExtended : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Setup Settings by Box Id, support extended setup.")]
        public int GetLastBoxSetupFromHistoryByBoxIdExtended(int userId, string SID, int boxId, ref string xml, bool isExtendedVersion)
        {
            try
            {
                Log(">>GetLastBoxSetupFromHistoryByBoxIdExtended(uid={0}, boxId={1}, isExtendedVersion={2})",
                          userId, boxId, isExtendedVersion);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                VLF.CLS.Def.Enums.MessageType messageType = isExtendedVersion ? VLF.CLS.Def.Enums.MessageType.SendExtendedSetup : VLF.CLS.Def.Enums.MessageType.BoxSetup;
                xml = GetLastSingleMessageFromHistory(userId, boxId, messageType);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxSetupFromHistoryByBoxIdExtended : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Status by License Plate.")]
        public int GetLastBoxStatusFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastBoxStatusFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                return GetLastBoxStatusFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxStatusFromHistoryByLicensePlate : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Status by Box Id.")]
        public int GetLastBoxStatusFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastBoxStatusFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.BoxStatus);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxStatusFromHistoryByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Status by License Plate, support extended status.")]
        public int GetLastBoxStatusFromHistoryByLicensePlateExtended(int userId, string SID, string licensePlate, ref string xml, bool isExtendedVersion)
        {
            try
            {
                Log(">>GetLastBoxStatusFromHistoryByLicensePlateExtended(uid={0}, LP={1}, isExtendedVersion={2} )",
                          userId, licensePlate, isExtendedVersion);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                return GetLastBoxStatusFromHistoryByBoxIdExtended(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml, isExtendedVersion);
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxStatusFromHistoryByLicensePlateExtended : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Box Status by Box Id, support extended status.")]
        public int GetLastBoxStatusFromHistoryByBoxIdExtended(int userId, string SID, int boxId, ref string xml, bool isExtendedVersion)
        {
            try
            {
                Log(">>GetLastBoxStatusFromHistoryByBoxIdExtended(uid={0}, boxId={1}, isExtendedVersion={2})",
                                            userId, boxId, isExtendedVersion);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                VLF.CLS.Def.Enums.MessageType messageType = isExtendedVersion ? VLF.CLS.Def.Enums.MessageType.SendExtendedStatus : VLF.CLS.Def.Enums.MessageType.BoxStatus;
                xml = GetLastSingleMessageFromHistory(userId, boxId, messageType);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxStatusFromHistoryByBoxIdExtended : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Geozone settings by License Plate, support extended status.")]
        public int GetLastGeoZoneIDsFromHistoryByLicensePlate(int userId, string SID, string licensePlate, ref string xml)
        {
            try
            {
                Log(">>GetLastGeoZoneIDsFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                return GetLastGeoZoneIDsFromHistoryByBoxId(userId, SID, GetBoxIDByLicensePlate(licensePlate), ref xml);
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleInfoXMLByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get last Geozone settings by Box ID.")]
        public int GetLastGeoZoneIDsFromHistoryByBoxId(int userId, string SID, int boxId, ref string xml)
        {
            try
            {
                Log(">>GetLastGeoZoneIDsFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xml = GetLastSingleMessageFromHistory(userId, boxId, VLF.CLS.Def.Enums.MessageType.GeoZoneIDs);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastGeoZoneIDsFromHistoryByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        /// <summary>
        ///      
        /// </summary>
        /// <comments>
        ///      GB -> see below, the eternal copy and paste!!!
        /// </comments>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="xmlBoxIDs"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get last Upload Firmware status.")]
        public int GetLastUploadFirmwareStatusFromHistory(int userId, string SID, DateTime fromDateTime,
                                                          string xmlBoxIDs, ref string xml)
        {
            try
            {
                Log(">>GetLastUploadFirmwareStatusFromHistory(uid={0}, BoxIDs = '{1}', fromDateTime='{2}' )", userId, xmlBoxIDs, fromDateTime.ToString());

                // 1. Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                StringReader strrXML = new StringReader(xmlBoxIDs);
                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);

                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    foreach (DataRow rowItem in ds.Tables[0].Rows)
                    {
                        //Authorization
                        if (!dbUser.ValidateUserBoxOne(userId, Convert.ToInt32(rowItem["BoxId"])))
                            return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                    }
                }


                DataSet dsResult = new DataSet();

                // 2. Parse xml(dsBoxIDs) to dataset
                strrXML = new StringReader(xmlBoxIDs);
                DataSet dsBoxIDs = new DataSet();
                dsBoxIDs.ReadXml(strrXML);

                // 3. Retrieve last UploadFirmwareStatus for each box
                if (Util.IsDataSetValid(dsBoxIDs))
                {
                    DataSet dsCurrResult = null;

                    using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                    {
                        foreach (DataRow ittr in dsBoxIDs.Tables[0].Rows)
                        {
                            dsCurrResult = dbMessageQueue.GetLastCommandFromHistory(Convert.ToInt32(ittr["BoxId"]), fromDateTime);
                            // 4. Add result to resDataSet
                            if (Util.IsDataSetValid(dsCurrResult))
                                dsResult.Merge(dsCurrResult);
                        }
                    }

                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastUploadFirmwareStatusFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///         if it is not authorized for one of the boxes fails for ALL
        /// </summary>
        /// <comment>
        ///      GB -> how do you know the format for xmlBoxIDs is good 
        ///      Michael, take a look at the previous loop in foreach and tell mw what was wrong !!!
        ///               why do you read again the string !!!
        ///      that should come from a store procedure !!!
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="xmlBoxIDs"></param>
        /// <param name="xml"></param>
        /// <returns>
        ///         InterfaceError.InvalidParameter -> data is not good
        /// </returns>
        [WebMethod(Description = "Get last Upload Firmware status.")]
        public int GetLastUploadFirmwareMessageFromHistory(int userId, string SID,
                                                           DateTime fromDateTime, string xmlBoxIDs, ref string xml)
        {
            try
            {
                Log(">>GetLastUploadFirmwareStatusFromHistory(uid={0}, BoxIDs= '{1}', dtFrom={2})",
                               userId, xmlBoxIDs, fromDateTime);

                // 1. Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                StringReader strrXML = new StringReader(xmlBoxIDs);
                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);

                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    foreach (DataRow rowItem in ds.Tables[0].Rows)
                    {
                        //Authorization
                        if (!dbUser.ValidateUserBoxOne(userId, Convert.ToInt32(rowItem["BoxId"])))
                            return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                    }
                }

                DataSet dsResult = new DataSet();
                // 2. Parse xml(dsBoxIDs) to dataset
                strrXML = new StringReader(xmlBoxIDs);
                DataSet dsBoxIDs = new DataSet();
                dsBoxIDs.ReadXml(strrXML);
                // 3. Retrieve last UploadFirmwareStatus for each box
                if (Util.IsDataSetValid(dsBoxIDs))
                {
                    DataSet dsCurrResult = null;
                    using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                    {
                        foreach (DataRow ittr in dsBoxIDs.Tables[0].Rows)
                        {
                            dsCurrResult =
                                  dbMessageQueue.GetLastUploadFirmwareMessageFromHistory(userId,
                                                                                        Convert.ToInt32(ittr["BoxId"]),
                                                                                        fromDateTime);
                            // 4. Add result to resDataSet
                            if (Util.IsDataSetValid(dsCurrResult))
                                dsResult.Merge(dsCurrResult);
                        }
                    }
                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                }
                else
                    return Convert.ToInt32(InterfaceError.InvalidParameter);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastUploadFirmwareStatusFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Update a Box location
        /// </summary>
        /// <comment>
        ///      GB -> REMOVE IT FROM WEB SERVICES !!!
        /// </comment>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="boxId"></param>
        /// <param name="originDateTime"></param>
        /// <param name="streetAddress"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a Box location (Address resolution).")]
        public int UpdateBoxStreetAddressInHistory(int userId, string SID, int boxId, DateTime originDateTime, string streetAddress, double latitude, double longitude)
        {
            try
            {
                Log(">> UpdateBoxStreetAddressInHistory(uid={0}, boxId={1}, originDateTime = '{2}', streetAddress = '{3}' , latitude={4}, longitude={5} )", userId, boxId, originDateTime.ToString(), streetAddress.TrimEnd(), latitude, longitude);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (string.IsNullOrEmpty(streetAddress))
                    throw new ERR.ASIDataNotFoundException("Unable to update street address with empty value.");

                using (SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    #region Retrieves map engine
                    DataSet dsGeoCodeInfo = dbSystemConfig.GetBoxGeoCodeEngineInfo(boxId);
                    if (Util.IsDataSetValid(dsGeoCodeInfo))
                    {
                        VLF.MAP.ClientMapProxy map = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeInfo));

                        DateTime dtNow = DateTime.Now.AddHours(-AppConfig.GetInstance().ServerTimeZone);
                        using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                        {
                            #region Retrieves nearest landmark
                            DataSet dsLandmarks = dbMessageQueue.GetLandMarksInfoByBoxId(boxId);
                            string nearestLandmark = "";
                            if (Util.IsDataSetValid(dsLandmarks))
                                DAS.DB.Report.PrepareStreetAddress(latitude, longitude,
                                                                   dsLandmarks.Tables[0], map, ref nearestLandmark);
                            #endregion

                            /// Update Street Address
                            dbMessageQueue.UpdateStreetAddressInBoxAndHistory(boxId, originDateTime, streetAddress,
                                                                300, Convert.ToInt16(dtNow.Year), Convert.ToInt16(dtNow.Month),
                                                                nearestLandmark, (short)map.LastUsedGeoCodeID);
                            LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfBox",
                                                   string.Format("BoxId={0}", boxId),
                                                   "Update street address", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Update street address({1}) for box({0})", boxId, streetAddress));
                        }
                    }
                    #endregion
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateBoxStreetAddressInHistory : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Gets any supported message by message type
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="SID">Security ID</param>
        /// <param name="licensePlate">License Plate</param>
        /// <param name="messageType">Message Type</param>
        /// <param name="xml">Referenced xml result</param>
        /// <returns>ASI Error code</returns>
        [WebMethod(Description = "Get last message from a history table by license plate")]
        public int GetLastBoxMessageFromHistoryByLicensePlate(int userId, string SID, string licensePlate,
                                                              int messageTypeCode, ref string xml)
        {
            try
            {
                Log(">> GetLastBoxMessageFromHistoryByLicensePlate(uid={0}, LP={1})", userId, licensePlate);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xml = GetLastSingleMessageFromHistory(userId, GetBoxID(userId, licensePlate),
                                                       (VLF.CLS.Def.Enums.MessageType)messageTypeCode);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxMessageFromHistoryByLicensePlate : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Gets any supported message by message type
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="SID">Security ID</param>
        /// <param name="boxId">Box Id</param>
        /// <param name="messageType">Message Type</param>
        /// <param name="xml">Referenced xml result</param>
        /// <returns>ASI Error code</returns>
        [WebMethod(Description = "Get last message from a history table by Box Id.")]
        public int GetLastBoxMessageFromHistoryByBoxId(int userId, string SID, int boxId,
                                                       int messageTypeCode, ref string xml)
        {
            try
            {
                Log(">> GetLastBoxMessageFromHistoryByBoxId(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xml = GetLastSingleMessageFromHistory(userId, boxId, (VLF.CLS.Def.Enums.MessageType)messageTypeCode);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastBoxMessageFromHistoryByBoxId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        protected string GetLastSingleMessageFromHistory(int userId, int boxId, VLF.CLS.Def.Enums.MessageType messageType)
        {
            string xml = "";

            DataSet dsMsgInHst = null;
            using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
            {
                dsMsgInHst = dbMessageQueue.GetLastSingleMessageFromHistory(userId, boxId, messageType);
                if (Util.IsDataSetValid(dsMsgInHst))
                    xml = dsMsgInHst.GetXml();
            }

            return xml;
        }

        /// <summary>
        /// Get box id by vehicle license plate
        /// </summary>
        /// <param name="licensePlate">License Plate</param>
        /// <returns>Box ID</returns>
        private int GetBoxID(int userId, string licensePlate)
        {
            int boxId = VLF.CLS.Def.Const.unassignedIntValue;
            using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
            {
                boxId = dbVehicle.GetBoxIdByLicensePlate(licensePlate);
            }

            return boxId;
        }

        #endregion     IDIOTIC WEB FUNCTIONS

        #region Vehicle Maintenance Information Interfaces

        // Changes for TimeZone Feature start

        [WebMethod(Description = "Retrieves vehicle maintenance information . XML File format :[BoxId],[VehicleId],[Description],[LastSrvOdo],[MaxSrvInterval],[LastSrvEngHrs],[EngHrsSrvInterval],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],[NextServiceDescription],[VehicleTypeId]")]
        [Obsolete("Obsolete method")]
        public int GetVehicleMaintenanceInfoXML_NewTZ(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetVehicleMaintenanceInfoXML(uid={0}, vId={1})", userId, vehicleId);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleMaintenanceInfo_NewTZ(vehicleId, userId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleMaintenanceInfoXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves vehicle maintenance information . XML File format :[BoxId],[VehicleId],[Description],[LastSrvOdo],[MaxSrvInterval],[LastSrvEngHrs],[EngHrsSrvInterval],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],[NextServiceDescription],[VehicleTypeId]")]
        [Obsolete("Obsolete method")]
        public int GetVehicleMaintenanceInfoXML(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetVehicleMaintenanceInfoXML(uid={0}, vId={1})", userId, vehicleId);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleMaintenanceInfo(vehicleId, userId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleMaintenanceInfoXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add vehicle maintenanceInfo to system.")]
        [Obsolete("Obsolete method")]
        public int AddVehicleMaintenanceInfo(int userId, string SID, Int64 vehicleId, double lastSrvOdo, double currentOdo,
                                            double maxSrvInterval, int lastSrvEngHrs, int currentEngHrs, int engHrsSrvInterval,
                                            string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving,
                                            string nextServiceDescription)
        {
            try
            {
                Log(">> AddVehicleMaintenaceInfo( uid={0}, vid= {1}, lastSrvOdo={2}, maxSrvInterval={3}, lastSrvEngHrs={4}, engHrsSrvInterval={5}, email = '{6}', timeZone={7}, dayLightSaving={8}, autoAdjustDayLightSaving={9}, CurrentOdo = (10), CurrentEngHrs={11}, NextServiceDescription = '{12}')", userId, vehicleId, lastSrvOdo, maxSrvInterval, lastSrvEngHrs, engHrsSrvInterval, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, currentOdo, currentEngHrs, nextServiceDescription);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AddVehicleMaintenanceInfo(vehicleId, lastSrvOdo, currentOdo, maxSrvInterval,
                                                        lastSrvEngHrs, currentEngHrs, engHrsSrvInterval, email, timeZone,
                                                        dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicleMaintenaceInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update vehicle maintenance info to system incl. odometer.")]
        [Obsolete("Obsolete method")]
        public int UpdateVehicleMaintenanceInfo(int userId, string SID, Int64 vehicleId,
           double lastSrvOdo, double currentOdo, double maxSrvInterval,
           string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving, string nextServiceDescription)
        {
            try
            {
                Log(">> UpdateVehicleMaintenanceInfo(uid={0}, vid={1}, lastSrvOdo={2}, maxSrvInterval={3}, email={4}, timeZone={5}, dayLightSaving={6}, autoAdjustDayLightSaving={7}, CurrentOdo=(8), NextServiceDescription='{9}')",
                               userId, vehicleId, lastSrvOdo, maxSrvInterval, email, timeZone, dayLightSaving,
                               autoAdjustDayLightSaving, currentOdo, nextServiceDescription);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance", vehicleId));

                    dbVehicle.UpdateVehicleMaintenanceInfo(vehicleId, lastSrvOdo, currentOdo, maxSrvInterval,
                       email, timeZone, dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleMaintenanceInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update vehicle maintenance info to system incl. engine hours.")]
        [Obsolete("Obsolete method")]
        public int UpdateVehicleEngineMaintenanceInfo(int userId, string SID, Int64 vehicleId,
                                                     int lastSrvEngHrs, int currentEngHrs, int engHrsSrvInterval,
                                                     string email, int timeZone, short dayLightSaving, short autoAdjustDayLightSaving,
                                                     string nextServiceDescription)
        {
            try
            {
                Log(">> UpdateVehicleMaintenanceInfo(uid={0}, vid={1}, lastSrvEngHrs={2}, engHrsSrvInterval={3}, email={4}, timeZone={5}, dayLightSaving={6}, autoAdjustDayLightSaving={7}, CurrentEngHrs={8}, NextServiceDescription={9})",
                   userId, vehicleId, lastSrvEngHrs, engHrsSrvInterval, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, currentEngHrs, nextServiceDescription);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance", vehicleId));

                    dbVehicle.UpdateVehicleEngineMaintenanceInfo(vehicleId,
                                      lastSrvEngHrs, currentEngHrs, engHrsSrvInterval,
                                      email, timeZone, dayLightSaving, autoAdjustDayLightSaving, nextServiceDescription);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleMaintenanceInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update vehicle odometer maintenance short info to system.")]
        [Obsolete("Obsolete method")]
        public int UpdateVehicleMaintenanceShortInfo(int userId, string SID, Int64 vehicleId, double lastSrvOdo, string nextServiceDescription)
        {
            try
            {
                Log(">> UpdateVehicleMaintenanceShortInfo(uid={0}, vid={1}, lastSrvOdo={2}, NextServiceDescription={3})",
                               userId, vehicleId, lastSrvOdo, nextServiceDescription);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance", vehicleId));

                    dbVehicle.UpdateVehicleMaintenanceInfo(vehicleId, lastSrvOdo, nextServiceDescription);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleMaintenanceShortInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update vehicle engine maintenance short info to system.")]
        [Obsolete("Obsolete method")]
        public int UpdateVehicleEngineMaintenanceShortInfo(int userId, string SID, Int64 vehicleId, int lastSrvEngHrs, string nextServiceDescription)
        {
            try
            {
                Log(">> UpdateVehicleEngineMaintenanceShortInfo( uid={0}, vid={1}, lastSrvEngHrs={2}, NextServiceDescription={3})",
                         userId, vehicleId, lastSrvEngHrs, nextServiceDescription);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance short info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance short info", vehicleId));

                    dbVehicle.UpdateVehicleEngineMaintenanceInfo(vehicleId, lastSrvEngHrs, nextServiceDescription);

                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfVehicleMaintenance",
                                                  string.Format("VehicleId={0}", vehicleId),
                                                  "Update vehicle engine maintenance short info", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Update vehicle({0}) engine maintenance short info", vehicleId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleEngineMaintenanceShortInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add vehicle odometer maintenance history to system.")]
        public int AddVehicleMaintenanceHst(int userId, string SID, Int64 vehicleId, DateTime serviceDateTime, string serviceDescription,
           double serviceOdo)
        {
            try
            {
                Log(">>AddVehicleMaintenanceHst(uid={0}, vid={1}, srvDateTime={2}, srvDescription={3}, srvOdo={4})",
                      userId, vehicleId, serviceDateTime, serviceDescription, serviceOdo);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AddVehicleMaintenanceHst(vehicleId, userId, serviceDateTime, serviceDescription, serviceOdo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicleMaintenanceHst : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add vehicle engine maintenance history to system.")]
        public int AddVehicleEngineMaintenanceHst(int userId, string SID, Int64 vehicleId, DateTime serviceDateTime, string serviceDescription,
           int serviceEngineHours)
        {
            try
            {
                Log(">> AddVehicleEngineMaintenanceHst( uid={0}, vid={1}, serviceDateTime = '{2}', serviceDescription = '{3}', serviceEngineHours={4})",
                   userId, vehicleId, serviceDateTime, serviceDescription, serviceEngineHours);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AddVehicleEngineMaintenanceHst(vehicleId, userId, serviceDateTime, serviceDescription, serviceEngineHours);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddVehicleEngineMaintenanceHst : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update vehicle maintenance history to system.")]
        public int UpdateVehicleMaintenanceHst(int userId, string SID, Int64 vehicleId,
                                               DateTime serviceDateTime, string serviceDescription, double serviceOdo)
        {
            try
            {
                Log(">>UpdateVehicleMaintenanceHst( uid={0}, vid= {1}, serviceDateTime = '{2}', serviceDescription = '{3}', serviceOdo={4})", userId, vehicleId, serviceDateTime, serviceDescription, serviceOdo);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.UpdateVehicleMaintenanceHst(vehicleId, serviceDateTime, serviceDescription, serviceOdo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleMaintenanceHst : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle maintenance history . XML File format :[VehicleId],[ServiceDateTime],[ServiceDescription],[ServiceOdo]")]
        public int GetVehicleMaintenanceHistoryXML(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleMaintenanceHistoryXML(uid={0}, vId={1})", userId, vehicleId);


                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleMaintenanceHistory(vehicleId, userId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleMaintenanceHistoryXML : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Max 2008-03-04 new methods
        [WebMethod(Description = "Add New Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlan_Add(int userId, string SID, int organizationId, long vehicleId, int serviceId,
           short operationTypeId, int notificationId, short frequency, int serviceValue, int serviceInterval, int endValue,
           string email, string description, string comments)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_Add(uid={0}, orgId={1}, serv.Id={2}, descr.={3})",
                                                       userId, organizationId, serviceId, description);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.AddVehicleService(userId, vehicleId, serviceId, operationTypeId, notificationId, frequency,
                                                serviceValue, serviceInterval, endValue, email, description, comments);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_Add : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }





        [WebMethod(Description = "Update Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlan_Update(int userId, string SID, int organizationId, int serviceId, long vehicleId,
           int serviceTypeId, short operationTypeId, int notificationId, short frequency, int serviceValue, int serviceInterval,
           int endValue, string email, string description, string comments)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_Update(uid={0}, orgId={1}, serv.Id={2}, descr.={3})",
                      userId, organizationId, serviceId, description);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.UpdateVehicleService(userId, serviceId, vehicleId, serviceTypeId, operationTypeId, notificationId,
                                                  frequency, serviceValue, serviceInterval, endValue, email, description, comments);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_Update : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete All Vehicle Maintenance Plans")]
        public int VehicleMaintenancePlan_DeleteAll(int userId, string SID, int organizationId, long vehicleId)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_DeleteAll(uid={0}, orgId={1}, vid={2})",
                      userId, organizationId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.DeleteVehicleServices(organizationId, vehicleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_DeleteAll : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlan_Delete(int userId, string SID, int organizationId, long vehicleId, int serviceId)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_Delete(uid={0}, orgId={1}, vid={2}, serviceId={3})",
                      userId, organizationId, vehicleId, serviceId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.DeleteVehicleService(userId, vehicleId, serviceId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_Delete : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Close Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlan_Close(int userId, string SID, int organizationId, long vehicleId, int serviceId)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_Close(uid={0}, orgId={1}, vid={2}, serviceId={3})",
                      userId, organizationId, vehicleId, serviceId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.CloseVehicleService(userId, vehicleId, serviceId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_Close : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Close Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlanExtended_Close(int userId, string SID, int organizationId, long vehicleId, int serviceId, int serviceValue, Int16 closeType, string comments)
        {
            try
            {
                Log(">> VehicleMaintenancePlanExtended_Close(uid={0}, orgId={1}, vId={2}, serviceId={3})",
                       userId, organizationId, vehicleId, serviceId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.CloseVehicleService_Extended(userId, vehicleId, serviceId, serviceValue, closeType, comments);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlanExtended_Close : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Vehicle's All Maintenance Plans")]
        public int VehicleMaintenancePlan_GetAll(int userId, string SID, int organizationId, long vehicleId, short dueFlag, ref string xmlString)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_GetAll(uid={0}, orgId={1}, vid={2})",
                      userId, organizationId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbVehicle.GetVehicleServices(vehicleId, dueFlag, userId);
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
                LogException("<< VehicleMaintenancePlan_GetAll : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Vehicle's All Maintenance Plans")]
        public int VehicleMaintenancePlan_GetAllByLang(int userId, string SID, int organizationId, long vehicleId, short dueFlag, string lang, ref string xmlString)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_GetAllByLang(uid={0}, orgId={1}, vid={2})",
                      userId, organizationId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbVehicle.GetVehicleServices(vehicleId, dueFlag, userId);
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
                LogException("<< VehicleMaintenancePlan_GetAllByLang : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Vehicle's Maintenance Plans By Type")]
        public int VehicleMaintenancePlan_GetByType(int userId, string SID, int organizationId, long vehicleId, short operationType, short dueFlag, ref string xmlString)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_GetByType(uid={0}, orgId={1}, vid= {2})",
                      userId, organizationId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbVehicle.GetVehicleServicebyTypeId(vehicleId, operationType, dueFlag, userId);
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
                LogException("<< VehicleMaintenancePlan_GetByType : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Vehicle's Maintenance Plans By Type")]
        public int VehicleMaintenancePlan_GetByTypeLang(int userId, string SID, int organizationId, long vehicleId, short operationType, short dueFlag, string lang, ref string xmlString)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_GetByTypeLang(uid={0}, orgId={1}, vid= {2})",
                      userId, organizationId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable dtResult = dbVehicle.GetVehicleServicebyTypeId(vehicleId, operationType, dueFlag, userId);
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
                LogException("<< VehicleMaintenancePlan_GetByTypeLang : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Vehicle Maintenance Plan")]
        public int VehicleMaintenancePlan_Get(int userId, string SID, int organizationId, long vehicleId, int serviceId, ref string xmlString)
        {
            try
            {
                Log(">> VehicleMaintenancePlan_Get(uid={0}, orgId={1}, vid={2}, serviceId={3})",
                      userId, organizationId, vehicleId, serviceId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    //DataSet dsResult = dbVehicle.GetVehicleService(serviceId);
                    DataSet dsResult = dbVehicle.GetVehicleService(vehicleId, serviceId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "VehicleMaintenance";
                        dsResult.Tables[0].TableName = "MaintenancePlans";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenancePlan_Get : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Vehicle's All Maintenance Plans History")]
        public int VehicleMaintenancePlanHistory_GetAll(int userId, string SID, int organizationId,
                                                        long vehicleId, ref string xmlString)
        {
            try
            {
                Log(">> VehicleMaintenancePlanHistory_GetAll(uid={0}, orgId={1}, vId={2})",
                       userId, organizationId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbVehicle.GetVehicleServicesHistory(vehicleId, userId);
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
                LogException("<< VehicleMaintenancePlanHistory_GetAll : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Operation Types")]
        public int VehicleOperationTypes_Get(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> VehicleOperationTypes_Get(uid={0}, orgId={1})", userId, organizationId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    if (!dbUser.ValidateUserOrganization(userId, organizationId))
                        return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                }

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbVehicle.GetOperationTypes();
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "VehicleOperationTypes";
                        dsResult.Tables[0].TableName = "OperationTypes";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleOperationTypes_Get : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update vehicle engine hours")]
        public int VehicleMaintenance_UpdateEngineHours(int userId, string SID, Int64 vehicleId, int engineHours)
        {
            try
            {
                Log(">> VehicleMaintenance_UpdateEngineHours( uid={0}, vId={1}, engineHours={2})",
                    userId, vehicleId, engineHours);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.UpdateVehicleCurrentEngHrsInfo(vehicleId, engineHours);
                    dbVehicle.AddVehicleEngineMaintenanceHst(vehicleId, userId, DateTime.UtcNow, "Update Engine Hours", engineHours);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenance_UpdateEngineHours : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves vehicle maintenance information. XML File format :[BoxId],[VehicleId],[Description],[LastSrvOdo],[MaxSrvInterval],[LastSrvEngHrs],[EngHrsSrvInterval],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],[NextServiceDescription],[VehicleTypeId]")]
        public int VehicleMaintenance_GetInfo(int userId, string SID, Int64 vehicleId, bool includeNotInit, ref string xml)
        {
            try
            {
                Log(">> VehicleMaintenance_GetInfo(uid={0}, vid= {1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehicleInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleInfo = dbVehicle.GetVehicleMaintenance(vehicleId, includeNotInit, userId);
                }

                if (Util.IsDataSetValid(dsVehicleInfo))
                    xml = dsVehicleInfo.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleMaintenance_GetInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Retrieves MDT Manufacturer and Model by MDT  Type")]
        public int GetMDTInfoByTypeId(int userId, string SID, int MdtTypeId, ref string xml)
        {
            try
            {
                Log(">> GetMDTInfoByTypeId(uid={0}, vid= {1})", userId, MdtTypeId);


                DataSet dsMdtInfo = null;
                using (TxtMsgs txtMsgs = new TxtMsgs(LoginManager.GetConnnectionString(userId)))
                {
                    dsMdtInfo = txtMsgs.GetMDTInfoByTypeId(MdtTypeId);
                }

                if (Util.IsDataSetValid(dsMdtInfo))
                    xml = dsMdtInfo.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMDTInfoByTypeId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get DTC Codes Description")]
        public int GetDTCCodesDescription(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetDTCCodesDescription(userId={0})",
                      userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet ds = null;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    ds = dbVehicle.GetDTCCodesDescription();
                }

                if (Util.IsDataSetValid(ds))
                    xml = ds.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleDTCCodeDescription : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get BoxId based on BlackBerry PIN")]
        public int GetBoxId4BB(int userId, string SID, string PIN, ref int boxId)
        {
            try
            {
                Log(">> GetBoxId4BB(userId={0},SID={1}, PIN={2}, boxId={3} )",
                         userId, SID, PIN, boxId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    boxId = dbBox.GetBoxId4BB(Convert.ToInt32(PIN));
                }

                return boxId;
                /// return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxId4BB : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Vehicle BSM Maintenance History")]
        public int GetVehicleExtraServiceHistoryByVehicleId(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetVehicleExtraServiceHistoryByVehicleId(userId={0},vehicleId={1} )",
                         userId, vehicleId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet ds = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    ds = dbVehicle.GetVehicleExtraServiceHistoryByVehicleId(vehicleId);
                }

                if (Util.IsDataSetValid(ds))
                    xml = ds.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleExtraServiceHistoryByVehicleId : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Add vehicle odometer maintenance history to system.")]
        public int VehicleExtraServiceHistory_Add_Update(int userId, string SID, Int64 VehicleId, string Field1, string Field2, string Field3, string Field4, string Field5, string Field6, string Field7,
             string Field8, string Field9, string Field10, string Field11, string Field12, string Field13, string Field14, string Field15, string Field16)
        {
            try
            {
                Log(">>VehicleExtraServiceHistory_Add_Update(uid={0}, vid={1})",
                      userId, VehicleId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dbVehicle.VehicleExtraServiceHistory_Add_Update(VehicleId, Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8, Field9, Field10, Field11, Field12, Field13, Field14, Field15, Field16);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleExtraServiceHistory_Add_Update : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Get Vehicle DTC Codes")]
        public int GetJ1708CodesVehicleFleet_NewTZ(int userId, string SID, int vehicleId, int fleetId, DateTime from, DateTime to, string lang, ref string xml)
        {
            try
            {
                Log(">> GetJ1708CodesVehicleFleet(userId={0},vehicleId={1},fleetId={2} )",
                         userId, vehicleId, fleetId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet ds = null;
                DataSet dsVehicle = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    ds = dbVehicle.GetJ1708CodesVehicleFleet_NewTZ(userId, vehicleId, fleetId, from, to, lang);
                }

                if (Util.IsDataSetValid(ds))
                    xml = ds.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetJ1708CodesVehicleFleet : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end

        [WebMethod(Description = "Get Vehicle DTC Codes")]
        public int GetJ1708CodesVehicleFleet(int userId, string SID, int vehicleId, int fleetId, DateTime from, DateTime to, string lang, ref string xml)
        {
            try
            {
                Log(">> GetJ1708CodesVehicleFleet(userId={0},vehicleId={1},fleetId={2} )",
                         userId, vehicleId, fleetId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet ds = null;
                DataSet dsVehicle = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    ds = dbVehicle.GetJ1708CodesVehicleFleet(userId, vehicleId, fleetId, from, to, lang);
                }

                if (Util.IsDataSetValid(ds))
                    xml = ds.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetJ1708CodesVehicleFleet : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Box Extra Information. Including: Status, Setup")]
        public int GetBoxExtraInfo(int userId, string SID, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetBoxExtraInfo(userId={0},vehicleId={1} )",
                         userId, vehicleId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet ds = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    ds = dbVehicle.GetBoxExtraInfo(vehicleId);
                }

                if (Util.IsDataSetValid(ds))
                    xml = ds.GetXml();
                else
                    xml = "";

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetBoxExtraInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        #endregion

        [WebMethod(Description = "It returns the mileage on every state the vehicle was in for the specified period.")]
        public int GetStateMileagePerVehicle(Int32 userId, string SID, string licensePlate, string FromDate, string ToDate, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetStateMileagePerVehicle(uId={0}, dtFrom={1}, dtTo={2}, licensePlate={3})", userId, FromDate, ToDate, licensePlate);

                // Authenticate 
                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet ds = new DataSet();


                using (VLF.DAS.Logic.Report rpt = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(userId)))
                {
                    ds = rpt.GetStateMileagePerVehicle(licensePlate, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), 3);
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

                LogFinal("<< GetStateMileagePerVehicle(uId={0}, dtFrom={1}, dtTo={2}, licensePlate={3}, tSpan={4})",
                                  userId, FromDate, ToDate, licensePlate, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetStateMileagePerFleet: uId={0}, EXC={1}, tSpan={2}", userId, Ex.Message, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #region MdtOTA
        /// <summary>
        ///            Add new MdtOTA process record
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="typeId"></param>
        /// <param name="customProp"></param>
        [WebMethod(Description = "Start MdtOTA task")]
        public int AddMdtOTA(int userId, string SID, int boxId, Int16 typeId, string customProp)
        {
            try
            {
                Log(">> AddMdtOTA(uid={0}, boxId={1})", userId, boxId);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // get last comm. datetime
                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dbBox.AddMdtOTA(boxId, typeId, customProp);
                    LoggerManager.RecordUserAction("Vehicle", userId, 0, "vlfMdtOTA",
                                                   string.Format("BoxId={0} AND TypeId={1} AND CustomProp='{2}'", boxId, typeId, customProp),
                                                   "Add MdtOTA", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Add MdtOTA for box({0})", boxId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddMdtOTA : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion


        [WebMethod(Description = "Returns ServiceConfigId and ServiceName (PostSpeedOver*).")]
        public int GetPostedSpeedServiceConfiguration(Int32 userId, string SID, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetPostedSpeedServiceConfiguration(uId={0})", userId);

                // Authenticate 
                if (!ValidateUserPostedSpeedServiceConfiguration(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet ds = new DataSet();
                //string sql = "SELECT ServiceConfigId, ServiceName FROM vlfServices WHERE ServiceConfigId IN (40,41,42)";
		        string sql = "GetPostedSpeedServiceConfigurationSettings";
                //Executes SQL statement
                using (System.Data.SqlClient.SqlConnection oSqlConnection = new System.Data.SqlClient.SqlConnection(LoginManager.GetConnnectionString(userId)))
                {
                    if (oSqlConnection.State != ConnectionState.Open) oSqlConnection.Open();

                    using (System.Data.SqlClient.SqlCommand oSqlCommand = new System.Data.SqlClient.SqlCommand(sql, oSqlConnection))
                    {
                        using (System.Data.SqlClient.SqlDataAdapter oSqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(oSqlCommand))
                        {
                            oSqlDataAdapter.Fill(ds);
                            ds.Tables[0].TableName = "PostedSpeedServiceConfiguration";
                        }
                    }

                    if (oSqlConnection.State != ConnectionState.Closed) oSqlConnection.Close();
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

                LogFinal("<< GetPostedSpeedServiceConfiguration(uId={0}, tSpan={1})", userId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetPostedSpeedServiceConfiguration: uId={0}, EXC={1}, tSpan={2}", userId, Ex.Message, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        private bool ValidateUserPostedSpeedServiceConfiguration(int userId, string SID)
        {
            bool ret = false;
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            //using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            //{
            //    return dbUser.ValidateUserFleet(userId, fleetId);
            //}

            ValidationResult res = SecurityKeyManager.GetInstance().ValidatePasskey(userId, SID);

            switch (res)
            {
                case ValidationResult.Failed:
                case ValidationResult.Expired:
                case ValidationResult.CallFrequencyExceeded:
                    ret = false;
                    break;
                default:
                    ret = true;
                    break;
            }

            return ret;
        }

        #region Vehicle Device Status

        [WebMethod(Description = "Returns XML of vehicles statuses. XML Format: [VehicleDeviceStatusID],[VehicleDeviceStatus]")]
        public int GetVehicleDeviceStatuses(int userId, string SID, int VehicleId, ref string xml)
        {
            try
            {
                Log(">>GetVehicleDeviceStatuses(uid={0}, VehicleId={1})", userId.ToString(), VehicleId.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsVehicleStatuses = new DataSet();
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicleStatuses = dbVehicle.GetVehicleDeviceStatuses(userId, VehicleId);
                }


                if (Util.IsDataSetValid(dsVehicleStatuses))
                    xml = dsVehicleStatuses.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleDeviceStatuses : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns updated VehicleId.")]
        public int UpdateVehicleDeviceStatus(int userId, int loginUserId, string SID, int VehicleDeviceStatusID, string StatusDate, string AuthorizationNo, int VehicleId, string Address, double Latitude, double Longitude)
        {
            int UpdatedVehicleId = 0;

            try
            {
                Log(">>UpdateVehicleDeviceStatus(uid={0}, VehicleId={1})", userId.ToString(), VehicleId.ToString());

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsVehicleStatuses = new DataSet();
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    UpdatedVehicleId = dbVehicle.UpdateVehicleDeviceStatus(VehicleDeviceStatusID, StatusDate, AuthorizationNo, VehicleId, loginUserId, Address, Latitude, Longitude);
                }
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateVehicleDeviceStatus : uid={0}, EXC={1}", userId, Ex.Message);      
            }

            return UpdatedVehicleId;
        }

        #endregion
    }
}
