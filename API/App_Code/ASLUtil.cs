using System;
using System.Diagnostics ;
using System.Data ;			// for DataSet
using VLF.ERRSecurity ;

namespace VLF.ASI
{
	// TODO: Add all possible ASI exception here
	public class ASIErrorCheck 
	{
		public static InterfaceError CheckError( Exception e)
		{
            InterfaceError retResult = ErrorCheck.CheckError(e);
            try
            {
                TraceError(retResult, e.Message, e.Source, e.StackTrace);
            }
            catch
            {
            }
            return retResult;
        }
        public static InterfaceError CheckError(InterfaceError retResult,string errMessage, 
                                                string errSource, string errStackTrace)
        {
            try
            {
                TraceError(retResult, errMessage, errSource, errStackTrace);
            }
            catch
            {
            }
            return retResult;
        }

      private static void TraceError(InterfaceError errCode, string errMessage, string errSource, string errStackTrace)
		{
            errMessage = errCode + " - " + errMessage;
            switch (errCode)
            {
                case InterfaceError.NoError:
                    break;
                case InterfaceError.AuthenticationFailed:
                case InterfaceError.AuthorizationFailed:
                case InterfaceError.UserExpired:
                case InterfaceError.SessionNotFound:
                    Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, 
                                      CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning, errMessage));
                    break;
                case InterfaceError.DatabaseConnectionClosed:
                case InterfaceError.OperationNotSupported:
                case InterfaceError.DatabaseError:
                case InterfaceError.CommInfoError:
                case InterfaceError.SessionIsBusyError:
                case InterfaceError.PermissionDenied:
                case InterfaceError.SessionLogicServerError:
                    Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                                      CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, errMessage));
					break;
                case InterfaceError.PassKeyExpired:
                    Trace.WriteLineIf(AppConfig.tsMain.TraceVerbose, 
                                      CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Verbose, errMessage));
					break;
                default:
                    Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                                      CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, 
                                                           errMessage + errSource + errStackTrace));
                    break;
            }
		}
		public static void StopIIS()
		{
			System.Diagnostics.Process executeCommand = new System.Diagnostics.Process();
			executeCommand.StartInfo.FileName = "net";
			executeCommand.StartInfo.Arguments = "stop iisadmin /y";
			executeCommand.Start();
		}

      public static bool IsAnyRecord(DataSet dsAlarmInfo)
      {
         return (dsAlarmInfo != null && dsAlarmInfo.Tables.Count > 0 && dsAlarmInfo.Tables[0].Rows.Count > 0);
      }

      /// <summary>
      ///      this function allow to switch from one language to another
      /// </summary>
      /// <param name="lang"></param>
      /// <returns></returns>
      public static bool IsLangSupported(string lang)
      {
         return (lang == "fr");
      }

      public static System.Globalization.CultureInfo NewCultureInfo(string lang)
      {
         return (lang != "en" && lang != null ? new System.Globalization.CultureInfo(lang) : null);
      }
	}
}
