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
using System.Configuration;

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmFleets.
	/// </summary>
	public partial class frmFleets : SentinelFMBasePage
	{
		protected System.Web.UI.WebControls.RequiredFieldValidator valReqEmail;
		protected System.Web.UI.WebControls.RegularExpressionValidator valEmail;

        public bool IsHydroQuebec = false;
		
        string confirm = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956)
                    IsHydroQuebec = true;
                else
                    IsHydroQuebec = false;

				if (!Page.IsPostBack)
				{
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref Form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
					GuiSecurity(this); 
					this.tblFleetAdd.Visible=false;  
					ViewState["ConfirmDelete"] = "0";
                    this.lblMessage.Text = "";  
					DgFleets_Fill(true);

                    if (IsHydroQuebec || sn.User.OrganizationId == 952)
                    {
                        cmdDriverSkill.Visible = false;
                    }
				}
			}

			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}


			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
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
			this.dgFleets.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgFleets_PageIndexChanged);
			this.dgFleets.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFleets_CancelCommand);
			this.dgFleets.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFleets_EditCommand);
			this.dgFleets.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFleets_UpdateCommand);
			this.dgFleets.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFleets_DeleteCommand);
			this.dgFleets.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgFleets_ItemDataBound);

		}
		#endregion

		protected void cmdEmails_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmEmails.aspx"); 
		}


		private void DgFleets_Fill(bool forceUpdate)
		{
			try
			{
                if (forceUpdate)
                {


                    DataSet dsFleets = new DataSet();
                    StringReader strrXML = null;

                    string xml = "";
                    ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                        if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                        {
                            this.dgFleets.DataSource = null;
                            this.dgFleets.DataBind();
                            return;
                        }


                    if (xml == "")
                    {
                        this.dgFleets.DataSource = null;
                        this.dgFleets.DataBind();
                        return;
                    }

                    strrXML = new StringReader(xml);
                    dsFleets.ReadXml(strrXML);

                    DataView FleetView = dsFleets.Tables[0].DefaultView;
                    FleetView.RowFilter = "FleetType<>'oh'";

                    //this.dgFleets.DataSource=dsFleets  ;

                    ViewState["dsFleets"] = dsFleets;
                }
                try {
                if (String.IsNullOrEmpty(this.txtSearchParam.Text))
                {
                    this.dgFleets.DataSource = ((DataSet)ViewState["dsFleets"]).Tables[0];
                    
                }
                else // filtered data
                {
                    this.dgFleets.DataSource = GetFilteredData();
                    
                }
               
                if (sn.Misc.ConfVehiclesSelectedGridPage < this.dgFleets.PageCount)
                {
                    this.dgFleets.CurrentPageIndex = sn.Misc.ConfVehiclesSelectedGridPage;
                    
                }
                else
                    this.dgFleets.CurrentPageIndex = 0;
                this.dgFleets.DataBind();
                }
                catch (HttpException ex)
                {
                    this.dgFleets.CurrentPageIndex = 0;
                }
                this.dgFleets.DataBind();
				
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
			
		}

	


        protected void dgFleets_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgFleets.EditItemIndex = -1;
			DgFleets_Fill(true);
			this.cmdAddFleet.Enabled=true; 
		}

		protected void dgFleets_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{




				//Check security for delete fleet
				bool cmdDelete=sn.User.ControlEnable(sn,11);
				if (!cmdDelete)
				{
					lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
					return;
				}

                if (confirm == "")
                    return;


				DataSet ds=new DataSet();
				ds=(DataSet)ViewState["dsFleets"];


				foreach(DataRow rowItem in ds.Tables[0].Rows)
				{
					if (Convert.ToInt32(rowItem["FleetId"])==Convert.ToInt32(dgFleets.DataKeys[e.Item.ItemIndex] ))
					{
                  if (rowItem["FleetName"].ToString().TrimEnd() == Resources.Const.defaultFleetName)
						{
							lblMessage.Visible = true;
                            lblMessage.Text = (string)base.GetLocalResourceObject("AllVehicles");
							return;
						}
					}
				}


				lblMessage.Text = "";
				lblMessage.Visible = true;
			

				ServerDBFleet.DBFleet     dbf = new ServerDBFleet.DBFleet()   ;
				

				
				if( objUtil.ErrCheck( dbf.DeleteFleetByFleetId   ( sn.UserID , sn.SecId  ,Convert.ToInt32(dgFleets.DataKeys[e.Item.ItemIndex] )) ,false ) )
					if( objUtil.ErrCheck( dbf.DeleteFleetByFleetId   ( sn.UserID , sn.SecId  ,Convert.ToInt32(dgFleets.DataKeys[e.Item.ItemIndex] )) ,true ) )
					{
						this.lblMessage.Visible=true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("DeleteFailed");  
						return;
					}

				
				dgFleets.SelectedIndex = -1;
                lblMessage.Text = (string)base.GetLocalResourceObject("Deleted");
				dgFleets.CurrentPageIndex=0; 
				DgFleets_Fill(true);
				ViewState["ConfirmDelete"] = "0";
                confirm = ""; 
				
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

        protected void dgFleets_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{


				//Check security for update fleet
				bool cmdUpdate=sn.User.ControlEnable(sn,12);
				if (!cmdUpdate)
				{
					lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
					return;
				}

				lblMessage.Text = "";

				TextBox txtFleetName;
				TextBox txtDescription;

				ViewState["ConfirmDelete"] = "0";
				string FleetId = dgFleets.DataKeys[e.Item.ItemIndex].ToString() ;
				txtFleetName =(TextBox) e.Item.FindControl("txtFleetName") ;
				txtDescription =(TextBox) e.Item.FindControl("txtDescription") ;
				

				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;
				

				if( objUtil.ErrCheck( dbf.UpdateFleetInfo    ( sn.UserID , sn.SecId,Convert.ToInt32(FleetId),txtFleetName.Text,sn.User.OrganizationId,txtDescription.Text),false ) )
					if( objUtil.ErrCheck( dbf.UpdateFleetInfo    ( sn.UserID , sn.SecId,Convert.ToInt32(FleetId),txtFleetName.Text,sn.User.OrganizationId,txtDescription.Text),true ) )
					{
						this.cmdAddFleet.Enabled=true; 
						return;
					}


				this.cmdAddFleet.Enabled=true; 
				lblMessage.Visible = true;
				dgFleets.EditItemIndex = -1;
				DgFleets_Fill(true);
                lblMessage.Text = (string)base.GetLocalResourceObject("Updated");
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		protected void cmdAddFleet_Click(object sender, System.EventArgs e)
		{
			this.lblMessage.Text="";  
			this.txtNewFleetName.Text="";  
			this.txtNewFleetDescription.Text="";  
			this.cmdAddFleet.Enabled=false ;  
			this.tblFleetAdd.Visible=true; 
		}

		protected void cmdSaveFleet_Click(object sender, System.EventArgs e)
		{
			try
			{

				Page.Validate(); 
				if (!Page.IsValid)
					return;


				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

                string assignVehiclesMsg = "";


				int FleetId=-1;
				if( objUtil.ErrCheck(  dbf.AddFleet    ( sn.UserID , sn.SecId ,this.txtNewFleetName.Text       ,sn.User.OrganizationId   ,this.txtNewFleetDescription.Text  ,ref FleetId) ,false ) )
					if( objUtil.ErrCheck(  dbf.AddFleet    ( sn.UserID , sn.SecId ,this.txtNewFleetName.Text       ,sn.User.OrganizationId   ,this.txtNewFleetDescription.Text  ,ref FleetId) ,true ) )
					{
						
						this.cmdAddFleet.Enabled=true;  
						this.lblMessage.Visible=true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("AddFleetFailed");
						return;
					}

            //if (FleetId!=-1)
            //{
            //   if( objUtil.ErrCheck(  dbf.AddUserToFleet   ( sn.UserID , sn.SecId ,FleetId,sn.UserID) ,false ) )
            //      if( objUtil.ErrCheck(  dbf.AddUserToFleet   ( sn.UserID , sn.SecId ,FleetId,sn.UserID) ,true ) )
            //      {
            //         this.cmdAddFleet.Enabled=true;  
            //         this.lblMessage.Visible=true;
            //                this.lblMessage.Text = (string)base.GetLocalResourceObject("AddUsertoFleetFailed");
            //         return;
            //      }
            //}
                
                if (FleetId != -1 && this.newFleetVehicleAssignment.FileName.Trim() != String.Empty)
                {
                    // Assign vehilces to the new created fleet
                    string fileName = "";
                    try
                    {
                        fileName = Server.MapPath("~/App_Data/") + this.newFleetVehicleAssignment.FileName;
                        this.newFleetVehicleAssignment.PostedFile.SaveAs(fileName);

                        DataTable dtVehicles = clsUtility.ReadDataFromExcel2003(fileName);
                        
                        ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                        foreach (DataRow dr in dtVehicles.Rows)
                        {
                            int boxid = 0;
                            int.TryParse(dr["BoxId"].ToString().Trim(), out boxid);
                            if (boxid <= 0)
                            {
                                string vehilceDescription = dr["Vehicle"].ToString();
                                dbv.GetBoxIDByVehicleDescription(sn.UserID, sn.SecId, vehilceDescription, sn.User.OrganizationId, ref boxid);
                            }
                            if (boxid > 0)
                            {
                                string xml = string.Empty;
                                dbv.GetVehicleInfoXMLByBoxId(sn.UserID, sn.SecId, boxid, ref xml);

                                if (xml != "")
                                {
                                    DataSet ds = new DataSet();
                                    ds.ReadXml(new StringReader(xml));
                                    long vehicleId = Convert.ToInt64(ds.Tables[0].Rows[0]["VehicleId"]);

                                    if( objUtil.ErrCheck( dbf.AddVehicleToFleet( sn.UserID , sn.SecId, FleetId, vehicleId ),false ) )
							            if( objUtil.ErrCheck( dbf.AddVehicleToFleet( sn.UserID , sn.SecId, FleetId, vehicleId ),true ) )
							            {
								            
							            }
                                }
                            }
                        }
                        
                    }
                    catch (Exception Ex)
                    {
                        assignVehiclesMsg = (string)base.GetLocalResourceObject("AssignVehiclesFailed");
                    }
                }
				 
				dgFleets.SelectedIndex = -1	;
				dgFleets.CurrentPageIndex = 0;
				DgFleets_Fill(true);



				this.txtNewFleetName.Text="";  
				this.txtNewFleetDescription.Text="";  
				this.lblMessage.Visible=true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("AddFleet") +  "<br />" + assignVehiclesMsg;
				this.cmdAddFleet.Enabled=true;  
				this.tblFleetAdd.Visible=false;  
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}

		}

        protected void dgFleets_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			this.cmdAddFleet.Enabled=true;  
			this.tblFleetAdd.Visible=false;  




			DataSet ds=new DataSet();
			ds=(DataSet)ViewState["dsFleets"];


			foreach(DataRow rowItem in ds.Tables[0].Rows)
			{
				if (Convert.ToInt32(rowItem["FleetId"])==Convert.ToInt32(dgFleets.DataKeys[e.Item.ItemIndex] ))
				{
               if (rowItem["FleetName"].ToString().TrimEnd() == Resources.Const.defaultFleetName)
					{
						lblMessage.Visible = true;
                        lblMessage.Text = (string)base.GetLocalResourceObject("AllVehicles");
						return;
					}
				}
			}

			this.lblMessage.Text="";  
			dgFleets.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
			DgFleets_Fill(true);
			dgFleets.SelectedIndex = -1;

			this.cmdAddFleet.Enabled=false;  
		}

		protected void cmdVehicles_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleInfo.aspx"); 
		}

		private void cmdPreference_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmPreference.aspx"); 
		}

		protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmFleetVehicles.aspx"); 
		}

		protected void cmdCancelAddFleet_Click(object sender, System.EventArgs e)
		{
			this.cmdAddFleet.Enabled=true;  
			this.tblFleetAdd.Visible=false;  
		}

		protected void cmdFleets_Click(object sender, System.EventArgs e)
		{
		
		}

		protected void cmdFleetUsers_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmFleetUsers.aspx"); 
		}

		protected void cmdUsers_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmUsers.aspx"); 
		}

        protected void cmdDriverSkill_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmdriverskill.aspx");
        }

        protected void dgFleets_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgFleets.CurrentPageIndex = e.NewPageIndex;
            sn.Misc.ConfVehiclesSelectedGridPage = e.NewPageIndex;
			DgFleets_Fill(false); 
			
		}

		private void dgFleets_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if ((e.Item.ItemType == ListItemType.AlternatingItem ) || (e.Item.ItemType == ListItemType.Item))
			{
                e.Item.Cells[2].ToolTip = (string)base.GetLocalResourceObject("dgFleets_Tooltip_Edit");
                e.Item.Cells[3].ToolTip = (string)base.GetLocalResourceObject("dgFleets_Tooltip_Delete");
			}
		}




        protected void dgFleets_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                confirm = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmDelete") + "')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[3].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
        }
        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx"); 
        }
        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx"); 
        }
        protected void cmdOrganization_Click(object sender, EventArgs e)
        {

        }


        protected void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = (DataSet)ViewState["dsFleets"];
            this.txtSearchParam.Text = "";
            this.dgFleets.DataSource = ds;
            this.dgFleets.DataBind();
            this.txtSearchParam.Focus();
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            DgFleets_Fill(false);

            
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            
            this.txtSearchParam.Text = "";
            this.dgFleets.DataSource = ((DataSet)ViewState["dsFleets"]).Tables[0];
            this.dgFleets.DataBind();
        }

        private DataTable GetFilteredData()
        {
            DataRow[] drCollections = null;
            DataTable dt = ((DataSet)ViewState["dsFleets"]).Tables[0].Clone();
           
            string filter = "";

            if (!string.IsNullOrEmpty(this.txtSearchParam.Text.Trim()))
            {
                string searchContent = this.txtSearchParam.Text.Trim().Replace("'", "''");

                switch (cboSearchType.SelectedItem.Value.Trim())
                {
                    case "0":
                        filter = String.Format("FleetName like '%{0}%'", searchContent);
                        break;

                    case "1":
                        filter = String.Format("Description like '%{0}%'", searchContent);
                        break;

                }
            }

            drCollections = ((DataSet)ViewState["dsFleets"]).Tables[0].Select(filter, "FleetName");
            if (drCollections != null && drCollections.Length > 0)
            {
                foreach (DataRow dr in drCollections)
                {
                    dt.ImportRow(dr);
                }
            }
            return dt;
        }
}
}
