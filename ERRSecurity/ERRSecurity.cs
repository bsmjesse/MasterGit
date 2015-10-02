using System;
//using System.Diagnostics;

namespace VLF.ERRSecurity
{
	/// <summary>
	/// Main ASI Error Codes Definitions
	/// </summary>
	public enum InterfaceError
	{
      NoError					= 0,
		ServerError				= 1,
		InvalidParameter		= 2,
		PermissionDenied		= 3,
		AuthenticationFailed	= 4,
		PassKeyExpired			= 5,
		DatabaseError			= 6,
		NotImplemented			= 7,
		ServerBusy				= 8,
		EventAlreadyExists		= 9,
		SessionIsAtomic			= 10,
		SessionNotFound			= 11,
		NotFound				= 12,
		OperationNotSupported	= 13,
		SessionLogicServerError	= 14,
		AuthorizationFailed		= 15,
		UserExpired				= 16,
		DatabaseConnectionClosed= 17,
      CommInfoError           = 18,
      SessionIsBusyError      = 19,
      CallFrequencyExceeded = 20,
      PasswordNotInRules = 21,
      PasswordDuplicatedInLastEight = 22,

	}
	
	public class InterfaceErrorDescription
	{	
		public static string GetDescription(InterfaceError errorCode )
		{
         //localization settings
         Resources.ERRSecurityConst.Culture = new System.Globalization.CultureInfo("en");

         
			string errDesc =Resources.ERRSecurityConst.ServerErrorTitle  ;
			switch( errorCode )
			{
				case InterfaceError.NoError:
					errDesc += Resources.ERRSecurityConst.NoError  ;
					break ;
				case InterfaceError.ServerError:
               errDesc += Resources.ERRSecurityConst.ServerError ;
					break ;
				case InterfaceError.NotImplemented:
               errDesc += Resources.ERRSecurityConst.NotImplemented ;
					break ;
				case InterfaceError.AuthenticationFailed:
               errDesc += Resources.ERRSecurityConst.AuthenticationFailed ;
					break ;
				case InterfaceError.PassKeyExpired:
               errDesc += Resources.ERRSecurityConst.PassKeyExpired ;
					break ;
				case InterfaceError.PermissionDenied:
               errDesc += Resources.ERRSecurityConst.PermissionDenied ;
					break ;
				case InterfaceError.DatabaseError:
               errDesc += Resources.ERRSecurityConst.DatabaseError ;
					break ;
				case InterfaceError.InvalidParameter:
               errDesc += Resources.ERRSecurityConst.InvalidParameter ;
					break ;
				case InterfaceError.ServerBusy:
               errDesc += Resources.ERRSecurityConst.SessionIsBusy ;
					break ;
				case InterfaceError.SessionNotFound:
               errDesc += Resources.ERRSecurityConst.SessionNotFound ;
					break ;
				case InterfaceError.OperationNotSupported:
               errDesc += Resources.ERRSecurityConst.OperationNotSupported ;
					break ;
                case InterfaceError.CommInfoError:
                   errDesc += Resources.ERRSecurityConst.CommunicationInfoMissing ;
					break ;
                case InterfaceError.SessionIsBusyError:
                   errDesc += Resources.ERRSecurityConst.SessionBusyExecutionAnotherCommand ;
                    break;
				case InterfaceError.NotFound:
               errDesc += Resources.ERRSecurityConst.NotFound ;
					break ;
				case InterfaceError.UserExpired:
               errDesc += Resources.ERRSecurityConst.UserExpired ;
					break ;
				default:
               errDesc += Resources.ERRSecurityConst.UnknownError ;
					break;
			}
			return errDesc ;
		}
      public static string GetDescription(InterfaceError errorCode,string lang)
      {

         //localization settings
         Resources.ERRSecurityConst.Culture = new System.Globalization.CultureInfo(lang);

         string errDesc = Resources.ERRSecurityConst.ServerErrorTitle;
         switch (errorCode)
         {
            case InterfaceError.NoError:
               errDesc += Resources.ERRSecurityConst.NoError;
               break;
            case InterfaceError.ServerError:
               errDesc += Resources.ERRSecurityConst.ServerError;
               break;
            case InterfaceError.NotImplemented:
               errDesc += Resources.ERRSecurityConst.NotImplemented;
               break;
            case InterfaceError.AuthenticationFailed:
               errDesc += Resources.ERRSecurityConst.AuthenticationFailed;
               break;
            case InterfaceError.PassKeyExpired:
               errDesc += Resources.ERRSecurityConst.PassKeyExpired;
               break;
            case InterfaceError.PermissionDenied:
               errDesc += Resources.ERRSecurityConst.PermissionDenied;
               break;
            case InterfaceError.DatabaseError:
               errDesc += Resources.ERRSecurityConst.DatabaseError;
               break;
            case InterfaceError.InvalidParameter:
               errDesc += Resources.ERRSecurityConst.InvalidParameter;
               break;
            case InterfaceError.ServerBusy:
               errDesc += Resources.ERRSecurityConst.SessionIsBusy;
               break;
            case InterfaceError.SessionNotFound:
               errDesc += Resources.ERRSecurityConst.SessionNotFound;
               break;
            case InterfaceError.OperationNotSupported:
               errDesc += Resources.ERRSecurityConst.OperationNotSupported;
               break;
            case InterfaceError.CommInfoError:
               errDesc += Resources.ERRSecurityConst.CommunicationInfoMissing;
               break;
            case InterfaceError.SessionIsBusyError:
               errDesc += Resources.ERRSecurityConst.SessionBusyExecutionAnotherCommand;
               break;
            case InterfaceError.NotFound:
               errDesc += Resources.ERRSecurityConst.NotFound;
               break;
            case InterfaceError.UserExpired:
               errDesc += Resources.ERRSecurityConst.UserExpired;
               break;
            default:
               errDesc += Resources.ERRSecurityConst.UnknownError;
               break;
         }
         return errDesc;
      }

	}




