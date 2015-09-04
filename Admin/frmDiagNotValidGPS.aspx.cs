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
using System.IO ;
namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmDiagNotValidGPS.
	/// </summary>
	public partial class frmDiagNotValidGPS : System.Web.UI.Page
	{
		protected SentinelFMSession sn = null;
		protected clsUtility objUtil;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string InvalidGPSPercent=Request.QueryString["InvalidGPSPercent"] ; 
			string ShowNotValidGPS=Request.QueryString["ShowNotValidGPS"].ToString().ToLower() ;
			string ShowBoxIpUpdates=Request.QueryString["ShowBoxIpUpdates"].ToString().ToLower();
			string ReportHours=Request.QueryString["ReportHours"].ToString().ToLower();
			string ShowReportedFrq=Request.QueryString["ShowReportedFrq"].ToString().ToLower();
			string TotMsgs=Request.QueryString["TotMsgs"].ToString().ToLower();
			this.lblHours.Text="(Within "+ ReportHours.ToString()  + " hours)";
			

			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

		

			this.lblNotValidGPS.Visible=false;
			this.lblBoxIpUpdates.Visible=false;
			this.lblReported.Visible=false;
 
			string xml="";
			StringReader strrXML = null;
			objUtil=new clsUtility(sn) ;
			ServerDBHistory.DBHistory    dbh = new ServerDBHistory.DBHistory() ;
			this.lblDate.Text=DateTime.Now.ToString();    

			if (ShowNotValidGPS=="true")
			{
				if( objUtil.ErrCheck( dbh.GetInvalidGPSStatistic( sn.UserID , sn.SecId ,Convert.ToInt32(InvalidGPSPercent),Convert.ToInt32(ReportHours) , ref xml ),false ) )
					if( objUtil.ErrCheck( dbh.GetInvalidGPSStatistic( sn.UserID , sn.SecId ,Convert.ToInt32(InvalidGPSPercent),Convert.ToInt32(ReportHours), ref xml ),true ) )
					{
						//return;
					}
				
				if (xml!="")
				{
					strrXML = new StringReader( xml ) ;
					DataSet ds=new DataSet();
					ds.ReadXml(strrXML);
					this.lblNotValidGPS.Visible=true;  

					if (ds.Tables.Count>0)
					{
						this.dgNotValidGPS.DataSource=ds.Tables[0] ;  
						this.dgNotValidGPS.DataBind();  
					}
					else
					{
						this.dgNotValidGPS.DataSource=null ;  
						this.dgNotValidGPS.DataBind();  
					}
				}
			}

			xml="";

			if (ShowBoxIpUpdates=="true")
			{
				if( objUtil.ErrCheck( dbh.GetBoxesWithoutIpUpdates ( sn.UserID , sn.SecId ,Convert.ToInt32(ReportHours), ref xml ),false ) )
					if( objUtil.ErrCheck( dbh.GetBoxesWithoutIpUpdates( sn.UserID , sn.SecId ,Convert.ToInt32(ReportHours), ref xml ),true ) )
					{
						//return;
					}
				
				if (xml!="")
				{
					strrXML = new StringReader( xml ) ;
					DataSet ds=new DataSet();
					ds.ReadXml(strrXML);
					this.lblBoxIpUpdates.Visible=true;
  
					if (ds.Tables.Count>0)
					{
						this.dgNoIPUpdates.DataSource=ds.Tables[0] ;  
						this.dgNoIPUpdates.DataBind();  
					}
					else
					{
						this.dgNoIPUpdates.DataSource=null ;  
						this.dgNoIPUpdates.DataBind();  
					}
				}
			}


			if (ShowReportedFrq=="true")
			{
				if( objUtil.ErrCheck( dbh.GetBoxesReportedFrequency  ( sn.UserID , sn.SecId ,Convert.ToInt32(ReportHours),Convert.ToInt32(TotMsgs),Convert.ToInt16(VLF.CLS.Def.Const.unassignedShortValue),     ref xml ),false ) )
					if( objUtil.ErrCheck( dbh.GetBoxesReportedFrequency  ( sn.UserID , sn.SecId ,Convert.ToInt32(ReportHours),Convert.ToInt32(TotMsgs),Convert.ToInt16(VLF.CLS.Def.Const.unassignedShortValue),     ref xml ),true ) )
					{
						//return;
					}
				
				if (xml!="")
				{
					strrXML = new StringReader( xml ) ;
					DataSet ds=new DataSet();
					ds.ReadXml(strrXML);
					this.lblReported.Visible=true;
  
					if (ds.Tables.Count>0)
					{
						this.dgReportedFrq.DataSource=ds.Tables[0] ;  
						this.dgReportedFrq.DataBind();  
					}
					else
					{
						this.dgReportedFrq.DataSource=null ;  
						this.dgReportedFrq.DataBind();  
					}
				}
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
