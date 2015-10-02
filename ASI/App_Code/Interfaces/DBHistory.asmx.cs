/** \file      DBHistory.asmx.cs
 *  \brief     is the web interface to get/filter/select messages from vlfMsgInHst 
 *  \comment   first update is to make the code cleaner
 *             second update is to record all web method calls in an activity table per user
 */ 
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using VLF.DAS.Logic;
using System.IO;
using VLF.CLS;
using System.Data.SqlClient;
using System.Configuration;
  

namespace VLF.ASI.Interfaces
{

    [WebService(Namespace = "http://www.sentinelfm.com")]

    /// <summary>
    /// Summary description for DBHistory.
    /// </summary>
    public class DBHistory : System.Web.Services.WebService
    {
        public DBHistory()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
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

       private bool ValidateUserFleet(int userId, string SID, int fleetId )
       {
          // Authenticate & Authorize
          LoginManager.GetInstance().SecurityCheck(userId, SID);

          //Authorization
          using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
          {
             return dbUser.ValidateUserFleet(userId, fleetId);
          }
       }

       private bool ValidateUserBox(int userId, string SID, int boxId)
       {
          // Authenticate & Authorize
          LoginManager.GetInstance().SecurityCheck(userId, SID);

          //Authorization
          using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
          {
             return dbUser.ValidateUserBox(userId, boxId);
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


       private bool ValidateUserMsg(int userId, string SID, int msgId)
       {
           // Authenticate & Authorize
           LoginManager.GetInstance().SecurityCheck(userId, SID);

           //Authorization
           using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
           {
               return dbUser.ValidateUserMsg(userId, msgId);
           }
       }
       #endregion  refactored functions

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

       #region getting historic data

       public int GetVehicleHistory(int userId, string SID, string licensePlate, string dateTimeFrom, string dateTimeTo, ref string xml)
       {
          DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehicleHistory(uId={0}, licensePlate={1}, dtFrom={2}, dtTo={3})", 
                                       userId, licensePlate, dateTimeFrom, dateTimeTo);

                if (!ValidateUserLicensePlate(userId, SID, licensePlate))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                LogFinal("<< GetVehicleHistory(uId={0}, licensePlate={1}, dtFrom={2}, dtTo={3}, tSpan={4})",
                        userId, licensePlate, dateTimeFrom, dateTimeTo, DateTime.Now.Subtract(dtNow).TotalMilliseconds);


                return (int)InterfaceError.NotImplemented;
            }
            catch (Exception Ex)
            {
               LogException("<< GetVehicleHistory : uid={0} EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
        public int GetVehicleStatusHistoryByVehicleId(int userId, string SID, Int64 vehicleId, 
                                                      string dateTimeFrom, string dateTimeTo, bool includeCoordinate, 
                                                      bool includeSensor, bool includePositionUpdate, bool includeInvalidGps, 
                                                      Int16 DclId, ref string xml, ref bool requestOverflowed)
        {
           DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehicleStatusHistoryByVehicleId(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})", 
                                       userId, vehicleId, dateTimeFrom, dateTimeTo);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsMsgInHst = null;
                
                using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    int totalSqlRecords = 0;
                    dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId,"en", "","",   ref requestOverflowed, ref totalSqlRecords);

                    if (dsMsgInHst != null && dsMsgInHst.Tables.Count > 0 && dsMsgInHst.Tables[0].Rows.Count > 0)
                        xml = dsMsgInHst.GetXml();
                }

                LogFinal("<< GetVehicleStatusHistoryByVehicleId(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, tSpan={4})",
                                                      userId, vehicleId, dateTimeFrom, dateTimeTo, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetVehicleStatusHistoryByVehicleId : uid={0} EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

       private void LocalizeHistory(int userId, ref  DataSet dsMsgInHst, string lang, ref string xml)
       {
          if (Util.IsDataSetValid(dsMsgInHst))
          {
             if (ASIErrorCheck.IsLangSupported(lang))
             {
                LocalizationLayer.ServerLocalizationLayer dbl =
                   new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                dbl.LocalizationData(lang, "BoxMsgInTypeId", "BoxMsgInTypeName", "MessageType", ref dsMsgInHst);
             }

             xml = dsMsgInHst.GetXml();

             if (ASIErrorCheck.IsLangSupported(lang))
             {
                Resources.Const.Culture = new CultureInfo(lang);
                xml = xml.Replace(VLF.CLS.Def.Const.cmdAck, Resources.Const.MessageType_Yes)
                         .Replace(VLF.CLS.Def.Const.cmdNotAck, Resources.Const.MessageType_No);
             }
          }
       }

       // Changes for TimeZone Feature start
       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByVehicleIdByLang_NewTZ(int userId, string SID, Int64 vehicleId,
                                                            string dateTimeFrom, string dateTimeTo, bool includeCoordinate,
                                                            bool includeSensor, bool includePositionUpdate, bool includeInvalidGps,
                                                            Int16 DclId, string lang, ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByVehicleIdByLang(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, lang={4})",
                                   userId, vehicleId, dateTimeFrom, dateTimeTo, lang);

               if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;
               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId_NewTZ(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, "", "", ref requestOverflowed, ref totalSqlRecords);


                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByVehicleIdByLang(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, lang={4}, tSpan={5})",
                                   userId, vehicleId, dateTimeFrom, dateTimeTo, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByVehicleIdByLang : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
       // Changes for TimeZone Feature end

       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByVehicleIdByLang(int userId, string SID, Int64 vehicleId,
                                                            string dateTimeFrom, string dateTimeTo, bool includeCoordinate,
                                                            bool includeSensor, bool includePositionUpdate, bool includeInvalidGps,
                                                            Int16 DclId, string lang, ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByVehicleIdByLang(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, lang={4})",
                                   userId, vehicleId, dateTimeFrom, dateTimeTo, lang);

               if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;
               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, "", "", ref requestOverflowed, ref totalSqlRecords);


                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByVehicleIdByLang(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, lang={4}, tSpan={5})",
                                   userId, vehicleId, dateTimeFrom, dateTimeTo, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByVehicleIdByLang : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }

