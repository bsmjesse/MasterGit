using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Diagnostics; 
using VLF.MAP;
using VLF.CLS.Interfaces;
using VLF.Reports;
using System.IO;
using System.Threading ;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using VLF.CLS;

namespace SentinelFM
{

    public partial class History_New_frmHistDataGridExtended : SentinelFMBasePage
    {
        private DataSet dsHistory;
        //private System.Threading.Timer _timer;
        // 1 if timer callback is executing; otherwise 0
        int _inTimerCallback = 0;
        private Stopwatch watch = new Stopwatch();
        private Stopwatch Pagewatch = new Stopwatch();

        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        public string errorLoad = "Failed to load data.";
        public string errorCancel = "Failed to cancel.";
        public string DispMapType = "";
        bool isExport = false;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["FromMapScreen"] == null )
               this.cmdSendCommand.OnClientClick = "javascript: return SensorsInfo('" + sn.History.LicensePlate + "')";

            if (!Page.IsPostBack || sender == null)
            {
                Pagewatch.Reset();
                Pagewatch.Start();

                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistory, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                //replace
                //if (sn.History.Animated && sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                    //StartTimer();

                if (sn.History.DsSelectedData != null && sn.History.DsSelectedData.Tables.Count > 0 && sn.History.DsSelectedData.Tables[0].Rows.Count > 1)
                    this.cmdSendCommand.Enabled = false;
                else
                    this.cmdSendCommand.Enabled = true;

                dgStops.Visible = false;
                dgTrips.Visible = false;

                //Replace
                //cboRows.SelectedIndex = cboRows.Items.IndexOf(cboRows.Items.FindByValue(sn.History.DgVisibleRows.ToString()));
                cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue(sn.History.DgItemsPerPage.ToString()));
                //Replace
                //dgHistoryDetails.Height = Unit.Pixel(107 + Convert.ToInt32(dgHistoryDetails.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                this.dgHistoryDetails.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
                //Replace
                //dgStops.Height = Unit.Pixel(107 + Convert.ToInt32(dgStops.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                this.dgStops.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
                //Replace
                //dgTrips.Height = Unit.Pixel(107 + Convert.ToInt32(dgTrips.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                this.dgTrips.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

                if ((sn.History.FromDateTime != "") && (sn.History.ToDateTime != ""))
                {
                    //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                    //    sn.MapSolute.LoadDefaultMap(sn);
                    if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                    {
                        watch.Reset();
                        watch.Start();
                        dgStops_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                        watch.Stop();
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History Stop&Idling-->Database Call and Dataminding (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                        watch.Reset();
                        watch.Start();

                        //Replace
                        //dgStops.ClearCachedDataSource();
                        //dgStops.RebindDataSource();
                        watch.Stop();
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History Stop&Idling-->DataGrid Bindind (sec):" + watch.Elapsed.TotalSeconds));
                        dgHistoryDetails.Visible = false;
                        dgStops.Visible = true;
                    }
                    else if (sn.History.ShowTrips)
                    {
                        dgTrips_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                        //Replace
                        //dgTrips.ClearCachedDataSource();
                        //dgTrips.RebindDataSource();

                        dgHistoryDetails.Visible = false;
                        dgStops.Visible = false;
                        dgTrips.Visible = true;
                    }
                    else
                    {
                        if (sn.History.DsHistoryInfo == null || sn.History.DsHistoryInfo.Tables.Count == 0 || sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0)
                        {
                            dgHistory_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);

                            watch.Reset();
                            watch.Start();
                            //replace
                            //dgHistoryDetails.ClearCachedDataSource();
                            //dgHistoryDetails.RebindDataSource();
                            watch.Stop();
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->DataGrid Bindind (sec):" + watch.Elapsed.TotalSeconds));

                            dgHistoryDetails.Visible = true;
                            dgStops.Visible = false;
                        }
                        else
                        {
                            //Replace
                            //dgHistoryDetails.ClearCachedDataSource();
                            //dgHistoryDetails.RebindDataSource();
                            dgHistoryDetails.Visible = true;
                            dgStops.Visible = false;
                        }
                    }

                    //RadAjaxManager1.ResponseScripts.Add("parent.frmHisMap.location.href='../MapNew/frmHistMap.aspx'");
                }
                Pagewatch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->History Page Load (sec):" + Pagewatch.Elapsed.TotalSeconds));

