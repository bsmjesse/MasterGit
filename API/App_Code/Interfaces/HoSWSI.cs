using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.ComponentModel;
using System.Diagnostics;
using VLF.ERRSecurity;

using SentinelHoSIntegration;

/// <summary>
/// Summary description for HoSWSI (Hour of Service)
/// </summary>
namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "http://www.sentinelfm.com", Description = "Sentinel Web Service Methods Exposer.", Name = "HoSWSI")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class HoSWSI : System.Web.Services.WebService
    {
        HoSDataIntegration iHoSDataIntegration = new HoSDataIntegration();

        public HoSWSI()
        {
            //Uncomment the following line if using designed components 
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

        #region Refactored Functions
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
                throw new Exception(exc.Message);
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
                throw new Exception(exc.Message);
            }
        }
        #endregion Refactored Functions 

        #region USER LOG ON 

        [WebMethod(Description = "Used to log on to the HoS system. XML Format :[userName],[password],[userIp],[userId],[SID]")]//,EnableSession=true)]

        public int Login(string userName, string password, string userIp, ref int userId, ref string SID)
        {
            using (SecurityManager iSecurityManager = new SecurityManager())
            {
                return iSecurityManager.Login(userName, password, userIp, ref userId, ref SID);
            }
        }
        #endregion

        #region UP TO DATE DATA 
        /// <summary>
        /// New HoS log data.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = @"Returns XML (&#60;LogDateInfoData&#62; ... &#60;/LogDateInfoData&#62;) of HoS new log data. XML Format: [CompanyID],[eLogID],[DriverID],[DriverName],[CompanyName],[CompanyAddress],[DistanceUnit],[DriverCycle],[DriverRegion],[LogDate],[Documents],[CoDriverIDs],[CoDrivers],[EditCount],[SensorFailureCount],[OffDuty],[SleeperBerth],[Driving],[OnDutyNotDriving],[OnDutyToday],[OnDutyThisWeek],[DistanceDriven],[TimeZone],[Trailers],[DutyStatusInfoCount],[DutyStatusInfo],[StartTime],[Duration],[CurrentActivity],[Location],[EditFlag],[Confirmed],[SensorFailure],[DistanceEdit],[Comment],[DutyStatusInfo],[TractorCount]&#60;TractorList&#62;[TractorID],[LicensePlate],[OdometerStart][OdometerEnd]&#60;/TractorList&#62;")]
        public int GetNewLogData(int userId, string SID, ref string xml)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                Log(">> GetNewLogData(uId={0})", userId);
                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                xml= iHoSDataIntegration.GetHOSInegrationNewLogData(userId);

                Log("<< GetNewLogData(uId={0}, tSpan={1})", userId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetNewLogData : uid={0}, , EXC={3}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }
        #endregion

        #region HISTORICAL DATA 
        /// <summary>
        /// Historical log data for a specific date.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="iDateTime"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = @"Returns XML (&#60;LogDateInfoData&#62; ... &#60;/LogDateInfoData&#62;) of HoS historical log data for specific date. XML Format: [CompanyID],[eLogID],[DriverID],[DriverName],[CompanyName],[CompanyAddress],[DistanceUnit],[DriverCycle],[DriverRegion],[LogDate],[Documents],[CoDriverIDs],[CoDrivers],[EditCount],[SensorFailureCount],[OffDuty],[SleeperBerth],[Driving],[OnDutyNotDriving],[OnDutyToday],[OnDutyThisWeek],[DistanceDriven],[TimeZone],[Trailers],[DutyStatusInfoCount],[DutyStatusInfo],[StartTime],[Duration],[CurrentActivity],[Location],[EditFlag],[Confirmed],[SensorFailure],[DistanceEdit],[Comment],[DutyStatusInfo],[TractorCount]&#60;TractorList&#62;[TractorID],[LicensePlate],[OdometerStart],[OdometerEnd]&#60;/TractorList&#62;")]
        public int GetHistLogData(int userId, string SID, DateTime iDateTime, ref string xml)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                Log(">> GetHistLogData(uId={0})", userId);
                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                xml = iHoSDataIntegration.GetHOSInegrationHistLogData(userId,iDateTime);

                Log("<< GetHistLogData(uId={0}, tSpan={1})", userId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetNewLogData : uid={0}, , EXC={3}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        /// <summary>
        /// Historical log data for specific driver with date range.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="driverId"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = @"Returns XML (&#60;LogDateInfoData&#62; ... &#60;/LogDateInfoData&#62;) of HoS historical log data for driver with date range (max 30 days). XML Format: [CompanyID],[eLogID],[DriverID],[DriverName],[CompanyName],[CompanyAddress],[DistanceUnit],[DriverCycle],[DriverRegion],[LogDate],[Documents],[CoDriverIDs],[CoDrivers],[EditCount],[SensorFailureCount],[OffDuty],[SleeperBerth],[Driving],[OnDutyNotDriving],[OnDutyToday],[OnDutyThisWeek],[DistanceDriven],[TimeZone],[Trailers],[DutyStatusInfoCount],[DutyStatusInfo],[StartTime],[Duration],[CurrentActivity],[Location],[EditFlag],[Confirmed],[SensorFailure],[DistanceEdit],[Comment],[DutyStatusInfo],[TractorCount]&#60;TractorList&#62;[TractorID],[LicensePlate],[OdometerStart][OdometerEnd]&#60;/TractorList&#62;")]
        public int GetDriverHistLogData(int userId, string SID, int driverId, DateTime dtFrom, DateTime dtTo, ref string xml)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                Log(">> GetNewLogData(uId={0})", userId);
                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                xml = iHoSDataIntegration.GetHOSInegrationDriverHistLogData(userId, driverId, dtFrom, dtTo);

                Log("<< GetDriverHistLogData(uId={0}, tSpan={1})", userId, DateTime.Now.Subtract(dtNow).TotalMilliseconds);
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverHistLogData : uid={0}, , EXC={3}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }
        #endregion

    }
}