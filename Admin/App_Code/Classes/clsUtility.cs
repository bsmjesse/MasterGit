using System;
using VLF.ERRSecurity;

namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsUtility.
	/// </summary>
	public class clsUtility
	{

		protected SentinelFMSession sn=null;

		public clsUtility(SentinelFMSession sn)
		{
			this.sn=sn;
		}


		public bool ErrCheck( int res, bool retrying )
		{
			bool retResult = true;
			switch( (InterfaceError)res )
			{
				case InterfaceError.NoError:
					retResult = false ;
					break;

				case InterfaceError.ServerError:
					retResult = true ;
					break;

				case InterfaceError.PassKeyExpired:
					//System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceVerbose,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Verbose,"ErrCheck::InterfaceError.PassKeyExpired"));    
					// if expired, we have to re-login and retry again...
					// if we call te method after relogin and still expired, handle an error!
					if(!retrying)
					{
						// relogin
						SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager() ;
						string secId = "";
						int result = sec.Relogin(  sn.UserName , sn.Password ,sn.User.IPAddr, ref secId ) ;
						if(result!=0)
							sn.SecId= secId;
					}
					break;
				default:
					// get error description
					
					break;
					
			}
			return retResult;
		}

		public Int16 IsDayLightSaving(bool AutoAdjustDayLightSaving)
		{
			if	(AutoAdjustDayLightSaving)
			{
				TimeZone timeZone = System.TimeZone.CurrentTimeZone;
				return Convert.ToInt16(timeZone.IsDaylightSavingTime(DateTime.Now));
			}
			else
			{
				return 0;
			}

		}
	}
}
