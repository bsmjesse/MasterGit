using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity ;

namespace VLF.ASI
{
	/// <summary>
	/// Summary description for SecurityManager.
	/// </summary>
	public class SecurityManager : System.Web.Services.WebService
	{
		public SecurityManager()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]
		public int Login( string userName, string password, ref int userId, ref string SID )
		{
			try
			{
				userId = LoginManager.GetInstance().LoginUser( userName, password, ref SID ) ;
				return (int)InterfaceError.NoError ;
			}
			catch(Exception Ex )
			{
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
		}

		[WebMethod]
		public int Logout( int userId, string SID )
		{
			try
			{
				LoginManager.GetInstance().SecurityCheck( userId, SID ) ;
				LoginManager.GetInstance().LogoutUser( userId ) ;
				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
		}

		[WebMethod]
		public int Relogin( string userName, string password, ref string SID )
		{
			try
			{
				LoginManager.GetInstance().ReloginUser( userName, password, ref SID ) ;
				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
		}


	}
}
