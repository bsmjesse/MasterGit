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
	/// Summary description for frmSystemUpdate.
	/// </summary>
	public partial class frmSystemUpdate : System.Web.UI.Page
	{
		protected SentinelFMSession sn = null;
		protected clsUtility objUtil;
		protected DataSet dsFontColor=new DataSet();


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
				DsFontColor_Fill();
				this.cboFontColorAdd.DataSource= dsFontColor.Tables[0] ;
				this.cboFontColorAdd.DataBind();  
 
			}
			
		}

        private void dgSystem_Fill()
		{
			string xml = "" ;
			objUtil=new clsUtility(sn) ;
			ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();

            if (objUtil.ErrCheck(dbs.GetFullInfoSystemUpdates(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus), ref xml), false))
                if (objUtil.ErrCheck(dbs.GetFullInfoSystemUpdates(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus), ref xml), false))
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


			// Show SevirityName
			DataColumn dc = new DataColumn();
			dc.ColumnName = "Visible";
			dc.DataType=Type.GetType("System.Boolean"); 
			dc.DefaultValue="false"; 
			ds.Tables[0].Columns.Add(dc); 


			
			foreach(DataRow rowItem in ds.Tables[0].Rows)
			{
                if (Convert.ToInt16(rowItem["SystemUpdateType"]) == Convert.ToInt16(Enums.SystemUpdateType.SystemStatus))
                    rowItem["Visible"] = true; 
			}

			dgSystem.DataSource = ds;
			dgSystem.DataBind(); 

		}



		private void DsFontColor_Fill()
		{
			try
			{
                DataTable tblFontColor = dsFontColor.Tables.Add("FontColor");

                tblFontColor.Columns.Add("FontColor", typeof(string));

				object[] objRow = new object[1] ;
				objRow[0] = "Black";
                dsFontColor.Tables[0].Rows.Add(objRow);
                objRow[0] = "Red";
                dsFontColor.Tables[0].Rows.Add(objRow);
                objRow[0] = "Green";
                dsFontColor.Tables[0].Rows.Add(objRow);
                objRow[0] = "Blue";
                dsFontColor.Tables[0].Rows.Add(objRow);

				}
			catch(Exception Ex)
			{

			}

		}


        public int GetFontColor(string FontColor) 
		{
			try
			{
				DropDownList cboFontColor=new DropDownList() ;
                cboFontColor.DataValueField = "FontColor";
                cboFontColor.DataTextField = "FontColor";
                DsFontColor_Fill();
                cboFontColor.DataSource = dsFontColor;
                cboFontColor.DataBind();

                cboFontColor.SelectedIndex = -1;
                cboFontColor.Items.FindByValue(FontColor.ToString()).Selected = true;
                return cboFontColor.SelectedIndex;
			}
			catch(Exception Ex)
			{

				return 0;
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
						lblMessage.Text = "Please click 'Delete' again to confirm removal of System Update";
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
                        lblMessage.Visible = true;
                        lblMessage.Text = "Failed delete message";
						return;
					}
			

				dgSystem.SelectedIndex = -1;
				lblMessage.Text = "System Update Deleted!";
				dgSystem.CurrentPageIndex=0; 
				dgSystem_Fill();
				ViewState["ConfirmDelete"] = "0";
				//	}
			}
			catch(Exception Ex)
			{
                lblMessage.Visible = true;
                lblMessage.Text = "Failed delete message";
			}
		}

		private void dgSystem_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgSystem.CurrentPageIndex = e.NewPageIndex;
			dgSystem_Fill();
			dgSystem.SelectedIndex = -1	;
		}

		protected void cmdAddMsg_Click(object sender, System.EventArgs e)
		{
            try
            {
                objUtil = new clsUtility(sn);
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                Int16 systemStatus = 0;
                if (chkVisibleMsg.Checked)
                    systemStatus = Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus);

                Int16 intBold = 0;
                if (chkBold.Checked)
                    intBold = 1;
                else
                    intBold = 0;


                if (objUtil.ErrCheck(dbs.AddSystemUpdate(sn.UserID, sn.SecId, this.txtMsg.Text, this.txtMsgFr.Text, systemStatus, (Int16)VLF.CLS.Def.Enums.AlarmSeverity.NoAlarm, this.cboFontColorAdd.SelectedItem.Value, intBold), false))
                    if (objUtil.ErrCheck(dbs.AddSystemUpdate(sn.UserID, sn.SecId, this.txtMsg.Text, this.txtMsgFr.Text, systemStatus, (Int16)VLF.CLS.Def.Enums.AlarmSeverity.NoAlarm, this.cboFontColorAdd.SelectedItem.Value, intBold), true))
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = "Failed to add new message";
                        return;
                    }

                this.lblMessage.Text = "";
                dgSystem.SelectedIndex = -1;
                dgSystem.CurrentPageIndex = 0;
                dgSystem_Fill();
                ViewState["ConfirmDelete"] = "0";

                lblMessage.Visible = true;
                lblMessage.Text = "New message added";
            }
            catch (Exception Ex)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Failed to add new message";
            }

		}








        protected void dgSystem_EditCommand(object source, DataGridCommandEventArgs e)
        {
            dgSystem.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
            dgSystem_Fill();
            dgSystem.SelectedIndex = -1;
        }
       
        protected void dgSystem_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            dgSystem.EditItemIndex = -1;
            dgSystem_Fill();
        }

        protected void dgSystem_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

            try
            {
                TextBox txtMsg;
                TextBox txtMsgFr;
                DropDownList cboColor;
                CheckBox chkVisible;
                CheckBox chkBold;


                string msgId = dgSystem.DataKeys[e.Item.ItemIndex].ToString();
                txtMsg = (TextBox)e.Item.FindControl("txtMsg");
                txtMsgFr = (TextBox)e.Item.FindControl("txtMsgFr");
                cboColor = (DropDownList)e.Item.FindControl("cboFontColor");

                chkVisible = (CheckBox)e.Item.FindControl("chkVisible");
                chkBold = (CheckBox)e.Item.FindControl("chkFontBold");


                Int16 systemStatus = 0;
                if (chkVisible.Checked)
                    systemStatus = Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus);



                Int16 intBold = 0;
                if (chkBold.Checked)
                    intBold = 1;
                else
                    intBold = 0;


                objUtil = new clsUtility(sn);
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                if (objUtil.ErrCheck(dbs.UpdateSystemUpdateTable(sn.UserID, sn.SecId, Convert.ToInt32(msgId), txtMsg.Text, txtMsgFr.Text, systemStatus, (Int16)VLF.CLS.Def.Enums.AlarmSeverity.NoAlarm, cboColor.SelectedItem.Value, intBold), false))
                    if (objUtil.ErrCheck(dbs.UpdateSystemUpdateTable(sn.UserID, sn.SecId, Convert.ToInt32(msgId), txtMsg.Text, txtMsgFr.Text, systemStatus, (Int16)VLF.CLS.Def.Enums.AlarmSeverity.NoAlarm, cboColor.SelectedItem.Value, intBold), true))
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = "Failed to update message";
                        return;
                    }



                dgSystem.EditItemIndex = -1;
                dgSystem_Fill();
                lblMessage.Visible = true;
                lblMessage.Text = "Message updated";
            }
            catch (Exception Ex)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Failed to update message";
            }
        }
        protected void dgSystem_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
}
}
