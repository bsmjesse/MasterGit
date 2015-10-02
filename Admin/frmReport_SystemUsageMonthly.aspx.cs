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

namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmReport_SystemUsageMonthly.
	/// </summary>
	public partial class frmReport_SystemUsageMonthly : System.Web.UI.Page
	{
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;
	
		int TotalBytes=0;
		int TotalMsgs=0;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strMonth=Request.QueryString["Month"]; 
			string strYear=Request.QueryString["Year"]; 
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


			this.lblMonth.Text=strMonth;
			this.lblOranizationName.Text=strOrgName;  
			string strFromDate="";
			string strToDate="";
	

			switch (strMonth)
			{
				case "January":
					strFromDate="01/01/"+strYear+" 12:00 AM" ;
					strToDate="02/01/"+strYear+" 12:00 AM";
					break;
				case "February":
					strFromDate="02/01/"+strYear+" 12:00 AM" ;
					strToDate="03/01/"+strYear+" 12:00 AM";
					break;
				case "March":
					strFromDate="03/01/"+strYear+" 12:00 AM" ;
					strToDate="04/01/"+strYear+" 12:00 AM";
					break;
				case "April":
					strFromDate="04/01/"+strYear+" 12:00 AM" ;
					strToDate="05/01/"+strYear+" 12:00 AM";
					break;
				case "May":
					strFromDate="05/01/"+strYear+" 12:00 AM" ;
					strToDate="06/01/"+strYear+" 12:00 AM";
					break;
				case "June":
					strFromDate="06/01/"+strYear+" 12:00 AM" ;
					strToDate="07/01/"+strYear+" 12:00 AM";
					break;
				case "July":
					strFromDate="07/01/"+strYear+" 12:00 AM" ;
					strToDate="08/01/"+strYear+" 12:00 AM";
					break;
				case "August":
					strFromDate="08/01/"+strYear+" 12:00 AM" ;
					strToDate="09/01/"+strYear+" 12:00 AM";
					break;
				case "September":
					strFromDate="09/01/"+strYear+" 12:00 AM" ;
					strToDate="10/01/"+strYear+" 12:00 AM";
					break;
				case "October":
					strFromDate="10/01/"+strYear+" 12:00 AM" ;
					strToDate="11/01/"+strYear+" 12:00 AM";
					break;
				case "November":
					strFromDate="11/01/"+strYear+" 12:00 AM" ;
					strToDate="12/01/"+strYear+" 12:00 AM";
					break;
				case "December":
					strFromDate="12/01/"+strYear+" 12:00 AM" ;
					strToDate="01/01/"+Convert.ToString((Convert.ToInt16(strYear)+1))+" 12:00 AM";
					break;
			}


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


			if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId ,ServerReport.ReportTypes.OrganizationSystemUsage  ,xmlParams, ref xml, ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords    ),false ) )
				if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId,ServerReport.ReportTypes.OrganizationSystemUsage,xmlParams, ref xml,ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords     ), true ) )
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
				
				if (Convert.ToDouble(e.Item.Cells[10].Text) >Convert.ToDouble(e.Item.Cells[12].Text) )
					e.Item.ForeColor=Color.Red;

				if (Convert.ToDouble(e.Item.Cells[11].Text) >Convert.ToDouble(e.Item.Cells[13].Text) )
					e.Item.ForeColor=Color.Red;

			}
		}
	}
}
