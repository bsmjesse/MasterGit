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
	///		Summary description for ctlFleetUsers.
	/// </summary>
    public partial class ctlFleetUsers : BaseControl
	{
		
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				

				if (!Page.IsPostBack)
				{
					GuiSecurity(this); 
					this.tblUsers.Visible=false;
					CboFleet_Fill();

					if (sn.User.DefaultFleet!=-1)
					{
						cboToFleet.SelectedIndex =cboToFleet.Items.IndexOf (cboToFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
						this.tblUsers.Visible=true; 
						lstUnAssUsers_Fill();
						lstAssUsers_Fill();  
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
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
			}
		}

		private void CboFleet_Fill()
		{
			try
			{

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter = "FleetType<>'oh'";
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
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.ascx"));    
			}
			
		}

		

		private void lstUnAssUsers_Fill()
		{

			try
			{
				
				StringReader strrXML = null;
				DataSet ds=new DataSet(); 
							
				string xml = "" ;
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;
			 
				if( objUtil.ErrCheck( dbf.GetUnassignedUsersInfoXMLByFleetId  ( sn.UserID , sn.SecId ,  Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetUnassignedUsersInfoXMLByFleetId  ( sn.UserID , sn.SecId ,  Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ),true ) )
					{
						return;
					}
				if (xml == "")
				{
					this.lstUnAss.Items.Clear();  
					return;
				}

				strrXML = new StringReader( xml ) ;
				ds.ReadXml(strrXML);
				DataColumn UserFullName = new DataColumn("UserFullName",Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(UserFullName);


				foreach(DataRow rw in ds.Tables[0].Rows)
					rw["UserFullName"]=rw["LastName"].ToString()+" "+rw["FirstName"].ToString() ;
				

				sn.History.DsUnAssUsers=ds;
				this.lstUnAss.DataSource=ds;
				this.lstUnAss.DataBind();  
			}

			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
			}
  
		}

		private void lstAssUsers_Fill()
		{
			try
			{
				
				StringReader strrXML = null;
				DataSet ds=new DataSet(); 
							
				string xml = "" ;
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;
			 
				if( objUtil.ErrCheck( dbf.GetUsersInfoXMLByFleetId  ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetUsersInfoXMLByFleetId  ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value), ref xml ),true ) )
					{
						return;
					}
				if (xml == "")
				{
					this.lstAss.Items.Clear();  
					return;
				}

				strrXML = new StringReader( xml ) ;
				ds.ReadXml(strrXML);
				DataColumn UserFullName = new DataColumn("UserFullName",Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(UserFullName);

				foreach(DataRow rw in ds.Tables[0].Rows)
					rw["UserFullName"]=rw["LastName"].ToString()+" "+rw["FirstName"].ToString() ;
				
                  
				sn.History.DsAssUsers=ds;
				this.lstAss.DataSource=ds;
				this.lstAss.DataBind();  
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void cmdAdd_Click(object sender, System.EventArgs e)
		{
			try
			{

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				this.lblMessage.Text="";

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
					return;
				}

			
				if (this.lstUnAss.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUser");
					return;
				}
			
				foreach(ListItem li in lstUnAss.Items)
				{
					if (li.Selected)
					{
						if( objUtil.ErrCheck( dbf.AddUserToFleet ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							if( objUtil.ErrCheck( dbf.AddUserToFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							{
								return;
							}
					}
 
				}


				lstUnAssUsers_Fill();
				lstAssUsers_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}


			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
			}
		}

		protected void cmdAddAll_Click(object sender, System.EventArgs e)
		{
			try
			{

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
					return;
				}

				this.lblMessage.Text="";

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				DataSet dsUnAssUsers=new DataSet();
				dsUnAssUsers=sn.History.DsUnAssUsers;
                  
				foreach(DataRow rowItem in dsUnAssUsers.Tables[0].Rows)
				{
					if( objUtil.ErrCheck( dbf.AddUserToFleet ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["UserId"] ) ),false ) )
						if( objUtil.ErrCheck( dbf.AddUserToFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["UserId"] ) ),false ) )
						{
							return;
						}
				}

				lstUnAssUsers_Fill();
				lstAssUsers_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
			}
		}

		protected void cmdRemove_Click(object sender, System.EventArgs e)
		{
			try
			{

				this.lblMessage.Text="";

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
					return;
				}


				if (this.lstAss.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUser");
					return;
				}

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				foreach(ListItem li in lstAss.Items)
				{
					if (li.Selected)
					{
						if( objUtil.ErrCheck( dbf.DeleteUserFromFleet  ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							if( objUtil.ErrCheck( dbf.DeleteUserFromFleet( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(li.Value ) ),false ) )
							{
								return;
							}
					}
 
				}


				lstUnAssUsers_Fill();
				lstAssUsers_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
			}

		}

		protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
		{
			try
			{

				if (this.cboToFleet.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectFleet");
					return;
				}

				this.lblMessage.Text="";

				
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

				DataSet dsAssUsers=new DataSet();
				dsAssUsers=sn.History.DsAssUsers;

				foreach(DataRow rowItem in dsAssUsers.Tables[0].Rows)
				{
					if( objUtil.ErrCheck( dbf.DeleteUserFromFleet  ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["UserId"] ) ),false ) )
						if( objUtil.ErrCheck( dbf.DeleteUserFromFleet  ( sn.UserID , sn.SecId ,Convert.ToInt32(cboToFleet.SelectedItem.Value),Convert.ToInt32(rowItem["UserId"] ) ),true ) )
						{
							return;
						}
				}

				lstUnAssUsers_Fill ();
				lstAssUsers_Fill(); 	
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:ctlFleetUsers.aspx"));    
			}
		}

		protected void cboToFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboToFleet.SelectedItem.Value)!=-1)
			{
				this.tblUsers.Visible=true;  
				lstUnAssUsers_Fill ();
				lstAssUsers_Fill ();  
			}
			else
			{
				this.lstAss.Items.Clear();   
				this.lstUnAss.Items.Clear();   
			}
		}
	}
}