    public class ErrorCheck
    {
        public static InterfaceError CheckError(Exception Ex)
        {
            if (Ex.GetType().IsSubclassOf(typeof(VLF.ERR.VLFException)))
            {
                VLF.ERR.VLFException vlfEx = (VLF.ERR.VLFException)Ex;


                if (vlfEx.GetType() == typeof(VLF.ERR.ASICallFrequencyExceededException))
                    return InterfaceError.CallFrequencyExceeded;
                else if (vlfEx.GetType() == typeof(VLF.ERR.ASIAuthenticationFailedException))
                    return InterfaceError.AuthenticationFailed;
                else if (vlfEx.GetType() == typeof(VLF.ERR.ASIAuthorizationFailedException) ||
                    vlfEx.GetType() == typeof(VLF.ERR.DASAuthorizationException))
                    return InterfaceError.AuthorizationFailed;
                else if (vlfEx.GetType() == typeof(VLF.ERR.ASIUserExpired))
                    return InterfaceError.UserExpired;
                else if (vlfEx.GetType() == typeof(VLF.ERR.DASConnectionsLimitReached))
                    return InterfaceError.DatabaseError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.SLSOperationNotSupported))
                 return InterfaceError.OperationNotSupported;
                else if (vlfEx.GetType() == typeof(VLF.ERR.SLSVehicleConfigurationErrorException))
                    return InterfaceError.DatabaseError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.SLSCommunicationInformationErrorException))
                    return InterfaceError.CommInfoError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.SLSSessionIsBusyException))
                    return InterfaceError.SessionIsBusyError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.ASIDataNotFoundException))
                    return InterfaceError.DatabaseError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.ASIPermissionDeniedException))
                    return InterfaceError.PermissionDenied;
                else if (vlfEx.GetType() == typeof(VLF.ERR.ASIPassKeyExpiredException))
                    return InterfaceError.PassKeyExpired;
                else if (vlfEx.GetType() == typeof(VLF.ERR.DASException))
                    return InterfaceError.DatabaseError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.SLSException))
                    return InterfaceError.SessionLogicServerError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.SLSSessionNotFound))
                    return InterfaceError.SessionNotFound;
                else if (vlfEx.GetType() == typeof(VLF.ERR.AMSOperationNotSupported))
                    return InterfaceError.OperationNotSupported;
                else if (vlfEx.GetType() == typeof(VLF.ERR.AMSVehicleConfigurationErrorException))
                    return InterfaceError.DatabaseError;
                else if (vlfEx.GetType() == typeof(VLF.ERR.DASDbConnectionClosed))
                    return InterfaceError.DatabaseConnectionClosed;

                //Trace.WriteLineIf(AppConfig.tsMain.TraceError, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, vlfEx.Message + vlfEx.Source + vlfEx.StackTrace));
                return InterfaceError.ServerError;
            }
            else
            {
                //Trace.WriteLineIf(AppConfig.tsMain.TraceError, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, Ex.Message + Ex.Source + Ex.StackTrace));
                return InterfaceError.ServerError;
            }
        }
     }	
}
