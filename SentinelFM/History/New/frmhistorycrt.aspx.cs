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
using System.Globalization;
using System.Text.RegularExpressions;

namespace SentinelFM
{

    public partial class History_New_frmhistorycrt : SentinelFMBasePage
    {
        protected System.Web.UI.WebControls.Label lblVehicle;
        protected System.Web.UI.WebControls.DataGrid dgVehicleList;

        public string DefaultOrganizationHierarchyFleetId = "0";
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        
        public string OrganizationHierarchyPath = "";
        public string PreviousDateformat = ""; 
        private string CurrentUICulture = "en-US";
        private string DateFormat = string.Empty;
        private string TimeFormat = string.Empty;
        public  string SelectedUICulture = "";
        public bool MutipleUserHierarchyAssignment;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                
                DateFormat = sn.User.DateFormat;
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                string defaultnodecode = string.Empty;
                SelectedUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
                txtFrom.Calendar.CultureInfo = new System.Globalization.CultureInfo(SelectedUICulture);
                txtTo.Calendar.CultureInfo = new System.Globalization.CultureInfo(SelectedUICulture);               
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

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                

                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
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
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;

                        if (MutipleUserHierarchyAssignment)
                        {
                            hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
							hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                            DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                            DefaultOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;
                        }
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode).ToString();
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    }


                    if (MutipleUserHierarchyAssignment)
                    {
                        if (DefaultOrganizationHierarchyNodeCode.Trim() == string.Empty)
                            DefaultOrganizationHierarchyFleetName = "";
                        else if (DefaultOrganizationHierarchyNodeCode.Contains(","))
                            DefaultOrganizationHierarchyFleetName = GetLocalResourceObject("ResMultipleHierarchies").ToString();
                        else
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    }

                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                    btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    
                    //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                    //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
                    lblFleetTitle.Visible = false;
                    cboFleet.Visible = false;
                    lblOhTitle.Visible = true;
                    btnOrganizationHierarchyNodeCode.Visible = true;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                }

                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                int i = 0;


                if ((sn == null) || (sn.UserName == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistoryCriteria, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    
                    string datetime = "";
                    if (sn.PreviousDateFormat == " ")
                    {
                        PreviousDateformat = DateFormat;
                    }
                    else
                    {
                        PreviousDateformat = sn.PreviousDateFormat;
                    }
                   
                    
                    try
                    {
                        if (Request[this.txtFrom.UniqueID] != null)
                            datetime = Convert.ToDateTime(Request[this.txtFrom.UniqueID].ToString()).ToString(DateFormat);
                        else if (!String.IsNullOrEmpty(sn.History.FromDate.ToString()))
                        {                          
                            datetime = DateTime.ParseExact(sn.History.FromDate.ToString(), PreviousDateformat, null).ToString(DateFormat);                          
                            
                           //datetime = DateTime.ParseExact(sn.History.FromDate.ToString(), DateFormat, CultureInfo.InvariantCulture).ToString(DateFormat);
                        }
                            
                           
                        else
                            datetime = DateTime.Now.ToString(DateFormat);
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, DateFormat, CurrentUICulture);

                        if (Request[this.txtTo.UniqueID] != null)
                            datetime = Convert.ToDateTime(Request[this.txtTo.UniqueID].ToString()).ToString(DateFormat);
                        else if (!String.IsNullOrEmpty(sn.History.ToDate))
                            
                                // datetime = DateTime.ParseExact(sn.History.ToDate, DateFormat, null).ToString(DateFormat);
                            datetime = DateTime.ParseExact(sn.History.ToDate, PreviousDateformat, null).ToString(DateFormat);
                           
                            else
                                datetime = DateTime.Now.AddDays(1).ToString(DateFormat);
         
                        this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, DateFormat, CurrentUICulture);

                    }
                    catch
                    {
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, DateFormat, CurrentUICulture);
                        datetime = DateTime.Now.AddDays(1).ToString(DateFormat);
                        this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, DateFormat, CurrentUICulture);
                    }

                    txtFrom.DateInput.DateFormat = DateFormat;
                    txtFrom.DateInput.DisplayDateFormat = DateFormat;
                    txtTo.DateInput.DateFormat = DateFormat;
                    txtTo.DateInput.DisplayDateFormat = DateFormat;


                    this.tblStopReport.Visible = false;
                    clsMisc.cboHoursFill(ref cboHoursFrom);
                    clsMisc.cboHoursFill(ref cboHoursTo);
                    CboFleet_Fill();

                    if (sn.History.FromHours != "")
                    {
                        this.cboHoursFrom.SelectedIndex = -1;
                        for (i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                        {
                            if (cboHoursFrom.Items[i].Value == sn.History.FromHours)
                            {
                                this.cboHoursFrom.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //this.cboHoursFrom.SelectedIndex = -1;
                        //for (i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                        //{
                        //    if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == 8)
                        //    {
                        //        cboHoursFrom.Items[i].Selected = true;
                        //        break;
                        //    }
                        //}

                        cboHoursFrom.Items[0].Selected = true;
                    }

                    if (sn.History.ToHours != "")
                    {
                        this.cboHoursTo.SelectedIndex = -1;
                        for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                        {
                            if (cboHoursTo.Items[i].Value == sn.History.ToHours)
                            {
                                this.cboHoursTo.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //this.cboHoursTo.SelectedIndex = -1;
                        //for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                        //{
                        //    if (cboHoursTo.Items[i].Value == DateTime.Now.AddHours(1).Hour.ToString())
                        //    {
                        //        cboHoursTo.Items[i].Selected = true;
                        //        break;
                        //    }
                        //}
                        cboHoursTo.Items[0].Selected = true;
                    }

                    if (sn.History.FleetId != 0)
                    {
                        this.cboFleet.SelectedIndex = -1;
                        for (i = 0; i <= cboFleet.Items.Count - 1; i++)
                        {
                            if (cboFleet.Items[i].Value == sn.History.FleetId.ToString())
                                cboFleet.Items[i].Selected = true;
                        }
                        CboVehicle_Fill(Convert.ToInt32(sn.History.FleetId));
                    }
                    else
                    {
                        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                        {
                            if (MutipleUserHierarchyAssignment)
                            {
                                CboVehicle_FillByMultipleFleetIds(DefaultOrganizationHierarchyFleetId);
                            }
                            else
                            {
                                CboVehicle_Fill(int.Parse(DefaultOrganizationHierarchyFleetId));
                            }
                        }
                        else if (sn.User.DefaultFleet != -1)
                        {
                            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                            CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                            //this.lblVehicleName.Visible = true;
                            //this.cboVehicle.Visible = true;
                        }
                    }

                    if (sn.History.VehicleId != 0)
                    {
                        //CboVehicle_Fill(Convert.ToInt32(sn.History.FleetId));

                        this.cboVehicle.SelectedIndex = -1;

                        for (i = 0; i <= cboVehicle.Items.Count - 1; i++)
                        {
                            if (cboVehicle.Items[i].Value == sn.History.VehicleId.ToString())
                            {
                                cboVehicle.Items[i].Selected = true;
                                break;
                            }
                        }
                        //this.lblVehicleName.Visible = true;
                        //this.cboVehicle.Visible = true;
                        LoadCommunicationInfo();
                    }

                    if (cboVehicle.Items.Count == 0)
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

                    if (sn.History.RedirectFromMapScreen)
                    {
                        ShowData();
                        sn.History.RedirectFromMapScreen = false;
                    }
                    lstBoxMsgs_Fill();

                    if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                    {
                        WebHistoryTab.Tabs[0].Width = 80;
                        WebHistoryTab.Tabs[0].CssClass = "tabAlignCenter";
                        WebHistoryTab.Tabs[1].Width = 68;
                        WebHistoryTab.Tabs[1].CssClass = "tabAlignCenter";
                        WebHistoryTab.Tabs[2].Width = 96;
                        WebHistoryTab.Tabs[2].CssClass = "tabAlignCenter";
                    }
                    AddHoursToDropdownList(cboMinutesFrom);
                    AddHoursToDropdownList(cboMinutesTo);
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }

            if (sn.User.OrganizationId != 570) cmdStreetLookup.Visible = false;
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

        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
        }

        private void lstBoxMsgs_Fill()
        {
            try
            {
                DataSet dsMsgTypes = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                if (objUtil.ErrCheck(dbs.GetBoxMsgInTypesByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetBoxMsgInTypesByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to GetBoxMsgInTypes for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }

                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to GetBoxMsgInTypes for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }

                strrXML = new StringReader(xml);
                dsMsgTypes.ReadXml(strrXML);

                lstMsgTypes.DataSource = dsMsgTypes;
                lstMsgTypes.DataBind();
                lstMsgTypes.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("lstMsgTypes_Item_0"), "-1"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        this.cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                        return;
                    }
                if (xml == "")
                {
                    this.cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                //ViewState["dsVehicle"] = dsVehicle;
                sn.History.DsHistoryVehicles = dsVehicle;
                cboVehicle.Visible = true;

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                cboVehicle.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
        }

        private void CboVehicle_FillByMultipleFleetIds(string fleetIds)
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
                        this.cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                        return;
                    }
                if (xml == "")
                {
                    this.cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                //ViewState["dsVehicle"] = dsVehicle;
                sn.History.DsHistoryVehicles = dsVehicle;
                cboVehicle.Visible = true;

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                cboVehicle.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
        }


        protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            {
                this.lblVehicleName.Visible = true;
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            }
        }

        protected void cmdShowHistory_Click(object sender, System.EventArgs e)
        {
            try
            {
                switch (this.cboHistoryType.SelectedItem.Value)
                {
                    case "0":
                        sn.History.ShowTrips = false;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = false;
                        sn.History.ShowStopsAndIdle = false;
                        break;
                    case "1":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = true;
                        sn.History.ShowStops = true;
                        sn.History.ShowIdle = true;
                        sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        break;
                    case "2":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = false;
                        sn.History.ShowStops = true;
                        sn.History.ShowIdle = false;
                        sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        break;
                    case "3":
                        sn.History.ShowTrips = false;
                        sn.History.ShowBreadCrumb = false;
                        sn.History.ShowStopsAndIdle = false;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = true;
                        sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                        break;
                    case "4":
                        sn.History.ShowTrips = true;
                        sn.History.ShowStops = false;
                        sn.History.ShowIdle = false;
                        sn.History.ShowStopsAndIdle = false;
                        break;
                }
                ShowData();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
        }

        private void ShowData()
        {
            try
            {
                string strFromDate = "";
                string strToDate = "";
                Page.Validate();
                if (!Page.IsValid)
                    return;


                strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + String.Format("{0:t}", this.cboHoursFrom.SelectedItem.Value + ":" + cboMinutesFrom.SelectedValue);
                strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " " + String.Format("{0:t}", this.cboHoursTo.SelectedItem.Value + ":" + cboMinutesTo.SelectedValue);

                System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentCulture;

                strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
                strToDate = Convert.ToDateTime(strToDate, ci).ToString();

                if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "";
                }

                string DisableFullHistoryData = ConfigurationManager.AppSettings["DisableFullHistoryData"].ToString();
                TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(strToDate).Ticks - Convert.ToDateTime(strFromDate).Ticks);
                if (Convert.ToBoolean(DisableFullHistoryData) && (currDuration.TotalHours > 24))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "Due to service loads, HISTORY requests have been temporarily limited to a period of 24 hours.";
                    return;
                }

                //TimeSpan DateDiff = Convert.ToDateTime(strToDate).Subtract(Convert.ToDateTime(strFromDate));
                //if (DateDiff.TotalDays > 62)
                //{
                //    this.lblMessage.Visible = true;
                //    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMaximumDays");
                //    return;
                //}

                //if (Convert.ToInt64(this.cboVehicle.SelectedItem.Value) == 0)
                //{
                //    int tempDays =Convert.ToInt32((60 / Convert.ToInt64(this.cboVehicle.Items.Count)));
                //    tempDays = tempDays == 0 ? 1 : tempDays;

                //    if (Convert.ToInt32(DateDiff.TotalDays) > tempDays)
                //    {
                //        this.lblMessage.Visible = true;
                //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMaximumDaysEntireFleet") + tempDays.ToString() ;
                //        return;
                //    }
                //}




		
        



                if (this.chkLatestMsg.Checked)
                    sn.History.SqlTopMsg = "TOP 1";
                else
                    sn.History.SqlTopMsg = "";

                sn.History.MsgList = "";
                foreach (ListItem li in lstMsgTypes.Items)
                {
                    if ((li.Selected))
                    {
                        if (li.Value == "-1")
                        {
                            sn.History.MsgList = "";
                            break;
                        }
                        else
                            sn.History.MsgList += li.Value + ",";
                    }
                }

                if (sn.History.MsgList != "")
                    sn.History.MsgList = sn.History.MsgList.Substring(0, sn.History.MsgList.Length - 1);

                //Update Previous dateformat
                sn.PreviousDateFormat = DateFormat;
               
                sn.History.FromDate = this.txtFrom.SelectedDate.Value.ToString(DateFormat);
                sn.History.ToDate = this.txtTo.SelectedDate.Value.ToString(DateFormat);                
                sn.History.FromHours = this.cboHoursFrom.SelectedItem.Value;
                sn.History.ToHours = this.cboHoursTo.SelectedItem.Value;
                sn.History.VehicleId = Convert.ToInt64(this.cboVehicle.SelectedItem.Value);
                if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    sn.History.FleetId = int.Parse(DefaultOrganizationHierarchyFleetId);
                }
                else
                {
                    sn.History.FleetId = Convert.ToInt64(this.cboFleet.SelectedItem.Value);
                }
                sn.History.ShowToolTip = true;
                sn.History.CarLatitude = "";
                sn.History.CarLongitude = "";
                sn.History.DsHistoryInfo = null;
                sn.History.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
                sn.History.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
                if (cboCommMode.Visible)
                    sn.History.DclId = Convert.ToInt16(this.cboCommMode.SelectedItem.Value);
                else
                    sn.History.DclId = -1;

                sn.History.FromDateTime = strFromDate;
                sn.History.ToDateTime = strToDate;
                if (WebHistoryTab.SelectedIndex == 1 && this.txtAddress.Text != "")
                    sn.History.Address = "%" + this.txtAddress.Text + "%";
                else
                    sn.History.Address = "";

                sn.History.TripSensor = Convert.ToInt16(this.optEndTrip.SelectedItem.Value);

                string str = "frmHistWait.aspx?VehicleId=" + this.cboVehicle.SelectedItem.Value + "&strFromDate=" + strFromDate + "&strToDate=" + strToDate;
                //if (sn.User.MapType != VLF.MAP.MapType.MapsoluteWeb)
                //   Response.Write("<script language='javascript'>  parent.location.href='" + str + "'; </script>");
                //else
                //Response.Write("<script language='javascript'>  parent.frmHis.location.href='" + str + "'; </script>");
                string vehiclePlate = string.Empty;
                if (sn.History.LicensePlate != null)
                    vehiclePlate = sn.History.LicensePlate;
                if (!sn.History.RedirectFromMapScreen) { 
                    RadAjaxManager1.ResponseScripts.Add("parent.frmHis.GenerateGridsDate('" + vehiclePlate + "');"); 
                }
                else
                {
                    //Disable loading data when redirecting from map
                    //RadAjaxManager1.ResponseScripts.Add("LoadHistoryGridData('" + vehiclePlate + "');");
                    RadAjaxManager1.ResponseScripts.Add("SetSendCommandEvent('" + vehiclePlate + "');");
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
        }

        protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LoadCommunicationInfo();
        }

        private void LoadCommunicationInfo()
        {
            try
            {
                DataRow[] DataRows = null; ;
                //DataRows = sn.History.DsHistoryVehicles.Tables[0].Select("VehicleId=" + Convert.ToInt32(this.cboVehicle.SelectedItem.Value.ToString().TrimEnd()));
                DataRows = sn.History.DsHistoryVehicles.Tables[0].Select("VehicleId='" + this.cboVehicle.SelectedItem.Value.ToString().TrimEnd() + "'");
                if ((DataRows != null) && (DataRows.Length > 0))
                {
                    DataRow drVehicle = DataRows[0];
                    sn.History.LicensePlate = drVehicle["LicensePlate"].ToString().TrimEnd();
                    CboCommMode_Fill(Convert.ToInt32(drVehicle["BoxId"]));
                    this.lblCommMode.Visible = true;
                    this.cboCommMode.Visible = true;
                    //if (sn.User.ControlEnable(sn, 43))
                    //{
                    //   CboCommMode_Fill(Convert.ToInt32(drVehicle["BoxId"]));
                    //   this.lblCommMode.Visible = true;
                    //   this.cboCommMode.Visible = true;
                    //}
                }
            }
            catch
            {
                this.lblCommMode.Visible = false;
                this.cboCommMode.Visible = false;
            }
            //if (sn.User.ControlEnable(sn, 43))
            //{
            //   DataSet ds = sn.History.DsHistoryVehicles;
            //   foreach (DataRow dr in ds.Tables[0].Rows)
            //   {
            //      if (Convert.ToInt32(dr["VehicleId"].ToString().TrimEnd()) == Convert.ToInt32(this.cboVehicle.SelectedItem.Value.ToString().TrimEnd()))
            //      {
            //         CboCommMode_Fill(Convert.ToInt32(dr["BoxId"]));
            //         this.lblCommMode.Visible = true;
            //         this.cboCommMode.Visible = true;
            //         return;
            //      }
            //   }
            //}
        }

        protected void cboHistoryType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            WebHistoryTab.SelectedIndex = 0;

            lnkMapLegends.Visible = false;
            if ((cboHistoryType.SelectedItem.Value == "1" || cboHistoryType.SelectedItem.Value == "2" || cboHistoryType.SelectedItem.Value == "3") && (this.cboVehicle.Visible))
            {
                this.tblStopReport.Visible = true;
                this.lnkMapLegends.Visible = true;
            }
            else if (cboHistoryType.SelectedItem.Value == "4")
            {
                WebHistoryTab.SelectedIndex = 2;
            }
            else
            {
                this.tblStopReport.Visible = false;
                this.lnkMapLegends.Visible = false;
            }
        }

        private void CboCommMode_Fill(Int32 BoxId)
        {
            try
            {
                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, BoxId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        return;
                    }
                if (xml == "")
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    return;
                }
                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                this.cboCommMode.DataSource = ds;
                this.cboCommMode.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
            finally
            {
                cboCommMode.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboCommMode_Item_0"), "-1"));
            }
        }

        private void AddHoursToDropdownList(DropDownList ddl)
        { 
            ddl.Items.Add("00");
            ddl.Items.Add("15");
            ddl.Items.Add("30");
            ddl.Items.Add("45");
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
        {
            if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
            {
                string fleetIds = hidOrganizationHierarchyFleetId.Value.ToString().Trim();
                if (fleetIds != string.Empty)
                    CboVehicle_FillByMultipleFleetIds(fleetIds);
                else
                    this.cboVehicle.Items.Clear();
            }
            else
            {

                int fleetId = 0;
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                if (fleetId > 0)
                    CboVehicle_Fill(fleetId);
                else
                    this.cboVehicle.Items.Clear();

                /*if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                {
                    this.lblVehicleName.Visible = true;
                    CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
                }*/
            }
        }

        public DateTime ConvertStringToDateTime(string DateValue, string Dateformat)
        {
            return ConvertStringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        public DateTime ConvertStringToDateTime(string value, string format, string cultureinfo)
        {

            CultureInfo culture = new CultureInfo(cultureinfo);
            DateTime date = DateTime.Now;
            string err = "";

            try
            {
                if (format.ToLower().IndexOf("hh") >= 0)
                    
                    value = Convert.ToDateTime(value).ToString(format);

                date = DateTime.ParseExact(value, format, null);
                err = "";
            }
            catch (FormatException fx)
            {
                err = fx.Message;
                date = DateTime.Now;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                date = DateTime.Now;
            }
            finally
            {
            }

            return date;
        }
    }
}
