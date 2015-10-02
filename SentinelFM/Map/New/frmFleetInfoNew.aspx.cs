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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Web.Services;
using System.Text;
using System.Web.Script.Serialization;

 using System.Web.Services;
 using System.Web.Script.Services;
namespace SentinelFM
{
    
    public partial class Map_Telerik_frmFleetInfoNew : SentinelFMBasePage
    {

        // System.Net.WebRequest myRequest;
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        public string redirectURL;
        private DataSet dsFleetInfo;
        public VLF.MAP.ClientMapProxy map;
        public VLF.MAP.ClientMapProxy geoMap;
        public string strGeoMicroURL;
        public bool chkShowAutoPostBack = false;
        public int AutoRefreshTimer = 60000;
        public static string NoShowString = "Nothing";
        private Stopwatch watch = new Stopwatch();
        private Stopwatch Pagewatch = new Stopwatch();
        public string _xml;
        public DropDownList BarItemcboFleet = null;
        public string errorLoad = "Failed to load data.";
        public string errorCancel = "Failed to cancel.";
        public string DispMapType = "";
        public static string ErrorMessage = "";
        bool isExport = false;

        public bool ShowOrganizationHierarchy;
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyMultipleFleetIds = string.Empty;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        //public string LoadVehiclesBasedOn = "fleet";
        public string OrganizationHierarchyPath = "";
        public Button BarItembtnOrganizationHierarchyNodeCode = null;
        public Label BarItemlblOhTitle = null;
        public Label BarItemlblFleetTitle = null;

        public bool MutipleUserHierarchyAssignment;
        public string ResMultipleHierarchy;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                BarItemcboFleet = (DropDownList)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("cboFleet");
                BarItembtnOrganizationHierarchyNodeCode = (Button)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("btnOrganizationHierarchyNodeCode");
                BarItemlblOhTitle = (Label)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("lblOhTitle");
                BarItemlblFleetTitle = (Label)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("lblFleetTitle");

                ResMultipleHierarchy = GetLocalResourceObject("MultipleHierarchy.Text").ToString(); //"Multiple Hierarchies";

                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

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

                    /*if (Convert.ToInt16(rowItem["PreferenceId"]) == 36)
                    {
                        string d = rowItem["PreferenceValue"].ToString().ToLower();
                        if (d == "hierarchy")
                            LoadVehiclesBasedOn = "hierarchy";
                        else
                            LoadVehiclesBasedOn = "fleet";
                    }*/
                }

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                    ShowOrganizationHierarchy = true;
                else
                {
                    ShowOrganizationHierarchy = false;
                    //LoadVehiclesBasedOn = "fleet";
                    sn.User.LoadVehiclesBasedOn = "fleet";
                }

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
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                        hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        //BarItembtnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    }

