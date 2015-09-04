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
	public sealed class AppConfig //: ASLBase
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
		//private static AppConfig instance = null ;
      private static readonly  AppConfig instance =  new AppConfig() ;
      AppConfig()
      {
         tsMain = new System.Diagnostics.TraceSwitch("tsMain", "Used for common purposes");
         tsReports = new BooleanSwitch("tsReports", "Reports");

         // we don't know log path yet - will change later
         logDirectory = "C:/";
         try
         {
            //retrieve path for logging and make full address
            logDirectory = ConfigurationSettings.AppSettings["LogFolder"];
         }
         catch (Exception)
         {
            VLF.ERR.LOG.LogFile(logDirectory, "SentinelFM", "Log Directory does not exists");
            return;
         }

         try
         {
             string logData = logDirectory + "/" + "SentinelFM" + "_" + DateTime.Now.ToString("MM/dd/yyyy").Replace("//", @"-").Replace(@"\", @"-").Replace(@"/", @"-") + ".log";
            // string logDataErr = logDirectory + "/" + "SentinelFM_ERR" + "_" + DateTime.Now.ToString("MM/dd/yyyy").Replace("//", @"-").Replace(@"\", @"-").Replace(@"/", @"-") + ".log";
            ///TextWriterTraceListener fileInfo = new TextWriterTraceListener(logData, logData);
            File4DayTraceListener fileInfo = new File4DayTraceListener(logData, logData);
            //File4DayTraceListener fileInfoErr = new File4DayTraceListener(logDataErr, logDataErr);
            Trace.Listeners.Add(fileInfo);
          //  Trace.Listeners.Add(fileInfoErr);
            Trace.WriteLine("AppConfig -> add listener " + logData);				
            Trace.AutoFlush = true;
              
         }
         catch (Exception ex)
         {
            VLF.ERR.LOG.LogFile(logDirectory, "SentinelFM", "Exception in AppConfig(): " + ex.Message);
         }
      }
		static  AppConfig()
		{
		}

      

      //public static AppConfig GetInstance()
      //{
       
      //      if( instance == null )
      //         instance = new AppConfig() ;
      //      return instance ;
       
      //}


      public static AppConfig Instance
      {
         get
         {
            return instance;
         }

      }

		#endregion Singleton functionality

	}
}
