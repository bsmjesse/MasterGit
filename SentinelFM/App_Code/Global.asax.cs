using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;

namespace SentinelFM
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
			clsReports.GetInstance() ;
          
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

		}

		protected void Session_End(Object sender, EventArgs e)
		{
            try
            {
                Session["SentinelFMSession"] = null;
                Session.RemoveAll();
                Session.Clear();
            }
            catch
            {
            }
		}

		protected void Application_End(Object sender, EventArgs e)
		{
			clsReports.GetInstance().Dispose();
         
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

