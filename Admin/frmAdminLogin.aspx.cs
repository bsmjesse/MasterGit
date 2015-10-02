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
namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmAdminLogin.
	/// </summary>
	public partial class frmAdminLogin : System.Web.UI.Page
	{
		protected ServerDBUser.DBUser   dbu;
		protected SentinelFMSession sn = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//Clear IIS cache
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetExpires(DateTime.Now);


			if (Session["auth_seed"]==null)
			{
				System.Random rnd=new System.Random();
				Session["auth_seed"]=rnd.NextDouble().ToString();
				return;
			}

			if (this.txtPassword.Text!="")  
				DoLogin();
			
		}



		private void DoLogin()
		{
			int uid=-1 ;
			string secId = "";

			string IpAddr= Request.ServerVariables["remote_addr"];
			string HashPassword=Request.Form["txtHash"].ToString().TrimEnd();   


			
           
            try
            {
                if (HttpContext.Current.Request.UserHostAddress.Trim() != "")
                    IpAddr = HttpContext.Current.Request.UserHostAddress.Trim();
                else if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].Trim() != "")
                    IpAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].Trim();
                else if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim() != "")
                    IpAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim();

            }
            catch
            {
            }


            #region start check IP for hgi user
            try
            {
                if (txtUserName.Text.Contains("hgi") == true && IpAddr.Contains("127.0.0.1") == false && IpAddr.Contains("::1") == false)
                {
                    string[] ip;
                    if (IpAddr.Contains("184.94.19") == true)
                    {
                        //ip = IpAddr.Split('.');
                        //if (Convert.ToInt16(ip[3]) < 97 || Convert.ToInt16(ip[3]) > 110)
                        // {
                        //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_AuthFailed");
                        //        this.lblMessage.Visible = true;
                        //        return;
                        //}
                    }
                    else if (IpAddr.Contains("192.168.199") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text = "Error";
                            this.lblMessage.Visible = true;
                            
                            return;
                        }
                    }
                    else if (IpAddr.Contains("67.70.185") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 121 || Convert.ToInt16(ip[3]) > 126)
                        {
                            this.lblMessage.Text = "Error";
                            this.lblMessage.Visible = true;
                            
                            return;
                        }
                    }
                    else if (IpAddr.Contains("172.16.3") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text ="Error";
                            this.lblMessage.Visible = true;
                           
                            return;
                        } 
                    }


 else if (IpAddr.Contains("173.165.67") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text ="Error";
                            this.lblMessage.Visible = true;
                           
                            return;
                        }      
                    }






			else if (IpAddr.Contains("184.94.19") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text ="Error";
                            this.lblMessage.Visible = true;
                           
                            return;
                        }
                    }

else if (IpAddr.Contains("142.46.86") == true)
                    {
                        ip = IpAddr.Split('.');
                        if (Convert.ToInt16(ip[3]) < 1 || Convert.ToInt16(ip[3]) > 254)
                        {
                            this.lblMessage.Text ="Error";
                            this.lblMessage.Visible = true;
                           
                            return;
                        }
                    }




                    else if (IpAddr.Contains("184.94.19") == false || IpAddr.Contains("192.168.199") == false || IpAddr.Contains("67.70.185") == false || IpAddr.Contains("172.16.3") == false || IpAddr.Contains("142.46.86") == false)
                    {
                        this.lblMessage.Text = "Error";
                        this.lblMessage.Visible = true;
                        
                        return;

                    }
                }



		

                

else if ((txtUserName.Text.ToLower().Trim() == "ncc1" || txtUserName.Text.ToLower().Trim() == "ncc2") && IpAddr.Contains("209.171.44.131") == false && IpAddr.Contains("206.162.182.113") == false)
                {
                    this.lblMessage.Text = "Error";
                    this.lblMessage.Visible = true;
                    
                    return;
                }
            }
            catch
            {
            }

            #endregion check IP


			try
			{

				SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager() ;
				int errCode = sec.LoginMD5(Session["auth_seed"].ToString(),txtUserName.Text, HashPassword,IpAddr, ref uid, ref secId ) ;
				if( errCode != (int)VLF.ERRSecurity.InterfaceError.NoError )
				{
					this.lblMessage.Text=VLF.ERRSecurity.InterfaceErrorDescription.GetDescription( (VLF.ERRSecurity.InterfaceError) errCode )  ;
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
					sn.User.ExistingPreference(sn);
					FormsAuthentication.RedirectFromLoginPage(uid.ToString(),false) ;
					Response.Redirect("frmAdminMenu.aspx") ;
				}
			}

			catch(Exception Ex)
			{
				
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
	}
}
