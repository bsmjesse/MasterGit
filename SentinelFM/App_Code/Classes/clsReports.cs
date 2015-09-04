using System;
using System.Threading ;
using System.Configuration;
using System.Data ;
namespace SentinelFM
{
	/// <summary>
	/// Summary description for Reports.
	/// </summary>
	public class clsReports
	{

		


		private Thread thReportCleaner ;

		private clsReports()
		{
			thReportCleaner = new Thread( new ThreadStart( CleanReports ) ) ;
			thReportCleaner.Start() ;
		}


		private void CleanReports()
		{
			try
			{
				while(thReportCleaner.ThreadState == ThreadState.Running)
				{
					string[] files ;
					// clean map folder; for now hardcoded 1 minute interval and 5 min lifetime
					string strPath= @ConfigurationSettings.AppSettings["serverPath"]+@ConfigurationSettings.AppSettings["ReportFolder"];
					
					files = System.IO.Directory.GetFiles(strPath, "*.*") ;
					foreach( string file in files)
					{
						if( System.IO.File.GetCreationTime(file) < DateTime.Now.AddMinutes(-5) )
							System.IO.File.Delete(file) ;
					}
				
					Thread.Sleep( new TimeSpan(0,1,0) ) ;
				}
			}
			catch
			{
				return ;
			}
		}
		public void Dispose()
		{
			thReportCleaner.Abort() ;
		}

		#region Singleton functionality
		private static clsReports instance ;
		public static  clsReports GetInstance()
		{
			if(instance == null)
				instance = new clsReports() ;
			return instance ;
		}
		#endregion Singleton functionality
	}
}
