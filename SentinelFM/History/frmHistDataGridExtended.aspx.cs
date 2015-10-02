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

namespace SentinelFM
{
    public partial class History_frmHistDataGridExtended : SentinelFMBasePage
    {
        private DataSet dsHistory;
        //private System.Threading.Timer _timer;
        // 1 if timer callback is executing; otherwise 0
        int _inTimerCallback = 0;
        private Stopwatch watch = new Stopwatch();
        private Stopwatch Pagewatch = new Stopwatch();

        protected void Page_Load(object sender, EventArgs e)
        {
            double ver = getInternetExplorerVersion();
            if (ver < 7.0)
            {
                dgStops.Width = Unit.Pixel(1000);
                dgHistoryDetails.Width = Unit.Pixel(1000);
            }

            this.cmdSendCommand.OnClientClick = "javascript: SensorsInfo('" + sn.History.LicensePlate + "')";

            if (!Page.IsPostBack)
            {
                Pagewatch.Reset();
                Pagewatch.Start();

                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistory, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                if (sn.History.Animated && sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                    StartTimer();

                if (sn.History.DsSelectedData != null && sn.History.DsSelectedData.Tables.Count > 0 && sn.History.DsSelectedData.Tables[0].Rows.Count > 1)
                    this.cmdSendCommand.Enabled = false;
                else
                    this.cmdSendCommand.Enabled = true;

                dgStops.LayoutSettings.ClientVisible = false;
                dgTrips.LayoutSettings.ClientVisible = false;

                cboRows.SelectedIndex = cboRows.Items.IndexOf(cboRows.Items.FindByValue(sn.History.DgVisibleRows.ToString()));
                cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue(sn.History.DgItemsPerPage.ToString()));
                dgHistoryDetails.Height = Unit.Pixel(107 + Convert.ToInt32(dgHistoryDetails.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                this.dgHistoryDetails.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

                dgStops.Height = Unit.Pixel(107 + Convert.ToInt32(dgStops.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                this.dgStops.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

                dgTrips.Height = Unit.Pixel(107 + Convert.ToInt32(dgTrips.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
                this.dgTrips.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

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
                        dgStops.ClearCachedDataSource();
                        dgStops.RebindDataSource();
                        watch.Stop();
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History Stop&Idling-->DataGrid Bindind (sec):" + watch.Elapsed.TotalSeconds));
                        dgHistoryDetails.LayoutSettings.ClientVisible = false;
                        dgStops.LayoutSettings.ClientVisible = true;
                    }
                    else if (sn.History.ShowTrips)
                    {
                        dgTrips_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);
                        dgTrips.ClearCachedDataSource();
                        dgTrips.RebindDataSource();

                        dgHistoryDetails.LayoutSettings.ClientVisible = false;
                        dgStops.LayoutSettings.ClientVisible = false;
                        dgTrips.LayoutSettings.ClientVisible = true;
                    }
                    else
                    {
                        if (sn.History.DsHistoryInfo == null || sn.History.DsHistoryInfo.Tables.Count == 0 || sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0)
                        {
                            dgHistory_Fill_NewTZ(sn.History.VehicleId, sn.History.FromDateTime, sn.History.ToDateTime);

                            watch.Reset();
                            watch.Start();
                            dgHistoryDetails.ClearCachedDataSource();
                            dgHistoryDetails.RebindDataSource();
                            watch.Stop();
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->DataGrid Bindind (sec):" + watch.Elapsed.TotalSeconds));

                            dgHistoryDetails.LayoutSettings.ClientVisible = true;
                            dgStops.LayoutSettings.ClientVisible = false;
                        }
                        else
                        {
                            dgHistoryDetails.ClearCachedDataSource();
                            dgHistoryDetails.RebindDataSource();
                            dgHistoryDetails.LayoutSettings.ClientVisible = true;
                            dgStops.LayoutSettings.ClientVisible = false;
                        }
                    }

                    if (sn.User.MapType == VLF.MAP.MapType.LSD)
                        Response.Write("<script language='javascript'> parent.frmHisMap.location.href='frmHistoryLSDMap.aspx' </script>");
                    else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                        //Response.Write("<script language='javascript'> parent.frmHisMap.location.href='frmHistoryMapVE.aspx' </script>");
                        Response.Write("<script language='javascript'> parent.frmHisMap.location.href='../MapVE/VEHistory.aspx' </script>");

                    else
                        Response.Write("<script language='javascript'> parent.frmHisMap.location.href='frmHistMap.aspx' </script>");
                }
                Pagewatch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->History Page Load (sec):" + Pagewatch.Elapsed.TotalSeconds));
            }
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
                dbh.Timeout = -1;
                bool RequestOverflowed = false;
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Hist.xsd";
                string strPath = Server.MapPath("../Datasets/Hist.xsd");
                //DumpBeforeCall(sn, string.Format("dgHistory_Fill -- : VehicleId = {0}", VehicleId));

                watch.Reset();
                watch.Start();
                if (sn.History.VehicleId != 0)
                {
                    if (sn.History.Address == "")
                    {
                        if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                            if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
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
                //int RowsCount = dsHistory.Tables[0].Rows.Count - 1;
                //sn.MessageText = "";
                //if (dsHistory.Tables[0].Columns.IndexOf("StreetAddress") == -1)
                //{
                //    DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                //    dsHistory.Tables[0].Columns.Add(colStreetAddress);
                //}

                //DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
                //dsHistory.Tables[0].Columns.Add(colDateTime);


                //// Show Heading

                //DataColumn dc = new DataColumn();
                //dc.ColumnName = "MyHeading";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsHistory.Tables[0].Columns.Add(dc);

                //// DataGrid Key

                //dc = new DataColumn();
                //dc.ColumnName = "dgKey";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsHistory.Tables[0].Columns.Add(dc);

                //// CustomUrl

                //dc = new DataColumn();
                //dc.ColumnName = "CustomUrl";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsHistory.Tables[0].Columns.Add(dc);


                //dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                //dc.DefaultValue = true;
                //dsHistory.Tables[0].Columns.Add(dc);

                //int i = 0;
                //string strStreetAddress = "";

                //UInt64 checkBit = 0;
                //Int16 bitnum = 31;
                //UInt64 shift = 1;
                //UInt64 intSensorMask = 0;

                //foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
                //{
                //    //Key 
                //    rowItem["dgKey"] = i.ToString();


                //    //CustomUrl 
                //    rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";


                //    // Date
                //    rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());

                //    // Heading
                //    if (dsHistory.Tables[0].Columns.IndexOf("Speed") != -1)
                //    {
                //        if ((rowItem["Speed"].ToString() != "0") && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.defNA))
                //        {
                //            if ((rowItem["Heading"] != null) &&
                //                (rowItem["Heading"].ToString() != "") && (rowItem["Heading"].ToString() != VLF.CLS.Def.Const.blankValue))
                //            {
                //                rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                //            }
                //        }
                //    }
                //    if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling)))
                //        rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();


                //    i++;

                //    if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                //          && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                //    {
                //        rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
                //    }

                //    strStreetAddress = rowItem["StreetAddress"].ToString().Trim();

                //    switch (strStreetAddress)
                //    {
                //        case VLF.CLS.Def.Const.addressNA:
                //            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                //            break;

                //        case VLF.CLS.Def.Const.noGPSData:
                //            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                //            break;

                //        case VLF.CLS.Def.Const.noValidAddress:
                //            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                //            break;

                //        default:
                //            break;
                //    }



                //    //Disable Speed for Store Position


                //    try
                //    {
                //        //Test for wrong Sensor Mask
                //        try
                //        {
                //            intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                //        }
                //        catch
                //        {
                //            //                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistDataGridExtended.aspx"));
                //        }

                //        checkBit = 0x8000000000000000; //shift << bitnum;
                //        //check bit for store position 
                //        if ((intSensorMask & checkBit) == checkBit)
                //        {
                //            rowItem["MyHeading"] = "";
                //            rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                //        }

                //    }
                //    catch
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error to disable speed for SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistDataGridExtended.aspx"));
                //    }


                //}
                ProcessHistoryData(ref dsHistory);

                sn.History.DsHistoryInfo = dsHistory;
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb && !sn.History.Animated)
                //    sn.MapSolute.LoadHistory(sn, dsHistory.Tables[0], "History");

                dgStops.LayoutSettings.ClientVisible = false;
                dgHistoryDetails.LayoutSettings.ClientVisible = true;

                watch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Data manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                {
                    string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    strUrl = strUrl + "	var myname='Message';";
                    strUrl = strUrl + " var w=370;";
                    strUrl = strUrl + " var h=50;";
                    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                    strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                    strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                    Response.Write(strUrl);
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
        // Changes for TimeZone feature end

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
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Hist.xsd";
                string strPath = Server.MapPath("../Datasets/Hist.xsd");
                //DumpBeforeCall(sn, string.Format("dgHistory_Fill -- : VehicleId = {0}", VehicleId));

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
                //int RowsCount = dsHistory.Tables[0].Rows.Count - 1;
                //sn.MessageText = "";
                //if (dsHistory.Tables[0].Columns.IndexOf("StreetAddress") == -1)
                //{
                //    DataColumn colStreetAddress = new DataColumn("StreetAddress", Type.GetType("System.String"));
                //    dsHistory.Tables[0].Columns.Add(colStreetAddress);
                //}

                //DataColumn colDateTime = new DataColumn("MyDateTime", Type.GetType("System.DateTime"));
                //dsHistory.Tables[0].Columns.Add(colDateTime);


                //// Show Heading

                //DataColumn dc = new DataColumn();
                //dc.ColumnName = "MyHeading";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsHistory.Tables[0].Columns.Add(dc);

                //// DataGrid Key

                //dc = new DataColumn();
                //dc.ColumnName = "dgKey";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsHistory.Tables[0].Columns.Add(dc);

                //// CustomUrl

                //dc = new DataColumn();
                //dc.ColumnName = "CustomUrl";
                //dc.DataType = Type.GetType("System.String");
                //dc.DefaultValue = "";
                //dsHistory.Tables[0].Columns.Add(dc);


                //dc = new DataColumn("chkBoxShow", Type.GetType("System.Boolean"));
                //dc.DefaultValue = true;
                //dsHistory.Tables[0].Columns.Add(dc);

                //int i = 0;
                //string strStreetAddress = "";

                //UInt64 checkBit = 0;
                //Int16 bitnum = 31;
                //UInt64 shift = 1;
                //UInt64 intSensorMask = 0;

                //foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
                //{
                //    //Key 
                //    rowItem["dgKey"] = i.ToString();


                //    //CustomUrl 
                //    rowItem["CustomUrl"] = "javascript:var w =HistoryInfo('" + i.ToString() + "')";


                //    // Date
                //    rowItem["MyDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());

                //    // Heading
                //    if (dsHistory.Tables[0].Columns.IndexOf("Speed") != -1)
                //    {
                //        if ((rowItem["Speed"].ToString() != "0") && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.blankValue) && (rowItem["Speed"].ToString() != VLF.CLS.Def.Const.defNA))
                //        {
                //            if ((rowItem["Heading"] != null) &&
                //                (rowItem["Heading"].ToString() != "") && (rowItem["Heading"].ToString() != VLF.CLS.Def.Const.blankValue))
                //            {
                //                rowItem["MyHeading"] = Heading(rowItem["Heading"].ToString());
                //            }
                //        }
                //    }
                //    if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeAcceleration)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.ExtremeBraking)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshAcceleration)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.HarshBraking)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)) ||
                //        (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling)))
                //        rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();


                //    i++;

                //    if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                //          && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                //    {
                //        rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
                //    }

                //    strStreetAddress = rowItem["StreetAddress"].ToString().Trim();

                //    switch (strStreetAddress)
                //    {
                //        case VLF.CLS.Def.Const.addressNA:
                //            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                //            break;

                //        case VLF.CLS.Def.Const.noGPSData:
                //            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                //            break;

                //        case VLF.CLS.Def.Const.noValidAddress:
                //            rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                //            break;

                //        default:
                //            break;
                //    }



                //    //Disable Speed for Store Position


                //    try
                //    {
                //        //Test for wrong Sensor Mask
                //        try
                //        {
                //            intSensorMask = Convert.ToUInt64(rowItem["SensorMask"]);
                //        }
                //        catch
                //        {
                //            //                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistDataGridExtended.aspx"));
                //        }

                //        checkBit = 0x8000000000000000; //shift << bitnum;
                //        //check bit for store position 
                //        if ((intSensorMask & checkBit) == checkBit)
                //        {
                //            rowItem["MyHeading"] = "";
                //            rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                //        }

                //    }
                //    catch
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error to disable speed for SensorMask: " + rowItem["SensorMask"] + " User:" + sn.UserID.ToString() + " frmHistDataGridExtended.aspx"));
                //    }


                //}
                ProcessHistoryData(ref dsHistory);

                sn.History.DsHistoryInfo = dsHistory;
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb && !sn.History.Animated)
                //    sn.MapSolute.LoadHistory(sn, dsHistory.Tables[0], "History");

                dgStops.LayoutSettings.ClientVisible = false;
                dgHistoryDetails.LayoutSettings.ClientVisible = true;

                watch.Stop();
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "StopWatch-->Get History -->Data manipulation (sec):" + watch.Elapsed.TotalSeconds + " ; parameters:" + sn.History.VehicleId + "," + sn.History.FromDateTime + "," + sn.History.ToDateTime));

                if ((sn.MessageText != "") && (sn.Message.ToString().Length > 0))
                {
                    string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
                    strUrl = strUrl + "	var myname='Message';";
                    strUrl = strUrl + " var w=370;";
                    strUrl = strUrl + " var h=50;";
                    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
                    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
                    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
                    strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);} ";

                    strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";
                    Response.Write(strUrl);
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

        protected void dgHistoryDetails_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
               && !(sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle || sn.History.ShowTrips))
            {
                e.DataSource = sn.History.DsHistoryInfo;
            }
        }

