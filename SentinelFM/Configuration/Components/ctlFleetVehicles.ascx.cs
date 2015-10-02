namespace SentinelFM.Components
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.IO;
	using System.Configuration;
 
	/// <summary>
	///		Summary description for ctlFleetVehicles.
	/// </summary>
	public partial  class ctlFleetVehicles : BaseControl 
	{
        public string ConfirmAddAll = string.Empty;
        public string ConfirmRemoveAll = string.Empty;
	

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                ConfirmAddAll = (string)base.GetLocalResourceObject("confirmAddAll");
                ConfirmRemoveAll = (string)base.GetLocalResourceObject("confirmRemoveAll");
                
				if (!Page.IsPostBack)
				{
                    
					GuiSecurity(this); 
					this.tblVehicles.Visible=false;
					CboFleet_Fill();

					if (sn.User.DefaultFleet!=-1)
					{
						cboToFleet.SelectedIndex =cboToFleet.Items.IndexOf (cboToFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
						this.tblVehicles.Visible=true; 
						lstUnAssVehicles_Fill();
						lstAssVehicles_Fill();  
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
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+ " Form:frmVehicleFleet.aspx"));    
			}
		}


		private void CboFleet_Fill()
		{
			try
			{

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter  = "FleetType<>'oh'";
                cboToFleet.DataSource = FleetView;
                cboToFleet.DataBind();
                cboToFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectFleet"), "-1"));
			
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}
			
		}


		private void lstUnAssVehicles_Fill()
		{

			try
			{
				
				StringReader strrXML = null;
				DataSet dsVehicle=new DataSet(); 
							
				string xml = "" ;
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;
			 
				if( objUtil.ErrCheck( dbf.GetAllUnassingToFleetVehiclesInfoXML ( sn.UserID , sn.SecId ,Convert.ToInt32(sn.User.OrganizationId),   Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetAllUnassingToFleetVehiclesInfoXML( sn.UserID , sn.SecId,Convert.ToInt32(sn.User.OrganizationId),Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ), true ) )
					{
						return;
					}
				if (xml == "")
				{
					this.lstUnAss.Items.Clear();  
					return;
				}

				strrXML = new StringReader( xml ) ;
				dsVehicle.ReadXml(strrXML);
				sn.History.DsUnAssVehicle=dsVehicle;
				this.lstUnAss.DataSource=dsVehicle;
				this.lstUnAss.DataBind();  
			}

			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}
  
		}



		private void lstAssVehicles_Fill()
		{
			try
			{
				
				StringReader strrXML = null;
				DataSet dsVehicle=new DataSet(); 
							
				string xml = "" ;
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;
			 
				if( objUtil.ErrCheck( dbf.GetVehiclesInfoXMLByFleetId ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetVehiclesInfoXMLByFleetId( sn.UserID , sn.SecId,Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ), true ) )
					{
						return;
					}
				if (xml == "")
				{
					this.lstAss.Items.Clear();  
					return;
				}

				strrXML = new StringReader( xml ) ;
				dsVehicle.ReadXml(strrXML);
				sn.History.DsAssVehicle=dsVehicle;
				  
				this.lstAss.DataSource=dsVehicle;
				this.lstAss.DataBind();  
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}
  
		}

	
		protected void cboToFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{

			if (Convert.ToInt32(cboToFleet.SelectedItem.Value)!=-1)
			{
				this.tblVehicles.Visible=true;  
				lstUnAssVehicles_Fill();
				lstAssVehicles_Fill();  
			}
			else
			{
				this.lstAss.Items.Clear();   
				this.lstUnAss.Items.Clear();   
			}
		}

		

		protected void cmdAdd_Click(object sender, System.EventArgs e)
		{
			try
			{

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

				this.lblMessage.Text="";

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
					return;
				}

			
				if (this.lstUnAss.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectVehicle");  //"Please select a vehicle.";
					return;
				}
			
				foreach(ListItem li in lstUnAss.Items)
				{
					if (li.Selected)
					{
						if( objUtil.ErrCheck( dbf.AddVehicleToFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							if( objUtil.ErrCheck( dbf.AddVehicleToFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),true ) )
							{
								return;
							}
                        poh.AssignFleetToAllParents(sn.User.OrganizationId, Convert.ToInt32(cboToFleet.SelectedItem.Value), Convert.ToInt32(li.Value));
					}
 
				}


				lstUnAssVehicles_Fill();
				lstAssVehicles_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}


			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}


		}

		protected void cmdRemove_Click(object sender, System.EventArgs e)
		{
			try
			{
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

				this.lblMessage.Text="";

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
					return;
				}


                if (this.cboToFleet.SelectedItem.Text.ToString().TrimEnd() == "All Vehicles" || this.cboToFleet.SelectedItem.Text.ToString().TrimEnd() == "Tous les véhicules")
                {
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("AllVehiclesMsg"); //"Can not remove vehicle(s) from fleet 'All Vehicles'.";
					return;
				}


				if (this.lstAss.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");  //"Please select a vehicle.";
					return;
				}

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				foreach(ListItem li in lstAss.Items)
				{
					if (li.Selected)
					{
						if( objUtil.ErrCheck( dbf.DeleteVehicleFromFleet ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							if( objUtil.ErrCheck( dbf.DeleteVehicleFromFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							{
								return;
							}

                        poh.RemoveVehicleFromFleet(sn.User.OrganizationId, Convert.ToInt32(cboToFleet.SelectedItem.Value), Convert.ToInt32(li.Value));
					}
 
				}


				lstUnAssVehicles_Fill();
				lstAssVehicles_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}

		}

		protected void cmdAddAll_Click(object sender, System.EventArgs e)
		{
			try
			{
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");// "Please select a fleet.";
					return;
				}

				this.lblMessage.Text="";

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				DataSet dsUnAssVehicle=new DataSet();
				dsUnAssVehicle=sn.History.DsUnAssVehicle;

				  

				if (dsUnAssVehicle!=null)
				{
					foreach(DataRow rowItem in dsUnAssVehicle.Tables[0].Rows)
					{
						if( objUtil.ErrCheck( dbf.AddVehicleToFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["VehicleId"] ) ),false ) )
							if( objUtil.ErrCheck( dbf.AddVehicleToFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["VehicleId"] ) ),false ) )
							{
								return;
							}

                        poh.AssignFleetToAllParents(sn.User.OrganizationId, Convert.ToInt32(cboToFleet.SelectedItem.Value), Convert.ToInt32(rowItem["VehicleId"]));
					}

					lstUnAssVehicles_Fill();
					lstAssVehicles_Fill(); 	
				}

			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}
			
		}

		protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
		{
			try
			{
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = null;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet"); // "Please select a fleet.";
					return;
				}

				if (this.cboToFleet.SelectedItem.Text.ToString().TrimEnd() =="All Vehicles" || this.cboToFleet.SelectedItem.Text.ToString().TrimEnd()=="Tous les véhicules")
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("AllVehiclesMsg"); //"Can not remove vehicle(s) from fleet 'All Vehicles'.";
					return;
				}

				this.lblMessage.Text="";

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				DataSet dsAssVehicle=new DataSet();
				dsAssVehicle=sn.History.DsAssVehicle;

				foreach(DataRow rowItem in dsAssVehicle.Tables[0].Rows)
				{
					if( objUtil.ErrCheck( dbf.DeleteVehicleFromFleet ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["VehicleId"] ) ),false ) )
						if( objUtil.ErrCheck( dbf.DeleteVehicleFromFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["VehicleId"] ) ),false ) )
						{
							return;
						}
                    poh.RemoveVehicleFromFleet(sn.User.OrganizationId, Convert.ToInt32(cboToFleet.SelectedItem.Value), Convert.ToInt32(rowItem["VehicleId"]));
				}

				lstUnAssVehicles_Fill();
				lstAssVehicles_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:frmVehicleFleet.aspx"));    
			}
		}

        protected void btnUpload_Click(object sender, System.EventArgs e)
        {
            if (this.newFleetVehicleAssignment.FileName.Trim() != String.Empty)
            {
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                int FleetId = Convert.ToInt32(cboToFleet.SelectedValue);
                string fileName = "";
                try
                {
                    DataTable dt = new DataTable();
                    fileName = Server.MapPath("~/App_Data/") + this.newFleetVehicleAssignment.FileName;
                    this.newFleetVehicleAssignment.PostedFile.SaveAs(fileName);

                    DataTable dtVehicles = clsUtility.ReadDataFromExcel2003(fileName);                    
                    dtVehicles.TableName = "Template";

                    string result;
                    using (StringWriter sw = new StringWriter())
                    {
                        dtVehicles.WriteXml(sw);
                        result = sw.ToString();
                    }
                    if (objUtil.ErrCheck(dbf.AddAllVehicleToFleet(sn.UserID, sn.SecId, result, FleetId, sn.User.OrganizationId), false))
                        if (objUtil.ErrCheck(dbf.AddAllVehicleToFleet(sn.UserID, sn.SecId, result, FleetId, sn.User.OrganizationId), true))
                        {
                            lblMessage.Visible = true;
                            lblMessage.Text = (string)base.GetLocalResourceObject("ErrorMsgFleet");
                            return;
                        }
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("SuccessMsgFleet") + " " + Convert.ToString(cboToFleet.SelectedItem);
                    
                }
                catch
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("ErrorMsgFleet");
                }
            }
        }

        //Changes
        protected void btnBatchUpload_Click(object sender, System.EventArgs e)
        {
            if (this.newMultipleFleetVehicleAssignment.FileName.Trim() != String.Empty)
            {
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
                int FleetId = Convert.ToInt32(cboToFleet.SelectedValue);
                string fileName = "";
                try
                {
                    DataTable dt = new DataTable();
                    fileName = Server.MapPath("~/App_Data/") + this.newMultipleFleetVehicleAssignment.FileName;
                    this.newMultipleFleetVehicleAssignment.PostedFile.SaveAs(fileName);

                    DataTable dtVehicles = clsUtility.ReadDataFromExcel2003(fileName);
                    dtVehicles.TableName = "NewTemplate";

                    string result;
                    using (StringWriter sw = new StringWriter())
                    {
                        dtVehicles.WriteXml(sw);
                        result = sw.ToString();                        
                    }


                    if (objUtil.ErrCheck(dbf.AddAllVehicleToMultipleFleet(sn.UserID, sn.SecId, result, sn.User.OrganizationId), false))
                        if (objUtil.ErrCheck(dbf.AddAllVehicleToMultipleFleet(sn.UserID, sn.SecId, result, sn.User.OrganizationId), true))
                        {
                            lblMessage.Visible = true;
                            lblMessage.Text = (string)base.GetLocalResourceObject("ErrorMsgFleet");
                            return;
                        }
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("SuccessMsgMultipleFleet");

                }
                catch
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = (string)base.GetLocalResourceObject("ErrorMsgFleet");
                }
            }
        }
        //Changes

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