                    if (MutipleUserHierarchyAssignment)
                    {

                        if (IsPostBack)
                        {
                            DefaultOrganizationHierarchyMultipleFleetIds = hidOrganizationHierarchyFleetId.Value;
                        }
                        else
                        {
                            DefaultOrganizationHierarchyMultipleFleetIds = sn.User.PreferFleetIds;
                            DefaultOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;
                            hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                            hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;                            
                        }

                        if (DefaultOrganizationHierarchyMultipleFleetIds.Trim() == string.Empty)
                            DefaultOrganizationHierarchyFleetName = "";
                        else if (DefaultOrganizationHierarchyMultipleFleetIds.Contains(","))
                            DefaultOrganizationHierarchyFleetName = ResMultipleHierarchy;
                        else
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyMultipleFleetIds));
                    }

                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                    //if (MutipleUserHierarchyAssignment && DefaultOrganizationHierarchyMultipleFleetIds.Contains(","))
                    //{
                    //    BarItembtnOrganizationHierarchyNodeCode.Text = "Multiple Hierarchies";
                    //}
                    //else
                    //{
                        BarItembtnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    //}
                    BarItemlblFleetTitle.Visible = false;
                    BarItemcboFleet.Visible = false;
                    BarItemlblOhTitle.Visible = false;
                    BarItembtnOrganizationHierarchyNodeCode.Visible = true;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);                    
                }

                //sn.User.MapType = VLF.MAP.MapType.LSD;

                System.Diagnostics.Trace.WriteLine("Page_Load->Thread ID:" + System.Threading.Thread.CurrentThread.GetHashCode().ToString());


                if (!sn.User.IsLSDEnabled)
                {
                    //dgFleetInfo.MasterTableView.Columns[11].Visible = false;
                    dgFleetInfo.MasterTableView.Columns.FindByUniqueName("LSD").Visible = false;
                }

                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;
                
                if (!Page.IsPostBack)
                {
                    try
                    {
                        if (sn.Map.MapRefresh && AutoRefreshTimer > 0)
                        {
                            CheckBox chkAutoRefresh = (CheckBox)radToolBarMenu.FindItemByValue("AutoRefresh").FindControl("chkAutoRefresh");
                            chkAutoRefresh.Checked = true;
                            chkShowAutoPostBack = true;
                            string secriptPostBack = "timeout_handles = window.setTimeout('SetReloadSetTimeOut()'," + AutoRefreshTimer.ToString() + ")";
                             RadAjaxManager1.ResponseScripts.Add(secriptPostBack);
                        }
                        else
                        {
                            chkShowAutoPostBack = false;
                            string secriptPostBack = "ClearReloadSetTimeOut()";
                            RadAjaxManager1.ResponseScripts.Add(secriptPostBack);

                        }

                        if (AutoRefreshTimer == 0)
                        {
                            CheckBox chkAutoRefresh = (CheckBox)radToolBarMenu.FindItemByValue("AutoRefresh").FindControl("chkAutoRefresh");
                            chkAutoRefresh.Checked = false;
                            chkAutoRefresh.Enabled = false;
                        }

                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--SetReloadSetTimeOut ---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);

                    }

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref FleetForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);



                    try
                    {

                       if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957)
                               cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue("999"));
                        else
                                cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue(sn.Map.DgItemsPerPage.ToString()));




                        this.dgFleetInfo.MasterTableView.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
                        if (sn.User.ShowMapGridFilter == 1)
                        {
                            hidFilter.Value = "1";
                        }
                        else
                        {   
                            hidFilter.Value = "";
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Grid Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);

                    }


                    GuiSecurity(this);



                    //Get Vehicles Info
                    sn.MessageText = "";
                    CboFleet_Fill();
                    if (sn.MessageText != "") //No Fleet for user
                        return;

                    string strUnitOfMes = sn.User.UnitOfMes == 1 ? " (km/h)" : " (mph)";
                    this.dgFleetInfo.MasterTableView.Columns.FindByUniqueName("CustomSpeed").HeaderText =
                        this.dgFleetInfo.MasterTableView.Columns.FindByUniqueName("CustomSpeed").HeaderText + strUnitOfMes; ;
                    FindExistingPreference();


                    try
                    {
                        if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                        {
                            sn.Map.SelectedMultiFleetIDs = DefaultOrganizationHierarchyMultipleFleetIds;
                            DgFleetInfo_MultiFill_NewTZ(DefaultOrganizationHierarchyMultipleFleetIds);
                        }
                        else if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                        {
                            sn.Map.SelectedFleetID = DefaultOrganizationHierarchyFleetId;
                            DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.Map.SelectedFleetID));
                        }
                        else
                        {
                            if (sn.Map.SelectedFleetID != 0)
                            {
                                BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                                DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.Map.SelectedFleetID));
                            }
                            else
                            {
                                if (sn.User.DefaultFleet != -1)
                                {
                                    BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                                    DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.User.DefaultFleet));
                                    sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                                }
                                else
                                {
                                    DgFleetInfo_Fill_NewTZ(-1);
                                    hidFilter.Value = "";
                                }
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);

                    }

                    ErrorMessage = GetScriptEscapeString(CheckUpdatePositionResult(sn));

                    if (sn.Map.ReloadMap)
                    {
                        LoadMap();
                        sn.Map.ReloadMap = false;
                    }
                    if (sn.User.MapType == VLF.MAP.MapType.LSD)
                    {
                        RadToolBarButton mnuItem = new RadToolBarButton();
                        mnuItem.Text = "LSD";
                        mnuItem.Value = "LSD";
                        mnuItem.Font.Underline = true;
                        radToolBarMenu.Items.Add(mnuItem);
                    }

                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

            //if (sn.User.MapType == VLF.MAP.MapType.LSD)
            //    DispMapType = "../frmLSDmap.aspx";
            //else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
            //    DispMapType = "../../MapVE/VEMap.aspx";
            //else
                DispMapType = "../frmvehiclemap.aspx";

            string exportGrid = "Export grid";
            try
            {
                showFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ShowFilter").ToString();
                hideFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_HideFilter").ToString();
                exportGrid = HttpContext.GetGlobalResourceObject("Const", "RadGrid_Export").ToString();
            }
            catch { };
            imgExport.ToolTip = exportGrid;

            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957)
            {
                dgFleetInfo.Columns[6].Visible = false; //Hydro-Quebec (Remove Armed From MapGrid:Map/new/frmFleetInfoNew.aspx)
            }
        }

        public static string CheckUpdatePositionResult(SentinelFMSession sn)
        {
            string ret = string.Empty;

            #region Check Results after update position

            sn.MessageText = "";

            if (sn.Cmd.DtUpdatePositionFails.Rows.Count > 0)
                ShowUpdatePositionsTimeOuts(sn);


            if (sn.Cmd.UpdatePositionSend)
            {
                ShowUpdatePositionsNotValid(sn);
                sn.Cmd.UpdatePositionSend = false;
            }


            if (sn.Cmd.DtUpdatePositionFails.Rows.Count > 0)
                sn.Cmd.DtUpdatePositionFails.Rows.Clear();

            #endregion



            if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
            {
                ret = sn.MessageText;
                //Replace
                //ShowErrorMessage();
            }
            return ret;
        }

        private void CboFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                BarItemcboFleet.DataSource = dsFleets;
                BarItemcboFleet.DataBind();
                BarItemcboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));



            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", CboFleet_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }
        // Changes for TimeZone Feature start
        private void DgFleetInfo_Fill_NewTZ(int fleetId)
        {
            try
            {

                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../../Datasets/FleetInfo.xsd");

                dsFleetInfo.ReadXmlSchema(strPath);

                if (fleetId == -1)
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                watch.Reset();
                watch.Start();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));


                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Map.DsFleetInfo = dsFleetInfo;
                        return;
                    }

                if (xml == "")
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                strrXML = new StringReader(xml);

                dsFleetInfo.ReadXml(strrXML);

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));

                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                ExceptionLogger("NullReferenceException ", Ex);
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
        // Changes for TimeZone Feature end

        private void DgFleetInfo_Fill(int fleetId)
        {
            try
            {

                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../../Datasets/FleetInfo.xsd");

                dsFleetInfo.ReadXmlSchema(strPath);

                if (fleetId == -1)
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                watch.Reset();
                watch.Start();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));


                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Map.DsFleetInfo = dsFleetInfo;
                        return;
                    }

                if (xml == "")
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                strrXML = new StringReader(xml);

                dsFleetInfo.ReadXml(strrXML);

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetId + ", User Id:" + sn.UserID.ToString()));

                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                ExceptionLogger("NullReferenceException ", Ex);
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
        // Changes for TimeZone Feature start
        private void DgFleetInfo_MultiFill_NewTZ(string fleetIds)
        {
            try
            {

                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../../Datasets/FleetInfo.xsd");

                dsFleetInfo.ReadXmlSchema(strPath);

                if (fleetIds == string.Empty)
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                watch.Reset();
                watch.Start();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetIds + ", User Id:" + sn.UserID.ToString()));


                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang_NewTZ(sn.UserID, sn.SecId, fleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang_NewTZ(sn.UserID, sn.SecId, fleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Map.DsFleetInfo = dsFleetInfo;
                        return;
                    }

                if (xml == "")
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                strrXML = new StringReader(xml);

                dsFleetInfo.ReadXml(strrXML);

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetIds + ", User Id:" + sn.UserID.ToString()));

                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                ExceptionLogger("NullReferenceException ", Ex);
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
        // Changes for TimeZone Feature end

        private void DgFleetInfo_MultiFill(string fleetIds)
        {
            try
            {

                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../../Datasets/FleetInfo.xsd");

                dsFleetInfo.ReadXmlSchema(strPath);

                if (fleetIds == string.Empty)
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                StringReader strrXML = null;
                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                watch.Reset();
                watch.Start();

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "-->GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetIds + ", User Id:" + sn.UserID.ToString()));


                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang(sn.UserID, sn.SecId, fleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByMultipleFleetsByLang(sn.UserID, sn.SecId, fleetIds, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        sn.Map.DsFleetInfo = dsFleetInfo;
                        return;
                    }

                if (xml == "")
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                strrXML = new StringReader(xml);

                dsFleetInfo.ReadXml(strrXML);

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "<-- GetVehiclesLastKnownPositionInfoByLang - Fleet Id:" + fleetIds + ", User Id:" + sn.UserID.ToString()));

                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                ExceptionLogger("NullReferenceException ", Ex);
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }






        private void FindExistingPreference()
        {
            try
            {

                // Changes for TimeZone Feature start
                if (sn.User.NewFloatTimeZone< 0)
                    ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
                else
                    ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")";
                // Changes for TimeZone Feature end
            }
             
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", FindExistingPreference--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }


        public void RefreshPosition()
        {
            try
            {

                bool VehicleSelected = false;
                StringBuilder vehicleIDs = new StringBuilder();
                foreach (GridDataItem gridItem in dgFleetInfo.Items)
                {
                    if (!(gridItem is GridDataItem)) continue;
                    CheckBox chkSelectVehicle = (CheckBox)gridItem.FindControl("chkSelectVehicle");
                    if (chkSelectVehicle != null)
                    {
                        if (chkSelectVehicle.Checked)
                        {
                            VehicleSelected = true;
                            vehicleIDs.Append("," + gridItem.GetDataKeyValue("VehicleId").ToString());
                        }
                    }
                }

                if (vehicleIDs.Length > 0) vehicleIDs.Append(",");
                SaveShowCheckBoxes_1(vehicleIDs.ToString(), false, sn);

                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                    DgFleetInfo_MultiFill_NewTZ(sn.Map.SelectedMultiFleetIDs);
                else
                    DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.Map.SelectedFleetID));


                //Check if vehicles selected

                if (VehicleSelected)
                    LoadMap();

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", RefreshPosition--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }



        private static void ShowUpdatePositionsTimeOuts(SentinelFMSession sn)
        {
            try
            {

                sn.MessageText = "";

                // TimeOut Messages
                string strSQL = "Status=" + Convert.ToString((int)CommandStatus.CommTimeout);
                DataRow[] foundRows = sn.Cmd.DtUpdatePositionFails.Select(strSQL);

                if (foundRows.Length > 0)
                {
                    sn.MessageText += GetLocalResourceValue("sn_MessageText_CommunicationWithVehicle1") + ": ";
                    foreach (DataRow rowItem in foundRows)
                    {
                        sn.MessageText += "\r" + rowItem["VehicleDesc"].ToString().TrimEnd() ;
                    }


                    //if (sn.MessageText.Length > 0)
                    //    sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);

                    sn.MessageText += "\r" + GetLocalResourceValue("sn_MessageText_CommunicationWithVehicle2");

                }

                //clear filter
                sn.Cmd.DtUpdatePositionFails.Select();

                //Queued Messages 
                strSQL = "Status=" + Convert.ToString((int)CommandStatus.Pending);
                foundRows = sn.Cmd.DtUpdatePositionFails.Select(strSQL);

                if (foundRows.Length > 0)
                {
                    if (sn.MessageText != "") sn.MessageText = sn.MessageText + "\r";
                    sn.MessageText +=  GetLocalResourceValue("sn_MessageText_UpdatePositionForVehicle1") + ": ";
                    foreach (DataRow rowItem in foundRows)
                    {
                        sn.MessageText += "\r" + rowItem["VehicleDesc"].ToString().TrimEnd();
                    }

                    //if (sn.MessageText.Length > 0)
                    //    sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);

                    sn.MessageText += "\r" + GetLocalResourceValue("sn_MessageText_UpdatePositionForVehicle2");
                }



            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                throw Ex;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsTimeOuts--> User:" + sn.UserID.ToString() + " Form:frmFleetInfoNew.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                throw Ex;
            }
        }


        private static void ShowUpdatePositionsNotValid(SentinelFMSession sn)
        {
            try
            {
                if (sn.MessageText != "")
                    sn.MessageText += "\r_________________________________________\r";


                //Delay for resolving "Store Position"
                //System.Threading.Thread.Sleep(2000);


                bool ShowTitle = false;

                DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
                Boolean errorExist = false;
                if (drArr != null || drArr.Length > 0)
                {
                    foreach (DataRow rowItem in drArr)
                    {
                        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                        {

                            bool UpdatePositionFails = false;

                            foreach (DataRow rw in sn.Cmd.DtUpdatePositionFails.Rows)
                            {
                                if (rw["VehicleDesc"].ToString().TrimEnd() == rowItem["Description"].ToString().TrimEnd())
                                {
                                    UpdatePositionFails = true;
                                    break;
                                }
                            }

                            // If not exist Error message for this vehicle
                            if (!UpdatePositionFails)
                            {
                                if (!ShowTitle)
                                {
                                    sn.MessageText += GetLocalResourceValue("sn_MessageText_GPSPositionForVehicle1") + ": ";
                                    ShowTitle = true;
                                }

                                sn.MessageText += "\r" + rowItem["Description"].ToString().TrimEnd();
                                errorExist = true;
                            }
                        }
                    }
                }



                if (sn.MessageText.Length > 0 && errorExist)// && (sn.MessageText.Substring(sn.MessageText.Length - 1, 1) == ","))
                {
                    //sn.MessageText = sn.MessageText.Substring(0, sn.MessageText.Length - 1);
                    sn.MessageText += "\r" + GetLocalResourceValue("sn_MessageText_GPSPositionForVehicle2");
                }

                sn.Cmd.DtUpdatePositionFails.Rows.Clear();

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                throw Ex;
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsNotValid--> User:" + sn.UserID.ToString() + " Form:frmFleetInfoNew.aspx"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                throw Ex;
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

        //Return JSON string
        public static string SaveShowCheckBoxes_1(string vehicleIDs,  Boolean isCheckTime, SentinelFMSession snP)
        {
            string UnitOfMes = snP.User.UnitOfMes == 1 ? "km/h" : "mi/h";
            StringBuilder OldDateVehicleNames = new StringBuilder();
            string OldDateVehicleNamesStr = string.Empty;
            ArrayList vehicleMain = new ArrayList();
            string originDate = "";

         

            foreach (DataRow rowItem in snP.Map.DsFleetInfo.Tables[0].Rows)
            {
                rowItem["chkBoxShow"] = false;
                rowItem["Updated"] = false;
                if (rowItem["VehicleId"] != DBNull.Value &&
                    vehicleIDs.IndexOf(string.Format(",{0},", rowItem["VehicleId"].ToString())) >= 0)
                {
                    rowItem["chkBoxShow"] = true;
                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                    //vehicleDic.Add("OriginDateTime", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                    //vehicleDic.Add("Latitude", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                    //vehicleDic.Add("Longitude", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                    //vehicleDic.Add("Description", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                    //vehicleDic.Add("IconTypeName", rowItem["IconTypeName"] is DBNull ? string.Empty : rowItem["IconTypeName"].ToString());
                    //vehicleDic.Add("MyHeading", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                    //vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                    //vehicleDic.Add("Speed", rowItem["Speed"] is DBNull ? string.Empty : rowItem["Speed"].ToString());

                    string icon = string.Empty;

                    if (rowItem["IconTypeName"] != DBNull.Value && rowItem["OriginDateTime"] != DBNull.Value)
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-snP.User.PositionExpiredTime))
                        {
                            icon = "Grey" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                        }
                        else
                        {
                            if (Convert.ToBoolean(rowItem["chkPTO"]))
                                icon = "Blue" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                            else if (rowItem["Speed"].ToString() != "0")
                                icon = "Green" + rowItem["IconTypeName"].ToString().TrimEnd() + rowItem["MyHeading"].ToString().TrimEnd() + ".ico";
                            else
                                icon = "Red" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                        }
                    }



                    //if (snP.SelectedLanguage == "fr-CA")
                    //    originDate = Convert.ToDateTime(rowItem["OriginDateTime"]).ToString("d/M/yyyy HH:mm:ss");                    
                        originDate = Convert.ToDateTime(rowItem["OriginDateTime"]).ToString(snP.User.DateFormat+" "+snP.User.TimeFormat);

                    vehicleDic.Add("id", rowItem["VehicleId"] is DBNull ? string.Empty : rowItem["VehicleId"].ToString());
                    //vehicleDic.Add("date", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                    vehicleDic.Add("date", rowItem["OriginDateTime"] is DBNull ? string.Empty : originDate);
                    vehicleDic.Add("lat", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                    vehicleDic.Add("lon", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                    vehicleDic.Add("desc", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                    vehicleDic.Add("icon", icon);
                    vehicleDic.Add("head", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                    vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                    vehicleDic.Add("spd", rowItem["CustomSpeed"] is DBNull ? string.Empty : rowItem["CustomSpeed"].ToString() + " " + UnitOfMes);
                    vehicleDic.Add("addr", rowItem["StreetAddress"] is DBNull ? string.Empty : rowItem["StreetAddress"].ToString());
                    vehicleDic.Add("stat", rowItem["VehicleStatus"] is DBNull ? string.Empty : rowItem["VehicleStatus"].ToString());


                    vehicleMain.Add(vehicleDic);

                    if (isCheckTime && rowItem["OriginDateTime"] != DBNull.Value && rowItem["Description"] != DBNull.Value)
                    {
                        if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-snP.User.PositionExpiredTime))
                           OldDateVehicleNames.Append(rowItem["Description"].ToString().TrimEnd() + "\r");
                    }
                }
            }
            if (OldDateVehicleNames.Length > 0)
            {
                OldDateVehicleNamesStr = "\r" + OldDateVehicleNames.ToString();
                //OldDateVehicleNamesStr =  OldDateVehicleNamesStr.Substring(0, OldDateVehicleNamesStr.Length - 1);
            }
            string vehicleMainStr = string.Empty;
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            if (vehicleMain.Count > 0)
            {
                vehicleMainStr = js.Serialize(vehicleMain);
            }
            string[] retArray = null;
            if (isCheckTime)
            {
                retArray = new string[2];
                retArray[0] = vehicleMainStr;
                retArray[1] = OldDateVehicleNamesStr;
            }
            else
            {
                retArray = new string[1];
                retArray[0] = vehicleMainStr;
            }

            return js.Serialize(retArray);

        }
        
        private void SaveShowCheckBoxes()
        {
            try
            {
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    rowItem["chkBoxShow"] = false;
                    rowItem["Updated"] = false;
                }

                //foreach (string keyValue in dgFleetInfo.RootTable.GetCheckedRows()) Replace
                foreach (GridDataItem radGridDataItem in dgFleetInfo.MasterTableView.Items)
                {
                    if (radGridDataItem["selectCheckBox"] != null &&
                        radGridDataItem["selectCheckBox"].Controls[0] is CheckBox)
                    {
                        CheckBox selectCheckBox = (CheckBox)(radGridDataItem["selectCheckBox"].Controls[0]);
                        if (selectCheckBox.Checked)
                        {
                            string keyValue = radGridDataItem.GetDataKeyValue("VehicleId").ToString();
                            DataRow[] drCollections = null;
                            drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId='" + keyValue + "'");
                            if (drCollections != null && drCollections.Length > 0)
                            {
                                DataRow dRow = drCollections[0];
                                dRow["chkBoxShow"] = true;
                            }
                        }
                    }
                }


            }
            catch
            {
            }
        }

        private void GuiSecurity(System.Web.UI.Control obj)
        {

            foreach (System.Web.UI.Control ctl in obj.Controls)
            {
                try
                {
                    if (ctl.HasControls())
                        GuiSecurity(ctl);

                    System.Web.UI.WebControls.Button CmdButton = (System.Web.UI.WebControls.Button)ctl;
                    bool CmdStatus = false;
                    if (CmdButton.CommandName != "")
                    {
                        CmdStatus = sn.User.ControlEnable(sn, Convert.ToInt32(CmdButton.CommandName));
                        CmdButton.Enabled = CmdStatus;
                    }

                }
                catch
                {
                }
            }
        }




        protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                sn.Map.DgItemsPerPage = Convert.ToInt32(cboGridPaging.SelectedItem.Value);
                this.dgFleetInfo.MasterTableView.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
                BindFleetInfo(true);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cboGridPaging_SelectedIndexChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }



        private void LoadMap()
        {
            try
            {

                ArrayList vehicleMain = new ArrayList();
                string UnitOfMes = sn.User.UnitOfMes == 1 ? "km/h" : "mi/h";
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["VehicleId"] != DBNull.Value &&
                        rowItem["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                        string icon = string.Empty;
                        if (rowItem["IconTypeName"] != DBNull.Value && rowItem["OriginDateTime"] != DBNull.Value)
                        {
                            if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            {
                                icon = "Grey" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                            }
                            else
                            {
                                if (Convert.ToBoolean(rowItem["chkPTO"]))
                                    icon = "Blue" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                                else if (rowItem["Speed"].ToString() != "0")
                                    icon = "Green" + rowItem["IconTypeName"].ToString().TrimEnd() + rowItem["MyHeading"].ToString().TrimEnd() + ".ico";
                                else
                                    icon = "Red" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                            }
                        }

                        vehicleDic.Add("id", rowItem["VehicleId"] is DBNull ? string.Empty : rowItem["VehicleId"].ToString());
                        vehicleDic.Add("date", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                        vehicleDic.Add("lat", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                        vehicleDic.Add("lon", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                        vehicleDic.Add("desc", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                        vehicleDic.Add("icon", icon);
                        vehicleDic.Add("head", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                        vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                        vehicleDic.Add("spd", rowItem["CustomSpeed"] is DBNull ? string.Empty : rowItem["CustomSpeed"].ToString() + " " + UnitOfMes);
                        vehicleDic.Add("addr", rowItem["StreetAddress"] is DBNull ? string.Empty : rowItem["StreetAddress"].ToString());
                        vehicleDic.Add("stat", rowItem["VehicleStatus"] is DBNull ? string.Empty : rowItem["VehicleStatus"].ToString());

                        vehicleMain.Add(vehicleDic);

                    }
                }

                string json = string.Empty;
                if (vehicleMain.Count > 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = int.MaxValue;
                    json = js.Serialize(vehicleMain); string mapStr = "if (clientMapData.length > 0) { ShowMapFrameData(clientMapData);}";
                    string javascriptStr = string.Format("var clientMapData={0}; clientMapData  = eval(clientMapData ) ;{1} ", json, mapStr);
                    RadAjaxManager1.ResponseScripts.Add(javascriptStr);
                }

                //if (sn.User.MapType == VLF.MAP.MapType.LSD)
                //    RadAjaxManager1.ResponseScripts.Add("parent.frmVehicleMap.location.href='../frmLSDmap.aspx';");
                //else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                //    RadAjaxManager1.ResponseScripts.Add("parent.frmVehicleMap.location.href='../../MapVE/VEMap.aspx';");
                //else
                //    RadAjaxManager1.ResponseScripts.Add("parent.frmVehicleMap.location.href='../frmvehiclemap.aspx' ");
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", LoadMap--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }


        }

        private void ResolveLSD()
        {
            SaveShowCheckBoxes();
            string[] addresses = null;
            string strAddress = "";
            try
            {

                using (LocationMgr.Location location = new LocationMgr.Location())
                {
                    DataRow[] drArrAddress = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true");

                    foreach (DataRow dr in drArrAddress)
                    {
                        try
                        {


                            if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), ref strAddress), false))
                                if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), ref  strAddress), true))
                                {
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Cannot Resolve LSD address"));
                                }

                            dr["StreetAddress"] = strAddress;
                        }
                        catch
                        {
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ResolveLSD--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }


        private Int64 FindMax(Int64[] array)
        {

            Int64 maxValue = array[0];

            for (int i = 1; i < array.Length; i++)
            {

                if (array[i] > maxValue)

                    maxValue = array[i];

            }

            return maxValue;

        }


        protected void dgFleetInfo_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            try
            {
                if (dgFleetInfo.MasterTableView.SortExpressions.Count == 0)
                {
                    GridSortExpression expression1 = new GridSortExpression();
                    expression1.FieldName = "chkBoxShow";
                    expression1.SortOrder = GridSortOrder.Descending;
                    dgFleetInfo.MasterTableView.SortExpressions.AddSortExpression(expression1);
                }
            }
            catch{}
           BindFleetInfo(false);
           //RadAjaxManager1.ResponseScripts.Add("SetVehicleGridHeight()");
        }

        private void BindFleetInfo(bool isDataBind)
        {
            try
            {
                if ((sn.Map.DsFleetInfo != null) && (sn.Map.DsFleetInfo.Tables[0] != null))
                {
                    dgFleetInfo.DataSource = sn.Map.DsFleetInfo;
                }
                else
                {
                    dgFleetInfo.DataSource = null;
                }
                if (isDataBind) dgFleetInfo.DataBind();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_InitializeDataSource--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
        protected void dgFleetInfo_ItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == GridItemType.Header)
                {
                    CheckBox chkSelectAllVehicles = (CheckBox)e.Item.FindControl("chkSelectAllVehicles");
                    if (chkSelectAllVehicles != null)
                    {
                        chkSelectAllVehicles.Attributes.Add("onclick", "chkSelectAllVehicles_Click()");
                    }
                    return;
                }
                if (!(e.Item is GridDataItem)) return;


		    //if (sn.SelectedLanguage.Contains("fr"))   
                        //((GridDataItem)e.Item)["MyDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["MyDateTime"].Text).ToString("d/M/yyyy HH:mm:ss"); 
                ((GridDataItem)e.Item)["MyDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["MyDateTime"].Text).ToString(sn.User.DateFormat+" "+sn.User.TimeFormat); 

                //  Stopwatch watchFleet = new Stopwatch();

                //DataRow[] drCollections = null; replace
                if (sn.Map.DsFleetInfo != null)
                {

                    if (!(e.Item.DataItem is DataRowView)) return;
                    string VehicleStatus = string.Empty;
                    if (((DataRowView)e.Item.DataItem)["VehicleStatus"] != DBNull.Value )
                        VehicleStatus = ((DataRowView)e.Item.DataItem)["VehicleStatus"].ToString();

                    string customUrl = ((DataRowView)e.Item.DataItem)["CustomUrl"].ToString();
                    HyperLink hpl = (HyperLink)((GridDataItem)e.Item)["Description"].Controls[0];
                    hpl.NavigateUrl = customUrl;
                    hpl.ForeColor = Color.Blue;
                    if ((VehicleStatus == Resources.Const.VehicleStatus_Parked) || (VehicleStatus == Resources.Const.VehicleStatus_Parked + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Untethered) || (VehicleStatus == Resources.Const.VehicleStatus_Untethered + "*"))
                    {
                        ((GridDataItem)e.Item)["VehicleStatus"].ForeColor = Color.Red;
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Idling) || (VehicleStatus == Resources.Const.VehicleStatus_Idling + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Tethered) || (VehicleStatus == Resources.Const.VehicleStatus_Tethered + "*"))
                    {
                        ((GridDataItem)e.Item)["VehicleStatus"].ForeColor = Color.DarkOrange;
                    }

                    if ((VehicleStatus == Resources.Const.VehicleStatus_Moving) || (VehicleStatus == Resources.Const.VehicleStatus_Moving + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON + "*") || (VehicleStatus == Resources.Const.VehicleStatus_Ignition_ON))
                    {
                        ((GridDataItem)e.Item)["VehicleStatus"].ForeColor = Color.Green;
                    }



                    if (!sn.User.ControlEnable(sn, 41))
                    {
                        ((GridDataItem)e.Item)["History"].Visible = false;
                    }
                    else
                    {
                        ((GridDataItem)e.Item)["History"].Visible = true;
                        ((LinkButton)((GridDataItem)e.Item)["History"].Controls[0]).Attributes.Add("onclick", "return History_Click(" + e.Item.ItemIndex + ", this)");
                    }
                    Boolean chkBoxShow = false;
                     if (((DataRowView)e.Item.DataItem)["chkBoxShow"] != DBNull.Value )
                         chkBoxShow = (Boolean)((DataRowView)e.Item.DataItem)["chkBoxShow"];

                     CheckBox chkSelectVehicle = ((CheckBox)((GridDataItem)e.Item)["selectCheckBox"].FindControl("chkSelectVehicle"));
                     if (chkSelectVehicle != null)
                     {
                         chkSelectVehicle.Checked = chkBoxShow;
                         //string vehicleId = string.Empty;
                         //if (((DataRowView)e.Item.DataItem)["vehicleId"] != DBNull.Value)
                         //    vehicleId = ((DataRowView)e.Item.DataItem)["vehicleId"].ToString();
                         chkSelectVehicle.Attributes.Add("onclick", "chkSelectVehicle_Click(" + e.Item.ItemIndex + ", this)");
                     }

                     if (((GridDataItem)e.Item)["StreetAddress"].Text == Resources.Const.InvalidAddress_addressNA)
                     {
                         try
                         {
                             clsMap _m = new clsMap();
                             ((GridDataItem)e.Item)["StreetAddress"].Text = _m.ResolveStreetAddressNavteq(((GridDataItem)e.Item)["Latitude"].Text.ToString(), ((GridDataItem)e.Item)["Longitude"].Text.ToString());

                             string filter = string.Format("BoxId = '{0}'", ((GridDataItem)e.Item)["BoxId"].Text);
                             DataRow[] foundRows = sn.Map.DsFleetInfo.Tables[0].Select(filter);
                             for (int i = 0; i < foundRows.Length; i++)
                             {
                                 try
                                 {
                                     int index = sn.Map.DsFleetInfo.Tables[0].Rows.IndexOf(foundRows[i]);
                                     sn.Map.DsFleetInfo.Tables[0].Rows[index]["StreetAddress"] = ((GridDataItem)e.Item)["StreetAddress"].Text;
                                     sn.Map.DsFleetInfo.Tables[0].AcceptChanges();
                                 }
                                 catch (Exception Ex)
                                 {
                                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                                 }
                             }
                         }
                         catch { }
                     }

                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_InitializeRow--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void dgFleetInfo_ItemCommand(object sender, GridCommandEventArgs e)
        {

            if (!(e.Item is GridDataItem)) return;
            sn.History.VehicleId = Convert.ToInt64(((GridDataItem)e.Item).GetDataKeyValue("VehicleId"));
            SaveShowCheckBoxes();
            if (e.CommandName == "LSD")
            {
                string strAddress = "";
                try
                {
                    using (LocationMgr.Location location = new LocationMgr.Location())
                    {
                        foreach (DataRow dr in sn.Map.DsFleetInfo.Tables[0].Rows)
                        {
                            if (Convert.ToInt32(dr["VehicleId"]) == Convert.ToInt32(sn.History.VehicleId))
                            {
                                try
                                {

                                    if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), ref  strAddress), false))
                                        if (objUtil.ErrCheck(location.GetSpecialAddressLSD(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), ref  strAddress), true))
                                        {
                                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Cannot Resolve LSD address"));
                                        }

                                    dr["StreetAddress"] = strAddress;

                                }
                                catch (Exception Ex)
                                {
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                                    ExceptionLogger(trace);

                                }


                                BindFleetInfo(true);
                                System.Threading.Thread.Sleep(0);
                                break;
                            }
                        }
                    }
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_ButtonClick--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);

                }
            }
        }
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {

            if (sn == null || sn.UserID == null || sn.UserID == 0)
            {
                Session.Abandon();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session is null, SessionId:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " RadAjaxManager1_AjaxRequest Form:" + Page.GetType().Name + " IP:" + Request.ServerVariables["remote_addr"]));
                RadAjaxManager1.ResponseScripts.Add(" top.document.all('TopFrame').cols='0,*';window.open('../../Login.aspx','_top'); ");
                return;
            }

            if (e.Argument == "Rebind")
            {
                try
                {
                    if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                    {
                        DgFleetInfo_MultiFill_NewTZ(hidOrganizationHierarchyFleetId.Value.ToString());
                    }
                    else
                    {

                        if (sn.Map.SelectedFleetID != 0)
                        {
                            BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                            DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.Map.SelectedFleetID));
                        }
                        else
                        {
                            if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                            {
                                int fleetId = 0;
                                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                                if (fleetId > 0)
                                {
                                    DgFleetInfo_Fill_NewTZ(fleetId);
                                }
                            }
                            else
                            {
                                if (BarItemcboFleet.SelectedIndex > 0)
                                {
                                    DgFleetInfo_Fill_NewTZ(Convert.ToInt32(BarItemcboFleet.SelectedItem.Value));
                                }
                                else DgFleetInfo_Fill_NewTZ(-1);
                            }
                        }
                    }
                    BindFleetInfo(true);

                    //if (sn.Map.ReloadMap)
                    {
                        LoadMap();
                        sn.Map.ReloadMap = false;
                    }

                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);

                }
            }

            if (e.Argument == "RebindSelectedFleet")
            {
                RebindSelectedFleet();
            }

            if (e.Argument == "RebindOrganizationHierarchyFleet")
            {
                //RebindSelectedFleet();

                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                {
                    string fleetIds = hidOrganizationHierarchyFleetId.Value.ToString();

                    //if (fleetIds != null && fleetIds != string.Empty)
                    //{
                        try
                        {
                            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                            hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                            //BarItembtnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                            DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                            DefaultOrganizationHierarchyMultipleFleetIds = fleetIds;
                            //DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                            

                            BarItembtnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;

                            



                            sn.Map.DsFleetInfo = null;
                            sn.Map.SelectedMultiFleetIDs = fleetIds;

                            DgFleetInfo_MultiFill_NewTZ(fleetIds);
                            BindFleetInfo(true);
                            RadAjaxManager1.ResponseScripts.Add(" parent.frmVehicleMap.location.href='../frmvehiclemap.aspx'; ");

                        }

                        catch (NullReferenceException Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                            RedirectToLogin();
                        }

                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cboFleet_SelectedIndexChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);

                        }
                    //}
                }
                else
                {

                    int fleetId = 0;
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                    if (fleetId > 0)
                    {
                        try
                        {
                            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                            hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                            //BarItembtnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                            DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                            DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                            //OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                            BarItembtnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;

                            //hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);



                            sn.Map.DsFleetInfo = null;
                            sn.Map.SelectedFleetID = fleetId;

                            DgFleetInfo_Fill_NewTZ(fleetId);
                            BindFleetInfo(true);
                            RadAjaxManager1.ResponseScripts.Add(" parent.frmVehicleMap.location.href='../frmvehiclemap.aspx'; ");

                        }

                        catch (NullReferenceException Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                            RedirectToLogin();
                        }

                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cboFleet_SelectedIndexChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);

                        }
                    }
                }
            }

            if (e.Argument == "SelectAllVehicles")
            {
                SelectAllVehicles();
            }

            if (e.Argument == "AutoRefresh")
            {
                CheckBox chkAutoRefresh = (CheckBox)radToolBarMenu.FindItemByValue("AutoRefresh").FindControl("chkAutoRefresh");
                chkAutoUpdateChanged(chkAutoRefresh.Checked);
            }

            if (e.Argument == "LSD")
            {
                ResolveLSD();
                BindFleetInfo(true);
            }

            if (e.Argument == "ClearAllFilters")
            {
                foreach (GridColumn column in dgFleetInfo.MasterTableView.Columns)
                {
                    column.CurrentFilterFunction = GridKnownFunction.NoFilter;
                    column.CurrentFilterValue = string.Empty;
                }
                dgFleetInfo.MasterTableView.FilterExpression = string.Empty;
                dgFleetInfo.MasterTableView.Rebind();

            }
        }



        private void SelectAllVehicles()
        {
            try
            {
                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                {
                    sn.History.MultiFleetIDs = hidOrganizationHierarchyFleetId.Value.ToString();
                }
                else if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                {
                    int fleetId = 0;
                    int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                    sn.History.FleetId = fleetId;
                }
                else
                {
                    sn.History.FleetId = Convert.ToInt64(this.BarItemcboFleet.SelectedItem.Value);
                }


                sn.History.FromDate = DateTime.Now.ToString(sn.User.DateFormat);//sn.History.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                sn.History.ToDate = DateTime.Now.AddDays(1).ToString(sn.User.DateFormat);
                sn.History.FromHours = "0";
                sn.History.ToHours = "0";

                Boolean isSelectedAll = false;

                foreach (GridHeaderItem headerItem in dgFleetInfo.MasterTableView.GetItems(GridItemType.Header))
                {
                    CheckBox chk = (CheckBox)headerItem["selectCheckBox"].FindControl("chkSelectAllVehicles"); // Get the header checkbox 
                    if (chk != null)
                    {
                        isSelectedAll = chk.Checked;
                        break;
                    }
                }

                foreach (GridDataItem gridItem in dgFleetInfo.Items)
                {
                    if (!(gridItem is GridDataItem)) continue;
                    CheckBox chkSelectVehicle = (CheckBox)gridItem.FindControl("chkSelectVehicle");
                    if (chkSelectVehicle != null)
                    {
                        chkSelectVehicle.Checked = isSelectedAll;
                        DataRow[] drCollections = null;
                        drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId=" + gridItem.GetDataKeyValue("VehicleId").ToString());
                        if (drCollections != null && drCollections.Length > 0)
                        {
                            DataRow dRow = drCollections[0];
                            dRow["chkBoxShow"] = isSelectedAll;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_RowChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void RebindSelectedFleet()
        {
            try
            {
                sn.Map.DsFleetInfo = null;

                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                {
                    sn.Map.SelectedMultiFleetIDs = hidOrganizationHierarchyFleetId.Value;
                    DgFleetInfo_MultiFill_NewTZ(sn.Map.SelectedMultiFleetIDs);
                }
                else
                {
                    if (Convert.ToInt32(BarItemcboFleet.SelectedItem.Value) != -1)
                    {
                        sn.Map.SelectedFleetID = Convert.ToInt32(BarItemcboFleet.SelectedItem.Value);
                    }
                    DgFleetInfo_Fill_NewTZ(Convert.ToInt32(BarItemcboFleet.SelectedItem.Value));
                }
                BindFleetInfo(true);



                //if (sn.User.MapType == VLF.MAP.MapType.LSD)
                //    RadAjaxManager1.ResponseScripts.Add("parent.frmVehicleMap.location.href='../frmLSDmap.aspx';");
                //else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                //    RadAjaxManager1.ResponseScripts.Add("parent.frmVehicleMap.location.href='../../MapVE/VEMap.aspx'; ");
                //else if (sn.User.MapType != VLF.MAP.MapType.MapsoluteWeb)
                    RadAjaxManager1.ResponseScripts.Add(" parent.frmVehicleMap.location.href='../frmvehiclemap.aspx'; ");


            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", cboFleet_SelectedIndexChanged--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        protected void dgFleetInfo_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            GridSortExpression expression = new GridSortExpression();
            expression.FieldName = e.SortExpression;
            expression.SortOrder = e.OldSortOrder;

            dgFleetInfo.MasterTableView.SortExpressions.Clear();
            GridSortExpression expression1 = new GridSortExpression();
            expression1.FieldName = "chkBoxShow";
            expression1.SortOrder = GridSortOrder.Descending;
            dgFleetInfo.MasterTableView.SortExpressions.AddSortExpression(expression1);
            dgFleetInfo.MasterTableView.SortExpressions.AddSortExpression(expression);
            dgFleetInfo.MasterTableView.Rebind();  
        }

        [WebMethod]
        public static string SelectVehicle(string fleetID, string vehicleID, bool isChecked)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment"))
                {
                    sn.History.MultiFleetIDs = fleetID;
                }
                else if (fleetID != "-1")
                    sn.History.FleetId = Convert.ToInt64(fleetID);

                sn.History.FromDate = DateTime.Now.ToString(sn.User.DateFormat);//("MM/dd/yyyy");
                sn.History.ToDate = DateTime.Now.AddDays(1).ToString(sn.User.DateFormat);// ("MM/dd/yyyy");
                sn.History.FromHours = "0";
                sn.History.ToHours = "0";



                DataRow[] drCollections = null;
                drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId=" + vehicleID);
                if (drCollections != null && drCollections.Length > 0)
                {
                    DataRow dRow = drCollections[0];
                    dRow["chkBoxShow"] = isChecked;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SelectVehicle() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json,
            UseHttpGet = true, XmlSerializeString = false)]
        public static string MapIt2(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                return SaveShowCheckBoxes_1(vehicleIDs, true, sn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MapIt() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        [WebMethod]
        public static string MapIt(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                return SaveShowCheckBoxes_1(vehicleIDs, true, sn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MapIt() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }
        
        [WebMethod]
        public static string UpdatePosition(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                Int64 sessionTimeOut = 0;
                clsUtility objUtil = new clsUtility(sn); 

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataColumn dc;

                dc = new DataColumn("Freq", Type.GetType("System.Int64"));
                dc.DefaultValue = 0;
                dt.Columns.Add(dc);


                bool cmdSent = false;


                sn.MessageText = "";
                LocationMgr.Location dbl = new LocationMgr.Location();

                SaveShowCheckBoxes_1(vehicleIDs, false, sn);
                bool ShowTimer = false;
                //Delete old timeouts
                sn.Cmd.DtUpdatePositionFails.Rows.Clear();


                if (vehicleIDs == string.Empty) return "1";

                //Replace
                DataRow[] drArr = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
                if (drArr == null || drArr.Length == 0)
                {
                    return "1";
                }
                //{
                //    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                //    ShowErrorMessage();
                //    this.tblWait.Visible = false;
                //    return;
                //}

                //DumpBeforeCall(sn, string.Format("frmFleetInfoNew -- cmdUpdatePosition"));


#if NEW_SLS
              


               int[] boxid=new int[drArr.Length];
               short[] protocolType=new short[drArr.Length];
               short[] commMode=new short[drArr.Length];
               bool[] sent = new bool[drArr.Length];
               Int64[] timeOut=new Int64[drArr.Length];
               short[] results = new short[drArr.Length];
               string[] vehicles = new string[drArr.Length];
               

               int i=0;
               foreach (DataRow rowItem in drArr)
                {
                       boxid[i]=Convert.ToInt32(rowItem["BoxId"]);
                       protocolType[i]=-1;
                       commMode[i]=-1;
                       sent[i]=false;
                       timeOut[i]=0;
                       results[i]=0;
                       vehicles[i]=rowItem["Description"].ToString() ;
                       i++;
                }

                if (objUtil.ErrCheck(dbl.SendCommandToMultipleVehicles(sn.UserID, sn.SecId, DateTime.Now,boxid , Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "",false, ref protocolType  , ref commMode  , ref sent, ref timeOut,ref results), false))
                   if (objUtil.ErrCheck(dbl.SendCommandToMultipleVehicles(sn.UserID, sn.SecId, DateTime.Now,boxid , Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "",false, ref protocolType  , ref commMode  , ref sent, ref timeOut,ref results), true ))
                     {
                         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Send update position failed. User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                         return;
                     }


                for (i = 0; i < sent.Length; i++)
                {
                    if (!sent[i])
                    {
                        DataRow drErr;
                        drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                        drErr["VehicleDesc"] = vehicles[i];
                        drErr["Status"] = (string)base.GetLocalResourceValue("sn_MessageText_SendCommandFailedError") + ": " + vehicles[i]; 
                        sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                    }
                    else
                    {
                        sn.Cmd.ArrBoxId.Add(boxid[i]);
                        sn.Cmd.ArrProtocolType.Add(protocolType[i]);
                        sn.Cmd.ArrVehicle.Add(vehicles[i]);   
                        ShowTimer = true;
                    }

                }
                
                 sessionTimeOut = FindMax(timeOut);

                
#else

                SentinelFMBasePage basePage = new SentinelFMBasePage();
                foreach (DataRow rowItem in drArr)
                {
                    short ProtocolId = -1;
                    short CommModeId = -1;
                    string errMsg = "";
                    if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                        if (errMsg == "")
                        {
                            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.UpdatePosition), "", ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref errMsg))
                            {
                                if (errMsg != "")
                                    sn.MessageText = errMsg;
                                else
                                    sn.MessageText = GetLocalResourceValue("sn_MessageText_SendCommandFailedError") + ": " + rowItem["Description"];

                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:Map\frmFleetInfoNew"));

                                DataRow drErr;
                                drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                                drErr["VehicleDesc"] = rowItem["Description"];
                                drErr["Status"] = sn.MessageText;
                                sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                                rowItem["Updated"] = CommandStatus.CommTimeout;

                            }
                        }
                        else
                        {
                            sn.MessageText = errMsg;
                            DataRow drErr;
                            drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                            drErr["VehicleDesc"] = rowItem["Description"];
                            drErr["Status"] = sn.MessageText;
                            sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                            rowItem["Updated"] = CommandStatus.CommTimeout;
                        }


                    DataRow dr = dt.NewRow();
                    dr["Freq"] = sessionTimeOut;// sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                    dt.Rows.Add(dr);

                    rowItem["ProtocolId"] = ProtocolId;
                    sn.Cmd.ProtocolTypeId = ProtocolId;

                    if (cmdSent)
                        ShowTimer = true;
                    else
                    {

                        //Create pop up message
                        sn.MessageText = GetLocalResourceValue("sn_MessageText_SendCommandToVehicle1") + " :" + rowItem["Description"].ToString().TrimEnd() + " " + GetLocalResourceValue("sn_MessageText_SendCommandToVehicle2");

                        DataRow drErr;
                        drErr = sn.Cmd.DtUpdatePositionFails.NewRow();
                        drErr["VehicleDesc"] = rowItem["Description"];
                        drErr["Status"] = sn.MessageText;
                        sn.Cmd.DtUpdatePositionFails.Rows.Add(drErr);
                        rowItem["Updated"] = CommandStatus.CommTimeout;

                    }

                }


                try
                {
                    ds.Tables.Add(dt);
                    DataView dv = ds.Tables[0].DefaultView;
                    dv.Sort = "Freq" + " DESC";
                    sessionTimeOut = Convert.ToInt64(dv[0].Row[0]);
                    sn.Cmd.GetCommandStatusRefreshFreq = sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                }
                catch
                {
                    sn.Cmd.GetCommandStatusRefreshFreq = 2000;
                }


#endif

                if (ShowTimer)
                {


                    //this.tblWait.Visible = true;
                    //this.tblFleetActions.Visible = false;
                    sn.Cmd.GetCommandStatusRefreshFreq = sessionTimeOut > 0 ? Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000 : 2000;
                    string msg = string.Empty;
                    if (sessionTimeOut > 60)
                        msg = GetLocalResourceValue("lblUpdatePosition_Text_Minutes1") + " " + Convert.ToInt64(Math.Round(sessionTimeOut / 60.0)).ToString() + " " + GetLocalResourceValue("lblUpdatePosition_Text_Minutes2");
                    else
                        msg = GetLocalResourceValue("lblUpdatePosition_Text_Seconds1") + " " + sessionTimeOut + " " + GetLocalResourceValue("lblUpdatePosition_Text_Seconds2");

                    sn.Map.TimerStatus = true;
                    sn.Cmd.UpdatePositionSend = true;
                    //Response.Write("<script language='javascript'> parent.frmStatus.location.href='../frmTimerPosition.aspx' </script>");
                    return msg;

                }
                else
                {
                    return NoShowString; //Not Show
                }

            }


            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";

            }
        }

        [WebMethod]
        public static string UpdatePositionResult()
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                return CheckUpdatePositionResult(sn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePositionResult() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
        }

        public static string GetLocalResourceValue(string key)
        {
            string path = HttpContext.Current.Server.MapPath("App_LocalResources/frmFleetInfoNew.aspx");
            return clsAsynGenerateReport.GetResourceObject(path, key);
        }

        [WebMethod]
        public static string CancelUpdatePos()
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                clsUtility objUtil = new clsUtility(sn); 
                LocationMgr.Location dbl = new LocationMgr.Location();


                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), false))
                            if (objUtil.ErrCheck(dbl.CancelCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), sn.Cmd.ProtocolTypeId), true))
                            {
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:frmFleetInfoNew.aspx"));
                            }

                    }
                }

                sn.Map.TimerStatus = false;
            }

            catch (NullReferenceException ex)
            {
                sn.Map.TimerStatus = false;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: CancelUpdatePos() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                return "0";
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: CancelUpdatePos() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";

            } 
            return "1";
        }

        [WebMethod]
        public static string History(string vehicleID, string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                sn.History.VehicleId = Int64.Parse(vehicleID);
                SaveShowCheckBoxes_1(vehicleIDs, false, sn);
                sn.History.DsSelectedData = null;
		sn.History.FromDate = "";
	        sn.History.ToDate = "";
		sn.History.FromHours="";
		sn.History.ToHours="";
                sn.History.RedirectFromMapScreen = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePositionResult() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        //for alarm
        [WebMethod]
        public static string MapitByVehicleIDs(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                StringBuilder OldDateVehicleNames = new StringBuilder();
                string OldDateVehicleNamesStr = string.Empty;
                ArrayList vehicleMain = new ArrayList();
                string UnitOfMes = sn.User.UnitOfMes == 1 ? "km/h" : "mi/h";
                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["VehicleId"] != DBNull.Value &&
                        vehicleIDs.IndexOf(string.Format(",{0},", rowItem["VehicleId"].ToString())) >= 0)
                    {
                        Dictionary<string, string> vehicleDic = new Dictionary<string, string>();

                        string icon = string.Empty;

                        if (rowItem["IconTypeName"] != DBNull.Value && rowItem["OriginDateTime"] != DBNull.Value)
                        {
                            if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                            {
                                icon = "Grey" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                            }
                            else
                            {
                                if (Convert.ToBoolean(rowItem["chkPTO"]))
                                    icon = "Blue" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                                else if (rowItem["Speed"].ToString() != "0")
                                    icon = "Green" + rowItem["IconTypeName"].ToString().TrimEnd() + rowItem["MyHeading"].ToString().TrimEnd() + ".ico";
                                else
                                    icon = "Red" + rowItem["IconTypeName"].ToString().TrimEnd() + ".ico";
                            }
                        }

                        vehicleDic.Add("id", rowItem["VehicleId"] is DBNull ? string.Empty : rowItem["VehicleId"].ToString());
                        vehicleDic.Add("date", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                        vehicleDic.Add("lat", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                        vehicleDic.Add("lon", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                        vehicleDic.Add("desc", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                        vehicleDic.Add("icon", icon);
                        vehicleDic.Add("head", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                        vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                        vehicleDic.Add("spd", rowItem["CustomSpeed"] is DBNull ? string.Empty : rowItem["CustomSpeed"].ToString() + " " + UnitOfMes);
                        vehicleDic.Add("addr", rowItem["StreetAddress"] is DBNull ? string.Empty : rowItem["StreetAddress"].ToString());
                        vehicleDic.Add("stat", rowItem["VehicleStatus"] is DBNull ? string.Empty : rowItem["VehicleStatus"].ToString());


                        vehicleMain.Add(vehicleDic);

                        if (rowItem["OriginDateTime"] != DBNull.Value && rowItem["Description"] != DBNull.Value)
                        {
                            if (Convert.ToDateTime(rowItem["OriginDateTime"]) < System.DateTime.Now.AddMinutes(-sn.User.PositionExpiredTime))
                                OldDateVehicleNames.Append(rowItem["Description"].ToString().TrimEnd() + "\r");
                        }
                    }
                }
                if (OldDateVehicleNames.Length > 0)
                {
                    OldDateVehicleNamesStr = "\r" + OldDateVehicleNames.ToString();
                    //OldDateVehicleNamesStr =  OldDateVehicleNamesStr.Substring(0, OldDateVehicleNamesStr.Length - 1);
                }
                string vehicleMainStr = string.Empty;
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                if (vehicleMain.Count > 0)
                {
                    vehicleMainStr = js.Serialize(vehicleMain);
                }
                string[] retArray = null;
                retArray = new string[2];
                retArray[0] = vehicleMainStr;
                retArray[1] = OldDateVehicleNamesStr;
                return js.Serialize(retArray);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MapIt() Page:Map\frmFleetInfoNew"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";

        }   

        private void chkAutoUpdateChanged(Boolean Refresh)
        {
            try
            {
                if (Refresh && AutoRefreshTimer > 0)
                {
                    if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                    {
                        sn.Map.MapRefresh = Refresh;
                        sn.Map.SelectedMultiFleetIDs = hidOrganizationHierarchyFleetId.Value.ToString();

                        chkShowAutoPostBack = Refresh;
                        RefreshPosition();
                        RadAjaxManager1.ResponseScripts.Add("timeout_handles = window.setTimeout('SetReloadSetTimeOut()'," + AutoRefreshTimer.ToString() + ")");
                    }
                    else
                    {
                        int fleetId;
                        if (sn.User.LoadVehiclesBasedOn == "hierarchy")
                        {
                            int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                        }
                        else
                        {
                            fleetId = Convert.ToInt32(BarItemcboFleet.SelectedItem.Value);
                        }
                        if (fleetId != -1)
                        {

                            sn.Map.MapRefresh = Refresh;
                            sn.Map.SelectedFleetID = fleetId;

                            chkShowAutoPostBack = Refresh;
                            RefreshPosition();
                            RadAjaxManager1.ResponseScripts.Add("timeout_handles = window.setTimeout('SetReloadSetTimeOut()'," + AutoRefreshTimer.ToString() + ")");
                        }
                    }
                }
                else
                {
                    sn.Map.MapRefresh = false;
                    RefreshPosition();
                    RadAjaxManager1.ResponseScripts.Add("ClearReloadSetTimeOut();");
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--SetReloadSetTimeOut ---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
            }

        }

        protected void dgFleetInfo_Init(object sender, EventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
            {
                dgFleetInfo.Culture = System.Globalization.CultureInfo.CurrentUICulture;


                dgFleetInfo.Columns[1].HeaderStyle.Width = Unit.Pixel(83);
                dgFleetInfo.Columns[1].ItemStyle.Width = Unit.Pixel(83);

                dgFleetInfo.Columns[2].HeaderStyle.Width = Unit.Pixel(83);
                dgFleetInfo.Columns[2].ItemStyle.Width = Unit.Pixel(83);



                dgFleetInfo.Columns[4].HeaderStyle.Width = Unit.Pixel(140);
                dgFleetInfo.Columns[4].ItemStyle.Width = Unit.Pixel(140);

                dgFleetInfo.Columns[5].HeaderStyle.Width = Unit.Pixel(100);
                dgFleetInfo.Columns[5].ItemStyle.Width = Unit.Pixel(100);


                dgFleetInfo.Columns[7].HeaderStyle.Width = Unit.Pixel(135);
                dgFleetInfo.Columns[7].ItemStyle.Width = Unit.Pixel(135);

                dgFleetInfo.Columns[10].HeaderStyle.Width = Unit.Pixel(40);
                dgFleetInfo.Columns[10].ItemStyle.Width = Unit.Pixel(40);

                dgFleetInfo.Columns[12].HeaderStyle.Width = Unit.Pixel(70);
                dgFleetInfo.Columns[12].ItemStyle.Width = Unit.Pixel(70);
            }

            GridFilterMenu menu = dgFleetInfo.FilterMenu;
            RadMenuItem item = new RadMenuItem();
            //menu.Items.Add(item);
            //item.Text = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ClearThisFilter").ToString(); ;
            //item.Value = "ClearThisFilter";
            //item.Menu.OnClientItemClicking = "itemClicked";

            //item = new RadMenuItem();
            menu.Items.Insert(1, item);
            //item.Text = "<div style='width:120px;border-bottom:1px solid #66ccff;'>" + HttpContext.GetGlobalResourceObject("Const", "RadGrid_ClearAllFilters").ToString() + "</div>";
            item.Text = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ClearAllFilters").ToString();
            item.Value = "ClearAllFilters";
            item.Attributes["realCommand"] = "ClearAllFilters";
            item.Menu.OnClientItemClicking = "itemClicked";

            //item = new RadMenuItem();
            //menu.Items.Insert(2, item);
            //item.Text = "<div style='width:100px;border-bottom:1px solid gray;'></div>";
            //item.Value = "line";
            //item.Menu.OnClientItemClicking = "itemClicked";

            item = new RadMenuItem();
            menu.Items.Add(item);
            item.Text = "&nbsp;";
            item.Value = "Nothing";
            item.Menu.OnClientItemClicking = "itemClicked";

        }
        protected void radContextExportMenu_ItemClick(object sender, RadMenuEventArgs e)
        {
            dgFleetInfo.ExportSettings.ExportOnlyData = true;
            dgFleetInfo.ExportSettings.IgnorePaging = true;
            dgFleetInfo.ExportSettings.OpenInNewWindow = true;
            dgFleetInfo.ExportSettings.HideStructureColumns = true;
            dgFleetInfo.MasterTableView.GetColumn("selectCheckBox").Visible = false;
            dgFleetInfo.MasterTableView.GetColumn("History").Visible = false; 
            isExport = true;
            //dgFleetInfo.MasterTableView.AllowFilteringByColumn = false;
            if (e.Item.Value == "pdf")
            {
                dgFleetInfo.ExportSettings.Pdf.PaperSize = GridPaperSize.A4;
                dgFleetInfo.ExportSettings.Pdf.PageWidth = Unit.Pixel(1280);

                dgFleetInfo.MasterTableView.ExportToPdf();
            }
            if (e.Item.Value == "word")
            {
                dgFleetInfo.MasterTableView.ExportToWord();
            }
            if (e.Item.Value == "excel")
            {
                dgFleetInfo.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
                dgFleetInfo.MasterTableView.ExportToExcel();
            }
            if (e.Item.Value == "csv")
            {
                dgFleetInfo.ExportSettings.ExportOnlyData = false;
                dgFleetInfo.MasterTableView.ExportToCSV();
            }
            //dgFleetInfo.MasterTableView.GetColumn("selectCheckBox").Visible = true;
        }
        protected void dgFleetInfo_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFilteringItem && isExport)
            {
                e.Item.Visible = false;

            }
        }        
}
}