using System;
using System.Configuration;
using System.Collections ;
using System.Diagnostics;
using VLF.CLS;
using VLF.DAS.Logic ;
using System.Web.Configuration;
namespace VLF.ASI
{
	/// <summary>
	/// Server Configuration module
	/// Contains static and dynamic Server settings
	/// Retrieves connection string to database from the xml file
	/// All other settings retrieves from database.
	/// </summary>
	public class AppConfig //: ASLBase
	{
		#region private variables
		private string connectionString = null;
		private string logDirectory = null;
		private string moduleName = "";
		private short serverTimeZone = 0;
      private int moduleId = -1;
		#endregion private variables

		#region static properties
		public static TraceSwitch tsMain;
		public static BooleanSwitch tsStat;
		public static BooleanSwitch tsPing;
		public static BooleanSwitch tsWeb;
		public static BooleanSwitch tsDas;
		#endregion static properties

		#region public properties
		/// <summary>
		/// Return server time zone
		/// </summary>
		public short ServerTimeZone
		{
			get{return serverTimeZone;}
		}
		/// <summary>
		/// Return connection string to database
		/// </summary>
		public string ConnectionString
		{
			get{return connectionString;}
		}
		/// <summary>
		/// Return log directory
		/// </summary>
		public string LogDirectory
		{
			get{return logDirectory;}
		}
		/// <summary>
		/// Return module name for configuration settings
		/// </summary>
		public string ModuleName
		{
			get{return moduleName;}
		}

      public int ModuleId
      {
         get { return moduleId; }
      }

		#endregion public properties

		#region Singleton functionality
		private static AppConfig instance = null ;
		/// <summary>
		/// Constructor
		/// </summary>
		private AppConfig(string moduleName)
		{
			this.moduleName = moduleName;
			tsMain = new TraceSwitch("tsMain","General") ;
			tsStat = new BooleanSwitch("tsStat","Statistics") ;
			tsPing = new BooleanSwitch("tsPing","Ping") ;
			tsWeb = new BooleanSwitch("tsWEB","Web Interfaces") ;
			tsDas = new BooleanSwitch("tsDAS","Web Interfaces") ;
			
			// we don't know log path yet - will change later
			logDirectory = "C:/";
			SystemConfig systemConfig = null;
			string timeShift = "-5";
			try
			{
				// 1. get DB connection string from acm.exe.config file
				//connectionString = System.Configuration.ConfigurationSettings.AppSettings["DBConnectionString"];
                connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString ;
                try
                {
                    systemConfig = new SystemConfig(connectionString);
                    // 2. retrieve path for logging and make full address
                    //logDirectory = systemConfig.GetConfigParameter(moduleName,(short)VLF.CLS.Def.Enums.ConfigurationGroups.LOG,"Log Path");
                    timeShift = systemConfig.GetConfigParameter(moduleName, (short)VLF.CLS.Def.Enums.ConfigurationGroups.Common, "Time Shift");
                }
                catch
                {
                }
			}
			catch(ERR.VLFException e)
			{
				//ERR.LOG.LogFile(logDirectory,moduleName,e.DetailLogMsg);
				if(systemConfig != null)
				{
					systemConfig.Dispose();
					systemConfig = null;
				}
				return;
			}
			catch(Exception exp)
			{
				ERR.LOG.LogFile(logDirectory,moduleName,exp.Message);
				if(systemConfig != null)
				{
					systemConfig.Dispose();
					systemConfig = null;
				}
				return;
			}
			finally
			{
				if(systemConfig != null)
					systemConfig.Dispose();
			}


            logDirectory = "C:/";
            try
            {
                //retrieve path for logging and make full address
                logDirectory = ConfigurationSettings.AppSettings["LogFolder"];
            }
            catch (Exception)
            {
                VLF.ERR.LOG.LogFile(logDirectory, "ASI", "Log Directory does not exists");
                return;
            }

			try
			{
                string logData = @logDirectory + "/" + moduleName + "_" + DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".log";
                //TextWriterTraceListener fileInfo = new TextWriterTraceListener(logData, logData) ;
                File4DayTraceListener fileInfo = new File4DayTraceListener(logData, logData);
                Trace.Listeners.Add(fileInfo);
                Trace.AutoFlush = true;

            /**
              <add key="RemoteIPLogger" value="192.168.9.45"/>
              <add key="RemotePortLogger" value="4060"/>     
              <add key="ASI_ID" value="100"/>    
             */
            moduleId = Convert.ToInt32(ConfigurationSettings.AppSettings["ASI_ID"]);
            string ipLogger = ConfigurationSettings.AppSettings["RemoteIPLogger"];
            ushort ipPort = Convert.ToUInt16(ConfigurationSettings.AppSettings["RemotePortLogger"]);
            VLF.CLS.UDPLogListener logListener = new VLF.CLS.UDPLogListener((int)moduleId, ipLogger, ipPort);
            Trace.Listeners.Add(logListener);

				if(timeShift == "")
					Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning,  "Time Shift does not exist." ));
				else if( Convert.ToInt16(serverTimeZone) > 13 || Convert.ToInt16(serverTimeZone) < -12 )
					Trace.WriteLineIf(AppConfig.tsMain.TraceWarning,CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Warning,  "Time Shift " + serverTimeZone + " is out of range. Setting up GMT time." ));
				serverTimeZone = Convert.ToInt16(timeShift);
			}
			catch(Exception ex)
			{
				Trace.WriteLineIf(tsMain.TraceError,CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,"Exception in AppConfig(): " + ex.Message));
			}
		}	

		public static AppConfig GetInstance()
		{
			if( instance == null )
				instance = new AppConfig("ASI") ;
			return instance ;
		}
		#endregion Singleton functionality


       

	}
}
