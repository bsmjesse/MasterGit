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
using System.Globalization;
  
namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmOutputs.
	/// </summary>
	public partial class frmOutputs : SentinelFMBasePage
	{


        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";

        public bool MutipleUserHierarchyAssignment = false;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                bool ShowOrganizationHierarchy = false;
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                }

                if (ShowOrganizationHierarchy)
                {
                    MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                    trFleetSelectOption.Visible = true;

                    string defaultnodecode = string.Empty;
                    ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                    string xml = "";
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                        {

                        }

                    StringReader strrXML = new StringReader(xml);
                    DataSet dsPref = new DataSet();
                    dsPref.ReadXml(strrXML);

                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {
                        if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                        {
                            defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                        }

                    }

                    defaultnodecode = defaultnodecode ?? string.Empty;
                    if (defaultnodecode == string.Empty)
                    {
                        if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                            defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                        else
                            defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

                    }
                    if (!IsPostBack)
                    {
                        DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                        hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();

                        if (MutipleUserHierarchyAssignment)
                        {
                            hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
							hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                            //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                        }
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }
                    if (MutipleUserHierarchyAssignment)
                    {

                        if (hidOrganizationHierarchyFleetId.Value.Trim() == string.Empty)
                            DefaultOrganizationHierarchyFleetName = "";
                        else if (hidOrganizationHierarchyFleetId.Value.Contains(","))
                            DefaultOrganizationHierarchyFleetName = GetLocalResourceObject("ResMultipleHierarchies").ToString();
                        else
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(hidOrganizationHierarchyFleetId.Value));
                    }
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                    //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                    //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                    //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
                }
                else
                {
                    trFleetSelectOption.Visible = false;
                }

				if (!Page.IsPostBack)
				{

                    //Devin
                    if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 480)
                    {
                        btnFuelCategory.Visible = true;
                    }

                    //if (sn.User.SuperOrganizationId == 382)
                 //   this.btnEquipmentAssignment.Visible = true;
                //else
                  //  this.btnEquipmentAssignment.Visible = false ;

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmEmails, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
					GuiSecurity(this); 
					CboFleet_Fill();

					if (sn.User.DefaultFleet!=-1)
					{
						cboFleet.SelectedIndex =cboFleet.Items.IndexOf (cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
						CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
						this.lblVehicleName.Visible=true;
						this.cboVehicle.Visible=true;  
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


		private void CboFleet_Fill()
		{
			try
			{
				
				DataSet dsFleets=new DataSet() ;
				StringReader strrXML = null;
				string xml = "" ;
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
					{
						cboFleet.DataSource = null;
						return;
					}
				strrXML = new StringReader( xml ) ;
				dsFleets.ReadXml (strrXML) ;

                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter = "FleetType<>'oh'";
                cboFleet.DataSource = FleetView;
				cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectFleet"), "-1"));
  
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


		private void CboVehicle_Fill(int fleetId)
		{
			try
			{
				DataSet dsVehicle;
				dsVehicle = new DataSet();

				
				StringReader strrXML = null;

							
				string xml = "" ;
				ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;
			 
				if( objUtil.ErrCheck( dbf.GetVehiclesInfoXMLByFleetId( sn.UserID , sn.SecId ,fleetId, ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetVehiclesInfoXMLByFleetId( sn.UserID , sn.SecId,fleetId, ref xml ), true ) )
					{
						return;
					}
				if (xml == "")
				{
                    cboVehicle.Items.Clear();
                    cboVehicle.DataSource = null;
                    cboVehicle.DataBind();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectVehicle"), "-1"));
                    return;
				}

				strrXML = new StringReader( xml ) ;
				dsVehicle.ReadXml(strrXML);
				cboVehicle.DataSource =dsVehicle;
				cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectVehicle"), "-1"));
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

        private void CboVehicle_MultipleFleet_Fill(string fleetIds)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();


                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), true))
                    {
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.DataSource = null;
                    cboVehicle.DataBind();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectVehicle"), "-1"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);
                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectVehicle"), "-1"));
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


		protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			/*if (Convert.ToInt32(cboFleet.SelectedItem.Value)!=-1)
			{
				this.cboVehicle.Visible=true;  
				this.cboVehicle.DataSource=null;
				this.cboVehicle.DataBind();
				this.lblVehicleName.Visible=true;
				this.dgOutputs.DataSource=null;
				this.dgOutputs.DataBind();   
				CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
			}
			else
			{
				this.cboVehicle.DataSource=null;
				this.cboVehicle.DataBind();
				this.cboVehicle.Visible=false;  
				dgOutputs.DataSource=null; 
				dgOutputs.DataBind(); 
				this.lblVehicleName.Visible=false;  
			}*/
            refillCboVehicle();
		}

        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {
            refillCboVehicle();
        }

        private void refillCboVehicle()
        {
            int fleetid = -1;
            string fleetids = string.Empty;
            if (optAssignBased.SelectedItem.Value == "0")
                fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
            else
            {
                if (MutipleUserHierarchyAssignment)
                    fleetids = hidOrganizationHierarchyFleetId.Value;
                else
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);

            }

            if (fleetid != -1 || fleetids != string.Empty)
            {
                this.cboVehicle.Visible = true;
                this.cboVehicle.DataSource = null;
                this.cboVehicle.DataBind();
                this.lblVehicleName.Visible = true;
                this.dgOutputs.DataSource = null;
                this.dgOutputs.DataBind();
                if (optAssignBased.SelectedItem.Value == "1" && MutipleUserHierarchyAssignment)
                    CboVehicle_MultipleFleet_Fill(fleetids);
                else
                    CboVehicle_Fill(fleetid);
            }
            else
            {
                this.cboVehicle.DataSource = null;
                this.cboVehicle.DataBind();
                this.cboVehicle.Visible = false;
                dgOutputs.DataSource = null;
                dgOutputs.DataBind();
                this.lblVehicleName.Visible = false;
            }
        }

		protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cboVehicle.SelectedIndex !=0)
			{
				dgOutputs.SelectedIndex = -1;
				dgOutputs.EditItemIndex =-1;
				dgOutputs.CurrentPageIndex=0; 
				DgOutputs_Fill();
			}
			else
			{
				dgOutputs.DataSource=null; 
				dgOutputs.DataBind(); 
			}
		}

		private void DgOutputs_Fill()
		{
					
			try
			{
				StringReader strrXML = null;
				DataSet dsOutputs=new DataSet(); 
				
				string xml = "" ;

				ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;

				if( objUtil.ErrCheck( dbv.GetVehicleOutputsXMLByLang    ( sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.ToString() ,CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml ),false ) )
                    if (objUtil.ErrCheck(dbv.GetVehicleOutputsXMLByLang(sn.UserID, sn.SecId, this.cboVehicle.SelectedItem.Value.ToString(), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,ref xml), true))
					{
						return;
					}

				if (xml == "")
				{
					return;
				}


				strrXML = new StringReader( xml ) ;
				dsOutputs.ReadXml (strrXML) ;



				// Show ActionOn
				DataColumn dc = new DataColumn();
                dc.ColumnName = "ActionOn";
				dc.DataType=Type.GetType("System.String"); 
				dc.DefaultValue=""; 
				dsOutputs.Tables[0].Columns.Add(dc); 



				// Show ActionOff
				dc = new DataColumn();
                dc.ColumnName = "ActionOff";
				dc.DataType=Type.GetType("System.String"); 
				dc.DefaultValue=""; 
				dsOutputs.Tables[0].Columns.Add(dc); 


				string[] Sen=new string [1];

				foreach(DataRow rowItem in dsOutputs.Tables[0].Rows)
				{
									
					Sen=rowItem["OutputAction"].ToString().Split('/');
					
					if (Sen[0].ToString().TrimEnd()!="*")  
						rowItem["ActionOn"]=Sen[0];
					else
						rowItem["ActionOn"]="";

					if (Sen[1].ToString().TrimEnd()!="*")  
						rowItem["ActionOff"]=Sen[1];
					else
						rowItem["ActionOff"]="";

				}

				this.dgOutputs.DataSource=dsOutputs;
				this.dgOutputs.DataBind();  
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
			this.dgOutputs.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgOutputs_PageIndexChanged);
			this.dgOutputs.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgOutputs_CancelCommand);
			this.dgOutputs.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgOutputs_EditCommand);
			this.dgOutputs.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgOutputs_UpdateCommand);
			this.dgOutputs.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgOutputs_ItemDataBound);

		}
		#endregion

		protected void dgOutputs_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgOutputs.EditItemIndex = -1;
			DgOutputs_Fill();
		}

        protected void dgOutputs_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgOutputs.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
			DgOutputs_Fill();
			dgOutputs.SelectedIndex = -1;
		}

        protected void dgOutputs_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{

			//Check security 
			bool cmd=sn.User.ControlEnable(sn,32);
			if (!cmd)
			{
				lblMessage.Visible = true;
                lblMessage.Text = (string)base.GetLocalResourceObject("NoPermissions");
				return;
			}

			dgOutputs.CurrentPageIndex = e.NewPageIndex;
			DgOutputs_Fill();
			dgOutputs.SelectedIndex = -1	;
		}

        protected void dgOutputs_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				TextBox txtOutputName;
				TextBox txtOutputActionOn;
				TextBox txtOutputActionOff;
				string OutputAction="";

				Int16 OutputID = Convert.ToInt16(dgOutputs.DataKeys[e.Item.ItemIndex]);
				txtOutputName =(TextBox) e.Item.FindControl("txtOutputName") ;
				txtOutputActionOn =(TextBox) e.Item.FindControl("txtOutputActionOn");
				txtOutputActionOff =(TextBox) e.Item.FindControl("txtOutputActionOff") ;
			
				if ((txtOutputActionOn.Text.TrimEnd() =="") && (txtOutputActionOn.Enabled==true)) 
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("dg_ActionOn");
					return;
				}

				if ((txtOutputActionOff.Text.TrimEnd()=="") && (txtOutputActionOff.Enabled==true)) 
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("dg_ActionOff"); 	
					return;
				}


				this.lblMessage.Visible=false;
				this.lblMessage.Text=""; 	


				if ((txtOutputActionOn.Text.TrimEnd()=="") && (txtOutputActionOn.Enabled==false)) 
					txtOutputActionOn.Text="*";

				if ((txtOutputActionOff.Text.TrimEnd()=="") && (txtOutputActionOff.Enabled==false)) 
					txtOutputActionOff.Text="*";

				OutputAction=txtOutputActionOn.Text.TrimEnd()  +"/"+txtOutputActionOff.Text.TrimEnd()   ;

				ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;
				

				if( objUtil.ErrCheck( dbv.UpdateOutputByLicencePlate  ( sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.TrimEnd()    ,OutputID,txtOutputName.Text.TrimEnd() ,OutputAction)  ,false ) )
					if( objUtil.ErrCheck( dbv.UpdateOutputByLicencePlate  ( sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.TrimEnd()    ,OutputID,txtOutputName.Text.TrimEnd() ,OutputAction)  ,true ) )
					{
						return;
					}


			
				dgOutputs.EditItemIndex = -1;
				DgOutputs_Fill();
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

		protected void cmdAlarms_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmAlarms.aspx") ;
		}

		private void cmdPreference_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmPreference.aspx") ;
		}

		protected void cmdVehicleInfo_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleInfo.aspx") ;
		}

		protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleFleet.aspx") ;
		}

	

		private void dgOutputs_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{

			if ((e.Item.ItemType == ListItemType.AlternatingItem ) || (e.Item.ItemType == ListItemType.Item))
			{
                e.Item.Cells[4].ToolTip = (string)base.GetLocalResourceObject("dg_ToolTip"); ;
			}

			if  ((dgOutputs.EditItemIndex!=-1) && (e.Item.ItemIndex==dgOutputs.EditItemIndex))   
			{
				TextBox txtOutputActionOn;
				TextBox txtOutputActionOff;
				txtOutputActionOn =(TextBox) e.Item.FindControl("txtOutputActionOn");
				txtOutputActionOff =(TextBox) e.Item.FindControl("txtOutputActionOff") ;

				if (txtOutputActionOn.Text.TrimEnd()=="") 
					txtOutputActionOn.Enabled=false ;

				if (txtOutputActionOff.Text.TrimEnd()=="") 
					txtOutputActionOff.Enabled=false ;
			}
		}

		private void cmdVehicleGeoZone_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleGeoZoneAss.htm") ;
		}

		protected void cmdFleets_Click(object sender, System.EventArgs e)
		{
				Response.Redirect("frmEmails.aspx"); 
		}



		protected void cmdUsers_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmUsers.aspx"); 
		}


        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx"); 
        }
        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx"); 
        }

        protected void optAssignBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optAssignBased.SelectedItem.Value == "0")
            {
                cboFleet.Visible = true;
                btnOrganizationHierarchyNodeCode.Visible = false;
                lblFleet.Text = (string)base.GetLocalResourceObject("lblFleetResource1.Text");
            }
            else
            {
                cboFleet.Visible = false;
                btnOrganizationHierarchyNodeCode.Visible = true;
                lblFleet.Text = (string)base.GetLocalResourceObject("lblOrganizationHierarchyFleet");
            }
            refillCboVehicle();

        }
}
}
