using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
	/// Summary description for frmMessages.
	/// </summary>
	public partial class frmMessages : SentinelFMBasePage
	{

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
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

                    if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                    {
                        string d = rowItem["PreferenceValue"].ToString().ToLower();
                        if (d == "hierarchy")
                            LoadVehiclesBasedOn = "hierarchy";
                        else
                            LoadVehiclesBasedOn = "fleet";
                    }
                }

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                    LoadVehiclesBasedOn = "fleet";
                }

                if (LoadVehiclesBasedOn == "hierarchy")
                {
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
                        OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                    }
                    else
                    {
                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }

                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == string.Empty ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    lblFleetTitle.Visible = false;
                    cboFleet.Visible = false;
                    valFleet.Enabled = false;
                    lblOhTitle.Visible = true;
                    btnOrganizationHierarchyNodeCode.Visible = true;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                }

                this.txtFrom.Text = Request[this.txtFrom.UniqueID];
                this.txtTo.Text = Request[this.txtTo.UniqueID];   



				if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMessagesForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
					GuiSecurity(this); 
					this.tblNoData.Visible=false;  
                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);
					CboFleet_Fill();

				
					
                //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToShortDateString() ;
                //sn.Message.FromDate=DateTime.Now.AddDays(-1).ToShortDateString() ;

                //this.txtTo.Text=DateTime.Now.AddDays(1).ToShortDateString();
                //sn.Message.ToDate=DateTime.Now.AddDays(1).ToShortDateString();


                    //this.txtFrom.Text=DateTime.Now.AddHours(-12).ToShortDateString() ;
                    this.txtFrom.Text = DateTime.Now.AddHours(-12).ToString("MM/dd/yyyy");
                    sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

                    //this.txtTo.Text = DateTime.Now.ToShortDateString();
                    this.txtTo.Text = DateTime.Now.ToString("MM/dd/yyyy");
                    sn.Message.ToDate =  DateTime.Now.ToShortDateString();


                    //this.cboHoursFrom.SelectedIndex = -1;
                    //    for (int i=0;i<=cboHoursFrom.Items.Count-1;i++)
                    //    {
                    //        if (Convert.ToInt32(cboHoursFrom.Items[i].Value) ==8)
                    //        {
                    //            cboHoursFrom.Items[i].Selected = true;
                    //            sn.History.FromHours=Convert.ToString(8);
                    //            break;
                    //        }
                    //    }

                    //this.cboHoursTo.SelectedIndex = -1;
                    //    for (int i=0;i<=cboHoursTo.Items.Count-1;i++)
                    //    {
                    //        if (cboHoursTo.Items[i].Value == DateTime.Now.AddHours(1).Hour.ToString())
                    //        {
                    //            cboHoursTo.Items[i].Selected = true;
                    //            break;
                    //        }
                    //    }




                    this.cboHoursFrom.SelectedIndex = -1;
                    for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                    {
                        if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-12).Hour    )
                        {
                            cboHoursFrom.Items[i].Selected = true;
                            sn.History.FromHours = DateTime.Now.AddHours(-12).Hour.ToString() ; 
                            break;
                        }
                    }

                    this.cboHoursTo.SelectedIndex = -1;
                    for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                    {
                        if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                        {
                            cboHoursTo.Items[i].Selected = true;
                            sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString() ;  
                            break;
                        }
                    }



					if (sn.Message.FleetId!=0 )
					{
						this.cboFleet.SelectedIndex = -1;
						for (int i=0;i<=cboFleet.Items.Count-1;i++)
						{
							if (cboFleet.Items[i].Value == sn.Message.FleetId.ToString())
								cboFleet.Items[i].Selected = true;
						}
					}

					else
					{
                        if (LoadVehiclesBasedOn == "hierarchy")
                        {
                            if (DefaultOrganizationHierarchyFleetId > 0)
                            {
                                CboVehicle_Fill(DefaultOrganizationHierarchyFleetId);
                                this.lblVehicleName.Visible = true;
                                this.cboVehicle.Visible = true; 
                            }
                        }
                        else if (sn.User.DefaultFleet != -1)
						{
							cboFleet.SelectedIndex =cboFleet.Items.IndexOf (cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
							CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
							this.lblVehicleName.Visible=true;
							this.cboVehicle.Visible=true;  
						}
					}


					if (Convert.ToInt32(cboFleet.SelectedItem.Value) !=-1 )
					{

						CboVehicle_Fill(Convert.ToInt32(sn.Message.FleetId));

						this.cboVehicle.SelectedIndex = -1;
					
						for (int i=0;i<=cboVehicle.Items.Count-1;i++)
						{
							if (cboVehicle.Items[i].Value == sn.Message.BoxId.ToString())
								cboVehicle.Items[i].Selected = true;
						}

						this.lblVehicleName.Visible=true;
						this.cboVehicle.Visible=true;  


						SetHours();	
						SetDirection();


						sn.Message.FromDate=Convert.ToDateTime(sn.Message.FromDate).ToShortDateString()+" "+sn.Message.FromHours;    
						sn.Message.ToDate =Convert.ToDateTime(sn.Message.ToDate).ToShortDateString()+" "+sn.Message.ToHours;
                        dgMessages_Fill_NewTZ();
					
					}

               this.tblSchButtons.Visible = false;
               this.tblMDTFormMessages.Visible = false;
               this.tblAlarms.Visible = false;  
				}
			}

			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}
		}

		protected void cmdNewMessage_Click(object sender, System.EventArgs e)
		{
			string strUrl="<script language='javascript'> function NewWindow(mypage) { ";
			strUrl=strUrl+"	var myname='Message';"; 
			strUrl=strUrl+" var w=560;";
			strUrl=strUrl+" var h=520;";
			strUrl=strUrl+" var winl = (screen.width - w) / 2; ";
			strUrl=strUrl+" var wint = (screen.height - h) / 2; ";
			strUrl=strUrl+" winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
			strUrl=strUrl+" win = window.open(mypage, myname, winprops);} " ;
			
			strUrl=strUrl+" NewWindow('frmNewMessageMain.aspx');</script>";
			Response.Write(strUrl) ;
		}
		
      // Changes for TimeZone Feature start
        private void dgMessages_Fill_NewTZ()
        {
            try
            {


                StringReader strrXML = null;

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
                CultureInfo ci = new CultureInfo("en-US");

                string strFromDT = sn.Message.FromDate;
                string strToDT = sn.Message.ToDate;
                string xml = "";

                if ((Convert.ToInt32(this.cboFleet.SelectedItem.Value) != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
                {
                    if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            this.tblNoData.Visible = true;
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                        {
                            this.tblNoData.Visible = true;
                            return;
                        }
                }

                if (xml == "")
                {
                    this.dgMessages.Visible = false;
                    this.tblNoData.Visible = true;
                    return;
                }

                strrXML = new StringReader(xml);
                this.tblNoData.Visible = false;
                this.dgMessages.Visible = true;
                dgSched.Visible = false;

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);


                // Show Combobox
                DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                ds.Tables[0].Columns.Add(dc);

                DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
                MsgKey.DefaultValue = "";
                ds.Tables[0].Columns.Add(MsgKey);

                if (ds.Tables[0].Columns.IndexOf("To") == -1)
                {
                    DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                    colTo.DefaultValue = "";
                    ds.Tables[0].Columns.Add(colTo);
                }

                foreach (DataRow rowItem in ds.Tables[0].Rows)
                {
                    rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
                    rowItem["MsgDateTime"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);
                }

                this.dgMessages.DataSource = ds;
                this.dgMessages.DataBind();
                sn.Message.DsHistoryMessages = ds;


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
     // Changes for TimeZone Feature end


		private void dgMessages_Fill()
		{
			try
			{

			
			StringReader strrXML = null;

			ServerDBHistory.DBHistory  hist = new ServerDBHistory.DBHistory() ;
            CultureInfo ci = new CultureInfo("en-US");

			string strFromDT=sn.Message.FromDate;
			string strToDT=sn.Message.ToDate;
			string xml="";

				if ((Convert.ToInt32(this.cboFleet.SelectedItem.Value)!=-1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value)==0))
				{
                    if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
						{
                            this.tblNoData.Visible = true; 
							return;
						}
				}
				else
				{
                    if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                        if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
						{
                            this.tblNoData.Visible = true; 
							return;
						}
				}
				
			if (xml == "")
			{
				this.dgMessages.Visible=false;
				this.tblNoData.Visible=true;  
				return;
			}
				
			strrXML = new StringReader( xml ) ;
			this.tblNoData.Visible=false;  
			this.dgMessages.Visible=true;
         dgSched.Visible = false; 

			DataSet ds=new DataSet();
			ds.ReadXml(strrXML);


         // Show Combobox
         DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
         dc.DefaultValue = false;
         ds.Tables[0].Columns.Add(dc);

			DataColumn MsgKey = new DataColumn("MsgKey",Type.GetType("System.String"));
			MsgKey.DefaultValue=""; 
			ds.Tables[0].Columns.Add(MsgKey);

			if (ds.Tables[0].Columns.IndexOf("To")==-1 )
			{
					DataColumn colTo= new DataColumn("To",Type.GetType("System.String"));
					colTo.DefaultValue=""; 
					ds.Tables[0].Columns.Add(colTo);
			}

			foreach(DataRow rowItem in ds.Tables[0].Rows)
			{
				rowItem["MsgKey"]=rowItem["MsgId"].ToString()+";"+ rowItem["Vehicleid"].ToString();
				rowItem["MsgDateTime"]=Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci) ;
			}

			this.dgMessages.DataSource=ds;
			this.dgMessages.DataBind();
         sn.Message.DsHistoryMessages = ds;  
       

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

				if( objUtil.ErrCheck( dbf.GetFleetsInfoXMLByUserIdByLang( sn.UserID , sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName , ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetFleetsInfoXMLByUserIdByLang( sn.UserID , sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml ), true ) )
					{
						cboFleet.DataSource = null;
						return;
					}
				strrXML = new StringReader( xml ) ;
				dsFleets.ReadXml (strrXML) ;

				cboFleet.DataSource=dsFleets  ;
				cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
  
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
                    this.cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
					return;
				}

				strrXML = new StringReader( xml ) ;
				dsVehicle.ReadXml(strrXML);
			
				cboVehicle.DataSource =dsVehicle;
				cboVehicle.DataBind();

				cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"),  "0"));

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

		private void dgMessages_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if ((e.Item.ItemType == ListItemType.AlternatingItem ) || (e.Item.ItemType == ListItemType.Item))
			{
				if (e.Item.Cells[4].Text.Length>57) 
				{
					e.Item.Cells[4].ToolTip =e.Item.Cells[4].Text;
					e.Item.Cells[4].Text=e.Item.Cells[4].Text.Substring(0,57)+"...";  
				}
				else
				{
					e.Item.Cells[4].ToolTip =e.Item.Cells[4].Text;
				}

				if (e.Item.Cells[5].Text.TrimEnd().Length>40) 
				{
					e.Item.Cells[5].ToolTip =e.Item.Cells[5].Text.TrimEnd() ;
					e.Item.Cells[5].Text=e.Item.Cells[5].Text.TrimEnd().Substring(0,40)+"...";  
				}
				else
				{
					e.Item.Cells[5].ToolTip =e.Item.Cells[5].Text.TrimEnd() ;
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
			this.dgMessages.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgMessages_PageIndexChanged);

		}
		#endregion

		protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboFleet.SelectedItem.Value)!=-1)
			{
				this.cboVehicle.Visible=true;  
				this.lblVehicleName.Visible=true;  
				CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
			}
		}

		protected void cmdShowMessages_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strFromDate="";
				string strToDate="";
				 dgMessages.SelectedIndex = -1	;
				 dgMessages.CurrentPageIndex=0;


                 if (this.chkAuto.Checked)
                 {

                     this.txtFrom.Text = DateTime.Now.AddHours(-12).ToShortDateString();
                     sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

                     this.txtTo.Text = DateTime.Now.ToShortDateString();
                     sn.Message.ToDate = DateTime.Now.ToShortDateString();

                     this.cboHoursFrom.SelectedIndex = -1;
                     for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                     {
                         if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-12).Hour)
                         {
                             cboHoursFrom.Items[i].Selected = true;
                             sn.History.FromHours = DateTime.Now.AddHours(-12).Hour.ToString();
                             break;
                         }
                     }

                     this.cboHoursTo.SelectedIndex = -1;
                     for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                     {
                         if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                         {
                             cboHoursTo.Items[i].Selected = true;
                             sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();
                             break;
                         }
                     }



                     if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                         strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                     if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                         strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                     if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                         strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                     if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                         strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                     if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                         strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                     if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                         strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";



                 }

                 else
                 {
                     if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                         strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                     if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                         strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                     if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                         strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                     if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                         strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                     if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                         strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                     if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                         strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";

                    
                 }

			
				this.lblMessage.Text="";
            CultureInfo ci = new CultureInfo("en-US");

				if (Convert.ToDateTime(strFromDate, ci)>Convert.ToDateTime(strToDate, ci))
				{
					this.lblMessage.Visible=true;
					this.lblMessage.Text=(string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate")  ;
					return;
				}
				else
				{
					this.lblMessage.Visible=true;
					this.lblMessage.Text=""  ;
				}

				
				sn.Message.FromDate=strFromDate;
				sn.Message.ToDate=strToDate;
				sn.Message.FromHours=this.cboHoursFrom.SelectedItem.Value;
				sn.Message.ToHours=this.cboHoursTo.SelectedItem.Value;
				sn.Message.DsMessages=null;
				sn.Message.VehicleName=this.cboVehicle.SelectedItem.Text.Replace("'","''");
				sn.Message.FleetName=this.cboFleet.SelectedItem.Text.Replace("'","''");  	
				sn.Message.BoxId=Convert.ToInt32(this.cboVehicle.SelectedItem.Value);     
			
				SetDirection();

				dgMessages_Fill_NewTZ();
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}
		}

		
		private void SetDirection()
		{
			if (Convert.ToInt16(this.cboDirection.SelectedItem.Value)==0)
				sn.Message.MessageDirectionId=Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Both);

			if (Convert.ToInt16(this.cboDirection.SelectedItem.Value)==1)
				sn.Message.MessageDirectionId=Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.In);    
				
			if (Convert.ToInt16(this.cboDirection.SelectedItem.Value)==2)
				sn.Message.MessageDirectionId=Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Out);    

		}
	


		private void SetHours()
		{
			if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)<12)
				sn.Message.FromHours=this.cboHoursFrom.SelectedItem.Value+ ":00 AM"  ;

			if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)==12)
				sn.Message.FromHours=this.cboHoursFrom.SelectedItem.Value+ ":00 PM"  ;


			if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)>12)
				sn.Message.FromHours=Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)-12)+ ":00 PM"  ;

			if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)<12)
				sn.Message.ToHours=this.cboHoursTo.SelectedItem.Value+ ":00 AM";

			if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)==12)
				sn.Message.ToHours=this.cboHoursTo.SelectedItem.Value+ ":00 PM";


			if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)>12)
				sn.Message.ToHours=Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)-12)+ ":00 PM"  ;
		}
		

        protected void dgMessages_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			 dgMessages.CurrentPageIndex = e.NewPageIndex;
          dgMessages.DataSource = sn.Message.DsHistoryMessages;
          dgMessages.DataBind(); 
			 dgMessages.SelectedIndex = -1	;
		}

		protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

        protected void cmdViewNewMessages_Click(object sender, EventArgs e)
        {
            try
            {
                string strFromDate = "";
                string strToDate = "";
                dgMessages.SelectedIndex = -1;
                dgMessages.CurrentPageIndex = 0;



                this.txtFrom.Text = DateTime.Now.AddHours(-12).ToShortDateString();
                sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

                this.txtTo.Text = DateTime.Now.ToShortDateString();
                sn.Message.ToDate = DateTime.Now.ToShortDateString();





                this.cboHoursFrom.SelectedIndex = -1;
                for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                {
                    if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-12).Hour)
                    {
                        cboHoursFrom.Items[i].Selected = true;
                        sn.History.FromHours = DateTime.Now.AddHours(-12).Hour.ToString();
                        break;
                    }
                }

                this.cboHoursTo.SelectedIndex = -1;
                for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                {
                    if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                    {
                        cboHoursTo.Items[i].Selected = true;
                        sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();
                        break;
                    }
                }






                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";



                this.lblMessage.Text = "";

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "";
                }


                sn.Message.FromDate = strFromDate;
                sn.Message.ToDate = strToDate;
                sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value;
                sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value;
                sn.Message.DsMessages = null;
                sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                sn.Message.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                sn.Message.BoxId = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);

                SetDirection();

                dgMessages_Fill_NewTZ();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
        }

      protected void optMessageType_SelectedIndexChanged(object sender, EventArgs e)
      {
         dgFormMessages.SelectedIndex = -1;
         dgSched.SelectedIndex = -1;
         dgMessages.SelectedIndex = -1;
         dgAlarms.SelectedIndex = -1;  

         dgFormMessages.CurrentPageIndex =0;
         dgSched.CurrentPageIndex = 0;
         dgMessages.CurrentPageIndex = 0;
         dgAlarms.CurrentPageIndex = 0;
  
         FleetVehicleOption(true); 

         switch (optMessageType.SelectedIndex)
         {
            case 0:
               this.lblFolderListTitle.Visible = true;
               this.cboDirection.Visible = true;
               this.tblMsgButtons.Visible = true;
               this.tblSchButtons.Visible = false;
               this.dgMessages.Visible = false;
               this.dgSched.Visible = false;
               this.dgFormMessages.Visible = false;
               this.dgAlarms.Visible = false;  
               this.tblMDTFormMessages.Visible = false;
               this.lblMDTForm.Visible = false;
               this.cboForms.Visible = false;
               this.cboScheduledMessageFilter.Visible = false;
               this.lblScheduledMessageFilter.Visible = false;
               tblAlarms.Visible = false ;  
               break;

            case 1:
               this.lblFolderListTitle.Visible = false;
               this.cboDirection.Visible = false;
               this.tblMsgButtons.Visible = false;
               this.tblSchButtons.Visible = false;
               this.dgMessages.Visible = false;
               this.dgSched.Visible = false;
               this.dgAlarms.Visible = false;
               this.dgFormMessages.Visible = false;
               this.tblMDTFormMessages.Visible = false;
               this.lblMDTForm.Visible = false;
               this.cboForms.Visible = false;
               this.cboScheduledMessageFilter.Visible = false;
               this.lblScheduledMessageFilter.Visible = false;
               tblAlarms.Visible = true;
               FleetVehicleOption(false);
               break;
            case 2:
               this.lblFolderListTitle.Visible = false;
               this.cboDirection.Visible = false;
               this.tblMsgButtons.Visible = false;
               this.tblSchButtons.Visible = true;
               this.tblMDTFormMessages.Visible = false;
               this.dgMessages.Visible = false;
               this.dgFormMessages.Visible = false;
               this.dgAlarms.Visible = false;
               this.dgSched.Visible = false;
               this.lblMDTForm.Visible = false;
               this.cboForms.Visible = false;
               this.cboScheduledMessageFilter.Visible = true;
               this.lblScheduledMessageFilter.Visible = true;
               tblAlarms.Visible = false;  
               break;
            case 3:
               this.lblFolderListTitle.Visible = false;
               this.cboDirection.Visible = false;
               this.tblMsgButtons.Visible = false;
               this.tblSchButtons.Visible = false;
               this.tblMDTFormMessages.Visible = true;
               this.dgMessages.Visible = false;
               this.dgSched.Visible = false;
               this.dgAlarms.Visible = false;
               this.dgFormMessages.Visible = false;
               this.lblMDTForm.Visible = true;
               this.cboForms.Visible = true;
               this.cboScheduledMessageFilter.Visible = false;
               this.lblScheduledMessageFilter.Visible = false;
               tblAlarms.Visible = false;  
               break;

         }
      }
      protected void cmdScheduledTasks_Click(object sender, EventArgs e)
      {
         try
         {
            dgSched.CurrentPageIndex = 0;
            dgSched_Fill();

         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }

      }

      private void dgSched_Fill()
      {
         string strFromDate = "";
         string strToDate = "";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
            strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
            strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";



          
         this.lblMessage.Text = "";
         CultureInfo ci = new CultureInfo("en-US");

         if (Convert.ToDateTime(strFromDate, ci) > Convert.ToDateTime(strToDate, ci))
         {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate");
            return;
         }
         else
         {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = "";
         }




         DataSet dsSched = new DataSet();
         
         StringReader strrXML = null;


         string xml = "";
         ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

         if (objUtil.ErrCheck(dbs.GetSheduledTasksHistory(sn.UserID, sn.SecId, strFromDate, strToDate, Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), false))
            if (objUtil.ErrCheck(dbs.GetSheduledTasksHistory(sn.UserID, sn.SecId, strFromDate, strToDate, Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), true))
            {
               dgSched.DataSource = null;
               dgSched.DataBind();
               tblNoData.Visible = true;
               return;
            }
         if (xml == "")
         {
            tblNoData.Visible = true;
            dgSched.DataSource = null;
            dgSched.DataBind();
            return;
         }
         
         strrXML = new StringReader(xml);
         dsSched.ReadXml(strrXML);

        

         if (cboScheduledMessageFilter.SelectedItem.Value == "2")
         {
            DataTable dt = new DataTable();
            dt=dsSched.Tables[0].Clone();  
            DataRow[] drCollections = null;
            drCollections = dsSched.Tables[0].Select("MsgOutDateTime<>''", "", DataViewRowState.CurrentRows);
            foreach (DataRow dr in drCollections)
               dt.ImportRow(dr); 

            dgSched.DataSource = dt;
            
         }
         else
         {
            dgSched.DataSource = dsSched;
         }

         dgSched.DataBind();
        

         tblNoData.Visible = false;
         dgMessages.Visible = false;
         dgSched.Visible = true; 
      }

      protected void cmdFormMessages_Click(object sender, EventArgs e)
      {
         try
         {
            dgFormMessages_Fill();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }

      }

      private void dgFormMessages_Fill()
      {
         string strFromDate = "";
         string strToDate = "";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
            strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
            strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";




         this.lblMessage.Text = "";
         CultureInfo ci = new CultureInfo("en-US");

         if (Convert.ToDateTime(strFromDate, ci) > Convert.ToDateTime(strToDate, ci))
         {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate");
            return;
         }
         else
         {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = "";
         }




         DataSet dsMessages = new DataSet();
         
         StringReader strrXML = null;



         int FormId = Convert.ToInt32(this.cboForms.SelectedItem.Value);
         string xml = "";
         ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();

         //if (objUtil.ErrCheck(dbh.GetMDTFormsMessages(sn.UserID, sn.SecId, strFromDate, strToDate, Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToInt32(this.cboVehicle.SelectedItem.Value),FormId, ref xml), false))
         //   if (objUtil.ErrCheck(dbh.GetMDTFormsMessages(sn.UserID, sn.SecId, strFromDate, strToDate, Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToInt32(this.cboVehicle.SelectedItem.Value),FormId, ref xml), false))
         //   {
         //      dgFormMessages.DataSource = null;
         //      dgFormMessages.DataBind();
         //      tblNoData.Visible = true;
         //      return;
         //   }
         //if (xml == "")
         //{
         //   tblNoData.Visible = true;
         //   dgFormMessages.DataSource = null;
         //   dgFormMessages.DataBind();
         //   return;
         //}

         strrXML = new StringReader(xml);
         dsMessages.ReadXml(strrXML);

        
        
         string FormSchema="";
         //ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
         // if (objUtil.ErrCheck(dbo.GetMDTFormSchema(sn.UserID, sn.SecId,sn.User.OrganizationId,FormId,ref FormSchema), false))
         //    if (objUtil.ErrCheck(dbo.GetMDTFormSchema(sn.UserID, sn.SecId, sn.User.OrganizationId, FormId, ref FormSchema), true))
         //    {
         //       dgFormMessages.DataSource = null;
         //       dgFormMessages.DataBind();
         //       tblNoData.Visible = true;
         //       return;
         //    }


         DataSet dsDgFormMessagesShema = new DataSet();
         strrXML = new StringReader(FormSchema);
         dsDgFormMessagesShema.ReadXmlSchema(strrXML);



         foreach (DataRow drMsg in dsMessages.Tables[0].Rows)
         {
            string[] arrVal = drMsg["CustomProp"].ToString().Split('\n');  
            DataRow dr = dsDgFormMessagesShema.Tables[0].NewRow();
            for (int i = 1; i < dsDgFormMessagesShema.Tables[0].Columns.Count ; i++)
               dr[i-1] = arrVal[i];

            dsDgFormMessagesShema.Tables[0].Rows.Add(dr); 
         }


         dgFormMessages.DataSource = dsDgFormMessagesShema;
         dgFormMessages.DataBind();
         tblNoData.Visible = false;
         dgMessages.Visible = false;
         dgSched.Visible = false;
         dgFormMessages.Visible = true; 
      }

      protected void dgSched_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         try
         {
            dgSched.CurrentPageIndex = e.NewPageIndex;
            dgSched_Fill();
            dgSched.SelectedIndex = -1;

            
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }
      protected void dgFormMessages_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         try
         {
            dgFormMessages.CurrentPageIndex = e.NewPageIndex;
            dgFormMessages_Fill() ;
            dgFormMessages.SelectedIndex = -1;


         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      private void SaveMessagesCheckBoxes()
      {

         for (int i = 0; i < dgMessages.Items.Count; i++)
         {
            CheckBox ch = (CheckBox)(dgMessages.Items[i].Cells[1].Controls[1]);

            foreach (DataRow rowItem in sn.Message.DsHistoryMessages.Tables[0].Rows)
            {
               if (dgMessages.Items[i].Cells[0].Text.ToString() == rowItem["MsgId"].ToString())
                  rowItem["chkBox"] = ch.Checked;
            }
         }

      }


      private void SaveAlarmsCheckBoxes()
      {

         for (int i = 0; i < dgAlarms.Items.Count; i++)
         {
            CheckBox ch = (CheckBox)(dgAlarms.Items[i].Cells[1].Controls[1]);

            foreach (DataRow rowItem in sn.Message.DsHistoryAlarms.Tables[0].Rows)
            {
               if (dgAlarms.Items[i].Cells[0].Text.ToString() == rowItem["AlarmId"].ToString())
                  rowItem["chkBox"] = ch.Checked;
            }
         }

      }

      protected void cmdMarkAsRead_Click(object sender, EventArgs e)
      {
         try
         {
         
         DataSet ds = new DataSet();
         ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();


         SaveMessagesCheckBoxes();

         foreach (DataRow rowItem in sn.Message.DsHistoryMessages.Tables[0].Rows)
         {
            //if (Convert.ToBoolean(rowItem["chkBox"]))
            //{

            //   if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["MsgId"])), false))
            //      if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["MsgId"])), true))
            //      {
            //         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
            //      }

            //}
         }

         dgMessages_Fill_NewTZ();
      }
      catch (NullReferenceException Ex)
      {
         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         RedirectToLogin();
      }

      catch (Exception Ex)
      {
         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
      }
    }

      private void FleetVehicleOption(bool visible)
      {
         valFleet.Enabled = visible;
         lblFleetTitle.Visible = visible;
         cboFleet.Visible = visible;
         valVehicle.Enabled = visible;
         lblVehicleName.Visible = visible;
         cboVehicle.Visible = visible;   
      }
      protected void cmdViewAlarms_Click(object sender, EventArgs e)
      {
          dgAlarms_Fill_NewTZ();
      }

        // Changes for TimeZone Feature start
      private void dgAlarms_Fill_NewTZ()
      {
          string strFromDate = "";
          string strToDate = "";

          if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
              strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

          if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
              strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

          if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
              strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

          if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
              strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

          if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
              strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

          if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
              strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";


          tblNoData.Visible = false;

          string xml = "";
          ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
          float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

          if (objUtil.ErrCheck(alarms.GetAlarmsXML_NewTZ(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), false))
              if (objUtil.ErrCheck(alarms.GetAlarmsXML_NewTZ(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), true))
              {
                  tblNoData.Visible = true;
                  dgAlarms.DataSource = null;
                  dgAlarms.DataBind();
                  return;
              }

          if (xml == "")
          {
              tblNoData.Visible = true;
              dgAlarms.DataSource = null;
              dgAlarms.DataBind();
              return;
          }

          StringReader strrXML = new StringReader(xml);

          DataSet ds = new DataSet();
          ds.ReadXml(strrXML);

          // Show Combobox
          DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
          dc.DefaultValue = false;
          ds.Tables[0].Columns.Add(dc);

          dgAlarms.DataSource = ds;
          dgAlarms.DataBind();
          this.dgAlarms.Visible = true;
          sn.Message.DsHistoryAlarms = ds;

      }
      // Changes for TimeZone Feature end

      private void dgAlarms_Fill()
      {
         string strFromDate = "";
         string strToDate = "";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
            strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
            strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
            strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
            strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";


         tblNoData.Visible = false;
         
         string xml = "";
         ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
         Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

         if (objUtil.ErrCheck(alarms.GetAlarmsXML(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), false))
            if (objUtil.ErrCheck(alarms.GetAlarmsXML(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), true))
            {
               tblNoData.Visible = true;
               dgAlarms.DataSource = null;
               dgAlarms.DataBind();
               return;
            }

         if (xml == "")
         {
            tblNoData.Visible = true;
            dgAlarms.DataSource = null;
            dgAlarms.DataBind();
            return;
         }

         StringReader strrXML = new StringReader(xml);

         DataSet ds = new DataSet();
         ds.ReadXml(strrXML);

         // Show Combobox
         DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
         dc.DefaultValue = false;
         ds.Tables[0].Columns.Add(dc);

         dgAlarms.DataSource = ds;
         dgAlarms.DataBind();
         this.dgAlarms.Visible = true;
         sn.Message.DsHistoryAlarms = ds;
  
      }
      protected void dgAlarms_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         dgAlarms.CurrentPageIndex = e.NewPageIndex;
         dgAlarms.DataSource = sn.Message.DsHistoryAlarms;
         dgAlarms.DataBind();
         dgAlarms.SelectedIndex = -1;
      }
      protected void cmdAccept_Click(object sender, EventArgs e)
      {
         try
         {
            
            DataSet ds = new DataSet();
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();


            SaveAlarmsCheckBoxes();

            foreach (DataRow rowItem in sn.Message.DsHistoryAlarms.Tables[0].Rows)
            {
               if (Convert.ToBoolean(rowItem["chkBox"]))
               {

                  if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), false))
                     if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), true))
                     {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     }

               }
            }

            dgAlarms_Fill_NewTZ();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }
      protected void cmdCloseAlarms_Click(object sender, EventArgs e)
      {
         try
         {
            
            DataSet ds = new DataSet();
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();


            SaveAlarmsCheckBoxes();

            foreach (DataRow rowItem in sn.Message.DsHistoryAlarms.Tables[0].Rows)
            {
               if (Convert.ToBoolean(rowItem["chkBox"]))
               {

                  if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), false))
                     if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["AlarmId"])), true))
                     {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm close failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     }

               }
            }

            dgAlarms_Fill_NewTZ();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
      {
          int fleetId = 0;
          int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
          
          this.cboVehicle.Items.Clear();
          if (fleetId > 0)
          {
              this.cboVehicle.Visible = true;
              this.lblVehicleName.Visible = true;
              CboVehicle_Fill(fleetId);
          } 
      }
   
}
}
