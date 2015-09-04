using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.IO ;
using System.Configuration; 
using System.Security.Cryptography;
using VLF.CLS;
namespace SentinelFM
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public partial class frmTimeOut : System.Web.UI.Page
	{
		protected ServerDBUser.DBUser   dbu;
		protected SentinelFMSession sn = null;
		protected clsUtility objUtil;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Clear IIS cache
				//				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				//				Response.Cache.SetExpires(DateTime.Now);

				

				// Check User License and cookies
				string Logo=Request.Form["Logo"]  ;
				string URL=Request.Form["URL"]  ;
				

				if (Logo!=null)
				{
					Session["CompanyLogo"]=Logo;
					Session["CompanyURL"]=URL;
					Response.Cookies.Clear();   
					HttpCookie Sentinel = new HttpCookie("SentinelCompany");
					Sentinel.Name="SentinelCompany";
					Sentinel.Value=Logo+"|"+URL; 
					Response.Cookies.Add(Sentinel);

				}
				else
				{
					HttpCookieCollection SentinelCookieColl;
					HttpCookie SentinelCookie;
					SentinelCookieColl=Request.Cookies;

					if (SentinelCookieColl.Count>0)
					{
						string[] arrCookies = SentinelCookieColl.AllKeys;
						for (int i = 0; i < arrCookies.Length; i++) 
						{
							SentinelCookie = SentinelCookieColl[arrCookies[i]];
							if (SentinelCookie.Name=="SentinelCompany") 
							{
								
								string[] values = SentinelCookie.Value.ToString().Split('|');

								Session["CompanyLogo"]=values[0];
								Session["CompanyURL"]=values[1];
								break;

							}
						}
					}
					else
					{
						Session["CompanyLogo"]=ConfigurationSettings.AppSettings["DefaultLogo"];
					}
				}


				if (ViewState["auth_seed"]==null)
				{
					System.Random rnd=new System.Random();
					ViewState["auth_seed"]=rnd.NextDouble().ToString();
					return;
				}

				if (this.txtPassword.Text!="")  
					DoLogin();
			}

			catch(System.Threading.ThreadAbortException)
			{
				return;
			}
			
			
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

	
		private void DoLogin()
		{
			int uid=-1 ;
			string secId = "";

			string IpAddr= Request.ServerVariables["remote_addr"];
			string HashPassword=Request.Form["txtHash"].ToString().TrimEnd();   
			try
			{

				SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager() ;
				int errCode = sec.LoginMD5 ( ViewState["auth_seed"].ToString() ,txtUserName.Text, HashPassword,IpAddr, ref uid, ref secId ) ;
				if( errCode != (int)VLF.ERRSecurity.InterfaceError.NoError )
				{
					this.lblMessage.Text="Authentication failed"  ;
					this.lblMessage.Visible=true;  
				}
				else
				{
					this.lblMessage.Visible=false;

					SentinelFMSession    sn = new SentinelFMSession() ;
					sn.UserID = uid;
					sn.SecId= secId;
					sn.UserName = txtUserName.Text ;
					sn.Password = txtPassword.Text ;
					sn.User.IPAddr=IpAddr; 
					Session.Add("SentinelFMSession", sn ) ;
					//FindExistingPreference();
					sn.User.ExistingPreference(sn);
					FormsAuthentication.RedirectFromLoginPage(uid.ToString(),false) ;
                    Response.Redirect("frmAdminMenu.aspx");
				}
			}

			catch(System.Threading.ThreadAbortException)
			{
				return;
			}
			
			
		}


		




	}
}

