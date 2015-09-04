using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using System.Xml;
using VLF.DAS.Logic;
using VLF.ASI;


namespace VLF.ASI.Interfaces
{

    [WebService(Namespace = "http://www.sentinelfm.com")]

    /// <summary>
    /// Summary description for Alarm.
    /// </summary>
    public class Alarms : System.Web.Services.WebService
    {
        public Alarms()
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
        private bool ValidateUserAlarm(int userId, string SID, int alarmId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            string cns = "";
            int _organizationId = GetOrganizationIdByUserId(userId);
            if (_organizationId == 952)
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                    cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                else
                    cns = LoginManager.GetConnnectionString(userId);
            }
            else
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                    cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                else
                    cns = LoginManager.GetConnnectionString(userId);
            }
            //using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            using (VLF.DAS.Logic.User dbUser = new User(cns))
            {
                return dbUser.ValidateUserAlarm(userId, alarmId);
            }

            return true;
        }

        #endregion  refactored functions

        // Changes for TimeZone Feature start

        /// <summary>
        ///    Get Alarms List
        /// </summary>
        /// <comment>
        ///    TO REEXAMINE
        /// </comment>
        [WebMethod(Description = "Get Alarms List")]
        public int GetCurrentAlarmsXML_NewTZ(int userId, string SID, float userTimeZone, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetCurrentAlarmsXML(uId={0}, userTimeZone={1})", userId, userTimeZone);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, exp.Message + ". Cannot find alarm life time. ASI will use default: " + alarmLifeTime));
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                float fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                float toTimeShift = -userTimeZone;

                #region GetCurrentAlarms
                xml = "";

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAllAlarmsInfo(DateTime.Now.AddHours(fromTimeShift), DateTime.Now.AddHours(toTimeShift), userId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo))
                    {
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarms = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarms.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = null;
                                objRow = new object[dsCurrentAlarms.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();
                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[3] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[4] = ittr["UserId"].ToString();
                                objRow[5] = ittr["vehicleDescription"].ToString();
                                objRow[6] = ittr["AlarmDescription"].ToString();
                                dsCurrentAlarms.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarms.GetXml();
                        }
                    }
                }
                #endregion

                LogFinal("<< GetCurrentAlarmsXML(uId={0}, userTimeZone={1}, tSpan={2})",
                                 userId, userTimeZone, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCurrentAlarmsXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end

        /// <summary>
        ///    Get Alarms List
        /// </summary>
        /// <comment>
        ///    TO REEXAMINE
        /// </comment>
        [WebMethod(Description = "Get Alarms List")]
        public int GetCurrentAlarmsXML(int userId, string SID, short userTimeZone, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetCurrentAlarmsXML(uId={0}, userTimeZone={1})", userId, userTimeZone);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, exp.Message + ". Cannot find alarm life time. ASI will use default: " + alarmLifeTime));
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                int fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                int toTimeShift = -userTimeZone;

                #region GetCurrentAlarms
                xml = "";

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAllAlarmsInfo(DateTime.Now.AddHours(fromTimeShift), DateTime.Now.AddHours(toTimeShift), userId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo))
                    {
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarms = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarms.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = null;
                                objRow = new object[dsCurrentAlarms.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();
                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[3] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[4] = ittr["UserId"].ToString();
                                objRow[5] = ittr["vehicleDescription"].ToString();
                                objRow[6] = ittr["AlarmDescription"].ToString();
                                dsCurrentAlarms.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarms.GetXml();
                        }
                    }
                }
                #endregion

                LogFinal("<< GetCurrentAlarmsXML(uId={0}, userTimeZone={1}, tSpan={2})",
                                 userId, userTimeZone, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCurrentAlarmsXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start

        [WebMethod(Description = "Get Alarms List")]
        public int GetAlarmsShortInfoXMLByLang_NewTZ(int userId, string SID, float userTimeZone, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetAlarmsShortInfoXMLByLang(uId={0}, userTimeZone={1})", userId, userTimeZone);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    LogException("-- GetAlarmsShortInfoXMLByLang : Cannot find alarm life time. ASI will use default: {0}", alarmLifeTime);
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                float fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                float toTimeShift = -userTimeZone;

                xml = "";
                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmsShortInfo_NewTZ(DateTime.Now.AddHours(fromTimeShift),
                                                                     DateTime.Now.AddHours(toTimeShift),
                                                                     userId);

                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo))
                    {
                        if (ASIErrorCheck.IsLangSupported(lang))
                        {
                            Resources.Const.Culture = ASIErrorCheck.NewCultureInfo(lang); // lang != "en" && lang != null ? new System.Globalization.CultureInfo(lang) : null;

                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                ittr["AlarmDescription"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(ittr["AlarmDescription"].ToString(), lang);
                            }
                        }

                        xml = dsAlarmInfo.GetXml();
                    }

                }

                LogFinal("<< GetAlarmsShortInfoXMLByLang(uId={0}, userTimeZone={1}, tSpan={2})",
                              userId, userTimeZone, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;


            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsShortInfoXMLByLang : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }


        // Changes for TimeZone Feature end
        [WebMethod(Description = "Get Alarms List")]
        public int GetAlarmsShortInfoXMLByLang(int userId, string SID, short userTimeZone, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetAlarmsShortInfoXMLByLang(uId={0}, userTimeZone={1})", userId, userTimeZone);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    LogException("-- GetAlarmsShortInfoXMLByLang : Cannot find alarm life time. ASI will use default: {0}", alarmLifeTime);
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                int fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                int toTimeShift = -userTimeZone;

                xml = "";
                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmsShortInfo(DateTime.Now.AddHours(fromTimeShift),
                                                                     DateTime.Now.AddHours(toTimeShift),
                                                                     userId);

                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo))
                    {
                        if (ASIErrorCheck.IsLangSupported(lang))
                        {
                            Resources.Const.Culture = ASIErrorCheck.NewCultureInfo(lang); // lang != "en" && lang != null ? new System.Globalization.CultureInfo(lang) : null;

                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                ittr["AlarmDescription"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(ittr["AlarmDescription"].ToString(), lang);
                            }
                        }

                        xml = dsAlarmInfo.GetXml();
                    }

                }

                LogFinal("<< GetAlarmsShortInfoXMLByLang(uId={0}, userTimeZone={1}, tSpan={2})",
                              userId, userTimeZone, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;


            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsShortInfoXMLByLang : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }


        // Changes for TimeZone Feature start

        [WebMethod(Description = "Get Alarms List")]
        public int GetCurrentAlarmsXMLByLang_NewTZ(int userId, string SID, float userTimeZone, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetCurrentAlarmsXMLByLang: uId={0}, userTimeZone={1}, lang={2})", userId, userTimeZone, lang);


                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, exp.Message + ". Cannot find alarm life time. ASI will use default: " + alarmLifeTime));
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                float fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                float toTimeShift = -userTimeZone;

                #region GetCurrentAlarms
                xml = "";
                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAllAlarmsInfo(DateTime.Now.AddHours(fromTimeShift), DateTime.Now.AddHours(toTimeShift), userId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo))
                    {
                        //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarms = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarms.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            Resources.Const.Culture = ASIErrorCheck.NewCultureInfo(lang);  // lang == "fr" && lang != null ? new System.Globalization.CultureInfo(lang) : null;
                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                object[] objRow = null;
                                objRow = new object[dsCurrentAlarms.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();
                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[3] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[4] = ittr["UserId"].ToString();
                                objRow[5] = ittr["vehicleDescription"].ToString();
                                objRow[6] = ittr["AlarmDescription"].ToString();
                                // LOCALIZATION
                                //if (lang != "en" && lang != null && ((string)ittr["AlarmType"]).Trim() != "GeoZone")
                                if (ASIErrorCheck.IsLangSupported(lang))
                                    //                                if (lang == "fr" && lang != null)
                                    objRow[6] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(objRow[6].ToString(), lang);
                                //objRow[6] = ((string)objRow[6]).Replace("Alarm", Resources.Const.MessageType_Alarm)
                                //                               .Replace("BadSensor", Resources.Const.MessageType_BadSensor)
                                //                               .Replace("Coordinate", Resources.Const.MessageType_Coordinate)
                                //                               .Replace("Door", Resources.Const.MessageType_Door)
                                //                               .Replace("Duration", Resources.Const.MessageType_Duration)
                                //                               .Replace("ExtremeAcceleration", Resources.Const.MessageType_ExtremeAcceleration)
                                //                               .Replace("ExtremeBraking", Resources.Const.MessageType_ExtremeBraking)
                                //                               .Replace("GeoFence", Resources.Const.MessageType_GeoFence)
                                //                               .Replace("GPSAntennaOK", Resources.Const.MessageType_GPSAntennaOK)
                                //                               .Replace("GPSAntennaOpen", Resources.Const.MessageType_GPSAntennaOpen)
                                //                               .Replace("GPSAntennaShort", Resources.Const.MessageType_GPSAntennaShort)
                                //                               .Replace("HarshAcceleration", Resources.Const.MessageType_HarshAcceleration)
                                //                               .Replace("HarshBraking", Resources.Const.MessageType_HarshBraking)
                                //                               .Replace("Idling", Resources.Const.MessageType_Idling)
                                //                               .Replace("Ignition", Resources.Const.MessageType_Ignition)
                                //                               .Replace("KeyFobArm", Resources.Const.MessageType_KeyFobArm)
                                //                               .Replace("KeyFobDisarm", Resources.Const.MessageType_KeyFobDisarm)
                                //                               .Replace("KeyFobPanic", Resources.Const.MessageType_KeyFobPanic)
                                //                               .Replace("Main Battery", Resources.Const.MessageType_Main_Battery)
                                //                               .Replace("Passenger In Seat", Resources.Const.MessageType_Passenger_In_Seat)
                                //                               .Replace("SeatBelt", Resources.Const.MessageType_SeatBelt)
                                //                               .Replace("Sensor", Resources.Const.MessageType_Sensor)
                                //                               .Replace("Speeding", Resources.Const.MessageType_Speeding)
                                //                               .Replace("Speed", Resources.Const.MessageType_Speed)
                                //                               .Replace("Status", Resources.Const.MessageType_Status)
                                //                               .Replace("StoredPosition", Resources.Const.MessageType_StoredPosition)
                                //                               .Replace("Low", Resources.Const.MessageType_Low)
                                //                               .Replace("Off", Resources.Const.MessageType_Off)
                                //                               .Replace("On", Resources.Const.MessageType_On)
                                //                               .Replace("Open", Resources.Const.MessageType_Open)
                                //                               .Replace("Yes", Resources.Const.MessageType_Yes);

                                dsCurrentAlarms.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarms.GetXml();
                        }
                    }
                }
                #endregion


                LogFinal("<< GetCurrentAlarmsXMLByLang(uId={0}, userTimeZone={1}, lang={2}, tSpan={3})",
                              userId, userTimeZone, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCurrentAlarmsXMLByLang: uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end
        [WebMethod(Description = "Get Alarms List")]
        public int GetCurrentAlarmsXMLByLang(int userId, string SID, short userTimeZone, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetCurrentAlarmsXMLByLang: uId={0}, userTimeZone={1}, lang={2})", userId, userTimeZone, lang);


                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, exp.Message + ". Cannot find alarm life time. ASI will use default: " + alarmLifeTime));
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                int fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                int toTimeShift = -userTimeZone;

                #region GetCurrentAlarms
                xml = "";
                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAllAlarmsInfo(DateTime.Now.AddHours(fromTimeShift), DateTime.Now.AddHours(toTimeShift), userId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo))
                    {
                        //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarms = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarms.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            Resources.Const.Culture = ASIErrorCheck.NewCultureInfo(lang);  // lang == "fr" && lang != null ? new System.Globalization.CultureInfo(lang) : null;
                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                object[] objRow = null;
                                objRow = new object[dsCurrentAlarms.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();
                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[3] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[4] = ittr["UserId"].ToString();
                                objRow[5] = ittr["vehicleDescription"].ToString();
                                objRow[6] = ittr["AlarmDescription"].ToString();
                                // LOCALIZATION
                                //if (lang != "en" && lang != null && ((string)ittr["AlarmType"]).Trim() != "GeoZone")
                                if (ASIErrorCheck.IsLangSupported(lang))
                                    //                                if (lang == "fr" && lang != null)
                                    objRow[6] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(objRow[6].ToString(), lang);
                                //objRow[6] = ((string)objRow[6]).Replace("Alarm", Resources.Const.MessageType_Alarm)
                                //                               .Replace("BadSensor", Resources.Const.MessageType_BadSensor)
                                //                               .Replace("Coordinate", Resources.Const.MessageType_Coordinate)
                                //                               .Replace("Door", Resources.Const.MessageType_Door)
                                //                               .Replace("Duration", Resources.Const.MessageType_Duration)
                                //                               .Replace("ExtremeAcceleration", Resources.Const.MessageType_ExtremeAcceleration)
                                //                               .Replace("ExtremeBraking", Resources.Const.MessageType_ExtremeBraking)
                                //                               .Replace("GeoFence", Resources.Const.MessageType_GeoFence)
                                //                               .Replace("GPSAntennaOK", Resources.Const.MessageType_GPSAntennaOK)
                                //                               .Replace("GPSAntennaOpen", Resources.Const.MessageType_GPSAntennaOpen)
                                //                               .Replace("GPSAntennaShort", Resources.Const.MessageType_GPSAntennaShort)
                                //                               .Replace("HarshAcceleration", Resources.Const.MessageType_HarshAcceleration)
                                //                               .Replace("HarshBraking", Resources.Const.MessageType_HarshBraking)
                                //                               .Replace("Idling", Resources.Const.MessageType_Idling)
                                //                               .Replace("Ignition", Resources.Const.MessageType_Ignition)
                                //                               .Replace("KeyFobArm", Resources.Const.MessageType_KeyFobArm)
                                //                               .Replace("KeyFobDisarm", Resources.Const.MessageType_KeyFobDisarm)
                                //                               .Replace("KeyFobPanic", Resources.Const.MessageType_KeyFobPanic)
                                //                               .Replace("Main Battery", Resources.Const.MessageType_Main_Battery)
                                //                               .Replace("Passenger In Seat", Resources.Const.MessageType_Passenger_In_Seat)
                                //                               .Replace("SeatBelt", Resources.Const.MessageType_SeatBelt)
                                //                               .Replace("Sensor", Resources.Const.MessageType_Sensor)
                                //                               .Replace("Speeding", Resources.Const.MessageType_Speeding)
                                //                               .Replace("Speed", Resources.Const.MessageType_Speed)
                                //                               .Replace("Status", Resources.Const.MessageType_Status)
                                //                               .Replace("StoredPosition", Resources.Const.MessageType_StoredPosition)
                                //                               .Replace("Low", Resources.Const.MessageType_Low)
                                //                               .Replace("Off", Resources.Const.MessageType_Off)
                                //                               .Replace("On", Resources.Const.MessageType_On)
                                //                               .Replace("Open", Resources.Const.MessageType_Open)
                                //                               .Replace("Yes", Resources.Const.MessageType_Yes);

                                dsCurrentAlarms.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarms.GetXml();
                        }
                    }
                }
                #endregion


                LogFinal("<< GetCurrentAlarmsXMLByLang(uId={0}, userTimeZone={1}, lang={2}, tSpan={3})",
                              userId, userTimeZone, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCurrentAlarmsXMLByLang: uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Featutre start
        /// <summary>
        ///          Get Alarms List
        /// </summary>
        /// <comment>
        ///          TO REEXAMINE !!!
        ///          
        /// </comment>
        [WebMethod(Description = "Get Alarms List based on DateTime range")]
        public int GetAlarmsXML_NewTZ(int userId, string SID, DateTime fromDate, DateTime toDate, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetAlarmsXML(uId={0}, dtFrom={1}, dtTo={2})", userId, fromDate, toDate);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                float userTimezone = 0;
                // Retrieve user preference
                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (VLF.DAS.Logic.User userPref = new VLF.DAS.Logic.User(LoginManager.GetConnnectionString(userId)))
                using (VLF.DAS.Logic.User userPref = new VLF.DAS.Logic.User(cns))
                {
                    DataSet dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.TimeZoneNew);

                    if (ASIErrorCheck.IsAnyRecord(dsUserPref))
                        userTimezone = Convert.ToSingle(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);

                    dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving);
                    if (ASIErrorCheck.IsAnyRecord(dsUserPref))
                        userTimezone += Convert.ToInt16(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);
                }

                #region GetCurrentAlarms
                xml = "";

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAllAlarmsInfo(fromDate.AddHours(-userTimezone), toDate.AddHours(-userTimezone), userId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) //  != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarms = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarms.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserName", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("StreetAddress", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("Latitude", typeof(float));
                            dsCurrentAlarms.Tables[0].Columns.Add("Longitude", typeof(float));
                            dsCurrentAlarms.Tables[0].Columns.Add("Notes", typeof(string)); // Salman Nov 28, 2012
                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = null;
                                objRow = new object[dsCurrentAlarms.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();
                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[3] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[4] = ittr["UserId"].ToString();
                                objRow[5] = ittr["vehicleDescription"].ToString();
                                objRow[6] = ittr["AlarmDescription"].ToString();
                                objRow[7] = ittr["UserName"].ToString();
                                objRow[8] = ittr["StreetAddress"].ToString();
                                objRow[9] = ittr["Latitude"].ToString();
                                objRow[10] = ittr["Longitude"].ToString();
                                objRow[11] = ittr["notes"].ToString(); // Salman Nov 28, 2012
                                dsCurrentAlarms.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarms.GetXml();

                        } // using( User dbUser                       
                    }
                } // using ( Alarm dbAlarm =

                #endregion

                LogFinal("<< GetAlarmsXML(uId={0}, dtFrom={1}, dtTo={2}, tSpan={3})",
                              userId, fromDate, toDate, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Featutre end

        /// <summary>
        ///          Get Alarms List
        /// </summary>
        /// <comment>
        ///          TO REEXAMINE !!!
        ///          
        /// </comment>
        [WebMethod(Description = "Get Alarms List based on DateTime range")]
        public int GetAlarmsXML(int userId, string SID, DateTime fromDate, DateTime toDate, ref string xml)
        {
            DateTime dtNow = DateTime.Now;
            try
            {
                Log(">> GetAlarmsXML(uId={0}, dtFrom={1}, dtTo={2})", userId, fromDate, toDate);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                short userTimezone = 0;
                // Retrieve user preference
                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (VLF.DAS.Logic.User userPref = new VLF.DAS.Logic.User(LoginManager.GetConnnectionString(userId)))
                using (VLF.DAS.Logic.User userPref = new VLF.DAS.Logic.User(cns))
                {
                    DataSet dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.TimeZone);

                    if (ASIErrorCheck.IsAnyRecord(dsUserPref))
                        userTimezone = Convert.ToInt16(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);

                    dsUserPref = userPref.GetUserPreferenceInfo(userId, (int)VLF.CLS.Def.Enums.Preference.DayLightSaving);
                    if (ASIErrorCheck.IsAnyRecord(dsUserPref))
                        userTimezone += Convert.ToInt16(dsUserPref.Tables[0].Rows[0]["PreferenceValue"]);
                }

                #region GetCurrentAlarms
                xml = "";

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAllAlarmsInfo(fromDate.AddHours(-userTimezone), toDate.AddHours(-userTimezone), userId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) //  != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarms = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarms.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("UserName", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("StreetAddress", typeof(string));
                            dsCurrentAlarms.Tables[0].Columns.Add("Latitude", typeof(float));
                            dsCurrentAlarms.Tables[0].Columns.Add("Longitude", typeof(float));
                            dsCurrentAlarms.Tables[0].Columns.Add("Notes", typeof(string)); // Salman Nov 28, 2012
                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = null;
                                objRow = new object[dsCurrentAlarms.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();
                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[2] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[3] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[4] = ittr["UserId"].ToString();
                                objRow[5] = ittr["vehicleDescription"].ToString();
                                objRow[6] = ittr["AlarmDescription"].ToString();
                                objRow[7] = ittr["UserName"].ToString();
                                objRow[8] = ittr["StreetAddress"].ToString();
                                objRow[9] = ittr["Latitude"].ToString();
                                objRow[10] = ittr["Longitude"].ToString();
                                objRow[11] = ittr["notes"].ToString(); // Salman Nov 28, 2012
                                dsCurrentAlarms.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarms.GetXml();

                        } // using( User dbUser                       
                    }
                } // using ( Alarm dbAlarm =

                #endregion

                LogFinal("<< GetAlarmsXML(uId={0}, dtFrom={1}, dtTo={2}, tSpan={3})",
                              userId, fromDate, toDate, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature start

        /// <summary>
        ///           TO REEXAMINE !!!
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="alarmId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Returns current alarm full info by Alarm ID.")]
        public int GetCurrentAlarmInfoXML_NewTZ(int userId, string SID, int alarmId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetCurrentAlarmInfoXML(uId={0}, alarmId={1})", userId, alarmId);

                if (!ValidateUserAlarm(userId, SID, alarmId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) // != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarm = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarm.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeAccepted", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeClosed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("VehicleId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("StreetAddress", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserName", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Latitude", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("Longitude", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("Speed", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("Heading", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("SensorMask", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("IsArmed", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("BoxId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("CustomProp", typeof(string));

                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = new object[dsCurrentAlarm.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value)
                                    objRow[2] = " ";
                                else
                                    objRow[2] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeAck"])).ToString();

                                if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[3] = " ";
                                else
                                    objRow[3] = dbUser.GetUserLocalTime_NewTZ(userId, Convert.ToDateTime(ittr["DateTimeClosed"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[5] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[6] = ittr["UserId"].ToString();
                                objRow[7] = ittr["vehicleDescription"].ToString();
                                objRow[8] = ittr["AlarmDescription"].ToString();
                                objRow[9] = ittr["VehicleId"].ToString();
                                string streetAddress = ittr["StreetAddress"].ToString().TrimEnd();
                                if (Convert.ToInt16(ittr["ValidGPS"]) == 0) // 0 - valid, 1 -invalid
                                {
                                    // If street address is empty, get it from database and update property
                                    if (streetAddress == "")
                                    {
                                        // TODO: retrieves from map engine			
                                    }
                                }
                                else
                                {
                                    streetAddress = VLF.CLS.Def.Const.noValidAddress;
                                }
                                objRow[10] = streetAddress;
                                objRow[11] = ittr["UserName"].ToString();
                                objRow[12] = ittr["Latitude"].ToString();
                                objRow[13] = ittr["Longitude"].ToString();
                                objRow[14] = ittr["Speed"].ToString();
                                objRow[15] = ittr["Heading"].ToString();
                                objRow[16] = ittr["SensorMask"].ToString();
                                objRow[17] = ittr["IsArmed"].ToString();
                                objRow[18] = ittr["BoxId"].ToString();
                                dsCurrentAlarm.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarm.GetXml();
                        }

                    } // using( User dbUser =

                } // using( Alarm dbAlarm =

                LogFinal("<< GetCurrentAlarmInfoXML(uId={0}, alarmId={1}, tSpan={2})",
                                 userId, alarmId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCurrentAlarmInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end
        /// <summary>
        ///           TO REEXAMINE !!!
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="alarmId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Returns current alarm full info by Alarm ID.")]
        public int GetCurrentAlarmInfoXML(int userId, string SID, int alarmId, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                Log(">> GetCurrentAlarmInfoXML(uId={0}, alarmId={1})", userId, alarmId);

                if (!ValidateUserAlarm(userId, SID, alarmId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) // != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                        using (User dbUser = new User(cns))
                        {
                            DataSet dsCurrentAlarm = new DataSet(dsAlarmInfo.DataSetName);
                            dsCurrentAlarm.Tables.Add(dsAlarmInfo.Tables[0].TableName);
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeCreated", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeAccepted", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("TimeClosed", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmState", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmLevel", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("vehicleDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("AlarmDescription", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("VehicleId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("StreetAddress", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("UserName", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("Latitude", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("Longitude", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("Speed", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("Heading", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("SensorMask", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("IsArmed", typeof(string), "0");
                            dsCurrentAlarm.Tables[0].Columns.Add("BoxId", typeof(string));
                            dsCurrentAlarm.Tables[0].Columns.Add("CustomProp", typeof(string));

                            foreach (DataRow ittr in dsAlarmInfo.Tables[0].Rows)
                            {
                                // LOCALIZATION
                                object[] objRow = new object[dsCurrentAlarm.Tables[0].Columns.Count];
                                objRow[0] = ittr["AlarmId"].ToString();
                                objRow[1] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeCreated"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value)
                                    objRow[2] = " ";
                                else
                                    objRow[2] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeAck"])).ToString();

                                if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[3] = " ";
                                else
                                    objRow[3] = dbUser.GetUserLocalTime(userId, Convert.ToDateTime(ittr["DateTimeClosed"])).ToString();

                                if (ittr["DateTimeAck"] == DBNull.Value && ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.New.ToString();
                                else if (ittr["DateTimeClosed"] == DBNull.Value)
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Accepted.ToString();
                                else
                                    objRow[4] = VLF.CLS.Def.Enums.AlarmState.Closed.ToString();
                                objRow[5] = ((VLF.CLS.Def.Enums.AlarmSeverity)Convert.ToInt32(ittr["AlarmLevel"])).ToString();
                                objRow[6] = ittr["UserId"].ToString();
                                objRow[7] = ittr["vehicleDescription"].ToString();
                                objRow[8] = ittr["AlarmDescription"].ToString();
                                objRow[9] = ittr["VehicleId"].ToString();
                                string streetAddress = ittr["StreetAddress"].ToString().TrimEnd();
                                if (Convert.ToInt16(ittr["ValidGPS"]) == 0) // 0 - valid, 1 -invalid
                                {
                                    // If street address is empty, get it from database and update property
                                    if (streetAddress == "")
                                    {
                                        // TODO: retrieves from map engine			
                                    }
                                }
                                else
                                {
                                    streetAddress = VLF.CLS.Def.Const.noValidAddress;
                                }
                                objRow[10] = streetAddress;
                                objRow[11] = ittr["UserName"].ToString();
                                objRow[12] = ittr["Latitude"].ToString();
                                objRow[13] = ittr["Longitude"].ToString();
                                objRow[14] = ittr["Speed"].ToString();
                                objRow[15] = ittr["Heading"].ToString();
                                objRow[16] = ittr["SensorMask"].ToString();
                                objRow[17] = ittr["IsArmed"].ToString();
                                objRow[18] = ittr["BoxId"].ToString();
                                dsCurrentAlarm.Tables[0].Rows.Add(objRow);
                            }
                            xml = dsCurrentAlarm.GetXml();
                        }

                    } // using( User dbUser =

                } // using( Alarm dbAlarm =

                LogFinal("<< GetCurrentAlarmInfoXML(uId={0}, alarmId={1}, tSpan={2})",
                                 userId, alarmId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetCurrentAlarmInfoXML : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Accept alarm by Alarm ID.")]
        public int AcceptCurrentAlarmWithNotes(int userId, string SID, int alarmId, string notes)
        {
            try
            {
                Log(">> AcceptCurrentAlarm(uId={0}, alarmId={1})", userId, alarmId);

                if (!ValidateUserAlarm(userId, SID, alarmId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DateTime timeAccepted = DateTime.Now.AddHours(-Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone));

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) //  != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeAck"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already accepted.");
                        }
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeClosed"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already closed.");
                        }
                    }
                    else
                    {
                        throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " does not exist in the system.");
                    }

                    LoggerManager.RecordInitialValues("Alarm", userId, 0, "vlfAlarm",
                                                  string.Format("AlarmId={0}", alarmId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Accept alarm({0}) - Initial values", alarmId));

                    dbAlarm.AcceptAlarm(alarmId, timeAccepted, userId, notes);

                    LoggerManager.RecordUserAction("Alarm", userId, 0, "vlfAlarm",
                                                  string.Format("AlarmId={0}", alarmId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Accept alarm({0})", alarmId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AcceptCurrentAlarm : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Alarms Severity")]
        public int GetAlarmSeverityXMLByLang(int userId, string SID, string lang, ref string xml)
        {
            DateTime dtNow = DateTime.Now;

            try
            {

                Log(">> GetAlarmSeverityXMLByLang : uId={0}, lang={1}", userId, lang);

                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet ds = new DataSet();

                DataTable tblSeverity = ds.Tables.Add("Severity");
                tblSeverity.Columns.Add("SeverityId", typeof(short));
                tblSeverity.Columns.Add("SeverityName", typeof(string));

                Array enmArr = Enum.GetValues(typeof(VLF.CLS.Def.Enums.AlarmSeverity));
                string Severity;
                object[] objRow;
                foreach (VLF.CLS.Def.Enums.AlarmSeverity ittr in enmArr)
                {
                    Severity = Enum.GetName(typeof(VLF.CLS.Def.Enums.AlarmSeverity), ittr);
                    objRow = new object[2];
                    objRow[0] = Convert.ToInt16(ittr);
                    objRow[1] = Severity;
                    ds.Tables[0].Rows.Add(objRow);

                }

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                if (ASIErrorCheck.IsLangSupported(lang))
                {
                    //LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                    LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(cns);
                    dbl.LocalizationData(lang, "SeverityId", "SeverityName", "AlarmSeverity", ref ds);
                }

                xml = ds.GetXml();

                LogFinal("<< GetAlarmSeverityXMLByLang : uId={0}, lang={1}, tSpan={2}",
                               userId, lang, DateTime.Now.Subtract(dtNow).TotalMilliseconds);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmSeverityXMLByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Accept alarm by Alarm ID.")]
        public int AcceptCurrentAlarm(int userId, string SID, int alarmId)
        {
            try
            {
                Log(">> AcceptCurrentAlarm(uId={0}, alarmId={1})", userId, alarmId);

                if (!ValidateUserAlarm(userId, SID, alarmId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DateTime timeAccepted = DateTime.Now.AddHours(-Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone));

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) //  != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeAck"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already accepted.");
                        }
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeClosed"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already closed.");
                        }
                    }
                    else
                    {
                        throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " does not exist in the system.");
                    }

                    LoggerManager.RecordInitialValues("Alarm", userId, 0, "vlfAlarm",
                                                  string.Format("AlarmId={0}", alarmId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Accept alarm({0}) - Initial values", alarmId));

                    dbAlarm.AcceptAlarm(alarmId, timeAccepted, userId);

                    LoggerManager.RecordUserAction("Alarm", userId, 0, "vlfAlarm",
                                                  string.Format("AlarmId={0}", alarmId),
                                                  "Update", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Accept alarm({0})", alarmId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AcceptCurrentAlarm : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Take over accepted alarm.")]
        public int TakeOverAlarm(int userId, string SID, int alarmId)
        {
            try
            {
                Log(">> TakeOverAlarm(uId={0}, alarmId={1})", userId, alarmId);

                if (!ValidateUserAlarm(userId, SID, alarmId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DateTime timeAccepted = DateTime.Now.AddHours(-Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone));

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    //					if(dsAlarmInfo != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo)) 
                    {
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeAck"] == DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has not been accepted yet.");
                        }
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeClosed"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already closed.");
                        }
                    }
                    else
                    {
                        throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " does not exist in the system.");
                    }

                    LoggerManager.RecordInitialValues("Alarm", userId, 0, "vlfAlarm",
                                                    string.Format("AlarmId={0}", alarmId.ToString()),
                                                    "Update", this.Context.Request.UserHostAddress,
                                                    this.Context.Request.RawUrl,
                                                    string.Format("Take over alarm({0}) - Initial values", alarmId));

                    dbAlarm.AcceptAlarm(alarmId, timeAccepted, userId);

                    LoggerManager.RecordUserAction("Alarm", userId, 0, "vlfAlarm",
                                                   string.Format("AlarmId={0}", alarmId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Take over alarm({0})", alarmId.ToString()));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< TakeOverAlarm : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Close alarm.")]
        public int CloseCurrentAlarm(int userId, string SID, int alarmId)
        {
            try
            {
                Log(">> CloseCurrentAlarm(uId={0}, alarmId={1})", userId, alarmId);

                if (!ValidateUserAlarm(userId, SID, alarmId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DateTime timeClosed = DateTime.Now.AddHours(-Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone));

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    //					if(dsAlarmInfo != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    if (ASIErrorCheck.IsAnyRecord(dsAlarmInfo)) //if (ASLUtil.IsAnyRecord(dsAlarmInfo))  
                    {
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeClosed"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already closed.");
                        }
                    }
                    else
                    {
                        throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " does not exist in the system.");
                    }

                    LoggerManager.RecordInitialValues("Alarm", userId, 0, "vlfAlarm",
                                                   string.Format("AlarmId={0}", alarmId.ToString()),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Close alarm({0}) - Initial values", alarmId.ToString()));

                    dbAlarm.CloseAlarm(alarmId, timeClosed, userId);

                    LoggerManager.RecordUserAction("Alarm", userId, 0, "vlfAlarm",
                                                   string.Format("AlarmId={0}", alarmId.ToString()),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Close alarm({0})", alarmId.ToString()));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< CloseCurrentAlarm : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Close alarm with reason or extra information.")]
        public int CloseCurrentAlarmWithNotes(int userId, string SID, int alarmId, string notes)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("CloseCurrentAlarm( userId = {0}, alarmId = {1} ,notes = {2} )", userId, alarmId, notes)));

                // Authenticate & Authorize
                //LoginManager.GetInstance().SecurityCheck(userId, SID);


                //Authorization
                VLF.DAS.Logic.User dbUser = new User(Application["ConnectionString"].ToString());
                //  if (!dbUser.ValidateUserAlarm(userId, alarmId))
                //     return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DateTime timeClosed = DateTime.Now.AddHours(-Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone));
                Alarm dbAlarm = null;
                try
                {
                    dbAlarm = new Alarm(Application["ConnectionString"].ToString());
                    DataSet dsAlarmInfo = dbAlarm.GetAlarmInfoByAlarmId(alarmId);
                    if (dsAlarmInfo != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0)
                    {
                        if (dsAlarmInfo.Tables[0].Rows[0]["DateTimeClosed"] != DBNull.Value)
                        {
                            throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " has been already closed.");
                        }
                    }
                    else
                    {
                        throw new ERR.ASIWrongOperation("Current alarm " + alarmId + " does not exist in the system.");
                    }

                    LoggerManager.RecordInitialValues("Alarm", userId, 0, "vlfAlarm",
                                                   string.Format("AlarmId={0}", alarmId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Close alarm({0}) - Initial values", alarmId));

                    dbAlarm.CloseAlarm(alarmId, timeClosed, userId, notes);

                    LoggerManager.RecordUserAction("Alarm", userId, 0, "vlfAlarm",
                                                   string.Format("AlarmId={0}", alarmId),
                                                   "Update", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Close alarm({0})", alarmId));
                }
                finally
                {
                    if (dbAlarm != null)
                        dbAlarm.Dispose();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        // Changes for TimeZone Feature start

        [WebMethod(Description = "Get Alarms Short Info CheckSum")]
        public int GetAlarmsShortInfoCheckSum_NewTZ(int userId, string SID, float userTimeZone, ref string checksum)
        {
            try
            {
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    LogException("-- GetAlarmsShortInfoCheckSum : Cannot find alarm life time. ASI will use default: {0}", alarmLifeTime);
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                float fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                float toTimeShift = -userTimeZone;

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    checksum = dbAlarm.GetAlarmsShortInfoCheckSum_NewTZ(DateTime.Now.AddHours(fromTimeShift),
                                                                     DateTime.Now.AddHours(toTimeShift),
                                                                     userId);
                }

                Log("<< GetAlarmsShortInfoCheckSum : checksum={0} , uId={1}", checksum, userId);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsShortInfoCheckSum : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end

        [WebMethod(Description = "Get Alarms Short Info CheckSum")]
        public int GetAlarmsShortInfoCheckSum(int userId, string SID, short userTimeZone, ref string checksum)
        {
            try
            {
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                // Retrieve timeshift from DB
                int alarmLifeTime = 24;
                try
                {
                    alarmLifeTime = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AlarmLifeTime"]);
                    if (alarmLifeTime < 0 || alarmLifeTime > Int32.MaxValue/*2,147,483,647*/)
                    {
                        alarmLifeTime = 24;
                        Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, "Alarm life time doesn't setup properly. ASI will use default: " + alarmLifeTime));
                    }
                }
                catch (Exception exp)
                {
                    LogException("-- GetAlarmsShortInfoCheckSum : Cannot find alarm life time. ASI will use default: {0}", alarmLifeTime);
                }
                //int fromTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) - alarmLifeTime - 12;
                int fromTimeShift = -userTimeZone - alarmLifeTime;
                //int toTimeShift = - Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone) + 13;
                int toTimeShift = -userTimeZone;

                string cns = "";
                int _organizationId = GetOrganizationIdByUserId(userId);
                if (_organizationId == 952)
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["G4SAlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }
                else
                {
                    if (System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"] != null)
                        cns = System.Configuration.ConfigurationManager.ConnectionStrings["AlarmDBConnectionString"].ConnectionString;
                    else
                        cns = LoginManager.GetConnnectionString(userId);
                }

                //using (Alarm dbAlarm = new Alarm(LoginManager.GetConnnectionString(userId)))
                using (Alarm dbAlarm = new Alarm(cns))
                {
                    checksum = dbAlarm.GetAlarmsShortInfoCheckSum(DateTime.Now.AddHours(fromTimeShift),
                                                                     DateTime.Now.AddHours(toTimeShift),
                                                                     userId);
                }

                Log("<< GetAlarmsShortInfoCheckSum : checksum={0} , uId={1}", checksum, userId);

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsShortInfoCheckSum : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        private Int32 GetOrganizationIdByUserId(Int32 userId)
        {

            string ConnnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            Int32 OrganizationId = -1;
            DataSet ds = new DataSet();


            using (VLF.DAS.Logic.User lg = new VLF.DAS.Logic.User(ConnnectionString))
            {

                ds = lg.GetUserInfoById(userId);

                if (VLF.CLS.Util.IsDataSetValid(ds))
                {
                    OrganizationId = Convert.ToInt32(ds.Tables[0].Rows[0]["OrganizationId"]);
                }
            }

            return OrganizationId;
        }


    }
}
