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
using VLF.CLS.Def;
using System.Configuration; 

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmFeatures.
	/// </summary>
	public partial class frmFeatures : System.Web.UI.Page
	{
		
		protected SentinelFMSession sn = null;
		protected clsUtility objUtil;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

			

			if (!Page.IsPostBack)
			{
				ViewState["ConfirmDelete"] = "0";

				dgSystem_Fill();
				
			}
		}


		private void dgSystem_Fill()
		{
			string xml = "" ;
			objUtil=new clsUtility(sn) ;
			ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();    

			if( objUtil.ErrCheck( dbs.GetSystemUpdates( sn.UserID , sn.SecId ,"","01/01/2222",Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature) , ref xml ),false ) )
				if( objUtil.ErrCheck( dbs.GetSystemUpdates( sn.UserID , sn.SecId ,"","01/01/2222",Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), ref xml ),false ) )
				{
					  
					dgSystem.DataSource = null;
					dgSystem.DataBind(); 
					return;
				}


			if (xml == "")
			{
				dgSystem.DataSource = null;
				dgSystem.DataBind(); 
				return;
			}

			StringReader strrXML = new StringReader( xml ) ;
			DataSet ds=new DataSet();
			ds.ReadXml (strrXML) ;


			

			dgSystem.DataSource = ds;
			dgSystem.DataBind(); 

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
			this.dgSystem.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgSystem_PageIndexChanged);
			this.dgSystem.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSystem_DeleteCommand);

		}
		#endregion

		private void dgSystem_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex != dgSystem.SelectedIndex)
				{
					ViewState["ConfirmDelete"] = "1";
					dgSystem.EditItemIndex = -1;
					dgSystem.SelectedIndex = e.Item.ItemIndex;
					dgSystem_Fill();
					lblMessage.Visible = true;
					lblMessage.Text = "Please Click Delete Again to Confirm.";
					return;
				}
				else
				{

					if (ViewState["ConfirmDelete"].ToString()  == "0")
					{
						lblMessage.Visible = true;
						lblMessage.Text = "Please click 'Delete' again to confirm removal of Features...";
						ViewState["ConfirmDelete"] = "1";
						return;
					}
				}
            
				lblMessage.Visible = true;
		

				
				objUtil=new clsUtility(sn) ;


				ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();    

				if( objUtil.ErrCheck( dbs.DeleteSystemUpdate ( sn.UserID , sn.SecId ,Convert.ToInt32(dgSystem.DataKeys[e.Item.ItemIndex]))  ,false  ))
					if( objUtil.ErrCheck( dbs.DeleteSystemUpdate ( sn.UserID , sn.SecId ,Convert.ToInt32(dgSystem.DataKeys[e.Item.ItemIndex]))  ,false  ))
					{
						return;
					}
			

				dgSystem.SelectedIndex = -1;
				lblMessage.Text = "Features Deleted!";
				dgSystem.CurrentPageIndex=0; 
				dgSystem_Fill();
				ViewState["ConfirmDelete"] = "0";
				//	}
			}
			catch(Exception Ex)
			{
				
			}
		}

		protected void cmdAddMsg_Click(object sender, System.EventArgs e)
		{
            //objUtil=new clsUtility(sn) ;
            //ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();    

            //if( objUtil.ErrCheck( dbs.AddSystemUpdate  ( sn.UserID , sn.SecId ,this.txtNewMsg.Text,Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature ),0)  ,false  ))
            //    if( objUtil.ErrCheck( dbs.AddSystemUpdate  ( sn.UserID , sn.SecId ,this.txtNewMsg.Text,Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature),0)  ,true  ))
            //    {
            //        return;
            //    }

            //this.lblMessage.Text="";  
            //dgSystem.SelectedIndex = -1;
            //dgSystem.CurrentPageIndex=0; 
            //dgSystem_Fill();
            //ViewState["ConfirmDelete"] = "0";
		}

		private void dgSystem_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgSystem.CurrentPageIndex = e.NewPageIndex;
			dgSystem_Fill();
			dgSystem.SelectedIndex = -1	;
		}
	}
}
