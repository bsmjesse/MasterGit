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
using VLF.CLS; 

namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmReport_MapUsage.
	/// </summary>
	public partial class frmReport_MapUsage : System.Web.UI.Page
	{
	
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strMonth=Request.QueryString["Month"]; 
			string strYear=Request.QueryString["Year"]; 
			string strOrgId=Request.QueryString["OrgId"];
			string strOrgName=Request.QueryString["OrgName"];

			this.lblMonth.Text=strMonth;  
			this.lblOranizationName.Text=strOrgName; 

			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}


			



			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerDBSystem.DBSystem   dbs = new ServerDBSystem.DBSystem()  ;
	
			string xml="";
			if( objUtil.ErrCheck( dbs.GetOrganizationMapUsageInfo( sn.UserID , sn.SecId ,Convert.ToInt32(strOrgId) ,Convert.ToInt32(VLF.MAP.MapType.MapPointWeb),Convert.ToInt16(strYear),Convert.ToInt16(strMonth), ref xml ),false ) )
				if( objUtil.ErrCheck( dbs.GetOrganizationMapUsageInfo( sn.UserID , sn.SecId ,Convert.ToInt32(strOrgId) ,Convert.ToInt32(VLF.MAP.MapType.MapPointWeb),Convert.ToInt16(strYear),Convert.ToInt16(strMonth), ref xml ),false ) )
				{
					return;
				}
			
			if (xml=="")
			{
				return;
			}


			strrXML = new StringReader( xml ) ;
			DataSet dsMapPoint=new DataSet();
			dsMapPoint.ReadXml(strrXML);

		
			if (dsMapPoint.Tables.Count>0)
			{
				Int64 TotalMapPoint=0;
				foreach(DataRow rowItem in dsMapPoint.Tables[0].Rows)
				{
					TotalMapPoint+=Convert.ToInt64(rowItem["Totals"]);
				}

				this.lblTotalMap.Text=TotalMapPoint.ToString();
				this.dgMapPoint.DataSource=dsMapPoint;  
				this.dgMapPoint.DataBind();  
			}



			 xml="";
			if( objUtil.ErrCheck( dbs.GetOrganizationMapUsageInfo( sn.UserID , sn.SecId ,Convert.ToInt32(strOrgId) ,Convert.ToInt32(VLF.MAP.MapType.GeoMicroWeb  ),Convert.ToInt16(strYear),Convert.ToInt16(strMonth), ref xml ),false ) )
            if (objUtil.ErrCheck(dbs.GetOrganizationMapUsageInfo(sn.UserID, sn.SecId, Convert.ToInt32(strOrgId), Convert.ToInt32(VLF.MAP.MapType.GeoMicroWeb), Convert.ToInt16(strYear), Convert.ToInt16(strMonth), ref xml), true))
				{
					return;
				}



			
			if (xml=="")
			{
				return;
			}


			strrXML = new StringReader( xml ) ;
			DataSet dsGeo=new DataSet();
			dsGeo.ReadXml(strrXML);

		
			if (dsGeo.Tables.Count>0)
			{
				Int64 TotalGeo=0;
				foreach(DataRow rowItem in dsGeo.Tables[0].Rows)
				{
					TotalGeo+=Convert.ToInt64(rowItem["Totals"]);
				}

				this.lblTotalGeo.Text=TotalGeo.ToString();    
				this.dgGeo.DataSource=dsGeo;  
				this.dgGeo.DataBind();  
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
