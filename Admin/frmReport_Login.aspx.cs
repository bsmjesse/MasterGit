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
using System.IO; 
namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmReport_Logins.
	/// </summary>
	public partial class frmReport_Logins : System.Web.UI.Page
	{
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;
	

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strFromDate=Request.QueryString["FromDT"]; 
			string strToDate=Request.QueryString["ToDT"];
			string strOrgId=Request.QueryString["OrgId"];
			string strOrgName=Request.QueryString["OrgName"];


			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

		

			this.lblFromDate.Text=strFromDate;
			this.lblToDate.Text=strToDate;
			this.lblOranizationName.Text=strOrgName;  

			string xml="";
			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerDBOrganization.DBOrganization   dbo = new ServerDBOrganization.DBOrganization() ;

		
 
			if( objUtil.ErrCheck( dbo.GetOrganizationUserLogins( sn.UserID , sn.SecId ,Convert.ToInt32(strOrgId) ,strFromDate,strToDate, ref xml ),false ) )
				if( objUtil.ErrCheck( dbo.GetOrganizationUserLogins( sn.UserID , sn.SecId ,Convert.ToInt32(strOrgId) ,strFromDate,strToDate, ref xml ),true ) )
				{
					return;
				}



			
			if (xml=="")
			{
				return;
			}
		

			strrXML = new StringReader( xml ) ;
			DataSet ds=new DataSet();
			ds.ReadXml(strrXML);


			
			
			if (ds.Tables.Count>0)
			{

				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					dr["LoginDateTime"]=Convert.ToDateTime(dr["LoginDateTime"].ToString());
				}

				this.dgData.DataSource=ds.Tables[0] ;  
				this.dgData.DataBind();  
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
