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
using System.IO;

namespace SentinelFM.Messages
{
	/// <summary>
	/// Summary description for frmMessageInfo.
	/// </summary>
	public partial class frmMessageInfo : SentinelFMBasePage
	{

		
		
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				//Clear IIS cache
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.Cache.SetExpires(DateTime.Now);

				sn = (SentinelFMSession) Session["SentinelFMSession"] ;
				
				if (!Page.IsPostBack)
				{
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMessageInfoForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
					string MsgKey=Request.QueryString["MsgKey"].ToString()  ;
					string[] indexArray = MsgKey.Split(';');
					
					int MessageId=Convert.ToInt32(indexArray[0]);
					int VehicleId=Convert.ToInt32(indexArray[1]);
					MessageInfoLoad(MessageId,VehicleId);
					
				}
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmMessageInfo.aspx"));    
			}

		}

		private void MessageInfoLoad(int MessageId,int VehicleId)
		{
			try
			{
				
				DataSet ds=new DataSet() ;
				StringReader strrXML = null;
				string xml = "" ;
				ServerDBHistory.DBHistory  hist = new ServerDBHistory.DBHistory() ;

				if( objUtil.ErrCheck( hist.GetTextMessageFullInfo ( sn.UserID , sn.SecId ,MessageId,VehicleId, ref xml ),false ) )
					if( objUtil.ErrCheck( hist.GetTextMessageFullInfo ( sn.UserID , sn.SecId ,MessageId,VehicleId, ref xml ),true ) )
					{
						return;
					}
				strrXML = new StringReader( xml ) ;


				if (xml == "")
				{
					return;
				}

				ds.ReadXml (strrXML) ;

				if (ds.Tables[0].Columns.IndexOf("To")==-1 )
				{
					DataColumn colTo= new DataColumn("To",Type.GetType("System.String"));
					colTo.DefaultValue=""; 
					ds.Tables[0].Columns.Add(colTo);
				}

				this.lblLicensePlate.Text=ds.Tables[0].Rows[0]["LicensePlate"].ToString().TrimEnd();       
				this.lblTimeCreated.Text =Convert.ToDateTime(ds.Tables[0].Rows[0]["MsgDateTime"].ToString().TrimEnd()).ToString() ; 
				this.lblFrom.Text =ds.Tables[0].Rows[0]["From"].ToString().TrimEnd();  
				this.lblTo.Text =ds.Tables[0].Rows[0]["To"].ToString().TrimEnd();
				this.txtResponse.Text=ds.Tables[0].Rows[0]["MsgResponse"].ToString().TrimEnd();      
				this.txtMessage.Text=ds.Tables[0].Rows[0]["MsgBody"].ToString().TrimEnd();  

				if (ds.Tables[0].Rows[0]["ResponseDateTime"].ToString().TrimEnd()!="N/A")
					this.lblResponseDate.Text=Convert.ToDateTime(ds.Tables[0].Rows[0]["ResponseDateTime"].ToString().TrimEnd()).ToString()  ;     

				this.lblAcknowledged.Text=ds.Tables[0].Rows[0]["Acknowledged"].ToString().TrimEnd() ;       
				this.lblRead.Text=ds.Tables[0].Rows[0]["UserName"].ToString().TrimEnd() ;         
				this.lblStreetAddress.Text=ds.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();      
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmMessageInfo.aspx"));    
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