                LoadVehicles();
            }

            DispMapType = "../MapNew/frmHistMap.aspx";

            try
            {
                showFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ShowFilter").ToString();
                hideFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_HideFilter").ToString();
            }
            catch { };
        }

        // Changes for TimeZone Feature start
        private void dgHistory_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();


                //dbh.GetVehicleStatus
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../../Datasets/Hist.xsd");

                watch.Reset();
                watch.Start();
                if (sn.History.VehicleId != 0)
                {
                    if (sn.History.Address == "")
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
                            //if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                            //    if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
                            {
                                sn.History.ImgPath = sn.Map.DefaultImgPath;
                                sn.Map.VehiclesMappings = "";
                                sn.Map.VehiclesToolTip = "";

                                if (RequestOverflowed)
                                {
                                    sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                                    ShowErrorMessage();
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                    }
                    else
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), true))
                            {
                                sn.History.ImgPath = sn.Map.DefaultImgPath;
                                sn.Map.VehiclesMappings = "";
                                sn.Map.VehiclesToolTip = "";
                                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                                //    sn.MapSolute.LoadDefaultMap(sn);
                                if (RequestOverflowed)
                                {
                                    sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                                    ShowErrorMessage();
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                    }

                    if (RequestOverflowed)
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                        //    sn.MapSolute.LoadDefaultMap(sn);

                        sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                        ShowErrorMessage();
                        return;
                    }

                    strrXML = new StringReader(xml);
                    if (xml == "")
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                        //    sn.MapSolute.LoadDefaultMap(sn);
                        return;
                    }

                    dsHistory.ReadXmlSchema(strPath);
                    dsHistory.ReadXml(strrXML);

                    xml = "";
                    ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                    //Get Vehicle IconType
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), false))
                        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), true))
                        {
                            return;
                        }

                    strrXML = new StringReader(xml);
                    if (xml == "")
                        return;

                    DataSet dsVehicle = new DataSet();
                    dsVehicle.ReadXml(strrXML);

                    sn.History.IconTypeName = dsVehicle.Tables[0].Rows[0]["IconTypeName"].ToString().TrimEnd();
                }
                else
                {
                    if (sn.History.Address == "")
                    {
                        foreach (DataRow rowItem in sn.History.DsHistoryVehicles.Tables[0].Rows)
                        {
                            DataSet ds = new DataSet();
                            xml = "";

                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, Convert.ToInt64(rowItem["VehicleId"]), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                                if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, Convert.ToInt64(rowItem["VehicleId"]), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                                    continue;

                            if (RequestOverflowed)
                                continue;

                            strrXML = new StringReader(xml);
                            if (xml == "")
                                continue;

                            ds.ReadXmlSchema(strPath);
                            ds.ReadXml(strrXML);
                            dsHistory.Merge(ds);
                        }
                    }
                    else
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByFleetIdByLangExtendedSearch_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByFleetIdByLangExtendedSearch_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), true))
                            {
                                sn.History.ImgPath = sn.Map.DefaultImgPath;
                                sn.Map.VehiclesMappings = "";
                                sn.Map.VehiclesToolTip = "";
                            }

                        strrXML = new StringReader(xml);
                        if (xml == "")
                            return;
                        dsHistory.ReadXmlSchema(strPath);
                        dsHistory.ReadXml(strrXML);
                    }
                }
                watch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Database Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                watch.Reset();
                watch.Start();


                ProcessHistoryData(ref dsHistory);

                sn.History.DsHistoryInfo = dsHistory;

                dgStops.Visible = false;
                dgHistoryDetails.Visible = true;
                watch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Data manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                {
                    //Replace
                    //string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    //strUrl = strUrl + "	var myname='Message';";
                    //strUrl = strUrl + " var w=370;";
                    //strUrl = strUrl + " var h=50;";
                    //strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    //strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    //strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                    //strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                    //strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                    //Response.Write(strUrl);
                    ShowErrorMessage();
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
        // Changes for TimeZone Feature end

        private void dgHistory_Fill(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();


                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../../Datasets/Hist.xsd");

                watch.Reset();
                watch.Start();
                if (sn.History.VehicleId != 0)
                {
                    if (sn.History.Address == "")
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
                            {
                                sn.History.ImgPath = sn.Map.DefaultImgPath;
                                sn.Map.VehiclesMappings = "";
                                sn.Map.VehiclesToolTip = "";

                                if (RequestOverflowed)
                                {
                                    sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                                    ShowErrorMessage();
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                    }
                    else
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtendedSearch(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), true))
                            {
                                sn.History.ImgPath = sn.Map.DefaultImgPath;
                                sn.Map.VehiclesMappings = "";
                                sn.Map.VehiclesToolTip = "";
                                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                                //    sn.MapSolute.LoadDefaultMap(sn);
                                if (RequestOverflowed)
                                {
                                    sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                                    ShowErrorMessage();
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                    }

                    if (RequestOverflowed)
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                        //    sn.MapSolute.LoadDefaultMap(sn);

                        sn.MessageText = (string)base.GetLocalResourceObject("lblTooManyRecordsMessage1Resource1") + ". " + (string)base.GetLocalResourceObject("lblTooManyRecordsMessage2Resource1");
                        ShowErrorMessage();
                        return;
                    }

                    strrXML = new StringReader(xml);
                    if (xml == "")
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                        //    sn.MapSolute.LoadDefaultMap(sn);
                        return;
                    }

                    dsHistory.ReadXmlSchema(strPath);
                    dsHistory.ReadXml(strrXML);

                    xml = "";
                    ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                    //Get Vehicle IconType
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), false))
                        if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, sn.History.VehicleId, ref xml), true))
                        {
                            return;
                        }

                    strrXML = new StringReader(xml);
                    if (xml == "")
                        return;

                    DataSet dsVehicle = new DataSet();
                    dsVehicle.ReadXml(strrXML);

                    sn.History.IconTypeName = dsVehicle.Tables[0].Rows[0]["IconTypeName"].ToString().TrimEnd();
                }
                else
                {
                    if (sn.History.Address == "")
                    {
                        foreach (DataRow rowItem in sn.History.DsHistoryVehicles.Tables[0].Rows)
                        {
                            DataSet ds = new DataSet();
                            xml = "";

                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, Convert.ToInt64(rowItem["VehicleId"]), Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                                if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, Convert.ToInt64(rowItem["VehicleId"]), Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                                    continue;

                            if (RequestOverflowed)
                                continue;

                            strrXML = new StringReader(xml);
                            if (xml == "")
                                continue;

                            ds.ReadXmlSchema(strPath);
                            ds.ReadXml(strrXML);
                            dsHistory.Merge(ds);
                        }
                    }
                    else
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByFleetIdByLangExtendedSearch(sn.UserID, sn.SecId, Convert.ToInt32(sn.History.FleetId), Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, sn.History.Address, ref xml, ref RequestOverflowed), true))
                            {
                                sn.History.ImgPath = sn.Map.DefaultImgPath;
                                sn.Map.VehiclesMappings = "";
                                sn.Map.VehiclesToolTip = "";
                            }

                        strrXML = new StringReader(xml);
                        if (xml == "")
                            return;
                        dsHistory.ReadXmlSchema(strPath);
                        dsHistory.ReadXml(strrXML);
                    }
                }
                watch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Database Call(sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                watch.Reset();
                watch.Start();


                ProcessHistoryData(ref dsHistory);

                sn.History.DsHistoryInfo = dsHistory;

                dgStops.Visible = false;
                dgHistoryDetails.Visible = true;
                watch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Data manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                {
                    //Replace
                    //string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    //strUrl = strUrl + "	var myname='Message';";
                    //strUrl = strUrl + " var w=370;";
                    //strUrl = strUrl + " var h=50;";
                    //strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    //strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    //strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                    //strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                    //strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                    //Response.Write(strUrl);
                    ShowErrorMessage();
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

        private string Heading(string heading)
        {
            return sn.Map.Heading(heading);
        }

        //Replace
        //protected void dgHistoryDetails_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        //{
        //    if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
        //       && !(sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle || sn.History.ShowTrips))
        //    {
        //        e.DataSource = sn.History.DsHistoryInfo;
        //    }
        //}

        //Replace
        //private void LoadSelectedVehiclesHistory()
        //{
        //    DataTable dt = sn.History.DsHistoryInfo.Tables[0].Clone();
        //    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
        //    {
        //        if (Convert.ToBoolean(rowItem["chkBoxShow"]))
        //            dt.ImportRow(rowItem);
        //    }
        //}

        //Replace
        //private void LoadSingleVehicleHistory()
        //{
        //    if ((!clsUtility.IsNumeric(sn.History.CarLatitude)) || (!clsUtility.IsNumeric(sn.History.CarLongitude)))
        //    {
        //        return;
        //    }

        //    DataTable dt = new DataTable();
        //    dt = sn.History.DsHistoryInfo.Tables[0].Clone();

        //    DataRow dr = dt.NewRow();
        //    dr["StreetAddress"] = sn.History.CarAddress;
        //    dr["Latitude"] = sn.History.CarLatitude;
        //    dr["Longitude"] = sn.History.CarLongitude;
        //    dr["Speed"] = sn.History.CarSpeed;
        //    dr["Heading"] = sn.History.Heading;
        //    dr["MyDateTime"] = sn.History.CarHistoryDate;

        //    dt.Rows.Add(dr);
        //}

        //private void LoadSingleVehicleStopHistory()
        //{
        //    if ((!clsUtility.IsNumeric(sn.History.CarLatitude))
        //          || (!clsUtility.IsNumeric(sn.History.CarLongitude)))
        //    {
        //        return;
        //    }

        //    DataTable dt = new DataTable();
        //    dt = sn.History.DsHistoryInfo.Tables["StopData"].Clone();

        //    DataRow dr = dt.NewRow();
        //    dr["Location"] = sn.History.CarAddress;
        //    dr["Latitude"] = sn.History.CarLatitude;
        //    dr["Longitude"] = sn.History.CarLongitude;
        //    dr["ArrivalDateTime"] = sn.History.StopDate;
        //    dt.Rows.Add(dr);
        //}

        //Replace
        //protected void dgHistoryDetails_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
        //{
        //    if (e.Column.Name == "MapIt")
        //        LoadSingleVehicleHistory();
        //}

        protected void dgHistoryDetails_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
        {
            sn.History.CarLatitude = e.Row.Cells.GetNamedItem("Latitude").Text;
            sn.History.CarLongitude = e.Row.Cells.GetNamedItem("Longitude").Text;
            sn.History.CarSpeed = e.Row.Cells.GetNamedItem("Speed").Text;
            sn.History.CarHistoryDate = e.Row.Cells.GetNamedItem("MyDateTime").Text;
            sn.History.CarMessageType = e.Row.Cells.GetNamedItem("BoxMsgInTypeName").Text;
            sn.History.CarAddress = e.Row.Cells.GetNamedItem("StreetAddress").Text;
            sn.History.OriginDateTime = e.Row.Cells.GetNamedItem("MyDateTime").Text;

            //Replace
            //dgHistoryDetails.ClientAction.RefreshModifiedControls();
        }

        // Changes for TimeZone Feature start
        private void dgStops_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId_NewTZ(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                string LicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();

                string xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, LicensePlate) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopFourthParamName, true.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopFifthParamName, sn.History.ReportstopDuration.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopSixthParamName, sn.History.ShowStops.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopSeventhParamName, sn.History.ShowIdle.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopEighthParamName, "3");

                ServerReports.Reports rpt = new ServerReports.Reports();

                bool RequestOverflowed = false;
                bool OutMaxOverflowed = false;
                int TotalSqlRecords = 0;
                int OutMaxRecords = 0;

                xml = "";

                if (objUtil.ErrCheck(rpt.GetXml_NewTZ(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), false))
                    if (objUtil.ErrCheck(rpt.GetXml_NewTZ(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), true))
                    {
                        if (RequestOverflowed)
                        {
                            sn.History.ImgPath = sn.Map.DefaultImgPath;
                            sn.Map.VehiclesMappings = "";
                            sn.Map.VehiclesToolTip = "";
                            return;
                        }
                        else
                        {
                            sn.History.ImgPath = sn.Map.DefaultImgPath;
                            sn.Map.VehiclesMappings = "";
                            sn.Map.VehiclesToolTip = "";
                            return;
                        }
                    }

                if (RequestOverflowed)
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                dsHistory.ReadXml(strrXML);

                if (dsHistory.Tables.IndexOf("StopData") == -1)
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = true;
                dsHistory.Tables["StopData"].Columns.Add(dc);

                foreach (DataRow rowItem in dsHistory.Tables["StopData"].Rows)
                {
                    // Date
                    if (rowItem["StopDuration"].ToString().TrimEnd() == "00:00:00")
                        //rowItem["StopDuration"] = VLF.CLS.Def.Const.blankValue;
                        rowItem["StopDuration"] = "";

                    rowItem["ArrivalDateTime"] = Convert.ToDateTime(rowItem["ArrivalDateTime"].ToString());

                    try
                    {
                        if (Convert.ToDateTime(rowItem["DepartureDateTime"]) == VLF.CLS.Def.Const.unassignedDateTime)
                            //rowItem["DepartureDateTime"] = VLF.CLS.Def.Const.blankValue;
                            rowItem["DepartureDateTime"] = "";
                        else
                            rowItem["DepartureDateTime"] = Convert.ToDateTime(rowItem["DepartureDateTime"].ToString());
                    }
                    catch { }
                }

                dgStops.Visible = true;
                dgHistoryDetails.Visible = false;

                sn.History.DsHistoryInfo = dsHistory;
                //sn.MapSolute.LoadVehicles(sn, dsHistory.Tables["StopData"], "HistoryStops");
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb && !sn.History.Animated)
                //    sn.MapSolute.LoadHistory(sn, dsHistory.Tables["StopData"], "HistoryStops");
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
        // Changes for TimeZone Feature end

        private void dgStops_Fill(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                string LicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();

                string xmlParams = ReportTemplate.MakePair(ReportTemplate.RpStopFirstParamName, LicensePlate) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopSecondParamName, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopThirdParamName, Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopFourthParamName, true.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopFifthParamName, sn.History.ReportstopDuration.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopSixthParamName, sn.History.ShowStops.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopSeventhParamName, sn.History.ShowIdle.ToString()) +
                    ReportTemplate.MakePair(ReportTemplate.RpStopEighthParamName, "3");

                ServerReports.Reports rpt = new ServerReports.Reports();

                bool RequestOverflowed = false;
                bool OutMaxOverflowed = false;
                int TotalSqlRecords = 0;
                int OutMaxRecords = 0;

                xml = "";

                if (objUtil.ErrCheck(rpt.GetXml(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), false))
                    if (objUtil.ErrCheck(rpt.GetXml(sn.UserID, sn.SecId, ServerReports.ReportTypes.Stop, xmlParams, ref xml, ref RequestOverflowed, ref TotalSqlRecords, ref OutMaxOverflowed, ref OutMaxRecords), true))
                    {
                        if (RequestOverflowed)
                        {
                            sn.History.ImgPath = sn.Map.DefaultImgPath;
                            sn.Map.VehiclesMappings = "";
                            sn.Map.VehiclesToolTip = "";
                            return;
                        }
                        else
                        {
                            sn.History.ImgPath = sn.Map.DefaultImgPath;
                            sn.Map.VehiclesMappings = "";
                            sn.Map.VehiclesToolTip = "";
                            return;
                        }
                    }

                if (RequestOverflowed)
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                dsHistory.ReadXml(strrXML);

                if (dsHistory.Tables.IndexOf("StopData") == -1)
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                dc.DefaultValue = true;
                dsHistory.Tables["StopData"].Columns.Add(dc);

                foreach (DataRow rowItem in dsHistory.Tables["StopData"].Rows)
                {
                    // Date
                    if (rowItem["StopDuration"].ToString().TrimEnd() == "00:00:00")
                        //rowItem["StopDuration"] = VLF.CLS.Def.Const.blankValue;
                        rowItem["StopDuration"] = "";

                    rowItem["ArrivalDateTime"] = Convert.ToDateTime(rowItem["ArrivalDateTime"].ToString());

                    try
                    {
                        if (Convert.ToDateTime(rowItem["DepartureDateTime"]) == VLF.CLS.Def.Const.unassignedDateTime)
                            //rowItem["DepartureDateTime"] = VLF.CLS.Def.Const.blankValue;
                            rowItem["DepartureDateTime"] = "";
                        else
                            rowItem["DepartureDateTime"] = Convert.ToDateTime(rowItem["DepartureDateTime"].ToString());
                    }
                    catch { }
                }

                dgStops.Visible = true;
                dgHistoryDetails.Visible = false;

                sn.History.DsHistoryInfo = dsHistory;
                //sn.MapSolute.LoadVehicles(sn, dsHistory.Tables["StopData"], "HistoryStops");
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb && !sn.History.Animated)
                //    sn.MapSolute.LoadHistory(sn, dsHistory.Tables["StopData"], "HistoryStops");
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

        //protected void dgStops_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        //{
        //    if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
        //       && (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
        //    {
        //        e.DataSource = sn.History.DsHistoryInfo.Tables["StopData"];
        //    }
        //}

        protected void dgStops_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
        {
            sn.Map.VehiclesToolTip = "";
            sn.Map.VehiclesMappings = "";

            sn.History.StopIndex = Convert.ToInt32(e.Row.Cells.GetNamedItem("StopIndex").Text);
            sn.History.CarLatitude = e.Row.Cells.GetNamedItem("Latitude").Text;
            sn.History.CarLongitude = e.Row.Cells.GetNamedItem("Longitude").Text;
            sn.History.StopStatus = e.Row.Cells.GetNamedItem("Remarks").Text;
            sn.History.StopDate = e.Row.Cells.GetNamedItem("ArrivalDateTime").Text;
            sn.History.StopDuration = e.Row.Cells.GetNamedItem("StopDuration").Text;
            sn.History.StopAddress = e.Row.Cells.GetNamedItem("Location").Text;
            sn.History.CarSpeed = "0";
            sn.History.OriginDateTime = "";
            sn.History.Heading = "";
            sn.History.ShowToolTip = false;
            sn.History.StopDurationVal = Convert.ToInt32(e.Row.Cells.GetNamedItem("StopDurationVal").Text);

            TimeSpan TripIndling;
            TripIndling = new TimeSpan(Convert.ToInt64(sn.History.StopDurationVal) * TimeSpan.TicksPerSecond);
            sn.History.StopDurationVal = TripIndling.TotalMinutes;
        }


        //replace
        //protected void dgStops_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
        //{
        //    if (e.Column.Name == "MapIt")
        //        LoadSingleVehicleStopHistory();
        //}

        protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgStops.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
            this.dgHistoryDetails.PageSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

            sn.History.DgItemsPerPage = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

            if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
            {
                BinddgStops(true);
                //dgStops.ClearCachedDataSource();
                //dgStops.RebindDataSource();
            }
            else
            {
                BinddgHistoryDetails(true);
                //dgHistoryDetails.ClearCachedDataSource();
                //dgHistoryDetails.RebindDataSource();
            }
        }


        //Replace
        //protected void dgHistoryDetails_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        //{
        //    if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
        //    {
        //        e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
        //        if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
        //            e.Layout.TextSettings.UseLanguage = "fr-FR";
        //    }
        //}

        //protected void dgStops_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        //{
        //    if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
        //    {
        //        e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
        //        if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
        //            e.Layout.TextSettings.UseLanguage = "fr-FR";
        //        else
        //            e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
        //    }
        //}

        //protected void dgHistoryDetails_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        //{
        //    try
        //    {
        //        ISNet.WebUI.WebGrid.WebGridCellCollection cells = e.Row.Cells;
        //        DataRow[] drCollections = null;
        //        if (sn.History.DsHistoryInfo != null)
        //        {
        //            drCollections = sn.History.DsHistoryInfo.Tables[0].Select("dgKey like '" + e.Row.KeyValue + "'");

        //            foreach (DataRow rowItem in drCollections)
        //            {
        //                e.Row.Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
        //            }
        //        }
        //    }
        //    catch { }
        //}

        //protected void dgStops_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        //{
        //    try
        //    {
        //        DataRow[] drCollections = null;
        //        if (sn.History.DsHistoryInfo != null)
        //        {
        //            drCollections = sn.History.DsHistoryInfo.Tables[0].Select("StopIndex like '" + e.Row.KeyValue + "'");

        //            foreach (DataRow rowItem in drCollections)
        //            {
        //                e.Row.Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
        //            }
        //        }
        //    }
        //    catch { }
        //}

        //Replace
        //protected void cboRows_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    sn.History.DgVisibleRows = Convert.ToInt32(cboRows.SelectedItem.Value);
        //    dgHistoryDetails.Height = Unit.Pixel(107 + Convert.ToInt32(dgHistoryDetails.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
        //    dgStops.Height = Unit.Pixel(107 + Convert.ToInt32(dgStops.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
        //}

        private void ShowErrorMessage()
        {
            RadAjaxManager1.ResponseScripts.Add("ShowErrorMessage('" + GetScriptEscapeString(sn.MessageText) + "')");
        }

        private void SaveShowCheckBoxes()
        {
            try
            {
                if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                {

                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                        rowItem["chkBoxShow"] = false;
                    foreach (GridDataItem gridItem in dgStops.Items)
                    {
                        CheckBox chkSelectVehicle = (CheckBox)gridItem.FindControl("chkSelectStops");
                        if (chkSelectVehicle != null && chkSelectVehicle.Checked)
                        {
                            string keyValue = gridItem.GetDataKeyValue("StopIndex").ToString();
                            DataRow[] drCollections = null;
                            drCollections = sn.History.DsHistoryInfo.Tables["StopData"].Select("StopIndex='" + keyValue + "'");
                            if (drCollections != null && drCollections.Length > 0)
                            {
                                DataRow dRow = drCollections[0];
                                dRow["chkBoxShow"] = true;
                            }

                        }
                    }
                    //foreach (string keyValue in dgStops.RootTable.GetCheckedRows())
                    //{
                    //    DataRow[] drCollections = null;
                    //    drCollections = sn.History.DsHistoryInfo.Tables["StopData"].Select("StopIndex='" + keyValue + "'");
                    //    if (drCollections != null && drCollections.Length > 0)
                    //    {
                    //        DataRow dRow = drCollections[0];
                    //        dRow["chkBoxShow"] = true;
                    //    }
                    //}
                }
                else if (sn.History.ShowTrips)
                {
                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[1].Rows)
                        rowItem["chkBoxShow"] = false;

                    foreach (GridDataItem gridItem in dgTrips.Items)
                    {
                        //if (gridItem.OwnerTableView.Name == "Trips")
                        {
                            foreach (GridDataItem childGridItem in gridItem.ChildItem.NestedTableViews[0].Items)
                            {
                                CheckBox chkSelectVehicle = (CheckBox)childGridItem.FindControl("chkSelectTrips");
                                HiddenField hiddgKey = (HiddenField)childGridItem.FindControl("hiddgKey");
                                if (chkSelectVehicle != null && chkSelectVehicle.Checked)
                                {
                                    string keyValue = hiddgKey.Value;
                                    DataRow[] drCollections = null;
                                    drCollections = sn.History.DsHistoryInfo.Tables[1].Select("dgKey='" + keyValue + "'");
                                    if (drCollections != null && drCollections.Length > 0)
                                    {
                                        DataRow dRow = drCollections[0];
                                        dRow["chkBoxShow"] = true;
                                    }

                                }
                            }
                        }
                    }

                    //foreach (string keyValue in dgTrips.RootTable.ChildTables[0].GetCheckedRows())
                    //{
                    //    DataRow[] drCollections = null;
                    //    drCollections = sn.History.DsHistoryInfo.Tables[1].Select("dgKey='" + keyValue + "'");
                    //    if (drCollections != null && drCollections.Length > 0)
                    //    {
                    //        DataRow dRow = drCollections[0];
                    //        dRow["chkBoxShow"] = true;
                    //    }
                    //}
                }
                else
                {
                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
                        rowItem["chkBoxShow"] = false;

                    foreach (GridDataItem gridItem in dgHistoryDetails.Items)
                    {
                        CheckBox chkSelectVehicle = (CheckBox)gridItem.FindControl("chkSelectHistoryDetails");
                        if (chkSelectVehicle != null && chkSelectVehicle.Checked)
                        {
                            string keyValue = gridItem.GetDataKeyValue("dgKey").ToString();
                            DataRow[] drCollections = null;
                            drCollections = sn.History.DsHistoryInfo.Tables[0].Select("dgKey='" + keyValue + "'");
                            if (drCollections != null && drCollections.Length > 0)
                            {
                                DataRow dRow = drCollections[0];
                                dRow["chkBoxShow"] = true;
                            }
                        }
                    }

                    //foreach (string keyValue in dgHistoryDetails.RootTable.GetCheckedRows())
                    //{
                    //    DataRow[] drCollections = null;
                    //    drCollections = sn.History.DsHistoryInfo.Tables[0].Select("dgKey='" + keyValue + "'");
                    //    if (drCollections != null && drCollections.Length > 0)
                    //    {
                    //        DataRow dRow = drCollections[0];
                    //        dRow["chkBoxShow"] = true;
                    //    }
                    //}
                }
            }
            catch { }
        }

        protected void cmdMapSelected_Click(object sender, EventArgs e)
        {
            try
            {
                SaveShowCheckBoxes();
                LoadVehicles();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }

        }


        //Replace
        //private void StartTimer()
        //{
        //    //_timer = new System.Threading.Timer(new TimerCallback(this.TimerCallback), null, sn.History.MapAnimationSpeed, sn.History.MapAnimationSpeed);
        //}

        //
        // if the timer fires frequently or the callback runs for a long period, you may
        // want to prevent two threads from calling it concurrently
        //

        //Replace
        //void TimerCallback(object state)
        //{
        //    if (Interlocked.Exchange(ref _inTimerCallback, 1) != 0)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        LoadMapsoluteAnimated();
        //    }
        //    finally
        //    {
        //        Interlocked.Exchange(ref _inTimerCallback, 0);
        //    }
        //}
        //
        // Before the AppDomain is shutdown, the timer must be disposed.  Otherwise,
        // the underlying native timer may crash the process if it fires and attempts
        // to call into the unloaded AppDomain.  In a multi-threaded environment,
        // you may need to use synchronization to ensure the timer is disposed at most once.
        //

        //internal void DisposeTimer()
        //{
        //}


        //Replace
        //private void LoadMapsoluteAnimated()
        //{
        //    if (sn.History.DsHistoryInfo == null || sn.History.DsHistoryInfo.Tables.Count == 0 || sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0)
        //        return;

        //    DataTable dt = sn.History.DsHistoryInfo.Tables[0].Clone();

        //    if (ViewState["dateTo"] == null)
        //    {
        //        ViewState["dateTo"] = Convert.ToDateTime(sn.History.DsHistoryInfo.Tables[0].Rows[0]["OriginDateTime"].ToString());
        //        ViewState["dateFrom"] = Convert.ToDateTime(sn.History.DsHistoryInfo.Tables[0].Rows[sn.History.DsHistoryInfo.Tables[0].Rows.Count - 1]["OriginDateTime"].ToString());
        //    }

        //    string sSelect = "";

        //    ViewState["dateFrom"] = Convert.ToDateTime(ViewState["dateFrom"]).AddMinutes(sn.History.MapAnimationHistoryInterval);
        //    DataRow[] drArr;

        //    if (Convert.ToDateTime(ViewState["dateFrom"]) < Convert.ToDateTime(ViewState["dateTo"]))
        //    {
        //        sSelect = "OriginDateTime > '" + Convert.ToDateTime(ViewState["dateFrom"]).AddMinutes(-sn.History.MapAnimationHistoryInterval).ToString() + "' and  OriginDateTime <'" + Convert.ToDateTime(ViewState["dateFrom"]).ToString() + "'";
        //        drArr = sn.History.DsHistoryInfo.Tables[0].Select(sSelect);
        //    }
        //    else
        //    {
        //        DisposeTimer();
        //        sSelect = "OriginDateTime > '" + Convert.ToDateTime(ViewState["dateFrom"]).AddMinutes(-sn.History.MapAnimationHistoryInterval) + "'";
        //        drArr = sn.History.DsHistoryInfo.Tables[0].Select(sSelect);
        //    }

        //    foreach (DataRow dr in drArr)
        //        dt.ImportRow(dr);
        //}

        // Changes for TimeZone Feature start
        private void dgTrips_Fill_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                string LicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();

                xml = "";
                Int16 sensorNum = sn.History.TripSensor;

                ServerReports.Reports rpt = new ServerReports.Reports();
                if (objUtil.ErrCheck(rpt.GetTripsSummaryDataByLicensePlate_NewTZ(sn.UserID, sn.SecId, LicensePlate, sensorNum, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), false))
                    if (objUtil.ErrCheck(rpt.GetTripsSummaryDataByLicensePlate_NewTZ(sn.UserID, sn.SecId, LicensePlate, sensorNum, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);
                dsHistory.ReadXmlSchema(Server.MapPath("../../Datasets/dstTripSummaryPerVehicle.xsd"));
                dsHistory.ReadXml(strrXML);

                if (dsHistory.Tables.Count > 0)
                {
                    DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                    dc.DefaultValue = true;
                    dsHistory.Tables[0].Columns.Add(dc);

                    dc = new DataColumn("TripId", Type.GetType("System.Int32"));
                    dc.DefaultValue = -1;
                    dsHistory.Tables[0].Columns.Add(dc);

                    int tripId = 0;
                    foreach (DataRow dr in dsHistory.Tables[0].Rows)
                    {
                        dr["TripId"] = tripId;
                        tripId++;
                    }
                }

                sn.History.DsHistoryInfo = dsHistory;
                GetTripDetails_NewTZ(VehicleId, strFromDate, strToDate);

                dgStops.Visible = false;
                dgHistoryDetails.Visible = false;
                dgTrips.Visible = true;
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
        // Changes for TimeZone Feature end

        private void dgTrips_Fill(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";

                DataSet ds = new DataSet();
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        sn.History.ImgPath = sn.Map.DefaultImgPath;
                        sn.Map.VehiclesMappings = "";
                        sn.Map.VehiclesToolTip = "";
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);
                ds.ReadXml(strrXML);

                string LicensePlate = ds.Tables[0].Rows[0]["LicensePlate"].ToString();

                xml = "";
                Int16 sensorNum = sn.History.TripSensor;

                ServerReports.Reports rpt = new ServerReports.Reports();
                if (objUtil.ErrCheck(rpt.GetTripsSummaryDataByLicensePlate(sn.UserID, sn.SecId, LicensePlate, sensorNum, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), ref xml), false))
                    if (objUtil.ErrCheck(rpt.GetTripsSummaryDataByLicensePlate(sn.UserID, sn.SecId, LicensePlate, sensorNum, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    sn.History.ImgPath = sn.Map.DefaultImgPath;
                    sn.Map.VehiclesMappings = "";
                    sn.Map.VehiclesToolTip = "";
                    return;
                }

                strrXML = new StringReader(xml);
                dsHistory.ReadXmlSchema(Server.MapPath("../../Datasets/dstTripSummaryPerVehicle.xsd"));
                dsHistory.ReadXml(strrXML);

                if (dsHistory.Tables.Count > 0)
                {
                    DataColumn dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                    dc.DefaultValue = true;
                    dsHistory.Tables[0].Columns.Add(dc);

                    dc = new DataColumn("TripId", Type.GetType("System.Int32"));
                    dc.DefaultValue = -1;
                    dsHistory.Tables[0].Columns.Add(dc);

                    int tripId = 0;
                    foreach (DataRow dr in dsHistory.Tables[0].Rows)
                    {
                        dr["TripId"] = tripId;
                        tripId++;
                    }
                }

                sn.History.DsHistoryInfo = dsHistory;
                GetTripDetails(VehicleId, strFromDate, strToDate);

                dgStops.Visible = false;
                dgHistoryDetails.Visible = false;
                dgTrips.Visible = true;
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


        //Replace
        //protected void dgTrips_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        //{
        //    if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
        //    && (sn.History.ShowTrips))
        //    {
        //        e.DataSource = sn.History.DsHistoryInfo;
        //    }
        //}

        // Changes for TimeZone Feature start
        private void GetTripDetails_NewTZ(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../../Datasets/Hist.xsd");

                if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                    if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsHistory.ReadXmlSchema(strPath);
                dsHistory.ReadXml(strrXML);
                ProcessHistoryData(ref dsHistory);

                dsHistory.Tables[0].TableName = "TripDetails";


                DataColumn dcTripID = new DataColumn();
                dcTripID.DataType = Type.GetType("System.Int32");
                dcTripID.AllowDBNull = true;
                dcTripID.Caption = "TripId";
                dcTripID.ColumnName = "TripId";
                dsHistory.Tables[0].Columns.Add(dcTripID);

                DataTable dtTemp = dsHistory.Tables[0].Copy();
                sn.History.DsHistoryInfo.Tables.Add(dtTemp);

                foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["Table"].Rows)
                {
                    DataRow[] drArr = sn.History.DsHistoryInfo.Tables["TripDetails"].Select("OriginDateTime >= '" + dr["DepartureTime"] + "' and OriginDateTime<='" + dr["ArrivalTime"] + "'");

                    for (int i = 0; i <= drArr.GetUpperBound(0); i++)
                        drArr[i]["TripId"] = dr["TripId"];
                }

                DataColumn parentCol = sn.History.DsHistoryInfo.Tables["Table"].Columns["TripId"];
                DataColumn childCol = sn.History.DsHistoryInfo.Tables["TripDetails"].Columns["TripId"];

                DataRelation relTripDetails = new DataRelation("relTripDetails", parentCol, childCol);
                sn.History.DsHistoryInfo.Relations.Add(relTripDetails);
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
        // Changes for TimeZone Feature start

        private void GetTripDetails(Int64 VehicleId, string strFromDate, string strToDate)
        {
            try
            {
                DataSet dsHistory = new DataSet();
                StringReader strrXML = null;

                string xml = "";
                ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                string strPath = Server.MapPath("../../Datasets/Hist.xsd");

                if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                    if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                strrXML = new StringReader(xml);
                dsHistory.ReadXmlSchema(strPath);
                dsHistory.ReadXml(strrXML);
                ProcessHistoryData(ref dsHistory);

                dsHistory.Tables[0].TableName = "TripDetails";


                DataColumn dcTripID = new DataColumn();
                dcTripID.DataType = Type.GetType("System.Int32");
                dcTripID.AllowDBNull = true;
                dcTripID.Caption = "TripId";
                dcTripID.ColumnName = "TripId";
                dsHistory.Tables[0].Columns.Add(dcTripID);

                DataTable dtTemp = dsHistory.Tables[0].Copy();
                sn.History.DsHistoryInfo.Tables.Add(dtTemp);

                foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["Table"].Rows)
                {
                    DataRow[] drArr = sn.History.DsHistoryInfo.Tables["TripDetails"].Select("OriginDateTime >= '" + dr["DepartureTime"] + "' and OriginDateTime<='" + dr["ArrivalTime"] + "'");

                    for (int i = 0; i <= drArr.GetUpperBound(0); i++)
                        drArr[i]["TripId"] = dr["TripId"];
                }

                DataColumn parentCol = sn.History.DsHistoryInfo.Tables["Table"].Columns["TripId"];
                DataColumn childCol = sn.History.DsHistoryInfo.Tables["TripDetails"].Columns["TripId"];

                DataRelation relTripDetails = new DataRelation("relTripDetails", parentCol, childCol);
                sn.History.DsHistoryInfo.Relations.Add(relTripDetails);
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

        private void ProcessHistoryData(ref DataSet dsHistory)
        {
            int RowsCount = dsHistory.Tables[0].Rows.Count - 1;
            sn.MessageText = "";

            if (dsHistory.Tables[0].Columns.IndexOf("StreetAddress") == -1)
            {
                DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                dsHistory.Tables[0].Columns.Add(colStreetAddress);
            }

            DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
            //DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.Object"));
            
            dsHistory.Tables[0].Columns.Add(colDateTime);

            // Show Heading
            DataColumn dc = new DataColumn();
            dc.ColumnName = "MyHeading";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsHistory.Tables[0].Columns.Add(dc);

            // DataGrid Key
            dc = new DataColumn();
            dc.ColumnName = "dgKey";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsHistory.Tables[0].Columns.Add(dc);

            // CustomUrl
            dc = new DataColumn();
            dc.ColumnName = "CustomUrl";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsHistory.Tables[0].Columns.Add(dc);

            dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
            dc.DefaultValue = true;
            dsHistory.Tables[0].Columns.Add(dc);

            int i = 0;
            string strStreetAddress = "";

            UInt64 checkBit = 0;
            Int16 bitnum = 31;
            UInt64 shift = 1;
            UInt64 intSensorMask = 0;

            foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
            {
                //Key 
                rowItem["dgKey"] = i.ToString();
                //CustomUrl 
                rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";
                // Date
                rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());
                //rowItem["MyDateTime"] = Convert.ToDateTime("01/01/2015");//.ToString("dd/MM/yyyy HH:mm:ss");
                
                // Heading
                if (dsHistory.Tables[0].Columns.IndexOf("Speed") != -1)
                {
                    if ((rowItem["Speed"].ToString() != "0") && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.defNA))
                    {
                        if ((rowItem["Heading"] != null) &&
                            (rowItem["Heading"].ToString() != "") && (rowItem["Heading"].ToString() != VLF.CLS.Def.Const.blankValue))
                        {
                            rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                        }
                    }
                }
                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)) )
                    rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                i++;

                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                      && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                {
                    rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
                }


                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 254))
                {
                    rowItem["MsgDetails"] = rowItem["CustomProp"];
                }

		 if (sn.User.OrganizationId == 999650 && (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == 1 || Convert.ToInt16(rowItem["BoxMsgInTypeId"])==2))
                {
                    rowItem["MsgDetails"] += "Temp:" +VLF.CLS.Util.PairFindValue("TEMP", rowItem["CustomProp"].ToString());
                }




         if (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Spreader) && rowItem["CustomProp"].ToString().Contains("RRCS"))
         {

             string tmpSpreaderData = rowItem["CustomProp"].ToString().Replace(":", "=").Replace("\"", "").Replace(",", ";");
             rowItem["MsgDetails"] = "Status:" + VLF.CLS.Util.PairFindValue("SDCStatus", tmpSpreaderData) + ";Mode:" + VLF.CLS.Util.PairFindValue("SDCSMode", tmpSpreaderData) + ";Solid Rate:" + VLF.CLS.Util.PairFindValue("SolidRate", tmpSpreaderData) + ";Liquid Rate:" + VLF.CLS.Util.PairFindValue("LiquidRate", tmpSpreaderData)
             + ";Air Temp:" + VLF.CLS.Util.PairFindValue("AirTemperature", tmpSpreaderData) + ";Road Temp:" + VLF.CLS.Util.PairFindValue("RoadTemperature", tmpSpreaderData) + ";Solid Rate Set Point:" + VLF.CLS.Util.PairFindValue("SolidRateSetpoint", tmpSpreaderData)
             + ";Liquid Rate Set Point:" + VLF.CLS.Util.PairFindValue("LiquidRateSetpoint", tmpSpreaderData) + ";Spreading Width Set Point:" + VLF.CLS.Util.PairFindValue("SpreadingWidthSetpoint", tmpSpreaderData)
             + ";Gate Position:" + VLF.CLS.Util.PairFindValue("GatePosition", tmpSpreaderData) + ";Solid Material:" + VLF.CLS.Util.PairFindValue("SolidMaterial", tmpSpreaderData) + ";Liquid Material:" + VLF.CLS.Util.PairFindValue("LiquidMaterial", tmpSpreaderData);
         }

                strStreetAddress = rowItem["StreetAddress"].ToString().Trim();

                switch (strStreetAddress)
                {
                    case VLF.CLS.Def.Const.addressNA:
                        rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                        break;

                    case VLF.CLS.Def.Const.noGPSData:
                        rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                        break;

                   

                    default:
                        break;
                }
                //Disable Speed for Store Position
                try
                {
                    //Test for wrong Sensor Mask
                    try
                    {
                        intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                    }
                    catch
                    {
                        //                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistDataGridExtended.aspx"));
                    }

                    checkBit = 0x8000000000000000; //shift << bitnum;
                    //check bit for store position 
                    if ((intSensorMask & checkBit) == checkBit)
                    {
                        rowItem["MyHeading"] = "";
                        rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                    }
                }
                catch
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error to disable speed for SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistDataGridExtended.aspx"));
                }
            }
        }

        //Replace
        //protected void dgTrips_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        //{
        //    try
        //    {
        //    }
        //    catch { }
        //}
        protected void dgHistoryDetails_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (!string.IsNullOrEmpty(dgHistoryDetails.MasterTableView.FilterExpression))
            {
                dgHistoryDetails.MasterTableView.FilterExpression = dgHistoryDetails.MasterTableView.FilterExpression.Replace("\"Speed\"", "\"Speed_1\"");
            }
            
            BinddgHistoryDetails(false);
        }
        
        private void BinddgHistoryDetails(bool isBind)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
               && !(sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle || sn.History.ShowTrips))
            {
                if (!sn.History.DsHistoryInfo.Tables[0].Columns.Contains("Speed_1"))
                {
                    System.Double dou = 0;
                    sn.History.DsHistoryInfo.Tables[0].Columns.Add("Speed_1", dou.GetType());
                    try
                    {
                        foreach (DataRow dr in sn.History.DsHistoryInfo.Tables[0].Rows)
                        {
                            if (dr["Speed"] != DBNull.Value)
                            {
                                float f = -1;
                                float.TryParse(dr["Speed"].ToString(), out f);
                                dr["Speed_1"] = f;
                            }
                            else dr["Speed_1"] = -1;
                        }
                    }
                    catch (Exception ex) { }
                }

                if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                {
                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
                    {
                        try
                        {
                            //rowItem["OriginDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString()).ToString("dd/MM/yyyy HH:mm:ss");

                            var tst = sn.History.FromDateTime;
                            string DateTimeString = rowItem["OriginDateTime"].ToString();

                            //if (DateTimeString.Contains("M")) // check AMPM exist
                            //{
                            //    var dy = DateTimeString.Substring(DateTimeString.IndexOf('/') + 1, DateTimeString.LastIndexOf('/') - DateTimeString.IndexOf('/') - 1);
                            //    var mo = DateTimeString.Substring(0, DateTimeString.IndexOf('/'));
                            //    var yr = DateTimeString.Substring(DateTimeString.LastIndexOf('/') + 1, 4);
                            //    string tm = DateTimeString.Substring(DateTimeString.LastIndexOf('/') + 5);
                            //    DateTime dtt = Convert.ToDateTime(tm);
                            //    tm = dtt.ToString("HH:mm:ss");
                            //    //rowItem["OriginDateTime"] = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}", dy, mo, yr, tm.Trim()), new System.Globalization.CultureInfo("fr-FR"));
                            //    rowItem["OriginDateTime"] = string.Format("{0}/{1}/{2} {3}", dy, mo, yr, tm.Trim());
                            //}
                        }
                        catch
                        {
                            //var t = rowItem["OriginDateTime"].ToString();
                            //rowItem["OriginDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString(), new System.Globalization.CultureInfo("fr-FR"));
                            
                            //rowItem["OriginDateTime"] = rowItem["OriginDateTime"].ToString();
                        }
                    }
                }
                else
                {
                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
                    {
                        try
                        {
                            var tst = sn.History.FromDate;
                            //rowItem["OriginDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());
                            string DateTimeString = rowItem["OriginDateTime"].ToString();
                            //ar li = DateTimeString.LastIndexOf('/');
                            if (!DateTimeString.Contains("M")) // check AMPM exist
                            {
                                var mo = DateTimeString.Substring(DateTimeString.IndexOf('/') + 1, DateTimeString.LastIndexOf('/') - DateTimeString.IndexOf('/') - 1);
                                var dy = DateTimeString.Substring(0, DateTimeString.IndexOf('/'));
                                var yr = DateTimeString.Substring(DateTimeString.LastIndexOf('/') + 1, 4);
                                string tm = DateTimeString.Substring(DateTimeString.LastIndexOf('/') + 5);
                                //rowItem["OriginDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString(), new System.Globalization.CultureInfo("en-US")).Ticks;
                                rowItem["OriginDateTime"] = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}", mo, dy, yr, tm.Trim()), new System.Globalization.CultureInfo("en-US"));
                            }
                        }
                        catch
                        {
                            //string DateTimeString = rowItem["OriginDateTime"].ToString();
                            ////ar li = DateTimeString.LastIndexOf('/');
                            //var mo = DateTimeString.Substring(DateTimeString.IndexOf('/') + 1, DateTimeString.LastIndexOf('/') - DateTimeString.IndexOf('/') - 1);
                            //var dy = DateTimeString.Substring(0, DateTimeString.IndexOf('/'));
                            //var yr = DateTimeString.Substring(DateTimeString.LastIndexOf('/') + 1, 4);
                            //string tm = DateTimeString.Substring(DateTimeString.LastIndexOf('/') + 5);
                            ////rowItem["OriginDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString(), new System.Globalization.CultureInfo("en-US")).Ticks;
                            //rowItem["OriginDateTime"] = Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}", mo, dy, yr, tm), new System.Globalization.CultureInfo("en-US"));
                        }
                    }
                }
                //dgHistoryDetails.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                dgHistoryDetails.DataSource = sn.History.DsHistoryInfo;

            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("dgKey");
                dgHistoryDetails.DataSource = dt;
            }

            if (isBind) dgHistoryDetails.DataBind();
        }
        protected void dgStops_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BinddgStops(false);
        }

        private void BinddgStops(bool isBind)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
               && (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
            {
                dgStops.DataSource = sn.History.DsHistoryInfo.Tables["StopData"];
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("StopIndex");
                dgStops.DataSource = dt;
            }
            if (isBind)
            {
                dgStops.DataBind();
            }
        }
        protected void dgHistoryDetails_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {

            try
            {
                if (e.Item.ItemType == GridItemType.Header)
                {
                    CheckBox chkSelectAllVehicles = (CheckBox)e.Item.FindControl("chkSelectAllHistoryDetails");
                    if (chkSelectAllVehicles != null)
                    {
                        chkSelectAllVehicles.Attributes.Add("onclick", "chkSelectAllHistoryDetails_Click(this)");
                    }
                    return;
                }
                if (!(e.Item is GridDataItem)) return;
                ((GridDataItem)e.Item)["MyDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["MyDateTime"].Text).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);

                if (!(e.Item.DataItem is DataRowView)) return;
                Boolean chkBoxShow = false;
                if (((DataRowView)e.Item.DataItem)["chkBoxShow"] != DBNull.Value)
                    chkBoxShow = (Boolean)((DataRowView)e.Item.DataItem)["chkBoxShow"];

                CheckBox chkSelectHistoryDetails = ((CheckBox)((GridDataItem)e.Item)["selectCheckBox"].FindControl("chkSelectHistoryDetails"));
                if (chkSelectHistoryDetails != null)
                {
                    chkSelectHistoryDetails.Checked = chkBoxShow;
                    //chkSelectVehicle.Attributes.Add("onclick", "chkSelectVehicle_Click(" + e.Item.ItemIndex + ", this)");
                }


                //long duration = Convert.ToDateTime(((DataRowView)e.Item.DataItem)["DateTimeReceived"]).Ticks - Convert.ToDateTime(((DataRowView)e.Item.DataItem)["OriginDateTime"]).Ticks;
                //duration = duration / TimeSpan.TicksPerMinute;
              
                long duration = 0L;
                try
                {
                    duration = Convert.ToDateTime(((DataRowView)e.Item.DataItem)["DateTimeReceived"]).Ticks - Convert.ToDateTime(((DataRowView)e.Item.DataItem)["OriginDateTime"]).Ticks;
                }
                catch
                {
                    duration = Convert.ToDateTime(((DataRowView)e.Item.DataItem)["DateTimeReceived"]).Ticks - Convert.ToDateTime(((DataRowView)e.Item.DataItem)["OriginDateTime"], new System.Globalization.CultureInfo("fr-FR")).Ticks;
                }
                duration = duration / TimeSpan.TicksPerMinute;
                
                if (sn.User.UserGroupId == 1 || sn.User.OrganizationId == 952)   
                   if((duration > 2 && (Int16)((DataRowView)e.Item.DataItem)["BoxMsgInTypeId"] != 25) || (duration > 5 && (Int16)((DataRowView)e.Item.DataItem)["BoxMsgInTypeId"] == 25))
                {
                    e.Item.Cells[5].BackColor = System.Drawing.Color.SlateBlue;
                    e.Item.Cells[5].ForeColor = System.Drawing.Color.White;
                }


                string odometer= Util.PairFindValue("Odometer", ((DataRowView)e.Item.DataItem)["CustomProp"].ToString());
                string unitTxt=sn.User.UnitOfMes==1? "km":"miles";

                e.Item.Cells[5].ToolTip ="Received Date/Time: "+((DataRowView)e.Item.DataItem)["DateTimeReceived"].ToString();
                if (odometer != "")
                {
                    e.Item.Cells[5].ToolTip += (char)10 + "Odometer: " + Convert.ToInt64(Convert.ToInt64(odometer) * sn.User.UnitOfMes) + " " + unitTxt;
                }	
	

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgHistoryDetails_ItemDataBound--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        protected void dgStops_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == GridItemType.Header)
                {
                    CheckBox chkSelectAllVehicles = (CheckBox)e.Item.FindControl("chkSelectAllStops");
                    if (chkSelectAllVehicles != null)
                    {
                        chkSelectAllVehicles.Attributes.Add("onclick", "chkSelectAllStops_Click(this)");
                    }
                    return;
                }
                if (!(e.Item is GridDataItem)) return;
                ((GridDataItem)e.Item)["ArrivalDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["ArrivalDateTime"].Text).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                ((GridDataItem)e.Item)["DepartureDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["DepartureDateTime"].Text).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);

                if (!(e.Item.DataItem is DataRowView)) return;
                Boolean chkBoxShow = false;
                if (((DataRowView)e.Item.DataItem)["chkBoxShow"] != DBNull.Value)
                    chkBoxShow = (Boolean)((DataRowView)e.Item.DataItem)["chkBoxShow"];

                CheckBox chkSelectStops = ((CheckBox)((GridDataItem)e.Item)["selectCheckBox"].FindControl("chkSelectStops"));
                if (chkSelectStops != null)
                {
                    chkSelectStops.Checked = chkBoxShow;
                    //chkSelectVehicle.Attributes.Add("onclick", "chkSelectVehicle_Click(" + e.Item.ItemIndex + ", this)");
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", dgStops_ItemDataBound--> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        protected void dgTrips_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
            && (sn.History.ShowTrips))
            {
                BinddgTrips(false);
            }
        }
        protected void dgTrips__ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem && e.Item.OwnerTableView.Name == "Trips")
            {
                ((GridDataItem)e.Item)["MyDateTime"].Text = Convert.ToDateTime(((GridDataItem)e.Item)["MyDateTime"].Text).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
            }
        }

        private void BinddgTrips(bool isBind)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
            && (sn.History.ShowTrips))
            {
                dgTrips.DataSource = sn.History.DsHistoryInfo;
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("TripId");
                dgTrips.DataSource = dt;
            }
            if (isBind) dgTrips.DataBind();
        }

        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
                if (e.Argument == "MapIt")
                {
                    cmdMapSelected_Click(null, null);
                }
            if (e.Argument == "GenerateGridsDate")
            {
                try
                {
                    Page_Load(null, null);
                    if (dgHistoryDetails.Visible) dgHistoryDetails.Rebind();
                    if (dgStops.Visible) dgStops.Rebind();
                    if (dgTrips.Visible) dgTrips.Rebind();

                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + ", RadAjaxManager1_AjaxRequest---> User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    string str = string.Format("alert('{0}')", errorLoad);
                    if (dgHistoryDetails.Visible) dgHistoryDetails.Rebind();
                    if (dgStops.Visible) dgStops.Rebind();
                    if (dgTrips.Visible) dgTrips.Rebind();

                    RadAjaxManager1.ResponseScripts.Add(str);

                }
            }
            
            if (e.Argument == "ClearAllFilters")
            {
                RadGrid rd = null;
                if (dgHistoryDetails.Visible) rd = dgHistoryDetails;
                if (dgStops.Visible) rd = dgStops;
                if (dgTrips.Visible) rd = dgTrips;
                if (rd == null) return;
                foreach (GridColumn column in rd.MasterTableView.Columns)
                {
                    column.CurrentFilterFunction = GridKnownFunction.NoFilter;
                    column.CurrentFilterValue = string.Empty;
                }
                rd.MasterTableView.FilterExpression = string.Empty;
                rd.MasterTableView.Rebind();

            }

        }

        protected void radContextExportMenu_ItemClick(object sender, RadMenuEventArgs e)
        {
            RadGrid rd = null;
            if (dgHistoryDetails.Visible) rd = dgHistoryDetails;
            if (dgStops.Visible) rd = dgStops;
            if (dgTrips.Visible) rd = dgTrips;
            if (rd == null) return;
            rd.ExportSettings.ExportOnlyData = true;
            rd.ExportSettings.IgnorePaging = true;
            rd.ExportSettings.OpenInNewWindow = true;
            rd.ExportSettings.HideStructureColumns = true;
            //rd.MasterTableView.GetColumn("selectCheckBox").Visible = false;
           // rd.MasterTableView.GetColumn("History").Visible = false;
            isExport = true;
            //dgFleetInfo.MasterTableView.AllowFilteringByColumn = false;
            if (e.Item.Value == "pdf")
            {
                rd.ExportSettings.Pdf.PaperSize = GridPaperSize.A4;
                rd.ExportSettings.Pdf.PageWidth = Unit.Pixel(1280);

                rd.MasterTableView.ExportToPdf();
            }
            if (e.Item.Value == "word")
            {
                rd.MasterTableView.ExportToWord();
            }
            if (e.Item.Value == "excel")
            {
                rd.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
                rd.MasterTableView.ExportToExcel();
            }
            if (e.Item.Value == "csv")
            {
                rd.MasterTableView.ExportToCSV();
            }
            //dgFleetInfo.MasterTableView.GetColumn("selectCheckBox").Visible = true;
        }



        protected void Grid_Init(object sender, EventArgs e)
        {
            ((RadGrid)sender).Culture = System.Globalization.CultureInfo.CurrentUICulture;
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
            {
                ((RadGrid)sender).Culture = System.Globalization.CultureInfo.CurrentUICulture;
            }

            GridFilterMenu menu = ((RadGrid)sender).FilterMenu;
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
        protected void Grid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFilteringItem && isExport)
            {
                e.Item.Visible = false;

            }
        }

        private void AddSpeed_1Column()
        {
            if (!sn.History.DsHistoryInfo.Tables["TripDetails"].Columns.Contains("Speed_1"))
            {
                System.Double dou = 0;
                sn.History.DsHistoryInfo.Tables["TripDetails"].Columns.Add("Speed_1", dou.GetType());
                try
                {
                    foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["TripDetails"].Rows)
                    {
                        if (dr["Speed"] != DBNull.Value)
                        {
                            float f = -1;
                            float.TryParse(dr["Speed"].ToString(), out f);
                            dr["Speed_1"] = f;
                        }
                        else dr["Speed_1"] = -1;
                    }
                }
                catch (Exception ex) { }
            }
        }

        protected void dgTrips_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            AddSpeed_1Column();
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            string filter = "TripId=" +  dataItem.GetDataKeyValue("TripId").ToString();
            //sn.History.DsHistoryInfo.Tables["TripDetails"].Select(filter);
            e.DetailTableView.DataSource = sn.History.DsHistoryInfo.Tables["TripDetails"].Select(filter);
            //switch (e.DetailTableView.Name)
            //{
            //    case "Orders":
            //        {
            //            string CustomerID = dataItem.GetDataKeyValue("CustomerID").ToString();
            //            e.DetailTableView.DataSource = GetDataTable("SELECT * FROM Orders WHERE CustomerID = '" + CustomerID + "'");
            //            break;
            //        }

            //    case "OrderDetails":
            //        {
            //            string OrderID = dataItem.GetDataKeyValue("OrderID").ToString();
            //            e.DetailTableView.DataSource = GetDataTable("SELECT * FROM [Order Details] WHERE OrderID = " + OrderID);
            //            break;
            //        }
            //}

        }

        private void LoadVehicles()
        {
            ArrayList vehicleMain = new ArrayList();

	     string strAddress = "Address";
            string strSpeed = "Speed";
 
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
            {
                strAddress = "Adresse";
                strSpeed = "Vitesse";
            }
	
            try
            {
                Boolean isDrawLabel = false;
                string IconType = sn.History.IconTypeName;
                string UnitOfMes = sn.User.UnitOfMes == 1 ? "km/h" : "mi/h";

                if (sn.History.VehicleId == 0)
                {
                    IconType = "Circle";
                    sn.History.IconTypeName = "Circle";
                }

                if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables.Count > 0))
                {
                    if (((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle)
                    ) && (sn.History.ShowStopSqNum))
                        isDrawLabel = true;
                    else
                        isDrawLabel = false;

                    //map.BreadCrumbPoints.DrawLabels = false;

                    if (!sn.History.ShowStops && !sn.History.ShowStopsAndIdle && !sn.History.ShowIdle)
                    {
                        if ((!sn.History.ShowBreadCrumb) || (sn.History.VehicleId == 0))
                        {
                            DataTable dt = new DataTable();
                            if (sn.History.ShowTrips)
                            {
                                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0) //no trips 
                                    return;

                                dt = sn.History.DsHistoryInfo.Tables[1];
                            }
                            else
                                dt = sn.History.DsHistoryInfo.Tables[0];

                            foreach (DataRow dr in dt.Rows)
                            {
                                Double lon = 0;
                                Double lat = 0;
                                Double.TryParse(dr["Longitude"].ToString().Trim(), out lon);
                                Double.TryParse(dr["Latitude"].ToString().Trim(), out lat);

                                if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue) && (lon != 0 && lat != 0))
                                {
                                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                                    vehicleDic.Add("id", dr["VehicleId"] is DBNull ? string.Empty : dr["VehicleId"].ToString());
                                    //vehicleDic.Add("date", dr["OriginDateTime"] is DBNull ? string.Empty : dr["OriginDateTime"].ToString());
                                    vehicleDic.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                                    vehicleDic.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                                    //vehicleDic.Add("spd", dr["Speed"] is DBNull ? string.Empty : dr["Speed"].ToString());
                                    //vehicleDic.Add("head", dr["MyHeading"] is DBNull ? string.Empty : dr["MyHeading"].ToString());
                                    //vehicleDic.Add("addr", dr["StreetAddress"] is DBNull ? string.Empty : dr["StreetAddress"].ToString());
                                    //try
                                    //{
                                    //    vehicleDic.Add("stat", dr["Remarks"] is DBNull ? string.Empty : dr["Remarks"].ToString());
                                    //}
                                    //catch {
                                    //    vehicleDic.Add("stat", "N");
                                    //}
                                    string icon = string.Empty;

                                    if (dr["MsgDetails"].ToString().Contains("PTO") && sn.User.OrganizationId != 343)
                                        icon = "Blue" + IconType + ".ico";
                                    else if (dr["Speed"].ToString() != "0")
                                        icon = "Green" + IconType + dr["MyHeading"].ToString().TrimEnd() + ".ico";
                                    else
                                        icon = "Red" + IconType + ".ico";
                                    vehicleDic.Add("icon", icon);
                                    vehicleDic.Add("trail", "0");

                                    string originDate = Convert.ToDateTime(dr["MyDateTime"]).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                    string toolTip = string.Empty;
                                    if (dr["MsgDetails"].ToString().TrimEnd() != "")
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        toolTip = "<B>" + " [" + originDate + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline; font-weight:bold;' > " + strAddress + ":</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  " + strSpeed + ":</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    else
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        toolTip = "<B>" + " [" + originDate + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' > " + strAddress + ":</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  " + strSpeed + ":</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    vehicleDic.Add("toolTip", toolTip);
                                    vehicleMain.Add(vehicleDic);
                                }
                            }
                        }
                        else
                        {

                            DataTable dt = new DataTable();
                            if (sn.History.ShowTrips)
                            {
                                if (sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0) //no trips 
                                    return;

                                dt = sn.History.DsHistoryInfo.Tables[1];
                            }
                            else
                                dt = sn.History.DsHistoryInfo.Tables[0];

                            foreach (DataRow dr in dt.Rows)
                            {
                                Double lon = 0;
                                Double lat = 0;
                                Double.TryParse(dr["Longitude"].ToString().Trim(), out lon);
                                Double.TryParse(dr["Latitude"].ToString().Trim(), out lat);

                                if (Convert.ToBoolean(dr["chkBoxShow"]) && (dr["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (dr["Longitude"].ToString().TrimEnd() != VLF.CLS.Def.Const.blankValue) && (lon != 0 && lat != 0))
                                {
                                    Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                                    vehicleDic.Add("id", dr["VehicleId"] is DBNull ? string.Empty : dr["VehicleId"].ToString());
                                    //vehicleDic.Add("date", dr["OriginDateTime"] is DBNull ? string.Empty : dr["OriginDateTime"].ToString());
                                    vehicleDic.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                                    vehicleDic.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                                    //vehicleDic.Add("spd", dr["Speed"] is DBNull ? string.Empty : dr["Speed"].ToString());
                                    //vehicleDic.Add("head", dr["MyHeading"] is DBNull ? string.Empty : dr["MyHeading"].ToString());
                                    //vehicleDic.Add("addr", dr["StreetAddress"] is DBNull ? string.Empty : dr["StreetAddress"].ToString());
                                    //try
                                    //{
                                    //    vehicleDic.Add("stat", dr["Remarks"] is DBNull ? string.Empty : dr["Remarks"].ToString());
                                    //}
                                    //catch
                                    //{
                                    //    vehicleDic.Add("stat", "N");
                                    //}

                                    string icon = string.Empty;
                                    if (dr["MsgDetails"].ToString().Contains("PTO") && sn.User.OrganizationId != 343)
                                        icon = "Blue" + sn.History.IconTypeName + ".ico";
                                    else if (dr["Speed"].ToString() != "0")
                                        icon = "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico";
                                    else
                                        icon = "Red" + sn.History.IconTypeName + ".ico";
                                    vehicleDic.Add("icon", icon);
                                    vehicleDic.Add("trail", "1");
                                    string toolTip = string.Empty;
                                    if (dr["MsgDetails"].ToString().TrimEnd() != "")
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        toolTip = "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + " " + dr["MsgDetails"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  "+strAddress +":</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  "+strSpeed+":</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    else
                                    {
                                        //if (sn.Map.VehiclesToolTip.Length == 0)
                                        //    sn.Map.VehiclesToolTip += "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        //else
                                        //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline' > Speed:</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes + "<BR>\",";
                                        toolTip = "<B>" + " [" + dr["MyDateTime"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["BoxMsgInTypeName"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  "+strAddress +":</FONT> " + dr["StreetAddress"].ToString().TrimEnd() + "<BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  "+strSpeed+":</FONT> " + dr["Speed"].ToString().TrimEnd() + " " + UnitOfMes;
                                    }
                                    vehicleDic.Add("toolTip", toolTip);

                                    vehicleMain.Add(vehicleDic);

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                        {
                            TimeSpan TripIndling;
                            TripIndling = new TimeSpan(Convert.ToInt64(dr["StopDurationVal"]) * TimeSpan.TicksPerSecond);
                            double StopDurationVal = TripIndling.TotalMinutes;
                            string icon = "";

                            Double lon = 0;
                            Double lat = 0;
                            Double.TryParse(dr["Longitude"].ToString().Trim(), out lon);
                            Double.TryParse(dr["Latitude"].ToString().Trim(), out lat);

                            if (Convert.ToBoolean(dr["chkBoxShow"]) && (lon != 0 && lat != 0))
                            {
                                if (dr["Remarks"].ToString() == "Idling")
                                    icon = "Idle.ico";
                                else if (StopDurationVal < 15)
                                    icon = "Stop_3.ico";
                                else if ((StopDurationVal > 15) && (StopDurationVal < 60))
                                    icon = "Stop_15.ico";
                                else if (StopDurationVal > 60)
                                    icon = "Stop_60.ico";

                                Dictionary<string, string> vehicleDic = new Dictionary<string, string>();
                                vehicleDic.Add("id", dr["VehicleId"] is DBNull ? string.Empty : dr["VehicleId"].ToString());
                                //vehicleDic.Add("date", dr["OriginDateTime"] is DBNull ? string.Empty : dr["OriginDateTime"].ToString());
                                vehicleDic.Add("lat", dr["Latitude"] is DBNull ? string.Empty : dr["Latitude"].ToString());
                                vehicleDic.Add("lon", dr["Longitude"] is DBNull ? string.Empty : dr["Longitude"].ToString());
                                //vehicleDic.Add("spd", dr["Speed"] is DBNull ? string.Empty : dr["Speed"].ToString());
                                //vehicleDic.Add("head", dr["MyHeading"] is DBNull ? string.Empty : dr["MyHeading"].ToString());
                                //vehicleDic.Add("addr", dr["StreetAddress"] is DBNull ? string.Empty : dr["StreetAddress"].ToString());
                                //try
                                //{
                                //    vehicleDic.Add("stat", dr["Remarks"] is DBNull ? string.Empty : dr["Remarks"].ToString());
                                //}
                                //catch
                                //{
                                //    vehicleDic.Add("stat", "N");
                                //}

                                //if (dr["MsgDetails"].ToString().Contains("PTO") && sn.User.OrganizationId != 343)
                                //    icon = "Blue" + sn.History.IconTypeName + ".ico";
                                //else if (dr["Speed"].ToString() != "0")
                                //    icon = "Green" + sn.History.IconTypeName + dr["MyHeading"].ToString().TrimEnd() + ".ico";
                                //else
                                //    icon = "Red" + sn.History.IconTypeName + ".ico";
                                vehicleDic.Add("icon", icon);
                                vehicleDic.Add("trail", "0");

                                string toolTip = string.Empty;
                                dr["Location"] = dr["Location"].ToString().TrimEnd().Replace(Convert.ToString(Convert.ToChar(13)), " ").Replace(Convert.ToString(Convert.ToChar(10)), " ");

                                //if (sn.Map.VehiclesToolTip.Length == 0)
                                //    sn.Map.VehiclesToolTip += "<B>" + " [" + rowItem["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["Remarks"].ToString().TrimEnd() + " (" + rowItem["StopDuration"].ToString().TrimEnd() + ") ," + rowItem["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["Location"].ToString().TrimEnd() + "<BR>\",";
                                //else
                                //    sn.Map.VehiclesToolTip += "\"<B>" + " [" + rowItem["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + rowItem["Remarks"].ToString().TrimEnd() + " (" + rowItem["StopDuration"].ToString().TrimEnd() + ") ," + rowItem["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline' > Address:</FONT> " + rowItem["Location"].ToString().TrimEnd() + "<BR> \",";
                                toolTip = "<B>" + " [" + dr["StopIndex"].ToString().TrimEnd() + "]:</B><FONT style='COLOR: Purple'>" + dr["Remarks"].ToString().TrimEnd() + " (" + dr["StopDuration"].ToString().TrimEnd() + ") ," + dr["ArrivalDateTime"].ToString().TrimEnd() + "</FONT><BR> <FONT style='TEXT-DECORATION: underline;font-weight:bold;' >  "+strAddress +":</FONT> " + dr["Location"].ToString().TrimEnd();
                                vehicleDic.Add("toolTip", toolTip);
                                vehicleMain.Add(vehicleDic);

                            }
                        }
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
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }
            if (vehicleMain.Count > 0)
            {

                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = int.MaxValue;
                string json = js.Serialize(vehicleMain);
                //string mapStr = "if (clientMapData.length > 0) { try{parent.frmHisMap.ShowHistoryMap(clientMapData);} catch(err){ setTimeout(\"setparent.frmHisMap.ShowHistoryMap(clientMapData)\", 1000);}}";
                string mapStr = "if (clientMapData.length > 0) { DrawHistoryMap();}";
                string javascriptStr = string.Format(" clientMapData={0}; clientMapData  = eval(clientMapData ) ;{1}", json, mapStr);
                RadAjaxManager1.ResponseScripts.Add(javascriptStr);

            }
            else
            {
                RadAjaxManager1.ResponseScripts.Add("parent.frmHisMap.ClearHistoryMap();");
                //RadAjaxManager1.ResponseScripts.Add(string.Format("alert('Hello from the server! Server time is {0}');", DateTime.Now.ToLongTimeString()));
                //string script = string.Format("alert('Hello from the server! Server time is {0}');", DateTime.Now.ToLongTimeString());
                //ScriptManager.RegisterStartupScript(Page, typeof(Page), "myscript", script, true);
            }

        }
        protected void dgHistoryDetails_ItemCommand(object sender, GridCommandEventArgs e)
        {
            //if (e.CommandName == RadGrid.FilterCommandName)
            //{
            //    if (!string.IsNullOrEmpty(dgHistoryDetails.MasterTableView.FilterExpression))
            //    {
            //        dgHistoryDetails.MasterTableView.FilterExpression = 
            //            dgHistoryDetails.MasterTableView.FilterExpression.Replace("Speed", "Speed_1");
            //    }
            //}

        }
}
}