        private void LoadSelectedVehiclesHistory()
        {
            DataTable dt = sn.History.DsHistoryInfo.Tables[0].Clone();
            foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
            {
                if (Convert.ToBoolean(rowItem["chkBoxShow"]))
                    dt.ImportRow(rowItem);
            }
            //sn.MapSolute.LoadHistory(sn, dt, "History");
        }

        private void LoadSingleVehicleHistory()
        {
            if ((!clsUtility.IsNumeric(sn.History.CarLatitude)) || (!clsUtility.IsNumeric(sn.History.CarLongitude)))
            {
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                //    sn.MapSolute.LoadDefaultMap(sn);
                return;
            }

            DataTable dt = new DataTable();
            dt = sn.History.DsHistoryInfo.Tables[0].Clone();

            DataRow dr = dt.NewRow();
            dr["StreetAddress"] = sn.History.CarAddress;
            dr["Latitude"] = sn.History.CarLatitude;
            dr["Longitude"] = sn.History.CarLongitude;
            dr["Speed"] = sn.History.CarSpeed;
            dr["Heading"] = sn.History.Heading;
            dr["MyDateTime"] = sn.History.CarHistoryDate;

            dt.Rows.Add(dr);
            //sn.MapSolute.LoadHistory(sn, dt, "History");
        }

