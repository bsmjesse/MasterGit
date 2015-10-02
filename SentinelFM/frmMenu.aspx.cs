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
using System.IO;
using System.Configuration;  
namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmMenu.
	/// </summary>
	public partial class frmMenu : SentinelFMBasePage
	{
		
		public string ConfigurationView="";
		public string MessagesView="";
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Clear IIS cache
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.Cache.SetExpires(DateTime.Now);

			

				
				if ((sn==null) || (sn.UserName=="") || (sn.User.OrganizationId==0))
				{
					RedirectToLogin();
					return;
				}
			
					Session["CompanyLogo"]=sn.User.CompanyLogo  ;
					Session["CompanyURL"]=sn.CompanyURL ;

					this.imgProdLogo.ImageUrl="images/"+sn.User.CompanyLogo;

			
					if (!Page.IsPostBack)
					{

						//Configuration View security check
						if(!sn.User.ControlEnable(sn,16))
							ConfigurationView="none";
						else
							ConfigurationView="";


						//Messages View security check
						if(!sn.User.ControlEnable(sn,24))
							MessagesView="none";
						else
							MessagesView="";

						
					}
			}
			catch(NullReferenceException)
			{
				RedirectToLogin();
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

		protected void cmdLogout_Click(object sender, System.EventArgs e)
		{

            string sDomainName = @ConfigurationSettings.AppSettings["DomainName"];
            string sPath = "https://" + sDomainName + "/Login.aspx";

			FormsAuthentication.SignOut(); 
			Session["SentinelFMSession"]=null;
			Session.Abandon();
			//Response.Write("<SCRIPT Language='javascript'>window.open('frmLoginRedirect.aspx','_top') </SCRIPT>");
			Response.Write("<SCRIPT Language='javascript'>window.open('"+sPath+"','_top') </SCRIPT>");
			
			
		}

		private void RedirectToLogin()
		{
			//Response.Write("<SCRIPT Language='javascript'>window.open('frmLoginRedirect.aspx','_top') </SCRIPT>");
			Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
		}

		
	} }