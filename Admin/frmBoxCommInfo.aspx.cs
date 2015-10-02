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
	/// Summary description for frmBoxCommInfo.
	/// </summary>
	public partial class frmBoxCommInfo : System.Web.UI.Page
	{

		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				sn = (SentinelFMSession) Session["SentinelFMSession"] ;

				int BoxId=Convert.ToInt32(Request.QueryString["BoxId"]) ;


				StringReader strrXML = null;
				objUtil=new clsUtility(sn) ;
				ServerDBVehicle.DBVehicle   dbv = new ServerDBVehicle.DBVehicle();
				string xml="";

				if( objUtil.ErrCheck( dbv.GetCommInfoByBoxId ( sn.UserID , sn.SecId ,BoxId,ref xml),false ) )
					if( objUtil.ErrCheck( dbv.GetCommInfoByBoxId ( sn.UserID , sn.SecId ,BoxId,ref xml),true ) )
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
				dgData.DataSource=ds;
				dgData.DataBind(); 

			}
			catch
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