        private void LoadSingleVehicleStopHistory()
        {
            if ((!clsUtility.IsNumeric(sn.History.CarLatitude))
                  || (!clsUtility.IsNumeric(sn.History.CarLongitude)))
            {
                //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
                //    sn.MapSolute.LoadDefaultMap(sn);
                return;
            }

            DataTable dt = new DataTable();
            dt = sn.History.DsHistoryInfo.Tables["StopData"].Clone();

            DataRow dr = dt.NewRow();
            dr["Location"] = sn.History.CarAddress;
            dr["Latitude"] = sn.History.CarLatitude;
            dr["Longitude"] = sn.History.CarLongitude;
            dr["ArrivalDateTime"] = sn.History.StopDate;
            dt.Rows.Add(dr);
            //sn.MapSolute.LoadHistory(sn, dt, "HistoryStops");
        }

        protected void dgHistoryDetails_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
        {
            if (e.Column.Name == "MapIt")
                LoadSingleVehicleHistory();
        }

        protected void dgHistoryDetails_RowChanged(object sender, ISNet.WebUI.WebGrid.RowChangedEventArgs e)
        {
            sn.History.CarLatitude = e.Row.Cells.GetNamedItem("Latitude").Text;
            sn.History.CarLongitude = e.Row.Cells.GetNamedItem("Longitude").Text;
            sn.History.CarSpeed = e.Row.Cells.GetNamedItem("Speed").Text;
            sn.History.CarHistoryDate = e.Row.Cells.GetNamedItem("MyDateTime").Text;
            sn.History.CarMessageType = e.Row.Cells.GetNamedItem("BoxMsgInTypeName").Text;
            sn.History.CarAddress = e.Row.Cells.GetNamedItem("StreetAddress").Text;
            sn.History.OriginDateTime = e.Row.Cells.GetNamedItem("MyDateTime").Text;
            dgHistoryDetails.ClientAction.RefreshModifiedControls();
        }

