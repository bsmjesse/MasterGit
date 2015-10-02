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
	/// Summary description for frmReport_ExceptionUsage.
	/// </summary>
	public partial class frmReport_ExceptionUsage : System.Web.UI.Page
	{
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strMonth=Request.QueryString["Month"]; 
			string strYear=Request.QueryString["Year"]; 
			

			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}


			this.lblMonth.Text=strMonth;
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

			xmlParams =	ReportTemplate.MakePair(ReportTemplate.RpSystemUsageExceptionReportForAllOrganizationsFirstParamName  ,strFromDate) +
				ReportTemplate.MakePair(ReportTemplate.RpSystemUsageExceptionReportForAllOrganizationsSecondParamName,strToDate);
				
				


			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerReport.Reports rpt = new ServerReport.Reports();

			bool RequestOverflowed=false;
			bool OutMaxOverflowed=false;
			int TotalSqlRecords=0;
			int OutMaxRecords=0;


			if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId ,ServerReport.ReportTypes.SystemUsageExceptionReportForAllOrganizations     ,xmlParams, ref xml, ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords    ),false ) )
				if( objUtil.ErrCheck( rpt.GetXml( sn.UserID , sn.SecId,ServerReport.ReportTypes.SystemUsageExceptionReportForAllOrganizations ,xmlParams, ref xml,ref RequestOverflowed,ref TotalSqlRecords,ref OutMaxOverflowed,ref OutMaxRecords     ), true ) )
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

		}
		#endregion
	}
}