       // Changes for TimeZone Feature start
       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id,messages and top number of records . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(int userId, string SID, Int64 vehicleId,
                                                string dateTimeFrom, string dateTimeTo, bool includeCoordinate,
                                                bool includeSensor, bool includePositionUpdate, bool includeInvalidGps,
                                                Int16 DclId, string lang, string msgList, string sqlTopRecords,
                                                ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByVehicleIdByLangExtended(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})",
                                      userId, vehicleId, dateTimeFrom, dateTimeTo);

               if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;

               //string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

               //if ((Convert.ToDateTime(dateTimeFrom).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo)> DateTime.Now))
               //        ConnnectionString=LoginManager.GetConnnectionString(userId);




               string ConnnectionString = GetConnectionStringByDateRange(userId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo));

               //using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               using (MessageQueue dbMessageQueue = new MessageQueue(ConnnectionString))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId_NewTZ(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, ref requestOverflowed, ref totalSqlRecords);
                   //dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId_Tmp(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, ref requestOverflowed, ref totalSqlRecords);





                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByVehicleIdByLangExtended(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, tSpan={4})",
                       userId, vehicleId, dateTimeFrom, dateTimeTo, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByVehicleIdByLangExtended : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
       // Changes for TimeZone Feature end

       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id,messages and top number of records . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByVehicleIdByLangExtended(int userId, string SID, Int64 vehicleId,
                                                string dateTimeFrom, string dateTimeTo, bool includeCoordinate,
                                                bool includeSensor, bool includePositionUpdate, bool includeInvalidGps,
                                                Int16 DclId, string lang, string msgList, string sqlTopRecords,
                                                ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByVehicleIdByLangExtended(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})",
                                      userId, vehicleId, dateTimeFrom, dateTimeTo);

               if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;

               //string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

               //if ((Convert.ToDateTime(dateTimeFrom).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo)> DateTime.Now))
               //        ConnnectionString=LoginManager.GetConnnectionString(userId);




               string ConnnectionString = GetConnectionStringByDateRange(userId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo));

               //using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               using (MessageQueue dbMessageQueue = new MessageQueue(ConnnectionString))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, ref requestOverflowed, ref totalSqlRecords);
                   //dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId_Tmp(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, ref requestOverflowed, ref totalSqlRecords);





                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByVehicleIdByLangExtended(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, tSpan={4})",
                       userId, vehicleId, dateTimeFrom, dateTimeTo, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByVehicleIdByLangExtended : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }

       // Changes for TimeZone Feature start
       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id,messages and top number of records . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch_NewTZ(int userId, string SID, Int64 vehicleId,
                                   string dateTimeFrom, string dateTimeTo, bool includeCoordinate,
                                   bool includeSensor, bool includePositionUpdate, bool includeInvalidGps,
                                   Int16 DclId, string lang, string msgList, string sqlTopRecords,
                                   string address, ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})",
                                      userId, vehicleId, dateTimeFrom, dateTimeTo);

               if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;

               //string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

               //if ((Convert.ToDateTime(dateTimeFrom).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo) > DateTime.Now))
               //    ConnnectionString = LoginManager.GetConnnectionString(userId);


               string ConnnectionString = GetConnectionStringByDateRange(userId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo));

               //using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               using (MessageQueue dbMessageQueue = new MessageQueue(ConnnectionString))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId_ExtendedSearch_NewTZ(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, address, ref requestOverflowed, ref totalSqlRecords);


                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, tSpan={4})",
                                      userId, vehicleId, dateTimeFrom, dateTimeTo, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
       // Changes for TimeZone Feature end


       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id,messages and top number of records . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(int userId, string SID, Int64 vehicleId,
                                   string dateTimeFrom, string dateTimeTo, bool includeCoordinate,
                                   bool includeSensor, bool includePositionUpdate, bool includeInvalidGps,
                                   Int16 DclId, string lang, string msgList, string sqlTopRecords,
                                   string address, ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})",
                                      userId, vehicleId, dateTimeFrom, dateTimeTo);

               if (!ValidateUserVehicle(userId, SID, vehicleId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;

               //string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

               //if ((Convert.ToDateTime(dateTimeFrom).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo).Day == DateTime.Now.Day) || (Convert.ToDateTime(dateTimeTo) > DateTime.Now))
               //    ConnnectionString = LoginManager.GetConnnectionString(userId);


               string ConnnectionString = GetConnectionStringByDateRange(userId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo));

               //using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               using (MessageQueue dbMessageQueue = new MessageQueue(ConnnectionString))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByVehicleId_ExtendedSearch(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, address, ref requestOverflowed, ref totalSqlRecords);


                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, tSpan={4})",
                                      userId, vehicleId, dateTimeFrom, dateTimeTo, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
       // Changes for TimeZone Feature start
       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id,messages and top number of records . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByFleetIdByLangExtendedSearch_NewTZ(int userId, string SID, Int32 fleetId, string dateTimeFrom, string dateTimeTo, bool includeCoordinate, bool includeSensor, bool includePositionUpdate, bool includeInvalidGps, Int16 DclId, string lang, string msgList, string sqlTopRecords, string address, ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, address={4})",
                             userId, fleetId, dateTimeFrom, dateTimeTo, address);

               if (!ValidateUserFleet(userId, SID, fleetId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;

               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByFleetId_ExtendedSearch(userId, fleetId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, address, ref requestOverflowed, ref totalSqlRecords);


                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, address={4}, tSpan={5})",
                             userId, fleetId, dateTimeFrom, dateTimeTo, address, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByFleetIdByLangExtendedSearch : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
       // Changes for TimeZone Feature end

       [WebMethod(Description = "Retrieves vehicles status from history by vehicle Id,messages and top number of records . XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
       public int GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(int userId, string SID, Int32 fleetId, string dateTimeFrom, string dateTimeTo, bool includeCoordinate, bool includeSensor, bool includePositionUpdate, bool includeInvalidGps, Int16 DclId, string lang, string msgList, string sqlTopRecords, string address, ref string xml, ref bool requestOverflowed)
       {
           DateTime dtNow = DateTime.Now;
           try
           {
               Log(">> GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, address={4})",
                             userId, fleetId, dateTimeFrom, dateTimeTo, address);

               if (!ValidateUserFleet(userId, SID, fleetId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

               DataSet dsMsgInHst = null;

               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                   int totalSqlRecords = 0;
                   dsMsgInHst = dbMessageQueue.GetMessagesFromHistoryByFleetId_ExtendedSearch(userId, fleetId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, DclId, lang, msgList, sqlTopRecords, address, ref requestOverflowed, ref totalSqlRecords);


                   //XSD generation
                   //string strURL = @"C:\Inetpub\wwwroot\ASI\Reports\TmpReports\Hist.xsd";
                   //StreamWriter writer = null;
                   //writer = new StreamWriter(strURL);
                   //dsMsgInHst.WriteXmlSchema(writer);
                   //if (writer != null)
                   //   writer.Close();

               }

               LocalizeHistory(userId, ref dsMsgInHst, lang, ref xml);

               LogFinal("<< GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, address={4}, tSpan={5})",
                             userId, fleetId, dateTimeFrom, dateTimeTo, address, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
               return (int)InterfaceError.NoError;
           }

           catch (Exception Ex)
           {
               LogException("<< GetVehicleStatusHistoryByFleetIdByLangExtendedSearch : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }



        [WebMethod(Description = "Retrieves vehicle off hours info from history. XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
        public int GetVehicleOffHoursInfo(int userId, string SID, Int64 vehicleId, string dateTimeFrom, string dateTimeTo, short dayFromHour, short dayFromMin, short dayToHour, short dayToMin, short weekendFromHour, short weekendFromMin, short weekendToHour, short weekendToMin, bool includeCoordinate, bool includeSensor, bool includePositionUpdate, bool includeInvalidGps, ref string xml, ref bool requestOverflowed)
        {
           DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetVehicleOffHoursInfo(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, dayFromHour={4}, dayFromMin={5}, dayToHour={6}, dayToMin={7}, weekendFromHour={8}, weekendFromMin={9}, weekendToHour={10}, weekendToMin={11})", 
                           userId, vehicleId, dateTimeFrom, dateTimeTo, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin);
               
                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsMsgInHst = null;
                using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    int totalSqlRecords = 0;
                    dsMsgInHst = dbMessageQueue.GetVehicleOffHoursInfo(userId, vehicleId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, ref requestOverflowed, ref totalSqlRecords);

                    if (Util.IsDataSetValid(dsMsgInHst))
                        xml = dsMsgInHst.GetXml();
                }

                LogFinal("<< GetVehicleOffHoursInfo(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3}, dayFromHour={4}, dayFromMin={5}, dayToHour={6}, dayToMin={7}, weekendFromHour={8}, weekendFromMin={9}, weekendToHour={10}, weekendToMin={11}, tSpan={12})",
                           userId, vehicleId, dateTimeFrom, dateTimeTo, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetVehicleOffHoursInfo : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
       
 

        [WebMethod(Description = "Retrieves fleet off hours info from history. XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize], [StreetAddress],[SequenceNum],[BoxArmed],[MsgDetails],[MsgDirection],[Acknowledged],[Scheduled]")]
        public int GetFleetOffHoursInfo(int userId, string SID, int fleetId, string dateTimeFrom, string dateTimeTo, short dayFromHour, short dayFromMin, short dayToHour, short dayToMin, short weekendFromHour, short weekendFromMin, short weekendToHour, short weekendToMin, bool includeCoordinate, bool includeSensor, bool includePositionUpdate, bool includeInvalidGps, ref string xml, ref bool requestOverflowed)
        {
           DateTime dtNow = DateTime.Now;
            try
            {
               Log(">> GetFleetOffHoursInfo(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, dayFromHour={4}, dayFromMin={5}, dayToHour={6}, dayToMin={7}, weekendFromHour={8}, weekendFromMin={9}, weekendToHour={10}, weekendToMin={11})", 
                              userId, fleetId, dateTimeFrom, dateTimeTo, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsMsgInHst = null;
                using(MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    DataTable tblLandmarks = null;
                    int totalSqlRecords = 0;
                    dsMsgInHst = dbMessageQueue.GetFleetOffHoursInfo(userId, fleetId, Convert.ToDateTime(dateTimeFrom), Convert.ToDateTime(dateTimeTo), dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, includeCoordinate, includeSensor, includePositionUpdate, includeInvalidGps, ref requestOverflowed, ref totalSqlRecords);

                    if (Util.IsDataSetValid(dsMsgInHst))
                        xml = dsMsgInHst.GetXml();
                }

                LogFinal("<< GetFleetOffHoursInfo(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, dayFromHour={4}, dayFromMin={5}, dayToHour={6}, dayToMin={7}, weekendFromHour={8}, weekendFromMin={9}, weekendToHour={10}, weekendToMin={11}, tSpan={12})",
                              userId, fleetId, dateTimeFrom, dateTimeTo, dayFromHour, dayFromMin, dayToHour, dayToMin, weekendFromHour, weekendFromMin, weekendToHour, weekendToMin, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetFleetOffHoursInfo : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature Start
        [WebMethod(Description = "Retrieves detailed information about incoming messages (Message In) from history by box id and DateTime. XML File format:[BoxId],[DateTime],[MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[BoxArmed],[UserName],[FirstName],[LastName]")]
        public int GetDetailedMessageInFromHistory_NewTZ(int userId, string SID, int boxId,
                                                   DateTime msgDateTime, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetDetailedMessageInFromHistory(uId={0}, boxId={1}, msgDateTime={2})",
                           userId, boxId, msgDateTime);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsResult = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    dsResult = dbMessageQueue.GetDetailedMessageInFromHistory_NewTZ(userId, boxId, msgDateTime);
                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                }

                LogFinal("<< GetDetailedMessageInFromHistory(uId={0}, boxId={1}, msgDateTime={2}, tSpan={3})",
                        userId, boxId, msgDateTime, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDetailedMessageInFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end
        [WebMethod(Description = "Retrieves detailed information about incoming messages (Message In) from history by box id and DateTime. XML File format:[BoxId],[DateTime],[MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[BoxArmed],[UserName],[FirstName],[LastName]")]
        public int GetDetailedMessageInFromHistory(int userId, string SID, int boxId,
                                                   DateTime msgDateTime, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetDetailedMessageInFromHistory(uId={0}, boxId={1}, msgDateTime={2})",
                           userId, boxId, msgDateTime);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsResult = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    dsResult = dbMessageQueue.GetDetailedMessageInFromHistory(userId, boxId, msgDateTime);
                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                }

                LogFinal("<< GetDetailedMessageInFromHistory(uId={0}, boxId={1}, msgDateTime={2}, tSpan={3})",
                        userId, boxId, msgDateTime, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDetailedMessageInFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves last messages from the history by received datetime (Maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
        public int GetLastMessagesFromHistory(int userId, string SID, short numOfRecords, 
                                             int boxId, short msgType, DateTime from, DateTime to, ref string xml)
        {
           DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetLastMessagesFromHistory(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, from={4}, to={5})", 
                                 userId, numOfRecords, boxId, msgType, from, to);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                /*
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                   if (!dbUser.ValidateHGISuperUser(userId))
                      return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                }
                */

                DataSet dsMsgInHst = null;
                 string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
                //using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                using (MessageQueue dbMessageQueue = new MessageQueue(sConnectionString))
                {
                    dsMsgInHst = dbMessageQueue.GetLastMessagesFromHistory(numOfRecords, boxId, msgType, from, to);

                    if (Util.IsDataSetValid(dsMsgInHst))
                        xml = dsMsgInHst.GetXml();
                }

                LogFinal("<< GetLastMessagesFromHistory(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, from={4}, to={5}, tSpan={6})",
                  userId, numOfRecords, boxId, msgType, from, to, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetLastMessagesFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        //[WebMethod(Description = "Retrieves last messages from the history by received datetime (Maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
        //public int GetLastMessagesFromHistory_Brickman201107c(int userId, string SID, short numOfRecords,
        //                                     int boxId, short msgType, Int64  from, Int64 to, ref string xml)
        //{
        //    DateTime dtNow = DateTime.Now;
        //    try
        //    {
        //        Log(">> GetLastMessagesFromHistory_Brickman201107c(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, from={4}, to={5})",
        //                         userId, numOfRecords, boxId, msgType, from, to);

        //        // Authenticate & Authorize
        //        LoginManager.GetInstance().SecurityCheck(userId, SID);

        //        //Authorization
        //        /*
        //        using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
        //        {
        //           if (!dbUser.ValidateHGISuperUser(userId))
        //              return Convert.ToInt32(InterfaceError.AuthorizationFailed);
        //        }
        //        */

        //        DataSet dsMsgInHst = null;
        //        using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
        //        {
        //            dsMsgInHst = dbMessageQueue.GetLastMessagesFromHistory_Brickman201107c (numOfRecords, boxId, msgType, from, to);

        //            if (Util.IsDataSetValid(dsMsgInHst))
        //                xml = dsMsgInHst.GetXml();
        //        }

        //        LogFinal("<< GetLastMessagesFromHistory_Brickman201107c(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, from={4}, to={5}, tSpan={6})",
        //          userId, numOfRecords, boxId, msgType, from, to, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

        //        return (int)InterfaceError.NoError;
        //    }
        //    catch (Exception Ex)
        //    {
        //        LogException("<< GetLastMessagesFromHistory_Brickman201107c : uid={0}, EXC={1}", userId, Ex.Message);
        //        return (int)ASIErrorCheck.CheckError(Ex);
        //    }
        //}



        [WebMethod(Description = "Retrieves last messages from the history by originated datetime (Maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
        public int GetLastMessagesFromHistoryByOriginDateTime(int userId, string SID, short numOfRecords,
                                             int boxId, short msgType, DateTime from, DateTime to, ref string xml)
        {
           DateTime dtNow = DateTime.Now;
            try
            {
                //Log(">> GetLastMessagesFromHistoryByOriginDateTime(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, from={4}, to={5})",
                //                 userId, numOfRecords, boxId, msgType, from, to);

                LastMessagesFromHistoryRequestsLog(userId, boxId, msgType, from, to);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                /*
                using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                   if (!dbUser.ValidateHGISuperUser(userId))
                      return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                }
                */

                DataSet dsMsgInHst = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsMsgInHst = dbMessageQueue.GetLastMessagesFromHistoryByOriginDateTime(numOfRecords, boxId, msgType, from, to);
                    //dsMsgInHst = dbMessageQueue.GetLastMessagesFromHistoryNotProcessed(boxId, msgType);


                    if (Util.IsDataSetValid(dsMsgInHst))
                    {
                        dsMsgInHst.DataSetName = "MsgInHistory";
                        dsMsgInHst.Tables[0].TableName = "LastMessages";  
                        xml = dsMsgInHst.GetXml();
                    }
                }

                LogFinal("<< GetLastMessagesFromHistoryByOriginDateTime(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, from={4}, to={5}, tSpan={6})",
                  userId, numOfRecords, boxId, msgType, from, to, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLastMessagesFromHistoryByOriginDateTime : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



       [WebMethod(Description = "Retrieves last messages from the history (Maximum return messages should be specified). XML File format:[BoxId],[DateTimeReceived],[OriginDateTime],[DclId],[BoxMsgInTypeId],[BoxMsgInTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[BlobDataSize],[StreetAddress],[SequenceNum],[BoxArmed]")]
       public int GetLastMessagesFromHistoryByOrganization(int userId, string SID, short numOfRecords, int orgId,int fleetId,int boxId, short msgType, DateTime from, DateTime to, ref string xml)
       {
          DateTime dtNow = DateTime.Now;
          try
          {
             Log(">> GetLastMessagesFromHistoryByOrganization (uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, dtFrom={4}, dtTo={5})", 
                        userId, numOfRecords, boxId, msgType, from, to);

             // Authenticate & Authorize
             LoginManager.GetInstance().SecurityCheck(userId, SID);

             //Authorization
             /*
             VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
             if (!dbUser.ValidateHGISuperUser(userId))
                return (int)InterfaceError.AuthorizationFailed;
             */

             DataSet dsMsgInHst = null;
             using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
             {
                dsMsgInHst = dbMessageQueue.GetLastMessagesFromHistoryByOrganization(numOfRecords,orgId,fleetId, boxId, msgType, from, to);

                if (Util.IsDataSetValid(dsMsgInHst))
                   xml = dsMsgInHst.GetXml();
             }

             LogFinal("<< GetLastMessagesFromHistoryByOrganization(uId={0}, numOfRecords={1}, boxId={2}, msgType={3}, dtFrom={4}, dtTo={5}, tSpan={6})",
                   userId, numOfRecords, boxId, msgType, from, to, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

             return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< GetLastMessagesFromHistoryByOrganization : uid={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }
       }

        [WebMethod(Description = "Retrieves last outgoing messages (Message Out) from history. XML File format:[DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],[BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]")]
        public int GetLastMessagesOutFromHistory(int userId, string SID, short numOfRecords, int boxId, short cmdType, 
                                                 DateTime from, DateTime to, ref string xml)
        {
           DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetLastMessagesOutFromHistory(uId={0}, numOfRecords={1}, boxId={2}, cmdType={3}, dtFrom={4}, dtTo={5})", 
                              userId, numOfRecords, boxId, cmdType, from, to);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                if (!dbUser.ValidateHGISuperUser(userId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


               DataSet dsMsgOutHst = null;
               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                    dsMsgOutHst = dbMessageQueue.GetLastMessagesOutFromHistory(numOfRecords, boxId, cmdType, from, to);

                    if (Util.IsDataSetValid(dsMsgOutHst))
                        xml = dsMsgOutHst.GetXml();
                }

                LogFinal("<< GetLastMessagesOutFromHistory(uId={0}, numOfRecords={1}, boxId={2}, cmdType={3}, dtFrom={4}, dtTo={5}, , tSpan={6})",
                              userId, numOfRecords, boxId, cmdType, from, to, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetLastMessagesOutFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


       [WebMethod(Description = "Retrieves last outgoing messages (Message Out) from history. XML File format:[DateTime],[BoxId],[UserId],[Priority],[DclId],[AslId],[BoxCmdOutTypeId],[BoxCmdOutTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[CommInfo1],[CommInfo2],[CustomProp],[SequenceNum],[Acknowledged]")]
       public int GetLastMessagesOutFromHistoryByOrganization(int userId, string SID, short numOfRecords,int orgId, int fleetId, int boxId, short cmdType, DateTime from, DateTime to, ref string xml)
       {
          DateTime dtNow = DateTime.Now;
          try
          {
             Log(">> GetLastMessagesOutFromHistoryByOrganization(uId={0}, numOfRecords={1}, boxId={2}, cmdType={3}, dtFrom={4}, dtTo={5})", 
                           userId, numOfRecords, boxId, cmdType, from, to);

             // Authenticate & Authorize
             LoginManager.GetInstance().SecurityCheck(userId, SID);

             //Authorization
             VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
             if (!dbUser.ValidateHGISuperUser(userId))
                return Convert.ToInt32(InterfaceError.AuthorizationFailed);


            DataSet dsMsgOutHst = null;
            using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
            {
                dsMsgOutHst = dbMessageQueue.GetLastMessagesOutFromHistoryByOrganization (numOfRecords,orgId,fleetId,  boxId, cmdType, from, to);

                if (Util.IsDataSetValid(dsMsgOutHst))
                   xml = dsMsgOutHst.GetXml();
            }

            LogFinal("<< GetLastMessagesOutFromHistoryByOrganization(uId={0}, numOfRecords={1}, boxId={2}, cmdType={3}, dtFrom={4}, dtTo={5}, tSpan={6})",
                       userId, numOfRecords, boxId, cmdType, from, to, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

            return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< GetLastMessagesOutFromHistoryByOrganization : uid={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }
       }

       // Changes for TimeZone Feature start
       [WebMethod(Description = "Retrieves detailed information about outgoing messages (Message Out) from history by box id and DateTime. XML File format:[BoxId],[DateTime],[MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[BoxArmed],[UserName],[FirstName],[LastName]")]
       public int GetDetailedMessageOutFromHistory_NewTZ(int userId, string SID, int boxId, DateTime msgDateTime, ref string xml)
       {
           DateTime dtNow = DateTime.Now;

           try
           {
               Log(">> GetDetailedMessageOutFromHistory(uId={0}, boxId={1}, msgDateTime={2})", userId, boxId, msgDateTime);

               if (!ValidateUserBox(userId, SID, boxId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);


               DataSet dsResult = null;

               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                   dsResult = dbMessageQueue.GetDetailedMessageOutFromHistory_NewTZ(userId, boxId, msgDateTime);
                   if (Util.IsDataSetValid(dsResult))
                       xml = dsResult.GetXml();
               }

               LogFinal("<< GetDetailedMessageOutFromHistory(uId={0}, boxId={1}, msgDateTime={2}, tSpan={3})",
                                               userId, boxId, msgDateTime, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               LogException("<< GetDetailedMessageOutFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
       // Changes for TimeZone Feature end

       [WebMethod(Description = "Retrieves detailed information about outgoing messages (Message Out) from history by box id and DateTime. XML File format:[BoxId],[DateTime],[MsgTypeId],[MsgTypeName],[BoxProtocolTypeId],[BoxProtocolTypeName],[ValidGps],[Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],[StreetAddress],[BoxArmed],[UserName],[FirstName],[LastName]")]
       public int GetDetailedMessageOutFromHistory(int userId, string SID, int boxId, DateTime msgDateTime, ref string xml)
       {
           DateTime dtNow = DateTime.Now;

           try
           {
               Log(">> GetDetailedMessageOutFromHistory(uId={0}, boxId={1}, msgDateTime={2})", userId, boxId, msgDateTime);

               if (!ValidateUserBox(userId, SID, boxId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);


               DataSet dsResult = null;

               using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
               {
                   dsResult = dbMessageQueue.GetDetailedMessageOutFromHistory(userId, boxId, msgDateTime);
                   if (Util.IsDataSetValid(dsResult))
                       xml = dsResult.GetXml();
               }

               LogFinal("<< GetDetailedMessageOutFromHistory(uId={0}, boxId={1}, msgDateTime={2}, tSpan={3})",
                                               userId, boxId, msgDateTime, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               LogException("<< GetDetailedMessageOutFromHistory : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }

        private void LastMessagesFromHistoryRequestsLog(int UserId, int BoxId,int MsgTypeId, DateTime FromDate ,DateTime ToDate)
        {
            try
            {
                //Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                //     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                //                       string.Format(strFormat, objects)));


                string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

                //if any one point of a geozone exist within boundary retrieve it...

                string sqlb = "LastMessagesFromHistoryRequestsLog_Insert";
                SqlConnection con = new SqlConnection(constr);
                SqlCommand com = new SqlCommand(sqlb, con);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@UserId", UserId);
                com.Parameters.AddWithValue("@BoxId", BoxId);
                com.Parameters.AddWithValue("@MsgTypeId", MsgTypeId);
                com.Parameters.AddWithValue("@FromDate", FromDate);
                com.Parameters.AddWithValue("@ToDate", ToDate);
                com.CommandType = CommandType.StoredProcedure;
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
            catch
            {

            }
        }

       #endregion  getting historic data

        

       #region TextMessaging Interfaces

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Retrieves text messages full information. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],[MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetTextMessagesFullInfo_NewTZ(int userId, string SID, int boxId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetTextMessagesFullInfo(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, boxId, from, to, msgDirection);

                if (!ValidateUserBox(userId, SID, boxId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetTextMessagesFullInfo_NewTZ(userId, boxId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }
                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                LogFinal("<< GetTextMessagesFullInfo(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                              userId, boxId, from, to, msgDirection);
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetTextMessagesFullInfo : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end
         [WebMethod(Description = "Retrieves text messages full information. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],[MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetTextMessagesFullInfo(int userId, string SID, int boxId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetTextMessagesFullInfo(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})", 
                           userId, boxId, from, to, msgDirection);

                if (!ValidateUserBox(userId, SID, boxId))
                   return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetTextMessagesFullInfo(userId, boxId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }
                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

               LogFinal("<< GetTextMessagesFullInfo(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                             userId, boxId, from, to, msgDirection);
               return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetTextMessagesFullInfo : uid={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves single text message full information. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId],[MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId],[Acknowledged],[UserName],[StreetAddress],[Latitude],[Longitude],[Speed],[Heading]")]
        public int GetTextMessageFullInfo(int userId, string SID, int msgId, Int64 vehicleId, ref string xml)
        {
            try
            {
                Log(">> GetTextMessageFullInfo(uId={0}, msgId={1})", userId, msgId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetMessageFullInfo(userId, msgId, vehicleId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                LogFinal("<< GetTextMessageFullInfo(uId={0}, msgId={1})", userId, msgId);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetTextMessageFullInfo : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves text messages full information by User ID. XML File format:  [VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetUserTextMessagesFullInfo(int userId, string SID, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetUserTextMessagesFullInfo(uId={0}, dtFrom={1}, dtTo={2}, msgDirection={3})", 
                           userId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetUserTextMessagesFullInfo(userId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogFinal("<< GetUserTextMessagesFullInfo : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Retrieves text messages by Fleet ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetFleetTextMessagesFullInfo_NewTZ(int userId, string SID, int fleetId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetFleetTextMessagesFullInfo(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, fleetId, from, to, msgDirection);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetFleetTxtMsgs_NewTZ(userId, fleetId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetTextMessagesFullInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves text messages by Fleet ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetFleetTextMessagesFullInfo(int userId, string SID, int fleetId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetFleetTextMessagesFullInfo(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})", 
                           userId, fleetId, from, to, msgDirection);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetFleetTxtMsgs(userId, fleetId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetFleetTextMessagesFullInfo : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Retrieves text messages (MDT + Garmin) by Fleet ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetFleetAllTextMessagesFullInfo_NewTZ(int userId, string SID, int fleetId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetFleetTextMessagesFullInfo(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, fleetId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetFleetAllTxtMsgs_NewTZ(userId, fleetId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetTextMessagesFullInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves text messages (MDT + Garmin) by Fleet ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetFleetAllTextMessagesFullInfo(int userId, string SID, int fleetId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetFleetTextMessagesFullInfo(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, fleetId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetFleetAllTxtMsgs(userId, fleetId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetTextMessagesFullInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start

        [WebMethod(Description = "Retrieves Garmin Destinations by Fleet ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetFleetAllDestinations_NewTZ(int userId, string SID, int fleetId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetFleetAllDestinations(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, fleetId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetFleetAllDestinations_NewTZ(userId, fleetId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetAllDestinations : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end


        [WebMethod(Description = "Retrieves Garmin Destinations by Fleet ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetFleetAllDestinations(int userId, string SID, int fleetId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetFleetAllDestinations(uId={0}, fleetId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, fleetId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetFleetAllDestinations(userId, fleetId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetAllDestinations : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Retrieves text messages (MDT + Garmin) by Vehilce ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetVehicleAllTextMessagesFullInfo_NewTZ(int userId, string SID, int boxId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetVehicleAllTxtMsgs(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, boxId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetVehicleAllTxtMsgs_NewTZ(userId, boxId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleAllTxtMsgs : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves text messages (MDT + Garmin) by Vehilce ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetVehicleAllTextMessagesFullInfo(int userId, string SID, int boxId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetVehicleAllTxtMsgs(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, boxId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetVehicleAllTxtMsgs(userId, boxId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleAllTxtMsgs : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start

        [WebMethod(Description = "Retrieves Garmin Destinations by Vehicle ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetVehicleAllDestinations_NewTZ(int userId, string SID, int boxId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetVehicleAllDestinations(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, boxId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetVehicleAllDestinations_NewTZ(userId, boxId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleAllDestinations : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves Garmin Destinations by Vehicle ID. XML File format:[VehicleId],[LicensePlate],[From],[To],[MsgId],[BoxId],[MsgDateTime],[TxtMsgTypeId], [MsgBody],[MsgDirection],[MsgResponse],[ResponseDateTime],[UserId]")]
        public int GetVehicleAllDestinations(int userId, string SID, int boxId, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">> GetVehicleAllDestinations(uId={0}, boxId={1}, dtFrom={2}, dtTo={3}, msgDirection={4})",
                           userId, boxId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    //dsInfo = dbMessageQueue.GetFleetTextMessagesFullInfo(userId,fleetId,Convert.ToDateTime(from),Convert.ToDateTime(to),msgDirection);
                    dsInfo = dbMessageQueue.GetVehicleAllDestinations(userId, boxId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleAllDestinations : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Retrieves messages info by User ID. XML File format:[VehicleId],[Description],[LicensePlate], [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]")]
        public int GetUserTextMessagesShortInfo_NewTZ(int userId, string SID, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">>GetUserTextMessagesShortInfo( uid={0}, dtFrom={1}, dtTo={2}, msgDirection={3} )",
                              userId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetUserTextMessagesShortInfo_NewTZ(userId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserTextMessagesShortInfo : uid={0}, EXc={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves messages info by User ID. XML File format:[VehicleId],[Description],[LicensePlate], [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]")]
        public int GetUserTextMessagesShortInfo(int userId, string SID, string from, string to, short msgDirection, ref string xml)
        {
            try
            {
                Log(">>GetUserTextMessagesShortInfo( uid={0}, dtFrom={1}, dtTo={2}, msgDirection={3} )", 
                              userId, from, to, msgDirection);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetUserTextMessagesShortInfo(userId, Convert.ToDateTime(from), Convert.ToDateTime(to), msgDirection);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetUserTextMessagesShortInfo : uid={0}, EXc={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start

        [WebMethod(Description = "Retrieves messages info by User ID. XML File format:[VehicleId],[Description],[LicensePlate], [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]")]
        public int GetUserIncomingTextMessagesShortInfo_NewTZ(int userId, string SID, string from, string to, ref string xml)
        {
            try
            {
                Log(">>GetUserIncomingTextMessagesShortInfo( uid={0}, dtFrom={1}, dtTo={2} )",
                              userId, from, to);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetUserIncomingTextMessagesShortInfoSP_NewTZ(userId, Convert.ToDateTime(from), Convert.ToDateTime(to));
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserIncomingTextMessagesShortInfo : uid={0}, EXc={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature end

        [WebMethod(Description = "Retrieves messages info by User ID. XML File format:[VehicleId],[Description],[LicensePlate], [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]")]
        public int GetUserIncomingTextMessagesShortInfo(int userId, string SID, string from, string to, ref string xml)
        {
            try
            {
                Log(">>GetUserIncomingTextMessagesShortInfo( uid={0}, dtFrom={1}, dtTo={2} )",
                              userId, from, to);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetUserIncomingTextMessagesShortInfoSP(userId, Convert.ToDateTime(from), Convert.ToDateTime(to));
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserIncomingTextMessagesShortInfo : uid={0}, EXc={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves message from MDT and Peripherals devices by User ID. XML File format:[VehicleId],[Description],[LicensePlate], [MsgId],[BoxId],[MsgDateTime],[MsgBody (20)]")]
        public int GetUserIncomingTextMessagesFullInfo(int userId, string SID, int msgId, int peripheralId, int msgTypeId, DateTime msgDateTime,int vehicleId, ref string xml)
        {
            try
            {
                Log(">>GetUserIncomingTextMessagesFullInfo( uid={0}, msgId={1}, msgTypeId={2} )",
                              userId, msgId, msgTypeId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbMessageQueue.GetUserIncomingTextMessagesFullInfo(userId, msgId, peripheralId, msgTypeId, Convert.ToDateTime(msgDateTime), vehicleId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUserIncomingTextMessagesFullInfo : uid={0}, EXc={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod(Description = "Get Msgs Short Info CheckSum")]
        public int GetMsgsShortInfoCheckSum_NewTZ(int userId, string SID, float userTimeZone, ref string checksum)
        {
            try
            {



                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                // Retrieve timeshift from DB
                int msgLifeTime = 24;

                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - msgLifeTime - 12;
                float fromTimeShift = -userTimeZone - msgLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                float toTimeShift = -userTimeZone;



                using (TxtMsgs txt = new TxtMsgs(LoginManager.GetConnnectionString(userId)))
                {
                    checksum = txt.GetMsgsShortInfoCheckSum_NewTZ(DateTime.Now.AddHours(fromTimeShift),
                                                                     DateTime.Now.AddHours(toTimeShift),
                                                                     userId);
                }


                LogFinal("<< GetMsgsShortInfoCheckSum : checksum={0},uId={1}", checksum, userId);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMsgsShortInfoCheckSum : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature end


        [WebMethod(Description = "Get Msgs Short Info CheckSum")]
        public int GetMsgsShortInfoCheckSum(int userId, string SID, short userTimeZone, ref string checksum)
        {
            try
            {

                

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                // Retrieve timeshift from DB
                int msgLifeTime = 24;
              
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - msgLifeTime - 12;
                int fromTimeShift = -userTimeZone - msgLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                int toTimeShift = -userTimeZone;



                using (TxtMsgs txt = new TxtMsgs(LoginManager.GetConnnectionString(userId)))
                {
                    checksum = txt.GetMsgsShortInfoCheckSum(DateTime.Now.AddHours(fromTimeShift),
                                                                     DateTime.Now.AddHours(toTimeShift),
                                                                     userId);
                }


                LogFinal("<< GetMsgsShortInfoCheckSum : checksum={0},uId={1}", checksum, userId);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetMsgsShortInfoCheckSum : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #if MDT_NEW

             [WebMethod(Description = "Mark message as read by User")]
        public int SetMsgUserId(int userId, string SID, int msgId,int boxId)
        {
            try
            {
                Log(">> SetMsgUserId( uid={0}, msgId = {1},boxId = {2} )", userId, msgId, boxId);

                if (!ValidateUserMsg(userId, SID, msgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);              

                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    dbMessageQueue.SetMsgUserId(boxId, msgId, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< SetMsgUserId : uid={0}, EXc={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
#else
        [WebMethod(Description = "Mark message as read by User")]
        public int SetMsgUserId(int userId, string SID, int msgId)
        {
            try
            {
                Log(">> SetMsgUserId( uid={0}, msgId = {1} )", userId, msgId);

              //  if (!ValidateUserMsg(userId, SID, msgId))
                 //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);              

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);



                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    dbMessageQueue.SetMsgUserId(msgId, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< SetMsgUserId : uid={0}, EXc={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
#endif
       
       

        [WebMethod(Description = "Mark message as read by User")]
        public int SetMsgUserIdExtended(int userId, string SID, int msgId, int msgTypeId, DateTime originDateTime, DateTime touchDateTime, int peripheralId, Int64 checksumId)
        {
            try
            {
                Log(">> SetMsgUserIdExtended( uid={0}, msgId = {1},originDateTime={2} )", userId, msgId, originDateTime.ToString() );

                //if (!ValidateUserMsg(userId, SID, msgId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {

                    dbMessageQueue.SetMsgUserIdExtended(msgId, userId, msgTypeId, originDateTime, touchDateTime, peripheralId, checksumId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetMsgUserIdExtended : uid={0}, EXc={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Update MsgInHist CustomProperty")]
       public int UpdateCustomPropInHistory(int userId, string SID, int boxId, DateTime origin, Int16 boxMsgInTypeId, string customPropertyAddons)
       {
          try
          {
             Log(">> UpdateCustomPropInHistory(uId={0}, dtOrigin={1},  boxMsgInTypeId={2}, customPropertyAddons={3})", 
                           userId, boxId, origin, boxMsgInTypeId, customPropertyAddons);

             // Authenticate & Authorize
             LoginManager.GetInstance().SecurityCheck(userId, SID);

             DataSet dsInfo = null;
             using ( MsgInLite dbMsgIn = new MsgInLite(LoginManager.GetConnnectionString(userId)))
             {
                dbMsgIn.UpdateCustomPropInHistory(boxId, origin, boxMsgInTypeId, customPropertyAddons);
             }
             return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< UpdateCustomPropInHistory : uid={0}, EXc={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }
       }



       //[WebMethod(Description = "Retrieves MDT Forms messages")]
       //public int GetMDTFormsMessages(int userId, string SID, string from, string to, int fleetId, int boxId,int formId,ref string xml)
       //{
       //   try
       //   {
       //      Log(">>GetUserTextMessagesShortInfo( uid={0}, from = {1}, to = {2}, fleetId = {3},boxId = {4} )", userId, from, to, fleetId,boxId );

       //      // Authenticate & Authorize
       //      LoginManager.GetInstance().SecurityCheck(userId, SID);

       //      DataSet dsInfo = null;
       //      MessageQueue dbMessageQueue = null;
       //      try
       //      {
       //         dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId));
       //         dsInfo = dbMessageQueue.GetMDTFormsMessages(Convert.ToDateTime(from), Convert.ToDateTime(to), fleetId, boxId,formId);
       //      }
       //      finally
       //      {
       //         if (dbMessageQueue != null)
       //            dbMessageQueue.Dispose();
       //      }
       //      if (dsInfo != null && dsInfo.Tables.Count > 0 && dsInfo.Tables[0].Rows.Count > 0)
       //         xml = dsInfo.GetXml();
       //      return (int)InterfaceError.NoError;
       //   }
       //   catch (Exception Ex)
       //   {
       //      return (int)ASIErrorCheck.CheckError(Ex);
       //   }
       //}



        [WebMethod(Description = "Get vehicles In history by location")]
        public int GetVehiclesInHistoryByLocation(int userId, string SID, int fleetId,
                                                  DateTime from,
                                                  DateTime to, double latitude, double longitude, ref string xml)
        {
            try
            {
                Log(">> GetVehiclesInHistoryByLocation( uid={0}, fleetId = {1}  )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsResult = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsResult = dbMessageQueue.GetVehiclesInHistoryByLocation(fleetId, from, to, latitude, longitude);
                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetVehiclesInHistoryByLocation : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #region Admin
        [WebMethod]
        public int GetCmdSend(int userId, string SID, int fleetId, string cmdType, ref string xml)
        {
            try
            {
                Log(">>GetCmdSend (uid={0}, fleetId={1})",
                            userId, fleetId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbBox.GetCmdSend(userId, fleetId, cmdType);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCmdSent : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int GetCmdRec(int userId, string SID, int fleetId, int msgTypeId, ref string xml)
        {
            try
            {
                Log(">>GetCmdSend (uid={0}, fleetId={1})",
                            userId, fleetId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsInfo = dbBox.GetCmdRec(userId, fleetId, msgTypeId);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCmdRec : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        [WebMethod(Description = "Get VehiclesInHistory By Address")]
        public int GetVehiclesInHistoryByAddress(int userId, string SID, int fleetId,
                                                   DateTime from,
                                                   DateTime to, string address,ref string xml)
        {
            try
            {
                Log(">>GetVehiclesInHistoryByAddress( uid={0}, fleetId = {1}  )", userId, fleetId);

                if (!ValidateUserFleet(userId, SID, fleetId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsResult = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsResult = dbMessageQueue.GetVehiclesInHistoryByAddress(fleetId, from, to, address);
                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetVehiclesInHistoryByAddress : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #endregion

       
        #region Diagnostic Interfaces
        [WebMethod(Description = "Box Diagnostic utilities")]
        public int GetInvalidGPSStatistic(int userId, string SID, int InvalidGPSPercent, int Hours, ref string xml)
        {
            try
            {
                Log(">>GetInvalidGPSStatistic( InvalidGPSPercent = {0})", InvalidGPSPercent);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsMsgInHst = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsMsgInHst = dbMessageQueue.GetInvalidGPSStatistic(InvalidGPSPercent, Hours);
                    if (Util.IsDataSetValid(dsMsgInHst))
                        xml = dsMsgInHst.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetInvalidGPSStatistic : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Box Diagnostic utilities")]
        public int GetBoxesWithoutIpUpdates(int userId, string SID, int Hours, ref string xml)
        {
            try
            {
                Log(">> GetBoxesWithoutIpUpdates(uid={0}, hours={1})", userId, Hours);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsMsgInHst = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsMsgInHst = dbMessageQueue.GetBoxesWithoutIpUpdates(Hours);
                    if (Util.IsDataSetValid(dsMsgInHst))
                        xml = dsMsgInHst.GetXml();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetBoxesWithoutIpUpdates : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Box Diagnostic utilities")]
        public int GetBoxesReportedFrequency(int userId, string SID, int Hours, int TotalMsg, 
                                 Int16 OrganizationId, ref string xml)
        {
            try
            {
                Log(">> GetBoxesReportedFrequency(uid={0}, hrs={1} totalMSG={2} orgId={3})", 
                                 userId, Hours, TotalMsg, OrganizationId) ;

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsMsgInHst = null;
                using (MessageQueue dbMessageQueue = new MessageQueue(LoginManager.GetConnnectionString(userId)))
                {
                    dsMsgInHst = dbMessageQueue.GetBoxesReportedFrequency(Hours, TotalMsg, OrganizationId);
                    if (Util.IsDataSetValid(dsMsgInHst))
                        xml = dsMsgInHst.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
               LogException("<< GetBoxesReportedFrequency : uId={0}, EXC={1}", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

       [WebMethod(Description = "Box Diagnostic Utilities")]
       public int GetBoxMissingMessages(int userId, string SID, int boxId, DateTime dtFrom, DateTime dtTo, 
                                                      ref string result)
       {
          try
          {
             Log(">> GetBoxMissingmessages(uid={0}, bid={1}, dtFrom={2} dtTo={3}", userId, boxId, dtFrom, dtTo);

             // Authenticate & Authorize
             LoginManager.GetInstance().SecurityCheck(userId, SID);

             // hgi user validation
             //using (VLF.DAS.Logic.User user = new User(this.LoginManager.GetConnnectionString(userId)))
             //{
             //   if (!user.ValidateHGISuperUser(userId))
             //      return (int)InterfaceError.AuthorizationFailed;
             //}

             using (VLF.DAS.Logic.Box box = new Box(LoginManager.GetConnnectionString(userId)))
             {
                result = box.GetMissingMessages(boxId, dtFrom, dtTo);
             }

             LogFinal("<< GetBoxMissingmessages(uid={0}, bid={1}, dtFrom={2} dtTo={3}", userId, boxId, dtFrom, dtTo);

             return (int)InterfaceError.NoError;
          }
          catch (Exception Ex)
          {
             LogException("<< GetBoxMissingmessages : uId={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }

       }
        #endregion Diagnostic Interfaces

       #region Staging Methods
/*
        [WebMethod(Description = "Append Fake GPS Messages")]
        public int AppendFakeGPSMessage(int userId, string SID, DateTime dtOriginated, string VinNum, double lat, double lon)
        {
            try
            {
                Log(">> AppendFakeGPSMessage(uid={0}, dtOriginated={1}, VinNum={2}, lat={3},lon={4}", dtOriginated, VinNum, lat, lon);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // hgi user validation
                //using (VLF.DAS.Logic.User user = new User(this.LoginManager.GetConnnectionString(userId)))
                //{
                //   if (!user.ValidateHGISuperUser(userId))
                //      return (int)InterfaceError.AuthorizationFailed;
                //}
                int retValue =-1;
                using (VLF.DAS.Logic.MsgInLite msgIn = new MsgInLite(LoginManager.GetConnnectionString(userId)))
                {
                    retValue=msgIn.AppendFakeGPSMessage(dtOriginated,VinNum , lat, lon);
                }

                LogFinal("<< AppendFakeGPSMessage(uid={0}, dtOriginated={1}, VinNum={2}, lat={3},lon={4}", dtOriginated, VinNum, lat, lon);

                return retValue;
            }
            catch (Exception Ex)
            {
                LogException("<< AppendFakeGPSMessage : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }
       /// <summary>
       ///        It allows third party software to add content to the activity of a device
       /// </summary>
       /// <param name="userId"></param>
       /// <param name="SID"></param>
       /// <param name="dtOriginated"></param>
       /// <param name="toMobile">direction of the packet</param>
       /// <param name="applicationId">it will allow decoding of the packet along with the organizationId</param>
       /// <param name="lat"></param>
       /// <param name="lon"></param>
       /// <param name="xmlData"></param>
       /// <returns></returns>
       /// <comments>
       ///     how do you protect yourself from abuses ?
       /// </comments>
       [WebMethod(Description = "It allows third party software to add content to the activity of a device")]       
       public int AddThirdPartyMessage(int userId, string SID, bool toMobile, int applicationId, 
                                       double lat, double lon, string xmlData)
       {
          try
          {
             Log(">> AddThirdPartyMessage(uid={0}, dir={1}, appId={2}, lat={3},lon={4}, xml={5}",
                                          userId, toMobile, applicationId, lat, lon, xmlData);

             // Authenticate & Authorize
             LoginManager.GetInstance().SecurityCheck(userId, SID);

             // hgi user validation
             //using (VLF.DAS.Logic.User user = new User(this.LoginManager.GetConnnectionString(userId)))
             //{
             //   if (!user.ValidateHGISuperUser(userId))
             //      return (int)InterfaceError.AuthorizationFailed;
             //}
             int retValue = -1;
             using (VLF.DAS.Logic.MsgInLite msgIn = new MsgInLite(LoginManager.GetConnnectionString(userId), true))
             {
                int boxId = 0;
                DateTime dtOriginated = DateTime.Now.ToUniversalTime() ;

                if (msgIn.DecodeThirdPartyMessage(userId, applicationId, xmlData, out boxId, out dtOriginated))
                   retValue = msgIn.AppendThirdPartyMessage(dtOriginated, boxId, lat, lon, 0, -1, xmlData);
             }

             Log("<< AddThirdPartyMessage(uid={0}, dir={1}, appId={2}, lat={3},lon={4}, xml={5}",
                                          userId, toMobile, applicationId, lat, lon, xmlData);

             return retValue;
          }
          catch (Exception Ex)
          {
             LogException("<< AppendFakeGPSMessage : uId={0}, EXC={1}", userId, Ex.Message);
             return (int)ASIErrorCheck.CheckError(Ex);
          }

       }
 */ 
       #endregion


 #region MapSearch
        [WebMethod(Description = "GetVehicleAreaSearch")]
        public int GetVehicleAreaSearch(int userId, string SID, double Latitude, double Longitude, double Radius,
                     string fromDate, string toDate, int orgId, int LandmarkID, string PolygonPoints, string FleetIDs, string BoxIDs, ref string xml)
        {

            try
            {
                Log(">> GetVehicleAreaSearch(uId={0}})", userId);

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);


                string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

                DataSet dsResult = null;

                using (MapSearch ms = new MapSearch(ConnnectionString))
                {

                    dsResult = ms.GetVehicleAreaSearch(Latitude, Longitude, Radius, fromDate, toDate, orgId, LandmarkID, PolygonPoints, FleetIDs, BoxIDs, userId);

                    if (Util.IsDataSetValid(dsResult))
                        xml = dsResult.GetXml();
                    else
                        return (int)InterfaceError.NotFound;
                }

                LogFinal("<< GetVehicleAreaSearch(uId={0})", userId);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehicleAreaSearch : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion



      private string GetConnectionStringByDateRange(int userId, DateTime from, DateTime to)
       {
           string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

           if ((Convert.ToDateTime(Convert.ToDateTime(from).ToShortDateString()) == Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToShortDateString()))
               || ((Convert.ToDateTime(Convert.ToDateTime(to).ToShortDateString()) == Convert.ToDateTime(Convert.ToDateTime(DateTime.Now).ToShortDateString()))) || (to > DateTime.Now))
               ConnnectionString = LoginManager.GetConnnectionString(userId);

           return ConnnectionString;
       }

   }
}
