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
using System.Globalization;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Telerik.Web.UI;
using System.Text;
namespace SentinelFM
{
    public partial class Map_New_frmFullScreenGrid : SentinelFMBasePage
    {
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";

        public string redirectURL;
        private DataSet dsFleetInfo;
        public VLF.MAP.ClientMapProxy map;
        public VLF.MAP.ClientMapProxy geoMap;
        public string strGeoMicroURL;
        public bool chkShowAutoPostBack = false;
        public int AutoRefreshTimer = 10000;
        public string strMapForm = "";

        public Label BarItemFleetTitle = null;
        public DropDownList BarItemcboFleet = null;
        public Label BarItemOHTitle = null;
        public Button BarItembtnOrganizationHierarchyNodeCode = null;
        public string errorLoad = "Failed to load data.";
        public string errorCancel = "Failed to cancel.";
        public string DispMapType = "";
        public static string ErrorMessage = "";
        bool isExport = false;

        public string DefaultOrganizationHierarchyFleetId = "0";
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";
        public bool MutipleUserHierarchyAssignment;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                string defaultnodecode = string.Empty;
                defaultnodecode = sn.User.PreferNodeCodes;

                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);

                BarItemFleetTitle = (Label)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("Label1");
                BarItemcboFleet = (DropDownList)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("cboFleet");
                BarItemOHTitle = (Label)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("lblOhTitle");
                BarItembtnOrganizationHierarchyNodeCode = (Button)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("btnOrganizationHierarchyNodeCode");
                if (sn.Map.DsFleetInfo != null && sn.Map.DsFleetInfo.Tables.Count>0)
                {
                    foreach (DataRow row in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        string dt = Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                        try
                        {
                            //Changes for TimeZone feature starts
                            row["OriginDateTime"] = DateTime.Parse(dt);
                            //Changes for TimeZone feature ends

                            //row["OriginDateTime"] = Convert.ToDateTime(dt);//Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));
                        }
                        catch (Exception ex)
                        { 
                        
                        }
                    }
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
                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                        hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;

                        if (MutipleUserHierarchyAssignment)
                        {
                            hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
							hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                            DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                        }
                    }
                    else
                    {
                        hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                        BarItembtnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                        DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode).ToString();
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    }
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                    BarItembtnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                    if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                        BarItembtnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
                    BarItemFleetTitle.Visible = false;
                    BarItemcboFleet.Visible = false;
                    BarItemOHTitle.Visible = true;
                    BarItembtnOrganizationHierarchyNodeCode.Visible = true;
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                }


                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;
                BarItemcboFleet = (DropDownList)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("cboFleet");
                if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                    strMapForm = "javascript:FullVEMap();";
                else
                    strMapForm = "javascript:FullMap();";



                if (!Page.IsPostBack)
                {

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref FleetForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                    GuiSecurity(this);
                    if (sn.Map.MapRefresh)
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
                    


                    //Get Vehicles Info
                    CboFleet_Fill();
                    string strUnitOfMes = sn.User.UnitOfMes == 1 ? " (km/h)" : " (mph)";
                    this.dgFleetInfo.MasterTableView.Columns.FindByUniqueName("CustomSpeed").HeaderText = this.dgFleetInfo.MasterTableView.Columns.FindByUniqueName("CustomSpeed").HeaderText
                        + strUnitOfMes;

                    FindExistingPreference();

                    try
                    {
                        if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                        {
                            DgFleetInfo_MultipleFill_NewTZ(hidOrganizationHierarchyFleetId.Value);
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



                    //--- Check Results after update position
                    ErrorMessage = CheckUpdatePositionResult(sn);




                    if (sn.Map.ReloadMap)
                    {
                        //LoadMap();
                        sn.Map.ReloadMap = false;
                    }

                    try
                    {
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
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load-> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
            string exportGrid = "Export grid";
            try
            {
                showFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ShowFilter").ToString();
                hideFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_HideFilter").ToString();
                exportGrid = HttpContext.GetGlobalResourceObject("Const", "RadGrid_Export").ToString();
            }
            catch { };
            imgExport.ToolTip = exportGrid;
        }

        protected void dgFleetInfo_DataBinding(object sender, EventArgs e)
        {            
            GridDateTimeColumn dateTimeColumn = (GridDateTimeColumn)dgFleetInfo.Columns.FindByUniqueName("MyDateTime");
            dateTimeColumn.DataFormatString = "{0:"+sn.User.DateFormat+" "+sn.User.TimeFormat+"}";//sn.User.DateFormat;  //Session["DataFormatString"];
        }


        public static string CheckUpdatePositionResult(SentinelFMSession sn)
        {
            string ret = string.Empty;
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

            //----------------------------------


            
            if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
            {
                ret = sn.MessageText;
                //replace
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", CboFleet_fill-> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }

        }

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

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        foreach (DataRow row in dsFleetInfo.Tables[0].Rows)
                        {
                            string dt = Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            row["OriginDateTime"] = Convert.ToDateTime(dt);//Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));
                        }
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
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                //if (dsFleetInfo != null && dsFleetInfo.Tables.Count > 0)
                //{
                //    foreach (DataRow row in dsFleetInfo.Tables[0].Rows)
                //    {
                //        if (row["OriginDateTime"] != null)
                //        {                            
                //            row["OriginDateTime"] = Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));//Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));
                //        }
                //    }
                //}
               
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill-> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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

                if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesLastKnownPositionInfoByLang_NewTZ(sn.UserID, sn.SecId, fleetId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                    {
                        foreach (DataRow row in dsFleetInfo.Tables[0].Rows)
                        {
                            string dt = Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            row["OriginDateTime"] = Convert.ToDateTime(dt);//Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));
                        }
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
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                //if (dsFleetInfo != null && dsFleetInfo.Tables.Count > 0)
                //{
                //    foreach (DataRow row in dsFleetInfo.Tables[0].Rows)
                //    {
                //        if (row["OriginDateTime"] != null)
                //        {                            
                //            row["OriginDateTime"] = Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));//Convert.ToDateTime(Convert.ToDateTime(row["OriginDateTime"].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat));
                //        }
                //    }
                //}

                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill-> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
        // Changes for TimeZone Feature end
        private void DgFleetInfo_MultipleFill(string fleetIds)
        {
            try
            {
                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../../Datasets/FleetInfo.xsd");
                dsFleetInfo.ReadXmlSchema(strPath);
                if (fleetIds == "-1" || fleetIds == "")
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

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
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill-> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
        // Changes for TimeZone Feature start
        private void DgFleetInfo_MultipleFill_NewTZ(string fleetIds)
        {
            try
            {
                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../../Datasets/FleetInfo.xsd");
                dsFleetInfo.ReadXmlSchema(strPath);
                if (fleetIds == "-1" || fleetIds == "")
                {
                    sn.Map.DsFleetInfo = dsFleetInfo;
                    return;
                }

                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

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
                sn.Map.DSFleetInfoGenerator(sn, ref dsFleetInfo);
                sn.Map.DsFleetInfo = dsFleetInfo;

            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill-> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }

        // Changes for TimeZone Feature end

        private void FindExistingPreference()
        {
            try
            {

                // Changes for TimeZone Feature start
                if (sn.User.NewFloatTimeZone < 0)
                    ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
                else
                    ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")";
            }
            // Changes for TimeZone Feature end
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




        private void RefreshPosition()
        {
            try
            {
                StringBuilder vehicleIDs = new StringBuilder();
                foreach (GridDataItem gridItem in dgFleetInfo.Items)
                {
                    if (!(gridItem is GridDataItem)) continue;
                    CheckBox chkSelectVehicle = (CheckBox)gridItem.FindControl("chkSelectVehicle");
                    if (chkSelectVehicle != null)
                    {
                        if (chkSelectVehicle.Checked)
                        {
                            vehicleIDs.Append("," + gridItem.GetDataKeyValue("VehicleId").ToString());
                        }
                    }
                }
                if (vehicleIDs.Length > 0) vehicleIDs.Append(",");
                SaveShowCheckBoxes_1(vehicleIDs.ToString(), sn);

                //Replace
                //SaveShowCheckBoxes();
                if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
                {
                    DgFleetInfo_MultipleFill_NewTZ(hidOrganizationHierarchyFleetId.Value);
                }
                else
                {
                    DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.Map.SelectedFleetID));
                }

           }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", CancelCommand--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
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
                        sn.MessageText += "\r" + rowItem["VehicleDesc"].ToString().TrimEnd();
                    }

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
                    sn.MessageText += GetLocalResourceValue("sn_MessageText_UpdatePositionForVehicle1") + ": ";
                    foreach (DataRow rowItem in foundRows)
                    {
                        sn.MessageText += "\r" + rowItem["VehicleDesc"].ToString().TrimEnd();
                    }

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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsTimeOuts--> User:" + sn.UserID.ToString() + " Form:frmFullScreenGrid.aspx"));
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




                bool InvalidExists = false;

                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                    {
                        if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                        {
                            InvalidExists = true;
                            break;
                        }
                    }
                }



                //Delay for resolving "Store Position"
                System.Threading.Thread.Sleep(2000);

                Boolean errorExist = false;
                if (InvalidExists)
                {
                    //Replace
                    //DgFleetInfo_Fill(Convert.ToInt32(this.cboFleet.SelectedItem.Value));

                    bool ShowTitle = false;
                    foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                    {
                        if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
                        {
                            if (Convert.ToDateTime(rowItem["LastCommunicatedDateTime"]) > Convert.ToDateTime(rowItem["OriginDateTime"]))
                            {

                                bool UpdatePositionFails = false;

                                foreach (DataRow rw in sn.Cmd.DtUpdatePositionFails.Rows)
                                {
                                    if (rw["VehicleDesc"].ToString().TrimEnd() == rowItem["VehicleDesc"].ToString().TrimEnd())
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

                                    sn.MessageText += "\r" + rowItem["Description"].ToString().TrimEnd() ;
                                    errorExist = true;
                                }
                            }
                        }
                    }
                }


                if (sn.MessageText.Length > 0 && errorExist) //&& (sn.MessageText.Substring(sn.MessageText.Length - 1, 1) == ","))
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsNotValid--> User:" + sn.UserID.ToString() + " Form:frmFullScreenGrid.aspx"));
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
        public static string SaveShowCheckBoxes_1(string vehicleIDs, SentinelFMSession snP)
        {

            ArrayList vehicleMain = new ArrayList();

            foreach (DataRow rowItem in snP.Map.DsFleetInfo.Tables[0].Rows)
            {
                rowItem["chkBoxShow"] = false;
                if (rowItem["VehicleId"] != DBNull.Value &&
                    vehicleIDs.IndexOf(string.Format(",{0},", rowItem["VehicleId"].ToString())) >= 0)
                {
                    rowItem["chkBoxShow"] = true;
                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                    vehicleDic.Add("OriginDateTime", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                    vehicleDic.Add("Latitude", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                    vehicleDic.Add("Longitude", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                    vehicleDic.Add("Description", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                    vehicleDic.Add("IconTypeName", rowItem["IconTypeName"] is DBNull ? string.Empty : rowItem["IconTypeName"].ToString());
                    vehicleDic.Add("MyHeading", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                    vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                    vehicleDic.Add("Speed", rowItem["Speed"] is DBNull ? string.Empty : rowItem["Speed"].ToString());
                    vehicleMain.Add(vehicleDic);
                }
            }
            string vehicleMainStr = string.Empty;
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = int.MaxValue;
            if (vehicleMain.Count > 0)
            {
                vehicleMainStr = js.Serialize(vehicleMain);
            }
            return vehicleMainStr;
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


        protected void dgFleetInfo_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {

        }
        protected void dgFleetInfo_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
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
                //replace
                // DataRow[] drCollections = null;
                if (sn.Map.DsFleetInfo != null)
                {

                    if (!(e.Item.DataItem is DataRowView)) return;
                    string VehicleStatus = string.Empty;
                    if (((DataRowView)e.Item.DataItem)["VehicleStatus"] != DBNull.Value)
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

                    
                    //replace
                    //e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString();
                    Boolean chkBoxShow = false;
                    if (((DataRowView)e.Item.DataItem)["chkBoxShow"] != DBNull.Value)
                        chkBoxShow = (Boolean)((DataRowView)e.Item.DataItem)["chkBoxShow"];
                    CheckBox chkSelectVehicle = ((CheckBox)((GridDataItem)e.Item)["selectCheckBox"].FindControl("chkSelectVehicle"));
                    if (chkSelectVehicle != null)
                    {
                        chkSelectVehicle.Checked = chkBoxShow;
                        chkSelectVehicle.Attributes.Add("onclick", "chkSelectVehicle_Click(" + e.Item.ItemIndex + ", this)");
                    }

                }
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgFleetInfo_InitializeRow--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);

            }
        }
        protected void dgFleetInfo_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
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
            catch { }
            BindFleetInfo(false);

        }
        protected void dgFleetInfo_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
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

        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "Rebind")
            {
                try
                {
                    if (sn.Map.SelectedFleetID != 0)
                    {
                        BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                        DgFleetInfo_Fill_NewTZ(Convert.ToInt32(sn.Map.SelectedFleetID));
                    }
                    else
                    {
                        if (BarItemcboFleet.SelectedIndex > 0)
                        {
                            DgFleetInfo_Fill_NewTZ(Convert.ToInt32(BarItemcboFleet.SelectedItem.Value));
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
                            }
                        }
                    }
                    BindFleetInfo(true);

                    if (sn.Map.ReloadMap)
                    {
                        //LoadMap();
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

            if (e.Argument == "SelectAllVehicles")
            {
                SelectAllVehicles();
            }

            if (e.Argument == "AutoRefresh")
            {
                sn.Map.CloseBigMap = "false";
                CheckBox chkAutoRefresh = (CheckBox)radToolBarMenu.FindItemByValue("AutoRefresh").FindControl("chkAutoRefresh");
                chkAutoUpdateChanged(chkAutoRefresh.Checked);
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

            RadAjaxManager1.ResponseScripts.Add("LoadMap()");
        }

        private void SelectAllVehicles()
        {
            try
            {
                sn.History.FleetId = Convert.ToInt64(BarItemcboFleet.SelectedItem.Value);

                sn.History.FromDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                sn.History.FromHours = "08";
                sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();



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


        public static string GetLocalResourceValue(string key)
        {
            string path = HttpContext.Current.Server.MapPath("App_LocalResources/frmFullScreenGrid.aspx");
            return clsAsynGenerateReport.GetResourceObject(path, key);
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

        private void RebindSelectedFleet()
        {
            try
            {

                sn.Map.ShowDefaultMap = true;

                sn.Map.DsFleetInfo = null;
                //Replace
                //dgFleetInfo.RootTable.Rows.Clear();


                if (Convert.ToInt32(BarItemcboFleet.SelectedItem.Value) != -1)
                {
                    sn.Map.SelectedFleetID = Convert.ToInt32(BarItemcboFleet.SelectedItem.Value);
                }
                DgFleetInfo_Fill_NewTZ(Convert.ToInt32(BarItemcboFleet.SelectedItem.Value));
                BindFleetInfo(true);
                //Replace
                //dgFleetInfo.ClearCachedDataSource();
                //dgFleetInfo.RebindDataSource();
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

        [WebMethod]
        public static string MapIt(string vehicleIDs)
        {
            SentinelFMSession sn;
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return "-1";
            if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

            try
            {
                sn.Map.CloseBigMap = "false";
                sn.MessageText = "";
                string mapRefresh = "";
                if (sn.Map.MapRefresh) mapRefresh = "1:";
                else mapRefresh = "0:";
                return mapRefresh + SaveShowCheckBoxes_1(vehicleIDs, sn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: MapIt() Page:Map\frmFullScreenGrid"));
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
                sn.Map.CloseBigMap = "false";
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                string updatePositionText = string.Empty;
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
                SaveShowCheckBoxes_1(vehicleIDs, sn);

                //Delete old timeouts
                sn.Cmd.DtUpdatePositionFails.Rows.Clear();

                bool ShowTimer = false;
                Int64 sessionTimeOut = 0;


                foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
                {
                    if (rowItem["chkBoxShow"].ToString().ToLower() == "true")
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

                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update position failed. User:" + sn.UserID.ToString() + " Web Method:UpdatePosition Form:frmFullScreenGrid.aspx"));
                                    string[] resulterror = new string[1];
                                    resulterror[0] = sn.MessageText;

                                    return js.Serialize(resulterror);
                                }
                            }
                            else
                            {
                                sn.MessageText = errMsg;
                                //Replace
                                //ShowErrorMessage();
                                string[] resulterror = new string[1];
                                resulterror[0] = errMsg;
                                return js.Serialize(resulterror);
                            }


                        DataRow dr = dt.NewRow();

                        if (sessionTimeOut > 0)
                            dr["Freq"] = Convert.ToInt64(Math.Round(sessionTimeOut / 15.0)) * 1000;
                        else
                            dr["Freq"] = 2000;

                        dt.Rows.Add(dr);

                        rowItem["ProtocolId"] = ProtocolId;
                        sn.Cmd.ProtocolTypeId = ProtocolId;

                        if (cmdSent)
                        {
                            ShowTimer = true;
                        }
                        else
                        {

                            //Create pop up message
                            if (sn.MessageText == "")
                                sn.MessageText = GetLocalResourceValue("sn_MessageText_SendCommandToVehicle1") + " :" + rowItem["Description"].ToString().TrimEnd() + " " + GetLocalResourceValue("sn_MessageText_SendCommandToVehicle2");
                            else
                                sn.MessageText = sn.MessageText + "\r" + GetLocalResourceValue("sn_MessageText_SendCommandToVehicle1") + " :" + rowItem["Description"].ToString().TrimEnd() + " " + GetLocalResourceValue("sn_MessageText_SendCommandToVehicle2");

                            //Replace
                            //ShowErrorMessage();

                        }

                        rowItem["Updated"] = Convert.ToInt16(CommandStatus.Sent);
                    }

                }


                if (ShowTimer)
                {

                    //this.dgFleetInfo.Visible = false;
                    //Replace
                    //this.tblWait.Visible = true;
                    //this.tblFleetActions.Visible = false;

                    DataRow[] drCollections = null;
                    drCollections = sn.Map.DsFleetInfo.Tables[0].Select("chkBoxShow=true", "", DataViewRowState.CurrentRows);
                    if (drCollections.Length == 1)
                    {
                        if (sessionTimeOut > 60)
                        {
                            Int64 SessionTime = Convert.ToInt64(Math.Round(sessionTimeOut / 60.0));
                            updatePositionText = GetLocalResourceValue("lblUpdatePosition_Text_Minutes1") + " " + SessionTime.ToString() + " " + GetLocalResourceValue("lblUpdatePosition_Text_Minutes2");
                        }
                        else if (sessionTimeOut == 0)
                            sn.Cmd.GetCommandStatusRefreshFreq = 15000;
                        else
                            updatePositionText = GetLocalResourceValue("lblUpdatePosition_Text_Seconds1") + " " + sessionTimeOut + " " + GetLocalResourceValue("lblUpdatePosition_Text_Seconds2");
                    }

                    try
                    {
                        ds.Tables.Add(dt);
                        DataView dv = ds.Tables[0].DefaultView;
                        dv.Sort = "Freq" + " DESC";
                        sn.Cmd.GetCommandStatusRefreshFreq = Convert.ToInt64(dv[0].Row[0]);
                    }
                    catch
                    {
                        sn.Cmd.GetCommandStatusRefreshFreq = 1000;
                    }

                    sn.Map.TimerStatus = true;
                    sn.Cmd.UpdatePositionSend = true;
                    
                    //Replace 
                    //Response.Write("<script language='javascript'> parent.frametimer.location.href='frmtimerpositionbigdetails.aspx' </script>");


                }
                string[] result = new string[3];
                result[0] = ShowTimer.ToString().ToLower();
                result[1] = updatePositionText;
                result[2] = sn.MessageText;
                return js.Serialize(result);
                //}
                //else
                //{

                //    sn.MessageText = (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle");
                //    ShowErrorMessage();
                //    this.tblWait.Visible = false;
                //    return;
                //}


            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePosition() Page:Map\frmFullScreenGrid"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
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
                                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " cannot Cancel command for User:" + sn.UserID.ToString() + " Form:frmFullScreenGrid.aspx"));
                            }

                    }
                }

                sn.Map.TimerStatus = false;
            }

            catch (NullReferenceException ex)
            {
                sn.Map.TimerStatus = false;
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: CancelUpdatePos() Page:Map\frmFullScreenGrid"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                return "0";
            }

            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: CancelUpdatePos() Page:Map\frmFullScreenGrid"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";

            }
            return "1";
        }

        private void chkAutoUpdateChanged(Boolean Refresh)
        {
            try
            {
                if (Refresh)
                {
                    if (Convert.ToInt32(BarItemcboFleet.SelectedItem.Value) != -1)
                    {

                        sn.Map.MapRefresh = Refresh;
                        sn.Map.SelectedFleetID = Convert.ToInt32(BarItemcboFleet.SelectedItem.Value);
                        chkShowAutoPostBack = Refresh;
                        RefreshPosition();
                        RadAjaxManager1.ResponseScripts.Add("timeout_handles = window.setTimeout('SetReloadSetTimeOut()'," + AutoRefreshTimer.ToString() + ")");
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: UpdatePositionResult() Page:Map\frmFullScreenGrid"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "";
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
                if (fleetID != "-1")
                    sn.History.FleetId = Convert.ToInt64(fleetID);
                sn.History.VehicleId = Convert.ToInt64(vehicleID);

                sn.History.FromDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                sn.History.FromHours = "08";
                sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();



                DataRow[] drCollections = null;
                drCollections = sn.Map.DsFleetInfo.Tables[0].Select("VehicleId='" + vehicleID + "'");
                if (drCollections != null && drCollections.Length > 0)
                {
                    DataRow dRow = drCollections[0];
                    dRow["chkBoxShow"] = isChecked;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: SelectVehicle() Page:Map\frmFullScreenGrid"));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                return "0";
            }
            return "1";
        }

        protected void dgFleetInfo_Init(object sender, EventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
            {
                dgFleetInfo.Culture = System.Globalization.CultureInfo.CurrentUICulture;

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

        protected void hidOrganizationHierarchyPostBack_Click(object sender, EventArgs e)
        {
            if (sn.User.LoadVehiclesBasedOn == "hierarchy" && MutipleUserHierarchyAssignment)
            {
                string fleetIds = hidOrganizationHierarchyFleetId.Value.ToString().Trim();

                DgFleetInfo_MultipleFill_NewTZ(fleetIds);
                
            }
            else
            {
                int fleetId = -1;
                int.TryParse(hidOrganizationHierarchyFleetId.Value.ToString(), out fleetId);
                DgFleetInfo_Fill_NewTZ(fleetId);
            }
            BindFleetInfo(true);
        }

}
}