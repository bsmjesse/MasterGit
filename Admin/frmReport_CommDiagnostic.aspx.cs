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
	/// Summary description for frmReport_CommDiagnostic.
	/// </summary>
	public partial class frmReport_CommDiagnostic : System.Web.UI.Page
	{

		protected SentinelFMSession sn = null;
protected clsUtility objUtil;
		protected void Page_Load(object sender, System.EventArgs e)
		{

			string strFromDate=Request.QueryString["FromDT"]; 
			string strToDate=Request.QueryString["ToDT"];
			string strOrgId=Request.QueryString["OrgId"];
			string strFleetId=Request.QueryString["FleetId"];
			string strVehicle=Request.QueryString["VehicleId"];
			string strOrgName=Request.QueryString["OrgName"];
			string CommModes=Request.QueryString["CommModes"];
	
				string[] indexArray = CommModes.Split(';');
				string strXML="<ROOT>";
				for (int i =0;i<indexArray.Length;i++) 
				{
						strXML+="<RowItem><CommModeId>"+indexArray[i].ToString()+"</CommModeId></RowItem>" ;
				}		
				strXML+="</ROOT>";


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

			xmlParams =	ReportTemplate.MakePair(ReportTemplate.RpLatencyFirstParamName   ,strOrgId) +
				ReportTemplate.MakePair(ReportTemplate.RpLatencySecondParamName   ,strFleetId) + 
				ReportTemplate.MakePair(ReportTemplate.RpLatencyThirdParamName    ,strVehicle)+
				ReportTemplate.MakePair(ReportTemplate.RpLatencyFourthParamName     ,strFromDate)+
				ReportTemplate.MakePair(ReportTemplate.RpLatencyFifthParamName      ,strToDate)+
				ReportTemplate.MakePair(ReportTemplate.RpLatencySixthParamName   ,strXML);


			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerReport.Reports rpt = new ServerReport.Reports();

			


			if( objUtil.ErrCheck( rpt.GetSystemReport( sn.UserID , sn.SecId ,ServerReport.ReportTypes.Latency   ,xmlParams, ref xml     ),false ) )
				if( objUtil.ErrCheck( rpt.GetSystemReport( sn.UserID , sn.SecId,ServerReport.ReportTypes.Latency,xmlParams, ref xml), true ) )
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
				this.dgCommDiag.DataSource=ds;
				this.dgCommDiag.DataBind();  
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
