using System;
using System.Configuration;
using System.Collections ;
using System.Diagnostics;
using VLF.CLS;

namespace SentinelFM
{
	/// <summary>
	/// Server Configuration module
	/// Contains static and dynamic Server settings
	/// Should be loaded from the xml file
	/// Singleton
	/// </summary>
	public class AppConfig //: ASLBase
	{
		#region private variables
		private string logDirectory = null;
		
		#endregion private variables

		#region static properties
		public static TraceSwitch tsMain;
		public static BooleanSwitch tsReports;
		#endregion static properties

		#region public properties

		public string LogDirectory
		{
			get{return logDirectory;}
		}

		#endregion public properties

		#region Singleton functionality
		private static AppConfig instance = null ;
		private AppConfig()
		{
			tsMain = new System.Diagnostics.TraceSwitch("tsMain","Used for common purposes") ;
			tsReports = new BooleanSwitch("tsReports","Reports") ;

			// we don't know log path yet - will change later
			logDirectory = "C:/";
			try
			{
				//retrieve path for logging and make full address
				logDirectory = ConfigurationSettings.AppSettings["LogFolder"];
			}
			catch(Exception)
			{
				
				return;
			}
			
			try
			{
				string logData = @logDirectory + "/" + "SentinelFM" + "_" + DateTime.Now.ToShortDateString().Replace(@"/",@"-").Replace(@"\",@"-") + ".log";
				TextWriterTraceListener fileInfo = new TextWriterTraceListener(logData, logData) ;
				Trace.Listeners.Add( fileInfo ) ;
				Trace.AutoFlush = true ;
			}
			catch(Exception ex)
			{

			}
		}	

		public static AppConfig GetInstance()
		{
			if( instance == null )
				instance = new AppConfig() ;
			return instance ;
		}
		#endregion Singleton functionality

	}
}
