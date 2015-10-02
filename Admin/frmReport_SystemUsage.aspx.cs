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
using VLF.CLS.Def ;
using VLF.CLS ;
using System.Web.Security;
using System.Configuration;
using VLF.Reports ;
using System.IO; 

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmReport_SystemUsage.
	/// </summary>
	public partial class frmReport_SystemUsage : System.Web.UI.Page
	{
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;
	
		int TotalBytes=0;
		int TotalMsgs=0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strFromDate=Request.QueryString["FromDT"]; 
			string strToDate=Request.QueryString["ToDT"];
			string strOrgId=Request.QueryString["OrgId"];
			string strOrgName=Request.QueryString["OrgName"];

			ViewState["TotalBytes"]=0;
			ViewState["TotalMsgs"]=0;

			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}


			this.lblFromDate.Text=strFromDate;
			this.lblToDate.Text=strToDate;
			this.lblOranizationName.Text=strOrgName;  


			string xmlParams="";
			string xml="";

			xmlParams =	ReportTemplate.MakePair(ReportTemplate.RpOrganizationSystemUsageFirstParamName  ,strOrgId) +
				ReportTemplate.MakePair(ReportTemplate.RpOrganizationSystemUsageSecondParamName ,strFromDate) + 
				ReportTemplate.MakePair(ReportTemplate.RpOrganizationSystemUsageThirdParamName,strToDate)+
				ReportTemplate.MakePair(ReportTemplate.RpOrganizationSystemUsageFourthParamName,"false");


			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerReport.Reports rpt = new ServerReport.Reports();

			bool RequestOverflowed=false;
			bool OutMaxOverflowed=false;
			int TotalSqlRecords=0;
			int OutMaxRecords=0;

			if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId ,ServerReport.ReportTypes.OrganizationSystemUsage  ,xmlParams, ref xml, ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords),false ) )
				if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId,ServerReport.ReportTypes.OrganizationSystemUsage,xmlParams, ref xml,ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords  ), true ) )
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
				this.dgData.DataSource=ds;  
				this.dgData.DataBind();  
			}

			this.lblTotBytes.Text=ViewState["TotalBytes"].ToString();
			this.lblTotMsgs.Text=ViewState["TotalMsgs"].ToString();
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
			this.dgData.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgData_ItemDataBound);

		}
		#endregion

		private void dgData_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			

				if ((e.Item.ItemType == ListItemType.AlternatingItem ) || (e.Item.ItemType == ListItemType.Item))
				{
					ViewState["TotalBytes"]=Convert.ToInt32(ViewState["TotalBytes"])+Convert.ToInt32(e.Item.Cells[11].Text);
					ViewState["TotalMsgs"]=Convert.ToInt32(ViewState["TotalMsgs"])+Convert.ToInt32(e.Item.Cells[12].Text);
				}

		
  
		}
	}
}
