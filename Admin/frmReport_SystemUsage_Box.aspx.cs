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
using System.Configuration;
using VLF.Reports ;
using System.IO; 

namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmReport_SystemUsage_Box.
	/// </summary>
	public partial class frmReport_SystemUsage_Box : System.Web.UI.Page
	{
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strFromDate=Request.QueryString["FromDT"]; 
			string strToDate=Request.QueryString["ToDT"];
			string strOrgId=Request.QueryString["OrgId"];
			string strOrgName=Request.QueryString["OrgName"];
			string strBoxId=Request.QueryString["BoxId"];

			
			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}



			this.lblFromDate.Text=strFromDate;
			this.lblToDate.Text=strToDate;
			this.lblBoxId.Text=strBoxId;  

			string xmlParams="";
			string xml="";

			xmlParams =	ReportTemplate.MakePair(ReportTemplate.RpBoxSystemUsageFirstParamName   ,strBoxId) +
				ReportTemplate.MakePair(ReportTemplate.RpBoxSystemUsageSecondParamName  ,strFromDate) + 
				ReportTemplate.MakePair(ReportTemplate.RpBoxSystemUsageThirdParamName ,strToDate)+
				ReportTemplate.MakePair(ReportTemplate.RpBoxSystemUsageFourthParamName ,"false");


			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerReport.Reports rpt = new ServerReport.Reports();

			bool RequestOverflowed=false;
			bool OutMaxOverflowed=false;
			int TotalSqlRecords=0;
			int OutMaxRecords=0;


			if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId ,ServerReport.ReportTypes.BoxSystemUsage   ,xmlParams, ref xml, ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords    ),false ) )
				if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId,ServerReport.ReportTypes.BoxSystemUsage,xmlParams, ref xml,ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords     ), true ) )
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
				this.dgDataNew.DataSource=ds;
                this.dgDataNew.DataBind();  
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
