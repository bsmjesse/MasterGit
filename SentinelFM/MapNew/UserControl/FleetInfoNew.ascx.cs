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


namespace SentinelFM
{
    public partial class MapNew_UserControl_FleetInfoNew : System.Web.UI.UserControl
    {
        protected SentinelFMSession sn = null;
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
        public static string ErrorMessage = "";
        public static string start_characters_begin = "0:";
        public static string start_characters_end = "1:";
        clsUtility objUtil = null;
        bool isExport = false;
                protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //sn.User.MapType = VLF.MAP.MapType.LSD;

                System.Diagnostics.Trace.WriteLine("Page_Load->Thread ID:" + System.Threading.Thread.CurrentThread.GetHashCode().ToString());


                if (!sn.User.IsLSDEnabled)
                {
                    //dgFleetInfo.MasterTableView.Columns[11].Visible = false;
                    dgFleetInfo.MasterTableView.Columns.FindByUniqueName("LSD").Visible = false;
                }

                AutoRefreshTimer = sn.User.GeneralRefreshFrequency;
                BarItemcboFleet = (DropDownList)radToolBarMenu.Items.FindItemByText("barFleet").FindControl("cboFleet");
                if (!Page.IsPostBack)
                {
                    try
                    {
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

                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--SetReloadSetTimeOut ---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
                        ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
                        if (sn.Map.SelectedFleetID != 0)
                        {
                            BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                            DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                        }
                        else
                        {
                            if (sn.User.DefaultFleet != -1)
                            {
                                BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                                DgFleetInfo_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                                sn.Map.SelectedFleetID = sn.User.DefaultFleet;
                            }
                            else
                            {
                                DgFleetInfo_Fill(-1);
                                hidFilter.Value = "";
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

                    }

                    ErrorMessage = CheckUpdatePositionResult(sn);

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
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

            }

        }

        private void DgFleetInfo_Fill(int fleetId)
        {
            try
            {

                dsFleetInfo = new DataSet();
                string strPath = Server.MapPath("../Datasets/FleetInfo.xsd");

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
                ((SentinelFMBasePage)this.Page).ExceptionLogger("NullReferenceException ", Ex);
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", DgFleetInfo_Fill--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

            }
        }






        private void FindExistingPreference()
        {
            try
            {

                // Changes for TimeZone Feature start
                if (sn.User.NewFloatTimeZone < 0)
                    ViewState["TimeZone"] = "(GMT-" + sn.User.NewFloatTimeZone.ToString() + ")";
                else
                    ViewState["TimeZone"] = "(GMT+" + sn.User.NewFloatTimeZone.ToString() + ")";// Changes for TimeZone Feature end
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
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

            }

        }


        public void RefreshPosition()
        {
            try
            {

                DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));

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
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
                    sn.MessageText += GetLocalResourceValue("sn_MessageText_UpdatePositionForVehicle1") + ": ";
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", ShowUpdatePositionsTimeOuts--> User:" + sn.UserID.ToString() + " Form:"  ));
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
        public static string SaveShowCheckBoxes_1(string vehicleIDs, Boolean isCheckTime, SentinelFMSession snP)
        {
            StringBuilder OldDateVehicleNames = new StringBuilder();
            string OldDateVehicleNamesStr = string.Empty;
            ArrayList vehicleMain = new ArrayList();

            foreach (DataRow rowItem in snP.Map.DsFleetInfo.Tables[0].Rows)
            {
                rowItem["chkBoxShow"] = false;
                rowItem["Updated"] = false;
                if (rowItem["VehicleId"] != DBNull.Value &&
                    vehicleIDs.IndexOf(string.Format(",{0},", rowItem["VehicleId"].ToString())) >= 0)
                {
                    rowItem["chkBoxShow"] = true;
                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();

                    string icon = string.Empty;

                    if (rowItem["IconTypeName"] != DBNull.Value && rowItem["OriginDateTime"] != DBNull.Value )
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

                    vehicleDic.Add("id", rowItem["VehicleId"] is DBNull ? string.Empty : rowItem["VehicleId"].ToString());
                    vehicleDic.Add("date", rowItem["OriginDateTime"] is DBNull ? string.Empty : rowItem["OriginDateTime"].ToString());
                    vehicleDic.Add("lat", rowItem["Latitude"] is DBNull ? string.Empty : rowItem["Latitude"].ToString());
                    vehicleDic.Add("lon", rowItem["Longitude"] is DBNull ? string.Empty : rowItem["Longitude"].ToString());
                    vehicleDic.Add("desc", rowItem["Description"] is DBNull ? string.Empty : rowItem["Description"].ToString());
                    vehicleDic.Add("icon", icon);
                    vehicleDic.Add("head", rowItem["MyHeading"] is DBNull ? string.Empty : rowItem["MyHeading"].ToString());
                    vehicleDic.Add("chkPTO", rowItem["chkPTO"] is DBNull ? string.Empty : rowItem["chkPTO"].ToString());
                    vehicleDic.Add("spd", rowItem["Speed"] is DBNull ? string.Empty : rowItem["Speed"].ToString());
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
                        vehicleDic.Add("spd", rowItem["Speed"] is DBNull ? string.Empty : rowItem["Speed"].ToString());
                        vehicleDic.Add("addr", rowItem["StreetAddress"] is DBNull ? string.Empty : rowItem["StreetAddress"].ToString());
                        vehicleDic.Add("stat", rowItem["VehicleStatus"] is DBNull ? string.Empty : rowItem["VehicleStatus"].ToString());

                        vehicleMain.Add(vehicleDic);

                    }
                }

                string json = string.Empty;
                if (vehicleMain.Count > 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    json = js.Serialize(vehicleMain);
                

                    string mapStr = "if (clientMapData.length > 0) {clientMapData = clientMapData[0];  ShowMultipleAssets(clientMapData);}";
                    string javascriptStr = string.Format("var clientMapData={0}; clientMapData  = eval(clientMapData ) ;{1} ", json, mapStr);
                    RadAjaxManager1.ResponseScripts.Add(javascriptStr);
                }
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
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
            catch { }
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
                //  Stopwatch watchFleet = new Stopwatch();

                //DataRow[] drCollections = null; replace
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
                    if (((DataRowView)e.Item.DataItem)["chkBoxShow"] != DBNull.Value)
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
                                    ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
                    ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

                }
            }
        }
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "Rebind" || e.Argument == "RebindGridAndMap")
            {
                try
                {
                    if (sn.Map.SelectedFleetID != 0)
                    {
                        BarItemcboFleet.SelectedIndex = BarItemcboFleet.Items.IndexOf(BarItemcboFleet.Items.FindByValue(sn.Map.SelectedFleetID.ToString()));
                        DgFleetInfo_Fill(Convert.ToInt32(sn.Map.SelectedFleetID));
                    }
                    else
                    {
                        if (BarItemcboFleet.SelectedIndex > 0)
                        {
                            DgFleetInfo_Fill(Convert.ToInt32(BarItemcboFleet.SelectedItem.Value));
                        }
                        else DgFleetInfo_Fill(-1);
                    }
                    BindFleetInfo(true);

                    if (sn.Map.ReloadMap || e.Argument == "RebindGridAndMap")
                    {
                        LoadMap();
                        sn.Map.ReloadMap = false;
                    }

                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", Page_Load--Fleet Index---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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
                sn.History.FleetId = Convert.ToInt64(this.BarItemcboFleet.SelectedItem.Value);
                sn.History.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                sn.History.ToDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
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
                if (Convert.ToInt32(BarItemcboFleet.SelectedItem.Value) != -1)
                {
                    sn.Map.SelectedFleetID = Convert.ToInt32(BarItemcboFleet.SelectedItem.Value);
                }
                DgFleetInfo_Fill(Convert.ToInt32(BarItemcboFleet.SelectedItem.Value));
                BindFleetInfo(true);

                LoadMap();
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
                ((SentinelFMBasePage)this.Page).ExceptionLogger(trace);

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

        public static string GetLocalResourceValue(string key)
        {
            string path = HttpContext.Current.Server.MapPath("~/MapNew/UserControl/App_LocalResources/FleetInfoNew.ascx");
            return clsAsynGenerateReport.GetResourceObject(path, key);
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

        protected void dgFleetInfo_Init(object sender, EventArgs e)
        {
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

        public void RedirectToLogin()
        {

            Session.Abandon();
            //((SentinelFMBasePage)this.Page).RedirectToLogin();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScript", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            return;
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }
            objUtil = new clsUtility(sn);
        }

    }
}