using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;

namespace VLF.ASI 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			// 1. init configuration module
            
         AppConfig appConfig = AppConfig.GetInstance();
			Application.Lock(); 
			Application["ModuleName"]		= appConfig.ModuleName;
            Application["ConnectionString"] = appConfig.ConnectionString; //System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString
			Application["LogDirectory"]		= appConfig.LogDirectory;
			Application.UnLock() ;


         Trace.WriteLine(CLS.Util.TraceFormat("------ " + Application["ModuleName"] + " Started & Initialized ---------"));
         Trace.WriteLine(CLS.Util.TraceFormat("------ " + Application["ModuleName"] + " is connected to " + CLS.Util.PairFindValue("Initial Catalog", Application["ConnectionString"].ToString()) +
            " on " + CLS.Util.PairFindValue("Data Source", Application["ConnectionString"].ToString())));
         Trace.WriteLine(CLS.Util.TraceFormat("------ " + Application["ModuleName"] + " log directory is " + Application["LogDirectory"]));

         

         //Failed Logins Logger
         System.Data.DataTable dtLoginFailedList = new System.Data.DataTable();

         System.Data.DataColumn colUserName = new System.Data.DataColumn("UserName", Type.GetType("System.String"));
         dtLoginFailedList.Columns.Add(colUserName);
         System.Data.DataColumn colIPAddress = new System.Data.DataColumn("IPAddress", Type.GetType("System.String"));
         dtLoginFailedList.Columns.Add(colIPAddress);
         System.Data.DataColumn colNumTrials = new System.Data.DataColumn("NumTrials", Type.GetType("System.Int16"));
         dtLoginFailedList.Columns.Add(colNumTrials);
         System.Data.DataColumn colCycle = new System.Data.DataColumn("Cycle", Type.GetType("System.Int16"));
         dtLoginFailedList.Columns.Add(colCycle);
         System.Data.DataColumn colStatus = new System.Data.DataColumn("Status", Type.GetType("System.String"));
         dtLoginFailedList.Columns.Add(colStatus);
         System.Data.DataColumn colLoginDate = new System.Data.DataColumn("LoginDate", Type.GetType("System.DateTime"));
         dtLoginFailedList.Columns.Add(colLoginDate);

         Application.Add("dtLoginFailedList", dtLoginFailedList);


		 LoginManager.GetInstance() ;
		 SecurityKeyManager.GetInstance() ;
         //AlarmsRemoting.Start();  
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{
         //Exception ex = Context.Error.GetBaseException();
         Exception ex = Server.GetLastError().GetBaseException();

         string Excp = "ASI " +
             "MESSAGE: " + ex.Message +
             "\nSOURCE: " + ex.Source +
             "\nFORM: " + Request.Form.ToString() +
             "\nQUERYSTRING: " + Request.QueryString.ToString() +
             "\nTARGETSITE: " + ex.TargetSite +
             "\nSTACKTRACE: " + ex.StackTrace;


         Trace.WriteLine(CLS.Util.TraceFormat("Unhandled error:" + Excp));

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{
            //AlarmsRemoting.Stop();  
			Application.Clear();
		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

