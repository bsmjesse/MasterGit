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
	/// Summary description for frmServicesUpdates.
	/// </summary>
	public partial class frmServicesUpdates : System.Web.UI.Page
	{
		protected SentinelFMSession sn = null;
		protected clsUtility objUtil;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				string str="";
				str="top.document.all('TopFrame').cols='0,*';";
				Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
				return;
			}

			
			dgFeatures_Fill();
		}

		private void dgFeatures_Fill()
		{
			string xml = "" ;	
			objUtil=new clsUtility(sn) ;
			ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();    

			if( objUtil.ErrCheck( dbs.GetSystemUpdates( sn.UserID , sn.SecId ,"","01/01/2222",Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), ref xml ),false ) )
				if( objUtil.ErrCheck( dbs.GetSystemUpdates( sn.UserID , sn.SecId ,"","01/01/2222",Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), ref xml ),false ) )
				{
					 
					dgFeatures.DataSource = null;
					dgFeatures.DataBind(); 
					return;
				}
			StringReader strrXML = new StringReader( xml ) ;
			DataSet ds=new DataSet();
			ds.ReadXml (strrXML) ;
			dgFeatures.DataSource = ds;
			dgFeatures.DataBind(); 

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
