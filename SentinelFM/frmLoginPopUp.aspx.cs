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
	/// Summary description for frmLoginPopUp.
	/// </summary>
	public partial class LoginPopUp : SentinelFMBasePage
	{

		protected ServerDBUser.DBUser   dbu;
		
		
	

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Clear IIS cache
				//Response.Cache.SetCacheability(HttpCacheability.NoCache);
				//Response.Cache.SetExpires(DateTime.Now);



				// Check User License and cookies
				 ViewState["Logo"]=Request.QueryString["Logo"]  ;
				 ViewState["URL"]=Request.QueryString["URL"]  ;

		

				if (ViewState["Logo"]!=null)
				{
					Session["CompanyLogo"]=ViewState["Logo"];
					Session["CompanyURL"]=ViewState["URL"];
					Response.Cookies.Clear();   
					HttpCookie Sentinel = new HttpCookie("SentinelCompany");
					Sentinel.Name="SentinelCompany";
					Sentinel.Value=ViewState["Logo"]+"|"+ViewState["URL"]; 
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
						Session["CompanyLogo"]=ConfigurationSettings.AppSettings["DefaultLogo"]  ;
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
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" Form:LoginPopUp.aspx"));    
			}
			
		}

		
		private void DoLogin()
		{
			int uid=-1 ;
			string secId = "";

			string IpAddr= Request.ServerVariables["remote_addr"];
			string HashPassword=Request.Form["txtHash"].ToString().TrimEnd();   
			try
			{

				SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager() ;
				int errCode = sec.LoginMD5( ViewState["auth_seed"].ToString() ,txtUserName.Text, HashPassword,IpAddr, ref uid, ref secId ) ;
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
                    sn.LoginUserID = uid;
					sn.SecId= secId;
					sn.UserName = txtUserName.Text ;
					sn.Password = txtPassword.Text ;
					sn.User.IPAddr=IpAddr;
					Session.Add("SentinelFMSession", sn ) ;
					//FindExistingPreference();
					sn.User.ExistingPreference(sn);
                    sn.User.GetGuiControlsInfo(sn);
					string strUrl="<script language='javascript'> function NewWindow(mypage) { ";
					strUrl=strUrl+"	var myname='';"; 
					strUrl=strUrl+" var w=screen.availWidth;";
					strUrl=strUrl+" var h=screen.availHeight;" ;
					strUrl=strUrl+" var winl = (screen.availWidth - w) / 2; ";
					strUrl=strUrl+" var wint = (screen.availHeight - h) / 2; ";
					strUrl=strUrl+" winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+',toolbar=yes,resizable=yes,location=yes,directories=yes,addressbar=yes,scrollbars=yes,status=yes,menubar=yes,' ; ";
					strUrl=strUrl+" win = window.open(mypage, myname, winprops); }" ;
					strUrl=strUrl+" NewWindow('frmMain.htm');";
					strUrl=strUrl+" window.close();</script>";
					Response.Write(strUrl) ;
				}
			}

			catch(System.Threading.ThreadAbortException)
			{
				return;
			}
			catch(Exception Ex)
			{
				
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:Login.aspx"));    
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
			this.ID = "LoginPopUp";

		}
		#endregion
	}
}
