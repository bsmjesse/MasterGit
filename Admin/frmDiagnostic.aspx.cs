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

namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmDiagnostic.
	/// </summary>
	public partial class frmDiagnostic : System.Web.UI.Page
	{
		protected SentinelFMSession sn = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
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

		protected void cmdView_Click(object sender, System.EventArgs e)
		{
			
			if ((!this.chkNotValidGPS.Checked) && (!this.chkWithoutIP.Checked) && (!this.chkReportedFrequency.Checked))
			{
				this.lblMessage.Text="Please select a report";
				return;
			}
			else
			{
				this.lblMessage.Text="";  
			}
    
			string strUrl="<script language='javascript'> function NewWindow(mypage) { ";
			strUrl=strUrl+"	var myname='Report';"; 
			strUrl=strUrl+" var w=800;";
			strUrl=strUrl+" var h=480;" ;
			strUrl=strUrl+" var winl = (screen.width - w) / 2; ";
			strUrl=strUrl+" var wint = (screen.height - h) / 2; ";
			strUrl=strUrl+" winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,scrollbars=1,menubar=1,'; ";
			strUrl=strUrl+" win = window.open(mypage, myname, winprops); }" ;
			string WindowUrl="frmDiagNotValidGPS.aspx?InvalidGPSPercent="+this.cboPercent.SelectedItem.Value.ToString()+"&ShowNotValidGPS="+this.chkNotValidGPS.Checked+"&ShowBoxIpUpdates="+this.chkWithoutIP.Checked+"&ReportHours="+this.cboHours.SelectedItem.Value+"&ShowReportedFrq="+this.chkReportedFrequency.Checked+"&TotMsgs="+this.txtReportFrq.Text;
				
			

			strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			Response.Write(strUrl) ;
		}

		
	}
}
