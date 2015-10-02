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
	/// Summary description for frmAlarms.
	/// </summary>
   public partial class frmAlarms : SentinelFMBasePage
	{
		
		
		protected DataSet dsAlarmSeverity=new DataSet();

        public bool MutipleUserHierarchyAssignment = false;
	
		
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
			this.dgSensors.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgSensors_PageIndexChanged);
			this.dgSensors.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSensors_CancelCommand);
			this.dgSensors.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSensors_EditCommand);
			this.dgSensors.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSensors_UpdateCommand);
			this.dgSensors.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgSensors_ItemDataBound);
			this.dgMessages.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgMessages_PageIndexChanged);
			this.dgMessages.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMessages_CancelCommand);
			this.dgMessages.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMessages_EditCommand);
			this.dgMessages.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMessages_UpdateCommand);
            this.dgMessages.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgMessages_ItemDataBound);

		}
		#endregion

        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";


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

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmAlarmsMain, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
					GuiSecurity(this); 
					this.tblHeader.Visible=false;  
					CboFleet_Fill();

					if (sn.User.DefaultFleet!=-1)
					{
						cboFleet.SelectedIndex =cboFleet.Items.IndexOf (cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
						CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
						this.lblVehicleName.Visible=true;
						this.cboVehicle.Visible=true;  
					}

				
					DsAlarmSeverity_Fill();
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


		protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
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
                this.dgSensors.DataSource = null;
                this.dgSensors.DataBind();
                this.cboVehicle.DataSource = null;
                this.cboVehicle.DataBind();
                this.dgMessages.DataSource = null;
                this.dgMessages.DataBind();
                this.cboVehicle.Visible = true;
                this.lblVehicleName.Visible = true;
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
                this.dgSensors.DataSource = null;
                this.dgSensors.DataBind();
                this.dgMessages.DataSource = null;
                this.dgMessages.DataBind();
                this.lblVehicleName.Visible = false;
                this.tblHeader.Visible = false;


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

		protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cboVehicle.SelectedIndex !=0)
			{
				dgSensors.CurrentPageIndex=0; 
				dgSensors.EditItemIndex = -1;
				dgSensors.SelectedIndex = -1;
				dgMessages.CurrentPageIndex=0; 
				dgMessages.EditItemIndex=-1;
  				dgMessages.SelectedIndex=-1;  
				DgSensors_Fill();
				DgMessage_Fill(); 
				this.tblHeader.Visible=true;  
			}
			else
			{
				this.tblHeader.Visible=false;
				this.dgSensors.DataSource=null;
				this.dgSensors.DataBind();  
				this.dgMessages.DataSource=null;
				this.dgMessages.DataBind();  
			}
		
		}

		public int GetAlarmSeverity(int AlarmSeverityId) 
		{
			try
			{
				DropDownList cboSensorActions=new DropDownList() ;
				cboSensorActions.DataValueField = "SeverityId";
				cboSensorActions.DataTextField = "SeverityName";
				DsAlarmSeverity_Fill();
				cboSensorActions.DataSource = dsAlarmSeverity;
				cboSensorActions.DataBind();

				cboSensorActions.SelectedIndex = -1;
				cboSensorActions.Items.FindByValue(AlarmSeverityId.ToString()).Selected = true;
				return cboSensorActions.SelectedIndex;
			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
				return 0;
			}

		}

	
		private void DgSensors_Fill()
		{
			try
			{
				StringReader strrXML = null;
				DataSet dsSensor=new DataSet(); 
				
				string xml = "" ;
				ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;

				if( objUtil.ErrCheck( dbv.GetVehicleSensorsXMLByLang  ( sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.ToString() , CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml ),false ) )
                    if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, this.cboVehicle.SelectedItem.Value.ToString(), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,ref xml), true))
					{
						return;
					}

				if (xml == "")
				{
					return;
				}


				strrXML = new StringReader( xml ) ;
				dsSensor.ReadXml (strrXML) ;

				// Show SevirityNameOn
				DataColumn dcOn = new DataColumn();
				dcOn.ColumnName = "SeverityNameOn";
				dcOn.DataType=Type.GetType("System.String"); 
				dcOn.DefaultValue=""; 
				dsSensor.Tables[0].Columns.Add(dcOn); 



				// Show SevirityNameOff
				DataColumn dcOff = new DataColumn();
				dcOff.ColumnName = "SeverityNameOff";
				dcOff.DataType=Type.GetType("System.String"); 
				dcOff.DefaultValue=""; 
				dsSensor.Tables[0].Columns.Add(dcOff); 


				// Show ActionOn
				DataColumn dc = new DataColumn();
				dc.ColumnName = "ActionOn";
				dc.DataType=Type.GetType("System.String"); 
				dc.DefaultValue=""; 
				dsSensor.Tables[0].Columns.Add(dc); 



				// Show ActionOff
				dc = new DataColumn();
				dc.ColumnName = "ActionOff";
				dc.DataType=Type.GetType("System.String"); 
				dc.DefaultValue=""; 
				dsSensor.Tables[0].Columns.Add(dc); 


				short enumId = 0;
				string[] Sen=new string [1];
                
				foreach(DataRow rowItem in dsSensor.Tables[0].Rows)
				{
					enumId = Convert.ToInt16(rowItem["AlarmLevelOn"]);
					//rowItem["SeverityNameOn"] = Enum.GetName(typeof(Enums.AlarmSeverity),(Enums.AlarmSeverity)enumId);
                    rowItem["SeverityNameOn"] = sn.Misc.DsAlarmSeverity.Tables[0].Rows[enumId][1].ToString();    
					enumId = Convert.ToInt16(rowItem["AlarmLevelOff"]);
					//rowItem["SeverityNameOff"] = Enum.GetName(typeof(Enums.AlarmSeverity),(Enums.AlarmSeverity)enumId);
                    rowItem["SeverityNameOff"] = sn.Misc.DsAlarmSeverity.Tables[0].Rows[enumId][1].ToString();    
                    if (rowItem["SensorAction"].ToString()  != "")
                    {
                        Sen = rowItem["SensorAction"].ToString().Split('/');
                        rowItem["ActionOn"] = Sen[0];
                        rowItem["ActionOff"] = Sen[1];
                    }
				
				}


			
				this.dgSensors.DataSource=dsSensor;
				this.dgSensors.DataBind(); 
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


		private void DgMessage_Fill()
		{
			try
			{
				StringReader strrXML = null;
				DataSet dsMessages=new DataSet(); 
				
				string xml = "" ;
				ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;

				int BoxId=0;
				dbv.GetBoxIDByLicensePlate(sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.ToString(),ref BoxId); 
				ViewState["BoxId"]=BoxId;

                if (objUtil.ErrCheck(dbv.GetAllSupportedMessagesByBoxIdByLang(sn.UserID, sn.SecId, BoxId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetAllSupportedMessagesByBoxIdByLang(sn.UserID, sn.SecId, BoxId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,  ref xml), true))
					{
                  this.dgMessages.DataSource = null;
                  this.dgMessages.DataBind();
						return;
					}

				if (xml == "")
				{
               this.dgMessages.DataSource = null;
               this.dgMessages.DataBind();
					return;
				}


				strrXML = new StringReader( xml ) ;
				dsMessages.ReadXml (strrXML) ;

				
				// Show SevirityName
				DataColumn dc = new DataColumn();
				dc.ColumnName = "SeverityName";
				dc.DataType=Type.GetType("System.String"); 
				dc.DefaultValue=""; 
				dsMessages.Tables[0].Columns.Add(dc); 

				foreach(DataRow rowItem in dsMessages.Tables[0].Rows)
				{
					try
					{
						//DsAlarmSeverity_Fill();
						Int16 enumId = Convert.ToInt16(rowItem["AlarmLevel"]);
						//rowItem["SeverityName"] = Enum.GetName(typeof(Enums.AlarmSeverity),(Enums.AlarmSeverity)enumId);
                        rowItem["SeverityName"] = sn.Misc.DsAlarmSeverity.Tables[0].Rows[enumId][1].ToString();    
					}
					catch
					{
					}

				}

				this.dgMessages.DataSource=dsMessages;
				this.dgMessages.DataBind();
  
				//this.tblSpeed.Visible=false;  
			
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

		private void DsAlarmSeverity_Fill()
		{
			try
			{
                //dsAlarmSeverity.Tables.Clear(); 
                //DataTable tblAlarmSeverity = dsAlarmSeverity.Tables.Add("AlarmSeverity") ;
                //tblAlarmSeverity.Columns.Add("SeverityId", typeof( short ) ) ;
                //tblAlarmSeverity.Columns.Add("SeverityName", typeof( string ) ) ;

                //Array enmArr = Enum.GetValues(typeof(Enums.AlarmSeverity));
                //string AlarmSeverity;
                //object[] objRow;
                //foreach(Enums.AlarmSeverity ittr in enmArr)
                //{
                //    AlarmSeverity = Enum.GetName(typeof(Enums.AlarmSeverity),ittr);
                //    objRow = new object[2] ;
                //    objRow[0] = Convert.ToInt16(ittr);
                //    objRow[1] =AlarmSeverity;
                //    dsAlarmSeverity.Tables[0].Rows.Add(objRow)   ;

				//}



                StringReader strrXML = null;
                dsAlarmSeverity.Tables.Clear(); 
                
                string xml = "";
                ServerAlarms.Alarms dba = new ServerAlarms.Alarms();



                if (objUtil.ErrCheck(dba.GetAlarmSeverityXMLByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dba.GetAlarmSeverityXMLByLang(sn.UserID, sn.SecId,  CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }


                strrXML = new StringReader(xml);
                dsAlarmSeverity.ReadXml(strrXML);
                sn.Misc.DsAlarmSeverity = dsAlarmSeverity; 
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

		protected  void dgSensors_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{

				//Check security 
				bool cmd=sn.User.ControlEnable(sn,31);
				if (!cmd)
				{
					lblMessage.Visible = true;
					lblMessage.Text = "You don't have appropriate permission to perform this operation.";
					return;
				}


				dgSensors.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
				DgSensors_Fill();
				dgSensors.SelectedIndex = -1;
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

		protected  void dgSensors_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				this.lblMessage.Text="";  
				dgSensors.EditItemIndex = -1;
				DgSensors_Fill();
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

        protected void dgSensors_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				TextBox txtSensorName;
				TextBox txtSensorActionOn;
				TextBox txtSensorActionOff;
				DropDownList alarmLevelOn; 
				DropDownList alarmLevelOff; 
				string SensorAction="";

				Int16 SensorID = Convert.ToInt16(dgSensors.DataKeys[e.Item.ItemIndex]);
				txtSensorName =(TextBox) e.Item.FindControl("txtSensorName") ;
				txtSensorActionOn =(TextBox) e.Item.FindControl("txtSensorActionOn");
				txtSensorActionOff =(TextBox) e.Item.FindControl("txtSensorActionOff") ;
				alarmLevelOn =(DropDownList) e.Item.FindControl("cboSensorActionsOn") ;
				alarmLevelOff =(DropDownList) e.Item.FindControl("cboSensorActionsOff") ;

				this.lblMessage.Visible=true;  
				if (txtSensorActionOn.Text=="")
				{
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("ActionOnReqErrMsg");
                   
					return;
				}

				if (txtSensorActionOff.Text=="")
				{
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("ActionOffReqErrMsg");
					return;
				}

				this.lblMessage.Text="";  
				this.lblMessage.Visible=false;  

				SensorAction=txtSensorActionOn.Text.TrimEnd()  +"/"+txtSensorActionOff.Text.TrimEnd()   ;

				ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;
				

				if( objUtil.ErrCheck( dbv.UpdateSensorByLicencePlate( sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.TrimEnd()    , SensorID,txtSensorName.Text.TrimEnd() ,SensorAction ,Convert.ToInt16(alarmLevelOn.SelectedItem.Value ),Convert.ToInt16(alarmLevelOff.SelectedItem.Value )) ,false ) )
				  if( objUtil.ErrCheck( dbv.UpdateSensorByLicencePlate( sn.UserID , sn.SecId , this.cboVehicle.SelectedItem.Value.TrimEnd()    , SensorID,txtSensorName.Text.TrimEnd() ,SensorAction ,Convert.ToInt16(alarmLevelOn.SelectedItem.Value ),Convert.ToInt16(alarmLevelOff.SelectedItem.Value )) ,true ) )
					{
						return;
					}


			
				dgSensors.EditItemIndex = -1;
				DgSensors_Fill();
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

		protected  void dgSensors_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			try
			{
				dgSensors.CurrentPageIndex = e.NewPageIndex;
				DgSensors_Fill();
				dgSensors.SelectedIndex = -1	;
			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		private void cmdPreference_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmPreference.aspx") ;
		}

	
		protected void cmdOutputs_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmOutputs.aspx"); 
		}

		protected void cmdVehicleInfo_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleInfo.aspx") ;
		}

		protected void cmdFleetVehicle_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmVehicleFleet.aspx") ;
		}

		
	
		private void dgSensors_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if ((e.Item.ItemType == ListItemType.AlternatingItem ) || (e.Item.ItemType == ListItemType.Item))
			{
                e.Item.Cells[6].ToolTip = (string)base.GetLocalResourceObject("ToolTipEdit");;
			}
		}

        private void dgMessages_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[3].ToolTip = (string)base.GetLocalResourceObject("ToolTipEdit");
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
		

		
		protected void cmdVehicles_Click(object sender, System.EventArgs e)
		{
		
		}

		protected void cmdUsers_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmUsers.aspx"); 
		}

		protected  void dgMessages_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			try
			{
				dgMessages.CurrentPageIndex = e.NewPageIndex;
				DgMessage_Fill();
				dgMessages.SelectedIndex = -1	;
			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

        protected void dgMessages_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{

				dgMessages.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
                DgMessage_Fill();
                
				dgMessages.SelectedIndex = -1;
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

		protected  void dgMessages_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle() ;
			

			DropDownList alarmLevel; 

			Int16 MessageTypeID = Convert.ToInt16(dgMessages.DataKeys[e.Item.ItemIndex]);
			alarmLevel =(DropDownList) e.Item.FindControl("cboMessageSeverity") ;


			if( objUtil.ErrCheck( dbv.UpdateMsgSeverity ( sn.UserID , sn.SecId ,Convert.ToInt32(ViewState["BoxId"]),MessageTypeID,Convert.ToInt16(alarmLevel.SelectedItem.Value)) ,false ) )
				if( objUtil.ErrCheck( dbv.UpdateMsgSeverity ( sn.UserID , sn.SecId ,Convert.ToInt32(ViewState["BoxId"]),MessageTypeID,Convert.ToInt16(alarmLevel.SelectedItem.Value)) ,false ) )
				{
					return;
				}


			dgMessages.EditItemIndex = -1;
			DgMessage_Fill ();
		}

		protected  void dgMessages_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				this.lblMessage.Text="";  
				dgMessages.EditItemIndex = -1;
				DgMessage_Fill();
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
