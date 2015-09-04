using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using VLF.ERRSecurity;
using VLF.ERR;
using VLF.DAS.Logic;
using VLF.CLS;
using System.Diagnostics;
namespace VLF.ASI.Interfaces
{

    /// <summary>
    /// Summary description for Garmin
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com")]
    
    public class Garmin : System.Web.Services.WebService
    {

        public Garmin()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

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


        [WebMethod(Description = "Send Text Message to Garmin.")]
        public int SendTextMessage(int userId, string SID, int boxId, int  typeId, string message)
        {
            try
            {
                Log(">> SendTextMessage(uId={0})", userId);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                int result = 0;
                using (VLF.DAS.Logic.Garmin dbGarmin = new VLF.DAS.Logic.Garmin(LoginManager.GetConnnectionString(userId), false))
                {
                    dbGarmin.SendTextMessage(boxId, (VLF.DAS.DB.GarminMessageType)typeId, message, userId);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SendTextMessage : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Send Text Message to Garmin.")]
        public int SendLocationMessage(int userId, string SID, int boxId, int typeId, string message,double latitude, double longitude,string address, string landmarkName)
        {
            try
            {
                Log(">> SendLocationMessage(uId={0})", userId);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                int result = 0;
                using (VLF.DAS.Logic.Garmin dbGarmin = new VLF.DAS.Logic.Garmin(LoginManager.GetConnnectionString(userId), false))
                {
                    dbGarmin.SendLocationMessage(boxId, (VLF.DAS.DB.GarminMessageType)typeId, latitude, longitude, message, userId, address, landmarkName);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SendLocationMessage : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Returns XML contains list of available Garmin devices based on user.")]
        public int GetGarminDevicesByUser(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetGarminDevicesByUser(uId={0})",userId);

                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsGarminDevices = null;
                using (VLF.DAS.Logic.Garmin dbGarmin = new VLF.DAS.Logic.Garmin(LoginManager.GetConnnectionString(userId),false))
                {
                    dsGarminDevices = dbGarmin.GetGarminDevicesByUser (userId);
                }

                if (Util.IsDataSetValid(dsGarminDevices))
                    xml = dsGarminDevices.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetGarminDevicesByUser : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Returns XML history of Garmin text messages")]
        public int GetGarminTextMsgHistory(int userId, string SID,int boxId, DateTime from,DateTime to,bool showstatus,  ref string xml)
        {
            try
            {
                Log(">> GetGarminTextMsgHistory(uId={0},boxId={1}, dtFrom={2},dtTo={3})", userId, boxId, from, to, showstatus);

                
                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsGarmin = null;
                using (VLF.DAS.Logic.Garmin dbGarmin = new VLF.DAS.Logic.Garmin(LoginManager.GetConnnectionString(userId), true))
                {
                    dsGarmin = dbGarmin.GetTextMessages(userId, boxId, from, to, showstatus);
                }

                if (Util.IsDataSetValid(dsGarmin))
                    xml = dsGarmin.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetGarminTextMsgHistory : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Returns XML history of Garmin location messages")]
        public int GetGarminLocationMsgHistory(int userId, string SID, int boxId, DateTime from, DateTime to,bool showstatus, ref string xml)
        {
            try
            {
                Log(">> GetGarminLocationMsgHistory(uId={0},boxId={1}, dtFrom={2},dtTo={3})", userId, boxId, from, to);


                //if (!ValidateUserFleet(userId, SID, fleetId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsGarmin = null;
                using (VLF.DAS.Logic.Garmin dbGarmin = new VLF.DAS.Logic.Garmin(LoginManager.GetConnnectionString(userId), true))
                {
                    dsGarmin = dbGarmin.GetLocationMessages(userId, boxId, from, to, showstatus);
                }

                if (Util.IsDataSetValid(dsGarmin))
                    xml = dsGarmin.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetGarminLocationMsgHistory : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
		
    }
}