        // Changes for TimeZone feature start
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

                dgStops.LayoutSettings.ClientVisible = true;
                dgHistoryDetails.LayoutSettings.ClientVisible = false;

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

                dgStops.LayoutSettings.ClientVisible = true;
                dgHistoryDetails.LayoutSettings.ClientVisible = false;

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

        protected void dgStops_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
               && (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
            {
                e.DataSource = sn.History.DsHistoryInfo.Tables["StopData"];
            }
        }

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

        protected void dgStops_ButtonClick(object sender, ISNet.WebUI.WebGrid.ButtonEventArgs e)
        {
            if (e.Column.Name == "MapIt")
                LoadSingleVehicleStopHistory();
        }

        protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgStops.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
            this.dgHistoryDetails.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

            sn.History.DgItemsPerPage = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

            if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
            {
                dgStops.ClearCachedDataSource();
                dgStops.RebindDataSource();
            }
            else
            {
                dgHistoryDetails.ClearCachedDataSource();
                dgHistoryDetails.RebindDataSource();
            }
        }

        protected void dgHistoryDetails_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                //else
                //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
            }
        }

        protected void dgStops_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                else
                    e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
            }
        }

        protected void dgHistoryDetails_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        {
            try
            {
                ISNet.WebUI.WebGrid.WebGridCellCollection cells = e.Row.Cells;
                //if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record && (!IsNumeric(cells.GetNamedItem("Latitude").Text.TrimEnd())
                //|| !IsNumeric(cells.GetNamedItem("Longitude").Text.TrimEnd())
                //|| (Convert.ToDouble(cells.GetNamedItem("Latitude").Text.TrimEnd()) == 0)
                //|| (Convert.ToDouble(cells.GetNamedItem("Longitude").Text.TrimEnd()) == 0)))
                //{

                //   cells.GetNamedItem("MapIt").Column.Visible = false;
                //}
                //else
                //{
                //   cells.GetNamedItem("MapIt").Column.Visible = true;
                //}
                DataRow[] drCollections = null;
                if (sn.History.DsHistoryInfo != null)
                {
                    drCollections = sn.History.DsHistoryInfo.Tables[0].Select("dgKey like '" + e.Row.KeyValue + "'");

                    foreach (DataRow rowItem in drCollections)
                    {
                        e.Row.Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
                    }
                }
            }
            catch {}
        }

        protected void dgStops_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        {
            try
            {
                //ISNet.WebUI.WebGrid.WebGridCellCollection cells = e.Row.Cells;
                //if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record && (!IsNumeric(cells.GetNamedItem("Latitude").Text.TrimEnd())
                //|| !IsNumeric(cells.GetNamedItem("Longitude").Text.TrimEnd())
                //|| (Convert.ToDouble(cells.GetNamedItem("Latitude").Text.TrimEnd()) == 0)
                //|| (Convert.ToDouble(cells.GetNamedItem("Longitude").Text.TrimEnd()) == 0)))
                //{
                //   cells.GetNamedItem("MapIt").Column.Visible = false;
                //}
                //else
                //{
                //   cells.GetNamedItem("MapIt").Column.Visible = true;
                //}
                DataRow[] drCollections = null;
                if (sn.History.DsHistoryInfo != null)
                {
                    drCollections = sn.History.DsHistoryInfo.Tables[0].Select("StopIndex like '" + e.Row.KeyValue + "'");

                    foreach (DataRow rowItem in drCollections)
                    {
                        e.Row.Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
                    }
                }
            }
            catch {}
        }

        protected void cboRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            sn.History.DgVisibleRows = Convert.ToInt32(cboRows.SelectedItem.Value);
            dgHistoryDetails.Height = Unit.Pixel(107 + Convert.ToInt32(dgHistoryDetails.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
            dgStops.Height = Unit.Pixel(107 + Convert.ToInt32(dgStops.LayoutSettings.RowHeightDefault.Value * Convert.ToInt16(cboRows.SelectedItem.Value)));
        }

        private float getInternetExplorerVersion()
        {
            // Returns the version of Internet Explorer or a -1
            // (indicating the use of another browser).
            float rv = -1;
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            if (browser.Browser == "IE")
                rv = (float)(browser.MajorVersion + browser.MinorVersion);
            return rv;
        }

        private void ShowErrorMessage()
        {
            //Create pop up message
            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='Message';";
            strUrl = strUrl + " var w=370;";
            strUrl = strUrl + " var h=50;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);}";

            strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";

            Response.Write(strUrl);
        }

        private void SaveShowCheckBoxes()
        {
            try
            {
                if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
                {
                    //foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                    //{
                    //    rowItem["chkBoxShow"] = false;
                    //    foreach (string keyValue in dgStops.RootTable.GetCheckedRows())
                    //    {
                    //        if (keyValue == rowItem["StopIndex"].ToString())
                    //        {
                    //            rowItem["chkBoxShow"] = true;
                    //            break;
                    //        }

                    //    }
                    //}

                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables["StopData"].Rows)
                        rowItem["chkBoxShow"] = false;

                    foreach (string keyValue in dgStops.RootTable.GetCheckedRows())
                    {
                        DataRow[] drCollections = null;
                        drCollections = sn.History.DsHistoryInfo.Tables["StopData"].Select("StopIndex='" + keyValue + "'");
                        if (drCollections != null && drCollections.Length > 0)
                        {
                            DataRow dRow = drCollections[0];
                            dRow["chkBoxShow"] = true;
                        }
                    }
                }
                else if (sn.History.ShowTrips)
                {
                    //foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[1].Rows)
                    //{
                    //    rowItem["chkBoxShow"] = false;

                    //    foreach (string keyValue in dgTrips.RootTable.ChildTables[0].GetCheckedRows())
                    //    {
                    //        if (keyValue == rowItem["dgKey"].ToString())
                    //        {
                    //            rowItem["chkBoxShow"] = true;
                    //            break;
                    //        }

                    //    }
                    //}
                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[1].Rows)
                        rowItem["chkBoxShow"] = false;

                    foreach (string keyValue in dgTrips.RootTable.ChildTables[0].GetCheckedRows())
                    {
                        DataRow[] drCollections = null;
                        drCollections = sn.History.DsHistoryInfo.Tables[1].Select("dgKey='" + keyValue + "'");
                        if (drCollections != null && drCollections.Length > 0)
                        {
                            DataRow dRow = drCollections[0];
                            dRow["chkBoxShow"] = true;
                        }
                    }
                }
                else
                {
                    //foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
                    //{
                    //    rowItem["chkBoxShow"] = false;

                    //    foreach (string keyValue in dgHistoryDetails.RootTable.GetCheckedRows())
                    //    {
                    //        if (keyValue == rowItem["dgKey"].ToString())
                    //        {
                    //            rowItem["chkBoxShow"] = true;
                    //            break;
                    //        }

                    //    }
                    //}

                    foreach (DataRow rowItem in sn.History.DsHistoryInfo.Tables[0].Rows)
                        rowItem["chkBoxShow"] = false;

                    foreach (string keyValue in dgHistoryDetails.RootTable.GetCheckedRows())
                    {
                        DataRow[] drCollections = null;
                        drCollections = sn.History.DsHistoryInfo.Tables[0].Select("dgKey='" + keyValue+"'");
                        if (drCollections != null && drCollections.Length > 0)
                        {
                            DataRow dRow = drCollections[0];
                            dRow["chkBoxShow"] = true;
                        }
                    }
                }
            }
            catch {}
        }

        protected void cmdMapSelected_Click(object sender, EventArgs e)
        {
            try
            {
                SaveShowCheckBoxes();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
            }

            if (sn.User.MapType == VLF.MAP.MapType.LSD)
                Response.Write("<script language='javascript'> parent.frmHisMap.location.href='frmHistoryLSDMap.aspx' </script>");
            else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                //Response.Write("<script language='javascript'> parent.frmHisMap.location.href='frmHistoryMapVE.aspx' </script>");
                Response.Write("<script language='javascript'> parent.frmHisMap.location.href='../MapVE/VEHistory.aspx' </script>");
            else
                Response.Write("<script language='javascript'> parent.frmHisMap.location.href='frmHistMap.aspx' </script>");
            //if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
            // LoadSelectedVehiclesHistory(); 
        }

        private void StartTimer()
        {
            //_timer = new System.Threading.Timer(new TimerCallback(this.TimerCallback), null, sn.History.MapAnimationSpeed, sn.History.MapAnimationSpeed);
        }

        //
        // if the timer fires frequently or the callback runs for a long period, you may
        // want to prevent two threads from calling it concurrently
        //
        void TimerCallback(object state)
        {
            // if the callback is already being executed, just return
            if (Interlocked.Exchange(ref _inTimerCallback, 1) != 0)
            {
                return;
            }
            try
            {
                // do work (potentially long running work that may call into native code)
                LoadMapsoluteAnimated();
            }
            finally
            {
                Interlocked.Exchange(ref _inTimerCallback, 0);
            }
        }
        //
        // Before the AppDomain is shutdown, the timer must be disposed.  Otherwise,
        // the underlying native timer may crash the process if it fires and attempts
        // to call into the unloaded AppDomain.  In a multi-threaded environment,
        // you may need to use synchronization to ensure the timer is disposed at most once.
        //

        internal void DisposeTimer()
        {
            //System.Threading.Timer timer = _timer;
            //if (timer != null
            //    && Interlocked.CompareExchange(ref _timer, null, timer) == timer) 
            //  {
            //        timer.Dispose();
            //    }

            //// if you don’t want the timer callback to be aborted during an AppDomain unload,
            //// or if it calls into native code, then loop until the callback has completed
            ////while (_inTimerCallback != 0) {
            ////    Thread.Sleep(100);
            ////}
        }

        private void LoadMapsoluteAnimated()
        {
            if (sn.History.DsHistoryInfo == null || sn.History.DsHistoryInfo.Tables.Count == 0 || sn.History.DsHistoryInfo.Tables[0].Rows.Count == 0)
                return;
            //DataTable dt = sn.History.DsHistoryInfo.Tables[0].Clone();

            //if (ViewState["From"] == null)
            //   ViewState["From"] = 20;
            //else if (Convert.ToInt32(ViewState["From"]) > sn.History.DsHistoryInfo.Tables[0].Rows.Count)
            //{
            //   DisposeTimer();
            //   return;
            //}
            //else
            //   ViewState["From"] = Convert.ToInt32(ViewState["From"]) + 10;

            //for (int i = 0; i < Convert.ToInt32(ViewState["From"]) && i < sn.History.DsHistoryInfo.Tables[0].Rows.Count; i++)
            //   dt.ImportRow(sn.History.DsHistoryInfo.Tables[0].Rows[i]);

            //sn.MapSolute.LoadHistoryAnimated(sn, dt, "History");

            DataTable dt = sn.History.DsHistoryInfo.Tables[0].Clone();

            if (ViewState["dateTo"] == null)
            {
                ViewState["dateTo"] = Convert.ToDateTime(sn.History.DsHistoryInfo.Tables[0].Rows[0]["OriginDateTime"].ToString());
                ViewState["dateFrom"] = Convert.ToDateTime(sn.History.DsHistoryInfo.Tables[0].Rows[sn.History.DsHistoryInfo.Tables[0].Rows.Count - 1]["OriginDateTime"].ToString());
            }

            string sSelect = "";

            ViewState["dateFrom"] = Convert.ToDateTime(ViewState["dateFrom"]).AddMinutes(sn.History.MapAnimationHistoryInterval);
            DataRow[] drArr;

            if (Convert.ToDateTime(ViewState["dateFrom"]) < Convert.ToDateTime(ViewState["dateTo"]))
            {
                sSelect = "OriginDateTime > '" + Convert.ToDateTime(ViewState["dateFrom"]).AddMinutes(-sn.History.MapAnimationHistoryInterval).ToString() + "' and  OriginDateTime <'" + Convert.ToDateTime(ViewState["dateFrom"]).ToString() + "'";
                drArr = sn.History.DsHistoryInfo.Tables[0].Select(sSelect);
            }
            else
            {
                DisposeTimer();
                sSelect = "OriginDateTime > '" + Convert.ToDateTime(ViewState["dateFrom"]).AddMinutes(-sn.History.MapAnimationHistoryInterval) + "'";
                drArr = sn.History.DsHistoryInfo.Tables[0].Select(sSelect);
            }

            foreach (DataRow dr in drArr)
                dt.ImportRow(dr);
            //sn.MapSolute.LoadHistoryAnimated(sn, dt, "History");
        }

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
                dsHistory.ReadXmlSchema(Server.MapPath("../Datasets/dstTripSummaryPerVehicle.xsd"));
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

                dgStops.LayoutSettings.ClientVisible = false;
                dgHistoryDetails.LayoutSettings.ClientVisible = false;
                dgTrips.LayoutSettings.ClientVisible = true;
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
                dsHistory.ReadXmlSchema(Server.MapPath("../Datasets/dstTripSummaryPerVehicle.xsd"));
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

                dgStops.LayoutSettings.ClientVisible = false;
                dgHistoryDetails.LayoutSettings.ClientVisible = false;
                dgTrips.LayoutSettings.ClientVisible = true;
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

        protected void dgTrips_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
            && (sn.History.ShowTrips))
            {
                e.DataSource = sn.History.DsHistoryInfo;
            }
        }

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
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Hist.xsd";
                string strPath = Server.MapPath("../Datasets/Hist.xsd");

                if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), false))
                    if (objUtil.ErrCheck(dbh.GetVehicleStatusHistoryByVehicleIdByLangExtended_NewTZ(sn.UserID, sn.SecId, VehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), true, true, true, true, sn.History.DclId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, sn.History.MsgList, sn.History.SqlTopMsg, ref xml, ref RequestOverflowed), true))
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
                    //sn.History.DsHistoryInfo.Tables["TripDetails"].Select("OriginDateTime >= '" + dr["DepartureTime"] + "' and OriginDateTime<='" + dr["ArrivalTime"] + "'")[0]["TripId"] = dr["TripId"]; 
                    DataRow[] drArr = sn.History.DsHistoryInfo.Tables["TripDetails"].Select("OriginDateTime >= '" + dr["DepartureTime"] + "' and OriginDateTime<='" + dr["ArrivalTime"] + "'");

                    for (int i = 0; i <= drArr.GetUpperBound(0); i++)
                        drArr[i]["TripId"] = dr["TripId"];
                }
                //DataTable dtTripDetails = new DataTable("TripDetails");
                //DataColumn dcTripID = new DataColumn();
                //dcTripID.DataType = Type.GetType("System.Int32");
                //dcTripID.AllowDBNull = true;
                //dcTripID.Caption = "TripId";
                //dcTripID.ColumnName = "TripId";
                //dtTripDetails.Columns.Add(dcTripID);
                //sn.History.DsHistoryInfo.Tables.Add(dtTripDetails);

                //DataRow dr = sn.History.DsHistoryInfo.Tables["TripDetails"].NewRow();
                //dr["TripId"] = 1;
                //sn.History.DsHistoryInfo.Tables["TripDetails"].Rows.Add(dr);

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
        // Changes for TimeZone Feature end

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
                //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
                //string strPath = MapPath(DataSetPath) + @"\Hist.xsd";
                string strPath = Server.MapPath("../Datasets/Hist.xsd");

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
                    //sn.History.DsHistoryInfo.Tables["TripDetails"].Select("OriginDateTime >= '" + dr["DepartureTime"] + "' and OriginDateTime<='" + dr["ArrivalTime"] + "'")[0]["TripId"] = dr["TripId"]; 
                    DataRow[] drArr = sn.History.DsHistoryInfo.Tables["TripDetails"].Select("OriginDateTime >= '" + dr["DepartureTime"] + "' and OriginDateTime<='" + dr["ArrivalTime"] + "'");

                    for (int i = 0; i <= drArr.GetUpperBound(0); i++)
                        drArr[i]["TripId"] = dr["TripId"];
                }
                //DataTable dtTripDetails = new DataTable("TripDetails");
                //DataColumn dcTripID = new DataColumn();
                //dcTripID.DataType = Type.GetType("System.Int32");
                //dcTripID.AllowDBNull = true;
                //dcTripID.Caption = "TripId";
                //dcTripID.ColumnName = "TripId";
                //dtTripDetails.Columns.Add(dcTripID);
                //sn.History.DsHistoryInfo.Tables.Add(dtTripDetails);

                //DataRow dr = sn.History.DsHistoryInfo.Tables["TripDetails"].NewRow();
                //dr["TripId"] = 1;
                //sn.History.DsHistoryInfo.Tables["TripDetails"].Rows.Add(dr);

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
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.SeatBelt)) ||
                    (Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling)))
                    rowItem["Speed"] = VLF.CLS.Def.Const.defNA.ToString();
                i++;

                if ((Convert.ToInt16(rowItem["BoxMsgInTypeId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.MessageType.Idling))
                      && (rowItem["MsgDetails"].ToString().TrimEnd() == "Duration: 00:00:00"))
                {
                    rowItem["MsgDetails"] = (string)base.GetLocalResourceObject("Text_StartIdling");
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

                    case VLF.CLS.Def.Const.noValidAddress:
                        rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
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

        protected void dgTrips_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
        {
            try
            {
                //string ttt="";
                //if (e.Row.ChildrenLoaded)
                //    ttt="1";

                //ISNet.WebUI.WebGrid.WebGridCellCollection cells = e.Row.Cells;

                //DataRow[] drCollections = null;

                //if (sn.History.DsHistoryInfo != null && sn.History.DsHistoryInfo.Tables.Count > 0)
                //{
                //    drCollections = sn.History.DsHistoryInfo.Tables[1].Select("dgKey like '" + e.Row.KeyValue + "' and chkBoxShow=true");
                //    //if (drCollections!=null && drCollections.Length>0)
                //        //e.Row.ExpandChildRow(); 

                //    //foreach (DataRow rowItem in drCollections)
                //    //{
                //    //    e.Row.Children[0].Checked = Convert.ToBoolean(rowItem["chkBoxShow"]);
                //    //}
                //}

                 //DataRowView dv;
                 //if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.GroupHeader)
                 //{
                 //    dv = (DataRowView)e.Row.Children[0].DataRow;

                 //}
            }
            catch { }
        }
    }
}
